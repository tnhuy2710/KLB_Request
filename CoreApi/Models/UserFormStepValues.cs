using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace CoreApi.Models
{
    public class UserFormStepValues: Entity<string>
    {
        [ForeignKey("UserForm")] 
        public long UserFormId { get; set; }
        public UserForm UserForm { get; set; }

        [ForeignKey("FormStep")]
        public string StepId { get; set; }
        public FormStep FormStep { get; set; }

        public string FormValue { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
