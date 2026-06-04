using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class DecisionTreeServiceTests
{
    private const int ValidNodeId = 1;
    private const int InvalidNodeId = 99;
    private const string NodeQuestionText = "How can we help you?";

    [Test]
    public void GetNodeByIdAsync_NodeDoesNotExist_ThrowsKeyNotFoundException()
    {
        var decisionTreeRepository = Substitute.For<IDecisionTreeRepository>();
        decisionTreeRepository.GetByIdAsync(InvalidNodeId).Returns(Task.FromResult<FAQNode?>(null));
        var service = new DecisionTreeService(decisionTreeRepository);

        Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetNodeByIdAsync(InvalidNodeId));
    }

    [Test]
    public async Task GetNodeByIdAsync_NodeExists_ReturnsNode()
    {
        var decisionTreeRepository = Substitute.For<IDecisionTreeRepository>();
        var node = new FAQNode(ValidNodeId, NodeQuestionText, new List<FAQOption>(), false);
        decisionTreeRepository.GetByIdAsync(ValidNodeId).Returns(Task.FromResult<FAQNode?>(node));
        var service = new DecisionTreeService(decisionTreeRepository);

        var result = await service.GetNodeByIdAsync(ValidNodeId);

        Assert.That(result, Is.EqualTo(node));
    }
}
