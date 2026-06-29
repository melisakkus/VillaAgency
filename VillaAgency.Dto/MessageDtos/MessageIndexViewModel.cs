namespace VillaAgency.Dto.MessageDtos
{
    public class MessageIndexViewModel
    {
        public List<ResultMessageDto> AllMessages { get; set; }
        public List<ResultMessageDto> UnreadMessages { get; set; }
        public List<ResultMessageDto> DeletedMessages { get; set; }

        public int AllCount { get; set; }
        public int UnreadCount { get; set; }
        public int DeletedCount { get; set; }
    }
}
