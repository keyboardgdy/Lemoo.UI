using CommunityToolkit.Mvvm.Input;
using FluentAssertions;
using Lemoo.UI.Models;
using Lemoo.UI.WPF.ViewModels.Pages;
using Xunit;

namespace Lemoo.UI.Controls.Tests.ViewModels.Pages;

public class ToolboxSampleViewModelTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_InitializeWithDefaultValues()
        {
            // Act
            var viewModel = new ToolboxSampleViewModel();

            // Assert
            viewModel.Should().NotBeNull();
            viewModel.SelectedControl.Should().BeNull();
            viewModel.ControlDetails.Should().BeNull();
            viewModel.SampleCode.Should().BeEmpty();
            viewModel.SelectedStyleVariant.Should().BeNull();
        }

        [StaFact]
        public void Should_InitializeStyleVariantsCollection()
        {
            // Act
            var viewModel = new ToolboxSampleViewModel();

            // Assert
            viewModel.StyleVariants.Should().NotBeNull();
            viewModel.StyleVariants.Should().BeEmpty();
        }
    }

    public class SelectedControl_Property
    {
        [StaFact]
        public void Should_UpdateControlDetails_WhenSetToNonNull()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button Content=\"点击\" />"
            };

            // Act
            viewModel.SelectedControl = control;

            // Assert
            viewModel.ControlDetails.Should().Be("按钮 (Button)\n基础按钮控件");
        }

        [StaFact]
        public void Should_UpdateSampleCode_WhenSetToNonNull()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button Content=\"点击\" />"
            };

            // Act
            viewModel.SelectedControl = control;

            // Assert
            viewModel.SampleCode.Should().Be("<Button Content=\"点击\" />");
        }

        [StaFact]
        public void Should_PopulateStyleVariants_WhenControlHasVariants()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
                    new() { StyleName = "Outline", DisplayName = "轮廓按钮", StyleKey = "Win11.Button.Outline" }
                }
            };

            // Act
            viewModel.SelectedControl = control;

            // Assert
            viewModel.StyleVariants.Should().HaveCount(2);
            viewModel.StyleVariants[0].StyleName.Should().Be("Primary");
            viewModel.StyleVariants[1].StyleName.Should().Be("Outline");
        }

        [StaFact]
        public void Should_SelectFirstStyleVariant_WhenControlHasVariants()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
                    new() { StyleName = "Outline", DisplayName = "轮廓按钮", StyleKey = "Win11.Button.Outline" }
                }
            };

            // Act
            viewModel.SelectedControl = control;

            // Assert
            viewModel.SelectedStyleVariant.Should().NotBeNull();
            viewModel.SelectedStyleVariant!.StyleName.Should().Be("Primary");
        }

        [StaFact]
        public void Should_ClearStyleVariants_WhenControlHasNoVariants()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var controlWithVariants = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" }
                }
            };
            viewModel.SelectedControl = controlWithVariants;

            var controlWithoutVariants = new ControlInfo
            {
                Name = "TextBox",
                DisplayName = "文本框",
                Description = "文本输入控件",
                Category = ControlCategory.Inputs,
                Type = ControlType.Styled,
                SampleCode = "<TextBox />",
                StyleVariants = null
            };

            // Act
            viewModel.SelectedControl = controlWithoutVariants;

            // Assert
            viewModel.StyleVariants.Should().BeEmpty();
            viewModel.SelectedStyleVariant.Should().BeNull();
        }

        [StaFact]
        public void Should_ClearAllProperties_WhenSetToNull()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" }
                }
            };
            viewModel.SelectedControl = control;

            // Act
            viewModel.SelectedControl = null;

            // Assert
            viewModel.ControlDetails.Should().BeNull();
            viewModel.SampleCode.Should().BeEmpty();
            viewModel.StyleVariants.Should().BeEmpty();
            viewModel.SelectedStyleVariant.Should().BeNull();
        }
    }

    public class SelectedStyleVariant_Property
    {
        [StaFact]
        public void Should_UpdateSampleCodeWithStyle_WhenSelectedAndControlIsSet()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
                    new() { StyleName = "Outline", DisplayName = "轮廓按钮", StyleKey = "Win11.Button.Outline" }
                }
            };
            viewModel.SelectedControl = control;
            var outlineVariant = control.StyleVariants![1];

            // Act
            viewModel.SelectedStyleVariant = outlineVariant;

            // Assert
            viewModel.SampleCode.Should().Be("<Button Style=\"{StaticResource Win11.Button.Outline}\" Content=\"按钮\" />");
        }

        [StaFact]
        public void Should_NotUpdateSampleCode_WhenControlIsNull()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var variant = new ControlStyleVariant
            {
                StyleName = "Primary",
                DisplayName = "主按钮",
                StyleKey = "Win11.Button.Primary"
            };

            // Act
            viewModel.SelectedStyleVariant = variant;

            // Assert
            viewModel.SampleCode.Should().BeEmpty();
        }

        [StaFact]
        public void Should_NotUpdateSampleCode_WhenStyleVariantHasEmptyKey()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button Content=\"默认\" />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Default", DisplayName = "默认", StyleKey = "" }
                }
            };
            viewModel.SelectedControl = control;
            var defaultVariant = control.StyleVariants![0];

            // Act
            viewModel.SelectedStyleVariant = defaultVariant;

            // Assert
            viewModel.SampleCode.Should().Be("<Button Content=\"默认\" />");
        }

        [StaFact]
        public void Should_UseOriginalSampleCode_WhenStyleVariantIsNull()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "TextBox",
                DisplayName = "文本框",
                Description = "文本输入控件",
                Category = ControlCategory.Inputs,
                Type = ControlType.Styled,
                SampleCode = "<TextBox Text=\"示例\" />",
                StyleVariants = null
            };
            viewModel.SelectedControl = control;

            // Act
            viewModel.SelectedStyleVariant = null;

            // Assert
            viewModel.SampleCode.Should().Be("<TextBox Text=\"示例\" />");
        }
    }

    public class CopySampleCode_Command
    {
        [StaFact]
        public void Should_DoNothing_WhenSampleCodeIsEmpty()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();

            // Act & Assert - should not throw
            viewModel.CopySampleCodeCommand.Execute(null);
        }

        [StaFact]
        public void Should_SetClipboardText_WhenSampleCodeIsNotEmpty()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button Content=\"测试\" />"
            };
            viewModel.SelectedControl = control;

            // Act & Assert - should not throw
            viewModel.CopySampleCodeCommand.Execute(null);

            // Verify clipboard was set (this requires STA thread)
            System.Windows.Clipboard.GetText().Should().Be("<Button Content=\"测试\" />");
        }
    }

    public class Dispose_Method
    {
        [StaFact]
        public void Should_ClearStyleVariants_WhenDisposed()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
                    new() { StyleName = "Outline", DisplayName = "轮廓按钮", StyleKey = "Win11.Button.Outline" }
                }
            };
            viewModel.SelectedControl = control;

            // Act
            viewModel.Dispose();

            // Assert
            viewModel.StyleVariants.Should().BeEmpty();
        }

        [StaFact]
        public void Should_AllowMultipleDisposeCalls()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();

            // Act & Assert - should not throw
            viewModel.Dispose();
            viewModel.Dispose();
        }
    }

    public class Integration_Scenarios
    {
        [StaFact]
        public void Should_HandleFullWorkflow_SelectControl_ChangeVariant_CopyCode()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var control = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button Content=\"默认按钮\" />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Default", DisplayName = "默认", StyleKey = "" },
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" },
                    new() { StyleName = "Danger", DisplayName = "危险按钮", StyleKey = "Win11.Button.Danger" }
                }
            };

            // Act 1: Select control
            viewModel.SelectedControl = control;

            // Assert 1
            viewModel.ControlDetails.Should().Be("按钮 (Button)\n基础按钮控件");
            viewModel.SampleCode.Should().Be("<Button Content=\"默认按钮\" />");
            viewModel.SelectedStyleVariant!.StyleName.Should().Be("Default");
            viewModel.StyleVariants.Should().HaveCount(3);

            // Act 2: Change to Primary variant
            viewModel.SelectedStyleVariant = control.StyleVariants![1];

            // Assert 2
            viewModel.SampleCode.Should().Be("<Button Style=\"{StaticResource Win11.Button.Primary}\" Content=\"按钮\" />");

            // Act 3: Change to Danger variant
            viewModel.SelectedStyleVariant = control.StyleVariants![2];

            // Assert 3
            viewModel.SampleCode.Should().Be("<Button Style=\"{StaticResource Win11.Button.Danger}\" Content=\"按钮\" />");

            // Act 4: Clear selection
            viewModel.SelectedControl = null;

            // Assert 4
            viewModel.ControlDetails.Should().BeNull();
            viewModel.SampleCode.Should().BeEmpty();
            viewModel.SelectedStyleVariant.Should().BeNull();
            viewModel.StyleVariants.Should().BeEmpty();
        }

        [StaFact]
        public void Should_HandleControlSwitching_BetweenDifferentControls()
        {
            // Arrange
            var viewModel = new ToolboxSampleViewModel();
            var buttonControl = new ControlInfo
            {
                Name = "Button",
                DisplayName = "按钮",
                Description = "基础按钮控件",
                Category = ControlCategory.Buttons,
                Type = ControlType.Styled,
                SampleCode = "<Button />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Primary", DisplayName = "主按钮", StyleKey = "Win11.Button.Primary" }
                }
            };
            var textBoxControl = new ControlInfo
            {
                Name = "TextBox",
                DisplayName = "文本框",
                Description = "文本输入控件",
                Category = ControlCategory.Inputs,
                Type = ControlType.Styled,
                SampleCode = "<TextBox />",
                StyleVariants = new List<ControlStyleVariant>
                {
                    new() { StyleName = "Default", DisplayName = "默认", StyleKey = "Win11.TextBoxStyle" },
                    new() { StyleName = "Search", DisplayName = "搜索框", StyleKey = "Win11.TextBox.Search" }
                }
            };

            // Act 1: Select Button
            viewModel.SelectedControl = buttonControl;

            // Assert 1
            viewModel.StyleVariants.Should().HaveCount(1);
            viewModel.SelectedStyleVariant!.StyleName.Should().Be("Primary");

            // Act 2: Switch to TextBox
            viewModel.SelectedControl = textBoxControl;

            // Assert 2
            viewModel.StyleVariants.Should().HaveCount(2);
            viewModel.SelectedStyleVariant!.StyleName.Should().Be("Default");
            viewModel.ControlDetails.Should().Be("文本框 (TextBox)\n文本输入控件");

            // Act 3: Switch back to Button
            viewModel.SelectedControl = buttonControl;

            // Assert 3
            viewModel.StyleVariants.Should().HaveCount(1);
            viewModel.SelectedStyleVariant!.StyleName.Should().Be("Primary");
        }
    }
}
