using AutoServiceCenter.Web.ViewModels;
using AutoServiceCenter.Web.ViewModels.Appointment;

namespace AutoServiceCenter.Services.Core.Contracts
{
    public interface IAppointmentService
    {
        Task<AppointmentIndexViewModel> GetAppointmentsAsync(int page, int pageSize, string userId, bool isAdminOrMechanic, string searchTerm);
        Task<AppointmentViewModel> GetAppointmentByIdAsync(Guid id, string userId, bool isAdminOrMechanic);
        Task CreateAppointmentAsync(AppointmentCreateViewModel model, string userId, bool isAdminOrMechanic);
        Task<AppointmentCreateViewModel> GetAppointmentForEditAsync(Guid id, string userId, bool isAdminOrMechanic);
        Task UpdateAppointmentAsync(Guid id, AppointmentCreateViewModel model, string userId, bool isAdminOrMechanic);
        Task DeleteAppointmentAsync(Guid id, string userId, bool isAdminOrMechanic);
        Task<List<DropdownItem>> GetCustomersAsync(bool isAdminOrMechanic, string userId);
        Task<List<DropdownItem>> GetVehiclesAsync(bool isAdminOrMechanic, string userId);
        Task<List<DropdownItem>> GetServicesAsync();
        Task<List<DropdownItem>> GetMechanicsAsync();
    }
}