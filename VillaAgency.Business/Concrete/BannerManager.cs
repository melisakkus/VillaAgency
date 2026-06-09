using VillaAgency.Business.Abstract;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class BannerManager : IBannerService
    {
        private readonly IGenericService<Banner> _service;

        public BannerManager(IGenericService<Banner> service)
        {
            _service = service;
        }

        public async Task<List<Banner>> GetActiveBannersAsync()
        {
            // return await _service.TGetFilteredListAsync(x => x.IsActive);
            return await _service.TGetListAsync();
        }
    }
}
