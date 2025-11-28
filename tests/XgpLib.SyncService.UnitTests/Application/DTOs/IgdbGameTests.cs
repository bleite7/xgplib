namespace XgpLib.SyncService.UnitTests.Application.DTOs;

public class IgdbGameTests
{
    [Fact]
    public void IgdbGame_ShouldInitializeWithDefaultValues()
    {
        // Act
        var igdbGame = new IgdbGame();

        // Assert
        igdbGame.Id.Should().Be(0);
        igdbGame.Name.Should().Be(string.Empty);
        igdbGame.Storyline.Should().Be(string.Empty);
        igdbGame.Summary.Should().Be(string.Empty);
        igdbGame.Genres.Should().BeEmpty();
    }

    [Fact]
    public void IgdbGame_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var id = 12345L;
        var name = "The Witcher 3: Wild Hunt";
        var storyline = "Geralt of Rivia is on a mission to find his adopted daughter.";
        var summary = "An open-world RPG set in a visually stunning fantasy universe.";
        var genres = new[] { 12, 31, 32 };

        // Act
        var igdbGame = new IgdbGame
        {
            Id = id,
            Name = name,
            Storyline = storyline,
            Summary = summary,
            Genres = genres
        };

        // Assert
        igdbGame.Id.Should().Be(id);
        igdbGame.Name.Should().Be(name);
        igdbGame.Storyline.Should().Be(storyline);
        igdbGame.Summary.Should().Be(summary);
        igdbGame.Genres.Should().BeEquivalentTo(genres);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999999)]
    [InlineData(long.MaxValue)]
    public void IgdbGame_Id_ShouldAcceptVariousLongValues(long id)
    {
        // Act
        var igdbGame = new IgdbGame { Id = id };

        // Assert
        igdbGame.Id.Should().Be(id);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Hades")]
    [InlineData("The Legend of Zelda: Breath of the Wild")]
    public void IgdbGame_Name_ShouldAcceptVariousValues(string name)
    {
        // Act
        var igdbGame = new IgdbGame { Name = name };

        // Assert
        igdbGame.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A compelling story about redemption and revenge.")]
    public void IgdbGame_Storyline_ShouldAcceptVariousValues(string storyline)
    {
        // Act
        var igdbGame = new IgdbGame { Storyline = storyline };

        // Assert
        igdbGame.Storyline.Should().Be(storyline);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A critically acclaimed action-adventure game.")]
    public void IgdbGame_Summary_ShouldAcceptVariousValues(string summary)
    {
        // Act
        var igdbGame = new IgdbGame { Summary = summary };

        // Assert
        igdbGame.Summary.Should().Be(summary);
    }

    [Fact]
    public void IgdbGame_Genres_ShouldStoreEmptyArray()
    {
        // Act
        var igdbGame = new IgdbGame { Genres = [] };

        // Assert
        igdbGame.Genres.Should().BeEmpty();
    }

    [Fact]
    public void IgdbGame_Genres_ShouldStoreSingleGenre()
    {
        // Arrange
        var genres = new[] { 5 };

        // Act
        var igdbGame = new IgdbGame { Genres = genres };

        // Assert
        igdbGame.Genres.Should().HaveCount(1);
        igdbGame.Genres.Should().Contain(5);
    }

    [Fact]
    public void IgdbGame_Genres_ShouldStoreMultipleGenres()
    {
        // Arrange
        var genres = new[] { 12, 31, 32, 9, 10 };

        // Act
        var igdbGame = new IgdbGame { Genres = genres };

        // Assert
        igdbGame.Genres.Should().HaveCount(5);
        igdbGame.Genres.Should().BeEquivalentTo(genres);
    }

    [Fact]
    public void IgdbGame_ShouldBeRecord()
    {
        // Arrange
        var game1 = new IgdbGame
        {
            Id = 1,
            Name = "Game",
            Storyline = "Story",
            Summary = "Summary",
            Genres = [1, 2]
        };

        var game2 = new IgdbGame
        {
            Id = 1,
            Name = "Game",
            Storyline = "Story",
            Summary = "Summary",
            Genres = new[] { 1, 2 }
        };

        // Assert
        game1.Should().NotBeSameAs(game2);
    }

    [Fact]
    public void IgdbGame_ShouldSupportWithExpression()
    {
        // Arrange
        var original = new IgdbGame
        {
            Id = 1,
            Name = "Original Game",
            Storyline = "Original Story",
            Summary = "Original Summary",
            Genres = [1]
        };

        // Act
        var modified = original with { Name = "Modified Game" };

        // Assert
        modified.Id.Should().Be(original.Id);
        modified.Name.Should().Be("Modified Game");
        modified.Storyline.Should().Be(original.Storyline);
        modified.Summary.Should().Be(original.Summary);
        modified.Genres.Should().BeEquivalentTo(original.Genres);
    }
}
