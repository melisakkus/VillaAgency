using MongoDB.Driver;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Context;

namespace VillaAgency.DataAccess.Repositories
{
    public class MongoRepository<T> : IRepository<T>
    {
        private readonly IMongoCollection<T> _collection;
        public MongoRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<T>();
        }

        public Task CreateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(string id, T entity)
        {
            throw new NotImplementedException();
        }
    }
}
