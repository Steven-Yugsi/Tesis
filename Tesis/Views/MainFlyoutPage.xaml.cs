using Firebase.Database;
using System;
using System.Threading.Tasks;
using Tesis.ViewModels;
using Tesis.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tesis.Views
{
    public partial class MainFlyoutPage : FlyoutPage
    {
        public Command ShowProfileCommand { get; }
        public Command ShowRolesCommand { get; set; }
        public Command ShowUserListCommand { get; }

        public MainFlyoutPage()
        {
            InitializeComponent();
            ShowProfileCommand = new Command(async () => await GoToUserProfilePage());
            ShowRolesCommand = new Command(async () => await OnAdministrarRolesClicked());
            ShowUserListCommand = new Command(OpenUserListPage);

            BindingContext = this;
            CheckUserProfile();
        }
        private async void OpenUserListPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new UserListPage());
        }

        private async void CheckUserProfile()
        {
            var firebaseToken = await SecureStorage.GetAsync("firebase_token");
            if (string.IsNullOrEmpty(firebaseToken))
            {
                await DisplayAlert("Error", "Token inválido. Inicia sesión nuevamente.", "Aceptar");
                Application.Current.MainPage = new LoginPage();
                return;
            }

            try
            {
                // Obtener ID de usuario autenticado
                var userId = await UserProfileViewModel.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Error", "No se pudo obtener el ID del usuario.", "Aceptar");
                    Application.Current.MainPage = new LoginPage();
                    return;
                }

                // Cargar los datos del perfil del usuario
                var userProfile = await GetUserProfileAsync(userId);

                // Si el tipo de perfil es "Administrador", mostrar la opción de Administrar Roles
                if (userProfile?.Tipoperfil == "Administrador")
                {
                    ShowRolesCommand = new Command(async () => await OnAdministrarRolesClicked());

                }
                else
                {
                    ShowRolesCommand = null; // No mostrar la opción para roles
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
            }
        }

        // Método que obtiene los datos del perfil del usuario desde Firebase
        private async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            // Aquí obtendrás el perfil del usuario desde Firebase usando su ID
            // Este es solo un ejemplo, reemplaza con la lógica que utilizas para obtener el perfil.
            var userProfile = new UserProfile { Tipoperfil = "Administrador" };  // Simulación
            return await Task.FromResult(userProfile);
        }

        // Método para ir al perfil del usuario
        private async Task GoToUserProfilePage()
        {
            try
            {
                var firebaseToken = await SecureStorage.GetAsync("firebase_token");
                if (string.IsNullOrEmpty(firebaseToken))
                {
                    await DisplayAlert("Error", "Token inválido. Inicia sesión nuevamente.", "Aceptar");
                    Application.Current.MainPage = new LoginPage();
                    return;
                }

                // Obtener ID de usuario autenticado
                var userId = await UserProfileViewModel.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Error", "No se pudo obtener el ID del usuario.", "Aceptar");
                    Application.Current.MainPage = new LoginPage();
                    return;
                }

                // Cargar datos del usuario
                var userProfilePage = new UserProfilePage(userId);
                await Navigation.PushAsync(userProfilePage);  // Navegar a la página del perfil
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
            }
        }

        // Cuando el usuario elige cerrar sesión
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await DisplayAlert("Confirmación",
                                         "¿Estás seguro de que deseas cerrar sesión?",
                                         "Sí",
                                         "No");

            if (isConfirmed)
            {
                try
                {
                    // Eliminar el token de Firebase (si lo estás almacenando)
                    SecureStorage.Remove("firebase_token");

                    // Redirigir a la pantalla de inicio de sesión
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "No se pudo cerrar sesión: " + ex.Message, "OK");
                }
            }
        }

        // Método que maneja la navegación cuando el usuario hace click en "Administrar Roles"
        private async Task OnAdministrarRolesClicked()
        {
            await Navigation.PushAsync(new AdministrarRolesPage());
        }
    }

    // Clase que representa la estructura del perfil del usuario
    public class UserProfile
    {
        public string Tipoperfil { get; set; }
        // Otros campos del perfil...
    }
}
