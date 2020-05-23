using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreApi.Extensions;

namespace CoreApi.Data.Repositories
{
    public interface IGlReportRepository
    {
        Task<double> GetGiaTriAsync(string date, string idKpi, string idOrgPer, string type);
        Task<double> GetGiaTriAsync(string date, string idKpi, string idOrgPer, string type, string kydanhgia);
        Task<List<List<string>>> GetKqKpiCtv(string date, string per);
        Task<List<List<string>>> GetKqKpiCtv(string date, string per, string KyDG);
    }

    public class GlReportRepository : IGlReportRepository
    {
        private readonly GlReportContext _glReport;

        public GlReportRepository(GlReportContext glReport)
        {
            _glReport = glReport;
        }


        public async Task<double> GetGiaTriAsync(string date, string idKpi, string idOrgPer, string type)
        {
            var parameters = new Dictionary<string, string>
            {
                {"v_ngay"   , date.Trim()},
                {"v_makpi"    , idKpi.Trim()},
                {"v_org_per"      , idOrgPer.Trim()},
                {"v_loaidl", type.Trim()}
            };

            return await _glReport.ExecuteStoredProcedureAsync(
                "pkg_kpi.get_giatri",
                parameters, 
                async reader =>
                {
                    if (reader.Rows.Count > 0)
                        return reader.Rows[0].TryGetDoubleValue(0);
                    return 0;
                });
        }

        public async Task<double> GetGiaTriAsync(string date, string idKpi, string idOrgPer, string type, string kydanhgia)
        {
            var parameters = new Dictionary<string, string>
            {
                {"v_ngay"   , date.Trim()},
                {"v_makpi"    , idKpi.Trim()},
                {"v_org_per"      , idOrgPer.Trim()},
                {"v_loaidl", type.Trim()},
                {"v_KyDG", kydanhgia.Trim()}
            };

            return await _glReport.ExecuteStoredProcedureAsync(
                "pkg_kpi.get_giatri_v2",
                parameters,
                async reader =>
                {
                    if (reader.Rows.Count > 0)
                        return reader.Rows[0].TryGetDoubleValue(0);
                    return 0;
                });
        }

        public async Task<List<List<string>>> GetKqKpiCtv(string date, string per)
        {
            var parameters = new Dictionary<string, string>
            {
                {"v_ngay"   , date.Trim()},
                {"v_id_ctv"    , per.Trim()},
                {"v_loai_nv"      , "CTV"},
            };

            return await _glReport.ExecuteStoredProcedureAsync(
                "pkg_kpi.get_kq_kpi_ctv",
                parameters,
                MapToListCells
                );
        }

        public async Task<List<List<string>>> GetKqKpiCtv(string date, string per, string KyDG)
        {
            var parameters = new Dictionary<string, string>
            {
                {"v_ngay"   , date.Trim()},
                {"v_id_ctv"    , per.Trim()},
                {"v_loai_nv"      , "CTV"},
                {"v_KyDG"      , KyDG},
            };

            return await _glReport.ExecuteStoredProcedureAsync(
                "pkg_kpi.get_kq_kpi_ctv_v2",
                parameters,
                MapToListCells
                );
        }

        private async Task<List<List<string>>> MapToListCells(DataTable source)
        {
            return await Task.Run(() =>
            {
                if (source?.Rows.Count > 0)
                {
                    var rows = new List<List<string>>();

                    // Get col count
                    var colCount = source.Columns.Count;

                    foreach (DataRow row in source.Rows)
                    {
                        var listItems = new List<string>();

                        // Get data foreach col
                        for (int i = 0; i < colCount; i++)
                            listItems.Add(row.TryGetStringValue(i));

                        rows.Add(listItems);
                    }

                    return rows;
                }

                return null;
            });
        }
    }
}
