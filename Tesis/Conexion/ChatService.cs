using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tesis.Models;

namespace Tesis.Conexion
{
    public class ChatService
    {
        private readonly string apiUrl = "https://servicios.hostsbeast.com/chat"; // Cambia esto si tu API Flask está en un servidor remoto

        public async Task<string> SendMessageAsync(string userMessage, string userId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Crear el objeto JSON para enviar
                    var data = new
                    {
                        message = userMessage,
                        userId = userId // Agregar el ID del usuario
                    };
                    var jsonContent = JsonConvert.SerializeObject(data);

                    // Configurar el contenido para la solicitud POST
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Hacer la solicitud POST
                    var response = await client.PostAsync(apiUrl, content);

                    // Registrar el estado de la respuesta
                    Console.WriteLine($"Código de estado: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(responseString);

                        return chatResponse.Response;
                    }
                    else
                    {
                        // Leer el contenido de error
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {errorContent}");
                        return $"Error al comunicarse con el asistente. Código de estado: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
