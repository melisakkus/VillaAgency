using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Dto.ContactDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IContactService
    {
        Task<List<ResultContactDto>> TGetAllAsync();
        Task<UpdateContactDto> TGetByIdAsync(ObjectId id);

        Task TCreateAsync(CreateContactDto dto);
        Task TUpdateAsync(UpdateContactDto dto);
        Task TDeleteAsync(ObjectId id);

        Task<List<ResultContactDto>> TGetFilteredListAsync(Expression<Func<Contact, bool>> predicate);
    }
}
