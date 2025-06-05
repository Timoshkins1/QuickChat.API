using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace QuickChat.Client.Converters
{
    public class MessageAlignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMine = value is bool b && b;
            return isMine ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
