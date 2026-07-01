using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Abstract
{
    public interface ICounterDal
    {
        Task<int> GetActiveProductsCountAsync();
        Task<int> GetAllProductsCountAsync();
        Task<int> GetSoldProductsCountAsync();


        Task<int> GetRentedProductsCountAsync();
        Task<int> GetUnReadMessagesCountAsync();
        Task<Dictionary<string, int>> GetProductCountsByCategoryAsync();

        Task<List<Message>> GetLastMessagesAsync(int count);
    }
}
