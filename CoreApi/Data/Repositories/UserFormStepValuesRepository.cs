using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Data.Repositories.Base;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;
using CoreApi.Extensions;
using Microsoft.EntityFrameworkCore;


namespace CoreApi.Data.Repositories
{
    public interface IUserFormStepValuesRepository : IRepository<UserFormStepValues, string>
    {
        Task<bool> InsertOrUpdateRowAsync(long userFormId, string stepId, string value);

        Task<UserFormStepValues> GetUserFormStepDataAsync(long userFormId, string stepId);
        Task<IList<UserFormStepValues>> GetUserFormStepDataAsync(long userFormId);
    }

    public class UserFormStepValuesRepository : BaseRepository<UserFormStepValues, string>, IUserFormStepValuesRepository
    {
        public UserFormStepValuesRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        {
        }
        public async Task<bool> InsertOrUpdateRowAsync(long userFormId, string stepId, string value)
        {
            bool isInsert = false;
            var entity = await GetUserFormStepDataAsync(userFormId, stepId);

            if (entity == null)
            {
                isInsert = true;
                entity = new UserFormStepValues()
                {
                    UserFormId = userFormId,
                    StepId = stepId.MakeLowerCase(),
                };
            }

            entity.FormValue = value;
            entity.DateCreated = DateTimeOffset.UtcNow;

            if (isInsert)
                return await InsertAsync(entity) != null;
            return await UpdateAsync(entity, false) != null;
            
        }

        public async Task<UserFormStepValues> GetUserFormStepDataAsync(long userFormId, string stepId)
        {
            return await Db.UserFormStepValues
                .Where(x => x.UserFormId.Equals(userFormId) && x.StepId.Equals(stepId.MakeLowerCase()))
                .Include(x => x.UserForm)
                .Include(x => x.FormStep)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<UserFormStepValues>> GetUserFormStepDataAsync(long userFormId)
        {
            return await Db.UserFormStepValues
                .Where(x => x.UserFormId.Equals(userFormId))
                .Include(x => x.UserForm)
                .Include(x => x.FormStep)
                .ToListAsync();
        }
    }
}