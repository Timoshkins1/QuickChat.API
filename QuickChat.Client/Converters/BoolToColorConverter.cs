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
                ? new SolidColorBrush(Color.FromRgb(173, 216, 230)) // голубой
                : new SolidColorBrush(Color.FromRgb(230, 230, 230)); // серый
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
