using AutoServiceCenter.Web.ViewModels.Customer;

namespace AutoServiceCenter.Services.Core.Contracts
{
    public interface ICustomerService
    {
        Task<CustomerIndexViewModel> GetCustomersAsync(int page, int pageSize, string searchTerm);
        Task<CustomerViewModel> GetCustomerByIdAsync(Guid id);
    }
}