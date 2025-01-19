using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Essentials;

namespace Tesis.ViewModels
{
    public class UserProfileViewModel : INotifyPropertyChanged
    {
        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _apellido;
        public string Apellido
        {
            get => _apellido;
            set
            {
                if (_apellido != value)
                {
                    _apellido = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _correo;
        public string Correo
        {
            get => _correo;
            set
            {
                if (_correo != value)
                {
                    _correo = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _telefono;
        public string Telefono
        {
            get => _telefono;
            set
            {
                if (_telefono != value)
                {
                    _telefono = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _imagenUrl;
        public string ImagenUrl
        {
            get => _imagenUrl;
            set
            {
                if (_imagenUrl != value)
                {
                    _imagenUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadUserData(string userId)
        {
            try
            {
                IsLoading = true;

                var usuario = await Conexionfirebase.firebase
                    .Child("Usuarios")
                    .Child(userId)
                    .OnceSingleAsync<MUsuarios>();

                if (usuario != null)
                {
                    Nombre = usuario.Nombre ?? "No especificado";
                    Apellido = usuario.Apellido ?? "No especificado";
                    Correo = usuario.Correo ?? "No especificado";
                    Telefono = usuario.Telefono ?? "No especificado";
                    ImagenUrl = usuario.Imagen ?? "profile_placeholder.png";
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Usuario no encontrado en Firebase.", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos del usuario: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error al cargar los datos: {ex.Message}", "Aceptar");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public static async Task<string> GetUserIdAsync()
        {
            try
            {
                // Obtener el token guardado
                var savedToken = await SecureStorage.GetAsync("firebase_token");

                if (!string.IsNullOrEmpty(savedToken))
                {
                    // Crear un proveedor de autenticación
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(DBConn.WepApyAuthentication));

                    // Reconstruir el objeto FirebaseAuth desde el token guardado
                    var firebaseAuth = JsonConvert.DeserializeObject<FirebaseAuth>(savedToken);

                    // Refrescar la autenticación con el objeto FirebaseAuth
                    var refreshedAuth = await authProvider.RefreshAuthAsync(firebaseAuth);

                    // Guardar el nuevo token actualizado
                    await SecureStorage.SetAsync("firebase_token", JsonConvert.SerializeObject(refreshedAuth));

                    // Retornar el ID del usuario autenticado
                    return refreshedAuth.User.LocalId;
                }

                throw new Exception("No se encontró un token de autenticación.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el ID del usuario: {ex.Message}");
                throw new Exception("Error al autenticar el usuario.");
            }
        }
    }
}
