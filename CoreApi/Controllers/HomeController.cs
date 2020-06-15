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
using CoreApi.ViewModels.FormViewModels;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using CoreApi.Models.Excel;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using OfficeOpenXml.Style;
using System.Text;
using System.Net.Http.Headers;

namespace CoreApi.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IFormRepository _formRepository;
        private readonly IFormStepRepository _formStepRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserFormRepository _userFormRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment _environment;
        private readonly ICustomPropertyRepository _customProperty;
        private readonly IFormValueRepository _formValues;
        private readonly IGlReportRepository _glReport;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserFormValueStorageRepository _userFormValueStorage;

        public HomeController(IFormStepRepository formStepRepository,
                              IFormRepository formRepository,
                              IUserFormRepository userFormRepository,
                              IEmployeeRepository employeeRepository,
                              IHostingEnvironment environment,
                              ICustomPropertyRepository customProperty,
                              IFormValueRepository formValues,
                              IGlReportRepository glReport,
                              IUserFormValueStorageRepository userFormValueStorage,
                              IUserRepository userRepository,
                              ICommentRepository commentRepository)
        {

            _formStepRepository = formStepRepository;
            _formRepository = formRepository;
            _userFormRepository = userFormRepository;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _environment = environment;
            _customProperty = customProperty;
            _formValues = formValues;
            _glReport = glReport;
            _userFormValueStorage = userFormValueStorage;
            _commentRepository = commentRepository;
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

            //                        if (!userForm.CurrentStepId.Equals(formStep.Id))
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

            //        if (vm.ViewForms.FirstOrDefault(x => x.Id.Equals(item.Id)) == null && formView.CloseDate >= System.DateTime.Now)
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

        //public IActionResult About()
        //{
        //    //ViewData["Message"] = "Your application description page.";

        //    //return View();
        //}

        [HttpGet("{formId}")]
        public async Task<IActionResult> About(string formId)
        {            
            var vm = new FormViewModel()
            {
                FormId = formId
            };

            var form = await _formRepository.FindByIdAsync(formId);
            if (form != null)
            {
                var userForm = await _userFormRepository.GetFormDataAsync(formId, GetCurrentUserId());
                var steps = await _formStepRepository.GetFromStepsByFormIdAsync(formId);
                var editStep = steps?.FirstOrDefault(x => x.Claims.Equals(FormStepActionType.Edit.ToString()) && x.Confirm == -1);

                // Check permission for editable
                var result = await _formStepRepository.IsGrantPermissionForThisStepAsync(editStep, GetCurrentUserEmpCode(), userForm);

                //// Check form view permission
                //if (result != null && !result.IsGrant)
                //{
                //    result.IsGrant = _formRepository.IsGrantPermissionForView(form, result.EmployeeDetails);
                //}
                result.IsGrant = true;

                if (result != null && result.IsGrant)
                {
                    // Passed
                    var folderDataPath = Path.Combine(_environment.ContentRootPath, "MyFolder");
                    var fileFormPath = Path.Combine(folderDataPath, $"{formId}.{form.FileType}");

                    if (System.IO.File.Exists(fileFormPath))
                    {
                        //vm.IsAllowEditable = result.IsAllow;
                        vm.IsAllowEditable = true;

                        var htmlForm = await ProcessExcelToHtmlAsync(fileFormPath, formId, form.SheetIndex, result.UserForm, GetCurrentUserEmpCode(), vm.IsAllowEditable);

                        vm.Steps = steps;
                        //vm.CurrentStepIndex = result.CurrentStep != null ? steps.IndexOf(result.CurrentStep) + 1 : 1;
                        vm.CurrentStepIndex = editStep.Index;
                        vm.FormHtmlContent = htmlForm;
                        vm.Name = form.Name;
                        vm.Description = form.Description;
                        vm.StepsDetails = new List<FormStepDetailsViewModel>();

                        vm.StepsDetails = await BuildMilestoneTimeline(userForm, steps, vm.CurrentStepIndex);

                        return View(vm);
                    }
                    else
                    {
                        //_log.LogError("Error cant not found Form file in UserData folder.");
                    }
                }
            }

            ModelState.AddModelError("form", "Lỗi không tìm thấy biểu mẫu, vui lòng liên hệ bộ phận hỗ trợ.");
            vm.Name = "Có lỗi xảy ra";
            return View(vm);
        }

        [RequirePermission("Home_Contact", RequirePermissionType.View)]        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("{formId}")]
        public async Task<IActionResult> Excel(string formId)
        {
            var vm = new FormViewModel()
            {
                FormId = formId
            };

            var form = await _formRepository.FindByIdAsync(formId);
            if (form != null)
            {                                
                var userForm = await _userFormRepository.GetFormDataAsync(formId, await _employeeRepository.GetUserID(GetCurrentUserEmpCode()));                
                if (userForm == null)
                {
                    userForm = await _userFormRepository.GetFormDataForViewAsync(formId);
                }

                var steps = await _formStepRepository.GetFromStepsByFormIdAsync(formId);

                // Check permission for editable                

                // Passed
                var folderDataPath = Path.Combine(_environment.ContentRootPath, "MyFolder");
                var fileFormPath = Path.Combine(folderDataPath, $"{formId}.{form.FileType}");

                if (System.IO.File.Exists(fileFormPath))
                {
                    vm.IsAllowEditable = true;
                    if (userForm.User == null)
                    {
                        vm.IsAllowEditable = false;
                    }                    

                    var htmlForm = await ProcessExcelToHtmlAsync(fileFormPath, formId, form.SheetIndex, userForm, GetCurrentUserEmpCode(), vm.IsAllowEditable);

                    vm.Steps = steps;
                    vm.CurrentStepIndex = userForm.CurrentStep.Index;
                    vm.FormHtmlContent = htmlForm;
                    vm.Name = form.Name;
                    vm.Description = form.Description;
                    vm.StepsDetails = new List<FormStepDetailsViewModel>();

                    vm.StepsDetails = await BuildMilestoneTimeline(userForm, steps, vm.CurrentStepIndex);

                    return View(vm);
                }
            }

            ModelState.AddModelError("form", "Lỗi không tìm thấy biểu mẫu, vui lòng liên hệ bộ phận hỗ trợ.");
            vm.Name = "Có lỗi xảy ra";
            return View(vm);
        }

        [HttpPost("{formId}")]
        public async Task<DataResponse> SubmitForm(string formId, [FromBody] List<Thing> tHING)
        {       
            if(tHING.Count > 0)
            {
                Excel excel = new Excel(formId, (await _formRepository.FindFormByID(formId)).SheetIndex);
                foreach (var item in tHING)
                {
                    if (!String.IsNullOrEmpty(item.Value))
                    {
                        var aDDRESS = item.Address.Split('-');
                        var row = int.Parse(aDDRESS[0]);
                        var column = int.Parse(aDDRESS[1]);
                        if (!await excel.WriteToCell(row, column, item.Value))
                        {
                            return BadRequest($"<b>Lỗi import Excel</b>");
                        }
                    }
                }
                excel.Close();
            }            

            var currentStep = await _userFormRepository.GetFormStepDataAsync(formId);            

            if (!await _formStepRepository.UpdateFormSteps(currentStep.CurrentStepId, 1))
            {                
                return BadRequest($"<b>Lỗi Update FormSteps table</b>");
            }

            if (currentStep.CurrentStep.Claims == FormStepActionType.View.ToString())
            {
                if (!(await _formRepository.UpdateFormAsync(formId, 1)))
                {                    
                    return BadRequest($"<b>Lỗi Update Forms table</b>");
                }
            }            
            else
            {
                var nextStep = (await _formStepRepository.GetFromStepsByFormIdAsync(formId))
                                .Where(x => x.Id.Equals(currentStep.CurrentStep.NextStepId))
                                .FirstOrDefault();
                //var temp = nextStep.Where()
                UserForm userForm = new UserForm();
                userForm.FormId = nextStep.FormId;
                userForm.UserId = await _employeeRepository.GetUserID(nextStep.GroupIds);
                userForm.CurrentStepId = nextStep.Id;
                userForm.AvailableFrom = nextStep.AvailableFrom;
                userForm.ExpireIn = nextStep.ExpireIn;
                userForm.DateCreated = DateTime.Now;
                if (!(await _userFormRepository.Insert_Into(userForm)))
                {                    
                    return BadRequest($"<b>Lỗi Insert UserForms table</b>");
                }
            }

            return Success($"Biểu mẫu của bạn đã được gửi <b>thành công</b>");
        }

        [HttpPost("{formId}")]
        public async Task<Comment> SubmitComment(string formId, string cONTENT, int? rEPLY)
        {
            ////Tim
            //if(!String.IsNullOrEmpty(rEPLY.ToString()))
            //{
            //    var repRep = await _commentRepository.GetCommentByID(rEPLY.Value);
            //    if(repRep.ReplyID != null)
            //    {
            //        rEPLY = repRep.ReplyID;
            //    }
            //}
            var dateTime = DateTimeOffset.Now;            

            Comment cOMMENT = new Comment(formId, cONTENT, dateTime, rEPLY, GetCurrentUserId());
            if (!await _commentRepository.InsertInto(cOMMENT))
            {
                return null;
            }

            return await _commentRepository.GetCommentByDateComment(dateTime);
        }

        [HttpPost("{formId}")]
        public async Task<bool> EditComment(int ID, string cONTENT)
        {
            var cOMMENT = await _commentRepository.GetCommentByID(ID);
            cOMMENT.Content = cONTENT;
            if (!await _commentRepository.Update(cOMMENT))
            {
                return false;
            }
            return true;
        }

        [HttpPost("{formId}")]
        public async Task<bool> DeleteComment(int ID)
        {
            var cOMMENT = await _commentRepository.GetCommentByID(ID);
            if (!await _commentRepository.Delete(cOMMENT))
            {
                return false;
            }
            return true;
        }

        [HttpPost("{formId}")]
        public async Task<DataResponse> DeclineForm(string formId, [FromBody] List<Thing> tHING)
        {
            if (tHING.Count > 0)
            {
                Excel excel = new Excel(formId, (await _formRepository.FindFormByID(formId)).SheetIndex);
                foreach (var item in tHING)
                {
                    if (!String.IsNullOrEmpty(item.Value))
                    {
                        var aDDRESS = item.Address.Split('-');
                        var row = int.Parse(aDDRESS[0]);
                        var column = int.Parse(aDDRESS[1]);
                        if (!await excel.WriteToCell(row, column, item.Value))
                        {
                            return BadRequest($"<b>Lỗi import Excel</b>");
                        }
                    }
                }
                excel.Close();
            }

            var current_Step = await _userFormRepository.GetFormStepDataAsync(formId);
            
            if (!await _formStepRepository.UpdateFormSteps(current_Step.CurrentStepId, 0))
            {
                return BadRequest($"<b>Lỗi Update FormSteps table</b>");
            }

            if (!(await _formRepository.UpdateFormAsync(formId, 0)))
            {
                return BadRequest($"<b>Lỗi Update Forms table</b>");
            }            

            return Success($"Biểu mẫu của bạn đã được gửi <b>thành công</b>");
        }

        public class Thing
        {
            public string Address { get; set; }
            public string Value { get; set; }
        }

        public class infoPermission
        {
            public string iD { get; set; }
            public string nAME { get; set; }
            public string dESCRIPTION { get; set; }
            public string publishDate { get; set; }
            public string closeDate { get; set; }
            public string sheetIndex { get; set; }
            public string viewPermissions { get; set; }
            public List<pERMISSION> pERMISSIONs { get; set; }
        }

        public class pERMISSION
        {
            public string nAME { get; set; }
            public string empCode { get; set; }
            public string availableFrom { get; set; }
            public string expireIn { get; set; }            
            public string isAllowSendEmail { get; set; }
        }

        

        [HttpPost]
        public async Task<DataResponse> SendRequestAsync([FromBody] infoPermission infoPermission)
        {
            var fileType = infoPermission.iD.Split('.');
            if (await _formRepository.FindFormByID(fileType[0]) !=  null)
            {
                return BadRequest($"<b>Tên File đã có trong dữ liệu</b>");                
            }            
            Form _form = new Form();
            //fileType input
            _form.Id = fileType[0];// infoPermission[0].Remove(infoPermission[0].IndexOf("."));
            //form_Name input
            _form.Name = infoPermission.nAME;
            //dESCRIPTION input
            _form.Description = infoPermission.dESCRIPTION;
            //date_Start input
            _form.PublishDate = DateTimeOffset.Parse(infoPermission.publishDate);
            //date_End input
            _form.CloseDate = DateTimeOffset.Parse(infoPermission.closeDate);//new DateTime(long.Parse(pERMISSION[1]));
            _form.DateCreated = DateTime.Now;
            //fileType input
            _form.FileType = fileType[1];
            //sHEET input
            _form.SheetIndex = int.Parse(infoPermission.sheetIndex);
            //viewPermission
            _form.ViewPermissions = infoPermission.viewPermissions;
            if (!await _formRepository.Insert_Into(_form))
            {
                return BadRequest($"<b>Lỗi Insert Form table</b>");                
            }

            FormStep _formStep = new FormStep();
            _formStep.FormId = fileType[0];
            _formStep.DateCreated = DateTime.Now;
            _formStep.DateUpdated = DateTime.Now;
            _formStep.Confirm = -1;

            var iNDEX = 0;
            for (int i = 0; i < infoPermission.pERMISSIONs.Count; i++)
            {
                iNDEX++;
                //insert Id
                _formStep.Id = fileType[0] + "_" + iNDEX;
                if (infoPermission.pERMISSIONs[i].nAME.Equals("Edit"))
                {                                        
                    //insert Name
                    _formStep.Name = "Đánh giá";
                    //insert Description
                    _formStep.Description = "Đánh giá " + fileType[0];                    
                }
                else if (infoPermission.pERMISSIONs[i].nAME.Equals("Confirm"))
                {                    
                    //insert Name
                    _formStep.Name = "Xét duyệt";
                    //insert Description
                    _formStep.Description = "Xét duyệt " + fileType[0];                    
                }

                else if (infoPermission.pERMISSIONs[i].nAME.Equals("View"))
                {                    
                    //insert Name
                    _formStep.Name = "Kết thúc";
                    //insert Description
                    _formStep.Description = "Kết thúc " + fileType[0];                    
                }
                //insert Claims
                _formStep.Claims = infoPermission.pERMISSIONs[i].nAME;
                //insert GroupsId
                _formStep.GroupIds = infoPermission.pERMISSIONs[i].empCode;
                //insert Index
                _formStep.Index = iNDEX;
                //insert AvailableFrom
                _formStep.AvailableFrom = DateTimeOffset.Parse(infoPermission.pERMISSIONs[i].availableFrom);
                //insert ExpireIn
                _formStep.ExpireIn = DateTimeOffset.Parse(infoPermission.pERMISSIONs[i].expireIn);

                //insert NextStepId
                if (i == 0)
                {
                    _formStep.PrevStepId = null;
                    _formStep.NextStepId = _form.Id + "_" + (iNDEX + 1).ToString();
                }
                else if (i == infoPermission.pERMISSIONs.Count - 1)
                {
                    _formStep.PrevStepId = _form.Id + "_" + (iNDEX - 1).ToString();
                    _formStep.NextStepId = null;
                }
                else
                {
                    _formStep.PrevStepId = _form.Id + "_" + (iNDEX - 1).ToString();
                    _formStep.NextStepId = _form.Id + "_" + (iNDEX + 1).ToString();
                }

                //insert IsAllowSendEmail
                _formStep.IsAllowSendEmail = bool.Parse(infoPermission.pERMISSIONs[i].isAllowSendEmail);

                if (!await _formStepRepository.Insert_Into(_formStep))
                {                    
                    return BadRequest($"<b>Lỗi Insert Form table</b>");
                }                
            }

            UserForm userForm = new UserForm();
            userForm.FormId = fileType[0];
            userForm.UserId = await _employeeRepository.GetUserID(GetCurrentUserEmpCode());
            userForm.CurrentStepId = _form.Id + "_1";
            userForm.AvailableFrom = DateTimeOffset.Parse(infoPermission.pERMISSIONs[0].availableFrom);
            userForm.DateCreated = DateTime.Now;
            userForm.ExpireIn = DateTimeOffset.Parse(infoPermission.pERMISSIONs[0].expireIn);
            if (!(await _userFormRepository.Insert_Into(userForm)))
            {                
                return BadRequest($"<b>Lỗi Insert UserForms table</b>");
            }
            return Success($"Biểu mẫu của bạn đã được gửi <b>thành công</b>");
        }

        [HttpPost]
        public async Task<bool> SaveUploadFile()
        {
            try
            {
                var fileType = HttpContext.Request.Form.Files[0];
                if (fileType != null)
                {
                    var folderDataPath = Path.Combine(_environment.ContentRootPath, "MyFolder");
                    if (!Directory.Exists(folderDataPath)) Directory.CreateDirectory(folderDataPath);
                    using (var fileStream = new FileStream(Path.Combine(folderDataPath, $"{fileType.FileName}"), FileMode.Create))
                    {
                        await fileType.CopyToAsync(fileStream);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }            
        }        

        //[HttpPost]
        //public async Task<string> SendIdeally(string[] iDEALLY, string FormID)
        //{
        //    Excel excel = new Excel(FormID, 1);

        //    int column = 2, row = 0;
        //    for (int i = 0; i < iDEALLY.Length; i++)
        //    {
        //        if (String.IsNullOrEmpty(iDEALLY[i]))
        //        {
        //            continue;
        //        }
        //        else
        //        {                    
        //            if (!await excel.WriteToCell(row, column, iDEALLY[i]))
        //            {
        //                return "Lỗi Import Excel";
        //            }
        //        }
        //        column++;
        //    }
        //    excel.Save();
        //    excel.Close();
        //    //var empDetails = _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode());
        //    var current_Step = _userFormRepository.GetFormStepDataAsync(FormID);

        //    if(!(await _userFormRepository.Update_ExpireInAsync(current_Step)))
        //    {
        //        return "Lỗi Update ExpireIn";
        //    }
        //    var next_Step = await _formStepRepository.UpdateFormSteps(current_Step);
        //    if(next_Step == null)
        //    {
        //        return "Lỗi Update FormSteps table";
        //    }

        //    if(next_Step.Claims == FormStepActionType.View.ToString())
        //    {
        //        if(!(await _formRepository.Update_FormAsync(FormID)))
        //        {
        //            return "Lỗi Update Forms table";
        //        }
        //    }
        //    else
        //    {
        //        UserForm userForm = new UserForm();
        //        userForm.FormId = next_Step.FormId;
        //        userForm.UserId = await _employeeRepository.GetUserID(GetCurrentUserEmpCode());
        //        userForm.CurrentStepId = next_Step.Id;
        //        userForm.AvailableFrom = next_Step.AvailableFrom;
        //        userForm.DateCreated = DateTime.Now;
        //        //Không thêm ExpireIn
        //        if (!(await _userFormRepository.Insert_Into(userForm)))
        //        {
        //            return "Lỗi Insert UserForms table";
        //        }
        //    }

        //    return "Lưu thành công";
        //}

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
        

        public async Task<string> ProcessExcelToHtmlAsync(string filePath, string formId, int sheetIndex, UserForm userForm, string targetEmpCode, bool isIncludeInput = true)
        {
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var ms = new MemoryStream())
                {
                    // Load file to Stream
                    await file.CopyToAsync(ms);

                    // Close stream after copy
                    file.Dispose();

                    // UserForm Values
                    var userFormValues = new List<UserFormValue>();

                    // Get list cell submitted and UserFormValues
                    //var cellsSubmitted = new List<CellSubmit>();
                    //if (userForm != null)
                    //{
                    //    try
                    //    {
                    //        cellsSubmitted = await Task.Run(() => JsonConvert.DeserializeObject<List<CellSubmit>>(userForm.InputValues));
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Debug.WriteLine($"Error while parse object from UserData: {e.Message}");
                    //        throw;
                    //    }

                    //    // Try get user form values
                    //    userFormValues = await _userFormRepository.GetValuesByIdAsync(userForm.Id);
                    //}

                    // Get User Details 
                    //Employee userDetails = await _employeeRepository.FindByEmpCodeAsync(targetEmpCode, await _customProperty.GetCustomFormPropertyAsync(formId, "KYDG"));
                    Employee userDetails = await _employeeRepository.FindByEmpCodeAsync(targetEmpCode);
                    //var userDetails = await _employeeRepository.FindByEmpCodeAsync(targetEmpCode);
                    if (userDetails == null)
                    {
                        throw new Exception("Lỗi không tìm thấy thông tin nhân sự.");
                    }
                    // Stored list
                    var rows = new List<Row>();
                    var rowsFill = new List<RowFillDetail>();

                    // Open Excel from Stream
                    using (var excelPackage = new ExcelPackage(ms))
                    {
                        // Load workSheet by Index
                        using (var workSheet = excelPackage.Workbook.Worksheets[sheetIndex])
                        {
                            // Get Size of Document
                            var dimension = workSheet.Dimension;

                            // Get total row and col of excel file
                            var totalRow = dimension.End.Row;
                            var totalCol = dimension.End.Column;

                            // Begin process data row by row
                            for (int rowPos = dimension.Start.Row; rowPos < totalRow; rowPos++)
                            {

                                // Ignore hidden row
                                if (workSheet.Row(rowPos).Hidden) continue;

                                // Get group index of current row
                                var groupIndex = workSheet.Row(rowPos).OutlineLevel;

                                //
                                var row = new Row()
                                {
                                    GroupIndex = groupIndex,
                                    Position = rowPos,
                                    OriginalPosition = rowPos,
                                };

                                // Begin process data column by column
                                for (int colPos = dimension.Start.Column; colPos <= totalCol; colPos++)
                                {

                                    // Get cell by row and col index
                                    using (var cell = workSheet.Cells[rowPos, colPos])
                                    {
                                        var cellItem = new Cell()
                                        {
                                            CellType = CellType.Display,
                                            Position = colPos,
                                            Address = cell.Address
                                        };

                                        var content = cell.Text;
                                        var classes = "";
                                        var styles = "";

                                        // Handle content for input
                                        //var inputPattern = @"\(input(\:(\w+)|)\)";
                                        var inputPattern = @"\(input(\:(.+)|)\)";
                                        var regex = new Regex(inputPattern, RegexOptions.IgnoreCase);
                                        var match = regex.Match(content);
                                        if (match.Success && string.IsNullOrEmpty(cell.Formula))
                                        {
                                            var inputType = "text";
                                            var inputList = "";

                                            // Get type of input
                                            if (match.Groups.Count >= 3)
                                            {
                                                switch (match.Groups[2].Value.MakeLowerCase())
                                                {
                                                    case "number":
                                                    case "currency":
                                                    case "percent":
                                                    case "message":
                                                        inputType = match.Groups[2].Value.MakeLowerCase();
                                                        break;
                                                    //(input: list[bjh; bhjj; jjjj])
                                                    default:
                                                        if (match.Groups[2].Value.MakeLowerCase().StartsWith("list["))
                                                        {
                                                            inputType = "list";
                                                            inputList = match.Groups[2].Value.Substring(5, match.Groups[2].Value.Length - 6);
                                                        }
                                                        break;

                                                }
                                            }

                                            //
                                            cellItem.CellType = CellType.Editable;
                                            cellItem.InputType = inputType;
                                            cellItem.InputList = inputList;
                                            //     cellItem.InputType = "List";
                                        }

                                        // Handle Formula
                                        if (!string.IsNullOrEmpty(cell.FormulaR1C1))
                                        {
                                            cellItem.Formula = cell.FormulaR1C1;
                                        }

                                        // Handle replace function to content
                                        {
                                            // Replace content with custom function
                                            content = content.Replace("GET_FULLNAME()", userDetails.FullName);
                                            content = content.Replace("GET_EMAIL()", userDetails.Email);
                                            content = content.Replace("GET_EMPCODE()", userDetails.EmpCode);
                                            content = content.Replace("GET_TITLE()", userDetails.Title);
                                            content = content.Replace("GET_STARTWORKINGDATE()", userDetails.StartWorkingDate);
                                            content = content.Replace("GET_PORTALID()", userDetails.Username);
                                            content = content.Replace("GET_BRANCHID()", userDetails.BranchId);
                                            content = content.Replace("GET_GROUPID()", userDetails.GroupId);
                                            content = content.Replace("GET_GROUPCODE()", userDetails.GroupCode);
                                            content = content.Replace("GET_POSITIONDATE()", userDetails.PositionDate.ToString("dd/MM/yyyy"));
                                            content = content.Replace("GET_LEVEL1ID()", userDetails.Level1Id);
                                            content = content.Replace("GET_LEVEL2ID()", userDetails.Level2Id);

                                            content = content.Replace("GET_BRANCHNAMEBYLEVEL()", GetBranchNameByLevel(userDetails));

                                            // Handle get custom value
                                            {
                                                while (content.TryGetFunction("GET_VALUE", out var function, out var functionContent))
                                                {
                                                    // Trying to get value from Parameter


                                                    var customProperty = await _customProperty.GetCustomFormPropertyAsync(formId, functionContent);
                                                    if (!string.IsNullOrEmpty(customProperty))
                                                    {
                                                        content = content.Replace(function, customProperty);
                                                    }
                                                    else
                                                    {
                                                        // Trying to find on UserFormValues
                                                        if (userFormValues?.Count > 0)
                                                        {
                                                            var value = userFormValues.FirstOrDefault(x => x.Key.Equals(functionContent.Trim()));
                                                            if (value != null)
                                                                content = content.Replace(function, value.Value);
                                                            else
                                                            {
                                                                // Replace empty content
                                                                content = content.Replace(function, "");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // Replace empty content
                                                            content = content.Replace(function, "");
                                                        }
                                                    }
                                                }
                                            }

                                            // Handle get custom get_value_v2
                                            {
                                                while (content.TryGetFunction("GET_VALUE_V2", out var function, out var functionContent))
                                                {
                                                    // Trying to get value from Parameter


                                                    var customProperty = await _customProperty.GetCustomFormPropertyAsync(formId, functionContent);
                                                    if (!string.IsNullOrEmpty(customProperty))
                                                    {
                                                        content = content.Replace(function, customProperty);
                                                    }
                                                    else
                                                    {
                                                        // Trying to find on UserFormValues
                                                        if (userFormValues?.Count > 0)
                                                        {
                                                            var value = userFormValues.FirstOrDefault(x => x.Key.Equals(functionContent.Trim()));
                                                            if (value != null)
                                                                content = content.Replace(function, value.Value);
                                                            else
                                                            {
                                                                // Replace empty content
                                                                content = content.Replace(function, "");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // Replace empty content
                                                            content = content.Replace(function, "");
                                                        }
                                                    }
                                                }
                                            }

                                            // Handle get values function
                                            {
                                                while (content.TryGetFunction("GET_VALUES", out var function, out var functionContent))
                                                {
                                                    var strParams = functionContent.Split(',');
                                                    if (strParams.Length > 0)
                                                    {
                                                        var functionName = strParams[0].Trim();
                                                        var parameters = strParams.Skip(1).Take(strParams.Length - 1).ToList();

                                                        var values = await _formValues.GetValuesByFunctionNameAsync(functionName, parameters);
                                                        if (values?.Count > 0)
                                                        {
                                                            // Add fill data to stored
                                                            foreach (var value in values)
                                                            {
                                                                rowsFill.Add(new RowFillDetail()
                                                                {
                                                                    RowStart = rowPos,
                                                                    ColStart = colPos,
                                                                    Values = value
                                                                });
                                                            }
                                                        }
                                                    }

                                                    // Remove function
                                                    content = content.Replace(function, "");
                                                }
                                            }

                                            // Handle get gia tri function
                                            {
                                                if (content.TryGetFunction("GET_GIATRI", out var function, out var functionContent))
                                                {
                                                    var childFormulaText = functionContent.Split(',');

                                                    // Handle today
                                                    var date = HandleToDay(childFormulaText[0].Trim());
                                                    var kpiCode = childFormulaText[1].Trim();
                                                    var perId = childFormulaText[2].Trim();
                                                    var dataType = childFormulaText[3].RemoveWhitespace().MakeUpperCase();

                                                    // Begin get value from database
                                                    var value = await _glReport.GetGiaTriAsync(date, kpiCode, perId, dataType);

                                                    // Replace content
                                                    content = content.Replace(function, value.ToString("#0.###"));
                                                }
                                            }
                                            // Handle GET_GIATRI_V2
                                            {
                                                if (content.TryGetFunction("GET_GIATRI_V2", out var function, out var functionContent))
                                                {
                                                    var childFormulaText = functionContent.Split(',');

                                                    // Handle today
                                                    var date = HandleToDay(childFormulaText[0].Trim());
                                                    var kpiCode = childFormulaText[1].Trim();
                                                    var perId = childFormulaText[2].Trim();
                                                    var dataType = childFormulaText[3].RemoveWhitespace().MakeUpperCase();
                                                    var KyDanhGia = childFormulaText[4].RemoveWhitespace().MakeUpperCase();

                                                    // Begin get value from database
                                                    var value = await _glReport.GetGiaTriAsync(date, kpiCode, perId, dataType, KyDanhGia);

                                                    // Replace content
                                                    content = content.Replace(function, value.ToString("#0.###"));
                                                }
                                            }
                                            // Handle get kq kpi ctv
                                            {
                                                if (content.TryGetFunction("GET_KQ_KPI_CTV", out var function, out var functionContent))
                                                {
                                                    var childFormulaText = functionContent.Split(',');

                                                    if (childFormulaText.Length >= 2)
                                                    {
                                                        var date = childFormulaText[0].Trim();
                                                        var branchId = childFormulaText[1].Trim();
                                                        var KyDG = childFormulaText.Length == 3 ? childFormulaText[2].Trim() : "";
                                                        var values = KyDG == "" ? await _glReport.GetKqKpiCtv(date, branchId) : await _glReport.GetKqKpiCtv(date, branchId, KyDG);

                                                        // Replace content
                                                        if (values?.Count > 0)
                                                        {
                                                            // Add fill data to stored
                                                            foreach (var value in values)
                                                            {
                                                                rowsFill.Add(new RowFillDetail()
                                                                {
                                                                    RowStart = rowPos,
                                                                    ColStart = colPos,
                                                                    Values = value
                                                                });
                                                            }
                                                        }
                                                    }

                                                }
                                            }

                                            // 
                                        }

                                        // Handle border, style, font color, format
                                        if (cell.Style != null)
                                        {
                                            var style = cell.Style;

                                            // Handle text format
                                            if (!string.IsNullOrEmpty(style.Numberformat?.Format))
                                            {
                                                switch (style.Numberformat?.Format)
                                                {
                                                    case "#,##0":
                                                        cellItem.Format = "currency";
                                                        break;
                                                }

                                                if (style.Numberformat.Format.Trim().EndsWith("%"))
                                                {
                                                    cellItem.Format = "percent";
                                                    if (content.EndsWith("%"))
                                                    {
                                                        content = (double.Parse(content.Replace("%", "")) / 100).ToString("0.##");
                                                    }
                                                }
                                            }

                                            // Handle border
                                            if (style.Border != null)
                                            {
                                                if (style.Border.Top?.Style != ExcelBorderStyle.None)
                                                {
                                                    classes += "border-top ";
                                                }

                                                if (style.Border.Bottom?.Style != ExcelBorderStyle.None)
                                                {
                                                    classes += "border-bottom ";
                                                }

                                                if (style.Border.Left?.Style != ExcelBorderStyle.None)
                                                {
                                                    classes += "border-left ";
                                                }

                                                if (style.Border.Right?.Style != ExcelBorderStyle.None)
                                                {
                                                    classes += "border-right ";
                                                }
                                            }

                                            // Handle position
                                            switch (style.VerticalAlignment)
                                            {
                                                case ExcelVerticalAlignment.Top:
                                                    classes += "vertical-top ";
                                                    break;
                                                case ExcelVerticalAlignment.Center:
                                                    classes += "vertical-middle ";
                                                    break;
                                                case ExcelVerticalAlignment.Bottom:
                                                    classes += "vertical-bottom ";
                                                    break;
                                                case ExcelVerticalAlignment.Justify:        // Not support yet
                                                case ExcelVerticalAlignment.Distributed:    // Not support yet  
                                                default:
                                                    classes += "vertical-top ";
                                                    break;
                                            }

                                            // Handle position
                                            switch (style.HorizontalAlignment)
                                            {

                                                case ExcelHorizontalAlignment.Left:
                                                    classes += "align-start ";
                                                    break;

                                                case ExcelHorizontalAlignment.Center:
                                                case ExcelHorizontalAlignment.CenterContinuous:
                                                    classes += "align-center ";
                                                    break;

                                                case ExcelHorizontalAlignment.Right:
                                                    classes += "align-end ";
                                                    break;

                                                case ExcelHorizontalAlignment.Justify:
                                                    classes += "align-justify ";
                                                    break;

                                                case ExcelHorizontalAlignment.Distributed:      // Not support yet
                                                case ExcelHorizontalAlignment.Fill:             // Not support yet
                                                case ExcelHorizontalAlignment.General:
                                                default:
                                                    classes += "align-inherit ";
                                                    break;
                                            }

                                            // Handle font style
                                            if (style.Font.Bold)
                                            {
                                                classes += "bold ";
                                            }

                                            if (style.Font.Italic)
                                            {
                                                classes += "italic ";
                                            }

                                            if (style.Font.Strike)
                                            {
                                                classes += "line-through ";
                                            }

                                            if (style.Font.UnderLine)
                                            {
                                                classes += "underline ";
                                            }

                                            // Font Size
                                            styles += $"font-size: {style.Font.Size}px; ";

                                            // Handle font color
                                            if (!string.IsNullOrEmpty(style.Font.Color.Rgb))
                                            {
                                                // Handle alpha
                                                var colorRgb = style.Font.Color.Rgb;

                                                if (colorRgb.Length == 8)
                                                {
                                                    colorRgb = colorRgb.Substring(2, 6);
                                                }

                                                styles += $"color: #{colorRgb}; ";
                                            }

                                            // Handle fill background
                                            if (!string.IsNullOrEmpty(style.Fill.BackgroundColor.Rgb))
                                            {
                                                // Handle alpha
                                                var colorRgb = style.Fill.BackgroundColor.Rgb;

                                                if (colorRgb.Length == 8)
                                                {
                                                    colorRgb = colorRgb.Substring(2, 6);
                                                }

                                                styles += $"background-color: #{colorRgb}; ";
                                            }
                                        }

                                        // Handle merge row
                                        if (cell.Merge)
                                        {
                                            cellItem.IsMerge = cell.Merge;
                                        }

                                        // Build item
                                        cellItem.Classes = classes;
                                        cellItem.Styles = styles;
                                        cellItem.Content = content;

                                        // Add cell item to list of current row
                                        row.Cells.Add(cellItem);

                                    }   // End cell process

                                }   // End col process

                                // Add row to list
                                rows.Add(row);

                            }   // End row process
                        }
                    }

                    // Fill all cells to temporary excel
                    using (var cloneExcel = new ExcelPackage(ms))
                    {
                        using (var newWorksheet = cloneExcel.Workbook.Worksheets[sheetIndex])
                        {
                            // Process rows for addition, edit, remove
                            if (rows.Count > 0)
                            {
                                var rowPadding = 0;

                                for (var rowPos = 0; rowPos < rows.Count; rowPos++)
                                {
                                    // Get row by position
                                    var row = rows[rowPos];

                                    // Handle rows fill
                                    if (rowsFill?.Count > 0)
                                    {
                                        int lastRowPos = 0;
                                        int totalRowExpanded = 0;
                                        int countRowFill = Int32.Parse(rowsFill.Count.ToString());

                                        for (int rowFillIndex = 0; rowFillIndex < countRowFill; rowFillIndex++)
                                        {
                                            var rowFill = rowsFill.FirstOrDefault();
                                            if (rowFill.RowStart + rowPadding - 1 != rowPos)
                                            {
                                                rowPadding += totalRowExpanded;
                                                goto endOfRowFills;
                                            }

                                            // Fill values for current row
                                            if (lastRowPos != rowFill.RowStart)
                                            {
                                                // Load data for current row
                                                for (int j = 0; j < rowFill.Values.Count; j++)
                                                {
                                                    rows[rowPos + rowFillIndex].Cells[rowFill.ColStart - 1 + j].Content = rowFill.Values[j];
                                                }

                                                // Update last row position
                                                lastRowPos = rowFill.RowStart;

                                                // Remove item
                                                rowsFill.Remove(rowFill);

                                                continue;
                                            }

                                            // Clone new row
                                            Row newRow = row.Clone();
                                            newRow.IsExpanded = true;
                                            newRow.Position += rowFillIndex + rowPadding;

                                            // 
                                            for (int j = 0; j < rowFill.Values.Count; j++)
                                            {
                                                var cell = rows[rowPos].Cells[rowFill.ColStart - 1 + j];

                                                // Clone cell
                                                var newCell = new Cell()
                                                {
                                                    Address = cell.Address,
                                                    Content = rowFill.Values[j],
                                                    Position = cell.Position,
                                                    CellType = cell.CellType,
                                                    IsMerge = cell.IsMerge,
                                                    Classes = cell.Classes,
                                                    Styles = cell.Styles,
                                                    InputType = cell.InputType,
                                                    Formula = cell.Formula,
                                                    Format = cell.Format
                                                };

                                                newRow.Cells[rowFill.ColStart - 1 + j] = newCell;
                                            }

                                            // Clone current and append to below
                                            rows.Insert(rowPos + rowFillIndex, newRow);

                                            // Clone row for worksheet
                                            newWorksheet.InsertRow(rowPos + rowFillIndex + 1, 1, rowPos + 1);

                                            //
                                            totalRowExpanded++;

                                            // Remove item
                                            rowsFill.Remove(rowFill);
                                        }

                                        rowPadding += totalRowExpanded;
                                    }

                                endOfRowFills:

                                    // Handle rows submitted by user, it mean addition rows
                                    //if (row.GroupIndex > 0)
                                    //{
                                    //    // Get all row base on target row
                                    //    var additionRows = cellsSubmitted.Where(x => !string.IsNullOrEmpty(x.Base) && x.Base.StartsWith($"{row.Position}-"))
                                    //        .GroupBy(x => x.Address.Substring(0, x.Address.IndexOf("-", StringComparison.Ordinal)))
                                    //        .ToList();

                                    //    if (additionRows?.Count > 0)
                                    //    {
                                    //        var rowIndex = 0;

                                    //        foreach (var additionRow in additionRows)
                                    //        {
                                    //            // Clone new row
                                    //            var newRow = row.Clone();
                                    //            newRow.IsExpanded = true;
                                    //            newRow.Position += rowIndex + 1;
                                    //            newRow.OriginalPosition = row.Position;
                                    //            newRow.Cells?.Clear();
                                    //            newRow.BaseOnRow = rowPos;

                                    //            // Load cells submitted of current row
                                    //            var additionCells = additionRow.ToList();

                                    //            // Load cells
                                    //            foreach (var rowCell in row.Cells)
                                    //            {
                                    //                // Get cell from submitted
                                    //                var cell = additionCells.FirstOrDefault(x => x.Address.EndsWith($"-{rowCell.Position}"));

                                    //                var cellContent = "";

                                    //                if (cell != null)
                                    //                {
                                    //                    cellContent = cell.Value;
                                    //                }

                                    //                // Clone cell
                                    //                var newCell = new Cell()
                                    //                {
                                    //                    Address = $"{additionRow.Key}-{rowCell.Position}",
                                    //                    Content = cellContent,
                                    //                    Position = rowCell.Position,
                                    //                    CellType = rowCell.CellType,
                                    //                    IsMerge = rowCell.IsMerge,
                                    //                    Classes = rowCell.Classes,
                                    //                    Styles = rowCell.Styles,
                                    //                    InputType = rowCell.InputType,
                                    //                    Formula = rowCell.Formula,
                                    //                    Format = rowCell.Format
                                    //                };

                                    //                // Add cell to row
                                    //                newRow.Cells.Add(newCell);
                                    //            }

                                    //            //
                                    //            rows.Insert(row.Position + rowIndex, newRow);

                                    //            // Clone row for worksheet
                                    //            var index = row.OriginalPosition + rowIndex++ + 1;
                                    //            newWorksheet.InsertRow(index, 1, row.OriginalPosition);
                                    //        }
                                    //    }
                                    //}

                                    // Get current row height
                                    var rowHeight = newWorksheet.Row(row.Position).Height;

                                    // Set row height
                                    newWorksheet.Row(rowPos + 1).Height = rowHeight;

                                    // Fill value from user submit or excel
                                    foreach (var cell in row.Cells)
                                    {
                                        var cellAddressPosition = $"{row.Position}-{cell.Position}";

                                        // Handle formula
                                        if (!string.IsNullOrEmpty(cell.Formula))
                                        {
                                            newWorksheet.Cells[rowPos + 1, cell.Position].FormulaR1C1 = cell.Formula;
                                        }

                                        // Build content value
                                        if (cell.CellType == CellType.Display)
                                        {
                                            var content = cell.Content;

                                            if (content.Equals("#VALUE!")) content = "";

                                            cell.Content = content;
                                        }
                                        else if (cell.CellType == CellType.Editable)
                                        {
                                            var inputValue = "";

                                            // Get cell data submitted
                                            //var cellSubmitted = cellsSubmitted.FirstOrDefault(x => x.Address.Equals(cellAddressPosition));

                                            //// Set value is submitted by user
                                            //if (cellSubmitted != null) inputValue = cellSubmitted.Value;

                                            // Build input content
                                            cell.Content = inputValue;
                                        }

                                        // Handle format Number, Text and set value for cell
                                        if (string.IsNullOrEmpty(cell.Formula))
                                        {
                                            dynamic value;
                                            if (cell.Content.IsDouble(out var doubleValue))
                                                value = doubleValue;
                                            else
                                                value = cell.Content;

                                            if (value.ToString().Equals("#VALUE!")) value = "";

                                            newWorksheet.Cells[rowPos + 1, cell.Position].Value = value;
                                        }
                                    }

                                    // Close row
                                }

                                // Let's temporary calculate all formula
                                newWorksheet.Calculate();
                            }

                            // Begin process temporary excel to html


                            // Re-index rows items storage
                            int startRow = 0;
                            foreach (var row in rows)
                            {
                                if (startRow < 1) startRow = row.Position;
                                else startRow++;

                                row.Position = startRow;
                            }

                            // Get Size of Document
                            var dimension = newWorksheet.Dimension;

                            // Get total row and col of excel file
                            var totalRow = dimension.End.Row;
                            var totalCol = dimension.End.Column;

                            // Start StringBuilder
                            var sb = new StringBuilder();

                            // Begin convert temporary excel to HTML
                            for (int rowPos = dimension.Start.Row; rowPos < totalRow; rowPos++)
                            {
                                // Ignore hidden row
                                if (newWorksheet.Row(rowPos).Hidden) continue;

                                // Get Row Data Storaged
                                var rowItem = rows.FirstOrDefault(x => x.Position.Equals(rowPos));

                                // Get group index of current row
                                var groupIndex = rows[rowPos - 1].GroupIndex;

                                // Get row height
                                var rowHeight = newWorksheet.Row(rowPos).Height;

                                // Handle row addable
                                var additionGroupInfo = "";
                                if (groupIndex > 0)
                                {
                                    additionGroupInfo = $" groupIndex=\"{groupIndex}\"";

                                    // Check next row is expanded or not
                                    if (rowPos < rows.Count)
                                    {
                                        var nextRow = rows[rowPos];
                                        if (nextRow.IsExpanded)
                                            additionGroupInfo = " type=\"added\"";
                                    }
                                }

                                // Create new TR HTML Tag
                                sb.AppendLine($"<tr{additionGroupInfo}>");

                                // Begin process data column by column
                                for (int colPos = dimension.Start.Column; colPos <= totalCol; colPos++)
                                {
                                    // Get cell by row and col index
                                    using (var cell = newWorksheet.Cells[rowPos, colPos])
                                    {
                                        // Get cell item from rows storage
                                        var cellItem = rowItem.Cells.FirstOrDefault(x => x.Position.Equals(colPos));

                                        var content = cell.Text;
                                        var classes = cellItem.Classes;
                                        var styles = cellItem.Styles;
                                        var colSpan = "";
                                        var rowSpan = "";

                                        var cellAddressPosition = $"{rowPos}-{colPos}";

                                        // Get cell width
                                        var colWidth = newWorksheet.Column(colPos).Width;

                                        // Styles
                                        styles += $" height: {rowHeight * 1.5}px; width: {colWidth * 7.2}px;";

                                        // Handle replace function to content
                                        {
                                            // Handle set user form value
                                            {
                                                while (content.TryGetFunction("SET_VALUE", out var function, out var functionContent))
                                                {
                                                    var strParams = functionContent.Split(',');

                                                    if (strParams.Length == 2)
                                                    {
                                                        var key = strParams[0].Trim();
                                                        var value = strParams[1].Trim();

                                                        // Insert or update only when UserForm is not null and not step view
                                                        if (userForm != null && userForm.CurrentStep.Claims != "View")
                                                        {
                                                            await _userFormRepository.SetValue(userForm.Id, key, value);
                                                        }
                                                    }

                                                    // Remove this function
                                                    content = content.Replace(function, "");
                                                }
                                            }

                                            // Handle link function
                                            {
                                                while (content.TryGetFunction("LINK", out var function, out var functionContent))
                                                {
                                                    var strParams = functionContent.Split(',');

                                                    if (strParams.Length >= 3)
                                                    {
                                                        var text = strParams[0].Trim();
                                                        var actionType = strParams[1].Trim();
                                                        var actionId = strParams[2].Trim();

                                                        // Build link content
                                                        switch (actionType.MakeLowerCase())
                                                        {
                                                            case "form":
                                                                content = content.Replace(function, $"<a href=\"{Url.Action("Form", new { formId = actionId })}\" target=\"blank\">{text}</a>");
                                                                break;

                                                            default:
                                                                // Remove this function
                                                                content = content.Replace(function, "");
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // Remove this function
                                                        content = content.Replace(function, "");
                                                    }
                                                }
                                            }

                                            // Handle save range function
                                            {
                                                while (content.TryGetFunction("SET_VALUES_RANGE", out var function, out var functionContent))
                                                {
                                                    var strParams = functionContent.Split(',');
                                                    if (strParams.Length >= 2 && userForm != null && userForm.CurrentStep.Claims != "View")
                                                    {
                                                        var fromStr = strParams[0].Trim();
                                                        var destStr = strParams[1].Trim();

                                                        //
                                                        var fromAddress = fromStr.TrySplit(":");
                                                        var destAddress = destStr.TrySplit(":");

                                                        if (fromAddress.Length == 2 && destAddress.Length == 2)
                                                        {
                                                            var addressMap = new[]
                                                            {
                                                                    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                                                    "K",
                                                                    "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                                                                    "V",
                                                                    "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE",
                                                                    "AF",
                                                                    "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN",
                                                                    "AO",
                                                                    "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW",
                                                                    "AX",
                                                                    "AY", "AZ",
                                                                };

                                                            var listValues = new Dictionary<long, List<string>>();

                                                            for (long rIndex = fromAddress[0].TryParseToLong(); rIndex <= destAddress[0].TryParseToLong(); rIndex++)
                                                            {
                                                                var rowValues = new List<string>();

                                                                for (long cIndex = 1; cIndex <= addressMap.Length; cIndex++)
                                                                {
                                                                    // Check are in range
                                                                    if (cIndex >= fromAddress[1].TryParseToLong() &&
                                                                        cIndex <= destAddress[1].TryParseToLong())
                                                                    {
                                                                        var cellSelect = newWorksheet.GetValue((int)rIndex, (int)cIndex);
                                                                        rowValues.Add(cellSelect.ToString());
                                                                    }
                                                                    else
                                                                    {
                                                                        rowValues.Add("");
                                                                    }
                                                                }

                                                                // Insert to list values
                                                                listValues.Add(rIndex, rowValues);
                                                            }

                                                            // Insert to database in background
                                                            await Task.Run(() => _userFormValueStorage.InsertOrUpdateRowAsync(userForm.Id, listValues));
                                                        }
                                                    }

                                                    // Remove this function
                                                    content = content.Replace(function, "");
                                                }
                                            }
                                        }

                                        // Handle merge row
                                        if (cell.Merge)
                                        {
                                            var cellExcelAddress = new ExcelAddress(cell.Address);
                                            var mCellsResult = (from c in newWorksheet.MergedCells
                                                                let addr = new ExcelAddress(c)
                                                                where cellExcelAddress.Start.Row >= addr.Start.Row &&
                                                                      cellExcelAddress.End.Row <= addr.End.Row &&
                                                                      cellExcelAddress.Start.Column >= addr.Start.Column &&
                                                                      cellExcelAddress.End.Column <= addr.End.Column
                                                                select addr);

                                            if (mCellsResult.Any())
                                            {
                                                var mCells = mCellsResult.First();

                                                // Nếu cell hiện tại không trùng với Merged Cell Start thì không xuất thẻ <td>
                                                if (mCells.Start.Address != cellExcelAddress.Start.Address)
                                                    continue;

                                                // Handle col merged
                                                if (mCells.Start.Column != mCells.End.Column)
                                                {
                                                    colSpan = $" colSpan=\"{mCells.End.Column + 1 - mCells.Start.Column}\"";

                                                    // Handle style of last cell
                                                    var lastCell = rowItem.Cells.FirstOrDefault(x => x.Position.Equals(mCells.End.Column));
                                                    if (lastCell != null)
                                                    {
                                                        var innerClasses = lastCell.Classes.Split(' ');
                                                        foreach (var classItem in innerClasses)
                                                        {
                                                            if (classes.Contains(classItem)) continue;
                                                            classes += classItem;
                                                        }
                                                    }
                                                }

                                                // Handle row merged
                                                if (mCells.Start.Row != mCells.End.Row)
                                                {
                                                    rowSpan = $" rowSpan=\"{mCells.End.Row + 1 - mCells.Start.Row}\"";

                                                    // Handle style of last cell
                                                    var lastRow = rows.FirstOrDefault(x => x.Position.Equals(mCells.End.Row));
                                                    var lastCell = lastRow?.Cells.FirstOrDefault(x => x.Position.Equals(mCells.End.Column));
                                                    if (lastCell != null)
                                                    {
                                                        var innerClasses = lastCell.Classes.Split(' ');
                                                        foreach (var classItem in innerClasses)
                                                        {
                                                            if (classes.Contains(classItem)) continue;
                                                            classes += classItem;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (cellItem.Format == "format")
                                        {

                                        }

                                        // Handle percent
                                        if (cellItem.Format == "percent")
                                        {
                                            if (content.Trim().EndsWith("%"))
                                                content = (double.Parse(content.Replace("%", "")) / 100).ToString("0.##");
                                            //content = cell.Value.ToString();
                                        }

                                        // Build content HTML
                                        if (cellItem.CellType == CellType.Display)
                                        {
                                            if (content.Equals("#VALUE!")) content = "";

                                            if (!cellItem.IsMerge)
                                                content = $"<div style=\"width: {colWidth * 7.5}px;\">{content}</div>";
                                        }
                                        else if (cellItem.CellType == CellType.Editable)
                                        {
                                            // Build input content
                                            if (isIncludeInput)
                                            {
                                                var dataBase = "";

                                                if (rowItem.BaseOnRow > 0)
                                                {
                                                    dataBase = $"data-base=\"{rowItem.BaseOnRow + 1}-{colPos}\"";
                                                }

                                                if (cellItem.InputType == "message")
                                                {


                                                    content = $"<textarea {dataBase} data-address=\"{cellAddressPosition}\" name=\"{cellAddressPosition}\" class=\"fit-all no-border\" value=\"{cell.Value}\"></textarea>";

                                                }
                                                else if (cellItem.InputType == "list")
                                                {
                                                    var list = cellItem.InputList.Split(';');

                                                    content = $"<select  {dataBase}  class=\"fit-all no-border\"  data-address=\"{cellAddressPosition}\"  name=\"{cellAddressPosition}\" >";
                                                    foreach (string itm in list)
                                                    {

                                                        if (cell.Value.ToString() == itm)
                                                        {
                                                            content += $"<option selected='selected'  value=\"{itm}\" >{itm} </option>";
                                                        }
                                                        else
                                                        {
                                                            content += $"<option value=\"{itm}\" >{itm} </option>";

                                                        }
                                                    }


                                                    // content += $"<option value = \"2\" > 2 </option>";

                                                    content += $"</select>";
                                                }

                                                else
                                                {
                                                    content = $"<input type=\"text\" {dataBase} data-format=\"{cellItem.InputType}\" data-address=\"{cellAddressPosition}\" name=\"{cellAddressPosition}\" class=\"fit-all no-border\" value=\"{cell.Value}\" />";
                                                }


                                            }
                                            else
                                                content = cell.Value.ToString();

                                            // Handle td width
                                            if (!cellItem.IsMerge)
                                                content = $"<div style=\"width: {colWidth * 7.5}px;\">{content}</div>";
                                        }

                                        // Build TD HTML Tag
                                        sb.AppendLine($"\t<td{colSpan}{rowSpan} data-address=\"{cellAddressPosition}\" data-format=\"{cellItem.Format}\" class=\"{classes}\" style=\"{styles}\">{content}</td>");

                                    }   // End cell process

                                }   // End col process

                                // Close TR HTML Tag
                                sb.AppendLine("</tr>");

                            }   // End row process

                            return sb.ToString();
                        }
                    }

                }
            }
        }

        private string GetBranchNameByLevel(Employee empDetails)
        {
            if (empDetails != null)
            {
                if (!string.IsNullOrEmpty(empDetails.Level2Name)) return empDetails.Level2Name.Trim();
                if (!string.IsNullOrEmpty(empDetails.Level1Name)) return empDetails.Level1Name.Trim();
            }

            return "";
        }
        private string HandleToDay(string formula)
        {
            var splitText = formula.RemoveWhitespace().Split(new[] { "+", "-" }, StringSplitOptions.RemoveEmptyEntries);

            var operation = "+";

            if (formula.Contains("-")) operation = "-";

            if (splitText.Length == 2)
            {
                if (splitText[0].ToLower().Equals("today()"))
                    return DateTime.Now.AddDays(Convert.ToDouble(operation + splitText[1])).ToString("dd/MM/yyyy");

                return DateTime.Now.AddDays(Convert.ToDouble(operation + splitText[0])).ToString("dd/MM/yyyy");
            }

            if (formula.MakeLowerCase().Equals("today()"))
                return DateTime.Now.ToString("dd/MM/yyyy");

            return formula;
        }

        private async Task<List<FormStepDetailsViewModel>> BuildMilestoneTimeline(UserForm userForm, IList<FormStep> steps, int currentStepIndex)
        {
            var result = new List<FormStepDetailsViewModel>();

            // Get UserFormLogs
            List<UserFormLog> userFormLogs = null;
            if (userForm != null)
                userFormLogs = await _userFormRepository.GetLogsAsync(userForm.Id);

            foreach (var formStep in steps)
            {
                var stepItem = new FormStepDetailsViewModel()
                {
                    StepId = formStep.Id,
                    Name = formStep.Name,
                    Description = formStep.Description,
                    Index = formStep.Index,
                    Claims = formStep.Claims,

                    IsActive = formStep.Index <= currentStepIndex,
                    IsCurrent = formStep.Index == currentStepIndex,
                };

                var availableTime = formStep.AvailableFrom.GetValueOrDefault();
                var expireTime = formStep.ExpireIn.GetValueOrDefault();

                // Get addition info
                if (formStep.Claims.Equals(FormStepActionType.Edit.ToString()))
                {
                    if (userForm != null)
                    {
                        // Trường hợp current step đang ở bước Edit
                        if (userForm.CurrentStepId.Equals(formStep.Id))
                        {
                            expireTime = userForm.ExpireIn.GetValueOrDefault(expireTime);

                            stepItem.InfoTitle = "Thời gian kết thúc";
                            stepItem.InfoDescription = expireTime.ToLocalTime().ToLocalString();
                        }
                        else // Trường hợp current step đang ở bước khác
                        {
                            if (userFormLogs?.Count > 0)
                            {
                                // Get logs để lấy thời gian gửi gần nhất
                                var firstLog = userFormLogs.FirstOrDefault(x => x.Action.Equals(UserFormLogActionType.Submit.ToString()));
                                if (firstLog != null)
                                {
                                    //stepItem.InfoTitle = "Đã gửi biểu mẫu";
                                    stepItem.InfoTitle = firstLog.Message;
                                    stepItem.InfoDescription = firstLog.DateCreated.ToLocalTime().ToLocalString();
                                }
                            }
                        }

                    }
                    else
                    {
                        stepItem.InfoTitle = "Thời gian kết thúc";
                        stepItem.InfoDescription = expireTime.ToLocalTime().ToLocalString();
                    }
                }
                else if (formStep.Claims.Equals(FormStepActionType.Confirm.ToString()))
                {
                    if (userForm != null)
                    {
                        // Trường hợp current step đang ở bước Confirm
                        if (userForm.CurrentStepId.Equals(formStep.Id))
                        {
                            expireTime = userForm.ExpireIn.GetValueOrDefault(expireTime);

                            stepItem.InfoTitle = "Thời gian kết thúc";
                            stepItem.InfoDescription = expireTime.ToLocalTime().ToLocalString();
                        }
                        else // Trường hợp current step đang ở bước khác
                        {
                            if (userFormLogs?.Count > 0)
                            {
                                // Get logs để lấy thời gian gửi gần nhất
                                var firstLog = userFormLogs.FirstOrDefault(x => (x.Action.Equals(UserFormLogActionType.Confirm.ToString()) || x.Action.Equals(UserFormLogActionType.Decline.ToString())) && x.StepId.Equals(formStep.Id));
                                if (firstLog != null)
                                {
                                    //stepItem.InfoTitle = "Đã duyệt";
                                    stepItem.InfoTitle = firstLog.Message;

                                    // Lấy thông tin người duyệt
                                    //var empDetails = await _employeeRepository.FindByEmpCodeAsync(firstLog.AuthorEmpCode);

                                    //if (empDetails != null)
                                    //{
                                    //    stepItem.InfoTitle = empDetails.FullName + " đã duyệt";
                                    //}
                                    //else
                                    //{
                                    //    _log.LogError($"Error cant found details of empcode {firstLog.AuthorEmpCode}.");
                                    //}

                                    stepItem.InfoDescription = firstLog.DateCreated.ToLocalTime().ToLocalString();
                                }
                            }
                        }

                    }
                }
                else if (formStep.Claims.Equals(FormStepActionType.View.ToString()))
                {
                    if (userForm != null)
                    {
                        // Trường hợp current step đang ở bước View
                        if (userForm.CurrentStepId.Equals(formStep.Id))
                        {
                            expireTime = userForm.ExpireIn.GetValueOrDefault(expireTime);

                            stepItem.InfoTitle = "";
                        }

                    }
                }

                result.Add(stepItem);
            }

            return result;
        }

        [HttpGet("{formId}/get_users_submit/{formDataId}")]
        public async Task<DataResponse> GetUsersSubmit(string formId, long formDataId)
        {
            GrantPermissionResult result;

            if (formDataId == -1)
                result = await _formStepRepository.GetPermissionAsync(formId, GetCurrentUserId());
            else
            {
                // Load form data

                var formData = await _userFormRepository.Queryable(x => x.Id.Equals(formDataId), 0, 100).Include(x => x.CurrentStep).FirstOrDefaultAsync();
                if (formData == null && formDataId > 0) return BadRequest();

                result = await _formStepRepository.GetPermissionAsync(formId, GetCurrentUserId(), formData);
            }

            //if (result.IsGrant)
            //{
            //    if (result.ActionType == FormStepActionType.Edit || result.ActionType == FormStepActionType.Confirm)
            //    {
            //        // Get next step
            //        if (!string.IsNullOrEmpty(result.CurrentStep.NextStepId))
            //        {
            //            var nextStep = result.Steps.FirstOrDefault(x => x.Id.Equals(result.CurrentStep.NextStepId));
            //            if (nextStep != null)
            //            {
            //                // Check next step id Finish or not
            //                if (nextStep.Claims.Equals(FormStepActionType.View.ToString()))
            //                    return BadRequest();

            //                // Get all user in group and level2Id
            //                var users = await _employeeRepository.GetAllByGroupIdsAndEmpCodeAsync(nextStep.GroupIds, GetCurrentUserEmpCode(), await _customProperty.GetCustomFormPropertyAsync(formId, "KYDG"));
            //                if (users.Count > 0)
            //                {
            //                    // Handle next step is duplicate
            //                    var chk_user = users.FirstOrDefault(x => x.EmpCode.Equals(GetCurrentUserEmpCode()));
            //                    if (/*users.FirstOrDefault(x => x.EmpCode.Equals(GetCurrentUserEmpCode()))*/ chk_user == null)
            //                    {
            //                        return Success(users);
            //                    }
            //                    else
            //                    {
            //                        //21-01-2019 hoangvm, Dat lại tên cho user neu co trong list, để biêt có quyền duyệt bước tiếp theo
            //                        //users.Remove(chk_user);
            //                        chk_user.FullName = "Duyệt các bước tiếp theo";
            //                        chk_user.Title = "Chỉ chọn nếu bạn có quyền ở bước tiếp theo";
            //                        return Success(users);

            //                        //return BadRequest("Hide choose user model and direct accept form.", 400.2);
            //                    }
            //                }

            //                return BadRequest("Lỗi không tìm thấy bất kỳ nhân viên nào để thực hiện công việc tiếp theo.", 400.1);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return Success();
            //    }

            //}

            return Success();
            //return BadRequest();
        }        

        private async Task<IList<Employee>> GetManagersAsync(string empCode)
        {
            return await _employeeRepository.GetManagersOfEmpCodeAsync(empCode);
        }

        [HttpGet("{formId}/confirm")]
        public async Task<IActionResult> Contact(string formId)
        {
            var vm = new FormViewModel()
            {
                FormId = formId
            };

            var form = await _formRepository.FindByIdAsync(formId);
            if (form != null)
            {
                var userForm = await _userFormRepository.GetFormDataAsync(formId, GetCurrentUserId());
                var steps = await _formStepRepository.GetFromStepsByFormIdAsync(formId);
                var confirmStep = steps?.FirstOrDefault(x => x.Claims.Equals(FormStepActionType.Confirm.ToString()) && x.Confirm == -1);

                // Check permission for editable
                var result = await _formStepRepository.IsGrantPermissionForThisStepAsync(confirmStep, GetCurrentUserEmpCode(), userForm);

                //// Check form view permission
                //if (result != null && !result.IsGrant)
                //{
                //    result.IsGrant = _formRepository.IsGrantPermissionForView(form, result.EmployeeDetails);
                //}
                result.IsGrant = true;

                if (result != null && result.IsGrant)
                {
                    // Passed
                    var folderDataPath = Path.Combine(_environment.ContentRootPath, "MyFolder");
                    var fileFormPath = Path.Combine(folderDataPath, $"{formId}.{form.FileType}");

                    if (System.IO.File.Exists(fileFormPath))
                    {
                        //vm.IsAllowEditable = result.IsAllow;
                        vm.IsAllowEditable = true;

                        var htmlForm = await ProcessExcelToHtmlAsync(fileFormPath, formId, form.SheetIndex, result.UserForm, GetCurrentUserEmpCode(), vm.IsAllowEditable);

                        vm.Steps = steps;
                        //vm.CurrentStepIndex = result.CurrentStep != null ? steps.IndexOf(result.CurrentStep) + 1 : 1;
                        vm.CurrentStepIndex = confirmStep.Index;
                        vm.FormHtmlContent = htmlForm;
                        vm.Name = form.Name;
                        vm.Description = form.Description;
                        vm.StepsDetails = new List<FormStepDetailsViewModel>();

                        vm.StepsDetails = await BuildMilestoneTimeline(userForm, steps, vm.CurrentStepIndex);

                        return View(vm);
                    }
                    else
                    {
                        //_log.LogError("Error cant not found Form file in UserData folder.");
                    }
                }
            }

            ModelState.AddModelError("form", "Lỗi không tìm thấy biểu mẫu, vui lòng liên hệ bộ phận hỗ trợ.");
            vm.Name = "Có lỗi xảy ra";
            return View(vm);
        }

        [HttpGet("{formId}")]
        public async Task<IList<Comment>> GetCommentByFormID(string formId)
        {
            var temp = await _commentRepository.GetCommentByFormID(formId);
            foreach (var item in temp)
            {
                if(!item.UserID.Equals(GetCurrentUserId()))
                {
                    item.UserID = "";
                }
            }
            return await _commentRepository.GetCommentByFormID(formId);
        }

    }
}
