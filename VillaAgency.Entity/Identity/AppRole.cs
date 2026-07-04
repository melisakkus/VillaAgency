using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace VillaAgency.Entity.Identity
{
    [CollectionName("Roles")]
    public class AppRole : MongoIdentityRole<string>
    {
        public AppRole() : base() { }
        public AppRole(string roleName) : base(roleName) { }
        }
}
