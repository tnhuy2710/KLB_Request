using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Data.Repositories.Base;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;

namespace CoreApi.Data.Repositories
{
    public interface IUserFormValueStorageRepository : IRepository<UserFormValueStorage, string>
    {
        Task<bool> InsertOrUpdateRowAsync(long userFormId, Dictionary<long, List<string>> values);
    }

    public class UserFormValueStorageRepository : BaseRepository<UserFormValueStorage, string>, IUserFormValueStorageRepository
    {
        public UserFormValueStorageRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        {
        }

        public async Task<bool> InsertOrUpdateRowAsync(long userFormId, Dictionary<long, List<string>> values)
        {
            var tableName = "UserFormValueStorages";

            foreach (var item in values)
            {
                var checkItem = await Db.ExecuteQueryAndReturnSingleAsync(
                    $"select * from {tableName} where UserFormId = @1 and RowNumber = @2",
                    new Dictionary<string, object>()
                    {
                        {"@1", userFormId},
                        {"@2", item.Key}
                    }, MapToModel);

                reRun:

                // Create new
                if (checkItem == null)
                {
                    var queryParams = new Dictionary<string, object>()
                    {
                        {"@1", Guid.NewGuid().ToString().ToLower()},
                        {"@2", userFormId},
                        {"@3", item.Key},
                    };

                    for (int i = 4; i <= item.Value.Count + 3; i++)
                        queryParams.Add($"@{i}", item.Value[i - 4]);

                    // Add DateTime
                    queryParams.Add($"@{item.Value.Count + 4}", DateTimeOffset.Now.ToString());

                    // Create new records
                    var query = $"insert into {tableName} values ({string.Join(",", queryParams.Keys)})";

                    await Db.ExecuteNonQueryAsync(query, queryParams);
                }
                else
                {
                    var deleteResult = await Db.ExecuteNonQueryAsync($"delete {tableName} where Id = @1", new Dictionary<string, object>()
                    {
                        {"@1", checkItem.Id}
                    });

                    if (deleteResult > 0)
                    {
                        checkItem = null;
                        goto reRun;
                    }
                }
            }

            return false;
        }

        private UserFormValueStorage MapToModel(IDataRecord row)
        {
            var colIndex = 0;
            return new UserFormValueStorage()
            {
                Id         = row.GetString(colIndex++),
                UserFormId = row.GetInt64(colIndex++),
                RowNumber  = row.GetInt32(colIndex++),
                A          = row.GetString(colIndex++),
                B          = row.GetString(colIndex++),
                C          = row.GetString(colIndex++),
                D          = row.GetString(colIndex++),
                E          = row.GetString(colIndex++),
                F          = row.GetString(colIndex++),
                G          = row.GetString(colIndex++),
                H          = row.GetString(colIndex++),
                I          = row.GetString(colIndex++),
                J          = row.GetString(colIndex++),
                K          = row.GetString(colIndex++),
                L          = row.GetString(colIndex++),
                M          = row.GetString(colIndex++),
                N          = row.GetString(colIndex++),
                O          = row.GetString(colIndex++),
                P          = row.GetString(colIndex++),
                Q          = row.GetString(colIndex++),
                R          = row.GetString(colIndex++),
                S          = row.GetString(colIndex++),
                T          = row.GetString(colIndex++),
                U          = row.GetString(colIndex++),
                V          = row.GetString(colIndex++),
                W          = row.GetString(colIndex++),
                X          = row.GetString(colIndex++),
                Y          = row.GetString(colIndex++),
                Z          = row.GetString(colIndex++),
                AA         = row.GetString(colIndex++),
                AB         = row.GetString(colIndex++),
                AC         = row.GetString(colIndex++),
                AD         = row.GetString(colIndex++),
                AE         = row.GetString(colIndex++),
                AF         = row.GetString(colIndex++),
                AG         = row.GetString(colIndex++),
                AH         = row.GetString(colIndex++),
                AI         = row.GetString(colIndex++),
                AJ         = row.GetString(colIndex++),
                AK         = row.GetString(colIndex++),
                AL         = row.GetString(colIndex++),
                AM         = row.GetString(colIndex++),
                AN         = row.GetString(colIndex++),
                AO         = row.GetString(colIndex++),
                AP         = row.GetString(colIndex++),
                AQ         = row.GetString(colIndex++),
                AR         = row.GetString(colIndex++),
                AS         = row.GetString(colIndex++),
                AT         = row.GetString(colIndex++),
                AU         = row.GetString(colIndex++),
                AV         = row.GetString(colIndex++),
                AW         = row.GetString(colIndex++),
                AX         = row.GetString(colIndex++),
                AY         = row.GetString(colIndex++),
                AZ         = row.GetString(colIndex++),
            };
        }
    }
}
