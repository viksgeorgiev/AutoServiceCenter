namespace AutoServiceCenter.Web.ViewModels.Services
{
    public class ServiceIndexViewModel
    {
        public List<ServiceViewModel> Services { get; set; } 
            = new List<ServiceViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}