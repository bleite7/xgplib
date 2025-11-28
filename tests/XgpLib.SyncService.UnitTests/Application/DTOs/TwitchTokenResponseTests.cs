using System.Text.Json;

namespace XgpLib.SyncService.UnitTests.Application.DTOs;

public class TwitchTokenResponseTests
{
    [Fact]
    public void TwitchTokenResponse_ShouldInitializeWithDefaultValues()
    {
        // Act
        var response = new TwitchTokenResponse();

        // Assert
        response.AccessToken.Should().Be(string.Empty);
        response.ExpiresIn.Should().Be(0);
        response.TokenType.Should().Be(string.Empty);
    }

    [Fact]
    public void TwitchTokenResponse_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var accessToken = "ya29.a0AfH6SMBx...";
        var expiresIn = 3600;
        var tokenType = "bearer";

        // Act
        var response = new TwitchTokenResponse
        {
            AccessToken = accessToken,
            ExpiresIn = expiresIn,
            TokenType = tokenType
        };

        // Assert
        response.AccessToken.Should().Be(accessToken);
        response.ExpiresIn.Should().Be(expiresIn);
        response.TokenType.Should().Be(tokenType);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ya29.a0AfH6SMBx")]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9")]
    public void TwitchTokenResponse_AccessToken_ShouldAcceptVariousValues(string accessToken)
    {
        // Act
        var response = new TwitchTokenResponse { AccessToken = accessToken };

        // Assert
        response.AccessToken.Should().Be(accessToken);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3600)]
    [InlineData(7200)]
    [InlineData(int.MaxValue)]
    public void TwitchTokenResponse_ExpiresIn_ShouldAcceptVariousValues(int expiresIn)
    {
        // Act
        var response = new TwitchTokenResponse { ExpiresIn = expiresIn };

        // Assert
        response.ExpiresIn.Should().Be(expiresIn);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("bearer")]
    [InlineData("Bearer")]
    [InlineData("OAuth")]
    public void TwitchTokenResponse_TokenType_ShouldAcceptVariousValues(string tokenType)
    {
        // Act
        var response = new TwitchTokenResponse { TokenType = tokenType };

        // Assert
        response.TokenType.Should().Be(tokenType);
    }

    [Fact]
    public void TwitchTokenResponse_ShouldBeRecord()
    {
        // Arrange
        var response1 = new TwitchTokenResponse
        {
            AccessToken = "token123",
            ExpiresIn = 3600,
            TokenType = "bearer"
        };

        var response2 = new TwitchTokenResponse
        {
            AccessToken = "token123",
            ExpiresIn = 3600,
            TokenType = "bearer"
        };

        // Assert
        response1.Should().NotBeSameAs(response2);
    }

    [Fact]
    public void TwitchTokenResponse_ShouldSupportWithExpression()
    {
        // Arrange
        var original = new TwitchTokenResponse
        {
            AccessToken = "original_token",
            ExpiresIn = 3600,
            TokenType = "bearer"
        };

        // Act
        var modified = original with { AccessToken = "new_token" };

        // Assert
        modified.AccessToken.Should().Be("new_token");
        modified.ExpiresIn.Should().Be(original.ExpiresIn);
        modified.TokenType.Should().Be(original.TokenType);
    }

    [Fact]
    public void TwitchTokenResponse_ShouldDeserializeFromJson()
    {
        // Arrange
        var json = """
        {
            "access_token": "test_token_123",
            "expires_in": 7200,
            "token_type": "bearer"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TwitchTokenResponse>(json);

        // Assert
        response.Should().NotBeNull();
        response!.AccessToken.Should().Be("test_token_123");
        response.ExpiresIn.Should().Be(7200);
        response.TokenType.Should().Be("bearer");
    }

    [Fact]
    public void TwitchTokenResponse_ShouldSerializeToJson()
    {
        // Arrange
        var response = new TwitchTokenResponse
        {
            AccessToken = "test_token_456",
            ExpiresIn = 5400,
            TokenType = "bearer"
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        json.Should().Contain("\"access_token\":\"test_token_456\"");
        json.Should().Contain("\"expires_in\":5400");
        json.Should().Contain("\"token_type\":\"bearer\"");
    }
}
