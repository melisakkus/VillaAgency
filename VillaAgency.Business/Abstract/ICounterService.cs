using System.Linq.Expressions;
using VillaAgency.Dto.MessageDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface ICounterService
    {
        Task<int> TGetActiveProductsCountAsync();
        Task<int> TGetAllProductsCountAsync(Expression<Func<Product, bool>>? predicate = null);
        Task<int> TGetSoldProductsCountAsync();
        Task<int> TGetRentedProductsCountAsync();
        Task<int> TGetUnReadMessagesCountAsync();
        Task<Dictionary<string, int>> TGetProductCountsByCategoryAsync();
        Task<List<ResultMessageDto>> TGetLastMessagesAsync(int count);

    }
}
