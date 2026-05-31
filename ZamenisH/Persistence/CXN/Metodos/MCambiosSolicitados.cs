using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MCambiosSolicitados : ICambiosSolicitados
    {
        void ICambiosSolicitados.InsertSolicitud(CXN_CAMBIOSSOLICITADOS B)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CAMBIOSSOLICITADOS " +
                                                             "(CupComplejidad, " +
                                                             "ServComplejidad, " +
                                                             "Solicitante, " +
                                                             "Fecha, " +
                                                             "Estado, " +
                                                             "Motivo, " +
                                                             "AdmisionOfertante) " +
                                         "values             (@param1, " +
                                                             "@param2, " +
                                                             "@param3, " +
                                                             "@param4, " +
                                                             "@param5, " +
                                                             "@param6, " +
                                                             "@param7)", con);

                    cmd.Parameters.AddWithValue("@param1", B.CupComplejidad);
                    cmd.Parameters.AddWithValue("@param2", B.ServComplejidad);
                    cmd.Parameters.AddWithValue("@param3", B.Solicitante);
                    cmd.Parameters.AddWithValue("@param4", Convert.ToDateTime(B.Fecha));
                    cmd.Parameters.AddWithValue("@param5", B.Estado);
                    cmd.Parameters.AddWithValue("@param6", B.Motivo);
                    cmd.Parameters.AddWithValue("@param7", B.AdmisionOfertante);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        List<CXN_CAMBIOSSOLICITADOS> ICambiosSolicitados.GetCambios()
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
                                         "FROM CXN_CAMBIOSSOLICITADOS " +
                                         "WHERE Estado = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", "P");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_CAMBIOSSOLICITADOS> C = new List<CXN_CAMBIOSSOLICITADOS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_CAMBIOSSOLICITADOS
                                    {
                                        CupComplejidad = Lectura_Hora["CupComplejidad"].ToString(),
                                        Estado = Lectura_Hora["Estado"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        ServComplejidad = Lectura_Hora["ServComplejidad"].ToString(),
                                        Solicitante = Lectura_Hora["Solicitante"].ToString(),
                                        Motivo = Lectura_Hora["Motivo"].ToString(),
                                        AdmisionOfertante = Convert.ToInt32(Lectura_Hora["AdmisionOfertante"])
                                    });
                                }

                                return C;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }                   
                }
            }
            catch
            {
                return null;
            }
        }
        CXN_CAMBIOSSOLICITADOS ICambiosSolicitados.GetSolicitud(int Posision)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_CAMBIOSSOLICITADOS " +
                                         "WHERE Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Posision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_CAMBIOSSOLICITADOS C = new CXN_CAMBIOSSOLICITADOS
                                {
                                    CupComplejidad = Lectura_Hora["CupComplejidad"].ToString(),
                                    Estado = Lectura_Hora["Estado"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                    ServComplejidad = Lectura_Hora["ServComplejidad"].ToString(),
                                    Solicitante = Lectura_Hora["Solicitante"].ToString(),
                                    Motivo = Lectura_Hora["Motivo"].ToString(),
                                    AdmisionOfertante = Convert.ToInt32(Lectura_Hora["AdmisionOfertante"])
                                };

                                return C;
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        void ICambiosSolicitados.CambiarEstadoSolicitud(int Id, string Estado)
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

                    string Busqueda = "UPDATE CXN_CAMBIOSSOLICITADOS " +
                                      "SET Estado = '" + Estado + "' " +
                                      "WHERE Id = '" + Id + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        (string DX1, string DX2, string DX3) ICambiosSolicitados.getDX(int admision)
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

                    String Cargar_Hora = "SELECT Car_Dx1, Car_Dx2, Car_Dx3 " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Adm_Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return (Lectura_Hora["Car_Dx1"].ToString(),
                                        Lectura_Hora["Car_Dx2"].ToString(),
                                        Lectura_Hora["Car_Dx3"].ToString());
                            }
                            else
                            {
                                return (null,null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, null, null);
            }
        }
    }
}
