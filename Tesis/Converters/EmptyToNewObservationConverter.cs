using System;
using System.Globalization;
using Xamarin.Forms;

namespace Tesis.Converters
{
    public class EmptyToNewObservationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value?.ToString()) ?
                "Agregar Observación" :
                "Editar Observación";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}