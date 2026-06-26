using MongoDB.Driver;
using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Common;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver.Common
{
    public class GenericRepository<T> : IGenericDal<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
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
            var result = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Update failed because no data was found. Id: {entity.Id}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount == 0)
                throw new KeyNotFoundException($"Entity not found. Id: {id}");
        }
       public async Task<List<T>> GetListAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }
    }
}
