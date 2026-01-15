using FluentAssertions;
using Lemoo.UI.Abstractions;
using Lemoo.UI.WPF.Abstractions;
using Lemoo.UI.WPF.Models;
using Lemoo.UI.WPF.Services;
using Moq;

namespace Lemoo.UI.WPF.Tests.Services;

public class NavigationServiceTests
{
    private readonly Mock<IPageRegistry> _pageRegistryMock;
    private readonly Mock<IMainViewModel> _mainViewModelMock;
    private readonly NavigationService _navigationService;

    public NavigationServiceTests()
    {
        _pageRegistryMock = new Mock<IPageRegistry>();
        _mainViewModelMock = new Mock<IMainViewModel>();
        _navigationService = new NavigationService(_pageRegistryMock.Object);
    }

    [Fact]
    public void Constructor_WithNullPageRegistry_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new NavigationService(null!));
    }

    [Fact]
    public void BuildNavigationTree_WithNullMainViewModel_ShouldThrowArgumentNullException()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _navigationService.BuildNavigationTree(null!, navItems));
    }

    [Fact]
    public void BuildNavigationTree_WithValidItems_ShouldPopulateNavigationItems()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>
        {
            new()
            {
                PageKey = "Page1",
                Title = "Page 1",
                Icon = "1",
                Module = "ModuleA",
                Order = 1,
                IsEnabled = true
            },
            new()
            {
                PageKey = "Page2",
                Title = "Page 2",
                Icon = "2",
                Module = "ModuleA",
                Order = 2,
                IsEnabled = true
            }
        };

        // Act
        _navigationService.BuildNavigationTree(_mainViewModelMock.Object, navItems);

        // Assert
        _mainViewModelMock.Verify(vm => vm.NavigationItems.Clear(), Times.Once);
        _mainViewModelMock.Verify(vm => vm.BottomNavigationItems.Clear(), Times.Once);
        _mainViewModelMock.Verify(vm => vm.NavigationItems.Add(It.IsAny<NavigationItem>()), Times.Exactly(2));
    }

    [Fact]
    public void BuildNavigationTree_WithDisabledItems_ShouldFilterThemOut()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>
        {
            new()
            {
                PageKey = "EnabledPage",
                Title = "Enabled Page",
                Icon = "E",
                Module = "ModuleA",
                IsEnabled = true
            },
            new()
            {
                PageKey = "DisabledPage",
                Title = "Disabled Page",
                Icon = "D",
                Module = "ModuleA",
                IsEnabled = false
            }
        };

        // Act
        _navigationService.BuildNavigationTree(_mainViewModelMock.Object, navItems);

        // Assert
        _mainViewModelMock.Verify(vm => vm.NavigationItems.Add(It.IsAny<NavigationItem>()), Times.Once);
    }

    [Fact]
    public void BuildNavigationTree_WithParentChildRelationship_ShouldBuildHierarchy()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>
        {
            new()
            {
                PageKey = "ParentPage",
                Title = "Parent",
                Icon = "P",
                Module = "ModuleA",
                Order = 1,
                IsEnabled = true
            },
            new()
            {
                PageKey = "ChildPage",
                Title = "Child",
                Icon = "C",
                Module = "ModuleA",
                Order = 1,
                ParentPageKey = "ParentPage",
                IsEnabled = true
            }
        };

        NavigationItem? capturedParentItem = null;
        _mainViewModelMock.Setup(vm => vm.NavigationItems.Add(It.IsAny<NavigationItem>()))
            .Callback<NavigationItem>(item => capturedParentItem = item);

        // Act
        _navigationService.BuildNavigationTree(_mainViewModelMock.Object, navItems);

        // Assert
        capturedParentItem.Should().NotBeNull();
        capturedParentItem!.Children.Should().HaveCount(1);
        capturedParentItem.Children[0].Title.Should().Be("Child");
    }

    [Fact]
    public void BuildNavigationTree_ShouldSortByModuleAndOrder()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>
        {
            new()
            {
                PageKey = "Page3",
                Title = "Page 3",
                Icon = "3",
                Module = "ModuleB",
                Order = 2,
                IsEnabled = true
            },
            new()
            {
                PageKey = "Page1",
                Title = "Page 1",
                Icon = "1",
                Module = "ModuleA",
                Order = 1,
                IsEnabled = true
            },
            new()
            {
                PageKey = "Page2",
                Title = "Page 2",
                Icon = "2",
                Module = "ModuleA",
                Order = 2,
                IsEnabled = true
            }
        };

        var addedItems = new List<NavigationItem>();
        _mainViewModelMock.Setup(vm => vm.NavigationItems.Add(It.IsAny<NavigationItem>()))
            .Callback<NavigationItem>(item => addedItems.Add(item));

        // Act
        _navigationService.BuildNavigationTree(_mainViewModelMock.Object, navItems);

        // Assert
        addedItems.Should().HaveCount(3);
        addedItems[0].Title.Should().Be("Page 1"); // ModuleA, Order 1
        addedItems[1].Title.Should().Be("Page 2"); // ModuleA, Order 2
        addedItems[2].Title.Should().Be("Page 3"); // ModuleB, Order 2
    }

    [Fact]
    public void BuildNavigationTreeFromRegistry_WithValidRegistry_ShouldUseRegistryPages()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>
        {
            new()
            {
                PageKey = "Page1",
                Title = "Page 1",
                Icon = "1",
                Module = "ModuleA",
                IsEnabled = true
            }
        };

        var allPages = new Dictionary<string, NavigationItemMetadata>
        {
            { "Page1", navItems[0] }
        };

        _pageRegistryMock.Setup(r => r.GetAllPages()).Returns(allPages);

        // Act
        _navigationService.BuildNavigationTreeFromRegistry(_mainViewModelMock.Object, _pageRegistryMock.Object);

        // Assert
        _pageRegistryMock.Verify(r => r.GetAllPages(), Times.Once);
        _mainViewModelMock.Verify(vm => vm.NavigationItems.Add(It.IsAny<NavigationItem>()), Times.Once);
    }

    [Fact]
    public void BuildNavigationTreeFromRegistry_WithNullRegistry_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _navigationService.BuildNavigationTreeFromRegistry(_mainViewModelMock.Object, null!));
    }

    [Fact]
    public void BuildNavigationTree_WithMultipleChildren_ShouldAddAllChildren()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>
        {
            new()
            {
                PageKey = "ParentPage",
                Title = "Parent",
                Icon = "P",
                Module = "ModuleA",
                Order = 1,
                IsEnabled = true
            },
            new()
            {
                PageKey = "Child1",
                Title = "Child 1",
                Icon = "1",
                Module = "ModuleA",
                Order = 1,
                ParentPageKey = "ParentPage",
                IsEnabled = true
            },
            new()
            {
                PageKey = "Child2",
                Title = "Child 2",
                Icon = "2",
                Module = "ModuleA",
                Order = 2,
                ParentPageKey = "ParentPage",
                IsEnabled = true
            }
        };

        NavigationItem? capturedParentItem = null;
        _mainViewModelMock.Setup(vm => vm.NavigationItems.Add(It.IsAny<NavigationItem>()))
            .Callback<NavigationItem>(item => capturedParentItem = item);

        // Act
        _navigationService.BuildNavigationTree(_mainViewModelMock.Object, navItems);

        // Assert
        capturedParentItem.Should().NotBeNull();
        capturedParentItem!.Children.Should().HaveCount(2);
        capturedParentItem.Children[0].Title.Should().Be("Child 1");
        capturedParentItem.Children[1].Title.Should().Be("Child 2");
    }

    [Fact]
    public void BuildNavigationTree_WithEmptyList_ShouldClearNavigation()
    {
        // Arrange
        var navItems = new List<NavigationItemMetadata>();

        // Act
        _navigationService.BuildNavigationTree(_mainViewModelMock.Object, navItems);

        // Assert
        _mainViewModelMock.Verify(vm => vm.NavigationItems.Clear(), Times.Once);
        _mainViewModelMock.Verify(vm => vm.NavigationItems.Add(It.IsAny<NavigationItem>()), Times.Never);
    }
}
