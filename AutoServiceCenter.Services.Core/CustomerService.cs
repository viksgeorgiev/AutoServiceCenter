using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Appointment;
using AutoServiceCenter.Web.ViewModels.Customer;
using AutoServiceCenter.Web.ViewModels.Vehicle;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoServiceCenter.Services.Core
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<CustomerService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<CustomerIndexViewModel> GetCustomersAsync(int page, int pageSize, string searchTerm)
        {
            _logger.LogInformation("Fetching customers for page {Page}, searchTerm: {SearchTerm}", page, searchTerm);

            IQueryable<Customer> query = _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Include(c => c.Appointments)
                .Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.User.UserName.ToLower().Contains(searchTerm) ||
                    c.Address.ToLower().Contains(searchTerm));
            }

            int totalItems = await query.CountAsync();
            List<Customer> customers = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new CustomerIndexViewModel
            {
                Customers = customers.Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.User?.UserName ?? "N/A",
                    Address = c.Address,
                    VehicleCount = c.Vehicles?.Count(v => !v.IsDeleted) ?? 0,
                    AppointmentCount = c.Appointments?.Count(a => !a.IsDeleted) ?? 0
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                SearchTerm = searchTerm
            };
        }

        public async Task<CustomerViewModel> GetCustomerByIdAsync(Guid id, string userId, bool isAdminOrMechanic)
        {
            Customer? customer = await _context.Customers
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

            if (customer == null || (!isAdminOrMechanic && customer.UserId != userId))
            {
                _logger.LogWarning("Customer with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                return null;
            }

            return new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.User?.UserName ?? "N/A",
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
                        Name = a.Customer?.Name ?? "N/A",
                        CustomerEmail = a.Customer?.User?.UserName ?? "N/A",
                        VehicleLicensePlate = a.Vehicle?.LicensePlate ?? "N/A",
                        ServiceName = a.Service?.Name ?? "N/A",
                        MechanicName = a.Mechanic?.Name ?? "N/A",
                        AppointmentDate = a.Date,
                        Status = a.Status,
                        Notes = a.Notes
                    }).ToList() ?? new List<AppointmentViewModel>()
            };
        }

        public async Task<CustomerCreateViewModel> GetCustomerForEditAsync(Guid id, string userId, bool isAdminOrMechanic)
        {
            Customer? customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (customer == null || (!isAdminOrMechanic && customer.UserId != userId))
            {
                _logger.LogWarning("Customer with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                return null;
            }

            return new CustomerCreateViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.User?.Email ?? string.Empty,
                Address = customer.Address
            };
        }

        public async Task UpdateCustomerAsync(Guid id, CustomerCreateViewModel model, string userId, bool isAdminOrMechanic)
        {
            Customer? customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (customer == null || (!isAdminOrMechanic && customer.UserId != userId))
            {
                _logger.LogWarning("Customer with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                throw new UnauthorizedAccessException("User can only update their own customer profile");
            }

            IdentityUser? user = await _userManager.FindByIdAsync(customer.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for customer update", customer.UserId);
                throw new InvalidOperationException("Associated user not found");
            }

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user for customer with ID {Id}", id);
                    throw new Exception("Failed to update user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            customer.Name = model.Name;
            customer.Address = model.Address;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated customer with ID {Id} and email {Email}", id, model.Email);
        }

        public async Task DeleteCustomerAsync(Guid id, string userId, bool isAdminOrMechanic)
        {
            Customer? customer = await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Include(c => c.Appointments)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (customer == null || (!isAdminOrMechanic && customer.UserId != userId))
            {
                _logger.LogWarning("Customer with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                throw new UnauthorizedAccessException("User can only delete their own customer profile");
            }

            customer.IsDeleted = true;
            customer.DeletedOn = DateTime.UtcNow;

            foreach (Vehicle vehicle in customer.Vehicles)
            {
                vehicle.IsDeleted = true;
                vehicle.DeletedOn = DateTime.UtcNow;
            }

            foreach (Appointment appointment in customer.Appointments)
            {
                appointment.IsDeleted = true;
                appointment.DeletedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted customer with ID {Id} and email {Email}", id, customer.User?.Email);
        }
    }
}