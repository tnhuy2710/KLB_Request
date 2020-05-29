using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Commons;

namespace CoreApi.Models
{
    public class FormStep : Entity<string>, ITimespan
    {
    
        /// <summary>
        /// Tên của Permission
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Mô tả chức năng của Permission này nếu có
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Danh sách các claims được cấp quyền khi User trong Permission này.
        /// Các item ngăn cách nhau bởi dấu ;
        /// </summary>
        [Required]
        public string Claims { get; set; }

        /// <summary>
        /// Danh sách các GroupId được phép sử dụng Permission này.
        /// Các item ngăn cách nhau bởi dấu ;
        /// </summary>
        [Required]
        public string GroupIds { get; set; }

        /// <summary>
        /// Thời gian bắt đầu công việc của Permission này.
        /// </summary>
        public DateTimeOffset? AvailableFrom { get; set; }

        /// <summary>
        /// Thời gian kết thúc công việc của Permission này.
        /// </summary>
        public DateTimeOffset? ExpireIn { get; set; }

        /// <summary>
        /// Id của Permission sau khi User Accept Form.
        /// </summary>
        public string NextStepId { get; set; }

        /// <summary>
        /// Id của Permission sau khi User Denied Form.
        /// </summary>
        public string PrevStepId { get; set; }

        public int Index { get; set; }

        public bool IsAllowSendEmail { get; set; }

        /// <summary>
        /// GroupIds có quyền xem form nếu đang ở Step chỉ định
        /// </summary>
        public string ViewPermissions { get; set; }

        public Nullable<int> Confirm { get; set; }

        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; } 


        [Required]
        public string FormId { get; set; }
        public virtual Form Form { get; set; }

        public bool HaveQlttGroup()
        {
            if (!string.IsNullOrEmpty(GroupIds) && GroupIds.Contains($"|{AppContants.QlttGroupCode}|"))
                return true;
            return false;
        }
    }
}
