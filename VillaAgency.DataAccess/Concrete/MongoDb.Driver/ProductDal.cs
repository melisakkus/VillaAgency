using MongoDB.Driver;
using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Concrete.MongoDb.Driver.Common;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver
{
    public class ProductDal : GenericRepository<Product>, IProductDal
    {
        public ProductDal(MongoDbContext context) : base(context)
        {
        }

        public async Task ChangeStatusAsync(string id, ProductStatus status)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Id, id);
            var update = Builders<Product>.Update
                  .Set(x => x.Status, status)
                  .Set(x => x.UpdatedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Product with Id {id} not found.");
            }
        }

        public async Task<List<string>> GetUniqueCategoriesAsync()
        {
            var distinctCategories = await _collection
                                    .DistinctAsync<string>("Category", new MongoDB.Bson.BsonDocument());

            return await distinctCategories.ToListAsync();
        }
    }
}
