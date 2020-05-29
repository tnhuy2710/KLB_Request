using System;
using System.ComponentModel.DataAnnotations;
using CoreApi.Enums;

namespace CoreApi.Models
{
    public class Group : Entity<string>
    {
        [MaxLength(255)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public GroupType GroupType { get; set; }


        /// <summary>
        /// Time begin to lockout
        /// </summary>
        public DateTimeOffset? LockoutStart { get; set; }

        /// <summary>
        /// Time to end of lockout
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
