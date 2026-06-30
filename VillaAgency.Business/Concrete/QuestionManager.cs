using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Dto.QuestionDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class QuestionManager : IQuestionService
    {
        private readonly IGenericDal<Question> _genericDal;
        private readonly ILogger<QuestionManager> _logger;

        public QuestionManager(IGenericDal<Question> genericDal, ILogger<QuestionManager> logger)
        {
            _genericDal = genericDal;
            _logger = logger;
        }

        public async Task TCreateAsync(CreateQuestionDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            var entity = dto.Adapt<Question>();
            await _genericDal.CreateAsync(entity);
            _logger.LogInformation("Question created successfully. Id: {Id}", entity.Id);
        }

        public async Task TDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            await _genericDal.DeleteAsync(id);
            _logger.LogInformation("Question deleted successfully. Id: {Id}", id);
        }

        public async Task<UpdateQuestionDto> TGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var entity = await _genericDal.GetByIdAsync(id);

            if (entity is null)
            {
                _logger.LogWarning("Question not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Question with Id {id} was not found.");
            }

            _logger.LogInformation("Fetched Question by Id: {Id}", id);
            return entity.Adapt<UpdateQuestionDto>();
        }

        public async Task<List<ResultQuestionDto>> TGetFilteredListAsync(Expression<Func<Question, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = await _genericDal.GetFilteredListAsync(predicate);
            _logger.LogInformation("Filtered questions retrieved. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultQuestionDto>>();
        }

        public async Task<List<ResultQuestionDto>> TGetListAsync()
        {
            var values = await _genericDal.GetListAsync();
            _logger.LogInformation("All features retrieved. Count: {Count}", values.Count);
            return values.Adapt<List<ResultQuestionDto>>();
        }

        public async Task TUpdateAsync(UpdateQuestionDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _genericDal.GetByIdAsync(dto.Id);
            if (entity is null)
            {
                _logger.LogWarning("Question not found. Id: {Id}", dto.Id);
                throw new KeyNotFoundException($"Question with Id {dto.Id} was not found.");
            }

            dto.Adapt(entity);
            await _genericDal.UpdateAsync(entity);
            _logger.LogInformation("Question updated successfully. Id: {Id}", dto.Id);
        }
    }
}
