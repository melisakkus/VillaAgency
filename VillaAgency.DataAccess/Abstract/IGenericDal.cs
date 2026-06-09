using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Entity.Common;

namespace VillaAgency.DataAccess.Abstract
{
    public interface IGenericDal<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(ObjectId id);

        Task<List<T>> GetListAsync();
        Task<T> GetByIdAsync(string id);

        Task<int> CountAsync();

        Task<List<T>> GetFilteredListAsync(Expression<Func<T,bool>> predicate);
    }
}
