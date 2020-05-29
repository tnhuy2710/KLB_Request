using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Enums;
using CoreApi.Models;
using Microsoft.AspNetCore.Identity;

namespace CoreApi.Extensions
{
    public static class RoleManagerExtensions
    {
        /// <summary>
        /// Add role to database and check it if exists.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="userRole"></param>
        /// <returns></returns>
        public static async Task AddRoleAsync(this RoleManager<Role> manager, UserRole userRole)
        {
            var r = await manager.RoleExistsAsync(userRole.ToString());
            if (!r)
            {
                await manager.CreateAsync(new Role(userRole.ToString(), (int)userRole));
            }

        }
    }
}
