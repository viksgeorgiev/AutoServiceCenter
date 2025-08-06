using System.ComponentModel.DataAnnotations;
using static AutoServiceCenter.GCommon.ValidationConstants.Service;

namespace AutoServiceCenter.Web.ViewModels.Services
{
    public class ServiceCreateViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}