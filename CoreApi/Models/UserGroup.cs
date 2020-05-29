using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class UserGroup
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }


        [ForeignKey("Group")]
        public string GroupId { get; set; }
        public virtual Group Group { get; set; }

        // Addition Data

    }
}
