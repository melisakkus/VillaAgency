using VillaAgency.Entity.Common;

namespace VillaAgency.Entity.Entities
{
    public class Question : BaseEntity
    {
        public string QuestionText { get; set; }
        public string Answer { get; set; }
    }
}
