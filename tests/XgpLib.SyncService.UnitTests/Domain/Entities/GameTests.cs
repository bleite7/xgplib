namespace XgpLib.SyncService.UnitTests.Domain.Entities;

/// <summary>
/// Unit tests for the Game entity
/// </summary>
public class GameTests
{
    [Fact]
    public void Game_ShouldInheritFromAuditableEntity()
    {
        // Arrange & Act
        var game = new Game();

        // Assert
        game.Should().BeAssignableTo<AuditableEntity>();
        game.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Game_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var game = new Game();

        // Assert
        game.Id.Should().Be(0);
        game.Name.Should().BeEmpty();
        game.Genres.Should().BeEmpty();
        game.Data.Should().BeEmpty();
        game.CreatedAt.Should().Be(default);
        game.ModifiedAt.Should().Be(default);
    }

    [Fact]
    public void Game_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var game = new Game();
        var genres = new[] { 1, 2, 3 };

        // Act
        game.Id = 456;
        game.Name = "The Witcher 3";
        game.Genres = genres;
        game.Data = "{\"id\": 456}";
        game.CreatedAt = now;
        game.CreatedBy = "system";
        game.ModifiedAt = now;
        game.LastModifiedBy = "admin";

        // Assert
        game.Id.Should().Be(456);
        game.Name.Should().Be("The Witcher 3");
        game.Genres.Should().BeEquivalentTo(genres);
        game.Data.Should().Be("{\"id\": 456}");
        game.CreatedAt.Should().Be(now);
        game.CreatedBy.Should().Be("system");
        game.ModifiedAt.Should().Be(now);
        game.LastModifiedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Cyberpunk 2077")]
    [InlineData("The Legend of Zelda: Breath of the Wild")]
    public void Game_Name_ShouldAcceptVariousValues(string name)
    {
        // Arrange
        var game = new Game
        {
            // Act
            Name = name
        };

        // Assert
        game.Name.Should().Be(name);
    }

    [Fact]
    public void Game_Genres_ShouldStoreEmptyArray()
    {
        // Arrange
        var game = new Game
        {
            // Act
            Genres = []
        };

        // Assert
        game.Genres.Should().BeEmpty();
    }

    [Fact]
    public void Game_Genres_ShouldStoreSingleGenre()
    {
        // Arrange
        var game = new Game();
        var genres = new[] { 5 };

        // Act
        game.Genres = genres;

        // Assert
        game.Genres.Should().HaveCount(1);
        game.Genres.Should().Contain(5);
    }

    [Fact]
    public void Game_Genres_ShouldStoreMultipleGenres()
    {
        // Arrange
        var game = new Game();
        var genres = new[] { 1, 2, 3, 4, 5 };

        // Act
        game.Genres = genres;

        // Assert
        game.Genres.Should().HaveCount(5);
        game.Genres.Should().BeEquivalentTo(genres);
    }

    [Fact]
    public void Game_Data_ShouldStoreJsonData()
    {
        // Arrange
        var game = new Game();
        var jsonData = "{\"id\":1,\"name\":\"Test Game\",\"rating\":95.5}";

        // Act
        game.Data = jsonData;

        // Assert
        game.Data.Should().Be(jsonData);
    }
}
