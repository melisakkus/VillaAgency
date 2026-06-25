using MongoDB.Bson;

namespace VillaAgency.Dto.FeatureDtos
{
    public class ResultFeatureDto : BaseFeatureDto
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }

    }
}
