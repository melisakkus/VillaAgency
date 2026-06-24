using MongoDB.Driver;
using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver
{
    public class ProductDal : GenericRepository<Product>, IProductDal
    {
        private readonly IMongoCollection<Product> _productCollection;
        public ProductDal(MongoDbContext context) : base(context)
        {
            _productCollection = context.GetCollection<Product>();
        }

        public async Task<List<Product>> GetPagedFilteredListAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>> predicate = null)
        {
            var filter = predicate ?? Builders<Product>.Filter.Empty;
            return await _productCollection.Find(filter)
                                        .SortByDescending(x=>x.CreatedAt)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Limit(pageSize)
                                        .ToListAsync();
        }

        public async Task<List<string>> GetUniqueCategoriesAsync()
        {
            var distinctCategories = await _productCollection
                                    .DistinctAsync<string>("Category", new MongoDB.Bson.BsonDocument());

            return await distinctCategories.ToListAsync();
        }
    }
}
