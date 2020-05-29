using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class RowFillDetail
    {
        public int RowStart { get; set; }
        public int ColStart { get; set; }
        public List<string> Values { get; set; }
    }
}
