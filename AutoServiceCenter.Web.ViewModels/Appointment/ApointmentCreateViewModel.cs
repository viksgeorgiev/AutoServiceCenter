using System.ComponentModel.DataAnnotations;
using AutoServiceCenter.Data.Common.Enums;
using static AutoServiceCenter.GCommon.ValidationConstants.Appointment;

namespace AutoServiceCenter.Web.ViewModels.Appointment
{
    public class AppointmentCreateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Vehicle is required")]
        public Guid VehicleId { get; set; }

        [Required(ErrorMessage = "Service is required")]
        public Guid ServiceId { get; set; }

        [Required(ErrorMessage = "Mechanic is required")]
        public Guid MechanicId { get; set; }

        [Required(ErrorMessage = "Appointment date is required")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        public AppointmentStatus Status { get; set; }

        [MaxLength(NotesMaxLength, ErrorMessage = "Notes cannot exceed {1} characters")]
        public string? Notes { get; set; }
    }
}