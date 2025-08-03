using AutoServiceCenter.Data;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Appointment;
using AutoServiceCenter.Web.ViewModels.Customer;
using AutoServiceCenter.Web.ViewModels.Vehicle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoServiceCenter.Services.Core
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ApplicationDbContext context, ILogger<CustomerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CustomerIndexViewModel> GetCustomersAsync(int page, int pageSize, string searchTerm)
        {
            _logger.LogInformation("Fetching customers for page {Page} with search term {SearchTerm}", page, searchTerm);

            var query = _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Include(c => c.Appointments)
                .Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    c.User.UserName.ToLower().Contains(searchTerm) ||
                    c.Address.ToLower().Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var customers = await query
                .OrderBy(c => c.User.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new CustomerIndexViewModel
            {
                Customers = customers.Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    Name = c.User?.UserName ?? "N/A",
                    Address = c.Address,
                    VehicleCount = c.Vehicles?.Count(v => !v.IsDeleted) ?? 0,
                    AppointmentCount = c.Appointments?.Count(a => !a.IsDeleted) ?? 0
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                SearchTerm = searchTerm
            };

            return viewModel;
        }

        public async Task<CustomerViewModel> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Vehicle)
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Service)
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Mechanic)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {Id} not found", id);
                return null;
            }

            return new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.User?.UserName ?? "N/A",
                Address = customer.Address,
                VehicleCount = customer.Vehicles?.Count(v => !v.IsDeleted) ?? 0,
                AppointmentCount = customer.Appointments?.Count(a => !a.IsDeleted) ?? 0,
                Vehicles = customer.Vehicles?
                    .Where(v => !v.IsDeleted)
                    .Select(v => new VehicleViewModel
                    {
                        Id = v.Id,
                        Make = v.Make,
                        Model = v.Model,
                        Year = v.Year,
                        LicensePlate = v.LicensePlate
                    }).ToList() ?? new List<VehicleViewModel>(),
                Appointments = customer.Appointments?
                    .Where(a => !a.IsDeleted)
                    .Select(a => new AppointmentViewModel
                    {
                        Id = a.Id,
                        VehicleLicensePlate = a.Vehicle?.LicensePlate ?? "N/A",
                        ServiceName = a.Service?.Name ?? "N/A",
                        MechanicName = a.Mechanic?.User?.UserName ?? "N/A",
                        AppointmentDate = a.Date,
                        Status = a.Status,
                        Notes = a.Notes
                    }).ToList() ?? new List<AppointmentViewModel>()
            };
        }
    }
}