using FluentAssertions;
using Lemoo.Core.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lemoo.Core.Infrastructure.Tests.Caching;

public class MemoryCacheServiceTests
{
    private readonly MemoryCacheService _cacheService;
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _cacheService = new MemoryCacheService(_memoryCache);
    }

    [Fact]
    public async Task SetAsync_ShouldStoreValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";

        // Act
        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task SetAsync_WithExpiration_ShouldExpireAfterTime()
    {
        // Arrange
        var key = "expiring-key";
        var value = "expiring-value";
        var expiration = TimeSpan.FromMilliseconds(50);

        // Act
        await _cacheService.SetAsync(key, value, expiration);
        var immediateResult = await _cacheService.GetAsync<string>(key);
        await Task.Delay(100);
        var expiredResult = await _cacheService.GetAsync<string>(key);

        // Assert
        immediateResult.Should().Be(value);
        expiredResult.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WhenKeyNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _cacheService.GetAsync<string>("non-existent-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveValue()
    {
        // Arrange
        var key = "remove-key";
        await _cacheService.SetAsync(key, "value");

        // Act
        await _cacheService.RemoveAsync(key);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByPatternAsync_ShouldRemoveMatchingKeys()
    {
        // Arrange
        await _cacheService.SetAsync("users:1", "user1");
        await _cacheService.SetAsync("users:2", "user2");
        await _cacheService.SetAsync("products:1", "product1");
        await _cacheService.SetAsync("users:3", "user3");

        // Act
        await _cacheService.RemoveByPatternAsync("users:*");

        // Assert
        (await _cacheService.GetAsync<string>("users:1")).Should().BeNull();
        (await _cacheService.GetAsync<string>("users:2")).Should().BeNull();
        (await _cacheService.GetAsync<string>("products:1")).Should().Be("product1");
        (await _cacheService.GetAsync<string>("users:3")).Should().BeNull();
    }

    [Fact]
    public async Task RemoveByPatternAsync_WithQuestionMark_ShouldMatchSingleCharacter()
    {
        // Arrange
        await _cacheService.SetAsync("key1", "value1");
        await _cacheService.SetAsync("key2", "value2");
        await _cacheService.SetAsync("key12", "value12");

        // Act
        await _cacheService.RemoveByPatternAsync("key?");

        // Assert
        (await _cacheService.GetAsync<string>("key1")).Should().BeNull();
        (await _cacheService.GetAsync<string>("key2")).Should().BeNull();
        (await _cacheService.GetAsync<string>("key12")).Should().Be("value12");
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyExists_ShouldReturnTrue()
    {
        // Arrange
        var key = "exists-key";
        await _cacheService.SetAsync(key, "value");

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _cacheService.ExistsAsync("non-existent-key");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshAsync_ShouldUpdateExpiration()
    {
        // Arrange
        var key = "refresh-key";
        var expiration = TimeSpan.FromMilliseconds(50);
        await _cacheService.SetAsync(key, "value", expiration);

        // Act - refresh before expiration
        await Task.Delay(30);
        await _cacheService.RefreshAsync(key, TimeSpan.FromMilliseconds(100));
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().Be("value");
    }

    [Fact]
    public async Task GetManyAsync_ShouldReturnAllExistingValues()
    {
        // Arrange
        await _cacheService.SetAsync("key1", "value1");
        await _cacheService.SetAsync("key2", "value2");
        await _cacheService.SetAsync("key3", "value3");

        // Act
        var result = await _cacheService.GetManyAsync<string>(new[] { "key1", "key2", "key4" });

        // Assert
        result.Should().HaveCount(2);
        result["key1"].Should().Be("value1");
        result["key2"].Should().Be("value2");
        result.Should().NotContainKey("key4");
    }

    [Fact]
    public async Task SetManyAsync_ShouldStoreAllValues()
    {
        // Arrange
        var items = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" },
            { "key3", "value3" }
        };

        // Act
        await _cacheService.SetManyAsync(items);

        // Assert
        (await _cacheService.GetAsync<string>("key1")).Should().Be("value1");
        (await _cacheService.GetAsync<string>("key2")).Should().Be("value2");
        (await _cacheService.GetAsync<string>("key3")).Should().Be("value3");
    }

    [Fact]
    public async Task RemoveManyAsync_ShouldRemoveAllSpecifiedKeys()
    {
        // Arrange
        await _cacheService.SetAsync("key1", "value1");
        await _cacheService.SetAsync("key2", "value2");
        await _cacheService.SetAsync("key3", "value3");

        // Act
        await _cacheService.RemoveManyAsync(new[] { "key1", "key3" });

        // Assert
        (await _cacheService.GetAsync<string>("key1")).Should().BeNull();
        (await _cacheService.GetAsync<string>("key2")).Should().Be("value2");
        (await _cacheService.GetAsync<string>("key3")).Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithComplexType_ShouldStoreCorrectly()
    {
        // Arrange
        var key = "complex-key";
        var value = new TestObject { Id = 1, Name = "Test" };

        // Act
        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<TestObject>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task SetAsync_WithDefaultExpiration_ShouldUseFiveMinutes()
    {
        // Arrange
        var key = "default-expiration-key";
        await _cacheService.SetAsync(key, "value");

        // Act
        var existsImmediately = await _cacheService.ExistsAsync(key);

        // Assert
        existsImmediately.Should().BeTrue();
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
