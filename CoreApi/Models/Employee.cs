using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoreApi.Models
{
    public class Employee
    {
        public string PortalId { get; set; }

        /// <summary>
        /// Username of Portal
        /// </summary>
        public string Username { get; set; }

        [JsonProperty("EmpId")]
        public string EmpId { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [JsonProperty("EmpCode")]
        public string EmpCode { get; set; }

        /// <summary>
        /// Họ và tên nhân viên
        /// </summary>
        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        /// <summary>
        /// Id chi nhánh quản lý nhân viên này
        /// </summary>
        public string Level1Id { get; set; }

        /// <summary>
        /// Tên chi nhánh quản lý nhân viên này
        /// </summary>
        public string Level1Name { get; set; }

        /// <summary>
        /// Id phòng/ban nhân viên đang công tác
        /// </summary>
        public string Level2Id { get; set; }

        /// <summary>
        /// Tên phòng/ban nhân viên đang công tác
        /// </summary>
        public string Level2Name { get; set; }

        /// <summary>
        /// Chức danh
        /// </summary>
        [JsonProperty("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Ngày sinh (dd/MM/yyyy)
        /// </summary>
        [JsonProperty("BirthDay")]
        public string BirthDay { get; set; }

        /// <summary>
        /// Giới tính (1: Nam, 0: Nữ)
        /// </summary>
        [JsonProperty("Gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Base64 ảnh đại diện
        /// </summary>
        [JsonProperty("FilePhoto")]
        public string FilePhoto { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Số chứng minh/hộ chiếu
        /// </summary>
        [JsonProperty("PIN")]
        public string Pin { get; set; }

        /// <summary>
        /// Ngày cấp số chứng minh/hộ chiếu
        /// </summary>
        [JsonProperty("PINDate")]
        public string PinDate { get; set; }

        /// <summary>
        /// Nơi cấp số chứng minh/hộ chiếu
        /// </summary>
        [JsonProperty("PINPlace")]
        public string PinPlace { get; set; }

        /// <summary>
        /// Địa chỉ thường trú
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Địa chỉ tạm trú
        /// </summary>
        public string TemporaryAddress { get; set; }

        /// <summary>
        /// Ngày bắt đầu vào làm
        /// </summary>
        public string StartWorkingDate { get; set; }

        /// <summary>
        /// Ngày bắt đầu làm ở vị trí công việc hiện tại
        /// </summary>
        public DateTime PositionDate { get; set; }

        public string GroupId { get; set; }

        public string GroupCode { get; set; }

        /// <summary>
        /// Số tài khoản TCBS dành cho Cộng Tác Viên
        /// </summary>
        public string SoTaiKhoanTCBS { get; set; }

        public string BranchId { get; set; }

        /// <summary>
        /// Trạng thái hoạt động tài khoản Portal
        /// </summary>
        public bool IsLockedout { get; set; }

        /// <summary>
        /// Mã chức vụ
        /// </summary>
        public string PositionCode { get; set; }

        public string GetLevelName()
        {
            if (!string.IsNullOrEmpty(Level1Name))
            {
                var levelName = Level1Name;
                if (!string.IsNullOrEmpty(Level2Name))
                    levelName = Level2Name;

                return levelName.Trim();
            }

            return "";
        }
    }
}
