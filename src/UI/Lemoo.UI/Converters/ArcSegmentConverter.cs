using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Lemoo.UI.Converters;

/// <summary>
/// Creates a PathGeometry representing an arc segment for circular progress indicators.
/// </summary>
public class ArcSegmentConverter : IMultiValueConverter
{
    public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 5)
            return null;

        if (values[0] is double width && values[1] is double height &&
            values[2] is double value && values[3] is double maximum &&
            values[4] is double thickness)
        {
            if (maximum <= 0)
                value = 0;
            else
                value = Math.Max(0, Math.Min(value, maximum));

            var center = new Point(width / 2, height / 2);
            var radius = Math.Min(width, height) / 2 - thickness / 2;

            if (radius <= 0)
                return null;

            // Calculate start and end angles (starting from top, clockwise)
            const double startAngle = -90; // Start at 12 o'clock position
            var progress = maximum > 0 ? value / maximum : 0;
            var endAngle = startAngle + (360 * progress);

            // Create the path geometry
            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = GetPointOnCircle(center, radius, startAngle) };

            if (progress > 0)
            {
                if (progress >= 1)
                {
                    // Full circle - use ArcSegment + ArcSegment to complete the circle
                    var midPoint = GetPointOnCircle(center, radius, startAngle + 180);
                    figure.Segments.Add(new ArcSegment(
                        midPoint,
                        new Size(radius, radius),
                        180,
                        false,
                        SweepDirection.Clockwise,
                        true));
                    figure.Segments.Add(new ArcSegment(
                        figure.StartPoint,
                        new Size(radius, radius),
                        180,
                        false,
                        SweepDirection.Clockwise,
                        true));
                }
                else
                {
                    var endPoint = GetPointOnCircle(center, radius, endAngle);
                    var arcAngle = endAngle - startAngle;

                    figure.Segments.Add(new ArcSegment(
                        endPoint,
                        new Size(radius, radius),
                        arcAngle,
                        arcAngle > 180,
                        SweepDirection.Clockwise,
                        true));
                }
            }

            geometry.Figures.Add(figure);
            return geometry;
        }

        return null;
    }

    private static Point GetPointOnCircle(Point center, double radius, double angleDegrees)
    {
        var angleRadians = angleDegrees * Math.PI / 180;
        return new Point(
            center.X + radius * Math.Cos(angleRadians),
            center.Y + radius * Math.Sin(angleRadians));
    }

    public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
