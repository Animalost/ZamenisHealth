using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APIController.Services.SignDocuments
{
    public class SignDocument
    {
        public async Task<(string URL, string CODE)> GenerarLink(ContenidoDocumento contentDoc, string endpoint)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "PostmanRuntime/7.43.4");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
                    //client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Token);

                    client.Timeout = TimeSpan.FromSeconds(30);

                    var loginData = new
                    {
                        NombrePaciente = contentDoc.NombrePaciente,
                        Documento = contentDoc.Documento,
                        Telefono = contentDoc.Telefono,
                        Email = contentDoc.Email,
                        NombreDoctor = contentDoc.NombreDoctor,
                        RegistroDoctor = contentDoc.RegistroDoctor,
                        FirmaDoctorBase64 = contentDoc.FirmaDoctorBase64,
                        TipoConsentimiento = contentDoc.TipoConsentimiento,
                        Compañia = contentDoc.Compañia.ToString(),
                        NIT = contentDoc.NIT.ToString()
                    };

                    var jsonBody = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.PostAsync(endpoint, content).ConfigureAwait(false);
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    int idx = jsonResponse.IndexOf('}');
                    if (idx > 0)
                        jsonResponse = jsonResponse.Substring(0, idx + 1);
                    var obj = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    return (obj.url, response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, "00");
            }
        }
    }
}
