using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Mechanic;

namespace AutoServiceCenter.Data.Models
{
    public class Mechanic : BaseDeletableEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        [MaxLength(SpecializationMaxLength)]
        public string Specialization { get; set; } = null!;

        [Range(ExperienceYearsMinValue,ExperienceYearsMaxValue)]
        public int ExperienceYears { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
            = new HashSet<Appointment>();
    }
}