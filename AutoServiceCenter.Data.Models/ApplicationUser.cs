using Microsoft.AspNetCore.Identity;

namespace AutoServiceCenter.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public DateTime? RegisteredOn { get; set; }

        public virtual Customer CustomerProfile { get; set; }
        public virtual Mechanic MechanicProfile { get; set; }
    }
}
