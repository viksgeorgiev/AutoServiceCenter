using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Mechanic;

namespace AutoServiceCenter.Web.ViewModels.Mechanics
{
    public class MechanicCreateViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(NameMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(SpecializationMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Specialization { get; set; } = null!;

        [Range(ExperienceYearsMinValue, ExperienceYearsMaxValue)]
        public int ExperienceYears { get; set; }
    }
}