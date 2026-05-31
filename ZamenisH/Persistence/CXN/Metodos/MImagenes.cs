using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MImagenes : IImagenes
    {
        List<CXN_HORARIO> IImagenes.getHistorias(string TID, string IDD)
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

                    String Cargar_Hora = "SELECT H.Hor_Id, H.Hor_Pac_Fecha_Cita, H.Hor_Imp_Age, H.Hor_Pac_Id, B.Bod_Responsable " +
                                         "FROM CXN_PACIENTES P " +
                                         "INNER JOIN CXN_HORARIO H ON P.Pac_Id = H.Hor_Pac_Id " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "WHERE P.Pac_IdNum = '" + IDD + "' " +
                                         "AND H.Hor_Estado <> 'C' " +
                                         "ORDER BY H.Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_Pac_Id = Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]),
                                        Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString()
                                    });
                                }

                                return H;
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
        List<CXN_IMAGENES> IImagenes.getImagenesByAdmition(int Admision)
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
                                         "FROM CXN_IMAGENES " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_IMAGENES.Ima_Med = CXN_BODEGAS.Bod_Numero " +
                                         "WHERE Ima_Adm = @param1 " +
                                         "ORDER BY Ima_Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_IMAGENES> H = new List<CXN_IMAGENES>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    H.Add(new CXN_IMAGENES
                                    {
                                        Ima_Adm = Convert.ToInt32(Lectura_Hora["Ima_Adm"]),
                                        Ima_Med = Convert.ToInt32(Lectura_Hora["Ima_Med"]),
                                        Ima_Id = Convert.ToInt32(Lectura_Hora["Ima_Id"]),
                                        Ima_Fecha = Convert.ToDateTime(Lectura_Hora["Ima_Fecha"]),
                                        Ima_Ruta = Lectura_Hora["Ima_Ruta"].ToString(),
                                        Pac_PrimerA = Lectura_Hora["Bod_Responsable"].ToString()
                                    });
                                }

                                return H;
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
