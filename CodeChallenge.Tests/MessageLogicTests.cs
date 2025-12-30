using CodeChallenge.Api.Logic;
using CodeChallenge.Api.Models;
using CodeChallenge.Api.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeChallenge.Tests;

public class MessageLogicTests
{
    private readonly Mock<IMessageRepository> _repositoryMock;
    private readonly MessageLogic _logic;
    private readonly Guid _organizationId = Guid.NewGuid();

    public MessageLogicTests()
    {
        _repositoryMock = new Mock<IMessageRepository>();
        _logic = new MessageLogic(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateMessage_Success_ReturnsCreated()
    {
        var request = new CreateMessageRequest
        {
            Title = "Valid Title",
            Content = "This is valid content with enough length"
        };

        _repositoryMock
            .Setup(r => r.GetByTitleAsync(_organizationId, request.Title))
            .ReturnsAsync((Message?)null);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Message>()))
            .ReturnsAsync((Message m) => m);

        var result = await _logic.CreateMessageAsync(_organizationId, request);

        result.Should().BeOfType<Created<Message>>();
    }

    [Fact]
    public async Task CreateMessage_DuplicateTitle_ReturnsConflict()
    {
        var request = new CreateMessageRequest
        {
            Title = "Duplicate",
            Content = "This is valid content with enough length"
        };

        _repositoryMock
            .Setup(r => r.GetByTitleAsync(_organizationId, request.Title))
            .ReturnsAsync(new Message());

        var result = await _logic.CreateMessageAsync(_organizationId, request);

        result.Should().BeOfType<Conflict>();
    }

    [Fact]
    public async Task CreateMessage_InvalidContentLength_ReturnsValidationError()
    {
        var request = new CreateMessageRequest
        {
            Title = "Valid Title",
            Content = "Short"
        };

        var result = await _logic.CreateMessageAsync(_organizationId, request);

        result.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public async Task UpdateMessage_NonExistent_ReturnsNotFound()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_organizationId, It.IsAny<Guid>()))
            .ReturnsAsync((Message?)null);

        var result = await _logic.UpdateMessageAsync(
            _organizationId,
            Guid.NewGuid(),
            new UpdateMessageRequest());

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task UpdateMessage_InactiveMessage_ReturnsValidationError()
    {
        var message = new Message
        {
            OrganizationId = _organizationId,
            IsActive = false
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(_organizationId, It.IsAny<Guid>()))
            .ReturnsAsync(message);

        var result = await _logic.UpdateMessageAsync(
            _organizationId,
            Guid.NewGuid(),
            new UpdateMessageRequest());

        result.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public async Task DeleteMessage_NonExistent_ReturnsNotFound()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(_organizationId, It.IsAny<Guid>()))
            .ReturnsAsync((Message?)null);

        var result = await _logic.DeleteMessageAsync(
            _organizationId,
            Guid.NewGuid());

        result.Should().BeOfType<NotFound>();
    }
}
