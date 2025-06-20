﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickChat.Client.Converters
{
    public class MessageColorConverter : IValueConverter
    {
        private static readonly Brush MineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8C00"));
        private static readonly Brush OthersBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMine = (bool)value;
            return isMine ? MineBrush : OthersBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
