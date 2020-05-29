using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreApi.Models
{
    public class Device : Entity<string>, ITimespan
    {
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Uuid { get; set; }

        [MaxLength(255)]
        public string OsName { get; set; }

        [MaxLength(100)]
        public string OsVersion { get; set; }

        /// <summary>
        /// This token for using Push Notification from Firebase
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        /// Trạng thái Lock của Device.
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        public virtual IList<UserDevice> UserDevices { get; set; }

        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
    }
}
