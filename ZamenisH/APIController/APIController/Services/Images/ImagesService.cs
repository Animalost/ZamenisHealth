using Domain.CXN;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APIController.Services.Images
{
    public class ImagesService
    {
        public async Task<int> insertImageAPI(CXN_IMAGENES I, string cadena)
        {
            try
            {
                string URLAPIConexion = ControladorConfiguracion.URLAPIConexion;

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    var loginData = new
                    {
                        Ima_Adm = I.Ima_Adm,
                        Ima_Med = I.Ima_Med,
                        Ima_Pac = I.Ima_Pac,
                        Ima_Nota = I.Ima_Nota,
                        Ima_Fecha = I.Ima_Fecha,
                        Pac_Categoria = I.Pac_Categoria,
                        Pac_Usr_Web = cadena,
                        Ima_Ruta = I.Ima_Ruta
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = URLAPIConexion.TrimEnd('/') + "/api/Images/InsertImage";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<int>(jsonResponse);

                        return result;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        public async Task<List<CXN_IMAGENES>> GetImages(int pacid, string cadena)
        {
            try
            {
                string URLAPIConexion = ControladorConfiguracion.URLAPIConexion;

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    var loginData = new
                    {
                        PacId = pacid,
                        Cadena = cadena,
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = URLAPIConexion.TrimEnd('/') + "/api/Images/getImages";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<CXN_IMAGENES>>(jsonResponse);

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<string> ViewImage(string ruta)
        {
            try
            {
                string URLAPIConexion = ControladorConfiguracion.URLAPIConexion;

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    var loginData = new
                    {
                        Ruta = ruta
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = URLAPIConexion.TrimEnd('/') + "/api/Images/ViewImage";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<string>(jsonResponse);

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<List<CXN_IMAGENES>> getImagesByAdmition(int admision, string cadena)
        {
            try
            {
                string URLAPIConexion = ControladorConfiguracion.URLAPIConexion;

                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    var loginData = new
                    {
                        Admision = admision,
                        Cadena = cadena,
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = URLAPIConexion.TrimEnd('/') + "/api/Images/getImagesByAdmition";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<CXN_IMAGENES>>(jsonResponse);

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
