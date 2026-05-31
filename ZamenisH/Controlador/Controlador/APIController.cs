using Controlador.Clases;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net;
using Domain.Licence;

namespace Controlador
{
    public class APIController
    {
        public static string TypeEndPoint { get; set; }

        public static async Task<T> SendMessageToAPI<T>(object parameters, string endpoint, bool encryptData = true)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    if (TypeEndPoint != "GET")
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    }

                    // Encripta los datos si encryptData es verdadero                                       
                    string json = null;
                    StringContent content = null;

                    if (parameters != null)
                    {
                        if (encryptData)
                        {
                            // Convertimos el objeto en JSON y reemplazamos los null por cadenas vacías
                            var cleanedParameters = JsonConvert.DeserializeObject<dynamic>(
                                JsonConvert.SerializeObject(parameters, new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Include // Asegura que los valores null se representen en el JSON
                                })
                            );

                            var encryptedData = EncryptObject(parameters);
                            json = JsonConvert.SerializeObject(new { Data = encryptedData });
                        }
                        else
                        {
                            json = JsonConvert.SerializeObject(parameters);
                        }

                        content = new StringContent(json, Encoding.UTF8, "application/json");
                    }

                    // Construye la URL final
                    string fullUrl = ControladorConfiguracion.URLAPIConexion.TrimEnd('/') + endpoint;
                    HttpResponseMessage response = null;

                    if (TypeEndPoint == "PATCH")
                    {
                        response = await client.PatchAsync(fullUrl, content).ConfigureAwait(false);
                    }
                    if (TypeEndPoint == "POST")
                    {
                        response = await client.PostAsync(fullUrl, content).ConfigureAwait(false);
                    }
                    if (TypeEndPoint == "GET")
                    {
                        response = await client.GetAsync(fullUrl).ConfigureAwait(false);
                    }


                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        // Desencripta la respuesta si encryptData es verdadero
                        /*if (encryptData)
                        {
                            jsonResponse = Decrypt(jsonResponse);
                        }*/

                        try
                        {
                            // Intentar convertir al tipo especificado
                            return JsonConvert.DeserializeObject<T>(jsonResponse);
                        }
                        catch
                        {
                            try
                            {
                                // Si el tipo es un string, devolverlo directamente
                                if (typeof(T) == typeof(string))
                                {
                                    return (T)(object)jsonResponse;
                                }

                                // Si es un número, intentar convertirlo
                                if (typeof(T) == typeof(int))
                                {
                                    return (T)(object)int.Parse(jsonResponse);
                                }
                                if (typeof(T) == typeof(double))
                                {
                                    return (T)(object)double.Parse(jsonResponse);
                                }
                                if (typeof(T) == typeof(bool))
                                {
                                    return (T)(object)bool.Parse(jsonResponse);
                                }

                                // Si no se puede convertir, devolver `default(T)`
                                return default;
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    }

                    return default;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return default;
            }
        }
        public static async Task<bool> CreateAuthorization(string key, string iv, string endpoint)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                                     
                    var datos = new
                    {
                        KeyRequest = key,
                        IVRequest = iv
                    };

                    string json = JsonConvert.SerializeObject(datos);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    string fullUrl = ControladorConfiguracion.URLAPIConexion.TrimEnd('/') + endpoint;
                    HttpResponseMessage response = await client.PostAsync(fullUrl, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.OK) 
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }                      
                    }
                }

                return false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public static async Task<ZHealth> GetLicence(string serial, string endpoint, string UrlBase)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var datos = new
                    {
                        Serial = serial
                    };

                    string json = JsonConvert.SerializeObject(datos);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    string fullUrl = UrlBase.TrimEnd('/') + endpoint;
                    HttpResponseMessage response = await client.PostAsync(fullUrl, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            
                            return JsonConvert.DeserializeObject<ZHealth>(jsonResponse);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                return null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        static string EncryptObject(object obj)
        {
            if (obj == null) return "☺"; // Evita errores si el objeto completo es null

            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include
            });

            return AESHelper.Encrypt(json);
        }
        static string Decrypt(string cipherText)
        {
            if (cipherText == null || cipherText == "") { return ""; }

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AESHelper.Key);
                aes.IV = Encoding.UTF8.GetBytes(AESHelper.IV);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] inputBytes = Convert.FromBase64String(cipherText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
