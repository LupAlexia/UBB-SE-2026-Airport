using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace AirportApp.Converters
{
    public class BoolToActiveBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isActive && isActive
                ? new SolidColorBrush(Color.FromArgb(255, 43, 184, 192))
                : new SolidColorBrush(Microsoft.UI.Colors.Transparent);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class BoolToInactiveBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isActive && !isActive
                ? new SolidColorBrush(Color.FromArgb(255, 43, 184, 192))
                : new SolidColorBrush(Microsoft.UI.Colors.Transparent);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class BoolToActiveBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isActive && !isActive ? new SolidColorBrush(Color.FromArgb(255, 63, 63, 70)) : null;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    public class BoolToActiveBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool isActive && !isActive ? new Thickness(1) : new Thickness(0);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
