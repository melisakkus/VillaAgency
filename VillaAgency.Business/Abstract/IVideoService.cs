using System.Linq.Expressions;
using VillaAgency.Dto.BannerDtos;
using VillaAgency.Dto.VideoViewDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IVideoService
    {
        Task TCreateAsync(CreateVideoViewDto dto);
        Task TUpdateAsync(UpdateVideoViewDto dto);
        Task TDeleteAsync(string id);

        Task<List<ResultVideoViewDto>> TGetListAsync();
        Task<UpdateVideoViewDto> TGetByIdAsync(string id);

        Task<List<ResultVideoViewDto>> TGetFilteredListAsync(Expression<Func<VideoView, bool>> predicate);

        Task TMakeItActive(string id);
        Task TMakeItPassive(string id);
    }
}
