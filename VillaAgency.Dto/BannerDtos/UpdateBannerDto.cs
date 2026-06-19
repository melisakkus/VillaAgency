using MongoDB.Bson;

namespace VillaAgency.Dto.BannerDtos
{
    public class UpdateBannerDto : BaseBannerDto
    {
        public ObjectId Id { get; set; }

    }
}
