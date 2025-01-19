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
        public ObservableCollection<MUsuarios> Usuarios { get; set; } = new ObservableCollection<MUsuarios>();

        private ObservableCollection<string> tiposDePerfil = new ObservableCollection<string>();
        public ObservableCollection<string> TiposDePerfil
        {
            get => tiposDePerfil;
            set => SetValue(ref tiposDePerfil, value);
        }

        public Command CargarUsuariosCommand { get; set; }

        public UserListViewModel()
        {
            CargarUsuariosCommand = new Command(async () => await CargarUsuariosAsync());
            MainThread.InvokeOnMainThreadAsync(async () => await CargarTiposDePerfilAsync());
        }

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

                        if (!string.IsNullOrEmpty(nuevoUsuario.Nombre) &&
                            !string.IsNullOrEmpty(nuevoUsuario.Apellido) &&
                            !string.IsNullOrEmpty(nuevoUsuario.TipoPerfil))
                        {
                            Usuarios.Add(nuevoUsuario);
                        }
                    }
                }

                Console.WriteLine($"Total de usuarios agregados: {Usuarios.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los usuarios: {ex.Message}");
            }
        }

        public async Task CargarTiposDePerfilAsync()
        {
            try
            {
                var roles = await Conexionfirebase.firebase
                    .Child("Roles")
                    .OnceAsync<dynamic>();

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

        public async Task ActualizarTipoPerfilAsync(MUsuarios usuario, string nuevoTipoPerfil)
        {
            if (usuario == null || string.IsNullOrEmpty(nuevoTipoPerfil))
                return;

            try
            {
                // Actualiza en Firebase
                await Conexionfirebase.firebase
                    .Child("Usuarios")
                    .Child(usuario.Id_User)
                    .PatchAsync(new { TipoPerfil = nuevoTipoPerfil });

                // Actualiza localmente
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
    }
}
