using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesis.Conexion;
using Tesis.Models;
using Tesis.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Atributos
        private string email;
        private string clave;
        private bool isPasswordVisible;  // Nueva propiedad para la visibilidad de la contraseña
        private string eyeIcon;  // Nueva propiedad para el ícono del ojo
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

        // Propiedad para la visibilidad de la contraseña
        public bool IsPasswordVisible
        {
            get { return isPasswordVisible; }
            set { SetValue(ref isPasswordVisible, value); }
        }

        // Propiedad para el ícono del ojo
        public string EyeIcon
        {
            get { return eyeIcon; }
            set { SetValue(ref eyeIcon, value); }
        }
        #endregion

        #region Commands
        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }
        public Command ForgotPasswordCommand { get; }
        public Command TogglePasswordVisibilityCommand { get; }  // Nuevo comando para alternar la visibilidad de la contraseña
        #endregion

        #region Métodos

        public async Task LoginUsuario()
        {
            // Validación inicial de entradas
            if (string.IsNullOrWhiteSpace(txtemail) || string.IsNullOrWhiteSpace(txtclave))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "Por favor ingresa tu correo y contraseña.", "Aceptar");
                return;
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,4}$";
            if (!Regex.IsMatch(txtemail, emailPattern))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "El formato del correo electrónico es inválido. Asegúrate de ingresarlo correctamente.", "Aceptar");
                return;
            }

            var objusuario = new UserModel
            {
                EmailField = txtemail,
                PasswordField = txtclave
            };

            try
            {
                // Configuración del proveedor de autenticación
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(DBConn.WepApyAuthentication));

                // Intentar iniciar sesión con Firebase
                var authuser = await authProvider.SignInWithEmailAndPasswordAsync(objusuario.EmailField, objusuario.PasswordField);

                if (!authuser.User.IsEmailVerified)
                {
                    await App.Current.MainPage.DisplayAlert(
                        "Cuenta no verificada",
                        "Por favor verifica tu correo antes de iniciar sesión.",
                        "Aceptar"
                    );
                    return;
                }

                var token = authuser.FirebaseToken;
                var authData = JsonConvert.SerializeObject(authuser);
                await SecureStorage.SetAsync("firebase_token", authData);
                // Si la autenticación es exitosa, redirige al usuario
                var navigationPage = new NavigationPage(new MainFlyoutPage())
                {
                    BarBackgroundColor = Color.RoyalBlue
                };

                App.Current.MainPage = navigationPage;
            }
            catch (FirebaseAuthException ex)
            {
                string mensajeError;
                if (ex.Reason == AuthErrorReason.WrongPassword)
                {
                    mensajeError = "La contraseña es incorrecta. Por favor, inténtalo de nuevo.";
                }
                else if (ex.Reason == AuthErrorReason.UnknownEmailAddress)
                {
                    mensajeError = "El correo electrónico no está registrado. Por favor, regístrate primero.";
                }
                else if (ex.Reason == AuthErrorReason.UserDisabled)
                {
                    mensajeError = "La cuenta ha sido deshabilitada. Contacta al soporte técnico.";
                }
                else if (ex.Reason == AuthErrorReason.TooManyAttemptsTryLater)
                {
                    mensajeError = "Se ha bloqueado temporalmente el acceso debido a demasiados intentos fallidos. Intenta más tarde.";
                }
                else
                {
                    mensajeError = $"Error de autenticación: {ex.Message}";
                }

                await App.Current.MainPage.DisplayAlert("Error de inicio de sesión", mensajeError, "Aceptar");
            }
            catch (Exception ex)
            {
                // Manejo de errores desconocidos
                await App.Current.MainPage.DisplayAlert("Advertencia", $"Error desconocido: {ex.Message}", "Aceptar");
            }
        }

        public async Task ForgotPassword()
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

        // Método para alternar la visibilidad de la contraseña
        public void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
            EyeIcon = IsPasswordVisible ? "icon_eye_open.png" : "icon_eye.png";  // Cambia el ícono del ojo
        }
        #endregion

        #region Constructor
        public LoginViewModel(INavigation navegar)
        {
            Navigation = navegar;
            LoginCommand = new Command(async () => await LoginUsuario());
            RegisterCommand = new Command(async () => await GoToRegisterPage());
            ForgotPasswordCommand = new Command(async () => await ForgotPassword());
            TogglePasswordVisibilityCommand = new Command(() => TogglePasswordVisibility());  // Comando para alternar visibilidad
            EyeIcon = "icon_eye.png";  // Inicializa el ícono como ojo cerrado
            IsPasswordVisible = false;  // Inicializa la contraseña como oculta
        }
        #endregion
    }
}
