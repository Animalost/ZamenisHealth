using Domain.CXN;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class LoginService
    {
        public async Task<bool> Loguear(string URLAPIConexion, string User, string Cadena) //Loguea al usuario y retorna el objeto CXN_LOGIN Imagenes
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

        public async Task<CXN_LOGIN> Loguear(string URLAPIConexion, string User, string Pass, string Ciudad) //NO MOVER
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
                        Password = Pass.ToString().Trim(),
                        Ciudad = Ciudad.ToUpper().Trim()
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = URLAPIConexion.TrimEnd('/') + "/api/auth/login";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<CXN_LOGIN>(jsonResponse);

                        if (result != null)
                        {
                            ControladorConfiguracion.Token = result.Token;  // Guardar el token
                            return result.cXN_LOGIN;  // Retorna el usuario
                        }

                        return null;
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

        public async Task<CXN_LOGIN> GetUser(string URLAPIConexion, string User)  //NO MODIFICAR
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
                        Username = User.ToUpper().Trim(),
                        Password = ""
                    };

                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = URLAPIConexion.TrimEnd('/') + "/api/auth/GetUser";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<CXN_LOGIN>(jsonResponse);

                        if (result != null)
                        {
                            return result.cXN_LOGIN;
                        }

                        return null;
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

        public async Task<List<string>> ListaUsuariosActivos() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<string>>(null, "/api/Usuarios/ListaUsuariosActivos", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<bool> changeClave(string User, string Pass) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Username = User.ToUpper().Trim(),
                    Password = Pass
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Usuarios/changeClave", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> Authorization(string key, string iv) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    KeyRequest = key,
                    IVRequest = iv
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/auth/Authorization", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        
        public async Task<bool> LogoutAsync() //HECHO
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);

            string endpoint = ControladorConfiguracion.URLAPIConexion.TrimEnd('/') + "/api/auth/LogOut";
            HttpResponseMessage response = await client.PostAsync(endpoint, null).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> saveAvatar(byte[] _avatar, string user) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Username = user,
                    Logo = _avatar
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Usuarios/saveAvatar", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> AcceptTyC(string user) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Username = user,
                    Password = ""
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Usuarios/AcceptTyC", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<int> ActualizarMisDatos(CXN_LOGIN L) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Celular = L.Log_Celular,
                    Identificacion = L.Log_Identificacion,
                    Email = L.Log_Email,
                    Usuario = L.Log_Usuario
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Usuarios/ActualizarMisDatos", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
    }
}
