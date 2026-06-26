namespace VillaAgency.Dto.ContactDtos
{
    public class CreateContactDto
    {
        public string? MapUrl { get; set; }
        public string? Email { get; set; }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _phoneNumber = value;
                    return;
                }

                string cleaned = value.Replace(" ", "").Trim();

                if (cleaned.StartsWith("0"))
                {
                    cleaned = "+90" + cleaned.Substring(1);
                }
                else if (!cleaned.StartsWith("+"))
                {
                    cleaned = "+90" + cleaned;
                }

                _phoneNumber = cleaned;
            }
        }
    }
}
