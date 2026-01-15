using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Dialogs;

public class DialogHostTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.Should().NotBeNull();
            dialogHost.IsOpen.Should().BeFalse();
            dialogHost.DialogContent.Should().BeNull();
            dialogHost.DialogContentTemplate.Should().BeNull();
            dialogHost.ShowOverlay.Should().BeTrue();
            dialogHost.OverlayOpacity.Should().Be(0.5);
            dialogHost.CloseOnClickOutside.Should().BeFalse();
            dialogHost.DialogHorizontalAlignment.Should().Be(HorizontalAlignment.Center);
            dialogHost.DialogVerticalAlignment.Should().Be(VerticalAlignment.Center);
            dialogHost.OpenAnimation.Should().Be(DialogAnimation.FadeIn);
            dialogHost.CloseAnimation.Should().Be(DialogAnimation.FadeOut);
        }
    }

    public class IsOpen_Property
    {
        [StaFact]
        public void Should_SetAndGetIsOpen()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.IsOpen = true;

            // Assert
            dialogHost.IsOpen.Should().BeTrue();
        }

        [StaFact]
        public void Should_RaiseDialogOpenedEvent_WhenOpened()
        {
            // Arrange
            var dialogHost = new DialogHost();
            DialogOpenedEventArgs? args = null;
            dialogHost.DialogOpened += (s, e) => args = e;

            // Act
            dialogHost.IsOpen = true;

            // Assert
            args.Should().NotBeNull();
        }

        [StaFact]
        public void Should_RaiseDialogClosedEvent_WhenClosed()
        {
            // Arrange
            var dialogHost = new DialogHost();
            DialogClosedEventArgs? args = null;
            dialogHost.DialogClosed += (s, e) => args = e;

            // Act
            dialogHost.IsOpen = true;
            dialogHost.IsOpen = false;

            // Assert
            args.Should().NotBeNull();
        }

        [StaFact]
        public void Should_NotRaiseOpenedEvent_WhenAlreadyOpen()
        {
            // Arrange
            var dialogHost = new DialogHost { IsOpen = true };
            int eventCount = 0;
            dialogHost.DialogOpened += (s, e) => eventCount++;

            // Act
            dialogHost.IsOpen = true;

            // Assert
            eventCount.Should().Be(0);
        }

        [StaFact]
        public void Should_NotRaiseClosedEvent_WhenAlreadyClosed()
        {
            // Arrange
            var dialogHost = new DialogHost();
            int eventCount = 0;
            dialogHost.DialogClosed += (s, e) => eventCount++;

            // Act
            dialogHost.IsOpen = false;

            // Assert
            eventCount.Should().Be(0);
        }
    }

    public class DialogContent_Property
    {
        [StaFact]
        public void Should_SetAndGetDialogContent()
        {
            // Arrange
            var dialogHost = new DialogHost();
            var content = new TextBlock { Text = "Dialog Content" };

            // Act
            dialogHost.DialogContent = content;

            // Assert
            dialogHost.DialogContent.Should().Be(content);
        }

        [StaFact]
        public void Should_AcceptNullDialogContent()
        {
            // Arrange
            var dialogHost = new DialogHost { DialogContent = new TextBlock() };

            // Act
            dialogHost.DialogContent = null;

            // Assert
            dialogHost.DialogContent.Should().BeNull();
        }

        [StaFact]
        public void Should_AcceptStringAsDialogContent()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.DialogContent = "String Content";

            // Assert
            dialogHost.DialogContent.Should().Be("String Content");
        }

        [StaFact]
        public void Should_AcceptUIElementAsDialogContent()
        {
            // Arrange
            var dialogHost = new DialogHost();
            var content = new StackPanel();

            // Act
            dialogHost.DialogContent = content;

            // Assert
            dialogHost.DialogContent.Should().Be(content);
        }
    }

    public class DialogContentTemplate_Property
    {
        [StaFact]
        public void Should_SetAndGetDialogContentTemplate()
        {
            // Arrange
            var dialogHost = new DialogHost();
            var template = new DataTemplate();

            // Act
            dialogHost.DialogContentTemplate = template;

            // Assert
            dialogHost.DialogContentTemplate.Should().Be(template);
        }

        [StaFact]
        public void Should_AcceptNullDialogContentTemplate()
        {
            // Arrange
            var dialogHost = new DialogHost { DialogContentTemplate = new DataTemplate() };

            // Act
            dialogHost.DialogContentTemplate = null;

            // Assert
            dialogHost.DialogContentTemplate.Should().BeNull();
        }
    }

    public class ShowOverlay_Property
    {
        [StaFact]
        public void Should_SetAndGetShowOverlay()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.ShowOverlay = false;

            // Assert
            dialogHost.ShowOverlay.Should().BeFalse();
        }

        [StaFact]
        public void Should_HaveDefaultShowOverlayTrue()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.ShowOverlay.Should().BeTrue();
        }
    }

    public class OverlayBackground_Property
    {
        [StaFact]
        public void Should_SetAndGetOverlayBackground()
        {
            // Arrange
            var dialogHost = new DialogHost();
            var brush = new SolidColorBrush(Colors.Red);

            // Act
            dialogHost.OverlayBackground = brush;

            // Assert
            dialogHost.OverlayBackground.Should().Be(brush);
        }

        [StaFact]
        public void Should_HaveDefaultOverlayBackgroundBlack()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.OverlayBackground.Should().Be(Brushes.Black);
        }
    }

    public class OverlayOpacity_Property
    {
        [StaFact]
        public void Should_SetAndGetOverlayOpacity()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.OverlayOpacity = 0.7;

            // Assert
            dialogHost.OverlayOpacity.Should().Be(0.7);
        }

        [StaFact]
        public void Should_HaveDefaultOverlayOpacity05()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.OverlayOpacity.Should().Be(0.5);
        }

        [StaTheory]
        [InlineData(0.0)]
        [InlineData(0.25)]
        [InlineData(0.5)]
        [InlineData(0.75)]
        [InlineData(1.0)]
        public void Should_AcceptValidOpacityValues(double opacity)
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.OverlayOpacity = opacity;

            // Assert
            dialogHost.OverlayOpacity.Should().Be(opacity);
        }
    }

    public class CloseOnClickOutside_Property
    {
        [StaFact]
        public void Should_SetAndGetCloseOnClickOutside()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.CloseOnClickOutside = true;

            // Assert
            dialogHost.CloseOnClickOutside.Should().BeTrue();
        }

        [StaFact]
        public void Should_HaveDefaultCloseOnClickOutsideFalse()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.CloseOnClickOutside.Should().BeFalse();
        }
    }

    public class DialogHorizontalAlignment_Property
    {
        [StaFact]
        public void Should_SetAndGetDialogHorizontalAlignment()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.DialogHorizontalAlignment = HorizontalAlignment.Left;

            // Assert
            dialogHost.DialogHorizontalAlignment.Should().Be(HorizontalAlignment.Left);
        }

        [StaFact]
        public void Should_HaveDefaultHorizontalAlignmentCenter()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.DialogHorizontalAlignment.Should().Be(HorizontalAlignment.Center);
        }

        [StaTheory]
        [InlineData(HorizontalAlignment.Left)]
        [InlineData(HorizontalAlignment.Center)]
        [InlineData(HorizontalAlignment.Right)]
        [InlineData(HorizontalAlignment.Stretch)]
        public void Should_AcceptAllHorizontalAlignments(HorizontalAlignment alignment)
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.DialogHorizontalAlignment = alignment;

            // Assert
            dialogHost.DialogHorizontalAlignment.Should().Be(alignment);
        }
    }

    public class DialogVerticalAlignment_Property
    {
        [StaFact]
        public void Should_SetAndGetDialogVerticalAlignment()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.DialogVerticalAlignment = VerticalAlignment.Top;

            // Assert
            dialogHost.DialogVerticalAlignment.Should().Be(VerticalAlignment.Top);
        }

        [StaFact]
        public void Should_HaveDefaultVerticalAlignmentCenter()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.DialogVerticalAlignment.Should().Be(VerticalAlignment.Center);
        }

        [StaTheory]
        [InlineData(VerticalAlignment.Top)]
        [InlineData(VerticalAlignment.Center)]
        [InlineData(VerticalAlignment.Bottom)]
        [InlineData(VerticalAlignment.Stretch)]
        public void Should_AcceptAllVerticalAlignments(VerticalAlignment alignment)
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.DialogVerticalAlignment = alignment;

            // Assert
            dialogHost.DialogVerticalAlignment.Should().Be(alignment);
        }
    }

    public class OpenAnimation_Property
    {
        [StaFact]
        public void Should_SetAndGetOpenAnimation()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.OpenAnimation = DialogAnimation.Zoom;

            // Assert
            dialogHost.OpenAnimation.Should().Be(DialogAnimation.Zoom);
        }

        [StaFact]
        public void Should_HaveDefaultOpenAnimationFadeIn()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.OpenAnimation.Should().Be(DialogAnimation.FadeIn);
        }

        [StaTheory]
        [InlineData(DialogAnimation.None)]
        [InlineData(DialogAnimation.FadeIn)]
        [InlineData(DialogAnimation.SlideFromTop)]
        [InlineData(DialogAnimation.SlideFromBottom)]
        [InlineData(DialogAnimation.SlideFromLeft)]
        [InlineData(DialogAnimation.SlideFromRight)]
        [InlineData(DialogAnimation.Zoom)]
        [InlineData(DialogAnimation.ZoomFade)]
        public void Should_AcceptAllAnimations(DialogAnimation animation)
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.OpenAnimation = animation;

            // Assert
            dialogHost.OpenAnimation.Should().Be(animation);
        }
    }

    public class CloseAnimation_Property
    {
        [StaFact]
        public void Should_SetAndGetCloseAnimation()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.CloseAnimation = DialogAnimation.Zoom;

            // Assert
            dialogHost.CloseAnimation.Should().Be(DialogAnimation.Zoom);
        }

        [StaFact]
        public void Should_HaveDefaultCloseAnimationFadeOut()
        {
            // Act
            var dialogHost = new DialogHost();

            // Assert
            dialogHost.CloseAnimation.Should().Be(DialogAnimation.FadeOut);
        }
    }

    public class ShowDialog_Method
    {
        [StaFact]
        public void Should_OpenDialog_WhenShowDialogCalled()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.ShowDialog();

            // Assert
            dialogHost.IsOpen.Should().BeTrue();
        }
    }

    public class CloseDialog_Method
    {
        [StaFact]
        public void Should_CloseDialog_WhenCloseDialogCalled()
        {
            // Arrange
            var dialogHost = new DialogHost { IsOpen = true };

            // Act
            dialogHost.CloseDialog();

            // Assert
            dialogHost.IsOpen.Should().BeFalse();
        }
    }

    public class Events
    {
        [StaFact]
        public void Should_SubscribeToDialogOpenedEvent()
        {
            // Arrange
            var dialogHost = new DialogHost();
            int eventCount = 0;
            dialogHost.DialogOpened += (s, e) => eventCount++;

            // Act
            dialogHost.IsOpen = true;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_SubscribeToDialogClosedEvent()
        {
            // Arrange
            var dialogHost = new DialogHost();
            int eventCount = 0;
            dialogHost.DialogClosed += (s, e) => eventCount++;

            // Act
            dialogHost.IsOpen = true;
            dialogHost.IsOpen = false;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var dialogHost = new DialogHost();
            int eventCount1 = 0;
            int eventCount2 = 0;
            dialogHost.DialogOpened += (s, e) => eventCount1++;
            dialogHost.DialogOpened += (s, e) => eventCount2++;

            // Act
            dialogHost.IsOpen = true;

            // Assert
            eventCount1.Should().Be(1);
            eventCount2.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowUnsubscribingFromEvents()
        {
            // Arrange
            var dialogHost = new DialogHost();
            int eventCount = 0;
            EventHandler<DialogOpenedEventArgs> handler = (s, e) => eventCount++;
            dialogHost.DialogOpened += handler;
            dialogHost.DialogOpened -= handler;

            // Act
            dialogHost.IsOpen = true;

            // Assert
            eventCount.Should().Be(0);
        }
    }

    public class Content_Property
    {
        [StaFact]
        public void Should_SetAndGetStringContent()
        {
            // Arrange
            var dialogHost = new DialogHost();

            // Act
            dialogHost.Content = "Main content";

            // Assert
            dialogHost.Content.Should().Be("Main content");
        }

        [StaFact]
        public void Should_AcceptUIElementAsContent()
        {
            // Arrange
            var dialogHost = new DialogHost();
            var content = new Grid();

            // Act
            dialogHost.Content = content;

            // Assert
            dialogHost.Content.Should().Be(content);
        }

        [StaFact]
        public void Should_AcceptNullContent()
        {
            // Arrange
            var dialogHost = new DialogHost { Content = "Test" };

            // Act
            dialogHost.Content = null;

            // Assert
            dialogHost.Content.Should().BeNull();
        }
    }
}
