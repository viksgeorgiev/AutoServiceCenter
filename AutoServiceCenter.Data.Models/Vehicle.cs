using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Vehicle;

namespace AutoServiceCenter.Data.Models
{
    public class Vehicle : BaseDeletableEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(MakeMaxLength)]
        public string Make { get; set; } = null!;

        [Required] 
        [MaxLength(ModelMaxLength)] 
        public string Model { get; set; } = null!;

        [Range(YearMinValue,YearMaxValue)]
        public int Year { get; set; }

        [Required]
        [MaxLength(LicensePlateMaxLength)]
        public string LicensePlate { get; set; } = null!;

        [Required]
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
    }
}