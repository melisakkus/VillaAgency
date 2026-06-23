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

        public async Task<int> GetActiveProductsCountAsync()
        {
            var filter = Builders<Product>.Filter.Eq(x=>x.Status,ProductStatus.Active);
            var count = await _context.GetCollection<Product>().CountDocumentsAsync(filter);
            return (int)count;
        }

        public async Task<int> GetAllProductsCountAsync()
        {
            var count = await _context.GetCollection<Product>().CountDocumentsAsync(Builders<Product>.Filter.Empty);
            return (int)count;
        }

        public async Task<int> GetSoldProductsCountAsync()
        {
            var filter = Builders<Product>.Filter.Eq(x=> x.Status,ProductStatus.Sold);
            var count = await _context.GetCollection<Product>().CountDocumentsAsync(filter);
            return (int)count;
        }
    }
}
