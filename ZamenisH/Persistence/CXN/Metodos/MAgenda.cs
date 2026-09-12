using Domain;
using Domain.CXN;

using Persistence.CXN.Interfaces;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Persistence.CXN.Metodos
{
    public class MAgenda : IAgenda
    {
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IAgenda repositorioAgenda = new MAgenda();

        List<CXN_HORARIO> IAgenda.Asistencia(string ID)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_HoraC = "WITH MiSubConsulta AS ( " +
                                          "SELECT  " +
                                          "P.Pac_Id, " +
                                          "H.Hor_Estado as Estado, " +
                                          "H.Hor_Pac_Fecha_Cita as Fecha, " +
                                          "H.Hor_Pac_Hora_Cita as Hora, " +
                                          "H.Hor_Pac_Cup, " +
                                          "H.Hor_Pac_Ase, " +
                                          "H.Hor_Pac_Tipo_Serv, " +
                                          "B.Bod_Responsable as Profesional, " +
                                          "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN as Nombre, " +
                                          "H.Hor_Usr_Cancela, " +
                                         "H.Hor_Usr_Admisiona, " +
                                         "H.Hor_Pac_UsrGraba, " +
                                         "H.Hor_Pac_Modalidad, " +
                                         "H.Hor_Pac_MCancela, " +
                                         "H.Hor_Pac_RCancela, " +
                                         "H.Hor_CantSesion, " +
                                         "H.Hor_Autoriza, " +
                                         "H.Hor_Pac_Id, " +
                                         "H.Hor_Id, " +
                                         "H.Hor_Color, " +
                                         "H.Hor_Pac_Bod, " +
                                         "H.Hor_Pac_Minutos + ' ->RAZON: ' + H.Hor_Pac_Razon as Tarde, " +
                                         "H.Hor_IniciaSesion " +
                                         "FROM " +
                                         "CXN_PACIENTES P " +
                                         "JOIN " +
                                         "CXN_HORARIO H ON P.Pac_Id = H.Hor_Pac_Id " +
                                         "JOIN " +
                                         "CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "WHERE " +
                                         "P.Pac_IdNum = @param1 " +
                                         ") " +
                                         "SELECT P.Pac_Id, " +
                                         "T2.Estado, " +
                                         "T2.Fecha,  " +
                                         "T2.Hora,  " +
                                         "T2.Hor_Pac_Cup,  " +
                                         "T2.Hor_Pac_Ase,  " +
                                         "T2.Hor_Pac_Tipo_Serv, " +
                                         "T2.Profesional, " +
                                         "T2.Hor_Id, " +
                                         "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN as Nombre,  " +
                                         "T2.Hor_Usr_Cancela,  " +
                                         "T2.Hor_Usr_Admisiona,  " +
                                         "T2.Hor_Pac_UsrGraba,  " +
                                         "T2.Hor_Pac_Modalidad, " +
                                         "T2.Hor_Pac_MCancela, " +
                                         "T2.Hor_Pac_RCancela, " +
                                         "T2.Hor_CantSesion, " +
                                         "T2.Hor_Autoriza, " +
                                         "T2.Tarde, " +
                                         "T2.Hor_Color, " +
                                         "T2.Hor_Pac_Bod, " +
                                         "T2.Hor_Pac_Id " +
                                         "FROM " +
                                         "CXN_PACIENTES P " +
                                         "LEFT JOIN " +
                                         "MiSubConsulta T2 " +
                                         "ON P.Pac_Id = T2.Hor_Pac_Id " +
                                         "WHERE P.Pac_IdNum = @param1";

                    using (SqlCommand Carga_CommandC = new SqlCommand(Cargar_HoraC, con))
                    {
                        Carga_CommandC.Parameters.AddWithValue("@param1", ID);

                        using (SqlDataReader Lectura_HoraC = (Carga_CommandC.ExecuteReader()))
                        {
                            if (Lectura_HoraC.HasRows)
                            {
                                List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                                int ses = 0;

                                while (Lectura_HoraC.Read() == true)
                                {
                                    string Ser = "NO HAY INFORMACION";
                                    var Service = repositorioConvenios.ServicioNombre(Lectura_HoraC["Hor_Pac_Cup"].ToString(),
                                                                                      Convert.ToInt32(Lectura_HoraC["Hor_Pac_Ase"]),
                                                                                      Lectura_HoraC["Hor_Pac_Tipo_Serv"].ToString());
                                    if (Service != null)
                                    {
                                        Ser = Service.Con_Nombre.ToString();
                                    }         
                                        
                                    if (Lectura_HoraC["Hor_CantSesion"] == DBNull.Value ||
                                        Lectura_HoraC["Hor_CantSesion"].ToString() == "0" ||
                                        Lectura_HoraC["Hor_CantSesion"].ToString() == "")
                                    {
                                        ses--;
                                    }
                                    else
                                    {
                                        ses = Convert.ToInt32(Lectura_HoraC["Hor_CantSesion"]);
                                    }                                              

                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_AdmOpnened = ses.ToString(),
                                        Hor_Estado = Lectura_HoraC["Estado"].ToString(),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_HoraC["Fecha"]),
                                        Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_HoraC["Hora"]),
                                        Hor_Imp_Age = Lectura_HoraC["Nombre"].ToString(),
                                        Hor_Regimen = Lectura_HoraC["Profesional"].ToString(), //Profesional
                                        Hor_RegAtn = Ser, //Servicio Nombre
                                        Hor_IniciaSesion = Lectura_HoraC["Hor_CantSesion"].ToString(), //cantidad sesiones
                                        Hor_Pac_UsrGraba = Lectura_HoraC["Hor_Pac_UsrGraba"].ToString(),
                                        Hor_Usr_Admisiona = Lectura_HoraC["Hor_Usr_Admisiona"].ToString(),
                                        Hor_Usr_Cancela = Lectura_HoraC["Hor_Usr_Cancela"].ToString(),
                                        Hor_Pac_RCancela = Lectura_HoraC["Hor_Pac_MCancela"].ToString() + " - " + Lectura_HoraC["Hor_Pac_RCancela"].ToString(),
                                        Hor_Pac_Modalidad = Lectura_HoraC["Hor_Pac_Modalidad"].ToString(),
                                        Hor_Pac_Minutos = Lectura_HoraC["Tarde"].ToString(),
                                        Hor_Id = Convert.ToInt32(Lectura_HoraC["Hor_Id"]),
                                        Hor_Autoriza = Lectura_HoraC["Hor_Autoriza"].ToString(),
                                        Hor_Color = Lectura_HoraC["Hor_Color"] == DBNull.Value ? "C" : Lectura_HoraC["Hor_Color"].ToString(),
                                        Hor_Pac_Bod = Convert.ToInt32(Lectura_HoraC["Hor_Pac_Bod"]),
                                        Hor_Pac_Tipo_Serv = Lectura_HoraC["Hor_Pac_Tipo_Serv"].ToString(),
                                        Hor_Pac_Id = Convert.ToInt32(Lectura_HoraC["Hor_Pac_Id"])
                                        
                                    });
                                }

                                List<CXN_HORARIO> listaOrdenada = H
                                                .OrderByDescending(x => x.Hor_Pac_Fecha_Cita)
                                                .ThenBy(x => x.Hor_Pac_Hora_Cita)
                                                .ToList();

                                List<CXN_HORARIO> ultimos100 = listaOrdenada
                                    .Take(100)
                                    .ToList();

                                return ultimos100;
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
        DateTime? getLastDateIniciaSesion(string Documento)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT TOP 1 H.Hor_Pac_Fecha_Cita " +
                                   "FROM CXN_HORARIO H " +
                                   "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                   "WHERE P.Pac_IdNum = '" + Documento + "' " +
                                   "AND H.Hor_IniciaSesion = 'S' " +
                                   "ORDER BY H.Hor_Pac_Fecha_Cita DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]);
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
        List<CXN_HORARIO> IAgenda.AsistenciaLastAut(string ID)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime? getLastDate = getLastDateIniciaSesion(ID);

                    if (getLastDate == null)
                    {
                        return null;
                    }

                    String Cargar_HoraC = "WITH MiSubConsulta AS ( " +
                                          "SELECT  " +
                                          "P.Pac_Id, " +
                                          "H.Hor_Estado as Estado, " +
                                          "H.Hor_Pac_Fecha_Cita, " +
                                          "H.Hor_Pac_Hora_Cita as Hora, " +
                                          "H.Hor_Pac_Cup, " +
                                          "H.Hor_Pac_Ase, " +
                                          "H.Hor_Pac_Tipo_Serv, " +
                                          "B.Bod_Responsable as Profesional, " +
                                          "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN as Nombre, " +
                                          "H.Hor_Usr_Cancela, " +
                                         "H.Hor_Usr_Admisiona, " +
                                         "H.Hor_Pac_UsrGraba, " +
                                         "H.Hor_Pac_Modalidad, " +
                                         "H.Hor_Pac_MCancela, " +
                                         "H.Hor_Pac_RCancela, " +
                                         "H.Hor_CantSesion, " +
                                         "H.Hor_Pac_Id, " +
                                         "H.Hor_Autoriza, " +
                                         "H.Hor_Id, " +
                                         "H.Hor_IniciaSesion, " +
                                         "H.Hor_Pac_Minutos + ' ->RAZON: ' + H.Hor_Pac_Razon as Tarde " +
                                         "FROM " +
                                         "CXN_PACIENTES P " +
                                         "JOIN " +
                                         "CXN_HORARIO H ON P.Pac_Id = H.Hor_Pac_Id " +
                                         "JOIN " +
                                         "CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "WHERE " +
                                         "P.Pac_IdNum = '" + ID + "' " +
                                         ") " +
                                         "SELECT P.Pac_Id, " +
                                         "T2.Estado, " +
                                         "T2.Hor_Pac_Fecha_Cita,  " +
                                         "T2.Hora,  " +
                                         "T2.Hor_Pac_Cup,  " +
                                         "T2.Hor_Pac_Ase,  " +
                                         "T2.Hor_Pac_Tipo_Serv, " +
                                         "T2.Profesional, " +
                                         "T2.Hor_Id, " +
                                         "P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN as Nombre,  " +
                                         "T2.Hor_Usr_Cancela,  " +
                                         "T2.Hor_Usr_Admisiona,  " +
                                         "T2.Hor_Pac_UsrGraba,  " +
                                         "T2.Hor_Pac_Modalidad, " +
                                         "T2.Hor_Pac_MCancela, " +
                                         "T2.Hor_Pac_RCancela, " +
                                         "T2.Hor_CantSesion, " +
                                         "T2.Hor_IniciaSesion, " +
                                         "T2.Hor_Autoriza, " +
                                         "T2.Tarde " +
                                         "FROM " +
                                         "CXN_PACIENTES P " +
                                         "LEFT JOIN " +
                                         "MiSubConsulta T2 " +
                                         "ON P.Pac_Id = T2.Hor_Pac_Id " +
                                         "WHERE P.Pac_IdNum = '" + ID + "' " +
                                         "AND T2.Hor_Pac_Fecha_Cita >= '" + Convert.ToDateTime(getLastDate).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                         "ORDER BY T2.Hor_Pac_Fecha_Cita ASC";
                    SqlCommand Carga_CommandC = new SqlCommand(Cargar_HoraC, con);
                    SqlDataReader Lectura_HoraC = (Carga_CommandC.ExecuteReader());
                    if (Lectura_HoraC.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Lectura_HoraC.Read() == true)
                        {
                            string Ser = "NO HAY INFORMACION";
                            var Service = repositorioConvenios.ServicioNombre(Lectura_HoraC["Hor_Pac_Cup"].ToString(),
                                                                              Convert.ToInt32(Lectura_HoraC["Hor_Pac_Ase"]),
                                                                              Lectura_HoraC["Hor_Pac_Tipo_Serv"].ToString());
                            if (Service != null)
                            {
                                Ser = Service.Con_Nombre.ToString();
                            }

                            H.Add(new CXN_HORARIO
                            {
                                Hor_Estado = Lectura_HoraC["Estado"].ToString(),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_HoraC["Hor_Pac_Fecha_Cita"]),
                                Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_HoraC["Hora"]),
                                Hor_Imp_Age = Lectura_HoraC["Nombre"].ToString(),
                                Hor_Regimen = Lectura_HoraC["Profesional"].ToString(), //Profesional
                                Hor_RegAtn = Ser, //Servicio Nombre
                                Hor_IniciaSesion = Lectura_HoraC["Hor_CantSesion"].ToString(), //cantidad sesiones
                                Hor_Pac_UsrGraba = Lectura_HoraC["Hor_Pac_UsrGraba"].ToString(),
                                Hor_Usr_Admisiona = Lectura_HoraC["Hor_Usr_Admisiona"].ToString(),
                                Hor_Usr_Cancela = Lectura_HoraC["Hor_Usr_Cancela"].ToString(),
                                Hor_Pac_RCancela = Lectura_HoraC["Hor_Pac_MCancela"].ToString() + " - " + Lectura_HoraC["Hor_Pac_RCancela"].ToString(),
                                Hor_Pac_Modalidad = Lectura_HoraC["Hor_Pac_Modalidad"].ToString(),
                                Hor_Pac_Minutos = Lectura_HoraC["Tarde"].ToString(),
                                Hor_Id = Convert.ToInt32(Lectura_HoraC["Hor_Id"]),
                                Hor_Autoriza = Lectura_HoraC["Hor_Autoriza"].ToString()
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        string IAgenda.SearchCuracionForMG(int Paciente, DateTime Fecha, int Medico)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT B.Bod_Responsable " +
                                   "FROM CXN_HORARIO H " +
                                   "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                   "WHERE H.Hor_Pac_Bod <> '" + Medico + "' " +
                                   "AND H.Hor_Pac_Id = '" + Paciente + "' " +
                                   "AND H.Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(Fecha).ToString(getDataConection["Format_Fecha"]) + "' " +
                                   "AND H.Hor_Estado <> 'C'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> listaProf = new List<string>();
                        string listado = "";

                        while (Reader.Read() == true)
                        {
                            listaProf.Add(Reader["Bod_Responsable"].ToString());
                        }

                        foreach (var i in listaProf)
                        {
                            listado = listado + i + " | ";
                        }

                        return listado;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }
        List<CXN_HORARIO> IAgenda.CitasProximas(int PacId, DateTime Fecha)
        {
            try
            {
                Dictionary<string, string> getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Agenda2 = "SELECT TOP 50 H.Hor_Id, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, B.Bod_Responsable, C.Con_Nombre, H.Hor_Estado " +
                                             "FROM CXN_HORARIO H " +
                                             "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                             "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                             "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                             "AND C.Con_Aseguradora = P.Pac_Aseguradora " +
                                             "WHERE H.Hor_Pac_Id = '" + PacId + "' " +
                                             "AND H.Hor_Pac_Fecha_Cita > '" + Convert.ToDateTime(Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                             "AND H.Hor_Estado <> 'C' " +
                                             "ORDER BY H.Hor_Pac_Fecha_Cita ASC";
                    SqlCommand Carga_Agenda2 = new SqlCommand(Cargar_Agenda2, con);
                    SqlDataReader Lectura_Hora = (Carga_Agenda2.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            H.Add(new CXN_HORARIO
                            {
                                Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                Hor_Estado = Lectura_Hora["Hor_Estado"].ToString(),
                                Com_Nombre = Lectura_Hora["Con_Nombre"].ToString(),
                                Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString() //profesional
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
        List<CXN_HORARIO> IAgenda.CargarPrevios(int PacId)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Agenda2 = "SELECT TOP 50 H.Hor_Id, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, B.Bod_Responsable, C.Con_Nombre, H.Hor_Estado, H.Hor_IniciaSesion, H.Hor_Autoriza, H.Hor_CantSesion " +
                                             "FROM CXN_HORARIO H " +
                                             "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                             "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                             "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                             "AND C.Con_Aseguradora = P.Pac_Aseguradora " +
                                             "WHERE H.Hor_Pac_Id = @param1 " +
                                             "ORDER BY H.Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Carga_Agenda2 = new SqlCommand(Cargar_Agenda2, con))
                    {
                        Carga_Agenda2.Parameters.AddWithValue("@param1", PacId);

                        using (SqlDataReader Lectura_Hora = (Carga_Agenda2.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                        Hor_Estado = Lectura_Hora["Hor_Estado"].ToString(),
                                        Com_Nombre = Lectura_Hora["Con_Nombre"].ToString(),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(), //profesional
                                        Hor_IniciaSesion = Lectura_Hora["Hor_IniciaSesion"].ToString(),
                                        Hor_CantSesion = Lectura_Hora["Hor_CantSesion"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Hor_CantSesion"]),
                                        Hor_Autoriza = Lectura_Hora["Hor_Autoriza"].ToString()
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        int IAgenda.AgendarPaciente(CXN_HORARIO horario)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    bool? inicio = false;
                    
                    if (horario.Hor_AvisoCurInicio != null)
                    {
                        inicio = horario.Hor_AvisoCurInicio;
                    }

                    DateTime Hoy = DateTime.Now;
                    DateTime Hora_Cita = Convert.ToDateTime("01/01/2021 " + horario.Hor_Pac_Hora_Cita.ToString("HH:mm tt"));
                    DateTime Hor_Pac_Fecha = Convert.ToDateTime(Hoy.ToString(getDataConection["Format_Fecha"]));
                    DateTime Hor_Pac_Hora = Convert.ToDateTime(Hoy.ToString("HH:mm tt"));

                    int canti = (horario.Hor_BloqEspaces != 0 ? horario.Hor_BloqEspaces : 0);

                    string arrastra = string.IsNullOrEmpty(horario.Hor_ArrastraHistoria) ? "N" : horario.Hor_ArrastraHistoria;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HORARIO (Hor_Estado, " + //param1
                                                      "Hor_Pac_Id, " + //param2
                                                      "Hor_Pac_Bod, " + //param3
                                                      "Hor_Pac_Tipo_Serv, " + //param4
                                                      "Hor_Pac_Cia, " + //param5
                                                      "Hor_Pac_Ase, " + //param6
                                                      "Hor_Pac_Cup, " + //param7
                                                      "Hor_Pac_UsrGraba, " + //param8
                                                      "Hor_Imp_Age, " + //param9
                                                      "Hor_Pac_Fecha, " + //param10
                                                      "Hor_Pac_Fecha_Cita, " + //param11
                                                      "Hor_Pac_Hora, " + //param12
                                                      "Hor_Pac_Id_Hora, " + //param13
                                                      "Hor_Pac_Hora_Cita, " + //param14
                                                      "Hor_Observacion, " + //param15
                                                      "Hor_Pac_Sal, " +
                                                      "Hor_Vales, " +
                                                      "Hor_Pac_Modalidad, " +
                                                      "Hor_BloqEspaces, " +
                                                      "Hor_GrupoServicios, " +
                                                      "Hor_Regimen, " +
                                                      "Hor_ArrastraHistoria, " +
                                                      "Hor_AvisoCurInicio, " +
                                                      "Hor_Tipo_Paciente, " +
                                                      "Hor_Color) " + //param16
                         "values                  (@param1, " + // Hor_Estado
                                                  "@param2, " + // Hor_Pac_Id
                                                  "@param3, " + // Hor_Pac_Bod
                                                  "@param4, " + // Hor_Pac_Tipo_Serv
                                                  "@param5, " + // Hor_Pac_Cia
                                                  "@param6, " + // Hor_Pac_Ase
                                                  "@param7, " + // Hor_Pac_Cup
                                                  "@param8, " + // Hor_Pac_UsrGraba
                                                  "@param9, " + // Hor_Imp_Age
                                                  "@param10, " + // Hor_Pac_Fecha
                                                  "@param11, " + // Hor_Pac_Fecha_Cita
                                                  "@param12, " + // Hor_Pac_Hora
                                                  "@param13, " + // Hor_Pac_Id_Hora
                                                  "@param14, " + // Hor_Pac_Hora_Cita
                                                  "@param15, " + // Hor_Observacion
                                                  "@param16, " +
                                                  "@param17, " +
                                                  "@param18," +
                                                  "@param19," +
                                                  "@param20," +
                                                  "@param21, " +
                                                  "@param22, " +
                                                  "@param23, " +
                                                  "@param24, " +
                                                  "@param25); SELECT SCOPE_IDENTITY();", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", horario.Hor_Estado);
                    cmd.Parameters.AddWithValue("@param2", horario.Hor_Pac_Id);
                    cmd.Parameters.AddWithValue("@param3", horario.Hor_Pac_Bod);
                    cmd.Parameters.AddWithValue("@param4", horario.Hor_Pac_Tipo_Serv);
                    cmd.Parameters.AddWithValue("@param5", horario.Hor_Pac_Cia);
                    cmd.Parameters.AddWithValue("@param6", horario.Hor_Pac_Ase);
                    cmd.Parameters.AddWithValue("@param7", horario.Hor_Pac_Cup);
                    cmd.Parameters.AddWithValue("@param8", horario.Hor_Pac_UsrGraba);
                    cmd.Parameters.AddWithValue("@param9", horario.Hor_Imp_Age);
                    cmd.Parameters.Add(new SqlParameter("@param10", SqlDbType.DateTime)).Value = Hor_Pac_Fecha;
                    cmd.Parameters.Add(new SqlParameter("@param11", SqlDbType.DateTime)).Value = horario.Hor_Pac_Fecha_Cita;
                    cmd.Parameters.Add(new SqlParameter("@param12", SqlDbType.DateTime)).Value = Hor_Pac_Hora;
                    cmd.Parameters.AddWithValue("@param13", horario.Hor_Pac_Id_Hora);
                    cmd.Parameters.Add(new SqlParameter("@param14", SqlDbType.DateTime)).Value = Hora_Cita;
                    cmd.Parameters.AddWithValue("@param15", horario.Hor_Observacion);
                    cmd.Parameters.AddWithValue("@param16", horario.Hor_Pac_Sal);
                    cmd.Parameters.AddWithValue("@param17", horario.Hor_Vales);
                    cmd.Parameters.AddWithValue("@param18", horario.Hor_Pac_Modalidad);
                    cmd.Parameters.AddWithValue("@param19", canti); 
                    cmd.Parameters.AddWithValue("@param20", horario.Hor_GrupoServicios);
                    cmd.Parameters.AddWithValue("@param21", horario.Hor_Regimen);
                    cmd.Parameters.AddWithValue("@param22", arrastra);
                    cmd.Parameters.AddWithValue("@param23", inicio);
                    cmd.Parameters.AddWithValue("@param24", horario.Hor_Tipo_Paciente ?? "");
                    cmd.Parameters.AddWithValue("@param25", horario.Hor_Color ?? "");

                    object _primaryKey = cmd.ExecuteScalar(); 
                    if (Convert.ToInt32(_primaryKey) >= 1)
                    {
                        return Convert.ToInt32(_primaryKey);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return 0;
            }
        }
        int IAgenda.AgendarPacienteJuntas(CXN_HORARIO horario)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;
                    DateTime Hora_Cita = Convert.ToDateTime("01/01/2021 " + horario.Hor_Pac_Hora_Cita.ToString("HH:mm tt"));
                    DateTime Hor_Pac_Fecha = Convert.ToDateTime(Hoy.ToString(getDataConection["Format_Fecha"]));
                    DateTime Hor_Pac_Hora = Convert.ToDateTime(Hoy.ToString("HH:mm tt"));

                    int canti = (horario.Hor_BloqEspaces != 0 ? horario.Hor_BloqEspaces : 0);

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HORARIO (Hor_Estado, " + //param1
                                                      "Hor_Pac_Id, " + //param2
                                                      "Hor_Pac_Bod, " + //param3
                                                      "Hor_Pac_Tipo_Serv, " + //param4
                                                      "Hor_Pac_Cia, " + //param5
                                                      "Hor_Pac_Ase, " + //param6
                                                      "Hor_Pac_Cup, " + //param7
                                                      "Hor_Pac_UsrGraba, " + //param8
                                                      "Hor_Imp_Age, " + //param9
                                                      "Hor_Pac_Fecha, " + //param10
                                                      "Hor_Pac_Fecha_Cita, " + //param11
                                                      "Hor_Pac_Hora, " + //param12
                                                      "Hor_Pac_Id_Hora, " + //param13
                                                      "Hor_Pac_Hora_Cita, " + //param14
                                                      "Hor_Observacion, " + //param15
                                                      "Hor_Pac_Sal, " +
                                                      "Hor_Vales, " +
                                                      "Hor_Pac_Modalidad, " +
                                                      "Hor_BloqEspaces, " +
                                                      "Hor_GrupoServicios, " +
                                                      "Hor_Regimen, " +
                                                      "Hor_Pac_Llegada, " +
                                                      "Hor_Pac_Atendido, " +
                                                      "Hor_Valida, " +
                                                      "Hor_Usr_Admisiona, " +
                                                      "Hor_CantSesion, " +
                                                      "Hor_IniciaSesion, " +
                                                      "Hor_AdmOpnened) " + //param16
                         "values                  (@param1, " + // Hor_Estado
                                                  "@param2, " + // Hor_Pac_Id
                                                  "@param3, " + // Hor_Pac_Bod
                                                  "@param4, " + // Hor_Pac_Tipo_Serv
                                                  "@param5, " + // Hor_Pac_Cia
                                                  "@param6, " + // Hor_Pac_Ase
                                                  "@param7, " + // Hor_Pac_Cup
                                                  "@param8, " + // Hor_Pac_UsrGraba
                                                  "@param9, " + // Hor_Imp_Age
                                                  "@param10, " + // Hor_Pac_Fecha
                                                  "@param11, " + // Hor_Pac_Fecha_Cita
                                                  "@param12, " + // Hor_Pac_Hora
                                                  "@param13, " + // Hor_Pac_Id_Hora
                                                  "@param14, " + // Hor_Pac_Hora_Cita
                                                  "@param15, " + // Hor_Observacion
                                                  "@param16, " +
                                                  "@param17, " +
                                                  "@param18," +
                                                  "@param19," +
                                                  "@param20," +
                                                  "@param21," +
                                                  "@param22," +
                                                  "@param23," +
                                                  "@param24," +
                                                  "@param25," +
                                                  "@param26," +
                                                  "@param27," +
                                                  "@param28); SELECT SCOPE_IDENTITY();", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", horario.Hor_Estado);
                    cmd.Parameters.AddWithValue("@param2", horario.Hor_Pac_Id);
                    cmd.Parameters.AddWithValue("@param3", horario.Hor_Pac_Bod);
                    cmd.Parameters.AddWithValue("@param4", horario.Hor_Pac_Tipo_Serv);
                    cmd.Parameters.AddWithValue("@param5", horario.Hor_Pac_Cia);
                    cmd.Parameters.AddWithValue("@param6", horario.Hor_Pac_Ase);
                    cmd.Parameters.AddWithValue("@param7", horario.Hor_Pac_Cup);
                    cmd.Parameters.AddWithValue("@param8", horario.Hor_Pac_UsrGraba);
                    cmd.Parameters.AddWithValue("@param9", horario.Hor_Imp_Age);
                    cmd.Parameters.Add(new SqlParameter("@param10", SqlDbType.DateTime)).Value = Hor_Pac_Fecha;
                    cmd.Parameters.Add(new SqlParameter("@param11", SqlDbType.DateTime)).Value = horario.Hor_Pac_Fecha_Cita;
                    cmd.Parameters.Add(new SqlParameter("@param12", SqlDbType.DateTime)).Value = Hor_Pac_Hora;
                    cmd.Parameters.AddWithValue("@param13", horario.Hor_Pac_Id_Hora);
                    cmd.Parameters.Add(new SqlParameter("@param14", SqlDbType.DateTime)).Value = Hora_Cita;
                    cmd.Parameters.AddWithValue("@param15", horario.Hor_Observacion);
                    cmd.Parameters.AddWithValue("@param16", horario.Hor_Pac_Sal);
                    cmd.Parameters.AddWithValue("@param17", horario.Hor_Vales);
                    cmd.Parameters.AddWithValue("@param18", horario.Hor_Pac_Modalidad);
                    cmd.Parameters.AddWithValue("@param19", canti);
                    cmd.Parameters.AddWithValue("@param20", horario.Hor_GrupoServicios);

                    cmd.Parameters.AddWithValue("@param21", horario.Hor_Regimen);
                    cmd.Parameters.Add(new SqlParameter("@param22", SqlDbType.DateTime)).Value = horario.Hor_Pac_Llegada;
                    cmd.Parameters.Add(new SqlParameter("@param23", SqlDbType.DateTime)).Value = horario.Hor_Pac_Atendido;
                    cmd.Parameters.AddWithValue("@param24", horario.Hor_Valida);
                    cmd.Parameters.AddWithValue("@param25", horario.Hor_Usr_Admisiona);
                    cmd.Parameters.AddWithValue("@param26", horario.Hor_CantSesion);
                    cmd.Parameters.AddWithValue("@param27", horario.Hor_IniciaSesion);
                    cmd.Parameters.AddWithValue("@param28", horario.Hor_AdmOpnened);

                    object _primaryKey = cmd.ExecuteScalar();
                    if (Convert.ToInt32(_primaryKey) >= 1)
                    {
                        return Convert.ToInt32(_primaryKey);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return 0;
            }
        }
        CXN_HORARIO IAgenda.getLastHorToCopy(int Admision)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Agenda = "SELECT * " +
                                           "FROM CXN_HORARIO " +
                                           "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Carga_Agenda = new SqlCommand(Cargar_Agenda, con);
                    SqlDataReader getHO = (Carga_Agenda.ExecuteReader());
                    if (getHO.Read() == true)
                    {
                        CXN_HORARIO H = new CXN_HORARIO
                        {
                            Hor_Estado = "H",
                            Hor_Pac_Id = Convert.ToInt32(getHO["Hor_Pac_Id"]),
                            Hor_Pac_Bod = Convert.ToInt32(getHO["Hor_Pac_Bod"]),
                            Hor_Pac_Tipo_Serv = getHO["Hor_Pac_Tipo_Serv"].ToString(),
                            Hor_Pac_Cia = Convert.ToInt32(getHO["Hor_Pac_Cia"]),
                            Hor_Pac_Ase = Convert.ToInt32(getHO["Hor_Pac_Ase"]),
                            Hor_Pac_Cup = getHO["Hor_Pac_Cup"].ToString(),
                            Hor_Imp_Age = getHO["Hor_Imp_Age"].ToString(),
                            Hor_Pac_Id_Hora = "0000",
                            Hor_Observacion = "ARRASTRE AUTOMATICO NOTAS DE ENFERMERIA",
                            Hor_Pac_Sal = getHO["Hor_Pac_Sal"].ToString(),
                            Hor_Vales = getHO["Hor_Vales"].ToString(),
                            Hor_Pac_Modalidad = getHO["Hor_Pac_Modalidad"].ToString(),
                            Hor_Pac_Fecha_Cita = Convert.ToDateTime(getHO["Hor_Pac_Fecha_Cita"]),
                            Hor_Regimen = getHO["Hor_Regimen"].ToString()
                        };

                        return H;
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
        int IAgenda.getLastIDToCopy(int Paciente)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Agenda = "SELECT Hor_Id " +
                                           "FROM CXN_HORARIO " +
                                           "WHERE Hor_Pac_Id = '" + Paciente + "' " +
                                           "AND Hor_Pac_Tipo_Serv = 'MG' " +
                                           "ORDER BY Hor_Id DESC";
                    SqlCommand Carga_Agenda = new SqlCommand(Cargar_Agenda, con);
                    SqlDataReader Lectura_Agenda = (Carga_Agenda.ExecuteReader());
                    if (Lectura_Agenda.Read() == true)
                    {
                        return Convert.ToInt32(Lectura_Agenda["Hor_Id"]);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        string IAgenda.Observacioprevia(int Admision)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Cargar_Agenda = "SELECT Hor_Observacion " +
                                       "FROM CXN_HORARIO " +
                                       "WHERE Hor_Id = '" + Admision + "'";
                SqlCommand Carga_Agenda = new SqlCommand(Cargar_Agenda, con);
                SqlDataReader Lectura_Agenda = (Carga_Agenda.ExecuteReader());
                if (Lectura_Agenda.Read() == true)
                {
                    return Lectura_Agenda["Hor_Observacion"].ToString();
                }
                else
                {
                    return "";
                }
            }
        }        
        CXN_HORARIO IAgenda.DatosforMailSMS(int Admision)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Clave = "SELECT H.Hor_Estado, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Ase, H.Hor_Imp_Age, H.Hor_Pac_Cia, " +
                                          "H.Hor_Pac_Hora_Cita, B.Bod_Responsable, P.Pac_Email, C.Con_Nombre, CI.Com_Nombre, CI.Com_Direccion, CI.Com_Telefono, " +
                                          "P.Pac_Telefono, CI.Com_Nombre_SMS, CI.Com_Telefono_SMS, H.Hor_Pac_Id " +
                                          "FROM CXN_HORARIO H " +
                                          "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                          "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                          "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                          "INNER JOIN CXN_CIA CI ON H.Hor_Pac_Cia = CI.Com_Identificador " +
                                          "WHERE H.Hor_Id = @Admision " +
                                          "AND H.Hor_Pac_Ase = C.Con_Aseguradora";

                    using (SqlCommand Carga_Clave = new SqlCommand(Cargar_Clave, con))
                    {
                        Carga_Clave.Parameters.AddWithValue("@Admision", Admision);

                        using (SqlDataReader Lectura_Clave = (Carga_Clave.ExecuteReader()))
                        {
                            if (Lectura_Clave.Read() == true)
                            {
                                CXN_HORARIO H = new CXN_HORARIO
                                {
                                    Hor_Pac_Cia = Convert.ToInt32(Lectura_Clave["Hor_Pac_Cia"]),
                                    Hor_Estado = Lectura_Clave["Hor_Estado"].ToString(),
                                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Clave["Hor_Pac_Fecha_Cita"]),
                                    Hor_Pac_Ase = Convert.ToInt32(Lectura_Clave["Hor_Pac_Ase"]),
                                    Hor_Imp_Age = Lectura_Clave["Hor_Imp_Age"].ToString(),
                                    Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Clave["Hor_Pac_Hora_Cita"]),
                                    Hor_Pac_Razon = Lectura_Clave["Bod_Responsable"].ToString(), //Profesional que atiende
                                    Hor_Observacion = Lectura_Clave["Pac_Email"].ToString(), //Email paciente
                                    Com_Nombre = Lectura_Clave["Com_Nombre"].ToString(),
                                    Hor_Pac_Inasistencia = Lectura_Clave["Con_Nombre"].ToString(), //nombre servicio
                                    Com_Direccion = Lectura_Clave["Com_Direccion"].ToString(),
                                    Com_Telefono = Lectura_Clave["Com_Telefono"].ToString(),
                                    Hor_Pac_Id = Convert.ToInt32(Lectura_Clave["Hor_Pac_Id"]),
                                    Com_Nombre_SMS = Lectura_Clave["Com_Nombre_SMS"].ToString(),
                                    Com_Telefono_SMS = Lectura_Clave["Com_Telefono_SMS"].ToString(),
                                    Hor_RegAtn = Lectura_Clave["Pac_Telefono"].ToString() //telefono paciente
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
            catch
            {
                return null;
            }
        }
        void IAgenda.desbloquearEspacio(string Usuario, int Admision)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string getPreviaOb = repositorioAgenda.Observacioprevia(Admision) + " ||| CITA CANCELADA POR " + Usuario;

                string Busqueda = "UPDATE CXN_HORARIO " +
                                   "SET Hor_Estado = 'C', " +
                                   "Hor_Usr_Cancela = '" + Usuario + "', " +
                                   "Hor_Observacion = '" + getPreviaOb + "' " +
                                   "WHERE Hor_Id = '" + Admision + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IAgenda.InicioControlCuraciones(int Admision, string estado)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_Pac_Sal = '" + estado + "' " +
                                      "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        string IAgenda.consularAdmisionEstado(int Admision)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Cargar_Hora = "SELECT Hor_Estado " +
                                     "FROM CXN_HORARIO " +
                                     "WHERE Hor_Id = @param1";
                using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                {
                    Carga_Command.Parameters.AddWithValue("@param1", Admision);

                    using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                    {
                        if (Lectura_Hora.Read() == true)
                        {
                            return Lectura_Hora["Hor_Estado"].ToString();
                        }
                        else
                        {
                            return "0";
                        }
                    }
                }                
            }
        }
        int IAgenda.consularMGMismoDia(int Pacientes, DateTime Fecha)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT TOP 1 Hor_Id " + 
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Id = @param1 " +
                                         "AND Hor_Pac_Fecha_Cita = @param2 " +
                                         "AND Hor_Estado <> 'C' " +
                                         "AND Hor_Pac_Sal = 'C'";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Pacientes);
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Fecha).ToString(getDataConection["Format_Fecha"]));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Hor_Id"]);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }                                          
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        void IAgenda.anularAdmision(int Admision, string UserLogged)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string getObPrev = repositorioAgenda.Observacioprevia(Admision) + " ||| ADMISION ANULADA POR " + UserLogged;

                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Estado = @param1, " +
                                  "Hor_RcCaja = @param2, " +
                                  "Hor_ValDerechos = @param2, " +
                                  "Hor_Autoriza = @param2, " +
                                  "Hor_CantSesion = @param3, " +
                                  "Hor_IniciaSesion = @param3, " +
                                  "Hor_RegAtn = @param2, " +
                                  "Hor_Valida = @param2, " +
                                  "Hor_Observacion = @param4 " +
                                  "WHERE Hor_Id = @param5 " +
                                  "AND Hor_Estado = @param6";

                using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                {
                    Accion.Parameters.AddWithValue("@param1", "A");
                    Accion.Parameters.AddWithValue("@param2", "");
                    Accion.Parameters.AddWithValue("@param3", DBNull.Value);
                    Accion.Parameters.AddWithValue("@param4", getObPrev);
                    Accion.Parameters.AddWithValue("@param5", Admision);
                    Accion.Parameters.AddWithValue("@param6", "P");
                    Accion.ExecuteNonQuery();

                    ConsultarPacSal_A(Admision);
                }                    
            }
        }
        void IAgenda.ActualizarAutorizacionMG(int Admision, string Autoriza, int Cantidad)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Autoriza = @param1, " +
                                  "Hor_CantSesion = @param2 " +
                                  "WHERE Hor_Id = @param3";

                using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                {
                    Accion.Parameters.AddWithValue("@param1", Autoriza);
                    Accion.Parameters.AddWithValue("@param2", Cantidad);
                    Accion.Parameters.AddWithValue("@param3", Admision);
                    Accion.ExecuteNonQuery();
                }
            }
        }
        void ConsultarPacSal_A(int Adm)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Pac_Sal = @param1 " +
                                  "WHERE Hor_Id = @param2 " +
                                  "AND Hor_Pac_Sal = @param3";

                using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                {
                    Accion.Parameters.AddWithValue("@param1", "");
                    Accion.Parameters.AddWithValue("@param2", Adm);
                    Accion.Parameters.AddWithValue("@param3", "A");
                    Accion.ExecuteNonQuery();
                }                
            }
        }
        List<CXN_CIA> HistoricoCitas(int Admision)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    CXN_HORARIO pacid = repositorioAgenda.getLastHorToCopy(Admision);
                    if (pacid == null)
                    {
                        return null;
                    }

                    String Cargar_Hora = "SELECT C.Com_Nombre, C.Com_Direccion, C.Com_Telefono, C.Com_Logo, C.Com_Identificacion, H.Hor_Pac_Hora_Cita, H.Hor_Pac_Fecha_Cita, H.Hor_Id, " +
                                         "A.Ase_Descripcion, B.Bod_Responsable, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, CO.Con_Nombre, P.Pac_TipoId + ' ' + P.Pac_IdNum AS Iddd " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CONVENIOS CO ON H.Hor_Pac_Cup = CO.Con_Id_Serv " +
                                         "WHERE H.Hor_Pac_Id = '" + pacid.Hor_Pac_Id + "' " +
                                         "AND H.Hor_Pac_Fecha_Cita <= '" + Convert.ToDateTime(pacid.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                         "AND H.Hor_Estado = 'H' " +
                                         "AND CO.Con_Aseguradora = A.Ase_Identificador " +
                                         "ORDER BY H.Hor_Pac_Fecha_Cita DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CIA> class_Certificado = new List<CXN_CIA>();

                        while (Lectura_Hora.Read() == true)
                        {                           
                            string L = Lectura_Hora["Com_Logo"].ToString();
                            Byte[] bytes = Convert.FromBase64String(L);

                            MemoryStream stmBLOBData = new MemoryStream(bytes);
                            PictureBox pic = new PictureBox();
                            pic.Image = Image.FromStream(stmBLOBData);

                            string Estatico = "El paciente " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() +
                                              " con documento de identidad " + Lectura_Hora["Iddd"].ToString() + ", ha asistido a nuestra institución médica en los dias relacionados a continuación: ";

                            class_Certificado.Add(new CXN_CIA
                            {
                                Com_Nombre = Lectura_Hora["Com_Nombre"].ToString(),
                                Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                Com_Cod_Prestador = Estatico.TrimEnd().TrimStart(), //Texto_Estatico
                                Com_Cod_Prestador_2 = Lectura_Hora["Bod_Responsable"].ToString(),
                                FechaBase = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                Com_UsuarioGraba = Lectura_Hora["Con_Nombre"].ToString(),
                                Logo = repositorioGenerales.GetBytes(pic.Image),
                                Com_DE = Convert.ToInt32(Lectura_Hora["Hor_Id"])
                            });
                        }
                     
                        return class_Certificado;
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
        List<CXN_CIA> IAgenda.certificadoAsistencia(int Admision, string Texto)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    if (Texto == "Historic") { return HistoricoCitas(Admision); }

                    String Cargar_Hora = "SELECT TOP 1 C.Com_Nombre, C.Com_Direccion, C.Com_Telefono, C.Com_Logo, C.Com_Identificacion, H.Hor_Pac_Hora_Cita, H.Hor_Pac_Fecha_Cita, " +
                                         "A.Ase_Descripcion, B.Bod_Responsable, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "WHERE H.Hor_Id = '" + Admision + "' " +
                                         "AND H.Hor_Estado = 'H'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<CXN_CIA> class_Certificado = new List<CXN_CIA>();
                        string L = Lectura_Hora["Com_Logo"].ToString();
                        Byte[] bytes = Convert.FromBase64String(L);

                        MemoryStream stmBLOBData = new MemoryStream(bytes);
                        PictureBox pic = new PictureBox();
                        pic.Image = Image.FromStream(stmBLOBData);

                        string Variable = "";
                        StreamReader leido = File.OpenText("C:\\Cxn\\Certificado.txt");
                        string contenido = null;
                        contenido = leido.ReadToEnd();
                        Variable = contenido.ToString();
                        leido.Close();

                        string Estatico = "El paciente " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() +
                                          " asistio a cita medica el dia " + Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]).ToString(getDataConection["Format_Fecha"]) + " a las " + Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]).ToString("HH:mm tt") +
                                          " con el profesional " + Lectura_Hora["Bod_Responsable"].ToString() + " bajo el convenio de " + Lectura_Hora["Ase_Descripcion"].ToString();

                        string tex = Texto;

                        if (Texto == "")
                        {
                            tex = Variable.TrimEnd().TrimStart();
                        }

                        class_Certificado.Add(new CXN_CIA
                        {
                            Com_Nombre = Lectura_Hora["Com_Nombre"].ToString(),
                            Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                            Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                            Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                            Com_Cod_Prestador = Estatico.TrimEnd().TrimStart(), //Texto_Estatico
                            Com_Cod_Prestador_2 = tex.ToString().TrimEnd().TrimStart(),  //Texto_Variable
                            Logo = repositorioGenerales.GetBytes(pic.Image)
                        });

                        return class_Certificado;
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
        List<CXN_HORARIO> IAgenda.CargarAgenda(int Medico, int Compañia, DateTime Desde, string Dia)
        {
            try
            {
                Dictionary<string, string> getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    using (SqlCommand cmd = new SqlCommand("ConsultarAgenda", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Med", Medico);
                        cmd.Parameters.AddWithValue("@Dia", Dia);
                        cmd.Parameters.AddWithValue("@HorPacCia", Compañia);
                        cmd.Parameters.AddWithValue("@HorPacFechaCita", Convert.ToDateTime(Desde).ToString(getDataConection["Format_Fecha"]));

                        using (SqlDataReader Lectura_Hora = cmd.ExecuteReader())
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    int pacid = (Lectura_Hora["Hor_Pac_Id"] != DBNull.Value ? Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]) : 0);
                                    CXN_PACIENTES PacTemp = repositorioPacientes.LlamarPacientebyId(pacid);                                    

                                    string PAC;
                                    string Estado = "";
                                    string Paciente = "";
                                    string saleconsulta = "";
                                    string Novedadtemporal = "";

                                    string ImaTempSMS = null;
                                    string ImaTempEmail = null;

                                    DateTime Hora = new DateTime();

                                    if (Lectura_Hora["Hor_Estado"] != DBNull.Value) { Estado = Lectura_Hora["Hor_Estado"].ToString(); }
                                    if (Lectura_Hora["Hor_Imp_Age"] != DBNull.Value) { Paciente = Lectura_Hora["Hor_Imp_Age"].ToString(); }
                                    if (Lectura_Hora["Hora"] != DBNull.Value) { Hora = Convert.ToDateTime(Lectura_Hora["Hora"]); }
                                    if (Lectura_Hora["SALECONSULTA"] != DBNull.Value) { saleconsulta = Lectura_Hora["SALECONSULTA"].ToString(); }

                                    if (Estado == "B" && Paciente == "")
                                    {
                                        PAC = "ESPACIO BLOQUEADO";
                                        ImaTempSMS = Conexion.BloqueosAgenda;
                                        ImaTempEmail = Conexion.BloqueosAgenda;
                                    }
                                    else if (Paciente == "")
                                    {
                                        PAC = "";
                                        ImaTempSMS = Conexion.BloqueosAgenda;
                                        ImaTempEmail = Conexion.BloqueosAgenda;
                                    }
                                    else
                                    {
                                        PAC = Paciente;
                                        ImaTempSMS = Conexion.SMSAgenda;
                                        ImaTempEmail = Conexion.EmailAgenda;
                                    }

                                    string TPaciente = (Lectura_Hora["Hor_Tipo_Paciente"] != DBNull.Value ? Lectura_Hora["Hor_Tipo_Paciente"].ToString() : "");

                                    Novedadtemporal = (TPaciente == "N" ? " ||| NUEVO" : "");
                                    Novedadtemporal = Novedadtemporal + (PacTemp != null ? (PacTemp.Pac_2VXS == "S" ? " \r ||| AGENDAR DOS VECES POR SEMANA" : "") : "");
                                    Novedadtemporal = Novedadtemporal + (PacTemp != null ? (PacTemp.Pac_Doble == "S" ? " \r ||| PACIENTE REQUIERE DOS ESPACIOS DE AGENDA" : "") : "");
                                    Novedadtemporal = Novedadtemporal + (PacTemp != null ? (PacTemp.Pac_Especial == "S" ? " \r ||| PACIENTE DE TRATAMIENTO ESPECIAL" : "") : "");

                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_Pac_Id_Hora = (Lectura_Hora["Ide"] != DBNull.Value ? Lectura_Hora["Ide"].ToString() : ""),
                                        Hor_Autoriza = (Lectura_Hora["Habilita"] != DBNull.Value ? Lectura_Hora["Habilita"].ToString() : ""),
                                        Hor_Pac_Hora_Cita = Convert.ToDateTime(Hora),
                                        Hor_Pac_Sal = (Lectura_Hora["Hor_Id"] != DBNull.Value ? Lectura_Hora["Hor_Id"].ToString() : ""),
                                        Hor_Imp_Age = PAC,
                                        Hor_Estado = Estado,
                                        Hor_Pac_Modalidad = (Lectura_Hora["Hor_Pac_Modalidad"] != DBNull.Value ? Lectura_Hora["Hor_Pac_Modalidad"].ToString() : ""),
                                        Hor_Pac_Minutos = (Lectura_Hora["Hor_Pac_Minutos"] != DBNull.Value ? Lectura_Hora["Hor_Pac_Minutos"].ToString() : ""),
                                        Hor_Observacion = (Lectura_Hora["Hor_Observacion"] != DBNull.Value ? Lectura_Hora["Hor_Observacion"].ToString() : ""),
                                        Hor_ValDerechos = (Lectura_Hora["Ase_Descripcion"] != DBNull.Value ? Lectura_Hora["Ase_Descripcion"].ToString() : ""),
                                        ImageEmail = Convert.FromBase64String(ImaTempEmail),
                                        ImageSMS = Convert.FromBase64String(ImaTempSMS),
                                        Hor_IniciaSesion = (Lectura_Hora["Hor_IniciaSesion"] != DBNull.Value ? Lectura_Hora["Hor_IniciaSesion"].ToString() : ""),
                                        Hor_Pac_Cup = (Lectura_Hora["Hor_Pac_Cup"] != DBNull.Value ? Lectura_Hora["Hor_Pac_Cup"].ToString() : ""),
                                        Hor_Pac_Razon = (Lectura_Hora["Hor_Pac_Sal"] != DBNull.Value ? Lectura_Hora["Hor_Pac_Sal"].ToString() : ""), //este seria el pac sal
                                        Hor_Tipo_Paciente = TPaciente,
                                        Hor_Pac_Id = pacid,
                                        Hor_BloqEspaces = (Lectura_Hora["Hor_BloqEspaces"] != DBNull.Value ? Convert.ToInt32(Lectura_Hora["Hor_BloqEspaces"]) : 0),
                                        SALECONSULTA = saleconsulta,
                                        HorObservaTemp = Novedadtemporal,
                                        DatosPaciente = PacTemp,
                                        Hor_ArrastraHistoria = string.IsNullOrEmpty(Lectura_Hora["Hor_ArrastraHistoria"].ToString()) ? "N" : Lectura_Hora["Hor_ArrastraHistoria"].ToString(),
                                        Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString()
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
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = "BackEnd",
                    Metodo = "CargarAgenda",
                    Usuario = "Persistencia"
                };

                OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }  
        Dictionary<string, string> IAgenda.SugerenciaServicio(int Paciente)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT TOP 1 HC_ServCatalogo, HC_CupCatalogo " +
                                    "FROM CXN_HCMG " +
                                    "WHERE HC_PacId = '" + Paciente + "' " +
                                    "ORDER BY HC_Fecha DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        Dictionary<string, string> DIC = new Dictionary<string, string>();
                        DIC.Add("Servicio", Reader["HC_ServCatalogo"].ToString());
                        DIC.Add("Cup", Reader["HC_CupCatalogo"].ToString());
                        return DIC;
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
        bool IAgenda.changeProfesional(int Admision, int Bodega, string IdHora, string Observacion, DateTime fechaCita, DateTime hora)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hora = Convert.ToDateTime("01/01/2021 " + hora.ToString("HH:mm tt"));
                    DateTime fCita = Convert.ToDateTime(fechaCita);

                    string Busqueda = (@"UPDATE CXN_HORARIO " +
                                      "SET Hor_Pac_Bod = @Bod, " +
                                      "Hor_Pac_Hora_Cita = @HoraCita, " +
                                      "Hor_Pac_Fecha_Cita = @FechaCita, " +
                                      "Hor_Pac_Id_Hora = @IDHora, " +
                                      "Hor_Observacion = @Observacion " +
                                      "WHERE Hor_Id = '" + Admision + "' " +
                                      "AND Hor_Estado = 'A'");
                    SqlCommand Accion = new SqlCommand(Busqueda, con);

                    Accion.Parameters.Add(new SqlParameter("@Bod", Bodega));
                    Accion.Parameters.Add(new SqlParameter("@HoraCita", SqlDbType.DateTime)).Value = Hora;
                    Accion.Parameters.Add(new SqlParameter("@IDHora", IdHora));
                    Accion.Parameters.Add(new SqlParameter("@Observacion", Observacion));
                    Accion.Parameters.Add(new SqlParameter("@FechaCita", SqlDbType.DateTime)).Value = fechaCita.Date;
                    Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        void IAgenda._updateAseHorario(int Ase, string Pac, int Adm)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda2 = "UPDATE CXN_HORARIO " +
                                       "SET Hor_Pac_Ase = @param1, " +
                                       "Hor_Imp_Age = @param2 " +
                                       "WHERE Hor_Id = @param3";

                    using (SqlCommand Accion2 = new SqlCommand(Busqueda2, con))
                    {
                        Accion2.Parameters.AddWithValue("@param1", Ase);
                        Accion2.Parameters.AddWithValue("@param2", Pac);
                        Accion2.Parameters.AddWithValue("@param3", Adm);
                        Accion2.ExecuteNonQuery();
                    }                       
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }            
        }
        void IAgenda.ActualizaAdmision(string Cup, int Adm)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET " +
                                      "Hor_Pac_Cup = '" + Cup + "' " +
                                      "WHERE Hor_Id = '" + Adm + "' " +
                                      "AND Hor_Estado = 'A'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        bool IAgenda.updateCitaAdmisionar(CXN_HORARIO H)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {

                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string getObPrev = repositorioAgenda.Observacioprevia(H.Hor_Id) + H.Hor_Observacion;

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                          "SET Hor_Estado = @param1, " +
                                          "Hor_Autoriza = @param2, " +
                                          "Hor_Valida = @param3, " +
                                          "Hor_Pac_Ase = @param4, " +
                                          "Hor_Imp_Age = @param5, " +
                                          "Hor_Regimen = @param6, " +
                                          "Hor_Pac_LLegada = @param7, " +
                                          "Hor_Usr_Admisiona = @param8, " +
                                          "Hor_Pac_Minutos = @param9, " +
                                          "Hor_Pac_Razon = @param10, " +
                                          "Hor_Pac_Cup = @param11, " +
                                          "Hor_RegAtn = @param12, " +
                                          "Hor_CantSesion = @param13, " +
                                          "Hor_IniciaSesion = @param14, " +
                                          "Hor_Observacion = @param15, " +
                                          "Hor_ValDerechos = @param16, " +
                                          "Hor_Vales = @param17, " +
                                          "Hor_Color = @param20 " +
                                          "WHERE Hor_Id = @param18 " +
                                          "AND Hor_Estado = @param19";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", H.Hor_Estado);
                    Accion.Parameters.AddWithValue("@param2", H.Hor_Autoriza);
                    Accion.Parameters.AddWithValue("@param3", H.Hor_Valida);
                    Accion.Parameters.AddWithValue("@param4", H.Hor_Pac_Ase);
                    Accion.Parameters.AddWithValue("@param5", H.Hor_Imp_Age);
                    Accion.Parameters.AddWithValue("@param6", H.Hor_Regimen);
                    Accion.Parameters.AddWithValue("@param7", Convert.ToDateTime(H.Hor_Pac_Llegada).ToString("HH:mm"));
                    Accion.Parameters.AddWithValue("@param8", H.Hor_Usr_Admisiona);
                    Accion.Parameters.AddWithValue("@param9", H.Hor_Pac_Minutos);
                    Accion.Parameters.AddWithValue("@param10", H.Hor_Pac_Razon);
                    Accion.Parameters.AddWithValue("@param11", H.Hor_Pac_Cup);
                    Accion.Parameters.AddWithValue("@param12", H.Hor_RegAtn);
                    Accion.Parameters.AddWithValue("@param13", H.Hor_CantSesion);
                    Accion.Parameters.AddWithValue("@param14", H.Hor_IniciaSesion);
                    Accion.Parameters.AddWithValue("@param15", getObPrev);
                    Accion.Parameters.AddWithValue("@param16", H.Hor_ValDerechos);
                    Accion.Parameters.AddWithValue("@param17", H.Hor_Vales);
                    Accion.Parameters.AddWithValue("@param18", H.Hor_Id);
                    Accion.Parameters.AddWithValue("@param19", "A");
                    Accion.Parameters.AddWithValue("@param20", string.IsNullOrEmpty(H.Hor_Color) ? "" : H.Hor_Color);

                    int Guarda = Accion.ExecuteNonQuery();
                    return Guarda > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        void IAgenda.PendientesChecked(int Adm, string Estado)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {

                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_Pac_Sal = '" + Estado + "', " +
                                      "Hor_IniciaSesion = 'S' " +
                                      "WHERE Hor_Id = '" + Adm + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IAgenda.CancelacionInterna(string Razon, string User, int HorId, string Motivo)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Estado = 'C', " +
                                  "Hor_Pac_MCancela = '" + Motivo + "', " +
                                  "Hor_Pac_RCancela = '" + Razon + "', " +
                                  "Hor_Autoriza = '', " +
                                  "Hor_Usr_Cancela = '" + User + "', " +
                                  "Hor_ValDerechos = '' " +
                                  "WHERE Hor_Id = '" + HorId + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IAgenda.Inasistencia_Cita(int horid, string razon)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Pac_Inasistencia = '" + razon + "' " +
                                  "WHERE Hor_Id = '" + horid + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IAgenda.Retardo_Cita(string Minutos, string Razon, int horid)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Pac_Minutos = '" + Minutos + "', " +
                                  "Hor_Pac_Razon = '" + Razon + "' " +
                                  "WHERE Hor_Id = '" + horid + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IAgenda.addObservation(int Admision, string Observacion)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string horario = "UPDATE CXN_HORARIO " +
                                 "SET Hor_Estado = 'P', " +
                                 "Hor_Observacion = '" + Observacion + "' " +
                                 "WHERE Hor_Id = '" + Admision + "'";
                SqlCommand commandhorario = new SqlCommand(horario, con);
                int Guarda2;
                Guarda2 = commandhorario.ExecuteNonQuery();
            }
        }
        void IAgenda.deleteHistoria(int Admision, string TipServ)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                if (TipServ == "CU")
                {
                    string nota = "DELETE FROM CXN_NOTAS " +
                                   "WHERE Not_Adm = '" + Admision + "'";
                    SqlCommand commandnota = new SqlCommand(nota, con);
                    int Guardanota;
                    Guardanota = commandnota.ExecuteNonQuery();
                }

                if (TipServ == "MG")
                {
                    string mg = "DELETE FROM CXN_HCMG " +
                                "WHERE HC_Adm = '" + Admision + "'";
                    SqlCommand commandmg = new SqlCommand(mg, con);
                    int Guardamg;
                    Guardamg = commandmg.ExecuteNonQuery();
                }

                if (TipServ == "FI")
                {
                    string fi = "DELETE FROM CXN_HCFI " +
                                "WHERE HC_Adm = '" + Admision + "'";
                    SqlCommand commandfi = new SqlCommand(fi, con);
                    int Guardafi;
                    Guardafi = commandfi.ExecuteNonQuery();
                }

                if (TipServ == "TO" || TipServ == "PS" || TipServ == "TF")
                {
                    string evoluciones = "DELETE FROM CXN_EVOFIB " +
                                "WHERE Evo_Adm = '" + Admision + "'";
                    SqlCommand commandevoluciones = new SqlCommand(evoluciones, con);
                    int Guardaevoluciones;
                    Guardaevoluciones = commandevoluciones.ExecuteNonQuery();
                }

                if (TipServ == "TO")
                {
                    string to = "DELETE FROM CXN_HCTO " +
                                "WHERE HC_Adm = '" + Admision + "'";
                    SqlCommand commandto = new SqlCommand(to, con);
                    int Guardato;
                    Guardato = commandto.ExecuteNonQuery();
                }

                if (TipServ == "TF")
                {
                    string tf = "DELETE FROM CXN_HCTF " +
                                "WHERE HC_Adm = '" + Admision + "'";
                    SqlCommand commandtf = new SqlCommand(tf, con);
                    int Guardatf;
                    Guardatf = commandtf.ExecuteNonQuery();
                }

                if (TipServ == "PS")
                {
                    string ps = "DELETE FROM CXN_HCPSI " +
                                "WHERE HC_Adm = '" + Admision + "'";
                    SqlCommand commandps = new SqlCommand(ps, con);
                    int Guardaps;
                    Guardaps = commandps.ExecuteNonQuery();
                }

                if (TipServ == "TF")
                {
                    string jm = "DELETE FROM CXN_HCJUNTAS " +
                                "WHERE Jun_Adm = '" + Admision + "'";
                    SqlCommand commandjm = new SqlCommand(jm, con);
                    int Guardajm;
                    Guardajm = commandjm.ExecuteNonQuery();
                }

            }
        }
        void IAgenda.addFestivo(DateTime Fecha, string Motivo)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand(@"Insert into CXN_FECHAS (F_Fecha, " + //param1
                                                             "F_Razon) " + //param16
                                    "values                  (@param1, " + // Hor_Estado
                                                             "@param2)", con); // Hor_Pac_Sal

                cmd.Parameters.AddWithValue("@param2", Motivo);
                cmd.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Fecha; // Fecha que graba cita
                cmd.ExecuteNonQuery();
            }
        }
        void IAgenda.ConsumirAdmision(int Admition)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_Estado = 'H' " +
                                      "WHERE Hor_Id = '" + Admition + "' " +
                                      "AND Hor_Estado = 'P'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IAgenda.Graba_Hora_Atencion(int Atention)
        {
            Dictionary<string,string> getData = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getData["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hora_Atendido = DateTime.Now;
                Hora_Atendido = Convert.ToDateTime(Hora_Atendido.ToString("HH:mm"));

                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Pac_Atendido = '" + Convert.ToDateTime(Hora_Atendido).ToString("HH:mm") + "' " +
                                  "WHERE Hor_Id = '" + Atention + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IAgenda.Graba_Hora_Salida(int Atention)
        {
            Dictionary<string, string> getData = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getData["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hora_Atendido = DateTime.Now;
                Hora_Atendido = Convert.ToDateTime(Hora_Atendido.ToString("HH:mm"));

                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET Hor_Pac_Hora_Salida = '" + Convert.ToDateTime(Hora_Atendido).ToString("HH:mm") + "' " +
                                  "WHERE Hor_Id = '" + Atention + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IAgenda.OPend(CXN_OPEND OP)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_OPEND (OP_Adm, " + //param1
                                                                 "OP_Estado, " +
                                                                 "OP_Registra) " + //param16
                                        "values                  (@param1, " + // Hor_Estado
                                                                 "@param2, " +
                                                                 "@param3)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", OP.OP_Adm);
                    cmd.Parameters.AddWithValue("@param2", OP.OP_Estado);
                    cmd.Parameters.AddWithValue("@param3", OP.OP_Registra);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<CXN_HORARIO> IAgenda.getListPendientes()
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT H.Hor_Imp_Age, H.Hor_Id, H.Hor_Pac_Fecha_Cita, O.OP_Estado, O.OP_Registra " +
                                   "FROM CXN_HORARIO H " +
                                   "INNER JOIN CXN_OPEND O ON H.Hor_Id = O.OP_Adm " +
                                   "WHERE O.OP_Estado = 'P' " +
                                   "ORDER BY H.Hor_Pac_Fecha_Cita DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_HORARIO> L = new List<CXN_HORARIO>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_HORARIO
                            {
                                Hor_Imp_Age = Reader["Hor_Imp_Age"].ToString(),
                                Hor_Id = Convert.ToInt32(Reader["Hor_Id"]),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]),
                                Hor_Estado = Reader["OP_Estado"].ToString(),
                                Hor_Observacion = Reader["OP_Registra"].ToString()
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
        void IAgenda.OPendUpdate(CXN_OPEND O)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_OPEND " +
                                      "SET OP_Estado = '" + O.OP_Estado + "', " +
                                      "OP_Cambia = '" + O.OP_Cambia + "', " +
                                      "OP_EstadoChange = '" + Convert.ToDateTime(O.OP_EstadoChange).ToString(getDataConection["Format_Fecha"]) + "' " +
                                      "WHERE OP_Adm = '" + O.OP_Adm + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IAgenda.OpenAdmition(int Admision, string Estado)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_AdmOpnened = '" + Estado + "' " +
                                      "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IAgenda.updateAseguradoraFromCargo(int Asegura, int Admision)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Busqueda = "UPDATE CXN_HORARIO " +
                                   "SET Hor_Pac_Ase = '" + Asegura + "' " +
                                   "WHERE Hor_Id = '" + Admision + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IAgenda.updateObservaTemp(string ObTemp, int Admision)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Busqueda = "UPDATE CXN_HORARIO " +
                                  "SET HorObservaTemp = '" + ObTemp + "' " +
                                  "WHERE Hor_Id = '" + Admision + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        bool IAgenda.addSALECONSULTA(int Admision)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = (@"UPDATE CXN_HORARIO " +
                                      "SET SALECONSULTA = @param1 " +
                                      "WHERE Hor_Id = @param2");
                    SqlCommand Accion = new SqlCommand(Busqueda, con);

                    Accion.Parameters.Add(new SqlParameter("@param1", "A"));
                    Accion.Parameters.Add(new SqlParameter("@param2", Admision));
                    int f = Accion.ExecuteNonQuery();
                    return (f >= 1 ? true : false);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        void IAgenda.UpdatecolorCita(DateTime FechaCita, int Paciente, string Color)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_Color = @param1 " +
                                      "WHERE Hor_Pac_Fecha_Cita = @param2 " +
                                      "AND Hor_Pac_Id = @param3 " +
                                      "AND Hor_Estado <> @param4";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", Color);
                        Accion.Parameters.AddWithValue("@param2", Convert.ToDateTime(FechaCita));
                        Accion.Parameters.AddWithValue("@param3", Paciente);
                        Accion.Parameters.AddWithValue("@param4", "C");

                        Accion.ExecuteNonQuery();
                    }                        
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
