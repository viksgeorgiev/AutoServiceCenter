using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Mechanics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceCenter.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class MechanicsController : Controller
    {
        private readonly IMechanicService _mechanicService;
        private readonly ILogger<MechanicsController> _logger;

        public MechanicsController(IMechanicService mechanicService, ILogger<MechanicsController> logger)
        {
            _mechanicService = mechanicService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            _logger.LogInformation("Accessing Mechanics/Index with page {Page}", page);

            try
            {
                var viewModel = await _mechanicService.GetMechanicsAsync(page, 5);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching mechanics for Index");
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        public IActionResult Create()
        {
            return View(new MechanicCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MechanicCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _mechanicService.CreateMechanicAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mechanic");
                ModelState.AddModelError("", "An error occurred while creating the mechanic.");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit action called with null ID");
                return RedirectToAction("Error", "Home", new { area = "" });
            }

            try
            {
                var model = await _mechanicService.GetMechanicForEditAsync(id.Value);
                if (model == null)
                {
                    _logger.LogWarning("Mechanic with ID {Id} not found for edit", id);
                    return RedirectToAction("Error", "Home", new { area = "" });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching mechanic for edit with ID {Id}", id);
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MechanicCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _mechanicService.UpdateMechanicAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mechanic with ID {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the mechanic.");
                return View(model);
            }
        }
    }
}