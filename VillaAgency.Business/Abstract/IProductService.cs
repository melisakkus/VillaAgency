using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Dto.ProductDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IProductService
    {
        Task TCreateAsync(CreateProductDto dto);
        Task TUpdateAsync(UpdateProductDto dto);
        Task TDeleteAsync(ObjectId id);

        Task<List<ResultProductDto>> TGetListAsync();
        Task<UpdateProductDto> TGetByIdAsync(ObjectId id);

        Task<int> TCountAsync();

        Task<List<ResultProductDto>> TGetFilteredListAsync(Expression<Func<Product, bool>> predicate);
    }
}
