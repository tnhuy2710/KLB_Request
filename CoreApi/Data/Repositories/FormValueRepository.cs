using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer.Scaffolding.Internal;
using Microsoft.Extensions.Configuration;

namespace CoreApi.Data.Repositories
{
    public interface IFormValueRepository
    {
        Task<List<List<string>>> GetValuesByFunctionNameAsync(string functionName, List<string> paramsList);
    }

    public class FormValueRepository : IFormValueRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public FormValueRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<List<List<string>>> GetValuesByFunctionNameAsync(string functionName, List<string> paramsList)
        {
            using (var cnn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Error cant found SQL Connection string in appSettings.")))
            {
                using (var cmd = new SqlCommand(functionName, cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    await cnn.OpenAsync();

                    // Discover Stored Procedure parameters
                    SqlCommandBuilder.DeriveParameters(cmd);

                    // Set value for parameters
                    for (int i = 1; i < cmd.Parameters.Count; i++)
                    {
                        if (i <= paramsList?.Count)
                        {
                            if (string.IsNullOrEmpty(paramsList[i - 1]))
                                cmd.Parameters[i].Value = DBNull.Value;
                            else
                                cmd.Parameters[i].Value = paramsList[i - 1].Trim();
                        }
                    }
                    
                    //
                    var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    if (reader.HasRows)
                    {
                        var listItems = new List<List<string>>();

                        while (await reader.ReadAsync())
                        {
                            var items = new List<string>();

                            for (int i = 0; i < reader.VisibleFieldCount; i++)
                            {
                                var value = reader.GetValue(i);
                                items.Add(value != null ? value.ToString().Trim() : "");
                            }

                            listItems.Add(items);
                        }

                        return listItems;
                    }
                }
            }

            return null;
        }
    }
}
