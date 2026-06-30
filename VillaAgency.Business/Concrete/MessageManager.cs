using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.MessageDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class MessageManager : IMessageService
    {
        private readonly IMessageDal _messageDal;
        private readonly ILogger<MessageManager> _logger;   

        public MessageManager(IMessageDal messageDal, ILogger<MessageManager> logger)
        {
            _messageDal = messageDal;
            _logger = logger;
        }

        public async Task TMarkAsDeletedAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _messageDal.MarkAsDeletedAsync(id);
            _logger.LogInformation("Message marked as deleted. Id: {Id}", id);
        }

        public async Task TMarkAsNotDeletedAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _messageDal.MarkAsNotDeletedAsync(id);
            _logger.LogInformation("Message marked as not deleted. Id: {Id}", id);
        }

        public async Task TMarkAsReadAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _messageDal.MarkAsReadAsync(id);
            _logger.LogInformation("Message marked as read. Id: {Id}", id);
        }

        public async Task TMarkAsNotReadAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _messageDal.MarkAsNotReadAsync(id);
            _logger.LogInformation("Message marked as not read. Id: {Id}", id);
        }

        public async Task<List<ResultMessageDto>> TGetFilteredListAsync(Expression<Func<Message, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            var values = await _messageDal.GetFilteredListAsync(predicate);
            _logger.LogInformation("Messages retrieved. Count: {Count}", values.Count);
            return values.Adapt<List<ResultMessageDto>>();
        }

        public async Task<int> TGetCountAsync(Expression<Func<Message, bool>> filter)
        {
            return await _messageDal.GetCountAsync(filter);
        }

        //create - delete - update - list -getbyid
        public async Task<UpdateMessageDto> TGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var entity = await _messageDal.GetByIdAsync(id);
            if (entity is null)
            {
                throw new KeyNotFoundException($"Message not found. Id: {id}");
            }
            _logger.LogInformation("Message found. Id: {Id}", id);
            return entity.Adapt<UpdateMessageDto>();

        }

        public async Task<List<ResultMessageDto>> TGetListAsync()
        {
            var values = await _messageDal.GetListAsync();
            _logger.LogInformation("Messages retrieved. Count: {Count}", values.Count);
            return values.Adapt<List<ResultMessageDto>>();
        }
        public async Task TCreateAsync(CreateMessageDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var entity = dto.Adapt<Message>();
            entity.IsRead = false;
            entity.MessageDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            await _messageDal.CreateAsync(entity);
            _logger.LogInformation("Message created successfully. Id: {Id}", entity.Id);
        }

        public async Task TDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _messageDal.DeleteAsync(id);
            _logger.LogInformation("Message deleted successfully. Id: {Id}", id);
        }       
        
        public async Task TUpdateAsync(UpdateMessageDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var existingMessage = await _messageDal.GetByIdAsync(dto.Id);
            if (existingMessage == null)
                throw new Exception("Message not found");

            existingMessage.Name = dto.Name;
            existingMessage.Email = dto.Email;
            existingMessage.Subject = dto.Subject;
            existingMessage.Content = dto.Content;

            await _messageDal.UpdateAsync(existingMessage);
            _logger.LogInformation("Message updated successfully. Id: {Id}", dto.Id);
        }
    }
}
