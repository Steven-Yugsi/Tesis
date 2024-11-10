using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Atributos
        private string email;
        private string clave;
        #endregion

        #region Propiedades
        public string Email
        {
            get { return email; }
            set { SetValue(ref email, value); }
        }

        public string Clave
        {
            get { return clave; }
            set { SetValue(ref clave, value); }
        }
        #endregion

        #region Command
        public Command RegisterCommand { get; }
        #endregion

        #region Metodo
        public async Task RegisterUsuario()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Clave))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "Por favor ingrese todos los campos.", "Aceptar");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "El campo de correo electrónico está vacío.", "Aceptar");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "Por favor ingrese un correo electrónico válido.", "Aceptar");
                return;
            }

            if (string.IsNullOrWhiteSpace(Clave))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "El campo de contraseña está vacío.", "Aceptar");
                return;
            }

            if (Clave.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "La contraseña debe tener al menos 6 caracteres.", "Aceptar");
                return;
            }

            var newUser = new UserModel()
            {
                EmailField = Email,
                PasswordField = Clave,
            };

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(DBConn.WepApyAuthentication));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(newUser.EmailField, newUser.PasswordField);
                string token = auth.FirebaseToken;

                await App.Current.MainPage.DisplayAlert("Registro exitoso", "El usuario se ha registrado correctamente.", "Aceptar");

                // Navegar a la página principal o de login después del registro exitoso
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error de registro", ex.Message, "Aceptar");
            }
        }
        private bool IsValidEmail(string email)
        {
            // Expresión regular para validar el formato de correo electrónico
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Constructor
        public RegisterViewModel(INavigation navigation)
        {
            Navigation = navigation;
            RegisterCommand = new Command(async () => await RegisterUsuario());
        }
        #endregion
    }
}
