using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VillaAgency.Entity.Common
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
