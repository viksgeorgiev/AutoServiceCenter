using System.ComponentModel.DataAnnotations;
using AutoServiceCenter.Data.Common.Enums;

namespace AutoServiceCenter.Data.Models
{
    public class Appointment : BaseDeletableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; } = null!;

        [Required]
        public Guid VehicleId { get; set; }

        public virtual Vehicle Vehicle { get; set; } = null!;

        [Required]
        public Guid MechanicId { get; set; }

        public virtual Mechanic Mechanic { get; set; } = null!;

        [Required]
        public Guid ServiceId { get; set; }

        public virtual Service Service { get; set; } = null!;

        [Required]
        public AppointmentStatus Status { get; set; }
            = AppointmentStatus.Pending;

        public string Notes { get; set; } = null!;
    }
}