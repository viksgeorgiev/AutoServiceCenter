using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoServiceCenter.Data.Common.Enums;
using static AutoServiceCenter.GCommon.ValidationConstants.Appointment;

namespace AutoServiceCenter.Data.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [Required]
        public Guid VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [Required]
        public Guid MechanicId { get; set; }

        [ForeignKey(nameof(MechanicId))]
        public virtual Mechanic Mechanic { get; set; } = null!;

        [Required]
        public Guid ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; } = null!;

        [Required]
        public AppointmentStatus Status { get; set; }
            = AppointmentStatus.Pending;

        [MaxLength(NotesMaxLength)]
        public string Notes { get; set; } = null!;

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedOn { get; set; }
    }
}