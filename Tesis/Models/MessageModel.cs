using System;
using System.Collections.Generic;
using System.Text;

namespace Tesis.Models
{
    public class MessageModel
    {
        public string Role { get; set; } // "user" o "assistant"
        public string Content { get; set; } // El contenido del mensaje
        public string UserId { get; set; }
    }

    public class ChatResponse
    {
        public string Response { get; set; } // Respuesta del asistente
    }
}
