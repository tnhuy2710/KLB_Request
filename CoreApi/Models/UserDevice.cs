using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class UserDevice
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }


        [ForeignKey("Device")]
        public string DeviceId { get; set; }
        public virtual Device Device { get; set; }

        //

        public string Token { get; set; }

        public DateTimeOffset ExpireIn { get; set; }
        public DateTimeOffset LastAccess { get; set; }
    }
}
