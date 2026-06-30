using MongoDB.Driver;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Concrete.MongoDb.Driver.Common;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver
{
    public class VideoDal : GenericRepository<VideoView>, IVideoDal
    {
        public VideoDal(MongoDbContext context) : base(context)
        {
        }

        public async Task MakeItActive(string id)
        {
            var update = Builders<VideoView>.Update.Set(x=>x.IsActive,true);
            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }

        public async Task MakeItPassive(string id)
        {
            var update = Builders<VideoView>.Update.Set(x => x.IsActive, false);
            await _collection.UpdateOneAsync(x => x.Id == id, update);
        }
    }
}
