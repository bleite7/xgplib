using Ardalis.Result;
using XgpLib.SyncService.Application.Genres.Queries.GetGenreById;
using XgpLib.SyncService.UnitTests.Helpers;

namespace XgpLib.SyncService.UnitTests.Application.Genres.Queries;

/// <summary>
/// Unit tests for GetGenreByIdQueryResponseHandler
/// </summary>
public class GetGenreByIdQueryResponseHandlerTests
{
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly GetGenreByIdQueryResponseHandler _handler;

    public GetGenreByIdQueryResponseHandlerTests()
    {
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _handler = new GetGenreByIdQueryResponseHandler(_genreRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithExistingGenre_ShouldReturnSuccessWithGenreResponse()
    {
        // Arrange
        var genre = TestDataBuilder.GenreFaker().Generate();
        var query = new GetGenreByIdQuery(genre.Id);

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert - Cast para Result<GenreResponse> para acessar propriedades de Result
        var typedResult = Assert.IsAssignableFrom<Result<GenreResponse>>(result);
        typedResult.Should().NotBeNull();
        typedResult.IsSuccess.Should().BeTrue();
        typedResult.Value.Should().NotBeNull();
        typedResult.Value!.Id.Should().Be(genre.Id);
        typedResult.Value.Name.Should().Be(genre.Name);
        typedResult.Value.Slug.Should().Be(genre.Slug);

        _genreRepositoryMock.Verify(
            x => x.GetGenreById(genre.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingGenre_ShouldReturnNotFound()
    {
        // Arrange
        var genreId = 999L;
        var query = new GetGenreByIdQuery(genreId);

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        var typedResult = Assert.IsAssignableFrom<Result<GenreResponse>>(result);
        typedResult.Should().NotBeNull();
        typedResult.IsSuccess.Should().BeFalse();
        typedResult.Status.Should().Be(ResultStatus.NotFound);

        _genreRepositoryMock.Verify(
            x => x.GetGenreById(genreId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1L)]
    [InlineData(100L)]
    [InlineData(999999L)]
    public async Task HandleAsync_WithDifferentGenreIds_ShouldQueryCorrectId(long genreId)
    {
        // Arrange
        var genre = TestDataBuilder.GenreFaker().Generate();
        genre.Id = genreId;
        var query = new GetGenreByIdQuery(genreId);

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        var typedResult = Assert.IsAssignableFrom<Result<GenreResponse>>(result);
        typedResult.IsSuccess.Should().BeTrue();
        typedResult.Value!.Id.Should().Be(genreId);

        _genreRepositoryMock.Verify(
            x => x.GetGenreById(genreId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapAllGenreProperties()
    {
        // Arrange
        var genre = new Genre
        {
            Id = 42,
            Name = "Adventure",
            Slug = "adventure",
            Data = "{\"test\": \"data\"}",
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "test-user"
        };
        var query = new GetGenreByIdQuery(genre.Id);

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        var typedResult = Assert.IsAssignableFrom<Result<GenreResponse>>(result);
        typedResult.IsSuccess.Should().BeTrue();
        typedResult.Value.Should().BeEquivalentTo(new GenreResponse(
            Id: genre.Id,
            Name: genre.Name,
            Slug: genre.Slug
        ));
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var genreId = 123L;
        var query = new GetGenreByIdQuery(genreId);
        var cts = new CancellationTokenSource();

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Genre?)null);

        // Act
        await _handler.HandleAsync(query, cts.Token);

        // Assert
        _genreRepositoryMock.Verify(
            x => x.GetGenreById(genreId, cts.Token),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var genreId = 123L;
        var query = new GetGenreByIdQuery(genreId);
        var expectedException = new InvalidOperationException("Database connection failed");

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genreId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database connection failed");
    }

    [Fact]
    public async Task HandleAsync_WithMultipleCalls_ShouldCallRepositoryEachTime()
    {
        // Arrange
        var genre = TestDataBuilder.GenreFaker().Generate();
        var query = new GetGenreByIdQuery(genre.Id);

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);
        await _handler.HandleAsync(query, CancellationToken.None);
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _genreRepositoryMock.Verify(
            x => x.GetGenreById(genre.Id, It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task HandleAsync_GenreResponse_ShouldBeRecord()
    {
        // Arrange
        var genre = TestDataBuilder.GenreFaker().Generate();
        var query = new GetGenreByIdQuery(genre.Id);

        _genreRepositoryMock
            .Setup(x => x.GetGenreById(genre.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(genre);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        var typedResult = Assert.IsAssignableFrom<Result<GenreResponse>>(result);
        var response = typedResult.Value;
        response.Should().NotBeNull();

        // Records have value equality
        var sameResponse = new GenreResponse(genre.Id, genre.Name, genre.Slug);
        response.Should().Be(sameResponse);
    }
}
