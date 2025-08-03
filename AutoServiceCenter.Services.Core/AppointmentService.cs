using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Common.Enums;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.Services.Core.Contracts;
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

        public async Task<AppointmentIndexViewModel> GetAppointmentsAsync(int page, int pageSize, string searchTerm)
        {
            _logger.LogInformation("Fetching appointments for page {Page} with search term {SearchTerm}", page, searchTerm);

            var query = _context.Appointments
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Vehicle)
                .Include(a => a.Service)
                .Include(a => a.Mechanic).ThenInclude(m => m.User)
                .Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(a =>
                    a.Customer.User.UserName.ToLower().Contains(searchTerm) ||
                    a.Vehicle.LicensePlate.ToLower().Contains(searchTerm) ||
                    a.Service.Name.ToLower().Contains(searchTerm) ||
                    a.Notes.ToLower().Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var appointments = await query
                .OrderBy(a => a.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new AppointmentIndexViewModel
            {
                Appointments = appointments.Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    CustomerName = a.Customer?.User?.UserName ?? "N/A",
                    VehicleLicensePlate = a.Vehicle?.LicensePlate ?? "N/A",
                    ServiceName = a.Service?.Name ?? "N/A",
                    MechanicName = a.Mechanic?.User?.UserName ?? "N/A",
                    AppointmentDate = a.Date,
                    Status = a.Status,
                    Notes = a.Notes
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                SearchTerm = searchTerm
            };

            return viewModel;
        }

        public async Task<AppointmentViewModel> GetAppointmentByIdAsync(Guid id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Vehicle)
                .Include(a => a.Service)
                .Include(a => a.Mechanic).ThenInclude(m => m.User)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {Id} not found", id);
                return null;
            }

            return new AppointmentViewModel
            {
                Id = appointment.Id,
                CustomerName = appointment.Customer?.User?.UserName ?? "N/A",
                VehicleLicensePlate = appointment.Vehicle?.LicensePlate ?? "N/A",
                ServiceName = appointment.Service?.Name ?? "N/A",
                MechanicName = appointment.Mechanic?.User?.UserName ?? "N/A",
                AppointmentDate = appointment.Date,
                Status = appointment.Status,
                Notes = appointment.Notes
            };
        }

        public async Task CreateAppointmentAsync(AppointmentCreateViewModel model, string userId)
        {
            try
            {
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    CustomerId = model.CustomerId,
                    VehicleId = model.VehicleId,
                    ServiceId = model.ServiceId,
                    MechanicId = model.MechanicId,
                    Date = model.AppointmentDate,
                    Status = AppointmentStatus.Pending,
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

        public async Task<AppointmentCreateViewModel> GetAppointmentForEditAsync(Guid id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {Id} not found for edit", id);
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

        public async Task UpdateAppointmentAsync(Guid id, AppointmentCreateViewModel model)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found for update", id);
                    throw new KeyNotFoundException("Appointment not found");
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
                _logger.LogInformation("Updated appointment with ID {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteAppointmentAsync(Guid id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found for deletion", id);
                    throw new KeyNotFoundException("Appointment not found");
                }

                appointment.IsDeleted = true;
                appointment.DeletedOn = DateTime.UtcNow;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted appointment with ID {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment with ID {Id}", id);
                throw;
            }
        }
    }
}