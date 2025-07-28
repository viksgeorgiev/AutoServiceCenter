using System.ComponentModel.DataAnnotations;

namespace AutoServiceCenter.Data.Models
{
    public abstract class BaseDeletableEntity
    {
        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedOn { get; set; }
    }
}