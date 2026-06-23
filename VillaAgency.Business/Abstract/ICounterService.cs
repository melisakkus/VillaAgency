namespace VillaAgency.Business.Abstract
{
    public interface ICounterService
    {
        Task<int> TGetActiveProductsCountAsync();  
        Task<int> TGetAllProductsCountAsync();  
        Task<int> TGetSoldProductsCountAsync();

        Task<int> TGetMessagesCount();
       
        // Task<int> TGetMessages();
    }
}
