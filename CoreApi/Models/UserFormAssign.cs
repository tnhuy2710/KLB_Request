using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class UserFormAssign : Entity<long>
    {
        public string EmpCode { get; set; }

        [ForeignKey("UserForm")]
        public long UserFormId { get; set; }
        public virtual UserForm UserForm { get; set; }

        [ForeignKey("FormStep")]
        public string StepId { get; set; }
        public virtual FormStep FormStep { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
