using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Abstract
{
    public interface IProductDal : IGenericDal<Product>
    {
        Task<List<string>> GetUniqueCategoriesAsync();
        Task ChangeStatusAsync(string id, ProductStatus status);
    }
}
