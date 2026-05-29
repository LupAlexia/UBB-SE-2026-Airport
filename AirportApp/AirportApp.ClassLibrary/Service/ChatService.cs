using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Service;

public class ChatService(IChatRepository chatRepository, IUserRepository userRepository) : IChatService
{
    public const int UNASSIGNED_CHAT_ID = 0;

    public async Task<Chat> OpenChatAsync(User userToOpenChatFor)
    {
        try
        {
            Chat newChat = new Chat(UNASSIGNED_CHAT_ID, userToOpenChatFor, ChatStatus.Active);
            int newIdentificationNumber = Convert.ToInt32(await chatRepository.AddAsync(newChat));
            newChat.Id = newIdentificationNumber;
            return newChat;
        }
        catch (Exception exceptionThrown)
        {
            throw new Exception(exceptionThrown.Message);
        }
    }

    public async Task CloseChatAsync(int chatId)
    {
        try
        {
            Chat chat = await chatRepository.GetByIdAsync(chatId) ?? throw new KeyNotFoundException($"Chat {chatId} not found.");
            chat.CloseChat();
            await chatRepository.UpdateAsync(chat);
        }
        catch (Exception exceptionThrown)
        {
            throw new Exception(exceptionThrown.Message);
        }
    }

    public async Task<IEnumerable<Chat>> GetAllChatsAsync()
    {
        try
        {
            return await chatRepository.GetAsync();
        }
        catch (Exception exceptionThrown)
        {
            throw new Exception(exceptionThrown.Message);
        }
    }

    public async Task<Chat> GetChatByIdAsync(int id)
    {
        try
        {
            return await chatRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Chat {id} not found.");
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception exceptionThrown)
        {
            throw new Exception(exceptionThrown.Message);
        }
    }

    public async Task UpdateChatAsync(int id, Chat chat)
    {
        try
        {
            await chatRepository.UpdateAsync(chat);
        }
        catch (Exception exceptionThrown)
        {
            throw new Exception(exceptionThrown.Message);
        }
    }
}
