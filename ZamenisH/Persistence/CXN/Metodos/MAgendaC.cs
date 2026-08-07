using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MAgendaC : IAgendaC
    {
        private static readonly ILogin rpoBod = new MLogin();
        private static readonly IConvenios repoConvenios = new MConvenios();

        private string Recepcion;
        bool IAgendaC.insert016(int pacid, DateTime fecha)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Cargar_Agenda = "SELECT TOP 1 Hor_Id " +
                                       "FROM CXN_HORARIO " +
                                       "WHERE Hor_Pac_Id = '" + pacid + "' " +
                                       "AND Hor_Estado IN ('A','P','H') " +
                                       "AND Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(fecha).ToString(getCon["Format_Fecha"]) + "' " +
                                       "ORDER BY Hor_Id DESC";

                using (SqlCommand Carga_Agenda = new SqlCommand(Cargar_Agenda, con))
                {
                    using (SqlDataReader Lectura_Agenda = (Carga_Agenda.ExecuteReader()))
                    {
                        if (Lectura_Agenda.Read() == true)
                        {
                            int Hor_Id = Convert.ToInt32(Lectura_Agenda["Hor_Id"]);

                            string Busqueda = "UPDATE CXN_HORARIO " +
                                              "SET Hor_Pac_Solicita = '" + Convert.ToDateTime(fecha).ToString(getCon["Format_Fecha"]) + "', " +
                                              "Hor_Tipo_Paciente = 'N' " +
                                              "WHERE Hor_Id = '" + Hor_Id + "'";

                            SqlCommand Accion = new SqlCommand(Busqueda, con);
                            int Guarda;
                            Guarda = Accion.ExecuteNonQuery();

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }                
            }
        }
        void IAgendaC.CancelacionInterna(string Razon, string User, int HorId, string Motivo)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
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
        Dictionary<string, string> IAgendaC.SugerenciaServicio(int Paciente)
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
        string IAgendaC.Calcular2(int Paciente, string TipoServicio)
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
                    String Query = "SELECT TOP 1 Hor_Id, Hor_CantSesion " +
                                    "FROM CXN_HORARIO " +
                                    "WHERE Hor_Pac_Id = @param1 " +
                                    "AND Hor_Pac_Tipo_Serv = @param2 " +
                                    "AND Hor_IniciaSesion = 'S' " +
                                    "AND Hor_Estado IN ('P', 'H') " +
                                    "AND Hor_CantSesion <> '0' " +
                                    "ORDER BY Hor_Id DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Paciente);
                        Commando.Parameters.AddWithValue("@param2", TipoServicio);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                var Can = Cantidad(Convert.ToInt32(Reader["Hor_Id"]), Paciente);
                                int CanRec = Can + 1;

                                if (this.Recepcion != "YES")
                                {
                                    return "Sesion " + CanRec.ToString() + " de " + Reader["Hor_CantSesion"].ToString();
                                }

                                return "Sesion " + CanRec.ToString() + " de " + Reader["Hor_CantSesion"].ToString();
                            }
                            else
                            {
                                return "No es posible calcular sesion debido a que no hay registros de autorizaciones vigentes";
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
        int Cantidad(int Hor_Id, int Hor_Pac_Id)
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
                    String Query = "SELECT COUNT(*) AS Total " +
                                    "FROM CXN_HORARIO " +
                                    "WHERE Hor_Pac_Id = '" + Hor_Pac_Id + "' " +
                                    "AND Hor_Pac_Tipo_Serv = 'CU' " +
                                    "AND Hor_Estado IN ('P', 'H') " +
                                    "AND Hor_Id >= '" + Hor_Id + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Convert.ToInt32(Reader["Total"]);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
        void IAgendaC.setRecepcion(string _valor)
        {
            this.Recepcion = _valor;
        }
        (string Autorizacion, string Cantidad, string Estado) IAgendaC.CitaMismoDiaGetAutorizacion(int PacId, DateTime fecha, int Bodega)
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

                    String Cargar_Hora = "Select Hor_Autoriza, Hor_CantSesion, Hor_Pac_Sal " +
                                         "FROM CXN_HORARIO  " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero  " +
                                         "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_Cia.Com_Identificador  " +
                                         "WHERE Hor_Pac_Id = @param1 " +
                                         "AND Hor_Pac_Fecha_Cita = @param2 " +
                                         "AND Hor_Pac_Tipo_Serv IN ('MG','CU') " +
                                         "AND Hor_Pac_Bod <> @param3 " +
                                         "AND Hor_Estado <> @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", PacId);
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(fecha).ToString(getCon["Format_Fecha"]));
                        Carga_Command.Parameters.AddWithValue("@param3", Bodega);
                        Carga_Command.Parameters.AddWithValue("@param4", "C");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                if (Lectura_Hora["Hor_Autoriza"] != DBNull.Value && Lectura_Hora["Hor_CantSesion"] != DBNull.Value)
                                {
                                    return (Lectura_Hora["Hor_Autoriza"].ToString(), Lectura_Hora["Hor_CantSesion"].ToString(), Lectura_Hora["Hor_Pac_Sal"].ToString());
                                }

                                return ("", "", "");                               
                            }
                            else
                            {
                                return ("", "", "");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ("", "", "");
            }
        }
        List<CXN_HORARIO> IAgendaC.ListarCitasXPaciente(int PacId, DateTime fecha)
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

                    String Cargar_Hora = "Select Hor_Id, Hor_Pac_Hora_Cita, Hor_Imp_Age, Hor_Estado, Bod_Responsable, Com_Nombre, Hor_Pac_Fecha_Cita " +
                                         "FROM CXN_HORARIO  " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero  " +
                                         "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_Cia.Com_Identificador  " +
                                         "WHERE Hor_Pac_Id = '" + PacId + "' " +
                                         "AND Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(fecha).ToString(getCon["Format_Fecha"]) + "' " +
                                         "AND Hor_Estado <> 'C'";

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
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(), //Profesional en este caso
                                        Hor_Estado = Lectura_Hora["Hor_Estado"].ToString(),
                                        Hor_Pac_Razon = Lectura_Hora["Com_Nombre"].ToString() //Prestador en este caso
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
            catch
            {
                return null;
            }
        }
        List<CXN_HORARIO> IAgendaC.ListarCitasXPaciente(int PacId, DateTime fecha, int Bodega)
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

                    String Cargar_Hora = "Select Hor_Id, Hor_Pac_Tipo_Serv, Hor_Pac_Hora_Cita, Hor_Imp_Age, Hor_Estado, Bod_Responsable, Com_Nombre, Hor_Pac_Fecha_Cita " +
                                         "FROM CXN_HORARIO  " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero  " +
                                         "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_Cia.Com_Identificador  " +
                                         "WHERE Hor_Pac_Id = '" + PacId + "' " +
                                         "AND Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(fecha).ToString(getCon["Format_Fecha"]) + "' " +
                                         "AND Hor_Estado = 'A' " +
                                         "AND Hor_Pac_Bod <> '" + Bodega + "'";

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
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(), //Profesional en este caso
                                        Hor_Estado = Lectura_Hora["Hor_Estado"].ToString(),
                                        Hor_Pac_Razon = Lectura_Hora["Com_Nombre"].ToString(), //Prestador en este caso
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
            catch
            {
                return null;
            }
        }
        List<CXN_HORARIO> IAgendaC.ListarCitasXPaciente2(int PacId, DateTime fecha, int Bodega)
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

                    String Cargar_Hora = "Select Hor_Id, Hor_Pac_Tipo_Serv, Hor_Pac_Hora_Cita, Hor_Imp_Age, Hor_Estado, Bod_Responsable, Com_Nombre, Hor_Pac_Fecha_Cita, " +
                                         "Hor_Autoriza " +
                                         "FROM CXN_HORARIO  " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero  " +
                                         "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_Cia.Com_Identificador  " +
                                         "WHERE Hor_Pac_Id = @param1 " +
                                         "AND Hor_Pac_Fecha_Cita = @param2 " +
                                         "AND Hor_Pac_Bod <> @param3 " +
                                         "AND hor_Estado <> @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", PacId);
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(fecha).ToString(getCon["Format_Fecha"]));
                        Carga_Command.Parameters.AddWithValue("@param3", Bodega);
                        Carga_Command.Parameters.AddWithValue("@param4", "C");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(), //Profesional en este caso
                                        Hor_Estado = Lectura_Hora["Hor_Estado"].ToString(),
                                        Hor_Pac_Razon = Lectura_Hora["Com_Nombre"].ToString(), //Prestador en este caso
                                        Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                        Hor_Autoriza = Lectura_Hora["Hor_Autoriza"] == DBNull.Value || 
                                                       Lectura_Hora["Hor_Autoriza"].ToString() == "" ?
                                                       "" : Lectura_Hora["Hor_Autoriza"].ToString()
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
            catch
            {
                return null;
            }
        }
        string IAgendaC.Extract(string input, int len, int ini)
        {
            if (string.IsNullOrEmpty(input) || input.Length < len)
            {
                return input;
            };

            return input.Substring(ini, len);
        }
        int IAgendaC.ConsultarFecha(DateTime _fecha, int IdProf)
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

                    String Query = "SELECT F_Estado " +
                                   "FROM CXN_DIAS_WEB " +
                                   "WHERE F_Fecha = '" + Convert.ToDateTime(_fecha).ToString(getCon["Format_Fecha"]) + "' " +
                                   "AND F_Prof = '" + IdProf + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        if (Reader["F_Estado"].ToString() == "B")
                        {
                            return 2;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
        List<CXN_HORARIO> IAgendaC.ObtenerCitasDelDia(int Prestador, DateTime FechaCita, int Profesional)
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

                    String Cargar_Hora = "SELECT A.Ase_Descripcion, H.Hor_Pac_Id_Hora, H.Hor_Estado, H.Hor_Id, " +
                                         "H.Hor_Pac_Id, H.Hor_Pac_Tipo_Serv, H.Hor_Color, H.Hor_BloqEspaces, H.Hor_Imp_Age, " +
                                         "P.Pac_Doble, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PacNombre " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "WHERE H.Hor_Pac_Cia = @param1 " +
                                         "AND H.Hor_Pac_Bod = @param2 " +
                                         "AND H.Hor_Estado <> @param3 " +
                                         "AND H.Hor_Pac_Fecha_Cita BETWEEN @param4 AND @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);
                        Carga_Command.Parameters.AddWithValue("@param2", Profesional);
                        Carga_Command.Parameters.AddWithValue("@param3", "C");
                        Carga_Command.Parameters.AddWithValue("@param4", new DateTime(FechaCita.Year, FechaCita.Month, FechaCita.Day, 00, 00, 00));
                        Carga_Command.Parameters.AddWithValue("@param5", new DateTime(FechaCita.Year, FechaCita.Month, FechaCita.Day, 23, 59, 59));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> Lista = new List<CXN_HORARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    Lista.Add(new CXN_HORARIO
                                    {
                                        PacienteNombre = Lectura_Hora["Hor_Imp_Age"] == DBNull.Value ? "" : Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Hor_Imp_Age = Lectura_Hora["PacNombre"].ToString(),
                                        Hor_Pac_Id_Hora = Lectura_Hora["Hor_Pac_Id_Hora"].ToString(),
                                        Hor_Estado = Lectura_Hora["Hor_Estado"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Pac_Id = Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]),
                                        Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                        Hor_Color = Lectura_Hora["Hor_Color"] == DBNull.Value ? "" : Lectura_Hora["Hor_Color"].ToString(),
                                        Hor_BloqEspaces = Lectura_Hora["Hor_BloqEspaces"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Hor_BloqEspaces"]),
                                        Hor_Valida = Lectura_Hora["Pac_Doble"] == DBNull.Value ? "" : Lectura_Hora["Pac_Doble"].ToString(),
                                    });
                                }

                                return Lista;
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
        otrosDatosPacienteHorario IAgendaC.cargarAdmision(int Admision, string filter)
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

                    String Cargar_Hora = "SELECT TOP 1 H.Hor_Pac_Bod, H.Hor_Pac_Id, P.Pac_TipoId, P.Pac_IdNum, CI.Com_Identificador, C.Con_Nombre, CI.Com_Telefono, CI.Com_Direccion, CI.Com_Identificacion, CI.Com_Logo, B.Bod_Responsable, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, A.Ase_Descripcion, CI.Com_Nombre, H.Hor_Imp_Age, H.Hor_ArrastraHistoria, H.Hor_DocFEModeradorCUFE, " +
                                         "P.Pac_Email, P.Pac_Telefono, P.Pac_TelefonoAux, P.Pac_Id, H.Hor_Observacion, H.Hor_Autoriza, H.Hor_Imp_Age, P.Pac_Dep_Cod, P.Pac_Mun_Cod, P.Pac_Sexo, P.Pac_Regimen, P.Pac_FechaNto, H.Hor_Pac_Ase, " +
                                         "P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, H.Hor_Vales, H.Hor_Pac_Tipo_Serv, H.Hor_Pac_Cup, H.Hor_Pac_Fecha_Cita, H.Hor_ValDerechos, H.Hor_RcCaja, H.Hor_RegAtn, H.Hor_Usr_Admisiona, " +
                                         "H.Hor_Pac_Fecha, H.Hor_Pac_Hora, H.HorTecnoSalud, H.Hor_Pac_Cia, H.Hor_CantSesion, H.Hor_Pac_Atendido, H.Hor_Estado, H.Hor_Pac_Llegada, H.Hor_Pac_Sal, P.Pac_Zona, P.Pac_Contrato, P.Pac_PaisOrigen, P.Pac_Residencia, H.Hor_Pac_UsrGraba, H.HorObservaTemp, P.Pac_Categoria, P.Pac_ECivil, P.Pac_Acudiente, P.Pac_Parentesco, P.Pac_Direccion, P.Pac_DireccionAcu, Pac_TelefonoAcu, Pac_CorreoAcu, P.VIH, P.Hepatitis, P.Pac_Ocupacion, " +
                                         "P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, B.Bod_Reg_Med, H.Hor_Pac_Modalidad, H.Hor_GrupoServicios, A.Ase_Cod_Emp, C.Con_CodServicio, H.Hor_AvisoCurInicio, H.Hor_Pac_Id_Hora, H.Hor_Color, P.Pac_Bonos, " +
                                         "H.Hor_Tipo_Paciente " +
                                         "FROM CXN_HORARIO H  " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero  " +
                                         "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA CI ON H.Hor_Pac_Cia = CI.Com_Identificador  " +
                                         "WHERE H.Hor_Id = @param1 " +
                                         "AND H.Hor_Estado IN (" + filter + ")";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                string arrastra = string.IsNullOrEmpty(Lectura_Hora["Hor_ArrastraHistoria"].ToString()) ? "N" : Lectura_Hora["Hor_ArrastraHistoria"].ToString();

                                otrosDatosPacienteHorario H = new otrosDatosPacienteHorario
                                {
                                    Pac_Bonos = Lectura_Hora["Pac_Bonos"] == DBNull.Value ? "N" : Lectura_Hora["Pac_Bonos"].ToString(),
                                    Hor_AvisoCurInicio = Lectura_Hora["Hor_AvisoCurInicio"] == DBNull.Value ? false : (bool)Lectura_Hora["Hor_AvisoCurInicio"],
                                    Hor_Estado = Lectura_Hora["Hor_Estado"] == DBNull.Value ? "" : Lectura_Hora["Hor_Estado"].ToString(),
                                    Hor_GrupoServicios = Lectura_Hora["Hor_GrupoServicios"].ToString(),
                                    Hor_Pac_Modalidad = Lectura_Hora["Hor_Pac_Modalidad"].ToString(),
                                    PrimerApellido = Lectura_Hora["Pac_PrimerA"].ToString(),
                                    SegundoApellido = Lectura_Hora["Pac_SegundoA"].ToString(),
                                    PrimerNombre = Lectura_Hora["Pac_PrimerN"].ToString(),
                                    SegundoNombre = Lectura_Hora["Pac_SegundoN"].ToString(),
                                    IdentificacionProfesional = Lectura_Hora["Bod_Reg_Med"].ToString(),

                                    Hor_Color = Lectura_Hora["Hor_Color"] == DBNull.Value ? "" : Lectura_Hora["Hor_Color"].ToString(),
                                    Hor_Pac_Id_Hora = Lectura_Hora["Hor_Pac_Id_Hora"] == DBNull.Value ? "" : Lectura_Hora["Hor_Pac_Id_Hora"].ToString(),
                                    Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                    Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                    Hor_Pac_Cup = Lectura_Hora["Hor_Pac_Cup"].ToString(),
                                    Hor_Regimen = Lectura_Hora["Pac_Regimen"].ToString(),
                                    Hor_Pac_Id = Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]),
                                    Hor_Pac_Bod = Convert.ToInt32(Lectura_Hora["Hor_Pac_Bod"]),
                                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                    Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                    Hor_Observacion = Lectura_Hora["Hor_Observacion"].ToString(),
                                    Hor_Vales = Lectura_Hora["Hor_Vales"] == DBNull.Value ? "N" : Lectura_Hora["Hor_Vales"].ToString(),
                                    Com_Nombre = Lectura_Hora["Com_Nombre"].ToString(),
                                    Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                    Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                    Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                    Com_Identificador = Convert.ToInt32(Lectura_Hora["Com_Identificador"]),
                                    PacienteAseguradora = Lectura_Hora["Ase_Cod_Emp"].ToString(),
                                    HorTecnoSalud = Lectura_Hora["HorTecnoSalud"] == DBNull.Value ? "15" : Lectura_Hora["HorTecnoSalud"].ToString(),
                                    Com_Doc_Soporte = Lectura_Hora["Con_CodServicio"] == DBNull.Value ? 328 : Convert.ToInt32(Lectura_Hora["Con_CodServicio"]),

                                    PacienteDireccion = Lectura_Hora["Pac_Direccion"] == DBNull.Value ? "" : Lectura_Hora["Pac_Direccion"].ToString(),
                                    Pac_TipoId = Lectura_Hora["Pac_TipoId"].ToString(),
                                    Pac_IdNum = Lectura_Hora["Pac_IdNum"].ToString(),
                                    Pac_PrimerN = Lectura_Hora["Pac_PrimerN"].ToString(),
                                    Pac_SegundoN = Lectura_Hora["Pac_SegundoN"].ToString(),
                                    Pac_PrimerA = Lectura_Hora["Pac_PrimerA"].ToString(),
                                    Pac_SegundoA = Lectura_Hora["Pac_SegundoA"].ToString(),
                                    Pac_Telefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                    Pac_TelefonoAux = Lectura_Hora["Pac_TelefonoAux"].ToString(),
                                    Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                    Pac_Dep_Cod = Lectura_Hora["Pac_Dep_Cod"].ToString(),
                                    Pac_Mun_Cod = Lectura_Hora["Pac_Mun_Cod"].ToString(),
                                    Bod_Responsable = Lectura_Hora["Bod_Responsable"].ToString(),
                                    Pac_FechaNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"]), 
                                    Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                    Hor_ValDerechos = Lectura_Hora["Hor_ValDerechos"].ToString(),
                                    Hor_RcCaja = (string.IsNullOrEmpty(Lectura_Hora["Hor_RcCaja"].ToString()) ? "0" : Lectura_Hora["Hor_RcCaja"].ToString()),
                                    Hor_RegAtn = Lectura_Hora["Hor_RegAtn"].ToString(),
                                    Hor_Usr_Admisiona = Lectura_Hora["Hor_Usr_Admisiona"].ToString(),
                                    Hor_Pac_Fecha = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha"]),
                                    Hor_Pac_Hora = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora"]),
                                    Com_Nombre_SMS = Lectura_Hora["Con_Nombre"].ToString(),
                                    Com_Telefono_SMS = Lectura_Hora["Ase_Descripcion"].ToString(),
                                    Hor_Autoriza = Lectura_Hora["Hor_Autoriza"].ToString(),
                                    Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                    Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                    Hor_Pac_Sal = Lectura_Hora["Hor_Pac_Sal"].ToString(),
                                    Pac_Zona = (Lectura_Hora["Pac_Zona"] != DBNull.Value ? Lectura_Hora["Pac_Zona"].ToString() : "U"),
                                    Pac_Contrato = Lectura_Hora["Pac_Contrato"].ToString(),
                                    Pac_PaisOrigen = Lectura_Hora["Pac_PaisOrigen"].ToString(), //pais
                                    Pac_PaisResidencia = Lectura_Hora["Pac_Residencia"].ToString(), //pais
                                    Com_Logo = Lectura_Hora["Com_Logo"].ToString(),
                                    Hor_Pac_UsrGraba = Lectura_Hora["Hor_Pac_UsrGraba"].ToString(),
                                    HorObservaTemp = Lectura_Hora["HorObservaTemp"].ToString(),
                                    Pac_Categoria = Lectura_Hora["Pac_Categoria"].ToString(),
                                    Pac_ECivilLoadAdmition = Lectura_Hora["Pac_Ecivil"].ToString(),
                                    Pac_AcudienteLoadAdmition = Lectura_Hora["Pac_Acudiente"].ToString(),
                                    Pac_CorreoLoadAdmition = Lectura_Hora["Pac_CorreoAcu"].ToString(),
                                    Pac_DireccionLoadAdmition = Lectura_Hora["Pac_DireccionAcu"].ToString(),
                                    Pac_ParentescoLoadAdmition = Lectura_Hora["Pac_Parentesco"].ToString(),
                                    Pac_TelefonoLoadAdmition = Lectura_Hora["Pac_TelefonoAcu"].ToString(),
                                    Hor_ArrastraHistoria = arrastra,
                                    Hor_DocFEModeradorCUFE = Lectura_Hora["Hor_DocFEModeradorCUFE"].ToString(),
                                    Pac_Ocupacion = Lectura_Hora["Pac_Ocupacion"] == DBNull.Value ? "" : Lectura_Hora["Pac_Ocupacion"].ToString(),     
                                    Hor_Tipo_Paciente = Lectura_Hora["Hor_Tipo_Paciente"] == DBNull.Value ? "" : Lectura_Hora["Hor_Tipo_Paciente"].ToString()                                    
                                };

                                H.VIH = (Lectura_Hora["VIH"] == DBNull.Value ? "Negativo" : Lectura_Hora["VIH"].ToString() == "P" ? "Positivo" : "Negativo");
                                H.Hepatitis = (Lectura_Hora["Hepatitis"] == DBNull.Value ? "Negativo" : Lectura_Hora["Hepatitis"].ToString());
                                H.Hor_CantSesion = (Lectura_Hora["Hor_CantSesion"] != DBNull.Value ? Convert.ToInt32(Lectura_Hora["Hor_CantSesion"]) : 0);

                                if (Lectura_Hora["Hor_Estado"].ToString() == "P")
                                {
                                    H.Hor_Pac_Llegada = (Lectura_Hora["Hor_Pac_Llegada"] != null ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Llegada"]) : new DateTime(1900, 01, 01, 00, 00, 00));
                                }

                                if (Lectura_Hora["Hor_Estado"].ToString() == "H")
                                {
                                    if (Lectura_Hora["Hor_Pac_Atendido"] != DBNull.Value)
                                    {
                                        H.Hor_Pac_Atendido = (Lectura_Hora["Hor_Pac_Atendido"] != null ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Atendido"]) : new DateTime(1900, 01, 01, 00, 00, 00));
                                        H.Hor_Pac_Llegada = (!string.IsNullOrEmpty(Lectura_Hora["Hor_Pac_Llegada"].ToString()) ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Llegada"]) : new DateTime(1900, 01, 01, 00, 00, 00));
                                    }
                                    else
                                    {
                                        DateTime g = new DateTime(1900, 01, 01, 00, 00, 00);
                                        H.Hor_Pac_Atendido = Convert.ToDateTime(g);

                                        if (string.IsNullOrEmpty(Lectura_Hora["Hor_Pac_Llegada"].ToString()))
                                        {
                                            H.Hor_Pac_Llegada = Convert.ToDateTime(g);
                                        }
                                        else
                                        {
                                            H.Hor_Pac_Llegada = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Llegada"]);
                                        }                                        
                                    }
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
        bool IAgendaC.addAutroizacion(int horid, string autroizacion, int Cantidad)
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

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_Autoriza = '" + autroizacion + "', " +
                                      "Hor_CantSesion = '" + Cantidad + "', " +
                                      "Hor_IniciaSesion = 'S' " +
                                      "WHERE Hor_Id = '" + horid + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        bool IAgendaC.addValidacionPin(int horid, string validaPin)
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

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_ValDerechos = '" + validaPin + "' " +
                                      "WHERE Hor_Id = '" + horid + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        bool IAgendaC.addRegAtn(int Admision, string Reg)
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

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_RegAtn = '" + Reg + "' " +
                                      "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        int IAgendaC.getIdPacByAdmition(int Admition, string Estado)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Cargar_Hora = "SELECT Hor_Pac_Id " +
                                     "FROM CXN_HORARIO " +
                                     "WHERE Hor_Id = '" + Admition + "' " +
                                     "AND Hor_Estado = '" + Estado + "'";
                SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                if (Lectura_Hora.Read() == true)
                {
                    return Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]);
                }
                else
                {
                    return 0;
                }
            }
        }
        string IAgendaC.getNameServicioFHIR(int CodeServ)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Cargar_Hora = "SELECT * " +
                                     "FROM CXN_CONVENIOS_2 " +
                                     "WHERE CodigoServicio = @param1";

                using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                {
                    Carga_Command.Parameters.AddWithValue("@param1", CodeServ);

                    using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                    {
                        if (Lectura_Hora.Read() == true)
                        {
                            return Lectura_Hora["NameServicio"].ToString();
                        }
                        else
                        {
                            return "MEDICINA GENERAL";
                        }
                    }
                }                                 
            }
        }
        CXN_HORARIO IAgendaC.getIdPacByAdmitionReportCitas(int Admition)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Cargar_Hora = "SELECT Hor_Pac_Id, Hor_Pac_Cia " +
                                     "FROM CXN_HORARIO " +
                                     "WHERE Hor_Id = '" + Admition + "'";
                SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                if (Lectura_Hora.Read() == true)
                {
                    CXN_HORARIO H = new CXN_HORARIO
                    {
                        Hor_Pac_Id = Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]),
                        Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"])
                    };

                    return H;
                }
                else
                {
                    return null;
                }
            }
        }
        List<CXN_DIAS_WEB> IAgendaC.CargarList(int Prof)
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
                                   "FROM CXN_DIAS_WEB " +
                                   "WHERE F_Prof = '" + Prof + "' " +
                                   "ORDER BY F_Fecha DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_DIAS_WEB> L = new List<CXN_DIAS_WEB>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_DIAS_WEB
                            {
                                F_Id = Convert.ToInt32(Reader["F_Id"]),
                                F_Fecha = Convert.ToDateTime(Reader["F_Fecha"]),
                                F_Estado = Reader["F_Estado"].ToString(),
                                F_Bloquea = Reader["F_Bloquea"].ToString(),
                                F_Desbloquea = Reader["F_Desbloquea"].ToString()
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
            catch
            {
                return null;
            }
        }
        bool IAgendaC.Grabar(CXN_DIAS_WEB dias)
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

                    DateTime Hoy = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_DIAS_WEB (F_Fecha, " +
                                                          "R_Razon, " +
                                                          "F_Prof, " +
                                                          "F_Bloquea, " +
                                                          "F_FBloquea, " +
                                                          "F_Estado) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6)", con);

                    cmd.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = dias.F_Fecha;
                    cmd.Parameters.AddWithValue("@param2", dias.R_Razon);
                    cmd.Parameters.AddWithValue("@param3", dias.F_Prof);
                    cmd.Parameters.AddWithValue("@param4", dias.F_Bloquea);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Hoy;
                    cmd.Parameters.AddWithValue("@param6", dias.F_Estado); //B
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        bool IAgendaC.Desbloquear(int Position, string User)
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

                    DateTime Hoy = DateTime.Now.Date;

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_DIAS_WEB " +
                                      "SET " +
                                      "F_Estado = @param1, " +
                                      "F_FDesbloquea = @param2, " +
                                      "F_Desbloquea = @param3 " +
                                      "WHERE F_Id = '" + Position + "'", con);

                    Busqueda.Parameters.AddWithValue("@param1", "D");
                    Busqueda.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Hoy;
                    Busqueda.Parameters.AddWithValue("@param3", User);
                    Busqueda.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        Dictionary<string, string> IAgendaC.ConsultaDatosAutorizacionMedGen(int Paciente, DateTime FechaCita)
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


                    String Query = "SELECT Hor_Autoriza, Hor_RegAtn, Hor_CantSesion, Hor_Pac_Sal " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_Pac_Id = '" + Paciente + "' " +
                                   "AND Hor_Pac_Tipo_Serv = 'MG' " +
                                   "AND Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(FechaCita).ToString(getCon["Format_Fecha"]) + "' " +
                                   "AND Hor_Estado <> 'C'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        if (Reader["Hor_CantSesion"].ToString() == "")
                        {
                            return null;
                        }
                        else
                        {
                            Dictionary<string, string> DIC = new Dictionary<string, string>();
                            DIC.Add("Autorizacion", Reader["Hor_Autoriza"].ToString());
                            DIC.Add("Registro", Reader["Hor_RegAtn"].ToString());
                            DIC.Add("Cantidad", Reader["Hor_CantSesion"].ToString());
                            DIC.Add("Confirma", "1");
                            DIC.Add("Hor_Pac_Sal", Reader["Hor_Pac_Sal"].ToString());
                            return DIC;
                        }
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
        string IAgendaC.consultarCitasMismoDia(int Paciente, int Bodega, DateTime Fecha)
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

                    String Consulta_Previas = "SELECT Bod_Responsable " +
                                              "FROM CXN_HORARIO " +
                                              "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                              "WHERE Hor_Pac_Id = '" + Paciente + "' " +
                                              "AND Hor_Estado = 'A' " +
                                              "AND Hor_Pac_Bod <> '" + Bodega + "'" +
                                              "AND Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(Fecha).ToString(getCon["Format_Fecha"]) + "'";
                    SqlCommand Carga_Commandos = new SqlCommand(Consulta_Previas, con);
                    SqlDataReader Lectura_Horas_Previas = (Carga_Commandos.ExecuteReader());
                    if (Lectura_Horas_Previas.Read() == true)
                    {
                        return Lectura_Horas_Previas["Bod_Responsable"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch
            {
                return "";
            }
        }
        bool IAgendaC.ConsultarNavyEnfermeria(int Paciente, int Bodega, DateTime Fecha)
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

                    String Consulta_Previas = "SELECT * " +
                                              "FROM CXN_HORARIO " +
                                              "WHERE Hor_Pac_Id = @param1 " +
                                              "AND Hor_Estado <> @param2 " +
                                              "AND Hor_Pac_Tipo_Serv = @param3 " +
                                              "AND Hor_Pac_Fecha_Cita = @param4";

                    using (SqlCommand Carga_Commandos = new SqlCommand(Consulta_Previas, con))
                    {
                        Carga_Commandos.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Commandos.Parameters.AddWithValue("@param2", "C");
                        Carga_Commandos.Parameters.AddWithValue("@param3", "MG");
                        Carga_Commandos.Parameters.AddWithValue("@param4", Convert.ToDateTime(Fecha).ToString(getCon["Format_Fecha"]));

                        using (SqlDataReader Lectura_Horas_Previas = (Carga_Commandos.ExecuteReader()))
                        {
                            if (Lectura_Horas_Previas.HasRows)
                            {
                                while (Lectura_Horas_Previas.Read() == true)
                                {
                                    Console.WriteLine(Lectura_Horas_Previas["Hor_Id"].ToString());
                                    if (Lectura_Horas_Previas["Hor_Pac_Sal"] != DBNull.Value)
                                    {
                                        if (Lectura_Horas_Previas["Hor_Pac_Sal"].ToString() == "Z")
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                return false;
                            }                           
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return false;
        }
        List<string> IAgendaC.CargarRazones()
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

                    String Query = "SELECT R_Cancela  " +
                                   "FROM CXN_MENUHGMG " +
                                   "ORDER BY R_Cancela ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();

                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["R_Cancela"].ToString());
                        }

                        return L;
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
        (DateTime limite, string observa, string tiposerv, string text) IAgendaC.searchAdmition(int Admision)
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

                    String Cargar_Hora = "SELECT Hor_Imp_Age, Hor_Pac_Fecha_Cita, Hor_Estado, Hor_Pac_Tipo_Serv, Hor_Observacion " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        string Estados;
                        string Tserv;

                        switch (Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString())
                        {
                            case "CU":
                                Tserv = "Curaciones";
                                break;

                            case "MG":
                                Tserv = "Medicina General";
                                break;

                            case "TF":
                                Tserv = "Terapia Fisica";
                                break;

                            case "TO":
                                Tserv = "Terapia Ocupacional";
                                break;

                            case "PS":
                                Tserv = "Psicologia";
                                break;

                            case "FI":
                                Tserv = "Fisiatria";
                                break;

                            default:
                                Tserv = "Servicio Desconocido";
                                break;
                        }


                        switch (Lectura_Hora["Hor_Estado"].ToString())
                        {
                            case "A":
                                Estados = "Cita No Asistida";
                                break;

                            case "C":
                                Estados = "Cita Cancelada";
                                break;

                            case "H":
                                Estados = "Cita Asistida";
                                break;

                            case "P":
                                Estados = "Cita asistida sin historia";
                                break;

                            case "B":
                                Estados = "Bloqueo de Agenda";
                                break;

                            default:
                                Estados = "Admision con inconvenientes";
                                break;
                        }

                        DateTime Limite = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]);

                        string Res = "Paciente: " + Lectura_Hora["Hor_Imp_Age"].ToString() + "\n\r " +
                                     "Fecha de Atencion: " + Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]).ToString(getCon["Format_Fecha"]) + "\n\r" +
                                     "Estado: " + Estados + "\n\r" +
                                     "Servicio: " + Tserv + "\n\r" +
                                     "Observaciones: " + Lectura_Hora["Hor_Observacion"].ToString();

                        string Observa = Lectura_Hora["Hor_Observacion"].ToString();
                        string TipServ = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString();

                        return (Limite, Observa, TipServ, Res);
                    }
                    else
                    {
                        DateTime J = new DateTime(2023, 06, 28);
                        return (J, "", "", "");
                    }
                }
            }
            catch
            {
                DateTime J = new DateTime(2023, 06, 28);
                return (J, "", "", "");
            }
        }
        string IAgendaC.Calcular3(int Paciente, string TipoServicio)
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
                    String Query = "SELECT TOP 1 Hor_Id, Hor_CantSesion " +
                                    "FROM CXN_HORARIO " +
                                    "WHERE Hor_Pac_Id = '" + Paciente + "' " +
                                    "AND Hor_Pac_Tipo_Serv = '" + TipoServicio + "' " +
                                    "AND Hor_IniciaSesion = 'S' " +
                                    "AND Hor_Estado IN ('P', 'H') " +
                                    "ORDER BY Hor_Id DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        var Can = Cantidad(Convert.ToInt32(Reader["Hor_Id"]), Paciente);
                        if (this.Recepcion != "YES")
                        {
                            return "Sesion " + Can.ToString() + " de " + Reader["Hor_CantSesion"].ToString();
                        }
                        return "Sesion " + Can.ToString() + " de " + Reader["Hor_CantSesion"].ToString();
                    }
                    else
                    {
                        return "No es posible calcular sesion debido a que no hay registros de autorizaciones vigentes";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        string IAgendaC.CrearHistorias(int Adm, string user)
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
                    String Query = "SELECT H.Hor_Pac_Cup, H.Hor_Pac_Tipo_Serv, C.Con_Clase " +
                                   "FROM CXN_HORARIO H " +
                                   "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                   "WHERE H.Hor_Id = '" + Adm + "' " +
                                   "AND H.Hor_Pac_Tipo_Serv = C.Con_Tipo_Serv " +
                                   "AND H.Hor_Pac_Ase = C.Con_Aseguradora";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "CU" && Reader["Con_Clase"].ToString() == "QX")
                        {
                            return "CURACION";
                        }

                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "CU" && Reader["Con_Clase"].ToString() == "DX")
                        {
                            var Jefe = rpoBod.getUser(user);
                            if (Jefe == null || Jefe.Log_Varios != "A")
                            {
                                return "NOBOSS";
                            }
                            else
                            {
                                return "BOSS";
                            }
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "MG" && Reader["Con_Clase"].ToString() == "DX")
                        {
                            return "MEDGEN";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "FI" && Reader["Con_Clase"].ToString() == "DX")
                        {
                            return "FISIATRIA";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "TO" && Reader["Con_Clase"].ToString() == "QX")
                        {
                            return "EVOLUCION";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "TF" && Reader["Con_Clase"].ToString() == "QX")
                        {
                            return "EVOLUCION";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "PS" && Reader["Con_Clase"].ToString() == "QX")
                        {
                            return "EVOLUCION";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "TO" && Reader["Con_Clase"].ToString() == "DX")
                        {
                            return "HTO";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "TF" && Reader["Con_Clase"].ToString() == "DX")
                        {
                            return "HTF";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "PS" && Reader["Con_Clase"].ToString() == "DX")
                        {
                            return "HPSI";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "TF" && Reader["Con_Clase"].ToString() == "JM")
                        {
                            return "JM";
                        }
                        if (Reader["Hor_Pac_Tipo_Serv"].ToString() == "RA" && Reader["Con_Clase"].ToString() == "DX")
                        {
                            return "RADIOLOGIA";
                        }

                        return "";
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
        List<CXN_HORARIO> IAgendaC.CargarGrilla(string Tipo)
        {
            try
            {
                var gtData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(gtData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Query = "";

                    if (Tipo == "MG")
                    {
                        Query = "SELECT HC_Adm, HC_Fecha, HC_Pac " +
                                "FROM CXN_HCMG " +
                                "WHERE HC_Cant = '0'";
                    }

                    if (Tipo == "FI")
                    {
                        Query = "SELECT HC_Adm, HC_Fecha, HC_Pac  " +
                                "FROM CXN_HCFI " +
                                "WHERE HC_Cant = '0'";
                    }

                    if (Tipo == "RA")
                    {
                        Query = "SELECT HCAdm AS HC_Adm, Fecha AS HC_Fecha, PacienteNombre AS HC_Pac " +
                                "FROM CXN_HCRADIOLOGIA " +
                                "WHERE HCCant = '0'";
                    }

                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_HORARIO> L = new List<CXN_HORARIO>();

                        while (Reader.Read() == true)
                        {
                            DateTime F;

                            if (Reader["HC_Fecha"] == DBNull.Value)
                            {
                                F = new DateTime(99, 01, 01);
                            }
                            else
                            {
                                F = Convert.ToDateTime(Reader["HC_Fecha"]);
                            }

                            L.Add(new CXN_HORARIO
                            {
                                Hor_Id = Convert.ToInt32(Reader["HC_Adm"]),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(F),
                                Hor_Imp_Age = Reader["HC_Pac"].ToString()
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
        List<CXN_HORARIO> IAgendaC.consultaCancelaWEB(string Documento)
        {
            try
            {
                var gtData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(gtData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "";

                    if (Documento == "XXX")
                    {
                        Query = "SELECT TOP 100 Hor_Imp_Age, Hor_Pac_Fecha_Cita, Hor_Id, Hor_Pac_RCancela " +
                                "FROM CXN_HORARIO " +
                                "WHERE Hor_Pac_MCancela = 'CANCELADO PACIENTE WEB' " +
                                "ORDER BY Hor_Pac_Fecha_Cita DESC";
                    }
                    else
                    {
                        Query = "SELECT TOP 100 Hor_Imp_Age, Hor_Pac_Fecha_Cita, Hor_Id, Hor_Pac_RCancela " +
                                "FROM CXN_HORARIO " +
                                "WHERE Hor_Pac_Id = '" + Documento + "' " +
                                "AND Hor_Pac_MCancela = 'CANCELADO PACIENTE WEB' " +
                                "ORDER BY Hor_Pac_Fecha_Cita DESC";
                    }
                    
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_HORARIO> L = new List<CXN_HORARIO>();

                        while (Reader.Read() == true)
                        {
                            L.Add(new CXN_HORARIO { 
                                Hor_Imp_Age = Reader["Hor_Imp_Age"].ToString(),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]),
                                Hor_Id = Convert.ToInt32(Reader["Hor_Id"]),
                                Hor_Pac_RCancela = Reader["Hor_Pac_RCancela"].ToString()
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
        List<CXN_HORARIO> IAgendaC.consultaCancelaWEB(DateTime Fecha)
        {
            try
            {
                var gtData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(gtData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Hor_Imp_Age, Hor_Pac_Fecha_Cita, Hor_Id, Hor_Pac_RCancela " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_Pac_Fecha_Cita = @param1 " +
                                   "AND Hor_Pac_MCancela = @param2 " +
                                   "AND Hor_Estado = @param3 " +
                                   "ORDER BY Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Convert.ToDateTime(Fecha));
                        Commando.Parameters.AddWithValue("@param2", "CANCELADO PACIENTE WEB");
                        Commando.Parameters.AddWithValue("@param3", "C");

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_HORARIO> L = new List<CXN_HORARIO>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(new CXN_HORARIO
                                    {
                                        Hor_Imp_Age = Reader["Hor_Imp_Age"].ToString(),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]),
                                        Hor_Id = Convert.ToInt32(Reader["Hor_Id"]),
                                        Hor_Pac_RCancela = Reader["Hor_Pac_RCancela"].ToString()
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        bool IAgendaC.EspacioRobado(int Cia, int Bod, string IdHora, DateTime Fecha)
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
                    String Query = "SELECT Hor_Id " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_Pac_Cia = '" + Cia + "' " +
                                   "AND Hor_Pac_Bod = '" + Bod + "' " +
                                   "AND Hor_Pac_Id_Hora = '" + IdHora + "' " +
                                   "AND Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                   "AND Hor_Estado <> 'C'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;   
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        void IAgendaC.updateServicoFromFactura(int Admision, string CUP)
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

                    DateTime Hoy = DateTime.Now.Date;

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_HORARIO " +
                                                      "SET " +
                                                      "Hor_Pac_Cup = @param1 " +
                                                      "WHERE Hor_Id = @param2", con);

                    Busqueda.Parameters.AddWithValue("@param1", CUP);
                    Busqueda.Parameters.AddWithValue("@param2", Admision);
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
