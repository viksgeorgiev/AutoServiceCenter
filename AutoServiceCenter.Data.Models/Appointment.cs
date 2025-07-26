using System.ComponentModel.DataAnnotations;
using AutoServiceCenter.Data.Common.Enums;

public class Appointment
{
    public Guid Id { get; set; }

    public DateTime Date { get; set; }

    public Guid CustomerId { get; set; }

    public virtual Customer Customer { get; set; }

    public Guid VehicleId { get; set; }

    public virtual Vehicle Vehicle { get; set; }

    public Guid MechanicId { get; set; }

    public virtual Mechanic Mechanic { get; set; }

    public Guid ServiceId { get; set; }

    public virtual Service Service { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string Notes { get; set; }
}