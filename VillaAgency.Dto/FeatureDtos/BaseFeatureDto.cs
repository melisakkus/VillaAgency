namespace VillaAgency.Dto.FeatureDtos
{
    public class BaseFeatureDto
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }

        public List<FAQItemDto> FAQs { get; set; } = new();
        public List<FeatureDetailDto> FeatureDetails { get; set; } = new();
    }
}
