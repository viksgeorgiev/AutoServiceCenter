using System.ComponentModel.DataAnnotations;

namespace AutoServiceCenter.Web.ViewModels.Admin
{
    public class EditUserRoleViewModel
    {
        public string UserId { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        public List<string> CurrentRoles { get; set; } 
            = new List<string>();
        public List<string> AvailableRoles { get; set; } 
            = new List<string>();

        [Display(Name = "Roles")]
        public List<string>? SelectedRoles { get; set; }
    }
}