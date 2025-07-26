using System.ComponentModel.DataAnnotations;

public class Service
{
    public Guid Id { get; set; }

    
    public string Name { get; set; }

    public string Description { get; set; }

    
    public decimal Price { get; set; }

    public ICollection<Appointment> Appointments { get; set; } 
        = new HashSet<Appointment>();
}