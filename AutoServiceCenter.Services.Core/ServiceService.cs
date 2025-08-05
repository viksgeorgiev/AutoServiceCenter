using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Appointment;
using AutoServiceCenter.Web.ViewModels.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoServiceCenter.Services.Core
{
    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(ApplicationDbContext context, ILogger<ServiceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceIndexViewModel> GetServicesAsync(int page, int pageSize)
        {
            _logger.LogInformation("Fetching services for page {Page}", page);

            IQueryable<Service> query = _context.Services
                .Include(s => s.Appointments)
                .Where(s => !s.IsDeleted);

            int totalItems = await query.CountAsync();
            List<Service> services = await query
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ServiceIndexViewModel viewModel = new ServiceIndexViewModel
            {
                Services = services.Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description ?? "N/A",
                    Price = s.Price,
                    AppointmentCount = s.Appointments?.Count(a => !a.IsDeleted) ?? 0
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return viewModel;
        }

        public async Task<ServiceViewModel> GetServiceByIdAsync(Guid id)
        {
            Service? service = await _context.Services
                .Include(s => s.Appointments)
                    .ThenInclude(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(s => s.Appointments)
                    .ThenInclude(a => a.Vehicle)
                .Include(s => s.Appointments)
                    .ThenInclude(a => a.Mechanic)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (service == null)
            {
                _logger.LogWarning("Service with ID {Id} not found", id);
                return null;
            }

            return new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description ?? "N/A",
                Price = service.Price,
                AppointmentCount = service.Appointments?.Count(a => !a.IsDeleted) ?? 0,
                Appointments = service.Appointments?
                    .Where(a => !a.IsDeleted)
                    .Select(a => new AppointmentViewModel
                    {
                        Id = a.Id,
                        Name = a.Customer.Name,
                        CustomerEmail = a.Customer?.User?.UserName ?? "N/A",
                        VehicleLicensePlate = a.Vehicle?.LicensePlate ?? "N/A",
                        MechanicName = a.Mechanic?.User?.UserName ?? "N/A",
                        AppointmentDate = a.Date,
                        Status = a.Status,
                        Notes = a.Notes
                    }).ToList() ?? new List<AppointmentViewModel>()
            };
        }

        public async Task CreateServiceAsync(ServiceCreateViewModel model)
        {
            Service service = new Service
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IsDeleted = false
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created service with ID {Id} and name {Name}", service.Id, service.Name);
        }

        public async Task<ServiceCreateViewModel> GetServiceForEditAsync(Guid id)
        {
            Service? service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (service == null)
            {
                _logger.LogWarning("Service with ID {Id} not found for edit", id);
                return null;
            }

            return new ServiceCreateViewModel
            {
                Name = service.Name,
                Description = service.Description,
                Price = service.Price
            };
        }

        public async Task UpdateServiceAsync(Guid id, ServiceCreateViewModel model)
        {
            Service? service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (service == null)
            {
                _logger.LogWarning("Service with ID {Id} not found for update", id);
                return;
            }

            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated service with ID {Id} and name {Name}", id, service.Name);
        }

        public async Task DeleteServiceAsync(Guid id)
        {
            Service? service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (service == null)
            {
                _logger.LogWarning("Service with ID {Id} not found for deletion", id);
                return;
            }

            service.IsDeleted = true;
            service.DeletedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Soft-deleted service with ID {Id} and name {Name}", id, service.Name);
        }
    }
}