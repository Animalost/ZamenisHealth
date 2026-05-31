using APIController.Clases;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APIController.Services.Email
{
    public class EmailService
    {
        public async Task<string> SendEmailAPI(EmailRequest E, string UrlApi)
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
                        EmailFrom = E.EmailFrom,
                        EmailTo = E.EmailTo,
                        EmailPassword = E.EmailPassword,
                        EmailBcc1 = E.EmailBcc1,
                        EmailBcc2 = E.EmailBcc2,
                        Asunto = E.Asunto,
                        AttachmentFile = E.AttachmentFile,
                        BodyMessage = E.BodyMessage,
                        TipoArchivo = E.TipoArchivo
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = UrlApi + "/api/Email/SendEmail";
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
