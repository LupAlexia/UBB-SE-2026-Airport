using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace AirportApp.Converters
{
    public sealed class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string path || string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var normalized = path.StartsWith("ms-appx:///", StringComparison.OrdinalIgnoreCase)
                ? path
                : $"ms-appx:///{path.TrimStart('/')}";

            return new BitmapImage(new Uri(normalized));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }
}
