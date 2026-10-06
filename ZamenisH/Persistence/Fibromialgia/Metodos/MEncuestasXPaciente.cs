using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Persistence.Fibromialgia.Interfaces;
using Domain.Fibromialgia;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MEncuestasXPaciente : IEncuestasXPaciente
    {
        #region Consultar Encuestas
        List<ClaseReportsFibro> IEncuestasXPaciente.GetAllEncuestas(DateTime desde, DateTime hasta, string NumId)
        {
            try
            {
                List<ClaseReportsFibro> E1 = GetAllEncuesta1(desde, hasta, NumId);
                List<ClaseReportsFibro> E2 = GetAllEncuesta2(desde, hasta, NumId);
                List<ClaseReportsFibro> E3 = GetAllEncuesta3(desde, hasta, NumId);

                List<ClaseReportsFibro> Respuesta = new List<ClaseReportsFibro>();

                if (E1 != null)
                    Respuesta.AddRange(E1);

                if (E2 != null)
                    Respuesta.AddRange(E2);

                if (E3 != null)
                    Respuesta.AddRange(E3);

                Respuesta = Respuesta
                    .OrderByDescending(x => x.Fecha1)
                    .ThenByDescending(x => x.Dato2)
                    .ToList();

                return Respuesta;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        List<ClaseReportsFibro> GetAllEncuesta1(DateTime desde, DateTime hasta, string NumId)
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

                    String Cargar_Hora2 = @"SELECT *    
                                          FROM FIB_ENCUESTA1 F 
                                          INNER JOIN CXN_PACIENTES P ON F.IdPaciente = P.Pac_Id 
                                          WHERE P.Pac_IdNum = @param1 
                                          AND F.FechaEncuesta BETWEEN @param2 AND @param3";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", NumId);
                        Carga_Command2.Parameters.AddWithValue("@param2", Convert.ToDateTime(desde).ToString("yyyy-MM-dd"));
                        Carga_Command2.Parameters.AddWithValue("@param3", Convert.ToDateTime(hasta).ToString("yyyy-MM-dd"));

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<ClaseReportsFibro> res = new List<ClaseReportsFibro>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    res.Add(new ClaseReportsFibro
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora2["Id"]),
                                        Fecha1 = Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]),
                                        Dato1 = Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                                Lectura_Hora2["Pac_SegundoA"].ToString() + " " +
                                                Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                                Lectura_Hora2["Pac_SegundoN"].ToString(),
                                        Dato2 = "Encuesta 1",
                                        Dato3 = Lectura_Hora2["UsuarioRegistra"].ToString(),
                                        Dato4 = Lectura_Hora2["Estado"].ToString()
                                    });
                                }

                                return res;
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
        List<ClaseReportsFibro> GetAllEncuesta2(DateTime desde, DateTime hasta, string NumId)
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

                    String Cargar_Hora2 = @"SELECT *    
                                          FROM FIB_ENCUESTA2 F 
                                          INNER JOIN CXN_PACIENTES P ON F.IdPaciente = P.Pac_Id 
                                          WHERE P.Pac_IdNum = @param1 
                                          AND F.FechaEncuesta BETWEEN @param2 AND @param3";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", NumId);
                        Carga_Command2.Parameters.AddWithValue("@param2", Convert.ToDateTime(desde).ToString("yyyy-MM-dd"));
                        Carga_Command2.Parameters.AddWithValue("@param3", Convert.ToDateTime(hasta).ToString("yyyy-MM-dd"));

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<ClaseReportsFibro> res = new List<ClaseReportsFibro>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    res.Add(new ClaseReportsFibro
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora2["Id"]),
                                        Fecha1 = Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]),
                                        Dato1 = Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                                Lectura_Hora2["Pac_SegundoA"].ToString() + " " +
                                                Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                                Lectura_Hora2["Pac_SegundoN"].ToString(),
                                        Dato2 = "Encuesta 2",
                                        Dato3 = Lectura_Hora2["UsuarioRegistra"].ToString(),
                                        Dato4 = Lectura_Hora2["Estado"].ToString()
                                    });
                                }

                                return res;
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
        List<ClaseReportsFibro> GetAllEncuesta3(DateTime desde, DateTime hasta, string NumId)
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

                    String Cargar_Hora2 = @"SELECT *    
                                          FROM FIB_ENCUESTA3 F 
                                          INNER JOIN CXN_PACIENTES P ON F.IdPaciente = P.Pac_Id 
                                          WHERE P.Pac_IdNum = @param1 
                                          AND F.FechaEncuesta BETWEEN @param2 AND @param3";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", NumId);
                        Carga_Command2.Parameters.AddWithValue("@param2", Convert.ToDateTime(desde).ToString("yyyy-MM-dd"));
                        Carga_Command2.Parameters.AddWithValue("@param3", Convert.ToDateTime(hasta).ToString("yyyy-MM-dd"));

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<ClaseReportsFibro> res = new List<ClaseReportsFibro>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    res.Add(new ClaseReportsFibro
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora2["Id"]),
                                        Fecha1 = Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]),
                                        Dato1 = Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                                Lectura_Hora2["Pac_SegundoA"].ToString() + " " +
                                                Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                                Lectura_Hora2["Pac_SegundoN"].ToString(),
                                        Dato2 = "Encuesta 2",
                                        Dato3 = Lectura_Hora2["UsuarioRegistra"].ToString(),
                                        Dato4 = Lectura_Hora2["Estado"].ToString()
                                    });
                                }

                                return res;
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
        bool IEncuestasXPaciente.UpdateStatusEncuesta(int Pos, bool Status, string TEncuesta)
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

                    string Query = "";

                    if (TEncuesta == "Encuesta 1")
                    {
                        Query = "UPDATE FIB_ENCUESTA1 " +
                                "SET Estado = @param1 " +
                                "WHERE Id = @param2";
                    }
                    else if (TEncuesta == "Encuesta 2")
                    {
                        Query = "UPDATE FIB_ENCUESTA2 " +
                                "SET Estado = @param1 " +
                                "WHERE Id = @param2";
                    }
                    else if (TEncuesta == "Encuesta 3")
                    {
                        Query = "UPDATE FIB_ENCUESTA3 " +
                                "SET Estado = @param1 " +
                                "WHERE Id = @param2";
                    }
                    else
                    {
                        return false;
                    }

                    using (SqlCommand Accion = new SqlCommand(Query, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", Status == true ? "V" : "A");
                        Accion.Parameters.AddWithValue("@param2", Pos);

                        return Accion.ExecuteNonQuery() > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        #endregion

        #region INFORME POR PACIENTE CONSOLIDADO DEL CUESTIONARIO SALUD FIQ
        List<int> IEncuestasXPaciente.ObtenerEncuestaXPaciente(DateTime desde, DateTime hasta, string NumId)
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

                    String Cargar_Hora2 = @"SELECT TOP 3 FIB_ENCUESTA1.Id   
                                          FROM FIB_ENCUESTA1 
                                          INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA1.IdPaciente = CXN_PACIENTES.Pac_Id 
                                          WHERE CXN_PACIENTES.Pac_IdNum = @param1 
                                          AND FIB_ENCUESTA1.FechaEncuesta BETWEEN @param2 AND @param3 
                                          AND FIB_ENCUESTA1.Estado = @param4 
                                          ORDER BY FIB_ENCUESTA1.FechaEncuesta ASC";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", NumId);
                        Carga_Command2.Parameters.AddWithValue("@param2", Convert.ToDateTime(desde).ToString("yyyy-MM-dd"));
                        Carga_Command2.Parameters.AddWithValue("@param3", Convert.ToDateTime(hasta).ToString("yyyy-MM-dd"));
                        Carga_Command2.Parameters.AddWithValue("@param4", "V");

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {                           
                            if (Lectura_Hora2.HasRows)
                            {
                                List<int> res = new List<int>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    res.Add(Convert.ToInt32(Lectura_Hora2["Id"]));
                                }

                                return res;
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
        List<int> IEncuestasXPaciente.ObtenerEncuesta2XPaciente(DateTime desde, DateTime hasta, string NumId)
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

                    String Cargar_Hora2 = @"SELECT TOP 3 FIB_ENCUESTA2.Id   
                                          FROM FIB_ENCUESTA2 
                                          INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA2.IdPaciente = CXN_PACIENTES.Pac_Id 
                                          WHERE CXN_PACIENTES.Pac_IdNum = @param1 
                                          AND FIB_ENCUESTA2.FechaEncuesta BETWEEN @param2 AND @param3 
                                          AND FIB_ENCUESTA2.Estado = @param4 
                                          ORDER BY FIB_ENCUESTA2.FechaEncuesta ASC";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", NumId);
                        Carga_Command2.Parameters.AddWithValue("@param2", Convert.ToDateTime(desde).ToString("yyyy-MM-dd"));
                        Carga_Command2.Parameters.AddWithValue("@param3", Convert.ToDateTime(hasta).ToString("yyyy-MM-dd"));
                        Carga_Command2.Parameters.AddWithValue("@param4", "V");


                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {                            
                            if (Lectura_Hora2.HasRows)
                            {
                                List<int> res = new List<int>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    res.Add(Convert.ToInt32(Lectura_Hora2["Id"]));
                                }

                                return res;
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
        ClaseReportsFibro IEncuestasXPaciente.getResultPacE1(int E1)
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
                    String Cargar_Hora2 = @"SELECT  *  
                                          FROM FIB_ENCUESTA1 
                                          INNER JOIN CXN_PACIENTES ON FIB_ENCUESTA1.IdPaciente = CXN_PACIENTES.Pac_Id 
                                          WHERE FIB_ENCUESTA1.Id = @param1";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", E1);

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                //pregunta de 1 a 11
                                int P1A11 = Convert.ToInt32(Lectura_Hora2["Pregunta1"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta2"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta3"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta4"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta5"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta6"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta7"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta8"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta9"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta10"]) +
                                            Convert.ToInt32(Lectura_Hora2["Pregunta11"]);

                                int Pregunta1 = Convert.ToInt32(Lectura_Hora2["Pregunta1"]);
                                int Pregunta2 = Convert.ToInt32(Lectura_Hora2["Pregunta2"]);
                                int Pregunta3 = Convert.ToInt32(Lectura_Hora2["Pregunta3"]);
                                int Pregunta4 = Convert.ToInt32(Lectura_Hora2["Pregunta4"]);
                                int Pregunta5 = Convert.ToInt32(Lectura_Hora2["Pregunta5"]);
                                int Pregunta6 = Convert.ToInt32(Lectura_Hora2["Pregunta6"]);
                                int Pregunta7 = Convert.ToInt32(Lectura_Hora2["Pregunta7"]);
                                int Pregunta8 = Convert.ToInt32(Lectura_Hora2["Pregunta8"]);
                                int Pregunta9 = Convert.ToInt32(Lectura_Hora2["Pregunta9"]);
                                int Pregunta10 = Convert.ToInt32(Lectura_Hora2["Pregunta10"]);
                                int Pregunta11 = Convert.ToInt32(Lectura_Hora2["Pregunta11"]);
                                int Pregunta12_2 = Convert.ToInt32(Lectura_Hora2["Pregunta12"]);

                                //Para suma
                                Double Pregunta1A11 = Convert.ToDouble(P1A11) / 11;
                                int Pregunta12 = 7 - Convert.ToInt32(Lectura_Hora2["Pregunta12"]);
                                int Pregunta13 = Convert.ToInt32(Lectura_Hora2["Pregunta13"]);
                                int Pregunta14 = Convert.ToInt32(Lectura_Hora2["Pregunta14"]);
                                int Pregunta15 = Convert.ToInt32(Lectura_Hora2["Pregunta15"]);
                                int Pregunta16 = Convert.ToInt32(Lectura_Hora2["Pregunta16"]);
                                int Pregunta17 = Convert.ToInt32(Lectura_Hora2["Pregunta17"]);
                                int Pregunta18 = Convert.ToInt32(Lectura_Hora2["Pregunta18"]);
                                int Pregunta19 = Convert.ToInt32(Lectura_Hora2["Pregunta19"]);
                                int Pregunta20 = Convert.ToInt32(Lectura_Hora2["Pregunta20"]);

                                //RESPUESTA FINAL FIQ
                                double res = Pregunta1A11 + Pregunta12 + Pregunta13 + Pregunta14 + Pregunta15 + Pregunta16
                                           + Pregunta17 + Pregunta18 + Pregunta19 + Pregunta20;


                                //Funcion Fisica
                                int P3 = Convert.ToInt32(Lectura_Hora2["Pregunta3"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta3"]) == 2 ? 50 : 100;
                                int P4 = Convert.ToInt32(Lectura_Hora2["Pregunta4"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta4"]) == 2 ? 50 : 100;
                                int P5 = Convert.ToInt32(Lectura_Hora2["Pregunta5"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta5"]) == 2 ? 50 : 100;
                                int P6 = Convert.ToInt32(Lectura_Hora2["Pregunta6"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta6"]) == 2 ? 50 : 100;
                                int P7 = Convert.ToInt32(Lectura_Hora2["Pregunta7"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta7"]) == 2 ? 50 : 100;
                                int P8 = Convert.ToInt32(Lectura_Hora2["Pregunta8"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta8"]) == 2 ? 50 : 100;
                                int P9 = Convert.ToInt32(Lectura_Hora2["Pregunta9"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta9"]) == 2 ? 50 : 100;
                                int P10 = Convert.ToInt32(Lectura_Hora2["Pregunta10"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta10"]) == 2 ? 50 : 100;
                                int P11 = Convert.ToInt32(Lectura_Hora2["Pregunta11"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta11"]) == 2 ? 50 : 100;
                                int P12 = Convert.ToInt32(Lectura_Hora2["Pregunta12"]) == 1 ? 0 :
                                            Convert.ToInt32(Lectura_Hora2["Pregunta12"]) == 2 ? 50 : 100;

                                //RESPUESTA FINAL FUCNION FISICA
                                double FactorFisico = (P3 + P4 + P5 + P6 + P7 + P8 + P9 + P10 + P11 + P12) / 10;

                                //Limitaciones salud fisica
                                int P13 = Convert.ToInt32(Lectura_Hora2["Pregunta13"]) == 1 ? 0 : 100;
                                int P14 = Convert.ToInt32(Lectura_Hora2["Pregunta14"]) == 1 ? 0 : 100;
                                int P15 = Convert.ToInt32(Lectura_Hora2["Pregunta15"]) == 1 ? 0 : 100;
                                int P16 = Convert.ToInt32(Lectura_Hora2["Pregunta16"]) == 1 ? 0 : 100;

                                //RESPUESTA FINAL LIMITACIONES SALUD FISICA
                                double LimitacionesFisicas = (P13 + P14 + P15 + P16) / 4;

                                //Limitaciones salud emocional
                                int P17 = Convert.ToInt32(Lectura_Hora2["Pregunta17"]) == 1 ? 0 : 100;
                                int P18 = Convert.ToInt32(Lectura_Hora2["Pregunta18"]) == 1 ? 0 : 100;
                                int P19 = Convert.ToInt32(Lectura_Hora2["Pregunta19"]) == 1 ? 0 : 100;

                                //RESPUESTA FINAL LIMITACIONES EMOCIONALS
                                double LimitacionesEmocionales = (P17 + P18 + P19) / 3;

                                //Energia/Fatiga
                                int P23 = Convert.ToInt32(Lectura_Hora2["Pregunta23"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta23"]) == 2 ? 80 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta23"]) == 3 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta23"]) == 4 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta23"]) == 5 ? 20 : 0;
                                int P27 = Convert.ToInt32(Lectura_Hora2["Pregunta27"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta27"]) == 2 ? 80 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta27"]) == 3 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta27"]) == 4 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta27"]) == 5 ? 20 : 0;
                                int P29 = Convert.ToInt32(Lectura_Hora2["Pregunta29"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta29"]) == 2 ? 20 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta29"]) == 3 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta29"]) == 4 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta29"]) == 5 ? 80 : 100;
                                int P31 = Convert.ToInt32(Lectura_Hora2["Pregunta31"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta31"]) == 2 ? 20 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta31"]) == 3 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta31"]) == 4 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta31"]) == 5 ? 80 : 100;

                                //RESPUESTA FINAL ENERGIA/FATIGA
                                double EnergiaFatiga = (P23 + P27 + P29 + P31) / 4;

                                //Bienestar Emocioanl
                                int P24 = Convert.ToInt32(Lectura_Hora2["Pregunta24"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta24"]) == 2 ? 20 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta24"]) == 3 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta24"]) == 4 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta24"]) == 5 ? 80 : 100;
                                int P25 = Convert.ToInt32(Lectura_Hora2["Pregunta25"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta25"]) == 2 ? 20 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta25"]) == 3 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta25"]) == 4 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta25"]) == 5 ? 80 : 100;
                                int P26 = Convert.ToInt32(Lectura_Hora2["Pregunta26"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta26"]) == 2 ? 80 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta26"]) == 3 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta26"]) == 4 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta26"]) == 5 ? 20 : 0;
                                int P28 = Convert.ToInt32(Lectura_Hora2["Pregunta28"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta28"]) == 2 ? 20 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta28"]) == 3 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta28"]) == 4 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta28"]) == 5 ? 80 : 100;
                                int P30 = Convert.ToInt32(Lectura_Hora2["Pregunta30"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta30"]) == 2 ? 80 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta30"]) == 3 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta30"]) == 4 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta30"]) == 5 ? 20 : 0;

                                //RESPUESTA FINAL BIENESTAR EMOCIONAL
                                double BienestarEmocionañ = (P24 + P25 + P26 * P28 + P30) / 5;

                                //Funcion Social
                                int P20 = Convert.ToInt32(Lectura_Hora2["Pregunta20"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta20"]) == 2 ? 75 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta20"]) == 3 ? 50 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta20"]) == 4 ? 25 : 0;
                                int P32 = Convert.ToInt32(Lectura_Hora2["Pregunta32"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta32"]) == 2 ? 25 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta32"]) == 3 ? 50 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta32"]) == 4 ? 75 : 100;

                                //RESPUESTA FINAL FUNCION SOCIAL
                                double FuncionSocial = (P20 + P32) / 2;

                                //Dolor
                                int P21 = Convert.ToInt32(Lectura_Hora2["Pregunta21"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta21"]) == 2 ? 80 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta21"]) == 3 ? 60 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta21"]) == 4 ? 40 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta21"]) == 5 ? 20 : 0;
                                int P22 = Convert.ToInt32(Lectura_Hora2["Pregunta22"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta22"]) == 2 ? 75 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta22"]) == 3 ? 50 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta22"]) == 4 ? 25 : 0;

                                //RESPUESTA FINAL DOLOR
                                double Dolor = (P21 + P22) / 2;

                                //Salud en General
                                int P1 = Convert.ToInt32(Lectura_Hora2["Pregunta1"]) == 1 ? 100 :
                                         Convert.ToInt32(Lectura_Hora2["Pregunta1"]) == 2 ? 75 :
                                         Convert.ToInt32(Lectura_Hora2["Pregunta1"]) == 3 ? 50 :
                                         Convert.ToInt32(Lectura_Hora2["Pregunta1"]) == 4 ? 25 : 0;
                                int P2 = Convert.ToInt32(Lectura_Hora2["Pregunta2"]) == 1 ? 100 :
                                         Convert.ToInt32(Lectura_Hora2["Pregunta2"]) == 2 ? 75 :
                                         Convert.ToInt32(Lectura_Hora2["Pregunta2"]) == 3 ? 50 :
                                         Convert.ToInt32(Lectura_Hora2["Pregunta2"]) == 4 ? 25 : 0;
                                int P33 = Convert.ToInt32(Lectura_Hora2["Pregunta33"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta33"]) == 2 ? 25 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta33"]) == 3 ? 50 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta33"]) == 4 ? 75 : 100;
                                int P34 = Convert.ToInt32(Lectura_Hora2["Pregunta34"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta34"]) == 2 ? 75 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta34"]) == 3 ? 50 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta34"]) == 4 ? 25 : 0;
                                int P35 = Convert.ToInt32(Lectura_Hora2["Pregunta35"]) == 1 ? 0 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta35"]) == 2 ? 25 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta35"]) == 3 ? 50 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta35"]) == 4 ? 75 : 100;
                                int P36 = Convert.ToInt32(Lectura_Hora2["Pregunta36"]) == 1 ? 100 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta36"]) == 2 ? 75 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta36"]) == 3 ? 50 :
                                          Convert.ToInt32(Lectura_Hora2["Pregunta36"]) == 4 ? 25 : 0;

                                //RESPUESTA SALUD GENERAL
                                double SaludGeneral = (P1 + P2 + P33 + P34 + P35 + P36) / 6;

                                //RES PUNTAJE GLOBAL
                                double PuntajeGlobal = (FactorFisico + LimitacionesFisicas + LimitacionesEmocionales + EnergiaFatiga + BienestarEmocionañ + FuncionSocial + Dolor + SaludGeneral) / 8.0;

                                //RES EMOCIONAL
                                double Emocional = (LimitacionesEmocionales + BienestarEmocionañ + FuncionSocial + SaludGeneral) / 4.0;

                                //RES FISICAL
                                double Fisica = (FactorFisico + LimitacionesFisicas + EnergiaFatiga + Dolor) / 8.0;

                                //ResFecha
                                DateTime fecha = Convert.ToDateTime(Lectura_Hora2["FechaEncuesta"]);

                                ClaseReportsFibro claseReportsFibros = new ClaseReportsFibro
                                {
                                    Fecha1 = fecha,
                                    Dato1 = res.ToString("F2"),

                                    Dato10 = FactorFisico.ToString("F2"),
                                    Dato2 = LimitacionesFisicas.ToString("F2"),
                                    Dato3 = LimitacionesEmocionales.ToString("F2"),
                                    Dato4 = EnergiaFatiga.ToString("F2"),
                                    Dato5 = BienestarEmocionañ.ToString("F2"),
                                    Dato6 = FuncionSocial.ToString("F2"),
                                    Dato7 = Dolor.ToString("F2"),
                                    Dato8 = SaludGeneral.ToString("F2"),
                                    Dato9 = Pregunta1A11.ToString("F2"),

                                    Dato11 = Emocional.ToString("F2"),
                                    Dato12 = Fisica.ToString("F2"),


                                    Dato36 = Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                             Lectura_Hora2["Pac_SegundoA"].ToString() + " " +
                                             Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                             Lectura_Hora2["Pac_SegundoN"].ToString()
                                };

                                return claseReportsFibros;
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
        FIB_ENCUESTA2 IEncuestasXPaciente.getResultE2(int E2)
        {
            try
            {
                if (E2 == 0)
                {
                    FIB_ENCUESTA2 F = new FIB_ENCUESTA2
                    {
                        Pregunta1 = "0",
                        Pregunta2 = "0",
                        Pregunta3 = "0",
                        Pregunta4 = "0",
                        Pregunta5 = "0",
                        Pregunta6 = "0",
                        Pregunta7 = "0",
                        Pregunta8 = "0",
                        Pregunta9 = "0",
                        Pregunta10 = "0",
                        Pregunta11 = "0",
                        Pregunta12 = "0",
                        Pregunta13 = "0",
                        Pregunta14 = "0",
                        Pregunta15 = "0",
                        Pregunta16 = "0",
                        Pregunta17 = "0",
                        Pregunta18 = "0",
                        Pregunta19 = "0",

                        ResultadoP3 = 0,

                        Fatiga = 0,
                        Sueño = 0,
                        Trastorno = 0
                    };

                    return F;
                }

                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora2 = "SELECT TOP 3 *  " +
                                          "FROM FIB_ENCUESTA2 " +
                                          "WHERE Id = @param1";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", E2);

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                FIB_ENCUESTA2 F = new FIB_ENCUESTA2
                                {
                                    Pregunta1 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta1"].ToString()) ? "0" : "1",
                                    Pregunta2 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta2"].ToString()) ? "0" : "1",
                                    Pregunta3 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta3"].ToString()) ? "0" : "1",
                                    Pregunta4 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta4"].ToString()) ? "0" : "1",
                                    Pregunta5 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta5"].ToString()) ? "0" : "1",
                                    Pregunta6 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta6"].ToString()) ? "0" : "1",
                                    Pregunta7 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta7"].ToString()) ? "0" : "1",
                                    Pregunta8 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta8"].ToString()) ? "0" : "1",
                                    Pregunta9 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta9"].ToString()) ? "0" : "1",
                                    Pregunta10 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta10"].ToString()) ? "0" : "1",
                                    Pregunta11 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta11"].ToString()) ? "0" : "1",
                                    Pregunta12 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta12"].ToString()) ? "0" : "1",
                                    Pregunta13 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta13"].ToString()) ? "0" : "1",
                                    Pregunta14 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta14"].ToString()) ? "0" : "1",
                                    Pregunta15 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta15"].ToString()) ? "0" : "1",
                                    Pregunta16 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta16"].ToString()) ? "0" : "1",
                                    Pregunta17 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta17"].ToString()) ? "0" : "1",
                                    Pregunta18 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta18"].ToString()) ? "0" : "1",
                                    Pregunta19 = string.IsNullOrEmpty(Lectura_Hora2["Pregunta19"].ToString()) ? "0" : "1",

                                    Fatiga = Convert.ToInt32(Lectura_Hora2["Fatiga"]),
                                    Sueño = Convert.ToInt32(Lectura_Hora2["Sueño"]),
                                    Trastorno = Convert.ToInt32(Lectura_Hora2["Trastorno"]),

                                    ResultadoP3 = Convert.ToInt32(Lectura_Hora2["Fatiga"]) + Convert.ToInt32(Lectura_Hora2["Sueño"]) + Convert.ToInt32(Lectura_Hora2["Trastorno"])
                                };
                                    
                                return F;
                            }
                            else
                            {
                                FIB_ENCUESTA2 F = new FIB_ENCUESTA2
                                {
                                    Pregunta1 = "0",
                                    Pregunta2 = "0",
                                    Pregunta3 = "0",
                                    Pregunta4 = "0",
                                    Pregunta5 = "0",
                                    Pregunta6 = "0",
                                    Pregunta7 = "0",
                                    Pregunta8 = "0",
                                    Pregunta9 = "0",
                                    Pregunta10 = "0",
                                    Pregunta11 = "0",
                                    Pregunta12 = "0",
                                    Pregunta13 = "0",
                                    Pregunta14 = "0",
                                    Pregunta15 = "0",
                                    Pregunta16 = "0",
                                    Pregunta17 = "0",
                                    Pregunta18 = "0",
                                    Pregunta19 = "0",

                                    Fatiga = 0,
                                    Sueño = 0,
                                    Trastorno = 0,

                                    ResultadoP3 = 0
                                };

                                return F;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                FIB_ENCUESTA2 F = new FIB_ENCUESTA2
                {
                    Pregunta1 = "0",
                    Pregunta2 = "0",
                    Pregunta3 = "0",
                    Pregunta4 = "0",
                    Pregunta5 = "0",
                    Pregunta6 = "0",
                    Pregunta7 = "0",
                    Pregunta8 = "0",
                    Pregunta9 = "0",
                    Pregunta10 = "0",
                    Pregunta11 = "0",
                    Pregunta12 = "0",
                    Pregunta13 = "0",
                    Pregunta14 = "0",
                    Pregunta15 = "0",
                    Pregunta16 = "0",
                    Pregunta17 = "0",
                    Pregunta18 = "0",
                    Pregunta19 = "0",

                    ResultadoP3 = 0,

                    Fatiga = 0,
                    Sueño = 0,
                    Trastorno = 0
                };

                return F;
            }
        }
        #endregion
    }
}
