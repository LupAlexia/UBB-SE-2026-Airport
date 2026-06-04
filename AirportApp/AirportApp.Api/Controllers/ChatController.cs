using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirportApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;
        private readonly IUserService userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            this.chatService = chatService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetAllAsync()
        {
            IEnumerable<Chat> chats = await chatService.GetAllChatsAsync();
            return Ok(chats);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetByIdAsync(int id)
        {
            try
            {
                Chat chat = await chatService.GetChatByIdAsync(id);
                return Ok(chat);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateChatDTO chatCreationData)
        {
            if (chatCreationData.userId <= 0)
            {
                return BadRequest("A valid user id is required to open a chat.");
            }

            User user;
            try
            {
                user = await userService.GetByIdAsync(chatCreationData.userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with id {chatCreationData.userId} was not found.");
            }

            Chat chat = await chatService.OpenChatAsync(user);
            return Ok(new ChatDTO(chat.Id, chat.User.Id, chat.Status, chat.Messages.Count));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Chat chat)
        {
            if (id != chat.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await chatService.UpdateChatAsync(id, chat);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await chatService.CloseChatAsync(id);
            return NoContent();
        }
    }
}