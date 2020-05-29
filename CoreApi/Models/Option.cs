using System.ComponentModel.DataAnnotations;

namespace CoreApi.Models
{
    public class Option : Entity<int>
    {
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
