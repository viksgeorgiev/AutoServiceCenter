using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels;
using AutoServiceCenter.Web.ViewModels.Appointment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoServiceCenter.Services.Core
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(ApplicationDbContext context, ILogger<AppointmentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AppointmentIndexViewModel> GetAppointmentsAsync(int page, int pageSize, string userId, bool isAdminOrMechanic, string searchTerm)
        {
            _logger.LogInformation("Fetching appointments for page {Page}, user {UserId}, isAdminOrMechanic: {IsAdminOrMechanic}, searchTerm: {SearchTerm}", page, userId, isAdminOrMechanic, searchTerm);

            IQueryable<Appointment> query = _context.Appointments
                .AsNoTracking()
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Vehicle)
                .Include(a => a.Service)
                .Include(a => a.Mechanic).ThenInclude(m => m.User);

            if (!isAdminOrMechanic)
            {
                query = query.Where(a => a.Customer.UserId == userId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(a =>
                    a.Customer.Name.ToLower().Contains(searchTerm) ||
                    a.Customer.User.UserName.ToLower().Contains(searchTerm) ||
                    a.Vehicle.LicensePlate.ToLower().Contains(searchTerm) ||
                    a.Service.Name.ToLower().Contains(searchTerm) ||
                    a.Notes.ToLower().Contains(searchTerm));
            }

            int totalItems = await query.CountAsync();
            List<AppointmentViewModel> appointments = await query
                .OrderBy(a => a.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    Name = a.Customer.Name ?? "N/A",
                    CustomerEmail = a.Customer.User.UserName ?? "N/A",
                    VehicleLicensePlate = a.Vehicle.LicensePlate,
                    ServiceName = a.Service.Name,
                    MechanicName = a.Mechanic.Name ?? "N/A",
                    AppointmentDate = a.Date,
                    Status = a.Status,
                    Notes = a.Notes
                })
                .ToListAsync();

            return new AppointmentIndexViewModel
            {
                Appointments = appointments,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                SearchTerm = searchTerm
            };
        }

        public async Task<AppointmentViewModel> GetAppointmentByIdAsync(Guid id, string userId, bool isAdminOrMechanic)
        {
            Appointment? appointment = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Vehicle)
                .Include(a => a.Service)
                .Include(a => a.Mechanic).ThenInclude(m => m.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null || (!isAdminOrMechanic && appointment.Customer.UserId != userId))
            {
                _logger.LogWarning("Appointment with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                return null;
            }

            return new AppointmentViewModel
            {
                Id = appointment.Id,
                Name = appointment.Customer?.Name ?? "N/A",
                CustomerEmail = appointment.Customer?.User?.UserName ?? "N/A",
                VehicleLicensePlate = appointment.Vehicle?.LicensePlate ?? "N/A",
                ServiceName = appointment.Service?.Name ?? "N/A",
                MechanicName = appointment.Mechanic?.Name ?? "N/A",
                AppointmentDate = appointment.Date,
                Status = appointment.Status,
                Notes = appointment.Notes
            };
        }

        public async Task CreateAppointmentAsync(AppointmentCreateViewModel model, string userId, bool isAdminOrMechanic)
        {
            try
            {
                Customer? customer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == model.CustomerId);
                if (!isAdminOrMechanic && customer?.UserId != userId)
                {
                    throw new UnauthorizedAccessException("User can only create appointments for themselves");
                }

                Appointment appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    CustomerId = model.CustomerId,
                    VehicleId = model.VehicleId,
                    ServiceId = model.ServiceId,
                    MechanicId = model.MechanicId,
                    Date = model.AppointmentDate,
                    Status = model.Status,
                    Notes = model.Notes ?? string.Empty,
                    IsDeleted = false,
                    DeletedOn = null
                };

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created appointment with ID {Id} by user {UserId}", appointment.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment by user {UserId}", userId);
                throw;
            }
        }

        public async Task<AppointmentCreateViewModel> GetAppointmentForEditAsync(Guid id, string userId, bool isAdminOrMechanic)
        {
            Appointment? appointment = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null || (!isAdminOrMechanic && appointment.Customer.UserId != userId))
            {
                _logger.LogWarning("Appointment with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                return null;
            }

            return new AppointmentCreateViewModel
            {
                Id = appointment.Id,
                CustomerId = appointment.CustomerId,
                VehicleId = appointment.VehicleId,
                ServiceId = appointment.ServiceId,
                MechanicId = appointment.MechanicId,
                AppointmentDate = appointment.Date,
                Status = appointment.Status,
                Notes = appointment.Notes
            };
        }

        public async Task UpdateAppointmentAsync(Guid id, AppointmentCreateViewModel model, string userId, bool isAdminOrMechanic)
        {
            try
            {
                Appointment? appointment = await _context.Appointments
                    .Include(a => a.Customer)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null || (!isAdminOrMechanic && appointment.Customer.UserId != userId))
                {
                    _logger.LogWarning("Appointment with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                    throw new UnauthorizedAccessException("User can only update their own appointments");
                }

                appointment.CustomerId = model.CustomerId;
                appointment.VehicleId = model.VehicleId;
                appointment.ServiceId = model.ServiceId;
                appointment.MechanicId = model.MechanicId;
                appointment.Date = model.AppointmentDate;
                appointment.Status = model.Status;
                appointment.Notes = model.Notes ?? string.Empty;

                _context.Update(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated appointment with ID {Id} by user {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteAppointmentAsync(Guid id, string userId, bool isAdminOrMechanic)
        {
            try
            {
                Appointment? appointment = await _context.Appointments
                    .Include(a => a.Customer)
                    .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

                if (appointment == null || (!isAdminOrMechanic && appointment.Customer.UserId != userId))
                {
                    _logger.LogWarning("Appointment with ID {Id} not found or unauthorized for user {UserId}", id, userId);
                    throw new UnauthorizedAccessException("User can only delete their own appointments");
                }

                appointment.IsDeleted = true;
                appointment.DeletedOn = DateTime.UtcNow;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted appointment with ID {Id} by user {UserId}", id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment with ID {Id}", id);
                throw;
            }
        }

        public async Task<List<DropdownItem>> GetCustomersAsync(bool isAdminOrMechanic, string userId)
        {
            IQueryable<Customer> query = _context.Customers
                .AsNoTracking()
                .Include(c => c.User);

            if (!isAdminOrMechanic)
            {
                query = query.Where(c => c.UserId == userId);
            }

            return await query
                .Select(c => new DropdownItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = !isAdminOrMechanic && c.UserId == userId
                })
                .ToListAsync();
        }

        public async Task<List<DropdownItem>> GetVehiclesAsync(bool isAdminOrMechanic, string userId)
        {
            IQueryable<Vehicle> query = _context.Vehicles
                .AsNoTracking()
                .Include(v => v.Customer);

            if (!isAdminOrMechanic)
            {
                query = query.Where(v => v.Customer.UserId == userId);
            }

            return await query
                .Select(v => new DropdownItem
                {
                    Value = v.Id.ToString(),
                    Text = v.LicensePlate
                })
                .ToListAsync();
        }

        public async Task<List<DropdownItem>> GetServicesAsync()
        {
            return await _context.Services
                .AsNoTracking()
                .Select(s => new DropdownItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }

        public async Task<List<DropdownItem>> GetMechanicsAsync()
        {
            return await _context.Mechanics
                .AsNoTracking()
                .Include(m => m.User)
                .Select(m => new DropdownItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                })
                .ToListAsync();
        }
    }
}