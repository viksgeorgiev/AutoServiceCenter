using AutoServiceCenter.Data.Common.Enums;

namespace AutoServiceCenter.Web.ViewModels.Appointment
{
    public class AppointmentIndexViewModel
    {
        public List<AppointmentViewModel> Appointments { get; set; } 
            = new List<AppointmentViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class AppointmentViewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string VehicleLicensePlate { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string MechanicName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}