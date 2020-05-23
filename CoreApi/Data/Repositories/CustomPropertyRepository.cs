using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Commons;
using CoreApi.Data.Repositories.Base;
using CoreApi.Extensions;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Data.Repositories
{
    public interface ICustomPropertyRepository : IRepository<CustomProperty, long>
    {
        Task<bool> SetCustomUserGroupIdAsync(string userId, string customGroupId);
        Task<bool> SetCustomUserGroupIdByUserNameAsync(string username, string customGroupId);

        Task<string> GetCustomUserGroupIdAsync(string userId);
        Task<string> GetCustomUserGroupIdByUserNameAsync(string username);

        Task<bool> SetCustomFormPropertyAsync(string formId, string key, string value);
        Task<string> GetCustomFormPropertyAsync(string formId, string key);

        Task<string> GetCustomUserPropertyAsync(string userName, string key);
        Task<IList<CustomProperty>> GetCustomUserPropertiesAsync(string userName);
    }

    public class CustomPropertyRepository : BaseRepository<CustomProperty, long>, ICustomPropertyRepository
    {
        public CustomPropertyRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        {
        }


        public async Task<bool> SetCustomUserGroupIdAsync(string userId, string customGroupId)
        {
            // Check exists
            var entity = await FindOneAsync(x =>
                x.TargetType.Equals(AppContants.UserCustomProperty) &&
                x.TargetValue.Equals(userId) &&
                x.Key.Equals("GroupId")
            );

            if (entity != null)
            {
                entity.Value = customGroupId;

                //
                return await UpdateAsync(entity, false) != null;
            }

            // Insert
            entity = new CustomProperty()
            {
                TargetType = AppContants.UserCustomProperty,
                TargetValue = userId,
                Key = "GroupId",
                Value = customGroupId
            };

            return await InsertAsync(entity) != null;
        }

        public async Task<bool> SetCustomUserGroupIdByUserNameAsync(string username, string customGroupId)
        {
            // Check exists
            var entity = await FindOneAsync(x =>
                x.TargetType.Equals(AppContants.UsernameCustomProperty) &&
                x.TargetValue.Equals(username) &&
                x.Key.Equals("GroupId")
            );

            if (entity != null)
            {
                entity.Value = customGroupId;

                //
                return await UpdateAsync(entity, false) != null;
            }

            // Insert
            entity = new CustomProperty()
            {
                TargetType = AppContants.UsernameCustomProperty,
                TargetValue = username,
                Key = "GroupId",
                Value = customGroupId
            };

            return await InsertAsync(entity) != null;
        }

        public async Task<string> GetCustomUserGroupIdAsync(string userId)
        {
            var result = await FindOneAsync(x =>
                x.TargetType.Equals(AppContants.UserCustomProperty) &&
                x.TargetValue.Equals(userId) &&
                x.Key.Equals("GroupId")
            );

            return result?.Value;
        }

        public async Task<string> GetCustomUserGroupIdByUserNameAsync(string username)
        {
            var entity = await FindOneAsync(x =>
                x.TargetType.Equals(AppContants.UsernameCustomProperty) &&
                x.TargetValue.Equals(username) &&
                x.Key.Equals("GroupId")
            );

            return entity?.Value;
        }

        public async Task<bool> SetCustomFormPropertyAsync(string formId, string key, string value)
        {
            // Check exists
            var entity = await FindOneAsync(x =>
                x.TargetType.Equals(AppContants.FormCustomProperty) &&
                x.TargetValue.Equals(formId) &&
                x.Key.Equals(key.MakeLowerCase())
            );

            if (entity != null)
            {
                entity.Value = value;

                //
                return await UpdateAsync(entity, false) != null;
            }

            // Insert
            entity = new CustomProperty()
            {
                TargetType = AppContants.FormCustomProperty,
                TargetValue = formId,
                Key = key.MakeLowerCase(),
                Value = value
            };

            return await InsertAsync(entity) != null;
        }

        public async Task<string> GetCustomFormPropertyAsync(string formId, string key)
        {
            var entity = await FindOneAsync(x =>
                x.TargetType.Equals(AppContants.FormCustomProperty) &&
                x.TargetValue.Equals(formId) &&
                x.Key.Equals(key.MakeLowerCase())
            );

            return entity?.Value;
        }

        public async Task<string> GetCustomUserPropertyAsync(string userName, string key)
        {
            var entity = await FindOneAsync(x =>
                x.TargetType.Equals(AppContants.UsernameCustomProperty) &&
                x.TargetValue.Equals(userName) &&
                x.Key.Equals(key)
            );

            return entity?.Value;
        }

        public async Task<IList<CustomProperty>> GetCustomUserPropertiesAsync(string userName)
        {
            return await Db.CustomProperties.Where(x =>
                    x.TargetType.Equals(AppContants.UsernameCustomProperty) && 
                    x.TargetValue.Equals(userName))
                .ToListAsync();
        }
    }
}
