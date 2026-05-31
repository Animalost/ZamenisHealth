using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MMedidasHerida : IMedidasHerida
    {
        List<CXN_HCMED> IMedidasHerida.Carga_Med(int Admision)
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

                    String Cargar_Hora = "Select * " +
                                         "FROM CXN_HCMED " +
                                         "WHERE Med_Adm = '" + Admision + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_HCMED> L = new List<CXN_HCMED>();

                        while (Lectura_Hora.Read() == true)
                        {
                            L.Add(new CXN_HCMED
                            {
                                Med_Id = Convert.ToInt32(Lectura_Hora["Med_Id"]),
                                Med_Adm = Convert.ToInt32(Lectura_Hora["Med_Adm"]),
                                Med_Largo = Lectura_Hora["Med_Largo"].ToString(),
                                Med_Ancho = Lectura_Hora["Med_Ancho"].ToString(),
                                Med_Profundidad = Lectura_Hora["Med_Profundidad"].ToString(),
                                Med_Determinacion = Lectura_Hora["Med_Determinacion"].ToString(),
                                Med_Observacion = Lectura_Hora["Med_Observacion"].ToString()
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

        bool IMedidasHerida.insertarHerida(CXN_HCMED H)
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

                    DateTime Hoy = DateTime.Now;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HCMED (Med_Adm, " + //param1
                                                          "Med_Fecha, " + //param2
                                                          "Med_Largo, " + //param3
                                                          "MEd_Ancho, " + //param4
                                                          "Med_Determinacion, " + //param5
                                                          "Med_Observacion, " + //param6
                                                          "Med_Pac, " + //param7
                                                          "Med_Profundidad) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Ase
                                                          "@param7, " + // Hor_Pac_Cup
                                                          "@param8)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", H.Med_Adm);
                    cmd.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Hoy; // Fecha que graba cita
                    cmd.Parameters.AddWithValue("@param3", H.Med_Largo);
                    cmd.Parameters.AddWithValue("@param4", H.Med_Ancho);
                    cmd.Parameters.AddWithValue("@param5", H.Med_Determinacion);
                    cmd.Parameters.AddWithValue("@param6", H.Med_Observacion);
                    cmd.Parameters.AddWithValue("@param7", H.Med_Pac);
                    cmd.Parameters.AddWithValue("@param8", H.Med_Profundidad);
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

        void IMedidasHerida.deleteMedida(int Posision)
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

                    string Busqueda3 = "DELETE FROM CXN_HCMED " +
                                      "WHERE Med_Id = '" + Posision + "'";
                    SqlCommand Accion3 = new SqlCommand(Busqueda3, con);
                    int Guarda3;
                    Guarda3 = Accion3.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }

    }
}
