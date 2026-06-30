using System.Linq.Expressions;
using VillaAgency.Dto.ProductDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IProductService
    {
        Task TCreateAsync(CreateProductDto dto);
        Task TUpdateAsync(UpdateProductDto dto);
        Task TDeleteAsync(string id);

        Task<List<ResultProductDto>> TGetListAsync();
        Task<UpdateProductDto> TGetByIdAsync(string id);

        Task<List<ResultProductDto>> TGetPagedFilteredListAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>> predicate = null);
        Task<List<string>> TGetUniqueCategoriesAsync();
        Task TChangeStatusAsync(string id, string status);
    }
}
