using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceCenter.Web.Controllers
{
    [Authorize(Roles = "Mechanic,Administrator")]
    public class CustomersController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            _logger.LogInformation("Accessing Customers/Index with page {Page} and search term {SearchTerm}", page, searchTerm);

            try
            {
                CustomerIndexViewModel viewModel = await _customerService.GetCustomersAsync(page, 5, searchTerm);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers for Index");
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
                CustomerViewModel? viewModel = await _customerService.GetCustomerByIdAsync(id.Value);
                if (viewModel == null)
                {
                    _logger.LogWarning("Customer with ID {Id} not found", id);
                    return RedirectToAction("Error", "Home", new { statusCode = 404 });
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer details for ID {Id}", id);
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }
    }
}

