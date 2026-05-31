using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static APIController.Clases.IAs;
using Message = APIController.Clases.IAs.Message;

namespace APIController.Services.IAs
{
    public class Perplexity
    {
        public static async Task<string> EnviarPerplexity(PerplexityRequest E, string UrlApi)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    var loginData = new
                    {
                        model = E.model,
                        messages = new List<Message>
                        {
                            new Message
                            {
                                role = E.messages[0].role,
                                content = E.messages[0].content
                            }
                        },
                        max_tokens = E.max_tokens,
                        temperature = E.temperature,
                        APIKey = E.APIKey
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var jsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                    // Armar nuevo objeto con la propiedad en base64
                    var body = new
                    {
                        cadena = jsonBase64
                    };

                    var jsonBody = JsonConvert.SerializeObject(body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = UrlApi + "/api/IA/Perplexity";
                    //string endpoint = "https://localhost:5101/api/IA/Perplexity";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
