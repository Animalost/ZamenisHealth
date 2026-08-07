using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Persistence.CXN.Metodos
{
    public class MOrdenes : IOrdenes
    {
        private readonly static IGenerales repositorioLogin = new MGenerales();

        void IOrdenes.Rpt_Ordenes(DateTime Desde, DateTime Hasta)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now;

                FileStream Query = new FileStream("C:/Cxn/Reportes/Ordenes_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".csv", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);

                SqlCommand comando = new SqlCommand("SELECT O.OM_Fecha, P.Pac_TipoId, P.Pac_IdNum, " +
                                                    "P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + " +
                                                    "P.Pac_SegundoA AS Nombre, P.Pac_Telefono, O.OM_DX1, " +
                                                    "O.OM_Desc, O.OM_Prof " +
                                                    "FROM CXN_OM O INNER JOIN " +
                                                    "CXN_PACIENTES P ON O.OM_Pac = P.Pac_Id " +
                                                    "WHERE O.OM_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getCon["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getCon["Format_Fecha"]) + "' " +
                                                    "AND O.OM_Planillar = 'S'", con);

                SqlDataReader leer;
                leer = comando.ExecuteReader();

                Escriba.Write("FECHA" + "|" + "TIPO ID" + "|" + "IDENTIFICACION" + "|" + "PACIENTE" + "|" + "CELULAR" + "|" + "DX" + "|" + "DESCRIPCION" + "|" + "PROFESIONAL");
                Escriba.WriteLine();
                Escriba.Flush();

                while (leer.Read())
                {
                    string D = leer["OM_Desc"].ToString();
                    string reemplazo = System.Text.RegularExpressions.Regex.Replace(D, @"\t|\n|\r", " ");
                    var DD = System.Text.RegularExpressions.Regex.Replace(reemplazo, @",", "|");

                    Escriba.Write(Convert.ToDateTime(leer["OM_Fecha"].ToString()).ToString("dd/MM/yyyy") + "|");
                    Escriba.Write(leer["Pac_TipoId"].ToString() + "|");
                    Escriba.Write(leer["Pac_IdNum"].ToString() + "|");
                    Escriba.Write(leer["Nombre"].ToString() + "|");
                    Escriba.Write(leer["Pac_Telefono"].ToString() + "|");
                    Escriba.Write(leer["OM_DX1"].ToString() + "|");
                    Escriba.Write(reemplazo.ToString() + "|");
                    Escriba.Write(leer["OM_Prof"].ToString());
                    Escriba.WriteLine();
                    Escriba.Flush();
                }
                Escriba.Close();
            }
        }
        void Actualiza_Datos_Paciente(string Dir, string Tel, string Genero, int Pac)
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

                    string Sex = "M";

                    switch (Genero)
                    {
                        case "Masculino":
                            Sex = "M";
                            break;

                        case "Femenino":
                            Sex = "F";
                            break;

                        default:
                            Sex = "M";
                            break;
                    }

                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_Direccion = '" + Dir + "', " +
                                      "Pac_Telefono = '" + Tel + "', " +
                                      "Pac_Sexo = '" + Sex + "' " +
                                      "WHERE Pac_Id = '" + Pac + "'";
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
        bool IOrdenes.CrearOrden(CXN_OM OM)
        {
            try
            {
                var getCon = Conexion.Conection();

                Actualiza_Datos_Paciente(OM.OM_Direccion, OM.OM_Telefono, OM.OM_Genero, OM.OM_Pac);

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Pla = (OM.OM_Planillar == "S" ? "S" : "");

                    DateTime OM_Fecha = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_OM (OM_Pac, " + //param1
                                                           "OM_Ase, " + //param2
                                                           "OM_Cia, " + //param3
                                                           "OM_Prof, " + //param4
                                                           "OM_Fecha, " + //param5
                                                           "OM_DX1, " + //param5
                                                           "OM_DX1T, " + //param5
                                                           "OM_DX2, " + //param5
                                                           "OM_DX2T, " + //param5
                                                           "OM_DX3, " + //param5
                                                           "OM_DX3T, " + //param5
                                                           "OM_Edad, " + //param5
                                                           "OM_Genero, " + //param5
                                                           "OM_Direccion, " + //param5
                                                           "OM_Tipo, " + //param5
                                                           "OM_Telefono, " + //param5
                                                           "OM_Num, " + //param5
                                                           "OM_Firma, " + //param5
                                                           "OM_Desc, " +
                                                           "OM_TEspecialidad, " +
                                                           "OM_Planillar, " +
                                                           "OM_Radicada, " +
                                                           "OM_Autorizacion, " +
                                                           "OM_Consumida, " +
                                                           "OM_Clasificacion, " +
                                                           "OM_FHIR_INC, " +
                                                           "OM_Dias, " +
                                                           "OM_Bilateral) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Cia
                                                          "@param7, " + // Hor_Pac_Cia
                                                          "@param8, " + // Hor_Pac_Cia
                                                          "@param9, " + // Hor_Pac_Cia
                                                          "@param10, " + // Hor_Pac_Cia
                                                          "@param11, " + // Hor_Pac_Cia
                                                          "@param12, " + // Hor_Pac_Cia
                                                          "@param13, " + // Hor_Pac_Cia
                                                          "@param14, " + // Hor_Pac_Cia
                                                          "@param15, " + // Hor_Pac_Cia
                                                          "@param16, " + // Hor_Pac_Cia
                                                          "@param17, " + // Hor_Pac_Cia
                                                          "@param18, " + // Hor_Pac_Cia
                                                          "@param19, " +
                                                          "@param20, " +
                                                          "@param21, " +
                                                          "@param22, " +
                                                          "@param23, " +
                                                          "@param24, " +
                                                          "@param25, " +
                                                          "@param26, " +
                                                          "@param27, " +
                                                          "@param28)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", OM.OM_Pac);
                    cmd.Parameters.AddWithValue("@param2", OM.OM_Ase);
                    cmd.Parameters.AddWithValue("@param3", OM.OM_Cia);
                    cmd.Parameters.AddWithValue("@param4", OM.OM_Prof);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = OM_Fecha; // Fecha que graba cita
                    cmd.Parameters.AddWithValue("@param6", OM.OM_DX1);
                    cmd.Parameters.AddWithValue("@param7", OM.OM_DX1T);
                    cmd.Parameters.AddWithValue("@param8", OM.OM_DX2);
                    cmd.Parameters.AddWithValue("@param9", OM.OM_DX2T);
                    cmd.Parameters.AddWithValue("@param10", OM.OM_DX3);
                    cmd.Parameters.AddWithValue("@param11", OM.OM_DX3T);
                    cmd.Parameters.AddWithValue("@param12", OM.OM_Edad);
                    cmd.Parameters.AddWithValue("@param13", OM.OM_Genero);
                    cmd.Parameters.AddWithValue("@param14", OM.OM_Direccion);
                    cmd.Parameters.AddWithValue("@param15", "S");
                    cmd.Parameters.AddWithValue("@param16", OM.OM_Telefono);
                    cmd.Parameters.AddWithValue("@param17", OM.OM_Num);
                    cmd.Parameters.AddWithValue("@param18", OM.OM_Firma);
                    cmd.Parameters.AddWithValue("@param19", OM.OM_Desc);
                    cmd.Parameters.AddWithValue("@param20", OM.OM_TEspecialidad);
                    cmd.Parameters.AddWithValue("@param21", Pla.ToString());
                    cmd.Parameters.AddWithValue("@param22", "N");
                    cmd.Parameters.AddWithValue("@param23", "");
                    cmd.Parameters.AddWithValue("@param24", "N");
                    cmd.Parameters.AddWithValue("@param25", OM.OM_Clasificacion);
                    cmd.Parameters.AddWithValue("@param26", OM.OM_FHIR_INC);
                    cmd.Parameters.AddWithValue("@param27", OM.OM_Dias);
                    cmd.Parameters.AddWithValue("@param28", OM.OM_Bilateral);
                    int s = cmd.ExecuteNonQuery();
                    if (s > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IOrdenes.CrearOrdenFHIR(CXN_ORDENESFHIR OM)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ORDENESFHIR (Admision, " + //param1
                                                           "CUP, " + //param2
                                                           "Servicio, " + //param3
                                                           "Tipo, " + //param4
                                                           "Paciente, " + //param5
                                                           "DX1, " + //param5
                                                           "DX2, " + //param5
                                                           "DX3, " + //param5
                                                           "Medico) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Cia
                                                          "@param7, " + // Hor_Pac_Cia
                                                          "@param8, " + // Hor_Pac_Cia
                                                          "@param9)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", OM.Admision);
                    cmd.Parameters.AddWithValue("@param2", OM.CUP);
                    cmd.Parameters.AddWithValue("@param3", OM.Servicio);
                    cmd.Parameters.AddWithValue("@param4", OM.Tipo);
                    cmd.Parameters.AddWithValue("@param5", OM.Paciente); 
                    cmd.Parameters.AddWithValue("@param6", OM.DX1);
                    cmd.Parameters.AddWithValue("@param7", OM.DX2);
                    cmd.Parameters.AddWithValue("@param8", OM.DX3);
                    cmd.Parameters.AddWithValue("@param9", OM.Medico);
                    
                    int s = cmd.ExecuteNonQuery();
                    if (s > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IOrdenes.CrearOrdenFHIRMED(CXN_ORDENESFHIR OM)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ORDENESFHIR (Admision, " + //param1
                                                           "CUP, " + //param2
                                                           "Servicio, " + //param3
                                                           "Tipo, " + //param4
                                                           "Paciente, " + //param5
                                                           "DX1, " + //param5
                                                           "DX2, " + //param5
                                                           "DX3, " + //param5
                                                           "Medico, " +
                                                           "Medicamento, " +
                                                           "CodMedicamento, " +
                                                           "Via, " +
                                                           "Cada, " +
                                                           "FrecAdmi, " +
                                                           "Cantidad, " +
                                                           "UMM, " +
                                                           "Duracion, " +
                                                           "Tiempo, " +
                                                           "TipoTecnologia, " +
                                                           "Observacion) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Cia
                                                          "@param7, " + // Hor_Pac_Cia
                                                          "@param8, " + // Hor_Pac_Cia
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
                                                          "@param20)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", OM.Admision);
                    cmd.Parameters.AddWithValue("@param2", OM.CUP);
                    cmd.Parameters.AddWithValue("@param3", OM.Servicio);
                    cmd.Parameters.AddWithValue("@param4", OM.Tipo);
                    cmd.Parameters.AddWithValue("@param5", OM.Paciente);
                    cmd.Parameters.AddWithValue("@param6", OM.DX1);
                    cmd.Parameters.AddWithValue("@param7", OM.DX2);
                    cmd.Parameters.AddWithValue("@param8", OM.DX3);
                    cmd.Parameters.AddWithValue("@param9", OM.Medico);

                    cmd.Parameters.AddWithValue("@param10", OM.Medicamento);
                    cmd.Parameters.AddWithValue("@param11", OM.CodMedicamento);
                    cmd.Parameters.AddWithValue("@param12", OM.Via);
                    cmd.Parameters.AddWithValue("@param13", OM.Cada);
                    cmd.Parameters.AddWithValue("@param14", OM.FrecAdmi);
                    cmd.Parameters.AddWithValue("@param15", OM.Cantidad);
                    cmd.Parameters.AddWithValue("@param16", OM.UMM);
                    cmd.Parameters.AddWithValue("@param17", OM.Duracion);
                    cmd.Parameters.AddWithValue("@param18", OM.Tiempo);
                    cmd.Parameters.AddWithValue("@param19", OM.TipoTecnologia);
                    cmd.Parameters.AddWithValue("@param20", OM.Observacion);

                    int s = cmd.ExecuteNonQuery();
                    if (s > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IOrdenes.CrearOrdenM(CXN_OM OM)
        {
            try
            {
                var getCon = Conexion.Conection();

                Actualiza_Datos_Paciente(OM.OM_Direccion, OM.OM_Telefono, OM.OM_Genero, OM.OM_Pac);

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_OM (OM_Duracion, " +
                                                              "OM_Cantidad, " +
                                                              "OM_Via, " +
                                                              "OM_Presentacion, " +
                                                              "OM_Detalle, " +
                                                              "OM_Firma, " +
                                                              "OM_Medicamento, " +
                                                              "OM_Direccion, " +
                                                              "OM_Telefono, " +
                                                              "OM_Genero, " +
                                                              "OM_Tecnologia, " +
                                                              "OM_Pac, " +
                                                              "OM_Ase, " +
                                                              "OM_Cia, " +
                                                              "OM_Prof, " +
                                                              "OM_Desc, " +
                                                              "OM_DX1, " +
                                                              "OM_DX2, " +
                                                              "OM_DX3, " +
                                                              "OM_DX1T, " +
                                                              "OM_DX2T, " +
                                                              "OM_DX3T, " +
                                                              "OM_Edad, " +
                                                              "OM_Num, " +
                                                              "OM_TEspecialidad, " +
                                                              "OM_Clasificacion, " +
                                                              "OM_Fecha, " +
                                                              "OM_Tipo, " +
                                                              "OM_Bilateral, " +
                                                              "OM_Cada, " +
                                                              "OM_Posologia, " +
                                                              "OM_CantidadMedicamento) " +
                                     "values                  (@param1, " +
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
                                                              "@param29, " +
                                                              "@param30, " +
                                                              "@param31, " +
                                                              "@param32)", con);

                    cmd.Parameters.AddWithValue("@param1", OM.OM_Duracion);
                    cmd.Parameters.AddWithValue("@param2", OM.OM_Cantidad);
                    cmd.Parameters.AddWithValue("@param3", OM.OM_Via);
                    cmd.Parameters.AddWithValue("@param4", OM.OM_Presentacion);
                    cmd.Parameters.AddWithValue("@param5", OM.OM_Detalle);
                    cmd.Parameters.AddWithValue("@param6", OM.OM_Firma);                                        
                    cmd.Parameters.AddWithValue("@param7", OM.OM_Medicamento);
                    cmd.Parameters.AddWithValue("@param8", OM.OM_Direccion);
                    cmd.Parameters.AddWithValue("@param9", OM.OM_Telefono); //usuario en este caso
                    cmd.Parameters.AddWithValue("@param10", OM.OM_Genero);
                    cmd.Parameters.AddWithValue("@param11", OM.OM_Tecnologia);
                    cmd.Parameters.AddWithValue("@param12", OM.OM_Pac);
                    cmd.Parameters.AddWithValue("@param13", OM.OM_Ase);
                    cmd.Parameters.AddWithValue("@param14", OM.OM_Cia);
                    cmd.Parameters.AddWithValue("@param15", OM.OM_Prof);
                    cmd.Parameters.AddWithValue("@param16", OM.OM_Desc);
                    cmd.Parameters.AddWithValue("@param17", OM.OM_DX1);
                    cmd.Parameters.AddWithValue("@param18", OM.OM_DX2);
                    cmd.Parameters.AddWithValue("@param19", OM.OM_DX3);
                    cmd.Parameters.AddWithValue("@param20", OM.OM_DX1T);
                    cmd.Parameters.AddWithValue("@param21", OM.OM_DX2T);
                    cmd.Parameters.AddWithValue("@param22", OM.OM_DX3T);
                    cmd.Parameters.AddWithValue("@param23", OM.OM_Edad);
                    cmd.Parameters.AddWithValue("@param24", OM.OM_Num);
                    cmd.Parameters.AddWithValue("@param25", OM.OM_TEspecialidad);
                    cmd.Parameters.AddWithValue("@param26", OM.OM_Clasificacion);
                    cmd.Parameters.AddWithValue("@param27", Convert.ToDateTime(DateTime.Now.Date));
                    cmd.Parameters.AddWithValue("@param28", OM.OM_Tipo);
                    cmd.Parameters.AddWithValue("@param29", OM.OM_Bilateral);
                    cmd.Parameters.AddWithValue("@param30", OM.OM_Cada);
                    cmd.Parameters.AddWithValue("@param31", OM.OM_Posologia);
                    cmd.Parameters.AddWithValue("@param32", OM.OM_CantidadMedicamento);
                    int s = cmd.ExecuteNonQuery();
                    if (s > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        List<Ordenes> IOrdenes.Generar_OrdenMedica(int Numero, int Compañia, string UserImprime)
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
                    String Cargar_Hora = "SELECT  Com_Nombre, Com_Identificacion, Com_Telefono, Com_Direccion, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, " +
                                         "Pac_SegundoA, OM_Num, OM_Fecha, Pac_TipoId, Pac_IdNum, Ase_Descripcion, Pac_Telefono, Bod_Responsable, OM_Desc, Com_Logo, " +
                                         "OM_DX1, OM_DX2, OM_DX3, OM_DX1T, OM_DX2T, OM_DX3T, OM_Edad, OM_Genero, OM_Direccion, OM_Telefono, OM_Firma, Bod_Firma, Bod_Reg_Med, Pac_Id, OM_Clasificacion, " +
                                         "OM_FHIR_INC, OM_Dias, OM_Bilateral " +
                                         "FROM CXN_OM " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_OM.OM_Pac = CXN_PACIENTES.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_OM.OM_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_OM.OM_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_OM.OM_Prof = CXN_BODEGAS.Bod_Usuario " +
                                         "WHERE CXN_OM.OM_Num = '" + Numero + "' " +
                                         "AND OM_Cia = '" + Compañia + "'";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<Ordenes> export_om_report = new List<Ordenes>();

                                while (Lectura_Hora.Read() == true)
                                {                                    
                                    //GENERADOR QR
                                    string Text_Codifica = "* PACIENTE: " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() + "\n" +
                                                           "* FECHA: " + Convert.ToDateTime(Lectura_Hora["OM_Fecha"]).ToString(getCon["Format_Fecha"]) + "\n" +
                                                           "* ORDEN: " + Lectura_Hora["OM_Num"].ToString();
                                    var ImaRes = repositorioLogin.CodifyQR(Text_Codifica);
                                    //TERMINA GENERADOR QR                        

                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString();
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    PictureBox picFirmaDigital = new PictureBox();

                                    if (Lectura_Hora["OM_Firma"].ToString() == "SI")
                                    {
                                        string FirmaDigital = Lectura_Hora["Bod_Firma"].ToString();
                                        Byte[] bytesFirmaDigital = Convert.FromBase64String(FirmaDigital);
                                        MemoryStream stmBLOBDataFirmaDigital = new MemoryStream(bytesFirmaDigital);
                                        picFirmaDigital.Image = Image.FromStream(stmBLOBDataFirmaDigital);
                                    }
                                    else
                                    {
                                        string FirmaDigital = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KC" +
                                        "gsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwM" +
                                        "DAwMDAwMDAwMDAz/wAARCAAlAD0DASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAw" +
                                        "UFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV" +
                                        "1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5uf" +
                                        "o6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQ" +
                                        "dhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOE" +
                                        "hYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAx" +
                                        "EAPwD9/KKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooA//Z";
                                        Byte[] bytesFirmaDigital = Convert.FromBase64String(FirmaDigital);
                                        MemoryStream stmBLOBDataFirmaDigital = new MemoryStream(bytesFirmaDigital);
                                        picFirmaDigital.Image = Image.FromStream(stmBLOBDataFirmaDigital);
                                    }

                                    string des = Lectura_Hora["OM_Bilateral"] == DBNull.Value ? "" :
                                                 Lectura_Hora["OM_Bilateral"].ToString() == "" ? "" :
                                                 Lectura_Hora["OM_Bilateral"].ToString() == "S" ? "  - BILATERAL" : "";

                                    export_om_report.Add(new Ordenes
                                    {
                                        RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),
                                        Firma = repositorioLogin.GetBytes(picFirmaDigital.Image),
                                        EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                        PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        Admision = Convert.ToInt32(Lectura_Hora["OM_Num"]),
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["OM_Fecha"]),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        PacienteTelefono = Lectura_Hora["OM_Telefono"].ToString(),
                                        Logo = repositorioLogin.GetBytes(pic.Image),
                                        QR = repositorioLogin.GetBytes(ImaRes),
                                        Descripcion = Lectura_Hora["OM_Desc"].ToString() + des,
                                        OM_DX1 = "Diagnostico Principal: " +
                                                                                  Lectura_Hora["OM_DX1"].ToString() +
                                                                                  " - " + Lectura_Hora["OM_DX1T"].ToString() + "\n\r" +
                                                                                  "Rel 1: " +
                                                                                  Lectura_Hora["OM_DX2"].ToString() + " - " +
                                                                                  Lectura_Hora["OM_DX2T"].ToString() + "\n\r" +
                                                                                  "Rel 2: " +
                                                                                  Lectura_Hora["OM_DX3"].ToString() + " - " +
                                                                                  Lectura_Hora["OM_DX3T"].ToString(),
                                        OM_DX2 = Lectura_Hora["OM_DX2"].ToString(),
                                        OM_DX3 = Lectura_Hora["OM_DX3"].ToString(),
                                        OM_DX1T = Lectura_Hora["OM_DX1T"].ToString(),
                                        OM_DX2T = Lectura_Hora["OM_DX2T"].ToString(),
                                        OM_DX3T = Lectura_Hora["OM_DX3T"].ToString(),
                                        Edad = Lectura_Hora["OM_Edad"].ToString(),
                                        Genero = Lectura_Hora["OM_Genero"].ToString(),
                                        PacienteDireccion = Lectura_Hora["OM_Direccion"].ToString(),
                                        ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                                        OM_Detalle = UserImprime.ToString(),
                                        OM_Cantidad = Lectura_Hora["Pac_Id"].ToString(),
                                        OM_Clasificacion = (Lectura_Hora["OM_Clasificacion"] == DBNull.Value ? "ORDEN MEDICA" : Lectura_Hora["OM_Clasificacion"].ToString()),
                                        OM_FHIR = (Lectura_Hora["OM_Clasificacion"] == DBNull.Value ? ""
                                                                                     : Lectura_Hora["OM_Clasificacion"].ToString() == "INCAPACIDAD MEDICA"
                                                                                     ? Lectura_Hora["OM_FHIR_INC"] == DBNull.Value ? ""
                                                                                     : Lectura_Hora["OM_FHIR_INC"].ToString() : ""),
                                        OM_Dias = Lectura_Hora["OM_Dias"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["OM_Dias"])
                                    });
                                }
                                
                                return export_om_report;
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
        int IOrdenes.getIdOMFHIR(int Paciente, DateTime Fecha)
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

                    String Query = "SELECT TOP 1 OM_Num " +
                                   "FROM CXN_OM " +
                                   "WHERE OM_Fecha = @param1 " +
                                   "AND OM_Clasificacion = @param2 " +
                                   "AND OM_Pac = @param3 " +
                                   "ORDER BY OM_Id DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Convert.ToDateTime(Fecha.Date));
                        Commando.Parameters.AddWithValue("@param2", "INCAPACIDAD MEDICA");
                        Commando.Parameters.AddWithValue("@param3", Paciente);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Convert.ToInt32(Reader["Om_Num"]);
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
        int IOrdenes.getIdOMMEDFHIR(int Paciente, DateTime Fecha)
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

                    String Query = "SELECT TOP 1 OM_Num " +
                                   "FROM CXN_OM " +
                                   "WHERE OM_Fecha = @param1 " +
                                   "AND OM_Clasificacion = @param2 " +
                                   "AND OM_Pac = @param3 " +
                                   "ORDER BY OM_Id DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Convert.ToDateTime(Fecha.Date));
                        Commando.Parameters.AddWithValue("@param2", "RECETA MEDICA");
                        Commando.Parameters.AddWithValue("@param3", Paciente);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Convert.ToInt32(Reader["Om_Num"]);
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
        List<string> IOrdenes.GetUMM()
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
                                   "FROM CXN_UMM " +
                                   "ORDER BY UnidadMedida ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            List<string> ords = new List<string>();

                            if (Reader.HasRows)
                            {
                                while (Reader.Read() == true)
                                {
                                    ords.Add(Reader["UnidadMedida"].ToString());
                                }

                                return ords;
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
        string IOrdenes.GetUMMCode(string UMM)
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
                                   "FROM CXN_UMM " +
                                   "WHERE UnidadMedida = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", UMM);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }
        List<CXN_ORDENESFHIR> IOrdenes.getOrdenesAdmition(int Admision)
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
                                   "FROM CXN_ORDENESFHIR " +
                                   "WHERE Admision = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            List<CXN_ORDENESFHIR> ords = new List<CXN_ORDENESFHIR>();

                            if (Reader.HasRows)
                            {
                                while (Reader.Read() == true)
                                {
                                    ords.Add(new CXN_ORDENESFHIR 
                                    { 
                                        Admision = Admision,
                                        CUP = Reader["CUP"].ToString(),
                                        DX1 = Reader["DX1"].ToString(),
                                        DX2 = Reader["DX2"].ToString(),
                                        DX3 = Reader["DX3"].ToString(),
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Medico = Reader["Medico"].ToString(),
                                        Paciente = Convert.ToInt32(Reader["Paciente"]),
                                        Servicio = Reader["Servicio"].ToString(),
                                        Tipo = Reader["Tipo"].ToString(),
                                        
                                        Cada = Reader["Cada"].ToString(),
                                        Cantidad = Reader["Cantidad"].ToString(),
                                        CodMedicamento = Reader["CodMedicamento"].ToString(),
                                        Duracion = Reader["Duracion"].ToString(),
                                        Medicamento = Reader["Medicamento"].ToString(),
                                        Observacion = Reader["Observacion"].ToString(),
                                        Tiempo = Reader["Tiempo"].ToString(),
                                        TipoTecnologia = Reader["TipoTecnologia"].ToString(),
                                        UMM = Reader["UMM"].ToString(),
                                        FrecAdmi = Reader["FrecAdmi"].ToString(),
                                        Via = Reader["Via"].ToString()
                                    });
                                }

                                return ords;
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
        List<Ordenes> IOrdenes.Genera_Orden_Medicamento(int Numero, int Compañia, string UserPrint)
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
                    String Cargar_Hora = "SELECT Com_Nombre, Com_Identificacion, Com_Telefono, Com_Direccion, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, " +
                                         "Pac_SegundoA, OM_Num, OM_Fecha, Pac_TipoId, Pac_IdNum, Ase_Descripcion, Pac_Telefono, Bod_Responsable, OM_Desc, Com_Logo, " +
                                         "OM_DX1, OM_DX2, OM_DX3, OM_DX1T, OM_DX2T, OM_DX3T, OM_Edad, OM_Genero, OM_Direccion, OM_Telefono, OM_Duracion, " +
                                         "OM_Cantidad, OM_Via, OM_Medicamento, OM_Presentacion, OM_Detalle, OM_Firma, Bod_Firma, Bod_Reg_Med, OM_Tecnologia, OM_Cada, OM_Posologia, OM_CantidadMedicamento " +
                                         "FROM CXN_OM " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_OM.OM_Pac = CXN_PACIENTES.Pac_Id " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_OM.OM_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA ON CXN_OM.OM_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_OM.OM_Prof = CXN_BODEGAS.Bod_Usuario " +
                                         "WHERE CXN_OM.OM_Num = @param1 " +
                                         "AND CXN_OM.OM_Cia = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Numero);
                        Carga_Command.Parameters.AddWithValue("@param2", Compañia);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<Ordenes> export_om_report = new List<Ordenes>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    //GENERADOR QR
                                    string Text_Codifica = "PACIENTE: " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() + "\n" +
                                                           "FECHA: " + Convert.ToDateTime(Lectura_Hora["OM_Fecha"]).ToString(getCon["Format_Fecha"]) + "\n" +
                                                           "ORDEN DE MEDICAMENTOS NUMERO: " + Lectura_Hora["OM_Num"].ToString() + "\n" +
                                                           "FORMULA MEDICA";
                                    var ImaRes = repositorioLogin.CodifyQR(Text_Codifica);
                                    //TERMINA GENERADOR QR                        

                                    string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString();
                                    Byte[] bytes = Convert.FromBase64String(Bod_Firma1);
                                    MemoryStream stmBLOBData = new MemoryStream(bytes);
                                    PictureBox pic = new PictureBox();
                                    pic.Image = Image.FromStream(stmBLOBData);

                                    PictureBox picFirmaDigital = new PictureBox();

                                    if (Lectura_Hora["OM_Firma"].ToString() == "SI")
                                    {
                                        string FirmaDigital = Lectura_Hora["Bod_Firma"].ToString();
                                        Byte[] bytesFirmaDigital = Convert.FromBase64String(FirmaDigital);
                                        MemoryStream stmBLOBDataFirmaDigital = new MemoryStream(bytesFirmaDigital);
                                        picFirmaDigital.Image = Image.FromStream(stmBLOBDataFirmaDigital);
                                    }
                                    else
                                    {
                                        string FirmaDigital = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KC" +
                                        "gsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwM" +
                                        "DAwMDAwMDAwMDAz/wAARCAAlAD0DASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAw" +
                                        "UFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV" +
                                        "1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5uf" +
                                        "o6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQ" +
                                        "dhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOE" +
                                        "hYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAx" +
                                        "EAPwD9/KKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooA//Z";
                                        Byte[] bytesFirmaDigital = Convert.FromBase64String(FirmaDigital);
                                        MemoryStream stmBLOBDataFirmaDigital = new MemoryStream(bytesFirmaDigital);
                                        picFirmaDigital.Image = Image.FromStream(stmBLOBDataFirmaDigital);
                                    }

                                    export_om_report.Add(new Ordenes
                                    {
                                        RegistroMedico = Lectura_Hora["Bod_Reg_Med"].ToString(),
                                        Firma = repositorioLogin.GetBytes(picFirmaDigital.Image),
                                        EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                                        EmpresaIdentificacion = Lectura_Hora["Com_Identificacion"].ToString(),
                                        EmpresaTelefono = Lectura_Hora["Com_Telefono"].ToString(),
                                        EmpresaDireccion = Lectura_Hora["Com_Direccion"].ToString(),
                                        PacienteNombre = Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString() + " " + Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString(),
                                        PacienteIdentificacion = Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(),
                                        Admision = Convert.ToInt32(Lectura_Hora["OM_Num"]),
                                        FechaBase = Convert.ToDateTime(Lectura_Hora["OM_Fecha"]),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        PacienteTelefono = Lectura_Hora["OM_Telefono"].ToString(),
                                        Logo = repositorioLogin.GetBytes(pic.Image),
                                        QR = repositorioLogin.GetBytes(ImaRes),
                                        Descripcion = Lectura_Hora["OM_Desc"].ToString(),
                                        OM_DX1 = "Diagnostico Principal: " + Lectura_Hora["OM_DX1"].ToString() + " - " + Lectura_Hora["OM_DX1T"].ToString() + "\n" +
                                                 "Rel 1: " + Lectura_Hora["OM_DX2"].ToString() + " - " + Lectura_Hora["OM_DX2T"].ToString() + "\n" +
                                                 "Rel 2: " + Lectura_Hora["OM_DX3"].ToString() + " - " + Lectura_Hora["OM_DX3T"].ToString(),
                                        OM_DX2 = Lectura_Hora["OM_DX2"].ToString(),
                                        OM_DX3 = Lectura_Hora["OM_DX3"].ToString(),
                                        OM_DX1T = Lectura_Hora["OM_DX1T"].ToString(),
                                        OM_DX2T = Lectura_Hora["OM_DX2T"].ToString(),
                                        OM_DX3T = Lectura_Hora["OM_DX3T"].ToString(),
                                        Edad = Lectura_Hora["OM_Edad"].ToString(),
                                        Genero = Lectura_Hora["OM_Genero"].ToString(),
                                        PacienteDireccion = Lectura_Hora["OM_Direccion"].ToString(),
                                        ProfesionalNombre = Lectura_Hora["Bod_Responsable"].ToString(),
                                        OM_Duracion = Lectura_Hora["OM_Duracion"].ToString(),
                                        OM_Cantidad = Lectura_Hora["OM_Cantidad"].ToString(),
                                        OM_Via = Lectura_Hora["OM_Via"].ToString(),
                                        OM_Medicamento = Lectura_Hora["OM_Medicamento"].ToString(),
                                        OM_Presentacion = Lectura_Hora["OM_Presentacion"].ToString(),
                                        OM_Detalle = Lectura_Hora["OM_Detalle"].ToString(),
                                        Com_Direccion = UserPrint,
                                        OM_Tecnologia = Lectura_Hora["OM_Tecnologia"].ToString(),
                                        OM_Cada = Lectura_Hora["OM_Cada"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["OM_Cada"]),
                                        OM_Posologia = Lectura_Hora["OM_Posologia"] == DBNull.Value ? "" : Lectura_Hora["OM_Posologia"].ToString(),
                                        OM_CantidadMedicamento = Lectura_Hora["OM_CantidadMedicamento"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["OM_CantidadMedicamento"]),
                                    });
                                }
                                return export_om_report;
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
        List<CXN_OM> IOrdenes.getOrdenes(string TID, string NID, int Cia)
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

                    String Cargar_Hora = "SELECT DISTINCT O.OM_Tipo, O.OM_Num, O.OM_Fecha, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, B.Bod_Responsable, O.OM_TEspecialidad, O.OM_Cia, O.OM_Planillar, " +
                        "O.OM_Radicada, O.OM_Autorizacion, O.OM_Consumida, OM_Clasificacion " +
                                         "FROM CXN_PACIENTES P " +
                                         "INNER JOIN CXN_OM O ON P.Pac_Id = O.OM_Pac " +
                                         "INNER JOIN CXN_BODEGAS B ON O.OM_Prof = B.Bod_Usuario " +
                                         "WHERE P.Pac_TipoId = '" + TID + "' " +
                                         "AND P.Pac_IdNum = '" + NID + "' " +
                                         "AND O.OM_Cia = '" + Cia + "' " +
                                         "GROUP BY O.OM_Tipo, O.OM_Num, O.OM_Fecha, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, B.Bod_Responsable, O.OM_TEspecialidad, O.OM_Cia, O.OM_Planillar, O.OM_Radicada, O.OM_Autorizacion, O.OM_Consumida, OM_Clasificacion  " +
                                         "ORDER BY O.OM_Num DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_OM> O = new List<CXN_OM>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    string Pla = "NO";

                                    if (Lectura_Hora["OM_Planillar"] != DBNull.Value)
                                    {
                                        Pla = (Lectura_Hora["OM_Planillar"].ToString() == "S" ? "SI" : "NO");
                                    }

                                    string Tipo;
                                    switch (Lectura_Hora["OM_Tipo"].ToString())
                                    {
                                        case "S":
                                            Tipo = "Servicios";
                                            break;

                                        case "M":
                                            Tipo = "Medicamentos";
                                            break;

                                        default:
                                            Tipo = "Error";
                                            break;
                                    }

                                    O.Add(new CXN_OM
                                    {
                                        OM_Num = Convert.ToInt32(Lectura_Hora["OM_Num"]),
                                        OM_Fecha = Convert.ToDateTime(Lectura_Hora["OM_Fecha"]),
                                        OM_Detalle = Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                        Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                        Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                        Lectura_Hora["Pac_SegundoN"].ToString(),
                                        OM_Prof = Lectura_Hora["Bod_Responsable"].ToString(),
                                        OM_Tipo = Tipo,
                                        OM_TEspecialidad = Lectura_Hora["OM_TEspecialidad"].ToString(),
                                        OM_Cia = Convert.ToInt32(Lectura_Hora["OM_Cia"]),
                                        OM_Planillar = Pla.ToString(),
                                        OM_Radicada = Lectura_Hora["OM_Radicada"].ToString(),
                                        OM_Autorizacion = Lectura_Hora["OM_Autorizacion"].ToString(),
                                        OM_Consumida = Lectura_Hora["OM_Consumida"].ToString(),
                                        OM_Clasificacion = (Lectura_Hora["OM_Clasificacion"] == DBNull.Value ? "ORDEN MEDICA" : Lectura_Hora["OM_Clasificacion"].ToString())
                                    });
                                }

                                return O;
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
        List<CXN_OM> IOrdenes.getOrdenes(int Paciente, string Especialidad)
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

                    String Cargar_Hora = "SELECT DISTINCT O.OM_Tipo, O.OM_Num, O.OM_Fecha, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, B.Bod_Responsable, O.OM_TEspecialidad, O.OM_Cia, O.OM_Planillar, " +
                                         "O.OM_Radicada, O.OM_Autorizacion, O.OM_Consumida, OM_Clasificacion " +
                                         "FROM CXN_PACIENTES P " +
                                         "INNER JOIN CXN_OM O ON P.Pac_Id = O.OM_Pac " +
                                         "INNER JOIN CXN_BODEGAS B ON O.OM_Prof = B.Bod_Usuario " +
                                         "WHERE P.Pac_Id = '" + Paciente + "' " +
                                         "AND O.OM_TEspecialidad = '" + Especialidad + "' " +
                                         "GROUP BY O.OM_Tipo, O.OM_Num, O.OM_Fecha, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, B.Bod_Responsable, O.OM_TEspecialidad, O.OM_Cia, O.OM_Planillar, O.OM_Radicada, O.OM_Autorizacion, O.OM_Consumida, OM_Clasificacion " +
                                         "ORDER BY O.OM_Num DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_OM> O = new List<CXN_OM>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    string Pla = "NO";
                                    string Aut = "";

                                    if (Lectura_Hora["OM_Planillar"] != DBNull.Value)
                                    {
                                        Pla = (Lectura_Hora["OM_Planillar"].ToString() == "S" ? "SI" : "NO");
                                    }

                                    if (Lectura_Hora["OM_Autorizacion"] != DBNull.Value)
                                    {
                                        Aut = Lectura_Hora["OM_Autorizacion"].ToString();
                                    }

                                    string Tipo;
                                    switch (Lectura_Hora["OM_Tipo"].ToString())
                                    {
                                        case "S":
                                            Tipo = "Servicios";
                                            break;

                                        case "M":
                                            Tipo = "Medicamentos";
                                            break;

                                        default:
                                            Tipo = "Error";
                                            break;
                                    }

                                    if (Lectura_Hora["OM_Num"] != DBNull.Value)
                                    {
                                        O.Add(new CXN_OM
                                        {
                                            OM_Num = Convert.ToInt32(Lectura_Hora["OM_Num"]),
                                            OM_Fecha = Convert.ToDateTime(Lectura_Hora["OM_Fecha"]),
                                            OM_Detalle = Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                            Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                            Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                            Lectura_Hora["Pac_SegundoN"].ToString(),
                                            OM_Prof = Lectura_Hora["Bod_Responsable"].ToString(),
                                            OM_Tipo = Tipo,
                                            OM_TEspecialidad = Lectura_Hora["OM_TEspecialidad"].ToString(),
                                            OM_Cia = Convert.ToInt32(Lectura_Hora["OM_Cia"]),
                                            OM_Planillar = Pla.ToString(),
                                            OM_Radicada = Lectura_Hora["OM_Radicada"].ToString(),
                                            OM_Autorizacion = Aut.ToString(),
                                            OM_Consumida = Lectura_Hora["OM_Consumida"].ToString(),
                                            OM_Clasificacion = (Lectura_Hora["OM_Clasificacion"] == DBNull.Value ? "ORDEN MEDICA" : Lectura_Hora["OM_Clasificacion"].ToString())
                                        });
                                    }
                                }

                                return O;
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
        CXN_OM IOrdenes.getOrden(int Numero, string Tipo, int Cia)
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
                                         "FROM CXN_OM " +
                                         "WHERE OM_Num = '" + Numero + "' " +
                                         "AND OM_Cia = '" + Cia + "' " +
                                         "AND OM_Tipo = '" + Tipo + "'";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_OM O = new CXN_OM
                                {
                                    OM_Ase = Convert.ToInt32(Lectura_Hora["OM_Ase"]),
                                    OM_Cantidad = Lectura_Hora["OM_Cantidad"].ToString(),
                                    OM_Cia = Convert.ToInt32(Lectura_Hora["OM_Cia"]),
                                    OM_Desc = Lectura_Hora["OM_Desc"].ToString(),
                                    OM_Detalle = Lectura_Hora["OM_Detalle"].ToString(),
                                    OM_Direccion = Lectura_Hora["OM_Direccion"].ToString(),
                                    OM_Duracion = Lectura_Hora["OM_Duracion"].ToString(),
                                    OM_DX1 = Lectura_Hora["OM_DX1"].ToString(),
                                    OM_DX1T = Lectura_Hora["OM_DX1T"].ToString(),
                                    OM_DX2 = Lectura_Hora["OM_DX2"].ToString(),
                                    OM_DX2T = Lectura_Hora["OM_DX2T"].ToString(),
                                    OM_DX3 = Lectura_Hora["OM_DX3"].ToString(),
                                    OM_DX3T = Lectura_Hora["OM_DX3T"].ToString(),
                                    OM_Edad = Lectura_Hora["OM_Edad"].ToString(),
                                    OM_Fecha = Convert.ToDateTime(Lectura_Hora["OM_Fecha"]),
                                    OM_Firma = Lectura_Hora["OM_Firma"].ToString(),
                                    OM_Genero = Lectura_Hora["OM_Genero"].ToString(),
                                    OM_Medicamento = Lectura_Hora["OM_Medicamento"].ToString(),
                                    OM_Num = Convert.ToInt32(Lectura_Hora["OM_Num"]),
                                    OM_Pac = Convert.ToInt32(Lectura_Hora["OM_Pac"]),
                                    OM_Presentacion = Lectura_Hora["OM_Presentacion"].ToString(),
                                    OM_Prof = Lectura_Hora["OM_Prof"].ToString(),
                                    OM_Telefono = Lectura_Hora["OM_Telefono"].ToString(),
                                    OM_TEspecialidad = Lectura_Hora["OM_TEspecialidad"].ToString(),
                                    OM_Tipo = Lectura_Hora["OM_Tipo"].ToString(),
                                    OM_Via = Lectura_Hora["OM_Via"].ToString(),
                                    OM_Id = Convert.ToInt32(Lectura_Hora["OM_Id"]),
                                    OM_Clasificacion = (Lectura_Hora["OM_Clasificacion"] == DBNull.Value ? "ORDEN MEDICA" : Lectura_Hora["OM_Clasificacion"].ToString())
                                };

                                return O;
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
        bool IOrdenes.insertOM(CXN_OM O)
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

                    string TE = "";
                    if (O.OM_TEspecialidad != null)
                    {
                        TE = O.OM_TEspecialidad;
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_OM (OM_Pac, " + //param1
                                                                                       "OM_Ase, " + //param2
                                                                                       "OM_Cia, " + //param3
                                                                                       "OM_Prof, " + //param4
                                                                                       "OM_Fecha, " + //param5
                                                                                       "OM_DX1, " + //param6
                                                                                       "OM_DX2, " + //param7
                                                                                       "OM_DX3, " + //param8
                                                                                       "OM_DX1T, " + //param9
                                                                                       "OM_DX2T, " + //param10
                                                                                       "OM_DX3T, " + //param11
                                                                                       "OM_Edad, " + //param12
                                                                                       "OM_Genero, " + //param13
                                                                                       "OM_Direccion, " + //param14
                                                                                       "OM_Tipo, " + //param15
                                                                                       "OM_Duracion, " + //param16
                                                                                       "OM_Cantidad, " + //param17
                                                                                       "OM_Via, " + //param18
                                                                                       "OM_Medicamento, " + //param19
                                                                                       "OM_Presentacion, " + //param20
                                                                                       "OM_Detalle, " + //param21
                                                                                       "OM_Telefono, " + //param22
                                                                                       "OM_Num, " + //param23
                                                                                       "OM_Firma, " + //param23
                                                                                       "OM_Desc, " +
                                                                                       "OM_TEspecialidad, " +
                                                                                       "OM_Planillar, " +
                                                                                       "OM_Radicada, " +
                                                                                       "OM_Autorizacion, " +
                                                                                       "OM_Consumida) " + //param24
                                                                       "values                  (@param1, " + // Hor_Estado
                                                                                                "@param2, " + // Hor_Pac_Id
                                                                                                "@param3, " + // Hor_Pac_Bod
                                                                                                "@param4, " + // Hor_Pac_Tipo_Serv
                                                                                                "@param5, " + // Hor_Pac_Cia
                                                                                                "@param6, " + // Hor_Pac_Cia
                                                                                                "@param7, " + // Hor_Pac_Cia
                                                                                                "@param8, " + // Hor_Pac_Cia
                                                                                                "@param9, " + // Hor_Pac_Cia
                                                                                                "@param10, " + // Hor_Pac_Cia
                                                                                                "@param11, " + // Hor_Pac_Cia
                                                                                                "@param12, " + // Hor_Pac_Cia
                                                                                                "@param13, " + // Hor_Pac_Cia
                                                                                                "@param14, " + // Hor_Pac_Cia
                                                                                                "@param15, " + // Hor_Pac_Cia
                                                                                                "@param16, " + // Hor_Pac_Cia
                                                                                                "@param17, " + // Hor_Pac_Cia
                                                                                                "@param18, " + // Hor_Pac_Cia
                                                                                                "@param19, " + // Hor_Pac_Cia
                                                                                                "@param20, " + // Hor_Pac_Cia
                                                                                                "@param21, " + // Hor_Pac_Cia
                                                                                                "@param22, " + // Hor_Pac_Cia
                                                                                                "@param23, " + // Hor_Pac_Cia
                                                                                                "@param24, " + // Hor_Pac_Cia
                                                                                                "@param25," +
                                                                                                "@param26, " +
                                                                                                "@param27, " +
                                                                                                "@param28, " +
                                                                                                "@param29, " +
                                                                                                "@param30)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", O.OM_Pac);
                    cmd.Parameters.AddWithValue("@param2", O.OM_Ase);
                    cmd.Parameters.AddWithValue("@param3", O.OM_Cia);
                    cmd.Parameters.AddWithValue("@param4", O.OM_Prof);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = O.OM_Fecha;
                    cmd.Parameters.AddWithValue("@param6", O.OM_DX1);
                    cmd.Parameters.AddWithValue("@param7", O.OM_DX2);
                    cmd.Parameters.AddWithValue("@param8", O.OM_DX3);
                    cmd.Parameters.AddWithValue("@param9", O.OM_DX1T);
                    cmd.Parameters.AddWithValue("@param10", O.OM_DX2T);
                    cmd.Parameters.AddWithValue("@param11", O.OM_DX3T);
                    cmd.Parameters.AddWithValue("@param12", O.OM_Edad);
                    cmd.Parameters.AddWithValue("@param13", O.OM_Genero);
                    cmd.Parameters.AddWithValue("@param14", O.OM_Direccion);
                    cmd.Parameters.AddWithValue("@param15", O.OM_Tipo);
                    cmd.Parameters.AddWithValue("@param16", O.OM_Duracion);
                    cmd.Parameters.AddWithValue("@param17", O.OM_Cantidad);
                    cmd.Parameters.AddWithValue("@param18", O.OM_Via);
                    cmd.Parameters.AddWithValue("@param19", O.OM_Medicamento);
                    cmd.Parameters.AddWithValue("@param20", O.OM_Presentacion);
                    cmd.Parameters.AddWithValue("@param21", O.OM_Detalle);
                    cmd.Parameters.AddWithValue("@param22", O.OM_Telefono);
                    cmd.Parameters.AddWithValue("@param23", O.OM_Num);
                    cmd.Parameters.AddWithValue("@param24", O.OM_Firma);
                    cmd.Parameters.AddWithValue("@param25", O.OM_Desc);
                    cmd.Parameters.AddWithValue("@param26", TE);
                    cmd.Parameters.AddWithValue("@param27", "N");
                    cmd.Parameters.AddWithValue("@param28", "N");
                    cmd.Parameters.AddWithValue("@param29", "");
                    cmd.Parameters.AddWithValue("@param30", "N");
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
        bool IOrdenes.SearchAutorization(string Autorization)
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
                    String Query = "SELECT OM_Autorizacion " +
                                   "FROM CXN_OM " +
                                   "WHERE OM_Autorizacion = '" + Autorization + "' " +
                                   "ORDER BY OM_Autorizacion DESC";
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
            catch
            {
                return false;
            }
        }
        bool IOrdenes.updateAutorizacion(int Orden, string Autorizacion, int Cia)
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
                    string Busqueda = "UPDATE CXN_OM " +
                                      "SET OM_Autorizacion = '" + Autorizacion + "' " +
                                      "WHERE OM_Num = '" + Orden + "' " +
                                      "AND OM_Cia = '" + Cia + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        bool IOrdenes.updateRadicar(int Orden, int Cia)
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
                    string Busqueda = "UPDATE CXN_OM " +
                                      "SET OM_Radicada = 'S' " +
                                      "WHERE OM_Num = '" + Orden + "' " +
                                      "AND OM_Cia = '" + Cia + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        string IOrdenes.SearchTypeOrden(int Orden, int Cia)
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
                    String Query = "SELECT OM_Tipo " +
                                   "FROM CXN_OM " +
                                   "WHERE OM_Num = '" + Orden + "'" +
                                   "AND OM_Cia = '" + Cia + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Reader["OM_Tipo"].ToString();
                    }
                    else
                    {
                        return "X";
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }    
        void IOrdenes.consumirAutorizacion(int Paciente, string Autorizacion, string Estado)
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

                    string Busqueda = "UPDATE CXN_OM " +
                                      "SET OM_Consumida = '" + Estado + "' " +
                                      "WHERE OM_Pac = '" + Paciente + "' " +
                                      "AND OM_Autorizacion = '" + Autorizacion + "'";
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
    }
}
