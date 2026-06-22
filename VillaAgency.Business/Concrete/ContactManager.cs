using Mapster;
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.ContactDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class ContactManager : IContactService
    {
        private readonly IGenericDal<Contact> _genericDal;
        public ContactManager(IGenericDal<Contact> genericDal)
        {
            _genericDal = genericDal ?? throw new ArgumentNullException(nameof(genericDal));
        }

        public async Task<int> TCountAsync()
        {
            return await _genericDal.CountAsync();
        }

        public async Task TCreateAsync(CreateContactDto dto)
        {
            if(dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            var entity = dto.Adapt<Contact>();
            await _genericDal.CreateAsync(entity);
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }
            await _genericDal.DeleteAsync(id);
        }

        public async Task<List<ResultContactDto>> TGetAllAsync()
        {
            var entities = await _genericDal.GetListAsync();
            return entities.Adapt<List<ResultContactDto>>();
        }

        public async Task<UpdateContactDto> TGetByIdAsync(ObjectId id)
        {
            if (id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }
            var entity = await _genericDal.GetByIdAsync(id);
            return entity.Adapt<UpdateContactDto>();
        }

        public async Task<List<ResultContactDto>> TGetFilteredListAsync(Expression<Func<Contact, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            var entities = await _genericDal.GetFilteredListAsync(predicate);
            return entities.Adapt<List<ResultContactDto>>();
        }

        public async Task TUpdateAsync(UpdateContactDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            if (dto.Id == ObjectId.Empty)
            {
                throw new ArgumentException("Entity to be updated must have a valid Id.");
            }
            var entity = dto.Adapt<Contact>();
            await _genericDal.UpdateAsync(entity);
        }
    }
}
