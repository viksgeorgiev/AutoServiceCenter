using AutoServiceCenter.Data.Models;
using System.ComponentModel.DataAnnotations;

public class Mechanic
{
    public Guid Id { get; set; }

    
    public string UserId { get; set; }

    public virtual ApplicationUser User { get; set; }

    
    public string Specialization { get; set; }

    public int ExperienceYears { get; set; }

    public ICollection<Appointment> Appointments { get; set; } 
        = new HashSet<Appointment>();
}