using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Entity.Common;

namespace VillaAgency.Business.Abstract
{
    public interface IGenericService<T> where T : BaseEntity
    {
        Task TCreateAsync(T entity);
        Task TUpdateAsync(T entity);
        Task TDeleteAsync(ObjectId id);

        Task<List<T>> TGetListAsync();
        Task<T> TGetByIdAsync(string id);

        Task<int> TCountAsync();

        Task<List<T>> TGetFilteredListAsync(Expression<Func<T, bool>> predicate);
    }
}
