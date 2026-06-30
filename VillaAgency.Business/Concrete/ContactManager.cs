using DnsClient.Internal;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Dto.ContactDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class ContactManager : IContactService
    {
        private readonly IGenericDal<Contact> _genericDal;
        private readonly ILogger<ContactManager> _logger;
        public ContactManager(IGenericDal<Contact> genericDal, ILogger<ContactManager> logger)
        {
            _genericDal = genericDal ?? throw new ArgumentNullException(nameof(genericDal));
            _logger = logger;
        }

        public async Task TCreateAsync(CreateContactDto dto)
        {
            if(dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }

            var entity = dto.Adapt<Contact>();
            await _genericDal.CreateAsync(entity);
            _logger.LogInformation("Contact created successfully.");
        }

        public async Task TDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            await _genericDal.DeleteAsync(id);
            _logger.LogInformation("Contact deleted successfully. Id: {Id}", id);
        }

        public async Task<List<ResultContactDto>> TGetAllAsync()
        {
            var entities = await _genericDal.GetListAsync();
            _logger.LogInformation("Retrieved all contacts. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultContactDto>>();
        }

        public async Task<UpdateContactDto> TGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var entity = await _genericDal.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Contact not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Contact with Id {id} was not found.");
            }

            _logger.LogInformation("Retrieved contact by Id: {Id}", id);
            return entity.Adapt<UpdateContactDto>();
        }

        public async Task<List<ResultContactDto>> TGetFilteredListAsync(Expression<Func<Contact, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = await _genericDal.GetFilteredListAsync(predicate);
            _logger.LogInformation("Retrieved filtered contacts. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultContactDto>>();
        }

        public async Task TUpdateAsync(UpdateContactDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _genericDal.GetByIdAsync(dto.Id);
            if (entity is null)
            {
                _logger.LogWarning("Contact not found. Id: {Id}", dto.Id);
                throw new KeyNotFoundException($"Contact with Id {dto.Id} was not found.");
            }

            dto.Adapt(entity);

            await _genericDal.UpdateAsync(entity);
            _logger.LogInformation("Contact updated successfully. Id: {Id}", dto.Id);
        }
    }
}