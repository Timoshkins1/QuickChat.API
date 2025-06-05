using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickChat.Client.Converters
{
    public class MessageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMine = value is bool b && b;
            return isMine ? new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC)) : new SolidColorBrush(Color.FromRgb(0x45, 0x45, 0x45));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
