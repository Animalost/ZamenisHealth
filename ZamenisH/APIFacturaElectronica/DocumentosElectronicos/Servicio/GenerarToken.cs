using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static DocumentosElectronicos.Request.ResponseAPI;

namespace DocumentosElectronicos.Servicio
{
    public class GenerarToken
    {
        public async Task<XMLResponseF1> GetToken(string user, string pass, int PrestadorCode, string URLApi)
        {
            try
            {
                string endpoint = URLApi + "/api/FacturacionElectronica/GenerarToken";

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var loginData = new
                    {
                        Prestador = PrestadorCode.ToString(),
                        Usuario = user,
                        Contraseña = pass
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

                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<XMLResponseF1>(jsonResponse);

                    return result;                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error grave: " + ex.Message);
                throw new Exception("Error grave: " + ex.Message);
            }
        }
       
    }
}
