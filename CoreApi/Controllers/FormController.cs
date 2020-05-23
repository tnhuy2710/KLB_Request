using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreApi.Commons;
using CoreApi.Data.Repositories;
using CoreApi.Enums;
using CoreApi.Extensions;
using CoreApi.Models;
using CoreApi.Models.Excel;
using CoreApi.Security;
using CoreApi.Services;
using CoreApi.Utilities;
using CoreApi.ViewModels.FormViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
#pragma warning disable 4014

namespace CoreApi.Controllers
{
    [Authorize]
    [Route("kpi")]
    public class FormController : BaseController
    {
        private readonly IHostingEnvironment _environment;
        private readonly IFormRepository _formRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFormStepRepository _formStep;
        private readonly IUserFormRepository _userFormData;
        private readonly IUserFormStepValuesRepository _userFormStepData;
        private readonly IFormValueRepository _formValues;
        private readonly ICustomPropertyRepository _customProperty;
        private readonly IUserFormValueStorageRepository _userFormValueStorage;
        private readonly IMessageSender _messageSender;

        private readonly ILogger<FormController> _log;
        private readonly IGlReportRepository _glReport;
        private readonly IKlbService _klbService;

        public FormController(IHostingEnvironment environment, IFormRepository formRepository, IGlReportRepository glReport, IUserRepository userRepository, IEmployeeRepository employeeRepository, IFormStepRepository formPermission, IUserFormRepository userFormData, IUserFormStepValuesRepository userFormStepData, ICustomPropertyRepository customProperty, IMessageSender messageSender, ILogger<FormController> log, IFormValueRepository formValues, IUserFormValueStorageRepository userFormValueStorage, IKlbService klbService)
        {
            _environment = environment;
            _formRepository = formRepository;
            _glReport = glReport;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _formStep = formPermission;
            _userFormData = userFormData;
            _userFormStepData = userFormStepData;
            _customProperty = customProperty;
            _messageSender = messageSender;
            _log = log;
            _formValues = formValues;
            _userFormValueStorage = userFormValueStorage;
            _klbService = klbService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (Request.Query.TryGetValue("action", out var value))
            {
                if (!string.IsNullOrEmpty(value) && value.Equals("add"))
                    return View();

                if (!string.IsNullOrEmpty(value) && value.Equals("update_time"))
                {
                    // Load all form
                    var forms = await _formRepository.GetAllAsync();
                    if (forms?.Count > 0)
                    {
                        foreach (var form in forms)
                        {
                            // Get steps 
                            var steps = await _formStep.GetFromStepsByFormIdAsync(form.Id);
                            if (steps?.Count > 0)
                            {
                                DateTimeOffset timeExpireLastStep = new DateTimeOffset(DateTime.Today);

                                foreach (var step in steps)
                                {
                                    // Update times
                                    step.AvailableFrom = timeExpireLastStep;
                                    step.ExpireIn = step.AvailableFrom.Value.AddDays(30);

                                    // Update to database
                                    await _formStep.UpdateAsync(step, false);

                                    // Update last time
                                    timeExpireLastStep = step.ExpireIn.Value;
                                }
                            }
                        }

                        Debug.WriteLine("Update time for all from success.");
                        return Ok(new { message = "Your action is done." });
                    }
                }

                if (!string.IsNullOrEmpty(value) && value.Equals("update_logs"))
                {
                    if (await _userFormData.UpdateLogsAsync())
                        return Ok(new { message = "Your action is done." });
                }
                if (!string.IsNullOrEmpty(value) && value.Equals("update_avatars"))
                {
                    // Get all user
                    var employees = await _employeeRepository.GetAllAsync();

                    if (employees?.Count > 0)
                    {
                        var tasks = new List<Task>();

                        foreach (var employee in employees)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                var defaultPhoto = await _klbService.GetEmployeeImageBytesByFileNameAsync($"{employee.EmpId}.jpg");
                                if (!(defaultPhoto?.Length > 0))
                                {
                                    defaultPhoto = Convert.FromBase64String(AppContants.DefaultBase64Image);

                                }

                                var imageOriginal = ImageUtils.ConvertByteArrayToBase64Image(defaultPhoto, 800);
                                var imageSize512 = ImageUtils.ConvertByteArrayToBase64Image(defaultPhoto, 512);
                                var imageSize125 = ImageUtils.ConvertByteArrayToBase64Image(defaultPhoto, 125);

                                await _employeeRepository.UpdateEmployeeAvatarImageAsync(employee.EmpCode,
                                    $"{employee.EmpId}.jpg", imageOriginal, imageSize512, imageSize125);
                            }));
                        }

                        await Task.WhenAll(tasks);
                    }

                    return Ok(new { message = "Your action is done." });
                }


                if (!string.IsNullOrEmpty(value) && value.Equals("update_col"))
                {
                    // Load all userForms by id
                    var userForms = await _userFormData.GetFormsDataAsync("ff7bf0cf-efd0-4dac-bdfa-faafbe91895f");
                    if (userForms?.Count > 0)
                    {
                        foreach (var userForm in userForms)
                        {
                            // Filter form by step
                            if (!userForm.CurrentStepId.Equals("0928c29a-b380-4a28-a15c-749a7dcbfccd")) continue;

                            // Parse value to object
                            try
                            {
                                var cellsSubmitted = await Task.Run(() => JsonConvert.DeserializeObject<List<CellSubmit>>(userForm.InputValues));
                                foreach (var cellSubmit in cellsSubmitted)
                                {
                                    if (!string.IsNullOrEmpty(cellSubmit.Address))
                                    {
                                        var strAddress = cellSubmit.Address.TrySplit("-");
                                        if (strAddress.Length == 2)
                                        {
                                            var newAddress = $"{strAddress[0]}-{strAddress[1].TryParseToLong() + 1}";

                                            // Update Address
                                            cellSubmit.Address = newAddress;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(cellSubmit.Base))
                                    {
                                        var strAddress = cellSubmit.Base.TrySplit("-");
                                        if (strAddress.Length == 2)
                                        {
                                            var newAddress = $"{strAddress[0]}-{strAddress[1].TryParseToLong() + 1}";

                                            // Update Address
                                            cellSubmit.Base = newAddress;
                                        }
                                    }
                                }

                                // Update to database
                                var strNewCellSubmit = JsonConvert.SerializeObject(cellsSubmitted);
                                userForm.InputValues = strNewCellSubmit;

                                //
                                await _userFormData.UpdateAsync(userForm);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Error cant parse values to object: " + e.Message);
                            }

                        }
                    }

                    return Ok(new { message = "Your action is done." });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // View

        [HttpGet("{formId}")]
        public async Task<IActionResult> Form(string formId)
        {
            var vm = new FormViewModel()
            {
                FormId = formId
            };

            var form = await _formRepository.FindByIdAsync(formId);
            if (form != null)
            {
                var userForm = await _userFormData.GetFormDataAsync(formId, GetCurrentUserId());
                var steps = await _formStep.GetFromStepsByFormIdAsync(formId);
                var editStep = steps?.FirstOrDefault(x => x.Claims.Equals(FormStepActionType.Edit.ToString()));

                // Check permission for editable
                var result = await _formStep.IsGrantPermissionForThisStepAsync(editStep, GetCurrentUserEmpCode(), userForm);

                // Check form view permission
                if (result != null && !result.IsGrant && !string.IsNullOrEmpty(form.ViewPermissions))
                {
                    result.IsGrant = _formRepository.IsGrantPermissionForView(form, result.EmployeeDetails);
                }

                if (result != null && result.IsGrant)
                {
                    // Passed
                    var folderDataPath = Path.Combine(_environment.ContentRootPath, "UserData/Forms");
                    var fileFormPath = Path.Combine(folderDataPath, $"{formId}.{form.FileType}");

                    if (System.IO.File.Exists(fileFormPath))
                    {
                        vm.IsAllowEditable = result.IsAllow;

                        var htmlForm = await ProcessExcelToHtmlAsync(fileFormPath, formId, form.SheetIndex, result.UserForm, GetCurrentUserEmpCode(), vm.IsAllowEditable);

                        vm.Steps = steps;
                        vm.CurrentStepIndex = result.CurrentStep != null ? steps.IndexOf(result.CurrentStep) + 1 : 1;
                        vm.FormHtmlContent = htmlForm;
                        vm.Name = form.Name;
                        vm.Description = form.Description;
                        vm.StepsDetails = new List<FormStepDetailsViewModel>();

                        vm.StepsDetails = await BuildMilestoneTimeline(userForm, steps, vm.CurrentStepIndex);

                        return View(vm);
                    }
                    else
                    {
                        _log.LogError("Error cant not found Form file in UserData folder.");
                    }
                }
            }

            ModelState.AddModelError("form", "Lỗi không tìm thấy biểu mẫu, vui lòng liên hệ bộ phận hỗ trợ.");
            vm.Name = "Có lỗi xảy ra";
            return View(vm);
        }

        [HttpGet("{formId}/view")]
        public async Task<IActionResult> FormViews(string formId)
        {
            var formDetails = await _formRepository.FindByIdAsync(formId);
            if (formDetails == null) return RedirectToAction("Index", "Home");

            var vm = new FormConfirmViewModel()
            {
                Id = formDetails.Id,
                Name = formDetails.Name,
                Description = formDetails.Description,
                StartDate = formDetails.PublishDate,
                CloseDate = formDetails.CloseDate,
            };

            var empDetails = await _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode(), await _customProperty.GetCustomFormPropertyAsync(formId,"KYDG"));
            if (empDetails == null)
            {
                ModelState.AddModelError("error", "Lỗi không tìm thấy thông tin nhân sự tài theo tài khoản của bạn.");
                return View(vm);
            }

            var formResult = _formRepository.IsGrantPermissionForView(formDetails, empDetails);

            var userForms = await _userFormData.GetFormsDataAsync(formId);
            if (userForms?.Count > 0)
            {
                var tasks = new List<Task>();

                // Passed
                foreach (var userForm in userForms)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var authorDetails = await _employeeRepository.FindByEmpCodeAsync(userForm.User.EmpCode, await _customProperty.GetCustomFormPropertyAsync(formId, "KYDG"));
                        if (authorDetails != null)
                        {
                            if (!formResult)
                            {
                                var result = _formStep.IsGrantPermissionForView(userForm.CurrentStep, empDetails);
                                if (!result.IsGrant)
                                {
                                    return;
                                }
                            }

                            var item = new FormConfirmItemViewModel()
                            {
                                Id = formId,
                                AuthorId = userForm.User.EmpCode,
                                AuthorName = authorDetails.FullName,
                                AuthorTitle = authorDetails.Title,
                                AuthorLevelName = authorDetails.GetLevelName()
                            };

                            vm.Forms.Add(item);
                        }
                    }));

                }

                // Wait all task finish
                await Task.WhenAll(tasks);
            }

            return View(vm);
        }

        [HttpGet("{formId}/view/{empCode}")]
        public async Task<IActionResult> FormViewDetails(string formId, string empCode)
        {
            var vm = new FormViewModel()
            {
                FormId = formId
            };

            var form = await _formRepository.FindByIdAsync(formId);
            if (form != null)
            {
                // Find User by EmpCode
                var targetUser = await _userRepository.FindByEmpCodeAsync(empCode);
                if (targetUser == null) return RedirectToAction("Index", "Home");

                // Prepare data
                var userForm = await _userFormData.GetFormDataAsync(formId, targetUser.Id);
                var steps = await _formStep.GetFromStepsByFormIdAsync(formId);
                var empDetails = await _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode(), await _customProperty.GetCustomFormPropertyAsync(formId, "KYDG"));

                // Check Null
                if (steps?.Count < 1 || empDetails == null)
                    return RedirectToAction("Index", "Home");

                //
                if (userForm?.CurrentStep == null)
                {
                    ModelState.AddModelError("form", "Bạn không thể xem biễu mẫu này do người đang thực hiện công việc Đánh giá/Đăng ký/Xét duyệt chưa hoàn tất.");
                    vm.Name = "Có lỗi xảy ra";
                    return View("Form", vm);
                }

                // Check grant permission for view
                var result = _formStep.IsGrantPermissionForView(userForm.CurrentStep, empDetails);

                // Check form view permission
                if (!_formRepository.IsGrantPermissionForView(form, empDetails) && !result.IsGrant)
                {
                    ModelState.AddModelError("form", "Bạn không thể xem biễu mẫu này do người đang thực hiện công việc Đánh giá/Đăng ký/Xét duyệt chưa hoàn tất hoặc bạn không được cấp quyền để xem Biểu Mẫu này.");
                    vm.Name = "Có lỗi xảy ra";
                    return View("Form", vm);
                }

                // Passed
                var folderDataPath = Path.Combine(_environment.ContentRootPath, "UserData/Forms");
                var fileFormPath = Path.Combine(folderDataPath, $"{formId}.{form.FileType}");

                if (System.IO.File.Exists(fileFormPath))
                {
                    vm.IsAllowEditable = false;

                    var htmlForm = await ProcessExcelToHtmlAsync(fileFormPath, formId, form.SheetIndex, userForm, empCode, vm.IsAllowEditable);

                    vm.Steps = steps;
                    vm.FormHtmlContent = htmlForm;
                    vm.CurrentStepIndex = userForm?.CurrentStep != null ? steps.IndexOf(userForm.CurrentStep) + 1 : 1;
                    vm.Name = form.Name;
                    vm.Description = form.Description;
                    vm.StepsDetails = new List<FormStepDetailsViewModel>();
                    vm.StepsDetails = await BuildMilestoneTimeline(userForm, steps, vm.CurrentStepIndex);

                    return View("Form", vm);
                }
                else
                {
                    _log.LogError($"Error cant not found Form file {form.Id} in UserData folder.");
                }
            }

            ModelState.AddModelError("form", "Lỗi không tìm thấy biểu mẫu, vui lòng liên hệ bộ phận hỗ trợ.");
            vm.Name = "Có lỗi xảy ra";
            return View("Form", vm);
        }

        [HttpPost("{formId}/savedraft")]
        public async Task<DataResponse> SaveDraft(string formId, [FromBody] List<CellSubmit> cellsData)
        {
            var result = await _formStep.IsGrantPermissionForThisFormAsync(formId, GetCurrentUserEmpCode());
            if (result.IsAllow && result.ActionType == FormStepActionType.Edit)
            {
                if (await _userFormData.SubmitFormAsync(formId, GetCurrentUserId(), result.CurrentStep.Id, cellsData))
                {
                    return Success();
                }

                return BadRequest("Lỗi không thể lưu dữ liệu người dùng.", 400.6);
            }

            return BadRequest("Lỗi không được cấp quyền để truy cập.", 400.3);
        }

        [HttpPost("{formId}")]
        public async Task<DataResponse> SubmitForm(string formId, [FromBody] FormSubmitViewModel model)
        {
            // Get employee details
            var employeeDetailsNext = await _employeeRepository.FindByEmpCodeAsync(model.EmpCodeNextStep);
            if (employeeDetailsNext == null)
                return BadRequest("Không tìm thấy thông tin của người thực hiện công việc tiếp theo.");

            // Check form and save form
            var result = await _formStep.IsGrantPermissionForThisFormAsync(formId, GetCurrentUserEmpCode());
            if (result.IsAllow && result.ActionType == FormStepActionType.Edit)
            {
                if (await _userFormData.SubmitFormAsync(formId, GetCurrentUserId(), result.CurrentStep.Id, model.Cells))
                {
                    //var steps = await _formStep.GetFromStepsByFormIdAsync(formId);
                    if (result.UserForm == null)
                        result.UserForm = await _userFormData.GetFormDataAsync(formId, GetCurrentUserId());

                    if (result.Steps?.Count > 0 && result.UserForm != null)
                    {
                        // Check permission
                        if (result.UserForm != null)
                        {
                            // lấy thông tin bước tiếp theo
                            var nextStep = result.Steps.FirstOrDefault(x => x.Id.Equals(result.CurrentStep.NextStepId));
                            if (nextStep != null)
                            {
                                // Check QLTT Group and get list managers
                                IList<Employee> acceptList = null;
                                if (nextStep.HaveQlttGroup())
                                {
                                    // Lấy danh sách quản lý của User hiện tại đang login để check cho next Step
                                    acceptList = await GetManagersAsync(GetCurrentUserEmpCode());
                                }

                                // Kiểm tra User được chọn có được cấp permission ở bước tiếp theo hay không
                                // Check next step have valid groupid with next user
                                var checkNextStepPermission = await _formStep.IsGrantPermissionForThisStepAsync(nextStep, employeeDetailsNext, result.UserForm, acceptList, isDeepCheck: false);
                                if (!checkNextStepPermission.IsGrant)
                                    return BadRequest("Lỗi không thể gửi biểu mẫu do có sai sót trong quá trình kiểm tra quyền hạn cấp quản lý của bạn.", 400.3);

                                // Add Logs
                                await _userFormData.AddLogsAsync(result.UserForm, UserFormLogActionType.Submit, GetCurrentUserEmpCode(), employeeDetailsNext.EmpCode, $"{result.UserForm.User.FullName} đã gửi biểu mẫu cho {employeeDetailsNext.FullName}");

                                // Update current step to user data
                                result.UserForm.CurrentStepId = nextStep.Id;
                                result.UserForm.AvailableFrom = DateTimeOffset.UtcNow;

                                // Update to database
                                await _userFormData.UpdateAsync(result.UserForm, false);

                                // Send email notitication to user was choiced
                                if (nextStep.IsAllowSendEmail)
                                {
                                    var currentFullName = User.FindFirst(x => x.Type.Equals(ClaimContants.Fullname)).Value;
                                    var currentTitle = User.FindFirst(x => x.Type.Equals(ClaimContants.Title)).Value;
                                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                                    var subject = $"{nextStep.Description} cho {currentFullName}";
                                    var message = _messageSender.GetEmailTemplate("Confirm");
                                    var email = employeeDetailsNext.Email;

                                    // Build template
                                    message = message.Replace("$TARGET.FULLNAME$", employeeDetailsNext.FullName);
                                    message = message.Replace("$TARGET.TITLE$", employeeDetailsNext.Title);

                                    message = message.Replace("$FULLNAME$", currentFullName);
                                    message = message.Replace("$TITLE$", currentTitle);
                                    message = message.Replace("$DESCRIPTION$", result.UserForm.CurrentStep.Description);
                                    message = message.Replace("$DATEFROM$", result.UserForm.DateCreated.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm:ss"));
                                    message = message.Replace("$DATEEND$", nextStep.ExpireIn?.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm:ss"));
                                    message = message.Replace("$LINK$", baseUrl + Url.Action("FormConfirm", new { formId = formId }));

                                    // handle custom property for email
                                    var customEmail = await _customProperty.GetCustomUserPropertyAsync(employeeDetailsNext.Username, "Email");
                                    if (customEmail != null) email = customEmail;

                                    if (!string.IsNullOrEmpty(message))
                                    {
                                        var resultSender = await _messageSender.SendEmailAsync(email, subject, message);
                                        if (!resultSender)
                                        {
                                            // Logger error cant send email
                                            Debug.WriteLine($"Error cant send new email to {employeeDetailsNext.Email}");
                                            _log.LogError($"Error cant send new email to {employeeDetailsNext.Email}");
                                        }
                                        else
                                            _log.LogInformation($"Send new email success to {employeeDetailsNext.Email}");
                                    }
                                    else
                                        _log.LogError($"Error cant send new email to {employeeDetailsNext.Email} because empty Message content.");
                                }

                                return Success($"Biểu mẫu của bạn đã được gửi <b>thành công</b> và đang chờ {nextStep.Name}. Ngoài ra bạn có thể theo dõi tiến trình ngay phía trên biểu mẫu.");
                            }
                        }
                    }
                }

                return BadRequest("Lỗi không thể lưu dữ liệu người dùng.", 400.6);
            }

            return BadRequest("Lỗi không được cấp quyền để truy cập.", 400.3);
        }

        //

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file?.Length > 0)
            {
                // full path to file in temp location
                var folderDataPath = Path.Combine(_environment.ContentRootPath, "UserData/Forms");

                // Handle folder exist
                if (!Directory.Exists(folderDataPath)) Directory.CreateDirectory(folderDataPath);

                // Get file type
                var fileType = Path.GetExtension(file.FileName)?.MakeLowerCase().Remove(0, 1);

                //
                var name = GetField("name");
                var description = GetField("description");
                var sheetIndex = int.Parse(GetField("sheetIndex"));

                // Create 
                var form = new Form()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Description = description,
                    SheetIndex = sheetIndex,
                    FileType = fileType,
                    DateCreated = DateTimeOffset.UtcNow
                };

                // Get Steps
                var listSteps = new List<FormStep>();
                for (int i = 1; i <= 4; i++)
                {

                    var check_step = GetField($"steps[{i}][name]");
                    if (!string.IsNullOrEmpty(check_step))
                    {

                        var step = new FormStep()
                        {

                            Id = Guid.NewGuid().ToString(),
                            Name = GetField($"steps[{i}][name]"),
                            Description = GetField($"steps[{i}][description]"),
                            FormId = form.Id,
                            IsAllowSendEmail = true,
                            Index = i,
                            GroupIds = GetField($"steps[{i}][groupIds]"),
                            Claims = GetField($"steps[{i}][claims]"),
                            DateCreated = DateTimeOffset.UtcNow,
                            DateUpdated = DateTimeOffset.UtcNow,
                            AvailableFrom = DateTime.ParseExact(GetField($"steps[{i}][startDate]"), "yyyy-MM-dd", new CultureInfo("en-US")).Date.ToUniversalTime(),
                            ExpireIn = DateTime.ParseExact(GetField($"steps[{i}][endDate]"), "yyyy-MM-dd", new CultureInfo("en-US")).Date.ToUniversalTime(),
                        };
                        listSteps.Add(step);




                        switch (i)
                        {
                            case 2:
                                listSteps[i - 2].NextStepId = step.Id;
                                break;

                            case 3:
                                listSteps[i - 2].NextStepId = step.Id;
                                listSteps[i - 2].PrevStepId = listSteps[i - 3].Id;
                                break;

                            case 4:
                                listSteps[i - 2].NextStepId = step.Id;
                                listSteps[i - 2].PrevStepId = listSteps[i - 3].Id;
                                break;
                        }
                    }
                }

                // Set date
                form.PublishDate = listSteps.FirstOrDefault().AvailableFrom.Value;
                form.CloseDate = listSteps.LastOrDefault().ExpireIn.Value;

                using (var fileStream = new FileStream(Path.Combine(folderDataPath, $"{form.Id}.{fileType}"), FileMode.Create))
                {
                    // Copy file first
                    await file.CopyToAsync(fileStream);

                    // Then insert to db
                    await _formRepository.InsertAsync(form);

                    // Insert steps
                    await _formStep.InsertAsync(listSteps);
                }

                return RedirectToAction("Index", new { test = true });
            }

            return RedirectToAction("Error", "Home");
        }

        // Confirm

        [HttpGet("{formId}/confirm")]
        public async Task<IActionResult> FormConfirm(string formId)
        {
            var vm = new FormConfirmViewModel();

            //
            var formDetails = await _formRepository.FindByIdAsync(formId);
            if (formDetails == null) return RedirectToAction("Index", "Home");

            vm.Id = formDetails.Id;
            vm.Name = formDetails.Name;
            vm.Description = formDetails.Description;
            vm.StartDate = formDetails.PublishDate;
            vm.CloseDate = formDetails.CloseDate;

            var empDetails = await _employeeRepository.FindByEmpCodeAsync(GetCurrentUserEmpCode());

            // Get all form assigned
            var formsAssigned = await _formStep.GetFormAssignedByEmpCodeAsync(GetCurrentUserEmpCode());
            if (formsAssigned?.Count > 0)
            {
                foreach (var userForm in formsAssigned)
                {
                    // Check current step
                    if (userForm.CurrentStep != null && userForm.FormId == formId)
                    {
                        //Chỉnh lại 21/05/2020
                        // Check duplicate
                        //if (vm.Forms.FirstOrDefault(x => x.AuthorId.Equals(userForm.UserId) && x.Id.Equals(userForm.FormId)) != null)
                        //    continue;

                        // Check Claim
                        if (!userForm.CurrentStep.Claims.Contains(FormStepActionType.Confirm.ToString())) continue;


                        // Check QLTT Group and get list managers
                        IList<Employee> acceptList = null;
                        if (userForm.CurrentStep.HaveQlttGroup())
                        {
                            var lastAssignd = await _userFormData.GetLastLogByUserFormAndStepAsync(userForm.Id, userForm.CurrentStep.PrevStepId);
                            acceptList = await GetManagersAsync(lastAssignd?.AuthorEmpCode);
                        }

                        var check = await _formStep.IsGrantPermissionForThisStepAsync(userForm.CurrentStep, empDetails, userForm, acceptList);
                        if (check != null && check.IsAllow)
                        {
                            //Chỉnh lại 21/05/2020
                            var targetUserDetails = await _userRepository.FindDetailsByIdAsync(userForm.FormId);
                            if (targetUserDetails != null)
                            {
                                var formItem = new FormConfirmItemViewModel()
                                {
                                    Id = userForm.FormId,
                                    AuthorId = targetUserDetails.Id,
                                    AuthorName = targetUserDetails.FULLNAME,
                                    AuthorTitle = targetUserDetails.TITLE,
                                    AuthorLevelName = targetUserDetails.GetLevelName()
                                };

                                //
                                vm.Forms.Add(formItem);
                            }
                        }
                    }
                }
            }

            return View(vm);
        }

        [HttpGet("{formId}/confirm/{userId}")]
        public async Task<IActionResult> FormConfirmDetails(string formId, string userId)
        {
            var vm = new FormItemDetailsViewModel();

            
            var data = await _userFormData
                .Queryable(x =>
                    x.FormId.Equals(formId) &&
                    x.UserId.Equals(userId)
                    )
                .Include(y => y.User)
                .Include(y => y.Form)
                .Include(y => y.CurrentStep)
                .FirstOrDefaultAsync();

            // Check data
            if (data == null)
            {
                ModelState.AddModelError("error", "Lỗi không thể tìm thấy thông tin của biểu mẫu này.");
                return View(vm);
            }

            // Load all steps data
            var steps = await _formStep.GetFromStepsByFormIdAsync(formId);
            if (!(steps?.Count > 0))
            {
                ModelState.AddModelError("error", "Lỗi không thể tìm thấy thông tin quy trình của biểu mẫu này.");
                return View(vm);
            }

            // Check is in Confirm mode
            if (!data.CurrentStep.Claims.Contains(FormStepActionType.Confirm.ToString()))
            {
                ModelState.AddModelError("error", "Lỗi không thể tìm thấy thông tin quy trình của biểu mẫu này.");
                return View(vm);
            }

            // Check QLTT Group and get list managers
            IList<Employee> acceptList = null;
            if (data.CurrentStep.HaveQlttGroup())
            {
                var lastAssignd = await _userFormData.GetLastLogByUserFormAndStepAsync(data.Id, data.CurrentStep.PrevStepId);
                acceptList = await GetManagersAsync(lastAssignd?.AuthorEmpCode);
            }

            // Check permission
            var check = await _formStep.IsGrantPermissionForThisStepAsync(data.CurrentStep, GetCurrentUserEmpCode(), data, acceptList);
            if (check.IsAllow)
            {
                // Check user info
                var userDetails = await _employeeRepository.FindByEmpCodeAsync(data.User.EmpCode);
                if (userDetails == null)
                {
                    ModelState.AddModelError("error", "Lỗi không thể tìm thấy thông tin của người gửi.");
                    return View(vm);
                }

                vm.Id = data.Id;
                vm.FormId = data.FormId;
                vm.Name = data.Form.Name;
                vm.Description = data.Form.Description;
                vm.SubmitDate = data.DateCreated;

                vm.AuthorId = data.UserId;
                vm.AuthorName = userDetails.FullName;
                vm.AuthorTitle = userDetails.Title;

                // Process excel file
                var folderDataPath = Path.Combine(_environment.ContentRootPath, "UserData/Forms");
                var fileFormPath = Path.Combine(folderDataPath, $"{formId}.{data.Form.FileType}");

                if (System.IO.File.Exists(fileFormPath))
                {
                    var htmlContent = await ProcessExcelToHtmlAsync(fileFormPath, formId, data.Form.SheetIndex, data, userDetails.EmpCode, false);
                    if (!string.IsNullOrEmpty(htmlContent))
                        vm.HtmlContent = htmlContent;

                    vm.StepsDetails = await BuildMilestoneTimeline(data, steps, data.CurrentStep.Index);

                    return View(vm);
                }
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpGet("{formId}/get_users_submit/{formDataId}")]
        public async Task<DataResponse> GetUsersSubmit(string formId, long formDataId)
        {
            GrantPermissionResult result;

            if (formDataId == -1)
                result = await _formStep.GetPermissionAsync(formId, GetCurrentUserId());
            else
            {
                // Load form data

                var formData = await _userFormData.Queryable(x => x.Id.Equals(formDataId), 0, 100).Include(x => x.CurrentStep).FirstOrDefaultAsync();
                if (formData == null && formDataId > 0) return BadRequest();

                result = await _formStep.GetPermissionAsync(formId, GetCurrentUserId(), formData);
            }

            if (result.IsGrant)
            {
                if (result.ActionType == FormStepActionType.Edit || result.ActionType == FormStepActionType.Confirm)
                {
                    // Get next step
                    if (!string.IsNullOrEmpty(result.CurrentStep.NextStepId))
                    {
                        var nextStep = result.Steps.FirstOrDefault(x => x.Id.Equals(result.CurrentStep.NextStepId));
                        if (nextStep != null)
                        {
                            // Check next step id Finish or not
                            if (nextStep.Claims.Equals(FormStepActionType.View.ToString()))
                                return BadRequest();

                            // Get all user in group and level2Id
                            var users = await _employeeRepository.GetAllByGroupIdsAndEmpCodeAsync(nextStep.GroupIds, GetCurrentUserEmpCode(), await _customProperty.GetCustomFormPropertyAsync(formId,"KYDG"));
                            if (users.Count > 0)
                            {
                                // Handle next step is duplicate
                                var chk_user = users.FirstOrDefault(x => x.EmpCode.Equals(GetCurrentUserEmpCode()));
                                if (/*users.FirstOrDefault(x => x.EmpCode.Equals(GetCurrentUserEmpCode()))*/ chk_user == null)
                                {
                                    return Success(users);
                                }
                                else
                                {
                                    //21-01-2019 hoangvm, Dat lại tên cho user neu co trong list, để biêt có quyền duyệt bước tiếp theo
                                    //users.Remove(chk_user);
                                    chk_user.FullName = "Duyệt các bước tiếp theo";
                                    chk_user.Title = "Chỉ chọn nếu bạn có quyền ở bước tiếp theo";
                                    return Success(users);

                                    //return BadRequest("Hide choose user model and direct accept form.", 400.2);
                                }
                            }

                            return BadRequest("Lỗi không tìm thấy bất kỳ nhân viên nào để thực hiện công việc tiếp theo.", 400.1);
                        }
                    }
                }
                else
                {
                    return Success();
                }

            }

            return BadRequest();
        }

        [HttpPost("{formId}/confirm/{userId}/accept")]
        public async Task<DataResponse> AcceptFormSubmit(string formId, string userId, [FromBody] AcceptFormSubmitViewModel viewModel)
        {
            var userForm = await _userFormData.GetFormDataAsync(formId, userId);
            if (userForm != null)
            {
                var steps = await _formStep.GetFromStepsByFormIdAsync(formId);
                if (steps?.Count > 0)
                {
                    reAction:

                    // Check QLTT Group and get list managers
                    IList<Employee> acceptList = null;
                    if (userForm.CurrentStep.HaveQlttGroup())
                    {
                        var lastAssignd = await _userFormData.GetLastLogByUserFormAndStepAsync(userForm.Id, userForm.CurrentStep.PrevStepId);
                        acceptList = await GetManagersAsync(lastAssignd?.AuthorEmpCode);
                    }

                    // Check permission cho phép thực hiện công việc Confirm hay không
                    var result = await _formStep.IsGrantPermissionForThisStepAsync(userForm.CurrentStep, GetCurrentUserEmpCode(), userForm, acceptList);
                    if (result.IsAllow)
                    {
                        // Get next step
                        if (!string.IsNullOrEmpty(userForm.CurrentStep.NextStepId))
                        {
                            var nextStep = steps.FirstOrDefault(x => x.Id.Equals(userForm.CurrentStep.NextStepId));
                            if (nextStep != null)
                            {
                                // Update userForm
                                userForm.AvailableFrom = DateTimeOffset.UtcNow;

                                // Cho phép thực hiện trong vòng 1 ngày nếu Step đã hết hạn
                                if (DateTimeOffset.UtcNow > userForm.CurrentStep.ExpireIn)
                                    userForm.ExpireIn = DateTimeOffset.UtcNow.AddDays(1);
                                else
                                    userForm.ExpireIn = null;

                                // Get current user info
                                var currentFullName = User.FindFirst(x => x.Type.Equals(ClaimContants.Fullname)).Value;
                                var currentTitle = User.FindFirst(x => x.Type.Equals(ClaimContants.Title)).Value;

                                // Next step still is confirm
                                if (nextStep.Claims.Equals(FormStepActionType.Confirm.ToString()))
                                {
                                    // Check viewmodel
                                    if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.EmpCodeChosen))
                                    {
                                        // Handle last
                                        if (viewModel.EmpCodeChosen.MakeLowerCase().Equals("last")) viewModel.EmpCodeChosen = GetCurrentUserEmpCode();

                                        // Get Next Step User Employee Details
                                        var employeeDetailsNext = await _employeeRepository.FindByEmpCodeAsync(viewModel.EmpCodeChosen);
                                        if (employeeDetailsNext == null) return BadRequest("Không tìm thấy thông tin của người thực hiện công việc tiếp theo.");

                                        // Check QLTT Group and get list managers
                                        acceptList = null;
                                        if (userForm.CurrentStep.HaveQlttGroup())
                                        {
                                            acceptList = await GetManagersAsync(GetCurrentUserEmpCode());
                                        }

                                        // Kiểm tra User được chọn có được cấp permission ở bước tiếp theo hay không
                                        // Check next step have valid groupid with next user
                                        var checkNextStepPermission = await _formStep.IsGrantPermissionForThisStepAsync(nextStep, employeeDetailsNext, acceptList: acceptList);
                                        if (!checkNextStepPermission.IsGrant)
                                            return BadRequest("Lỗi không thể gửi biểu mẫu do có sai sót trong quá trình kiểm tra quyền hạn cấp quản lý của bạn.", 400.3);

                                        // Add Logs
                                        await _userFormData.AddLogsAsync(result.UserForm, UserFormLogActionType.Confirm, GetCurrentUserEmpCode(), employeeDetailsNext.EmpCode, $"{currentFullName} đã duyệt biểu mẫu này và chuyển tiếp cho {employeeDetailsNext.FullName}");

                                        // Update userForm
                                        userForm.CurrentStepId = nextStep.Id;

                                        // Update user form to database
                                        await _userFormData.UpdateAsync(result.UserForm, false);

                                        // Check next step if duplicate confirm user
                                        var nextStepUsers = await _employeeRepository.GetAllByGroupIdsAndEmpCodeAsync(nextStep.GroupIds, GetCurrentUserEmpCode(), await _customProperty.GetCustomFormPropertyAsync(formId, "KYDG"));
                                        if (nextStepUsers?.FirstOrDefault(x => x.EmpCode.Equals(GetCurrentUserEmpCode())) != null)
                                        {
                                            // Update userForm
                                            userForm = await _userFormData.GetFormDataAsync(formId, userId);
                                            if (userForm != null)
                                            {
                                                goto reAction;
                                            }
                                        }

                                        // Send email notification
                                        if (nextStep.IsAllowSendEmail)
                                        {
                                            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                                            var subject = $"{nextStep.Description} cho {currentFullName}";
                                            var message = _messageSender.GetEmailTemplate("Confirm");
                                            var email = employeeDetailsNext.Email;

                                            // Build template
                                            message = message.Replace("$TARGET.FULLNAME$", employeeDetailsNext.FullName);
                                            message = message.Replace("$TARGET.TITLE$", employeeDetailsNext.Title);

                                            message = message.Replace("$FULLNAME$", currentFullName);
                                            message = message.Replace("$TITLE$", currentTitle);
                                            message = message.Replace("$DESCRIPTION$", result.UserForm.CurrentStep.Description);
                                            message = message.Replace("$DATEFROM$", result.UserForm.DateCreated.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm"));
                                            message = message.Replace("$DATEEND$", nextStep.ExpireIn?.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm"));
                                            message = message.Replace("$LINK$", baseUrl + Url.Action("FormConfirm", new { formId = formId }));

                                            // handle custom property for email
                                            var customEmail = await _customProperty.GetCustomUserPropertyAsync(employeeDetailsNext.Username, "Email");
                                            if (customEmail != null) email = customEmail;

                                            if (!string.IsNullOrEmpty(message))
                                            {
                                                var resultSender = await _messageSender.SendEmailAsync(email, subject, message);
                                                if (!resultSender)
                                                {
                                                    // Logger error cant send email
                                                    Debug.WriteLine($"Error cant send new email to {employeeDetailsNext.Email}");
                                                    _log.LogError($"Error cant send new email to {employeeDetailsNext.Email}");
                                                }
                                                else
                                                    _log.LogInformation($"Send new email success to {employeeDetailsNext.Email}");
                                            }
                                            else
                                                _log.LogError($"Error cant send new email to {employeeDetailsNext.Email} because empty Message content.");
                                        }

                                        return Success($"Biểu mẫu đã được <b>Duyệt thành công</b> và đang chờ {nextStep.Name}. Ngoài ra bạn có thể theo dõi tiến trình ngay phía trên biểu mẫu.");
                                    }

                                    return BadRequest();
                                }
                                else if (nextStep.Claims.Equals(FormStepActionType.View.ToString()))
                                {
                                    // Go to end of all step, it just output report

                                    // Add log
                                    await _userFormData.AddLogsAsync(userForm, UserFormLogActionType.Confirm, GetCurrentUserEmpCode(), null, $"{currentFullName} đã duyệt biểu mẫu này");

                                    // Update userForm
                                    userForm.CurrentStepId = nextStep.Id;

                                    // Update user form data
                                    if (await _userFormData.UpdateAsync(userForm) == null)
                                        return BadRequest("Lỗi không thể cập nhật thông tin.", 400.1);

                                    // Send email notification
                                    if (nextStep.IsAllowSendEmail)
                                    {

                                    }

                                    return Success("Biểu mẫu đã được <b>Duyệt thành công</b>.");
                                }

                                return BadRequest("Có lỗi xảy ra trong quá trình xử lý", 400.11);
                            }
                        }
                        else
                        {
                            // End of process
                        }
                    }

                    return BadRequest("Bạn không được cấp phép để thực hiện công việc này.", 400.10);
                }
            }

            return BadRequest();
        }

        [HttpPost("{formId}/confirm/{userId}/decline")]
        public async Task<DataResponse> DeclineFormSubmit(string formId, string userId, [FromBody] DeclineFormSubmitViewModel viewModel)
        {
            var userForm = await _userFormData.GetFormDataAsync(formId, userId);
            if (userForm != null && ModelState.IsValid)
            {
                var steps = await _formStep.GetFromStepsByFormIdAsync(formId);
                if (steps?.Count > 0)
                {
                    reAction:

                    // Check QLTT Group and get list managers
                    IList<Employee> acceptList = null;
                    if (userForm.CurrentStep.HaveQlttGroup())
                    {
                        var lastAssignd = await _userFormData.GetLastLogByUserFormAndStepAsync(userForm.Id, userForm.CurrentStep.PrevStepId);
                        acceptList = await GetManagersAsync(lastAssignd?.AuthorEmpCode);
                    }

                    // Check permission cho phép thực hiện công việc Confirm hay không
                    var result = await _formStep.IsGrantPermissionForThisStepAsync(userForm.CurrentStep, GetCurrentUserEmpCode(), userForm, acceptList);
                    if (result.IsAllow)
                    {
                        if (!string.IsNullOrEmpty(userForm.CurrentStep.PrevStepId))
                        {
                            var currentFullName = User.FindFirst(x => x.Type.Equals(ClaimContants.Fullname)).Value;

                            var prevStep = steps.FirstOrDefault(x => x.Id.Equals(userForm.CurrentStep.PrevStepId));
                            if (prevStep != null)
                            {
                                // Add log
                                await _userFormData.AddLogsAsync(userForm, UserFormLogActionType.Decline, GetCurrentUserEmpCode(), "", $"{currentFullName} đã từ chối duyệt với lý do: {viewModel.Message}");

                                // Update userForm
                                userForm.CurrentStepId = prevStep.Id;
                                userForm.AvailableFrom = DateTimeOffset.UtcNow;

                                // Cho phép chỉnh sửa trong vòng 1 ngày nếu Step đã hết hạn
                                if (DateTimeOffset.UtcNow > userForm.CurrentStep.ExpireIn)
                                    userForm.ExpireIn = DateTimeOffset.UtcNow.AddDays(1);
                                else
                                    userForm.ExpireIn = null;

                                // Update UserForm data
                                if (await _userFormData.UpdateAsync(userForm) == null)
                                    return BadRequest("Lỗi không thể cập nhật thông tin.", 400.1);

                                // Area for handle duplicate users when Decline
                                {
                                    var empCodeNeedToGetPrevUsers = GetCurrentUserEmpCode();

                                    // Prev Step of Prev Step
                                    if (!string.IsNullOrEmpty(prevStep.PrevStepId))
                                    {
                                        var nextPrevStep = steps.FirstOrDefault(x => x.Id.Equals(prevStep.PrevStepId));
                                        if (nextPrevStep != null)
                                        {
                                            // Get log of this step for find Author Code
                                            var lastLog = await _userFormData.GetLastLogByUserFormAndStepAsync(userForm.Id, nextPrevStep.Id);
                                            if (lastLog != null) empCodeNeedToGetPrevUsers = lastLog.AuthorEmpCode;
                                        }
                                    }

                                    // Check prev step is duplicate user
                                    var nextStepUsers = await _employeeRepository.GetAllByGroupIdsAndEmpCodeAsync(prevStep.GroupIds, empCodeNeedToGetPrevUsers, await _customProperty.GetCustomFormPropertyAsync(formId, "KYDG"));
                                    if (nextStepUsers?.FirstOrDefault(x => x.EmpCode.Equals(GetCurrentUserEmpCode())) != null)
                                    {
                                        // Update userForm
                                        userForm = await _userFormData.GetFormDataAsync(formId, userId);
                                        if (userForm != null)
                                        {
                                            goto reAction;
                                        }
                                    }
                                }

                                // Handle get time
                                var availableTime = prevStep.AvailableFrom.GetValueOrDefault(DateTimeOffset.UtcNow);
                                var expireTime = prevStep.ExpireIn.GetValueOrDefault(DateTimeOffset.UtcNow);

                                if (userForm.AvailableFrom.HasValue) availableTime = userForm.AvailableFrom.Value;
                                if (userForm.ExpireIn.HasValue) expireTime = userForm.ExpireIn.Value;

                                // Prev step still is confirm
                                if (prevStep.Claims.Equals(FormStepActionType.Confirm.ToString()))
                                {
                                    // Lấy mã nhân sự đã assign ở step trước
                                    var lastLog = await _userFormData.GetLastLogByUserFormAndStepAsync(userForm.Id, prevStep.Id);
                                    if (lastLog != null)
                                    {
                                        var empCode = lastLog.AuthorEmpCode;

                                        // Send email notification
                                        if (prevStep.IsAllowSendEmail)
                                        {
                                            // Lấy thông tin user ở step trước
                                            var empPrevStep = await _employeeRepository.FindByEmpCodeAsync(empCode);
                                            if (empPrevStep != null)
                                            {
                                                currentFullName = User.FindFirst(x => x.Type.Equals(ClaimContants.Fullname)).Value;
                                                var currentTitle = User.FindFirst(x => x.Type.Equals(ClaimContants.Title)).Value;
                                                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                                                var subject = $"Biểu mẫu {prevStep.Form.Description} của bạn không được duyệt";
                                                var message = _messageSender.GetEmailTemplate("Decline");
                                                var email = userForm.User.Email;

                                                // Build template

                                                message = message.Replace("$TARGET.FULLNAME$", empPrevStep.FullName);
                                                message = message.Replace("$TARGET.TITLE$", empPrevStep.Title);

                                                message = message.Replace("$FULLNAME$", currentFullName);
                                                message = message.Replace("$TITLE$", currentTitle);
                                                message = message.Replace("$DESCRIPTION$", userForm.CurrentStep.Description);
                                                message = message.Replace("$DATEFROM$", availableTime.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm"));
                                                message = message.Replace("$DATEEND$", expireTime.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm"));
                                                message = message.Replace("$LINK$", baseUrl + Url.Action("Form", new { formId = formId }));
                                                message = message.Replace("$MESSAGE$", viewModel.Message);

                                                // handle custom property for email
                                                var customEmail = await _customProperty.GetCustomUserPropertyAsync(userForm.User.UserName, "Email");
                                                if (customEmail != null) email = customEmail;

                                                var resultSender = await _messageSender.SendEmailAsync(email, subject, message);

                                                if (!resultSender)
                                                {
                                                    // Logger error cant send email
                                                    Debug.WriteLine($"Error cant send new email to {userForm.User.Email}");
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (prevStep.Claims.Equals(FormStepActionType.Edit.ToString()))
                                {
                                    // Go to end of all step, it just output report
                                    if (prevStep.IsAllowSendEmail)
                                    {
                                        currentFullName = User.FindFirst(x => x.Type.Equals(ClaimContants.Fullname)).Value;
                                        var currentTitle = User.FindFirst(x => x.Type.Equals(ClaimContants.Title)).Value;
                                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                                        var subject = $"Biểu mẫu {prevStep.Form.Description} của bạn không được duyệt";
                                        var message = _messageSender.GetEmailTemplate("Decline");
                                        var email = userForm.User.Email;

                                        // Build template

                                        message = message.Replace("$TARGET.FULLNAME$", userForm.User.FullName);
                                        message = message.Replace("$TARGET.TITLE$", userForm.User.Title);

                                        message = message.Replace("$FULLNAME$", currentFullName);
                                        message = message.Replace("$TITLE$", currentTitle);
                                        message = message.Replace("$DESCRIPTION$", userForm.CurrentStep.Description);
                                        message = message.Replace("$DATEFROM$", availableTime.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm"));
                                        message = message.Replace("$DATEEND$", expireTime.ToLocalTime().ToString("'ngày' dd/MM/yyyy 'lúc' HH:mm"));
                                        message = message.Replace("$LINK$", baseUrl + Url.Action("Form", new { formId = formId }));
                                        message = message.Replace("$MESSAGE$", viewModel.Message);

                                        // handle custom property for email
                                        var customEmail = await _customProperty.GetCustomUserPropertyAsync(userForm.User.UserName, "Email");
                                        if (customEmail != null) email = customEmail;

                                        var resultSender = await _messageSender.SendEmailAsync(email, subject, message);

                                        if (!resultSender)
                                        {
                                            // Logger error cant send email
                                            Debug.WriteLine($"Error cant send new email to {userForm.User.Email}");
                                        }
                                    }
                                }

                                return Success();
                            }
                        }
                        else
                        {
                            // End of process

                            return BadRequest("Có lỗi xảy ra, vui lòng thử lại lần nữa.", 400.5);
                        }
                    }

                    return BadRequest("Bạn không được cấp phép để thực hiện công việc này.", 400.5);
                }
            }

            return BadRequest("Có lỗi xảy ra, vui lòng kiểm tra lại đường dẫn");
        }

        /// <summary>
        /// Process excel file to HTML and auto combine with user data.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="userForm"></param>
        /// <param name="targetEmpCode"></param>
        /// <param name="isIncludeInput"></param>
        /// <returns></returns>
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
                    var cellsSubmitted = new List<CellSubmit>();
                    if (userForm != null)
                    {
                        try
                        {
                            cellsSubmitted = await Task.Run(() => JsonConvert.DeserializeObject<List<CellSubmit>>(userForm.InputValues));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"Error while parse object from UserData: {e.Message}");
                            throw;
                        }

                        // Try get user form values
                        userFormValues = await _userFormData.GetValuesByIdAsync(userForm.Id);
                    }

                    // Get User Details 
                    Employee userDetails = await _employeeRepository.FindByEmpCodeAsync(targetEmpCode, await _customProperty.GetCustomFormPropertyAsync(formId, "KYDG"));
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
                                                        var values = KyDG==""? await _glReport.GetKqKpiCtv(date, branchId): await _glReport.GetKqKpiCtv(date, branchId,KyDG);

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
                                    if (row.GroupIndex > 0)
                                    {
                                        // Get all row base on target row
                                        var additionRows = cellsSubmitted.Where(x => !string.IsNullOrEmpty(x.Base) && x.Base.StartsWith($"{row.Position}-"))
                                            .GroupBy(x => x.Address.Substring(0, x.Address.IndexOf("-", StringComparison.Ordinal)))
                                            .ToList();

                                        if (additionRows?.Count > 0)
                                        {
                                            var rowIndex = 0;

                                            foreach (var additionRow in additionRows)
                                            {
                                                // Clone new row
                                                var newRow = row.Clone();
                                                newRow.IsExpanded = true;
                                                newRow.Position += rowIndex + 1;
                                                newRow.OriginalPosition = row.Position;
                                                newRow.Cells?.Clear();
                                                newRow.BaseOnRow = rowPos;

                                                // Load cells submitted of current row
                                                var additionCells = additionRow.ToList();

                                                // Load cells
                                                foreach (var rowCell in row.Cells)
                                                {
                                                    // Get cell from submitted
                                                    var cell = additionCells.FirstOrDefault(x => x.Address.EndsWith($"-{rowCell.Position}"));

                                                    var cellContent = "";

                                                    if (cell != null)
                                                    {
                                                        cellContent = cell.Value;
                                                    }

                                                    // Clone cell
                                                    var newCell = new Cell()
                                                    {
                                                        Address = $"{additionRow.Key}-{rowCell.Position}",
                                                        Content = cellContent,
                                                        Position = rowCell.Position,
                                                        CellType = rowCell.CellType,
                                                        IsMerge = rowCell.IsMerge,
                                                        Classes = rowCell.Classes,
                                                        Styles = rowCell.Styles,
                                                        InputType = rowCell.InputType,
                                                        Formula = rowCell.Formula,
                                                        Format = rowCell.Format
                                                    };

                                                    // Add cell to row
                                                    newRow.Cells.Add(newCell);
                                                }

                                                //
                                                rows.Insert(row.Position + rowIndex, newRow);

                                                // Clone row for worksheet
                                                var index = row.OriginalPosition + rowIndex++ + 1;
                                                newWorksheet.InsertRow(index, 1, row.OriginalPosition);
                                            }
                                        }
                                    }

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
                                            var cellSubmitted = cellsSubmitted.FirstOrDefault(x => x.Address.Equals(cellAddressPosition));

                                            // Set value is submitted by user
                                            if (cellSubmitted != null) inputValue = cellSubmitted.Value;

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
                                                        if (userForm != null && userForm.CurrentStep.Claims!="View")
                                                        {
                                                            await _userFormData.SetValue(userForm.Id, key, value);
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
                                                            Task.Run(() => _userFormValueStorage.InsertOrUpdateRowAsync(userForm.Id, listValues));
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

        /// <summary>
        /// Process to save form
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="cellsData"></param>
        /// <returns></returns>
        private async Task<DataResponse> SaveForm(string formId, List<CellSubmit> cellsData)
        {
            if (cellsData.Count > 0)
            {
                var result = await _formStep.IsGrantPermissionForThisFormAsync(formId, GetCurrentUserEmpCode());
                if (result.IsAllow && result.ActionType == FormStepActionType.Edit)
                {
                    if (await _userFormData.SubmitFormAsync(formId, GetCurrentUserId(), result.CurrentStep.Id, cellsData))
                    {
                        return Success();
                    }

                    return BadRequest("Lỗi không thể lưu dữ liệu người dùng.", 400.6);
                }

                return BadRequest("Lỗi không được cấp quyền để truy cập.", 400.3);
            }

            return BadRequest();
        }

        //

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

        private string GetBranchNameByLevel(Employee empDetails)
        {
            if (empDetails != null)
            {
                if (!string.IsNullOrEmpty(empDetails.Level2Name)) return empDetails.Level2Name.Trim();
                if (!string.IsNullOrEmpty(empDetails.Level1Name)) return empDetails.Level1Name.Trim();
            }

            return "";
        }

        private async Task<List<FormStepDetailsViewModel>> BuildMilestoneTimeline(UserForm userForm, IList<FormStep> steps, int currentStepIndex)
        {
            var result = new List<FormStepDetailsViewModel>();

            // Get UserFormLogs
            List<UserFormLog> userFormLogs = null;
            if (userForm != null)
                userFormLogs = await _userFormData.GetLogsAsync(userForm.Id);

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

        //

        private async Task<IList<Employee>> GetManagersAsync(string empCode)
        {
            return await _employeeRepository.GetManagersOfEmpCodeAsync(empCode);
        }
    }
}
