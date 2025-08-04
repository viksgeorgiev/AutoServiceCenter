using System.ComponentModel.DataAnnotations;
using AutoServiceCenter.GCommon;

namespace AutoServiceCenter.Web.ViewModels.Services
{
    public class ServiceCreateViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.Service.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(ValidationConstants.Service.DescriptionMaxLength)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}