namespace XgpLib.SyncService.UnitTests.Domain.Entities;

/// <summary>
/// Unit tests for the BaseEntity class
/// </summary>
public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_ShouldInitializeWithDefaultId()
    {
        // Arrange & Act
        var entity = new TestBaseEntity();

        // Assert
        entity.Id.Should().Be(0);
    }

    [Fact]
    public void BaseEntity_ShouldAllowSettingId()
    {
        // Arrange
        var entity = new TestBaseEntity
        {
            // Act
            Id = 123
        };

        // Assert
        entity.Id.Should().Be(123);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    [InlineData(long.MaxValue)]
    public void BaseEntity_Id_ShouldAcceptVariousLongValues(long id)
    {
        // Arrange
        var entity = new TestBaseEntity
        {
            // Act
            Id = id
        };

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void BaseEntity_ShouldSupportNegativeIds()
    {
        // Arrange
        var entity = new TestBaseEntity
        {
            // Act
            Id = -1
        };

        // Assert
        entity.Id.Should().Be(-1);
    }

    /// <summary>
    /// Test implementation of BaseEntity for testing purposes
    /// </summary>
    private class TestBaseEntity : BaseEntity
    {
    }
}
