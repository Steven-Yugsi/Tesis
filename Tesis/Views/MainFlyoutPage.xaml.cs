using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;
using Tesis.Conexion;
using Tesis.Models;
using Tesis.ViewModels;
using Tesis.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tesis.Views
{
    public partial class MainFlyoutPage : FlyoutPage
    {
        public Command ShowProfileCommand { get; }
        public Command ShowRolesCommand { get; }
        public Command ShowUserListCommand { get; }
        public Command GoToHomePageCommand { get; }
        private string userProfileType;


        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public MainFlyoutPage()
        {
            InitializeComponent();
            ShowProfileCommand = new Command(async () => await GoToUserProfilePage());
            ShowRolesCommand = new Command(async () => await OnAdministrarRolesClicked());
            GoToHomePageCommand = new Command(GoToHomePage);
            ShowUserListCommand = new Command(OpenUserListPage);

            BindingContext = this;
            CheckUserProfile();
        }

        private async void OpenUserListPage()
        {
            try
            {
                Detail = new NavigationPage(new UserListPage());
                IsPresented = false; // Cerrar el menú lateral
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir la página de usuarios: {ex.Message}", "Aceptar");
            }
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
                if (userProfile != null)
                {
                    userProfileType = userProfile.TipoPerfil; // Guardar el tipo de perfil
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
            }
        }

        private async Task<MUsuarios> GetUserProfileAsync(string userId)
        {
            {
                try
                {
                    // Asegúrate de usar tu cliente Firebase configurado en `Conexionfirebase`
                    var userProfile = await Conexionfirebase.firebase
                        .Child("Usuarios") // Nodo principal donde almacenas los usuarios
                        .Child(userId)  // Nodo del usuario específico
                        .OnceSingleAsync<MUsuarios>(); // Mapea los datos al modelo UserProfile

                    return userProfile; // Devuelve el perfil del usuario
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener el perfil: {ex.Message}");
                    return null; // En caso de error, devuelve null
                }
            }
        }

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

                var userId = await UserProfileViewModel.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Error", "No se pudo obtener el ID del usuario.", "Aceptar");
                    Application.Current.MainPage = new LoginPage();
                    return;
                }

                Detail = new NavigationPage(new UserProfilePage(userId));
                IsPresented = false; // Cerrar el menú lateral
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
            }
        }

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
                    SecureStorage.Remove("firebase_token");
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "No se pudo cerrar sesión: " + ex.Message, "OK");
                }
            }
        }

        private async Task OnAdministrarRolesClicked()
        {
            try
            {
                Detail = new NavigationPage(new AdministrarRolesPage());
                IsPresented = false; // Cerrar el menú lateral
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir la página de roles: {ex.Message}", "Aceptar");
            }
        }
        private void GoToHomePage()
        {
            if (string.IsNullOrEmpty(userProfileType))
            {
                DisplayAlert("Error", "No se pudo determinar el tipo de perfil del usuario.", "Aceptar");
                return;
            }
            // Redirigir a la página correspondiente según el tipo de perfil
            Page homePage = GetHomePageForProfile(userProfileType);

            if (homePage != null)
            {
                Detail = new NavigationPage(homePage);
                IsPresented = false; // Cerrar el menú lateral
            }
            else
            {
                DisplayAlert("Error", "No se pudo determinar la página de inicio para este perfil.", "Aceptar");
            }
        }
        private Page GetHomePageForProfile(string profileType)
        {
            // Devuelve la página correspondiente según el perfil
            switch (profileType)
            {
                case "Administrador":
                    return new AdminMainPage(); // Reemplaza con tu página real para el administrador
                case "Estudiante":
                    return new StudentPage(); // Reemplaza con tu página real para el estudiante
                case "Psicologo":
                    return new PsychologistPage(); // Reemplaza con tu página real para el psicólogo
                default:
                    return null; // Si no hay un perfil válido
            }
        }
    }
    public class UserProfile
    {
        public string Tipoperfil { get; set; }
        // Otros campos del perfil...

    }
}


