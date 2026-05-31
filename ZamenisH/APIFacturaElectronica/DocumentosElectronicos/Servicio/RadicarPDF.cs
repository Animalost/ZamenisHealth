using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentosElectronicos.Servicio
{
    public class RadicarPDF
    {
        public async Task SendPDF(string Token, string URLApi, string user, string pass, int PrestadorCode, string DocB64, string Cufe, string NFactura)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {                
                string endpoint = URLApi + "/api/FacturacionElectronica/RadicarPDF";

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
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Token);

                    client.Timeout = TimeSpan.FromSeconds(30);

                    var loginData = new
                    {
                        Usuario = user,
                        Contraseña = pass,
                        Cufe = Cufe,
                        PdfB64 = DocB64,
                        Prestador = 0,
                        ClaveTecnica = Token
                    };

                    // Serializar y codificar en Base64
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

                    response = await client.PostAsync(endpoint, content).ConfigureAwait(false);
                    string f = "OK";
                   /* if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        try
                        {
                            var jsonr = JObject.Parse(jsonResponse);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }                        
                    } */                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(NFactura + ": " + ex.Message);
            }
        }
    }
}
