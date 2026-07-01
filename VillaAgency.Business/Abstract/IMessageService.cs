using System.Linq.Expressions;
using VillaAgency.Dto.MessageDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IMessageService
    {
        Task TCreateAsync(CreateMessageDto dto);
        Task TUpdateAsync(UpdateMessageDto dto);
        Task TDeleteAsync(string id);

        Task<List<ResultMessageDto>> TGetListAsync();
        Task<UpdateMessageDto> TGetByIdAsync(string id);

        Task TMarkAsReadAsync(string id);
        Task TMarkAsNotReadAsync(string id);
        Task TMarkAsDeletedAsync(string id);
        Task TMarkAsNotDeletedAsync(string id);
        Task<int> TGetCountAsync(Expression<Func<Message, bool>> filter);
        Task<List<ResultMessageDto>> TGetFilteredListAsync(
                    Expression<Func<Message, bool>> predicate,
                    int? page = null,
                    int? pageSize = null);
    }
}
