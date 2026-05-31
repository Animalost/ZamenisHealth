using APIFhir.Clases;
using APIFhir.Controlador;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIFhir.Servicio
{
    public class EndPoint_Token : CreateToken
    {
        private readonly IFHIR _context = new MFHIR();

        async Task<string> CreateToken.ObtenerTokenIHCE(int Prestador)
        {
            try
            {
                Dictionary<string, string> datos = getDataMinSalud(Prestador);
                if (datos == null)
                {
                    throw new Exception("No se encontraron datos de conexión para el prestador especificado.");
                }

                string tenantId = datos["TenantID"];
                string urlToken = datos["URLToken"];
                var url = $"{urlToken}/{tenantId}/oauth2/v2.0/token";

                var client = new HttpClient();

                var form = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", datos["ClientID"] },
                    { "client_secret", datos["ClientSecret"] },
                    { "scope", datos["Scope"] }
                };

                var content = new FormUrlEncodedContent(form);

                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var token = JsonSerializer.Deserialize<TokenResponse>(json);

                CXN_TOKENS_FHIR T = new CXN_TOKENS_FHIR
                {
                    Token = token.access_token,
                    Fecha = DateTime.Now,
                    Prestador = Prestador
                };

                _context.InsertarToken(T);

                return token.access_token;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el token: " + ex.Message);
            }            
        }
        Dictionary<string, string> CreateToken.getDataMinSalud(int Prestador)
        {
            return getDataMinSalud(Prestador);
        }
        Dictionary<string, string> getDataMinSalud(int Prestador)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_DATOS_FHIR " +
                                         "WHERE Prestador = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                Dictionary<string, string> s = new Dictionary<string, string>();

                                while (Lectura_Hora.Read())
                                {
                                    s.Add(Lectura_Hora["Llave"].ToString(), Lectura_Hora["Valor"].ToString());
                                }                                

                                return s;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los datos de conexión: " + ex.Message);
            }
        }
    }
}
