using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class UserForm : Entity<long>
    {
        //public string UserFormId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public string FormId { get; set; }
        public virtual Form Form { get; set; }

        /// <summary>
        /// JSON Data of user input values
        /// </summary>
        public string InputValues { get; set; }

        public string CurrentStepId { get; set; }
        public virtual FormStep CurrentStep { get; set; }

        /// <summary>
        /// Thời gian bắt đầu của Current Step, it will orverride AvailableFrom of current step.
        /// </summary>
        public DateTimeOffset? AvailableFrom { get; set; }

        /// <summary>
        /// Thời gian kết thúc của Current Step, it will orverride ExpireIn of current step.
        /// </summary>
        public DateTimeOffset? ExpireIn { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public UserForm(string FormId, string UserId, string InputValues, string CurrentStepId, DateTimeOffset? AvailableFrom, DateTimeOffset? ExpireIn, DateTimeOffset DateCreated)
        {
            this.FormId = FormId;
            this.UserId = UserId;
            this.InputValues = InputValues;
            this.CurrentStepId = CurrentStepId;
            this.AvailableFrom = AvailableFrom;
            this.ExpireIn = ExpireIn;
            this.DateCreated = DateCreated;
        }
        public UserForm()
        {
        }

    }
}
