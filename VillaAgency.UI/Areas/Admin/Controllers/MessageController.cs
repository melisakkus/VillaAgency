using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;
using VillaAgency.Dto.MessageDtos;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    public class MessageController : AdminBaseController
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        public async Task<IActionResult> Index(string activeTab = "all-messages", int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            ViewBag.ActiveTab = activeTab;
            ViewBag.CurrentPage = page;

            var allCount = await _messageService.TGetCountAsync(x => !x.IsDeleted);
            var unreadCount = await _messageService.TGetCountAsync(x => !x.IsDeleted && !x.IsRead);
            var deletedCount = await _messageService.TGetCountAsync(x => x.IsDeleted);

            int targetCount = activeTab switch
            {
                "unread-messages" => unreadCount,
                "deleted-messages" => deletedCount,
                _ => allCount
            };

            int totalPages = (int)Math.Ceiling((double)targetCount / pageSize);
            if (totalPages == 0) totalPages = 1;

            var model = new MessageIndexViewModel
            {
                AllCount = allCount,
                UnreadCount = unreadCount,
                DeletedCount = deletedCount,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,

                AllMessages = activeTab == "all-messages"
                    ? await _messageService.TGetFilteredListAsync(x => !x.IsDeleted, page, pageSize)
                    : new List<ResultMessageDto>(),

                UnreadMessages = activeTab == "unread-messages"
                    ? await _messageService.TGetFilteredListAsync(x => !x.IsRead && !x.IsDeleted, page, pageSize)
                    : new List<ResultMessageDto>(),

                DeletedMessages = activeTab == "deleted-messages"
                    ? await _messageService.TGetFilteredListAsync(x => x.IsDeleted, page, pageSize)
                    : new List<ResultMessageDto>()
            };

            return View(model);
        }

        public async Task<IActionResult> MarkAsRead(string id)
        {
            await _messageService.TMarkAsReadAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MarkAsNotRead(string id)
        {
            await _messageService.TMarkAsNotReadAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MarkAsNotDeleted(string id)
        {
            await _messageService.TMarkAsNotDeletedAsync(id);
            return RedirectToAction("Index");
        }

        // delete(mark delete) - create - update
        public async Task<IActionResult> Delete(string id)
        {
            await _messageService.TMarkAsDeletedAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _messageService.TCreateAsync(dto);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string id, string activeTab = "all-messages")
        {
            ViewBag.ActiveTab = activeTab;
            var value = await _messageService.TGetByIdAsync(id);
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateMessageDto dto, string activeTab = "all-messages")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActiveTab = activeTab;
                return View(dto);
            }

            await _messageService.TUpdateAsync(dto);
            return RedirectToAction("Index", new { activeTab = activeTab });
        }
    }
}
