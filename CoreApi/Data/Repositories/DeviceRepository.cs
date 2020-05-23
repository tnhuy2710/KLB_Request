using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Data.Repositories.Base;
using CoreApi.Extensions;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Data.Repositories
{
    public interface IDeviceRepository : IRepository<Device, string>
    {
        Task<bool> RegisterNewUserDeviceAsync(UserDevice entity);

        Task<bool> UpdateUserDeviceAsync(UserDevice entity);

        Task<bool> CleanAllSessionsAsync();

        Task<bool> CleanSessionsByDeviceIdAsync(string deviceId);

        Task<bool> CleanSessionsByUserIdAsync(string userId);

        Task<bool> CleanSessionByUserDeviceAsync(string userId, string deviceId);
    }

    public class DeviceRepository : BaseRepository<Device, string>, IDeviceRepository
    {
        public DeviceRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        { }


        public async Task<bool> RegisterNewUserDeviceAsync(UserDevice entity)
        {
            Db.UserDevices.Add(entity);
            return await Db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserDeviceAsync(UserDevice entity)
        {
            Db.UserDevices.Update(entity);
            return await Db.SaveChangesAsync() > 0;
        }

        public async Task<bool> CleanAllSessionsAsync()
        {
            return await Db.Database.ExecuteSqlCommandAsync("delete from UserDevices") > 0;
        }

        public async Task<bool> CleanSessionsByDeviceIdAsync(string deviceId)
        {
            var items = Db.UserDevices.Where(x => x.DeviceId.Equals(deviceId.MakeLowerCase()));
            Db.UserDevices.RemoveRange(items);
            return await Db.SaveChangesAsync() > 0;
        }

        public async Task<bool> CleanSessionsByUserIdAsync(string userId)
        {
            var items = Db.UserDevices.Where(x => x.UserId.Equals(userId.MakeLowerCase()));
            Db.UserDevices.RemoveRange(items);
            return await Db.SaveChangesAsync() > 0;
        }

        public async Task<bool> CleanSessionByUserDeviceAsync(string userId, string deviceId)
        {
            var items = Db.UserDevices.Where(x => x.UserId.Equals(userId.MakeLowerCase()) && x.DeviceId.Equals(deviceId.MakeLowerCase()));
            Db.UserDevices.RemoveRange(items);
            return await Db.SaveChangesAsync() > 0;
        }
    }
}
