using AutoServiceCenter.Web.ViewModels.Appointment;
using AutoServiceCenter.Web.ViewModels.Vehicle;

namespace AutoServiceCenter.Web.ViewModels.Customer
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int VehicleCount { get; set; }
        public int AppointmentCount { get; set; }
        public List<VehicleViewModel> Vehicles { get; set; } 
            = new List<VehicleViewModel>();
        public List<AppointmentViewModel> Appointments { get; set; } 
            = new List<AppointmentViewModel>();
    }
}