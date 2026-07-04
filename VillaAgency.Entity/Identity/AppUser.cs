using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace VillaAgency.Entity.Identity
{
    [CollectionName("Users")]
    public class AppUser : MongoIdentityUser<string>
    {
        public string FullName { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
