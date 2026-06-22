using Mapster;
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.ProductDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IGenericDal<Product> _genericDal;


        public ProductManager(IGenericDal<Product> genericDal)
        {
            _genericDal = genericDal ?? throw new ArgumentNullException(nameof(genericDal));
        }

        public async Task<List<ResultProductDto>> TGetListAsync()
        {
            var entities = await _genericDal.GetListAsync();
            return entities.Adapt<List<ResultProductDto>>();
        }

        public async Task<int> TCountAsync()
        {
            return await _genericDal.CountAsync();
        }

        public async Task TCreateAsync(CreateProductDto dto)
        {
            if(dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            var entity = dto.Adapt<Product>();
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = null;
            entity.Status = ProductStatus.Active;
            await _genericDal.CreateAsync(entity);
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }
            var entity = await _genericDal.GetByIdAsync(id);
            entity.Status = ProductStatus.Archived;
            entity.UpdatedDate = DateTime.UtcNow;
            await _genericDal.UpdateAsync(entity);
        }

        public async Task<UpdateProductDto> TGetByIdAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }
            var value = await _genericDal.GetByIdAsync(id);
            return value.Adapt<UpdateProductDto>();
        }

        public async Task<List<ResultProductDto>> TGetFilteredListAsync(Expression<Func<Product, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            var entities = await _genericDal.GetFilteredListAsync(predicate);
            return entities.Adapt<List<ResultProductDto>>();
        }

        public async Task TUpdateAsync(UpdateProductDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            if (dto.Id == ObjectId.Empty)
            {
                throw new ArgumentException("Entity to be updated must have a valid Id.");
            }
            var entity = await _genericDal.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new Exception("Product not found");
            dto.Adapt(entity);
            entity.UpdatedDate = DateTime.UtcNow;
            await _genericDal.UpdateAsync(entity);
        }
    }
}
