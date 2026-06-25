using MongoDB.Bson;

namespace VillaAgency.Dto.ProductDtos
{
    public class UpdateProductDto : BaseProductDto
    {
        public string Id { get; set; }

    }
}
