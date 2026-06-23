using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MPlantilla : IPlantilla
    {
        List<CXN_PLANTILLA> IPlantilla.getPlantillas(string user)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String CONV2 = "SELECT * FROM CXN_PLANTILLA where Pla_Usr like '" + user + "'";
                    SqlCommand Com_CONV2 = new SqlCommand(CONV2, con);
                    SqlDataReader Lec_CONV2 = (Com_CONV2.ExecuteReader());
                    if (Lec_CONV2.HasRows)
                    {
                        List<CXN_PLANTILLA> L = new List<CXN_PLANTILLA>();

                        while (Lec_CONV2.Read() == true)
                        {
                            L.Add(new CXN_PLANTILLA
                            {
                                Pla_Nota = Lec_CONV2["Pla_Nota"].ToString(),
                                Pla_Observa = Lec_CONV2["Pla_Observa"].ToString(),
                                Pla_Recomienda = Lec_CONV2["Pla_Recomienda"].ToString()
                            });
                        }

                        return L;
                    }

                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        bool IPlantilla.savePlantillas(CXN_PLANTILLA P)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PLANTILLA (Pla_Usr, " + //param1
                                                               "Pla_Nota, " + //param2
                                                               "Pla_Observa, " + //param3
                                                               "Pla_Recomienda) " + //param16
                                      "values                  (@param1, " + // Hor_Estado
                                                               "@param2, " + // Hor_Pac_Id
                                                               "@param3, " + // Hor_Pac_Bod
                                                               "@param4)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", P.Pla_Usr);
                    cmd.Parameters.AddWithValue("@param2", P.Pla_Nota);
                    cmd.Parameters.AddWithValue("@param3", P.Pla_Observa);
                    cmd.Parameters.AddWithValue("@param4", P.Pla_Recomienda);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
    }
}
