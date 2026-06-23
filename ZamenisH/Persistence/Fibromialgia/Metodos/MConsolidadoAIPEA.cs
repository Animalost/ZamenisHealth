using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.Fibromialgia;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MConsolidadoAIPEA : IConsolidadoAIPEA
    {
        private static readonly ICompañia repositorioCompañia = new MCompañia();
        private static readonly ICIE10 repositorioCIE10 = new MCIE10();
        List<InformeFibromialgia> IConsolidadoAIPEA.getEgresos(DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_TipoId, P.Pac_IdNum, " +
                                   "J.Jun_Fecha " +
                                   "FROM CXN_HCJUNTAS J " +
                                   "INNER JOIN CXN_PACIENTES P ON J.Jun_Pac = P.Pac_Id " +
                                   "WHERE J.Jun_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND J.Jun_Egresa = 'Egreso' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "ORDER BY P.Pac_PrimerA ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<InformeFibromialgia> L = new List<InformeFibromialgia>();

                        var getLogo = repositorioCompañia.getPrestadorbyCode(10);
                        string Bod_Firma1 = getLogo.Com_Logo;
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);

                        int Contar = Conteo(Desde, Hasta);

                        while (Reader.Read() == true)
                        {
                            string Dia = Convert.ToDateTime(Reader["Jun_Fecha"]).ToString("MMMM");

                            L.Add(new InformeFibromialgia
                            {
                                Paciente = Reader["Pac_PrimerA"].ToString() + " " +
                                           Reader["Pac_SegundoA"].ToString() + " " +
                                           Reader["Pac_PrimerN"].ToString() + " " +
                                           Reader["Pac_SegundoN"].ToString(),
                                TDocumento = Reader["Pac_TipoId"].ToString(),
                                NDocumento = Reader["Pac_IdNum"].ToString(),
                                Mes = Dia,
                                Cantidad = Contar,
                                Desde = Convert.ToDateTime(Desde),
                                Hasta = Convert.ToDateTime(Hasta),
                                Logo = bytes
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
        List<InformeFibromialgia> IConsolidadoAIPEA.getAdherencia(DateTime Desde, DateTime Hasta)
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

                    //Obtengo pacientes del mes del reporte
                    String Query = "SELECT P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, E.Evo_Pac, E.Evo_Fecha, E.Evo_Tipo " +
                                   "FROM CXN_EVOFIB E " +
                                   "INNER JOIN CXN_PACIENTES P ON E.Evo_Pac = P.Pac_Id " +
                                   "WHERE E.Evo_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "ORDER BY E.Evo_Fecha, P.Pac_PrimerA ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<InformeFibromialgia> L = new List<InformeFibromialgia>();

                        var getLogo = repositorioCompañia.getPrestadorbyCode(10);
                        string Bod_Firma1 = getLogo.Com_Logo;
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);

                        int Contar = ConteoAdherencia(Desde, Hasta);

                        while (Reader.Read() == true)
                        {
                            string Mes = Convert.ToDateTime(Reader["Evo_Fecha"]).ToString("yyyy-MM-dd");

                            L.Add(new InformeFibromialgia
                            {
                                Paciente = Reader["Pac_PrimerA"].ToString() + " " +
                                      Reader["Pac_SegundoA"].ToString() + " " +
                                      Reader["Pac_PrimerN"].ToString() + " " +
                                      Reader["Pac_SegundoN"].ToString(),
                                TDocumento = Reader["Pac_TipoId"].ToString(),
                                NDocumento = Reader["Pac_IdNum"].ToString(),
                                Mes = Mes,
                                Cantidad = Contar,
                                Desde = Convert.ToDateTime(Desde),
                                Hasta = Convert.ToDateTime(Hasta),
                                Logo = bytes,
                                Servicio = Reader["Evo_Tipo"].ToString()
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
        int ConteoAdherencia(DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT DISTINCT COUNT(*) AS Conteo " +
                                   "FROM CXN_EVOFIB E " +
                                   "INNER JOIN CXN_PACIENTES P ON E.Evo_Pac = P.Pac_Id " +
                                   "WHERE E.Evo_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND P.Pac_FibInf IS NULL";

                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Convert.ToInt32(Reader["Conteo"]);
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
        List<InformeFibromialgia> IConsolidadoAIPEA.getActivosYbase(DateTime Desde, DateTime Hasta)
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

                    //Obtengo pacientes del mes del reporte
                    String Query = "SELECT DISTINCT P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_FechaNto, P.Pac_Sexo, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, E.Evo_Pac, P.Pac_Telefono, P.Pac_Email, A.Ase_Descripcion " +
                                   "FROM CXN_EVOFIB E " +
                                   "INNER JOIN CXN_PACIENTES P ON E.Evo_Pac = P.Pac_Id " +
                                   "INNER JOIN CXN_ASEGURADORA A ON P.Pac_Aseguradora = A.Ase_Identificador " +
                                   "WHERE E.Evo_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "ORDER BY P.Pac_PrimerA ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<InformeFibromialgia> L = new List<InformeFibromialgia>();

                        var getLogo = repositorioCompañia.getPrestadorbyCode(10);
                        string Bod_Firma1 = getLogo.Com_Logo;
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);

                        while (Reader.Read() == true)
                        {
                            //consultor fecha de ingreso en la hctf
                            DateTime? getFIngreso = getFIngresobyPac(Convert.ToInt32(Reader["Evo_Pac"]));
                            if (getFIngreso != null)
                            {
                                //obtener mes de reporte de paciente
                                string Mes = Convert.ToDateTime(getFIngreso).ToString("yyyy-MM-dd");
                                string _edad = "";
                                string _sexo = "";

                                try
                                {
                                    DateTime nacimiento = Convert.ToDateTime(Reader["Pac_FechaNto"]);
                                    int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                                    _edad = edad + " Años";
                                }
                                catch
                                {
                                    _edad = "No calculado";
                                }

                                switch (Reader["Pac_Sexo"].ToString())
                                {
                                    case "M":
                                        _sexo = "Masculino";
                                        break;

                                    case "F":
                                        _sexo = "Femenino";
                                        break;

                                    default:
                                        _sexo = "No Definido";
                                        break;

                                }

                                string DX = getFDXbyPac(Convert.ToInt32(Reader["Evo_Pac"]));

                                L.Add(new InformeFibromialgia
                                {
                                    Paciente = Reader["Pac_PrimerA"].ToString() + " " +
                                          Reader["Pac_SegundoA"].ToString() + " " +
                                          Reader["Pac_PrimerN"].ToString() + " " +
                                          Reader["Pac_SegundoN"].ToString(),
                                    TDocumento = Reader["Pac_TipoId"].ToString(),
                                    NDocumento = Reader["Pac_IdNum"].ToString(),
                                    Correo = Reader["Pac_Email"].ToString(),
                                    Telefono = Reader["Pac_Telefono"].ToString(),
                                    Aseguradora = Reader["Ase_Descripcion"].ToString(),
                                    Edad = _edad,
                                    Desde = Convert.ToDateTime(Desde),
                                    Hasta = Convert.ToDateTime(Hasta),
                                    Logo = bytes,
                                    Sexo = _sexo,
                                    Inicio = Mes,
                                    Diagnostico = DX
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        List<InformeFibromialgia> IConsolidadoAIPEA.getPrevalentes(DateTime Desde, DateTime Hasta)
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

                    //Obtengo pacientes del mes del reporte
                    String Query = "SELECT DISTINCT P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, " +
                                   "P.Pac_TipoId, P.Pac_IdNum, E.Evo_Pac " +
                                   "FROM CXN_EVOFIB E " +
                                   "INNER JOIN CXN_PACIENTES P ON E.Evo_Pac = P.Pac_Id " +
                                   "WHERE E.Evo_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND P.Pac_FibInf IS NULL";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<InformeFibromialgia> L = new List<InformeFibromialgia>();

                        var getLogo = repositorioCompañia.getPrestadorbyCode(10);
                        string Bod_Firma1 = getLogo.Com_Logo;
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);

                        while (Reader.Read() == true)
                        {
                            //consultor fecha de ingreso en la hctf
                            DateTime? getFIngreso = getFIngresobyPac(Convert.ToInt32(Reader["Evo_Pac"]));
                            if (getFIngreso != null)
                            {
                                //obtener mes de reporte de paciente
                                string Mes = Convert.ToDateTime(getFIngreso).ToString("MMMM") + " - " + Convert.ToDateTime(getFIngreso).ToString("yyyy");

                                L.Add(new InformeFibromialgia
                                {
                                    Paciente = Reader["Pac_PrimerA"].ToString() + " " +
                                          Reader["Pac_SegundoA"].ToString() + " " +
                                          Reader["Pac_PrimerN"].ToString() + " " +
                                          Reader["Pac_SegundoN"].ToString(),
                                    TDocumento = Reader["Pac_TipoId"].ToString(),
                                    NDocumento = Reader["Pac_IdNum"].ToString(),
                                    Mes = Mes,
                                    Desde = Convert.ToDateTime(Desde),
                                    Hasta = Convert.ToDateTime(Hasta),
                                    Logo = bytes
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        string getFDXbyPac(int Paciente)
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

                    //Obtengo pacientes
                    String Query = "SELECT H.HC_CIE10 " +
                                   "FROM CXN_HCTF H " +
                                   "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                   "WHERE H.HC_PacId = '" + Paciente + "' " +
                                   "AND H.TipoHistoria = 'Ingreso' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "ORDER BY H.HC_Fecha DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return repositorioCIE10.BuscaDX(Reader["HC_CIE10"].ToString());
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
        DateTime? getFIngresobyPac(int Paciente)
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

                    //Obtengo pacientes
                    String Query = "SELECT H.HC_Fecha " +
                                   "FROM CXN_HCTF H " +
                                   "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                   "WHERE H.HC_PacId = '" + Paciente + "' " +
                                   "AND H.TipoHistoria = 'Ingreso' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "ORDER BY H.HC_Fecha DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Convert.ToDateTime(Reader["HC_Fecha"]);
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
        List<InformeFibromialgia> IConsolidadoAIPEA.getIngresos(DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_TipoId, P.Pac_IdNum, " +
                                   "H.HC_Fecha " +
                                   "FROM CXN_HCTF H " +
                                   "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                   "WHERE H.HC_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND H.TipoHistoria = 'Ingreso' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "ORDER BY P.Pac_PrimerA ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<InformeFibromialgia> L = new List<InformeFibromialgia>();

                        var getLogo = repositorioCompañia.getPrestadorbyCode(10);
                        string Bod_Firma1 = getLogo.Com_Logo;
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1);

                        int Contar = ConteoIngresos(Desde, Hasta);

                        while (Reader.Read() == true)
                        {
                            string Dia = Convert.ToDateTime(Reader["HC_Fecha"]).ToString("MMMM");

                            L.Add(new InformeFibromialgia
                            {
                                Paciente = Reader["Pac_PrimerA"].ToString() + " " +
                                           Reader["Pac_SegundoA"].ToString() + " " +
                                           Reader["Pac_PrimerN"].ToString() + " " +
                                           Reader["Pac_SegundoN"].ToString(),
                                TDocumento = Reader["Pac_TipoId"].ToString(),
                                NDocumento = Reader["Pac_IdNum"].ToString(),
                                Mes = Dia,
                                Cantidad = Contar,
                                Desde = Convert.ToDateTime(Desde),
                                Hasta = Convert.ToDateTime(Hasta),
                                Logo = bytes
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
        int Conteo(DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT COUNT(*) AS Conteo " +
                                   "FROM CXN_HCJUNTAS J " +
                                   "INNER JOIN CXN_PACIENTES P ON J.Jun_Pac = P.Pac_Id " +
                                   "WHERE J.Jun_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "AND J.Jun_Egresa = 'Egreso'";

                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Convert.ToInt32(Reader["Conteo"]);
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
        int ConteoIngresos(DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT COUNT(*) AS Conteo " +
                                   "FROM CXN_HCTF H " +
                                   "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                   "WHERE H.HC_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND P.Pac_FibInf IS NULL " +
                                   "AND H.TipoHistoria = 'Ingreso'";

                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Convert.ToInt32(Reader["Conteo"]);
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
    }
}
