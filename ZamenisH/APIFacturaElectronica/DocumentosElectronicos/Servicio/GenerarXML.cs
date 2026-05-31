using DocumentosElectronicos.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DocumentosElectronicos.Request.ResponseAPI;

namespace DocumentosElectronicos.Servicio
{
    public class GenerarXML
    {
        public async Task<XMLResponseF1> SendXML(RequestRecibidoFElectronDecodificado jsonToSend, 
                                                 string UrlAPI)
        {
            XMLResponseF1 result;
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                string endpoint = UrlAPI + "/api/FacturacionElectronica/RadicarXML";

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    // Encabezados
                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "PostmanRuntime/7.43.4");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", jsonToSend.ClaveTecnica);

                    client.Timeout = TimeSpan.FromSeconds(30);

                    var loginData = new
                    {
                        TipoFactura = jsonToSend.TipoFactura,
                        Contraseña = jsonToSend.Contraseña,
                        Usuario = jsonToSend.Usuario,
                        Factura = jsonToSend.Factura,
                        ClaveTecnica = jsonToSend.ClaveTecnica
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

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        
                        try
                        {
                            var jsonr = JObject.Parse(jsonResponse);

                            if (jsonr.TryGetValue("statusCode", out JToken valor))
                            {
                                string statusCode = valor.ToString();
                                string cufeRecibido = "";
                                string XmlRecibido = "";
                                string errorRecibido = "";

                                if (statusCode == "00")
                                {
                                    XmlRecibido = (string)jsonr["xml"];

                                    if (jsonr.ContainsKey("cufe"))
                                    {
                                        cufeRecibido = (string)jsonr["cufe"];
                                        
                                        result = new XMLResponseF1
                                        {   
                                            StatusCode = statusCode,
                                            cufe = cufeRecibido,
                                            xml = XmlRecibido,
                                            error = ""
                                        };
                                    }
                                    else
                                    {
                                        result = new XMLResponseF1
                                        {
                                            StatusCode = statusCode,
                                            cufe = cufeRecibido,
                                            xml = XmlRecibido,
                                            error = "No hay cufe"
                                        };
                                    }                                    
                                }
                                else
                                {
                                    errorRecibido = (string)jsonr["error"];

                                    result = new XMLResponseF1
                                    {
                                        StatusCode = statusCode,
                                        cufe = "",
                                        xml = "",
                                        error = errorRecibido
                                    };
                                }
                            }
                            else
                            {
                                result = new XMLResponseF1
                                {
                                    StatusCode = "",
                                    cufe = "",
                                    xml = "",
                                    error = "Error Front Desconocido"
                                };
                            }
                        }
                        catch (Exception ex) 
                        {
                            result = new XMLResponseF1
                            {
                                StatusCode = "",
                                cufe = "",
                                xml = "",
                                error = "Error Front: " + ex.Message
                            };
                        }
                    }
                    else
                    {
                        result = new XMLResponseF1
                        {
                            StatusCode = "",
                            cufe = "",
                            xml = "",
                            error = "Error Front: No se recibio respuesta"
                        };
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                result = new XMLResponseF1
                {
                    StatusCode = "",
                    cufe = "",
                    xml = "",
                    error = "Exception Front: " + ex.Message
                };

                return result;
            }
        }
    }
}
