using MongoDB.Bson;
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
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(entity.Id));

            var result = await _collection.ReplaceOneAsync(filter, entity);

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
            return await _collection
                           .Find(Builders<T>.Filter.Empty)
                           .Sort(Builders<T>.Sort.Descending(x => x.Id))
                           .ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate,
                                                                int? page = null,
                                                                int? pageSize = null)
        {
            var query = _collection.Find(predicate).Sort(Builders<T>.Sort.Descending(x => x.Id));

            if (page.HasValue && pageSize.HasValue && page.Value > 0 && pageSize.Value > 0)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Limit(pageSize.Value);
            }

            return await query.ToListAsync();
        }
    }
}
