namespace XgpLib.SyncService.UnitTests.Helpers;

/// <summary>
/// Helper class for building test data using Bogus
/// </summary>
public static class TestDataBuilder
{
    /// <summary>
    /// Creates a Faker for Genre entities
    /// </summary>
    public static Faker<Genre> GenreFaker()
    {
        return new Faker<Genre>()
            .RuleFor(g => g.Id, f => f.Random.Long(1, 1000))
            .RuleFor(g => g.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(g => g.Slug, (f, g) => g.Name.ToLower().Replace(" ", "-"))
            .RuleFor(g => g.Data, f => f.Lorem.Paragraph())
            .RuleFor(g => g.CreatedAt, f => f.Date.PastOffset(1))
            .RuleFor(g => g.CreatedBy, f => f.Internet.UserName())
            .RuleFor(g => g.ModifiedAt, f => f.Date.RecentOffset())
            .RuleFor(g => g.LastModifiedBy, f => f.Internet.UserName());
    }

    /// <summary>
    /// Creates a Faker for Game entities
    /// </summary>
    public static Faker<Game> GameFaker()
    {
        return new Faker<Game>()
            .RuleFor(g => g.Id, f => f.Random.Long(1, 10000))
            .RuleFor(g => g.Name, f => f.Commerce.ProductName())
            .RuleFor(g => g.Genres, f => f.Random.ArrayElements([1, 2, 3, 4, 5], f.Random.Int(1, 3)))
            .RuleFor(g => g.Data, f => f.Lorem.Paragraph())
            .RuleFor(g => g.CreatedAt, f => f.Date.PastOffset(1))
            .RuleFor(g => g.CreatedBy, f => f.Internet.UserName())
            .RuleFor(g => g.ModifiedAt, f => f.Date.RecentOffset())
            .RuleFor(g => g.LastModifiedBy, f => f.Internet.UserName());
    }

    /// <summary>
    /// Creates a Faker for IgdbGenre DTOs
    /// </summary>
    public static Faker<IgdbGenre> IgdbGenreFaker()
    {
        return new Faker<IgdbGenre>()
            .CustomInstantiator(f => new IgdbGenre
            {
                Id = f.Random.Long(1, 1000),
                Name = f.Commerce.Categories(1)[0],
                Slug = f.Lorem.Slug()
            });
    }

    /// <summary>
    /// Creates a Faker for IgdbGame DTOs
    /// </summary>
    public static Faker<IgdbGame> IgdbGameFaker()
    {
        return new Faker<IgdbGame>()
            .CustomInstantiator(f => new IgdbGame
            {
                Id = f.Random.Long(1, 10000),
                Name = f.Commerce.ProductName(),
                Storyline = f.Lorem.Paragraph(),
                Summary = f.Lorem.Sentence(),
                Genres = f.Random.ArrayElements([1, 2, 3, 4, 5], f.Random.Int(1, 3))
            });
    }
}
