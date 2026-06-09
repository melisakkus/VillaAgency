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
            _genericDal = genericDal ?? throw new ArgumentNullException(nameof(genericDal));
        }

        public async Task<int> TCountAsync()
        {
            return await _genericDal.CountAsync();
        }

        public async Task TCreateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity), "Entity boş olamaz.");

            // MongoDB için Id kontrolü (isteğe bağlı ama iyi pratik)
            if (entity.Id == ObjectId.Empty)
                entity.Id = ObjectId.GenerateNewId();

            await _genericDal.CreateAsync(entity);
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            if (id == ObjectId.Empty)
                throw new ArgumentException("Geçersiz Id (Empty ObjectId).", nameof(id));

            await _genericDal.DeleteAsync(id);
        }

        public async Task<T> TGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id boş olamaz.", nameof(id));

            if (!ObjectId.TryParse(id, out var objectId))
                throw new FormatException("Id geçerli bir ObjectId formatında değil.");

            return await _genericDal.GetByIdAsync(objectId.ToString());
        }

        public async Task<List<T>> TGetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return await _genericDal.GetFilteredListAsync(predicate);
        }

        public async Task<List<T>> TGetListAsync()
        {
            return await _genericDal.GetListAsync();
        }

        public async Task TUpdateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity), "Entity boş olamaz.");

            if (entity.Id == ObjectId.Empty)
                throw new ArgumentException("Güncellenecek entity'nin Id değeri olmalı.");

            await _genericDal.UpdateAsync(entity);
        }
    }
}