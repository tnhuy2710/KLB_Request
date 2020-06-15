using System;

namespace CoreApi.Models
{
    public class Form : Entity<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
            
        public int SheetIndex { get; set; }
        public string FileType { get; set; }

        public DateTimeOffset PublishDate { get; set; }
        public DateTimeOffset CloseDate { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// Danh sách Users or GroupIds được phép xem biểu mẫu này
        /// </summary>
        public string ViewPermissions { get; set; }

        public Nullable<int> Confirm { get; set; }
    }
}
