using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Firebase.Database.Query;
using Tesis.Models;
using System;
using Tesis.Conexion;
using Xamarin.Forms;
using Xamarin.Essentials;
using Newtonsoft.Json.Linq;

namespace Tesis.ViewModels
{
    public class UserListViewModel : BaseViewModel
    {
        // Observable collection de usuarios
        public ObservableCollection<UsuarioWrapper> Usuarios { get; set; } = new ObservableCollection<UsuarioWrapper>();

        // Observable collection de tipos de perfil
        private ObservableCollection<string> tiposDePerfil = new ObservableCollection<string>();
        public ObservableCollection<string> TiposDePerfil
        {
            get => tiposDePerfil;
            set => SetValue(ref tiposDePerfil, value);
        }

        // Comando para cargar usuarios
        public Command CargarUsuariosCommand { get; set; }

        public UserListViewModel()
        {
            // Cargar usuarios al iniciar
            CargarUsuariosCommand = new Command(async () => await CargarUsuariosAsync());

            // Cargar tipos de perfil
            MainThread.InvokeOnMainThreadAsync(async () => await CargarTiposDePerfilAsync());
        }

        // Cargar usuarios desde Firebase
        public async Task CargarUsuariosAsync()
        {
            try
            {
                var usuariosJson = await Conexionfirebase.firebase
                    .Child("Usuarios")
                    .OnceAsync<object>();

                Usuarios.Clear();

                foreach (var usuario in usuariosJson)
                {
                    var usuarioData = usuario.Object as JObject;

                    if (usuarioData != null)
                    {
                        var nuevoUsuario = usuarioData.ToObject<MUsuarios>();

                        // Verifica que el usuario tenga los datos necesarios
                        if (!string.IsNullOrEmpty(nuevoUsuario.Nombre) &&
                            !string.IsNullOrEmpty(nuevoUsuario.Apellido) &&
                            !string.IsNullOrEmpty(nuevoUsuario.TipoPerfil))
                        {
                            Usuarios.Add(new UsuarioWrapper(nuevoUsuario, this));
                        }
                    }
                }

                Console.WriteLine($"Total de usuarios agregados: {Usuarios.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los usuarios: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar los usuarios desde el servidor.", "Aceptar");
            }
        }

        // Cargar tipos de perfil (roles) desde Firebase
        public async Task CargarTiposDePerfilAsync()
        {
            try
            {
                var roles = await Conexionfirebase.firebase
                    .Child("Roles")
                    .OnceAsync<dynamic>();

                // Actualizar la lista de tipos de perfil en el hilo principal
                Device.BeginInvokeOnMainThread(() =>
                {
                    TiposDePerfil.Clear();
                    foreach (var role in roles)
                    {
                        if (role.Object?.Nombre != null)
                        {
                            TiposDePerfil.Add(role.Object.Nombre.ToString());
                        }
                    }
                });

                Console.WriteLine($"Roles cargados: {string.Join(", ", TiposDePerfil)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los roles desde Firebase: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar los roles desde el servidor.", "Aceptar");
            }
        }

        // Método para actualizar el tipo de perfil de un usuario
        public async Task ActualizarTipoPerfilAsync(MUsuarios usuario, string nuevoTipoPerfil)
        {
            if (usuario == null || string.IsNullOrEmpty(nuevoTipoPerfil))
                return;

            try
            {
                // Actualizar en Firebase
                await Conexionfirebase.firebase
                    .Child("Usuarios")
                    .Child(usuario.Id_User)
                    .PatchAsync(new { TipoPerfil = nuevoTipoPerfil });

                // Actualizar localmente
                usuario.TipoPerfil = nuevoTipoPerfil;
                Console.WriteLine($"Tipo de perfil actualizado para {usuario.NombreCompleto} a {nuevoTipoPerfil}");

                await App.Current.MainPage.DisplayAlert("Éxito", "El tipo de perfil se actualizó correctamente.", "Aceptar");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el tipo de perfil: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el tipo de perfil.", "Aceptar");
            }
        }
        public class UsuarioWrapper : BaseViewModel
        {
            private MUsuarios _usuario;
            private UserListViewModel _viewModel;

            public UsuarioWrapper(MUsuarios usuario, UserListViewModel viewModel)
            {
                _usuario = usuario;
                _viewModel = viewModel;
            }

            public string TipoPerfil
            {
                get => _usuario.TipoPerfil;
                set
                {
                    if (_usuario.TipoPerfil != value)
                    {
                        _usuario.TipoPerfil = value;
                        OnPropertyChanged();
                        _ = _viewModel.ActualizarTipoPerfilAsync(_usuario, value); // Actualiza en Firebase
                    }
                }
            }

            // Propiedades adicionales para binding (NombreCompleto, Correo, etc.)
            public string NombreCompleto => _usuario.NombreCompleto;
            public string Correo => _usuario.Correo;
        }
    }
}
