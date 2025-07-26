using AutoServiceCenter.Data.Models;
using System.ComponentModel.DataAnnotations;

public class Customer
{
    public Guid Id { get; set; }

    
    public string UserId { get; set; }

    public virtual ApplicationUser User { get; set; }

    
    public string Address { get; set; }

    public ICollection<Vehicle> Vehicles { get; set; } 
        = new HashSet<Vehicle>();
    public ICollection<Appointment> Appointments { get; set; } 
        = new HashSet<Appointment>();
}