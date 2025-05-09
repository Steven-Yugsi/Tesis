﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace Tesis.Converters
{
    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string role)
            {
                return role == "user" ? LayoutOptions.EndAndExpand : LayoutOptions.StartAndExpand;
            }
            return LayoutOptions.StartAndExpand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
