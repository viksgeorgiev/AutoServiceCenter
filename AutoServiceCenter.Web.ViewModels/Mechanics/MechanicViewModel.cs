using AutoServiceCenter.Web.ViewModels.Appointment;


namespace AutoServiceCenter.Web.ViewModels.Mechanics
{
    public class MechanicViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public int AppointmentCount { get; set; }
        public List<AppointmentViewModel> Appointments { get; set; } 
            = new List<AppointmentViewModel>();
    }
}