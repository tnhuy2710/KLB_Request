using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Commons;
using CoreApi.Data.Repositories.Base;
using CoreApi.Extensions;
using CoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Data.Repositories
{
    public interface IFormRepository : IRepository<Form, string>
    {
        bool IsGrantPermissionForView(Form form, Employee empDetails, IList<Employee> acceptList = null);
        Task<bool> Insert_Into(Form form);
        Task<int> Count_ID_Count(string id);

        Task<Form> FindFormByID(string id);

        Task<bool> UpdateFormAsync(string ID, int cONFIRM);

        Task<IList<Form>> GetAllGrantPermissionForViewAsync(Employee empDetails);
    }

    public class FormRepository : BaseRepository<Form, string>, IFormRepository
    {
        public FormRepository(IHttpContextAccessor httpContext, ApplicationDbContext db) : base(httpContext, db)
        {
        }


        public bool IsGrantPermissionForView(Form form, Employee empDetails, IList<Employee> acceptList = null)
        {
            if (!string.IsNullOrEmpty(form.ViewPermissions))
            {
                var groupIds = form.ViewPermissions.TrySplit(";");
                if (groupIds?.Length > 0)
                {
                    foreach (var id in groupIds)
                    {
                        // Phân tách từng group để lấy thông tin theo pattern
                        // EmpCode|PosCode|Level1Id|Level2Id

                        var elements = id.Split('|');

                        var eEmpCode = elements[0];
                        var ePosCode = elements[1];
                        var eLevel1Id = elements[2];
                        var eLevel2Id = elements[3];

                        if (eEmpCode.Equals(empDetails.EmpCode))
                        {
                            return true;
                        }
                        else if (!string.IsNullOrEmpty(ePosCode))
                        {
                            if (ePosCode.Equals(AppContants.QlttGroupCode) && acceptList?.Count > 0)
                            {
                                if (acceptList.FirstOrDefault(x => x.EmpCode.Equals(empDetails.EmpCode)) != null)
                                    return true;
                            }

                            // Check level nếu khớp GroupCode
                            if (empDetails.PositionCode.MakeLowerCase().Equals(ePosCode.MakeLowerCase()))
                            {
                                // Check level 1 and level 2 khác empty và khớp
                                if (!string.IsNullOrEmpty(eLevel1Id) && empDetails.Level1Id.MakeLowerCase()
                                        .Equals(eLevel1Id.MakeLowerCase()) &&
                                    !string.IsNullOrEmpty(eLevel2Id) && empDetails.Level2Id.MakeLowerCase()
                                        .Equals(eLevel2Id.MakeLowerCase())
                                )
                                    return true;

                                // Check level 1 nếu khác empty và level 2 is empty
                                else if (!string.IsNullOrEmpty(eLevel1Id) && empDetails.Level1Id.MakeLowerCase()
                                             .Equals(eLevel1Id.MakeLowerCase()))
                                    return true;

                                // Level 1 và 2 đều empty thì hiển nhiên được cấp quyền 
                                else
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public async Task<IList<Form>> GetAllGrantPermissionForViewAsync(Employee empDetails)
        {
            //var empGroup = $"{empDetails.EmpCode}|||";
            //var posGroup = $"|{empDetails.PositionCode}|";

            //return await Queryable(x =>
            //        !string.IsNullOrEmpty(x.ViewPermissions) &&
            //        (x.ViewPermissions.Contains(empGroup) || x.ViewPermissions.Contains(posGroup)))
            //    .ToListAsync();

            return await Db.Forms.Where(x => x.ViewPermissions.Contains(empDetails.EmpCode)).ToListAsync();
        }

        [HttpPost]
        public async Task<bool> Insert_Into(Form _form)
        {
            if (!String.IsNullOrEmpty(_form.Id))
            {
                try
                {
                    Db.Forms.Add(_form);
                    await Db.SaveChangesAsync();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<int> Count_ID_Count(string ID)
        {            
            return Db.Forms.Count(a => a.Id.Contains(ID)) + 1;
        }

        public async Task<bool> UpdateFormAsync(string ID, int cONFIRM)
        {
            Form _form = Db.Forms.Where(x => x.Id.Equals(ID)).FirstOrDefault();
            _form.Confirm = cONFIRM;            
            try
            {
                Db.Entry(_form).State = EntityState.Modified;
                await Db.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<Form> FindFormByID(string id)
        {
            return await Db.Forms.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }
    }
}
