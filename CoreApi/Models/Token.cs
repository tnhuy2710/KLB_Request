using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Enums;

namespace CoreApi.Models
{
    public class Token : Entity<int>
    {
        [Required]
        public string Value { get; set; }

        public string UserTarget { get; set; }
        public string DeviceTarget { get; set; }


        public TokenType Type { get; set; }
        public DateTimeOffset ExpireIn { get; set; }
    }
}
