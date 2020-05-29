using System.ComponentModel.DataAnnotations;

namespace CoreApi.Models
{
    public class Whitelist : Entity<int>
    {
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Username { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }
    }
}
