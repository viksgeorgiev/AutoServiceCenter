namespace AutoServiceCenter.Web.ViewModels.Admin
{
    public class UserRoleIndexViewModel
    {
        public List<UserRoleViewModel> Users { get; set; } 
            = new List<UserRoleViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = null!;
    }
}