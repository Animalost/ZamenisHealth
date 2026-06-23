using Domain;
using Domain.CXN;
using Persistence.Informes.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

namespace Persistence.Informes.Methods
{
    public class MInformeEstadistico : IInformeEstadistico
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IMedidasHerida repoMedidas = new MMedidasHerida();
        private static readonly ICIE10 repoCIE10 = new MCIE10();

        List<CXN_HORARIO> IInformeEstadistico.getCitasAsistidas(int Pac, string Tipo)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT H.Hor_Id, H.Hor_Pac_Fecha_Cita, C.Con_Nombre, H.Hor_Pac_Tipo_Serv " +
                                         "FROM CXN_HORARIO H " +
                                         "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                         "WHERE Hor_Estado = @param1 " +
                                         "AND Hor_Pac_Id = @param2 " +
                                         "AND Hor_Pac_Tipo_Serv IN (" + Tipo + ") " +
                                         "AND H.Hor_Pac_Ase = C.Con_Aseguradora " +
                                         "ORDER BY Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", "H");
                        Carga_Command.Parameters.AddWithValue("@param2", Pac);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                                while(Lectura_Hora.Read() == true)
                                {
                                    H.Add(new CXN_HORARIO 
                                    {
                                        Hor_Id = Convert.ToInt32(Lectura_Hora["Hor_Id"]),
                                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        Hor_Observacion = Lectura_Hora["Con_Nombre"].ToString(),
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        string IInformeEstadistico.getClase(int Admision)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT HC_TipoINGSAL " +
                                         "FROM CXN_HCMG " +
                                         "WHERE HC_Adm = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                switch (Lectura_Hora["HC_TipoINGSAL"].ToString())
                                {
                                    case "I":
                                        return "Ingreso";

                                    case "S":
                                        return "Salida";

                                    case "N/A":
                                        return "En Tratamiento";

                                    default:
                                        return "En Tratamiento";
                                }
                            }
                            else
                            {
                                String Cargar_Hora2 = "SELECT Not_EstINGSAL " +
                                        "FROM CXN_NOTAS " +
                                        "WHERE Not_adm = @param1";
                                using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                                {
                                    Carga_Command2.Parameters.AddWithValue("@param1", Admision);

                                    using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                                    {
                                        if (Lectura_Hora2.Read() == true)
                                        {
                                            switch (Lectura_Hora2["Not_EstINGSAL"].ToString())
                                            {
                                                case "I":
                                                    return "Ingreso";

                                                case "S":
                                                    return "Salida";

                                                case "N/A":
                                                    return "En Tratamiento";

                                                default:
                                                    return "En Tratamiento";
                                            }
                                        }
                                        else
                                        {
                                            return "En Tratamiento";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "ERROR";
            }
        }
        void IInformeEstadistico.updateTable(string Table, string Tipe, int Adm)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand Busqueda = null;

                    if (Table == "MG")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_HCMG " +
                                     "SET  " +
                                     "HC_TipoINGSAL = @param1 " +
                                     "WHERE HC_Adm = @param2", con);
                    }

                    if (Table == "CU")
                    {
                        Busqueda = new SqlCommand(@"UPDATE CXN_NOTAS " +
                                     "SET  " +
                                     "Not_EstINGSAL = @param1 " +
                                     "WHERE Not_Adm = @param2", con);
                    }

                    string dato = "";

                    /*
                     Marcar Ingreso de Paciente
Marcar Salida de Paciente
Marcar en Tratamiento
                     */

                    switch (Tipe)
                    {
                        case "Marcar Ingreso de Paciente":
                            dato = "I";
                            break;

                        case "Marcar Salida de Paciente":
                            dato = "S";
                            break;

                        case "Marcar en Tratamiento":
                            dato = "";
                            break;

                        default:
                            dato = "";
                            break;
                    }

                    Busqueda.Parameters.AddWithValue("@param1", dato);
                    Busqueda.Parameters.AddWithValue("@param2", Adm);                    
                    Busqueda.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<ExportInExcel> IInformeEstadistico.RptRecPaciente(DateTime Desde, DateTime Hasta, string tipoReporte)
        {
            try
            {
                Dictionary<string, string> getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;

                    DateTime? FIngreso = null;
                    string NamePaciente = "";
                    string Documento = "";
                    string DX = "";
                    string ComplejidadInicial = "";
                    string CantHeridas = "";
                    string TamañoHerida = "";
                    string ZonaHeridas = "";
                    string ManejoInicial = "";
                    string DuracionTratamiento = "";
                    DateTime? FSalida = null;
                    string UltimoCManEnfermero = "";
                    string EpitelioGanado = "";
                    string PorcentualEtiologia = ""; //PENDIENTE
                    string Ingreso = "";

                    List<int> getPacs = getPacs = getPacientes(Desde, Hasta, tipoReporte); //Obtener pacientes sin duplicados por rangos de fecha                    
                    if (getPacs != null)
                    {
                        List<ExportInExcel> excelList = new List<ExportInExcel>();
                        Dictionary<string, int> DicPatologiarecuperacion = new Dictionary<string, int>();
                        int ContadorNulo = 0;

                        //Obtener datos de ingreso
                        foreach (int pac in getPacs)
                        {
                            CXN_HCMG getIngreso = getCitasAsistidasIngreso(pac, "I", Hasta); // Obtiene el primer ingreso desde la fecha final de consult hacia atras
                            if (getIngreso != null)
                            {
                                if (!DicPatologiarecuperacion.ContainsKey("INGRESO_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia))
                                {
                                    DicPatologiarecuperacion.Add("INGRESO_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia, 1);
                                }
                                else
                                {
                                    DicPatologiarecuperacion["INGRESO_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia] = DicPatologiarecuperacion["INGRESO_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia] + 1;
                                }                                

                                string serv = repoCIE10.BuscaDX(getIngreso.HC_DX1T);
                                FIngreso = Convert.ToDateTime(getIngreso.HC_Fecha);
                                NamePaciente = getIngreso.HC_Pac;
                                Documento = getIngreso.HC_Estado;
                                DX = getIngreso.HC_DX1T + " " + serv;
                                ComplejidadInicial = getIngreso.HC_ServCatalogo;
                                ManejoInicial = getIngreso.HC_PManejo;
                                Ingreso = getIngreso.HC_Patologia;

                                //Obtener datos de Heridas Ingreso
                                List<CXN_HCMED> getMedidas = repoMedidas.Carga_Med(getIngreso.HC_Adm);
                                if (getMedidas != null)
                                {
                                    CantHeridas = getMedidas.Count.ToString();

                                    foreach (CXN_HCMED m in getMedidas)
                                    {
                                        TamañoHerida = TamañoHerida + "Largo: " + m.Med_Largo + " Ancho: " + m.Med_Ancho + " Profundidad: " + m.Med_Profundidad + "\n\r";
                                        ZonaHeridas = ZonaHeridas + m.Med_Observacion + "\n\r";
                                    }
                                }
                                else
                                {
                                    CantHeridas = "";
                                    TamañoHerida = "";
                                    ZonaHeridas = "";
                                }

                                //Obtener datos de salida
                                CXN_HCMG getSalida = getCitasAsistidasSalida(pac, "S", Convert.ToDateTime(FIngreso)); // Obtiene fecha de salida desde la fecha de ingreso hacia adelante
                                if (getSalida != null)
                                {
                                    //Obtener porcentaje de recuperacion por patologia
                                    if (!DicPatologiarecuperacion.ContainsKey("SALIDA_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia))
                                    {
                                        DicPatologiarecuperacion.Add("SALIDA_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia, 1);
                                    }
                                    else
                                    {
                                        DicPatologiarecuperacion["SALIDA_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia] = DicPatologiarecuperacion["SALIDA_" + getIngreso.HC_Fecha.ToString("MM-yyyy") + getIngreso.HC_Patologia] + 1;
                                    }                                    

                                    TimeSpan difFechas = getSalida.HC_Fecha - getIngreso.HC_Fecha;
                                    int Dias = difFechas.Days;
                                    int Mes = Dias / 30;
                                    int Año = Dias / 365;

                                    if (Mes > 12)
                                    {
                                        Año = Mes / 12;
                                    }

                                    DuracionTratamiento = "Años: " + Año.ToString() + "\n\r" +
                                                          "Meses: " + Mes.ToString() + "\n\r" +
                                                          "Dias: " + Dias.ToString();

                                    FSalida = Convert.ToDateTime(getSalida.HC_Fecha);

                                    //Obtener datos de Heridas Salida
                                    List<CXN_HCMED> getMedidasSalida = repoMedidas.Carga_Med(getIngreso.HC_Adm);
                                    if (getMedidasSalida != null)
                                    {
                                        foreach (CXN_HCMED medSalida in getMedidasSalida)
                                        {
                                            double MedidaCm2 = Convert.ToDouble(medSalida.Med_Profundidad);
                                            double MedidaCm2DivDias = 0;
                                            if (Dias != 0)
                                            {
                                                MedidaCm2DivDias = MedidaCm2 / Dias;
                                            }

                                            EpitelioGanado = EpitelioGanado + MedidaCm2DivDias.ToString() + "mm2 \n\r";
                                        }
                                    }
                                    else
                                    {
                                        EpitelioGanado = "";
                                    }

                                    //Ultimo Cambio de Manejo Enfermero
                                    CXN_CMAN getLastCMan = getManejosxPaciente(pac, Convert.ToDateTime(FSalida));
                                    if (getLastCMan != null)
                                    {
                                        UltimoCManEnfermero = getLastCMan.Cam_Adherencia.ToString();
                                    }
                                    else
                                    {
                                        UltimoCManEnfermero = "N/A";
                                    }
                                }
                                else
                                {
                                    UltimoCManEnfermero = "N/A";
                                    DuracionTratamiento = "";
                                    FSalida = null;
                                    DicPatologiarecuperacion.Add("NULO_" + ContadorNulo, ContadorNulo);
                                }
                            }
                            else
                            {


                                FIngreso = null;
                                NamePaciente = repoPac.LlamarPacientebyId(pac).Pac_PrimerA + " " + repoPac.LlamarPacientebyId(pac).Pac_SegundoA + " " +
                                    repoPac.LlamarPacientebyId(pac).Pac_PrimerN + " " + repoPac.LlamarPacientebyId(pac).Pac_SegundoN;
                                Documento = repoPac.LlamarPacientebyId(pac).Pac_TipoId + " " + repoPac.LlamarPacientebyId(pac).Pac_IdNum;
                                DX = "";
                                ComplejidadInicial = "";
                                ManejoInicial = "";
                                DicPatologiarecuperacion.Add("NULO_" + ContadorNulo, ContadorNulo);
                            }                           

                            excelList.Add(new ExportInExcel
                            {
                                Dato1 = (FIngreso != null ? Convert.ToDateTime(FIngreso).ToString(Conexion.ConectionDictionary["Format_Fecha"]) : ""),
                                Dato2 = NamePaciente.ToString(),
                                Dato3 = Documento.ToString(),
                                Dato4 = DX.ToString(),
                                Dato5 = ComplejidadInicial.ToString(),
                                Dato6 = CantHeridas.ToString(),
                                Dato7 = TamañoHerida.ToString(),
                                Dato8 = ZonaHeridas.ToString(),
                                Dato9 = ManejoInicial.ToString(),
                                Dato10 = DuracionTratamiento.ToString(),
                                Dato11 = (FSalida != null ? Convert.ToDateTime(FSalida).ToString(Conexion.ConectionDictionary["Format_Fecha"]) : ""),
                                Dato12 = UltimoCManEnfermero.ToString(),
                                Dato13 = EpitelioGanado.ToString(),
                                Dato14 = PorcentualEtiologia.ToString(),
                                Dato15 = Ingreso.ToString(),
                                Dato27 = DicPatologiarecuperacion
                            });

                            FIngreso = null;
                            NamePaciente = "";
                            Documento = "";
                            DX = "";
                            ComplejidadInicial = "";
                            CantHeridas = "";
                            TamañoHerida = "";
                            ZonaHeridas = "";
                            ManejoInicial = "";
                            DuracionTratamiento = "";
                            FSalida = null;
                            UltimoCManEnfermero = "";
                            EpitelioGanado = "";
                            PorcentualEtiologia = ""; //PENDIENTE
                            ContadorNulo++;
                        }

                        return excelList;
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
        CXN_CMAN getManejosxPaciente(int IdPac, DateTime FechaFin)
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
                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_CMAN " +
                                         "WHERE Cam_IdPac = '" + IdPac + "' " +
                                         "AND Cam_Fecha <= '" + Convert.ToDateTime(FechaFin).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                         "ORDER BY Cam_Fecha DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        CXN_CMAN L = new CXN_CMAN
                        {
                            Cam_Id = Convert.ToInt32(Lectura_Hora["Cam_Id"]),
                            Cam_UsrGenera = Lectura_Hora["Cam_UsrGenera"].ToString(),
                            Cam_Fecha = Convert.ToDateTime(Lectura_Hora["Cam_Fecha"]),
                            Cam_Adherencia = Lectura_Hora["Cam_Adherencia"].ToString()
                        };

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
        List<int> getPacientes(DateTime Desde, DateTime Hasta, string Tipo)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "";

                    if (Tipo == "DelMes")
                    {
                        Cargar_Hora = "SELECT DISTINCT HC_PacId " +
                                      "FROM CXN_HCMG " +
                                      "WHERE HC_Fecha BETWEEN @paramD AND @paramH";
                    }
                    else if (Tipo == "Salidas")
                    {
                        Cargar_Hora = "SELECT DISTINCT HC_PacId " +
                                      "FROM CXN_HCMG " +
                                      "WHERE HC_Fecha BETWEEN @paramD AND @paramH"; //aqui quite el filtro de salidas S
                    }
                    else
                    {
                        return null;
                    }                   

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.Add(new SqlParameter("@paramD", SqlDbType.DateTime)).Value = Convert.ToDateTime(Desde);
                        Carga_Command.Parameters.Add(new SqlParameter("@paramH", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hasta);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> H = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    H.Add(Convert.ToInt32(Lectura_Hora["HC_PacId"]));
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
        CXN_HCMG getCitasAsistidasIngreso(int Pac, string Tipo, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT TOP 1 H.HC_Fecha, H.HC_Pac, P.Pac_TipoId + ' ' + P.Pac_IdNum AS DOCUMENTOPACIENTE, H.HC_DX1 AS DXSERVICE, HC_ServCatalogo, " +
                                         "H.HC_PManejo, H.HC_Adm, H.HC_Patologia " +
                                         "FROM CXN_HCMG H " +
                                         "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                         "WHERE H.HC_PacId = @param1 " +
                                         "AND H.HC_TipoINGSAL = @param2 " +
                                         "AND H.HC_Fecha <= @paramH " +
                                         "ORDER BY H.HC_Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Pac);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.Add(new SqlParameter("@paramH", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hasta);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_HCMG H = new CXN_HCMG
                                {
                                    HC_Fecha = Convert.ToDateTime(Lectura_Hora["HC_Fecha"]),
                                    HC_Pac = Lectura_Hora["HC_Pac"].ToString(),
                                    HC_Estado = Lectura_Hora["DOCUMENTOPACIENTE"].ToString(),
                                    HC_DX1T = Lectura_Hora["DXSERVICE"].ToString(),
                                    HC_ServCatalogo = Lectura_Hora["HC_ServCatalogo"].ToString(),
                                    HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                    HC_Adm = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                                    HC_Patologia = Lectura_Hora["HC_Patologia"].ToString()
                                };

                                return H;
                            }
                            else
                            {
                                return getCitasAsistidasIngresoN(Pac, Tipo, Hasta);
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
        CXN_HCMG getCitasAsistidasIngresoN(int Pac, string Tipo, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT TOP 1 N.Not_Fecha, P.Pac_PrimerA + ' ' + P.Pac_SegundoA + ' ' + P.Pac_PrimerN + ' ' + P.Pac_SegundoN AS PAC, P.Pac_TipoId + ' ' + P.Pac_IdNum AS DOCUMENTOPACIENTE, CI.Cie_Cod AS DXSERVICE, C.Car_Item, " +
                                         "N.Not_Adherencia, N.Not_Adm, N.Not_Patologia " +
                                         "FROM CXN_NOTAS N " +
                                         "INNER JOIN CXN_PACIENTES P ON N.Not_Pac = P.Pac_Id " +
                                         "INNER JOIN CXN_CARGOS C ON N.Not_Adm = C.Car_Adm_Id " +
                                         "INNER JOIN CXN_CIE10 CI ON C.Car_Dx1 = CI.Cie_Cod " + 
                                         "WHERE N.Not_Pac = @param1 " +
                                         "AND C.Car_Tipo = 'Nota' " +
                                         "AND N.Not_EstINGSAL = @param2 " +
                                         "AND N.Not_Fecha <= @paramH " +
                                         "ORDER BY N.Not_Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Pac);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.Add(new SqlParameter("@paramH", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hasta);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_HCMG H = new CXN_HCMG
                                {
                                    HC_Fecha = Convert.ToDateTime(Lectura_Hora["Not_Fecha"]),
                                    HC_Pac = Lectura_Hora["PAC"].ToString(),
                                    HC_Estado = Lectura_Hora["DOCUMENTOPACIENTE"].ToString(),
                                    HC_DX1T = Lectura_Hora["DXSERVICE"].ToString(),
                                    HC_ServCatalogo = Lectura_Hora["Car_Item"].ToString(),
                                    HC_PManejo = Lectura_Hora["Not_Adherencia"].ToString(),
                                    HC_Adm = Convert.ToInt32(Lectura_Hora["Not_Adm"]),
                                    HC_Patologia = Lectura_Hora["Not_Patologia"].ToString()
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
        CXN_HCMG getCitasAsistidasSalida(int Pac, string Tipo, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT TOP 1 H.HC_Fecha, H.HC_Pac, P.Pac_TipoId + ' ' + P.Pac_IdNum AS DOCUMENTOPACIENTE, H.HC_DX1 + ' - ' + H.HC_DX1T AS DXSERVICE, HC_ServCatalogo, " +
                                         "H.HC_PManejo, H.HC_Adm, H.HC_Patologia " +
                                         "FROM CXN_HCMG H " +
                                         "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                         "WHERE H.HC_PacId = @param1 " +
                                         "AND H.HC_TipoINGSAL = @param2 " +
                                         "AND H.HC_Fecha >= @paramH " +
                                         "ORDER BY H.HC_Fecha ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Pac);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.Add(new SqlParameter("@paramH", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hasta);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_HCMG H = new CXN_HCMG
                                {
                                    HC_Fecha = Convert.ToDateTime(Lectura_Hora["HC_Fecha"]),
                                    HC_Pac = Lectura_Hora["HC_Pac"].ToString(),
                                    HC_Estado = Lectura_Hora["DOCUMENTOPACIENTE"].ToString(),
                                    HC_DX1T = Lectura_Hora["DXSERVICE"].ToString(),
                                    HC_ServCatalogo = Lectura_Hora["HC_ServCatalogo"].ToString(),
                                    HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                    HC_Adm = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                                    HC_Patologia = Lectura_Hora["HC_Patologia"].ToString()
                                };

                                return H;
                            }
                            else
                            {
                                return getCitasAsistidasSalidaN(Pac, Tipo, Hasta);
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
        CXN_HCMG getCitasAsistidasSalidaN(int Pac, string Tipo, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT TOP 1 H.HC_Fecha, H.HC_Pac, P.Pac_TipoId + ' ' + P.Pac_IdNum AS DOCUMENTOPACIENTE, H.HC_DX1 + ' - ' + H.HC_DX1T AS DXSERVICE, HC_ServCatalogo, " +
                                         "H.HC_PManejo, H.HC_Adm, H.HC_Patologia " +
                                         "FROM CXN_HCMG H " +
                                         "INNER JOIN CXN_PACIENTES P ON H.HC_PacId = P.Pac_Id " +
                                         "WHERE H.HC_PacId = @param1 " +
                                         "AND H.HC_TipoINGSAL = @param2 " +
                                         "AND H.HC_Fecha >= @paramH " +
                                         "ORDER BY H.HC_Fecha ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Pac);
                        Carga_Command.Parameters.AddWithValue("@param2", Tipo);
                        Carga_Command.Parameters.Add(new SqlParameter("@paramH", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hasta);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_HCMG H = new CXN_HCMG
                                {
                                    HC_Fecha = Convert.ToDateTime(Lectura_Hora["HC_Fecha"]),
                                    HC_Pac = Lectura_Hora["HC_Pac"].ToString(),
                                    HC_Estado = Lectura_Hora["DOCUMENTOPACIENTE"].ToString(),
                                    HC_DX1T = Lectura_Hora["DXSERVICE"].ToString(),
                                    HC_ServCatalogo = Lectura_Hora["HC_ServCatalogo"].ToString(),
                                    HC_PManejo = Lectura_Hora["HC_PManejo"].ToString(),
                                    HC_Adm = Convert.ToInt32(Lectura_Hora["HC_Adm"]),
                                    HC_Patologia = Lectura_Hora["HC_Patologia"].ToString()
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
    }
}
