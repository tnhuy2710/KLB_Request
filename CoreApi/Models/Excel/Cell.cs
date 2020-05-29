using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Enums;

namespace CoreApi.Models.Excel
{
    public class Cell
    {
        public int Position { get; set; }
        public string Address { get; set; }

        public CellType CellType { get; set; }
        public bool IsMerge { get; set; }

        public string Styles { get; set; }
        public string Classes { get; set; }

        public string Content { get; set; }
        public string Formula { get; set; }
        public string Format { get; set; }

        public string InputType { get; set; }
        public string InputList { get; set; }
    }
}
