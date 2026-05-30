using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class ChatServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IChatService
{
    private const string BaseUrl = "api/chat";

    public async Task<Chat> OpenChatAsync(User userToOpenChatFor)
    {
        return await PostForResultAsync<User, Chat>($"{BaseUrl}/open", userToOpenChatFor);
    }

    public async Task CloseChatAsync(int chatId)
    {
        await PostAsync<object>($"{BaseUrl}/{chatId}/close", null!);
    }

    public async Task<IEnumerable<Chat>> GetAllChatsAsync()
    {
        return await GetListAsync<Chat>(BaseUrl);
    }

    public async Task<Chat> GetChatByIdAsync(int id)
    {
        return await GetRequiredAsync<Chat>($"{BaseUrl}/{id}");
    }

    public async Task UpdateChatAsync(int id, Chat chat)
    {
        await PutAsync($"{BaseUrl}/{id}", chat);
    }
}
