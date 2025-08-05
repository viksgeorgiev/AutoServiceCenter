using System.ComponentModel.DataAnnotations;
using AutoServiceCenter.GCommon;

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
        [StringLength(ValidationConstants.Mechanic.NameMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(ValidationConstants.Mechanic.SpecializationMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Specialization { get; set; } = null!;

        [Range(ValidationConstants.Mechanic.ExperienceYearsMinValue, ValidationConstants.Mechanic.ExperienceYearsMaxValue)]
        public int ExperienceYears { get; set; }
    }
}