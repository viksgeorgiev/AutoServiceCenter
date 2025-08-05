using AutoServiceCenter.GCommon;
using System.ComponentModel.DataAnnotations;

namespace AutoServiceCenter.Web.ViewModels.Account
{
    public class ManageViewModel
    {
        [Required]
        [StringLength(ValidationConstants.Customer.NameMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(ValidationConstants.Customer.AddressMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(ValidationConstants.Vehicle.MakeMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Vehicle Make")]
        public string VehicleMake { get; set; } = null!;

        [Required]
        [StringLength(ValidationConstants.Vehicle.ModelMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Vehicle Model")]
        public string VehicleModel { get; set; } = null!;

        [Required]
        [Range(ValidationConstants.Vehicle.YearMinValue, ValidationConstants.Vehicle.YearMaxValue, ErrorMessage = "The {0} must be between {1} and {2}.")]
        [Display(Name = "Vehicle Year")]
        public int VehicleYear { get; set; }

        [Required]
        [StringLength(ValidationConstants.Vehicle.LicensePlateMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Vehicle License Plate")]
        public string VehicleLicensePlate { get; set; } = null!;

        public Guid VehicleId { get; set; }
    }
}