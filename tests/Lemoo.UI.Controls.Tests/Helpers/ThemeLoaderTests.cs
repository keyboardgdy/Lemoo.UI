using Xunit;
using Lemoo.UI.Helpers;
using System;
using System.Linq;

namespace Lemoo.UI.Controls.Tests.Helpers
{
    /// <summary>
    /// ThemeLoader 测试
    /// 验证资源加载顺序的正确性
    /// 注意：CreateThemeDictionary 测试需要在 WPF 应用程序上下文中运行，
    /// 单元测试环境中 Pack URI 无法解析，因此跳过这些测试
    /// </summary>
    public class ThemeLoaderTests
    {
        [Fact]
        public void GetThemeResources_BaseTheme_ReturnsCorrectResources()
        {
            // Arrange & Act
            var resources = ThemeLoader.GetThemeResources("Base", isBaseTheme: true);

            // Assert
            Assert.NotNull(resources);
            Assert.Equal("Base", resources.ThemeName);
            Assert.True(resources.Resources.Count > 0);

            // 验证资源层次顺序
            VerifyResourceOrder(resources);
        }

        [Fact]
        public void GetThemeResources_DarkTheme_ReturnsCorrectResources()
        {
            // Arrange & Act
            var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

            // Assert
            Assert.NotNull(resources);
            Assert.Equal("Dark", resources.ThemeName);
            Assert.True(resources.Resources.Count > 0);

            // 验证资源层次顺序
            VerifyResourceOrder(resources);
        }

        [Fact(Skip = "需要在 WPF 应用程序上下文中运行")]
        public void CreateThemeDictionary_BaseTheme_ReturnsValidDictionary()
        {
            // Arrange & Act
            var dict = ThemeLoader.CreateThemeDictionary("Base", isBaseTheme: true);

            // Assert
            Assert.NotNull(dict);
            Assert.NotNull(dict.MergedDictionaries);
            Assert.True(dict.MergedDictionaries.Count > 0);
        }

        [Fact(Skip = "需要在 WPF 应用程序上下文中运行")]
        public void CreateThemeDictionary_DarkTheme_ReturnsValidDictionary()
        {
            // Arrange & Act
            var dict = ThemeLoader.CreateThemeDictionary("Dark", isBaseTheme: false);

            // Assert
            Assert.NotNull(dict);
            Assert.NotNull(dict.MergedDictionaries);
            Assert.True(dict.MergedDictionaries.Count > 0);
        }

        [Fact]
        public void ValidateThemeResources_BaseTheme_ReturnsValid()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Base", isBaseTheme: true);

            // Act
            var (isValid, errors) = ThemeLoader.ValidateThemeResources(resources);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Fact]
        public void ValidateThemeResources_DarkTheme_ReturnsValid()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

            // Act
            var (isValid, errors) = ThemeLoader.ValidateThemeResources(resources);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Fact]
        public void IsThemeAvailable_BaseTheme_ReturnsTrue()
        {
            // Act
            var result = ThemeLoader.IsThemeAvailable("Base");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsThemeAvailable_InvalidTheme_ReturnsFalse()
        {
            // Act
            var result = ThemeLoader.IsThemeAvailable("InvalidTheme");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetResourceLayerDescription_ReturnsDescription()
        {
            // Act
            var description = ThemeLoader.GetResourceLayerDescription();

            // Assert
            Assert.NotNull(description);
            Assert.Contains("设计基线", description);
            Assert.Contains("基础主题", description);
            Assert.Contains("主题覆盖", description);
            Assert.Contains("控件样式", description);
        }

        [Fact]
        public void GetThemeStatistics_BaseTheme_ReturnsStatistics()
        {
            // Act
            var stats = ThemeLoader.GetThemeStatistics("Base", isBaseTheme: true);

            // Assert
            Assert.NotNull(stats);
            Assert.Contains("Base", stats);
            Assert.Contains("资源文件总数", stats);
        }

        [Fact]
        public void ResourceLoadingOrder_MustFollowHierarchy()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

            // Act & Assert
            ThemeLoader.ResourceLayer? previousLayer = null;
            foreach (var resource in resources.Resources)
            {
                if (previousLayer.HasValue)
                {
                    // 验证当前资源层不小于前一个资源层
                    Assert.True(resource.Layer >= previousLayer.Value,
                        $"资源层次顺序错误: {resource.Description} (Layer: {resource.Layer}) " +
                        $"应该在 {previousLayer.Value} 之后");
                }
                previousLayer = resource.Layer;
            }
        }

        [Fact]
        public void ResourceLoadingOrder_DesignBaselineFirst()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

            // Act
            var firstResource = resources.Resources.First();

            // Assert
            Assert.Equal(ThemeLoader.ResourceLayer.DesignBaseline, firstResource.Layer);
        }

        [Fact]
        public void ResourceLoadingOrder_ControlStylesLast()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

            // Act
            var lastResource = resources.Resources.Last();

            // Assert
            Assert.Equal(ThemeLoader.ResourceLayer.ControlStyles, lastResource.Layer);
        }

        [Fact]
        public void ResourceLoadingOrder_BaseThemeBeforeOverride()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

            // Act
            var baseThemeIndex = resources.Resources.FindIndex(r => r.Layer == ThemeLoader.ResourceLayer.BaseTheme);
            var overrideIndex = resources.Resources.FindIndex(r => r.Layer == ThemeLoader.ResourceLayer.ThemeOverride);

            // Assert
            Assert.True(baseThemeIndex >= 0, "应该有 BaseTheme 资源");
            Assert.True(overrideIndex >= 0, "应该有 ThemeOverride 资源");
            Assert.True(baseThemeIndex < overrideIndex, "BaseTheme 应该在 ThemeOverride 之前加载");
        }

        [Fact]
        public void ResourceLayersCount_BaseTheme_HasCorrectLayers()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Base", isBaseTheme: true);

            // Act
            var designBaselineCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.DesignBaseline);
            var baseThemeCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.BaseTheme);
            var controlStylesCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.ControlStyles);

            // Assert
            Assert.Equal(4, designBaselineCount); // Typography, Spacing, Shadows, Animations
            Assert.Equal(3, baseThemeCount); // ColorPalette, SemanticTokens, ComponentBrushes
            Assert.Equal(1, controlStylesCount); // Win11.Controls
        }

        [Fact]
        public void ResourceLayersCount_DarkTheme_HasCorrectLayers()
        {
            // Arrange
            var resources = ThemeLoader.GetThemeResources("Dark", isBaseTheme: false);

            // Act
            var designBaselineCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.DesignBaseline);
            var baseThemeCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.BaseTheme);
            var themeOverrideCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.ThemeOverride);
            var controlStylesCount = resources.Resources.Count(r => r.Layer == ThemeLoader.ResourceLayer.ControlStyles);

            // Assert
            Assert.Equal(4, designBaselineCount); // Typography, Spacing, Shadows, Animations
            Assert.Equal(3, baseThemeCount); // ColorPalette, SemanticTokens, ComponentBrushes (Base)
            Assert.Equal(3, themeOverrideCount); // ColorPalette, SemanticTokens, ComponentBrushes (Dark)
            Assert.Equal(1, controlStylesCount); // Win11.Controls
        }

        /// <summary>
        /// 验证资源顺序的辅助方法
        /// </summary>
        private void VerifyResourceOrder(ThemeLoader.ThemeResources resources)
        {
            // 验证四个资源层都存在
            var hasDesignBaseline = resources.Resources.Any(r => r.Layer == ThemeLoader.ResourceLayer.DesignBaseline);
            var hasBaseTheme = resources.Resources.Any(r => r.Layer == ThemeLoader.ResourceLayer.BaseTheme);
            var hasControlStyles = resources.Resources.Any(r => r.Layer == ThemeLoader.ResourceLayer.ControlStyles);

            Assert.True(hasDesignBaseline, "应该有 DesignBaseline 资源");
            Assert.True(hasBaseTheme, "应该有 BaseTheme 资源");
            Assert.True(hasControlStyles, "应该有 ControlStyles 资源");

            // 验证顺序
            ThemeLoader.ResourceLayer? previousLayer = null;
            foreach (var resource in resources.Resources)
            {
                if (previousLayer.HasValue)
                {
                    Assert.True(resource.Layer >= previousLayer.Value,
                        $"资源层次顺序错误: {resource.Description}");
                }
                previousLayer = resource.Layer;
            }
        }
    }
}
