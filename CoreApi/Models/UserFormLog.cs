using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class UserFormLog : Entity<long>
    {
        public long UserFormId { get; set; }
        public string StepId { get; set; }
        public string Action { get; set; }
        public string AuthorEmpCode { get; set; }
        public string TargetEmpCode { get; set; }
        public string Message { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        [NotMapped]
        public UserForm UserForm { get; set; }
        [NotMapped]
        public FormStep Step { get; set; }
    }
}
