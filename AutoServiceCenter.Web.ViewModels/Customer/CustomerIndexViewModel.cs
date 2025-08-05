namespace AutoServiceCenter.Web.ViewModels.Customer
{
    public class CustomerIndexViewModel
    {
        public List<CustomerViewModel> Customers { get; set; } 
            = new List<CustomerViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = null!;
    }
}