using VillaAgency.Entity.Common;

namespace VillaAgency.Entity.Entities
{
    public class FeatureSection : BaseEntity
    {
        public string ImageUrl { get; set; } 
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public List<FAQItem> FAQs { get; set; } = new List<FAQItem>();
        public List<FeatureDetail> FeatureDetails { get; set; } = new List<FeatureDetail>();

    }

    public class FeatureDetail
    {
        public string Icon { get; set; }  
        public string Title { get; set; }     
        public string SubTitle { get; set; }
    }

    public class FAQItem
    {
        public string Question { get; set; } 
        public string Answer { get; set; }  
    }
}
