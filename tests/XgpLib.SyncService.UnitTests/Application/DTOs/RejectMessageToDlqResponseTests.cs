namespace XgpLib.SyncService.UnitTests.Application.DTOs;

public class RejectMessageToDlqResponseTests
{
    [Fact]
    public void RejectMessageToDlqResponse_ShouldInitializeWithRequiredParameters()
    {
        // Arrange
        var success = true;
        var dlqName = "main-queue.dlq";

        // Act
        var response = new RejectMessageToDlqResponse(success, dlqName);

        // Assert
        response.Success.Should().Be(success);
        response.DlqName.Should().Be(dlqName);
        response.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void RejectMessageToDlqResponse_ShouldInitializeWithAllParameters()
    {
        // Arrange
        var success = false;
        var dlqName = "queue.dlq";
        var errorMessage = "DLQ not found";

        // Act
        var response = new RejectMessageToDlqResponse(success, dlqName, errorMessage);

        // Assert
        response.Success.Should().Be(success);
        response.DlqName.Should().Be(dlqName);
        response.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void RejectMessageToDlqResponse_WithSuccess_ShouldNotHaveErrorMessage()
    {
        // Arrange
        var dlqName = "successful-queue.dlq";

        // Act
        var response = new RejectMessageToDlqResponse(true, dlqName);

        // Assert
        response.Success.Should().BeTrue();
        response.DlqName.Should().Be(dlqName);
        response.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void RejectMessageToDlqResponse_WithFailure_ShouldHaveErrorMessage()
    {
        // Arrange
        var errorMessage = "Failed to send message to DLQ";

        // Act
        var response = new RejectMessageToDlqResponse(false, "queue.dlq", errorMessage);

        // Assert
        response.Success.Should().BeFalse();
        response.ErrorMessage.Should().Be(errorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("queue.dlq")]
    [InlineData("my-service.main-queue.dlq")]
    [InlineData("PRODUCTION_DLQ")]
    public void RejectMessageToDlqResponse_DlqName_ShouldAcceptVariousValues(string dlqName)
    {
        // Act
        var response = new RejectMessageToDlqResponse(true, dlqName);

        // Assert
        response.DlqName.Should().Be(dlqName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Connection timeout")]
    [InlineData("DLQ capacity exceeded")]
    public void RejectMessageToDlqResponse_ErrorMessage_ShouldAcceptVariousValues(string? errorMessage)
    {
        // Act
        var response = new RejectMessageToDlqResponse(false, "queue.dlq", errorMessage);

        // Assert
        response.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void RejectMessageToDlqResponse_ShouldBeRecord()
    {
        // Arrange
        var response1 = new RejectMessageToDlqResponse(true, "queue.dlq");
        var response2 = new RejectMessageToDlqResponse(true, "queue.dlq");

        // Assert
        response1.Should().Be(response2);
        response1.Should().NotBeSameAs(response2);
    }

    [Fact]
    public void RejectMessageToDlqResponse_ShouldSupportWithExpression()
    {
        // Arrange
        var original = new RejectMessageToDlqResponse(true, "original.dlq");

        // Act
        var modified = original with { Success = false, ErrorMessage = "Failed" };

        // Assert
        modified.Success.Should().BeFalse();
        modified.DlqName.Should().Be(original.DlqName);
        modified.ErrorMessage.Should().Be("Failed");
    }

    [Fact]
    public void RejectMessageToDlqResponse_ShouldSupportDeconstruction()
    {
        // Arrange
        var response = new RejectMessageToDlqResponse(true, "test.dlq", null);

        // Act
        var (success, dlqName, errorMessage) = response;

        // Assert
        success.Should().BeTrue();
        dlqName.Should().Be("test.dlq");
        errorMessage.Should().BeNull();
    }

    [Fact]
    public void RejectMessageToDlqResponse_EqualityCheck_ShouldWorkCorrectly()
    {
        // Arrange
        var response1 = new RejectMessageToDlqResponse(true, "queue.dlq");
        var response2 = new RejectMessageToDlqResponse(true, "queue.dlq");
        var response3 = new RejectMessageToDlqResponse(false, "queue.dlq");
        var response4 = new RejectMessageToDlqResponse(true, "other.dlq");
        var response5 = new RejectMessageToDlqResponse(true, "queue.dlq", "error");

        // Assert
        response1.Should().Be(response2);
        response1.Should().NotBe(response3);
        response1.Should().NotBe(response4);
        response1.Should().NotBe(response5);
    }

    [Fact]
    public void RejectMessageToDlqResponse_WithBothSuccessAndErrorMessage_ShouldAllowIt()
    {
        // This tests edge case - typically success would not have error message
        // but the record doesn't enforce this constraint

        // Act
        var response = new RejectMessageToDlqResponse(true, "queue.dlq", "Warning: DLQ almost full");

        // Assert
        response.Success.Should().BeTrue();
        response.DlqName.Should().Be("queue.dlq");
        response.ErrorMessage.Should().Be("Warning: DLQ almost full");
    }

    [Fact]
    public void RejectMessageToDlqResponse_WithLongDlqName_ShouldStoreCorrectly()
    {
        // Arrange
        var longDlqName = "production.service-name.very-long-queue-name-with-many-segments.dead-letter-queue.dlq";

        // Act
        var response = new RejectMessageToDlqResponse(true, longDlqName);

        // Assert
        response.DlqName.Should().Be(longDlqName);
        response.DlqName.Length.Should().BeGreaterThan(50);
    }
}
