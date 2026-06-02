using System.Security.Claims;
using System.Text.Json;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Web.Models;
using AirportApp.Web.Models.ChatBot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportApp.Web.Controllers
{
    [Authorize]
    public class ChatBotController : Controller
    {
        private readonly IChatService chatService;
        private readonly IMessageService messageService;
        private readonly IDecisionTreeService decisionTreeService;
        private const int FirstOptionNodeId = 1;

        public ChatBotController(
            IChatService chatService,
            IMessageService messageService,
            IDecisionTreeService decisionTreeService)
        {
            this.chatService = chatService;
            this.messageService = messageService;
            this.decisionTreeService = decisionTreeService;
        }

        private int? GetCurrentUserId()
        {
            string? claimUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimUserId, out int parsedId))
            {
                return parsedId;
            }

            return null;
        }

        public async Task<IActionResult> Index(int? chatId)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!chatId.HasValue)
            {
                var user = new User { Id = userId.Value };
                var newChat = await chatService.OpenChatAsync(user);
                chatId = newChat.Id;
            }

            var messages = await messageService.GetAllMessagesAsync(chatId.Value);
            var messageList = messages.ToList();

            List<FAQOptionDTO> currentOptions;

            if (TempData["CurrentOptions"] is string optionsJson && !string.IsNullOrEmpty(optionsJson))
            {
                currentOptions = JsonSerializer.Deserialize<List<FAQOptionDTO>>(optionsJson)
                    ?? new List<FAQOptionDTO>();
            }
            else if (!messageList.Any())
            {
                var firstNode = await decisionTreeService.GetNodeByIdAsync(FirstOptionNodeId);
                var firstOption = new FAQOption("Hello! I need help.", firstNode);
                var botReply = await messageService.SendMessageAsync(
                    chatId.Value,
                    new User { Id = userId.Value },
                    firstOption);

                messages = await messageService.GetAllMessagesAsync(chatId.Value);
                messageList = messages.ToList();

                currentOptions = botReply.FAQOptions?
                    .Select(o => new FAQOptionDTO
                    {
                        OptionId = o.OptionId,
                        Label = o.Label,
                        NextNodeId = o.NextOption?.NodeId
                    }).ToList() ?? new List<FAQOptionDTO>();

                if (!currentOptions.Any())
                {
                    var restartNode = await decisionTreeService.GetNodeByIdAsync(FirstOptionNodeId);
                    currentOptions = restartNode.Options.Select(o => new FAQOptionDTO
                    {
                        OptionId = o.OptionId,
                        Label = o.Label,
                        NextNodeId = o.NextOption?.NodeId
                    }).ToList();
                }
            }
            else
            {
                var firstNode = await decisionTreeService.GetNodeByIdAsync(FirstOptionNodeId);
                currentOptions = firstNode.Options.Select(o => new FAQOptionDTO
                {
                    OptionId = o.OptionId,
                    Label = o.Label,
                    NextNodeId = o.NextOption?.NodeId
                }).ToList();
            }

            var viewModel = new ChatBotModel
            {
                ChatId = chatId.Value,
                UserId = userId.Value,
                Messages = messageList.Select(m => new MessageDTO
                {
                    MessageId = m.Id,
                    MessageText = m.Text,
                    Timestamp = m.Timestamp,
                    SenderId = m.Sender?.RetrieveUniqueDatabaseIdentifierForBot() ?? 0,
                    IsOutgoing = (m.Sender?.RetrieveUniqueDatabaseIdentifierForBot() ?? 0) == userId.Value
                }).ToList(),
                CurrentOptions = currentOptions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOption(int chatId, string optionLabel, int? nextNodeId)
        {
            int? userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            FAQNode? nextNode = null;
            if (nextNodeId.HasValue)
            {
                nextNode = await decisionTreeService.GetNodeByIdAsync(nextNodeId.Value);
            }

            var option = new FAQOption(optionLabel, nextNode);
            var user = new User { Id = userId.Value };

            var botReply = await messageService.SendMessageAsync(chatId, user, option);

            List<FAQOptionDTO> nextOptions;
            if (botReply.FAQOptions != null && botReply.FAQOptions.Any())
            {
                nextOptions = botReply.FAQOptions.Select(option => new FAQOptionDTO
                {
                    OptionId = option.OptionId,
                    Label = option.Label,
                    NextNodeId = option.NextOption?.NodeId
                }).ToList();
            }
            else
            {
                var restartNode = await decisionTreeService.GetNodeByIdAsync(FirstOptionNodeId);
                nextOptions = new List<FAQOptionDTO>
                {
                    new FAQOptionDTO
                    {
                        OptionId = 0,
                        Label = "Restart Chat",
                        NextNodeId = restartNode.NodeId
                    }
                };
            }

            TempData["CurrentOptions"] = JsonSerializer.Serialize(nextOptions);
            return RedirectToAction(nameof(Index), new { chatId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndChat(int chatId)
        {
            await chatService.CloseChatAsync(chatId);
            return RedirectToAction("SupportDashboard", "Dashboard");
        }
    }
}

