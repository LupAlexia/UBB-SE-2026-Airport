using System;
using Microsoft.UI.Xaml.Data;

namespace AirportApp.Converters
{
    public sealed class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool flag ? !flag : value;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => value is bool flag ? !flag : value;
    }
}
