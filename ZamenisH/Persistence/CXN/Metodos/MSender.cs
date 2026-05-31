using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MSender : ISender
    {
        private static readonly IGenerales repoGenerales = new MGenerales();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly ICompañia repoCia = new MCompañia();

        bool ISender.Actualizar(CXN_EMAIL E)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_EMAIL " +
                                      "SET  " +
                                      "Ema_Email = @param1, " +
                                      "Ema_Pass = @param2, " +
                                      "Ema_Server = @param3, " +
                                      "Ema_Muestra = @param4, " +
                                      "Ema_URL_CitasM = @param5, " +
                                      "Ema_URL_CitasS = @param6, " +
                                      "Ema_Baja_Email = @param7, " +
                                      "Ema_Puerto = @param8 " +
                                      "WHERE Ema_Email = '" + E.Ema_Email + "'", con);

                    Busqueda.Parameters.AddWithValue("@param1", E.Ema_Email);
                    Busqueda.Parameters.AddWithValue("@param2", E.Ema_Pass);
                    Busqueda.Parameters.AddWithValue("@param3", E.Ema_Server);
                    Busqueda.Parameters.AddWithValue("@param4", E.Ema_Muestra);
                    Busqueda.Parameters.AddWithValue("@param5", E.Ema_URL_CitasM);
                    Busqueda.Parameters.AddWithValue("@param6", E.Ema_URL_CitasS);
                    Busqueda.Parameters.AddWithValue("@param7", E.Ema_Baja_Email);
                    Busqueda.Parameters.AddWithValue("@param8", E.Ema_Puerto);
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
        List<CXN_HORARIO> ISender.EnviarSMS(string TEnvio, DateTime Fecha)
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

                    String Cargar_Hora = "SELECT P.Pac_Telefono, H.Hor_Id, H.Hor_Pac_Tipo_Serv, H.Hor_Pac_Cia, H.Hor_Pac_Id, H.Hor_Pac_Ase, C.Com_Identificacion  " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "WHERE H.Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(Fecha).ToString(getData["Format_Fecha"]) + "' " +
                                         "AND H.Hor_Estado = 'A' " +
                                         "AND H.Hor_Pac_Tipo_Serv = '" + TEnvio + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            if (TEnvio == "CU")
                            {
                                bool consmedGen = ConsMedGen(Fecha, Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]));
                                if (consmedGen == false)
                                {
                                    if (Lectura_Hora["Pac_Telefono"].ToString().Length == 10)
                                    {
                                        var cons = ConsultarExcluidos(Lectura_Hora["Pac_Telefono"].ToString());

                                        if (cons != true)
                                        {
                                            H.Add(new CXN_HORARIO
                                            {
                                                PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                                Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                                Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                                Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                                Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                                Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString()
                                            });
                                        }
                                    }
                                }
                            }
                            else if (TEnvio == "MG")
                            {
                                //metodo para medicina genral
                                return getListaMG(Fecha);
                            }
                            else
                            {
                                if (Lectura_Hora["Pac_Telefono"].ToString().Length == 10)
                                {
                                    var cons = ConsultarExcluidos(Lectura_Hora["Pac_Telefono"].ToString());

                                    if (cons != true)
                                    {
                                        H.Add(new CXN_HORARIO
                                        {
                                            PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                            Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                            Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                            Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                            Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                            Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString()
                                        });
                                    }
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
            catch
            {
                return null;
            }
        }
        List<CXN_HORARIO> ISender.EnviarEMAIL(string TEnvio, DateTime Fecha)
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

                    String Cargar_Hora = "SELECT P.Pac_Email, H.Hor_Id, H.Hor_Pac_Tipo_Serv, H.Hor_Pac_Cia, H.Hor_Pac_Id, H.Hor_Imp_Age, " +
                                         "H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, C.Com_Direccion, C.Com_Telefono, H.Hor_Pac_Ase, C.Com_Identificacion  " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "WHERE H.Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(Fecha).ToString(getData["Format_Fecha"]) + "' " +
                                         "AND H.Hor_Estado = 'A' " +
                                         "AND H.Hor_Pac_Tipo_Serv = '" + TEnvio + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            if (TEnvio == "CU")
                            {
                                bool consmedGen = ConsMedGen(Fecha, Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]));
                                if (consmedGen == false)
                                {
                                    if (repoPacientes.ValidaEmail(Lectura_Hora["Pac_Email"].ToString()) == true)
                                    {
                                        var cons = ConsultarExcluidos(Lectura_Hora["Pac_Email"].ToString());

                                        if (cons != true)
                                        {
                                            H.Add(new CXN_HORARIO
                                            {
                                                Com_Email = Lectura_Hora["Pac_Email"].ToString(),
                                                Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                                Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                                Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                                Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                                Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                                Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                                Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                                Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                                Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString()
                                            });
                                        }
                                    }
                                }
                            }
                            else if (TEnvio == "MG")
                            {
                                //metodo para medicina genral
                                return getListaMG(Fecha);
                            }
                            else
                            {
                                if (repoPacientes.ValidaEmail(Lectura_Hora["Pac_Email"].ToString()) == true)
                                {
                                    var cons = ConsultarExcluidos(Lectura_Hora["Pac_Email"].ToString());

                                    if (cons != true)
                                    {
                                        H.Add(new CXN_HORARIO
                                        {
                                            Com_Email = Lectura_Hora["Pac_Email"].ToString(),
                                            Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                            Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                            Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                            Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                            Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                            Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                            Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                            Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                            Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                            Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString()
                                        });
                                    }
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        //HECHO
        (string TextoCuerpo, byte[] GoogleCalendar) ISender.Envia_Mail(string TipoCita,
                                                                       int Admision,
                                                                       string EmailPaciente,
                                                                       int Compañia,
                                                                       string Baja,
                                                                       string Email,
                                                                       DateTime FechaCita,
                                                                       DateTime HoraCita,
                                                                       string Paciente,
                                                                       string DireccionCia,
                                                                       string TelefonoCia,
                                                                       string Link)
        {
            try
            {
                var Comprobar_Clave = repoGenerales.Base64Encode(Admision.ToString());

                string Text_TCITA;
                switch (TipoCita)
                {
                    case "MG":
                        Text_TCITA = "Medicina General de Curaciones";
                        break;

                    case "CU":
                        Text_TCITA = "Curacion de Heridas";
                        break;

                    case "FI":
                        Text_TCITA = "Fisiatria";
                        break;

                    case "PS":
                        Text_TCITA = "Psicologia";
                        break;

                    case "TO":
                        Text_TCITA = "Terapia Ocupacional";
                        break;

                    case "TF":
                        Text_TCITA = "Terapia Fisica";
                        break;

                    case "RA":
                        Text_TCITA = "Radiologia";
                        break;

                    default:
                        Text_TCITA = "Clinica de Heridas o Fibromialgia";
                        break;
                }

                string prestador = repoCia.getPrestadorbyCode(Compañia).Com_Nombre;

                byte[] rutaIcs = EnviarCorreoConInvitacion("Recordatorio de Citas",
                                                                    "Cita Medica Proxima",
                                                                    Convert.ToDateTime(HoraCita),
                                                                    Convert.ToDateTime(HoraCita.AddMinutes(20)),
                                                                    Convert.ToDateTime(FechaCita),
                                                                    "Centro Medico",
                                                                    Email,
                                                                    prestador.ToUpper());

                string html = "<center>" +
                                   "<img src='cid:imagen' />" +
                                   "<h2 style='background:#3D38B2; color:white;'>Hola, " + Paciente + "</h2>" +
                                   "<p>Queremos recordarte que tienes una cita pendiente en " + prestador.ToUpper() + " al servicio de: " + Text_TCITA + ", asignada de la siguiente manera: <br><br>" +
                                   "<b style='color:blue'>Fecha: </b><b style='color:black'>" + Convert.ToDateTime(FechaCita).ToString("yyyy-MM-dd") + "</b><br><br>" +
                                   "<b style='color:blue'>Hora: </b><b style='color:black'>" + Convert.ToDateTime(HoraCita).ToString("HH:mm tt") + "</b><br><br>" +
                                   "<b style='color:blue'>Direccion: </b><b style='color:black'>" + DireccionCia + "</b><br><br>" +
                                   "<b style='color:blue'>Telefono: </b><b style='color:black'>" + TelefonoCia + "</b><br><br>" +
                                   "<b style='color:black'>Si deseas dejar tus comentarios o sugerencias puedes hacer clic </b><b style='color:blue'>" + "<a href='https://slsoft.net:5010/PQRSF' style='color:red; font-size:12px;'>AQUI</a>" + "</b><br><br>" +
                                   "</p>" +
                                   "<p>Si no puedes asistir, por favor cancela tu cita medica antes de la fecha mensionada en el siguiente link <br><br>" +
                                   //"<a style='background:blue; color:white; padding:3px; border: 3px solid #000; border-color:black;' href='" + URL + "?a=" + Comprobar_Clave + "'> Cancelar Cita</a>" +
                                   "<a style='background:blue; color:white; padding:3px; border: 3px solid #000; border-color:black;' href='" + Link.ToString().Trim() + "'> Cancelar Cita</a>" +
                                   "</p><br><hr>Identificador de Cita: <b>" + Admision.ToString() + "</b><hr><br><br> <p><b>Conoce nuestros Productos: <a href='https://slsoft.net'>https://slsoft.net</a></b></p>" +
                                   "<p style='color:gray'>" +
                                   "AVISO LEGAL: La información transmitida a través de este correo electrónico es confidencial y dirigida única y exclusivamente para uso de su(s) destinatario(s). " +
                                   "Su reproducción, lectura o uso está prohibido a cualquier persona o entidad diferente, sin autorización previa por escrito. Si usted lo ha recibido por error, " +
                                   "por favor notifíquelo inmediatamente al remitente y elimínelo de su sistema. Cualquier uso, divulgación, copia, distribución, impresión o acto derivado del " +
                                   "conocimiento total o parcial de este mensaje sin autorización del remitente será sancionado de acuerdo con las normas legales vigentes. Las opiniones, " +
                                   "conclusiones y otra información contenida en este correo, no relacionadas con las actividades de la institucion medica, deben entenderse como personales " +
                                   "y de ninguna manera son avaladas por dicha Institución. Aunque la institucion medica ha realizado su mejor esfuerzo para asegurar que el presente mensaje y " +
                                   "sus archivos anexos se encuentran libre de virus y defectos que puedan llegar a afectar los computadores o sistemas que lo reciban, no se hace responsable " +
                                   "por la eventual transmisión de virus o programas dañinos por este conducto, y por lo tanto es responsabilidad del destinatario confirmar la existencia " +
                                   "de este tipo de elementos al momento de recibirlo y abrirlo. No se acepta responsabilidad alguna por eventuales daños o alteraciones derivados de la recepción " +
                                   "o uso del presente mensaje." +
                                   "</p><br>" +
                                   "<b>Para darse de baja de estos mensajes y no recibir mas estas notificaciones haga clic <a href='" + Baja.ToString() + "?a=" + Comprobar_Clave.ToString() + "'>AQUI</a>:</b>" +
                               "</center>";

                return (html, rutaIcs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //GRABA CONSTANCIA NO POR ERROR
                GrabaSQL_Evidencia(ex.Message,
                                        Convert.ToInt32(Admision),
                                        EmailPaciente,
                                        "Recordatorio de Cita WEB",
                                        "ASP EMAIL");

                return (null, null);
            }
        }

        //Archivo .ics
        public byte[] EnviarCorreoConInvitacion(string asunto,
                                         string mensaje,
                                         DateTime inicioEvento,
                                         DateTime finEvento,
                                         DateTime fechaEvento,
                                         string ubicacion,
                                         string Organizador, 
                                         string Empresa)
        {
            try
            {
                DateTime fecha = new DateTime(fechaEvento.Year, fechaEvento.Month, fechaEvento.Day);
                DateTime hora = new DateTime(1, 1, 1, inicioEvento.Hour, inicioEvento.Minute, 0);
                inicioEvento = inicioEvento.AddMinutes(20);
                DateTime horaFin = new DateTime(1, 1, 1, inicioEvento.Hour, inicioEvento.Minute, 0);

                // Combinar fecha y hora
                DateTime inicioEventos = fecha.Date + hora.TimeOfDay;
                DateTime finEventos = fecha.Date + horaFin.TimeOfDay;

                // Crear el contenido del archivo .ics
                var ics = new StringBuilder();
                ics.AppendLine("BEGIN:VCALENDAR");
                ics.AppendLine("PRODID:-//" + Empresa + "//Zamenis Health//ES");
                ics.AppendLine("VERSION:2.0");
                ics.AppendLine("METHOD:REQUEST");

                ics.AppendLine("BEGIN:VEVENT");
                ics.AppendLine("UID:" + Guid.NewGuid().ToString());

                ics.AppendLine("ORGANIZER;CN=\"" + Empresa + "\":mailto:" + Organizador);
                ics.AppendLine("CALSCALE:GREGORIAN");
                ics.AppendLine("SEQUENCE:0");
                ics.AppendLine("STATUS:CONFIRMED");
                ics.AppendLine($"DTSTART:{inicioEventos:yyyyMMddTHHmmss}"); // Hora de inicio en formato UTC
                ics.AppendLine($"DTEND:{finEventos:yyyyMMddTHHmmss}");      // Hora de fin en formato UTC
                ics.AppendLine("SUMMARY:" + asunto);                       // Asunto del evento
                ics.AppendLine("DESCRIPTION:" + mensaje);                  // Descripción del evento

                ics.AppendLine("ORGANIZER;CN=\"" + Empresa + "\":mailto:" + Organizador);
                ics.AppendLine("LOCATION:" + ubicacion);                   // Ubicación del evento
                ics.AppendLine("TRIGGER:-PT60M");
                ics.AppendLine("DESCRIPTION:Recordatorio de Cita Medica");
                ics.AppendLine("END:VEVENT");
                ics.AppendLine("END:VCALENDAR");

                /*ics.AppendLine("CLASS:PUBLIC");                
                ics.AppendLine("TRANSP:OPAQUE");
                ics.AppendLine("BEGIN:VALARM");
                ics.AppendLine("ACTION:DISPLAY");                
                ics.AppendLine("END:VALARM");*/                

                byte[] byteArray = Encoding.UTF8.GetBytes(ics.ToString());

                return byteArray;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        List<CXN_HORARIO> getListaMG(DateTime Fecha)
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

                    String Cargar_Hora = "SELECT P.Pac_Telefono, P.Pac_Email, H.Hor_Id, H.Hor_Pac_Tipo_Serv, H.Hor_Pac_Ase, H.Hor_Pac_Cia, " +
                                         "H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, H.Hor_Imp_Age, C.Com_Direccion, C.Com_Telefono, C.Com_Identificacion " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "WHERE H.Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(Fecha).ToString(getData["Format_Fecha"]) + "' " +
                                         "AND H.Hor_Estado = 'A' " +
                                         "AND H.Hor_Pac_Tipo_Serv = 'MG'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            H.Add(new CXN_HORARIO
                            {
                                PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                Com_Email = Lectura_Hora["Pac_Email"].ToString(),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString()
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
        bool ConsMedGen(DateTime Fecha, int Paciente)
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

                    String Cargar_Hora = "SELECT Hor_Id " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(Fecha).ToString(getData["Format_Fecha"]) + "' " +
                                         "AND Hor_Estado = 'A' " +
                                         "AND Hor_Pac_Tipo_Serv = 'MG' " +
                                         "AND Hor_Pac_Id = '" + Paciente + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return true;
            }
        }
        bool ConsultarExcluidos(string Dato)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_EXCLUIDOS " +
                                         "WHERE Exc_Mail = '" + Dato + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return true;
            }
        }
        string ISender.Envia_SMS(string Celular_Class,
                               string Hor_Id_Class,
                               string T_Cita,
                               int Compañia,
                               string URL,
                               string UsuarioLogueado, 
                               string URLFinal)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                CXN_CIA getSMS = repoCia.getPrestadorbyCode(Compañia);
                if (getSMS.Com_SMS <= 0)
                {
                    GrabaSQL_Evidencia("No hay creditos de mensajes suficientes",
                                Convert.ToInt32(Hor_Id_Class),
                                Celular_Class,
                                "No se logro enviar el mensaje por falta de creditos",
                                "ASPSMS");
                    return null;
                }
                else
                {
                    string Text_TCITA;
                    switch (T_Cita)
                    {
                        case "MG":
                            Text_TCITA = "Medicina General";
                            break;

                        case "CU":
                            Text_TCITA = "Curacion de Heridas";
                            break;

                        case "FI":
                            Text_TCITA = "Fisiatria";
                            break;

                        case "PS":
                            Text_TCITA = "Psicologia";
                            break;

                        case "TO":
                            Text_TCITA = "Terapia Ocupacional";
                            break;

                        case "TF":
                            Text_TCITA = "Terapia Fisica";
                            break;

                        case "RA":
                            Text_TCITA = "Radiologia";
                            break;

                        default:
                            Text_TCITA = "Clinica de Heridas o Fibromialgia";
                            break;
                    }

                    string Mensaje_SMS_Class = "Tienes una cita medica por " + Text_TCITA + ", mas informacion aqui: " + URLFinal.ToString();
                    int nueCons = getSMS.Com_SMS - 1;
                    repoCia.ConsecutivoActualiza(Compañia, "SMS", nueCons);
                    return Mensaje_SMS_Class;
                }
            }
            catch (Exception ex)
            {
                GrabaSQL_Evidencia("Error General",
                                Convert.ToInt32(Hor_Id_Class),
                                Celular_Class,
                                ex.Message + ": No se logro enviar el mensaje por un error interno en la plataforma",
                                "ASPSMS");

                return null;
            }
        }
        public void GrabaSQL_Evidencia(string Est,
                                       int Adm,
                                       string Destino,
                                       string MessageSend,
                                       string TipoEnvio)
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

                    cmd.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = Hoyy;
                    cmd.Parameters.AddWithValue("@param2", Est);
                    cmd.Parameters.AddWithValue("@param3", "Mensaje Automatico");
                    cmd.Parameters.AddWithValue("@param4", MessageSend);
                    cmd.Parameters.AddWithValue("@param5", Destino);
                    cmd.Parameters.AddWithValue("@param6", Adm);
                    cmd.Parameters.AddWithValue("@param7", TipoEnvio);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
        }
    }
}