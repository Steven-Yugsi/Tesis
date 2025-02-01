using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using Tesis.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Tesis.Conexion;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Windows.Input;
using Tesis.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tesis.ViewModels
{
    public class PsychologistViewModel : BaseViewModel
    {
        public ObservableCollection<Alerta> Alertas { get; set; }
        public ICommand NavegarAObservacionCommand { get; }

        private bool _hayAlertas;
        public bool HayAlertas
        {
            get => _hayAlertas;
            set => SetValue(ref _hayAlertas, value);
        }

        public PsychologistViewModel()
        {
            Alertas = new ObservableCollection<Alerta>();
            _ = CargarAlertas();
            NavegarAObservacionCommand = new Command<Alerta>(async (alerta) => await NavegarAObservacion(alerta));

            Alertas.CollectionChanged += (s, e) => HayAlertas = Alertas.Any();
            Task.Run(async () => await CargarAlertas()).ContinueWith(t =>
            {
                if (t.Exception != null)
                    Console.WriteLine($"Error: {t.Exception}");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task NavegarAObservacion(Alerta alerta)
        {
            try
            {
                if (alerta == null) throw new ArgumentNullException(nameof(alerta), "Alerta no válida.");

                var observacionPage = new ObservacionPage(alerta);
                if (Application.Current.MainPage is MainFlyoutPage flyoutPage &&
                    flyoutPage.Detail is NavigationPage navPage)
                {
                    await navPage.Navigation.PushAsync(observacionPage);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"No se pudo abrir la vista: {ex.Message}", "Aceptar");
            }
        }

        private async Task CargarAlertas()
        {
            try
            {
                var listaTemporal = new List<Alerta>();

                var usuarios = await Conexionfirebase.firebase
                    .Child("Alertas")
                    .OnceAsync<Dictionary<string, object>>();

                foreach (var usuario in usuarios)
                {
                    var alertas = await Conexionfirebase.firebase
                        .Child("Alertas")
                        .Child(usuario.Key)
                        .OnceAsync<Alerta>();

                    foreach (var alertaFirebase in alertas)
                    {
                        var alerta = alertaFirebase.Object;
                        alerta.Id = alertaFirebase.Key;
                        alerta.Estudiante = usuario.Key;
                        alerta.PropertyChanged += Alerta_PropertyChanged;
                        listaTemporal.Add(alerta);
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Alertas.Clear();
                    foreach (var alerta in listaTemporal.OrderByDescending(a => a.Fecha))
                    {
                        Alertas.Add(alerta);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Error al cargar alertas", "OK");
            }
        }

        private async void Alerta_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Alerta alerta)
            {
                await ActualizarAlertaEnFirebase(alerta);
            }
        }

        private async Task ActualizarAlertaEnFirebase(Alerta alerta)
        {
            try
            {
                var alertaRef = Conexionfirebase.firebase
                    .Child("Alertas")
                    .Child(alerta.Estudiante)
                    .Child(alerta.Id);

                await alertaRef.PutAsync(new
                {
                    alerta.Asignado,
                    alerta.Leida,
                    alerta.Observacion,
                    fecha = alerta.Fecha.ToString("o"),
                    alerta.Mensaje,
                    alerta.Apellido,
                    alerta.Telefono
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error al actualizar: {ex.Message}", "Aceptar");
            }
        }
    }
}