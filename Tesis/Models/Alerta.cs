using Newtonsoft.Json;
using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tesis.Models
{
    public class Alerta : INotifyPropertyChanged
    {
        private bool _asignado;
        private bool _leida;
        private string _observacion = "";
        private string _fecha;

        [JsonIgnore]
        public string Id { get; set; }

        [JsonProperty("apellido")]
        public string Apellido { get; set; }

        [JsonProperty("fecha")]
        public string FechaRaw
        {
            get => _fecha;
            set => _fecha = value;
        }

        public DateTime Fecha => DateTime.Parse(_fecha);

        [JsonProperty("mensaje")]
        public string Mensaje { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        public string Nombre
        {
            get
            {
                if (string.IsNullOrEmpty(Mensaje) || !Mensaje.Contains("usuario "))
                    return "Nombre no disponible";

                var partes = Mensaje.Split(new[] { "usuario " }, StringSplitOptions.None);
                return partes.Length > 1
                    ? partes[1].Split(' ')[0].Trim()
                    : "Formato inválido";
            }
        }

        public string UsuarioNombre => $"{Nombre} {Apellido}";

        [JsonProperty("observacion")]
        public string Observacion
        {
            get => _observacion;
            set
            {
                _observacion = value;
                OnPropertyChanged(nameof(Observacion));
            }
        }

        [JsonProperty("asignado")]
        public bool Asignado
        {
            get => _asignado;
            set
            {
                if (_asignado || value == false) return; // Solo permite cambiar a true

                _asignado = true;
                OnPropertyChanged(nameof(Asignado));
                OnPropertyChanged(nameof(Leida));

            }
        }

        [JsonProperty("leida")]
        public bool Leida
        {
            get => _leida;
            set
            {
                if (_leida || value == false) return; // Solo permite cambiar a true

                if (!Asignado)
                {
                    MostrarErrorAsignacion();
                    return;
                }

                _leida = true;
                OnPropertyChanged(nameof(Leida));
                OnPropertyChanged(nameof(Asignado));
            }
        }

        public string Estudiante { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Alerta()
        {
            Asignado = false;
            Leida = false;
        }
        private async void MostrarErrorAsignacion()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Debe asignar la alerta antes de finalizarla",
                    "Aceptar");
            });
        }
    }
}