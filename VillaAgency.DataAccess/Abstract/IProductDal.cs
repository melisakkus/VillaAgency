using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Abstract
{
    public interface IProductDal : IGenericDal<Product>
    {
        Task<List<Product>> GetPagedFilteredListAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>> predicate = null);
        Task<List<string>> GetUniqueCategoriesAsync();
    }
}
