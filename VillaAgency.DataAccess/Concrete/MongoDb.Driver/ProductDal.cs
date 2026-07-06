using MongoDB.Bson;
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


        public async Task<List<Product>> GetRandomProductPerCategoryAsync(int countPerCategory = 1)
        {
            int poolSize = countPerCategory == 1 ? 20 : 40;
            if (countPerCategory <= 1)
            {
                return await _collection.Aggregate()
                            .Match(Builders<Product>.Filter.Eq(p => p.Status, ProductStatus.Active))
                            .Sort(Builders<Product>.Sort.Descending(p => p.Id))
                            .Limit(20)
                            .Sample(20)
                            .Group(
                                p => p.Category,
                                g => g.First()
                            )
                            .ToListAsync();
            }

            var pipeline = new List<BsonDocument>
                        {
                            new BsonDocument("$match",
                                new BsonDocument("Status", ProductStatus.Active.ToString())),

                            new BsonDocument("$sample",
                                new BsonDocument("size", 200)),

                            new BsonDocument("$group",
                                new BsonDocument
                                {
                                    { "_id", "$Category" },
                                    { "AllProducts", new BsonDocument("$push","$$ROOT") }
                                }),

                            new BsonDocument("$project",
                                new BsonDocument
                                {
                                    { "Products",
                                        new BsonDocument("$slice",
                                            new BsonArray { "$AllProducts", countPerCategory })
                                    }
                                }),

                            new BsonDocument("$unwind","$Products"),

                            new BsonDocument("$replaceRoot",
                                new BsonDocument("newRoot","$Products"))
                        };

            return await _collection.Aggregate<Product>(pipeline).ToListAsync();
        }
    }
}
