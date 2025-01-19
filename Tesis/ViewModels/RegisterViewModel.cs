using Firebase.Auth;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Atributos
        private MUsuarios usuario = new MUsuarios();
        private string profileImagePath;
        //private List<string> tiposDePerfil = new List<string>();
        private ObservableCollection<string> tiposDePerfil = new ObservableCollection<string>();
        #endregion

        #region Propiedades
        public MUsuarios Usuario
        {
            get => usuario;
            set => SetValue(ref usuario, value);
        }

        public string ProfileImage
        {
            get => profileImagePath;
            set => SetValue(ref profileImagePath, value);
        }
        public ObservableCollection<string> TiposDePerfil
        {
            get => tiposDePerfil;
            set => SetValue(ref tiposDePerfil, value);
        }
        #endregion

        #region Comandos
        public Command RegisterCommand { get; }
        public Command SelectImageCommand { get; }
        #endregion

        #region Métodos
        private async Task CargarTiposDePerfilAsync()
        {
            try
            {
                var roles = await Conexionfirebase.firebase
                    .Child("Roles") // Nombre del nodo donde están los roles
                    .OnceAsync<dynamic>(); // Asume que los roles son cadenas de texto

                // Extraer el valor de cada nodo y asignarlo a la lista
                Device.BeginInvokeOnMainThread(() =>
                {
                    TiposDePerfil.Clear();

                    foreach (var role in roles)
                    {
                        if (role.Object != null && role.Object.Nombre != null)
                        {
                            TiposDePerfil.Add(role.Object.Nombre.ToString());
                        }
                        else
                        {
                            Console.WriteLine($"Rol inválido o sin nombre: {role.Object}");
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
        // Solicitar permisos de almacenamiento o medios según el sistema operativo
        private async Task<bool> RequestStoragePermissionAsync()
        {
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    var version = DeviceInfo.Version.Major;

                    if (version >= 13) // Android 13 o superior
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.Media>();
                        if (status != PermissionStatus.Granted)
                        {
                            status = await Permissions.RequestAsync<Permissions.Media>();
                        }
                        if (status != PermissionStatus.Granted)
                        {
                            await App.Current.MainPage.DisplayAlert(
                                "Permiso requerido",
                                "Se necesita acceso a las imágenes para continuar.",
                                "Aceptar"
                            );
                            return false;
                        }
                    }
                    else
                    {
                        var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                        if (status != PermissionStatus.Granted)
                        {
                            status = await Permissions.RequestAsync<Permissions.StorageRead>();
                        }
                        if (status != PermissionStatus.Granted)
                        {
                            await App.Current.MainPage.DisplayAlert(
                                "Permiso requerido",
                                "Se necesita acceso al almacenamiento para continuar.",
                                "Aceptar"
                            );
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al solicitar permisos: {ex.Message}");
                return false;
            }
        }

        // Seleccionar imagen del dispositivo
        public async Task<string> PickImageAsync()
        {
            if (!await RequestStoragePermissionAsync())
                return null;

            try
            {
                if (!MediaPicker.IsCaptureSupported)
                {
                    await App.Current.MainPage.DisplayAlert("No soportado", "El selector de imágenes no está disponible en este dispositivo.", "Aceptar");
                    return null;
                }

                var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecciona una imagen de perfil"
                });

                return photo?.FullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar la imagen: {ex.Message}");
                return null;
            }
        }

        // Subir imagen seleccionada a Firebase Storage
        public async Task<string> UploadImageAsync(string imagePath, string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath) || string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("La ruta de la imagen o el nombre del archivo no pueden estar vacíos.");

                if (!File.Exists(imagePath))
                    throw new FileNotFoundException("El archivo no existe en la ruta especificada.");

                // Validar extensión de archivo
                string fileExtension = Path.GetExtension(imagePath)?.ToLower();
                if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jpeg")
                    throw new NotSupportedException("Solo se permiten imágenes en formato JPG, PNG o JPEG.");

                string sanitizedFileName = $"{Usuario.Nombre}_{Usuario.Apellido}_profile{fileExtension}"
                    .Replace(" ", "_")
                    .Replace("/", "")
                    .Replace("\\", "");

                Console.WriteLine($"Subiendo imagen: {imagePath} como {sanitizedFileName}");

                var storageClient = Conexionfirebase.firebaseStorage;

                using (var stream = File.OpenRead(imagePath))
                {
                    var imageUrl = await storageClient
                        .Child(sanitizedFileName)
                        .PutAsync(stream);

                    Console.WriteLine($"URL de la imagen subida: {imageUrl}");
                    return imageUrl;
                }
            }
            catch (FirebaseStorageException ex)
            {
                Console.WriteLine($"Error de Firebase Storage: {ex.Message}");
                throw new Exception("Error de Firebase al subir la imagen: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir la imagen: {ex.Message}");
                throw new Exception("Error al subir la imagen: " + ex.Message);
            }
        }

        // Manejar la selección de imagen
        // Seleccionar y mostrar la imagen
        public async Task SelectImage()
        {
            try
            {
                var imagePath = await PickImageAsync();
                if (!string.IsNullOrEmpty(imagePath))
                {
                    ProfileImage = imagePath;
                    Console.WriteLine($"Ruta de imagen seleccionada: {ProfileImage}");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se seleccionó ninguna imagen.", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SelectImage: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un problema al seleccionar la imagen.", "Aceptar");
            }
        }

        // Registrar usuario
        public async Task RegisterUsuario()
        {
            if (string.IsNullOrWhiteSpace(Usuario.Correo) || string.IsNullOrWhiteSpace(Usuario.Contraseña))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Advertencia",
                    "Por favor, complete todos los campos obligatorios.",
                    "Aceptar"
                );
                return;
            }
            if (Usuario.Contraseña.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Contraseña débil",
                    "La contraseña debe tener al menos 6 caracteres.",
                    "Aceptar"
                );
                return;
            }

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(DBConn.WepApyAuthentication));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(Usuario.Correo, Usuario.Contraseña);
                Usuario.Id_User = auth.User.LocalId;

                if (auth.User != null)
                {
                    // Enviar el correo de verificación
                    await EnviarCorreoVerificacion(auth.FirebaseToken);

                    // Informar al usuario
                    await App.Current.MainPage.DisplayAlert("¡Cuenta creada!",
                                                             "Hemos enviado un correo de verificación a tu cuenta. Debes verificarlo para poder ingresar.",
                                                             "OK");
                }

                if (!string.IsNullOrEmpty(ProfileImage))
                {
                    // Usar la extensión correcta para el archivo
                    string fileName = $"{Usuario.Nombre}_{Usuario.Apellido}_profile.jpg"; // Puede cambiar a .png si lo prefieres
                    Usuario.Imagen = await UploadImageAsync(ProfileImage, fileName);
                }

                await Conexionfirebase.firebase
                    .Child("Usuarios")
                    .Child(Usuario.Id_User)
                    .PutAsync(Usuario);

                
                Usuario = new MUsuarios();
                ProfileImage = null;

                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
            catch (FirebaseAuthException ex)
            {
                // Verifica el tipo de error específico
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Este correo electrónico ya está registrado.", "Aceptar");
                    return;
                }
                else if (ex.Message.Contains("INVALID_EMAIL"))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "El correo electrónico proporcionado no es válido.", "Aceptar");
                }
                else if (ex.Message.Contains("WEAK_PASSWORD"))
                {
                    await App.Current.MainPage.DisplayAlert("Contraseña débil", "La contraseña debe tener al menos 6 caracteres.", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"Error al registrar el usuario: {ex.Message}", "Aceptar");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el usuario: {ex.Message}");
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Error al registrar el usuario: {ex.Message}",
                    "Aceptar"
                );
            }
        }
        public async Task EnviarCorreoVerificacion(string idToken)
        {
            try
            {
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={DBConn.WepApyAuthentication}";
                var httpClient = new HttpClient();

                var payload = new
                {
                    requestType = "VERIFY_EMAIL",
                    idToken = idToken
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Correo de verificación enviado correctamente.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al enviar el correo de verificación: {errorContent}");
                    throw new Exception("No se pudo enviar el correo de verificación.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo de verificación: {ex.Message}");
                throw;
            }
        }

        //public List<string> TiposDePerfil { get; set; } = new List<string>
        //{
        //    "Doctor",
        //    "Estudiante",
        //    "Profesor",
        //    "Paciente"
        //};
        #endregion

        #region Constructor
        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await RegisterUsuario());
            SelectImageCommand = new Command(async () => await SelectImage());
            //Task.Run(async () => await CargarTiposDePerfilAsync());
            MainThread.InvokeOnMainThreadAsync(async () => await CargarTiposDePerfilAsync());

        }
        #endregion
    }
}
