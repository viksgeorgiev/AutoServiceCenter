using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static AutoServiceCenter.GCommon.ValidationConstants.Vehicle;

namespace AutoServiceCenter.Data.Models
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(MakeMaxLength)]
        public string Make { get; set; } = null!;

        [Required]
        [MaxLength(ModelMaxLength)]
        public string Model { get; set; } = null!;

        [Range(YearMinValue, YearMaxValue)]
        public int Year { get; set; }

        [Required]
        [MaxLength(LicensePlateMaxLength)]
        public string LicensePlate { get; set; } = null!;

        [Required]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedOn { get; set; }
    }
}