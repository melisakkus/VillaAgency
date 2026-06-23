using Mapster;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProductManager> _logger;

        public ProductManager(IGenericDal<Product> genericDal, ILogger<ProductManager> logger)
        {
            _genericDal = genericDal ?? throw new ArgumentNullException(nameof(genericDal));
            _logger = logger;
        }

        public async Task<List<ResultProductDto>> TGetListAsync()
        {
            var entities = await _genericDal.GetListAsync();
            _logger.LogInformation("Retrieved all products. Count: {Count}", entities.Count);
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
            _logger.LogInformation("Product created successfully. Id: {Id}", entity.Id);
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }

            var entity = await _genericDal.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Product to delete not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Product with Id {id} was not found.");
            }

            if (entity.Status == ProductStatus.Archived)
            {
                _logger.LogInformation("Product is already archived. Id: {Id}", id);
                return;
            }

            entity.Status = ProductStatus.Archived;
            entity.UpdatedDate = DateTime.UtcNow;

            await _genericDal.UpdateAsync(entity);
            _logger.LogInformation("Product archived successfully. Id: {Id}", id);
        }

        public async Task<UpdateProductDto> TGetByIdAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }

            var value = await _genericDal.GetByIdAsync(id);
            if (value == null)
            {
                _logger.LogWarning("Product not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Product with Id {id} was not found.");
            }

            _logger.LogInformation("Fetched product by Id: {Id}", id);
            return value.Adapt<UpdateProductDto>();
        }

        public async Task<List<ResultProductDto>> TGetFilteredListAsync(Expression<Func<Product, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = await _genericDal.GetFilteredListAsync(predicate);
            _logger.LogInformation("Retrieved filtered products. Count: {Count}", entities.Count);
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
            {
                _logger.LogWarning("Product to update not found. Id: {Id}", dto.Id);
                throw new KeyNotFoundException($"Product with Id {dto.Id} was not found.");
            }

            dto.Adapt(entity);
            entity.UpdatedDate = DateTime.UtcNow;

            await _genericDal.UpdateAsync(entity);
            _logger.LogInformation("Product updated successfully. Id: {Id}", dto.Id);
        }
    }
}
