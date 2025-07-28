using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Customer;

namespace AutoServiceCenter.Data.Models
{
    public class Customer : BaseDeletableEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required] 
        public string UserId { get; set; } = null!;

        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(AddressMaxLength)]
        public string Address { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; }
            = new HashSet<Vehicle>();

        public ICollection<Appointment> Appointments { get; set; }
            = new HashSet<Appointment>();
    }
}