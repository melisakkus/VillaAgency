using Mapster;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.BannerDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class BannerManager : IBannerService
    {
        private readonly IGenericDal<Banner> _genericDal;
        private readonly ILogger<BannerManager> _logger;

        public BannerManager(IGenericDal<Banner> genericDal, ILogger<BannerManager> logger)
        {
            _genericDal = genericDal ?? throw new ArgumentNullException(nameof(genericDal));
            _logger = logger;
        }

        public async Task TCreateAsync(CreateBannerDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");

            var entity = dto.Adapt<Banner>();
            await _genericDal.CreateAsync(entity);
            _logger.LogInformation("Banner created successfully. Id: {Id}", entity.Id);
        }

        public async Task TDeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                throw new ArgumentException("Invalid ObjectId.", nameof(id));
            }

            var entity = await _genericDal.GetByIdAsync(id);
            if (entity is null)
            {
                _logger.LogWarning("Banner to delete not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Banner with Id {id} was not found.");
            }

            await _genericDal.DeleteAsync(id);
            _logger.LogInformation("Banner deleted successfully. Id: {Id}", id);
        }

        public async Task<UpdateBannerDto> TGetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                throw new ArgumentException("Invalid ObjectId.", nameof(id));
            }

            var entity = await _genericDal.GetByIdAsync(id);

            if (entity is null)
            {
                _logger.LogWarning("Banner not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Banner with Id {id} was not found.");
            }

            _logger.LogInformation("Fetched banner by Id: {Id}", id);
            return entity.Adapt<UpdateBannerDto>();
        }

        public async Task<List<ResultBannerDto>> TGetFilteredListAsync(Expression<Func<Banner, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = await _genericDal.GetFilteredListAsync(predicate);
            _logger.LogInformation("Filtered banners retrieved. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultBannerDto>>();
        }

        public async Task<List<ResultBannerDto>> TGetListAsync()
        {
            var entities = await _genericDal.GetListAsync();
            _logger.LogInformation("All banners retrieved. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultBannerDto>>();
        }

        public async Task TUpdateAsync(UpdateBannerDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            if (!ObjectId.TryParse(dto.Id, out _))
            {
                throw new ArgumentException("Invalid ObjectId.", nameof(dto.Id));
            }

            var entity = await _genericDal.GetByIdAsync(dto.Id);

            if (entity is null)
            {
                _logger.LogWarning("Banner to update not found. Id: {Id}", dto.Id);
                throw new KeyNotFoundException($"Banner with Id {dto.Id} was not found.");
            }

            dto.Adapt(entity);
            await _genericDal.UpdateAsync(entity);
            _logger.LogInformation("Banner updated successfully. Id: {Id}", dto.Id);
        }
    }
}
