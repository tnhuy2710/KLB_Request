using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApi.Data.Repositories.Base;
using CoreApi.Extensions;
using CoreApi.Models;
using CoreApi.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Data.Repositories
{
    public interface IUserRepository : IRepository<User, string>
    {
        Task<User> FindByUserNameAsync(string userName);

        Task<User> FindByEmailAsync(string email);

        Task<User> FindByEmpCodeAsync(string empCode);

        Task<User> FindByPhoneNumberAsync(string phoneNumber);

        Task<bool> ResetAccessFailedCountAsync(string userId);

        Task<IList<Claim>> GetClaimsByUserIdAsync(string userId);

        Task<string> GetPermissionsByUserIdAsync(string userId);

        Task<UserEmployeeDetails> FindDetailsByIdAsync(string userId);
        Task<UserEmployeeDetails> FindDetailsByEmpCodeAsync(string userId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IRoleRepository _roleRepository;
        private readonly ICustomPropertyRepository _customProperty;

        public UserRepository(ApplicationDbContext db, UserManager<User> userManager, IRoleRepository roleRepository, ICustomPropertyRepository customProperty)
        {
            _db = db;
            _userManager = userManager;
            _roleRepository = roleRepository;
            _customProperty = customProperty;
        }

        #region Base Functions

        public async Task<IList<User>> GetAllAsync()
        {
            return await _db.Users
                .Include(x => x.UserDevices)
                .Include(x => x.UserGroups)
                .ToListAsync();
        }

        public async Task<User> FindByIdAsync(string entityId)
        {
            var item = await FindOneAsync(x => x.Id.Equals(entityId));
            if (item != null)
            {
                await CheckUserCustomPropertiesAsync(item);
                return item;
            }

            return null;
        }

        public async Task<User> FindOneAsync(Expression<Func<User, bool>> predicate)
        {
            var item = await _db.Users
                .Include(x => x.UserDevices)
                .ThenInclude(y => y.Device)
                .Include(x => x.UserGroups)
                .ThenInclude(y => y.Group)
                .FirstOrDefaultAsync(predicate);

            if (item != null)
            {
                await CheckUserCustomPropertiesAsync(item);
                return item;
            }

            return null;
        }

        public async Task<IList<User>> FindAsync(Expression<Func<User, bool>> predicate, int pageSize, int currentPage)
        {
            return await _db.Users
                .Where(predicate)
                .ToListAsync();
        }

        public IQueryable<User> Queryable()
        {
            return _db.Users;
        }

        public IQueryable<User> Queryable(Expression<Func<User, bool>> predicate)
        {
            return _db.Users.Where(predicate);
        }

        public IQueryable<User> Queryable(int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize == 0)
                return _db.Users;

            return _db.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IQueryable<User> Queryable(Expression<Func<User, bool>> predicate, int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize == 0)
                return Queryable()
                    .Where(predicate);

            return Queryable(predicate).Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task<bool> Exists(User entity)
        {
            var itemCheck = await FindByIdAsync(entity.Id);
            if (itemCheck != null) return true;
            return false;
        }

        public async Task<User> InsertAsync(User entity)
        {
            _db.Users.Add(entity);
            return await _db.SaveChangesAsync() > 0 ? entity : null;
        }

        public async Task<bool> InsertAsync(IList<User> entityList)
        {
            await _db.Users.AddRangeAsync(entityList);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<User> UpdateAsync(User entity, bool checkIsExists = true)
        {
            if (checkIsExists)
                if (!await Exists(entity)) throw new Exception("This entity not found, so you cant update.");

            // Passed
            _db.Users.Update(entity);

            return await _db.SaveChangesAsync() > 0 ? entity : null;
        }

        public async Task<bool> DeleteAsync(User entity, bool checkIsExists = true)
        {
            if (checkIsExists)
                if (!await Exists(entity)) throw new Exception("This entity not found, so you cant deleted.");

            // Passed
            _db.Users.Remove(entity);

            return await _db.SaveChangesAsync() > 0;
        }

        #endregion

        public async Task<User> FindByUserNameAsync(string userName)
        {
            return await FindOneAsync(x => x.NormalizedUserName.Equals(userName.MakeUpperCase()));
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await FindOneAsync(x => x.NormalizedEmail.Equals(email.MakeUpperCase()));
        }

        public async Task<User> FindByEmpCodeAsync(string empCode)
        {
            return await FindOneAsync(x => x.EmpCode.Equals(empCode));
        }

        public async Task<User> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await FindOneAsync(x => x.PhoneNumber.Equals(phoneNumber.Trim()));
        }

        public async Task<bool> ResetAccessFailedCountAsync(string userId)
        {
            return await _db.ExecuteNonQueryAsync(
                "Update [dbo].[Users] set [AccessFailedCount] = @AccessFailedCount,[DateUpdated] = @DateUpdated where [Id] = @Id",
                new Dictionary<string, object>()
                {
                    {"AccessFailedCount", 0},
                    {"DateUpdated", DateTimeOffset.UtcNow},
                    {"Id", userId.Trim()},
                }) > 0;
        }

        public async Task<IList<Claim>> GetClaimsByUserIdAsync(string userId)
        {
            var claims = await _db.UserClaims.Where(x => x.UserId.Equals(userId.Trim())).ToListAsync();
            if (claims?.Count > 0)
            {
                return claims.Select(identityUserClaim => new Claim(identityUserClaim.ClaimType, identityUserClaim.ClaimValue)).ToList();
            }

            return null;
        }

        public async Task<string> GetPermissionsByUserIdAsync(string userId)
        {
            var data = await _db.ExecuteQueryAndReturnSingleAsync(
                "Select [ClaimValue] from [dbo].[UserClaims] where [UserId] = @UserId and [ClaimType] = @ClaimType", new Dictionary<string, object>()
                {
                    {"UserId", userId.Trim()},
                    {"ClaimType", ClaimContants.Permission},
                },
                record => record.GetString(0));

            return !string.IsNullOrEmpty(data) ? data.Trim() : string.Empty;
        }

        public async Task<UserEmployeeDetails> FindDetailsByIdAsync(string userId)
        {
            var item = await _db.ExecuteQueryAndReturnSingleAsync("prGetUserDetailsById",
                new Dictionary<string, object>()
                {
                    {"@pId", userId}
                }, MapToUserEmployeeDetails
                , CommandType.StoredProcedure);

            return item;
        }

        public async Task<UserEmployeeDetails> FindDetailsByEmpCodeAsync(string userId)
        {            
            var item = await _db.ExecuteQueryAndReturnSingleAsync("prGetUserDetailsByEmpCode",
                new Dictionary<string, object>()
                {
                    {"@pEmpCode", userId}
                }, MapToUserEmployeeDetails
                , CommandType.StoredProcedure);

            return item;
        }


        private async Task CheckUserCustomPropertiesAsync(User user)
        {
            var properties = await _customProperty.GetCustomUserPropertiesAsync(user.UserName);
            if (properties?.Count > 0)
            {
                var userProperties = typeof(User).GetProperties(BindingFlags.GetProperty);
                foreach (var customProperty in properties)
                {
                    var property = userProperties.FirstOrDefault(x => x.Name.Equals(customProperty.Key));
                    if (property != null)
                    {
                        property.SetValue(user, customProperty.Value, null);
                    }
                }
            }
        }

        private UserEmployeeDetails MapToUserEmployeeDetails(IDataRecord row)
        {
            //var index = 0;
            var entity               = new UserEmployeeDetails()
            {
                Id                   = row.GetValueOrDefault<string>("Id"),
                Username             = row.GetValueOrDefault<string>("UserName"),
                NormalizedUserName   = row.GetValueOrDefault<string>("NormalizedUserName"),
                Email                = row.GetValueOrDefault<string>("Email"),
                NormalizedEmail      = row.GetValueOrDefault<string>("NormalizedEmail"),
                EmailConfirmed       = row.GetValueOrDefault<bool>("EmailConfirmed"),
                PhoneNumber          = row.GetValueOrDefault<string>("PhoneNumber"),
                AccessFailedCount    = row.GetValueOrDefault<int>("AccessFailedCount"),
                LockoutEnabled       = row.GetValueOrDefault<bool>("LockoutEnabled"),
                LockoutStart         = row.GetValueOrDefault<DateTimeOffset?>("LockoutStart"),
                LockoutEnd           = row.GetValueOrDefault<DateTimeOffset?>("LockoutEnd"),
                DateCreated          = row.GetValueOrDefault<DateTimeOffset>("DateCreated"),
                DateUpdated          = row.GetValueOrDefault<DateTimeOffset>("DateUpdated"),
                EmpCode              = row.GetValueOrDefault<string>("EmpCode"),
                EMPID                = row.GetValueOrDefault<string>("EMPID"),
                FULLNAME             = row.GetValueOrDefault<string>("FULLNAME"),
                LEVEL1ID             = row.GetValueOrDefault<string>("LEVEL1ID"),
                LEVEL1NAME           = row.GetValueOrDefault<string>("LEVEL1NAME"),
                LEVEL2ID             = row.GetValueOrDefault<string>("LEVEL2ID"),
                LEVEL2NAME           = row.GetValueOrDefault<string>("LEVEL2NAME"),
                LEVEL3ID             = row.GetValueOrDefault<string>("LEVEL3ID"),
                LEVEL3NAME           = row.GetValueOrDefault<string>("LEVEL3NAME"),
                ISMANAGER            = row.GetValueOrDefault<decimal>("ISMANAGER"),
                LSJOBGROUPID         = row.GetValueOrDefault<string>("LSJOBGROUPID"),
                LSPOSITIONCODE       = row.GetValueOrDefault<string>("LSPOSITIONCODE"),
                POSITIONID           = row.GetValueOrDefault<string>("POSITIONID"),
                TITLE                = row.GetValueOrDefault<string>("TITLE"),
                WORKINGDATE          = row.GetValueOrDefault<string>("StartWorkingDate"),
                DIRECTEMPID          = row.GetValueOrDefault<string>("DIRECTEMPID"),
                DIRECTEMPMANAGER     = row.GetValueOrDefault<string>("DIRECTEMPMANAGER"),
                THUONGTRU            = row.GetValueOrDefault<string>("THUONGTRU"),
                TAMTRU               = row.GetValueOrDefault<string>("TAMTRU"),
                BIRTHDAY             = row.GetValueOrDefault<string>("BIRTHDAY"),
                GENDER               = row.GetValueOrDefault<string>("GENDER"),
                PIN                  = row.GetValueOrDefault<string>("PIN"),
                PINDATE              = row.GetValueOrDefault<string>("PINDATE"),
                PINPLACE             = row.GetValueOrDefault<string>("PINPLACE"),
                LSPOSGROUPID         = row.GetValueOrDefault<string>("LSPOSGROUPID"),
                LSPOSGROUPCODEID     = row.GetValueOrDefault<string>("LSPOSGROUPCODEID"),
                POSITION_DATE        = row.GetValueOrDefault<DateTime>("POSITION_DATE"),
            };

            return entity;
        }
    }
}

