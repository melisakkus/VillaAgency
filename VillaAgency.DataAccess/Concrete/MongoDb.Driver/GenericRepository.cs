using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Common;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver
{
    public class GenericRepository<T> : IGenericDal<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;
        public GenericRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var id = entity.Id;
            var result = await _collection.ReplaceOneAsync(x => x.Id == id, entity);
            if (result.MatchedCount == 0)
            {
                throw new Exception("Update failed: entity not found.");
            }
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
       public async Task<List<T>> GetListAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task<T> GetByIdAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("_id",id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }
    }
}
