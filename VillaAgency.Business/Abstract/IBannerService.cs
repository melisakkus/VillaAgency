using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Dto.BannerDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IBannerService
    {
        Task TCreateAsync(CreateBannerDto dto);
        Task TUpdateAsync(UpdateBannerDto dto);
        Task TDeleteAsync(ObjectId id);

        Task<List<ResultBannerDto>> TGetListAsync();
        Task<UpdateBannerDto> TGetByIdAsync(ObjectId id);

        Task<int> TCountAsync();

        Task<List<ResultBannerDto>> TGetFilteredListAsync(Expression<Func<Banner, bool>> predicate);
    }
}
