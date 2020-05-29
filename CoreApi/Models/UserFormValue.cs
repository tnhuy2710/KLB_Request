using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CoreApi.Models
{
    public class UserFormValue : Entity<long>
    {
        [ForeignKey("UserForm")]
        public long UserFormId { get; set; }
        public UserForm UserForm { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
