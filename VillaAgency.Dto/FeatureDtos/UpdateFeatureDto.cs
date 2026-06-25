using MongoDB.Bson;

namespace VillaAgency.Dto.FeatureDtos
{
    public class UpdateFeatureDto : BaseFeatureDto
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }

    }
}
