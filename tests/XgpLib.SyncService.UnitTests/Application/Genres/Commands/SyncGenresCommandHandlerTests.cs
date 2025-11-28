using System.Text.Json;
using XgpLib.SyncService.Application.Genres.Commands.SyncGenres;
using XgpLib.SyncService.UnitTests.Helpers;

namespace XgpLib.SyncService.UnitTests.Application.Genres.Commands;

/// <summary>
/// Unit tests for SyncGenresCommandHandler
/// </summary>
public class SyncGenresCommandHandlerTests
{
    private readonly Mock<ILogger<SyncGenresCommandHandler>> _loggerMock;
    private readonly Mock<IIgdbService> _igdbServiceMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SyncGenresCommandHandler _handler;

    public SyncGenresCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<SyncGenresCommandHandler>>();
        _igdbServiceMock = new Mock<IIgdbService>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new SyncGenresCommandHandler(
            _loggerMock.Object,
            _igdbServiceMock.Object,
            _genreRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidGenres_ShouldSyncSuccessfully()
    {
        // Arrange
        var command = new SyncGenresCommand();
        var igdbGenres = TestDataBuilder.IgdbGenreFaker().Generate(5);

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGenres);

        _genreRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _igdbServiceMock.Verify(
            x => x.FetchGenresAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _genreRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(
                It.Is<IEnumerable<Genre>>(g => g.Count() == 5),
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
        var command = new SyncGenresCommand();

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<IgdbGenre>?)null!);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _genreRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenApiReturnsEmptyList_ShouldReturnSuccessWithoutSaving()
    {
        // Arrange
        var command = new SyncGenresCommand();

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _genreRepositoryMock.Verify(
            x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapIgdbGenresToDomainGenres()
    {
        // Arrange
        var command = new SyncGenresCommand();
        var igdbGenre = new IgdbGenre
        {
            Id = 123,
            Name = "Action",
            Slug = "action"
        };

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([igdbGenre]);

        Genre? capturedGenre = null;
        _genreRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Genre>, CancellationToken>((genres, ct) => capturedGenre = genres.First())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedGenre.Should().NotBeNull();
        capturedGenre!.Id.Should().Be(123);
        capturedGenre.Name.Should().Be("Action");
        capturedGenre.Slug.Should().Be("action");
        capturedGenre.Data.Should().NotBeNullOrEmpty();

        // Verify JSON data contains the expected information
        var jsonData = JsonSerializer.Deserialize<IgdbGenre>(capturedGenre.Data);
        jsonData.Should().NotBeNull();
        jsonData!.Id.Should().Be(123);
        jsonData.Name.Should().Be("Action");
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var command = new SyncGenresCommand();
        var igdbGenres = TestDataBuilder.IgdbGenreFaker().Generate(3);

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGenres);

        var expectedException = new InvalidOperationException("Database error");
        _genreRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenSaveChangesThrowsException_ShouldThrowException()
    {
        // Arrange
        var command = new SyncGenresCommand();
        var igdbGenres = TestDataBuilder.IgdbGenreFaker().Generate(3);

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGenres);

        _genreRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var expectedException = new InvalidOperationException("Save failed");
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Save failed");
    }

    [Fact]
    public async Task HandleAsync_ShouldLogInformation_WhenFetchingGenres()
    {
        // Arrange
        var command = new SyncGenresCommand();
        var igdbGenres = TestDataBuilder.IgdbGenreFaker().Generate(5);

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGenres);

        _genreRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetching genres from IGDB API")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 5 genres from IGDB API")),
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
    public async Task HandleAsync_ShouldLogWarning_WhenNoGenresFound()
    {
        // Arrange
        var command = new SyncGenresCommand();

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No genres found in the API response")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var command = new SyncGenresCommand();
        var igdbGenres = TestDataBuilder.IgdbGenreFaker().Generate(3);

        _igdbServiceMock
            .Setup(x => x.FetchGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(igdbGenres);

        var expectedException = new InvalidOperationException("Database error");
        _genreRepositoryMock
            .Setup(x => x.AddOrUpdateRangeAsync(It.IsAny<IEnumerable<Genre>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to synchronize genres")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
