namespace XgpLib.SyncService.UnitTests.Application.DTOs;

public class ReceiveMessagesResponseTests
{
    [Fact]
    public void ReceiveMessagesResponse_ShouldInitializeWithRequiredParameters()
    {
        // Arrange
        var success = true;
        var messages = new List<string> { "message1", "message2" };

        // Act
        var response = new ReceiveMessagesResponse(success, messages);

        // Assert
        response.Success.Should().Be(success);
        response.Messages.Should().BeEquivalentTo(messages);
        response.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ReceiveMessagesResponse_ShouldInitializeWithAllParameters()
    {
        // Arrange
        var success = false;
        var messages = new List<string>();
        var errorMessage = "Failed to receive messages";

        // Act
        var response = new ReceiveMessagesResponse(success, messages, errorMessage);

        // Assert
        response.Success.Should().Be(success);
        response.Messages.Should().BeEmpty();
        response.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void ReceiveMessagesResponse_WithSuccess_ShouldHaveMessages()
    {
        // Arrange
        var messages = new List<string> { "msg1", "msg2", "msg3" };

        // Act
        var response = new ReceiveMessagesResponse(true, messages);

        // Assert
        response.Success.Should().BeTrue();
        response.Messages.Should().HaveCount(3);
        response.Messages.Should().Contain("msg1");
        response.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ReceiveMessagesResponse_WithFailure_ShouldHaveErrorMessage()
    {
        // Arrange
        var errorMessage = "Connection timeout";

        // Act
        var response = new ReceiveMessagesResponse(false, new List<string>(), errorMessage);

        // Assert
        response.Success.Should().BeFalse();
        response.Messages.Should().BeEmpty();
        response.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void ReceiveMessagesResponse_Messages_ShouldBeEmptyList()
    {
        // Act
        var response = new ReceiveMessagesResponse(true, new List<string>());

        // Assert
        response.Messages.Should().NotBeNull();
        response.Messages.Should().BeEmpty();
    }

    [Fact]
    public void ReceiveMessagesResponse_ShouldBeRecord()
    {
        // Arrange
        var messages = new List<string> { "msg" };
        var response1 = new ReceiveMessagesResponse(true, messages);
        var response2 = new ReceiveMessagesResponse(true, messages);

        // Assert
        response1.Should().Be(response2);
        response1.Should().NotBeSameAs(response2);
    }

    [Fact]
    public void ReceiveMessagesResponse_ShouldSupportWithExpression()
    {
        // Arrange
        var original = new ReceiveMessagesResponse(
            true,
            new List<string> { "message" }
        );

        // Act
        var modified = original with { Success = false, ErrorMessage = "Error occurred" };

        // Assert
        modified.Success.Should().BeFalse();
        modified.ErrorMessage.Should().Be("Error occurred");
        modified.Messages.Should().BeEquivalentTo(original.Messages);
    }

    [Fact]
    public void ReceiveMessagesResponse_ShouldSupportDeconstruction()
    {
        // Arrange
        var messages = new List<string> { "msg1" };
        var response = new ReceiveMessagesResponse(true, messages, "no error");

        // Act
        var (success, messageList, errorMessage) = response;

        // Assert
        success.Should().BeTrue();
        messageList.Should().BeEquivalentTo(messages);
        errorMessage.Should().Be("no error");
    }

    [Fact]
    public void ReceiveMessagesResponse_EqualityCheck_ShouldWorkCorrectly()
    {
        // Arrange
        var messages = new List<string> { "msg1" };

        var response1 = new ReceiveMessagesResponse(true, messages);
        var response2 = new ReceiveMessagesResponse(true, messages); // Same reference
        var response3 = new ReceiveMessagesResponse(false, messages);

        // Assert
        response1.Should().Be(response2);
        response1.Should().NotBe(response3);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Error message")]
    [InlineData("Queue not found")]
    public void ReceiveMessagesResponse_ErrorMessage_ShouldAcceptVariousValues(string? errorMessage)
    {
        // Act
        var response = new ReceiveMessagesResponse(false, new List<string>(), errorMessage);

        // Assert
        response.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void ReceiveMessagesResponse_WithMultipleMessages_ShouldStoreAll()
    {
        // Arrange
        var messages = new List<string>
        {
            "message 1",
            "message 2",
            "message 3",
            "message 4",
            "message 5"
        };

        // Act
        var response = new ReceiveMessagesResponse(true, messages);

        // Assert
        response.Messages.Should().HaveCount(5);
        response.Messages.Should().BeEquivalentTo(messages);
    }
}
