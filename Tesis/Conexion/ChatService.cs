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
        private readonly string apiUrl = "http://10.0.2.2:5000/chat"; // Cambia esto si tu API Flask está en un servidor remoto

        public async Task<string> SendMessageAsync(string userMessage)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Crear el objeto JSON para enviar
                    var data = new { message = userMessage };
                    var jsonContent = JsonConvert.SerializeObject(data);

                    // Configurar el contenido para la solicitud POST
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Hacer la solicitud POST
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer la respuesta del asistente
                        var responseString = await response.Content.ReadAsStringAsync();
                        var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(responseString);

                        return chatResponse.Response;
                    }
                    else
                    {
                        return "Error al comunicarse con el asistente.";
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