using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.ViewModels.AuthenticationViewModels
{
    public class AuthenticationRequestViewModel
    {
        [MinLength(6, ErrorMessage = "Độ dài tối thiểu là 6 kí tự")]
        [MaxLength(100, ErrorMessage = "Độ dài tối đa là 100 kí tự")]
        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Độ dài tối thiểu là 6 kí tự")]
        [MaxLength(255, ErrorMessage = "Độ dài tối đa là 255 kí tự")]
        public string DeviceUuid { get; set; }
    }
}
