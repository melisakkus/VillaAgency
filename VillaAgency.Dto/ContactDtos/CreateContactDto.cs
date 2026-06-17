using MongoDB.Bson;

namespace VillaAgency.Dto.ContactDtos
{
    public class CreateContactDto
    {
        public string MapUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
