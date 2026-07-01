using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver
{
    public class CounterDal : ICounterDal
    {
        private readonly MongoDbContext _context;

        public CounterDal(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUnReadMessagesCountAsync()
        {
            var filter = Builders<Message>.Filter.Eq(x => x.IsRead, false);
            var count = await _context.GetCollection<Message>().CountDocumentsAsync(filter);
            return (int)count;
        }

        public async Task<int> GetAllProductsCountAsync()
        {
            var count = await _context.GetCollection<Product>().CountDocumentsAsync(Builders<Product>.Filter.Empty);
            return (int)count;
        }

        public async Task<int> GetActiveProductsCountAsync()
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Status, ProductStatus.Active);
            var count = await _context.GetCollection<Product>().CountDocumentsAsync(filter);
            return (int)count;
        }

        public async Task<int> GetSoldProductsCountAsync()
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Status, ProductStatus.Sold);
            var count = await _context.GetCollection<Product>().CountDocumentsAsync(filter);
            return (int)count;
        }

        public async Task<int> GetRentedProductsCountAsync()
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Status, ProductStatus.Rented);
            var count = await _context.GetCollection<Product>().CountDocumentsAsync(filter);
            return (int)count;
        }
        public async Task<Dictionary<string, int>> GetProductCountsByCategoryAsync()
        {
            var _collection = _context.GetCollection<Product>();

            var counts = await _collection.Aggregate()
                    .Group(
                        x => x.Category, 
                        g => new CategoryCountResult
                        {
                            Category = g.Key,
                            Count = g.Count() 
                        }
                    )
                    .ToListAsync();
            return counts.ToDictionary(x => x.Category, x => x.Count);
        }

        public async Task<List<Message>> GetLastMessagesAsync(int count)
        {
            var _collection = _context.GetCollection<Message>();
            var filter = Builders<Message>.Filter.Eq(x => x.IsRead, false);
            var messages = await _collection.Find(filter)
                                            .Sort(Builders<Message>.Sort.Descending(x=>x.Id))
                                            .Limit(count).ToListAsync();
            return messages;
        }

        public class CategoryCountResult
        {
            [BsonId]
            public string Category { get; set; }
            public int Count { get; set; }
        }
    }
}
