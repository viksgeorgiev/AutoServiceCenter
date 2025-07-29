using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static AutoServiceCenter.GCommon.ValidationConstants.ApplicationUser;

namespace AutoServiceCenter.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(FullNameMaxLength)]
        public string FullName { get; set; } = null!;

        public DateTime? RegisteredOn { get; set; }

        public virtual Customer CustomerProfile { get; set; } = null!;
        public virtual Mechanic MechanicProfile { get; set; } = null!;
    }
}
