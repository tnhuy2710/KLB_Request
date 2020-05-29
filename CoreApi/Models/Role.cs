using Microsoft.AspNetCore.Identity;

namespace CoreApi.Models
{
    public class Role : IdentityRole<string>
    {
        // Add this contructor for resolve error
        public Role()
        {

        }

        public Role(string name, int level) : base(name)
        {
            this.Name = name;
            this.Level = level;
        }

        public int Level { get; set; }
    }
}
