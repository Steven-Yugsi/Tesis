using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Tesis.Converters
{
    public class MessageAuthorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Verifica si el valor es nulo o no es una cadena
            var role = value as string;
            if (role == null) // Verificación compatible con C# 7.3
                return Color.Black;

            return role == "Usuario" ? Color.White : Color.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
