using AutoServiceCenter.Data;
using AutoServiceCenter.Data.Models;
using AutoServiceCenter.Services.Core.Contracts;
using AutoServiceCenter.Web.ViewModels.Appointment;
using AutoServiceCenter.Web.ViewModels.Mechanics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoServiceCenter.Services.Core
{
    public class MechanicService : IMechanicService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<MechanicService> _logger;

        public MechanicService(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<MechanicService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<MechanicIndexViewModel> GetMechanicsAsync(int page, int pageSize)
        {
            _logger.LogInformation("Fetching mechanics for page {Page}", page);

            IQueryable<Mechanic> query = _context.Mechanics
                .AsNoTracking()
                .Include(m => m.User)
                .Include(m => m.Appointments);

            int totalItems = await query.CountAsync();
            List<Mechanic> mechanics = await query
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            MechanicIndexViewModel viewModel = new MechanicIndexViewModel
            {
                Mechanics = mechanics.Select(m => new MechanicViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    UserName = m.User?.UserName ?? "N/A",
                    Specialization = m.Specialization,
                    ExperienceYears = m.ExperienceYears,
                    AppointmentCount = m.Appointments?.Count(a => !a.IsDeleted) ?? 0
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return viewModel;
        }

        public async Task<MechanicViewModel> GetMechanicByIdAsync(Guid id)
        {
            Mechanic? mechanic = await _context.Mechanics
                .AsNoTracking()
                .Include(m => m.User)
                .Include(m => m.Appointments)
                    .ThenInclude(a => a.Customer)
                    .ThenInclude(c => c.User)
                .Include(m => m.Appointments)
                    .ThenInclude(a => a.Vehicle)
                .Include(m => m.Appointments)
                    .ThenInclude(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mechanic == null)
            {
                _logger.LogWarning("Mechanic with ID {Id} not found", id);
                return null;
            }

            return new MechanicViewModel
            {
                Id = mechanic.Id,
                Name = mechanic.Name,
                UserName = mechanic.User?.UserName ?? "N/A",
                Specialization = mechanic.Specialization,
                ExperienceYears = mechanic.ExperienceYears,
                AppointmentCount = mechanic.Appointments?.Count(a => !a.IsDeleted) ?? 0,
                Appointments = mechanic.Appointments?
                    .Where(a => !a.IsDeleted)
                    .Select(a => new AppointmentViewModel
                    {
                        Id = a.Id,
                        Name = a.Customer?.Name ?? "N/A",
                        CustomerEmail = a.Customer?.User?.UserName ?? "N/A",
                        VehicleLicensePlate = a.Vehicle?.LicensePlate ?? "N/A",
                        ServiceName = a.Service?.Name ?? "N/A",
                        AppointmentDate = a.Date,
                        Status = a.Status,
                        Notes = a.Notes
                    }).ToList() ?? new List<AppointmentViewModel>()
            };
        }

        public async Task CreateMechanicAsync(MechanicCreateViewModel model)
        {
            
            if (!await _roleManager.RoleExistsAsync("Mechanic"))
            {
                IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole { Name = "Mechanic", NormalizedName = "MECHANIC" });
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Failed to create Mechanic role: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    throw new Exception("Failed to create Mechanic role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            
            IdentityUser? user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                _logger.LogWarning("Attempt to create mechanic with existing email: {Email}", model.Email);
                throw new InvalidOperationException("A user with this email already exists.");
            }

           
            user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true 
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create user for mechanic with email {Email}: {Errors}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            
            result = await _userManager.AddToRoleAsync(user, "Mechanic");
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to assign Mechanic role to user {Email}: {Errors}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception("Failed to assign Mechanic role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            
            Mechanic mechanic = new Mechanic
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                UserId = user.Id,
                Specialization = model.Specialization,
                ExperienceYears = model.ExperienceYears,
                IsDeleted = false
            };

            _context.Mechanics.Add(mechanic);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created mechanic with ID {Id} and email {Email}", mechanic.Id, model.Email);
        }

        public async Task<MechanicCreateViewModel> GetMechanicForEditAsync(Guid id)
        {
            Mechanic? mechanic = await _context.Mechanics
                .AsNoTracking()
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mechanic == null)
            {
                _logger.LogWarning("Mechanic with ID {Id} not found for edit", id);
                return null;
            }

            return new MechanicCreateViewModel
            {
                Id = mechanic.Id,
                Name = mechanic.Name,
                Email = mechanic.User?.Email ?? string.Empty,
                Specialization = mechanic.Specialization,
                ExperienceYears = mechanic.ExperienceYears
            };
        }

        public async Task UpdateMechanicAsync(Guid id, MechanicCreateViewModel model)
        {
            Mechanic? mechanic = await _context.Mechanics
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mechanic == null)
            {
                _logger.LogWarning("Mechanic with ID {Id} not found for update", id);
                return;
            }

            IdentityUser? user = await _userManager.FindByIdAsync(mechanic.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for mechanic update", mechanic.UserId);
                return;
            }

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user for mechanic with ID {Id}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new Exception("Failed to update user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            mechanic.Name = model.Name;
            mechanic.Specialization = model.Specialization;
            mechanic.ExperienceYears = model.ExperienceYears;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated mechanic with ID {Id} and email {Email}", id, model.Email);
        }
    }
}