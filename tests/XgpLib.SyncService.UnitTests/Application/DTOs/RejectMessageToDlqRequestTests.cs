namespace XgpLib.SyncService.UnitTests.Application.DTOs;

public class RejectMessageToDlqRequestTests
{
    [Fact]
    public void RejectMessageToDlqRequest_ShouldInitializeWithAllParameters()
    {
        // Arrange
        var queue = "main-queue";
        var message = "Invalid message content";
        var reason = "Malformed JSON";

        // Act
        var request = new RejectMessageToDlqRequest(queue, message, reason);

        // Assert
        request.Queue.Should().Be(queue);
        request.Message.Should().Be(message);
        request.Reason.Should().Be(reason);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("queue-1")]
    [InlineData("my.dlq.queue")]
    [InlineData("PRODUCTION_QUEUE")]
    public void RejectMessageToDlqRequest_Queue_ShouldAcceptVariousValues(string queue)
    {
        // Act
        var request = new RejectMessageToDlqRequest(queue, "message", "reason");

        // Assert
        request.Queue.Should().Be(queue);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Simple message")]
    [InlineData("{\"id\":1,\"data\":\"test\"}")]
    [InlineData("A very long message with multiple words and characters !@#$%")]
    public void RejectMessageToDlqRequest_Message_ShouldAcceptVariousValues(string message)
    {
        // Act
        var request = new RejectMessageToDlqRequest("queue", message, "reason");

        // Assert
        request.Message.Should().Be(message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Invalid format")]
    [InlineData("Processing timeout")]
    [InlineData("Business validation failed: missing required field")]
    public void RejectMessageToDlqRequest_Reason_ShouldAcceptVariousValues(string reason)
    {
        // Act
        var request = new RejectMessageToDlqRequest("queue", "message", reason);

        // Assert
        request.Reason.Should().Be(reason);
    }

    [Fact]
    public void RejectMessageToDlqRequest_ShouldBeRecord()
    {
        // Arrange
        var request1 = new RejectMessageToDlqRequest("queue", "message", "reason");
        var request2 = new RejectMessageToDlqRequest("queue", "message", "reason");

        // Assert
        request1.Should().Be(request2);
        request1.Should().NotBeSameAs(request2);
    }

    [Fact]
    public void RejectMessageToDlqRequest_ShouldSupportWithExpression()
    {
        // Arrange
        var original = new RejectMessageToDlqRequest("queue-1", "msg-1", "reason-1");

        // Act
        var modified = original with { Reason = "Updated reason" };

        // Assert
        modified.Queue.Should().Be(original.Queue);
        modified.Message.Should().Be(original.Message);
        modified.Reason.Should().Be("Updated reason");
    }

    [Fact]
    public void RejectMessageToDlqRequest_ShouldSupportDeconstruction()
    {
        // Arrange
        var request = new RejectMessageToDlqRequest("test-queue", "test-message", "test-reason");

        // Act
        var (queue, message, reason) = request;

        // Assert
        queue.Should().Be("test-queue");
        message.Should().Be("test-message");
        reason.Should().Be("test-reason");
    }

    [Fact]
    public void RejectMessageToDlqRequest_EqualityCheck_ShouldWorkCorrectly()
    {
        // Arrange
        var request1 = new RejectMessageToDlqRequest("queue", "msg", "reason");
        var request2 = new RejectMessageToDlqRequest("queue", "msg", "reason");
        var request3 = new RejectMessageToDlqRequest("queue2", "msg", "reason");
        var request4 = new RejectMessageToDlqRequest("queue", "msg2", "reason");
        var request5 = new RejectMessageToDlqRequest("queue", "msg", "reason2");

        // Assert
        request1.Should().Be(request2);
        request1.Should().NotBe(request3);
        request1.Should().NotBe(request4);
        request1.Should().NotBe(request5);
    }

    [Fact]
    public void RejectMessageToDlqRequest_WithComplexMessage_ShouldStoreCorrectly()
    {
        // Arrange
        var complexMessage = """
        {
            "orderId": 12345,
            "items": [
                {"id": 1, "name": "Product A"},
                {"id": 2, "name": "Product B"}
            ],
            "total": 99.99
        }
        """;

        // Act
        var request = new RejectMessageToDlqRequest("orders-queue", complexMessage, "Invalid order total");

        // Assert
        request.Message.Should().Be(complexMessage);
        request.Message.Should().Contain("orderId");
        request.Message.Should().Contain("items");
    }

    [Fact]
    public void RejectMessageToDlqRequest_WithEmptyValues_ShouldAcceptThem()
    {
        // Act
        var request = new RejectMessageToDlqRequest("", "", "");

        // Assert
        request.Queue.Should().Be("");
        request.Message.Should().Be("");
        request.Reason.Should().Be("");
    }
}
