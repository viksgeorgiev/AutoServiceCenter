using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Service;

namespace AutoServiceCenter.Data.Models
{
    public class Service
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Range(PriceMinValue, PriceMaxValue)]
        public decimal Price { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
            = new HashSet<Appointment>();

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedOn { get; set; }
    }
}