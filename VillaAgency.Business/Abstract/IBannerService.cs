using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IBannerService
    {
        Task<List<Banner>> GetActiveBannersAsync();
    }
}
