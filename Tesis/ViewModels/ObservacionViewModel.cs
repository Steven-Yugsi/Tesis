using System.Threading.Tasks;
using System;
using System.Windows.Input;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Forms;
using Newtonsoft.Json;
using Firebase.Database.Query;
using System.Linq;

namespace Tesis.ViewModels
{
    public class ObservacionViewModel : BaseViewModel
    {
        private readonly Alerta _alerta;
        private readonly INavigation _navigation;

        public string Observacion
        {
            get => _alerta.Observacion;
            set
            {
                _alerta.Observacion = value;
                OnPropertyChanged();
            }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public ObservacionViewModel(Alerta alerta , INavigation navigation)
        {
            _alerta = alerta;
            _navigation = navigation;

            GuardarCommand = new Command(async () => await GuardarObservacion());
            CancelarCommand = new Command(async () => await Cancelar());
            
        }

        private async Task GuardarObservacion()
        {
            try
            {
                Console.WriteLine($"ID de la alerta recibido: {_alerta.Id}"); // 👈 Verifica esto
                if (string.IsNullOrWhiteSpace(_alerta.Id))
                    throw new Exception("No se encontró el ID de la alerta");

                var alertaId = _alerta.Id; // ID de la alerta, no del usuario

                await Conexionfirebase.firebase
                    .Child("Alertas")
                    .Child(_alerta.Estudiante)
                    .Child(_alerta.Id)
                    .PatchAsync(new
                    {
                        observacion = Observacion?.Trim() ?? "",                        
                    });

                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error guardando observación: {ex.Message}", "OK");
                Console.WriteLine($"ERROR: {ex}");
            }
        }

        private async Task Cancelar()
        {
            await _navigation.PopAsync();
        }
    }
}