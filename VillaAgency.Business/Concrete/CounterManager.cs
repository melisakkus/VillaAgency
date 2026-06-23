using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;

namespace VillaAgency.Business.Concrete
{
    public class CounterManager : ICounterService
    {
        private readonly ICounterDal _counterDal;

        public CounterManager(ICounterDal counterDal)
        {
            _counterDal = counterDal;
        }

        public async Task<int> TGetActiveProductsCountAsync()
        {
            return await _counterDal.GetActiveProductsCountAsync();
        }

        public async Task<int> TGetAllProductsCountAsync()
        {
            return await _counterDal.GetAllProductsCountAsync();
        }

        public Task<int> TGetMessagesCount()
        {
            throw new NotImplementedException();
        }

        public async Task<int> TGetSoldProductsCountAsync()
        {
            return await _counterDal.GetSoldProductsCountAsync();
        }
    }
}
