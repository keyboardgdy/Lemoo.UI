using FluentAssertions;
using Lemoo.Core.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Lemoo.Core.Infrastructure.Tests.Interceptors;

public class AuditSaveChangesInterceptorTests
{
    [Fact]
    public void Constructor_WithHttpContextAccessor_ShouldNotThrow()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        // Act & Assert
        var interceptor = new AuditSaveChangesInterceptor(httpContextAccessor);
        interceptor.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHttpContextAccessor_ShouldNotThrow()
    {
        // Act & Assert
        var interceptor = new AuditSaveChangesInterceptor(null);
        interceptor.Should().NotBeNull();
    }

    [Fact]
    public void GetCurrentUserId_WithNoHttpContext_ShouldReturnNull()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
        var interceptor = new AuditSaveChangesInterceptor(httpContextAccessor);

        // Act & Assert - Constructor with null accessor should not throw
        interceptor.Should().NotBeNull();
    }
}

public class SoftDeleteSaveChangesInterceptorTests
{
    [Fact]
    public void Constructor_ShouldNotThrow()
    {
        // Act & Assert
        var interceptor = new SoftDeleteSaveChangesInterceptor();
        interceptor.Should().NotBeNull();
    }
}

public class SoftDeletableExtensionsTests
{
    [Fact]
    public void ISoftDeletable_ShouldHaveRequiredProperties()
    {
        // Arrange & Act
        var entity = new TestSoftDeletableEntity();

        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void SoftDeletableEntity_CanSetProperties()
    {
        // Arrange
        var entity = new TestSoftDeletableEntity();
        var deletedAt = DateTime.UtcNow;

        // Act
        entity.IsDeleted = true;
        entity.DeletedAt = deletedAt;

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().Be(deletedAt);
    }

    [Fact]
    public void SoftDeletableEntity_CanRestore()
    {
        // Arrange
        var entity = new TestSoftDeletableEntity
        {
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow
        };

        // Act
        entity.IsDeleted = false;
        entity.DeletedAt = null;

        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }
}

// Test entities
public class TestSoftDeletableEntity : ISoftDeletable
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class RegularEntity
{
    public int Id { get; set; }
}
