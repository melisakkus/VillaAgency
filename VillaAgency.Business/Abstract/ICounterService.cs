using VillaAgency.Dto.MessageDtos;

namespace VillaAgency.Business.Abstract
{
    public interface ICounterService
    {
        Task<int> TGetActiveProductsCountAsync();  
        Task<int> TGetAllProductsCountAsync();  
        Task<int> TGetSoldProductsCountAsync();
        Task<int> TGetRentedProductsCountAsync();
        Task<int> TGetUnReadMessagesCountAsync();
        Task<Dictionary<string, int>> TGetProductCountsByCategoryAsync();
        Task<List<ResultMessageDto>> TGetLastMessagesAsync(int count);

    }
}
