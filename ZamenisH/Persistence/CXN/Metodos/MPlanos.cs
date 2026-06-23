using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Persistence.CXN.Metodos
{
    public class MPlanos : IPlanos
    {
        private static readonly IPacientes repoPac = new MPacientes();

        void IPlanos.CA(DateTime desde, DateTime hasta)
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

                    DateTime Hoy = DateTime.Now;

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Cancelaciones_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);

                    SqlCommand comando = new SqlCommand("SELECT P.Pac_TipoId, P.Pac_IdNum, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS Nombre, " +
                                                        "P.Pac_Telefono, P.Pac_TelefonoAux, P.Pac_Email, A.Ase_Descripcion, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_MCancela, H.Hor_Pac_RCancela, H.Hor_Pac_Tipo_Serv " +
                                                        "FROM  CXN_HORARIO H " +
                                                        "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                                        "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                                        "WHERE H.Hor_Estado = 'C' " +
                                                        "AND H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(hasta).ToString(getData["Format_Fecha"]) + "' " +
                                                        "ORDER BY H.Hor_Pac_Fecha_Cita DESC", con);
                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("TIPO DOCUMENTO PACIENTE" + "," + "DOCUMENTO PACIENTE" + "," + "PACIENTE" + "," + "TELEFONO" + "," + "TELEFONO 2" + "," + "EMAIL" + "," + "ASEGURADORA" + "," + "MOTIVO CANCELA" + "," + "OTRA RAZON CANCELA" + "," + "FECHA INASISTENCIA" + "," + "ESPECIALIDAD");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read() == true)
                    {
                        string ESPE;
                        switch (leer["Hor_Pac_Tipo_Serv"].ToString())
                        {
                            case "CU":
                                ESPE = "Curaciones";
                                break;

                            case "MG":
                                ESPE = "Medicina General";
                                break;

                            case "FI":
                                ESPE = "Fisiatria";
                                break;

                            case "PS":
                                ESPE = "Psicologia";
                                break;

                            case "TO":
                                ESPE = "Terapia Ocupacional";
                                break;

                            case "TF":
                                ESPE = "Terapia Fisica";
                                break;

                            default:
                                ESPE = "Sin Datos";
                                break;
                        }

                        Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                        Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                        Escriba.Write(leer["Nombre"].ToString() + ",");
                        Escriba.Write(leer["Pac_Telefono"].ToString() + ",");
                        Escriba.Write(leer["Pac_TelefonoAux"].ToString() + ",");
                        Escriba.Write(leer["Pac_Email"].ToString() + ",");
                        Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                        Escriba.Write(leer["Hor_Pac_MCancela"].ToString() + ",");
                        Escriba.Write(leer["Hor_Pac_RCancela"].ToString() + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"]).ToString(getData["Format_Fecha"]) + ",");
                        Escriba.Write(ESPE);
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();
                    MessageBox.Show("Generado en C CXN Reportes Cancelaciones.txt");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IPlanos.RE(DateTime desde, DateTime hasta)
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

                    DateTime Hoy = DateTime.Now;

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Retardos_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);

                    SqlCommand comando = new SqlCommand("SELECT P.Pac_TipoId, P.Pac_IdNum, Pac_PrimerA + ' ' + Pac_SegundoA + ' ' + Pac_PrimerN + ' ' + Pac_SegundoN AS Nombre, " +
                                                        "Pac_Telefono, Pac_TelefonoAux, Pac_Email, A.Ase_Descripcion, Hor_Pac_Fecha_Cita, H.Hor_Pac_Minutos, H.Hor_Pac_Razon, H.Hor_Pac_Tipo_Serv " +
                                                        "FROM  CXN_HORARIO H " +
                                                        "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                                        "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                                        "WHERE H.Hor_Estado = 'H' " +
                                                        "AND H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(hasta).ToString(getData["Format_Fecha"]) + "' " +
                                                        "ORDER BY H.Hor_Pac_Fecha_Cita DESC", con);
                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("TIPO DOCUMENTO PACIENTE" + "," + "DOCUMENTO PACIENTE" + "," + "PACIENTE" + "," + "TELEFONO" + "," + "TELEFONO 2" + "," + "EMAIL" + "," + "ASEGURADORA" + "," + "MOTIVO RETARDO" + "," + "MINUTOS DE RETARDO" + "," + "FECHA INASISTENCIA" + "," + "ESPECIALIDAD");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read() == true)
                    {
                        string ESPE;
                        switch (leer["Hor_Pac_Tipo_Serv"].ToString())
                        {
                            case "CU":
                                ESPE = "Curaciones";
                                break;

                            case "MG":
                                ESPE = "Medicina General";
                                break;

                            case "FI":
                                ESPE = "Fisiatria";
                                break;

                            case "PS":
                                ESPE = "Psicologia";
                                break;

                            case "TO":
                                ESPE = "Terapia Ocupacional";
                                break;

                            case "TF":
                                ESPE = "Terapia Fisica";
                                break;

                            default:
                                ESPE = "Sin Datos";
                                break;
                        }

                        Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                        Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                        Escriba.Write(leer["Nombre"].ToString() + ",");
                        Escriba.Write(leer["Pac_Telefono"].ToString() + ",");
                        Escriba.Write(leer["Pac_TelefonoAux"].ToString() + ",");
                        Escriba.Write(leer["Pac_Email"].ToString() + ",");
                        Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                        Escriba.Write(leer["Hor_Pac_Minutos"].ToString() + ",");
                        Escriba.Write(leer["Hor_Pac_Razon"].ToString() + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"]).ToString(getData["Format_Fecha"]) + ",");
                        Escriba.Write(ESPE);
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();
                    MessageBox.Show("Generado en C CXN Reportes Retardos.txt");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IPlanos.ANCS(DateTime desde, DateTime hasta)
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

                    DateTime Hoy = DateTime.Now;

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Informe_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);

                    SqlCommand comando = new SqlCommand("SELECT P.Pac_TipoId, P.Pac_IdNum, Pac_PrimerA + ' ' + Pac_SegundoA + ' ' + Pac_PrimerN + ' ' + Pac_SegundoN AS Nombre, " +
                                                        "Pac_Telefono, Pac_TelefonoAux, Pac_Email, A.Ase_Descripcion, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Sal " +
                                                        "FROM  CXN_HORARIO H " +
                                                        "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                                        "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                                        "WHERE H.Hor_Estado = 'H' " +
                                                        "AND H.Hor_Pac_Tipo_Serv = 'MG' " +
                                                        "AND H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(hasta).ToString(getData["Format_Fecha"]) + "' " +
                                                        "ORDER BY H.Hor_Pac_Fecha_Cita DESC", con);
                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("TIPO DOCUMENTO PACIENTE" + "," + "DOCUMENTO PACIENTE" + "," + "PACIENTE" + "," + "TELEFONO" + "," + "TELEFONO 2" + "," + "EMAIL" + "," + "ASEGURADORA" + "," + "TIPO DE CITA" + "," + "FECHA DE CITA");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read() == true)
                    {
                        string TC;
                        switch (leer["Hor_Pac_Sal"].ToString())
                        {
                            case "N":
                                TC = "Paciente Nuevo";
                                break;

                            case "A":
                                TC = "Paciente Antiguo";
                                break;

                            case "R":
                                TC = "Reingreso";
                                break;

                            case "C":
                                TC = "Cita de Control";
                                break;

                            case "E":
                                TC = "Salida por Enfermeria";
                                break;

                            default:
                                TC = "SIN DATOS";
                                break;
                        }

                        Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                        Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                        Escriba.Write(leer["Nombre"].ToString() + ",");
                        Escriba.Write(leer["Pac_Telefono"].ToString() + ",");
                        Escriba.Write(leer["Pac_TelefonoAux"].ToString() + ",");
                        Escriba.Write(leer["Pac_Email"].ToString() + ",");
                        Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                        Escriba.Write(TC + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"]).ToString(getData["Format_Fecha"]) + ",");
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();
                    MessageBox.Show("Generado en C CXN Reportes Informe.txt");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IPlanos.RI(DateTime desde, DateTime hasta)
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

                    DateTime Hoy = DateTime.Now;

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Inasistencias_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);

                    SqlCommand comando = new SqlCommand("SELECT P.Pac_TipoId, P.Pac_IdNum, Pac_PrimerA + ' ' + Pac_SegundoA + ' ' + Pac_PrimerN + ' ' + Pac_SegundoN AS Nombre, " +
                                                        "Pac_Telefono, Pac_TelefonoAux, Pac_Email, A.Ase_Descripcion, Hor_Pac_Fecha_Cita, Hor_Pac_Inasistencia, H.Hor_Pac_Tipo_Serv " +
                                                        "FROM  CXN_HORARIO H " +
                                                        "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                                        "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                                        "WHERE H.Hor_Estado = 'A' " +
                                                        "AND H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(hasta).ToString(getData["Format_Fecha"]) + "' " +
                                                        "ORDER BY H.Hor_Pac_Fecha_Cita DESC", con);
                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("TIPO DOCUMENTO PACIENTE" + "," + "DOCUMENTO PACIENTE" + "," + "PACIENTE" + "," + "TELEFONO" + "," + "TELEFONO 2" + "," + "EMAIL" + "," + "ASEGURADORA" + "," + "MOTIVO INASISTENCIA" + "," + "FECHA INASISTENCIA" + "," + "ESPECIALIDAD");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read() == true)
                    {
                        string ESPE;
                        switch (leer["Hor_Pac_Tipo_Serv"].ToString())
                        {
                            case "CU":
                                ESPE = "Curaciones";
                                break;

                            case "MG":
                                ESPE = "Medicina General";
                                break;

                            case "FI":
                                ESPE = "Fisiatria";
                                break;

                            case "PS":
                                ESPE = "Psicologia";
                                break;

                            case "TO":
                                ESPE = "Terapia Ocupacional";
                                break;

                            case "TF":
                                ESPE = "Terapia Fisica";
                                break;

                            default:
                                ESPE = "Sin Datos";
                                break;
                        }

                        Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                        Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                        Escriba.Write(leer["Nombre"].ToString() + ",");
                        Escriba.Write(leer["Pac_Telefono"].ToString() + ",");
                        Escriba.Write(leer["Pac_TelefonoAux"].ToString() + ",");
                        Escriba.Write(leer["Pac_Email"].ToString() + ",");
                        Escriba.Write(leer["Ase_Descripcion"].ToString() + ",");
                        Escriba.Write(leer["Hor_Pac_Inasistencia"].ToString() + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"]).ToString(getData["Format_Fecha"]) + ",");
                        Escriba.Write(ESPE);
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();
                    MessageBox.Show("Generado en C CXN Reportes Inasistencias.txt");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IPlanos.ExpPlanoFacturacion(CXN_FACTURA F)
        {
            try
            {
                var getDta = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDta["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;
                    FileStream Querys = new FileStream("C:/Cxn/Reportes/Facturacion_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".csv", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Querys);
                    string Texto = "";

                    String Query = "SELECT Fac_Num_Fac, Homologo, Fac_Fecha " +
                                   "FROM CXN_FACTURA " +
                                   "WHERE Fac_Fecha BETWEEN @param1 AND @param2 " +
                                   "AND Fac_Ase = @param3 " +
                                   "AND Fac_Cia = @param4 " +
                                   "AND Fac_Estado = @param5";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = F.Fac_Fecha_Des;
                        Commando.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = F.Fac_Fecha_Has;
                        Commando.Parameters.AddWithValue("@param3", F.Fac_Ase);
                        Commando.Parameters.AddWithValue("@param4", F.Fac_Cia);
                        Commando.Parameters.AddWithValue("@param5", "F");

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {                                
                                Texto = "Factura Electronica|Fecha Factura|Fecha Radicacion|Valor Facturado|Valor Pagado|Retencion Fuente";

                                while (Reader.Read() == true)
                                {
                                    string Homologo = (Reader["Homologo"] != DBNull.Value ? Reader["Homologo"].ToString() : "NOT FOUND");


                                    String Query2 = "SELECT SUM(Car_Val_Tot) AS Valor " +
                                                    "FROM CXN_CARGOS " +
                                                    "WHERE Car_Factura = @param1 " +
                                                    "AND Car_Ase = @param2 " +
                                                    "AND Car_Cia = @param3 " +
                                                    "AND Car_Estado = @param4";

                                    using (SqlCommand Commando2 = new SqlCommand(Query2, con))
                                    {
                                        Commando2.Parameters.AddWithValue("@param1", Reader["Fac_Num_Fac"].ToString());
                                        Commando2.Parameters.AddWithValue("@param2", F.Fac_Ase);
                                        Commando2.Parameters.AddWithValue("@param3", F.Fac_Cia);
                                        Commando2.Parameters.AddWithValue("@param4", "F");

                                        using (SqlDataReader Reader2 = (Commando2.ExecuteReader()))
                                        {
                                            if (Reader2.Read() == true)
                                            {
                                                int vrfacturado = (Reader2["Valor"] != DBNull.Value ? Convert.ToInt32(Reader2["Valor"]) : 0);

                                                String Query3 = "SELECT * " +
                                                                "FROM CXN_PAGOS " +
                                                                "WHERE Homologo = @param1 " +
                                                                "AND Cia = @param2";

                                                using (SqlCommand Commando3 = new SqlCommand(Query3, con))
                                                {
                                                    Commando3.Parameters.AddWithValue("@param1", Homologo);
                                                    Commando3.Parameters.AddWithValue("@param2", F.Fac_Cia);

                                                    using (SqlDataReader Reader3 = (Commando3.ExecuteReader()))
                                                    {
                                                        if (Reader3.Read() == true)
                                                        {
                                                            string fradica = (Reader3["FRadica"] != DBNull.Value ? Convert.ToDateTime(Reader3["FRadica"]).ToString(Conexion.ConectionDictionary["Format_Fecha"]) : "N/A");
                                                            int vrpagado = (Reader3["VrPagado"] != DBNull.Value ? Convert.ToInt32(Reader3["VrPagado"]) : 0);
                                                            decimal retencion = (Reader3["RT"] != DBNull.Value ? Convert.ToDecimal(Reader3["RT"]) : 0);

                                                            Texto = Texto + "\r" +
                                                                            Homologo + "|" +
                                                                            Convert.ToDateTime(Reader["Fac_Fecha"]).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "|" +
                                                                            fradica + "|" +
                                                                            vrfacturado.ToString() + "|" +
                                                                            vrpagado.ToString() + "|" + 
                                                                            retencion;
                                                        }
                                                        else
                                                        {
                                                            Texto = Texto + "\r" +
                                                                            Homologo + "|" +
                                                                            Convert.ToDateTime(Reader["Fac_Fecha"]).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "|" +
                                                                            "|" +
                                                                            "|" +
                                                                            "";
                                                        }
                                                    }
                                                }                                                    
                                            }
                                            else
                                            {
                                                Texto = Texto + "\r" +
                                                      Homologo + "|" +
                                                      Convert.ToDateTime(Reader["Fac_Fecha"]).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "|" +
                                                      "|" +
                                                      "|" +
                                                      "";
                                            }                                        
                                        }
                                    }
                                }

                                Escriba.Write(Texto);
                                Escriba.WriteLine();
                                Escriba.Flush();
                                Escriba.Close();

                                MessageBox.Show("Generado");
                            }
                            else
                            {
                                MessageBox.Show("No hay datos");
                            }
                        }
                    }                      
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IPlanos.ExpPlanoOrdenesPendientes(DateTime Desde, DateTime Hasta)
        {
            try
            {
                var getDta = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDta["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;

                    FileStream Query = new FileStream("C:/Cxn/Reportes/OrdenesP_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".csv", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);
                    SqlCommand comando = new SqlCommand("SELECT O.OP_Adm, O.OP_Estado, O.OP_Registra, O.OP_Cambia, O.OP_EstadoChange, H.Hor_Imp_Age, H.Hor_Pac_Fecha_Cita " +
                                         " FROM CXN_OPEND O " +
                                         " INNER JOIN CXN_HORARIO H ON O.OP_Adm = H.Hor_Id " +
                                         " WHERE H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getDta["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getDta["Format_Fecha"]) + "' " +
                                         " ORDER BY H.Hor_Pac_Fecha_Cita ASC", con);
                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("ADMISION" + "," + "ESTADO" + "," + "FECHA REGISTRO" + "," + "USUARIO QUE REGISTRA" + "," + "PACIENTE" + "," + "FECHA DE CAMBIO" + "," + "USUARIO QUE CAMBIA");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read())
                    {
                        string Est;

                        switch (leer["OP_Estado"].ToString())
                        {
                            case "P":
                                Est = "Pendiente Consumo";
                                break;

                            case "H":
                                Est = "Consumo Realizado";
                                break;

                            case "E":
                                Est = "Registro Eliminado";
                                break;

                            default:
                                Est = "ERROR";
                                break;

                        }

                        DateTime FechaFalsa = new DateTime(1900, 01, 01);

                        DateTime FechaImprime = (leer["OP_EstadoChange"] == DBNull.Value ? FechaFalsa : Convert.ToDateTime(leer["OP_EstadoChange"]));

                        Escriba.Write(leer["OP_Adm"].ToString() + ",");
                        Escriba.Write(Est.ToString() + ",");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"]).ToString(getDta["Format_Fecha"]) + ",");
                        Escriba.Write(leer["OP_Registra"].ToString() + ",");
                        Escriba.Write(leer["Hor_Imp_Age"].ToString() + ",");
                        Escriba.Write(Convert.ToDateTime(FechaImprime).ToString(getDta["Format_Fecha"]) + ",");
                        Escriba.Write(leer["OP_Cambia"].ToString());
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();

                    MessageBox.Show("Generado");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "AP");
            }
        }
    }
}
