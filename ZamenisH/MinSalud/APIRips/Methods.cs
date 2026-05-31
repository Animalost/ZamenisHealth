using APIRips.ClassRequest;
using Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIRips
{
    public class Methods
    {
        private static readonly HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        private static readonly HttpClient _httpClient = new HttpClient(handler);

        public static async Task<(int statusCode, LoginResponse data, string errorMessage)> Loguear(string URLAPIConexion, 
                                                                                                    string User, 
                                                                                                    string Pass, 
                                                                                                    string Nit, 
                                                                                                    string Tipo) 
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var loginData = new loginRequest
                    {
                        persona = new persona
                        {
                            identificacion = new identificacion
                            {
                                tipo = Tipo,
                                numero = User
                            }
                        },

                        clave = Pass,
                        nit = Nit
                    };

                    string jsonContent = System.Text.Json.JsonSerializer.Serialize(loginData, new  JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(URLAPIConexion, content);
                    int statusCode = (int)response.StatusCode;
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserializar la respuesta JSON a un objeto LoginResponse
                        var data = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(jsonResponse, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                    
                        //LoginResponse data = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(jsonResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        string primerError = data.errors != null && data.errors.Length > 0 ? data.errors[0] : null;

                        return (statusCode, data, primerError);
                    }
                    else
                    {
                        return (statusCode, null, $"Error: {jsonResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (0, null, ex.Message);
            }
        }

        public static async Task<(int statusCode, string responseData, string errorMessage)> SendFacturaMinSalud(string url, string token, TransaccionDocker texto)
        {
            try
            {
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(texto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


                var jsonObj = JObject.Parse(jsonContent); 

                var usuarios = jsonObj.SelectToken("rips.usuarios") as JArray;
                if (usuarios != null)
                {
                    foreach (JObject usuario in usuarios)
                    {
                        var servicios = usuario["servicios"] as JObject;
                        if (servicios != null)
                        {
                            // Verifica y elimina si son nulos o arrays vacíos
                            JToken consultas = servicios["consultas"];
                            if (consultas == null || (consultas.Type == JTokenType.Array && !consultas.HasValues))
                            {
                                servicios.Remove("consultas");
                            }

                            JToken procedimientos = servicios["procedimientos"];
                            if (procedimientos == null || (procedimientos.Type == JTokenType.Array && !procedimientos.HasValues))
                            {
                                servicios.Remove("procedimientos");
                            }

                            JToken otrosServicios = servicios["otrosServicios"];
                            if (otrosServicios == null || (otrosServicios.Type == JTokenType.Array && !otrosServicios.HasValues))
                            {
                                servicios.Remove("otrosServicios");
                            }

                            // Si servicios ya no tiene ninguna propiedad, lo quitamos completamente
                            if (!servicios.HasValues)
                            {
                                usuario.Remove("servicios");
                            }
                        }
                    }
                }

                // Convertimos de nuevo a string limpio para enviar
                jsonContent = jsonObj.ToString(Newtonsoft.Json.Formatting.None);


                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                int statusCode = (int)response.StatusCode;
                string jsonResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine(FormatJson(jsonResponse));
                return (statusCode, FormatJson(jsonResponse), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (500, null, ex.Message);
            }
        }

        static string FormatJson(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    return System.Text.Json.JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                }
            }
            catch
            {
                return json; 
            }
        }
    }
}
