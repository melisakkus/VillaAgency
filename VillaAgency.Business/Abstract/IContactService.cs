using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Dto.ContactDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IContactService
    {
        Task<List<ResultContactDto>> TGetAllAsync();
        Task<UpdateContactDto> TGetByIdAsync(string id);

        Task TCreateAsync(CreateContactDto dto);
        Task TUpdateAsync(UpdateContactDto dto);
        Task TDeleteAsync(string id);

        Task<List<ResultContactDto>> TGetFilteredListAsync(Expression<Func<Contact, bool>> predicate,int? page = null,
                                                                int? pageSize = null);
    }
}
