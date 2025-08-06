using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Customer;
using static AutoServiceCenter.GCommon.ValidationConstants.Vehicle;

namespace AutoServiceCenter.Web.ViewModels.Account
{
    public class ManageViewModel
    {
        [Required]
        [StringLength(NameMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(AddressMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(MakeMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Vehicle Make")]
        public string VehicleMake { get; set; } = null!;

        [Required]
        [StringLength(ModelMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Vehicle Model")]
        public string VehicleModel { get; set; } = null!;

        [Required]
        [Range(YearMinValue, YearMaxValue, ErrorMessage = "The {0} must be between {1} and {2}.")]
        [Display(Name = "Vehicle Year")]
        public int VehicleYear { get; set; }

        [Required]
        [StringLength(LicensePlateMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Vehicle License Plate")]
        public string VehicleLicensePlate { get; set; } = null!;

        public Guid VehicleId { get; set; }
    }
}