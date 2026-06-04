using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interface;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class MessageServiceTests
{
    private const int ValidChatId = 1;
    private const int InvalidChatId = 99;
    private const int ValidMessageId = 10;
    private const int InvalidMessageId = 99;
    private const int ValidSenderId = 5;
    private const int OtherChatId = 2;
    private const string MessageText = "Hello";

    private static User CreateUser() => new User(1, "John Doe", "john@example.com");

    private static Chat CreateActiveChat(int id = ValidChatId) =>
        new Chat(id, new User(1, "John Doe", "john@example.com"), ChatStatus.Active);

    private static (IChatRepository, IMessageRepository, IDecisionTreeService, BotEngineIdentity, MessageService) CreateService()
    {
        var chatRepository = Substitute.For<IChatRepository>();
        var messageRepository = Substitute.For<IMessageRepository>();
        var decisionTreeService = Substitute.For<IDecisionTreeService>();
        var botStrategy = Substitute.For<IBotStrategy>();
        var botEngine = new BotEngineIdentity(botStrategy);
        var service = new MessageService(chatRepository, messageRepository, decisionTreeService, botEngine);
        return (chatRepository, messageRepository, decisionTreeService, botEngine, service);
    }

    [Test]
    public void SendMessageAsync_SelectedOptionIsNull_ThrowsArgumentNullException()
    {
        var (_, _, _, _, service) = CreateService();

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendMessageAsync(ValidChatId, CreateUser(), null!));
    }

    [Test]
    public void SendMessageAsync_ChatDoesNotExist_ThrowsKeyNotFoundException()
    {
        var (chatRepository, _, _, _, service) = CreateService();
        chatRepository.GetByIdAsync(InvalidChatId).Returns(Task.FromResult<Chat?>(null));
        var option = new FAQOption("Option A", new FAQNode(2, "Next question", new List<FAQOption>(), false));

        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.SendMessageAsync(InvalidChatId, CreateUser(), option));
    }

    [Test]
    public void SendMessageAsync_ChatIsClosed_ThrowsInvalidOperationException()
    {
        var (chatRepository, _, _, _, service) = CreateService();
        var closedChat = new Chat(ValidChatId, CreateUser(), ChatStatus.Closed);
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(closedChat));
        var option = new FAQOption("Option A", new FAQNode(2, "Next question", new List<FAQOption>(), false));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendMessageAsync(ValidChatId, CreateUser(), option));
    }

    [Test]
    public async Task SendMessageAsync_OptionHasNextNode_ReturnsBotMessageWithNodeText()
    {
        var (chatRepository, messageRepository, decisionTreeService, botEngine, service) = CreateService();
        var chat = CreateActiveChat();
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        messageRepository.AddAsync(Arg.Any<Message>()).Returns(Task.FromResult(1));
        var nextNode = new FAQNode(5, "How can I help?", new List<FAQOption>(), false);
        decisionTreeService.GetNodeByIdAsync(5).Returns(Task.FromResult(nextNode));
        var option = new FAQOption("Help", new FAQNode { NodeId = 5 });

        var result = await service.SendMessageAsync(ValidChatId, CreateUser(), option);

        Assert.That(result.Text, Is.EqualTo(nextNode.QuestionText));
    }

    [Test]
    public async Task SendMessageAsync_OptionHasNextNode_SavesBothUserAndBotMessages()
    {
        var (chatRepository, messageRepository, decisionTreeService, botEngine, service) = CreateService();
        var chat = CreateActiveChat();
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        messageRepository.AddAsync(Arg.Any<Message>()).Returns(Task.FromResult(1));
        var nextNode = new FAQNode(5, "How can I help?", new List<FAQOption>(), false);
        decisionTreeService.GetNodeByIdAsync(5).Returns(Task.FromResult(nextNode));
        var option = new FAQOption("Help", new FAQNode { NodeId = 5 });

        await service.SendMessageAsync(ValidChatId, CreateUser(), option);

        await messageRepository.Received(2).AddAsync(Arg.Any<Message>());
    }

    [Test]
    public async Task SendMessageAsync_NextNodeIdIsOne_ResetsBotConversation()
    {
        var (chatRepository, messageRepository, decisionTreeService, _, service) = CreateService();
        var chat = CreateActiveChat();
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        messageRepository.AddAsync(Arg.Any<Message>()).Returns(Task.FromResult(1));
        var rootNode = new FAQNode(1, "Welcome back", new List<FAQOption>(), false);
        decisionTreeService.GetNodeByIdAsync(1).Returns(Task.FromResult(rootNode));
        var option = new FAQOption("Start over", new FAQNode { NodeId = 1 });

        await service.SendMessageAsync(ValidChatId, CreateUser(), option);

        await decisionTreeService.Received(1).GetNodeByIdAsync(1);
    }

    [Test]
    public void GetMessageAsync_MessageDoesNotExist_ThrowsKeyNotFoundException()
    {
        var (_, messageRepository, _, _, service) = CreateService();
        messageRepository.GetByIdAsync(InvalidMessageId).Returns(Task.FromResult<Message?>(null));

        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetMessageAsync(ValidChatId, InvalidMessageId));
    }

    [Test]
    public void GetMessageAsync_MessageBelongsToDifferentChat_ThrowsInvalidOperationException()
    {
        var (_, messageRepository, _, _, service) = CreateService();
        var otherChat = new Chat(OtherChatId, CreateUser(), ChatStatus.Active);
        var message = new Message(ValidMessageId, CreateUser(), otherChat, MessageText, DateTimeOffset.UtcNow);
        messageRepository.GetByIdAsync(ValidMessageId).Returns(Task.FromResult<Message?>(message));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetMessageAsync(ValidChatId, ValidMessageId));
    }

    [Test]
    public async Task GetMessageAsync_MessageBelongsToChat_ReturnsMessage()
    {
        var (_, messageRepository, _, _, service) = CreateService();
        var chat = CreateActiveChat();
        var message = new Message(ValidMessageId, CreateUser(), chat, MessageText, DateTimeOffset.UtcNow);
        messageRepository.GetByIdAsync(ValidMessageId).Returns(Task.FromResult<Message?>(message));

        var result = await service.GetMessageAsync(ValidChatId, ValidMessageId);

        Assert.That(result, Is.EqualTo(message));
    }

    [Test]
    public async Task GetAllMessagesAsync_WithMessagesFromOtherChats_ExcludesMessagesFromOtherChats()
    {
        var (_, messageRepository, _, _, service) = CreateService();
        var chat = CreateActiveChat(ValidChatId);
        var otherChat = CreateActiveChat(OtherChatId);
        var sender = CreateUser();
        var timestamp = DateTimeOffset.UtcNow;
        var messages = new List<Message>
        {
            new Message(1, sender, chat, "Mine", timestamp),
            new Message(2, sender, otherChat, "Not mine", timestamp),
        };
        messageRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Message>>(messages));

        var result = (await service.GetAllMessagesAsync(ValidChatId)).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllMessagesAsync_WithUnsortedMessages_ReturnsMessagesOrderedByTimestampAscending()
    {
        var (_, messageRepository, _, _, service) = CreateService();
        var chat = CreateActiveChat(ValidChatId);
        var sender = CreateUser();
        var earlier = DateTimeOffset.UtcNow.AddMinutes(-10);
        var later = DateTimeOffset.UtcNow;
        var messages = new List<Message>
        {
            new Message(1, sender, chat, "Second", later),
            new Message(2, sender, chat, "First", earlier),
        };
        messageRepository.GetAsync().Returns(Task.FromResult<IEnumerable<Message>>(messages));

        var result = (await service.GetAllMessagesAsync(ValidChatId)).ToList();

        Assert.That(result[0].Text, Is.EqualTo("First"));
    }

    [Test]
    public void CreateMessageAsync_ChatDoesNotExist_ThrowsKeyNotFoundException()
    {
        var (chatRepository, _, _, _, service) = CreateService();
        chatRepository.GetByIdAsync(InvalidChatId).Returns(Task.FromResult<Chat?>(null));

        Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.CreateMessageAsync(InvalidChatId, ValidSenderId, MessageText, DateTimeOffset.UtcNow));
    }

    [Test]
    public async Task CreateMessageAsync_ChatExists_ReturnsIdFromRepository()
    {
        var (chatRepository, messageRepository, _, _, service) = CreateService();
        var chat = CreateActiveChat();
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        messageRepository.GetSenderByIdAsync(ValidSenderId).Returns(Task.FromResult<Sender>(CreateUser()));
        messageRepository.AddAsync(Arg.Any<Message>()).Returns(Task.FromResult(ValidMessageId));

        var result = await service.CreateMessageAsync(ValidChatId, ValidSenderId, MessageText, DateTimeOffset.UtcNow);

        Assert.That(result, Is.EqualTo(ValidMessageId));
    }

    [Test]
    public async Task CreateMessageAsync_ChatExists_CallsRepositoryAdd()
    {
        var (chatRepository, messageRepository, _, _, service) = CreateService();
        var chat = CreateActiveChat();
        chatRepository.GetByIdAsync(ValidChatId).Returns(Task.FromResult<Chat?>(chat));
        messageRepository.GetSenderByIdAsync(ValidSenderId).Returns(Task.FromResult<Sender>(CreateUser()));
        messageRepository.AddAsync(Arg.Any<Message>()).Returns(Task.FromResult(ValidMessageId));

        await service.CreateMessageAsync(ValidChatId, ValidSenderId, MessageText, DateTimeOffset.UtcNow);

        await messageRepository.Received(1).AddAsync(Arg.Any<Message>());
    }

    [Test]
    public void GetByIdAsync_MessageDoesNotExist_ThrowsKeyNotFoundException()
    {
        var (_, messageRepository, _, _, service) = CreateService();
        messageRepository.GetByIdAsync(InvalidMessageId).Returns(Task.FromResult<Message?>(null));

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(InvalidMessageId));
    }

    [Test]
    public async Task GetByIdAsync_MessageExists_ReturnsMessage()
    {
        var (_, messageRepository, _, _, service) = CreateService();
        var chat = CreateActiveChat();
        var message = new Message(ValidMessageId, CreateUser(), chat, MessageText, DateTimeOffset.UtcNow);
        messageRepository.GetByIdAsync(ValidMessageId).Returns(Task.FromResult<Message?>(message));

        var result = await service.GetByIdAsync(ValidMessageId);

        Assert.That(result, Is.EqualTo(message));
    }
}
