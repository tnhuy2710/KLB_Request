using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Commons;
using CoreApi.Extensions;
using CoreApi.Models;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses;

namespace CoreApi.Data.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> FindByPortalIdAsync(string id);

        Task<Employee> FindByEmpCodeAsync(string id);

        Task<Employee> FindByEmpCodeAsync(string id, string KyDanhGia);

        Task<Employee> FindByEmailAsync(string email);

        Task<IList<Employee>> GetAllAsync();
        Task<IList<Employee>> GetAllByGroupIdAndLevel2IdAsync(string groupId, string level2Id);
        Task<IList<Employee>> GetAllByGroupIdAndEmpCodeAsync(string groupId, string empCode);
        Task<IList<Employee>> GetAllByGroupIdAndEmpCodeAsync(string groupId, string empCode, string KyDanhGia);
        Task<IList<Employee>> GetManagersOfEmpCodeAsync(string empCode);

        Task<IList<Employee>> GetAllByGroupIdsAndEmpCodeAsync(string groupIds, string empCode);
        Task<IList<Employee>> GetAllByGroupIdsAndEmpCodeAsync(string groupIds, string empCode, string KyDanhGia);
        Task<bool> UpdateEmployeeAvatarImageAsync(string empCode, string filename, string original, string size512, string size125);

        Task<string> GetUserID(string empCode);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly GlReportContext _glReport;
        private readonly ICustomPropertyRepository _customProperty;
        private readonly ApplicationDbContext _db;

        public EmployeeRepository(GlReportContext glReport, ICustomPropertyRepository customProperty, ApplicationDbContext db)
        {
            _glReport = glReport;
            _customProperty = customProperty;
            _db = db;
        }

        public async Task<Employee> FindByPortalIdAsync(string id)
        {
            return await _db.ExecuteQueryAsync("prGetEmployeeDetailsByUsername", new Dictionary<string, object>()
            {
                {"@pUserName", id.Trim()},
            }, MapToEmployee, CommandType.StoredProcedure);
        }

        public async Task<Employee> FindByEmpCodeAsync(string id)
        {
            return await _db.ExecuteQueryAsync("prGetEmployeeDetailsByEmpCode", new Dictionary<string, object>()
            {
                {"@pEmpCode", id.Trim()},
            }, MapToEmployee, CommandType.StoredProcedure);
        }

        public async Task<Employee> FindByEmpCodeAsync(string id, string KyDanhGia)
        {
            return await _db.ExecuteQueryAsync("prGetEmployeeDetailsByEmpCode_KyDG", 
                new Dictionary<string, object>()
            {
                 {"@pEmpCode", id.Trim() }, {"@pKyDanhGia", KyDanhGia} 
            }, MapToEmployee, CommandType.StoredProcedure);
        }

        public async Task<Employee> FindByEmailAsync(string email)
        {
            return await _db.ExecuteQueryAsync("prGetEmployeeDetailsByEmail", new Dictionary<string, object>()
            {
                {"@pEmail", email.Trim()},
            }, MapToEmployee, CommandType.StoredProcedure);
        }

        public async Task<IList<Employee>> GetAllAsync()
        {
            return await _db.ExecuteQueryAsync("prGetEmployees", null, MapToEmployees, CommandType.StoredProcedure);
        }

        public async Task<IList<Employee>> GetAllByGroupIdAndLevel2IdAsync(string groupId, string level2Id)
        {
            return await _glReport.ExecuteStoredProcedureAsync(
                "PKG_USER.get_users_by_groupid_level2id",
                new Dictionary<string, string>()
                {
                    {"v_group_id", groupId.Trim()},
                    {"v_level_2_id", level2Id.Trim()},
                },
                MapToEmployees
            );
        }

        public async Task<IList<Employee>> GetAllByGroupIdAndEmpCodeAsync(string groupId, string empCode)
        {
            return await _db.ExecuteQueryAsync("GET_USERS_BY_GROUP_EMPCODE", new Dictionary<string, object>()
            {
                {"@v_group", groupId},
                {"@v_emp_code", empCode}
            }, MapToEmployees, CommandType.StoredProcedure);
        }

        public async Task<IList<Employee>> GetAllByGroupIdAndEmpCodeAsync(string groupId, string empCode, string KyDanhGia)
        {
            return await _db.ExecuteQueryAsync("GET_USERS_BY_GROUP_EMPCODE_KYDG", new Dictionary<string, object>()
            {
                {"@v_group", groupId},
                {"@v_emp_code", empCode},
                {"@v_KyDG", KyDanhGia}
            }, MapToEmployees, CommandType.StoredProcedure);
        }

        public async Task<IList<Employee>> GetManagersOfEmpCodeAsync(string empCode)
        {
            return await GetAllByGroupIdAndEmpCodeAsync($"|{AppContants.QlttGroupCode}||", empCode);
        }

        public async Task<IList<Employee>> GetManagersOfEmpCodeAsync(string empCode, string KyDanhGia)
        {
            return await GetAllByGroupIdAndEmpCodeAsync($"|{AppContants.QlttGroupCode}||", empCode, KyDanhGia);
        }

        public async Task<IList<Employee>> GetAllByGroupIdsAndEmpCodeAsync(string groupIds, string empCode)
        {
            // Get all user in group and level2Id
            var users = new List<Employee>();
            var tasks = new List<Task>();

            foreach (var s in groupIds.TrySplit(";"))
            {
                tasks.Add(Task.Run(async () =>
                {
                    var items = await GetAllByGroupIdAndEmpCodeAsync(s, empCode);
                    if (items?.Count > 0)
                    {
                        users.AddRange(items);
                    }
                }));
            }

            // Wait all background task done
            await Task.WhenAll(tasks);

            var tempUsers = new List<Employee>();

            // Filter duplicate user
            if (users?.Count > 0)
            {
                foreach (var employee in users)
                {
                    if (!tempUsers.Exists(x => x.EmpCode.Equals(employee.EmpCode)))
                        tempUsers.Add(employee);
                }
            }

            return tempUsers;
        }

        public async Task<IList<Employee>> GetAllByGroupIdsAndEmpCodeAsync(string groupIds, string empCode, string KyDanhGia)
        {
            // Get all user in group and level2Id
            var users = new List<Employee>();
            var tasks = new List<Task>();

            foreach (var s in groupIds.TrySplit(";"))
            {
                tasks.Add(Task.Run(async () =>
                {
                    var items = await GetAllByGroupIdAndEmpCodeAsync(s, empCode, KyDanhGia);
                    if (items?.Count > 0)
                    {
                        users.AddRange(items);
                    }
                }));
            }

            // Wait all background task done
            await Task.WhenAll(tasks);

            var tempUsers = new List<Employee>();

            // Filter duplicate user
            if (users?.Count > 0)
            {
                foreach (var employee in users)
                {
                    if (!tempUsers.Exists(x => x.EmpCode.Equals(employee.EmpCode)))
                        tempUsers.Add(employee);
                }
            }

            return tempUsers;
        }


        public async Task<bool> UpdateEmployeeAvatarImageAsync(string empCode, string filename, string original, string size512, string size125)
        {
            return await _db.ExecuteNonQueryAsync("prInsertOrUpdateEmployeeImage", new Dictionary<string, object>()
            {
                {"@pEmpCode", empCode},
                {"@pFileName", filename},
                {"@pOriginal", original},
                {"@p512", size512},
                {"@p125", size125},
            }, CommandType.StoredProcedure) > 0;
        }

        // private methods

        private async Task<Employee> MapToEmployee(DataTable source)
        {
            return (await MapToEmployees(source))?.FirstOrDefault();
        }

        private async Task<List<Employee>> MapToEmployees(DataTable source)
        {
            return await Task.Run(() =>
            {
                if (source?.Rows.Count > 0)
                {
                    var listItems = new List<Employee>();

                    foreach (DataRow row in source.Rows)
                    {
                        var entity = new Employee()
                        {
                            PortalId              = row.TryGetStringValue("USERID"),
                            Username              = row.TryGetStringValue("USERNAME"),
                            EmpId                 = row.TryGetStringValue("EmpID"),
                            EmpCode               = row.TryGetStringValue("EmpCode"),
                            FullName              = row.TryGetStringValue("FullName"),
                            Email                 = row.TryGetStringValue("Email"),
                            PhoneNumber           = row.TryGetStringValue("CURRENTPHONE"),
                            Title                 = row.TryGetStringValue("Title"),
                            BranchId              = row.TryGetStringValue("BRANCHID"),
                            Level1Id              = row.TryGetStringValue("Level1ID"),
                            Level1Name            = row.TryGetStringValue("Level1name"),
                            Level2Id              = row.TryGetStringValue("Level2ID"),
                            Level2Name            = row.TryGetStringValue("Level2Name"),
                            GroupId               = row.TryGetStringValue("LSPOSGROUPID"),
                            GroupCode             = row.TryGetStringValue("LSPOSGROUPCODEID"),
                            PositionCode          = row.TryGetStringValue("LSPositionCode"),
                            StartWorkingDate      = row.TryGetStringValue("STARTWORKINGDATE"),
                            PositionDate          = row.TryGetDateTimeValue("POSITION_DATE"),
                            BirthDay              = row.TryGetStringValue("BirthDay"),
                            Gender                = row.TryGetStringValue("Gender"),
                            IsLockedout           = row.TryGetIntValue("ISLOCKEDOUT") == 0,

                            Pin                   = row.TryGetStringValue("PIN"),
                            PinDate               = row.TryGetStringValue("PINDATE"),
                            PinPlace              = row.TryGetStringValue("PINPLACE"),

                            Address               = row.TryGetStringValue("THUONGTRU"),
                            TemporaryAddress      = row.TryGetStringValue("TAMTRU"),
                        };

                        //
                        listItems.Add(entity);
                    }

                    return listItems;
                }

                return null;
            });
        }

        public async Task<string> GetUserID(string empCode)
        {            
            return await (from uSER in _db.Users
                          where uSER.EmpCode.Equals(empCode)
                          select uSER.Id).FirstOrDefaultAsync();
        }
    }
}
