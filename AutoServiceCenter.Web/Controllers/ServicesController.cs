using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceCenter.Web.Controllers
{
    [Authorize(Roles = "Mechanic,Administrator")]
    public class ServicesController : BaseController
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(IServiceService serviceService, ILogger<ServicesController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            _logger.LogInformation("Accessing Services/Index with page {Page}", page);

            try
            {
                var viewModel = await _serviceService.GetServicesAsync(page, 5);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services for Index");
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
                var viewModel = await _serviceService.GetServiceByIdAsync(id.Value);
                if (viewModel == null)
                {
                    _logger.LogWarning("Service with ID {Id} not found", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service details for ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        public IActionResult Create()
        {
            return View(new ServiceCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _serviceService.CreateServiceAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service");
                ModelState.AddModelError("", "An error occurred while creating the service.");
                return View(model);
            }
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
                var model = await _serviceService.GetServiceForEditAsync(id.Value);
                if (model == null)
                {
                    _logger.LogWarning("Service with ID {Id} not found for edit", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service for edit with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ServiceCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _serviceService.UpdateServiceAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service with ID {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the service.");
                return View(model);
            }
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
                var model = await _serviceService.GetServiceByIdAsync(id.Value);
                if (model == null)
                {
                    _logger.LogWarning("Service with ID {Id} not found for deletion", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service for deletion with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _serviceService.DeleteServiceAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }
    }
}