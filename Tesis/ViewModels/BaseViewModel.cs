using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Evento para notificar cambios de propiedades
        public event PropertyChangedEventHandler PropertyChanged;

        // INavigation para navegación en ViewModels
        public INavigation Navigation { get; set; }

        // Método para notificar cambios en una propiedad
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método genérico para establecer un valor y notificar el cambio
        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
