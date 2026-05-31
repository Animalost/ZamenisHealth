using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APIController.Services.Login
{
    public class LoginService
    {
        public async Task<bool> Loguear(string URLAPIConexion, string User, string Cadena) 
        {
            try
            {
                ControladorConfiguracion.URLAPIConexion = URLAPIConexion;

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    var loginData = new
                    {
                        Username = User.ToUpper().Trim(),
                        Cadena = Cadena.ToString().Trim()
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = URLAPIConexion.TrimEnd('/') + "/api/auth/LoginZH";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);
                    int statusCode = response.StatusCode.GetHashCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<string>(jsonResponse);

                        if (result != null && statusCode == 200)
                        {
                            ControladorConfiguracion.Token = result;  // Guardar el token
                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }        
    }
}
