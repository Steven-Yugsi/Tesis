using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tesis.Conexion;
using Tesis.Models;
using Tesis.Views;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Atributos
        private string email;
        private string clave;
        #endregion

        #region Propiedades
        public string txtemail
        {
            get { return email; }
            set { SetValue(ref email, value); }
        }
        public string txtclave
        {
            get { return clave; }
            set { SetValue(ref clave, value); }
        }

        #endregion

        #region Command
        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }
        public Command ForgotPasswordCommand { get; }
        #endregion

        #region Metodo
        public async Task LoginUsuario()
        {
            var objusuario = new UserModel()
            {
                EmailField = email,
                PasswordField = clave,
            };
            try
            {
                // Autenticación con Firebase
                var autenticacion = new FirebaseAuthProvider(new FirebaseConfig(DBConn.WepApyAuthentication));
                var authuser = await autenticacion.SignInWithEmailAndPasswordAsync(objusuario.EmailField, objusuario.PasswordField);

                string obtenerToken = authuser.FirebaseToken;  // Token del usuario autenticado

                // Al iniciar sesión con éxito, redirigimos al usuario al MainFlyoutPage
                // MainFlyoutPage es la página con el menú hamburguesa
                var propiedades_NavigationPage = new NavigationPage(new MainFlyoutPage());
                propiedades_NavigationPage.BarBackgroundColor = Color.RoyalBlue;

                // Cambiar la página principal a MainFlyoutPage
                App.Current.MainPage = propiedades_NavigationPage;

            }
            catch (Exception)
            {

                await App.Current.MainPage.DisplayAlert("Advertencia", "Los datos introducidos son incorrectos o el usuario se encuentra inactivo.", "Aceptar");
                //await App.Current.MainPage.DisplayAlert("Advertencia", ex.Message, "Aceptar");
            }
        }
        public async Task ForgotPassword()  // Método para restablecer la contraseña
        {
            if (string.IsNullOrWhiteSpace(txtemail))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "Por favor ingresa tu correo electrónico.", "Aceptar");
                return;
            }

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(DBConn.WepApyAuthentication));
                await authProvider.SendPasswordResetEmailAsync(txtemail);
                await App.Current.MainPage.DisplayAlert("Éxito", "Se ha enviado un correo para restablecer la contraseña.", "Aceptar");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error al enviar correo de restablecimiento: {ex.Message}", "Aceptar");
            }
        }

        private async Task GoToRegisterPage()
        {
            await Navigation.PushAsync(new RegisterPage());
        }
        #endregion

        #region Constructor
        public LoginViewModel(INavigation navegar)
        {
            Navigation = navegar;
            LoginCommand = new Command(async () => await LoginUsuario());
            RegisterCommand = new Command(async () => await GoToRegisterPage());
            ForgotPasswordCommand = new Command(async () => await ForgotPassword());
                
        }
        #endregion
    }
}