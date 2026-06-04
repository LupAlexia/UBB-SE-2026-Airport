using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class ChatServiceTests
{
    private const int ValidChatId = 1;
    private const int InvalidChatId = 99;
    private const int NewChatId = 42;

    private static User CreateUser() => new User(1, "John Doe", "john@example.com");

    [Test]
    public async Task OpenChatAsync_ReturnsActiveChatWithIdFromRepository_WhenRepositorySucceeds()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        chatRepository.AddAsync(Arg.Any<Chat>()).Returns(Task.FromResult(NewChatId));
        var service = new ChatService(chatRepository, userRepository);

        var result = await service.OpenChatAsync(CreateUser());

        Assert.That(result.Id, Is.EqualTo(NewChatId));
        Assert.That(result.Status, Is.EqualTo(ChatStatus.Active));
    }

    [Test]
    public async Task OpenChatAsync_CallsRepositoryAdd_WhenRepositorySucceeds()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        chatRepository.AddAsync(Arg.Any<Chat>()).Returns(Task.FromResult(NewChatId));
        var service = new ChatService(chatRepository, userRepository);

        await service.OpenChatAsync(CreateUser());

        await chatRepository.Received(1).AddAsync(Arg.Any<Chat>());
    }

    [Test]
    public void CloseChatAsync_ThrowsException_WhenChatDoesNotExist()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        chatRepository.GetByIdAsync(InvalidChatId).Returns(Task.FromResult<Chat?>(null));
        var service = new ChatService(chatRepository, userRepository);

        Assert.ThrowsAsync<Exception>(() => service.CloseChatAsync(InvalidChatId));
    }

    [Test]
    public async Task CloseChatAsync_SetsChatStatusToClosed_WhenChatExists()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var chat = new Chat(ValidChatId, CreateUser(), ChatStatus.Active);
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        var service = new ChatService(chatRepository, userRepository);

        await service.CloseChatAsync(ValidChatId);

        Assert.That(chat.Status, Is.EqualTo(ChatStatus.Closed));
    }

    [Test]
    public async Task CloseChatAsync_CallsRepositoryUpdate_WhenChatExists()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var chat = new Chat(ValidChatId, CreateUser(), ChatStatus.Active);
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        var service = new ChatService(chatRepository, userRepository);

        await service.CloseChatAsync(ValidChatId);

        await chatRepository.Received(1).UpdateAsync(chat);
    }

    [Test]
    public void GetChatByIdAsync_ThrowsKeyNotFoundException_WhenChatDoesNotExist()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        chatRepository.GetByIdAsync(InvalidChatId).Returns(Task.FromResult<Chat?>(null));
        var service = new ChatService(chatRepository, userRepository);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetChatByIdAsync(InvalidChatId));
    }

    [Test]
    public async Task GetChatByIdAsync_ReturnsChat_WhenChatExists()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var chat = new Chat(ValidChatId, CreateUser(), ChatStatus.Active);
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        var service = new ChatService(chatRepository, userRepository);

        var result = await service.GetChatByIdAsync(ValidChatId);

        Assert.That(result, Is.EqualTo(chat));
    }
}
