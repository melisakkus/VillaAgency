using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Entity.Common;

namespace VillaAgency.Business.Concrete
{
    public class GenericManager<T> : IGenericService<T> where T : BaseEntity
    {
        private readonly IGenericDal<T> _genericDal;
        public GenericManager(IGenericDal<T> genericDal)
        {
            _genericDal = genericDal;
        }

        public async Task<int> TCountAsync()
        {
            return await _genericDal.CountAsync();
        }

        public async Task TCreateAsync(T entity)
        {
            await _genericDal.CreateAsync(entity);
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            await _genericDal.DeleteAsync(id);
        }

        public Task<T> TGetByIdAsync(string id)
        {
            return _genericDal.GetByIdAsync(id);
        }

        public Task<List<T>> TGetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            return _genericDal.GetFilteredListAsync(predicate);
        }

        public Task<List<T>> TGetListAsync()
        {
            return _genericDal.GetListAsync();
        }

        public async Task TUpdateAsync(T entity)
        {
            await _genericDal.UpdateAsync(entity);
        }
    }
}
