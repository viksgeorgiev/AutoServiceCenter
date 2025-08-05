using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AutoServiceCenter.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            ICustomerService customerService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _customerService = customerService;
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
                IdentityUser user = new IdentityUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with email {Email}", model.Email);

                    Customer customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        UserId = user.Id,
                        Address = model.Address,
                        IsDeleted = false
                    };

                    Vehicle vehicle = new Vehicle
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
                    _logger.LogInformation("User {UserId} signed in after registration", user.Id);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }

                foreach (IdentityError error in result.Errors)
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
                SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in with email {Email}", model.Email);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found for manage page");
                return NotFound();
            }

            Customer? customer = await _context.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsDeleted);

            if (customer == null)
            {
                _logger.LogWarning("Customer not found for user {UserId}", user.Id);
                return NotFound();
            }

            Vehicle? vehicle = customer.Vehicles?.FirstOrDefault(v => !v.IsDeleted);

            ManageViewModel model = new ManageViewModel
            {
                Name = customer.Name,
                Email = user.Email,
                Address = customer.Address,
                VehicleMake = vehicle?.Make ?? string.Empty,
                VehicleModel = vehicle?.Model ?? string.Empty,
                VehicleYear = vehicle?.Year ?? 0,
                VehicleLicensePlate = vehicle?.LicensePlate ?? string.Empty,
                VehicleId = vehicle?.Id ?? Guid.Empty
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found for manage update");
                return NotFound();
            }

            Customer? customer = await _context.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsDeleted);

            if (customer == null)
            {
                _logger.LogWarning("Customer not found for user {UserId}", user.Id);
                return NotFound();
            }

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            customer.Name = model.Name;
            customer.Address = model.Address;

            Vehicle? vehicle = customer.Vehicles?.FirstOrDefault(v => !v.IsDeleted);
            if (vehicle != null)
            {
                vehicle.Make = model.VehicleMake;
                vehicle.Model = model.VehicleModel;
                vehicle.Year = model.VehicleYear;
                vehicle.LicensePlate = model.VehicleLicensePlate;
            }
            else
            {
                vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    Make = model.VehicleMake,
                    Model = model.VehicleModel,
                    Year = model.VehicleYear,
                    LicensePlate = model.VehicleLicensePlate,
                    IsDeleted = false
                };
                _context.Vehicles.Add(vehicle);
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated customer profile for user {UserId}", user.Id);
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update customer profile for user {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating your profile.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
    }
}