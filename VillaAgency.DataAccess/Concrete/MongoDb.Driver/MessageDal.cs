using MongoDB.Driver;
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
    }
}
