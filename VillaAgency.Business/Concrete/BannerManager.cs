using Mapster;
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.BannerDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class BannerManager : IBannerService
    {
        private readonly IGenericDal<Banner> _genericDal;
        public BannerManager(IGenericDal<Banner> genericDal)
        {
            _genericDal = genericDal ?? throw new ArgumentNullException(nameof(genericDal));
        }

        public async Task<int> TCountAsync()
        {
            return await _genericDal.CountAsync();
        }

        public async Task TCreateAsync(CreateBannerDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");

            try
            {
                var entity = dto.Adapt<Banner>();
                await _genericDal.CreateAsync(entity);
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while creating the banner.", ex);
            }
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));

            try
            {
                await _genericDal.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the banner.",ex);
            }
        }

        public async Task<UpdateBannerDto> TGetByIdAsync(ObjectId id)
        {
            if(id == ObjectId.Empty)
            {
                throw new ArgumentException("Invalid Id (Empty ObjectId).", nameof(id));
            }

            try
            {
                var entity = await _genericDal.GetByIdAsync(id);
                return entity.Adapt<UpdateBannerDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the banner.", ex);
            }
        }

        public async Task<List<ResultBannerDto>> TGetFilteredListAsync(Expression<Func<Banner, bool>> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            try
            {
                var entities = await _genericDal.GetFilteredListAsync(predicate);
                return entities.Adapt<List<ResultBannerDto>>();
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while retrieving the filtered list of banners.", ex);
            }
        }

        public async Task<List<ResultBannerDto>> TGetListAsync()
        {
            try
            {
                var entities = await _genericDal.GetListAsync();
                return entities.Adapt<List<ResultBannerDto>>();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the list of banners.", ex);
            }
        }

        public async Task TUpdateAsync(UpdateBannerDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null.");
            }
            if(dto.Id == ObjectId.Empty)
            {
                throw new ArgumentException("Entity to be updated must have a valid Id.");
            }

            try
            {
                var entity = dto.Adapt<Banner>();
                await _genericDal.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the banner.", ex);
            }
        }
    }
}
