using System.ComponentModel.DataAnnotations;
using AutoServiceCenter.GCommon;

namespace AutoServiceCenter.Web.ViewModels.Customer;

public class CustomerCreateViewModel
{
    public Guid Id { get; set; }

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
}