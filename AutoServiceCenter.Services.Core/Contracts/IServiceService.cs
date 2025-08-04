using AutoServiceCenter.Web.ViewModels.Services;

namespace AutoServiceCenter.Services.Core.Contracts
{
    public interface IServiceService
    {
        Task<ServiceIndexViewModel> GetServicesAsync(int page, int pageSize);
        Task<ServiceViewModel> GetServiceByIdAsync(Guid id);
        Task CreateServiceAsync(ServiceCreateViewModel model);
        Task<ServiceCreateViewModel> GetServiceForEditAsync(Guid id);
        Task UpdateServiceAsync(Guid id, ServiceCreateViewModel model);
        Task DeleteServiceAsync(Guid id);
    }
}