namespace AutoServiceCenter.Web.ViewModels.Admin
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } 
            = new List<string>();
    }
}