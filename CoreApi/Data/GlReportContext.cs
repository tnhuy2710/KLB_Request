using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace CoreApi.Data
{
    public class GlReportContext
    {
        private readonly IConfiguration _configuration;

        private const string SettingPath = "GLReportConnection";  // Settings for get connection string

        public GlReportContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Execute Stored Proceduce and do action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedureName"></param>
        /// <param name="paramerters"></param>
        /// <param name="action"></param>
        /// <param name="haveCursor"></param>
        /// <returns></returns>
        public async Task<T> ExecuteStoredProcedureAsync<T>(
            string storedProcedureName,
            IDictionary<string, string> paramerters,
            Func<DataTable, Task<T>> action
        )
        {
            using (var cnn = new OracleConnection(_configuration.GetConnectionString(SettingPath)))
            {
                using (var cmd = new OracleCommand(storedProcedureName, cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.InitialLONGFetchSize = 10000;

                    // Adding Parameters
                    foreach (var paramerter in paramerters)
                    {
                        if (string.IsNullOrEmpty(paramerter.Value))
                            cmd.Parameters.Add(new OracleParameter(paramerter.Key.Trim(), DBNull.Value));
                        else
                            cmd.Parameters.Add(new OracleParameter(paramerter.Key.Trim(), paramerter.Value.Trim()));
                    }

                    // Add output parameter
                    cmd.Parameters.Add("v_out", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (var dt = new DataTable("Test"))
                    {
                        try
                        {
                            // Open Connect
                            await cnn.OpenAsync();

                            // Open Transaction for fix bugs cant using Procedure if have DBLink with MSSQL
                            using (var tran = cnn.BeginTransaction())
                            {
                                using (var ds = new DataSet())
                                {
                                    ds.EnforceConstraints = false;
                                    ds.Tables.Add(dt);
                                    
                                    dt.Load( await cmd.ExecuteReaderAsync());

                                    tran.Commit();
                                }
                            }

                            return await action(dt);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"Error when executing StoreProcedure: {e.Message}");
                            throw;
                        }
                    }
                }
            }
        }

        public async Task<double> ExecuteStoredProcedureAsync<T>(
            string storedProcedureName,
            IDictionary<string, string> paramerters
        )
        {
            using (var cnn = new OracleConnection(_configuration.GetConnectionString(SettingPath)))
            {
                using (var cmd = new OracleCommand(storedProcedureName, cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.InitialLONGFetchSize = 1000;

                    // Adding Parameters
                    foreach (var paramerter in paramerters)
                        cmd.Parameters.Add(new OracleParameter(paramerter.Key.Trim(), paramerter.Value.Trim()));

                    // Add output parameter
                    cmd.Parameters.Add("v_out", OracleDbType.Double).Direction = ParameterDirection.Output;

                    // Open Connect
                    await cnn.OpenAsync();

                    // Execute
                    await cmd.ExecuteReaderAsync();

                    if (Double.TryParse(cmd.Parameters["v_out"].Value.ToString(), out var result))
                        return result;
                    return 0;
                }
            }
        }
    }
}
