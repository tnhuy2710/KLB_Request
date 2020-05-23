using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using CoreApi.Commons;
using CoreApi.Data.Repositories.Base;
using CoreApi.Enums;
using CoreApi.Extensions;
using CoreApi.Models;
using CoreApi.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreApi.Data.Repositories
{
    public interface IFormStepRepository : IRepository<FormStep, string>
    {
        Task<IList<FormStep>> GetFromStepsByFormIdAsync(string formId);
        Task<IList<FormStep>> GetFromStepsByEmpCodeAsync(string empCode);
        Task<IList<FormStep>> GetFromStepsByEmpCodeAsync(Employee empDetails);

        Task<IList<FormStep>> GetViewPermissionByEmpCodeAsync(Employee empDetails);


        Task<IList<FormStep>> GetFormStepEditableByGroupIdAsync(string groupId);
        Task<IList<UserForm>> GetFormAssignedByEmpCodeAsync(string empCode);

        Task<IList<Employee>> GetManagersByStepIdAsync(string stepId, IList<FormStep> steps = null);

        Task<bool> AssignUserToFormAsync(string userId, long userFormId, string stepId);
        Task<bool> AssignUserToFormByEmpCodeAsync(string empCode, long userFormId, string stepId);
        Task<bool> UnAssignUserToFormByEmpCodeAsync(string empCode, long userFormId);

        Task<IList<UserFormAssign>> GetAllAssignAsync(long userFormId, string stepId);

        /// <summary>
        /// Check và lấy Permission theo UserId và FormId
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<GrantPermissionResult> GetPermissionAsync(string formId, string userId);

        /// <summary>
        /// Check và lấy Permission theo UserId, FormId đối với UserForm đã xác định (trường hợp này dành cho Confirm)
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="userId"></param>
        /// <param name="userForm"></param>
        /// <returns></returns>
        Task<GrantPermissionResult> GetPermissionAsync(string formId, string userId, UserForm userForm);

        //

        Task<GrantPermissionResult> IsGrantPermissionForThisFormAsync(string formId, string empCode, UserForm userForm = null);

        //

        Task<GrantPermissionResult> IsGrantPermissionForThisStepAsync(string stepId, string empCode, UserForm userForm = null, IList<Employee> acceptList = null, bool isDeepCheck = true);
        Task<GrantPermissionResult> IsGrantPermissionForThisStepAsync(FormStep step, string empCode, UserForm userForm = null, IList<Employee> acceptList = null, bool isDeepCheck = true);
        Task<GrantPermissionResult> IsGrantPermissionForThisStepAsync(FormStep step, Employee employeeDetails, UserForm userForm = null, IList<Employee> acceptList = null, bool isDeepCheck = true);

        //

        GrantPermissionResult IsGrantPermissionForView(FormStep step, Employee employeeDetails);
        Task<IList<Form>> GetAllGrantPermissionForViewAsync(Employee empDetails);

        Task<bool> Insert_Into(FormStep _formStep);

        Task<FormStep> UpdateFormSteps(string ID);
    }

    public class FormStepRepository : BaseRepository<FormStep, string>, IFormStepRepository
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserFormRepository _userFormRepository;
        private readonly ILogger<FormStepRepository> _log;

        public FormStepRepository(IHttpContextAccessor httpContext, ApplicationDbContext db, IEmployeeRepository employeeRepository, IUserFormRepository userFormRepository, ILogger<FormStepRepository> log) : base(httpContext, db)
        {
            _employeeRepository = employeeRepository;
            _userFormRepository = userFormRepository;
            _log = log;
        }

        public async Task<IList<FormStep>> GetFromStepsByFormIdAsync(string formId)
        {
            return await Db.FormSteps.Where(x => x.FormId.Equals(formId)).OrderBy(x => x.Index).ToListAsync();
        }

        public async Task<IList<FormStep>> GetFromStepsByEmpCodeAsync(string empCode)
        {
            var empDetails = await _employeeRepository.FindByEmpCodeAsync(empCode);
            return await GetFromStepsByEmpCodeAsync(empDetails);
        }

        public async Task<IList<FormStep>> GetFromStepsByEmpCodeAsync(Employee empDetails)
        {
            var listItems = new List<FormStep>();
            if (empDetails != null)
            {
                // Get all from by group, level
                //var formsByGroup = await Db.FormSteps
                //    .Where(x => x.GroupIds.Contains("|" + empDetails.PositionCode + "|"))
                //    .Include(y => y.Form)
                //    .ToListAsync();
                //if (formsByGroup?.Count > 0)
                //{
                //    foreach (var formStep in formsByGroup)
                //    {
                //        var groupIds = formStep.GroupIds.TrySplit(";");
                //        if (groupIds.Length > 0)
                //        {
                //            foreach (var groupId in groupIds)
                //            {
                //                var groupEles = groupId.Split('|');

                //                var eEmpCode = groupEles[0];
                //                var eGroupId = groupEles[1];
                //                var eLevel1Id = groupEles[2];
                //                var eLevel2Id = groupEles[3];

                //                if (!string.IsNullOrEmpty(eGroupId))
                //                {
                //                    if (string.IsNullOrEmpty(eLevel1Id))
                //                    {
                //                        listItems.Add(formStep);
                //                        goto nextForm;
                //                    }
                //                    else
                //                    {
                //                        if (empDetails.Level1Id.Equals(eLevel1Id) &&
                //                            empDetails.Level2Id.Equals(eLevel2Id))
                //                        {
                //                            listItems.Add(formStep);
                //                            goto nextForm;
                //                        }
                //                    }
                //                }
                //            }
                //        }

                //        nextForm:;
                //    }
                //}

                // Get all form by empCode
                //var formByEmpCode = await Db.FormSteps
                //    .Where(x => x.GroupIds.Contains(empDetails.EmpCode + "|"))
                //    .Include(y => y.Form)
                //    .ToListAsync();                
                //if (formByEmpCode?.Count > 0)
                //{
                //    foreach (var _formStep in formByEmpCode)
                //    {
                //        var groupIds = _formStep.GroupIds.TrySplit(";");
                //        if (groupIds.Length > 0)
                //        {
                //            foreach (var groupId in groupIds)
                //            {
                //                var groupEles = groupId.Split('|');
                //                try
                //                {
                //                    var eEmpCode = groupEles[0];
                //                    var eGroupId = groupEles[1];
                //                    var eLevel1Id = groupEles[2];
                //                    var eLevel2Id = groupEles[3];

                //                    if (!string.IsNullOrEmpty(eEmpCode))
                //                    {
                //                        listItems.Add(_formStep);
                //                        goto nextForm;
                //                    }
                //                }
                //                catch { }
                //            }
                //        }

                //        nextForm:;
                //    }
                //}

                var formByEmpCode = await Db.FormSteps
                    .Where(x => x.GroupIds.Contains(empDetails.EmpCode + ",false") && x.Confirm == -1)
                    .Include(y => y.Form)
                    .ToListAsync();
                if (formByEmpCode?.Count > 0)
                {                    
                    foreach (var _formStep in formByEmpCode)
                    {
                        if(await Db.UserForms.Where(x => x.CurrentStepId.Equals(_formStep.Id) && String.IsNullOrEmpty(x.ExpireIn.ToString()))
                                             .FirstOrDefaultAsync() != null)
                        {
                            listItems.Add(_formStep);
                        }                                       
                    }
                }
            }

            return listItems;
        }

        public async Task<IList<FormStep>> GetFormStepEditableByGroupIdAsync(string groupId)
        {
            return null;
        }

        public async Task<IList<UserForm>> GetFormAssignedByEmpCodeAsync(string empCode)
        {
            var formAssignsQueryable = Db.UserFormLogs.Where(x => x.TargetEmpCode.Equals(empCode));
            foreach (var userFormLog in formAssignsQueryable)
            {
                userFormLog.UserForm = Db.UserForms.Where(x => x.Id.Equals(userFormLog.UserFormId)).Include(y => y.CurrentStep).FirstOrDefault();
                userFormLog.Step = Db.FormSteps.FirstOrDefault(x => x.Id.Equals(userFormLog.StepId));
                if (userFormLog.UserForm != null)
                    userFormLog.UserForm.Form = Db.Forms.FirstOrDefault(x => x.Id.Equals(userFormLog.UserForm.FormId));
            }

            var formAssigns = await formAssignsQueryable.ToListAsync();
            formAssigns = formAssigns.Where(x => x.UserForm != null).ToList();

            return formAssigns.Select(x => x.UserForm).ToList();

            //var formAssigns = await Db.UserFormAssigns
            //    .Where(x => x.EmpCode.Equals(empCode))

            //    .Include(x => x.UserForm)
            //    .ThenInclude(y => y.CurrentStep)
            //    .ThenInclude(y => y.Form)

            //    .Include(x => x.UserForm)
            //    .ThenInclude(y => y.User)

            //    .ToListAsync();

            //return formAssigns.Select(x => x.UserForm).ToList();
        }

        public async Task<IList<Employee>> GetManagersByStepIdAsync(string stepId, IList<FormStep> steps = null)
        {
            if (steps == null)
            {
                var stepDetails = await FindByIdAsync(stepId);
                if (stepDetails != null)
                {
                    if (stepDetails.Claims.Equals(FormStepActionType.Edit.ToString()) ||
                        stepDetails.Claims.Equals(FormStepActionType.Confirm.ToString()))
                    {
                        steps = await GetFromStepsByFormIdAsync(stepDetails.FormId);
                    }
                }
            }

            if (steps?.Count > 0)
            {

            }

            return null;
        }


        public async Task<bool> AssignUserToFormAsync(string userId, long userFormId, string stepId)
        {
            var user = await Db.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (user != null)
            {
                return await AssignUserToFormByEmpCodeAsync(user.EmpCode, userFormId, stepId);
            }
            return false;
        }

        public async Task<bool> AssignUserToFormByEmpCodeAsync(string empCode, long userFormId, string stepId)
        {
            var itemCheck = await Db.UserFormAssigns.FirstOrDefaultAsync(x => x.EmpCode.Equals(empCode) && x.UserFormId.Equals(userFormId));
            if (itemCheck == null)
            {
                itemCheck = new UserFormAssign()
                {
                    EmpCode = empCode,
                    UserFormId = userFormId,
                    StepId = stepId,
                    DateCreated = DateTimeOffset.UtcNow
                };

                Db.UserFormAssigns.Add(itemCheck);

                return await Db.SaveChangesAsync() > 0;
            }
            else
            {
                itemCheck.EmpCode = empCode;
                Db.UserFormAssigns.Update(itemCheck);

                return await Db.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UnAssignUserToFormByEmpCodeAsync(string empCode, long userFormId)
        {
            var itemCheck = await Db.UserFormAssigns.FirstOrDefaultAsync(x => x.EmpCode.Equals(empCode) && x.UserFormId.Equals(userFormId));
            if (itemCheck != null)
            {
                Db.UserFormAssigns.Remove(itemCheck);
                return await Db.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<IList<UserFormAssign>> GetAllAssignAsync(long userFormId, string stepId)
        {
            return await Db.UserFormAssigns.Where(x =>
                x.UserFormId.Equals(userFormId) &&
                x.StepId.Equals(stepId))
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();
        }

        public async Task<GrantPermissionResult> GetPermissionAsync(string formId, string userId)
        {

            // Lấy các thông tin cần thiết
            var steps = await GetFromStepsByFormIdAsync(formId);                        // Các bước quy trình
            var user = await Db.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));    // Thông tin User đang logged in

            // Create result
            var result = new GrantPermissionResult()
            {
                IsGrant = false,
                IsAllow = false,
                User = user,
                Steps = steps
            };

            // Check valid data
            if (steps?.Count > 0 && user != null)
            {
                FormStep stepSelect = null;

                var userFormData = await Db.UserForms.FirstOrDefaultAsync(x => x.FormId.Equals(formId) && x.UserId.Equals(userId));
                if (userFormData == null)
                {
                    // Trường hợp chưa có dữ liệu nhập từ người dùng, tức chỉ mới đang ở Step đầu tiên
                    // Với trường hợp này chỉ check xem User có trong GroupIds đầu tiên hay không

                    var firstStep = steps.FirstOrDefault();

                    // Set Step select
                    stepSelect = firstStep;
                }
                else
                {
                    // Trường hợp này đã có dữ liệu từ người dùng
                    // Tức vẫn đang ở trạng thái Editable, chưa qua Confirm

                    stepSelect = userFormData.CurrentStep ?? throw new Exception("CurrentStep is null");
                    result.UserForm = userFormData;
                }

                // Check group and time
                if (stepSelect != null)
                {
                    // Set current step
                    result.CurrentStep = stepSelect;

                    // Check QLTT Group and get list managers
                    IList<Employee> acceptList = null;
                    if (stepSelect.HaveQlttGroup() && userFormData != null)
                    {
                        var lastAssignd = await _userFormRepository.GetLastLogByUserFormAndStepAsync(userFormData.Id, userFormData.CurrentStep.PrevStepId);
                        acceptList = await _employeeRepository.GetManagersOfEmpCodeAsync(lastAssignd?.AuthorEmpCode);
                    }

                    // Check user are in group or not
                    var check = await IsGrantPermissionForThisStepAsync(stepSelect, user.EmpCode, userFormData, acceptList, isDeepCheck: false);
                    if (check != null)
                    {
                        result.ActionType = check.ActionType;
                        result.IsAllow = check.IsAllow;
                        result.IsGrant = check.IsGrant;
                    }
                }
            }

            return result;
        }

        public async Task<GrantPermissionResult> GetPermissionAsync(string formId, string userId, UserForm userForm)
        {
            // Lấy các thông tin cần thiết
            var steps = await GetFromStepsByFormIdAsync(formId);                        // Các bước quy trình
            var user = await Db.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));    // Thông tin User đang logged in

            // Create result
            var result = new GrantPermissionResult()
            {
                IsGrant = false,
                IsAllow = false,
                User = user,
                Steps = steps
            };

            // Check valid data
            if (steps?.Count > 0 && user != null)
            {
                if (userForm != null)
                {
                    if (userForm.CurrentStep == null) throw new Exception("CurrentStep is null");

                    result.UserForm = userForm;
                    result.CurrentStep = userForm.CurrentStep;

                    // Trường hợp này đã có dữ liệu từ người dùng
                    // Bước này kiểm tra User có đang trong groupIds của current step hay không

                    // Check QLTT Group and get list managers
                    IList<Employee> acceptList = null;
                    if (userForm.CurrentStep.HaveQlttGroup() && userForm != null)
                    {
                        var lastAssignd = await _userFormRepository.GetLastLogByUserFormAndStepAsync(userForm.Id, userForm.CurrentStep.PrevStepId);
                        acceptList = await _employeeRepository.GetManagersOfEmpCodeAsync(lastAssignd?.AuthorEmpCode);
                    }

                    // Check user are in group or not
                    var check = await IsGrantPermissionForThisStepAsync(userForm.CurrentStep, user.EmpCode, userForm, acceptList, isDeepCheck: false);
                    if (check != null)
                    {
                        result.ActionType = check.ActionType;
                        result.IsAllow = check.IsAllow;
                        result.IsGrant = check.IsGrant;
                    }
                }
                else
                    throw new Exception("UserForm is null");
            }

            return result;
        }

        //

        public async Task<GrantPermissionResult> IsGrantPermissionForThisFormAsync(string formId, string empCode, UserForm userForm = null)
        {
            var steps = await GetFromStepsByFormIdAsync(formId);                                // Các bước quy trình
            var user = await Db.Users.FirstOrDefaultAsync(x => x.EmpCode.Equals(empCode));     // Thông tin User được chọn

            var result = new GrantPermissionResult();

            if (steps?.Count > 0 && user != null)
            {
                result.Steps = steps;
                result.User = user;

                var empDetail = await _employeeRepository.FindByEmpCodeAsync(empCode);
                if (empDetail != null)
                {
                    result.EmployeeDetails = empDetail;

                    // Kiểm tra có được cấp quyền hay không
                    foreach (var step in steps)
                    {
                        var check = await IsGrantPermissionForThisStepAsync(step, empDetail);
                        if (check != null)
                        {
                            if (check.IsGrant && !result.IsGrant)
                            {
                                result.ActionType = check.ActionType;
                                result.IsGrant = true;
                            }
                            if (check.IsAllow) result.IsAllow = true;
                        }
                    }

                    // Kiểm tra có cho phép thực thi công việc đã được cấp quyền hay không
                    // Check current step
                    if (userForm?.CurrentStep != null)
                    {
                        result.CurrentStep = userForm.CurrentStep;
                        result.UserForm = userForm;

                        result.ActionType = (FormStepActionType)Enum.Parse(typeof(FormStepActionType), userForm.CurrentStep.Claims);

                        var check = await IsGrantPermissionForThisStepAsync(userForm.CurrentStep, empDetail, userForm);
                        if (check != null)
                        {
                            if (check.IsAllow)
                            {
                                result.ActionType = check.ActionType;
                                result.IsAllow = true;
                            }
                            else
                            {
                                result.IsAllow = false;
                            }
                        }
                    }
                    else
                    {
                        result.CurrentStep = steps.FirstOrDefault();
                    }
                }
            }

            return result;
        }

        //

        public async Task<GrantPermissionResult> IsGrantPermissionForThisStepAsync(string stepId, string empCode, UserForm userForm = null, IList<Employee> acceptList = null, bool isDeepCheck = true)
        {
            var step = await Db.FormSteps
                .Where(x => x.Id.Equals(stepId))
                .Include(y => y.Form)
                .FirstOrDefaultAsync();

            if (stepId != null)
            {
                return await IsGrantPermissionForThisStepAsync(step, empCode, userForm, acceptList, isDeepCheck);
            }

            return null;
        }

        public async Task<GrantPermissionResult> IsGrantPermissionForThisStepAsync(FormStep step, string empCode, UserForm userForm = null, IList<Employee> acceptList = null, bool isDeepCheck = true)
        {
            var empDetails = await _employeeRepository.FindByEmpCodeAsync(empCode);
            if (empDetails != null)
                return await IsGrantPermissionForThisStepAsync(step, empDetails, userForm, acceptList, isDeepCheck);

            return null;
        }

        public async Task<GrantPermissionResult> IsGrantPermissionForThisStepAsync(FormStep step, Employee employeeDetails, UserForm userForm = null, IList<Employee> acceptList = null, bool isDeepCheck = true)
        {
            var result = new GrantPermissionResult()
            {
                CurrentStep = step,
                EmployeeDetails = employeeDetails,
                UserForm = userForm
            };

            // Tách và lấy danh sách các groups
            var groupIds = step?.GroupIds.TrySplit(";");

            if (groupIds?.Length > 0)
            {
                foreach (var id in groupIds)
                {
                    // Phân tách từng group để lấy thông tin theo pattern
                    // EmpCode|PosCode|Level1Id|Level2Id

                    var elements = id.Split('|');

                    if (elements.Length == 4)
                    {
                        var eEmpCode = elements[0];
                        var ePosCode = elements[1];
                        var eLevel1Id = elements[2];
                        var eLevel2Id = elements[3];

                        // Set action type
                        result.ActionType = (FormStepActionType)Enum.Parse(typeof(FormStepActionType), step.Claims);

                        // Kiếm tra mã Emp trước
                        if (eEmpCode.Equals(employeeDetails.EmpCode))
                        {
                            result.IsGrant = true;
                        }
                        else if (!string.IsNullOrEmpty(ePosCode))     // Check theo mã position code
                        {
                            // Check if group is QLTT
                            if (ePosCode.Equals(AppContants.QlttGroupCode))
                            {
                                if (acceptList?.Count > 0)
                                {
                                    if (acceptList.FirstOrDefault(x => x.EmpCode.Equals(employeeDetails.EmpCode)) != null)
                                        result.IsGrant = true;
                                }
                            }
                            else if (employeeDetails.PositionCode.MakeLowerCase().Equals(ePosCode.MakeLowerCase())) // Check level nếu khớp GroupCode
                            {
                                // Check level 1 and level 2 khác empty và khớp
                                if (!string.IsNullOrEmpty(eLevel1Id) && employeeDetails.Level1Id.MakeLowerCase().Equals(eLevel1Id.MakeLowerCase()) &&
                                    !string.IsNullOrEmpty(eLevel2Id) && employeeDetails.Level2Id.MakeLowerCase().Equals(eLevel2Id.MakeLowerCase())
                                )
                                    result.IsGrant = true;

                                // Check level 1 nếu khác empty và level 2 is empty
                                else if (!string.IsNullOrEmpty(eLevel1Id) && employeeDetails.Level1Id.MakeLowerCase().Equals(eLevel1Id.MakeLowerCase()))
                                    result.IsGrant = true;

                                // Level 1 và 2 đều empty thì hiển nhiên được cấp quyền 
                                else
                                    result.IsGrant = true;
                            }
                        }
                    }
                    else
                    {
                        _log.LogError($"Found a group with wrong format at StepId {step.Id} with group is {elements}.");
                    }
                }

                // Nếu vẫn không được cấp phép thì check tiếp tới assigned và deep check is true
                if (userForm != null && isDeepCheck)
                {
                    result.CurrentStep = userForm.CurrentStep;

                    if (!result.IsGrant)
                    {
                        // Check theo Assign trước
                        var assigned = await Db.UserFormAssigns.Where(
                                x => x.EmpCode.Equals(employeeDetails.EmpCode) &&
                                     x.StepId.Equals(step.Id) &&
                                     x.UserFormId.Equals(userForm.Id))
                            .FirstOrDefaultAsync();

                        if (assigned != null)
                        {
                            result.IsGrant = true;
                            result.LastAssign = assigned;
                        }
                    }
                }

                // Check is allow to do action right now or not
                if (result.IsGrant)
                {
                    var startFrom = step.AvailableFrom.GetValueOrDefault();
                    var expireIn = step.ExpireIn.GetValueOrDefault();

                    // Nếu UserForm được chỉ định thì check trường hợp đã quá thời gian của Step
                    if (userForm != null)
                    {
                        if (userForm.AvailableFrom.HasValue) startFrom = userForm.AvailableFrom.Value;
                        if (userForm.ExpireIn.HasValue) expireIn = userForm.ExpireIn.Value;
                    }

                    if (DateTimeUtils.IsValid(startFrom, expireIn))
                        result.IsAllow = true;

                    // Check current step action type
                    if (userForm != null && isDeepCheck)
                    {
                        // check current step
                        if (userForm.CurrentStep.Claims.Equals(result.ActionType.ToString()))
                            result.IsAllow = true;
                        else
                            result.IsAllow = false;
                    }
                }
            }

            return result;
        }

        public GrantPermissionResult IsGrantPermissionForView(FormStep step, Employee employeeDetails)
        {
            var result = new GrantPermissionResult()
            {
                CurrentStep = step,
                EmployeeDetails = employeeDetails,
            };

            // Tách và lấy danh sách các groups
            var groupIds = step?.ViewPermissions?.TrySplit(";");

            if (groupIds?.Length > 0)
            {
                foreach (var id in groupIds)
                {
                    // Phân tách từng group để lấy thông tin theo pattern
                    // EmpCode|PosCode|Level1Id|Level2Id

                    var elements = id.Split('|');

                    if (elements.Length == 4)
                    {
                        var eEmpCode = elements[0];
                        var ePosCode = elements[1];
                        var eLevel1Id = elements[2];
                        var eLevel2Id = elements[3];

                        // Set action type
                        result.ActionType = (FormStepActionType)Enum.Parse(typeof(FormStepActionType), step.Claims);

                        // Kiếm tra mã Emp trước
                        if (eEmpCode.Equals(employeeDetails.EmpCode))
                        {
                            result.IsGrant = true;
                            return result;
                        }
                        else if (!string.IsNullOrEmpty(ePosCode))     // Check theo mã position code
                        {
                            if (employeeDetails.PositionCode.MakeLowerCase().Equals(ePosCode.MakeLowerCase())) // Check level nếu khớp GroupCode
                            {
                                // Check level 1 and level 2 khác empty và khớp
                                if (!string.IsNullOrEmpty(eLevel1Id) && employeeDetails.Level1Id.MakeLowerCase()
                                        .Equals(eLevel1Id.MakeLowerCase()) &&
                                    !string.IsNullOrEmpty(eLevel2Id) && employeeDetails.Level2Id.MakeLowerCase()
                                        .Equals(eLevel2Id.MakeLowerCase())
                                )
                                {
                                    result.IsGrant = true;
                                    return result;
                                }

                                // Check level 1 nếu khác empty và level 2 is empty
                                if (!string.IsNullOrEmpty(eLevel1Id) && employeeDetails.Level1Id.MakeLowerCase().Equals(eLevel1Id.MakeLowerCase()))
                                {
                                    result.IsGrant = true;
                                    return result;
                                }

                                // Level 1 và 2 đều empty thì hiển nhiên được cấp quyền 
                                else
                                {
                                    result.IsGrant = true;
                                    return result;
                                }
                            }
                        }
                    }
                    else
                    {
                        _log.LogError($"Found a group with wrong format at StepId {step.Id} with group is {elements}.");
                    }
                }
            }

            return result;
        }

        public async Task<IList<Form>> GetAllGrantPermissionForViewAsync(Employee empDetails)
        {
            var empGroup = $"{empDetails.EmpCode}|||";
            var posGroup = $"|{empDetails.PositionCode}|";

            var formSteps = await Queryable(x => !string.IsNullOrEmpty(x.ViewPermissions) && (
                                                     x.ViewPermissions.Contains(empGroup) ||
                                                     x.ViewPermissions.Contains(posGroup)
                                                 )).Include(x => x.Form).ToListAsync();
            if (formSteps?.Count > 0)
            {
                var forms = new List<Form>();

                foreach (var formStep in formSteps)
                {
                    if (forms.FirstOrDefault(x => x.Id.Equals(formStep.Id)) == null)
                    {
                        var userForm = await Db.UserForms.FirstOrDefaultAsync(x => x.CurrentStepId.Equals(formStep.Id) && x.FormId.Equals(formStep.FormId));
                        if (userForm != null)
                            forms.Add(formStep.Form);
                    }
                }

                return forms;
            }

            return null;
        }

        public async Task<bool> Insert_Into(FormStep _formStep)
        {
            if (!String.IsNullOrEmpty(_formStep.Id))
            {
                try
                {
                    Db.FormSteps.Add(_formStep);
                    await Db.SaveChangesAsync();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<IList<FormStep>> GetViewPermissionByEmpCodeAsync(Employee empDetails)
        {
            var listItems = new List<FormStep>();
            if (empDetails != null)
            {                
                var formByEmpCode = await Db.FormSteps
                    .Where(x => x.ViewPermissions.Contains(empDetails.EmpCode))
                    .Include(y => y.Form)
                    .ToListAsync();
                if (formByEmpCode?.Count > 0)
                {
                    foreach (var _formStep in formByEmpCode)
                    {
                        listItems.Add(_formStep);
                    }
                }
            }

            return listItems;
        }

        public async Task<FormStep> UpdateFormSteps(string ID)
        {
            FormStep _formStep = Db.FormSteps.Where(x => x.Id.Equals(ID)).FirstOrDefault();
            _formStep.Confirm = 1;
            var next_Step = _formStep.NextStepId;
            try
            {
                Db.Entry(_formStep).State = EntityState.Modified;
                await Db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return null;
            }            

            return await Db.FormSteps.Where(x => x.Id.Equals(next_Step)).FirstOrDefaultAsync();
        }
    }
}
