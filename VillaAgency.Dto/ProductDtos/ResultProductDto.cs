using MongoDB.Bson;

namespace VillaAgency.Dto.ProductDtos
{
    public class ResultProductDto : BaseProductDto
    {
        public ObjectId Id { get; set; }
    }
}
