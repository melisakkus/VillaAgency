namespace VillaAgency.DataAccess.Abstract
{
    public interface ICounterDal
    {
        Task<int> GetActiveProductsCountAsync();
        Task<int> GetAllProductsCountAsync();
        Task<int> GetSoldProductsCountAsync();
    }
}
