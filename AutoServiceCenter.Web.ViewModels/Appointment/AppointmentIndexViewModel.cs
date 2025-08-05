using System.Collections.Generic;

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
}