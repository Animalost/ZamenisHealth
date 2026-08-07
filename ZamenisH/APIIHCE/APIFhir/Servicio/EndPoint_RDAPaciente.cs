using APIFhir.Controlador;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APIFhir.Servicio
{
    public class EndPoint_RDAPaciente : EnviarRDAPaciente
    {
        private readonly IFHIR _context = new MFHIR();
        private readonly CreateToken _contextToken = new EndPoint_Token();
        private readonly HttpClient _httpClient;

        public EndPoint_RDAPaciente()
        {
            _httpClient = new HttpClient();            
        }

        async Task<(string Resp, string Est)> EnviarRDAPaciente.SendBundleAsync(string json, int Prestador, string TipoRDA)
        {
            try
            {
                string token = _context.RecuperarToken(Prestador);
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("No se pudo recuperar el token de autenticación.");
                }

                var dataUrl = _contextToken.getDataMinSalud(Prestador);  

                string endpointUrl = "";
                StringContent content = null;

                if (TipoRDA == "Paciente")
                {
                    endpointUrl = dataUrl["URLRDAPaciente"];
                    content = new StringContent(json, Encoding.UTF8, "application/fhir+json");
                }
                else if (TipoRDA == "CExterna")
                {
                    _httpClient.DefaultRequestHeaders.Clear();

                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/fhir+json");
                   // _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "PostmanRuntime/7.43.4");
                   // _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
                   // _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                   // _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                   // _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");

                    endpointUrl = dataUrl["URLRDACExterna"];
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    return ($"Error al enviar el Bundle: RDA No Definido", "ERROR RDA");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PostAsync(endpointUrl, content);

                if (TipoRDA == "Paciente")
                {
                    try
                    {
                        var responseBodyBytes = await response.Content.ReadAsByteArrayAsync();
                        string resultado = "";

                        using (var input = new MemoryStream(responseBodyBytes))
                        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                        using (var reader = new StreamReader(gzip, Encoding.UTF8))
                        {
                            resultado = reader.ReadToEnd();

                            if (response.IsSuccessStatusCode)
                            {
                                return (resultado, "OK");
                            }
                            else
                            {
                                return (resultado, "ERR");
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.Message);
                        string responseBody = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            return (responseBody, "OK");
                        }
                        else
                        {
                            return (responseBody, "ERR");
                        }
                    }
                }
                else if (TipoRDA == "CExterna")
                {
                    try
                    {
                        var responseBodyBytes = await response.Content.ReadAsByteArrayAsync();
                        string resultado = "";

                        using (var input = new MemoryStream(responseBodyBytes))
                        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                        using (var reader = new StreamReader(gzip, Encoding.UTF8))
                        {
                            resultado = reader.ReadToEnd();

                            if (response.IsSuccessStatusCode)
                            {
                                return (resultado, "OK");
                            }                            
                            else
                            {
                                return (resultado, "ERR");
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.Message);
                        string responseBody = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            return (responseBody, "OK");
                        }
                        else
                        {
                            return (responseBody, "ERR");
                        }
                    }                                    
                } 
                else
                {
                    return ("ERROR DE RESPUESTA RDA CONSULTA EXTERNA", "ERROR RDA");
                }
            }
            catch (Exception ex)
            {
                return ($"Error al enviar el Bundle: {ex.Message}", "Exception");
            }            
        }
    }
}
