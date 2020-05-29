using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models.Excel
{
    public class MergeRow
    {
        public int RowStart { get; set; }
        public int ColumnStart { get; set; }

        public int RowEnd { get; set; }
        public int ColumnEnd { get; set; }

        public string Address { get; set; }
    }
}
