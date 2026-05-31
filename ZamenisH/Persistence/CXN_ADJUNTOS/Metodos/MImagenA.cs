using Persistence.CXN_ADJUNTOS.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN_ADJUNTOS;
using Domain;

namespace Persistence.CXN_ADJUNTOS.Metodos
{
    public class MImagenA : IImagenA
    {
        List<byte[]> IImagenA.getImagenes(DateTime Desde, DateTime Hasta, string Carpeta)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                List<PDF> I = new List<PDF>();

                using (SqlConnection con2 = new SqlConnection(getData["ConexionAdjuntos"]))
                {
                    if (con2 != null && con2.State == ConnectionState.Closed)
                    {
                        con2.Open();
                    }

                    String Cargar_HoraIMA = "SELECT Imagen, Archivo  " +
                                            "FROM IMAGES " +
                                            "WHERE Fecha BETWEEN @param1 AND @param2 " +
                                            "AND Carpeta = @param3";

                    using (SqlCommand Carga_CommandIMA = new SqlCommand(Cargar_HoraIMA, con2))
                    {
                        Carga_CommandIMA.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Desde.Date;
                        Carga_CommandIMA.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Hasta.Date;
                        Carga_CommandIMA.Parameters.Add(new SqlParameter("@param3", Carpeta));

                        using (SqlDataReader Lectura_HoraIMA = (Carga_CommandIMA.ExecuteReader()))
                        {
                            List<byte[]> Im = new List<byte[]>();

                            if (Lectura_HoraIMA.HasRows)
                            {
                                while (Lectura_HoraIMA.Read() == true)
                                {
                                    Im.Add((byte[])Lectura_HoraIMA["Imagen"]);
                                }                                

                                return Im;
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        List<string> IImagenA.getImagenesCarpetas(DateTime Desde, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                List<string> I = new List<string>();

                using (SqlConnection con2 = new SqlConnection(getData["ConexionAdjuntos"]))
                {
                    if (con2 != null && con2.State == ConnectionState.Closed)
                    {
                        con2.Open();
                    }

                    String Cargar_HoraIMA = "SELECT DISTINCT Carpeta " +
                                            "FROM IMAGES " +
                                            "WHERE Fecha BETWEEN @param1 AND @param2";

                    using (SqlCommand Carga_CommandIMA = new SqlCommand(Cargar_HoraIMA, con2))
                    {
                        Carga_CommandIMA.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Desde.Date;
                        Carga_CommandIMA.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Hasta.Date;

                        using (SqlDataReader Lectura_HoraIMA = (Carga_CommandIMA.ExecuteReader()))
                        {
                            List<string> Im = new List<string>();

                            if (Lectura_HoraIMA.HasRows)
                            {
                                while (Lectura_HoraIMA.Read() == true)
                                {
                                    Im.Add(Lectura_HoraIMA["Carpeta"].ToString());
                                }

                                return Im;
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        List<string> IImagenA.getImagenesCarpetasPDF(DateTime Desde, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                List<string> I = new List<string>();

                using (SqlConnection con2 = new SqlConnection(getData["ConexionAdjuntos"]))
                {
                    if (con2 != null && con2.State == ConnectionState.Closed)
                    {
                        con2.Open();
                    }

                    String Cargar_HoraIMA = "SELECT DISTINCT Carpeta " +
                                            "FROM PDF " +
                                            "WHERE Fecha BETWEEN @param1 AND @param2";

                    using (SqlCommand Carga_CommandIMA = new SqlCommand(Cargar_HoraIMA, con2))
                    {
                        Carga_CommandIMA.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Desde.Date;
                        Carga_CommandIMA.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Hasta.Date;

                        using (SqlDataReader Lectura_HoraIMA = (Carga_CommandIMA.ExecuteReader()))
                        {
                            List<string> Im = new List<string>();

                            if (Lectura_HoraIMA.HasRows)
                            {
                                while (Lectura_HoraIMA.Read() == true)
                                {
                                    Im.Add(Lectura_HoraIMA["Carpeta"].ToString());
                                }

                                return Im;
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        byte[] IImagenA.getPDF(DateTime Desde, DateTime Hasta, string Carpeta)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["ConexionAdjuntos"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Imagen  " +
                                   "FROM PDF " +
                                   "WHERE Fecha BETWEEN @param1 AND @param2 " +
                                   "AND Carpeta = @param3";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Desde.Date;
                        Commando.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Hasta.Date;
                        Commando.Parameters.AddWithValue("@param3", Carpeta);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return (byte[])Reader["Imagen"];
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
