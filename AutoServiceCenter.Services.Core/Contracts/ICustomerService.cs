using AutoServiceCenter.Web.ViewModels.Customer;

namespace AutoServiceCenter.Services.Core.Contracts
{
    public interface ICustomerService
    {
        Task<CustomerIndexViewModel> GetCustomersAsync(int page, int pageSize, string searchTerm);
        Task<CustomerViewModel> GetCustomerByIdAsync(Guid id, string userId, bool isAdminOrMechanic);
        Task<CustomerCreateViewModel> GetCustomerForEditAsync(Guid id, string userId, bool isAdminOrMechanic);
        Task UpdateCustomerAsync(Guid id, CustomerCreateViewModel model, string userId, bool isAdminOrMechanic);
        Task DeleteCustomerAsync(Guid id, string userId, bool isAdminOrMechanic);
    }
}