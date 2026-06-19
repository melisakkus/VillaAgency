using MongoDB.Bson;

namespace VillaAgency.Dto.ProductDtos
{
    public class UpdateProductDto : BaseProductDto
    {
        public ObjectId Id { get; set; }

    }
}
