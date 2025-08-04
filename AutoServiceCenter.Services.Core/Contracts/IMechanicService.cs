using AutoServiceCenter.Web.ViewModels.Mechanics;

namespace AutoServiceCenter.Services.Core.Contracts
{
    public interface IMechanicService
    {
        Task<MechanicIndexViewModel> GetMechanicsAsync(int page, int pageSize);
        Task<MechanicViewModel> GetMechanicByIdAsync(Guid id);
        Task CreateMechanicAsync(MechanicCreateViewModel model);
        Task<MechanicCreateViewModel> GetMechanicForEditAsync(Guid id);
        Task UpdateMechanicAsync(Guid id, MechanicCreateViewModel model);
    }
}