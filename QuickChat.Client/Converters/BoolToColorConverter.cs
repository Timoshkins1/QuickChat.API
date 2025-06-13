using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickChat.Client.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value
                ? new SolidColorBrush(Color.FromRgb(0, 200, 0)) // зелёный
                : new SolidColorBrush(Color.FromRgb(120, 120, 120)); // серый
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
