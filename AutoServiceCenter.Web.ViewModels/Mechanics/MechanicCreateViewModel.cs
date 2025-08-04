using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Mechanic;

namespace AutoServiceCenter.Web.ViewModels.Mechanics
{
    public class MechanicCreateViewModel
    {
        public Guid Id { get; set; }
            
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(SpecializationMaxLength)]
        public string Specialization { get; set; } = string.Empty;

        [Range(ExperienceYearsMinValue, ExperienceYearsMaxValue)]
        public int ExperienceYears { get; set; }
    }
}