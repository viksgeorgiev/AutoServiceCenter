namespace AutoServiceCenter.Web.ViewModels.Vehicle
{
    public class VehicleViewModel
    {
        public Guid Id { get; set; }
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string LicensePlate { get; set; } = null!;
    }
}