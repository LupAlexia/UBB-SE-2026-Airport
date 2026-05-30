using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class MessageServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IMessageService
{
    private const string BaseUrl = "api/message";

    public async Task<BotMessage> SendMessageAsync(int chatId, Sender sender, FAQOption selectedOption)
    {
        var payload = new { ChatId = chatId, Sender = sender, SelectedOption = selectedOption };
        return await PostForResultAsync<object, BotMessage>($"{BaseUrl}/send", payload);
    }

    public async Task<IMessage> GetMessageAsync(int chatId, int messageId)
    {
        return await GetRequiredAsync<BotMessage>($"{BaseUrl}/chat/{chatId}/message/{messageId}");
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId)
    {
        return await GetListAsync<Message>($"{BaseUrl}/chat/{chatId}");
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await GetListAsync<Message>(BaseUrl);
    }

    public async Task<Message> GetByIdAsync(int id)
    {
        return await GetRequiredAsync<Message>($"{BaseUrl}/{id}");
    }

    public async Task<int> CreateMessageAsync(int chatId, int senderId, string text, DateTimeOffset timestamp)
    {
        var payload = new { ChatId = chatId, SenderId = senderId, Text = text, Timestamp = timestamp };
        return await PostForResultAsync<object, int>(BaseUrl, payload);
    }

    public async Task UpdateByIdAsync(int id, Message message)
    {
        await PutAsync($"{BaseUrl}/{id}", message);
    }

    public async Task DeleteByIdAsync(int id)
    {
        await DeleteAsync($"{BaseUrl}/{id}");
    }

    public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
    {
        return await GetListAsync<Message>($"{BaseUrl}/chat/{chatId}");
    }

    public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
    {
        return await GetListAsync<Message>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}");
    }
}
