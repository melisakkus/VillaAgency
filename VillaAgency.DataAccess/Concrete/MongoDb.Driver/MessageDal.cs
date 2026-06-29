using MongoDB.Driver;
using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Concrete.MongoDb.Driver.Common;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver
{
    public class MessageDal : GenericRepository<Message>, IMessageDal
    {

        public MessageDal(MongoDbContext context) : base(context)
        {
        }

        public async Task MarkAsReadAsync(string id)
        {
            var update = Builders<Message>.Update.Set(x=>x.IsRead,true);
            await _collection.UpdateOneAsync(x=>x.Id==id,update);
        }

        public async Task MarkAsNotReadAsync(string id)
        {
            var update = Builders<Message>.Update.Set(x => x.IsRead, false);
            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task MarkAsDeletedAsync(string id)
        {
            var update = Builders<Message>.Update.Set(x => x.IsDeleted, true);
            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task MarkAsNotDeletedAsync(string id)
        {
            var update = Builders<Message>.Update.Set(x => x.IsDeleted, false);
            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task<int> GetCountAsync(Expression<Func<Message, bool>> filter)
        {
            var count = await _collection.CountDocumentsAsync(filter);
            return (int)count;
        }
    }
}
