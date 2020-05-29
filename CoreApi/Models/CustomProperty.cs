using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class CustomProperty : Entity<long>
    {
        public string TargetType { get; set; }
        public string TargetValue { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
