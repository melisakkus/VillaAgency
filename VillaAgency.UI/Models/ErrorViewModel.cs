namespace VillaAgency.WebUI.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string UserMessage { get; set; } = "This operation cannot be performed at the moment. Please try again later.";

    }
}
