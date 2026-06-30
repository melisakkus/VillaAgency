using DnsClient.Internal;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.DataAccess.Concrete.MongoDb.Driver;
using VillaAgency.Dto.VideoViewDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class VideoManager : IVideoService
    {
        private readonly IVideoDal _videoDal;
        private readonly ILogger<VideoManager> _logger;

        public VideoManager(IVideoDal videoDal, ILogger<VideoManager> logger)
        {
            _videoDal = videoDal;
            _logger = logger;
        }

        public async Task TCreateAsync(CreateVideoViewDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            var entity = dto.Adapt<VideoView>();
            entity.IsActive = false;
            await _videoDal.CreateAsync(entity);
            _logger.LogInformation("Video enitity added successfully. Id: {Id}", entity.Id);
        }

        public async Task TDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            await _videoDal.DeleteAsync(id);
            _logger.LogInformation("Video enitity deleted successfully. Id: {Id}", id);
        }

        public async Task<UpdateVideoViewDto> TGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var entity = await _videoDal.GetByIdAsync(id);

            if (entity is null)
            {
                _logger.LogWarning("Video enitity not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Video enitity with Id {id} was not found.");
            }

            _logger.LogInformation("Fetched Video enitity by Id: {Id}", id);
            return entity.Adapt<UpdateVideoViewDto>();
        }

        public async Task<List<ResultVideoViewDto>> TGetFilteredListAsync(Expression<Func<VideoView, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = await _videoDal.GetFilteredListAsync(predicate);
            _logger.LogInformation("Filtered Video enitities retrieved. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultVideoViewDto>>();
        }

        public async Task<List<ResultVideoViewDto>> TGetListAsync()
        {
            var values = await _videoDal.GetListAsync();
            _logger.LogInformation("All video enitities retrieved. Count: {Count}", values.Count);
            return values.Adapt<List<ResultVideoViewDto>>();
        }

        public async Task TMakeItActive(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _videoDal.MakeItActive(id);
            _logger.LogInformation("Video marked as active. Id: {Id}", id);
        }

        public async Task TMakeItPassive(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _videoDal.MakeItPassive(id);
            _logger.LogInformation("Video marked as passive. Id: {Id}", id);
        }

        public async Task TUpdateAsync(UpdateVideoViewDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _videoDal.GetByIdAsync(dto.Id);
            if (entity is null)
            {
                _logger.LogWarning("Video enitity not found. Id: {Id}", dto.Id);
                throw new KeyNotFoundException($"Video enitity with Id {dto.Id} was not found.");
            }

            dto.Adapt(entity);

            await _videoDal.UpdateAsync(entity);
            _logger.LogInformation("Video enitity updated successfully. Id: {Id}", dto.Id);
        }
    }
}
