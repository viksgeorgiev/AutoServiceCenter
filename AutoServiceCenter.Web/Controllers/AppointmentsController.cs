using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Common.Enums;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceCenter.Web.Controllers
{
    public class AppointmentsController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ApplicationDbContext context, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _context = context;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            _logger.LogInformation("Accessing Appointments/Index with page {Page} and search term {SearchTerm}", page, searchTerm);

            try
            {
                var viewModel = await _appointmentService.GetAppointmentsAsync(page, 5, searchTerm);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments for Index");
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details action called with null ID");
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            try
            {
                var viewModel = await _appointmentService.GetAppointmentByIdAsync(id.Value);
                if (viewModel == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });
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
            await PopulateDropdowns();
            return View(new AppointmentCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _appointmentService.CreateAppointmentAsync(model, GetUserId());
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating appointment");
                    ModelState.AddModelError("", "An error occurred while creating the appointment.");
                }
            }

            await PopulateDropdowns();
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
                var viewModel = await _appointmentService.GetAppointmentForEditAsync(id.Value);
                if (viewModel == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found for edit", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });
                }

                await PopulateDropdowns();
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

            if (ModelState.IsValid)
            {
                try
                {
                    await _appointmentService.UpdateAppointmentAsync(id, model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating appointment with ID {Id}", id);
                    ModelState.AddModelError("", "An error occurred while updating the appointment.");
                }
            }

            await PopulateDropdowns();
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
                var viewModel = await _appointmentService.GetAppointmentByIdAsync(id.Value);
                if (viewModel == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });
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
                await _appointmentService.DeleteAppointmentAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.Customers = new SelectList(
                await _context.Customers
                    .Include(c => c.User)
                    .Where(c => !c.IsDeleted)
                    .Select(c => new { Id = c.Id, Name = c.User.UserName })
                    .ToListAsync(),
                "Id", "Name");

            ViewBag.Vehicles = new SelectList(
                await _context.Vehicles
                    .Where(v => !v.IsDeleted)
                    .Select(v => new { Id = v.Id, LicensePlate = v.LicensePlate })
                    .ToListAsync(),
                "Id", "LicensePlate");

            ViewBag.Services = new SelectList(
                await _context.Services
                    .Where(s => !s.IsDeleted)
                    .Select(s => new { Id = s.Id, Name = s.Name })
                    .ToListAsync(),
                "Id", "Name");

            ViewBag.Mechanics = new SelectList(
                await _context.Mechanics
                    .Include(m => m.User)
                    .Where(m => !m.IsDeleted)
                    .Select(m => new { Id = m.Id, Name = m.User.UserName })
                    .ToListAsync(),
                "Id", "Name");

            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(AppointmentStatus)));
        }
    }
}