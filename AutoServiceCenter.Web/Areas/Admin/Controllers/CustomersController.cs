using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceCenter.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator,Mechanic")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomersController(ICustomerService customerService, UserManager<IdentityUser> userManager)
        {
            _customerService = customerService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            int pageSize = 10;
            CustomerIndexViewModel model = await _customerService.GetCustomersAsync(page, pageSize, searchTerm);
            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            CustomerViewModel? model = await _customerService.GetCustomerByIdAsync(id, user.Id, isAdminOrMechanic);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            CustomerCreateViewModel? model = await _customerService.GetCustomerForEditAsync(id, user.Id, isAdminOrMechanic);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            try
            {
                await _customerService.UpdateCustomerAsync(id, model, user.Id, isAdminOrMechanic);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the customer.");
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            CustomerViewModel? model = await _customerService.GetCustomerByIdAsync(id, user.Id, isAdminOrMechanic);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            try
            {
                await _customerService.DeleteCustomerAsync(id, user.Id, isAdminOrMechanic);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the customer.");
                return RedirectToAction("Details", new { id });
            }
        }
    }
}