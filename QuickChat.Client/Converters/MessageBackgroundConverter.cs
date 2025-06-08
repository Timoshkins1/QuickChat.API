using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickChat.Client.Converters
{
    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMine = (bool)value;
            return isMine ? new SolidColorBrush(Color.FromRgb(255, 140, 0))  // оранжевый
                          : new SolidColorBrush(Color.FromRgb(50, 50, 50));  // тёмный фон
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
