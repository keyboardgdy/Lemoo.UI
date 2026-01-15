using FluentAssertions;
using Lemoo.UI.WPF.Abstractions;
using Lemoo.UI.WPF.Models;
using Lemoo.UI.WPF.ViewModels;
using Moq;

namespace Lemoo.UI.WPF.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultTitle()
    {
        // Act
        var viewModel = new MainViewModel();

        // Assert
        viewModel.Title.Should().Be("Lemoo - UI 示例");
    }

    [Fact]
    public void Constructor_ShouldInitializeCollections()
    {
        // Act
        var viewModel = new MainViewModel();

        // Assert
        viewModel.MenuItems.Should().NotBeNull();
        viewModel.NavigationItems.Should().NotBeNull();
        viewModel.BottomNavigationItems.Should().NotBeNull();
        viewModel.MenuItems.Should().BeEmpty();
        viewModel.NavigationItems.Should().BeEmpty();
        viewModel.BottomNavigationItems.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldInitializeDefaultNavigation()
    {
        // Act
        var viewModel = new MainViewModel();

        // Assert
        viewModel.MenuItems.Should().NotBeEmpty();
        viewModel.NavigationItems.Should().NotBeEmpty();
        viewModel.BottomNavigationItems.Should().NotBeEmpty();
    }

    [Fact]
    public void InitializeDefaultNavigation_ShouldClearAndRebuildNavigation()
    {
        // Arrange
        var viewModel = new MainViewModel();
        var originalMenuCount = viewModel.MenuItems.Count;
        var originalNavCount = viewModel.NavigationItems.Count;

        // Add some extra items
        viewModel.MenuItems.Add(new MenuItemModel { Header = "Extra" });
        viewModel.NavigationItems.Add(new NavigationItem { Title = "Extra" });

        // Act
        viewModel.InitializeDefaultNavigation();

        // Assert
        viewModel.MenuItems.Count.Should().Be(originalMenuCount);
        viewModel.NavigationItems.Count.Should().Be(originalNavCount);
    }

    [Fact]
    public void InitializeNavigationFromMetadata_WithValidItems_ShouldPopulateCollections()
    {
        // Arrange
        var viewModel = new MainViewModel();
        viewModel.ClearNavigation();

        var menuItems = new List<MenuItemModel>
        {
            new() { Header = "Test Menu", Icon = "T" }
        };

        var navItems = new List<NavigationItem>
        {
            new() { Title = "Test Nav", Icon = "N", PageKey = "Test" }
        };

        var bottomNavItems = new List<NavigationItem>
        {
            new() { Title = "Bottom Nav", Icon = "B", PageKey = "Bottom" }
        };

        // Act
        viewModel.InitializeNavigationFromMetadata(menuItems, navItems, bottomNavItems);

        // Assert
        viewModel.MenuItems.Should().HaveCount(1);
        viewModel.NavigationItems.Should().HaveCount(1);
        viewModel.BottomNavigationItems.Should().HaveCount(1);
        viewModel.MenuItems[0].Header.Should().Be("Test Menu");
        viewModel.NavigationItems[0].Title.Should().Be("Test Nav");
        viewModel.BottomNavigationItems[0].Title.Should().Be("Bottom Nav");
    }

    [Fact]
    public void InitializeNavigationFromMetadata_WithoutBottomNav_ShouldNotPopulateBottomNav()
    {
        // Arrange
        var viewModel = new MainViewModel();
        viewModel.ClearNavigation();

        var menuItems = new List<MenuItemModel>
        {
            new() { Header = "Test Menu" }
        };

        var navItems = new List<NavigationItem>
        {
            new() { Title = "Test Nav", PageKey = "Test" }
        };

        // Act
        viewModel.InitializeNavigationFromMetadata(menuItems, navItems);

        // Assert
        viewModel.MenuItems.Should().HaveCount(1);
        viewModel.NavigationItems.Should().HaveCount(1);
        viewModel.BottomNavigationItems.Should().BeEmpty();
    }

    [Fact]
    public void InitializeNavigationFromMetadata_WithNullBottomNav_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new MainViewModel();
        viewModel.ClearNavigation();

        var menuItems = new List<MenuItemModel>();
        var navItems = new List<NavigationItem>();

        // Act & Assert
        var act = () => viewModel.InitializeNavigationFromMetadata(menuItems, navItems, null);
        act.Should().NotThrow();
    }

    [Fact]
    public void ClearNavigation_ShouldClearAllCollections()
    {
        // Arrange
        var viewModel = new MainViewModel();

        // Ensure there are items to clear
        viewModel.MenuItems.Should().NotBeEmpty();
        viewModel.NavigationItems.Should().NotBeEmpty();
        viewModel.BottomNavigationItems.Should().NotBeEmpty();

        // Act
        viewModel.ClearNavigation();

        // Assert
        viewModel.MenuItems.Should().BeEmpty();
        viewModel.NavigationItems.Should().BeEmpty();
        viewModel.BottomNavigationItems.Should().BeEmpty();
    }

    [Fact]
    public void StatusMessage_ShouldBeSettable()
    {
        // Arrange
        var viewModel = new MainViewModel();

        // Act
        viewModel.StatusMessage = "Test Status";

        // Assert
        viewModel.StatusMessage.Should().Be("Test Status");
    }

    [Fact]
    public void IsLoading_ShouldBeSettable()
    {
        // Arrange
        var viewModel = new MainViewModel();

        // Act
        viewModel.IsLoading = true;

        // Assert
        viewModel.IsLoading.Should().BeTrue();
    }

    [Fact]
    public void InitializeNavigationFromMetadata_ShouldClearExistingItemsFirst()
    {
        // Arrange
        var viewModel = new MainViewModel();
        viewModel.InitializeDefaultNavigation();

        var originalMenuCount = viewModel.MenuItems.Count;
        var originalNavCount = viewModel.NavigationItems.Count;

        var newMenuItems = new List<MenuItemModel>
        {
            new() { Header = "New Menu" }
        };

        var newNavItems = new List<NavigationItem>
        {
            new() { Title = "New Nav" }
        };

        // Act
        viewModel.InitializeNavigationFromMetadata(newMenuItems, newNavItems);

        // Assert
        viewModel.MenuItems.Should().HaveCount(1, "because old items should be cleared");
        viewModel.NavigationItems.Should().HaveCount(1, "because old items should be cleared");
        viewModel.BottomNavigationItems.Should().BeEmpty("because no bottom nav items were provided");
    }

    [Fact]
    public void InitializeNavigationFromMetadata_WithMultipleItems_ShouldAddAllItems()
    {
        // Arrange
        var viewModel = new MainViewModel();
        viewModel.ClearNavigation();

        var menuItems = new List<MenuItemModel>
        {
            new() { Header = "Menu 1" },
            new() { Header = "Menu 2" },
            new() { Header = "Menu 3" }
        };

        var navItems = new List<NavigationItem>
        {
            new() { Title = "Nav 1", PageKey = "Nav1" },
            new() { Title = "Nav 2", PageKey = "Nav2" }
        };

        // Act
        viewModel.InitializeNavigationFromMetadata(menuItems, navItems);

        // Assert
        viewModel.MenuItems.Should().HaveCount(3);
        viewModel.NavigationItems.Should().HaveCount(2);
    }

    [Fact]
    public void DefaultNavigation_ContainsExpectedPages()
    {
        // Arrange & Act
        var viewModel = new MainViewModel();

        // Assert
        // Check that we have the sample menu
        viewModel.MenuItems.Should().ContainSingle(m => m.Header == "示例");

        // Check that we have navigation items
        viewModel.NavigationItems.Should().NotBeEmpty();

        // Check that we have bottom navigation
        viewModel.BottomNavigationItems.Should().NotBeEmpty();
    }

    [Fact]
    public void ClearNavigation_CanBeCalledMultipleTimes()
    {
        // Arrange
        var viewModel = new MainViewModel();

        // Act
        viewModel.ClearNavigation();
        viewModel.ClearNavigation();
        viewModel.ClearNavigation();

        // Assert
        viewModel.MenuItems.Should().BeEmpty();
        viewModel.NavigationItems.Should().BeEmpty();
        viewModel.BottomNavigationItems.Should().BeEmpty();
    }

    [Fact]
    public void InitializeDefaultNavigation_AfterClear_ShouldRepopulate()
    {
        // Arrange
        var viewModel = new MainViewModel();
        viewModel.ClearNavigation();

        // Act
        viewModel.InitializeDefaultNavigation();

        // Assert
        viewModel.MenuItems.Should().NotBeEmpty();
        viewModel.NavigationItems.Should().NotBeEmpty();
        viewModel.BottomNavigationItems.Should().NotBeEmpty();
    }
}
