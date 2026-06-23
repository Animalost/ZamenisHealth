using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Persistence.CXN.Metodos
{
    public class MFHIR : IFHIR
    {
        async Task<List<CXN_HORARIO>> IFHIR.FiltrarEspecialidad(DateTime Fecha, string Especialidad)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    String Cargar_Hora = "";

                    switch (Especialidad)
                    {
                        case "MG":
                            Cargar_Hora = "SELECT H.Hor_Imp_Age, R.HC_Adm AS AdmisionUnica " +
                                          "FROM CXN_HCMG R " +
                                          "INNER JOIN CXN_HORARIO H ON R.HC_Adm = H.Hor_Id " +
                                          "WHERE R.HC_Fecha BETWEEN @param1 AND @param2 " +
                                          "ORDER BY R.HC_Id DESC";
                            break;

                        case "CU":
                            Cargar_Hora = "SELECT H.Hor_Imp_Age, R.Not_Adm AS AdmisionUnica " +
                                          "FROM CXN_NOTAS R " +
                                          "INNER JOIN CXN_HORARIO H ON R.Not_Adm = H.Hor_Id " +
                                          "WHERE R.Not_Fecha BETWEEN @param1 AND @param2 " +
                                          "ORDER BY R.Not_Id DESC";
                            break;

                        case "FI":
                            Cargar_Hora = "SELECT H.Hor_Imp_Age, R.HC_Adm AS AdmisionUnica " +
                                          "FROM CXN_HCFI R " +
                                          "INNER JOIN CXN_HORARIO H ON R.HC_Adm = H.Hor_Id " +
                                          "WHERE R.HC_Fecha BETWEEN @param1 AND @param2 " +
                                          "ORDER BY R.HC_Id DESC";
                            break;

                        case "TO":
                            Cargar_Hora = "SELECT H.Hor_Imp_Age, R.HC_Adm AS AdmisionUnica " +
                                          "FROM CXN_HCTO R " +
                                          "INNER JOIN CXN_HORARIO H ON R.HC_Adm = H.Hor_Id " +
                                          "WHERE R.HC_Fecha BETWEEN @param1 AND @param2 " +
                                          "ORDER BY R.HC_Id DESC";
                            break;

                        case "TF":
                            Cargar_Hora = "SELECT H.Hor_Imp_Age, R.HC_Adm AS AdmisionUnica " +
                                          "FROM CXN_HCTF R " +
                                          "INNER JOIN CXN_HORARIO H ON R.HC_Adm = H.Hor_Id " +
                                          "WHERE R.HC_Fecha BETWEEN @param1 AND @param2 " +
                                          "ORDER BY R.HC_Id DESC";
                            break;

                        case "PS":
                            Cargar_Hora = "SELECT H.Hor_Imp_Age, R.HC_Adm AS AdmisionUnica " +
                                          "FROM CXN_HCPSI R " +
                                          "INNER JOIN CXN_HORARIO H ON R.HC_Adm = H.Hor_Id " +
                                          "WHERE R.HC_Fecha BETWEEN @param1 AND @param2 " +
                                          "ORDER BY R.HC_Id DESC";
                            break;

                        case "RA":
                            Cargar_Hora = "SELECT H.Hor_Imp_Age, R.HCAdm AS AdmisionUnica " +
                                          "FROM CXN_HCRADIOLOGIA R " +
                                          "INNER JOIN CXN_HORARIO H ON R.HCAdm = H.Hor_Id " +
                                          "WHERE R.Fecha BETWEEN @param1 AND @param2 " +
                                          "ORDER BY R.Id DESC";
                            break;

                        case "NAMG":
                            Cargar_Hora = "SELECT HC_Adm AS AdmisionUnica, HC_Pac AS Hor_Imp_Age " +
                                          "FROM CXN_HCMG " +
                                          "WHERE HC_Fecha BETWEEN @param1 AND @param2 " +
                                          "AND HC_Nota_Acl IS NOT NULL " +
                                          "ORDER BY HC_Id DESC";
                            break;

                        default:
                            return null;
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Fecha.Date);
                        Carga_Command.Parameters.AddWithValue("@param2", Fecha.Date);

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> L = new List<CXN_HORARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    CXN_RDA r = await ConsultarEnviosRDA(Convert.ToInt32(Lectura_Hora["AdmisionUnica"])); 

                                    if (r == null)
                                    {
                                        L.Add(new CXN_HORARIO
                                        {
                                            Hor_Id = Convert.ToInt32(Lectura_Hora["AdmisionUnica"]),
                                            Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                            Hor_ArrastraHistoria = "", //RDA PACIENTE
                                            Hor_AdmOpnened = "", //RDA AMBULATORIO
                                            Hor_Autoriza = "", //Fecha RDA Paciente
                                            Hor_RegAtn = "", //Fecha RDA Ambulatorio
                                            Hor_Usr_Admisiona = "", //Persona Reporta RDA Paciente
                                            PacienteAseguradora = "" // Persona Reporta RDA Ambulatorio
                                        });
                                    }
                                    else
                                    {
                                        L.Add(new CXN_HORARIO
                                        {
                                            Hor_Id = Convert.ToInt32(Lectura_Hora["AdmisionUnica"]),
                                            Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                            Hor_ArrastraHistoria = r.RDAPaciente, //RDA PACIENTE
                                            Hor_AdmOpnened = r.RDAAmbulatorio, //RDA AMBULATORIO
                                            Hor_Autoriza = string.IsNullOrEmpty(r.FechaReporteRDAPacienteText) ? "" : Convert.ToDateTime(r.FechaReporteRDAPacienteText).ToString("yyyy-MM-dd"), //Fecha RDA Paciente
                                            Hor_RegAtn = string.IsNullOrEmpty(r.FechaReporteRDAAmbulatorioText) ? "" : Convert.ToDateTime(r.FechaReporteRDAAmbulatorioText).ToString("yyyy-MM-dd"), //Fecha RDA Ambulatorio
                                            Hor_Usr_Admisiona = r.PersonaReportaRDAPaciente, //Persona Reporta RDA Paciente
                                            PacienteAseguradora = r.PersonaReportaRDAAmbulatorio // Persona Reporta RDA Ambulatorio
                                        });
                                    }                                    
                                }

                                return L;
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
        async Task<CXN_RDA> ConsultarEnviosRDA(int Admision)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_RDA " +
                                         "WHERE Admision = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = await (Carga_Command.ExecuteReaderAsync()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_RDA r = new CXN_RDA
                                {
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    Especialidad = Lectura_Hora["Especialidad"].ToString(),
                                    RDAPaciente = Lectura_Hora["RDAPaciente"].ToString(),
                                    RDAAmbulatorio = Lectura_Hora["RDAAmbulatorio"].ToString(),
                                    FechaReporteRDAPacienteText = Lectura_Hora["FechaReporteRDAPaciente"] == DBNull.Value ? "" : Convert.ToDateTime(Lectura_Hora["FechaReporteRDAPaciente"]).ToString(),
                                    FechaReporteRDAAmbulatorioText = Lectura_Hora["FechaReporteRDAAmbulatorio"] == DBNull.Value ? "" :  Convert.ToDateTime(Lectura_Hora["FechaReporteRDAAmbulatorio"]).ToString(),
                                    Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                    PersonaReportaRDAPaciente = Lectura_Hora["PersonaReportaRDAPaciente"].ToString(),
                                    PersonaReportaRDAAmbulatorio = Lectura_Hora["PersonaReportaRDAAmbulatorio"].ToString()
                                };
                                return r;
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
        string IFHIR.Gender(string Tipo)
        {
            switch (Tipo)
            {
                case "M":
                    return "male";
                case "F":
                    return "female";
                default:
                    return "male";
            }
        }
        List<CXN_CONDICIONES> IFHIR.ConsultarAlergias(int Paciente)
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
                                         "FROM CXN_CONDICIONES " +
                                         "WHERE Paciente = @param1 " +
                                         "AND Condicion = 'ALERGIA' " +
                                         "AND CodigoFHIR IS NOT NULL";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_CONDICIONES> a = new List<CXN_CONDICIONES>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    a.Add(new CXN_CONDICIONES
                                    { 
                                        Detalle = Lectura_Hora["Detalle"].ToString(),
                                        CodigoFHIR = Lectura_Hora["CodigoFHIR"].ToString()
                                    });
                                }
                                
                                return a;
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
        string IFHIR.Modalidad(string code)
        {
            string Modalidad_Cita = "Intramural";

            switch (code)
            {
                case "01":
                    Modalidad_Cita = "Intramural";
                    break;

                case "02":
                    Modalidad_Cita = "Extramural unidad móvil";
                    break;

                case "03":
                    Modalidad_Cita = "Extramural domiciliaria";
                    break;

                case "04":
                    Modalidad_Cita = "Extramural jornada de salud";
                    break;

                case "06":
                    Modalidad_Cita = "Telemedicina interactiva";
                    break;

                case "07":
                    Modalidad_Cita = "Telemedicina no interactiva";
                    break;

                case "08":
                    Modalidad_Cita = "Telemedicina telexperticia";
                    break;

                case "09":
                    Modalidad_Cita = "Telemedicina telemonitoreo";
                    break;

                default:
                    Modalidad_Cita = "Intramural";
                    break;
            }

            return Modalidad_Cita;
        }
        string IFHIR.GrupoServicios(string Code)
        {
            switch (Code)
            {
                case "01":
                    return "Consulta externa";
                case "02":
                    return "Apoyo diagnóstico y complementación terapéutica";
                case "03":
                    return "Internación";
                case "04":
                    return "Quirúrgico";
                case "05":
                    return "Atención inmediata";
                default:
                    return "Consulta externa";
            }
        }
        string IFHIR.Etnia(string Name)
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
                                   "FROM CXN_ETNIA " +
                                   "WHERE Etnia = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Name);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Codigo"].ToString();
                            }
                            else
                            {
                                return "Ninguna de las anteriores";
                            }
                        }
                    }                  
                }
            }
            catch
            {
                return "Ninguna de las anteriores";
            }
        }
        string IFHIR.Discapacidad(string Name)
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
                                   "FROM CXN_DISCAPACIDAD " +
                                   "WHERE Discapacidad = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Name);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Codigo"].ToString();
                            }
                            else
                            {
                                return "Sin discapacidad";
                            }
                        }
                    }
                }
            }
            catch
            {
                return "Sin discapacidad";
            }
        }
        bool IFHIR.InsertarToken(CXN_TOKENS_FHIR t)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_TOKENS_FHIR (Token, " +
                                                                  "Fecha, " +
                                                                  "Prestador) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3)", con);

                    cmd.Parameters.AddWithValue("@param1", t.Token);
                    cmd.Parameters.AddWithValue("@param2", Convert.ToDateTime(t.Fecha));
                    cmd.Parameters.AddWithValue("@param3", t.Prestador);
                    int c = cmd.ExecuteNonQuery();
                    return c > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        string IFHIR.RecuperarToken(int Prestador)
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

                    String Cargar_Hora = "SELECT TOP 1 * " +
                                         "FROM CXN_TOKENS_FHIR " +
                                         "WHERE Prestador = @param1 " +
                                         "ORDER BY Id DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {                                
                                return Lectura_Hora["Token"].ToString();
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
        CXN_TOKENS_FHIR IFHIR.RecuperarClaseToken(int Prestador)
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

                    String Cargar_Hora = "SELECT TOP 1 * " +
                                         "FROM CXN_TOKENS_FHIR " +
                                         "WHERE Prestador = @param1 " +
                                         "ORDER BY Id DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_TOKENS_FHIR T = new CXN_TOKENS_FHIR()
                                {
                                    Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                    Token = Lectura_Hora["Token"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    Prestador = Prestador
                                };

                                return T;
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
        void IFHIR.InsertarEnvio_RDAPaciente(CXN_RDA Respuesta)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_RDA (Admision, " +
                                                                  "RDAPaciente, " +
                                                                  "Especialidad, " +
                                                                  "PersonaReportaRDAPaciente, " +
                                                                  "FechaReporteRDAPaciente) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", Respuesta.Admision);
                    cmd.Parameters.Add("@param2", SqlDbType.VarChar, -1).Value = (object)Respuesta.RDAPaciente ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@param3", Respuesta.Especialidad);
                    cmd.Parameters.AddWithValue("@param4", Respuesta.PersonaReportaRDAPaciente);
                    cmd.Parameters.AddWithValue("@param5", Convert.ToDateTime(Respuesta.FechaReporteRDAPaciente));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IFHIR.InsertarEnvio_RDAAmbulatorio(CXN_RDA Respuesta)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_RDA (Admision, " +
                                                                  "RDAAmbulatorio, " +
                                                                  "Especialidad, " +
                                                                  "PersonaReportaRDAAmbulatorio, " +
                                                                  "FechaReporteRDAAmbulatorio, " +
                                                                  "URLPDF) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5, " +
                                                                  "@param6)", con);

                    cmd.Parameters.AddWithValue("@param1", Respuesta.Admision);
                    cmd.Parameters.Add("@param2", SqlDbType.VarChar, -1).Value = (object)Respuesta.RDAAmbulatorio ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@param3", Respuesta.Especialidad);
                    cmd.Parameters.AddWithValue("@param4", Respuesta.PersonaReportaRDAAmbulatorio);
                    cmd.Parameters.AddWithValue("@param5", Convert.ToDateTime(Respuesta.FechaReporteRDAAmbulatorio));
                    cmd.Parameters.AddWithValue("@param6", Respuesta.URLPdf);
                    //cmd.Parameters.AddWithValue("@param7", Respuesta.idCompositionRecorded);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        CXN_RDA IFHIR.ConsultarAdmision(int Admision)
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
                                         "FROM CXN_RDA " +
                                         "WHERE Admision = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_RDA a = new CXN_RDA 
                                { 
                                    Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                    Especialidad = Lectura_Hora["Especialidad"].ToString(),
                                    FechaReporteRDAAmbulatorioText = Lectura_Hora["FechaReporteRDAAmbulatorio"].ToString(),
                                    FechaReporteRDAPacienteText = Lectura_Hora["FechaReporteRDAPaciente"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    PersonaReportaRDAAmbulatorio = Lectura_Hora["PersonaReportaRDAAmbulatorio"].ToString(),
                                    PersonaReportaRDAPaciente = Lectura_Hora["PersonaReportaRDAPaciente"].ToString(),
                                    RDAAmbulatorio = Lectura_Hora["RDAAmbulatorio"].ToString(),
                                    RDAPaciente = Lectura_Hora["RDAPaciente"].ToString()
                                };

                                return a;
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
        void IFHIR.ActualizarEnvio_RDAPaciente(CXN_RDA Respuesta)
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

                    string Busqueda = "UPDATE CXN_RDA " +
                                      "SET RDAPaciente = @param1, " +
                                      "Especialidad = @param2, " +
                                      "FechaReporteRDAPaciente = @param3, " +
                                      "PersonaReportaRDAPaciente = @param4 " +
                                      "WHERE Admision = @param5";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.Add("@param1", SqlDbType.VarChar, -1).Value = (object)Respuesta.RDAPaciente ?? DBNull.Value;
                    Accion.Parameters.AddWithValue("@param2", Respuesta.Especialidad);
                    Accion.Parameters.AddWithValue("@param3", Convert.ToDateTime(Respuesta.FechaReporteRDAPaciente));
                    Accion.Parameters.AddWithValue("@param4", Respuesta.PersonaReportaRDAPaciente);
                    Accion.Parameters.AddWithValue("@param5", Respuesta.Admision);

                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IFHIR.ActualizarEnvio_RDACExterna(CXN_RDA Respuesta)
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

                    string Busqueda = "UPDATE CXN_RDA " +
                                      "SET RDAAmbulatorio = @param1, " +
                                      "Especialidad = @param2, " +
                                      "FechaReporteRDAAmbulatorio = @param3, " +
                                      "PersonaReportaRDAAmbulatorio = @param4, " +
                                      "URLPdf = @param6  " +
                                      "WHERE Admision = @param5";
                    
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.Add("@param1", SqlDbType.VarChar, -1).Value = (object)Respuesta.RDAAmbulatorio ?? DBNull.Value;
                    Accion.Parameters.AddWithValue("@param2", Respuesta.Especialidad);
                    Accion.Parameters.AddWithValue("@param3", Convert.ToDateTime(Respuesta.FechaReporteRDAAmbulatorio));
                    Accion.Parameters.AddWithValue("@param4", Respuesta.PersonaReportaRDAAmbulatorio);
                    Accion.Parameters.AddWithValue("@param5", Respuesta.Admision);
                    Accion.Parameters.AddWithValue("@param6", Respuesta.URLPdf);
                    //Accion.Parameters.AddWithValue("@param7", Respuesta.idCompositionRecorded);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        string IFHIR.GrupoAlergias(string Code)
        {
            switch (Code)
            {
                case "01":
                    return "Medicamento";
                case "02":
                    return "Alimento";
                case "03":
                    return "Sustancia del ambiente";
                case "04":
                    return "Sustancia que entran en contacto con la piel";
                case "05":
                    return "Picadura de insectos";
                case "06":
                    return "Otra";
                default:
                    return "Otra";
            }
        }
        string IFHIR.GetCodeMedicamento(string Name)
        {
            return GetCodeMedicamento(Name);
        }
        string GetCodeMedicamento(string Name)
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

                    String Query = "SELECT Codigo " +
                                   "FROM CXN_MEDICAMENTOS " +
                                   "WHERE Medicamento = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Name);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Codigo"].ToString();
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }
        List<string> IFHIR.CargarRISK()
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

                    String Cargar_Hora = "SELECT Descripcion " +
                                         "FROM CXN_RISK";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<string> a = new List<string>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    a.Add(Lectura_Hora["Descripcion"].ToString());
                                }

                                return a;
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
        string IFHIR.GetCodeRisk(string Name)
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

                    String Query = "SELECT Codigo " +
                                   "FROM CXN_RISK " +
                                   "WHERE Descripcion = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Name);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Codigo"].ToString();
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }
        string IFHIR.GetDesctecnoSalud(string Code)
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

                    String Query = "SELECT Tec_DX " +
                                   "FROM CXN_TECNOSALUD " +
                                   "WHERE Tec_Cod = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Tec_DX"].ToString();
                            }
                            else
                            {
                                return "DIAGNOSTICO";
                            }
                        }
                    }
                }
            }
            catch
            {
                return "DIAGNOSTICO";
            }
        }
        List<string> IFHIR.GetConsumos()
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

                    String Cargar_Hora = "SELECT Consumo " +
                                         "FROM CXN_CONSUMOMED " +
                                         "ORDER BY Consumo ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<string> a = new List<string>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    a.Add(Lectura_Hora["Consumo"].ToString());
                                }

                                return a;
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
        string IFHIR.GetCodeConsumo(string Name)
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

                    String Cargar_Hora = "SELECT Codigo " +
                                         "FROM CXN_CONSUMOMED " +
                                         "WHERE Consumo = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Name);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {                                
                                return Lectura_Hora["Codigo"].ToString();
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
        List<string> IFHIR.GetOcupaciones(string Ocupacion)
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

                    String Cargar_Hora = "";

                    if (string.IsNullOrEmpty(Ocupacion))
                    {
                        Cargar_Hora = "SELECT Nombre " +
                                         "FROM CXN_OCUPACION " +
                                         "ORDER BY Nombre ASC";
                    }
                    else
                    {
                        Cargar_Hora = "SELECT Nombre " +
                                      "FROM CXN_OCUPACION " +
                                      "WHERE Nombre LIKE '%" + Ocupacion + "%' " +
                                      "ORDER BY Nombre ASC";
                    }

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<string> a = new List<string>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    a.Add(Lectura_Hora["Nombre"].ToString());
                                }

                                return a;
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
        string IFHIR.GetCodeOcupacion(string Name)
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

                    String Cargar_Hora = "SELECT Codigo " +
                                         "FROM CXN_OCUPACION " +
                                         "WHERE Nombre = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Name);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Lectura_Hora["Codigo"].ToString();
                            }
                            else
                            {
                                return "9210";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "9210";
            }
        }
        CXN_HCMG IFHIR.GetNAMG(int Admition)
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

                    String Query = "SELECT H.HC_Nota_Acl, R.RDAAmbulatorio, P.Pac_TipoId, P.Pac_IdNum, H.HC_Prof " +
                                   "FROM CXN_HCMG H " +
                                   "INNER JOIN CXN_RDA R ON H.HC_ADM = R.Admision " +
                                   "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                   "WHERE R.Admision = @param1 " +
                                   "AND H.HC_ADM = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Admition);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_HCMG H = new CXN_HCMG
                                {
                                    HC_Nota_Acl = Reader["HC_Nota_Acl"].ToString(),
                                    HC_DescHer = Reader["RDAAmbulatorio"].ToString(),
                                    HC_Pac = Reader["Pac_IdNum"].ToString(),
                                    HC_Ocupacion = Reader["Pac_TipoId"].ToString(),
                                    HC_Prof = Convert.ToInt32(Reader["HC_Prof"])
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        void IFHIR.InsertarLOG_RDA(CXN_RDA_LOG Respuesta)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_RDA_LOG (Admision, " +
                                                                  "Fecha, " +
                                                                  "Detalle, " +
                                                                  "Usuario, " +
                                                                  "Clase) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", Respuesta.Admision);
                    cmd.Parameters.AddWithValue("@param2", Convert.ToDateTime(Respuesta.Fecha));
                    cmd.Parameters.AddWithValue("@param3", Respuesta.Detalle);
                    cmd.Parameters.AddWithValue("@param4", Respuesta.Usuario);
                    cmd.Parameters.AddWithValue("@param5", Respuesta.Clase);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<CXN_RDA_LOG> IFHIR.GetLogs(DateTime Desde, DateTime Hasta)
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
                                   "FROM CXN_RDA_LOG " +
                                   "WHERE Fecha BETWEEN @param1 AND @param2 " +
                                   "ORDER BY Fecha, Admision DESC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde.Date));
                        Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_RDA_LOG> log = new List<CXN_RDA_LOG>();

                                while (Reader.Read()) 
                                {
                                    log.Add(new CXN_RDA_LOG 
                                    { 
                                        Admision = Convert.ToInt32(Reader["Admision"]),
                                        Fecha = Convert.ToDateTime(Reader["Fecha"]),
                                        Detalle = Reader["Detalle"].ToString(),
                                        Usuario = Reader["Usuario"].ToString(),
                                        Clase = Reader["Clase"].ToString(),
                                        Id = Convert.ToInt32(Reader["Id"])
                                    });                                    
                                }

                                return log;
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
        string IFHIR.GetLogFHIR(int Id)
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

                    String Query = "SELECT Detalle " +
                                   "FROM CXN_RDA_LOG " +
                                   "WHERE Id = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Id);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Detalle"].ToString();
                            }
                            else
                            {
                                return "SIN RESULTADOS";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        List<CXN_DATOS_FHIR> IFHIR.ListaDatosConfFHIR(int Prestador)
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
                                   "FROM CXN_DATOS_FHIR " +
                                   "WHERE Prestador = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Prestador);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_DATOS_FHIR> L = new List<CXN_DATOS_FHIR>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(new CXN_DATOS_FHIR 
                                    { 
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Llave = Reader["Llave"].ToString(),
                                        Prestador = Convert.ToInt32(Reader["Prestador"]),
                                        Valor = Reader["Valor"].ToString()
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
