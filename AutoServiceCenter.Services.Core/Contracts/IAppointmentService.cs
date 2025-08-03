using AutoServiceCenter.Web.ViewModels.Appointment;

namespace AutoServiceCenter.Services.Core.Contracts
{
    public interface IAppointmentService
    {
        Task<AppointmentIndexViewModel> GetAppointmentsAsync(int page, int pageSize, string searchTerm);
        Task<AppointmentViewModel> GetAppointmentByIdAsync(Guid id);
        Task CreateAppointmentAsync(AppointmentCreateViewModel model, string userId);
        Task<AppointmentCreateViewModel> GetAppointmentForEditAsync(Guid id);
        Task UpdateAppointmentAsync(Guid id, AppointmentCreateViewModel model);
        Task DeleteAppointmentAsync(Guid id);
    }
}