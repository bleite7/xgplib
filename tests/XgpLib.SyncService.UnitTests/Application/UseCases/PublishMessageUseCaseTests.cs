using XgpLib.SyncService.Application.UseCases;

namespace XgpLib.SyncService.UnitTests.Application.UseCases;

/// <summary>
/// Unit tests for PublishMessageUseCase
/// </summary>
public class PublishMessageUseCaseTests
{
    private readonly Mock<ILogger<PublishMessageUseCase>> _loggerMock;
    private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
    private readonly PublishMessageUseCase _useCase;

    public PublishMessageUseCaseTests()
    {
        _loggerMock = new Mock<ILogger<PublishMessageUseCase>>();
        _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
        _useCase = new PublishMessageUseCase(_loggerMock.Object, _messageBrokerServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldPublishMessageSuccessfully()
    {
        // Arrange
        var request = new PublishMessageRequest("test-topic", "test message");

        _messageBrokerServiceMock
            .Setup(x => x.PublishMessageAsync(request.Topic, request.Message, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();

        _messageBrokerServiceMock.Verify(
            x => x.PublishMessageAsync(request.Topic, request.Message, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_WithNullOrEmptyTopic_ShouldReturnFailure(string? topic)
    {
        // Arrange
        var request = new PublishMessageRequest(topic!, "test message");

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Topic cannot be null or empty");

        _messageBrokerServiceMock.Verify(
            x => x.PublishMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_WithNullOrEmptyMessage_ShouldReturnFailure(string? message)
    {
        // Arrange
        var request = new PublishMessageRequest("test-topic", message!);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Message cannot be null or empty");

        _messageBrokerServiceMock.Verify(
            x => x.PublishMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMessageBrokerThrowsException_ShouldReturnFailureWithErrorMessage()
    {
        // Arrange
        var request = new PublishMessageRequest("test-topic", "test message");

        var expectedException = new InvalidOperationException("Connection failed");

        _messageBrokerServiceMock
            .Setup(x => x.PublishMessageAsync(request.Topic, request.Message, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Connection failed");

        _messageBrokerServiceMock.Verify(
            x => x.PublishMessageAsync(request.Topic, request.Message, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogInformation_WhenPublishingMessage()
    {
        // Arrange
        var request = new PublishMessageRequest("test-topic", "test message");

        _messageBrokerServiceMock
            .Setup(x => x.PublishMessageAsync(request.Topic, request.Message, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Publishing message to topic")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var request = new PublishMessageRequest("test-topic", "test message");

        var expectedException = new InvalidOperationException("Connection failed");

        _messageBrokerServiceMock
            .Setup(x => x.PublishMessageAsync(request.Topic, request.Message, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to publish message")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new PublishMessageRequest("test-topic", "test message");

        var cts = new CancellationTokenSource();
        cts.Cancel();

        _messageBrokerServiceMock
            .Setup(x => x.PublishMessageAsync(request.Topic, request.Message, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _useCase.ExecuteAsync(request, cts.Token);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }
}
