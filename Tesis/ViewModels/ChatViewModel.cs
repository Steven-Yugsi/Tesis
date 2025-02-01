using Firebase.Auth;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Essentials; // Necesario para acceder a SecureStorage
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly ChatService _chatService;
        private string _userMessage;
        private string _assistantResponse;
        public string UserId { get; set; }

        public ObservableCollection<MessageModel> Messages { get; set; }

        public string UserMessage
        {
            get => _userMessage;
            set => SetValue(ref _userMessage, value);
        }

        public string AssistantResponse
        {
            get => _assistantResponse;
            set => SetValue(ref _assistantResponse, value);
        }

        public ICommand SendMessageCommand { get; }

        public ChatViewModel()
        {
            _chatService = new ChatService();
            Messages = new ObservableCollection<MessageModel>();
            SendMessageCommand = new Command(async () => await SendMessage());

            // Recuperar el UserId desde el SecureStorage
            Task.Run(async () => await GetUserId());
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
            if (string.IsNullOrWhiteSpace(UserMessage))
                return;

            // Verificar que el UserId esté disponible
            if (string.IsNullOrWhiteSpace(UserId))
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo obtener el ID del usuario.", "Aceptar");
                return;
            }

            // Agregar el mensaje del usuario a la lista de mensajes
            Messages.Add(new MessageModel { Role = "user", Content = UserMessage, UserId = UserId });
            Console.WriteLine($"Mensaje del usuario agregado: {UserMessage}");

            // Enviar el mensaje al asistente junto con el ID del usuario
            var response = await _chatService.SendMessageAsync(UserMessage, UserId);
            Console.WriteLine($"Respuesta del asistente: {response}");

            // Agregar la respuesta del asistente a la lista de mensajes
            Messages.Add(new MessageModel { Role = "assistant", Content = response });

            // Limpiar el mensaje del usuario
            UserMessage = string.Empty;
        }
    }
}
