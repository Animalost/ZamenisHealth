using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Persistence.CXN.Metodos
{
    public class MCargos : ICargos
    {
        private static readonly IFacturacion repositorioFactura = new MFacturacion();

        string ICargos.insertService(CXN_CARGOS c)
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    int Prec_Servicio_Unitario = Convert.ToInt32(c.Car_Val_Tot) / Convert.ToInt32(c.Car_Cant);

                    DateTime Hoy = DateTime.Now;
                    DateTime Car_Fecha = Convert.ToDateTime(Hoy.ToString(datCone["Format_Fecha"]));

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CARGOS ( " + //param1
                                                          "Car_Pac, " + //param2
                                                          "Car_Cia, " + //param3
                                                          "Car_Ase, " + //param4
                                                          "Car_Fecha, " + //param6
                                                          "Car_Estado, " + //param7
                                                          "Car_Tipo, " + //param8
                                                          "Car_Cod, " + //param9
                                                          "Car_Cant, " + //param10
                                                          "Car_Val_Un, " + //param11
                                                          "Car_Val_Tot, " + //param12
                                                          "Car_Item, " +
                                                          "Car_Tipo_Doc, " +
                                                          "Car_Factura, " +
                                                          "Car_Usr_Graba, " +
                                                          "Car_Adm_Id) " + //param16
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
                                                          "@param11, " +
                                                          "@param12, " +
                                                          "@param13, " +
                                                          "@param14, " +
                                                          "@param15)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", c.Car_Pac);
                    cmd.Parameters.AddWithValue("@param2", c.Car_Cia);
                    cmd.Parameters.AddWithValue("@param3", c.Car_Ase);
                    cmd.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Car_Fecha;
                    cmd.Parameters.AddWithValue("@param5", c.Car_Estado);
                    cmd.Parameters.AddWithValue("@param6", c.Car_Tipo);
                    cmd.Parameters.AddWithValue("@param7", c.Car_Cod);
                    cmd.Parameters.AddWithValue("@param8", c.Car_Cant);
                    cmd.Parameters.AddWithValue("@param9", Prec_Servicio_Unitario);
                    cmd.Parameters.AddWithValue("@param10", c.Car_Val_Tot);
                    cmd.Parameters.AddWithValue("@param11", c.Car_Item);
                    cmd.Parameters.AddWithValue("@param12", c.Car_Tipo_Doc);
                    cmd.Parameters.AddWithValue("@param13", c.Car_Factura);
                    cmd.Parameters.AddWithValue("@param14", c.Car_Usr_Graba);
                    cmd.Parameters.AddWithValue("@param15", c.Car_Pac);
                    cmd.ExecuteNonQuery();
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        List<CotizacionR> ICargos.GenerarDocumento(int Docs, int Cia)
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT Ci.Com_Nombre, Ci.Com_Direccion, Ci.Com_Telefono, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, " +
                                          "C.Car_Adm_Id, C.Car_Cod, C.Car_Cant, C.Car_Item, C.Car_Val_Un, C.Car_Val_Tot, P.Pac_Telefono, C.Car_Fecha, F.Fac_Num_Fac " +
                                          "FROM Cxn_Cargos C " +
                                          "INNER JOIN Cxn_Pacientes P ON C.Car_Pac = P.Pac_Id " +
                                          "INNER JOIN Cxn_Cia Ci ON C.Car_Cia = Ci.Com_Identificador " +
                                          "INNER JOIN Cxn_Factura F ON C.Car_Factura = F.Fac_Num_Fac " +
                                          "WHERE C.Car_Factura = '" + Docs + "' " +
                                          "AND F.Fac_Num_Fac = '" + Docs + "' " +
                                          "AND C.Car_Estado = 'C' " +
                                          "AND F.Fac_Tipo_Doc = 'CO' " +
                                          "AND C.Car_Tipo_Doc = 'CO' " +
                                          "AND C.Car_Cia = '" + Cia + "' " +
                                          "AND F.Fac_Cia = '" + Cia + "' " +
                                          "AND C.Car_Tipo = 'Cotizacion'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<CotizacionR> Class_Cotiza1 = new List<CotizacionR>();
                        var Totalizado_Valor = Calculos(Docs, Cia);

                        string Letra;
                        Letra = repositorioFactura.enletras(Totalizado_Valor.ToString()).ToUpper() + " PESOS";

                        while (Lectura_Hora2.Read() == true)
                        {
                            Class_Cotiza1.Add(new CotizacionR
                            {
                                EmpresaNombre = Lectura_Hora2["Com_Nombre"].ToString(),
                                EmpresaDireccion = Lectura_Hora2["Com_Direccion"].ToString(),
                                EmpresaTelefono = Lectura_Hora2["Com_Telefono"].ToString(),
                                PacienteNombre = Lectura_Hora2["Pac_PrimerN"].ToString() + " " +
                                                 Lectura_Hora2["Pac_SegundoN"].ToString() + " " +
                                                 Lectura_Hora2["Pac_PrimerA"].ToString() + " " +
                                                 Lectura_Hora2["Pac_SegundoA"].ToString(),
                                Car_Cod_Cotiza = Lectura_Hora2["Car_Cod"].ToString(),
                                Car_Cant_Cotiza = Lectura_Hora2["Car_Cant"].ToString(),
                                Car_Item_Cotiza = Lectura_Hora2["Car_Item"].ToString(),
                                Car_Val_Un_Cotiza = Convert.ToInt32(Lectura_Hora2["Car_Val_Un"]),
                                Car_Val_Tot_Cotiza = Convert.ToInt32(Lectura_Hora2["Car_Val_Tot"]),
                                PacienteTelefono = Lectura_Hora2["Pac_Telefono"].ToString(),
                                Car_Id_Cotiza = Lectura_Hora2["Fac_Num_Fac"].ToString(),
                                ValorLetras = Letra,
                                ValorNumeroTotal = Convert.ToInt32(Totalizado_Valor),
                                FechaBase = Convert.ToDateTime(Lectura_Hora2["Car_Fecha"]),
                                Admision = Docs
                            });
                        }
                        return Class_Cotiza1;
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
        int Calculos(int Fac, int Cia)
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora = "SELECT Car_Factura, SUM(Car_Val_Tot) AS TOT " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Factura = '" + Fac + "' " +
                                         "AND Car_Estado = 'C' " +
                                         "AND Car_Tipo = 'Cotizacion' " +
                                         "AND Car_Tipo_Doc = 'CO' " +
                                         "AND Car_Cia = '" + Cia + "' " +
                                         "GROUP BY Car_Factura";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Convert.ToInt32(Lectura_Hora["TOT"]);
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
        ListaCarga ICargos.Carga_Plantilla()
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_COD_CAR";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        ListaCarga Lista = new ListaCarga();

                        if (Lectura_Hora["Cod_Cod_1"] == DBNull.Value)
                        {
                            Lista.Cod1 = "";
                        }
                        else
                        {
                            Lista.Cod1 = Lectura_Hora["Cod_Cod_1"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_2"] == DBNull.Value)
                        {
                            Lista.Cod2 = "";
                        }
                        else
                        {
                            Lista.Cod2 = Lectura_Hora["Cod_Cod_2"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_3"] == DBNull.Value)
                        {
                            Lista.Cod3 = "";
                        }
                        else
                        {
                            Lista.Cod3 = Lectura_Hora["Cod_Cod_3"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_4"] == DBNull.Value)
                        {
                            Lista.Cod4 = "";
                        }
                        else
                        {
                            Lista.Cod4 = Lectura_Hora["Cod_Cod_4"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_5"] == DBNull.Value)
                        {
                            Lista.Cod5 = "";
                        }
                        else
                        {
                            Lista.Cod5 = Lectura_Hora["Cod_Cod_5"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_6"] == DBNull.Value)
                        {
                            Lista.Cod6 = "";
                        }
                        else
                        {
                            Lista.Cod6 = Lectura_Hora["Cod_Cod_6"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_7"] == DBNull.Value)
                        {
                            Lista.Cod7 = "";
                        }
                        else
                        {
                            Lista.Cod7 = Lectura_Hora["Cod_Cod_7"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_8"] == DBNull.Value)
                        {
                            Lista.Cod8 = "";
                        }
                        else
                        {
                            Lista.Cod8 = Lectura_Hora["Cod_Cod_8"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_9"] == DBNull.Value)
                        {
                            Lista.Cod9 = "";
                        }
                        else
                        {
                            Lista.Cod9 = Lectura_Hora["Cod_Cod_9"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_10"] == DBNull.Value)
                        {
                            Lista.Cod10 = "";
                        }
                        else
                        {
                            Lista.Cod10 = Lectura_Hora["Cod_Cod_10"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_11"] == DBNull.Value)
                        {
                            Lista.Cod11 = "";
                        }
                        else
                        {
                            Lista.Cod11 = Lectura_Hora["Cod_Cod_11"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_12"] == DBNull.Value)
                        {
                            Lista.Cod12 = "";
                        }
                        else
                        {
                            Lista.Cod12 = Lectura_Hora["Cod_Cod_12"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_13"] == DBNull.Value)
                        {
                            Lista.Cod13 = "";
                        }
                        else
                        {
                            Lista.Cod13 = Lectura_Hora["Cod_Cod_13"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_14"] == DBNull.Value)
                        {
                            Lista.Cod14 = "";
                        }
                        else
                        {
                            Lista.Cod14 = Lectura_Hora["Cod_Cod_14"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_15"] == DBNull.Value)
                        {
                            Lista.Cod15 = "";
                        }
                        else
                        {
                            Lista.Cod15 = Lectura_Hora["Cod_Cod_15"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_16"] == DBNull.Value)
                        {
                            Lista.Cod16 = "";
                        }
                        else
                        {
                            Lista.Cod16 = Lectura_Hora["Cod_Cod_16"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_17"] == DBNull.Value)
                        {
                            Lista.Cod17 = "";
                        }
                        else
                        {
                            Lista.Cod17 = Lectura_Hora["Cod_Cod_17"].ToString();
                        }

                        if (Lectura_Hora["Cod_Cod_18"] == DBNull.Value)
                        {
                            Lista.Cod18 = "";
                        }
                        else
                        {
                            Lista.Cod18 = Lectura_Hora["Cod_Cod_18"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_1"] == DBNull.Value)
                        {
                            Lista.Can1 = "";
                        }
                        else
                        {
                            Lista.Can1 = Lectura_Hora["Cod_Can_1"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_2"] == DBNull.Value)
                        {
                            Lista.Can2 = "";
                        }
                        else
                        {
                            Lista.Can2 = Lectura_Hora["Cod_Can_2"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_3"] == DBNull.Value)
                        {
                            Lista.Can3 = "";
                        }
                        else
                        {
                            Lista.Can3 = Lectura_Hora["Cod_Can_3"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_4"] == DBNull.Value)
                        {
                            Lista.Can4 = "";
                        }
                        else
                        {
                            Lista.Can4 = Lectura_Hora["Cod_Can_4"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_5"] == DBNull.Value)
                        {
                            Lista.Can5 = "";
                        }
                        else
                        {
                            Lista.Can5 = Lectura_Hora["Cod_Can_5"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_6"] == DBNull.Value)
                        {
                            Lista.Can6 = "";
                        }
                        else
                        {
                            Lista.Can6 = Lectura_Hora["Cod_Can_6"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_7"] == DBNull.Value)
                        {
                            Lista.Can7 = "";
                        }
                        else
                        {
                            Lista.Can7 = Lectura_Hora["Cod_Can_7"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_8"] == DBNull.Value)
                        {
                            Lista.Can8 = "";
                        }
                        else
                        {
                            Lista.Can8 = Lectura_Hora["Cod_Can_8"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_9"] == DBNull.Value)
                        {
                            Lista.Can9 = "";
                        }
                        else
                        {
                            Lista.Can9 = Lectura_Hora["Cod_Can_9"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_10"] == DBNull.Value)
                        {
                            Lista.Can10 = "";
                        }
                        else
                        {
                            Lista.Can10 = Lectura_Hora["Cod_Can_10"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_11"] == DBNull.Value)
                        {
                            Lista.Can11 = "";
                        }
                        else
                        {
                            Lista.Can11 = Lectura_Hora["Cod_Can_11"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_12"] == DBNull.Value)
                        {
                            Lista.Can12 = "";
                        }
                        else
                        {
                            Lista.Can12 = Lectura_Hora["Cod_Can_12"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_13"] == DBNull.Value)
                        {
                            Lista.Can13 = "";
                        }
                        else
                        {
                            Lista.Can13 = Lectura_Hora["Cod_Can_13"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_14"] == DBNull.Value)
                        {
                            Lista.Can14 = "";
                        }
                        else
                        {
                            Lista.Can14 = Lectura_Hora["Cod_Can_14"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_15"] == DBNull.Value)
                        {
                            Lista.Can15 = "";
                        }
                        else
                        {
                            Lista.Can15 = Lectura_Hora["Cod_Can_15"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_16"] == DBNull.Value)
                        {
                            Lista.Can16 = "";
                        }
                        else
                        {
                            Lista.Can16 = Lectura_Hora["Cod_Can_16"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_17"] == DBNull.Value)
                        {
                            Lista.Can17 = "";
                        }
                        else
                        {
                            Lista.Can17 = Lectura_Hora["Cod_Can_17"].ToString();
                        }

                        if (Lectura_Hora["Cod_Can_18"] == DBNull.Value)
                        {
                            Lista.Can18 = "";
                        }
                        else
                        {
                            Lista.Can18 = Lectura_Hora["Cod_Can_18"].ToString();
                        }

                        return Lista;
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
        List<CXN_CARGOS> ICargos.cotizacionesPrevias(string TID, string ID)
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT F.Fac_Num_Fac, F.Fac_Fecha, F.Fac_Usr_Graba, F.Fac_Cia, " +
                                         "P.Pac_PrimerN + ' ' + P.Pac_SegundoN + ' ' + P.Pac_PrimerA + ' ' + P.Pac_SegundoA AS Nombres " +
                                         "FROM CXN_CARGOS C " +
                                         "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                         "WHERE P.Pac_TipoId = '" + TID + "' " +
                                         "AND P.Pac_IdNum = '" + ID + "' " +
                                         "AND C.Car_Estado = 'C' " +
                                         "AND C.Car_Tipo = 'Cotizacion' " +
                                         "AND C.Car_Tipo_Doc = 'CO' " +
                                         "AND F.Fac_Tipo_Doc = 'CO' " +
                                         "GROUP BY F.Fac_Num_Fac, F.Fac_Fecha, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, " +
                                         "F.Fac_Usr_Graba, F.Fac_Cia " +
                                         "ORDER BY F.Fac_Fecha DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CARGOS> C = new List<CXN_CARGOS>();

                        while (Lectura_Hora.Read() == true)
                        {
                            C.Add(new CXN_CARGOS
                            {
                                Car_Factura = Lectura_Hora["Fac_Num_Fac"].ToString(),
                                Car_Fecha = Convert.ToDateTime(Lectura_Hora["Fac_Fecha"]),
                                Car_Detalle = Lectura_Hora["Nombres"].ToString(), //Nombre paciente ne este caso
                                Car_Usr_Graba = Lectura_Hora["Fac_Usr_Graba"].ToString(),
                                Car_Cia = Convert.ToInt32(Lectura_Hora["Fac_Cia"])
                            });
                        }

                        return C;
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
        void ICargos.Individual(DateTime Desde, DateTime Hasta, string Doc)
        {

            var datCone = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                FileStream Query = new FileStream("C:/Cxn/Reportes/" + Doc + "-Informe.txt", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);
                SqlCommand comando = new SqlCommand("SELECT Pac_PrimerA + ' ' + Pac_SegundoA + ' ' + Pac_PrimerN + ' ' + Pac_SegundoN AS NOMBRE, " +
                                                    "Pac_TipoId, Pac_IdNum, Car_Fecha, Car_Cod, Car_Val_Tot, Car_Cant, Car_Item " +
                                                    "FROM CXN_CARGOS " +
                                                    "INNER JOIN CXN_PACIENTES ON CXN_CARGOS.Car_Pac = CXN_PACIENTES.Pac_Id " +
                                                    "WHERE CXN_CARGOS.Car_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(datCone["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(datCone["Format_Fecha"]) + "' " +
                                                    "AND CXN_PACIENTES.Pac_IdNum = '" + Doc + "' " +
                                                    "ORDER BY CXN_CARGOS.Car_Fecha, Car_Cod ASC", con);
                SqlDataReader leer;
                leer = comando.ExecuteReader();

                Escriba.Write("PACIENTE" + "," + "TIPO ID" + "," + "NUMERO ID" + "," + "FECHA" + "," + "CODIGO" + "," + "SERVICIO" + "," + "CANTIDAD" + "," + "VALOR TOTAL");
                Escriba.WriteLine();
                Escriba.Flush();

                while (leer.Read())
                {
                    Escriba.Write(leer["NOMBRE"].ToString() + ",");
                    Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                    Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                    Escriba.Write(Convert.ToDateTime(leer["Car_Fecha"].ToString()).ToString(datCone["Format_Fecha"]) + ",");
                    Escriba.Write(leer["Car_Cod"].ToString() + ",");
                    Escriba.Write(leer["Car_Item"].ToString() + ",");
                    Escriba.Write(leer["Car_Cant"].ToString() + ",");
                    Escriba.Write(Convert.ToInt32(leer["Car_Val_Tot"]).ToString());
                    Escriba.WriteLine();
                    Escriba.Flush();
                }
                Escriba.Close();
            }
        }
        void ICargos.Total(DateTime Desde, DateTime Hasta)
        {
            var datCone = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                FileStream Query = new FileStream("C:/Cxn/Reportes/" + "CargosTotal" + "-Informe-Consolidado.csv", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);
                SqlCommand comando = new SqlCommand("SELECT Pac_PrimerA + ' ' + Pac_SegundoA + ' ' + Pac_PrimerN + ' ' + Pac_SegundoN AS NOMBRE, " +
                                                    "Pac_TipoId, Pac_IdNum, Car_Fecha, Car_Cod, SUM(CAST(Car_Val_Tot as INT)) AS TOTAL, SUM(CAST(Car_Cant as INT)) AS CANTIDAD, Car_Item " +
                                                    "FROM CXN_CARGOS " +
                                                    "INNER JOIN CXN_PACIENTES ON CXN_CARGOS.Car_Pac = CXN_PACIENTES.Pac_Id " +
                                                    "WHERE CXN_CARGOS.Car_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(datCone["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(datCone["Format_Fecha"]) + "' " +
                                                    "GROUP BY Pac_PrimerA, Pac_SegundoA, Pac_PrimerN, Pac_SegundoN, Pac_TipoId, Pac_IdNum, Car_Fecha, Car_Cod, Car_Item " +
                                                    "ORDER BY CXN_CARGOS.Car_Fecha, Car_Cod ASC", con);
                SqlDataReader leer;
                leer = comando.ExecuteReader();

                Escriba.Write("PACIENTE" + "," + "TIPO ID" + "," + "NUMERO ID" + "," + "FECHA" + "," + "CODIGO" + "," + "SERVICIO" + "," + "CANTIDAD" + "," + "VALOR TOTAL");
                Escriba.WriteLine();
                Escriba.Flush();

                while (leer.Read())
                {
                    Escriba.Write(leer["NOMBRE"].ToString() + ",");
                    Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                    Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                    Escriba.Write(Convert.ToDateTime(leer["Car_Fecha"].ToString()).ToString(datCone["Format_Fecha"]) + ",");
                    Escriba.Write(leer["Car_Cod"].ToString() + ",");
                    Escriba.Write(leer["Car_Item"].ToString() + ",");
                    Escriba.Write(leer["CANTIDAD"].ToString() + ",");
                    Escriba.Write(Convert.ToInt32(leer["TOTAL"]).ToString());
                    Escriba.WriteLine();
                    Escriba.Flush();
                }
                Escriba.Close();

            }
        }
        void ICargos.Rpt_Car_Tot(DateTime Desde, DateTime Hasta)
        {
            var datCone = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now;

                FileStream Query = new FileStream("C:/Cxn/Reportes/Cargos_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);

                SqlCommand comando = new SqlCommand("SELECT Pac_PrimerA, Pac_SegundoA, Pac_PrimerN, Pac_SegundoN, Pac_TipoId, Pac_IdNum, Car_Cod, Car_Item, Car_Cant, Car_Val_Tot, Car_Fecha, Car_Factura, Car_Estado " +
                                                    "FROM CXN_CARGOS " +
                                                    "INNER JOIN CXN_PACIENTES ON CXN_CARGOS.Car_Pac = CXN_PACIENTES.Pac_Id " +
                                                    "WHERE Car_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(datCone["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(datCone["Format_Fecha"]) + "'", con);
                SqlDataReader leer;
                leer = comando.ExecuteReader();

                Escriba.Write("TIPO DOCUMENTO PACIENTE" + "," +
                              "DOCUMENTO PACIENTE" + "," +
                              "PRIMER NOMBRE" + "," +
                              "SEGUNDO NOMBRE" + "," +
                              "PRIMER APELLIDO" + "," +
                              "SEGUNDO APELLIDO" + "," +
                              "ESTADO CARGO" + "," +
                              "FACTURA (Si Aplica)" + "," +
                              "CODIGO" + "," +
                              "ITEM" + "," +
                              "CANTIDAD" + "," +
                              "VALOR TOTAL" + "," +
                              "FECHA CARGO");
                Escriba.WriteLine();
                Escriba.Flush();

                string Estado_Cita = "";

                while (leer.Read())
                {
                    if (leer["Car_Estado"].ToString() == "F") { Estado_Cita = "CARGO FACTURADO"; }
                    if (leer["Car_Estado"].ToString() == "G") { Estado_Cita = "CARGO SIN FACTURAR"; }
                    if (leer["Car_Estado"].ToString() == "A") { Estado_Cita = "CARGO ANULADO"; }

                    Escriba.Write(leer["Pac_TipoId"].ToString() + ",");
                    Escriba.Write(leer["Pac_IdNum"].ToString() + ",");
                    Escriba.Write(leer["Pac_PrimerN"].ToString() + ",");
                    Escriba.Write(leer["Pac_SegundoN"].ToString() + ",");
                    Escriba.Write(leer["Pac_PrimerA"].ToString() + ",");
                    Escriba.Write(leer["Pac_SegundoA"].ToString() + ",");
                    Escriba.Write(Estado_Cita + ",");
                    Escriba.Write(leer["Car_Factura"].ToString() + ",");
                    Escriba.Write(leer["Car_Cod"].ToString().ToString() + ",");
                    Escriba.Write(leer["Car_Item"].ToString().ToString() + ",");
                    Escriba.Write(leer["Car_Cant"].ToString().ToString() + ",");
                    Escriba.Write(Convert.ToInt32(leer["Car_Val_Tot"]).ToString("N0") + ",");
                    Escriba.Write(Convert.ToDateTime(leer["Car_Fecha"].ToString()).ToString(datCone["Format_Fecha"]));
                    Escriba.WriteLine();
                    Escriba.Flush();
                }
                Escriba.Close();
            }
        }
        bool ICargos.InsertarCargoHistorias(CXN_CARGOS c)
        {
            try
            {
                var datCone = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    //int Prec_Servicio_Unitario = Convert.ToInt32(c.Car_Val_Tot) / Convert.ToInt32(c.Car_Cant);

                    DateTime Hoy = DateTime.Now;
                    DateTime Car_Fecha = Convert.ToDateTime(Hoy.ToString(datCone["Format_Fecha"]));

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CARGOS " +
                                                        "(Car_Adm_Id, " +
                                                        "Car_Pac, " +
                                                        "Car_Cia, " +
                                                        "Car_Ase, " +
                                                        "Car_Prof, " +
                                                        "Car_Fecha, " +
                                                        "Car_Estado, " +
                                                        "Car_Tipo, " +
                                                        "Car_Cod, " +
                                                        "Car_Tipo_Serv, " +
                                                        "Car_Cant, " +
                                                        "Car_Val_Un, " +
                                                        "Car_Val_Tot, " +
                                                        "Car_Item, " +
                                                        "Car_Detalle, " +
                                                        "Car_DX1, " +
                                                        "Car_DX2, " +
                                                        "Car_DX3, " +
                                                        "Car_Regimen, " +
                                                        "Car_Ambito, " +
                                                        "Car_Finalidad, " +
                                                        "Car_Personal, " +
                                                        "Car_CExterna, " +
                                                        "Car_Finalidad_CO, " +
                                                        "Car_Imp_Dx) " +
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
                                                             "@param25)", con);

                    cmd.Parameters.AddWithValue("@param1", c.Car_Adm_Id);
                    cmd.Parameters.AddWithValue("@param2", c.Car_Pac);
                    cmd.Parameters.AddWithValue("@param3", c.Car_Cia);
                    cmd.Parameters.AddWithValue("@param4", c.Car_Ase);
                    cmd.Parameters.AddWithValue("@param5", c.Car_Prof);
                    cmd.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = c.Car_Fecha;
                    cmd.Parameters.AddWithValue("@param7", "G");
                    cmd.Parameters.AddWithValue("@param8", c.Car_Tipo);
                    cmd.Parameters.AddWithValue("@param9", c.Car_Cod);
                    cmd.Parameters.AddWithValue("@param10", c.Car_Tipo_Serv);
                    cmd.Parameters.AddWithValue("@param11", c.Car_Cant);
                    cmd.Parameters.AddWithValue("@param12", c.Car_Val_Un);
                    cmd.Parameters.AddWithValue("@param13", c.Car_Val_Tot);
                    cmd.Parameters.AddWithValue("@param14", c.Car_Item);
                    cmd.Parameters.AddWithValue("@param15", c.Car_Detalle);
                    cmd.Parameters.AddWithValue("@param16", c.Car_Dx1);
                    cmd.Parameters.AddWithValue("@param17", c.Car_Dx2);
                    cmd.Parameters.AddWithValue("@param18", c.Car_Dx3);
                    cmd.Parameters.AddWithValue("@param19", c.Car_Regimen);
                    cmd.Parameters.AddWithValue("@param20", c.Car_Ambito);
                    cmd.Parameters.AddWithValue("@param21", c.Car_Finalidad);
                    cmd.Parameters.AddWithValue("@param22", c.Car_Personal);
                    cmd.Parameters.AddWithValue("@param23", c.Car_CExterna);
                    cmd.Parameters.AddWithValue("@param24", c.Car_Finalidad_CO);
                    cmd.Parameters.AddWithValue("@param25", c.Car_Imp_Dx);
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
        void ICargos.deleteHistoria(int Admision)
        {
            var datCone = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(datCone["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string cargo = "DELETE FROM CXN_CARGOS " +
                               "WHERE Car_Adm_id = '" + Admision + "' " +
                               "AND Car_Estado = 'G'";
                SqlCommand commandcargo = new SqlCommand(cargo, con);
                int Guarda;
                Guarda = commandcargo.ExecuteNonQuery();
            }
        }
        CXN_CARGOS ICargos.GetCargosFHIR(int Admision)
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
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Adm_Id = @param1 " +
                                         "AND Car_Tipo IN ('Nota','Historia')";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_CARGOS c = new CXN_CARGOS
                                {
                                    Car_Dx1 = Lectura_Hora["Car_Dx1"] == DBNull.Value ? "0" : Lectura_Hora["Car_Dx1"].ToString(),
                                    Car_Imp_Dx = Lectura_Hora["Car_Imp_Dx"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Car_Imp_Dx"]),
                                    Car_CExterna = Lectura_Hora["Car_CExterna"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Car_CExterna"]),
                                    Car_CodeEgreso = Lectura_Hora["Car_CodeEgreso"] == DBNull.Value ? 0 : Convert.ToInt32(Lectura_Hora["Car_CodeEgreso"]),
                                    Car_Estado = Lectura_Hora["Car_Estado"].ToString()
                                };

                                return c;
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
        CXN_HORARIO ICargos.BuscarCargo(int Admision)
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

                    String Cargar_Hora = "SELECT P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, H.Hor_Pac_Fecha_Cita, B.Bod_Responsable, H.Hor_Pac_Bod, B.Bod_Tipo, " +
                                         "P.Pac_Id, H.Hor_Pac_Cia, H.Hor_Pac_Ase, N.Not_Nota, N.Not_Recomienda, N.Not_Observa, N.Not_Adherencia, N.Not_NotaAcla, A.Ase_Descripcion " +
                                         "FROM CXN_NOTAS N " +
                                         "INNER JOIN CXN_HORARIO H ON N.Not_Adm = H.Hor_Id " +
                                         "INNER JOIN CXN_CARGOS C ON N.Not_Adm = C.Car_Adm_Id " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                         "INNER JOIN CXN_ASEGURADORA A ON C.Car_Ase = A.Ase_Identificador " +
                                         "WHERE N.Not_Adm = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                bool _getCargosPrevios2 = getCargosPrevios2(Admision);
                                if (_getCargosPrevios2 == true)
                                {
                                    CXN_HORARIO H = new CXN_HORARIO
                                    {
                                        Hor_Imp_Age = Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() + " " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString(),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Hor_Pac_Ase = 0,
                                        Hor_Pac_Tipo_Serv = "",
                                        Hor_Pac_Id = 0,
                                        Hor_Pac_Cia = 0,
                                        Hor_Pac_Bod = 0,
                                        PacienteDireccion = "",
                                        PacienteIdentificacion = "",
                                        PacienteNombre = "",
                                        PacienteTelefono = "",
                                        Hor_Autoriza = ""
                                    };

                                    return H;
                                }
                                else
                                {
                                    CXN_HORARIO H = new CXN_HORARIO
                                    {
                                        Hor_Imp_Age = Lectura_Hora["Pac_PrimerN"].ToString() + " " + Lectura_Hora["Pac_SegundoN"].ToString() + " " + Lectura_Hora["Pac_PrimerA"].ToString() + " " + Lectura_Hora["Pac_SegundoA"].ToString(),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(),
                                        PacienteAseguradora = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                        Hor_Pac_Tipo_Serv = Lectura_Hora["Bod_Tipo"].ToString(),
                                        Hor_Pac_Id = Convert.ToInt32(Lectura_Hora["Pac_Id"]),
                                        Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                        Hor_Pac_Bod = Convert.ToInt32(Lectura_Hora["Hor_Pac_Bod"]),
                                        PacienteDireccion = Lectura_Hora["Not_Nota"].ToString().ToUpper(),
                                        PacienteIdentificacion = Lectura_Hora["Not_Observa"].ToString().ToUpper(),
                                        PacienteNombre = Lectura_Hora["Not_Recomienda"].ToString().ToUpper(),
                                        PacienteTelefono = Lectura_Hora["Not_Adherencia"].ToString().ToUpper(),
                                        Hor_Autoriza = Lectura_Hora["Not_NotaAcla"].ToString().ToUpper()
                                    };

                                    return H;
                                }
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
        bool getCargosPrevios2(int Admision)
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

                    String Cargar_Hora = "SELECT Car_Tipo " +
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Adm_Id = @param1 " +
                                         "AND Car_Tipo = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);
                        Carga_Command.Parameters.AddWithValue("@param2", "Cargo");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_CARGOS> ICargos.getCargosPrevios(int Paciente)
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

                    String Cargar_Hora = "SELECT C.Car_Adm_Id, C.Car_Fecha, C.Car_Item, B.Bod_Responsable, " +
                                         "P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN " +
                                         "FROM CXN_CARGOS C " +
                                         "INNER JOIN CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero " +
                                         "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                         "WHERE C.Car_Pac = '" + Paciente + "' " +
                                         "AND C.Car_Tipo = 'Nota' " +
                                         "ORDER BY Car_Fecha DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CARGOS> H = new List<CXN_CARGOS>();

                        while (Lectura_Hora.Read() == true)
                        {
                            H.Add(new CXN_CARGOS
                            {
                                Car_Adm_Id = Convert.ToInt32(Lectura_Hora["Car_Adm_Id"]),
                                Car_Fecha = Convert.ToDateTime(Lectura_Hora["Car_Fecha"]),
                                Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                Car_Detalle = Lectura_Hora["Bod_Responsable"].ToString(),
                                Car_Usr_Graba = Lectura_Hora["Pac_PrimerA"].ToString() + " " +
                                                Lectura_Hora["Pac_SegundoA"].ToString() + " " +
                                                Lectura_Hora["Pac_PrimerN"].ToString() + " " +
                                                Lectura_Hora["Pac_SegundoN"].ToString()
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
        async Task<List<CXN_HORARIO>> ICargos.ListaCargos(DateTime fecha, bool Hecho)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        await con.OpenAsync();
                    }

                    DateTime Hoy = DateTime.Now;

                    SqlCommand cmd = new SqlCommand("ConsultaCargos", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Fecha", Convert.ToDateTime(fecha).ToString(getData["Format_Fecha"]));
                    SqlDataReader Lectura_Hora = await cmd.ExecuteReaderAsync();
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            SqlCommand cmd2 = new SqlCommand("ConsultaAdmisionCargo", con);
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.AddWithValue("@Admision", Convert.ToInt32(Lectura_Hora["Hor_Id"]));
                            SqlDataReader Lectura_Hora2 = await cmd2.ExecuteReaderAsync();
                            if (Lectura_Hora2.Read() == true)
                            {
                                if (Hecho == true)
                                {
                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(),
                                        Hor_Estado = "Hecho"
                                    });
                                }
                            }
                            else
                            {
                                if (Hecho == false)
                                {
                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Observacion = Lectura_Hora["Bod_Responsable"].ToString(),
                                        Hor_Estado = "Pendiente"
                                    });
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
        List<CXN_CARGOS> ICargos.CargosAnt(int Admi)
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

                    String Cargar_Hora = "Select Car_Fecha, Car_Cod, Car_Item, Car_Cant, Pac_PrimerA + ' ' + Pac_SegundoA + ' ' + Pac_PrimerN + ' ' + Pac_SegundoN AS Nombre " +
                                         "FROM CXN_CARGOS " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_CARGOS.Car_Pac = CXN_PACIENTES.Pac_Id " +
                                         "WHERE Car_Adm_Id = '" + Admi + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CARGOS> C = new List<CXN_CARGOS>();

                        while (Lectura_Hora.Read() == true)
                        {
                            C.Add(new CXN_CARGOS
                            {
                                Car_Detalle = Lectura_Hora["Nombre"].ToString() + " - " + Convert.ToDateTime(Lectura_Hora["Car_Fecha"]).ToString(getData["Format_Fecha"]),
                                Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                Car_Cant = Convert.ToInt32(Lectura_Hora["Car_Cant"])
                            });
                        }

                        return C;
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
        bool ICargos.deleteCargo(int Admision)
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

                    String Cargar_Hora2 = "SELECT Car_Tipo " +
                                             "FROM Cxn_Cargos " +
                                             "WHERE Car_Adm_Id = '" + Admision + "' " +
                                             "AND Car_Tipo = 'Cargo' " +
                                             "AND Car_Estado = 'G'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
                    {
                        string Busqueda = "DELETE FROM Cxn_Cargos " +
                                          "WHERE Car_Adm_Id = '" + Admision + "' " +
                                          "AND Car_Tipo = 'Cargo' " +
                                          "AND Car_Estado = 'G'";
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }

        }
        ListaProd ICargos.DatoProd(int Ase, string Cod)
        {
            Dictionary<string,string> getData = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getData["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                String Cargar_Hora = "SELECT InvPrecio, InvItem, InvDetalle, InvImagen, InvCod " +
                                     "FROM CXN_INVENTARIO " +
                                     "WHERE InvConvenio = @param1 " +
                                     "AND InvCod = @param2";

                using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                {
                    Carga_Command.Parameters.AddWithValue("@param1", Ase);
                    Carga_Command.Parameters.AddWithValue("@param2", Cod);

                    using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                    {
                        if (Lectura_Hora.Read() == true)
                        {
                            ListaProd L = new ListaProd();

                            if (Lectura_Hora["InvItem"] == DBNull.Value)
                            {
                                L.NombreProducto = "";
                            }
                            else
                            {
                                L.NombreProducto = Lectura_Hora["InvItem"].ToString();
                            }

                            if (Lectura_Hora["InvPrecio"] == DBNull.Value)
                            {
                                L.ValorProducto = 0;
                            }
                            else
                            {
                                L.ValorProducto = Convert.ToInt32(Lectura_Hora["InvPrecio"]);
                            }

                            if (Lectura_Hora["InvDetalle"] == DBNull.Value)
                            {
                                L.DetalleProducto = "";
                            }
                            else
                            {
                                L.DetalleProducto = Lectura_Hora["InvDetalle"].ToString();
                            }

                            L.CodigoEPS = Lectura_Hora["InvImagen"] == null ? Lectura_Hora["InvCod"].ToString() : Lectura_Hora["InvImagen"].ToString();

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
        async Task<int> ICargos.SaveCargo(CXN_CARGOS C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CARGOS (Car_Adm_Id, " + //param1
                                                          "Car_Pac, " + //param2
                                                          "Car_Cia, " + //param3
                                                          "Car_Ase, " + //param4
                                                          "Car_Prof, " + //param5
                                                          "Car_Fecha, " + //param6
                                                          "Car_Estado, " + //param7
                                                          "Car_Tipo, " + //param8
                                                          "Car_Cod, " + //param9
                                                          "Car_Cant, " + //param10
                                                          "Car_Val_Un, " + //param11
                                                          "Car_Val_Tot, " + //param12
                                                          "Car_Item, " + //param13
                                                          "Car_Detalle, " + //param14
                                                          "Car_Usr_Graba, " + //param14
                                                          "Car_Tipo_Serv, " +
                                                          "CarCodEPSConvenio) " + //param16
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
                                                          "@param15, " + // Hor_Pac_Hora_Cita
                                                          "@param16, " +
                                                          "@param17)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", C.Car_Adm_Id);
                    cmd.Parameters.AddWithValue("@param2", C.Car_Pac);
                    cmd.Parameters.AddWithValue("@param3", C.Car_Cia);
                    cmd.Parameters.AddWithValue("@param4", C.Car_Ase);
                    cmd.Parameters.AddWithValue("@param5", C.Car_Prof);
                    cmd.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = C.Car_Fecha;
                    cmd.Parameters.AddWithValue("@param7", C.Car_Estado);
                    cmd.Parameters.AddWithValue("@param8", C.Car_Tipo);
                    cmd.Parameters.AddWithValue("@param9", C.Car_Cod);
                    cmd.Parameters.AddWithValue("@param10", C.Car_Cant);
                    cmd.Parameters.AddWithValue("@param11", C.Car_Val_Un);
                    cmd.Parameters.AddWithValue("@param12", C.Car_Val_Tot);
                    cmd.Parameters.AddWithValue("@param13", C.Car_Item);
                    cmd.Parameters.AddWithValue("@param14", C.Car_Detalle);
                    cmd.Parameters.AddWithValue("@param15", C.Car_Usr_Graba);
                    cmd.Parameters.AddWithValue("@param16", C.Car_Tipo_Serv);
                    cmd.Parameters.AddWithValue("@param17", C.CarCodEPSConvenio);
                    int R = await cmd.ExecuteNonQueryAsync();
                    return R;
                }
            }
            catch
            {
                return 0;
            }
        }
        CXN_CARGOS ICargos.getValores(int Posision)
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

                    String Cargar_Hora4 = "SELECT Car_Cant, Car_Val_Un, Car_Val_Tot, Car_Item, Car_Ase, Car_Tipo_Serv, Car_Cod, Car_Id " +
                                          "FROM CXN_CARGOS " +
                                          "WHERE Car_Id = '" + Posision + "'";
                    SqlCommand Carga_Command4 = new SqlCommand(Cargar_Hora4, con);
                    SqlDataReader Lectura_Hora4 = (Carga_Command4.ExecuteReader());
                    if (Lectura_Hora4.Read() == true)
                    {
                        CXN_CARGOS C = new CXN_CARGOS
                        {
                            Car_Item = Lectura_Hora4["Car_Item"].ToString(),
                            Car_Cant = Convert.ToInt32(Lectura_Hora4["Car_Cant"]),
                            Car_Val_Un = Convert.ToInt32(Lectura_Hora4["Car_Val_Un"]),
                            Car_Ase = Convert.ToInt32(Lectura_Hora4["Car_Ase"]),
                            Car_Tipo_Serv = Lectura_Hora4["Car_Tipo_Serv"].ToString(),
                            Car_Cod = Lectura_Hora4["Car_Cod"].ToString(),
                            Car_Id = Convert.ToInt32(Lectura_Hora4["Car_Id"])
                        };

                        return C;
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
        bool ICargos.updateValores(CXN_CARGOS C)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                     "SET Car_Cant = '" + C.Car_Cant + "', " +
                                     "Car_Val_Un = '" + C.Car_Val_Un + "', " +
                                     "Car_Val_Tot = '" + C.Car_Val_Tot + "' " +
                                     "WHERE Car_Id = '" + C.Car_Id + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }  
        CXN_CARGOS ICargos.getRIPS(int Admision)
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
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Adm_Id = '" + Admision + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        CXN_CARGOS C = new CXN_CARGOS
                        {
                            Car_Ambito = Convert.ToInt32(Lectura_Hora["Car_Ambito"]),
                            Car_Personal = Convert.ToInt32(Lectura_Hora["Car_Personal"]),
                            Car_CExterna = Convert.ToInt32(Lectura_Hora["Car_CExterna"]),
                            Car_Finalidad = Convert.ToInt32(Lectura_Hora["Car_Finalidad"]),
                            Car_Finalidad_CO = Convert.ToInt32(Lectura_Hora["Car_Finalidad_CO"]),
                            Car_Imp_Dx = Convert.ToInt32(Lectura_Hora["Car_Imp_Dx"])
                        };

                        return C;
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
        bool ICargos.updateCia(int Cia, int Posision)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                      "SET Car_Cia = '" + Cia + "' " +
                                      "WHERE Car_Id = '" + Posision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ICargos.updateAse(int Ase, int Posision)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                          "SET Car_Ase = '" + Ase + "' " +
                                          "WHERE Car_Id = '" + Posision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ICargos.updateDate(DateTime Fecha, int Posision)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                       "SET Car_Fecha = '" + Convert.ToDateTime(Fecha).ToString(getData["Format_Fecha"]) + "' " +
                                       "WHERE Car_Id = '" + Posision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ICargos.HabilitaInhabilita(string Est, int Posision)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                     "SET Car_Estado = '" + Est + "' " +
                                     "WHERE Car_Id = '" + Posision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ICargos.updateCargos(CXN_CARGOS C)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                        "SET Car_Cod = '" + C.Car_Cod + "', " +
                                        "Car_Val_Un = '" + Convert.ToInt32(C.Car_Val_Un).ToString() + "', " +
                                        "Car_Val_Tot = '" + Convert.ToInt32(C.Car_Val_Tot).ToString() + "', " +
                                        "Car_Item = '" + C.Car_Item + "', " +
                                        "Car_Tipo_Serv = '" + C.Car_Tipo_Serv + "' " +
                                        "WHERE Car_Id = '" + C.Car_Id + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool ICargos.updateCargosMasivo(CXN_CARGOS C)
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

                    string Busqueda = "UPDATE CXN_CARGOS " +
                                        "SET Car_Cod = @param1, " +
                                        "Car_Val_Un = @param2, " +
                                        "Car_Val_Tot = @param3, " +
                                        "Car_Item = @param4, " +
                                        "Car_Tipo_Serv = @param5 " +
                                        "WHERE Car_Adm_Id = @param6 " +
                                        "AND Car_Estado = 'G' " +
                                        "AND Car_Tipo = @param7, " +
                                        "AND Car_Ase = @param8";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", C.Car_Cod);
                    Accion.Parameters.AddWithValue("@param2", Convert.ToInt32(C.Car_Val_Un).ToString());
                    Accion.Parameters.AddWithValue("@param3", Convert.ToInt32(C.Car_Val_Tot).ToString());
                    Accion.Parameters.AddWithValue("@param4", C.Car_Item);
                    Accion.Parameters.AddWithValue("@param5", C.Car_Tipo_Serv);
                    Accion.Parameters.AddWithValue("@param6", C.Car_Adm_Id);
                    Accion.Parameters.AddWithValue("@param7", C.Car_Tipo);
                    Accion.Parameters.AddWithValue("@param8", C.Car_Ase);
                    Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_CARGOS> ICargos.BuscarCargoTotal(int Admision)
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
                    String Cargar_HoraC = "SELECT C.Car_Id, C.Car_Adm_Id, C.Car_Item, C.Car_Fecha, C.Car_Val_Un, C.Car_Cant, " +
                                          "C.Car_Val_Tot, A.Ase_Descripcion, C.Car_Estado, CI.Com_Nombre " +
                                          "FROM CXN_CARGOS C " +
                                          "INNER JOIN CXN_ASEGURADORA A ON C.Car_Ase = A.Ase_Identificador " +
                                          "INNER JOIN CXN_CIA CI ON C.Car_Cia = CI.Com_Identificador " +
                                          "WHERE C.Car_Adm_Id = '" + Admision + "'";
                    SqlCommand Carga_CommandC = new SqlCommand(Cargar_HoraC, con);
                    SqlDataReader Lectura_HoraC = (Carga_CommandC.ExecuteReader());
                    if (Lectura_HoraC.HasRows)
                    {
                        List<CXN_CARGOS> C = new List<CXN_CARGOS>();

                        while (Lectura_HoraC.Read() == true)
                        {
                            string Estado = Lectura_HoraC["Car_Estado"].ToString();
                            switch (Estado)
                            {
                                case "A":
                                    Estado = "Excluido";
                                    break;

                                case "G":
                                    Estado = "Disponible";
                                    break;

                                case "F":
                                    Estado = "Facturado";
                                    break;

                                default:
                                    Estado = "Error";
                                    break;

                            }

                            C.Add(new CXN_CARGOS
                            {
                                Car_Id = Convert.ToInt32(Lectura_HoraC["Car_Id"]),
                                Car_Adm_Id = Convert.ToInt32(Lectura_HoraC["Car_Adm_Id"]),
                                Car_Item = Lectura_HoraC["Car_Item"].ToString(),
                                Car_Fecha = Convert.ToDateTime(Lectura_HoraC["Car_Fecha"]),
                                Car_Val_Un = Convert.ToInt32(Lectura_HoraC["Car_Val_Un"]),
                                Car_Cant = Convert.ToInt32(Lectura_HoraC["Car_Cant"]),
                                Car_Val_Tot = Convert.ToInt32(Lectura_HoraC["Car_Val_Tot"]),
                                Car_Tipo_Doc = Lectura_HoraC["Ase_Descripcion"].ToString(),
                                Car_Estado = Estado,
                                Car_Detalle = Lectura_HoraC["Com_Nombre"].ToString()
                            });
                        }

                        return C;
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
        CXN_CARGOS ICargos.getLasCargoToCopy(int Admision)
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
                    String Cargar_HoraC = "SELECT * " +
                                          "FROM CXN_CARGOS " +
                                          "WHERE Car_Adm_Id = '" + Admision + "'";
                    SqlCommand Carga_CommandC = new SqlCommand(Cargar_HoraC, con);
                    SqlDataReader getC = (Carga_CommandC.ExecuteReader());
                    if (getC.Read() == true)
                    {
                        CXN_CARGOS CA = new CXN_CARGOS
                        {
                            Car_Pac = Convert.ToInt32(getC["Car_Pac"]),
                            Car_Cia = Convert.ToInt32(getC["Car_Cia"]),
                            Car_Ase = Convert.ToInt32(getC["Car_Ase"]),
                            Car_Prof = Convert.ToInt32(getC["Car_Prof"]),
                            Car_Cod = getC["Car_Cod"].ToString(),
                            Car_Val_Tot = Convert.ToInt32(getC["Car_Val_Tot"]),
                            Car_Val_Un = Convert.ToInt32(getC["Car_Val_Un"]),
                            Car_Tipo_Serv = getC["Car_Tipo_Serv"].ToString(),
                            Car_Detalle = getC["Car_Detalle"].ToString(),
                            Car_Item = getC["Car_Item"].ToString(),
                            Car_Dx1 = getC["Car_Dx1"].ToString(),
                            Car_Dx2 = getC["Car_Dx2"].ToString(),
                            Car_Dx3 = getC["Car_Dx3"].ToString(),
                            Car_Ambito = Convert.ToInt32(getC["Car_Ambito"]),
                            Car_Personal = Convert.ToInt32(getC["Car_Personal"]),
                            Car_CExterna = Convert.ToInt32(getC["Car_CExterna"]),
                            Car_Finalidad = Convert.ToInt32(getC["Car_Finalidad"]),
                            Car_Finalidad_CO = Convert.ToInt32(getC["Car_Finalidad_CO"]), //motivo                                           
                            Car_Imp_Dx = Convert.ToInt32(getC["Car_Imp_Dx"]),
                            Car_Regimen = getC["Car_Regimen"].ToString()
                        };

                        return CA;
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
        bool ICargos.cargoExiste(int Admision)
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
                                         "FROM CXN_CARGOS " +
                                         "WHERE Car_Adm_Id = @Admision " +
                                         "AND Car_Tipo = @param1 " +
                                         "ORDER BY Car_Id DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@Admision", Admision);
                        Carga_Command.Parameters.AddWithValue("@param1", "Cargo");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return true;
            }
        }
    }
}
