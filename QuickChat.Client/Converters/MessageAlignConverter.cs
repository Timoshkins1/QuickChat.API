using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QuickChat.Client.Converters
{
    public class MessageAlignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMine = (bool)value;
            return isMine ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
