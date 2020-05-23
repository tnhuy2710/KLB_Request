using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Data.Repositories;
using CoreApi.Enums;
using CoreApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using CoreApi.Models;
using CoreApi.Security.Attributes;
using CoreApi.Utilities;
using CoreApi.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using CoreApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using CoreApi.MyFolder;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using System.Web;
using System.IO;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace CoreApi.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IFormRepository _formRepository;
        private readonly IFormStepRepository _formStepRepository;
        private readonly IUserFormRepository _userFormRepository;
        private readonly IEmployeeRepository _employeeRepository;        

        public HomeController(IFormStepRepository formStepRepository, IFormRepository formRepository, IUserFormRepository userFormRepository, IEmployeeRepository employeeRepository)
        {

            _formStepRepository = formStepRepository;
            _formRepository = formRepository;
            _userFormRepository = userFormRepository;
            _employeeRepository = employeeRepository;            
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {            
            var vm = new HomeViewModel();
            var empDetails = await _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode());

            var _formStep = await _formStepRepository.GetFromStepsByEmpCodeAsync(empDetails);
            if (_formStep?.Count > 0)
            {                
                foreach (var formStep in _formStep)
                {
                    var item = new FormItemViewModel()
                    {
                        Id = formStep.FormId,
                        Name = formStep.Form.Name,
                        Description = formStep.Form.Description,
                        ActionType = "view",
                        FormName = "#" + formStep.FormId.Remove(formStep.FormId.IndexOf('_')) + "_Form"
                    };

                    // Set action
                    item.ActionType = formStep.Claims.MakeLowerCase();

                    // Check Claims
                    switch ((FormStepActionType)Enum.Parse(typeof(FormStepActionType), formStep.Claims))
                    {
                        //Edit form
                        case FormStepActionType.Edit:
                            vm.EditForms.Add(item);
                            break;

                        //Confirm form
                        case FormStepActionType.Confirm:
                            // Check UserForm
                            vm.ConfirmForms.Add(item);
                            break;

                        //View form
                        case FormStepActionType.View:
                            // Check UserForm
                            vm.ViewForms.Add(item);
                            break;
                    }
                }
            }

            //var _viewPermission = await _formStepRepository.GetViewPermissionByEmpCodeAsync(empDetails);
            var _viewPermission = await _formRepository.GetAllGrantPermissionForViewAsync(empDetails);
            if (_viewPermission?.Count > 0)
            {
                foreach (var viewPermission in _viewPermission)
                {
                    var item = new FormItemViewModel()
                    {
                        Id = viewPermission.Id,
                        Name = viewPermission.Name,
                        Description = viewPermission.Description,
                        ActionType = "view",
                        FormName = "#" + viewPermission.Id.Remove(viewPermission.Id.IndexOf("_")) + "_Form"
                    };

                    // Set action
                    //item.ActionType = viewPermission.Claims.MakeLowerCase();
                    vm.ViewForms.Add(item);                                        
                }
            }
            //if (_formStep?.Count > 0)
            //{
            //    // List all UserForms create by this User
            //    var listUserForms = await _userFormRepository.GetFormsDataByUserIdAsync(GetCurrentUserId());
            //    foreach (var formStep in _formStep)
            //    {
            //        var item = new FormItemViewModel()
            //        {
            //            Id = formStep.FormId,
            //            Name = formStep.Form.Name,
            //            Description = formStep.Form.Description,
            //            ActionType = "view"
            //        };

            //        // Set action
            //        item.ActionType = formStep.Claims.MakeLowerCase();

            //        // Check Claims
            //        switch ((FormStepActionType)Enum.Parse(typeof(FormStepActionType), formStep.Claims))
            //        {
            //            //case FormStepActionType.View:
            //            //    if (DateTimeUtils.IsValid(formStep.AvailableFrom, formStep.ExpireIn))
            //            //        vm.ViewForms.Add(item);
            //            //    break;

            //            case FormStepActionType.Edit:
            //                // Check UserForm
            //                if (listUserForms?.Count > 0)
            //                {
            //                    var userForm = listUserForms.FirstOrDefault(x => x.FormId.Equals(formStep.FormId));
            //                    if (userForm != null)
            //                    {
            //                        // Kiểm tra current step nếu trùng với form step hiện tại là Edit thì add vào EditForms
            //                        // Nếu khác thì chuyển sang ViewForms vì Users hiện tại được phép xem Form của mình
            //                        //17/01/2018, kiem tra con thoi gian thi hien thi, het thi an luon

            //                        if (!userForm.CurrentStepId.Equals(formStep.Id) )
            //                        {
            //                            if (formStep.Form.CloseDate >= System.DateTime.Now)
            //                            {
            //                                vm.ViewForms.Add(item);
            //                            }
            //                            continue;
            //                        }
            //                    }
            //                }

            //                //
            //                if (formStep.Form.CloseDate >= System.DateTime.Now)
            //                {
            //                    vm.EditForms.Add(item);
            //                }
            //                break;
            //        }

            //    }
            //}


            //// Get all form assigned
            //var formsAssigned = await _formStepRepository.GetFormAssignedByEmpCodeAsync(GetCurrentUserEmpCode());
            //if (formsAssigned?.Count > 0)
            //{
            //    foreach (var userForm in formsAssigned)
            //    {
            //        // Check form exist
            //        if (vm.ConfirmForms?.FirstOrDefault(x => x.Id.Equals(userForm.FormId)) != null)
            //            continue;

            //        // Check current step
            //        if (userForm.CurrentStep != null)
            //        {
            //            if (!userForm.CurrentStep.Claims.Equals(FormStepActionType.Confirm.ToString()))
            //                continue;

            //            // Check QLTT Group and get list managers
            //            IList<Employee> acceptList = null;
            //            if (userForm.CurrentStep.HaveQlttGroup())
            //            {
            //                var lastAssignd = await _userFormRepository.GetLastLogByUserFormAndStepAsync(userForm.Id, userForm.CurrentStep.PrevStepId);
            //                acceptList = await _employeeRepository.GetManagersOfEmpCodeAsync(lastAssignd?.AuthorEmpCode);
            //            }

            //            //
            //            var check = await _formStepRepository.IsGrantPermissionForThisStepAsync(userForm.CurrentStep, empDetails, userForm, acceptList);
            //            if (check != null && check.IsAllow)
            //            {
            //                var item = new FormItemViewModel()
            //                {
            //                    Id = userForm.FormId,
            //                    Name = userForm.Form.Name,
            //                    Description = userForm.Form.Description,
            //                };

            //                // Set action
            //                item.ActionType = userForm.CurrentStep.Claims.MakeLowerCase();

            //                var expireTime = userForm.CurrentStep.ExpireIn;

            //                if (userForm.ExpireIn.HasValue) expireTime = userForm.ExpireIn.Value;

            //                // Check Claims
            //                switch ((FormStepActionType)Enum.Parse(typeof(FormStepActionType), userForm.CurrentStep.Claims))
            //                {
            //                    case FormStepActionType.Confirm:

            //                        if (DateTimeUtils.IsValid(userForm.AvailableFrom, expireTime))
            //                            vm.ConfirmForms?.Add(item);
            //                        break;
            //                }
            //            }
            //        }
            //    }
            //}

            //// Get all form can be view
            //var formViews = await _formRepository.GetAllGrantPermissionForViewAsync(empDetails);
            //if (formViews?.Count > 0)
            //{
            //    foreach (var formView in formViews)
            //    {
            //        var item = new FormItemViewModel()
            //        {
            //            Id = formView.Id,
            //            Name = formView.Name,
            //            Description = formView.Description,
            //            ViewType = "list"
            //        };

            //        if (vm.ViewForms.FirstOrDefault(x => x.Id.Equals(item.Id)) == null && formView.CloseDate>=System.DateTime.Now)
            //            vm.ViewForms.Add(item);
            //    }
            //}

            //// Get all form step can be view
            //var formStepViews = await _formStepRepository.GetAllGrantPermissionForViewAsync(empDetails);
            //if (formStepViews?.Count > 0)
            //{
            //    foreach (var formView in formStepViews)
            //    {
            //        var item = new FormItemViewModel()
            //        {
            //            Id = formView.Id,
            //            Name = formView.Name,
            //            Description = formView.Description,
            //            ViewType = "list"
            //        };

            //        if (vm.ViewForms.FirstOrDefault(x => x.Id.Equals(item.Id)) == null && formView.CloseDate >= System.DateTime.Now)
            //            vm.ViewForms.Add(item);
            //    }
            //}

            return View(vm);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [RequirePermission("Home_Contact", RequirePermissionType.View)]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Excel()
        {

            return View();
        }

        [HttpPost("excel/submit")]
        public async Task<IActionResult> SubmitForm()
        {

            return View("Excel");
        }

        //public class Thing
        //{
        //    public string Radio { get; set; }
        //    public string FirstTurn { get; set; }
        //    public string SecondTurn { get; set; }
        //    public string ThirdTurn { get; set; }
        //    public string SignGD { get; set; }
        //    public string SignTDV { get; set; }
        //    public string SignTDG { get; set; }

        //}

        public void PassThings(List<Thing> things)
        {
            var t = things;
        }

        public class Thing
        {
            public string PublishDate { get; set; }
            public string CloseDate { get; set; }
        }

        [HttpPost]
        public async Task<string> SendRequestAsync(string[] iTEM, string FormID, string[] cONTENT, string[] pERMISSION, string[] sTEPS)
        {
            Form _form = new Form();
            _form.Id = "GEMAIL_" + await _formRepository.Count_ID_Count("GEMAIL");
            _form.Name = "Phiếu yêu cầu gửi group gmail";
            _form.Description = "";
            _form.PublishDate = DateTimeOffset.Parse(pERMISSION[0]);// new DateTime(long.Parse());
            _form.CloseDate = DateTimeOffset.Parse(pERMISSION[1]);//new DateTime(long.Parse(pERMISSION[1]));
            _form.DateCreated = DateTime.Now;
            _form.FileType = pERMISSION[2];
            //Excel temp = new Excel(FormID, 1);
            //temp.SaveAsAnotherFile(@"D:\New folder\Klb_Request1\CoreApi\MyFolder\SaveFile\" + pERMISSION[2]);
            _form.SheetIndex = 1;
            _form.ViewPermissions = pERMISSION[3];
            _form.Confirm = -1;
            if (!await _formRepository.Insert_Into(_form))
            {
                return "Lỗi Insert Form table";
            }
            var iNDEX = 0;
            FormStep _formStep = new FormStep();
            //insert Id
            _formStep.FormId = _form.Id;
            //insert Name
            _formStep.Description = "Phiếu yêu cầu gửi group gmail";
            //insert Claims
            //insert GroupsId
            //insert Index
            //insert AvailableFrom
            //insert ExpireIn
            _formStep.DateCreated = DateTime.Now;
            _formStep.DateUpdated = DateTime.Now;
            _formStep.IsAllowSendEmail = false;
            _formStep.ViewPermissions = pERMISSION[3];
            _formStep.Confirm = -1;

            for (int i = 0; i < sTEPS.Length; i++)
            {
                iNDEX++;
                //insert Id
                _formStep.Id = _form.Id + "_" + iNDEX + ".Turns";
                if (sTEPS[i].Contains("-Ideally"))
                {
                    //insert Name
                    _formStep.Name = "Đánh giá";
                    //insert Claims
                    _formStep.Claims = "Edit";
                    //insert GroupsId
                    _formStep.GroupIds = sTEPS[i].Remove(sTEPS[i].IndexOf('-'));
                }
                else if (sTEPS[i].Contains("-End"))
                {
                    //insert Name
                    _formStep.Name = "Kết thúc";
                    //insert Claims
                    _formStep.Claims = "View";
                    //insert GroupsId
                    _formStep.GroupIds = sTEPS[i].Remove(sTEPS[i].IndexOf('-'));
                }
                else
                {
                    //insert Name
                    _formStep.Name = "Xét duyệt";
                    //insert Claims
                    _formStep.Claims = "Confirm";
                    //insert GroupsId
                    _formStep.GroupIds = sTEPS[i];
                }
                //insert Index
                _formStep.Index = iNDEX;
                //insert AvailableFrom
                _formStep.AvailableFrom = DateTime.Now;
                //insert ExpireIn
                _formStep.ExpireIn = DateTime.Now;
                if (i == 0)
                {

                    _formStep.PrevStepId = null;
                    _formStep.NextStepId = _form.Id + "_" + int.Parse((iNDEX + 1).ToString()) + ".Turns";
                }
                else if (i == 2)
                {
                    _formStep.PrevStepId = _form.Id + "_" + int.Parse((iNDEX - 1).ToString()) + ".Turns";
                    _formStep.NextStepId = null;
                }
                else
                {
                    _formStep.PrevStepId = _form.Id + "_" + int.Parse((iNDEX - 1).ToString()) + ".Turns";
                    _formStep.NextStepId = _form.Id + "_" + int.Parse((iNDEX + 1).ToString()) + ".Turns";
                }

                if (!await _formStepRepository.Insert_Into(_formStep))
                {
                    return "Lỗi Insert FormSteps table";
                }

                //HttpPostedFile file;
                //HttpServerUtilityBase Server = new HttpServerUtilityBase();
                //Server.MapPath("~/Images/" + fileName);
                //var fileName = Path.GetFileName(file.FileName);
                //var path = Path.Combine(Server.MapPath("~/Images/" + fileName));
                //file.SaveAs(path);
            }

            UserForm userForm = new UserForm();
            userForm.FormId = _form.Id;
            userForm.UserId = await _employeeRepository.GetUserID(GetCurrentUserEmpCode());
            userForm.CurrentStepId = _form.Id + "_" + 1 + ".Turns";
            userForm.AvailableFrom = DateTime.Now;
            userForm.DateCreated = DateTime.Now;
            if (!(await _userFormRepository.Insert_Into(userForm)))
            {
                return "Lỗi Insert UserForms table";
            }


            Excel excel = new Excel(FormID, 1);
            //Import phân quyền
            int column = 0, row = 0;
            //if(!await ImportExcel(excel, row, column, cONTENT))
            //{
            //    return "Lỗi Import Excel";
            //}
            for (int i = 0; i < cONTENT.Length; i++)
            {
                if (!(await excel.WriteToCell(row, column, cONTENT[i])))
                {
                    return "Lỗi Import Excel";
                }
                column++;
            }

            //Import số lượng input
            column = 0;
            row++;
            if (!(await excel.WriteToCell(row, column, iTEM.Length.ToString())))
            {
                return "Lỗi Import Excel";
            }
            //if (!await ImportExcel(excel, row, column, iTEM))
            //{
            //    return "Lỗi Import Excel";
            //}

            //Import input
            column = 0;
            row++;
            int index = 0;
            for (int i = 0; i < iTEM.Length; i++)
            {
                if (index == 7)
                {
                    row++;
                    column = 0;
                    index = 0;
                }
                if (! await excel.WriteToCell(row, column, iTEM[i]))
                {
                    return "Lỗi Import Excel";
                }
                column++;
                index++;
            }
            excel.SaveAs(_form.Id);
            //excel.SaveAs("hehe");
            excel.Close();
            return "Thêm thành công";
        }

        [HttpPost]
        public async Task<string> SendIdeally(string[] iDEALLY, string FormID)
        {
            Excel excel = new Excel(FormID, 1);

            int column = 2, row = 0;
            for (int i = 0; i < iDEALLY.Length; i++)
            {
                if (String.IsNullOrEmpty(iDEALLY[i]))
                {
                    continue;
                }
                else
                {                    
                    if (!await excel.WriteToCell(row, column, iDEALLY[i]))
                    {
                        return "Lỗi Import Excel";
                    }
                }
                column++;
            }
            excel.Save();
            excel.Close();
            //var empDetails = _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode());
            var current_Step = _userFormRepository.GetFormStepDataAsync(FormID);            

            if(!(await _userFormRepository.Update_ExpireInAsync(current_Step)))
            {
                return "Lỗi Update ExpireIn";
            }
            var next_Step = await _formStepRepository.UpdateFormSteps(current_Step);
            if(next_Step == null)
            {
                return "Lỗi Update FormSteps table";
            }

            if(next_Step.Claims == FormStepActionType.View.ToString())
            {
                if(!(await _formRepository.Update_FormAsync(FormID)))
                {
                    return "Lỗi Update Forms table";
                }
            }
            else
            {
                UserForm userForm = new UserForm();
                userForm.FormId = next_Step.FormId;
                userForm.UserId = await _employeeRepository.GetUserID(GetCurrentUserEmpCode());
                userForm.CurrentStepId = next_Step.Id;
                userForm.AvailableFrom = next_Step.AvailableFrom;
                userForm.DateCreated = DateTime.Now;
                //Không thêm ExpireIn
                if (!(await _userFormRepository.Insert_Into(userForm)))
                {
                    return "Lỗi Insert UserForms table";
                }
            }            

            return "Lưu thành công";
        }

        [HttpGet]
        public async Task<string> GetCurrentUserOrganizationAsync()
        {
            var empDetails = await _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode());
            return empDetails.BranchId;
        }

        [HttpGet]
        public async Task<Employee> GetCurrentUserAsync()
        {
            var empDetails = await _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode());
            return empDetails;
        }

        [HttpGet]
        public async Task<Employee> GetCurrentUserByIDAsync(string id)
        {
            var empDetails = await _employeeRepository.FindByEmpCodeAsync(id);
            return empDetails;
        }

        [HttpGet]
        public async Task<List<string>> Get_cONTENT_FromExcelByFormID(string id)
        {            
            return await Task.Run(() =>
            {
                Excel excel = new Excel(id, 1);
                List<string> cONTENT = new List<string>();
                if (!String.IsNullOrEmpty(id))
                {                                    
                    //data[0]
                    cONTENT.Add(GetCurrentUserEmpCode());

                    int column = 0, row = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        //var temp = excel.ReadCell(row, column);
                        cONTENT.Add(excel.ReadCell(row, column));
                        column++;
                    }
                    excel.Close();
                }
                return cONTENT;
            });
        }

        [HttpGet]
        public async Task<List<string>> Get_iTEM_FromExcelByFormID(string id)
        {            
            return await Task.Run( () =>
            {
                Excel excel = new Excel(id, 1);
                List<string> iTEM = new List<string>();
                if (!String.IsNullOrEmpty(id))
                {                    
                    //data[0]
                    int column = 0, row = 1;
                    var total_Column = int.Parse(excel.ReadCell(row, column));
                    row++;
                    int index = 0;
                    for (int i = 0; i < total_Column; i++)
                    {
                        if (index == 7)
                        {
                            row++;
                            column = 0;
                            index = 0;
                        }
                        iTEM.Add(excel.ReadCell(row, column));
                        column++;
                        index++;
                    }
                    excel.Close();
                }
                return iTEM;
            });                                                
        }

        public async Task<bool> ImportExcel(Excel excel, int row, int column, string[] _content)
        {
            return await Task.Run(async () =>
            {
                if (_content.Length > 0)
                {
                    //Excel excel = new Excel(Id, 1);
                    for (int i = 0; i < _content.Length; i++)
                    {
                        if (!await excel.WriteToCell(row, column, _content[i]))
                        {
                            return false;
                        }
                        column++;
                    }
                }
                return true;
            });           
        }
    }
}
