using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoServiceCenter.GCommon.ValidationConstants.Mechanic;

namespace AutoServiceCenter.Data.Models
{
    public class Mechanic
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual IdentityUser User { get; set; } = null!;

        [Required]
        [MaxLength(SpecializationMaxLength)]
        public string Specialization { get; set; } = null!;

        [Range(ExperienceYearsMinValue, ExperienceYearsMaxValue)]
        public int ExperienceYears { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
            = new HashSet<Appointment>();

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedOn { get; set; }
    }
}