using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class UsuarioViewModel : BaseViewModel
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string TipoPerfil { get; set; }

        public List<string> TiposDePerfil { get; set; } = new List<string>
        {
            "Doctor",
            "Estudiante",
            "Profesor",
            "Paciente"
        };
        public Command RegisterCommand { get; }

        public UsuarioViewModel()
        {
            RegisterCommand = new Command(async () => await InsertarUsuario());
        }

        public async Task InsertarUsuario()
        {
            // Validación de campos vacíos
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Contraseña) || 
                string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) ||
                string.IsNullOrWhiteSpace(Telefono) || string.IsNullOrWhiteSpace(TipoPerfil))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "Por favor ingrese todos los campos.", "Aceptar");
                return;
            }

            // Validación de correo electrónico vacío
            if (string.IsNullOrWhiteSpace(Correo))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "El campo de correo electrónico está vacío.", "Aceptar");
                return;
            }

            // Validación de formato de correo electrónico
            if (!IsValidEmail(Correo))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "Por favor ingrese un correo electrónico válido.", "Aceptar");
                return;
            }

            // Validación de contraseña vacía
            if (string.IsNullOrWhiteSpace(Contraseña))
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "El campo de contraseña está vacío.", "Aceptar");
                return;
            }

            // Validación de contraseña mínima de 6 caracteres
            if (Contraseña.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "La contraseña debe tener al menos 6 caracteres.", "Aceptar");
                return;
            }

            // Crear el objeto de usuario
            var nuevoUsuario = new MUsuarios
            {
                Nombre = this.Nombre,
                Apellido = this.Apellido,
                Telefono = this.Telefono,
                Correo = this.Correo,
                Contraseña = this.Contraseña,
                TipoPerfil = this.TipoPerfil
            };

            // Intentar insertar el usuario en la base de datos Firebase
            try
            {
                await Conexionfirebase.firebase
                    .Child("Usuarios")
                    .PostAsync(nuevoUsuario);

                // Mostrar mensaje de éxito
                await App.Current.MainPage.DisplayAlert("Registro exitoso", "El usuario se ha registrado correctamente.", "Aceptar");
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
            catch (Exception ex)
            {
                // Mostrar error en caso de fallo al insertar
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
    }
}
