using System.ComponentModel;
using Xamarin.Forms;
using Tesis.Models;
using Tesis.Conexion;
using Firebase.Database.Query;
using System.Threading.Tasks;
using System;
using Tesis.Views;

namespace Tesis.ViewModels
{
    public class RolesViewModel : INotifyPropertyChanged
    {
        private string nombreRol;
        private string descripcionRol;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NombreRol
        {
            get => nombreRol;
            set
            {
                if (nombreRol != value)
                {
                    nombreRol = value;
                    OnPropertyChanged(nameof(NombreRol));
                }
            }
        }

        public string DescripcionRol
        {
            get => descripcionRol;
            set
            {
                if (descripcionRol != value)
                {
                    descripcionRol = value;
                    OnPropertyChanged(nameof(DescripcionRol));
                }
            }
        }
        public Command AgregarRolCommand { get; }
        public Command CrearRolCommand { get; }

        public RolesViewModel()
        {
            AgregarRolCommand = new Command(async () => await NavegarACrearRol());
            CrearRolCommand = new Command(async () => await CrearRol());
        }
        private async Task NavegarACrearRol()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CrearRolesPage());
        }

        private async Task CrearRol()
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(NombreRol) || string.IsNullOrWhiteSpace(DescripcionRol))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Campos Vacíos",
                    "Por favor completa todos los campos antes de continuar.",
                    "OK"
                );
                return;
            }

            var nuevoRol = new Roles
            {
                Nombre = NombreRol,
                Descripcion = DescripcionRol
            };

            try
            {
                // Guardar el rol en Firebase
                var resultado = await Conexionfirebase.firebase
                    .Child("Roles")
                    .PostAsync(nuevoRol);

                // Obtener el ID generado
                string idGenerado = resultado.Key;
                nuevoRol.Rolid = idGenerado;

                // Notificar éxito al usuario
                await Application.Current.MainPage.DisplayAlert(
                    "Éxito",
                    "El rol ha sido creado correctamente.",
                    "OK"
                );

                // Limpiar los campos de entrada
                NombreRol = string.Empty;
                DescripcionRol = string.Empty;
            }
            catch (Exception ex)
            {
                // Mostrar error al usuario
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Ocurrió un error al crear el rol: {ex.Message}",
                    "OK"
                );
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
