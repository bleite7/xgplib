namespace XgpLib.SyncService.UnitTests.Domain.Entities;

/// <summary>
/// Unit tests for the Genre entity
/// </summary>
public class GenreTests
{
    [Fact]
    public void Genre_ShouldInheritFromAuditableEntity()
    {
        // Arrange & Act
        var genre = new Genre();

        // Assert
        genre.Should().BeAssignableTo<AuditableEntity>();
        genre.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Genre_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var genre = new Genre();

        // Assert
        genre.Id.Should().Be(0);
        genre.Name.Should().BeEmpty();
        genre.Slug.Should().BeEmpty();
        genre.Data.Should().BeEmpty();
        genre.CreatedAt.Should().Be(default);
        genre.ModifiedAt.Should().Be(default);
    }

    [Fact]
    public void Genre_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var genre = new Genre
        {
            // Act
            Id = 123,
            Name = "Action",
            Slug = "action",
            Data = "{\"id\": 123}",
            CreatedAt = now,
            CreatedBy = "system",
            ModifiedAt = now,
            LastModifiedBy = "admin"
        };

        // Assert
        genre.Id.Should().Be(123);
        genre.Name.Should().Be("Action");
        genre.Slug.Should().Be("action");
        genre.Data.Should().Be("{\"id\": 123}");
        genre.CreatedAt.Should().Be(now);
        genre.CreatedBy.Should().Be("system");
        genre.ModifiedAt.Should().Be(now);
        genre.LastModifiedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Adventure")]
    [InlineData("Role-Playing Game (RPG)")]
    public void Genre_Name_ShouldAcceptVariousValues(string name)
    {
        // Arrange
        var genre = new Genre
        {
            // Act
            Name = name
        };

        // Assert
        genre.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("action")]
    [InlineData("role-playing-game-rpg")]
    [InlineData("puzzle-platformer")]
    public void Genre_Slug_ShouldAcceptVariousValues(string slug)
    {
        // Arrange
        var genre = new Genre
        {
            // Act
            Slug = slug
        };

        // Assert
        genre.Slug.Should().Be(slug);
    }

    [Fact]
    public void Genre_Data_ShouldStoreJsonData()
    {
        // Arrange
        var genre = new Genre();
        var jsonData = "{\"id\":1,\"name\":\"Action\",\"url\":\"https://example.com\"}";

        // Act
        genre.Data = jsonData;

        // Assert
        genre.Data.Should().Be(jsonData);
    }
}
