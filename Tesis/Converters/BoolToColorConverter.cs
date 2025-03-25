using System;
using System.Globalization;
using Xamarin.Forms;

namespace Tesis.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isNotBusy && parameter is string colors)
            {
                string[] colorArray = colors.Split(';');
                if (colorArray.Length == 2)
                {
                    return isNotBusy ? Color.FromHex(colorArray[0]) : Color.FromHex(colorArray[1]);
                }
            }
            return Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
