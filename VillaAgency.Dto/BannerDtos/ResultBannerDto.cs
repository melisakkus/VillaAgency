using MongoDB.Bson;

namespace VillaAgency.Dto.BannerDtos
{
    public class ResultBannerDto : BaseBannerDto
    {
        public ObjectId Id { get; set; }

    }
}
