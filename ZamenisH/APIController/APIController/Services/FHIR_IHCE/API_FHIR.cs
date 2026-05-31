using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APIController.Services.FHIR_IHCE
{
    public class API_FHIR
    {
        public async Task<(bool status, string res)> GetToken(ReceiveToken T, string UrlAPI)
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
                        NitPrestador = T.NitPrestador,
                        TenantID = T.TenantID,
                        ClientID = T.ClientID,
                        ClientSecret = T.ClientSecret,
                        URLToken = T.URLToken,
                        Scope = T.Scope
                    };

                    //Serializar en JSON y codificar en Base64
                    var json = JsonConvert.SerializeObject(loginData);
                    var jsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                    // Armar nuevo objeto con la propiedad ya en base64
                    var body = new
                    {
                        cadena = jsonBase64
                    };

                    var jsonBody = JsonConvert.SerializeObject(body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = UrlAPI + "/api/IHCE/GenerarToken";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if ((int)response.StatusCode == 200)
                    {
                        return (true, await response.Content.ReadAsStringAsync());
                    }

                    return (false, await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool status, string res)> SendRDAPaciente(string B64, string UrlAPI)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    // Armar nuevo objeto con la propiedad ya en base64
                    var body = new
                    {
                        cadena = B64
                    };

                    var jsonBody = JsonConvert.SerializeObject(body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = UrlAPI + "/api/IHCE/RDAPaciente";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if ((int)response.StatusCode < 299)
                    {
                        return (true, await response.Content.ReadAsStringAsync());
                    }

                    return (false, await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool status, string res)> SendRDAAmbulatorio(string B64, string UrlAPI)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    // Armar nuevo objeto con la propiedad ya en base64
                    var body = new
                    {
                        cadena = B64
                    };

                    var jsonBody = JsonConvert.SerializeObject(body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = UrlAPI + "/api/IHCE/RDAAmbulatorio";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if ((int)response.StatusCode < 299)
                    {
                        return (true, await response.Content.ReadAsStringAsync());
                    }

                    return (false, await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
