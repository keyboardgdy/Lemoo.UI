using FluentAssertions;
using Lemoo.UI.Abstractions;
using Lemoo.UI.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lemoo.UI.WPF.Tests.Services;

public class PageRegistryServiceTests
{
    private readonly PageRegistryService _pageRegistry;

    public PageRegistryServiceTests()
    {
        _pageRegistry = new PageRegistryService();
    }

    [Fact]
    public void RegisterPage_WithValidParameters_ShouldRegisterPage()
    {
        // Arrange
        var pageKey = "TestPage";
        var pageType = typeof(TestPage);
        var metadata = new NavigationItemMetadata
        {
            PageKey = pageKey,
            Title = "Test Page",
            Icon = "T",
            Module = "TestModule"
        };

        // Act
        _pageRegistry.RegisterPage(pageKey, pageType, metadata);

        // Assert
        var registeredType = _pageRegistry.GetPageType(pageKey);
        registeredType.Should().Be(pageType);
    }

    [Fact]
    public void RegisterPage_WithNullPageKey_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _pageRegistry.RegisterPage(null!, typeof(TestPage), new NavigationItemMetadata()));
    }

    [Fact]
    public void RegisterPage_WithEmptyPageKey_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _pageRegistry.RegisterPage("", typeof(TestPage), new NavigationItemMetadata()));
    }

    [Fact]
    public void RegisterPage_WithNullPageType_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _pageRegistry.RegisterPage("TestPage", null!, new NavigationItemMetadata()));
    }

    [Fact]
    public void RegisterPage_WithNullMetadata_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _pageRegistry.RegisterPage("TestPage", typeof(TestPage), null!));
    }

    [Fact]
    public void GetPageType_WhenPageNotRegistered_ShouldReturnNull()
    {
        // Act
        var type = _pageRegistry.GetPageType("NonExistentPage");

        // Assert
        type.Should().BeNull();
    }

    [Fact]
    public void GetPageType_WhenPageRegistered_ShouldReturnCorrectType()
    {
        // Arrange
        var pageKey = "TestPage";
        var pageType = typeof(TestPage);
        var metadata = new NavigationItemMetadata { PageKey = pageKey };
        _pageRegistry.RegisterPage(pageKey, pageType, metadata);

        // Act
        var result = _pageRegistry.GetPageType(pageKey);

        // Assert
        result.Should().Be(pageType);
    }

    [Fact]
    public void CreatePage_WithServiceProvider_ShouldCreatePage()
    {
        // Arrange
        var pageKey = "TestPage";
        var pageType = typeof(TestPage);
        var metadata = new NavigationItemMetadata { PageKey = pageKey };
        _pageRegistry.RegisterPage(pageKey, pageType, metadata);

        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        var page = _pageRegistry.CreatePage(pageKey, serviceProvider);

        // Assert
        page.Should().NotBeNull();
        page.Should().BeOfType<TestPage>();
    }

    [Fact]
    public void CreatePage_WhenPageNotRegistered_ShouldReturnNull()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        var page = _pageRegistry.CreatePage("NonExistentPage", serviceProvider);

        // Assert
        page.Should().BeNull();
    }

    [Fact]
    public void GetPagesByModule_WhenModuleExists_ShouldReturnPages()
    {
        // Arrange
        var metadata1 = new NavigationItemMetadata
        {
            PageKey = "Page1",
            Module = "TestModule"
        };
        var metadata2 = new NavigationItemMetadata
        {
            PageKey = "Page2",
            Module = "TestModule"
        };
        var metadata3 = new NavigationItemMetadata
        {
            PageKey = "Page3",
            Module = "OtherModule"
        };

        _pageRegistry.RegisterPage("Page1", typeof(TestPage), metadata1);
        _pageRegistry.RegisterPage("Page2", typeof(TestPage), metadata2);
        _pageRegistry.RegisterPage("Page3", typeof(TestPage), metadata3);

        // Act
        var testModulePages = _pageRegistry.GetPagesByModule("TestModule");

        // Assert
        testModulePages.Should().HaveCount(2);
        testModulePages.Should().OnlyContain(p => p.Module == "TestModule");
    }

    [Fact]
    public void GetPagesByModule_WhenModuleDoesNotExist_ShouldReturnEmpty()
    {
        // Act
        var pages = _pageRegistry.GetPagesByModule("NonExistentModule");

        // Assert
        pages.Should().BeEmpty();
    }

    [Fact]
    public void GetAllPages_ShouldReturnAllRegisteredPages()
    {
        // Arrange
        var metadata1 = new NavigationItemMetadata { PageKey = "Page1", Module = "Module1" };
        var metadata2 = new NavigationItemMetadata { PageKey = "Page2", Module = "Module2" };

        _pageRegistry.RegisterPage("Page1", typeof(TestPage), metadata1);
        _pageRegistry.RegisterPage("Page2", typeof(TestPage), metadata2);

        // Act
        var allPages = _pageRegistry.GetAllPages();

        // Assert
        allPages.Should().HaveCount(2);
        allPages.Should().ContainKey("Page1");
        allPages.Should().ContainKey("Page2");
    }

    [Fact]
    public void GetAllPages_ShouldReturnReadOnlyDictionary()
    {
        // Arrange
        var metadata = new NavigationItemMetadata { PageKey = "Page1" };
        _pageRegistry.RegisterPage("Page1", typeof(TestPage), metadata);

        // Act
        var allPages = _pageRegistry.GetAllPages();

        // Assert
        allPages.Should().BeReadOnly();
    }

    // Test page class
    private class TestPage
    {
        public TestPage() { }
    }
}
