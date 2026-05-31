using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MHelisa : IHelisa
    {
        private static readonly IGenerales repoGenerales = new MGenerales();
        Dictionary<string, string> IHelisa.Claves(string nameClient, int Prestador) 
        {
            try
            {
                var getConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    Dictionary<string, string> DIC = new Dictionary<string, string>();

                    String Query = "SELECT TOP 1 * " +
                                   "FROM WS_CLIENTS " +
                                   "WHERE WS_Cliente = @param1 " +
                                   "AND WS_IdPrestador = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", nameClient);
                        Commando.Parameters.AddWithValue("@param2", Prestador);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                if (nameClient == "Helisa")
                                {
                                    DIC.Add("User", repoGenerales.Base64Decode(Reader["WS_User"].ToString()));
                                    DIC.Add("Pass", repoGenerales.Base64Decode(Reader["WS_Pass"].ToString()));
                                    DIC.Add("Company", repoGenerales.Base64Decode(Reader["WS_Company"].ToString()));
                                    DIC.Add("Client", repoGenerales.Base64Decode(Reader["WS_ClientCode"].ToString()));
                                    return DIC;
                                }
                                else if (nameClient == "ShortLinks")
                                {
                                    DIC.Add("User", repoGenerales.Base64Decode(Reader["WS_User"].ToString()));
                                    DIC.Add("Pass", repoGenerales.Base64Decode(Reader["WS_Pass"].ToString()));
                                    DIC.Add("Code", Reader["WS_ClientCode"].ToString());
                                    return DIC;
                                }
                                else if (nameClient == "MinSalud")
                                {
                                    DIC.Add("User", repoGenerales.Base64Decode(Reader["WS_User"].ToString()));
                                    DIC.Add("Pass", repoGenerales.Base64Decode(Reader["WS_Pass"].ToString()));
                                    DIC.Add("Nit", repoGenerales.Base64Decode(Reader["WS_Company"].ToString()));
                                    DIC.Add("Tipo", repoGenerales.Base64Decode(Reader["WS_Clientcode"].ToString()));
                                    return DIC;
                                }
                                else if (nameClient == "Factura1Token" || nameClient == "Factura1XML" || nameClient == "Factura1PDF")
                                {
                                    DIC.Add("User", Reader["WS_User"].ToString());
                                    DIC.Add("Pass", Reader["WS_Pass"].ToString());
                                    DIC.Add("Url", Reader["WS_ClientCode"].ToString());
                                    DIC.Add("Tipo", Reader["WS_Company"].ToString());
                                    return DIC;
                                }
                                else if (nameClient == "ServerRTC")
                                {
                                    DIC.Add("User", Reader["WS_User"].ToString());
                                    DIC.Add("Pass", Reader["WS_Pass"].ToString());
                                    DIC.Add("Url", Reader["WS_ClientCode"].ToString());
                                    return DIC;
                                }
                                else if (nameClient == "Perplexity")
                                {
                                    DIC.Add("IAaPIPerplexity", Reader["WS_ClientCode"].ToString());
                                    return DIC;
                                }
                                else
                                {
                                    return null;
                                }                                
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }        
    }
}
