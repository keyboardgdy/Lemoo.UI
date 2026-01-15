using CommunityToolkit.Mvvm.Input;
using FluentAssertions;
using Lemoo.UI.Models;
using Lemoo.UI.ViewModels;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Toolbox;

/// <summary>
/// ToolboxViewModel的单元测试
/// 注：ToolboxView需要完整的WPF应用程序上下文（包括样式资源）才能初始化，
/// 因此这里只测试ViewModel的逻辑，不测试View的UI部分
/// </summary>
public class ToolboxViewModelTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_InitializeWithDefaultValues()
        {
            // Act
            var viewModel = new ToolboxViewModel();

            // Assert
            viewModel.Should().NotBeNull();
            viewModel.SearchKeyword.Should().BeEmpty();
            viewModel.SelectedControl.Should().BeNull();
            viewModel.SelectedStyleVariant.Should().BeNull();
            viewModel.IsSearching.Should().BeFalse();
        }

        [StaFact]
        public void Should_LoadAllControls_WhenCreated()
        {
            // Act
            var viewModel = new ToolboxViewModel();

            // Assert
            viewModel.AllControls.Should().NotBeEmpty();
        }

        [StaFact]
        public void Should_InitializeCategories_WhenCreated()
        {
            // Act
            var viewModel = new ToolboxViewModel();

            // Assert
            viewModel.Categories.Should().NotBeEmpty();
        }

        [StaFact]
        public void Should_ExpandAllCategories_WhenCreated()
        {
            // Act
            var viewModel = new ToolboxViewModel();

            // Assert
            foreach (ControlCategory category in Enum.GetValues(typeof(ControlCategory)))
            {
                var hasControls = Services.ControlRegistry.GetControlsByCategory(category).Count > 0;
                if (hasControls)
                {
                    viewModel.ExpandedCategories.Should().Contain(category);
                }
            }
        }
    }

    public class SearchKeyword_Property
    {
        [StaFact]
        public void Should_SetIsSearchingTrue_WhenKeywordIsNotEmpty()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            viewModel.SearchKeyword = "Button";

            // Assert
            viewModel.IsSearching.Should().BeTrue();
        }

        [StaFact]
        public void Should_SetIsSearchingFalse_WhenKeywordIsEmpty()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.SearchKeyword = "Button";

            // Act
            viewModel.SearchKeyword = "";

            // Assert
            viewModel.IsSearching.Should().BeFalse();
        }

        [StaFact]
        public void Should_SetIsSearchingFalse_WhenKeywordIsWhitespace()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.SearchKeyword = "Button";

            // Act
            viewModel.SearchKeyword = "   ";

            // Assert
            viewModel.IsSearching.Should().BeFalse();
        }

        [StaFact]
        public void Should_FilterControls_WhenSearchingByName()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            viewModel.SearchKeyword = "Button";

            // Assert
            viewModel.SearchResults.Should().NotBeEmpty();
            viewModel.SearchResults.Should().OnlyContain(c =>
                c.Name.Contains("Button", StringComparison.OrdinalIgnoreCase) ||
                c.DisplayName.Contains("Button", StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains("Button", StringComparison.OrdinalIgnoreCase));
        }

        [StaFact]
        public void Should_BeCaseInsensitive_WhenSearching()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            viewModel.SearchKeyword = "button";

            // Assert
            viewModel.SearchResults.Should().NotBeEmpty();
            viewModel.SearchResults.Should().Contain(c => c.Name == "Button");
        }

        [StaFact]
        public void Should_ReturnEmpty_WhenNoMatchesFound()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            viewModel.SearchKeyword = "NonExistentControlXYZ123";

            // Assert
            viewModel.SearchResults.Should().BeEmpty();
        }
    }

    public class SelectControl_Command
    {
        [StaFact]
        public void Should_SetSelectedControl_WhenExecuted()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            var control = new ControlInfo
            {
                Name = "TestButton",
                DisplayName = "测试按钮",
                Description = "测试",
                Category = ControlCategory.Buttons,
                Type = ControlType.Custom
            };

            // Act
            viewModel.SelectControlCommand.Execute(control);

            // Assert
            viewModel.SelectedControl.Should().Be(control);
        }

        [StaFact]
        public void Should_SetFirstStyleVariant_WhenControlHasVariants()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
                    new() { StyleName = "Outline", DisplayName = "轮廓按钮", StyleKey = "Win11.Button.Outline" }
                }
            };

            // Act
            viewModel.SelectControlCommand.Execute(control);

            // Assert
            viewModel.SelectedStyleVariant.Should().NotBeNull();
            viewModel.SelectedStyleVariant!.StyleName.Should().Be("Primary");
        }

        [StaFact]
        public void Should_SetNullStyleVariant_WhenControlHasNoVariants()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            var control = new ControlInfo
            {
                Name = "TestControl",
                DisplayName = "测试控件",
                Description = "测试",
                Category = ControlCategory.Buttons,
                Type = ControlType.Custom,
                StyleVariants = null
            };

            // Act
            viewModel.SelectControlCommand.Execute(control);

            // Assert
            viewModel.SelectedStyleVariant.Should().BeNull();
        }
    }

    public class ClearSearch_Command
    {
        [StaFact]
        public void Should_ClearSearchKeyword_WhenExecuted()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.SearchKeyword = "Button";

            // Act
            viewModel.ClearSearchCommand.Execute(null);

            // Assert
            viewModel.SearchKeyword.Should().BeEmpty();
        }

        [StaFact]
        public void Should_SetIsSearchingFalse_WhenExecuted()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.SearchKeyword = "Button";

            // Act
            viewModel.ClearSearchCommand.Execute(null);

            // Assert
            viewModel.IsSearching.Should().BeFalse();
        }

        [StaFact]
        public void Should_ClearSearchResults_WhenExecuted()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.SearchKeyword = "Button";

            // Act
            viewModel.ClearSearchCommand.Execute(null);

            // Assert
            viewModel.SearchResults.Should().BeEmpty();
        }
    }

    public class ExpandAllCategories_Command
    {
        [StaFact]
        public void Should_ExpandAllCategories_WhenExecuted()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.ExpandedCategories.Clear();

            // Act
            viewModel.ExpandAllCategoriesCommand.Execute(null);

            // Assert
            foreach (ControlCategory category in Enum.GetValues(typeof(ControlCategory)))
            {
                var hasControls = Services.ControlRegistry.GetControlsByCategory(category).Count > 0;
                if (hasControls)
                {
                    viewModel.ExpandedCategories.Should().Contain(category);
                }
            }
        }
    }

    public class CollapseAllCategories_Command
    {
        [StaFact]
        public void Should_CollapseAllCategories_WhenExecuted()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            viewModel.CollapseAllCategoriesCommand.Execute(null);

            // Assert
            viewModel.ExpandedCategories.Should().BeEmpty();
        }
    }

    public class ToggleCategory_Command
    {
        [StaFact]
        public void Should_ExpandCategory_WhenCollapsed()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.ExpandedCategories.Clear();

            // Act
            viewModel.ToggleCategoryCommand.Execute(ControlCategory.Buttons);

            // Assert
            viewModel.ExpandedCategories.Should().Contain(ControlCategory.Buttons);
        }

        [StaFact]
        public void Should_CollapseCategory_WhenExpanded()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            viewModel.ToggleCategoryCommand.Execute(ControlCategory.Buttons);

            // Assert
            viewModel.ExpandedCategories.Should().NotContain(ControlCategory.Buttons);
        }
    }

    public class IsCategoryExpanded_Method
    {
        [StaFact]
        public void Should_ReturnTrue_WhenCategoryIsExpanded()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            var result = viewModel.IsCategoryExpanded(ControlCategory.Buttons);

            // Assert
            result.Should().BeTrue();
        }

        [StaFact]
        public void Should_ReturnFalse_WhenCategoryIsCollapsed()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            viewModel.ExpandedCategories.Clear();

            // Act
            var result = viewModel.IsCategoryExpanded(ControlCategory.Buttons);

            // Assert
            result.Should().BeFalse();
        }
    }

    public class GetInsertXaml_Method
    {
        [StaFact]
        public void Should_ReturnEmpty_WhenNoControlSelected()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();

            // Act
            var result = viewModel.GetInsertXaml();

            // Assert
            result.Should().BeEmpty();
        }

        [StaFact]
        public void Should_ReturnSampleCode_WhenNoStyleVariant()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button Content=\"测试\" />"
            };
            viewModel.SelectedControl = control;
            viewModel.SelectedStyleVariant = null;

            // Act
            var result = viewModel.GetInsertXaml();

            // Assert
            result.Should().Be("<Button Content=\"测试\" />");
        }

        [StaFact]
        public void Should_ReturnStyledXaml_WhenStyleVariantSelected()
        {
            // Arrange
            var viewModel = new ToolboxViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />"
            };
            var variant = new ControlStyleVariant
            {
                StyleName = "Primary",
                DisplayName = "主按钮",
                StyleKey = "Win11.Button.Primary"
            };
            viewModel.SelectedControl = control;
            viewModel.SelectedStyleVariant = variant;

            // Act
            var result = viewModel.GetInsertXaml();

            // Assert
            result.Should().Be("<Button Style=\"{StaticResource Win11.Button.Primary}\" Content=\"按钮\" />");
        }
    }
}
