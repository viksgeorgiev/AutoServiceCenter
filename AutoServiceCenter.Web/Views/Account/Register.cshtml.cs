using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.GCommon;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoServiceCenter.Web.Views.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(ValidationConstants.Customer.AddressMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [StringLength(ValidationConstants.Vehicle.MakeMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
            [Display(Name = "Vehicle Make")]
            public string VehicleMake { get; set; }

            [Required]
            [StringLength(ValidationConstants.Vehicle.ModelMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
            [Display(Name = "Vehicle Model")]
            public string VehicleModel { get; set; }

            [Required]
            [Range(ValidationConstants.Vehicle.YearMinValue, ValidationConstants.Vehicle.YearMaxValue, ErrorMessage = "The {0} must be between {1} and {2}.")]
            [Display(Name = "Vehicle Year")]
            public int VehicleYear { get; set; }

            [Required]
            [StringLength(ValidationConstants.Vehicle.LicensePlateMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
            [Display(Name = "Vehicle License Plate")]
            public string VehicleLicensePlate { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    Customer customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Address = Input.Address,
                        IsDeleted = false
                    };

                    Vehicle vehicle = new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        Make = Input.VehicleMake,
                        Model = Input.VehicleModel,
                        Year = Input.VehicleYear,
                        LicensePlate = Input.VehicleLicensePlate,
                        IsDeleted = false
                    };

                    try
                    {
                        _context.Customers.Add(customer);
                        _context.Vehicles.Add(vehicle);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create Customer or Vehicle for user {UserId}", user.Id);
                        ModelState.AddModelError(string.Empty, "An error occurred while creating the customer or vehicle profile.");
                        return Page();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}