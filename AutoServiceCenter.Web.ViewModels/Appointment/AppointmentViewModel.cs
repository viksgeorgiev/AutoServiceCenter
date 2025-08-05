using AutoServiceCenter.Data.Common.Enums;
using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Appointment;

namespace AutoServiceCenter.Web.ViewModels.Appointment
{
    public class AppointmentViewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string VehicleLicensePlate { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public string MechanicName { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class AppointmentCreateViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public Guid MechanicId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        public AppointmentStatus Status { get; set; } 
            = AppointmentStatus.Pending;

        [MaxLength(NotesMaxLength)]
        public string? Notes { get; set; } 
    }
}