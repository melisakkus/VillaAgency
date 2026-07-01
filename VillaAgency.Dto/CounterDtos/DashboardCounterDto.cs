using VillaAgency.Dto.MessageDtos;

namespace VillaAgency.Dto.CounterDtos
{
    public class DashboardCounterDto
    {
        public int AllProductsCount { get; set; }
        public int ActiveProductsCount { get; set; }
        public int SoldProductsCount { get; set; }

        public int RentedProductsCount { get; set; }
        public int UnreadMessagesCount { get; set; }


        public Dictionary<string, int> CategoriesWithCount { get; set; }

        public List<ResultMessageDto> LastMessages { get; set; }

    }
}
