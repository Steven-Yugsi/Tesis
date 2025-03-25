using Firebase.Auth;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly ChatService _chatService;
        private string _userMessage;
        private bool _isBusy;
        public string UserId { get; set; }

        public ObservableCollection<MessageModel> Messages { get; set; }

        public string UserMessage
        {
            get => _userMessage;
            set
            {
                SetValue(ref _userMessage, value);
                // Actualizar disponibilidad del botón
                (SendMessageCommand as Command)?.ChangeCanExecute();
            }
        }

        
        public ICommand SendMessageCommand { get; }

        public ChatViewModel()
        {
            _chatService = new ChatService();
            Messages = new ObservableCollection<MessageModel>();
            SendMessageCommand = new Command(async () => await SendMessage(), () => CanExecuteSendMessage());

            // Recuperar el UserId desde el SecureStorage
            Task.Run(async () => await GetUserId());
        }
        private bool CanExecuteSendMessage()
        {
            return !string.IsNullOrWhiteSpace(UserMessage?.Trim());
        }

        public async Task GetUserId()
        {
            // Intentar obtener el UserId almacenado de forma segura
            var userId = await SecureStorage.GetAsync("user_id");

            if (userId != null)
            {
                UserId = userId; // Asignar el UserId recuperado
                Console.WriteLine($"UserId recuperado: {UserId}"); // Depuración
            }
            else
            {
                UserId = "unknown"; // Si no se encuentra el UserId
                Console.WriteLine("UserId no encontrado, usando 'unknown'."); // Depuración
            }
        }

        private async Task SendMessage()
        {
            // Evitar múltiples ejecuciones simultáneas
            if (_isBusy || !CanExecuteSendMessage()) // Doble validación
                return;
            _isBusy = true;


            if (string.IsNullOrWhiteSpace(UserMessage))
                return;

            // Verificar UserId
            if (string.IsNullOrWhiteSpace(UserId))
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo obtener el ID del usuario.", "Aceptar");
                return;
            }

            _isBusy = true;

            try
            {
                // Agregar y limpiar mensaje INMEDIATAMENTE
                var userMessage = UserMessage;
                Messages.Add(new MessageModel
                {
                    Role = "user",
                    Content = userMessage,
                    UserId = UserId
                });

                UserMessage = string.Empty; // Limpiar cuadro de texto aquí
                Console.WriteLine($"Mensaje del usuario agregado: {userMessage}");

                // Obtener respuesta
                var response = await _chatService.SendMessageAsync(userMessage, UserId);
                Console.WriteLine($"Respuesta del asistente: {response}");

                Messages.Add(new MessageModel
                {
                    Role = "assistant",
                    Content = response
                });
            }
            finally
            {
                _isBusy = false; // Siempre liberar el bloqueo
            }
        }

    }
}
