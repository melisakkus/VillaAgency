namespace VillaAgency.Business.Middleware_Reference
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } 
        public string Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
