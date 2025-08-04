using AutoServiceCenter.Web.ViewModels.Mechanics;

namespace AutoServiceCenter.Web.ViewModels.Mechanics
{
    public class MechanicIndexViewModel
    {
        public List<MechanicViewModel> Mechanics { get; set; } 
            = new List<MechanicViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}