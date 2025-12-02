namespace XgpLib.SyncService.UnitTests.Application.DTOs;

public class ReceiveMessagesRequestTests
{
    [Fact]
    public void ReceiveMessagesRequest_ShouldInitializeWithRequiredParameters()
    {
        // Arrange
        var queue = "test-queue";

        // Act
        var request = new ReceiveMessagesRequest(queue);

        // Assert
        request.Queue.Should().Be(queue);
        request.MaxMessages.Should().Be(10); // Default value
    }

    [Fact]
    public void ReceiveMessagesRequest_ShouldInitializeWithAllParameters()
    {
        // Arrange
        var queue = "test-queue";
        short maxMessages = 5;

        // Act
        var request = new ReceiveMessagesRequest(queue, maxMessages);

        // Assert
        request.Queue.Should().Be(queue);
        request.MaxMessages.Should().Be(maxMessages);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("queue-1")]
    [InlineData("my.queue.name")]
    [InlineData("UPPERCASE_QUEUE")]
    public void ReceiveMessagesRequest_Queue_ShouldAcceptVariousValues(string queue)
    {
        // Act
        var request = new ReceiveMessagesRequest(queue);

        // Assert
        request.Queue.Should().Be(queue);
    }

    [Theory]
    [InlineData(short.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(short.MaxValue)]
    public void ReceiveMessagesRequest_MaxMessages_ShouldAcceptVariousValues(short maxMessages)
    {
        // Act
        var request = new ReceiveMessagesRequest("test-queue", maxMessages);

        // Assert
        request.MaxMessages.Should().Be(maxMessages);
    }

    [Fact]
    public void ReceiveMessagesRequest_ShouldBeRecord()
    {
        // Arrange
        var request1 = new ReceiveMessagesRequest("queue", 10);
        var request2 = new ReceiveMessagesRequest("queue", 10);

        // Assert
        request1.Should().Be(request2);
        request1.Should().NotBeSameAs(request2);
    }

    [Fact]
    public void ReceiveMessagesRequest_ShouldSupportWithExpression()
    {
        // Arrange
        var original = new ReceiveMessagesRequest("original-queue", 5);

        // Act
        var modified = original with { Queue = "modified-queue" };

        // Assert
        modified.Queue.Should().Be("modified-queue");
        modified.MaxMessages.Should().Be(original.MaxMessages);
    }

    [Fact]
    public void ReceiveMessagesRequest_ShouldSupportDeconstruction()
    {
        // Arrange
        var request = new ReceiveMessagesRequest("test-queue", 15);

        // Act
        var (queue, maxMessages) = request;

        // Assert
        queue.Should().Be("test-queue");
        maxMessages.Should().Be(15);
    }

    [Fact]
    public void ReceiveMessagesRequest_WithDefaultMaxMessages_ShouldBe10()
    {
        // Act
        var request = new ReceiveMessagesRequest("queue");

        // Assert
        request.MaxMessages.Should().Be(10);
    }

    [Fact]
    public void ReceiveMessagesRequest_EqualityCheck_ShouldWorkCorrectly()
    {
        // Arrange
        var request1 = new ReceiveMessagesRequest("queue-1", 20);
        var request2 = new ReceiveMessagesRequest("queue-1", 20);
        var request3 = new ReceiveMessagesRequest("queue-2", 20);
        var request4 = new ReceiveMessagesRequest("queue-1", 30);

        // Assert
        request1.Should().Be(request2);
        request1.Should().NotBe(request3);
        request1.Should().NotBe(request4);
    }
}
