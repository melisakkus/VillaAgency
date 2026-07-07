using Mapster;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.MessageDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class CounterManager : ICounterService
    {
        private readonly ICounterDal _counterDal;

        public CounterManager(ICounterDal counterDal)
        {
            _counterDal = counterDal;
        }

        public async Task<Dictionary<string, int>> TGetProductCountsByCategoryAsync()
        {
            return await _counterDal.GetProductCountsByCategoryAsync();
        }

        public async Task<int> TGetRentedProductsCountAsync()
        {
            return await _counterDal.GetRentedProductsCountAsync();
        }

        public async Task<int> TGetUnReadMessagesCountAsync()
        {
            return await _counterDal.GetUnReadMessagesCountAsync();
        }

        public async Task<int> TGetActiveProductsCountAsync()
        {
            return await _counterDal.GetActiveProductsCountAsync();
        }

        public async Task<int> TGetAllProductsCountAsync(Expression<Func<Product, bool>>? predicate = null)
        {
            return await _counterDal.GetAllProductsCountAsync(predicate);
        }

        public async Task<int> TGetSoldProductsCountAsync()
        {
            return await _counterDal.GetSoldProductsCountAsync();
        }

        public async Task<List<ResultMessageDto>> TGetLastMessagesAsync(int count)
        {
            var values = await _counterDal.GetLastMessagesAsync(count);
            return values.Adapt<List<ResultMessageDto>>();  
        }
    }
}
