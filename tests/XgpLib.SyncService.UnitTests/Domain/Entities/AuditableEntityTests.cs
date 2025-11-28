namespace XgpLib.SyncService.UnitTests.Domain.Entities;

/// <summary>
/// Unit tests for the AuditableEntity base class
/// </summary>
public class AuditableEntityTests
{
    [Fact]
    public void AuditableEntity_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var entity = new TestAuditableEntity();

        // Assert
        entity.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void AuditableEntity_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var entity = new TestAuditableEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.CreatedAt.Should().Be(default);
        entity.CreatedBy.Should().BeNull();
        entity.ModifiedAt.Should().Be(default);
        entity.LastModifiedBy.Should().BeNull();
    }

    [Fact]
    public void AuditableEntity_ShouldAllowSettingCreatedAt()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        entity.CreatedAt = timestamp;

        // Assert
        entity.CreatedAt.Should().Be(timestamp);
    }

    [Fact]
    public void AuditableEntity_ShouldAllowSettingCreatedBy()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        var creator = "test-user";

        // Act
        entity.CreatedBy = creator;

        // Assert
        entity.CreatedBy.Should().Be(creator);
    }

    [Fact]
    public void AuditableEntity_ShouldAllowSettingModifiedAt()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        entity.ModifiedAt = timestamp;

        // Assert
        entity.ModifiedAt.Should().Be(timestamp);
    }

    [Fact]
    public void AuditableEntity_ShouldAllowSettingLastModifiedBy()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        var modifier = "admin-user";

        // Act
        entity.LastModifiedBy = modifier;

        // Assert
        entity.LastModifiedBy.Should().Be(modifier);
    }

    [Fact]
    public void AuditableEntity_ShouldAllowNullableAuditFields()
    {
        // Arrange
        var entity = new TestAuditableEntity
        {
            CreatedBy = "user1",
            LastModifiedBy = "user2"
        };

        // Act
        entity.CreatedBy = null;
        entity.LastModifiedBy = null;

        // Assert
        entity.CreatedBy.Should().BeNull();
        entity.LastModifiedBy.Should().BeNull();
    }

    [Fact]
    public void AuditableEntity_ShouldSupportAuditTrail()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        var createdTime = DateTimeOffset.UtcNow.AddDays(-1);
        var modifiedTime = DateTimeOffset.UtcNow;

        // Act
        entity.CreatedAt = createdTime;
        entity.CreatedBy = "creator";
        entity.ModifiedAt = modifiedTime;
        entity.LastModifiedBy = "modifier";

        // Assert
        entity.CreatedAt.Should().Be(createdTime);
        entity.CreatedBy.Should().Be("creator");
        entity.ModifiedAt.Should().Be(modifiedTime);
        entity.LastModifiedBy.Should().Be("modifier");
        entity.ModifiedAt.Should().BeAfter(entity.CreatedAt);
    }

    /// <summary>
    /// Test implementation of AuditableEntity for testing purposes
    /// </summary>
    private class TestAuditableEntity : AuditableEntity
    {
    }
}
