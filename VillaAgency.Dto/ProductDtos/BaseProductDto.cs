namespace VillaAgency.Dto.ProductDtos
{
    public class BaseProductDto
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

        public ProductStatusDto? Status { get; set; }
    }

    public enum ProductStatusDto
    {
        Active,
        Sold,
        Rented,
        Archived
    }
}
