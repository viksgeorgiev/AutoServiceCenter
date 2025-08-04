using AutoServiceCenter.Web.ViewModels.Appointment;

namespace AutoServiceCenter.Web.ViewModels.Services
{
    public class ServiceViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AppointmentCount { get; set; }
        public List<AppointmentViewModel> Appointments { get; set; } 
            = new List<AppointmentViewModel>();
    }
}