using MongoDB.Bson;

namespace VillaAgency.Dto.ContactDtos
{
    public class ResultContactDto
    {
        public ObjectId Id { get; set; }
        public string MapUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
