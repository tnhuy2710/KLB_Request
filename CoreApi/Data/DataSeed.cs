using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreApi.Enums;
using CoreApi.Extensions;
using CoreApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApi.Data
{
    public static class DataSeedBuilder
    {
        public static async void DataSeed(this IApplicationBuilder builder)
        {
            using (var db = new ApplicationDbContext(
                builder.ApplicationServices.GetRequiredService<DbContextOptions<ApplicationDbContext>>(),
                builder.ApplicationServices.GetRequiredService<IConfiguration>()
                ))
            {
                // Make sure data is created
                await db.Database.EnsureCreatedAsync();

                // Load Service
                //var userManager = builder.ApplicationServices.GetRequiredService<UserManager<User>>();
                var roleManager = builder.ApplicationServices.GetRequiredService<RoleManager<Role>>();

                // Add default roles
                if (!db.Roles.Any())
                {
                    await roleManager.AddRoleAsync(UserRole.Administrator);
                    await roleManager.AddRoleAsync(UserRole.Developer);
                    await roleManager.AddRoleAsync(UserRole.Moderator);
                    await roleManager.AddRoleAsync(UserRole.Supporter);
                    await roleManager.AddRoleAsync(UserRole.Registered);
                    await roleManager.AddRoleAsync(UserRole.Banned);

                }

                // Create StoredProcedures
                await CreateCleanTemporaryTokenPr(db);
            }
        }


        private static async Task CreateCleanTemporaryTokenPr(ApplicationDbContext db)
        {
            try
            {
                // Clean Temperature Token StoredProcedures
                var sb = new StringBuilder();
                sb.AppendLine("IF NOT EXISTS (SELECT * FROM sysobjects  WHERE  id = object_id(N'[dbo].[prCleanTemporaryTokens]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                sb.AppendLine("BEGIN");
                sb.AppendLine("exec('");
                sb.AppendLine("CREATE PROCEDURE prCleanTemporaryTokens " +
                              "\r\nAS" +
                              "\r\nBEGIN" +
                              "\r\n\tSET NOCOUNT ON;" +
                              "\r\n\r\n    delete from TemporaryTokens" +
                              "\r\n\twhere Id in (" +
                              "\r\n\t\tselect Id from TemporaryTokens" +
                              "\r\n\t\twhere ExpireIn < cast(GETUTCDATE() as datetimeoffset)" +
                              "\r\n\t)" +
                              "\r\n\r\nEND");
                sb.AppendLine("')");
                sb.AppendLine("END");
                await db.Database.ExecuteSqlCommandAsync(sb.ToString());
            }
            catch (Exception e)
            {
                var message = "Error at DataSeed line 66 - Can't exec create CleanTemperatureToken StoredProcedures: " + e.Message;
                Debug.WriteLine(message);
            }
        }
    }
}
