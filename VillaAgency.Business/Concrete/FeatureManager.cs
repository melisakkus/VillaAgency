using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Dto.FeatureDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class FeatureManager : IFeatureService
    {
        private readonly IGenericDal<FeatureSection> _genericDal;
        private readonly ILogger<FeatureManager> _logger;

        public FeatureManager(IGenericDal<FeatureSection> genericDal, ILogger<FeatureManager> logger)
        {
            _genericDal = genericDal;
            _logger = logger;
        }

        public async Task TCreateAsync(CreateFeatureDto dto)
        {
            if (dto is null) 
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            var entity = dto.Adapt<FeatureSection>();
            entity.IsActive = true;
            await _genericDal.CreateAsync(entity);
            _logger.LogInformation("Feature created successfully. Id: {Id}", entity.Id);
        }

        public async Task TDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            await _genericDal.DeleteAsync(id);
            _logger.LogInformation("Feature deleted successfully. Id: {Id}", id);
        }

        public async Task<List<ResultFeatureDto>> TGetAllAsync()
        {
            var values = await _genericDal.GetListAsync();
            _logger.LogInformation("All features retrieved. Count: {Count}", values.Count);
            return values.Adapt<List<ResultFeatureDto>>();
        }

        public async Task<UpdateFeatureDto> TGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var entity = await _genericDal.GetByIdAsync(id);

            if (entity is null)
            {
                _logger.LogWarning("Feature not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Feature with Id {id} was not found.");
            }

            _logger.LogInformation("Fetched Feature by Id: {Id}", id);
            return entity.Adapt<UpdateFeatureDto>();
        }

        public async Task<List<ResultFeatureDto>> TGetFilteredListAsync(Expression<Func<FeatureSection, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = await _genericDal.GetFilteredListAsync(predicate);
            _logger.LogInformation("Filtered features retrieved. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultFeatureDto>>();
        }

        public async Task TUpdateAsync(UpdateFeatureDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _genericDal.GetByIdAsync(dto.Id);
            if (entity is null)
            {
                _logger.LogWarning("Feature not found. Id: {Id}", dto.Id);
                throw new KeyNotFoundException($"Feature with Id {dto.Id} was not found.");
            }

            dto.Adapt(entity);

            await _genericDal.UpdateAsync(entity);
            _logger.LogInformation("Feature updated successfully. Id: {Id}", dto.Id);
        }
    }
}
