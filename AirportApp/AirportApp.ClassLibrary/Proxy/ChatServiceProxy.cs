using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ChatServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IChatService
{
    private const string BaseUrl = "api/chat";

    public async Task<Chat> OpenChatAsync(User userToOpenChatFor)
    {
        var req = new CreateChatDTO(userToOpenChatFor.Id, ChatStatus.Open);
        var dto = await PostForResultAsync<CreateChatDTO, ChatDTO>($"{BaseUrl}/open", req);
        return MapToEntity(dto);
    }

    public async Task CloseChatAsync(int chatId)
    {
        await PostAsync<object>($"{BaseUrl}/{chatId}/close", null!);
    }

    public async Task<IEnumerable<Chat>> GetAllChatsAsync()
    {
        var dtos = await GetListAsync<ChatDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Chat> GetChatByIdAsync(int id)
    {
        var dto = await GetRequiredAsync<ChatDTO>($"{BaseUrl}/{id}");
        return MapToEntity(dto);
    }

    public async Task UpdateChatAsync(int id, Chat chat)
    {
        var dto = new ChatDTO(chat.Id, chat.User.Id, chat.Status, chat.Messages.Count);
        await PutAsync($"{BaseUrl}/{id}", dto);
    }

    private static Chat MapToEntity(ChatDTO dto)
    {
        return new Chat(dto.chatId, new User { Id = dto.userId }, dto.status);
    }
}
