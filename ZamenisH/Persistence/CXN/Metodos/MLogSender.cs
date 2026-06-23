using Persistence.CXN.Interfaces;
using System;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using System.Collections.Generic;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MLogSender : ILogSender
    {     
        void ILogSender.Log(int Admision,
                            string Salida,
                            string Mensaje_SMS,
                            string Celular,
                            string Usuario,
                            string TipoEnvio)
        {

            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Log_Tipo = TipoEnvio;
                DateTime Log_Fecha_Envio = DateTime.Now;
                string Log_Estado = Salida;
                string Log_Usuario = Usuario;
                string Log_Mensaje = Mensaje_SMS;
                string Log_Destinatario = Celular;

                SqlCommand cmd = new SqlCommand(@"Insert into CXN_LOG_SENDER (Log_Tipo, " +
                                                                             "Log_Fecha_Envio, " +
                                                                             "Log_Estado, " +
                                                                             "Log_Usuario, " +
                                                                             "Log_Mensaje, " +
                                                                             "Log_Destinatario, " +
                                                                             "Log_Admision) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6, " +
                                                          "@param7)", con);

                cmd.Parameters.AddWithValue("@param1", Log_Tipo);
                cmd.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Log_Fecha_Envio;
                cmd.Parameters.AddWithValue("@param3", Log_Estado);
                cmd.Parameters.AddWithValue("@param4", Log_Usuario);
                cmd.Parameters.AddWithValue("@param5", Log_Mensaje);
                cmd.Parameters.AddWithValue("@param6", Log_Destinatario);
                cmd.Parameters.AddWithValue("@param7", Admision);
                cmd.ExecuteNonQuery();
            }

        }

        void ILogSender.GrabaSQL_Evidencia(CXN_LOG_SENDER S)
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
                    DateTime Hoyy = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_LOG_SENDER (Log_Fecha_Envio, " + //param1
                                                          "Log_Estado, " + //param2
                                                          "Log_Usuario, " + //param3
                                                          "Log_Mensaje, " + //param4
                                                          "Log_Destinatario, " + //param5
                                                          "Log_Admision, " + //param6
                                                          "Log_Tipo) " + //param7
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Cia
                                                          "@param7)", con); // Hor_Pac_Sal

                    cmd.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = S.Log_Fecha_Envio;
                    cmd.Parameters.AddWithValue("@param2", S.Log_Estado);
                    cmd.Parameters.AddWithValue("@param3", S.Log_Usuario);
                    cmd.Parameters.AddWithValue("@param4", S.Log_Mensaje);
                    cmd.Parameters.AddWithValue("@param5", S.Log_Destinatario);
                    cmd.Parameters.AddWithValue("@param6", S.Log_Admision);
                    cmd.Parameters.AddWithValue("@param7", S.Log_Tipo);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<CXN_LOG_SENDER> ILogSender.Historial(DateTime Desde, DateTime Hasta)
        {
            try
            {
                var getCon = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_LOG_SENDER " +
                                   "WHERE Log_Fecha_Envio BETWEEN '" + Convert.ToDateTime(Desde).ToString(getCon["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getCon["Format_Fecha"]) + "' " +
                                   "ORDER BY Log_Fecha_Envio ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_LOG_SENDER> L = new List<CXN_LOG_SENDER>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_LOG_SENDER
                            {
                                Log_Fecha_Envio = Convert.ToDateTime(Reader["Log_Fecha_Envio"]),
                                Log_Estado = Reader["Log_Estado"].ToString(),
                                Log_Mensaje = Reader["Log_Mensaje"].ToString(),
                                Log_Destinatario = Reader["Log_Destinatario"].ToString()
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
        List<CXN_LOG_SENDER> ILogSender.Historial(DateTime Desde, DateTime Hasta, string Criterio)
        {
            try
            {
                var getCon = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_LOG_SENDER " +
                                   "WHERE Log_Fecha_Envio BETWEEN '" + Convert.ToDateTime(Desde).ToString(getCon["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getCon["Format_Fecha"]) + "' " +
                                   "AND Log_Destinatario LIKE '%" + Criterio + "%'" +
                                   "ORDER BY Log_Fecha_Envio ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_LOG_SENDER> L = new List<CXN_LOG_SENDER>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_LOG_SENDER
                            {
                                Log_Fecha_Envio = Convert.ToDateTime(Reader["Log_Fecha_Envio"]),
                                Log_Estado = Reader["Log_Estado"].ToString(),
                                Log_Mensaje = Reader["Log_Mensaje"].ToString(),
                                Log_Destinatario = Reader["Log_Destinatario"].ToString()
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
    }
}
