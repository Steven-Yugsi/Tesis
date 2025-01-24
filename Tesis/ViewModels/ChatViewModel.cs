using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Tesis.Conexion;
using Tesis.Models;
using Xamarin.Forms;

namespace Tesis.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly ChatService _chatService;
        private string _userMessage;
        private string _assistantResponse;


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
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(UserMessage))
                return;

            // Agregar el mensaje del usuario a la lista de mensajes
            Messages.Add(new MessageModel { Role = "user", Content = UserMessage });

            // Enviar el mensaje al asistente
            var response = await _chatService.SendMessageAsync(UserMessage);

            // Agregar la respuesta del asistente a la lista de mensajes
            Messages.Add(new MessageModel { Role = "assistant", Content = response });

            // Limpiar el mensaje del usuario
            UserMessage = string.Empty;
        }
        public class Message
        {
            public string Content { get; set; }
            public string Role { get; set; } // 'User' o 'System'

            // Propiedad para definir la alineación
            public LayoutOptions MessageAlignment
            {
                get
                {
                    return Role == "User" ? LayoutOptions.End : LayoutOptions.Start; // Usuario a la derecha, sistema a la izquierda
                }
            }

            // Propiedad para definir el color de fondo
            public Color MessageBackgroundColor
            {
                get
                {
                    return Role == "User" ? Color.LightBlue : Color.Gray; // Azul para usuario, gris para sistema
                }
            }
        }
    }
}
