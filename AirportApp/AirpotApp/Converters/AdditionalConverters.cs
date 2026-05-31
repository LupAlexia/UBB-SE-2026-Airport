using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace AirportApp.Converters
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isVisible && isVisible ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isVisible && !isVisible ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class DateTimeToLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                DateTimeOffset offset => offset.ToLocalTime().ToString("g"),
                DateTime dateTime => dateTime.ToLocalTime().ToString("g"),
                _ => value?.ToString() ?? string.Empty
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class BooleanToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isOutgoing && isOutgoing ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class BooleanToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isExpanded && isExpanded ? "\uE70E" : "\uE70D";

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class HelpfulBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => ConverterResources.GetBrush(value is bool isSelected && isSelected ? "SuccessBrush" : "Transparent");

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class HelpfulForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isSelected && isSelected
                ? new SolidColorBrush(Microsoft.UI.Colors.White)
                : ConverterResources.GetBrush("PrimaryTextBrush");

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class HelpfulBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => ConverterResources.GetBrush(value is bool isSelected && isSelected ? "SuccessBrush" : "BorderBrush");

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class NotHelpfulBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => ConverterResources.GetBrush(value is bool isSelected && isSelected ? "ErrorBrush" : "Transparent");

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class NotHelpfulForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isSelected && isSelected
                ? new SolidColorBrush(Microsoft.UI.Colors.White)
                : ConverterResources.GetBrush("PrimaryTextBrush");

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    public sealed class NotHelpfulBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => ConverterResources.GetBrush(value is bool isSelected && isSelected ? "ErrorBrush" : "BorderBrush");

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }

    internal static class ConverterResources
    {
        public static Brush GetBrush(string key)
        {
            if (key == "Transparent")
            {
                return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
            }

            if (Application.Current?.Resources.TryGetValue(key, out var resource) == true && resource is Brush brush)
            {
                return brush;
            }

            return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }
    }
}
