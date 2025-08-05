using AutoServiceCenter.Data.Common.Enums;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels;
using AutoServiceCenter.Web.ViewModels.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceCenter.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, UserManager<IdentityUser> userManager, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            _logger.LogInformation("Accessing Appointments/Index with page {Page} and search term {SearchTerm}", page, searchTerm);

            try
            {
                IdentityUser? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found for Appointments/Index");
                    return Redirect("/Identity/Account/Login");
                }

                bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
                string userId = GetUserId();

                AppointmentIndexViewModel viewModel = await _appointmentService.GetAppointmentsAsync(page, 5, userId, isAdminOrMechanic, searchTerm);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments for Index");
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details action called with null ID");
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            try
            {
                IdentityUser? user = await _userManager.GetUserAsync(User);
                bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
                string userId = GetUserId();

                AppointmentViewModel? viewModel = await _appointmentService.GetAppointmentByIdAsync(id.Value, userId, isAdminOrMechanic);
                if (viewModel == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found or unauthorized", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 403 });
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment details for ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        public async Task<IActionResult> Create()
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            string userId = GetUserId();

            AppointmentCreateViewModel model = new AppointmentCreateViewModel { AppointmentDate = DateTime.Now };

            if (!isAdminOrMechanic)
            {
                List<DropdownItem> customers = await _appointmentService.GetCustomersAsync(false, userId);
                DropdownItem? customer = customers.FirstOrDefault();
                if (customer != null)
                {
                    model.CustomerId = Guid.Parse(customer.Value);
                }
                else
                {
                    _logger.LogWarning("No customer found for user {UserId}", userId);
                    ModelState.AddModelError("", "No customer profile found. Please contact support.");
                    await PopulateDropdowns(isAdminOrMechanic, userId);
                    return View(model);
                }
            }

            await PopulateDropdowns(isAdminOrMechanic, userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            string userId = GetUserId();

            if (ModelState.IsValid)
            {
                try
                {
                    await _appointmentService.CreateAppointmentAsync(model, userId, isAdminOrMechanic);
                    return RedirectToAction(nameof(Index));
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("Unauthorized attempt to create appointment by user {UserId}", userId);
                    ModelState.AddModelError("", "You are not authorized to create this appointment.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating appointment by user {UserId}", userId);
                    ModelState.AddModelError("", "An error occurred while creating the appointment.");
                }
            }

            await PopulateDropdowns(isAdminOrMechanic, userId);
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit action called with null ID");
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            try
            {
                IdentityUser? user = await _userManager.GetUserAsync(User);
                bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
                string userId = GetUserId();

                AppointmentCreateViewModel? viewModel = await _appointmentService.GetAppointmentForEditAsync(id.Value, userId, isAdminOrMechanic);
                if (viewModel == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found or unauthorized for edit", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 403 });
                }

                await PopulateDropdowns(isAdminOrMechanic, userId);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment for edit with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AppointmentCreateViewModel model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning("Mismatched ID in Edit action: {Id} vs {ModelId}", id, model.Id);
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            IdentityUser? user = await _userManager.GetUserAsync(User);
            bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
            string userId = GetUserId();

            if (ModelState.IsValid)
            {
                try
                {
                    await _appointmentService.UpdateAppointmentAsync(id, model, userId, isAdminOrMechanic);
                    return RedirectToAction(nameof(Index));
                }
                catch (UnauthorizedAccessException)
                {
                    _logger.LogWarning("Unauthorized attempt to update appointment {Id} by user {UserId}", id, userId);
                    ModelState.AddModelError("", "You are not authorized to update this appointment.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating appointment with ID {Id}", id);
                    ModelState.AddModelError("", "An error occurred while updating the appointment.");
                }
            }

            await PopulateDropdowns(isAdminOrMechanic, userId);
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete action called with null ID");
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            try
            {
                IdentityUser? user = await _userManager.GetUserAsync(User);
                bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
                string userId = GetUserId();

                AppointmentViewModel? viewModel = await _appointmentService.GetAppointmentByIdAsync(id.Value, userId, isAdminOrMechanic);
                if (viewModel == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found or unauthorized for delete", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 403 });
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment for delete with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                IdentityUser? user = await _userManager.GetUserAsync(User);
                bool isAdminOrMechanic = await _userManager.IsInRoleAsync(user, "Administrator") || await _userManager.IsInRoleAsync(user, "Mechanic");
                string userId = GetUserId();

                await _appointmentService.DeleteAppointmentAsync(id, userId, isAdminOrMechanic);
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized attempt to delete appointment {Id} by user {UserId}", id, GetUserId());
                return RedirectToAction("Error", "Home", new { statusCode = 403 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        private async Task PopulateDropdowns(bool isAdminOrMechanic, string userId)
        {
            ViewBag.Customers = await _appointmentService.GetCustomersAsync(isAdminOrMechanic, userId);
            ViewBag.Vehicles = await _appointmentService.GetVehiclesAsync(isAdminOrMechanic, userId);
            ViewBag.Services = await _appointmentService.GetServicesAsync();
            ViewBag.Mechanics = await _appointmentService.GetMechanicsAsync();
            ViewBag.Statuses = Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>().Select(s => new DropdownItem
            {
                Value = s.ToString(),
                Text = s.ToString()
            }).ToList();
            ViewBag.IsAdminOrMechanic = isAdminOrMechanic;
        }
    }
}