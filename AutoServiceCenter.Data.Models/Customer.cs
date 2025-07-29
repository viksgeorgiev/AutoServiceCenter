using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Customer;

namespace AutoServiceCenter.Data.Models
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;

        [Required]
        [MaxLength(AddressMaxLength)]
        public string Address { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; }
            = new HashSet<Vehicle>();

        public ICollection<Appointment> Appointments { get; set; }
            = new HashSet<Appointment>();

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedOn { get; set; }
    }
}