using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Enums;

namespace CoreApi.Models
{
    public class GrantPermissionResult
    {
        /// <summary>
        /// Công việc hiện tại theo UserId và GroupId
        /// </summary>
        public FormStepActionType ActionType { get; set; }

        /// <summary>
        /// User được cấp phép thực hiện công việc hiện tại hay không
        /// </summary>
        public bool IsGrant { get; set; }

        /// <summary>
        /// Cho phép thực hiện hành động hiện tại hay không?
        /// </summary>
        public bool IsAllow { get; set; }

        public User User { get; set; }
        public UserForm UserForm { get; set; }
        public Employee EmployeeDetails { get; set; }
        public IList<FormStep> Steps { get; set; }
        public FormStep CurrentStep { get; set; }
        public UserFormAssign LastAssign { get; set; }

    }
}
