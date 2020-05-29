using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Extensions;
using CoreApi.Models;
using KlbService;
using Microsoft.Extensions.Configuration;

namespace CoreApi.Services
{
    public interface IKlbService
    {
        Task<bool> AuthenticationAsync(string username, string password);

        Task<Employee> FindByEmailAsync(string email);

        Task<Employee> FindByEmpCodeAsync(string empCode);

        Task<string> GetEmailByPortalIdAsync(string userName);

        Task<string> GetUserPortalIdByEmailAsync(string email);

        Task<byte[]> GetEmployeeImageBytesByFileNameAsync(string fileName);

        Task<string> GetEmployeeImageBase64ByFileNameAsync(string fileName);

        Task<IDictionary<string, string>> GetUserPortalInfoByEmailAsync(string email);
    }

    public class KlbService : IKlbService
    {
        private readonly EmployeeService _employeeService;
        private readonly PortalAuthService _portalAuthService;

        private readonly IConfiguration _configuration;

        public KlbService(IConfiguration configuration)
        {
            _configuration = configuration;

            _employeeService = new EmployeeService(_configuration.GetValue<string>("WebServiceUrls:KlbEmployeeWebService"));
            _portalAuthService = new PortalAuthService(_configuration.GetValue<string>("WebServiceUrls:PortalAuthenticationWebService"));
        }


        // KlbEmployee methods

        private async Task<IList<Employee>> CallQueryAndReturnListAsync(string sqlQuery)
        {
            var itemsRemote = await _employeeService.GetEmployeeByWhereAsync(sqlQuery);
            return await MapToListEmployees(itemsRemote);
        }

        private async Task<IList<Employee>> MapToListEmployees(DataSet source)
        {
            if (source?.Tables.Count == 1)
            {
                var listItems = new List<Employee>();

                foreach (DataRow row in source.Tables[0].Rows)
                {
                    listItems.Add(MapToEmployee(row));
                }

                return listItems;
            }

            return null;
        }

        private Employee MapToEmployee(DataRow row)
        {
            var entity = new Employee()
            {
                EmpId = row.TryGetStringValue("EmpID"),
                EmpCode = row.TryGetStringValue("EmpCode"),
                FullName = row.TryGetStringValue("FullName"),
                Email = row.TryGetStringValue("Email"),
                BirthDay = row.TryGetStringValue("BirthDay"),
                Gender = row.TryGetStringValue("Gender"),
                FilePhoto = row.TryGetStringValue("FilePhoto"),
                PhoneNumber = row.TryGetStringValue("mobifone"),
                Title = row.TryGetStringValue("Title"),
                Level1Id = row.TryGetStringValue("Level1ID"),
                Level1Name = row.TryGetStringValue("Level1name"),
                Level2Id = row.TryGetStringValue("Level2ID"),
                Level2Name = row.TryGetStringValue("Level2Name"),
                GroupId = row.TryGetStringValue("LSPosGroupID")
            };

            return entity;
        }

        public async Task<bool> AuthenticationAsync(string username, string password)
        {
            if (!StringExtensions.IsEmptyOrNull(username, password))
            {
                var result = await _portalAuthService.Authentication(username.Trim(), password.Trim());
                if (!string.IsNullOrEmpty(result))
                    return true;
            }

            return false;
        }

        public async Task<Employee> FindByEmailAsync(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return
                    (await CallQueryAndReturnListAsync($"ltrim(rtrim(email)) = '{email.MakeLowerCase()}'"))
                    .FirstOrDefault();
            }

            return null;
        }

        public async Task<Employee> FindByEmpCodeAsync(string empCode)
        {
            return (await CallQueryAndReturnListAsync($"ltrim(rtrim(EmpCode)) = '{empCode.MakeLowerCase()}'"))?.FirstOrDefault();
        }

        public async Task<byte[]> GetEmployeeImageBytesByFileNameAsync(string fileName)
        {
            return await _employeeService.GetEmployeeImageBytesByFileNameAsync(fileName);
        }

        public async Task<string> GetEmployeeImageBase64ByFileNameAsync(string fileName)
        {
            return await _employeeService.GetEmployeeBase64ByFileNameAsync(fileName);
        }

        // Portal methods

        public async Task<IDictionary<string, string>> GetUserPortalInfoByEmailAsync(string email)
        {
            var result = await _portalAuthService.GetInfoPortalByEmailAsync(email.MakeLowerCase());
            if (result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                return new Dictionary<string, string>()
                {
                    {"USERID"    , result?.Tables[0].Rows[0]["USERID"].ToString().MakeLowerCase()},
                    {"USERNAME"  , result?.Tables[0].Rows[0]["USERNAME"].ToString().MakeLowerCase()},
                    {"CELLPHONE" , result?.Tables[0].Rows[0]["CELLPHONE"].ToString().MakeLowerCase()},
                    {"BRANCHID"  , result?.Tables[0].Rows[0]["BRANCHID"].ToString().MakeLowerCase()},
                    {"BRANCHNAME", result?.Tables[0].Rows[0]["BRANCHNAME"].ToString().MakeLowerCase()},
                    {"EMAIL"     , result?.Tables[0].Rows[0]["EMAIL"].ToString().MakeLowerCase()},
                    {"ISACTIVE"  , result?.Tables[0].Rows[0]["ISACTIVE"].ToString().MakeLowerCase()},
                };
            }

            return null;
        }

        public async Task<string> GetEmailByPortalIdAsync(string userName)
        {
            return await _portalAuthService.GetInfoPortalByUserNameAsync(userName);
        }

        public async Task<string> GetUserPortalIdByEmailAsync(string email)
        {
            var result = await _portalAuthService.GetInfoPortalByEmailAsync(email.MakeLowerCase());
            if (result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                return result?.Tables[0].Rows[0]["USERNAME"].ToString().MakeLowerCase();

            return string.Empty;
        }
    }
}
