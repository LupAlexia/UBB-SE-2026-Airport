using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
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
        var dto = await GetRequiredAsync<MessageDTO>($"{BaseUrl}/chat/{chatId}/message/{messageId}");
        return MapToEntity(dto);
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId)
    {
        var dtos = await GetListAsync<MessageDTO>($"{BaseUrl}/chat/{chatId}");
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        var dtos = await GetListAsync<MessageDTO>(BaseUrl);
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<Message> GetByIdAsync(int id)
    {
        var dto = await GetRequiredAsync<MessageDTO>($"{BaseUrl}/{id}");
        return MapToEntity(dto);
    }

    public async Task<int> CreateMessageAsync(int chatId, int senderId, string text, DateTimeOffset timestamp)
    {
        var payload = new { ChatId = chatId, SenderId = senderId, Text = text, Timestamp = timestamp };
        return await PostForResultAsync<object, int>(BaseUrl, payload);
    }

    public async Task UpdateByIdAsync(int id, Message message)
    {
        await PutAsync($"{BaseUrl}/{id}", MapToDto(message));
    }

    public async Task DeleteByIdAsync(int id)
    {
        await DeleteAsync($"{BaseUrl}/{id}");
    }

    public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
    {
        var dtos = await GetListAsync<MessageDTO>($"{BaseUrl}/chat/{chatId}");
        return dtos.Select(MapToEntity).ToList();
    }

    public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
    {
        var dtos = await GetListAsync<MessageDTO>($"{BaseUrl}/chat/{chatId}/since/{firstMessageId}");
        return dtos.Select(MapToEntity).ToList();
    }

    private static Message MapToEntity(MessageDTO dto)
    {
        return new Message(dto.MessageId, dto.Sender as Sender ?? new User { Id = dto.SenderId }, new Chat { Id = dto.ChatId }, dto.MessageText, dto.Timestamp);
    }

    private static MessageDTO MapToDto(Message message)
    {
        return new MessageDTO(message.Id, message.Chat?.Id ?? 0, message.Sender?.Id ?? 0, message.Sender, message.Text, message.Timestamp, new List<FAQOption>());
    }
}
