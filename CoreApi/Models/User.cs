using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CoreApi.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class User : IdentityUser, ITimespan
    {
        /// <summary>
        /// Time begin to lock
        /// </summary>
        public DateTimeOffset? LockoutStart { get; set; }

        /// <summary>
        /// Mã nhân sự
        /// </summary>
        public string EmpCode { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// List devices Login
        /// </summary>
        public virtual IList<UserDevice> UserDevices { get; set; }

        /// <summary>
        /// List groups user Joined
        /// </summary>
        public virtual IList<UserGroup> UserGroups { get; set; }
        

        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
    }
}
