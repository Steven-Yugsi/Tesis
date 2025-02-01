using System;
using System.Globalization;
using Xamarin.Forms;

namespace Tesis.Converters
{
    public class ObservationToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ?
                Color.FromHex("#4CAF50") : // Verde para nuevas
                Color.FromHex("#2196F3");  // Azul para editar
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}