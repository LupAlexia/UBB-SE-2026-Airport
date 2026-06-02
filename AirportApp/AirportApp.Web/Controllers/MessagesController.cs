using System.Security.Claims;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        private int? GetCurrentUserId()
        {
            string? claimUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimUserId, out int parsedId))
            {
                return parsedId;
            }

            return UserSession.CurrentUser?.Id;
        }

        private string? GetCurrentUserName()
        {
            return User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Name);
        }

        private int? ResolveChatId(int? chatId)
        {
            return chatId ?? 1;
        }

        private void SetCurrentUserViewData()
        {
            ViewBag.CurrentUserId = GetCurrentUserId();
            ViewBag.CurrentUserName = GetCurrentUserName();
        }

        // GET: Messages
        public async Task<IActionResult> Index(int? chatId)
        {
            SetCurrentUserViewData();

            IEnumerable<Message> messages;
            if (chatId.HasValue)
            {
                messages = await messageService.GetByChatIdAsync(chatId.Value);
            }
            else
            {
                messages = await messageService.GetAllAsync();
            }
            ViewBag.ChatId = chatId;
            return View(messages);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id, int? chatId)
        {
            SetCurrentUserViewData();
            ViewBag.ChatId = chatId;
            if (id == null)
            {
                return NotFound();
            }

            Message message = await messageService.GetByIdAsync(id.Value);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create(int? chatId)
        {
            SetCurrentUserViewData();
            ViewBag.ChatId = chatId;
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? chatId, [Bind("Id,Text,Timestamp")] Message message)
        {
            int? resolvedChatId = ResolveChatId(chatId);

            int? resolvedUserId = GetCurrentUserId();
            if (!resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "A user id is required to send a message.");
                SetCurrentUserViewData();
                ViewBag.ChatId = resolvedChatId;
                return View(message);
            }
            ModelState.Remove("Chat");
            ModelState.Remove("User");
            ModelState.Remove("User.Id");
            ModelState.Remove("Sender");

            if (!resolvedChatId.HasValue || !resolvedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "ChatId and UserId are required.");
                SetCurrentUserViewData();
                ViewBag.ChatId = resolvedChatId;
                return View(message);
            }

            if (ModelState.IsValid)
            {
                await messageService.CreateMessageAsync(
                    resolvedChatId.Value,
                    resolvedUserId.Value,
                    message.Text,
                    DateTimeOffset.UtcNow);
                    return RedirectToAction(nameof(Index), new { chatId = resolvedChatId });
            }
            SetCurrentUserViewData();
            ViewBag.ChatId = resolvedChatId;
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id, int? chatId)
        {
            SetCurrentUserViewData();
            if (id == null)
            {
                return NotFound();
            }

            Message message = await messageService.GetByIdAsync(id.Value);
            if (message == null)
            {
                return NotFound();
            }

            ViewBag.ChatId = chatId ?? message.Chat?.Id;
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, int? chatId, [Bind("Id,Text,Timestamp")] Message message)
        {
            SetCurrentUserViewData();
            if (id != message.Id)
            {
                return NotFound();
            }

            var existingMessage = await messageService.GetByIdAsync(message.Id);
            if (!chatId.HasValue)
            {
                chatId = existingMessage?.Chat?.Id;
            }
            // message.Sender = new User { Id = existingMessage.Sender.Id };
            // message.Chat = new Chat { Id = existingMessage.Chat.Id };
            message.Sender = existingMessage.Sender;
            message.Chat = existingMessage.Chat;
            message.Chat.User = new User(message.Sender.Id, message.Sender.FullName, message.Sender.EmailAddress);
            if (!chatId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "ChatId is required.");
                ViewBag.ChatId = null;
                return View(message);
            }

            ModelState.Remove(nameof(Message.Chat));
            ModelState.Remove(nameof(Message.Sender));

            if (ModelState.IsValid)
            {
                await messageService.UpdateByIdAsync((int)id, message);
                return RedirectToAction(nameof(Index), new { chatId });
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id, int? chatId)
        {
            SetCurrentUserViewData();
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.ChatId = chatId;

            var message = await messageService.GetByIdAsync(id.Value);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? chatId)
        {
            SetCurrentUserViewData();
            var message = await messageService.GetByIdAsync(id);
            if (message != null)
            {
                await messageService.DeleteByIdAsync(id);
            }

            return RedirectToAction(nameof(Index), new { chatId });
        }

        private async Task<bool> MessageExists(int id)
        {
            return await messageService.GetByIdAsync(id) != null;
        }
    }
}

