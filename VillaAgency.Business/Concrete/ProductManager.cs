using Mapster;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.ProductDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductDal _productDal;
        private readonly ILogger<ProductManager> _logger;

        public ProductManager(IProductDal productDal, ILogger<ProductManager> logger)
        {
            _productDal = productDal ?? throw new ArgumentNullException(nameof(_productDal));
            _logger = logger;
        }

        public async Task<List<ResultProductDto>> TGetListAsync()
        {
            var entities = await _productDal.GetListAsync();
            _logger.LogInformation("Retrieved all products. Count: {Count}", entities.Count);
            return entities.Adapt<List<ResultProductDto>>();
        }

        public async Task TCreateAsync(CreateProductDto dto)
        {
            if(dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }

            var entity = dto.Adapt<Product>();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedDate = null;
            entity.Floor ??= 0;
            await _productDal.CreateAsync(entity);
            _logger.LogInformation("Product created successfully. Id: {Id}", entity.Id);
        }

        public async Task TDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var entity = await _productDal.GetByIdAsync(id);

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

            await _productDal.UpdateAsync(entity);
            _logger.LogInformation("Product archived successfully. Id: {Id}", id);
        }

        public async Task<UpdateProductDto> TGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var value = await _productDal.GetByIdAsync(id);
            if (value == null)
            {
                _logger.LogWarning("Product not found. Id: {Id}", id);
                throw new KeyNotFoundException($"Product with Id {id} was not found.");
            }

            _logger.LogInformation("Fetched product by Id: {Id}", id);
            return value.Adapt<UpdateProductDto>();
        }

        public async Task TUpdateAsync(UpdateProductDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _productDal.GetByIdAsync(dto.Id);
            if (entity is null)
            {
                _logger.LogWarning("Product not found. Id: {Id}", dto.Id);
                throw new KeyNotFoundException($"Product with Id {dto.Id} was not found.");
            }

            dto.Adapt(entity);
            entity.UpdatedDate = DateTime.UtcNow;
            await _productDal.UpdateAsync(entity);
            _logger.LogInformation("Product updated successfully. Id: {Id}", dto.Id);
        }

        public async Task<List<ResultProductDto>> TGetPagedFilteredListAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>> predicate = null)
        {
            var entities = await _productDal.GetPagedFilteredListAsync(pageNumber, pageSize, predicate);
            _logger.LogInformation("Retrieved products for page {PageNumber}. Count: {Count}", pageNumber, entities.Count);
            return entities.Adapt<List<ResultProductDto>>();
        }

        public async Task<List<string>> TGetUniqueCategoriesAsync()
        {
            return await _productDal.GetUniqueCategoriesAsync();
        }

        public async Task TChangeStatusAsync(string id, string status)
        {
            if (!Enum.TryParse<ProductStatus>(status, true, out var enumStatus))
            {
                throw new ArgumentException("Invalid status value.", nameof(status));
            }

            await _productDal.ChangeStatusAsync(id, enumStatus);
        }
    }
}
