using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MJuntas : IJuntas
    {
        List<CXN_HCJUNTAS> IJuntas.listarJuntasFirma(string TID, string IDD)
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
                    String Query = "SELECT P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                   "J.Jun_Adm AS ADM, J.Jun_Fecha AS FECHA, B.Bod_Responsable AS PROF, J.Jun_Tipo, J.Jun_Firma " +
                                   "FROM CXN_HCJUNTAS J " +
                                   "INNER JOIN CXN_PACIENTES P ON J.Jun_Pac = P.Pac_Id " +
                                   "INNER JOIN CXN_BODEGAS B ON J.Jun_Bodega = B.Bod_Numero " +
                                   "WHERE P.Pac_TipoId = '" + TID + "' " +
                                   "AND P.Pac_IdNum = '" + IDD + "' " +
                                   "ORDER BY J.Jun_Fecha DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_HCJUNTAS> H = new List<CXN_HCJUNTAS>();

                        while (Reader.Read() == true)
                        {
                            string Firmado;

                            if (Reader["Jun_Firma"].ToString() == "SI")
                            {
                                Firmado = "Firmado";
                            }
                            else
                            {
                                Firmado = "No Firmado";
                            }

                            H.Add(new CXN_HCJUNTAS
                            {
                                Jun_Adm = Convert.ToInt32(Reader["ADM"]),
                                Jun_Fecha = Convert.ToDateTime(Reader["FECHA"]),
                                Jun_Tipo = Reader["Jun_Tipo"].ToString(),
                                Jun_Firma = Firmado.ToString(),
                                Jun_MedFirma = Reader["PROF"].ToString(), // profesional
                                Jun_TF = Reader["PACIENTE"].ToString(), //paciente
                                Jun_Observa = Firmado //firma si no
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
            catch
            {
                return null;
            }
        }
        CXN_HCJUNTAS IJuntas.getJuntaCompleta(int Admision)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_HCJUNTAS " +
                                   "WHERE Jun_Adm = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_HCJUNTAS H = new CXN_HCJUNTAS
                                {
                                    Jun_Pac = Convert.ToInt32(Reader["Jun_Pac"]),
                                    Jun_Cia = Convert.ToInt32(Reader["Jun_Cia"]),
                                    Jun_Ase = Convert.ToInt32(Reader["Jun_Ase"]),
                                    Jun_Adm = Convert.ToInt32(Reader["Jun_Adm"]),
                                    Jun_Fecha = Convert.ToDateTime(Reader["Jun_Fecha"]),
                                    Jun_Tipo = Reader["Jun_Tipo"].ToString(),
                                    Jun_MedFirma = Reader["Jun_MedFirma"].ToString(), // profesional
                                    Jun_TF = Reader["Jun_TF"].ToString(), //paciente
                                    Jun_Observa = Reader["Jun_Observa"].ToString(), //firma si no
                                    Jun_Fisiatra = Reader["Jun_Fisiatra"].ToString(),
                                    Jun_Psicologo = Reader["Jun_Psicologo"].ToString(),
                                    Jun_TO = Reader["Jun_TO"].ToString(),
                                    Jun_DX_FI = Reader["Jun_DX_FI"].ToString(),
                                    Jun_DX_TF = Reader["Jun_DX_TF"].ToString(),
                                    Jun_DX_TO = Reader["Jun_DX_TO"].ToString(),
                                    Jun_DX_PS = Reader["Jun_DX_PS"].ToString(),
                                    Jun_Pro_FI = Reader["Jun_Pro_FI"].ToString(),
                                    Jun_Pro_TF = Reader["Jun_Pro_TF"].ToString(),
                                    Jun_Pro_TO = Reader["Jun_Pro_TO"].ToString(),
                                    Jun_Pro_PS = Reader["Jun_Pro_PS"].ToString(),
                                    Jun_Con_FI = Reader["Jun_Con_FI"].ToString(),
                                    Jun_Con_PS = Reader["Jun_Con_PS"].ToString(),
                                    Jun_Con_TF = Reader["Jun_Con_TF"].ToString(),
                                    Jun_Con_TO = Reader["Jun_Con_TO"].ToString(),
                                    Jun_Observa_TO = Reader["Jun_Observa_TO"].ToString(),
                                    Jun_Observa_PS = Reader["Jun_Observa_PS"].ToString(),
                                    Jun_Prox_Cita = Convert.ToDateTime(Reader["Jun_Prox_Cita"]),
                                    Jun_CIE10 = Reader["Jun_CIE10"].ToString(),
                                    Jun_Egresa = Reader["Jun_CIE10"].ToString()
                                };

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
        void IJuntas.Firmar(int admision, string Med)
        {
            Dictionary<string,string> getData = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getData["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now;

                SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_HCJUNTAS " +
                                  "SET " +
                                  "Jun_Firma = @param1, " +
                                  "Jun_MedFirma = @param2, " +
                                  "Jun_MedFirmaFecha = @param3 " +
                                  "WHERE Jun_Adm = '" + admision + "'", con);

                Busqueda.Parameters.AddWithValue("@param1", "SI");
                Busqueda.Parameters.AddWithValue("@param2", Med);
                Busqueda.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = Hoy;
                Busqueda.ExecuteNonQuery();
            }
        }
        bool IJuntas.InsertarJunta(CXN_HCJUNTAS H)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HCJUNTAS " +
                                                     "(Jun_Pac, " +
                                                     "Jun_Cia, " +
                                                     "Jun_Fecha, " +
                                                     "Jun_Bodega, " +
                                                     "Jun_Fisiatra, " +
                                                     "Jun_Psicologo, " +
                                                     "Jun_TF, " +
                                                     "Jun_TO, " +
                                                     "Jun_Adm, " +
                                                     "Jun_DX_FI, " +
                                                     "Jun_DX_TF, " +
                                                     "Jun_DX_TO, " +
                                                     "Jun_DX_PS, " +
                                                     "Jun_Pro_FI, " +
                                                     "Jun_Pro_TF, " +
                                                     "Jun_Pro_TO, " +
                                                     "Jun_Pro_PS, " +
                                                     "Jun_Con_FI, " +
                                                     "Jun_Con_TF, " +
                                                     "Jun_Con_TO, " +
                                                     "Jun_Con_PS, " +
                                                     "Jun_Observa, " +
                                                     "Jun_Prox_Cita, " +
                                                     "Jun_CIE10, " +
                                                     "Jun_Cant, " +
                                                     "Jun_Edad, " +
                                                     "Jun_Tipo, " +
                                                     "Jun_Ase, " +
                                                     "Jun_Observa_TO) " +
                                 "VALUES                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6, " +
                                                          "@param7, " +
                                                          "@param8, " +
                                                          "@param9, " +
                                                          "@param10, " +
                                                          "@param11, " +
                                                          "@param12, " +
                                                          "@param13, " +
                                                          "@param14, " +
                                                          "@param15, " +
                                                          "@param16, " +
                                                          "@param17, " +
                                                          "@param18, " +
                                                          "@param19, " +
                                                          "@param20, " +
                                                          "@param21, " +
                                                          "@param22, " +
                                                          "@param23, " +
                                                          "@param24, " +
                                                          "@param25, " +
                                                          "@param26, " +
                                                          "@param27, " +
                                                          "@param28, " +
                                                          "@param29)", con);

                    cmd.Parameters.AddWithValue("@param1", H.Jun_Pac);
                    cmd.Parameters.AddWithValue("@param2", H.Jun_Cia);
                    cmd.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = H.Jun_Fecha;
                    cmd.Parameters.AddWithValue("@param4", H.Jun_Bodega);
                    cmd.Parameters.AddWithValue("@param5", H.Jun_Fisiatra);
                    cmd.Parameters.AddWithValue("@param6", H.Jun_Psicologo);
                    cmd.Parameters.AddWithValue("@param7", H.Jun_TF);
                    cmd.Parameters.AddWithValue("@param8", H.Jun_TO);
                    cmd.Parameters.AddWithValue("@param9", H.Jun_Adm);
                    cmd.Parameters.AddWithValue("@param10", H.Jun_DX_FI);
                    cmd.Parameters.AddWithValue("@param11", H.Jun_DX_TF);
                    cmd.Parameters.AddWithValue("@param12", H.Jun_DX_TO);
                    cmd.Parameters.AddWithValue("@param13", H.Jun_DX_PS);
                    cmd.Parameters.AddWithValue("@param14", H.Jun_Pro_FI);
                    cmd.Parameters.AddWithValue("@param15", H.Jun_Pro_TF);
                    cmd.Parameters.AddWithValue("@param16", H.Jun_Pro_TO);
                    cmd.Parameters.AddWithValue("@param17", H.Jun_Pro_PS);
                    cmd.Parameters.AddWithValue("@param18", H.Jun_Con_FI);
                    cmd.Parameters.AddWithValue("@param19", H.Jun_Con_TF);
                    cmd.Parameters.AddWithValue("@param20", H.Jun_Con_TO);
                    cmd.Parameters.AddWithValue("@param21", H.Jun_Con_PS);
                    cmd.Parameters.AddWithValue("@param22", H.Jun_Observa);
                    cmd.Parameters.Add(new SqlParameter("@param23", SqlDbType.DateTime)).Value = H.Jun_Prox_Cita;
                    cmd.Parameters.AddWithValue("@param24", H.Jun_CIE10);
                    cmd.Parameters.AddWithValue("@param25", H.Jun_Cant);
                    cmd.Parameters.AddWithValue("@param26", H.Jun_Edad);
                    cmd.Parameters.AddWithValue("@param27", H.Jun_Tipo);
                    cmd.Parameters.AddWithValue("@param28", H.Jun_Ase);
                    cmd.Parameters.AddWithValue("@param29", H.Jun_Observa_TO);
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
        List<CXN_HCJUNTAS> IJuntas.getJuntasForComplete(string TID, string NID, bool Informe)
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

                    String Query = "";

                    if (Informe == true)
                    {
                        Query = "SELECT P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                "J.Jun_Adm AS ADM, J.Jun_Fecha AS FECHA, B.Bod_Responsable AS PROF, J.Jun_Tipo AS TIPO, J.Jun_Egresa AS EGRESO " +
                                "FROM CXN_HCJUNTAS J " +
                                "INNER JOIN CXN_PACIENTES P ON J.Jun_Pac = P.Pac_Id " +
                                "INNER JOIN CXN_BODEGAS B ON J.Jun_Bodega = B.Bod_Numero " +
                                "WHERE P.Pac_TipoId = '" + TID + "' " +
                                "AND P.Pac_IdNum = '" + NID + "' " +
                                "ORDER BY J.Jun_Fecha DESC";
                    }
                    if (Informe == false)
                    {
                        Query = "SELECT TOP 10 H.Hor_Imp_Age AS PACIENTE, " +
                                   "H.Hor_Id AS ADM, H.Hor_Pac_Fecha_Cita AS FECHA, H.Hor_Observacion AS TIPO, B.Bod_Responsable AS PROF, '' AS EGRESO " +
                                   "FROM CXN_HORARIO H " +
                                   "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                   "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                   "WHERE P.Pac_TipoId = '" + TID + "' " +
                                   "AND P.Pac_IdNum = '" + NID + "' " +
                                   "AND H.Hor_Pac_tipo_Serv = 'TF' " +
                                   "AND H.Hor_Pac_Cup = '890505' " +
                                   "ORDER BY H.Hor_Pac_Fecha_Cita DESC";
                    }

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_HCJUNTAS> J = new List<CXN_HCJUNTAS>();

                                while (Reader.Read() == true)
                                {
                                    string Pac = Reader["PACIENTE"].ToString();

                                    J.Add(new CXN_HCJUNTAS
                                    {
                                        Jun_Adm = Convert.ToInt32(Reader["ADM"]),
                                        Jun_Fecha = Convert.ToDateTime(Reader["FECHA"]),
                                        Jun_Tipo = Reader["TIPO"].ToString(),
                                        Jun_Con_FI = Reader["PROF"].ToString(), //profesional
                                        Jun_Observa = Pac, //paciente
                                        Jun_Egresa = Reader["EGRESO"].ToString()
                                    });
                                }

                                return J;
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
        bool IJuntas.UpdateObservations(CXN_HCJUNTAS H)
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

                    SqlCommand Busqueda = new SqlCommand();

                    if (H.Jun_MedFirma == "PS")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_HCJUNTAS " +
                                                  "SET  " +
                                                  "Jun_Observa_PS = @param1, " +
                                                  "Jun_DX_PS = @param2, " +
                                                  "Jun_Pro_PS = @param3, " +
                                                  "Jun_Con_PS = @param4 " +
                                                  "WHERE Jun_Adm = '" + H.Jun_Adm + "'", con);
                    }

                    if (H.Jun_MedFirma == "TO")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_HCJUNTAS " +
                                                   "SET  " +
                                                   "Jun_Observa_TO = @param1, " +
                                                   "Jun_DX_TO = @param2, " +
                                                   "Jun_Pro_TO = @param3, " +
                                                   "Jun_Con_TO = @param4 " +
                                                   "WHERE Jun_Adm = '" + H.Jun_Adm + "'", con);
                    }

                    if (H.Jun_MedFirma == "FI")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_HCJUNTAS " +
                                                  "SET  " +
                                                  "Jun_Observa = @param1, " +
                                                  "Jun_DX_FI = @param2, " +
                                                  "Jun_Pro_FI = @param3, " +
                                                  "Jun_Con_FI = @param4 " +
                                                  "WHERE Jun_Adm = '" + H.Jun_Adm + "'", con);
                    }

                    if (H.Jun_MedFirma == "TF")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_HCJUNTAS " +
                                                  "SET  " +
                                                  "Jun_DX_TF = @param2, " +
                                                  "Jun_Pro_TF = @param3, " +
                                                  "Jun_Con_TF = @param4 " +
                                                  "WHERE Jun_Adm = '" + H.Jun_Adm + "'", con);
                    }

                    Busqueda.Parameters.AddWithValue("@param1", (string.IsNullOrEmpty(H.Jun_Observa) ? "" : H.Jun_Observa.ToUpper()));
                    Busqueda.Parameters.AddWithValue("@param2", H.Jun_DX_TF.ToUpper());
                    Busqueda.Parameters.AddWithValue("@param3", H.Jun_Pro_TF.ToUpper());
                    Busqueda.Parameters.AddWithValue("@param4", H.Jun_Con_TF.ToUpper());
                    Busqueda.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IJuntas.UpdateEgreso(int Admision, string Seleccion)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_HCJUNTAS " +
                                                  "SET  " +
                                                  "Jun_Egresa = @param1 " +
                                                  "WHERE Jun_Adm = '" + Admision + "'", con);

                    Busqueda.Parameters.AddWithValue("@param1", Seleccion);
                    Busqueda.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        void IJuntas.updateJuntaMedica(CXN_HCJUNTAS H)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_HCJUNTAS " +
                                                           "SET  " +
                                                           "Jun_Fisiatra = @param1, " +
                                                           "Jun_Psicologo = @param2, " +
                                                           "Jun_TO = @param3, " +
                                                           "Jun_TF = @param4, " +
                                                           "Jun_CIE10 = @param5, " +
                                                           "Jun_Edad = @param6, " +
                                                           "Jun_Prox_Cita = @param7 " +
                                                           "WHERE Jun_Adm = @param8", con);

                    Busqueda.Parameters.AddWithValue("@param1", H.Jun_Fisiatra);
                    Busqueda.Parameters.AddWithValue("@param2", H.Jun_Psicologo);
                    Busqueda.Parameters.AddWithValue("@param3", H.Jun_TO);
                    Busqueda.Parameters.AddWithValue("@param4", H.Jun_TF);
                    Busqueda.Parameters.AddWithValue("@param5", H.Jun_CIE10);
                    Busqueda.Parameters.AddWithValue("@param6", H.Jun_Edad);
                    Busqueda.Parameters.Add(new SqlParameter("@param7", SqlDbType.DateTime)).Value = H.Jun_Prox_Cita;
                    Busqueda.Parameters.AddWithValue("@param8", H.Jun_Adm);
                    Busqueda.ExecuteNonQuery();
                }               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
