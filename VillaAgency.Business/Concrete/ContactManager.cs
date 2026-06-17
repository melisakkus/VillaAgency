using Mapster;
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.BannerDtos;
using VillaAgency.Dto.ContactDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class ContactManager : IContactService
    {
        private readonly IGenericDal<Contact> _genericDal;
        public ContactManager(IGenericDal<Contact> genericDal)
        {
            _genericDal = genericDal;
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
            try
            {
                var entity = dto.Adapt<Contact>();
                await _genericDal.CreateAsync(entity);
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while creating the contact.", ex);
            }            
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }

            try
            {
                await _genericDal.DeleteAsync(id);
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while deleting the contact.", ex);
            }
        }

        public async Task<List<ResultContactDto>> TGetAllAsync()
        {
            try
            {
                var entities = await _genericDal.GetListAsync();
                return entities.Adapt<List<ResultContactDto>>();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the list of contacts.", ex);
            }
        }

        public async Task<ResultContactDto> TGetByIdAsync(ObjectId id)
        {
            if (id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }

            try
            {
                var entity = await _genericDal.GetByIdAsync(id);
                return entity.Adapt<ResultContactDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the contact.", ex);
            }
        }

        public async Task<List<ResultContactDto>> TGetFilteredListAsync(Expression<Func<Contact, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            try
            {
                var entities = await _genericDal.GetFilteredListAsync(predicate);
                return entities.Adapt<List<ResultContactDto>>();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the filtered list of contacts.", ex);
            }
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

            try
            {
                var entity = dto.Adapt<Contact>();
                await _genericDal.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the contact.", ex);
            }
        }
    }
}
