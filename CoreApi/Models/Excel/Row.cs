using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models.Excel
{
    public class Row
    {
        public int Position { get; set; }
        public int OriginalPosition { get; set; }

        public int GroupIndex { get; set; }
        public bool IsExpanded { get; set; }

        public int BaseOnRow { get; set; }

        public List<Cell> Cells { get; set; }

        public Row()
        {
            Cells = new List<Cell>();
        }

        public Row Clone()
        {
            return new Row()
            {
                Cells      = Cells.ToList(),
                GroupIndex = GroupIndex,
                Position   = Position,
                IsExpanded = IsExpanded,
                BaseOnRow  = BaseOnRow,
            };
        }
    }
}
