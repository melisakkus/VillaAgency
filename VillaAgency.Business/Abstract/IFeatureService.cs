using System.Linq.Expressions;
using VillaAgency.Dto.FeatureDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IFeatureService
    {

        Task<List<ResultFeatureDto>> TGetAllAsync();
        Task<UpdateFeatureDto> TGetByIdAsync(string id);
        Task TCreateAsync(CreateFeatureDto dto);
        Task TUpdateAsync(UpdateFeatureDto dto);
        Task TDeleteAsync(string id);

        Task<List<ResultFeatureDto>> TGetFilteredListAsync(
            Expression<Func<FeatureSection, bool>> predicate,
            int? page = null,
            int? pageSize = null);
    }
}
