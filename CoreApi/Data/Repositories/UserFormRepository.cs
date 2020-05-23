using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreApi.Data.Repositories.Base;
using CoreApi.Enums;
using CoreApi.Extensions;
using CoreApi.Models;
using CoreApi.ViewModels.FormViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace CoreApi.Data.Repositories
{
    public interface IUserFormRepository : IRepository<UserForm, long>
    {
        Task<bool> SubmitFormAsync(string formId, string userId, string currentStepId, List<CellSubmit> cellSubmits);

        Task<UserForm> GetFormDataAsync(string formId, string userId);

        Task<UserForm> GetFormDataAsync(string formId);

        Task<IList<UserForm>> GetFormsDataAsync(string formId);

        Task<IList<UserForm>> GetFormsDataByUserIdAsync(string userId);

        Task<List<UserFormValue>> GetValuesByIdAsync(long userFormid);

        Task<List<UserFormValue>> GetValuesByFormIdAsync(string formId);

        Task<bool> SetValue(long userFormId, string key, string value);

        Task<string> GetValue(long userFormId, string key);


        Task<bool> AddLogsAsync(UserForm userForm, UserFormLogActionType actionType, string authorEmpCode, string targetEmpCode, string message);

        Task<List<UserFormLog>> GetLogsAsync(long userFormId);

        Task<List<UserFormLog>> GetLogsByUserFormAndStepAsync(long userFormId, string stepId);
        
        Task<UserFormLog> GetLastLogByUserFormAndStepAsync(long userFormId, string stepId); 
        Task<bool> UpdateLogsAsync();

        Task<bool> Insert_Into(UserForm _userForm);

        Task<bool> Update_ExpireInAsync(string currentStepID);
        string GetFormStepDataAsync(string FormID);
    }

    public class UserFormRepository : BaseRepository<UserForm, long>, IUserFormRepository
    {
        private readonly IUserRepository _userRepository;

        public UserFormRepository(IHttpContextAccessor httpContext, ApplicationDbContext db, IUserRepository userRepository) : base(httpContext, db)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> SubmitFormAsync(string formId, string userId, string currentStepId, List<CellSubmit> cellSubmits)
        {
            var valueJson = await Task.Run(() => JsonConvert.SerializeObject(cellSubmits));
            bool isInsert = false;
            var entity = await GetFormDataAsync(formId, userId);

            if (entity == null)
            {
                isInsert = true;
                entity = new UserForm()
                {
                    FormId = formId.MakeLowerCase(),
                    UserId = userId.MakeLowerCase(),
                };
            }

            entity.InputValues = valueJson;
            entity.DateCreated = DateTimeOffset.UtcNow;
            entity.CurrentStepId = currentStepId;

            if (isInsert)
                return await InsertAsync(entity) != null;
            return await UpdateAsync(entity, false) != null;
        }

        //public async Task<bool> SubmitFormStepValueAsync(string userFormId, string currentStepId, string htmlForm)
        //{
        //    bool isInsert = false;
        //    var entity = await GetFormStepDataAsync(userFormId, currentStepId);

        //    if (entity == null)
        //    {
        //        isInsert = true;
        //        entity = new UserForm()
        //        {
        //            UserFormId = userFormId,
        //            CurrentStepId = currentStepId.MakeLowerCase(),
        //        };
        //    }

        //    entity.FormStepValue = htmlForm;
        //    entity.CurrentStepId = currentStepId.MakeLowerCase();

        //    if (isInsert)
        //        return await InsertAsync(entity) != null;
        //    return await UpdateAsync(entity, false) != null;
        //}

        public async Task<UserForm> GetFormDataAsync(string formId, string userId)
        {
            return await Db.UserForms
                .Where(x => x.FormId.Equals(formId.MakeLowerCase()) && x.UserId.Equals(userId.MakeLowerCase()))
                .Include(x => x.Form)
                .Include(x => x.User)
                .FirstOrDefaultAsync();
        }

        public async Task<UserForm> GetFormDataAsync(string formId)
        {
            return null;
        }        

        public async Task<IList<UserForm>> GetFormsDataAsync(string formId)
        {
            return await Db.UserForms.Where(x => x.FormId.Equals(formId))
                .Include(y => y.User)
                .Include(y => y.CurrentStep)
                .ToListAsync();
        }

        public async Task<IList<UserForm>> GetFormsDataByUserIdAsync(string userId)
        {
            return await Queryable(x => x.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<UserFormValue>> GetValuesByIdAsync(long userFormid)
        {
            return await Db.UserFormValues.Where(x => x.UserFormId.Equals(userFormid))
                .Include(x => x.UserForm)
                .ToListAsync();
        }

        public async Task<List<UserFormValue>> GetValuesByFormIdAsync(string formId)
        {
            var userForms = await (
                    from userForm in Db.UserForms
                    where userForm.FormId.Equals(formId)
                    select userForm.Id
                ).ToListAsync();

            if (userForms?.Count > 0)
            {
                var items = await (
                    from value in Db.UserFormValues
                    where userForms.Contains(value.UserFormId)
                    select value
                ).ToListAsync();

                return items;
            }

            return null;
        }

        public async Task<bool> SetValue(long userFormId, string key, string value)
        {
            // Check
            var item = await Db.UserFormValues.FirstOrDefaultAsync(x =>
                x.UserFormId.Equals(userFormId) && x.Key.Equals(key.Trim()));

            if (item != null)
            {
                item.Value = value;

                //
                Db.UserFormValues.Update(item);

                // Update
                return await Db.SaveChangesAsync() > 0;
            }
            else
            {
                item = new UserFormValue()
                {
                    UserFormId  = userFormId,
                    Key         = key.Trim(),
                    Value       = value,
                    DateCreated = DateTimeOffset.UtcNow
                };

                // Insert
                Db.UserFormValues.Add(item);

                //
                return await Db.SaveChangesAsync() > 0;
            }
        }

        public async Task<string> GetValue(long userFormId, string key)
        {
            var item = await Db.UserFormValues.Where(x => x.UserFormId.Equals(userFormId) && x.Key.Equals(key.Trim()))
                .Include(x => x.UserForm)
                .FirstOrDefaultAsync();

            return item?.Value;
        }

        public async Task<bool> AddLogsAsync(UserForm userForm, UserFormLogActionType actionType, string authorEmpCode, string targetEmpCode, string message)
        {
            var entity = new UserFormLog()
            {
                UserFormId  = userForm.Id,
                StepId      = userForm.CurrentStepId,
                Action      = actionType.ToString(),
                AuthorEmpCode = authorEmpCode,
                TargetEmpCode = targetEmpCode,
                Message     = message,
                DateCreated = DateTimeOffset.UtcNow
            };

            Db.UserFormLogs.Add(entity);
            await Db.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserFormLog>> GetLogsAsync(long userFormId)
        {
            return await Db.UserFormLogs
                .Where(x => x.UserFormId.Equals(userFormId))
                .OrderByDescending(y => y.DateCreated)
                .ToListAsync();
        }

        public async Task<List<UserFormLog>> GetLogsByUserFormAndStepAsync(long userFormId, string stepId)
        {
            return await Db.UserFormLogs.Where(x => x.UserFormId.Equals(userFormId) && x.StepId.Equals(stepId)).ToListAsync();
        }

        public async Task<UserFormLog> GetLastLogByUserFormAndStepAsync(long userFormId, string stepId)
        {
            return await Db.UserFormLogs.Where(x => x.UserFormId.Equals(userFormId) && x.StepId.Equals(stepId)).OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateLogsAsync()
        {
            var logs = await Db.UserFormLogs.ToListAsync();
            if (logs?.Count > 0)
            {
                var logsNeedUpdate = new List<UserFormLog>();
                var tasks = new List<Task>();

                foreach (var log in logs)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        if (!string.IsNullOrEmpty(log.Message))
                        {
                            if (log.Action.Equals(UserFormLogActionType.Submit.ToString()) || log.Action.Equals(UserFormLogActionType.Confirm.ToString()))
                            {
                                // Get a collection of matches.
                                MatchCollection matches = Regex.Matches(log.Message, @"\d+");

                                if (matches?.Count == 2)
                                {

                                    // First match is author
                                    var authorEmpCode = matches[0].Value.Trim();
                                    var targetEmpCode = matches[1].Value.Trim();
                                    if (!string.IsNullOrEmpty(authorEmpCode) && !string.IsNullOrEmpty(targetEmpCode))
                                    {
                                        log.AuthorEmpCode = authorEmpCode;

                                        // Find Details 
                                        var author = await _userRepository.FindDetailsByEmpCodeAsync(authorEmpCode);
                                        if (author != null)
                                            log.Message = log.Message.Replace($"EmpCode {authorEmpCode}", author.FULLNAME.Trim());

                                        //

                                        log.TargetEmpCode = targetEmpCode;

                                        // Find Details 
                                        var target = await _userRepository.FindDetailsByEmpCodeAsync(targetEmpCode);
                                        if (target != null)
                                            log.Message = log.Message.Replace($"EmpCode {targetEmpCode}", target.FULLNAME.Trim());

                                        // Add to list
                                        logsNeedUpdate.Add(log);
                                    }
                                }
                            }
                        }
                    }));
                }

                // 
                await Task.WhenAll(tasks);

                // Update to database
                if (logsNeedUpdate.Count > 0)
                {
                    Db.UserFormLogs.UpdateRange(logsNeedUpdate);
                    return await Db.SaveChangesAsync() > 0;
                }
            }

            return false;
        }

        public async Task<bool> Insert_Into(UserForm _userForm)
        {
            if (!String.IsNullOrEmpty(_userForm.FormId))
            {
                try
                {
                    Db.UserForms.Add(_userForm);
                    await Db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return true;
        }

        public string GetFormStepDataAsync(string FormID)
        {
            var current_StepId = from userForm in Db.UserForms
                                 where userForm.FormId.Equals(FormID)
                                 select userForm.CurrentStepId;
            return current_StepId.FirstOrDefault();

        }

        public async Task<bool> Update_ExpireInAsync(string currentStepID)
        {
            UserForm _userForm = Db.UserForms.Where(x => x.CurrentStepId.Equals(currentStepID)).FirstOrDefault();
            _userForm.ExpireIn = DateTime.Now;            
            try
            {
                Db.Entry(_userForm).State = EntityState.Modified;
                await Db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
