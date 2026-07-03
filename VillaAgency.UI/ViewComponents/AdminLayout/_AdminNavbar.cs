using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.ViewComponents.AdminLayout
{
    public class _AdminNavbar : ViewComponent
    {
        private readonly IMessageService _messageService;

        public _AdminNavbar(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var unreadMessages = await _messageService.TGetFilteredListAsync((x=>x.IsRead == false && x.IsDeleted == false ),1,5);
            var allMessagesCount = await _messageService.TGetCountAsync(x=>x.IsRead == false && x.IsDeleted ==false );
            return View(Tuple.Create(unreadMessages, allMessagesCount));
        }
    }
}
