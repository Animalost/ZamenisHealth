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
    public class MReportes : IReportes
    {
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IGenerales repoGen = new MGenerales();
        private static readonly ICIE10 repoCIE10 = new MCIE10();
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly ICompañia repoCIA = new MCompañia();
        private static readonly ICondiciones repoCondiciones = new MCondiciones();
        private static readonly IPacientes repoPacinetes = new MPacientes();

        List<FirmasR> IReportes.Firmas_Print(int Adm_Selected, FirmasR F, bool Autocompletar)
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

                    String Query = "SELECT TOP 1 P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, A.Ase_Descripcion, P.Pac_TipoId, " +
                                   "P.Pac_IdNum, H.Hor_Autoriza, B.Bod_Responsable, H.Hor_CantSesion " +
                                   "FROM CXN_HORARIO H " +
                                   "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                   "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                   "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                   "WHERE H.Hor_Id = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Adm_Selected);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                List<FirmasR> firmasR = new List<FirmasR>();

                                Byte[] bytes = Convert.FromBase64String(F.Com_Logo); //convierte a bytes

                                string Cur = ""; string Cons = "";

                                if (Autocompletar == true)
                                {
                                    Cur = "CURACION";
                                    Cons = "CONSULTA";
                                }

                                firmasR.Add(new FirmasR
                                {
                                    PacienteNombre = Reader["Pac_PrimerA"].ToString() + " " + Reader["Pac_SegundoA"].ToString() + " " + Reader["Pac_PrimerN"].ToString() + " " + Reader["Pac_SegundoN"].ToString(),
                                    PacienteAseguradora = Reader["Ase_Descripcion"].ToString(),
                                    PacienteIdentificacion = Reader["Pac_TipoId"].ToString() + " " + Reader["Pac_IdNum"].ToString(),
                                    PacienteTelefono = Reader["Hor_Autoriza"].ToString(),
                                    EmpresaNombre = F.Com_Nombre,
                                    EmpresaDireccion = F.Com_Direccion,
                                    EmpresaTelefono = F.Com_Telefono,
                                    Logo = bytes,
                                    PacienteDireccion = Reader["Bod_Responsable"].ToString(), //profesional
                                    Com_Email = F.Com_Email, //fase de fibromialgia
                                    Com_Nombre_SMS = F.Com_Nombre_SMS,
                                    Com_Direccion = Reader["Hor_CantSesion"].ToString(), //cantidad de sesiones
                                    Com_UsuarioGraba = Cur,
                                    Com_Resolucion = Cons
                                });

                                return firmasR;
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
        List<GerencialR> IReportes.Rpt_Atenciones(DateTime Desde, DateTime Hasta)
        {
            List<GerencialR> datos_asistencia = new List<GerencialR>();
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora = "SELECT Hor_Estado, Hor_Pac_Bod, Bod_Responsable, Con_Nombre, COUNT(Hor_Estado) AS Asistencia " +
                                         "FROM CXN_HORARIO " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                         "INNER JOIN CXN_CONVENIOS ON CXN_HORARIO.Hor_Pac_Cup = CXN_CONVENIOS.Con_Id_Serv " +
                                         "WHERE Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataConection["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getDataConection["Format_Fecha"]) + "' " +
                                         "AND Con_Aseguradora = Hor_Pac_Ase " +
                                         "GROUP BY Hor_Estado, Hor_Pac_Bod, Bod_Responsable, Con_Nombre " +
                                         "ORDER BY Bod_Responsable ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        DateTime Hoy = DateTime.Now;

                        while (Lectura_Hora.Read() == true)
                        {
                            string Est;
                            switch (Lectura_Hora["Hor_Estado"].ToString())
                            {
                                case "P":
                                    Est = Lectura_Hora["Hor_Estado"].ToString() + " - Pendiente Crear Historia";
                                    break;
                                case "A":
                                    Est = Lectura_Hora["Hor_Estado"].ToString() + " - No Asiste Paciente";
                                    break;
                                case "C":
                                    Est = Lectura_Hora["Hor_Estado"].ToString() + " - Citas Canceladas";
                                    break;
                                case "B":
                                    Est = Lectura_Hora["Hor_Estado"].ToString() + " - Espacio Bloqueado";
                                    break;
                                case "H":
                                    Est = Lectura_Hora["Hor_Estado"].ToString() + " - Cita Asistida Correctamente";
                                    break;

                                default:
                                    Est = "E - ERROR DE ESTADO";
                                    break;
                            }

                            datos_asistencia.Add(new GerencialR
                            {
                                Medico = Lectura_Hora["Bod_Responsable"].ToString(),
                                Asistidas = Lectura_Hora["Asistencia"].ToString(),
                                Estado = Est,
                                Fecha_Actual = Convert.ToDateTime(Hoy),
                                Desde1 = Convert.ToDateTime(Desde),
                                Hasta1 = Convert.ToDateTime(Hasta),
                                Service = Lectura_Hora["Con_Nombre"].ToString(),
                                Bodega = Lectura_Hora["Hor_Pac_Bod"].ToString()
                            });
                        }

                        return datos_asistencia;
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
        void IReportes.Rpt_Ing_Ger(DateTime Desde, DateTime Hasta)
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

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Ingresos_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);
                    SqlCommand comando = new SqlCommand("SELECT Pac_TipoId, Pac_IdNum, Pac_FechaNto, Pac_Sexo, Pac_PrimerA, Pac_SegundoA, " +
                                                        "Pac_PrimerN, Pac_SegundoN, Ase_Cod_Emp, Hor_Estado, Hor_Pac_Solicita, Hor_Pac_Fecha, " +
                                                        "Hor_Pac_Fecha_Cita, Ase_Descripcion, HC_Patologia " +
                                                        "FROM  CXN_HORARIO " +
                                                        "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                                        "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                                        "INNER JOIN CXN_HCMG ON CXN_HORARIO.Hor_Id = CXN_HCMG.HC_Adm " +
                                                        "WHERE Hor_Tipo_Paciente = 'N' " +
                                                        "AND Hor_Estado <> 'B' " +
                                                        "AND Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataConection["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getDataConection["Format_Fecha"]) + "'", con);

                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("TIPO DOCUMENTO PACIENTE" + "," + "DOCUMENTO PACIENTE" + "," + "FECHA NACIMIENTO" + "," + "SEXO" + "," + "PRIMER APELLIDO" + "," + "SEGUNDO APELLIDO" + "," + "PRIMER NOMBRE" + "," + "SEGUNDO NOMBRE" + "," + "CODIGO EPS" +
                                  "," + "ESTADO CITA" + "," + "FECHA SOLICITADA POR PACIENTE" + "," + "FECHA ASIGNACION DE CITA" + "," + "FECHA DE LA CITA" + "," + "ASEGURADORA" + "," + "PATOLOGIA");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    string Estado_Cita = "";

                    while (leer.Read())
                    {
                        if (leer["Hor_Estado"].ToString() == "H") { Estado_Cita = "ASISTIO"; }
                        if (leer["Hor_Estado"].ToString() == "C") { Estado_Cita = "CANCELADA"; }
                        if (leer["Hor_Estado"].ToString() == "P") { Estado_Cita = "SIN NOTA O HISTORIA"; }
                        if (leer["Hor_Estado"].ToString() == "A") { Estado_Cita = "NO ASISTIO"; }

                        Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                        Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Pac_FechaNto"].ToString()).ToString(getDataConection["Format_Fecha"]) + ",");
                        Escriba.Write(leer["Pac_Sexo"].ToString().ToString() + ",");
                        Escriba.Write(leer["Pac_PrimerA"].ToString() + ",");
                        Escriba.Write(leer["Pac_SegundoA"].ToString() + ",");
                        Escriba.Write(leer["Pac_PrimerN"].ToString() + ",");
                        Escriba.Write(leer["Pac_SegundoN"].ToString() + ",");
                        Escriba.Write(leer["Ase_Cod_Emp"].ToString() + ",");
                        Escriba.Write(Estado_Cita + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Solicita"].ToString()).ToString(getDataConection["Format_Fecha"]) + ","); //FEcha de solicitud
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha"].ToString()).ToString(getDataConection["Format_Fecha"]) + ","); //Fecha asignacion de cita
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"].ToString()).ToString(getDataConection["Format_Fecha"]) + ","); //Fecha solicitada por el usuario                        
                        Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                        Escriba.Write(leer["HC_Patologia"].ToString());
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IReportes.Rpt_Rece_Ger(DateTime Desde, DateTime Hasta)
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

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Cancelaciones_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);
                    SqlCommand comando = new SqlCommand("SELECT H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, H.Hor_Imp_Age, " +
                                                        "H.Hor_Pac_MCancela, H.Hor_Pac_RCancela, A.Ase_Descripcion, B.Bod_Responsable " +
                                                        "FROM  CXN_HORARIO H " +
                                                        "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                                        "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_ASe = A.Ase_Identificador " +
                                                        "WHERE Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataConection["Format_Fecha"]) + "' and '" + Convert.ToDateTime(Hasta).ToString(getDataConection["Format_Fecha"]) + "' " +
                                                        "AND Hor_Estado ='C'", con);
                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("FECHA" + "," + "PACIENTE" + "," + "PROFESIONAL" + "," + "HORA" + "," + "ASEGURADORA" + "," + "MOTIVO" + "," + "OBSERVACIONES");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read())
                    {
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"].ToString()).ToString(getDataConection["Format_Fecha"]) + ",");
                        Escriba.Write(leer["Hor_Imp_Age"].ToString() + ",");
                        Escriba.Write(leer["Bod_Responsable"].ToString() + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Hora_Cita"].ToString()).ToString("HH:mm tt") + ",");
                        Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                        Escriba.Write(leer["Hor_Pac_MCancela"].ToString() + ",");
                        Escriba.Write(leer["Hor_Pac_RCancela"].ToString());
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();
                    Rpt_Rece_Ger_2(Desde, Hasta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Rpt_Rece_Ger_2(DateTime Desde, DateTime Hasta)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now;

                FileStream Query = new FileStream("C:/Cxn/Reportes/Tardanzas_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);
                SqlCommand comando = new SqlCommand("SELECT H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, H.Hor_Imp_Age, " +
                                                    "H.Hor_Pac_Minutos, H.Hor_Pac_Razon, A.Ase_Descripcion, B.Bod_Responsable " +
                                                    "FROM  CXN_HORARIO H " +
                                                    "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                                    "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_ASe = A.Ase_Identificador " +
                                                    "WHERE Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataConection["Format_Fecha"]) + "' and '" + Convert.ToDateTime(Hasta).ToString(getDataConection["Format_Fecha"]) + "' " +
                                                    "AND Hor_Estado = 'H'", con);
                SqlDataReader leer;
                leer = comando.ExecuteReader();

                Escriba.Write("FECHA" + "," + "PACIENTE" + "," + "PROFESIONAL" + "," + "HORA" + "," + "ASEGURADORA" + "," + "MINUTOS" + "," + "OBSERVACIONES");
                Escriba.WriteLine();
                Escriba.Flush();

                while (leer.Read())
                {
                    Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"].ToString()).ToString(getDataConection["Format_Fecha"]) + ",");
                    Escriba.Write(leer["Hor_Imp_Age"].ToString() + ",");
                    Escriba.Write(leer["Bod_Responsable"].ToString() + ",");
                    Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Hora_Cita"].ToString()).ToString("HH:mm tt") + ",");
                    Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                    Escriba.Write(leer["Hor_Pac_Minutos"].ToString() + ",");
                    Escriba.Write(leer["Hor_Pac_Razon"].ToString());
                    Escriba.WriteLine();
                    Escriba.Flush();
                }
                Escriba.Close();
                Rpt_Rece_Ger_3(Desde, Hasta);
            }
        }
        void Rpt_Rece_Ger_3(DateTime Desde, DateTime Hasta)
        {
            var getDataConection = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now;

                FileStream Query = new FileStream("C:/Cxn/Reportes/Inasistencias_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);
                SqlCommand comando = new SqlCommand("SELECT H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, H.Hor_Imp_Age, " +
                                                    "H.Hor_Pac_Inasistencia, A.Ase_Descripcion, B.Bod_Responsable " +
                                                    "FROM  CXN_HORARIO H " +
                                                    "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                                    "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_ASe = A.Ase_Identificador " +
                                                    "WHERE Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDataConection["Format_Fecha"]) + "' and '" + Convert.ToDateTime(Hasta).ToString(getDataConection["Format_Fecha"]) + "' " +
                                                    "AND Hor_Estado = 'A'", con);
                SqlDataReader leer;
                leer = comando.ExecuteReader();

                Escriba.Write("FECHA" + "," + "PACIENTE" + "," + "PROFESIONAL" + "," + "HORA" + "," + "ASEGURADORA" + "," + "RAZON");
                Escriba.WriteLine();
                Escriba.Flush();

                while (leer.Read())
                {
                    Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"].ToString()).ToString(getDataConection["Format_Fecha"]) + ",");
                    Escriba.Write(leer["Hor_Imp_Age"].ToString() + ",");
                    Escriba.Write(leer["Bod_Responsable"].ToString() + ",");
                    Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Hora_Cita"].ToString()).ToString("HH:mm tt") + ",");
                    Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                    Escriba.Write(leer["Hor_Pac_Inasistencia"].ToString());
                    Escriba.WriteLine();
                    Escriba.Flush();
                }
                Escriba.Close();
            }
        }
        #region Notas de enfermeria
        List<CXN_NOTASMED> MedidasHeridas(int Admision)
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

                    String Cargar_Hora2 = "SELECT * " +
                                          "FROM CXN_NOTASMED " +
                                          "WHERE Admision = @param1";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        Carga_Command2.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<CXN_NOTASMED> export_notas_report = new List<CXN_NOTASMED>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    export_notas_report.Add(new CXN_NOTASMED  
                                    {
                                        Ancho = Convert.ToDecimal(Lectura_Hora2["Ancho"]),
                                        Id = Convert.ToInt32(Lectura_Hora2["Id"]),
                                        Largo = Convert.ToDecimal(Lectura_Hora2["Largo"]),
                                        Profundidad = Convert.ToDecimal(Lectura_Hora2["Profundidad"]),
                                        Total = Convert.ToDecimal(Lectura_Hora2["Total"]),                                        
                                        Evolucion = Lectura_Hora2["Evolucion"].ToString(),
                                        NovedadHerida = Lectura_Hora2["NovedadHerida"].ToString(),
                                        Admision = Convert.ToInt32(Lectura_Hora2["Admision"]),
                                        txtGranulacion = Lectura_Hora2["txtGranulacion"].ToString(),
                                        txtFibrina = Lectura_Hora2["txtFibrina"].ToString(),
                                        txtNecroticoHumedo = Lectura_Hora2["txtNecroticoHumedo"].ToString(),
                                        txtNecroticoSeco = Lectura_Hora2["txtNecroticoSeco"].ToString(),
                                        txtEpitelizacion = Lectura_Hora2["txtEpitelizacion"].ToString(),
                                        txtOtros = Lectura_Hora2["txtOtros"].ToString(),
                                        txtLocalizacion = Lectura_Hora2["txtLocalizacion"].ToString(),
                                        selExudado = Lectura_Hora2["selExudado"].ToString()
                                    });
                                }
                                return export_notas_report;
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
        List<ReportNotas> IReportes.NotasMetodo(int Admision)
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

                    String Cargar_Hora2 = "SELECT Co.Com_Nombre, Co.Com_Direccion, Co.Com_Telefono, " +
                                          "P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_Id, P.Pac_TipoId, P.Pac_IdNum, P.Pac_FechaNto, P.Pac_Sexo, P.Pac_ECivil, P.Pac_Ocupacion, P.Pac_Direccion, P.Pac_Telefono + ' ' + P.Pac_TelefonoAux AS Tels, " +
                                          "P.Pac_Residencia, P.VIH, P.Hepatitis, " +
                                          "P.Pac_Regimen, P.Pac_Acudiente, P.Pac_TelefonoAcu, P.Pac_Parentesco, " +
                                          "A.Ase_Descripcion, " +
                                          "B.Bod_Reg_Med, B.Bod_Responsable, " +
                                          "H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, H.Hor_Pac_Hora_Salida, " +
                                          "N.Not_Nota, N.Not_Observa, N.Not_Edad, N.Not_CaracTej, N.Not_Recomienda , N.Not_Adm, N.Not_Epidemia, N.Not_Adherencia, N.Not_NotaAcla, N.Not_Acompañante, N.Not_Telefono, " +
                                          "C.Car_Cod, C.Car_Item, C.Car_Detalle, C.Car_Cant " +
                                          "FROM CXN_NOTAS N " +
                                          "INNER JOIN CXN_HORARIO H ON N.Not_Adm = H.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                          "INNER JOIN CXN_CIA Co ON H.Hor_Pac_Cia = Co.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                          "INNER JOIN CXN_CARGOS C ON H.Hor_Id = C.Car_Adm_Id " +
                                          "WHERE N.Not_Adm = '" + Admision + "' " +
                                          "AND C.Car_Tipo IN ('Cargo','Nota')";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<CXN_NOTASMED> lista = MedidasHeridas(Admision);

                                var LogoImage = Genera_QR_Notas(Admision);
                                if (LogoImage == null)
                                {
                                    return null;
                                }

                                List<ReportNotas> export_notas_report = new List<ReportNotas>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    var Dato1 = repoCIE10.BuscaDX(LogoImage.DX1.ToString());
                                    var Dato2 = repoCIE10.BuscaDX(LogoImage.DX2.ToString());
                                    var Dato3 = repoCIE10.BuscaDX(LogoImage.DX3.ToString());

                                    string CondicionesText = repoCondiciones.getCondicionesForPrint(Convert.ToInt32(Lectura_Hora2["Pac_Id"]), Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]));

                                    export_notas_report.Add(new ReportNotas
                                    {
                                        Car_Cod = Lectura_Hora2["Car_Cod"].ToString(),
                                        Car_Item = Lectura_Hora2["Car_Item"].ToString(),
                                        Car_Cant = Convert.ToInt32(Lectura_Hora2["Car_Cant"]),
                                        Car_Detalle = Lectura_Hora2["Car_Detalle"].ToString(),
                                        EmpresaNombre = Lectura_Hora2["Com_Nombre"].ToString(), //
                                        EmpresaDireccion = Lectura_Hora2["Com_Direccion"].ToString(),  //
                                        EmpresaTelefono = Lectura_Hora2["Com_Telefono"].ToString(), //
                                        ProfesionalNombre = Lectura_Hora2["Bod_Responsable"].ToString(), //
                                        PacienteNombre = Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() + " " + Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString(), //
                                        PacienteAseguradora = Lectura_Hora2["Ase_Descripcion"].ToString(), //
                                        PacienteIdentificacion = Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString(), //
                                        RegistroMedico = Lectura_Hora2["Bod_Reg_Med"].ToString(), //
                                        Pac_Sexo = Lectura_Hora2["Pac_Sexo"].ToString(),
                                        Edad = Lectura_Hora2["Not_Edad"].ToString(),
                                        FNto = Convert.ToDateTime(Lectura_Hora2["Pac_FechaNto"]), //
                                        FechaBase = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]), //
                                        EmpresaIdentificacion = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Hora_Cita"]).ToString("HH:mm:ss tt"), //Hora de la cita
                                        Nota = Lectura_Hora2["Not_Nota"].ToString(),
                                        Observa = Lectura_Hora2["Not_Observa"].ToString(),
                                        Recomienda = Lectura_Hora2["Not_Recomienda"].ToString(),
                                        Admision = Admision,
                                        Epidemia = Lectura_Hora2["Not_Epidemia"].ToString(),
                                        Adherencia = Lectura_Hora2["Not_Adherencia"].ToString(),
                                        NotaAclaratoria = Lectura_Hora2["Not_NotaAcla"].ToString(),
                                        Diagnostico1 = LogoImage.DX1.ToString() + " - " + Dato1.ToString(),
                                        HC_EstadoEmo = LogoImage.DX1.ToString(),
                                        Diagnostico_Rel2 = LogoImage.DX2.ToString() + " - " + Dato2.ToString(),
                                        Diagnostico_Rel3 = LogoImage.DX3.ToString() + " - " + Dato3.ToString(),
                                        CaracTej = Lectura_Hora2["Not_CaracTej"].ToString().ToUpper(),
                                        Logo = LogoImage.QRNota,
                                        HoraSalida = (Lectura_Hora2["Hor_Pac_Hora_Salida"] == DBNull.Value ? Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Hora_Cita"]).AddMinutes(20) : Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Hora_Salida"])),
                                        Ocupacion = CondicionesText,
                                        HC_ActEje = Lectura_Hora2["Pac_Ocupacion"].ToString(), //Ocupacion
                                        Com_Direccion = Lectura_Hora2["Pac_ECivil"].ToString(), //Estado Civil
                                        HC_Altura = Lectura_Hora2["Tels"].ToString(), //Telefonos
                                        HC_AntAle = Lectura_Hora2["Pac_Direccion"].ToString(), //Direccion
                                        HC_AntFam = (Lectura_Hora2["Pac_Residencia"] != DBNull.Value ? repoPacinetes.getNamePais(Lectura_Hora2["Pac_Residencia"].ToString()) : ""), //Pais Residencia
                                        HC_AntFarma = (Lectura_Hora2["Pac_Regimen"] != DBNull.Value ? repoPacinetes.Carga_Regimen(Lectura_Hora2["Pac_Regimen"].ToString()) : ""), //Redimen
                                        HC_AntPat = Lectura_Hora2["Pac_Acudiente"].ToString(), //Acudiente
                                        HC_AparienciaG = Lectura_Hora2["Pac_TelefonoAcu"].ToString(), //Tel Acudiente
                                        HC_AntQui = Lectura_Hora2["Pac_Parentesco"].ToString(), //Parentesco
                                        HC_EnfA = Lectura_Hora2["Not_Acompañante"].ToString(), //Acompañante
                                        HC_Cardiovascular = Lectura_Hora2["Not_Telefono"].ToString(), //Tel Acompañante
                                        VIH = Lectura_Hora2["VIH"] == DBNull.Value ? "Negativo" :
                                              Lectura_Hora2["VIH"].ToString() != "N" ? "Positivo" : "Negativo",
                                        listaMedidas = lista,
                                        Hepatitis = Lectura_Hora2["Hepatitis"] == DBNull.Value ? "Negativo" :
                                                                        Lectura_Hora2["Hepatitis"].ToString() != "N" ? "Positivo" : "Negativo"
                                    });
                                }
                                return export_notas_report;
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
        Dictionary<int, List<ReportNotas>> IReportes.NotasMetodoRDLC(int PacienteRDLC, DateTime Desde, DateTime Hasta)
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
                    String Cargar_Hora2 = "SELECT Com_Nombre, Com_Direccion, Com_Telefono, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Id, " +
                                          "Ase_Descripcion, Pac_TipoId, Pac_IdNum, Bod_Reg_Med, Pac_Sexo, Not_Edad, Pac_FechaNto, Hor_Pac_Fecha_Cita, Not_Nota, Not_Observa, " +
                                          "Not_Recomienda , Not_Adm, Not_Pac, Car_Cod, Car_Item, Car_Detalle, Car_Cant, Not_Epidemia, Not_Adherencia, Not_NotaAcla, Pac_Ocupacion, Pac_ECivil, Pac_Telefono + ' ' + Pac_TelefonoAux AS Tels, Pac_Direccion, Pac_Residencia, " +
                                          "Pac_Regimen, Pac_Acudiente, Pac_TelefonoAcu, Pac_Parentesco, Not_Acompañante, Not_Telefono, VIH, Hepatitis " +
                                          "FROM CXN_NOTAS " +
                                          "INNER JOIN CXN_HORARIO ON CXN_NOTAS.Not_Adm = CXN_HORARIO.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                          "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                          "INNER JOIN CXN_CARGOS ON CXN_HORARIO.Hor_Id = CXN_CARGOS.Car_Adm_Id " +
                                          "WHERE CXN_NOTAS.Not_Pac = '" + PacienteRDLC + "' " +
                                          "AND CXN_NOTAS.Not_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "' " +
                                          "AND CXN_CARGOS.Car_Tipo IN ('Nota')";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                Dictionary<int, List<ReportNotas>> D = new Dictionary<int, List<ReportNotas>>();
                                int Contador = 1;

                                while (Lectura_Hora2.Read() == true)
                                {
                                    List<ReportNotas> export_notas_report = new List<ReportNotas>();

                                    var LogoImage = Genera_QR_Notas(Convert.ToInt32(Lectura_Hora2["Not_Adm"]));
                                    if (LogoImage == null)
                                    {
                                        MessageBox.Show("Error inesperado en QR del reporte", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return null;
                                    }

                                    var Dato1 = repoCIE10.BuscaDX(LogoImage.DX1.ToString());
                                    var Dato2 = repoCIE10.BuscaDX(LogoImage.DX2.ToString());
                                    var Dato3 = repoCIE10.BuscaDX(LogoImage.DX3.ToString());

                                    string CondicionesText = repoCondiciones.getCondicionesForPrint(Convert.ToInt32(Lectura_Hora2["Pac_Id"]), Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]));

                                    List<CXN_NOTASMED> lista = MedidasHeridas(Convert.ToInt32(Lectura_Hora2["Not_Adm"]));

                                    String Query = "SELECT Car_Cod, Car_Item, Car_Cant, Car_Detalle " +
                                                   "FROM CXN_CARGOS " +
                                                   "WHERE Car_Adm_Id = '" + Lectura_Hora2["Not_Adm"].ToString() + "' " +
                                                   "AND Car_Pac = '" + Lectura_Hora2["Not_Pac"].ToString() + "' " +
                                                   "AND Car_Tipo IN ('Nota','Cargo')";
                                    SqlCommand Commando = new SqlCommand(Query, con);
                                    SqlDataReader Reader = (Commando.ExecuteReader());
                                    if (Reader.HasRows)
                                    {
                                        while (Reader.Read() == true)
                                        {
                                            export_notas_report.Add(new ReportNotas
                                            {
                                                Car_Cod = Reader["Car_Cod"].ToString(),
                                                Car_Item = Reader["Car_Item"].ToString(),
                                                Car_Cant = Convert.ToInt32(Reader["Car_Cant"]),
                                                Car_Detalle = Reader["Car_Detalle"].ToString(),
                                                EmpresaNombre = Lectura_Hora2["Com_Nombre"].ToString(), //
                                                EmpresaDireccion = Lectura_Hora2["Com_Direccion"].ToString(),  //
                                                EmpresaTelefono = Lectura_Hora2["Com_Telefono"].ToString(), //
                                                ProfesionalNombre = Lectura_Hora2["Bod_Responsable"].ToString(), //
                                                PacienteNombre = Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() + " " + Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString(), //
                                                PacienteAseguradora = Lectura_Hora2["Ase_Descripcion"].ToString(), //
                                                PacienteIdentificacion = Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString(), //
                                                RegistroMedico = Lectura_Hora2["Bod_Reg_Med"].ToString(), //
                                                Pac_Sexo = Lectura_Hora2["Pac_Sexo"].ToString(),
                                                Edad = Lectura_Hora2["Not_Edad"].ToString(),
                                                FNto = Convert.ToDateTime(Lectura_Hora2["Pac_FechaNto"]), //
                                                FechaBase = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]), //
                                                Nota = Lectura_Hora2["Not_Nota"].ToString().ToUpper(),
                                                Observa = Lectura_Hora2["Not_Observa"].ToString().ToUpper(),
                                                Recomienda = Lectura_Hora2["Not_Recomienda"].ToString().ToUpper(),
                                                Admision = Convert.ToInt32(Lectura_Hora2["Not_Adm"]),
                                                Epidemia = Lectura_Hora2["Not_Epidemia"].ToString().ToUpper(),
                                                Adherencia = Lectura_Hora2["Not_Adherencia"].ToString(),
                                                NotaAclaratoria = Lectura_Hora2["Not_NotaAcla"].ToString().ToUpper(),
                                                Diagnostico1 = LogoImage.DX1.ToString() + " - " + Dato1.ToString(),
                                                Diagnostico_Rel2 = LogoImage.DX2.ToString() + " - " + Dato2.ToString(),
                                                Diagnostico_Rel3 = LogoImage.DX3.ToString() + " - " + Dato3.ToString(),
                                                Logo = LogoImage.QRNota,
                                                Ocupacion = CondicionesText,
                                                HC_ActEje = Lectura_Hora2["Pac_Ocupacion"].ToString(), //Ocupacion
                                                Com_Direccion = Lectura_Hora2["Pac_ECivil"].ToString(), //Estado Civil
                                                HC_Altura = Lectura_Hora2["Tels"].ToString(), //Telefonos
                                                HC_AntAle = Lectura_Hora2["Pac_Direccion"].ToString(), //Direccion
                                                HC_AntFam = (Lectura_Hora2["Pac_Residencia"] != DBNull.Value ? repoPacinetes.getNamePais(Lectura_Hora2["Pac_Residencia"].ToString()) : ""), //Pais Residencia
                                                HC_AntFarma = (Lectura_Hora2["Pac_Regimen"] != DBNull.Value ? repoPacinetes.Carga_Regimen(Lectura_Hora2["Pac_Regimen"].ToString()) : ""), //Redimen
                                                HC_AntPat = Lectura_Hora2["Pac_Acudiente"].ToString(), //Acudiente
                                                HC_AparienciaG = Lectura_Hora2["Pac_TelefonoAcu"].ToString(), //Tel Acudiente
                                                HC_AntQui = Lectura_Hora2["Pac_Parentesco"].ToString(), //Parentesco
                                                HC_EnfA = Lectura_Hora2["Not_Acompañante"].ToString(), //Acompañante
                                                HC_Cardiovascular = Lectura_Hora2["Not_Telefono"].ToString(), //Tel Acompañante
                                                VIH = Lectura_Hora2["VIH"] == DBNull.Value ? "Negativo" :
                                                      Lectura_Hora2["VIH"].ToString() != "N" ? "Positivo" : "Negativo",
                                                listaMedidas = lista,
                                                Hepatitis = Lectura_Hora2["Hepatitis"] == DBNull.Value ? "Negativo" :
                                                                        Lectura_Hora2["Hepatitis"].ToString() != "N" ? "Positivo" : "Negativo"
                                            });
                                        }

                                        D.Add(Contador, export_notas_report);
                                        Contador = Contador + 1;
                                    }
                                }

                                return D;
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
        Dictionary<int, object> IReportes.NotasMetodoRDLC(Dictionary<int, string> Dic) 
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

                    Dictionary<int, object> D = new Dictionary<int, object>();
                    int Contador = 1;

                    foreach (int adm in Dic.Keys)
                    {
                        if (Dic[adm] == "Nota")
                        {
                            String Cargar_Hora2 = "SELECT Com_Nombre, Com_Direccion, Com_Telefono, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, Pac_Id, Pac_Ocupacion, Pac_ECivil, Pac_Direccion, Pac_Telefono + ' ' + Pac_TelefonoAux AS Tels, Pac_Residencia, " +
                                         "Ase_Descripcion, Pac_TipoId, Pac_IdNum, Bod_Reg_Med, Pac_Sexo, Not_Edad, Pac_FechaNto, Hor_Pac_Fecha_Cita, Not_Nota, Not_Observa, " +
                                         "Not_Recomienda , Not_Adm, Not_Pac, Car_Cod, Car_Item, Car_Detalle, Car_Cant, Not_Epidemia, Not_Adherencia, Not_NotaAcla, Hor_Pac_Hora_Cita, Hor_Pac_Hora_Salida, " +
                                         "Pac_Regimen, Pac_Acudiente, Pac_TelefonoAcu, Pac_Parentesco, Not_Acompañante, Not_Telefono, VIH, Hepatitis " +
                                         "FROM CXN_NOTAS " +
                                         "INNER JOIN CXN_HORARIO ON CXN_NOTAS.Not_Adm = CXN_HORARIO.Hor_id " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                         "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CARGOS ON CXN_HORARIO.Hor_Id = CXN_CARGOS.Car_Adm_Id " +
                                         "WHERE CXN_NOTAS.Not_Adm = '" + adm + "' " +
                                         "AND CXN_CARGOS.Car_Tipo IN ('Nota')";

                            using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                            {
                                using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                                {
                                    if (Lectura_Hora2.Read() == true)
                                    {
                                        List<CXN_NOTASMED> lista = MedidasHeridas(adm);
                                        List<ReportNotas> export_notas_report = new List<ReportNotas>();

                                        ListaNota LogoImage = Genera_QR_Notas(Convert.ToInt32(Lectura_Hora2["Not_Adm"]));
                                        if (LogoImage == null)
                                        {
                                            MessageBox.Show("Error inesperado en QR del reporte", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return null;
                                        }

                                        string Dato1 = repoCIE10.BuscaDX(LogoImage.DX1.ToString());
                                        string Dato2 = repoCIE10.BuscaDX(LogoImage.DX2.ToString());
                                        string Dato3 = repoCIE10.BuscaDX(LogoImage.DX3.ToString());

                                        String Query = "SELECT Car_Cod, Car_Item, Car_Cant, Car_Detalle " +
                                                       "FROM CXN_CARGOS " +
                                                       "WHERE Car_Adm_Id = '" + Lectura_Hora2["Not_Adm"].ToString() + "' " +
                                                       "AND Car_Pac = '" + Lectura_Hora2["Not_Pac"].ToString() + "' " +
                                                       "AND Car_Tipo IN ('Nota','Cargo')";
                                        using (SqlCommand Commando = new SqlCommand(Query, con))
                                        {
                                            using (SqlDataReader Reader = (Commando.ExecuteReader()))
                                            {
                                                if (Reader.HasRows)
                                                {
                                                    while (Reader.Read() == true)
                                                    {
                                                        string CondicionesText = repoCondiciones.getCondicionesForPrint(Convert.ToInt32(Lectura_Hora2["Pac_Id"]), Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]));

                                                        export_notas_report.Add(new ReportNotas
                                                        {
                                                            Car_Cod = Reader["Car_Cod"].ToString(),
                                                            Car_Item = Reader["Car_Item"].ToString(),
                                                            Car_Cant = Convert.ToInt32(Reader["Car_Cant"]),
                                                            Car_Detalle = Reader["Car_Detalle"].ToString(),
                                                            EmpresaNombre = Lectura_Hora2["Com_Nombre"].ToString(), //
                                                            EmpresaDireccion = Lectura_Hora2["Com_Direccion"].ToString(),  //
                                                            EmpresaTelefono = Lectura_Hora2["Com_Telefono"].ToString(), //
                                                            ProfesionalNombre = Lectura_Hora2["Bod_Responsable"].ToString(), //
                                                            PacienteNombre = Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() + " " + Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString(), //
                                                            PacienteAseguradora = Lectura_Hora2["Ase_Descripcion"].ToString(), //
                                                            PacienteIdentificacion = Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString(), //
                                                            RegistroMedico = Lectura_Hora2["Bod_Reg_Med"].ToString(), //
                                                            Pac_Sexo = Lectura_Hora2["Pac_Sexo"].ToString(),
                                                            Edad = Lectura_Hora2["Not_Edad"].ToString(),
                                                            FNto = Convert.ToDateTime(Lectura_Hora2["Pac_FechaNto"]), //
                                                            FechaBase = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]), //
                                                            Nota = Lectura_Hora2["Not_Nota"].ToString().ToUpper(),
                                                            Observa = Lectura_Hora2["Not_Observa"].ToString().ToUpper(),
                                                            Recomienda = Lectura_Hora2["Not_Recomienda"].ToString().ToUpper(),
                                                            Admision = Convert.ToInt32(Lectura_Hora2["Not_Adm"]),
                                                            Epidemia = Lectura_Hora2["Not_Epidemia"].ToString().ToUpper(),
                                                            Adherencia = Lectura_Hora2["Not_Adherencia"].ToString(),
                                                            NotaAclaratoria = Lectura_Hora2["Not_NotaAcla"].ToString().ToUpper(),
                                                            Diagnostico1 = LogoImage.DX1.ToString() + " - " + Dato1.ToString(),
                                                            Diagnostico_Rel2 = LogoImage.DX2.ToString() + " - " + Dato2.ToString(),
                                                            Diagnostico_Rel3 = LogoImage.DX3.ToString() + " - " + Dato3.ToString(),
                                                            Logo = LogoImage.QRNota,
                                                            HoraSalida = (Lectura_Hora2["Hor_Pac_Hora_Salida"] == DBNull.Value ? Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Hora_Cita"]).AddMinutes(20) : Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Hora_Salida"])),
                                                            EmpresaIdentificacion = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Hora_Cita"]).ToString("HH:mm:ss tt"), //Hora de la cita 
                                                            Ocupacion = CondicionesText,
                                                            HC_ActEje = Lectura_Hora2["Pac_Ocupacion"].ToString(), //Ocupacion
                                                            Com_Direccion = Lectura_Hora2["Pac_ECivil"].ToString(), //Estado Civil
                                                            HC_Altura = Lectura_Hora2["Tels"].ToString(), //Telefonos
                                                            HC_AntAle = Lectura_Hora2["Pac_Direccion"].ToString(), //Direccion
                                                            HC_AntFam = (Lectura_Hora2["Pac_Residencia"] != DBNull.Value ? repoPacinetes.getNamePais(Lectura_Hora2["Pac_Residencia"].ToString()) : ""), //Pais Residencia
                                                            HC_AntFarma = (Lectura_Hora2["Pac_Regimen"] != DBNull.Value ? repoPacinetes.Carga_Regimen(Lectura_Hora2["Pac_Regimen"].ToString()) : ""), //Redimen
                                                            HC_AntPat = Lectura_Hora2["Pac_Acudiente"].ToString(), //Acudiente
                                                            HC_AparienciaG = Lectura_Hora2["Pac_TelefonoAcu"].ToString(), //Tel Acudiente
                                                            HC_AntQui = Lectura_Hora2["Pac_Parentesco"].ToString(), //Parentesco
                                                            HC_EnfA = Lectura_Hora2["Not_Acompañante"].ToString(), //Acompañante
                                                            HC_Cardiovascular = Lectura_Hora2["Not_Telefono"].ToString(), //Tel Acompañante
                                                            VIH = Lectura_Hora2["VIH"] == DBNull.Value ? "Negativo" :
                                                                  Lectura_Hora2["VIH"].ToString() != "N" ? "Positivo" : "Negativo",
                                                            listaMedidas = lista,
                                                            Hepatitis = Lectura_Hora2["Hepatitis"] == DBNull.Value ? "Negativo" :
                                                                        Lectura_Hora2["Hepatitis"].ToString() != "N" ? "Positivo" : "Negativo"
                                                        });
                                                    }

                                                    D.Add(Contador, export_notas_report);
                                                    Contador = Contador + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (Dic[adm] == "Historia")
                        {
                            String Cargar_Hora = "SELECT TOP 1 Com_Nombre,  Com_Identificacion, Com_Direccion, Com_Telefono, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_IdNum, Pac_Direccion, Pac_FechaNto, Pac_Sexo, HC_Edad, Pac_Telefono, HC_SubPat, HC_Patologia, Hor_Pac_Hora_Cita, Hor_Pac_Hora_Salida, " +
                                            " Ase_Descripcion, Pac_Email, HC_Ocupacion, Pac_Acudiente, Pac_Parentesco, Pac_DireccionAcu, Pac_TelefonoAcu, Pac_CorreoAcu, HC_Vez, HC_Fecha, HC_MotivoC, HC_EnfA, HC_Neurologico, HC_Cardiovascular, " +
                                            " HC_Gastrourinario, HC_Piel, HC_Respiratorio, HC_Gastrointestinal, HC_Osteomuscular, HC_AntQui, HC_AntFam, HC_AntPat, HC_AntFarma, HC_AntAle, HC_Hematolin, HC_Nota_Acl, HC_Presart, HC_FreCar, " +
                                            " HC_FreRes, HC_Temp, HC_Peso, HC_IMC, HC_Altura, HC_ITB, HC_AparienciaG, HC_EstadoEmo, HC_EstadoNut, HC_GradoC, HC_ActEje, HC_TipoLes, HC_DescHer, HC_CantHer, HC_Bolsillo, HC_Longitud, HC_Ancho, " +
                                            " HC_Profundidad, HC_NumCav, HC_ConsCant,HC_TejCom,  HC_CaracTej, HC_Exudado, HC_SignosInf, HC_PielCirc, HC_TamañoHP, HC_Analisis, HC_PManejo, HC_ProtoInst, HC_Dx1T, HC_Dx2, HC_Dx3, HC_PruebasDiag, HC_Complicacion, " +
                                            " HC_Dx1, Bod_Firma, Bod_Responsable, HC_Dolor, HC_Estado, ' ' AS Vacio, HC_PacId, Pac_Id, HC_Ase, Ase_Identificador, HC_Com, Com_Identificador, HC_Prof, Bod_Numero, HC_Adm, " +
                                            " 'Motivo de Consulta' as MCONS, 'Enfermedad Actual' as ENFAC, 'Revision a Sistemas' as RSIST, 'Neurologico' as NEURO, 'Cardiovascular' as CARDIO, 'Gastrourinario' as GASTROU, 'OsteoMuscular' as OSTEO, " +
                                            " 'GastroIntestinal' as GASTROI, 'Piel' as PIEL, 'Respiratorio' as RESPI, 'ANTECEDENTES' as ANT, 'Quirurgicos' as QUI, 'Familiares' as FAMI, 'Patologicos' as PATO, 'Farmacologicos' as FARMA, " +
                                            " 'Alergicos' AS ALER, 'Hematologico y Linfatico' HEMA, 'Examen Fisico' AS EXAF, 'Presion Arterial' AS PRES, 'Frecuencia Cardiaca' AS FRECC, 'Frec. Respiratoria' AS FRECR, 'Temperatura' AS TEMP,  " +
                                            " 'Peso' AS PESO, 'IMC' AS IMC, 'Talla' AS Talla, 'Indice Tobillo Brazo' AS ITB, 'Apariencia General' AS APAG, 'Estado Emocional' AS ESTE, 'Estado Nutricional' AS ESTN, 'Grado de Cuidado' AS GCUI, " +
                                            " 'Actividad y Ejercicio' AS ACEJ, 'Tipo de Lesion' AS TLES, 'Descripcion de la Herida' AS DESH, 'Consistencia y Cantidad de Exudado' AS CONC, 'Tejidos Comprometidos' AS TCOM, " +
                                            " 'Caracteristicas del Tejido' AS CTEJ, 'Exudado' AS EXUD, 'Signos de Infeccion' AS SINF, 'Piel Circundante' AS PCIR, 'Tamaño' AS TAMA, " +
                                            " 'PLAN DE MANEJO' AS PMAN, 'Analisis' AS ANAL, 'Plan de Manejo' AS PMAN2, 'Diagnostico' AS DIAG, 'Pruebas Diagnosticas Complementarias' AS PDC, 'Complicaciones Durante el Tratamiento en esta Institucion' AS CTRA, 'Notas Aclaratorias' AS NOTAA, 'Firma y Sello' AS FIRMA, HC_Epidemia " +
                                            " FROM CXN_HCMG " +
                                            " INNER JOIN CXN_PACIENTES ON CXN_HCMG.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                            " INNER JOIN CXN_HORARIO ON CXN_HCMG.HC_ADM = CXN_HORARIO.Hor_Id " +
                                            " INNER JOIN CXN_ASEGURADORA ON CXN_HCMG.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                            " INNER JOIN CXN_CIA ON CXN_HCMG.HC_Com = CXN_CIA.Com_Identificador " +
                                            " INNER JOIN CXN_BODEGAS ON CXN_HCMG.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                            " WHERE CXN_HCMG.HC_Adm = '" + adm + "'";

                            using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                            {
                                using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                                {
                                    if (Lectura_Hora.Read() == true)
                                    {
                                        List<HCMG> sub_Class_HCMG = new List<HCMG>();

                                        string Bod_Firma1 = Lectura_Hora["Bod_Firma"].ToString();
                                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                                        MemoryStream stmBLOBData = new MemoryStream(bytes);
                                        PictureBox pic = new PictureBox();
                                        pic.Image = Image.FromStream(stmBLOBData);

                                        string Diag1 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx1"].ToString());
                                        string Diag2 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx2"].ToString());
                                        string Diag3 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx3"].ToString());

                                        sub_Class_HCMG.Add(new HCMG
                                        {
                                            PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                            Admision = adm,
                                            Firma = repoGen.GetBytes(pic.Image), //
                                            Ocupacion = Lectura_Hora["HC_Ocupacion"].ToString(), //
                                            EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(), //
                                            EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(), //
                                            EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(), //
                                            EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(), //
                                            HC_Vez = Lectura_Hora["HC_Vez"].ToString(),
                                            FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()), //
                                            ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(), //
                                            RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(), //
                                            PacienteNombre = Lectura_Hora["HC_Pac"].ToString(), //
                                            PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                            Pac_Acudiente = Lectura_Hora["Pac_Acudiente"].ToString(),
                                            Pac_Parentesco = Lectura_Hora["Pac_Parentesco"].ToString(),
                                            Pac_DireccionAcu = Lectura_Hora["Pac_DireccionAcu"].ToString(),
                                            Pac_TelefonoAcu = Lectura_Hora["Pac_TelefonoAcu"].ToString(),
                                            Pac_CorreoAcu = Lectura_Hora["Pac_CorreoAcu"].ToString(),
                                            PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                            FNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"].ToString()),
                                            Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                            Edad = Lectura_Hora["HC_Edad"].ToString(), //
                                            PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                            Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                            HC_MotivoC = Lectura_Hora["HC_MotivoC"].ToString(),
                                            HC_EnfA = Lectura_Hora["HC_EnfA"].ToString(),
                                            HC_Neurologico = Lectura_Hora["HC_Neurologico"].ToString(),
                                            HC_Cardiovascular = Lectura_Hora["HC_Cardiovascular"].ToString(),
                                            HC_Gastrourinario = Lectura_Hora["HC_Gastrourinario"].ToString(),
                                            HC_Osteomuscular = Lectura_Hora["HC_Osteomuscular"].ToString(),
                                            HC_Gastrointestinal = Lectura_Hora["HC_Gastrointestinal"].ToString(),
                                            HC_Piel = Lectura_Hora["HC_Piel"].ToString(),
                                            HC_Respiratorio = Lectura_Hora["HC_Respiratorio"].ToString(),
                                            HC_AntQui = Lectura_Hora["HC_AntQui"].ToString(),
                                            HC_AntFam = Lectura_Hora["HC_AntFam"].ToString(),
                                            HC_AntPat = Lectura_Hora["HC_AntPat"].ToString(),
                                            HC_AntFarma = Lectura_Hora["HC_AntFarma"].ToString(),
                                            HC_AntAle = Lectura_Hora["HC_AntAle"].ToString(),
                                            HC_Hematolin = Lectura_Hora["HC_Hematolin"].ToString(),
                                            HC_Presart = Lectura_Hora["HC_Presart"].ToString(),
                                            HC_FreCar = Lectura_Hora["HC_FreCar"].ToString(),
                                            HC_FreRes = Lectura_Hora["HC_FreRes"].ToString(),
                                            HC_Temp = Lectura_Hora["HC_Temp"].ToString(),
                                            HC_Peso = Lectura_Hora["HC_Peso"].ToString(),
                                            HC_IMC = Lectura_Hora["HC_IMC"].ToString(),
                                            HC_Altura = Lectura_Hora["HC_Altura"].ToString(),
                                            HC_ITB = Lectura_Hora["HC_ITB"].ToString(),
                                            HC_AparienciaG = Lectura_Hora["HC_AparienciaG"].ToString(),
                                            HC_EstadoEmo = Lectura_Hora["HC_EstadoEmo"].ToString(),
                                            HC_EstadoNut = Lectura_Hora["HC_EstadoNut"].ToString(),
                                            HC_GradoC = Lectura_Hora["HC_GradoC"].ToString(),
                                            HC_ActEje = Lectura_Hora["HC_ActEje"].ToString(),
                                            HC_TipoLes = Lectura_Hora["HC_TipoLes"].ToString(),
                                            HC_DescHer = Lectura_Hora["HC_DescHer"].ToString(),
                                            HC_ConsCant = Lectura_Hora["HC_ConsCant"].ToString(),
                                            HC_TejCom = Lectura_Hora["HC_TejCom"].ToString(),
                                            HC_CaracTej = Lectura_Hora["HC_CaracTej"].ToString(),
                                            HC_Exudado = Lectura_Hora["HC_Exudado"].ToString(),
                                            HC_SignosInf = Lectura_Hora["HC_SignosInf"].ToString(),
                                            HC_PielCirc = Lectura_Hora["HC_PielCirc"].ToString(),
                                            HC_Analisis = Lectura_Hora["HC_Analisis"].ToString(),
                                            HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                            Diagnostico1 = Lectura_Hora["HC_Dx1"].ToString() + " - " + Diag1.ToString(),
                                            Diagnostico_Rel2 = Lectura_Hora["HC_Dx2"].ToString() + " - " + Diag2.ToString(),
                                            Diagnostico_Rel3 = Lectura_Hora["HC_Dx3"].ToString() + " - " + Diag3.ToString(),
                                            HC_PruebasDiag = Lectura_Hora["HC_PruebasDiag"].ToString(),
                                            HC_Complicacion = Lectura_Hora["HC_Complicacion"].ToString(),
                                            NotaAclaratoria = Lectura_Hora["HC_Nota_Acl"].ToString(),
                                            HC_Dolor = Lectura_Hora["HC_Dolor"].ToString(),
                                            HC_Estado = Lectura_Hora["HC_Estado"].ToString(),
                                            Epidemia = Lectura_Hora["HC_Epidemia"].ToString(),
                                            HC_Patologia = Lectura_Hora["HC_Patologia"].ToString(),
                                            HoraSalida = (Lectura_Hora["Hor_Pac_Hora_Salida"] == DBNull.Value ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]).AddMinutes(25) : Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Salida"])),
                                            HC_ServCatalogo = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]).ToString("HH:mm:ss tt"), //hora de la cita
                                        });

                                        D.Add(Contador, sub_Class_HCMG);
                                        Contador = Contador + 1;
                                    }
                                }
                            }
                        }
                    }

                    return D;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        #endregion Notas de Enferemeria
        ListaNota Genera_QR_Notas(int Admi)
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

                    String Cargar_Hora2 = "SELECT Top 1 P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerN, " +
                                          "P.Pac_SegundoN, H.Hor_Pac_Fecha_Cita, P.Pac_TipoId, " +
                                          "P.Pac_IdNum, C.Com_Nombre, CA.Car_Dx1, CA.Car_Dx2, CA.Car_Dx3 " +
                                          "FROM CXN_NOTAS N " +
                                          "INNER JOIN CXN_HORARIO H ON N.Not_Adm = H.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                          "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                          "INNER JOIN CXN_CARGOS CA ON H.Hor_Id = CA.Car_Adm_Id " +
                                          "WHERE N.Not_Adm = '" + Admi + "' " +
                                          "AND CA.Car_Tipo = 'Nota'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
                    {
                        ListaNota L = new ListaNota();
                        string Datos = "PACIENTE: " + Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString() + " " + Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() +
                                       " FECHA ATENCION: " + Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]).ToString(getData["Format_Fecha"]) +
                                       " IDENTIFICACION: " + Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString() +
                                       " CLINICA: " + Lectura_Hora2["Com_Nombre"].ToString();
                        var ImaRes = repoGen.CodifyQR(Datos);

                        L.DX1 = Lectura_Hora2["Car_Dx1"].ToString();
                        L.DX2 = Lectura_Hora2["Car_Dx2"].ToString();
                        L.DX3 = Lectura_Hora2["Car_Dx3"].ToString();
                        L.QRNota = repoGen.GetBytes(ImaRes);
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
        List<CXN_HORARIO> IReportes.BuscarHistorias(int Paciente, string Tipo)
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

                    List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                    String Cargar_Hora2 = "";

                    switch (Tipo)
                    {
                        case "Notas":
                            Cargar_Hora2 = "SELECT '0' AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                          "P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, H.Hor_Pac_Fecha_Cita AS FECHA, B.Bod_Responsable AS PROF, Not_Adm AS ADM " +
                                         " FROM CXN_NOTAS N " +
                                         " INNER JOIN CXN_HORARIO H ON N.Not_Adm = H.Hor_Id " +
                                         " INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         " INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         " WHERE P.Pac_Id = '" + Paciente + "' " +
                                         " ORDER BY H.Hor_Pac_Fecha_Cita DESC";
                            break;

                        case "HisMG":
                            Cargar_Hora2 = "SELECT H.HC_Pacid AS PacienteCom, H.HC_Adm AS ADM, P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                          "B.Bod_Responsable AS PROF, H.HC_Fecha AS FECHA " +
                                          "FROM CXN_HCMG H " +
                                          "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                          "INNER JOIN CXN_CIA C ON H.HC_Com = C.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS B ON H.HC_Prof = B.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA A ON H.HC_Ase = A.Ase_Identificador " +
                                          "WHERE P.Pac_Id = '" + Paciente + "' " +
                                          "ORDER BY H.HC_Fecha DESC";
                            break;

                        case "CMan":
                            Cargar_Hora2 = "SELECT '0' AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                           "C.Cam_Id AS ADM, P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, B.Bod_Responsable AS PROF, C.Cam_Fecha AS FECHA " +
                                           "FROM CXN_PACIENTES P " +
                                           "INNER JOIN CXN_CMAN C ON P.Pac_Id = C.Cam_IdPac " +
                                           "INNER JOIN CXN_CIA CI ON C.Cam_Cia = CI.Com_Identificador " +
                                           "INNER JOIN CXN_BODEGAS B ON C.Cam_UsrGenera = B.Bod_Usuario " +
                                           "INNER JOIN CXN_ASEGURADORA A ON P.Pac_Aseguradora = A.Ase_Identificador " +
                                           "WHERE P.Pac_Id = '" + Paciente + "' " +
                                           "ORDER BY C.Cam_Fecha DESC";
                            break;

                        case "HFI":
                            Cargar_Hora2 = "SELECT P.Pac_Id AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                           "H.HC_Adm AS ADM, P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, B.Bod_Responsable AS PROF, H.HC_Fecha AS FECHA " +
                                         " FROM CXN_HCFI H " +
                                         " INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                         " INNER JOIN CXN_CIA C ON H.HC_Cia = C.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS B ON H.HC_Prof = B.Bod_Numero " +
                                         " INNER JOIN CXN_ASEGURADORA A ON H.HC_Ase = A.Ase_Identificador " +
                                         " WHERE P.Pac_Id = '" + Paciente + "' " +
                                         " ORDER BY H.HC_Fecha DESC";
                            break;

                        case "TF":
                            Cargar_Hora2 = "SELECT '0' AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                           "H.HC_Adm AS ADM, P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, B.Bod_Responsable AS PROF, H.HC_Fecha AS FECHA " +
                                           " FROM CXN_HCTF H " +
                                           " INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                           " INNER JOIN CXN_BODEGAS B ON H.HC_Prof = B.Bod_Numero " +
                                           " WHERE P.Pac_Id = '" + Paciente + "' " +
                                           " ORDER BY H.HC_Fecha DESC";
                            break;

                        case "TO":
                            Cargar_Hora2 = "SELECT '0' AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                           "H.HC_Adm AS ADM, P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, B.Bod_Responsable AS PROF, H.HC_Fecha AS FECHA " +
                                           " FROM CXN_HCTO H " +
                                           " INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                           " INNER JOIN CXN_BODEGAS B ON H.HC_Prof = B.Bod_Numero " +
                                           " WHERE P.Pac_Id = '" + Paciente + "' " +
                                           " ORDER BY H.HC_Fecha DESC";
                            break;

                        case "PSI":
                            Cargar_Hora2 = "SELECT '0' AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                           "H.HC_Adm AS ADM, P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, B.Bod_Responsable AS PROF, H.HC_Fecha AS FECHA " +
                                         " FROM CXN_HCPSI H " +
                                         " INNER JOIN CXN_PACIENTES P ON H.HC_Pac_Id = P.Pac_Id " +
                                         " INNER JOIN CXN_BODEGAS B ON H.HC_Prof = B.Bod_Numero " +
                                         " WHERE P.Pac_Id = '" + Paciente + "' " +
                                         " ORDER BY H.HC_Fecha DESC";
                            break;

                        case "HJ1":
                            Cargar_Hora2 = "SELECT '0' AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                           "P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, Pac_Id, J.Jun_Adm AS ADM, J.Jun_Fecha AS FECHA, B.Bod_Responsable AS PROF " +
                                         " FROM CXN_HCJUNTAS J " +
                                         " INNER JOIN CXN_PACIENTES P ON J.Jun_Pac = P.Pac_Id " +
                                         " INNER JOIN CXN_BODEGAS B ON J.Jun_Bodega = B.Bod_Numero " +
                                         " WHERE P.Pac_Id = '" + Paciente + "' " +
                                         " AND J.Jun_Tipo = 'Junta1' " +
                                         " ORDER BY J.Jun_Fecha DESC";
                            break;

                        case "HJ2":
                            Cargar_Hora2 = "SELECT '0' AS PacienteCom, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                           "P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, Pac_Id, J.Jun_Adm AS ADM, J.Jun_Fecha AS FECHA, B.Bod_Responsable AS PROF " +
                                         " FROM CXN_HCJUNTAS J " +
                                         " INNER JOIN CXN_PACIENTES P ON J.Jun_Pac = P.Pac_Id " +
                                         " INNER JOIN CXN_BODEGAS B ON J.Jun_Bodega = B.Bod_Numero " +
                                         " WHERE P.Pac_Id = '" + Paciente + "' " +
                                         " AND J.Jun_Tipo = 'Junta2' " +
                                         " ORDER BY J.Jun_Fecha DESC";
                            break;

                        case "RA":
                            Cargar_Hora2 = "SELECT H.Pacid AS PacienteCom, H.HCAdm AS ADM, P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                          "B.Bod_Responsable AS PROF, H.Fecha AS FECHA " +
                                          "FROM CXN_HCRADIOLOGIA H " +
                                          "INNER JOIN CXN_PACIENTES P ON H.PacId = P.Pac_Id " +
                                          "INNER JOIN CXN_CIA C ON H.Compañia = C.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS B ON H.Medico = B.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA A ON H.Aseguradora = A.Ase_Identificador " +
                                          "WHERE P.Pac_Id = '" + Paciente + "' " +
                                          "ORDER BY H.Fecha DESC";
                            break;

                        case "HEVO":
                            String Cargar_HoraEVO = "SELECT P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PACIENTE, " +
                                          "P.Pac_TipoId AS TID, P.Pac_IdNum AS ID, E.Evo_Adm AS ADM, E.Evo_Fecha AS FECHA, B.Bod_Responsable AS PROF, E.Evo_Tipo " +
                                          " FROM CXN_EVOFIB E " +
                                          " INNER JOIN CXN_PACIENTES P ON E.Evo_Pac = P.Pac_Id " +
                                          " INNER JOIN CXN_BODEGAS B ON E.Evo_Med = B.Bod_Usuario " +
                                          " WHERE P.Pac_Id = '" + Paciente + "' " +
                                          " ORDER BY E.Evo_Fecha DESC";
                            SqlCommand Carga_CommandEVO = new SqlCommand(Cargar_HoraEVO, con);
                            SqlDataReader Lectura_HoraEVO = (Carga_CommandEVO.ExecuteReader());
                            if (Lectura_HoraEVO.HasRows)
                            {
                                while (Lectura_HoraEVO.Read() == true)
                                {
                                    if (Lectura_HoraEVO["ADM"] != DBNull.Value)
                                    {
                                        H.Add(new CXN_HORARIO
                                        {
                                            Hor_Id = Convert.ToInt32(Lectura_HoraEVO["ADM"]),
                                            Com_Tipo_Doc = Lectura_HoraEVO["TID"].ToString(),
                                            PacienteIdentificacion = Lectura_HoraEVO["ID"].ToString(),
                                            Hor_Imp_Age = Lectura_HoraEVO["PACIENTE"].ToString(),
                                            Hor_Observacion = Lectura_HoraEVO["PROF"].ToString(),
                                            Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_HoraEVO["FECHA"]),
                                            Hor_Pac_Tipo_Serv = Lectura_HoraEVO["Evo_Tipo"].ToString()
                                        });
                                    }                                   
                                }

                                return H;
                            }
                            else
                            {
                                return null;
                            }

                        default:
                            return null;
                    }

                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        while (Lectura_Hora.Read() == true)
                        {
                            H.Add(new CXN_HORARIO
                            {
                                Hor_Pac_Id = Convert.ToInt32(Lectura_Hora["PacienteCom"]),
                                Hor_Id = Convert.ToInt32(Lectura_Hora["ADM"]),
                                Com_Tipo_Doc = Lectura_Hora["TID"].ToString(),
                                PacienteIdentificacion = Lectura_Hora["ID"].ToString(),
                                Hor_Imp_Age = Lectura_Hora["PACIENTE"].ToString(),
                                Hor_Observacion = Lectura_Hora["PROF"].ToString(),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["FECHA"])
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
        List<HCMG> IReportes.MedicinaGeneral(int Admision)
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

                    String Cargar_Hora = "SELECT Com_Nombre,  Com_Identificacion, Com_Direccion, Com_Telefono, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_IdNum, Pac_Direccion, Pac_FechaNto, Pac_Sexo, HC_Edad, Pac_Telefono, HC_SubPat, HC_Patologia, " +
                                         " Ase_Descripcion, Pac_Email, HC_Ocupacion, Pac_Acudiente, Pac_Parentesco, Pac_DireccionAcu, Pac_TelefonoAcu, Pac_CorreoAcu, HC_Vez, HC_Fecha, HC_MotivoC, HC_EnfA, HC_Neurologico, HC_Cardiovascular, " +
                                         " HC_Gastrourinario, HC_Piel, HC_Respiratorio, HC_Gastrointestinal, HC_Osteomuscular, HC_AntQui, HC_AntFam, HC_AntPat, HC_AntFarma, HC_AntAle, HC_Hematolin, HC_Nota_Acl, HC_Presart, HC_FreCar, " +
                                         " HC_FreRes, HC_Temp, HC_Peso, HC_IMC, HC_Altura, HC_ITB, HC_AparienciaG, HC_EstadoEmo, HC_EstadoNut, HC_GradoC, HC_ActEje, HC_TipoLes, HC_DescHer, HC_CantHer, HC_Bolsillo, HC_Longitud, HC_Ancho, " +
                                         " HC_Profundidad, HC_NumCav, HC_ConsCant,HC_TejCom,  HC_CaracTej, HC_Exudado, HC_SignosInf, HC_PielCirc, HC_TamañoHP, HC_Analisis, HC_PManejo, HC_ProtoInst, HC_Dx1T, HC_Dx2, HC_Dx3, HC_PruebasDiag, HC_Complicacion, " +
                                         " HC_Dx1, Bod_Firma, Bod_Responsable, HC_Dolor, HC_Estado, ' ' AS Vacio, HC_PacId, Pac_Id, HC_Ase, Ase_Identificador, HC_Com, Com_Identificador, HC_Prof, Bod_Numero, HC_Adm, " +
                                         " 'Motivo de Consulta' as MCONS, 'Enfermedad Actual' as ENFAC, 'Revision a Sistemas' as RSIST, 'Neurologico' as NEURO, 'Cardiovascular' as CARDIO, 'Gastrourinario' as GASTROU, 'OsteoMuscular' as OSTEO, " +
                                         " 'GastroIntestinal' as GASTROI, 'Piel' as PIEL, 'Respiratorio' as RESPI, 'ANTECEDENTES' as ANT, 'Quirurgicos' as QUI, 'Familiares' as FAMI, 'Patologicos' as PATO, 'Farmacologicos' as FARMA, " +
                                         " 'Alergicos' AS ALER, 'Hematologico y Linfatico' HEMA, 'Examen Fisico' AS EXAF, 'Presion Arterial' AS PRES, 'Frecuencia Cardiaca' AS FRECC, 'Frec. Respiratoria' AS FRECR, 'Temperatura' AS TEMP,  " +
                                         " 'Peso' AS PESO, 'IMC' AS IMC, 'Talla' AS Talla, 'Indice Tobillo Brazo' AS ITB, 'Apariencia General' AS APAG, 'Estado Emocional' AS ESTE, 'Estado Nutricional' AS ESTN, 'Grado de Cuidado' AS GCUI, " +
                                         " 'Actividad y Ejercicio' AS ACEJ, 'Tipo de Lesion' AS TLES, 'Descripcion de la Herida' AS DESH, 'Consistencia y Cantidad de Exudado' AS CONC, 'Tejidos Comprometidos' AS TCOM, " +
                                         " 'Caracteristicas del Tejido' AS CTEJ, 'Exudado' AS EXUD, 'Signos de Infeccion' AS SINF, 'Piel Circundante' AS PCIR, 'Tamaño' AS TAMA, " +
                                         " 'PLAN DE MANEJO' AS PMAN, 'Analisis' AS ANAL, 'Plan de Manejo' AS PMAN2, 'Diagnostico' AS DIAG, 'Pruebas Diagnosticas Complementarias' AS PDC, 'Complicaciones Durante el Tratamiento en esta Institucion' AS CTRA, 'Notas Aclaratorias' AS NOTAA, 'Firma y Sello' AS FIRMA, HC_Epidemia, Hor_Pac_Hora_Cita, Hor_Pac_Hora_Salida " +
                                         " FROM CXN_HCMG " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCMG.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCMG.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCMG.HC_Com = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCMG.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " INNER JOIN CXN_HORARIO ON CXN_HCMG.HC_ADM = CXN_HORARIO.Hor_Id " +
                                         " WHERE CXN_HCMG.HC_Adm = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                List<HCMG> sub_Class_HCMG = new List<HCMG>();

                                string Bod_Firma1 = Lectura_Hora["Bod_Firma"].ToString();
                                Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                                MemoryStream stmBLOBData = new MemoryStream(bytes);
                                PictureBox pic = new PictureBox();
                                pic.Image = Image.FromStream(stmBLOBData);

                                var Diag1 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx1"].ToString());
                                var Diag2 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx2"].ToString());
                                var Diag3 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx3"].ToString());

                                string CondicionesText = repoCondiciones.getCondicionesForPrint(Convert.ToInt32(Lectura_Hora["Pac_Id"]), Convert.ToDateTime(Lectura_Hora["HC_Fecha"]));

                                sub_Class_HCMG.Add(new HCMG
                                {
                                    PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                    Admision = Admision,
                                    Firma = repoGen.GetBytes(pic.Image), //
                                    Ocupacion = Lectura_Hora["HC_Ocupacion"].ToString(), //
                                    EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(), //
                                    EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(), //
                                    EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(), //
                                    EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(), //
                                    HC_Vez = Lectura_Hora["HC_Vez"].ToString(),
                                    FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()), //
                                    HC_ServCatalogo = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]).ToString("HH:mm:ss tt"), //hora de la cita
                                    ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(), //
                                    RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(), //
                                    PacienteNombre = Lectura_Hora["HC_Pac"].ToString(), //
                                    PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                    Pac_Acudiente = Lectura_Hora["Pac_Acudiente"].ToString(),
                                    Pac_Parentesco = Lectura_Hora["Pac_Parentesco"].ToString(),
                                    Pac_DireccionAcu = Lectura_Hora["Pac_DireccionAcu"].ToString(),
                                    Pac_TelefonoAcu = Lectura_Hora["Pac_TelefonoAcu"].ToString(),
                                    Pac_CorreoAcu = Lectura_Hora["Pac_CorreoAcu"].ToString(),
                                    PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                    FNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"].ToString()),
                                    Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                    Edad = Lectura_Hora["HC_Edad"].ToString(), //
                                    PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                    Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                    HC_MotivoC = Lectura_Hora["HC_MotivoC"].ToString(),
                                    HC_EnfA = Lectura_Hora["HC_EnfA"].ToString(),
                                    HC_Neurologico = Lectura_Hora["HC_Neurologico"].ToString(),
                                    HC_Cardiovascular = Lectura_Hora["HC_Cardiovascular"].ToString(),
                                    HC_Gastrourinario = Lectura_Hora["HC_Gastrourinario"].ToString(),
                                    HC_Osteomuscular = Lectura_Hora["HC_Osteomuscular"].ToString(),
                                    HC_Gastrointestinal = Lectura_Hora["HC_Gastrointestinal"].ToString(),
                                    HC_Piel = Lectura_Hora["HC_Piel"].ToString(),
                                    HC_Respiratorio = Lectura_Hora["HC_Respiratorio"].ToString(),
                                    HC_AntQui = Lectura_Hora["HC_AntQui"].ToString(),
                                    HC_AntFam = Lectura_Hora["HC_AntFam"].ToString(),
                                    HC_AntPat = Lectura_Hora["HC_AntPat"].ToString(),
                                    HC_AntFarma = Lectura_Hora["HC_AntFarma"].ToString(),
                                    HC_AntAle = Lectura_Hora["HC_AntAle"].ToString(),
                                    HC_Hematolin = Lectura_Hora["HC_Hematolin"].ToString(),
                                    HC_Presart = Lectura_Hora["HC_Presart"].ToString(),
                                    HC_FreCar = Lectura_Hora["HC_FreCar"].ToString(),
                                    HC_FreRes = Lectura_Hora["HC_FreRes"].ToString(),
                                    HC_Temp = Lectura_Hora["HC_Temp"].ToString(),
                                    HC_Peso = Lectura_Hora["HC_Peso"].ToString(),
                                    HC_IMC = Lectura_Hora["HC_IMC"].ToString(),
                                    HC_Altura = Lectura_Hora["HC_Altura"].ToString(),
                                    HC_ITB = Lectura_Hora["HC_ITB"].ToString(),
                                    HC_AparienciaG = Lectura_Hora["HC_AparienciaG"].ToString(),
                                    HC_EstadoEmo = Lectura_Hora["HC_EstadoEmo"].ToString(),
                                    HC_EstadoNut = Lectura_Hora["HC_EstadoNut"].ToString(),
                                    HC_GradoC = Lectura_Hora["HC_GradoC"].ToString(),
                                    HC_ActEje = Lectura_Hora["HC_ActEje"].ToString(),
                                    HC_TipoLes = Lectura_Hora["HC_TipoLes"].ToString(),
                                    HC_DescHer = Lectura_Hora["HC_DescHer"].ToString(),
                                    HC_ConsCant = Lectura_Hora["HC_ConsCant"].ToString(),
                                    HC_TejCom = Lectura_Hora["HC_TejCom"].ToString(),
                                    HC_CaracTej = Lectura_Hora["HC_CaracTej"].ToString(),
                                    HC_Exudado = Lectura_Hora["HC_Exudado"].ToString(),
                                    HC_SignosInf = Lectura_Hora["HC_SignosInf"].ToString(),
                                    HC_PielCirc = Lectura_Hora["HC_PielCirc"].ToString(),
                                    HC_Analisis = Lectura_Hora["HC_Analisis"].ToString(),
                                    HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                    Diagnostico1 = Lectura_Hora["HC_Dx1"].ToString() + " - " + Diag1.ToString(),
                                    Diagnostico_Rel2 = Lectura_Hora["HC_Dx2"].ToString() + " - " + Diag2.ToString(),
                                    Diagnostico_Rel3 = Lectura_Hora["HC_Dx3"].ToString() + " - " + Diag3.ToString(),

                                    DX1Code = Lectura_Hora["HC_Dx1"].ToString(),
                                    DX2Code = Lectura_Hora["HC_Dx2"].ToString(),
                                    DX3Code = Lectura_Hora["HC_Dx3"].ToString(),

                                    HC_PruebasDiag = Lectura_Hora["HC_PruebasDiag"].ToString(),
                                    HC_Complicacion = Lectura_Hora["HC_Complicacion"].ToString(),
                                    NotaAclaratoria = Lectura_Hora["HC_Nota_Acl"].ToString(),
                                    HC_Dolor = Lectura_Hora["HC_Dolor"].ToString(),
                                    HC_Estado = Lectura_Hora["HC_Estado"].ToString(),
                                    Epidemia = Lectura_Hora["HC_Epidemia"].ToString(),
                                    HC_Patologia = Lectura_Hora["HC_Patologia"].ToString(),
                                    HoraSalida = (Lectura_Hora["Hor_Pac_Hora_Salida"] == DBNull.Value ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]).AddMinutes(25) : Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Salida"])),
                                    Med_Determinacion = CondicionesText
                                });

                                return sub_Class_HCMG;
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
        List<HCMG> IReportes.MedicinaGeneralCompleto(int PacienteHCCompleto, DateTime Desde, DateTime Hasta, string userGenera)
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

                    String Cargar_Hora = "SELECT Com_Nombre,  Com_Identificacion, Com_Direccion, Com_Telefono, Com_Logo, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_IdNum, Pac_Direccion, Pac_FechaNto, Pac_Sexo, HC_Edad, Pac_Telefono, HC_SubPat, HC_Patologia, " +
                                         " Ase_Descripcion, Pac_Email, HC_Ocupacion, Pac_Acudiente, Pac_Parentesco, Pac_DireccionAcu, Pac_TelefonoAcu, Pac_CorreoAcu, HC_Vez, HC_Fecha, HC_MotivoC, HC_EnfA, HC_Neurologico, HC_Cardiovascular, " +
                                         " HC_Gastrourinario, HC_Piel, HC_Respiratorio, HC_Gastrointestinal, HC_Osteomuscular, HC_AntQui, HC_AntFam, HC_AntPat, HC_AntFarma, HC_AntAle, HC_Hematolin, HC_Nota_Acl, HC_Presart, HC_FreCar, " +
                                         " HC_FreRes, HC_Temp, HC_Peso, HC_IMC, HC_Altura, HC_ITB, HC_AparienciaG, HC_EstadoEmo, HC_EstadoNut, HC_GradoC, HC_ActEje, HC_TipoLes, HC_DescHer, HC_CantHer, HC_Bolsillo, HC_Longitud, HC_Ancho, " +
                                         " HC_Profundidad, HC_NumCav, HC_ConsCant,HC_TejCom,  HC_CaracTej, HC_Exudado, HC_SignosInf, HC_PielCirc, HC_TamañoHP, HC_Analisis, HC_PManejo, HC_ProtoInst, HC_Dx1T, HC_Dx2, HC_Dx3, HC_PruebasDiag, HC_Complicacion, " +
                                         " HC_Dx1, Bod_Firma, Bod_Responsable, HC_Dolor, HC_Estado, ' ' AS Vacio, HC_PacId, Pac_Id, HC_Ase, Ase_Identificador, HC_Com, Com_Identificador, HC_Prof, Bod_Numero, HC_Adm, " +
                                         " 'Motivo de Consulta' as MCONS, 'Enfermedad Actual' as ENFAC, 'Revision a Sistemas' as RSIST, 'Neurologico' as NEURO, 'Cardiovascular' as CARDIO, 'Gastrourinario' as GASTROU, 'OsteoMuscular' as OSTEO, " +
                                         " 'GastroIntestinal' as GASTROI, 'Piel' as PIEL, 'Respiratorio' as RESPI, 'ANTECEDENTES' as ANT, 'Quirurgicos' as QUI, 'Familiares' as FAMI, 'Patologicos' as PATO, 'Farmacologicos' as FARMA, " +
                                         " 'Alergicos' AS ALER, 'Hematologico y Linfatico' HEMA, 'Examen Fisico' AS EXAF, 'Presion Arterial' AS PRES, 'Frecuencia Cardiaca' AS FRECC, 'Frec. Respiratoria' AS FRECR, 'Temperatura' AS TEMP,  " +
                                         " 'Peso' AS PESO, 'IMC' AS IMC, 'Talla' AS Talla, 'Indice Tobillo Brazo' AS ITB, 'Apariencia General' AS APAG, 'Estado Emocional' AS ESTE, 'Estado Nutricional' AS ESTN, 'Grado de Cuidado' AS GCUI, " +
                                         " 'Actividad y Ejercicio' AS ACEJ, 'Tipo de Lesion' AS TLES, 'Descripcion de la Herida' AS DESH, 'Consistencia y Cantidad de Exudado' AS CONC, 'Tejidos Comprometidos' AS TCOM, " +
                                         " 'Caracteristicas del Tejido' AS CTEJ, 'Exudado' AS EXUD, 'Signos de Infeccion' AS SINF, 'Piel Circundante' AS PCIR, 'Tamaño' AS TAMA, " +
                                         " 'PLAN DE MANEJO' AS PMAN, 'Analisis' AS ANAL, 'Plan de Manejo' AS PMAN2, 'Diagnostico' AS DIAG, 'Pruebas Diagnosticas Complementarias' AS PDC, 'Complicaciones Durante el Tratamiento en esta Institucion' AS CTRA, " +
                                         " 'Notas Aclaratorias' AS NOTAA, 'Firma y Sello' AS FIRMA, HC_Epidemia, HC_ServCatalogo, HC_CupCatalogo " +
                                         " FROM CXN_HCMG " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCMG.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCMG.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCMG.HC_Com = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCMG.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE CXN_HCMG.HC_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "' " +
                                         " AND CXN_HCMG.HC_Pacid = '" + PacienteHCCompleto + "' " +
                                         " ORDER BY CXN_HCMG.HC_Fecha DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<HCMG> sub_Class_HCMG = new List<HCMG>();
                        DateTime Hoy = DateTime.Now.Date;

                        while (Lectura_Hora.Read() == true)
                        {
                            string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString();
                            Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                            MemoryStream stmBLOBData = new MemoryStream(bytes);
                            PictureBox pic = new PictureBox();
                            pic.Image = Image.FromStream(stmBLOBData);

                            var Diag1 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx1"].ToString());
                            var Diag2 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx2"].ToString());
                            var Diag3 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx3"].ToString());

                            string Datos = "PACIENTE: " + Lectura_Hora["HC_Pac"].ToString() + "\n\r" +
                                           "FECHA REPORTE: " + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "\n\r" +
                                           "IDENTIFICACION: " + Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString() + "\n\r" +
                                           "CLINICA: " + Lectura_Hora["Com_Nombre"].ToString() + "\n\r" +
                                           "USUARIO QUE GENERA COPIA: " + userGenera;
                            var ImaRes = repoGen.CodifyQR(Datos);

                            string CondicionesText = repoCondiciones.getCondicionesForPrint(Convert.ToInt32(Lectura_Hora["Pac_Id"]), Convert.ToDateTime(Lectura_Hora["HC_Fecha"]));

                            sub_Class_HCMG.Add(new HCMG
                            {
                                Logo = repoGen.GetBytes(ImaRes),
                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                Admision = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                                Firma = repoGen.GetBytes(pic.Image), //
                                Ocupacion = Lectura_Hora["HC_Ocupacion"].ToString(), //
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(), //
                                EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(), //
                                EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(), //
                                EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(), //
                                HC_Vez = Lectura_Hora["HC_Vez"].ToString(),
                                FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()), //
                                ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(), //
                                RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(), //
                                PacienteNombre = Lectura_Hora["HC_Pac"].ToString(), //
                                PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                Pac_Acudiente = Lectura_Hora["Pac_Acudiente"].ToString(),
                                Pac_Parentesco = Lectura_Hora["Pac_Parentesco"].ToString(),
                                Pac_DireccionAcu = Lectura_Hora["Pac_DireccionAcu"].ToString(),
                                Pac_TelefonoAcu = Lectura_Hora["Pac_TelefonoAcu"].ToString(),
                                Pac_CorreoAcu = Lectura_Hora["Pac_CorreoAcu"].ToString(),
                                PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                FNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"].ToString()),
                                Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                Edad = Lectura_Hora["HC_Edad"].ToString(), //
                                PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                HC_MotivoC = Lectura_Hora["HC_MotivoC"].ToString(),
                                HC_EnfA = Lectura_Hora["HC_EnfA"].ToString(),
                                HC_Neurologico = Lectura_Hora["HC_Neurologico"].ToString(),
                                HC_Cardiovascular = Lectura_Hora["HC_Cardiovascular"].ToString(),
                                HC_Gastrourinario = Lectura_Hora["HC_Gastrourinario"].ToString(),
                                HC_Osteomuscular = Lectura_Hora["HC_Osteomuscular"].ToString(),
                                HC_Gastrointestinal = Lectura_Hora["HC_Gastrointestinal"].ToString(),
                                HC_Piel = Lectura_Hora["HC_Piel"].ToString(),
                                HC_Respiratorio = Lectura_Hora["HC_Respiratorio"].ToString(),
                                HC_AntQui = Lectura_Hora["HC_AntQui"].ToString(),
                                HC_AntFam = Lectura_Hora["HC_AntFam"].ToString(),
                                HC_AntPat = Lectura_Hora["HC_AntPat"].ToString(),
                                HC_AntFarma = Lectura_Hora["HC_AntFarma"].ToString(),
                                HC_AntAle = Lectura_Hora["HC_AntAle"].ToString(),
                                HC_Hematolin = Lectura_Hora["HC_Hematolin"].ToString(),
                                HC_Presart = Lectura_Hora["HC_Presart"].ToString(),
                                HC_FreCar = Lectura_Hora["HC_FreCar"].ToString(),
                                HC_FreRes = Lectura_Hora["HC_FreRes"].ToString(),
                                HC_Temp = Lectura_Hora["HC_Temp"].ToString(),
                                HC_Peso = Lectura_Hora["HC_Peso"].ToString(),
                                HC_IMC = Lectura_Hora["HC_IMC"].ToString(),
                                HC_Altura = Lectura_Hora["HC_Altura"].ToString(),
                                HC_ITB = Lectura_Hora["HC_ITB"].ToString(),
                                HC_AparienciaG = Lectura_Hora["HC_AparienciaG"].ToString(),
                                HC_EstadoEmo = Lectura_Hora["HC_EstadoEmo"].ToString(),
                                HC_EstadoNut = Lectura_Hora["HC_EstadoNut"].ToString(),
                                HC_GradoC = Lectura_Hora["HC_GradoC"].ToString(),
                                HC_ActEje = Lectura_Hora["HC_ActEje"].ToString(),
                                HC_TipoLes = Lectura_Hora["HC_TipoLes"].ToString(),
                                HC_DescHer = Lectura_Hora["HC_DescHer"].ToString(),
                                HC_ConsCant = Lectura_Hora["HC_ConsCant"].ToString(),
                                HC_TejCom = Lectura_Hora["HC_TejCom"].ToString(),
                                HC_CaracTej = Lectura_Hora["HC_CaracTej"].ToString(),
                                HC_Exudado = Lectura_Hora["HC_Exudado"].ToString(),
                                HC_SignosInf = Lectura_Hora["HC_SignosInf"].ToString(),
                                HC_PielCirc = Lectura_Hora["HC_PielCirc"].ToString(),
                                HC_Analisis = Lectura_Hora["HC_Analisis"].ToString(),
                                HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                Diagnostico1 = Lectura_Hora["HC_Dx1"].ToString() + " - " + Diag1.ToString(),
                                Diagnostico_Rel2 = Lectura_Hora["HC_Dx2"].ToString() + " - " + Diag2.ToString(),
                                Diagnostico_Rel3 = Lectura_Hora["HC_Dx3"].ToString() + " - " + Diag3.ToString(),
                                HC_PruebasDiag = Lectura_Hora["HC_PruebasDiag"].ToString(),
                                HC_Complicacion = Lectura_Hora["HC_Complicacion"].ToString(),
                                NotaAclaratoria = Lectura_Hora["HC_Nota_Acl"].ToString(),
                                HC_Dolor = Lectura_Hora["HC_Dolor"].ToString(),
                                HC_Estado = Lectura_Hora["HC_Estado"].ToString(),
                                Epidemia = Lectura_Hora["HC_Epidemia"].ToString(),
                                HC_ServCatalogo = Lectura_Hora["HC_ServCatalogo"].ToString(),
                                HC_CupCatalogo = Lectura_Hora["HC_CupCatalogo"].ToString(),
                                HC_Patologia = Lectura_Hora["HC_Patologia"].ToString(),
                                Med_Determinacion = CondicionesText
                            });
                        }
                        return sub_Class_HCMG;
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
        List<HCMG> IReportes.DiametrosHeridas(int Admision)
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

                    String Cargar_Hora = "SELECT Bod_Firma, Med_Largo, Med_Ancho, Med_Profundidad, Med_Determinacion, Med_Observacion, HC_Ocupacion, Com_Nombre, Com_Identificacion, " +
                                      "Com_Direccion, Com_Telefono, HC_Vez, HC_Fecha, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_Acudiente, Pac_Parentesco, " +
                                      "Pac_DireccionAcu, Pac_TelefonoAcu, Pac_CorreoAcu, Pac_Direccion, Pac_FechaNto, Pac_Sexo, HC_Edad, Pac_Telefono, Ase_Descripcion, Pac_Email " +
                                      "FROM CXN_HCMG " +
                                      "INNER JOIN CXN_PACIENTES ON CXN_HCMG.Hc_PacId = CXN_PACIENTES.Pac_Id " +
                                      "INNER JOIN CXN_CIA ON CXN_HCMG.HC_Com = CXN_CIA.Com_Identificador " +
                                      "INNER JOIN CXN_BODEGAS ON CXN_HCMG.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                      "INNER JOIN CXN_ASEGURADORA ON CXN_HCMG.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                      "INNER JOIN CXN_HCMED ON CXN_HCMG.HC_Adm = CXN_HCMED.Med_Adm " +
                                      "WHERE CXN_HCMG.HC_Adm = '" + Admision + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<HCMG> sub_Class_HCMG = new List<HCMG>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            string Bod_Firma1 = Lectura_Hora2["Bod_Firma"].ToString();
                            Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                            MemoryStream stmBLOBData = new MemoryStream(bytes);
                            PictureBox pic = new PictureBox();
                            pic.Image = Image.FromStream(stmBLOBData);

                            sub_Class_HCMG.Add(new HCMG
                            {
                                Med_Largo = Lectura_Hora2["Med_Largo"].ToString(),
                                Med_Ancho = Lectura_Hora2["Med_Ancho"].ToString(),
                                Med_Profundidad = Lectura_Hora2["Med_Profundidad"].ToString(),
                                Med_Determinacion = Lectura_Hora2["Med_Determinacion"].ToString(),
                                Med_Observacion = Lectura_Hora2["Med_Observacion"].ToString(),
                                Firma = repoGen.GetBytes(pic.Image), //
                                Ocupacion = Lectura_Hora2["HC_Ocupacion"].ToString(), //
                                EmpresaNombre = Lectura_Hora2["Com_Nombre"].ToString(), //
                                EmpresaIdentificacion = Lectura_Hora2["Com_Identificacion"].ToString(), //
                                EmpresaDireccion = Lectura_Hora2["Com_Direccion"].ToString(), //
                                EmpresaTelefono = Lectura_Hora2["Com_Telefono"].ToString(), //
                                HC_Vez = Lectura_Hora2["HC_Vez"].ToString(), //
                                FechaBase = Convert.ToDateTime(Lectura_Hora2["HC_Fecha"].ToString()), //
                                ProfesionalNombre = Lectura_Hora2["Bod_Responsable"].ToString(), //
                                RegistroMedico = Lectura_Hora2["Bod_Reg_Med"].ToString(), //
                                PacienteNombre = Lectura_Hora2["HC_Pac"].ToString(), //
                                PacienteIdentificacion = Lectura_Hora2["Pac_TipoId"].ToString(), //
                                Pac_Acudiente = Lectura_Hora2["Pac_Acudiente"].ToString(),
                                Pac_Parentesco = Lectura_Hora2["Pac_Parentesco"].ToString(),
                                Pac_DireccionAcu = Lectura_Hora2["Pac_DireccionAcu"].ToString(),
                                Pac_TelefonoAcu = Lectura_Hora2["Pac_TelefonoAcu"].ToString(),
                                Pac_CorreoAcu = Lectura_Hora2["Pac_CorreoAcu"].ToString(),
                                PacienteDireccion = Lectura_Hora2["Pac_Direccion"].ToString(),
                                FNto = Convert.ToDateTime(Lectura_Hora2["Pac_FechaNto"].ToString()),
                                Pac_Sexo = Lectura_Hora2["Pac_Sexo"].ToString(),
                                Edad = Lectura_Hora2["HC_Edad"].ToString(), //
                                PacienteTelefono = Lectura_Hora2["Pac_Telefono"].ToString(),
                                PacienteAseguradora = Lectura_Hora2["Ase_Descripcion"].ToString(), //
                                Pac_Email = Lectura_Hora2["Pac_Email"].ToString()
                            });
                        }
                        return sub_Class_HCMG;
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
        List<ReportCMAN> IReportes.CambiosManejo(int Admision)
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
                    String Cargar_Hora = "SELECT Com_Logo, Com_Nombre, Com_Identificacion, Com_Telefono, Com_Direccion, Pac_PrimerA, " +
                                         "Pac_SegundoA, Pac_PrimerN, Pac_SegundoN, Pac_TipoId, Pac_IdNum, Ase_Descripcion, Cam_Fecha, " +
                                         "Bod_Responsable, Cam_ActualizaOb, Cam_Descripcion, Cam_UsrActualiza, Cam_Adherencia " +
                                         "FROM CXN_PACIENTES " +
                                         "INNER JOIN CXN_CMAN ON CXN_PACIENTES.Pac_Id = CXN_CMAN.Cam_IdPac " +
                                         "INNER JOIN CXN_CIA ON CXN_CMAN.Cam_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_CMAN.Cam_UsrGenera = CXN_BODEGAS.Bod_Usuario " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_PACIENTES.Pac_Aseguradora = CXN_ASEGURADORA.Ase_Identificador " +
                                         "AND Cam_Id = '" + Admision + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<ReportCMAN> sub_Class_CMAN = new List<ReportCMAN>();

                        string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString();
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                        MemoryStream stmBLOBData = new MemoryStream(bytes);
                        PictureBox pic = new PictureBox();
                        pic.Image = Image.FromStream(stmBLOBData);

                        sub_Class_CMAN.Add(new ReportCMAN
                        {
                            Logo = repoGen.GetBytes(pic.Image),
                            EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                            EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                            EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                            EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                            PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                            PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                            PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString().ToString(),
                            FechaBase = Convert.ToDateTime(Lectura_Hora["Cam_Fecha"]),
                            ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                            Cam_ActualizaOb = Lectura_Hora["Cam_ActualizaOb"].ToString(),
                            Cam_UsrActualiza = Lectura_Hora["Cam_UsrActualiza"].ToString(),
                            Adherencia = Lectura_Hora["Cam_Adherencia"].ToString(),
                            Cam_Descripcion = Lectura_Hora["Cam_Descripcion"].ToString()
                        });

                        return sub_Class_CMAN;
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
        List<ReportHCFI> IReportes.ReporteFisiatria(int Admition)
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

                    String Cargar_Hora = "SELECT Com_Direccion, Com_Telefono, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_IdNum, " +
                                         " Pac_Direccion, Ase_Descripcion, HC_FechaNto, Pac_Sexo, Pac_Email, Pac_Telefono, HC_Fecha, HC_MotCons, HC_EnfAct, " +
                                         " HC_Neurologico, HC_Mental, HC_Edad, HC_OrgSent, HC_Respiratorio, HC_Cardiovascular, HC_GastroI, " +
                                         " HC_GenitoU, HC_Hematolin, HC_Ant, HC_Presart, HC_Peso, HC_PerimetroC, HC_EstCons, HC_FreCar, HC_FrecResp, HC_Talla, " +
                                         " HC_PerimetroA, HC_Glasshow, HC_IMC, HC_Temp, HC_Embriaguez, HC_ObservaFis, HC_ObservaNeu, HC_Cabeza, HC_Cuello, " +
                                         " HC_Torax, HC_Pulmonar, HC_Cardiovascular, HC_Abdomen, HC_Ombligo, HC_Ano, HC_Extremidades, HC_PielFan, HC_Orl, " +
                                         " HC_OsteoM, HC_Genitales, HC_GenitoU, HC_Hematolin, Bod_Firma, HC_Egreso, HC_Analisis, HC_PManejo, HC_NotaA, HC_DX1, " +
                                         " HC_DX2, HC_DX3, HC_RH, HC_EAV, HC_Epidemia  " +
                                         " FROM CXN_HCFI " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCFI.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCFI.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCFI.HC_Cia = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCFI.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE CXN_HCFI.HC_Adm = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admition);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                List<ReportHCFI> sub_Class_HCFI = new List<ReportHCFI>();

                                string Bod_Firma1 = Lectura_Hora["Bod_Firma"].ToString();
                                Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                                MemoryStream stmBLOBData = new MemoryStream(bytes);
                                PictureBox pic = new PictureBox();
                                pic.Image = Image.FromStream(stmBLOBData);

                                var DX1 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX1"].ToString());
                                var DX2 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX2"].ToString());
                                var DX3 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX3"].ToString());

                                sub_Class_HCFI.Add(new ReportHCFI
                                {
                                    Firma = repoGen.GetBytes(pic.Image), //publico imagen
                                    EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(), //
                                    EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                    ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(), //
                                    RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),//
                                    PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                    PacienteNombre = Lectura_Hora["HC_Pac"].ToString(), //
                                    FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()),
                                    FNto = Convert.ToDateTime(Lectura_Hora["HC_FechaNto"].ToString()),
                                    PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                    PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(), //
                                    Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                    Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                    PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                    Diagnostico1 = Lectura_Hora["HC_DX1"].ToString() + " - " + DX1.ToString(), //
                                    Diagnostico_Rel2 = Lectura_Hora["HC_DX2"].ToString() + " - " + DX2.ToString(), //
                                    Diagnostico_Rel3 = Lectura_Hora["HC_DX3"].ToString() + " - " + DX3.ToString(), //
                                    DX1Code = Lectura_Hora["HC_DX1"].ToString(),
                                    DX2Code = Lectura_Hora["HC_DX2"].ToString(),
                                    DX3Code = Lectura_Hora["HC_DX3"].ToString(),
                                    HC_RH = Lectura_Hora["HC_RH"].ToString(),
                                    HC_EAV = Lectura_Hora["HC_EAV"].ToString(),
                                    HC_Gastrointestinal = Lectura_Hora["HC_GastroI"].ToString(),
                                    HC_Ant = Lectura_Hora["HC_Ant"].ToString(),
                                    HC_Neurologico = Lectura_Hora["HC_Neurologico"].ToString(),
                                    HC_Mental = Lectura_Hora["HC_Mental"].ToString(),
                                    HC_OrgSent = Lectura_Hora["HC_OrgSent"].ToString(),
                                    HC_Respiratorio = Lectura_Hora["HC_Respiratorio"].ToString(),
                                    HC_Cardiovascular = Lectura_Hora["HC_Cardiovascular"].ToString(),
                                    HC_Presart = Lectura_Hora["HC_Presart"].ToString(),
                                    HC_Peso = Lectura_Hora["HC_Peso"].ToString(),
                                    HC_PerimetroC = Lectura_Hora["HC_PerimetroC"].ToString(),
                                    HC_Estado = Lectura_Hora["HC_EstCons"].ToString(),
                                    HC_FreRes = Lectura_Hora["HC_FrecResp"].ToString(),
                                    HC_IMC = Lectura_Hora["HC_IMC"].ToString(),
                                    HC_Glasshow = Lectura_Hora["HC_Glasshow"].ToString(),
                                    HC_Temp = Lectura_Hora["HC_Temp"].ToString(),
                                    HC_Embriaguez = Lectura_Hora["HC_Embriaguez"].ToString(),
                                    HC_ObservaFis = Lectura_Hora["HC_ObservaFis"].ToString(),
                                    HC_ObservaNeu = Lectura_Hora["HC_ObservaNeu"].ToString(),
                                    HC_Cabeza = Lectura_Hora["HC_Cabeza"].ToString(),
                                    HC_Cuello = Lectura_Hora["HC_Cuello"].ToString(),
                                    HC_Torax = Lectura_Hora["HC_Torax"].ToString(),
                                    HC_Pulmonar = Lectura_Hora["HC_Pulmonar"].ToString(),
                                    HC_Abdomen = Lectura_Hora["HC_Abdomen"].ToString(),
                                    HC_Ombligo = Lectura_Hora["HC_Ombligo"].ToString(),
                                    HC_Ano = Lectura_Hora["HC_Ano"].ToString(),
                                    HC_Extremidades = Lectura_Hora["HC_Extremidades"].ToString(),
                                    HC_Piel = Lectura_Hora["HC_PielFan"].ToString(),
                                    HC_Orl = Lectura_Hora["HC_Orl"].ToString(),
                                    HC_Osteomuscular = Lectura_Hora["HC_OsteoM"].ToString(),
                                    HC_Genitales = Lectura_Hora["HC_Genitales"].ToString(),
                                    HC_Gastrourinario = Lectura_Hora["HC_GenitoU"].ToString(),
                                    HC_Hematolin = Lectura_Hora["HC_Hematolin"].ToString(),
                                    HC_Egreso = Lectura_Hora["HC_Egreso"].ToString(),
                                    HC_Analisis = Lectura_Hora["HC_Analisis"].ToString(),
                                    HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                    NotaAclaratoria = Lectura_Hora["HC_NotaA"].ToString(), //
                                    HC_MotivoC = Lectura_Hora["HC_MotCons"].ToString(), //
                                    HC_EnfA = Lectura_Hora["HC_EnfAct"].ToString(), //
                                    Edad = Lectura_Hora["HC_Edad"].ToString(),
                                    HC_PerimetroA = Lectura_Hora["HC_PerimetroA"].ToString(),
                                    HC_FreCar = Lectura_Hora["HC_FreCar"].ToString(),
                                    Epidemia = Lectura_Hora["HC_Epidemia"].ToString(),
                                    HC_Talla = Lectura_Hora["HC_Talla"].ToString()
                                });

                                return sub_Class_HCFI;
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
        List<ReportHCFI> IReportes.ReportefisiatriaCompleto(int PacienteCom, DateTime Fecha, string user)
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

                    String Cargar_Hora = "SELECT Com_Logo, Com_Direccion, Com_Telefono, Com_Nombre, Com_Identificacion, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_IdNum, " +
                                         " Pac_Direccion, Ase_Descripcion, HC_FechaNto, Pac_Sexo, Pac_Email, Pac_Telefono, HC_Fecha, HC_MotCons, HC_EnfAct, " +
                                         " HC_Neurologico, HC_Mental, HC_Edad, HC_OrgSent, HC_Respiratorio, HC_Cardiovascular, HC_GastroI, " +
                                         " HC_GenitoU, HC_Hematolin, HC_Ant, HC_Presart, HC_Peso, HC_PerimetroC, HC_EstCons, HC_FreCar, HC_FrecResp, HC_Talla, " +
                                         " HC_PerimetroA, HC_Adm, HC_Glasshow, HC_IMC, HC_Temp, HC_Embriaguez, HC_ObservaFis, HC_ObservaNeu, HC_Cabeza, HC_Cuello, " +
                                         " HC_Torax, HC_Pulmonar, HC_Cardiovascular, HC_Abdomen, HC_Ombligo, HC_Ano, HC_Extremidades, HC_PielFan, HC_Orl, " +
                                         " HC_OsteoM, HC_Genitales, HC_GenitoU, HC_Hematolin, Bod_Firma, HC_Egreso, HC_Analisis, HC_PManejo, HC_NotaA, HC_DX1, " +
                                         " HC_DX2, HC_DX3, HC_RH, HC_EAV, HC_Epidemia  " +
                                         " FROM CXN_HCFI " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCFI.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCFI.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCFI.HC_Cia = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCFI.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE CXN_HCFI.HC_Pacid = '" + PacienteCom + "' " +
                                         " AND CXN_HCFI.HC_Fecha <= '" + Convert.ToDateTime(Fecha).ToString(getData["Format_Fecha"]) + " '";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<ReportHCFI> sub_Class_HCFI = new List<ReportHCFI>();
                        DateTime Hoy = DateTime.Now.Date;

                        while (Lectura_Hora.Read() == true)
                        {
                            var DX1 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX1"].ToString());
                            var DX2 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX2"].ToString());
                            var DX3 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX3"].ToString());

                            string Datos = "PACIENTE: " + Lectura_Hora["HC_Pac"].ToString() + "\n\r" +
                                           "FECHA REPORTE: " + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "\n\r" +
                                           "IDENTIFICACION: " + Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString() + "\n\r" +
                                           "CLINICA: " + Lectura_Hora["Com_Nombre"].ToString() + "\n\r" +
                                           "USUARIO QUE GENERA COPIA: " + user;
                            var ImaRes = repoGen.CodifyQR(Datos);

                            sub_Class_HCFI.Add(new ReportHCFI
                            {
                                Admision = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                                Logo = repoGen.GetBytes(ImaRes),
                                EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(), //
                                EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(), //
                                RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),//
                                PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                PacienteNombre = Lectura_Hora["HC_Pac"].ToString(), //
                                FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()),
                                FNto = Convert.ToDateTime(Lectura_Hora["HC_FechaNto"].ToString()),
                                PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(), //
                                Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                Diagnostico1 = Lectura_Hora["HC_DX1"].ToString() + " - " + DX1.ToString(), //
                                Diagnostico_Rel2 = Lectura_Hora["HC_DX2"].ToString() + " - " + DX2.ToString(), //
                                Diagnostico_Rel3 = Lectura_Hora["HC_DX3"].ToString() + " - " + DX3.ToString(), //
                                HC_RH = Lectura_Hora["HC_RH"].ToString(),
                                HC_EAV = Lectura_Hora["HC_EAV"].ToString(),
                                HC_Gastrointestinal = Lectura_Hora["HC_GastroI"].ToString(),
                                HC_Ant = Lectura_Hora["HC_Ant"].ToString(),
                                HC_Neurologico = Lectura_Hora["HC_Neurologico"].ToString(),
                                HC_Mental = Lectura_Hora["HC_Mental"].ToString(),
                                HC_OrgSent = Lectura_Hora["HC_OrgSent"].ToString(),
                                HC_Respiratorio = Lectura_Hora["HC_Respiratorio"].ToString(),
                                HC_Cardiovascular = Lectura_Hora["HC_Cardiovascular"].ToString(),
                                HC_Presart = Lectura_Hora["HC_Presart"].ToString(),
                                HC_Peso = Lectura_Hora["HC_Peso"].ToString(),
                                HC_PerimetroC = Lectura_Hora["HC_PerimetroC"].ToString(),
                                HC_Estado = Lectura_Hora["HC_EstCons"].ToString(),
                                HC_FreRes = Lectura_Hora["HC_FrecResp"].ToString(),
                                HC_IMC = Lectura_Hora["HC_IMC"].ToString(),
                                HC_Glasshow = Lectura_Hora["HC_Glasshow"].ToString(),
                                HC_Temp = Lectura_Hora["HC_Temp"].ToString(),
                                HC_Embriaguez = Lectura_Hora["HC_Embriaguez"].ToString(),
                                HC_ObservaFis = Lectura_Hora["HC_ObservaFis"].ToString(),
                                HC_ObservaNeu = Lectura_Hora["HC_ObservaNeu"].ToString(),
                                HC_Cabeza = Lectura_Hora["HC_Cabeza"].ToString(),
                                HC_Cuello = Lectura_Hora["HC_Cuello"].ToString(),
                                HC_Torax = Lectura_Hora["HC_Torax"].ToString(),
                                HC_Pulmonar = Lectura_Hora["HC_Pulmonar"].ToString(),
                                HC_Abdomen = Lectura_Hora["HC_Abdomen"].ToString(),
                                HC_Ombligo = Lectura_Hora["HC_Ombligo"].ToString(),
                                HC_Ano = Lectura_Hora["HC_Ano"].ToString(),
                                HC_Extremidades = Lectura_Hora["HC_Extremidades"].ToString(),
                                HC_Piel = Lectura_Hora["HC_PielFan"].ToString(),
                                HC_Orl = Lectura_Hora["HC_Orl"].ToString(),
                                HC_Osteomuscular = Lectura_Hora["HC_OsteoM"].ToString(),
                                HC_Genitales = Lectura_Hora["HC_Genitales"].ToString(),
                                HC_Gastrourinario = Lectura_Hora["HC_GenitoU"].ToString(),
                                HC_Hematolin = Lectura_Hora["HC_Hematolin"].ToString(),
                                HC_Egreso = Lectura_Hora["HC_Egreso"].ToString(),
                                HC_Analisis = Lectura_Hora["HC_Analisis"].ToString(),
                                HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                NotaAclaratoria = Lectura_Hora["HC_NotaA"].ToString(), //
                                HC_MotivoC = Lectura_Hora["HC_MotCons"].ToString(), //
                                HC_EnfA = Lectura_Hora["HC_EnfAct"].ToString(), //
                                Edad = Lectura_Hora["HC_Edad"].ToString(),
                                HC_PerimetroA = Lectura_Hora["HC_PerimetroA"].ToString(),
                                HC_FreCar = Lectura_Hora["HC_FreCar"].ToString(),
                                Epidemia = Lectura_Hora["HC_Epidemia"].ToString(),
                                HC_Talla = Lectura_Hora["HC_Talla"].ToString()
                            });
                        }
                        return sub_Class_HCFI;
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
        List<CXN_HCRADIOLOGIA> IReportes.ReporteRadiologiaCompleto(int PacienteCom, DateTime Desde, DateTime Hasta, string user)
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

                    String Cargar_Hora = "SELECT C.Com_Nombre,  C.Com_Identificacion, C.Com_Direccion, C.Com_Telefono, B.Bod_Responsable, B.Bod_Reg_Med, R.PacienteNombre, R.PacienteTipoId, R.PacienteId, P.Pac_Direccion, P.Pac_FechaNto, P.Pac_Sexo, P.Pac_Telefono, " +
                                         "A.Ase_Descripcion, P.Pac_Email, P.Pac_Acudiente, P.Pac_Parentesco, P.Pac_DireccionAcu, P.Pac_TelefonoAcu, P.Pac_CorreoAcu, R.Fecha, " +
                                         "B.Bod_Firma, B.Bod_Responsable, A.Ase_Identificador, C.Com_Identificador, B.Bod_Numero, " +
                                         "H.Hor_Pac_Hora_Cita, H.Hor_Pac_Hora_Salida, " +
                                         "R.MotivoConsulta, R.EnfermedadActual, R.EvolucionSintomas, R.AntecedentesRelevantes, R.MedicamentosActuales, R.AntecedentesRenales, " +
                                         "R.AntecedentesCardioVasculares, R.Embarazo, R.ImplantesMetalicos, R.MenorEdad, R.Presion, R.Peso, R.GlassHow, R.Talla, R.FResp, " +
                                         "R.FCar, R.RH, R.Conciencia, R.IMC, R.ObservacionExaMedico, R.TipoEstudio, R.MedioContraste, R.ReaccionAdversa, R.Tecnica, " +
                                         "R.Hallazgos, R.DX1, R.DX2, R.DX3, R.NotaDX1, R.NotaDX2, R.NotaDX3, R.CausaExterna, R.ImpDX1, R.ImpDX2, R.ImpDX3, R.EstudioComplementario, " +
                                         "R.ControlSeguimiento, R.Compañia, R.NotaAclaratoria, R.PacId, R.Aseguradora, R.Medico, R.Fecha, R.HCAdm, R.HCCant, R.NotaAclaratoria " +
                                         "FROM CXN_HCRADIOLOGIA R " +
                                         "INNER JOIN CXN_PACIENTES P ON R.PacId = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON R.Aseguradora = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON R.Compañia = C.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS B ON R.Medico = B.Bod_Numero " +
                                         "INNER JOIN CXN_HORARIO H ON R.HCADM = H.Hor_Id " +
                                         "WHERE R.PacId = @param1 " +
                                         "AND R.Fecha BETWEEN @param2 AND @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", PacienteCom);
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param3", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HCRADIOLOGIA> sub_Class_HCFI = new List<CXN_HCRADIOLOGIA>();
                                DateTime Hoy = DateTime.Now.Date;

                                while (Lectura_Hora.Read() == true)
                                {
                                    var DX1 = repoCIE10.BuscaDX(Lectura_Hora["DX1"].ToString());
                                    var DX2 = repoCIE10.BuscaDX(Lectura_Hora["DX2"].ToString());
                                    var DX3 = repoCIE10.BuscaDX(Lectura_Hora["DX3"].ToString());

                                    string Datos = "PACIENTE: " + Lectura_Hora["PacienteNombre"].ToString() + "\n\r" +
                                                   "FECHA REPORTE: " + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "\n\r" +
                                                   "IDENTIFICACION: " + Lectura_Hora["PacienteTipoId"].ToString() + " " + Lectura_Hora["PacienteId"].ToString() + "\n\r" +
                                                   "CLINICA: " + Lectura_Hora["Com_Nombre"].ToString() + "\n\r" +
                                                   "USUARIO QUE GENERA COPIA: " + user;
                                    var ImaRes = repoGen.CodifyQR(Datos);

                                    sub_Class_HCFI.Add(new CXN_HCRADIOLOGIA
                                    {
                                        Com_Nombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        Bod_Responsable = Lectura_Hora["Bod_Responsable"].ToString(),
                                        Bod_Reg_Med = Lectura_Hora["Bod_Reg_Med"].ToString(),
                                        Pac_Direccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                        Pac_FechaNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"]),
                                        Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                        Pac_Telefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                        Ase_Descripcion = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                        Pac_Acudiente = Lectura_Hora["Pac_Acudiente"].ToString(),
                                        Pac_Parentesco = Lectura_Hora["Pac_Parentesco"].ToString(),
                                        Pac_DireccionAcu = Lectura_Hora["Pac_DireccionAcu"].ToString(),
                                        Pac_TelefonoAcu = Lectura_Hora["Pac_TelefonoAcu"].ToString(),
                                        Pac_CorreoAcu = Lectura_Hora["Pac_CorreoAcu"].ToString(),
                                        //Bod_Firma = bytes,
                                        Hor_Pac_Hora_Salida = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Salida"]),
                                        Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                        PacId = Convert.ToInt32(Lectura_Hora["PacId"]),
                                        PacienteNombre = Lectura_Hora["PacienteNombre"].ToString(),
                                        PacienteTipoId = Lectura_Hora["PacienteTipoId"].ToString(),
                                        PacienteId = Lectura_Hora["PacienteId"].ToString(),
                                        MotivoConsulta = Lectura_Hora["MotivoConsulta"].ToString(),
                                        EnfermedadActual = Lectura_Hora["EnfermedadActual"].ToString(),
                                        EvolucionSintomas = Lectura_Hora["EvolucionSintomas"].ToString(),
                                        AntecedentesRelevantes = Lectura_Hora["AntecedentesRelevantes"].ToString(),
                                        MedicamentosActuales = Lectura_Hora["MedicamentosActuales"].ToString(),
                                        AntecedentesRenales = Lectura_Hora["AntecedentesRenales"].ToString(),
                                        AntecedentesCardioVasculares = Lectura_Hora["AntecedentesCardioVasculares"].ToString(),
                                        Embarazo = Lectura_Hora["Embarazo"].ToString(),
                                        ImplantesMetalicos = Lectura_Hora["ImplantesMetalicos"].ToString(),
                                        MenorEdad = Lectura_Hora["MenorEdad"].ToString(),
                                        Presion = Lectura_Hora["Presion"].ToString(),
                                        Peso = Lectura_Hora["Peso"].ToString(),
                                        GlassHow = Lectura_Hora["GlassHow"].ToString(),
                                        Talla = Lectura_Hora["Talla"].ToString(),
                                        FResp = Lectura_Hora["FResp"].ToString(),
                                        FCar = Lectura_Hora["FCar"].ToString(),
                                        RH = Lectura_Hora["RH"].ToString(),
                                        Conciencia = Lectura_Hora["Conciencia"].ToString(),
                                        IMC = Lectura_Hora["IMC"].ToString(),
                                        ObservacionExaMedico = Lectura_Hora["ObservacionExaMedico"].ToString(),
                                        TipoEstudio = Lectura_Hora["TipoEstudio"].ToString(),
                                        MedioContraste = Lectura_Hora["MedioContraste"].ToString(),
                                        ReaccionAdversa = Lectura_Hora["ReaccionAdversa"].ToString(),
                                        Tecnica = Lectura_Hora["Tecnica"].ToString(),
                                        Hallazgos = Lectura_Hora["Hallazgos"].ToString(),
                                        DX1 = Lectura_Hora["DX1"].ToString() + " - " + DX1,
                                        DX2 = Lectura_Hora["DX2"].ToString() + " - " + DX2,
                                        DX3 = Lectura_Hora["DX3"].ToString() + " - " + DX3,
                                        NotaDX1 = Lectura_Hora["NotaDX1"].ToString(),
                                        NotaDX2 = Lectura_Hora["NotaDX2"].ToString(),
                                        NotaDX3 = Lectura_Hora["NotaDX3"].ToString(),
                                        CausaExterna = Lectura_Hora["CausaExterna"].ToString(),
                                        ImpDX1 = Lectura_Hora["ImpDX1"].ToString(),
                                        ImpDX2 = Lectura_Hora["ImpDX2"].ToString(),
                                        ImpDX3 = Lectura_Hora["ImpDX3"].ToString(),
                                        EstudioComplementario = Lectura_Hora["EstudioComplementario"].ToString(),
                                        ControlSeguimiento = Lectura_Hora["ControlSeguimiento"].ToString(),
                                        HCAdm = Convert.ToInt32(Lectura_Hora["HCAdm"]),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        NotaAclaratoria = Lectura_Hora["NotaAclaratoria"].ToString()
                                    });
                                }
                                return sub_Class_HCFI;
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
        List<ReportHCFI> IReportes.ReportefisiatriaCompleto(int PacienteCom, DateTime Desde, DateTime Hasta, string user)
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

                    String Cargar_Hora = "SELECT Com_Logo, Com_Direccion, Com_Telefono, Com_Nombre, Com_Identificacion, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_IdNum, " +
                                         " Pac_Direccion, Ase_Descripcion, HC_FechaNto, Pac_Sexo, Pac_Email, Pac_Telefono, HC_Fecha, HC_MotCons, HC_EnfAct, " +
                                         " HC_Neurologico, HC_Mental, HC_Edad, HC_OrgSent, HC_Respiratorio, HC_Cardiovascular, HC_GastroI, " +
                                         " HC_GenitoU, HC_Hematolin, HC_Ant, HC_Presart, HC_Peso, HC_PerimetroC, HC_EstCons, HC_FreCar, HC_FrecResp, HC_Talla, " +
                                         " HC_PerimetroA, HC_Adm, HC_Glasshow, HC_IMC, HC_Temp, HC_Embriaguez, HC_ObservaFis, HC_ObservaNeu, HC_Cabeza, HC_Cuello, " +
                                         " HC_Torax, HC_Pulmonar, HC_Cardiovascular, HC_Abdomen, HC_Ombligo, HC_Ano, HC_Extremidades, HC_PielFan, HC_Orl, " +
                                         " HC_OsteoM, HC_Genitales, HC_GenitoU, HC_Hematolin, Bod_Firma, HC_Egreso, HC_Analisis, HC_PManejo, HC_NotaA, HC_DX1, " +
                                         " HC_DX2, HC_DX3, HC_RH, HC_EAV, HC_Epidemia  " +
                                         " FROM CXN_HCFI " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCFI.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCFI.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCFI.HC_Cia = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCFI.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE CXN_HCFI.HC_Pacid = '" + PacienteCom + "' " +
                                         " AND CXN_HCFI.HC_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "' ";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<ReportHCFI> sub_Class_HCFI = new List<ReportHCFI>();
                        DateTime Hoy = DateTime.Now.Date;

                        while (Lectura_Hora.Read() == true)
                        {
                            var DX1 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX1"].ToString());
                            var DX2 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX2"].ToString());
                            var DX3 = repoCIE10.BuscaDX(Lectura_Hora["HC_DX3"].ToString());

                            string Datos = "PACIENTE: " + Lectura_Hora["HC_Pac"].ToString() + "\n\r" +
                                           "FECHA REPORTE: " + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "\n\r" +
                                           "IDENTIFICACION: " + Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString() + "\n\r" +
                                           "CLINICA: " + Lectura_Hora["Com_Nombre"].ToString() + "\n\r" +
                                           "USUARIO QUE GENERA COPIA: " + user;
                            var ImaRes = repoGen.CodifyQR(Datos);

                            sub_Class_HCFI.Add(new ReportHCFI
                            {
                                Admision = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                                Logo = repoGen.GetBytes(ImaRes),
                                EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(), //
                                EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(), //
                                RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),//
                                PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                PacienteNombre = Lectura_Hora["HC_Pac"].ToString(), //
                                FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()),
                                FNto = Convert.ToDateTime(Lectura_Hora["HC_FechaNto"].ToString()),
                                PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(), //
                                Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                Diagnostico1 = Lectura_Hora["HC_DX1"].ToString() + " - " + DX1.ToString(), //
                                Diagnostico_Rel2 = Lectura_Hora["HC_DX2"].ToString() + " - " + DX2.ToString(), //
                                Diagnostico_Rel3 = Lectura_Hora["HC_DX3"].ToString() + " - " + DX3.ToString(), //
                                HC_RH = Lectura_Hora["HC_RH"].ToString(),
                                HC_EAV = Lectura_Hora["HC_EAV"].ToString(),
                                HC_Gastrointestinal = Lectura_Hora["HC_GastroI"].ToString(),
                                HC_Ant = Lectura_Hora["HC_Ant"].ToString(),
                                HC_Neurologico = Lectura_Hora["HC_Neurologico"].ToString(),
                                HC_Mental = Lectura_Hora["HC_Mental"].ToString(),
                                HC_OrgSent = Lectura_Hora["HC_OrgSent"].ToString(),
                                HC_Respiratorio = Lectura_Hora["HC_Respiratorio"].ToString(),
                                HC_Cardiovascular = Lectura_Hora["HC_Cardiovascular"].ToString(),
                                HC_Presart = Lectura_Hora["HC_Presart"].ToString(),
                                HC_Peso = Lectura_Hora["HC_Peso"].ToString(),
                                HC_PerimetroC = Lectura_Hora["HC_PerimetroC"].ToString(),
                                HC_Estado = Lectura_Hora["HC_EstCons"].ToString(),
                                HC_FreRes = Lectura_Hora["HC_FrecResp"].ToString(),
                                HC_IMC = Lectura_Hora["HC_IMC"].ToString(),
                                HC_Glasshow = Lectura_Hora["HC_Glasshow"].ToString(),
                                HC_Temp = Lectura_Hora["HC_Temp"].ToString(),
                                HC_Embriaguez = Lectura_Hora["HC_Embriaguez"].ToString(),
                                HC_ObservaFis = Lectura_Hora["HC_ObservaFis"].ToString(),
                                HC_ObservaNeu = Lectura_Hora["HC_ObservaNeu"].ToString(),
                                HC_Cabeza = Lectura_Hora["HC_Cabeza"].ToString(),
                                HC_Cuello = Lectura_Hora["HC_Cuello"].ToString(),
                                HC_Torax = Lectura_Hora["HC_Torax"].ToString(),
                                HC_Pulmonar = Lectura_Hora["HC_Pulmonar"].ToString(),
                                HC_Abdomen = Lectura_Hora["HC_Abdomen"].ToString(),
                                HC_Ombligo = Lectura_Hora["HC_Ombligo"].ToString(),
                                HC_Ano = Lectura_Hora["HC_Ano"].ToString(),
                                HC_Extremidades = Lectura_Hora["HC_Extremidades"].ToString(),
                                HC_Piel = Lectura_Hora["HC_PielFan"].ToString(),
                                HC_Orl = Lectura_Hora["HC_Orl"].ToString(),
                                HC_Osteomuscular = Lectura_Hora["HC_OsteoM"].ToString(),
                                HC_Genitales = Lectura_Hora["HC_Genitales"].ToString(),
                                HC_Gastrourinario = Lectura_Hora["HC_GenitoU"].ToString(),
                                HC_Hematolin = Lectura_Hora["HC_Hematolin"].ToString(),
                                HC_Egreso = Lectura_Hora["HC_Egreso"].ToString(),
                                HC_Analisis = Lectura_Hora["HC_Analisis"].ToString(),
                                HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                NotaAclaratoria = Lectura_Hora["HC_NotaA"].ToString(), //
                                HC_MotivoC = Lectura_Hora["HC_MotCons"].ToString(), //
                                HC_EnfA = Lectura_Hora["HC_EnfAct"].ToString(), //
                                Edad = Lectura_Hora["HC_Edad"].ToString(),
                                HC_PerimetroA = Lectura_Hora["HC_PerimetroA"].ToString(),
                                HC_FreCar = Lectura_Hora["HC_FreCar"].ToString(),
                                Epidemia = Lectura_Hora["HC_Epidemia"].ToString(),
                                HC_Talla = Lectura_Hora["HC_Talla"].ToString()
                            });
                        }
                        return sub_Class_HCFI;
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
        List<ReportPSI> IReportes.ReportePsicologia(int Admition)
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

                    String Cargar_Hora = "SELECT HC_Adm, HC_Pac, Pac_TipoId, Pac_IdNum, Pac_Sexo, HC_Fecha, HC_Edad, HC_Ocupacion, HC_Estado, Pac_Telefono, " +
                                         " 'ANTECEDENTES MEDICOS Y PERSONALES' AS AMYP, 'QUIRURGICOS' AS QUI, 'FARMACOLOGICO' AS FAR, 'FRACTURAS' AS FRA, 'PATOLOGICOS' AS PAT, " +
                                         " 'TOXICOS Y ALERGICOS' AS TAL, 'FAMILIARES' AS FAM, 'FARMACOLOGICO' AS FAR, 'PSICOLOGICOS' AS PSI, " +
                                         " 'OTROS' AS OTR, 'FACTORES DE RIESGO' AS FDR, 'FACTORES PROTECTORES' AS FP, 'IMPRESION DIAGNOSTICA' AS IDX, " +
                                         " 'PRONOSTICO' AS PRO, 'PLAN DE ACCION' AS PDA, 'RECOMENDACIONES' AS REC, ' ' AS Vacio1, 'DATOS DE INGRESO' AS DDI, 'DIAGNOSTICO MEDICO' AS DXM, 'Escala de Dolor' AS EDD, 'Caracteristica' AS CTA, 'Tiempo de Evolucion' AS TDE, " +
                                         " 'Nombre del Fisiatra' AS NDF, 'Nombre del Psiquiatra' AS NDP, 'Nombre de quien Remite' AS NDQR, " +
                                         " HC_Patologico, HC_Quirurgico, HC_Farmacologico, HC_Fracturas, HC_Tox_Ale, HC_Familiares, HC_Psicologicos, " +
                                         " HC_Otros, HC_Riesgo, HC_FacPro, HC_ImpDiag, HC_Pronostico, HC_Paccion, HC_Recomienda, " +
                                         " Bod_Responsable, Bod_Reg_Med, HC_CIE10, HC_Esc_Dol, HC_TEvolucion, HC_Fisiatra, HC_Psiquiatra, HC_Remite, HC_Caracteristica, " +
                                         " HC_RedApSoc, HC_FSoporte, HC_ObSoporte, HC_OtroSoporte, HC_TFamilia, HC_TRelacion, HC_Sust, HC_Compo, HC_EstadoOtro, HC_TRelacion_2, " +
                                         " HC_TiRelacion, HC_Sust_2, HC_EstSex, HC_ObservSex, HC_AutoEsq, HC_ObseAuto, HC_Suicida, HC_LabEst, HC_ObLav, HC_TiLab, HC_ApGen_1, " +
                                         " HC_ApGen_2, HC_ApGen_3, HC_Cons, HC_ObCons, HC_Aten_1, HC_Aten_2, HC_ObAten, HC_Orien_1, HC_ObOrien, HC_Sue_1, HC_Sue_2, HC_Sue_3, HC_Sue_4, HC_ObSue, HC_Satis, HC_DolMol, " +
                                         " 'EXPLORACION GENERAL ( Areas de Ajuste )' AS EXPOGEN, 'SOCIAL' as SOCIAL, 'Adecuada red de apoyo social' as ARDAS, 'Fuente de Soporte' as FDS, 'Cual' as Cuall, 'Observaciones' as Obs, " +
                                         " 'FAMILIAR' as FAM, 'Tipo de Familia' as TDF, 'Tipo de Relacion' as TDR, 'Sustentado en' as SE, 'Composicion de la Familia' as CDF, " +
                                         " 'AFECTIVO' as AFEC, 'Estado' as ESTA, 'Tiempo de Relacion' as TDRR, 'SEXUAL' as Sexual, 'Nivel de Satisfaccion' as NDS, 'Dolor o Molestia' as DOM, " +
                                         " 'PERSONAL' as Personal, 'Autoesquemas Conservados' as AEC, 'CONDUCTA SUICIDA' as CS, 'LABORAL ACADEMICA' as LA, 'Tiempo' as Times, 'APARIENCIA GENERAL, PORTE Y ACTITUD' AGPYA, " +
                                         " 'Acordes con edad, contexto y estatus socio-cultural' as Acordes, 'Discurso estructurado y coherente' as Discurso, 'Establece contacto visual' as ECV, 'CONSCIENCIA' as Conciense, 'Paciente ingresa por sus propios medios, consciente y alerta' as PACMED, " +
                                         " 'Tipo de alteración si se presenta' as TIPOALT2, 'ATENCION' as Atention, 'Paciente atento y colaborador' as PACATENTO, 'Tipo de alteración si se presenta' as TIPOALT, 'Sigue hilo conductor de entrevista' AS HILO, " +
                                         " 'ORIENTACION' as Orien, 'Orientado en espacio, tiempo, persona' as ORIENESPA, 'Tipo de alteración si se presenta' as TASSP, 'SUEÑO' as dream, 'Patrones de sueño con disrupción' as Patron, 'Cual' as Cual3, 'Apnea de sueño' as APNEA, 'USA CPAP' as UCPAP, 'Medicacion para Dormir' as MEDIDOR " +
                                         " FROM CXN_HCPSI " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCPSI.HC_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCPSI.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCPSI.HC_Cia = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCPSI.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE CXN_HCPSI.HC_Adm = '" + Admition + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<ReportPSI> sub_Class_HCPSI = new List<ReportPSI>();
                        var Diag = repoCIE10.BuscaDX(Lectura_Hora["HC_CIE10"].ToString());

                        sub_Class_HCPSI.Add(new ReportPSI
                        {
                            Admision = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                            PacienteNombre = Lectura_Hora["HC_Pac"].ToString(),
                            PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                            Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                            FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()),
                            Edad = Lectura_Hora["HC_Edad"].ToString(),
                            Ocupacion = Lectura_Hora["HC_Ocupacion"].ToString(),
                            HC_Estado = Lectura_Hora["HC_Estado"].ToString(),
                            PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                            ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                            RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),
                            HC_Patologia = Lectura_Hora["HC_Patologico"].ToString(),
                            HC_AntQui = Lectura_Hora["HC_Quirurgico"].ToString(),
                            HC_AntFarma = Lectura_Hora["HC_Farmacologico"].ToString(),
                            HC_Fracturas = Lectura_Hora["HC_Fracturas"].ToString(),
                            HC_Tox_Ale = Lectura_Hora["HC_Tox_Ale"].ToString(),
                            HC_AntFam = Lectura_Hora["HC_Familiares"].ToString(),
                            HC_Riesgo = Lectura_Hora["HC_Riesgo"].ToString(),
                            HC_Psicologicos = Lectura_Hora["HC_Psicologicos"].ToString(),
                            HC_Otros = Lectura_Hora["HC_Otros"].ToString(),
                            HC_FacPro = Lectura_Hora["HC_FacPro"].ToString(),
                            HC_ImpDiag = Lectura_Hora["HC_ImpDiag"].ToString(),
                            HC_Pronostico = Lectura_Hora["HC_Pronostico"].ToString(),
                            HC_Paccion = Lectura_Hora["HC_Paccion"].ToString(),
                            HC_Recomienda = Lectura_Hora["HC_Recomienda"].ToString(),
                            HC_CIE10 = Lectura_Hora["HC_CIE10"].ToString() + " - " + Diag,
                            HC_Esc_Dol = Lectura_Hora["HC_Esc_Dol"].ToString(),
                            HC_TEvolucion = Lectura_Hora["HC_TEvolucion"].ToString(),
                            HC_Fisiatra = Lectura_Hora["HC_Fisiatra"].ToString(),
                            HC_Psiquiatra = Lectura_Hora["HC_Psiquiatra"].ToString(),
                            HC_Remite = Lectura_Hora["HC_Remite"].ToString(),
                            HC_Caracteristica = Lectura_Hora["HC_Caracteristica"].ToString(),
                            HC_RedApSoc = Lectura_Hora["HC_RedApSoc"].ToString(),
                            HC_FSoporte = Lectura_Hora["HC_FSoporte"].ToString(),
                            HC_ObSoporte = Lectura_Hora["HC_ObSoporte"].ToString(),
                            HC_OtroSoporte = Lectura_Hora["HC_OtroSoporte"].ToString(),
                            HC_TFamilia = Lectura_Hora["HC_TFamilia"].ToString(),
                            HC_TRelacion = Lectura_Hora["HC_TRelacion"].ToString(),
                            HC_EstadoOtro = Lectura_Hora["HC_EstadoOtro"].ToString(),
                            HC_TRelacion_2 = Lectura_Hora["HC_TRelacion_2"].ToString(),
                            HC_TiRelacion = Lectura_Hora["HC_TiRelacion"].ToString(),
                            HC_Compo = Lectura_Hora["HC_Compo"].ToString(),
                            HC_Sust = Lectura_Hora["HC_Sust"].ToString(),
                            HC_ApGen_3 = Lectura_Hora["HC_ApGen_3"].ToString(),
                            HC_Sust_2 = Lectura_Hora["HC_Sust_2"].ToString(),
                            HC_EstSex = Lectura_Hora["HC_EstSex"].ToString(),
                            HC_ObservSex = Lectura_Hora["HC_ObservSex"].ToString(),
                            HC_Satis = Lectura_Hora["HC_Satis"].ToString(),
                            HC_DolMol = Lectura_Hora["HC_DolMol"].ToString(),
                            HC_AutoEsq = Lectura_Hora["HC_AutoEsq"].ToString(),
                            HC_ObseAuto = Lectura_Hora["HC_ObseAuto"].ToString(),
                            HC_Suicida = Lectura_Hora["HC_Suicida"].ToString(),
                            HC_LabEst = Lectura_Hora["HC_LabEst"].ToString(),
                            HC_ObLav = Lectura_Hora["HC_ObLav"].ToString(),
                            HC_TiLab = Lectura_Hora["HC_TiLab"].ToString(),
                            HC_ApGen_1 = Lectura_Hora["HC_ApGen_1"].ToString(),
                            HC_ApGen_2 = Lectura_Hora["HC_ApGen_2"].ToString(),
                            HC_Sue_1 = Lectura_Hora["HC_Sue_1"].ToString(),
                            HC_Cons = Lectura_Hora["HC_Cons"].ToString(),
                            HC_ObCons = Lectura_Hora["HC_ObCons"].ToString(),
                            HC_Aten_1 = Lectura_Hora["HC_Aten_1"].ToString(),
                            HC_ObAten = Lectura_Hora["HC_ObAten"].ToString(),
                            HC_Aten_2 = Lectura_Hora["HC_Aten_2"].ToString(),
                            HC_Orien_1 = Lectura_Hora["HC_Orien_1"].ToString(),
                            HC_ObOrien = Lectura_Hora["HC_ObOrien"].ToString(),
                            HC_ObSue = Lectura_Hora["HC_ObSue"].ToString(),
                            HC_Sue_2 = Lectura_Hora["HC_Sue_2"].ToString(),
                            HC_Sue_3 = Lectura_Hora["HC_Sue_3"].ToString(),
                            HC_Sue_4 = Lectura_Hora["HC_Sue_4"].ToString()
                        });
                        return sub_Class_HCPSI;
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
        List<ReportEVO> IReportes.ReporteEvoluciones(int Paciente,
                                                    DateTime Desde,
                                                    DateTime Hasta,
                                                    string Tipo_Evo_Rep)
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
                    String Cargar_Hora = "SELECT Evo_Hora, Evo_Fecha, Pac_PrimerA, Pac_SegundoA, Pac_PrimerN, Pac_SegundoN, Pac_TipoId, " +
                                         "Pac_IdNum, Evo_Edad, Evo_Sesion, Bod_Responsable, Bod_Reg_Med, Evo_Evo, Evo_Fase, Com_Logo, Com_Nombre, Evo_Adm " +
                                         "FROM CXN_EVOFIB " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_EVOFIB.Evo_Pac = CXN_PACIENTES.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_EVOFIB.Evo_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_EVOFIB.Evo_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_EVOFIB.Evo_Med = CXN_BODEGAS.Bod_Usuario " +
                                         "WHERE Pac_Id = '" + Paciente + "'  " +
                                         "AND Evo_Fecha BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(getData["Format_Fecha"]) + "' " +
                                         "AND Evo_Tipo = '" + Tipo_Evo_Rep + "' " +
                                         "ORDER BY Evo_Fecha, Evo_Sesion ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<ReportEVO> export_om_evoluciones = new List<ReportEVO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            string LogoCia = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                            Byte[] bytes = Convert.FromBase64String(LogoCia); //convierte a bytes
                            MemoryStream stmBLOBData = new MemoryStream(bytes);
                            PictureBox pic = new PictureBox();
                            pic.Image = Image.FromStream(stmBLOBData);

                            if (Lectura_Hora["Evo_Adm"] != DBNull.Value)
                            {
                                export_om_evoluciones.Add(new ReportEVO
                                {
                                    Admision = Convert.ToInt32(Lectura_Hora["Evo_Adm"]),
                                    Evo_Hora = Lectura_Hora["Evo_Hora"].ToString(),
                                    FechaBase = Convert.ToDateTime(Lectura_Hora["Evo_Fecha"]),
                                    PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                    PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                    Edad = Lectura_Hora["Evo_Edad"].ToString(),
                                    Evo_Sesion = Lectura_Hora["Evo_Sesion"].ToString(),
                                    ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                                    RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),
                                    Evo_Evo = Lectura_Hora["Evo_Evo"].ToString(),
                                    Evo_Fase = Lectura_Hora["Evo_Fase"].ToString(),
                                    Logo = repoGen.GetBytes(pic.Image),
                                    EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                    Evo_Texto = "FIBROMIALGIA - " + Tipo_Evo_Rep
                                });
                            }                           
                        }

                        return export_om_evoluciones;
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
        List<ReportEVO> IReportes.ReporteEvoluciones(int Paciente,
                                                     int Admision,
                                                     string Tipo_Evo_Rep)
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
                    String Cargar_Hora = "SELECT Evo_Hora, Evo_Fecha, Pac_PrimerA, Pac_SegundoA, Pac_PrimerN, Pac_SegundoN, Pac_TipoId, " +
                                         "Pac_IdNum, Evo_Edad, Evo_Sesion, Bod_Responsable, Bod_Reg_Med, Evo_Evo, Evo_Fase, Com_Logo, Com_Nombre, Evo_Adm " +
                                         "FROM CXN_EVOFIB " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_EVOFIB.Evo_Pac = CXN_PACIENTES.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_EVOFIB.Evo_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_EVOFIB.Evo_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_EVOFIB.Evo_Med = CXN_BODEGAS.Bod_Usuario " +
                                         "WHERE Pac_Id = '" + Paciente + "' " +
                                         "AND Evo_Adm = '" + Admision + "' " +
                                         "AND Evo_Tipo = '" + Tipo_Evo_Rep + "' " +
                                         "ORDER BY Evo_Fecha, Evo_Sesion ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<ReportEVO> export_om_evoluciones = new List<ReportEVO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            string LogoCia = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                            Byte[] bytes = Convert.FromBase64String(LogoCia); //convierte a bytes
                            MemoryStream stmBLOBData = new MemoryStream(bytes);
                            PictureBox pic = new PictureBox();
                            pic.Image = Image.FromStream(stmBLOBData);

                            export_om_evoluciones.Add(new ReportEVO
                            {
                                Admision = Convert.ToInt32(Lectura_Hora["Evo_Adm"]),
                                Evo_Hora = Lectura_Hora["Evo_Hora"].ToString(),
                                FechaBase = Convert.ToDateTime(Lectura_Hora["Evo_Fecha"]),
                                PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                Edad = Lectura_Hora["Evo_Edad"].ToString(),
                                Evo_Sesion = Lectura_Hora["Evo_Sesion"].ToString(),
                                ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                                RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),
                                Evo_Evo = Lectura_Hora["Evo_Evo"].ToString(),
                                Evo_Fase = Lectura_Hora["Evo_Fase"].ToString(),
                                Logo = repoGen.GetBytes(pic.Image),
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                Evo_Texto = "FIBROMIALGIA - " + Tipo_Evo_Rep
                            });
                        }

                        return export_om_evoluciones;
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

        List<ReportJuntas> IReportes.ReporteJuntas(int Admition, string Tipo)
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
                    String Cargar_Hora = "SELECT P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_TipoId, P.Pac_IdNum, " +
                                         "J.Jun_Edad, J.Jun_Fecha, J.Jun_Fisiatra, J.Jun_TF, J.Jun_TO, J.Jun_Psicologo, J.Jun_DX_FI, J.Jun_DX_TF, " +
                                         "J.Jun_DX_TO, J.Jun_DX_PS, J.Jun_Pro_FI, J.Jun_Pro_TF, J.Jun_Pro_TO, J.Jun_Pro_PS, J.Jun_Con_FI, " +
                                         "J.Jun_Con_TF, J.Jun_Con_TO, J.Jun_Con_PS, J.Jun_Observa, J.Jun_Prox_Cita, J.Jun_Adm, J.Jun_Firma, J.Jun_MedFirma, " +
                                         "J.Jun_Observa_TO, J.Jun_Observa_PS, J.Jun_Bodega, " +
                                         "C.Com_Nombre, C.Com_Identificacion, C.Com_Telefono, C.Com_Direccion, C.Com_Logo " +
                                         "FROM CXN_HCJUNTAS J " +
                                         "INNER JOIN CXN_PACIENTES P ON J.Jun_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON J.Jun_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON J.Jun_Cia = C.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS B ON J.Jun_Bodega = B.Bod_Numero " +
                                         "WHERE J.Jun_Adm = '" + Admition + "' " +
                                         "AND J.Jun_Tipo = '" + Tipo + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        string Bod_Firma1 = null;

                        if (Lectura_Hora["Jun_Firma"].ToString() == "SI")
                        {
                            var firma = repoBodegas.getDatosUser(Lectura_Hora["Jun_MedFirma"].ToString());
                            Bod_Firma1 = firma.Bod_Firma;
                        }
                        else
                        {
                            Bod_Firma1 = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBw" +
                                        "YHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAw" +
                                        "MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAAZABgDASIAAhEB" +
                                        "AxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQID" +
                                        "AAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdIS" +
                                        "UpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usL" +
                                        "DxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAE" +
                                        "CAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMz" +
                                        "UvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6" +
                                        "goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn" +
                                        "6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKACiiigAooooAKKKKAP/2Q==";
                        }

                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                        MemoryStream stmBLOBData = new MemoryStream(bytes);
                        PictureBox pic = new PictureBox();
                        pic.Image = Image.FromStream(stmBLOBData);

                        string LogoCia = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                        Byte[] bytes2 = Convert.FromBase64String(LogoCia); //convierte a bytes
                        MemoryStream stmBLOBData2 = new MemoryStream(bytes2);
                        PictureBox pic2 = new PictureBox();
                        pic2.Image = Image.FromStream(stmBLOBData2);

                        List<ReportJuntas> sub_Class_HCJUNTAS = new List<ReportJuntas>();
                        sub_Class_HCJUNTAS.Add(new ReportJuntas
                        {
                            PacienteNombre = Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() + " " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString(),
                            PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                            Edad = Lectura_Hora["Jun_Edad"].ToString(),
                            FechaBase = Convert.ToDateTime(Lectura_Hora["Jun_Fecha"].ToString()),
                            Jun_Fisiatra = Lectura_Hora["Jun_Fisiatra"].ToString(),
                            Jun_TF = Lectura_Hora["Jun_TF"].ToString(),
                            Jun_TO = Lectura_Hora["Jun_TO"].ToString(),
                            Jun_Psicologo = Lectura_Hora["Jun_Psicologo"].ToString(),
                            Jun_DX_FI = Lectura_Hora["Jun_DX_FI"].ToString(),
                            Jun_DX_TF = Lectura_Hora["Jun_DX_TF"].ToString(),
                            Jun_DX_TO = Lectura_Hora["Jun_DX_TO"].ToString(),
                            Jun_DX_PS = Lectura_Hora["Jun_DX_PS"].ToString(),
                            Jun_Pro_FI = Lectura_Hora["Jun_Pro_FI"].ToString(),
                            Jun_Pro_TF = Lectura_Hora["Jun_Pro_TF"].ToString(),
                            Jun_Pro_TO = Lectura_Hora["Jun_Pro_TO"].ToString(),
                            Jun_Pro_PS = Lectura_Hora["Jun_Pro_PS"].ToString(),
                            Jun_Con_FI = Lectura_Hora["Jun_Con_FI"].ToString(),
                            Jun_Con_TF = Lectura_Hora["Jun_Con_TF"].ToString(),
                            Jun_Con_TO = Lectura_Hora["Jun_Con_TO"].ToString(),
                            Jun_Con_PS = Lectura_Hora["Jun_Con_PS"].ToString(),
                            Jun_Observa = Lectura_Hora["Jun_Observa"].ToString(),
                            Jun_Prox_Cita = Convert.ToDateTime(Lectura_Hora["Jun_Prox_Cita"]),
                            Firma = repoGen.GetBytes(pic.Image),
                            Admision = Convert.ToInt32(Lectura_Hora["Jun_Adm"]),
                            Jun_Observa_TO = Lectura_Hora["Jun_Observa_TO"].ToString(),
                            Jun_Observa_PS = Lectura_Hora["Jun_Observa_PS"].ToString(),
                            Logo = repoGen.GetBytes(pic2.Image),
                            EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                            EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                            EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                            EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString()
                        });
                        return sub_Class_HCJUNTAS;
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
        List<ReportTF> IReportes.ReporteTerapiaFisica(int Admition)
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

                    String Cargar_Hora = "SELECT '    ' AS Vacio1, 'ANTECEDENTES' AS Antecedentes, 'PATOLOGICOS' AS Patologicos, 'NEUROLOGICO' AS Neurologico, 'SALUD MENTAL' AS SaludMental, 'OSTEOMUSCULAR' AS OsteoMuscular, 'ENDOCRINOLOGICO' AS Endocrinologico, HC_Text60, HC_Fecha, " +
                                         " 'QUIRURGICO' AS Quirurgico, 'RESPIRATORIO' AS Respiratorio, 'DERMATOLOGICO' AS Dermatologico, 'FARMACOLOGICO' AS Farmacologico, 'FAMILIAR' AS Familiar, HC_AntPat, HC_AntNeu, HC_SalMen, HC_OsteoMusc, HC_Endocrino, HC_Quir, " +
                                         " HC_CIE10, HC_Resp, HC_Derma, HC_Farma, HC_AntFam, 'MOTIVO DE CONSULTA' AS MotivoC, 'EXAMEN FISICO' AS ExaFis, HC_MotivoCons, HC_Pac, HC_Ocupacion, Pac_FechaNto, HC_Hijos, HC_Edad, Pac_Sexo, " +
                                         " Pac_TipoId, Pac_IdNum, Pac_Direccion, HC_EstadoC, Pac_Telefono, HC_Estudios, HC_TiempoE, HC_ExaDiag, Pac_Email, HC_Dx, HC_TratamientosP, 'Dolor' AS Dolor, 'EVA' AS Eva, 'Caracteristicas del Dolor' AS CaracDol, 'EVALUACION POSTURAL' AS Eva_Pos, " +
                                         " 'Sensibilidad' AS Sensibilidad, 'Sintomas' AS Sintomas, 'Habitos Toxicos' AS HabTox, 'Actividad Fisica' AS ActFis, 'Medios Externos' AS MedExt, 'Frecuencia Dolor' AS FrecDol, 'Tipo Dolor' AS TDolor, HC_DolorEIAN, " +
                                         " HC_EVA, HC_CaracDol, HC_SensiDol, HC_SintAso, HC_HabTox, HC_Cual1, HC_Cual2, HC_FrecDol, HC_TipoDol, 'MARCHA: ' AS Marcha, 'Cabeza' AS Cabeza, 'Hombros' AS Hombros, 'Brazos' AS Brazos, 'Cadera' AS Cadera, " +
                                         " 'Rodilla' AS Rodilla, 'Rotula' AS Rotula, 'Tibia' AS Tibia, 'Tobillo' AS Tobillo, 'Pie' AS Pie, HisCabeza, HisHombros, HisBrazos, HisCadera, HisRodilla, " +
                                         " HisRotula, HisTibia, HisTobillo, HisPie, 'PRUEBAS ESPECIFICAS' AS PrueEspe, 'DIAGNOSTICO' AS Diagnostico, 'PRONOSTICO' AS Pronostico, 'ACCIONES' AS Acciones, 'CONDUCTA' AS Conducta, HisDiagnostico, " +
                                         " HisPronostico, HisAcciones, HisConducta, Bod_Responsable, Bod_Reg_Med, Com_Nombre, Com_Identificacion, Com_Telefono, Marcha_Mec, " +
                                         " HisCuello, HisEspaldaA, HisTorax, HisAbdomen, HisEspaldaB, HisPelvis, HisEscapula, HisColumna, HisCrestasIliacas, HisGluteos, HisFosa, HC_CC1, HC_CC2, HC_CC3, HC_CC4, HC_CC5, HC_CC6, HC_CD1, HC_CD2, HC_CD3, HC_CD4, HC_H1, HC_H2, HC_H3, HC_H4, HC_H5, HC_H6, HC_C1, HC_C2, HC_C3, HC_C4, " +
                                         " HC_C5, HC_C6, Hc_T1, Hc_T2, Hc_T3, Hc_T4, Hc_T5, Hc_T6, Hc_T7, Hc_T8, Hc_T9, Hc_T10, Hc_T11, Hc_T12, Hc_T13, Hc_T14, HisExtCui, HisExtCuD, HisEscalenosI, HisEscalenosD, HisTrapSupI, HisTrapSupD, HisTrapMedI, HisTrapMedD, HisTrapInfI, HisTrapInfD, HisSerAntI, HisSerAntD, HisPecMayI, " +
                                         " HisPecMayD, HisRomI, HisRomD, HisAbSupI, HisAbSupD, HisAbdInfI, HisAbdInfD, HisOblicuoI, HisOblicuoD, HisExtDorsalI, HisExtDorsalD, HisExtLumbarI, HisExtLumbarD, HisGluMayI, HisGluMayD, HisGluMedI, HisGluMedD, HisCuadriI, HisCuadriD, HisIsquiI, HisIsquiD, HisGemeloI, HisGemeloD, HisTibAntI, " +
                                         " HisTibAntD, HisEscaAntI, HisEscaAntD, HisEscaMedI, HisEscaMedD, HisEscaPosI, HisEscaPosD, HisECMI, HisECMD, HisCualLumI, HisCualLumD, HisCuadI, HisCuadD, HisIsquiI2, HisIsqui2D, HisTensorI, HisTensorD, HisGastroI, HisGastroD, HisTAquilesI, HisTAquilesD, 'Vista Anterior' as VISTANT, " +
                                         " 'Vista Lateral' as VistaL, 'Vista Posterior' as VistaP, 'Cuello' as Cuello, 'Espalda Alta' as EspaAl, 'Torax' as Torax, 'Abdomen' as Abdomen, 'Espalda Baja' as EspaBa, 'Pelvis' as Pelvis, 'Escapula' as Escapula, 'Columna' as Columna, 'Crestas Iliacas' as Cresta, 'Gluteos' as Gluteos, 'Fosa Poplitea' as Fosa " +
                                         " FROM CXN_HCTF INNER JOIN  CXN_PACIENTES ON CXN_HCTF.HC_PacId = CXN_PACIENTES.Pac_Id INNER JOIN  CXN_ASEGURADORA ON CXN_HCTF.HC_Ase = CXN_ASEGURADORA.Ase_Identificador INNER JOIN CXN_CIA ON CXN_HCTF.HC_Cia = CXN_CIA.Com_Identificador INNER JOIN CXN_BODEGAS ON CXN_HCTF.HC_Prof = CXN_BODEGAS.Bod_Numero WHERE CXN_HCTF.HC_Adm = '" + Admition + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<ReportTF> sub_Class_HCTF = new List<ReportTF>();

                        sub_Class_HCTF.Add(new ReportTF
                        {
                            PacienteNombre = Lectura_Hora["HC_Pac"].ToString(),
                            Ocupacion = Lectura_Hora["HC_Ocupacion"].ToString(),
                            FNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"].ToString()),
                            FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"].ToString()),
                            HC_Hijos = Lectura_Hora["HC_Hijos"].ToString(),
                            Edad = Lectura_Hora["HC_Edad"].ToString(),
                            Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                            PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString() + " - " + Admition,
                            PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                            HC_EstadoC = Lectura_Hora["HC_EstadoC"].ToString(),
                            PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),
                            HC_Estudios = Lectura_Hora["HC_Estudios"].ToString(),
                            HC_TiempoE = Lectura_Hora["HC_TiempoE"].ToString(),
                            HC_ExaDiag = Lectura_Hora["HC_ExaDiag"].ToString(),
                            Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                            Diagnostico1 = Lectura_Hora["HC_CIE10"].ToString() + " - " + Lectura_Hora["HC_Dx"].ToString(),
                            HC_TratamientosP = Lectura_Hora["HC_TratamientosP"].ToString(),
                            HC_MotivoC = Lectura_Hora["HC_MotivoCons"].ToString(),
                            HC_DolorEIAN = Lectura_Hora["HC_DolorEIAN"].ToString(),
                            HC_EVA = Lectura_Hora["HC_EVA"].ToString(),
                            HC_CaracDol = Lectura_Hora["HC_CaracDol"].ToString(),
                            HC_Text60 = Lectura_Hora["HC_Text60"].ToString(),
                            HC_SensiDol = Lectura_Hora["HC_SensiDol"].ToString(),
                            HC_SintAso = Lectura_Hora["HC_SintAso"].ToString(),
                            HC_HabTox = Lectura_Hora["HC_HabTox"].ToString(),
                            HC_Cual1 = Lectura_Hora["HC_Cual1"].ToString(),
                            HC_FrecDol = Lectura_Hora["HC_FrecDol"].ToString(),
                            HC_TipoDol = Lectura_Hora["HC_TipoDol"].ToString(),
                            HC_Cual2 = Lectura_Hora["HC_Cual2"].ToString(),
                            HC_AntPat = Lectura_Hora["HC_AntPat"].ToString(),
                            HC_AntNeu = Lectura_Hora["HC_AntNeu"].ToString(),
                            HC_SalMen = Lectura_Hora["HC_SalMen"].ToString(),
                            HC_OsteoMusc = Lectura_Hora["HC_OsteoMusc"].ToString(),
                            HC_Endocrino = Lectura_Hora["HC_Endocrino"].ToString(),
                            HC_Quir = Lectura_Hora["HC_Quir"].ToString(),
                            HC_Resp = Lectura_Hora["HC_Resp"].ToString(),
                            HC_Derma = Lectura_Hora["HC_Derma"].ToString(),
                            HC_Farma = Lectura_Hora["HC_Farma"].ToString(),
                            HC_AntFam = Lectura_Hora["HC_AntFam"].ToString(),
                            HisCabeza = Lectura_Hora["HisCabeza"].ToString(),
                            HisHombros = Lectura_Hora["HisHombros"].ToString(),
                            HisBrazos = Lectura_Hora["HisBrazos"].ToString(),
                            HisCadera = Lectura_Hora["HisCadera"].ToString(),
                            HisRodilla = Lectura_Hora["HisRodilla"].ToString(),
                            HisRotula = Lectura_Hora["HisRotula"].ToString(),
                            HisTibia = Lectura_Hora["HisTibia"].ToString(),
                            HisTobillo = Lectura_Hora["HisTobillo"].ToString(),
                            HisPie = Lectura_Hora["HisPie"].ToString(),
                            HisDiagnostico = Lectura_Hora["HisDiagnostico"].ToString(),
                            HisPronostico = Lectura_Hora["HisPronostico"].ToString(),
                            HisAcciones = Lectura_Hora["HisAcciones"].ToString(),
                            HisConducta = Lectura_Hora["HisConducta"].ToString(),
                            Marcha_Mec = Lectura_Hora["Marcha_Mec"].ToString(),
                            HisCuello = Lectura_Hora["HisCuello"].ToString(),
                            HisEspaldaA = Lectura_Hora["HisEspaldaA"].ToString(),
                            HisTorax = Lectura_Hora["HisTorax"].ToString(),
                            HisAbdomen = Lectura_Hora["HisAbdomen"].ToString(),
                            HisEspaldaB = Lectura_Hora["HisEspaldaB"].ToString(),
                            HisPelvis = Lectura_Hora["HisPelvis"].ToString(),
                            HisEscapula = Lectura_Hora["HisEscapula"].ToString(),
                            HisColumna = Lectura_Hora["HisColumna"].ToString(),
                            HisCrestasIliacas = Lectura_Hora["HisCrestasIliacas"].ToString(),
                            HisGluteos = Lectura_Hora["HisGluteos"].ToString(),
                            HisFosa = Lectura_Hora["HisFosa"].ToString(),
                            HC_CC1 = Lectura_Hora["HC_CC1"].ToString(),
                            HC_CC2 = Lectura_Hora["HC_CC2"].ToString(),
                            HC_CC3 = Lectura_Hora["HC_CC3"].ToString(),
                            HC_CC4 = Lectura_Hora["HC_CC4"].ToString(),
                            HC_CC5 = Lectura_Hora["HC_CC5"].ToString(),
                            HC_CC6 = Lectura_Hora["HC_CC6"].ToString(),
                            HC_CD1 = Lectura_Hora["HC_CD1"].ToString(),
                            HC_CD2 = Lectura_Hora["HC_CD2"].ToString(),
                            HC_CD3 = Lectura_Hora["HC_CD3"].ToString(),
                            HC_CD4 = Lectura_Hora["HC_CD4"].ToString(),
                            HC_H1 = Lectura_Hora["HC_H1"].ToString(),
                            HC_H2 = Lectura_Hora["HC_H2"].ToString(),
                            HC_H3 = Lectura_Hora["HC_H3"].ToString(),
                            HC_H4 = Lectura_Hora["HC_H4"].ToString(),
                            HC_H5 = Lectura_Hora["HC_H5"].ToString(),
                            HC_H6 = Lectura_Hora["HC_H6"].ToString(),
                            HC_C1 = Lectura_Hora["HC_C1"].ToString(),
                            HC_C2 = Lectura_Hora["HC_C2"].ToString(),
                            HC_C3 = Lectura_Hora["HC_C3"].ToString(),
                            HC_C4 = Lectura_Hora["HC_C4"].ToString(),
                            HC_C5 = Lectura_Hora["HC_C5"].ToString(),
                            HC_C6 = Lectura_Hora["HC_C6"].ToString(),
                            Hc_T1 = Lectura_Hora["Hc_T1"].ToString(),
                            Hc_T2 = Lectura_Hora["Hc_T2"].ToString(),
                            Hc_T3 = Lectura_Hora["Hc_T3"].ToString(),
                            Hc_T4 = Lectura_Hora["Hc_T4"].ToString(),
                            Hc_T5 = Lectura_Hora["Hc_T5"].ToString(),
                            Hc_T6 = Lectura_Hora["Hc_T6"].ToString(),
                            Hc_T7 = Lectura_Hora["Hc_T7"].ToString(),
                            Hc_T8 = Lectura_Hora["Hc_T8"].ToString(),
                            Hc_T9 = Lectura_Hora["Hc_T9"].ToString(),
                            Hc_T10 = Lectura_Hora["Hc_T10"].ToString(),
                            Hc_T11 = Lectura_Hora["Hc_T11"].ToString(),
                            Hc_T12 = Lectura_Hora["Hc_T12"].ToString(),
                            Hc_T13 = Lectura_Hora["Hc_T13"].ToString(),
                            Hc_T14 = Lectura_Hora["Hc_T14"].ToString(),
                            HisExtCui = Lectura_Hora["HisExtCui"].ToString(),
                            HisExtCuD = Lectura_Hora["HisExtCuD"].ToString(),
                            HisEscalenosI = Lectura_Hora["HisEscalenosI"].ToString(),
                            HisEscalenosD = Lectura_Hora["HisEscalenosD"].ToString(),
                            HisTrapSupI = Lectura_Hora["HisTrapSupI"].ToString(),
                            HisTrapSupD = Lectura_Hora["HisTrapSupD"].ToString(),
                            HisTrapMedI = Lectura_Hora["HisTrapMedI"].ToString(),
                            HisTrapMedD = Lectura_Hora["HisTrapMedD"].ToString(),
                            HisTrapInfI = Lectura_Hora["HisTrapInfI"].ToString(),
                            HisTrapInfD = Lectura_Hora["HisTrapInfD"].ToString(),
                            HisSerAntI = Lectura_Hora["HisSerAntI"].ToString(),
                            HisSerAntD = Lectura_Hora["HisSerAntD"].ToString(),
                            HisPecMayI = Lectura_Hora["HisPecMayI"].ToString(),
                            HisPecMayD = Lectura_Hora["HisPecMayD"].ToString(),
                            HisRomI = Lectura_Hora["HisRomI"].ToString(),
                            HisRomD = Lectura_Hora["HisRomD"].ToString(),
                            HisAbSupI = Lectura_Hora["HisAbSupI"].ToString(),
                            HisAbSupD = Lectura_Hora["HisAbSupD"].ToString(),
                            HisAbdInfI = Lectura_Hora["HisAbdInfI"].ToString(),
                            HisAbdInfD = Lectura_Hora["HisAbdInfD"].ToString(),
                            HisOblicuoI = Lectura_Hora["HisOblicuoI"].ToString(),
                            HisOblicuoD = Lectura_Hora["HisOblicuoD"].ToString(),
                            HisExtDorsalI = Lectura_Hora["HisExtDorsalI"].ToString(),
                            HisExtDorsalD = Lectura_Hora["HisExtDorsalD"].ToString(),
                            HisExtLumbarI = Lectura_Hora["HisExtLumbarI"].ToString(),
                            HisExtLumbarD = Lectura_Hora["HisExtLumbarD"].ToString(),
                            HisGluMayI = Lectura_Hora["HisGluMayI"].ToString(),
                            HisGluMayD = Lectura_Hora["HisGluMayD"].ToString(),
                            HisGluMedI = Lectura_Hora["HisGluMedI"].ToString(),
                            HisGluMedD = Lectura_Hora["HisGluMedD"].ToString(),
                            HisCuadriI = Lectura_Hora["HisCuadriI"].ToString(),
                            HisCuadriD = Lectura_Hora["HisCuadriD"].ToString(),
                            HisIsquiI = Lectura_Hora["HisIsquiI"].ToString(),
                            HisIsquiD = Lectura_Hora["HisIsquiD"].ToString(),
                            HisGemeloI = Lectura_Hora["HisGemeloI"].ToString(),
                            HisGemeloD = Lectura_Hora["HisGemeloD"].ToString(),
                            HisTibAntI = Lectura_Hora["HisTibAntI"].ToString(),
                            HisTibAntD = Lectura_Hora["HisTibAntD"].ToString(),
                            HisEscaAntI = Lectura_Hora["HisEscaAntI"].ToString(),
                            HisEscaAntD = Lectura_Hora["HisEscaAntD"].ToString(),
                            HisEscaMedI = Lectura_Hora["HisEscaMedI"].ToString(),
                            HisEscaMedD = Lectura_Hora["HisEscaMedD"].ToString(),
                            HisEscaPosI = Lectura_Hora["HisEscaPosI"].ToString(),
                            HisEscaPosD = Lectura_Hora["HisEscaPosD"].ToString(),
                            HisECMI = Lectura_Hora["HisECMI"].ToString(),
                            HisECMD = Lectura_Hora["HisECMD"].ToString(),
                            HisCualLumI = Lectura_Hora["HisCualLumI"].ToString(),
                            HisCualLumD = Lectura_Hora["HisCualLumD"].ToString(),
                            HisCuadI = Lectura_Hora["HisCuadI"].ToString(),
                            HisCuadD = Lectura_Hora["HisCuadD"].ToString(),
                            HisIsquiI2 = Lectura_Hora["HisIsquiI2"].ToString(),
                            HisIsqui2D = Lectura_Hora["HisIsqui2D"].ToString(),
                            HisTensorI = Lectura_Hora["HisTensorI"].ToString(),
                            HisTensorD = Lectura_Hora["HisTensorD"].ToString(),
                            HisGastroI = Lectura_Hora["HisGastroI"].ToString(),
                            HisGastroD = Lectura_Hora["HisGastroD"].ToString(),
                            HisTAquilesI = Lectura_Hora["HisTAquilesI"].ToString(),
                            HisTAquilesD = Lectura_Hora["HisTAquilesD"].ToString(),
                            ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                            RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),
                            EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                            EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                            EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString()
                        });
                        return sub_Class_HCTF;
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
        List<ReportTO> IReportes.ReporteTerapiaOcupacional(int Admition)
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

                    String Cargar_Hora = "SELECT *, 'DIAGNOSTICO OCUPACIONAL' AS DXO, 'PRONOSTICO OCUPACIONAL' AS POC, 'ACCIONES' AS ACTIONS, ' ' AS Vacio, 'Historia Ocupacional' AS HOCU, 'Antecedentes Laborales' AS ALABO, " +
                                         "'Descripcion del Hogar' as DHOGAR, 'Observaciones de la Funcionalidad de los Miembros Superiores' AS OFMS, 'FUNCIONALIDAD DE LOS MIEMBROS SUPERIORES E INFERIORES' AS FMSEI, " +
                                         "'Patrones' AS PAT, 'MANO - CABEZA' AS MANCA, 'MANO - BOCA' AS MANBO, 'MANO - HOMBRO' AS MANHO, 'MANO - ESPALDA' AS MANES, 'MANO - CINTURA' AS MANCI, 'MANO - PERINE' AS MANPE, " +
                                         "'MANO - RODILLA' AS MANRO, 'MANO - PIE' AS MANPI " +
                                         " FROM CXN_HCTO " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCTO.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCTO.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCTO.HC_Cia = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCTO.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE CXN_HCTO.HC_Adm = '" + Admition + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<ReportTO> sub_Class_HCTO = new List<ReportTO>();

                        sub_Class_HCTO.Add(new ReportTO
                        {
                            EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                            EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                            PacienteNombre = Lectura_Hora["HC_Pac"].ToString(),
                            HC_DiagMedico = Lectura_Hora["HC_DiagMedico"].ToString(),
                            Ocupacion = Lectura_Hora["HC_OcuAct"].ToString(),
                            Edad = Lectura_Hora["HC_Edad"].ToString(),
                            FechaBase = Convert.ToDateTime(Lectura_Hora["HC_Fecha"]),
                            Admision = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                            PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),

                            HC_HisFam = Lectura_Hora["HC_HisFam"].ToString(),

                            MSD_MCP = Lectura_Hora["MSD_MCP"].ToString(),
                            MSD_MCA = Lectura_Hora["MSD_MCA"].ToString(),
                            MSD_MBP = Lectura_Hora["MSD_MBP"].ToString(),
                            MSD_MBA = Lectura_Hora["MSD_MBA"].ToString(),
                            MSD_MHP = Lectura_Hora["MSD_MHP"].ToString(),
                            MSD_MHA = Lectura_Hora["MSD_MHA"].ToString(),
                            MSD_MEP = Lectura_Hora["MSD_MEP"].ToString(),
                            MSD_MEA = Lectura_Hora["MSD_MEA"].ToString(),
                            MSD_MCIP = Lectura_Hora["MSD_MCIP"].ToString(),
                            MSD_MCIA = Lectura_Hora["MSD_MCIA"].ToString(),
                            MSD_MPP = Lectura_Hora["MSD_MPP"].ToString(),
                            MSD_MPA = Lectura_Hora["MSD_MPA"].ToString(),
                            MSD_MRP = Lectura_Hora["MSD_MRP"].ToString(),
                            MSD_MRA = Lectura_Hora["MSD_MRA"].ToString(),
                            MSD_MPIEP = Lectura_Hora["MSD_MPIEP"].ToString(),
                            MSD_MPIEA = Lectura_Hora["MSD_MPIEA"].ToString(),

                            MSI_MCP = Lectura_Hora["MSI_MCP"].ToString(),
                            MSI_MCA = Lectura_Hora["MSI_MCA"].ToString(),
                            MSI_MBP = Lectura_Hora["MSI_MBP"].ToString(),
                            MSI_MBA = Lectura_Hora["MSI_MBA"].ToString(),
                            MSI_MHP = Lectura_Hora["MSI_MHP"].ToString(),
                            MSI_MHA = Lectura_Hora["MSI_MHA"].ToString(),
                            MSI_MEP = Lectura_Hora["MSI_MEP"].ToString(),
                            MSI_MEA = Lectura_Hora["MSI_MEA"].ToString(),
                            MSI_MCIP = Lectura_Hora["MSI_MCIP"].ToString(),
                            MSI_MCIA = Lectura_Hora["MSI_MCIA"].ToString(),
                            MSI_MPP = Lectura_Hora["MSI_MPP"].ToString(),
                            MSI_MPA = Lectura_Hora["MSI_MPA"].ToString(),
                            MSI_MRP = Lectura_Hora["MSI_MRP"].ToString(),
                            MSI_MRA = Lectura_Hora["MSI_MRA"].ToString(),
                            MSI_MPIEP = Lectura_Hora["MSI_MPIEP"].ToString(),
                            MSI_MPIEA = Lectura_Hora["MSI_MPIEA"].ToString(),

                            MSD_AAP = Lectura_Hora["MSD_AAP"].ToString(),
                            MSD_AADP = Lectura_Hora["MSD_AADP"].ToString(),
                            MSD_AATP = Lectura_Hora["MSD_AATP"].ToString(),
                            MSD_AALP = Lectura_Hora["MSD_AALP"].ToString(),
                            MSD_AGAP = Lectura_Hora["MSD_AGAP"].ToString(),
                            MSD_AGACILP = Lectura_Hora["MSD_AGACILP"].ToString(),
                            MSD_AGAESFP = Lectura_Hora["MSD_AGAESFP"].ToString(),
                            MSD_PINFP = Lectura_Hora["MSD_PINFP"].ToString(),
                            MSD_PINTP = Lectura_Hora["MSD_PINTP"].ToString(),
                            MSD_PLATP = Lectura_Hora["MSD_PLATP"].ToString(),

                            MSD_AAA = Lectura_Hora["MSD_AAA"].ToString(),
                            MSD_AADA = Lectura_Hora["MSD_AADA"].ToString(),
                            MSD_AATA = Lectura_Hora["MSD_AATA"].ToString(),
                            MSD_AALA = Lectura_Hora["MSD_AALA"].ToString(),
                            MSD_AGAA = Lectura_Hora["MSD_AGAA"].ToString(),
                            MSD_AGACILA = Lectura_Hora["MSD_AGACILA"].ToString(),
                            MSD_AGAESFA = Lectura_Hora["MSD_AGAESFA"].ToString(),
                            MSD_PINFA = Lectura_Hora["MSD_PINFA"].ToString(),
                            MSD_PINTA = Lectura_Hora["MSD_PINTA"].ToString(),
                            MSD_PLATA = Lectura_Hora["MSD_PLATA"].ToString(),

                            MSI_AAP = Lectura_Hora["MSI_AAP"].ToString(),
                            MSI_AADP = Lectura_Hora["MSI_AADP"].ToString(),
                            MSI_AATP = Lectura_Hora["MSI_AATP"].ToString(),
                            MSI_AALP = Lectura_Hora["MSI_AALP"].ToString(),
                            MSI_AGAP = Lectura_Hora["MSI_AGAP"].ToString(),
                            MSI_AGACILP = Lectura_Hora["MSI_AGACILP"].ToString(),
                            MSI_AGAESFP = Lectura_Hora["MSI_AGAESFP"].ToString(),
                            MSI_PINFP = Lectura_Hora["MSI_PINFP"].ToString(),
                            MSI_PINTP = Lectura_Hora["MSI_PINTP"].ToString(),
                            MSI_PLATP = Lectura_Hora["MSI_PLATP"].ToString(),

                            MSI_AAA = Lectura_Hora["MSI_AAA"].ToString(),
                            MSI_AADA = Lectura_Hora["MSI_AADA"].ToString(),
                            MSI_AATA = Lectura_Hora["MSI_AATA"].ToString(),
                            MSI_AALA = Lectura_Hora["MSI_AALA"].ToString(),
                            MSI_AGAA = Lectura_Hora["MSI_AGAA"].ToString(),
                            MSI_AGACILA = Lectura_Hora["MSI_AGACILA"].ToString(),
                            MSI_AGAESFA = Lectura_Hora["MSI_AGAESFA"].ToString(),
                            MSI_PINFA = Lectura_Hora["MSI_PINFA"].ToString(),
                            MSI_PINTA = Lectura_Hora["MSI_PINTA"].ToString(),
                            MSI_PLATA = Lectura_Hora["MSI_PLATA"].ToString(),

                            HC_A_T1 = Lectura_Hora["HC_A_T1"].ToString(),
                            HC_A_T2 = Lectura_Hora["HC_A_T2"].ToString(),
                            HC_A_T3 = Lectura_Hora["HC_A_T3"].ToString(),
                            HC_A_T4 = Lectura_Hora["HC_A_T4"].ToString(),
                            HC_A_T5 = Lectura_Hora["HC_A_T5"].ToString(),
                            HC_A_T6 = Lectura_Hora["HC_A_T6"].ToString(),
                            HC_A_T7 = Lectura_Hora["HC_A_T7"].ToString(),
                            HC_A_T8 = Lectura_Hora["HC_A_T8"].ToString(),
                            HC_A_T9 = Lectura_Hora["HC_A_T9"].ToString(),
                            HC_A_T10 = Lectura_Hora["HC_A_T10"].ToString(),

                            HC_A_C1 = Lectura_Hora["HC_A_C1"].ToString(),
                            HC_A_C2 = Lectura_Hora["HC_A_C2"].ToString(),
                            HC_A_C3 = Lectura_Hora["HC_A_C3"].ToString(),
                            HC_A_C4 = Lectura_Hora["HC_A_C4"].ToString(),
                            HC_A_C5 = Lectura_Hora["HC_A_C5"].ToString(),
                            HC_A_C6 = Lectura_Hora["HC_A_C6"].ToString(),
                            HC_A_C7 = Lectura_Hora["HC_A_C7"].ToString(),
                            HC_A_C8 = Lectura_Hora["HC_A_C8"].ToString(),
                            HC_A_C9 = Lectura_Hora["HC_A_C9"].ToString(),
                            HC_A_C10 = Lectura_Hora["HC_A_C10"].ToString(),
                            HC_A_C11 = Lectura_Hora["HC_A_C11"].ToString(),
                            HC_A_C12 = Lectura_Hora["HC_A_C12"].ToString(),
                            HC_A_C13 = Lectura_Hora["HC_A_C13"].ToString(),
                            HC_A_C14 = Lectura_Hora["HC_A_C14"].ToString(),
                            HC_A_C15 = Lectura_Hora["HC_A_C15"].ToString(),
                            HC_A_C16 = Lectura_Hora["HC_A_C16"].ToString(),
                            HC_A_C17 = Lectura_Hora["HC_A_C17"].ToString(),
                            HC_A_C18 = Lectura_Hora["HC_A_C18"].ToString(),
                            HC_A_C19 = Lectura_Hora["HC_A_C19"].ToString(),
                            HC_A_C20 = Lectura_Hora["HC_A_C20"].ToString(),
                            HC_A_C21 = Lectura_Hora["HC_A_C21"].ToString(),
                            HC_A_C22 = Lectura_Hora["HC_A_C22"].ToString(),
                            HC_A_C23 = Lectura_Hora["HC_A_C23"].ToString(),
                            HC_A_C24 = Lectura_Hora["HC_A_C24"].ToString(),

                            HC_A_T11 = Lectura_Hora["HC_A_T11"].ToString(),
                            HC_A_T12 = Lectura_Hora["HC_A_T12"].ToString(),
                            HC_A_T13 = Lectura_Hora["HC_A_T13"].ToString(),
                            HC_A_T14 = Lectura_Hora["HC_A_T14"].ToString(),
                            HC_A_T15 = Lectura_Hora["HC_A_T15"].ToString(),
                            HC_A_T16 = Lectura_Hora["HC_A_T16"].ToString(),
                            HC_A_T17 = Lectura_Hora["HC_A_T17"].ToString(),
                            HC_A_T18 = Lectura_Hora["HC_A_T18"].ToString(),

                            HC_A_T19 = Lectura_Hora["HC_A_T19"].ToString(),
                            HC_A_T20 = Lectura_Hora["HC_A_T20"].ToString(),
                            HC_A_T21 = Lectura_Hora["HC_A_T21"].ToString(),
                            HC_A_T22 = Lectura_Hora["HC_A_T22"].ToString(),
                            HC_A_T23 = Lectura_Hora["HC_A_T23"].ToString(),
                            HC_A_T24 = Lectura_Hora["HC_A_T24"].ToString(),

                            HC_A_C25 = Lectura_Hora["HC_A_C25"].ToString(),
                            HC_A_C26 = Lectura_Hora["HC_A_C26"].ToString(),
                            HC_A_C27 = Lectura_Hora["HC_A_C27"].ToString(),
                            HC_A_C28 = Lectura_Hora["HC_A_C28"].ToString(),
                            HC_A_C29 = Lectura_Hora["HC_A_C29"].ToString(),

                            HC_A_T25 = Lectura_Hora["HC_A_T25"].ToString(),
                            HC_A_T26 = Lectura_Hora["HC_A_T26"].ToString(),
                            HC_A_T27 = Lectura_Hora["HC_A_T27"].ToString(),
                            HC_A_T28 = Lectura_Hora["HC_A_T28"].ToString(),
                            HC_A_T29 = Lectura_Hora["HC_A_T29"].ToString(),
                            HC_A_T30 = Lectura_Hora["HC_A_T30"].ToString(),
                            HC_A_T31 = Lectura_Hora["HC_A_T31"].ToString(),
                            HC_A_T32 = Lectura_Hora["HC_A_T32"].ToString(),
                            HC_A_T33 = Lectura_Hora["HC_A_T33"].ToString(),
                            HC_A_T34 = Lectura_Hora["HC_A_T34"].ToString(),
                            HC_A_T35 = Lectura_Hora["HC_A_T35"].ToString(),

                            HC_A_T37 = Lectura_Hora["HC_A_T37"].ToString(),
                            HC_A_T38 = Lectura_Hora["HC_A_T38"].ToString(),
                            HC_A_T36 = Lectura_Hora["HC_A_T46"].ToString(),
                            HC_A_T39 = Lectura_Hora["HC_A_T39"].ToString(),

                            HC_A_T40 = Lectura_Hora["HC_A_T40"].ToString(),
                            HC_A_T41 = Lectura_Hora["HC_A_T41"].ToString(),
                            HC_A_T42 = Lectura_Hora["HC_A_T42"].ToString(),
                            HC_A_T43 = Lectura_Hora["HC_A_T43"].ToString(),

                            HC_A_T48 = Lectura_Hora["HC_A_T48"].ToString(),
                            HC_A_T47 = Lectura_Hora["HC_A_T47"].ToString(),
                            HC_A_T45 = Lectura_Hora["HC_A_T45"].ToString(),
                            HC_A_T44 = Lectura_Hora["HC_A_T44"].ToString(),
                            HC_A_T50 = Lectura_Hora["HC_A_T50"].ToString(),
                            HC_A_T51 = Lectura_Hora["HC_A_T51"].ToString(),
                            HC_A_T52 = Lectura_Hora["HC_A_T52"].ToString(),
                            HC_A_T53 = Lectura_Hora["HC_A_T53"].ToString(),
                            HC_A_T54 = Lectura_Hora["HC_A_T54"].ToString(),
                            HC_A_T55 = Lectura_Hora["HC_A_T55"].ToString(),
                            HC_A_T56 = Lectura_Hora["HC_A_T56"].ToString(),
                            HC_A_T57 = Lectura_Hora["HC_A_T57"].ToString(),
                            HC_A_T58 = Lectura_Hora["HC_A_T58"].ToString(),
                            HC_A_T59 = Lectura_Hora["HC_A_T59"].ToString(),
                            HC_A_T60 = Lectura_Hora["HC_A_T60"].ToString(),

                            HC_HisOcu = Lectura_Hora["HC_HisOcu"].ToString(),
                            HC_AntLaboral = Lectura_Hora["HC_AntLaboral"].ToString(),
                            HC_ObservaFMS = Lectura_Hora["HC_ObservaFMS"].ToString(),
                            HC_A_ObservaVD = Lectura_Hora["HC_A_ObservaVD"].ToString(),
                            HC_A_ObservaH = Lectura_Hora["HC_A_ObservaH"].ToString(),
                            HC_A_ObservaAVD = Lectura_Hora["HC_A_ObservaAVD"].ToString(),
                            HC_HabRut = Lectura_Hora["HC_HabRut"].ToString(),

                            HC_DiagOcu = Lectura_Hora["HC_DiagOcu"].ToString(),
                            HC_PronoOcu = Lectura_Hora["HC_PronoOcu"].ToString(),
                            HC_Acciones = Lectura_Hora["HC_Acciones"].ToString(),
                            ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                            RMedico = Lectura_Hora["Bod_Reg_Med"].ToString()
                        });
                        return sub_Class_HCTO;
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
        List<OrdenesC> IReportes.ExportaOrdenCompra(int Orden)
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

                    String Cargar_Hora2 = "SELECT * " +
                                          "FROM CXN_PEDIDOSF " +
                                          "INNER JOIN CXN_PEDIDOS ON CXN_PEDIDOSF.Num_Orden = CXN_PEDIDOS.Num_Pedido " +
                                          "INNER JOIN  CXN_CIA ON CXN_PEDIDOSF.Codigo_Prest = CXN_CIA.Com_Identificador " +
                                          "INNER JOIN CXN_PROVEEDORES ON CXN_PEDIDOSF.Codigo_Provee = CXN_PROVEEDORES.Codigo " +
                                          "WHERE CXN_PEDIDOSF.Num_Orden = '" + Orden + "'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<OrdenesC> Class_OC1 = new List<OrdenesC>();

                        while (Lectura_Hora.Read() == true)
                        {
                            string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                            Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                            MemoryStream stmBLOBData = new MemoryStream(bytes);
                            PictureBox pic = new PictureBox();
                            pic.Image = Image.FromStream(stmBLOBData);

                            Class_OC1.Add(new OrdenesC
                            {
                                Logo = repoGen.GetBytes(pic.Image),
                                FechaBase = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                Num_Orden = Convert.ToInt32(Lectura_Hora["Num_Orden"]),
                                EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                Com_Email = Lectura_Hora["Com_Email"].ToString(),
                                PacienteNombre = Lectura_Hora["Nombre"].ToString(),
                                PacienteIdentificacion = Lectura_Hora["Identificacion"].ToString(),
                                PacienteDireccion = Lectura_Hora["Direccion"].ToString(),
                                PacienteTelefono = Lectura_Hora["Telefono"].ToString(),
                                Horario = "Lunes a Viernes de 7 am a 2 pm",
                                Recibe = "Area Administrativa",
                                Usuario = Lectura_Hora["Usuario"].ToString(),
                                ObservacionG = Lectura_Hora["ObservacionG"].ToString(),
                                Cod_Item_Pro = Lectura_Hora["Cod_Item_Pro"].ToString(),
                                Cod_ItemI = Lectura_Hora["Cod_ItemI"].ToString(),
                                Item = Lectura_Hora["Item"].ToString(),
                                Observacion = "Detalle: " + Lectura_Hora["Observacion"].ToString(),
                                Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"])
                            });
                        }
                        return Class_OC1;
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
        public class PrntAgendas : AgendasRecepcionImprime
        {
            public static List<PrntAgendas> Genera_Export_Citas(string tipo_reporte_citas,
                DateTime FechaAgenda,
                string prof_med_citas)
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

                        String Cargar_Hora2 = "";
                        string Tipo = "";

                        if (tipo_reporte_citas == "Mixto")
                        {
                            Tipo = "Seleccion personal de medicos de la agenda";

                            Cargar_Hora2 = "SELECT Ase_Identificador, Ase_Descripcion, Hor_Pac_Bod, Hor_Pac_id, Hor_Estado, Hor_Pac_Hora_Cita, " +
                                                  "Hor_Pac_Fecha_Cita, Hor_Imp_Age, Pac_Id, Pac_Telefono, Bod_Responsable, Hor_Id, Hor_Observacion, " +
                                                  "Hor_Vales, Hor_AvisoCurInicio, Hor_Tipo_Paciente, Pac_Bonos " +
                                                  "FROM Cxn_Horario " +
                                                  "INNER JOIN Cxn_Pacientes ON Cxn_Horario.Hor_Pac_Id = Cxn_Pacientes.Pac_Id " +
                                                  "INNER JOIN Cxn_Bodegas ON Cxn_Horario.Hor_Pac_Bod = Cxn_Bodegas.Bod_Numero " +
                                                  "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador  " +
                                                  "WHERE Cxn_Horario.Hor_Pac_Bod IN (" + prof_med_citas + ") " +
                                                  "AND Cxn_Horario.Hor_Pac_Fecha_Cita = @param1 " +
                                                  "AND Cxn_Horario.Hor_Estado <> 'C' " +
                                                  "AND Cxn_Horario.Hor_Pac_Ase <> '88' " +
                                                  "ORDER BY Cxn_Bodegas.Bod_Responsable, Cxn_Horario.Hor_Pac_Hora_Cita ASC";
                        }
                        else if (tipo_reporte_citas == "Total")
                        {
                            Tipo = "Reporte total de medicos de la agenda";

                            Cargar_Hora2 = "SELECT Ase_Identificador, Ase_Descripcion, Hor_Pac_Bod, Hor_Pac_id, Hor_Estado, Hor_Pac_Hora_Cita, " +
                                                  "Hor_Pac_Fecha_Cita, Hor_Imp_Age, Pac_Id, Pac_Telefono, Bod_Responsable, Hor_Id, Hor_Observacion, " +
                                                  "Hor_Vales, Hor_AvisoCurInicio, Hor_Tipo_Paciente, Pac_Bonos " +
                                                  "FROM Cxn_Horario " +
                                                  "INNER JOIN Cxn_Pacientes ON Cxn_Horario.Hor_Pac_Id = Cxn_Pacientes.Pac_Id " +
                                                  "INNER JOIN Cxn_Bodegas ON Cxn_Horario.Hor_Pac_Bod = Cxn_Bodegas.Bod_Numero " +
                                                  "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                                  "WHERE Cxn_Horario.Hor_Pac_Fecha_Cita = @param1  " +
                                                  "AND Cxn_Horario.Hor_Estado <> 'C' " +
                                                  "AND Cxn_Horario.Hor_Pac_Ase <> '88' " +
                                                  "ORDER BY Cxn_Bodegas.Bod_Responsable, Cxn_Horario.Hor_Pac_Hora_Cita ASC";
                        }
                        else
                        {
                            return null;
                        }                        

                        using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                        {
                            Carga_Command2.Parameters.AddWithValue("@param1", Convert.ToDateTime(FechaAgenda).ToString(getData["Format_Fecha"]));

                            using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                            {
                                if (Lectura_Hora2.HasRows)
                                {
                                    List<PrntAgendas> export_citas_report = new List<PrntAgendas>();

                                    while (Lectura_Hora2.Read() == true)
                                    {
                                        string est = "NO REGISTRA";

                                        switch (Lectura_Hora2["Hor_Estado"].ToString())
                                        {
                                            case "A":
                                                est = "AGENDADO";
                                                break;

                                            case "P":
                                                est = "PENDIENTE LLAMADO";
                                                break;

                                            case "H":
                                                est = "ATENDIDO";
                                                break;

                                            default:
                                                est = "NO REGISTRA";
                                                break;
                                        }

                                        bool ini = (bool)Lectura_Hora2["Hor_AvisoCurInicio"];

                                        export_citas_report.Add(new PrntAgendas
                                        {
                                            Admision = Convert.ToInt32(Lectura_Hora2["Hor_Id"]),
                                            HoraCita = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Hora_Cita"]),
                                            PacienteNombre = Lectura_Hora2["Hor_Imp_Age"].ToString(),
                                            PacienteTelefono = Lectura_Hora2["Pac_Telefono"].ToString(),
                                            ProfesionalNombre = Lectura_Hora2["Bod_Responsable"].ToString(),
                                            Hor_Observacion = Lectura_Hora2["Hor_Observacion"].ToString(),
                                            Tipo_Detalle = Tipo,
                                            PacienteAseguradora = Lectura_Hora2["Ase_Descripcion"].ToString(),
                                            Estado_Cita = est,
                                            fecha_rpt_citas = Convert.ToDateTime(FechaAgenda),
                                            ESPE = Lectura_Hora2["Pac_Bonos"] == DBNull.Value ? "FIRMAS" :
                                                   Lectura_Hora2["Pac_Bonos"].ToString() == "A" ? "BONOS" :
                                                   Lectura_Hora2["Pac_Bonos"].ToString() == "N" ? "FIRMAS" :
                                                   "VERIFICAR",
                                            Dia_Cita = ini == true ? "INICIO" :
                                                       Lectura_Hora2["Hor_Tipo_Paciente"].ToString() == "N" ?
                                                       "NUEVO" : ""
                                        });
                                    }

                                    return export_citas_report;
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

            public static List<PrntAgendas> Genera_CitasXPac_Report(string DOC,
                                                                    DateTime Desde,
                                                                    DateTime Hasta)
            {
                try
                {
                    Dictionary<string,string> getData = Conexion.Conection();

                    DateTime Hoy = DateTime.Now.Date;
                    using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                    {
                        if (con != null && con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        String Cargar_Hora2 = "SELECT (CASE DATENAME(dw, CXN_HORARIO.Hor_Pac_Fecha_Cita) " +
                                         " WHEN 'MONDAY' THEN 'LUNES' " +
                                         " WHEN 'TUESDAY' THEN 'MARTES' " +
                                         " WHEN 'WEDNESDAY' THEN 'MIERCOLES' " +
                                         " WHEN 'THURSDAY' THEN 'JUEVES' " +
                                         " WHEN 'FRIDAY' THEN 'VIERNES' " +
                                         " WHEN 'SATURDAY' THEN 'SABADO' " +
                                         " WHEN 'SUNDAY' THEN 'DOMINGO' " +
                                         " END) as DIAN, " +
                                         "(CASE WHEN (CXN_BODEGAS.Bod_Tipo = 'FI') THEN 'FISIATRA' " +
                                         " WHEN (CXN_BODEGAS.Bod_Tipo = 'TF' AND CXN_HORARIO.Hor_Pac_Cup <> '890505') THEN 'TERAPIA FISICA' WHEN (CXN_BODEGAS.Bod_Tipo = 'TF') AND (CXN_HORARIO.Hor_Pac_Cup = '890505') THEN 'JUNTA MEDICA' " +
                                         " WHEN (CXN_BODEGAS.Bod_Tipo = 'TO') THEN 'TERAPIA OCUPACIONAL' " +
                                         " WHEN (CXN_BODEGAS.Bod_Tipo = 'PS') THEN 'PSICOLOGIA' " +
                                         " WHEN (CXN_BODEGAS.Bod_Tipo = 'MG') THEN 'MEDICINA GENERAL' " +
                                         " WHEN (CXN_BODEGAS.Bod_Tipo = 'CU') THEN 'CURACION' END) AS ESPE, " +
                                         " CXN_ASEGURADORA.Ase_Identificador,  CXN_ASEGURADORA.Ase_Descripcion, CXN_HORARIO.Hor_Pac_Bod, CXN_HORARIO.Hor_Pac_Id, CXN_HORARIO.Hor_Estado, " +
                                         " CXN_HORARIO.Hor_Pac_Hora_Cita as Hora, CXN_HORARIO.Hor_Pac_Fecha_Cita, CXN_HORARIO.Hor_Imp_Age, CXN_PACIENTES.Pac_Id, " +
                                         " CXN_PACIENTES.Pac_Telefono , CXN_BODEGAS.Bod_Responsable, CXN_HORARIO.Hor_Id, CXN_CIA.Com_Logo " +
                                         " FROM CXN_HORARIO " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador" +
                                         " WHERE (CXN_PACIENTES.Pac_IdNum = '" + DOC + "') " +
                                         " AND (CXN_HORARIO.Hor_Pac_Fecha_Cita BETWEEN  '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "') " +
                                         " AND (CXN_HORARIO.Hor_Estado <> 'C') " +
                                         " ORDER BY CXN_HORARIO.Hor_Pac_Fecha_Cita, CXN_HORARIO.Hor_Pac_Hora_Cita ASC";
                        SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con); 
                        SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                        if (Lectura_Hora2.HasRows)
                        {
                            List<PrntAgendas> export_citas_report = new List<PrntAgendas>();
                            while (Lectura_Hora2.Read() == true)
                            {
                                string Bod_Firma1 = Lectura_Hora2["Com_Logo"].ToString(); //trae base64
                                Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                MemoryStream stmBLOBData = new MemoryStream(bytes);
                                PictureBox pic = new PictureBox();
                                pic.Image = System.Drawing.Image.FromStream(stmBLOBData);

                                export_citas_report.Add(new PrntAgendas
                                {
                                    Admision = Convert.ToInt32(Lectura_Hora2["Hor_Id"]),
                                    HoraCita = Convert.ToDateTime(Lectura_Hora2["Hora"]),
                                    PacienteNombre = Lectura_Hora2["Hor_Imp_Age"].ToString(),
                                    PacienteTelefono = Lectura_Hora2["Pac_Telefono"].ToString(),
                                    ProfesionalNombre = Lectura_Hora2["Bod_Responsable"].ToString(),
                                    Dia_Cita = Lectura_Hora2["DIAN"].ToString(),
                                    FechaBase = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]),
                                    ESPE = Lectura_Hora2["ESPE"].ToString(),
                                    fecha_rpt_citas = Convert.ToDateTime(Hoy),
                                    Logo = repoGen.GetBytes(pic.Image)
                                });
                            }
                            return export_citas_report;
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
        }
        List<FacturacionReports> IReportes.Exportar(DateTime Desde,
                                                        DateTime Hasta,
                                                        int Cia,
                                                        int Ase,
                                                        string Tipos)
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
                    String Cargar_Hora = "SELECT CXN_FACTURA.Fac_Num_Fac, CXN_FACTURA.Fac_Num_Aut, CXN_FACTURA.Fac_Cia, CXN_FACTURA.Fac_Ase, CXN_FACTURA.Fac_Fecha, CXN_FACTURA.Fac_Usr_Graba, CXN_FACTURA.Homologo, CXN_FACTURA.CUV, CXN_FACTURA.FormaPago, CXN_FACTURA.Num_Cruce, " +
                                         " CXN_FACTURA.Fac_Pac, CXN_ASEGURADORA.Ase_Descripcion, CXN_PACIENTES.Pac_PrimerN + ' ' + CXN_PACIENTES.Pac_SegundoN + ' ' + CXN_PACIENTES.Pac_PrimerA + ' ' + CXN_PACIENTES.Pac_SegundoA AS Nombre, CXN_FACTURA.Fac_Tipo_Doc, CXN_PACIENTES.Pac_Id, " +
                                         " SUM(CXN_CARGOS.Car_Val_Tot) as Valor " +
                                         " FROM CXN_CARGOS " +
                                         " INNER JOIN CXN_FACTURA ON CXN_CARGOS.Car_Factura = CXN_FACTURA.Fac_Num_Fac " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_FACTURA.Fac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_CARGOS.Car_Pac = CXN_PACIENTES.Pac_Id " +
                                         " WHERE CXN_FACTURA.Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "' " +
                                         " AND CXN_FACTURA.Fac_Cia = '" + Cia + "' " +
                                         " AND CXN_FACTURA.Fac_Ase = '" + Ase + "'  " +
                                         " AND CXN_FACTURA.Fac_Estado = 'F' " +
                                         " AND CXN_FACTURA.Fac_Tipo_Doc = '" + Tipos + "' " +
                                         " AND CXN_CARGOS.Car_Tipo_Doc = '" + Tipos + "' " +
                                         " GROUP BY CXN_FACTURA.fac_num_fac, CXN_FACTURA.Fac_Cia, CXN_FACTURA.Fac_Ase, CXN_FACTURA.Fac_Fecha, CXN_FACTURA.Fac_Usr_Graba, CXN_FACTURA.Homologo, " +
                                         " CXN_FACTURA.Fac_Pac, CXN_ASEGURADORA.Ase_Descripcion, CXN_FACTURA.Fac_Num_Aut, CXN_PACIENTES.Pac_Id, CXN_PACIENTES.Pac_PrimerN, CXN_PACIENTES.Pac_SegundoN, CXN_PACIENTES.Pac_PrimerA, CXN_PACIENTES.Pac_SegundoA, CXN_FACTURA.Fac_Tipo_Doc, CXN_FACTURA.CUV, CXN_FACTURA.FormaPago, CXN_FACTURA.Num_Cruce " +
                                         " ORDER BY CXN_FACTURA.Fac_Num_Fac ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FacturacionReports> Class_Fac_Serv1 = new List<FacturacionReports>();
                                var Total_Fac = Total_Fac_Serv(Tipos, Desde, Hasta, Cia, Ase);
                                var Empresa = repoCIA.getPrestadorbyCode(Cia);
                                var Asegura = repoAse.getInfoFromAsebyCode(Ase);

                                while (Lectura_Hora.Read() == true)
                                {
                                    switch (Lectura_Hora["Fac_Tipo_Doc"].ToString())
                                    {
                                        case "DE":
                                            Tipos = "Documento Equivalente";
                                            break;

                                        case "FA":
                                            Tipos = "Factura";
                                            break;

                                        case "OP":
                                            Tipos = "Orden de Pedido";
                                            break;

                                        default:
                                            Tipos = "ERROR";
                                            break;
                                    }

                                    string TipoReporte = Ase == 99 ? "Particulares" : "Aseguradoras";

                                    Class_Fac_Serv1.Add(new FacturacionReports
                                    {
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                        Admision = Convert.ToInt32(Lectura_Hora["Fac_Num_Fac"]),
                                        PacienteDireccion = Lectura_Hora["Fac_Usr_Graba"].ToString(), //USUARIO QUE GRABA
                                        PacienteNombre = Lectura_Hora["Nombre"].ToString(),
                                        ValorReciboFactura = Convert.ToInt32(Lectura_Hora["Valor"]),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        TotalRpt = Convert.ToInt32(Total_Fac),
                                        Homologo = Lectura_Hora["Homologo"] == DBNull.Value ? "" : Lectura_Hora["Homologo"].ToString(),
                                        Desde = Convert.ToDateTime(Desde),
                                        Hasta = Convert.ToDateTime(Hasta),
                                        EmpresaNombre = Empresa.Com_Nombre.ToString(),
                                        Tipo = Tipos,
                                        ProfesionalNombre = Asegura.Ase_Descripcion.ToString(), //ASEGURADORA TITULO REPORTE
                                        Com_Direccion = (Lectura_Hora["CUV"] == DBNull.Value ? "" : Lectura_Hora["CUV"].ToString()),
                                        PacienteTelefono = (Lectura_Hora["FormaPago"] == DBNull.Value ? "" : Lectura_Hora["FormaPago"].ToString()),
                                        Num_Cruce = Lectura_Hora["Num_Cruce"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Num_Cruce"]),
                                        TReport = TipoReporte,
                                        PacienteIdentificacion = Convert.ToInt32(Lectura_Hora["Pac_Id"]).ToString(),
                                        EmpresaDireccion = (Lectura_Hora["Fac_Num_Aut"] == DBNull.Value ? "" : Lectura_Hora["Fac_Num_Aut"].ToString())                                        
                                    });
                                }

                                int Efectivo = 0;
                                int TarjetaCredito = 0;
                                int TarjetaDebito = 0;
                                int Nequi = 0;
                                int Daviplata = 0;
                                int OtrasBilleteras = 0;
                                int SinClasificar = 0;

                                if (Class_Fac_Serv1 != null && Class_Fac_Serv1.Count > 0)
                                {
                                    Efectivo = Class_Fac_Serv1.Where(x => x.PacienteTelefono == "Efectivo").Sum(x => x.ValorReciboFactura);
                                    TarjetaCredito = Class_Fac_Serv1.Where(x => x.PacienteTelefono == "Tarjeta Credito").Sum(x => x.ValorReciboFactura);
                                    TarjetaDebito = Class_Fac_Serv1.Where(x => x.PacienteTelefono == "Tarjeta Debito").Sum(x => x.ValorReciboFactura);
                                    Nequi = Class_Fac_Serv1.Where(x => x.PacienteTelefono == "Nequi").Sum(x => x.ValorReciboFactura);
                                    Daviplata = Class_Fac_Serv1.Where(x => x.PacienteTelefono == "Daviplata").Sum(x => x.ValorReciboFactura);
                                    OtrasBilleteras = Class_Fac_Serv1.Where(x => x.PacienteTelefono == "Otras Billeteras").Sum(x => x.ValorReciboFactura);
                                    SinClasificar = Class_Fac_Serv1.Where(x => x.PacienteTelefono == "" || x.PacienteTelefono is null).Sum(x => x.ValorReciboFactura);

                                    foreach (var i in Class_Fac_Serv1)
                                    {
                                        i.Efectivo = Efectivo;
                                        i.TarjetaCredito = TarjetaCredito;
                                        i.Nequi = Nequi;
                                        i.TarjetaDebito = TarjetaDebito;
                                        i.Daviplata = Daviplata;
                                        i.OtrasBilleteras = OtrasBilleteras;
                                        i.SinClasificar = SinClasificar;
                                    }
                                }

                                return Class_Fac_Serv1;
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
        int Total_Fac_Serv(string Tips,
                           DateTime Desde1,
                           DateTime Hasta1,
                           int Cia1,
                           int Ase1)
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
                    String Cargar_Hora = " SELECT CXN_FACTURA.Fac_Cia, CXN_FACTURA.Fac_Ase, " +
                                         " SUM(CXN_CARGOS.Car_Val_Tot) as Valor " +
                                         " FROM CXN_CARGOS " +
                                         " INNER JOIN CXN_FACTURA ON CXN_CARGOS.Car_Factura = CXN_FACTURA.Fac_Num_Fac " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_FACTURA.Fac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_CARGOS.Car_Pac = CXN_PACIENTES.Pac_Id " +
                                         " WHERE CXN_FACTURA.Fac_Fecha BETWEEN '" + Convert.ToDateTime(Desde1).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta1).ToString(getData["Format_Fecha"]) + "' " +
                                         " AND CXN_FACTURA.Fac_Cia = '" + Cia1 + "' " +
                                         " AND CXN_FACTURA.Fac_Ase = '" + Ase1 + "'  " +
                                         " AND CXN_FACTURA.Fac_Estado = 'F' " +
                                         " AND CXN_FACTURA.Fac_Tipo_Doc = '" + Tips + "' " +
                                         " AND CXN_CARGOS.Car_Tipo_Doc = '" + Tips + "' " +
                                         " GROUP BY CXN_FACTURA.Fac_Cia, CXN_FACTURA.Fac_Ase";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Convert.ToInt32(Lectura_Hora["Valor"]);
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
        List<CXN_CIA> IReportes.ActasMedicos(int Admision)
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
                    String Cargar_Hora = "SELECT P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_IdNum, P.Pac_TipoId, " +
                                         "B.Bod_Responsable, P.Pac_Acudiente, P.Pac_Parentesco, Pac_Direccion, Pac_Telefono, B.Bod_Reg_Med, " +
                                         "C.Com_Logo, C.Com_Nombre, C.Com_Direccion, C.Com_Telefono, C.Com_Identificacion, C.Com_Tipo_Doc " +
                                         "FROM CXN_PACIENTES P " +
                                         "INNER JOIN CXN_HORARIO H ON P.Pac_Id = H.Hor_Pac_Id " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                         "WHERE H.Hor_Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_CIA> lista = new List<CXN_CIA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = System.Drawing.Image.FromStream(stmBLOBData);

                                    lista.Add(new CXN_CIA
                                    {
                                        PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                                         Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                                         Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                                         Lectura_Hora["Pac_SegundoN"].ToString(),
                                        PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " +
                                                                 Lectura_Hora["Pac_IdNum"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Bod_Responsable"].ToString(),//medico
                                        Logo = repoGen.GetBytes(pic.Image),
                                        Com_Nombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        Com_Identificacion = Lectura_Hora["Com_Tipo_Doc"].ToString() + " " + Lectura_Hora["Com_Identificacion"].ToString(),
                                        Com_Cod_Prestador = Lectura_Hora["Pac_Acudiente"].ToString(),
                                        Com_Cod_Prestador_2 = Lectura_Hora["Pac_Parentesco"].ToString(),
                                        PacienteDireccion = Lectura_Hora["Pac_Direccion"].ToString(),
                                        PacienteTelefono = Lectura_Hora["Pac_Telefono"].ToString(),

                                        Com_UsuarioGraba = "Yo " + Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                                                  Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                                                  Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                                                  Lectura_Hora["Pac_SegundoN"].ToString() + ", " +
                                                                  "identificado con documento " + Lectura_Hora["Pac_TipoId"].ToString() + " " +
                                                                  Lectura_Hora["Pac_IdNum"].ToString() +
                                                                  ", autorizo al Doctor(a) " + Lectura_Hora["Bod_Responsable"].ToString() + " " +
                                                                  "y sus asistentes en el " + Lectura_Hora["Com_Nombre"].ToString() + " " +
                                                                  ", a realizar en mi la siguiente(s) intervención(es) quirúrgica(s) o procedimiento(s)" +
                                                                  "/ tratamiento(s) especial(es) CURACIONES CON APOSITOS ESPECIALES Y/O USO DE VENDAJES " +
                                                                  "SEGÚN CRITERIO MEDICO, ASI COMO LA TOMA DE FOTOS DE LAS HERIDAS COMO PARTE DEL " +
                                                                  "SEGUIMIENTO NECESARIO PARA MI EVOLUCION. \n\r \n\r" +
                                                                  "El Doctor(a) " + Lectura_Hora["Bod_Responsable"].ToString() + " " +
                                                                  "me ha explicado la naturaleza y propósito de la intervención(es) quirúrgica(s) o " +
                                                                  "procedimiento(s) / tratamiento(s) especial(es), me han informado de las ventajas, " +
                                                                  "complicaciones, molestias, posibles alternativas y riesgos inherentes a la " +
                                                                  "intervención propuesta y en particular los siguientes, SANGRADO, REACCIONES ALERGICAS, " +
                                                                  "INFECCIONES, TROMBOSIS, NECROSIS, AUMENTO DE LA LESION, NUEVAS LESIONES, DOLOR. " +
                                                                  "Se me ha informado y entiendo que en el curso de la intervención(es) / tratamiento(s) " +
                                                                  "propuesto(s) pueden presentarse situaciones imprevistas que requieran procedimiento(s) / " +
                                                                  "tratamiento(s) adicional(es), los cuales pueden LIMPIEZA QUIRURGICA, TOMA DE BIOPSIA, " +
                                                                  "INJERTOS, REFERENCIAS A ESPECIALISTAS, SOLICITUD DE TRATAMIENTO ANTIBIOTICO U " +
                                                                  "EXAMENES PARACLINICOS, los cuales deben ser realizados por su EPS. \n\r \n\r" +
                                                                  "Finalmente manifiesto que he recibido y comprendido toda la información respecto al " +
                                                                  "procedimiento(s), intervención(es) y/o tratamiento(s) propuesto(s) y todos los " +
                                                                  "espacios en blanco han sido llenados antes de mi firma. Yo me encuentro en capacidad " +
                                                                  "de expresar mi consentimiento y si no puedo firmarlo, mi familiar o acompañante " +
                                                                  "firmara confirmando mi consentimiento.", //consentimiento
                                        Com_Nombre_SMS = "Yo " + Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                                                  Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                                                  Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                                                  Lectura_Hora["Pac_SegundoN"].ToString() + ", " +
                                                                  "identificado con " + Lectura_Hora["Pac_TipoId"].ToString() + " " +
                                                                  Lectura_Hora["Pac_IdNum"].ToString() + " " +
                                                                  "me declaro totalmente curado de mi herida y me comprometo a cumplir con " +
                                                                  "todos los cuidados de egreso y recomendaciones de salida para contribuir con mi salud", //salida
                                        Com_Telefono_SMS = "Yo " + Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                                                  Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                                                  Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                                                  Lectura_Hora["Pac_SegundoN"].ToString() + " " +
                                                                  "identificado con " + Lectura_Hora["Pac_TipoId"].ToString() + " " +
                                                                  Lectura_Hora["Pac_IdNum"].ToString() + " " +
                                                                  "me declaro totalmente comprometido con el cuidado de mi herida y me comprometo " +
                                                                  "a cumplir con todos los cuidados indicados para mi tratamiento y así " +
                                                                  "contribuir con mi salud."  //ingreso
                                    });

                                    break;
                                }
                                return lista;
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
        bool IReportes.UpdateFormaPago(int FacZamenis, int Cia, string TipoPago, string Factura)
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

                    string Busqueda = "";

                    if (Factura == "Ventas")
                    {
                        Busqueda = "UPDATE CXN_VENTAS " +
                                   "SET FormaPago = @param1 " +
                                   "WHERE Ven_Factura = @param2 " +
                                   "AND Ven_Cod_Cia = @param3";
                    }
                    else if (Factura == "Caja")
                    {
                        Busqueda = "UPDATE CXN_RC_CAJA " +
                                   "SET FormaPago = @param1 " +
                                   "WHERE Rc_Id = @param2 " +
                                   "AND Rc_Caja_Cia = @param3";
                    }
                    else if (Factura == "Particular")
                    {
                        Busqueda = "UPDATE CXN_FACTURA " +
                                   "SET FormaPago = @param1 " +
                                   "WHERE Fac_Num_Fac = @param2 " +
                                   "AND Fac_Cia = @param3";
                    }
                    else
                    {
                        return false;
                    }
                   
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", TipoPago);
                    Accion.Parameters.AddWithValue("@param2", FacZamenis);
                    Accion.Parameters.AddWithValue("@param3", Cia);

                    int Guarda = Accion.ExecuteNonQuery();

                    if (Guarda >= 1) return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_HCRADIOLOGIA> IReportes.RadiologiaReport(int Admision)
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

                    String Cargar_Hora = "SELECT C.Com_Nombre,  C.Com_Identificacion, C.Com_Direccion, C.Com_Telefono, B.Bod_Responsable, B.Bod_Reg_Med, R.PacienteNombre, R.PacienteTipoId, R.PacienteId, P.Pac_Direccion, P.Pac_FechaNto, P.Pac_Sexo, P.Pac_Telefono, " +
                                         "A.Ase_Descripcion, P.Pac_Email, P.Pac_Acudiente, P.Pac_Parentesco, P.Pac_DireccionAcu, P.Pac_TelefonoAcu, P.Pac_CorreoAcu, R.Fecha, " +
                                         "B.Bod_Firma, B.Bod_Responsable, A.Ase_Identificador, C.Com_Identificador, B.Bod_Numero, " +
                                         "H.Hor_Pac_Hora_Cita, H.Hor_Pac_Hora_Salida, " +
                                         "R.MotivoConsulta, R.EnfermedadActual, R.EvolucionSintomas, R.AntecedentesRelevantes, R.MedicamentosActuales, R.AntecedentesRenales, " +
                                         "R.AntecedentesCardioVasculares, R.Embarazo, R.ImplantesMetalicos, R.MenorEdad, R.Presion, R.Peso, R.GlassHow, R.Talla, R.FResp, " +
                                         "R.FCar, R.RH, R.Conciencia, R.IMC, R.ObservacionExaMedico, R.TipoEstudio, R.MedioContraste, R.ReaccionAdversa, R.Tecnica, " +
                                         "R.Hallazgos, R.DX1, R.DX2, R.DX3, R.NotaDX1, R.NotaDX2, R.NotaDX3, R.CausaExterna, R.ImpDX1, R.ImpDX2, R.ImpDX3, R.EstudioComplementario, " +
                                         "R.ControlSeguimiento, R.Compañia, R.NotaAclaratoria, R.PacId, R.Aseguradora, R.Medico, R.Fecha, R.HCAdm, R.HCCant, R.NotaAclaratoria " +
                                         "FROM CXN_HCRADIOLOGIA R " +
                                         "INNER JOIN CXN_PACIENTES P ON R.PacId = P.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA A ON R.Aseguradora = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA C ON R.Compañia = C.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS B ON R.Medico = B.Bod_Numero " +
                                         "INNER JOIN CXN_HORARIO H ON R.HCADM = H.Hor_Id " +
                                         "WHERE R.HCAdm = '" + Admision + "'";

                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<CXN_HCRADIOLOGIA> sub_Class_HCMG = new List<CXN_HCRADIOLOGIA>();

                        string Bod_Firma1 = Lectura_Hora["Bod_Firma"].ToString();
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);

                        var Diag1 = repoCIE10.BuscaDX(Lectura_Hora["Dx1"].ToString());
                        var Diag2 = repoCIE10.BuscaDX(Lectura_Hora["Dx2"].ToString());
                        var Diag3 = repoCIE10.BuscaDX(Lectura_Hora["Dx3"].ToString());

                        string CondicionesText = repoCondiciones.getCondicionesForPrint(Convert.ToInt32(Lectura_Hora["PacId"]), Convert.ToDateTime(Lectura_Hora["Fecha"]));

                        sub_Class_HCMG.Add(new CXN_HCRADIOLOGIA
                        {
                            Com_Nombre = Lectura_Hora["Com_Nombre"].ToString(),
                            Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                            Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                            Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                            Bod_Responsable = Lectura_Hora["Bod_Responsable"].ToString(),
                            Bod_Reg_Med = Lectura_Hora["Bod_Reg_Med"].ToString(),
                            Pac_Direccion =Lectura_Hora["Pac_Direccion"].ToString(),
                            Pac_FechaNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"]),
                            Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                            Pac_Telefono = Lectura_Hora["Pac_Telefono"].ToString(),
                            Ase_Descripcion = Lectura_Hora["Ase_Descripcion"].ToString(),
                            Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                            Pac_Acudiente = Lectura_Hora["Pac_Acudiente"].ToString(),
                            Pac_Parentesco = Lectura_Hora["Pac_Parentesco"].ToString(),
                            Pac_DireccionAcu = Lectura_Hora["Pac_DireccionAcu"].ToString(),
                            Pac_TelefonoAcu = Lectura_Hora["Pac_TelefonoAcu"].ToString(),
                            Pac_CorreoAcu = Lectura_Hora["Pac_CorreoAcu"].ToString(),
                            Bod_Firma = bytes,
                            Hor_Pac_Hora_Salida = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Salida"]),
                            Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                            PacId = Convert.ToInt32(Lectura_Hora["PacId"]),
                            PacienteNombre = Lectura_Hora["PacienteNombre"].ToString(),
                            PacienteTipoId = Lectura_Hora["PacienteTipoId"].ToString(),
                            PacienteId = Lectura_Hora["PacienteId"].ToString(),
                            MotivoConsulta = Lectura_Hora["MotivoConsulta"].ToString(),
                            EnfermedadActual = Lectura_Hora["EnfermedadActual"].ToString(),
                            EvolucionSintomas = Lectura_Hora["EvolucionSintomas"].ToString(),
                            AntecedentesRelevantes = Lectura_Hora["AntecedentesRelevantes"].ToString(),
                            MedicamentosActuales = Lectura_Hora["MedicamentosActuales"].ToString(),
                            AntecedentesRenales = Lectura_Hora["AntecedentesRenales"].ToString(),
                            AntecedentesCardioVasculares = Lectura_Hora["AntecedentesCardioVasculares"].ToString(),
                            Embarazo = Lectura_Hora["Embarazo"].ToString(),
                            ImplantesMetalicos = Lectura_Hora["ImplantesMetalicos"].ToString(),
                            MenorEdad = Lectura_Hora["MenorEdad"].ToString(),
                            Presion = Lectura_Hora["Presion"].ToString(),
                            Peso = Lectura_Hora["Peso"].ToString(),
                            GlassHow = Lectura_Hora["GlassHow"].ToString(),
                            Talla = Lectura_Hora["Talla"].ToString(),
                            FResp = Lectura_Hora["FResp"].ToString(),
                            FCar = Lectura_Hora["FCar"].ToString(),
                            RH = Lectura_Hora["RH"].ToString(),
                            Conciencia = Lectura_Hora["Conciencia"].ToString(),
                            IMC = Lectura_Hora["IMC"].ToString(),
                            ObservacionExaMedico = Lectura_Hora["ObservacionExaMedico"].ToString(),
                            TipoEstudio = Lectura_Hora["TipoEstudio"].ToString(),
                            MedioContraste = Lectura_Hora["MedioContraste"].ToString(),
                            ReaccionAdversa = Lectura_Hora["ReaccionAdversa"].ToString(),
                            Tecnica = Lectura_Hora["Tecnica"].ToString(),
                            Hallazgos = Lectura_Hora["Hallazgos"].ToString(),
                            DX1 = Lectura_Hora["DX1"].ToString() + " - " + Diag1,
                            DX2 = Lectura_Hora["DX2"].ToString() + " - " + Diag2,
                            DX3 = Lectura_Hora["DX3"].ToString() + " - " + Diag3,
                            NotaDX1 = Lectura_Hora["NotaDX1"].ToString(),
                            NotaDX2 = Lectura_Hora["NotaDX2"].ToString(),
                            NotaDX3 = Lectura_Hora["NotaDX3"].ToString(),
                            CausaExterna = Lectura_Hora["CausaExterna"].ToString(),
                            ImpDX1 = Lectura_Hora["ImpDX1"].ToString(),
                            ImpDX2 = Lectura_Hora["ImpDX2"].ToString(),
                            ImpDX3 = Lectura_Hora["ImpDX3"].ToString(),
                            EstudioComplementario = Lectura_Hora["EstudioComplementario"].ToString(),
                            ControlSeguimiento = Lectura_Hora["ControlSeguimiento"].ToString(),
                            HCAdm = Convert.ToInt32(Lectura_Hora["HCAdm"]),
                            Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                            NotaAclaratoria = Lectura_Hora["NotaAclaratoria"].ToString()
                        });

                        return sub_Class_HCMG;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        (Dictionary<int, string> DicNotas, Dictionary<int, string> DicHistorias) IReportes.getAdmitionByInvoiceZamenis(int FacZamenis, int Cia)
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

                    String Cargar_Hora = "SELECT Car_Adm_Id, Car_Tipo " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Factura = @param1 " +
                                         "AND Car_Tipo IN ('Nota','Historia') " +
                                         "AND Car_Cia = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", FacZamenis);
                        Carga_Command.Parameters.AddWithValue("@param2", Cia);

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                Dictionary<int, string> AdmisionesNotas = new Dictionary<int, string>();
                                Dictionary<int, string> AdmisionesHistorias = new Dictionary<int, string>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    int Admision = Convert.ToInt32(Lectura_Hora2["Car_Adm_Id"]);
                                    string Tipo = Lectura_Hora2["Car_Tipo"].ToString();

                                    if (Tipo == "Nota")
                                    {
                                        if (!AdmisionesNotas.ContainsKey(Convert.ToInt32(Lectura_Hora2["Car_Adm_Id"])))
                                        {
                                            AdmisionesNotas.Add(Convert.ToInt32(Lectura_Hora2["Car_Adm_Id"]), Lectura_Hora2["Car_Tipo"].ToString());
                                        }
                                    }
                                    if (Tipo == "Historia")
                                    {
                                        if (!AdmisionesHistorias.ContainsKey(Convert.ToInt32(Lectura_Hora2["Car_Adm_Id"])))
                                        {
                                            AdmisionesHistorias.Add(Convert.ToInt32(Lectura_Hora2["Car_Adm_Id"]), Lectura_Hora2["Car_Tipo"].ToString());
                                        }
                                    }
                                }

                                return (AdmisionesNotas, AdmisionesHistorias);
                            }
                            else
                            {
                                return (null, null);
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (null, null);
            }
        }
    }
}
