using System.Linq.Expressions;
using VillaAgency.Dto.QuestionDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IQuestionService
    {
        Task TCreateAsync(CreateQuestionDto dto);
        Task TUpdateAsync(UpdateQuestionDto dto);
        Task TDeleteAsync(string id);

        Task<List<ResultQuestionDto>> TGetListAsync();
        Task<UpdateQuestionDto> TGetByIdAsync(string id);

        Task<List<ResultQuestionDto>> TGetFilteredListAsync(Expression<Func<Question, bool>> predicate);
    }
}
