using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tesis
{
    public partial class MainFlyoutPage : FlyoutPage
    {
        public MainFlyoutPage()
        {
            InitializeComponent();
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
                    SecureStorage.Remove("firebase_token");  // Si usas SecureStorage
                    Preferences.Remove("firebase_token");   // Si usas Preferences

                    // Redirigir a la pantalla de inicio de sesión
                    Application.Current.MainPage = new LoginPage();  // Reemplaza con tu página de login
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    await DisplayAlert("Error", "No se pudo cerrar sesión: " + ex.Message, "OK");
                }
            }
            else
            {
                // Si el usuario cancela, no hacer nada.
                return;
            }
        }
    }
}
