using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApi.Data.Repositories.Base;
using CoreApi.Models;
using CoreApi.Security;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Data.Repositories
{
    public interface IRoleRepository : IRepository<Role, string>
    {
        /// <summary>
        /// Get role user joined
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Role> GetRoleOfUserAsync(string userId);

        Task<string> GetRoleIdByUserIdAsync(string userId);

        Task<IList<Claim>> GetClaimsByRoleIdAsync(string roleId);

        Task<string> GetPermissionByRoleIdAsync(string roleId);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _db;

        public RoleRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        #region Base Functions

        public async Task<IList<Role>> GetAllAsync()
        {
            return await _db.Roles.ToListAsync();
        }

        public async Task<Role> FindByIdAsync(string entityId)
        {
            return await _db.Roles.FirstOrDefaultAsync(x => x.Id.Equals(entityId));
        }

        public async Task<Role> FindOneAsync(Expression<Func<Role, bool>> predicate)
        {
            return await _db.Roles.FirstOrDefaultAsync(predicate);
        }

        public async Task<IList<Role>> FindAsync(Expression<Func<Role, bool>> predicate, int pageSize, int currentPage)
        {
            return await _db.Roles
                .Where(predicate)

                .ToListAsync();
        }

        public IQueryable<Role> Queryable()
        {
            return _db.Roles;
        }

        public IQueryable<Role> Queryable(Expression<Func<Role, bool>> predicate)
        {
            return _db.Roles.Where(predicate);
        }

        public IQueryable<Role> Queryable(int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize == 0)
                return _db.Roles;

            return _db.Roles
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public IQueryable<Role> Queryable(Expression<Func<Role, bool>> predicate, int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize == 0)
                return Queryable()
                    .Where(predicate);

            return Queryable(predicate).Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task<bool> Exists(Role entity)
        {
            var itemCheck = await FindByIdAsync(entity.Id);
            if (itemCheck != null) return true;
            return false;
        }

        public async Task<Role> InsertAsync(Role entity)
        {
            _db.Roles.Add(entity);
            return await _db.SaveChangesAsync() > 0 ? entity : null;
        }

        public async Task<bool> InsertAsync(IList<Role> entityList)
        {
            await _db.Roles.AddRangeAsync(entityList);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<Role> UpdateAsync(Role entity, bool checkIsExists = true)
        {
            if (checkIsExists)
                if (!await Exists(entity)) throw new Exception("This entity not found, so you cant update.");

            // Passed
            _db.Roles.Update(entity);

            return await _db.SaveChangesAsync() > 0 ? entity : null;
        }

        public async Task<bool> DeleteAsync(Role entity, bool checkIsExists = true)
        {
            if (checkIsExists)
                if (!await Exists(entity)) throw new Exception("This entity not found, so you cant deleted.");

            // Passed
            _db.Roles.Remove(entity);

            return await _db.SaveChangesAsync() > 0;
        }

        #endregion


        public async Task<Role> GetRoleOfUserAsync(string userId)
        {
            var roleId = (await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId.Equals(userId)))?.RoleId;
            if (!string.IsNullOrEmpty(roleId))
                return await FindByIdAsync(roleId);
            return null;
        }

        public async Task<string> GetRoleIdByUserIdAsync(string userId)
        {
            var data = await _db.ExecuteQueryAndReturnSingleAsync(
                "Select top 1 [RoleId] from [dbo].[UserRoles] where [UserId] = @userId", new Dictionary<string, object>()
                {
                    {"userId", userId.Trim()},
                },
                record => record.GetString(0));

            return !string.IsNullOrEmpty(data) ? data.Trim() : string.Empty;
        }

        public async Task<IList<Claim>> GetClaimsByRoleIdAsync(string roleId)
        {
            var claims = await _db.RoleClaims.Where(x => x.RoleId.Equals(roleId.Trim())).ToListAsync();
            if (claims?.Count > 0)
            {
                return claims.Select(identityUserClaim => new Claim(identityUserClaim.ClaimType, identityUserClaim.ClaimValue)).ToList();
            }

            return null;
        }

        public async Task<string> GetPermissionByRoleIdAsync(string roleId)
        {
            var data = await _db.ExecuteQueryAndReturnSingleAsync(
                "Select [ClaimValue] from [dbo].[RoleClaims] where [RoleId] = @RoleId and [ClaimType] = @ClaimType", new Dictionary<string, object>()
                {
                    {"RoleId", roleId.Trim()},
                    {"ClaimType", ClaimContants.Permission},
                },
                record => record.GetString(0));

            return !string.IsNullOrEmpty(data) ? data.Trim() : string.Empty;
        }
    }
}
