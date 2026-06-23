using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VillaAgency.Entity.Common;

namespace VillaAgency.Entity.Entities
{
    public class Product : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public string Title { get; set; }
        public int? BedroomCount { get; set; }
        public int? BathroomCount { get; set; }
        public int? Area { get; set; }
        public int? Floor { get; set; }
        public int? ParkingCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ProductStatus Status { get; set; }
    }

    public enum ProductStatus
    {
        Active,
        Sold,
        Rented,
        Archived
    }
}
