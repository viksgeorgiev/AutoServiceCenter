using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.GCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AutoServiceCenter.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with email {Email}", model.Email);

                    var customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        UserId = user.Id,
                        Address = model.Address,
                        IsDeleted = false
                    };

                    var vehicle = new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        Make = model.VehicleMake,
                        Model = model.VehicleModel,
                        Year = model.VehicleYear,
                        LicensePlate = model.VehicleLicensePlate,
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
                        return View(model);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in with email {Email}", model.Email);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, model.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out for email {Email}", model.Email);
                //    return RedirectToAction("Lockout");
                //}
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Lockout()
        //{
        //    return View();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult LoginWith2fa()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult Manage()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [StringLength(ValidationConstants.Customer.NameMaxLength, ErrorMessage = "The {0} must be at most {1} characters long.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

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
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}