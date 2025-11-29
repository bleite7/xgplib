namespace XgpLib.SyncService.UnitTests.Application.Games.Commands;

public class SyncGamesCommandHandlerTests
{
    private readonly Mock<ILogger<SyncGamesCommandHandler>> _loggerMock;
    private readonly Mock<IIgdbService> _igdbServiceMock;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SyncGamesCommandHandler _handler;

    public SyncGamesCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<SyncGamesCommandHandler>>();
        _igdbServiceMock = new Mock<IIgdbService>();
        _gameRepositoryMock = new Mock<IGameRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new SyncGamesCommandHandler(
            _loggerMock.Object,
            _igdbServiceMock.Object,
            _gameRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidGames_ShouldSyncSuccessfully()
    {
        // Arrange
        var platformIds = new[] { 49, 169 }; // Xbox One, Xbox Series X|S
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(5);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _gameRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(
                It.Is<IEnumerable<Game>>(games => games.Count() == 5),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenApiReturnsNull_ShouldReturnSuccessWithoutSaving()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<IgdbGame>?)null!);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _gameRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenApiReturnsEmptyList_ShouldReturnSuccessWithoutSaving()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IgdbGame>());

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _gameRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapIgdbGamesToDomainGames()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGame = new IgdbGame
        {
            Id = 12345,
            Name = "Halo Infinite",
            Summary = "The next chapter in the legendary franchise",
            Storyline = "Master Chief returns",
            Genres = [5, 31]
        };

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { igdbGame });

        Game? capturedGame = null;
        _gameRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Game>, CancellationToken>((games, _) => capturedGame = games.First());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedGame.Should().NotBeNull();
        capturedGame!.Id.Should().Be(igdbGame.Id);
        capturedGame.Name.Should().Be(igdbGame.Name);
        capturedGame.Genres.Should().BeEquivalentTo(igdbGame.Genres);
        capturedGame.Data.Should().Contain("Halo Infinite");
        capturedGame.Data.Should().Contain("Master Chief returns");
    }

    [Fact]
    public async Task HandleAsync_ShouldSerializeGameDataAsJson()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(1);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        Game? capturedGame = null;
        _gameRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Game>, CancellationToken>((games, _) => capturedGame = games.First());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedGame.Should().NotBeNull();
        capturedGame!.Data.Should().NotBeNullOrEmpty();

        // Verify it's valid JSON
        var deserializedGame = System.Text.Json.JsonSerializer.Deserialize<IgdbGame>(capturedGame.Data);
        deserializedGame.Should().NotBeNull();
        deserializedGame!.Id.Should().Be(igdbGames[0].Id);
        deserializedGame.Name.Should().Be(igdbGames[0].Name);
    }

    [Fact]
    public async Task HandleAsync_WithMultiplePlatforms_ShouldPassAllPlatformIds()
    {
        // Arrange
        var platformIds = new[] { 49, 169, 6 }; // Xbox One, Xbox Series X|S, PC
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(10);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _igdbServiceMock.Verify(
            x => x.FetchGamesByPlatformsAsync(
                It.Is<int[]>(ids => ids.SequenceEqual(platformIds)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithSinglePlatform_ShouldWork()
    {
        // Arrange
        var platformIds = new[] { 169 }; // Xbox Series X|S only
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(3);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _gameRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(
                It.Is<IEnumerable<Game>>(games => games.Count() == 3),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(3);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        _gameRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.HandleAsync(command, CancellationToken.None));

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenSaveChangesThrowsException_ShouldThrowException()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(3);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to save"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldLogInformation_WhenFetchingGames()
    {
        // Arrange
        var platformIds = new[] { 49, 169 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(5);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetching games from IGDB API")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 5 games from IGDB API")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully synchronized")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogWarning_WhenNoGamesFound()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IgdbGame>());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No games found in the API response")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(3);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        _gameRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        try
        {
            await _handler.HandleAsync(command, CancellationToken.None);
        }
        catch
        {
            // Expected exception
        }

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to synchronize games")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await _handler.HandleAsync(command, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_WithEmptyPlatformIds_ShouldStillCallApi()
    {
        // Arrange
        var platformIds = Array.Empty<int>();
        var command = new SyncGamesCommand(platformIds);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IgdbGame>());

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _igdbServiceMock.Verify(
            x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldPreserveAllGameProperties()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGame = new IgdbGame
        {
            Id = 999,
            Name = "Test Game",
            Summary = "Test Summary",
            Storyline = "Test Storyline",
            Genres = new[] { 1, 2, 3 }
        };

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync([igdbGame]);

        Game? capturedGame = null;
        _gameRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Game>, CancellationToken>((games, _) => capturedGame = games.First());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedGame.Should().NotBeNull();
        capturedGame!.Id.Should().Be(999);
        capturedGame.Name.Should().Be("Test Game");
        capturedGame.Genres.Should().HaveCount(3);
        capturedGame.Genres.Should().ContainInOrder(1, 2, 3);
        capturedGame.Data.Should().Contain("Test Game");
        capturedGame.Data.Should().Contain("Test Summary");
        capturedGame.Data.Should().Contain("Test Storyline");
    }

    [Fact]
    public async Task HandleAsync_WithLargeNumberOfGames_ShouldHandleAll()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(100);

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _gameRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(
                It.Is<IEnumerable<Game>>(games => games.Count() == 100),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithGamesWithoutGenres_ShouldMapEmptyArray()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGame = new IgdbGame
        {
            Id = 123,
            Name = "Game Without Genres",
            Summary = "Test",
            Storyline = "Test",
            Genres = []
        };

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync([igdbGame]);

        Game? capturedGame = null;
        _gameRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Game>, CancellationToken>((games, _) => capturedGame = games.First());

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedGame.Should().NotBeNull();
        capturedGame!.Genres.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryBeforeUnitOfWork()
    {
        // Arrange
        var platformIds = new[] { 49 };
        var command = new SyncGamesCommand(platformIds);
        var igdbGames = TestDataBuilder.IgdbGameFaker().Generate(1);
        var callOrder = new List<string>();

        _igdbServiceMock
            .Setup(x => x.FetchGamesByPlatformsAsync(platformIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGames);

        _gameRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Game>>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Repository"));

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("UnitOfWork"))
            .ReturnsAsync(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        callOrder.Should().HaveCount(2);
        callOrder[0].Should().Be("Repository");
        callOrder[1].Should().Be("UnitOfWork");
    }
}
