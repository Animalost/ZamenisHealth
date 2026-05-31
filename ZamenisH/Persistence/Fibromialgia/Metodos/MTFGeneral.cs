using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using Domain.CXN;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MTFGeneral : ITFGeneral
    {
        public int Admision { get; set; }

        Byte[] ChartsGenerateEAV(string E, string R1, string R2)
        {
            var chart = new Chart
            {
                Width = 600,
                Height = 450,
                RenderType = RenderType.ImageTag,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };

            chart.Titles.Add("Evaluación análoga verbal (EAV)");
            chart.Titles[0].Font = new System.Drawing.Font("Arial", 12f);

            chart.ChartAreas.Add("");
            chart.ChartAreas[0].AxisX.Title = "";
            chart.ChartAreas[0].AxisY.Title = "Nivel";
            chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 12f);
            chart.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 12f);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 10f);
            chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            chart.ChartAreas[0].BackColor = Color.White;

            chart.Series.Add("");
            chart.Series[0].ChartType = SeriesChartType.Column;

            chart.Series[0].Points.AddXY("Evaluacion - VALOR: " + E.ToString(), E);
            chart.Series[0].Points.AddXY("ReValoracion 1 - VALOR:  " + R1.ToString(), R1);
            chart.Series[0].Points.AddXY("ReValoracion 2 - VALOR:  " + R2.ToString(), R2);

            var ms = new System.IO.MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Jpeg);
            byte[] pdfBytes = ms.ToArray();
            return pdfBytes;
        }

        DatosTF Eav(int Adm)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_HCTF " +
                                   "WHERE HC_Adm = '" + Adm + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        DatosTF dTF = new DatosTF
                        {
                            EAV = Convert.ToInt32(Reader["HC_Eva"]),
                            Cervical_HC_CC1 = Reader["HC_CC1"].ToString(),
                            Cervical_HC_CC2 = Reader["HC_CC2"].ToString(),
                            Cervical_HC_CC3 = Reader["HC_CC3"].ToString(),
                            Cervical_HC_CC4 = Reader["HC_CC4"].ToString(),
                            Cervical_HC_CC5 = Reader["HC_CC5"].ToString(),
                            Cervical_HC_CC6 = Reader["HC_CC6"].ToString(),

                            Cervical_HC_CD1 = Reader["HC_CD1"].ToString(),
                            Cervical_HC_CD2 = Reader["HC_CD2"].ToString(),
                            Cervical_HC_CD3 = Reader["HC_CD3"].ToString(),
                            Cervical_HC_CD4 = Reader["HC_CD4"].ToString(),

                            Hombro_HC_H1 = Reader["HC_H1"].ToString(),
                            Hombro_HC_H2 = Reader["HC_H2"].ToString(),
                            Hombro_HC_H3 = Reader["HC_H3"].ToString(),
                            Hombro_HC_H4 = Reader["HC_H4"].ToString(),
                            Hombro_HC_H5 = Reader["HC_H5"].ToString(),
                            Hombro_HC_H6 = Reader["HC_H6"].ToString(),

                            Cadera_HC_C1 = Reader["HC_C1"].ToString(),
                            Cadera_HC_C2 = Reader["HC_C2"].ToString(),
                            Cadera_HC_C3 = Reader["HC_C3"].ToString(),
                            Cadera_HC_C4 = Reader["HC_C4"].ToString(),
                            Cadera_HC_C5 = Reader["HC_C5"].ToString(),
                            Cadera_HC_C6 = Reader["HC_C6"].ToString(),

                            Codo_HC_T1 = Reader["HC_T1"].ToString(),
                            Codo_HC_T2 = Reader["HC_T2"].ToString(),

                            Rodilla_HC_T3 = Reader["HC_T3"].ToString(),
                            Rodilla_HC_T4 = Reader["HC_T4"].ToString(),

                            Antebrazo_HC_T5 = Reader["HC_T5"].ToString(),
                            Antebrazo_HC_T6 = Reader["HC_T6"].ToString(),

                            Tobillo_HC_T7 = Reader["HC_T7"].ToString(),
                            Tobillo_HC_T8 = Reader["HC_T8"].ToString(),
                            Tobillo_HC_T9 = Reader["HC_T9"].ToString(),
                            Tobillo_HC_T10 = Reader["HC_T10"].ToString(),

                            RadioCarpiana_HC_T11 = Reader["HC_T11"].ToString(),
                            RadioCarpiana_HC_T12 = Reader["HC_T12"].ToString(),
                            RadioCarpiana_HC_T13 = Reader["HC_T13"].ToString(),
                            RadioCarpiana_HC_T14 = Reader["HC_T14"].ToString(),

                            HisExtCui = Reader["HisExtCui"].ToString(),
                            HisExtCuD = Reader["HisExtCuD"].ToString(),
                            HisEscalenosI = Reader["HisEscalenosI"].ToString(),
                            HisEscalenosD = Reader["HisEscalenosD"].ToString(),
                            HisTrapSupI = Reader["HisTrapSupI"].ToString(),
                            HisTrapSupD = Reader["HisTrapSupD"].ToString(),
                            HisTrapMedI = Reader["HisTrapMedI"].ToString(),
                            HisTrapMedD = Reader["HisTrapMedD"].ToString(),
                            HisTrapInfI = Reader["HisTrapInfI"].ToString(),
                            HisTrapInfD = Reader["HisTrapInfD"].ToString(),
                            HisSerAntI = Reader["HisSerAntI"].ToString(),
                            HisSerAntD = Reader["HisSerAntD"].ToString(),
                            HisPecMayI = Reader["HisPecMayI"].ToString(),
                            HisPecMayD = Reader["HisPecMayD"].ToString(),
                            HisRomI = Reader["HisRomI"].ToString(),
                            HisRomD = Reader["HisRomD"].ToString(),
                            HisAbSupI = Reader["HisAbSupI"].ToString(),
                            HisAbSupD = Reader["HisAbSupD"].ToString(),
                            HisAbdInfI = Reader["HisAbdInfI"].ToString(),
                            HisAbdInfD = Reader["HisAbdInfD"].ToString(),
                            HisOblicuoI = Reader["HisOblicuoI"].ToString(),
                            HisOblicuoD = Reader["HisOblicuoD"].ToString(),
                            HisExtDorsalI = Reader["HisExtDorsalI"].ToString(),
                            HisExtDorsalD = Reader["HisExtDorsalD"].ToString(),
                            HisExtLumbarI = Reader["HisExtLumbarI"].ToString(),
                            HisExtLumbarD = Reader["HisExtLumbarD"].ToString(),
                            HisGluMayI = Reader["HisGluMayI"].ToString(),
                            HisGluMayD = Reader["HisGluMayD"].ToString(),
                            HisGluMedI = Reader["HisGluMedI"].ToString(),
                            HisGluMedD = Reader["HisGluMedD"].ToString(),
                            HisCuadriI = Reader["HisCuadriI"].ToString(),
                            HisCuadriD = Reader["HisCuadriD"].ToString(),
                            HisIsquiI = Reader["HisIsquiI"].ToString(),
                            HisIsquiD = Reader["HisIsquiD"].ToString(),
                            HisGemeloI = Reader["HisGemeloI"].ToString(),
                            HisGemeloD = Reader["HisGemeloD"].ToString(),
                            HisTibAntI = Reader["HisTibAntI"].ToString(),
                            HisTibAntD = Reader["HisTibAntD"].ToString(),

                            HisEscaAntI = Reader["HisEscaAntI"].ToString(),
                            HisEscaAntD = Reader["HisEscaAntD"].ToString(),
                            HisEscaMedI = Reader["HisEscaMedI"].ToString(),
                            HisEscaMedD = Reader["HisEscaMedD"].ToString(),
                            HisEscaPosI = Reader["HisEscaPosI"].ToString(),
                            HisEscaPosD = Reader["HisEscaPosD"].ToString(),
                            HisECMI = Reader["HisECMI"].ToString(),
                            HisECMD = Reader["HisECMD"].ToString(),
                            HisCualLumI = Reader["HisCualLumI"].ToString(),
                            HisCualLumD = Reader["HisCualLumD"].ToString(),
                            HisCuadI = Reader["HisCuadI"].ToString(),
                            HisCuadD = Reader["HisCuadD"].ToString(),
                            HisIsquiI2 = Reader["HisIsquiI2"].ToString(),
                            HisIsqui2D = Reader["HisIsqui2D"].ToString(),
                            HisTensorI = Reader["HisTensorI"].ToString(),
                            HisTensorD = Reader["HisTensorD"].ToString(),
                            HisGastroI = Reader["HisGastroI"].ToString(),
                            HisGastroD = Reader["HisGastroD"].ToString(),
                            HisTAquilesI = Reader["HisTAquilesI"].ToString(),
                            HisTAquilesD = Reader["HisTAquilesD"].ToString()
                        };

                        return dTF;
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

        Paciente getPacId(string TID, string IDD)
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

                    String Query = "SELECT * " +
                                   "FROM CXN_PACIENTES " +
                                   "WHERE Pac_TipoId = '" + TID + "' " +
                                   "AND Pac_IdNum = '" + IDD + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        Paciente P = new Paciente
                        {
                            PacId = Convert.ToInt32(Reader["Pac_Id"]),
                            Nombre = Reader["Pac_PrimerN"].ToString() + " " +
                                     Reader["Pac_SegundoN"].ToString() + " " +
                                     Reader["Pac_PrimerA"].ToString() + " " +
                                     Reader["Pac_SegundoA"].ToString()
                        };

                        return P;
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

        Dictionary<string, string> getEAV(DatosForInforme I, int PacId)
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

                    String Query = "SELECT TOP 3 HC_Adm " +
                                   "FROM CXN_HCTF " +
                                   "WHERE HC_PacId = '" + PacId + "' " +
                                   "AND HC_Fecha BETWEEN '" + Convert.ToDateTime(I.Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(I.Hasta).ToString("yyyy-MM-dd") + "' " +
                                   "AND TipoHistoria IN ('Ingreso', 'Revaloracion1', 'Revaloracion2') " +
                                   "ORDER BY HC_Fecha ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<DatosTF> P = new List<DatosTF>();
                        Dictionary<string, string> D = new Dictionary<string, string>();

                        while (Reader.Read() == true)
                        {
                            P.Add(new DatosTF
                            {
                                Admision = Convert.ToInt32(Convert.ToInt32(Reader["HC_Adm"]))
                            });
                        }

                        int Contador = 1;

                        foreach (var i in P)
                        {
                            var _eav = Eav(i.Admision);
                            D.Add("EAV" + Contador, _eav.EAV.ToString());

                            D.Add("Cervical_HC_CC1" + Contador, _eav.Cervical_HC_CC1.ToString());
                            D.Add("Cervical_HC_CC2" + Contador, _eav.Cervical_HC_CC2.ToString());
                            D.Add("Cervical_HC_CC3" + Contador, _eav.Cervical_HC_CC3.ToString());
                            D.Add("Cervical_HC_CC4" + Contador, _eav.Cervical_HC_CC4.ToString());
                            D.Add("Cervical_HC_CC5" + Contador, _eav.Cervical_HC_CC5.ToString());
                            D.Add("Cervical_HC_CC6" + Contador, _eav.Cervical_HC_CC6.ToString());

                            D.Add("Cervical_HC_CD1" + Contador, (_eav.Cervical_HC_CD1 != null) ? _eav.Cervical_HC_CD1.ToString() : "");
                            D.Add("Cervical_HC_CD2" + Contador, (_eav.Cervical_HC_CD2 != null) ? _eav.Cervical_HC_CD2.ToString() : "");
                            D.Add("Cervical_HC_CD3" + Contador, (_eav.Cervical_HC_CD3 != null) ? _eav.Cervical_HC_CD3.ToString() : "");
                            D.Add("Cervical_HC_CD4" + Contador, (_eav.Cervical_HC_CD4 != null) ? _eav.Cervical_HC_CD4.ToString() : "");

                            D.Add("Hombro_HC_H1" + Contador, (_eav.Hombro_HC_H1 != null) ? _eav.Hombro_HC_H1.ToString() : "");
                            D.Add("Hombro_HC_H2" + Contador, (_eav.Hombro_HC_H2 != null) ? _eav.Hombro_HC_H2.ToString() : "");
                            D.Add("Hombro_HC_H3" + Contador, (_eav.Hombro_HC_H3 != null) ? _eav.Hombro_HC_H3.ToString() : "");
                            D.Add("Hombro_HC_H4" + Contador, (_eav.Hombro_HC_H4 != null) ? _eav.Hombro_HC_H4.ToString() : "");
                            D.Add("Hombro_HC_H5" + Contador, (_eav.Hombro_HC_H5 != null) ? _eav.Hombro_HC_H5.ToString() : "");
                            D.Add("Hombro_HC_H6" + Contador, (_eav.Hombro_HC_H6 != null) ? _eav.Hombro_HC_H6.ToString() : "");

                            D.Add("Cadera_HC_C1" + Contador, (_eav.Cadera_HC_C1 != null) ? _eav.Cadera_HC_C1.ToString() : "");
                            D.Add("Cadera_HC_C2" + Contador, (_eav.Cadera_HC_C2 != null) ? _eav.Cadera_HC_C2.ToString() : "");
                            D.Add("Cadera_HC_C3" + Contador, (_eav.Cadera_HC_C3 != null) ? _eav.Cadera_HC_C3.ToString() : "");
                            D.Add("Cadera_HC_C4" + Contador, (_eav.Cadera_HC_C4 != null) ? _eav.Cadera_HC_C4.ToString() : "");
                            D.Add("Cadera_HC_C5" + Contador, (_eav.Cadera_HC_C5 != null) ? _eav.Cadera_HC_C5.ToString() : "");
                            D.Add("Cadera_HC_C6" + Contador, (_eav.Cadera_HC_C6 != null) ? _eav.Cadera_HC_C6.ToString() : "");

                            D.Add("Codo_HC_T1A" + Contador, (_eav.Codo_HC_T1 != null) ? _eav.Codo_HC_T1.ToString() : "");
                            D.Add("Codo_HC_T2A" + Contador, (_eav.Codo_HC_T2 != null) ? _eav.Codo_HC_T2.ToString() : "");

                            D.Add("Rodilla_HC_T3" + Contador, (_eav.Rodilla_HC_T3 != null) ? _eav.Rodilla_HC_T3.ToString() : "");
                            D.Add("Rodilla_HC_T4" + Contador, (_eav.Rodilla_HC_T4 != null) ? _eav.Rodilla_HC_T4.ToString() : "");

                            D.Add("Antebrazo_HC_T5" + Contador, (_eav.Antebrazo_HC_T5 != null) ? _eav.Antebrazo_HC_T5.ToString() : "");
                            D.Add("Antebrazo_HC_T6" + Contador, (_eav.Antebrazo_HC_T6 != null) ? _eav.Antebrazo_HC_T6.ToString() : "");

                            D.Add("Tobillo_HC_T7" + Contador, (_eav.Tobillo_HC_T7 != null) ? _eav.Tobillo_HC_T7.ToString() : "");
                            D.Add("Tobillo_HC_T8" + Contador, (_eav.Tobillo_HC_T8 != null) ? _eav.Tobillo_HC_T8.ToString() : "");
                            D.Add("Tobillo_HC_T9" + Contador, (_eav.Tobillo_HC_T9 != null) ? _eav.Tobillo_HC_T9.ToString() : "");
                            D.Add("Tobillo_HC_T10" + Contador, (_eav.Tobillo_HC_T10 != null) ? _eav.Tobillo_HC_T10.ToString() : "");

                            D.Add("RadioCarpiana_HC_T11" + Contador, (_eav.RadioCarpiana_HC_T11 != null) ? _eav.RadioCarpiana_HC_T11.ToString() : "");
                            D.Add("RadioCarpiana_HC_T12" + Contador, (_eav.RadioCarpiana_HC_T12 != null) ? _eav.RadioCarpiana_HC_T12.ToString() : "");
                            D.Add("RadioCarpiana_HC_T13" + Contador, (_eav.RadioCarpiana_HC_T13 != null) ? _eav.RadioCarpiana_HC_T13.ToString() : "");
                            D.Add("RadioCarpiana_HC_T14" + Contador, (_eav.RadioCarpiana_HC_T14 != null) ? _eav.RadioCarpiana_HC_T14.ToString() : "");

                            //Table 3
                            D.Add("HisExtCui" + Contador, (_eav.HisExtCui != null) ? _eav.HisExtCui.ToString() : "");
                            D.Add("HisExtCuD" + Contador, (_eav.HisExtCuD != null) ? _eav.HisExtCuD.ToString() : "");
                            D.Add("HisEscalenosI" + Contador, (_eav.HisEscalenosI != null) ? _eav.HisEscalenosI.ToString() : "");
                            D.Add("HisEscalenosD" + Contador, (_eav.HisEscalenosD != null) ? _eav.HisEscalenosD.ToString() : "");
                            D.Add("HisTrapSupI" + Contador, (_eav.HisTrapSupI != null) ? _eav.HisTrapSupI.ToString() : "");
                            D.Add("HisTrapSupD" + Contador, (_eav.HisTrapSupD != null) ? _eav.HisTrapSupD.ToString() : "");
                            D.Add("HisTrapMedI" + Contador, (_eav.HisTrapMedI != null) ? _eav.HisTrapMedI.ToString() : "");
                            D.Add("HisTrapMedD" + Contador, (_eav.HisTrapMedD != null) ? _eav.HisTrapMedD.ToString() : "");
                            D.Add("HisTrapInfI" + Contador, (_eav.HisTrapInfI != null) ? _eav.HisTrapInfI.ToString() : "");
                            D.Add("HisTrapInfD" + Contador, (_eav.HisTrapInfD != null) ? _eav.HisTrapInfD.ToString() : "");
                            D.Add("HisSerAntI" + Contador, (_eav.HisSerAntI != null) ? _eav.HisSerAntI.ToString() : "");
                            D.Add("HisSerAntD" + Contador, (_eav.HisSerAntD != null) ? _eav.HisSerAntD.ToString() : "");
                            D.Add("HisPecMayI" + Contador, (_eav.HisPecMayI != null) ? _eav.HisPecMayI.ToString() : "");
                            D.Add("HisPecMayD" + Contador, (_eav.HisPecMayD != null) ? _eav.HisPecMayD.ToString() : "");
                            D.Add("HisRomI" + Contador, (_eav.HisRomI != null) ? _eav.HisRomI.ToString() : "");
                            D.Add("HisRomD" + Contador, (_eav.HisRomD != null) ? _eav.HisRomD.ToString() : "");
                            D.Add("HisAbSupI" + Contador, (_eav.HisAbSupI != null) ? _eav.HisAbSupI.ToString() : "");
                            D.Add("HisAbSupD" + Contador, (_eav.HisAbSupD != null) ? _eav.HisAbSupD.ToString() : "");
                            D.Add("HisAbdInfI" + Contador, (_eav.HisAbdInfI != null) ? _eav.HisAbdInfI.ToString() : "");
                            D.Add("HisAbdInfD" + Contador, (_eav.HisAbdInfD != null) ? _eav.HisAbdInfD.ToString() : "");
                            D.Add("HisOblicuoI" + Contador, (_eav.HisOblicuoI != null) ? _eav.HisOblicuoI.ToString() : "");
                            D.Add("HisOblicuoD" + Contador, (_eav.HisOblicuoD != null) ? _eav.HisOblicuoD.ToString() : "");
                            D.Add("HisExtDorsalI" + Contador, (_eav.HisExtDorsalI != null) ? _eav.HisExtDorsalI.ToString() : "");
                            D.Add("HisExtDorsalD" + Contador, (_eav.HisExtDorsalD != null) ? _eav.HisExtDorsalD.ToString() : "");
                            D.Add("HisExtLumbarI" + Contador, (_eav.HisExtLumbarI != null) ? _eav.HisExtLumbarI.ToString() : "");
                            D.Add("HisExtLumbarD" + Contador, (_eav.HisExtLumbarD != null) ? _eav.HisExtLumbarD.ToString() : "");
                            D.Add("HisGluMayI" + Contador, (_eav.HisGluMayI != null) ? _eav.HisGluMayI.ToString() : "");
                            D.Add("HisGluMayD" + Contador, (_eav.HisGluMayD != null) ? _eav.HisGluMayD.ToString() : "");
                            D.Add("HisGluMedI" + Contador, (_eav.HisGluMedI != null) ? _eav.HisGluMedI.ToString() : "");
                            D.Add("HisGluMedD" + Contador, (_eav.HisGluMedD != null) ? _eav.HisGluMedD.ToString() : "");
                            D.Add("HisCuadriI" + Contador, (_eav.HisExtCui != null) ? _eav.HisCuadriI.ToString() : "");
                            D.Add("HisCuadriD" + Contador, (_eav.HisCuadriD != null) ? _eav.HisCuadriD.ToString() : "");
                            D.Add("HisIsquiI" + Contador, (_eav.HisIsquiI != null) ? _eav.HisIsquiI.ToString() : "");
                            D.Add("HisIsquiD" + Contador, (_eav.HisIsquiD != null) ? _eav.HisIsquiD.ToString() : "");
                            D.Add("HisGemeloI" + Contador, (_eav.HisGemeloI != null) ? _eav.HisGemeloI.ToString() : "");
                            D.Add("HisGemeloD" + Contador, (_eav.HisGemeloD != null) ? _eav.HisGemeloD.ToString() : "");
                            D.Add("HisTibAntI" + Contador, (_eav.HisTibAntI != null) ? _eav.HisTibAntI.ToString() : "");
                            D.Add("HisTibAntD" + Contador, (_eav.HisTibAntD != null) ? _eav.HisTibAntD.ToString() : "");

                            D.Add("HisEscaAntI" + Contador, (_eav.HisEscaAntI != null) ? _eav.HisEscaAntI.ToString() : "");
                            D.Add("HisEscaAntD" + Contador, (_eav.HisEscaAntD != null) ? _eav.HisEscaAntD.ToString() : "");
                            D.Add("HisEscaMedI" + Contador, (_eav.HisEscaMedI != null) ? _eav.HisEscaMedI.ToString() : "");
                            D.Add("HisEscaMedD" + Contador, (_eav.HisEscaMedD != null) ? _eav.HisEscaMedD.ToString() : "");
                            D.Add("HisEscaPosI" + Contador, (_eav.HisEscaPosI != null) ? _eav.HisEscaPosI.ToString() : "");
                            D.Add("HisEscaPosD" + Contador, (_eav.HisEscaPosD != null) ? _eav.HisEscaPosD.ToString() : "");
                            D.Add("HisECMI" + Contador, (_eav.HisECMI != null) ? _eav.HisECMI.ToString() : "");
                            D.Add("HisECMD" + Contador, (_eav.HisECMD != null) ? _eav.HisECMD.ToString() : "");
                            D.Add("HisCualLumI" + Contador, (_eav.HisCualLumI != null) ? _eav.HisCualLumI.ToString() : "");
                            D.Add("HisCualLumD" + Contador, (_eav.HisCualLumD != null) ? _eav.HisCualLumD.ToString() : "");
                            D.Add("HisCuadI" + Contador, (_eav.HisCuadI != null) ? _eav.HisCuadI.ToString() : "");
                            D.Add("HisCuadD" + Contador, (_eav.HisCuadD != null) ? _eav.HisCuadD.ToString() : "");
                            D.Add("HisIsquiI2" + Contador, (_eav.HisIsquiI2 != null) ? _eav.HisIsquiI2.ToString() : "");
                            D.Add("HisIsqui2D" + Contador, (_eav.HisIsqui2D != null) ? _eav.HisIsqui2D.ToString() : "");
                            D.Add("HisTensorI" + Contador, (_eav.HisTensorI != null) ? _eav.HisTensorI.ToString() : "");
                            D.Add("HisTensorD" + Contador, (_eav.HisTensorD != null) ? _eav.HisTensorD.ToString() : "");
                            D.Add("HisGastroI" + Contador, (_eav.HisGastroI != null) ? _eav.HisGastroI.ToString() : "");
                            D.Add("HisGastroD" + Contador, (_eav.HisGastroD != null) ? _eav.HisGastroD.ToString() : "");
                            D.Add("HisTAquilesI" + Contador, (_eav.HisTAquilesI != null) ? _eav.HisTAquilesI.ToString() : "");
                            D.Add("HisTAquilesD" + Contador, (_eav.HisTAquilesD != null) ? _eav.HisTAquilesD.ToString() : "");

                            Contador = Contador + 1;
                        }

                        return D;
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

        List<ClaseReportsFibro> ITFGeneral.ExportarInforme1TF(DatosForInforme I)
        {
            try
            {
                var DatoPaciente = getPacId(I.TID, I.IDD);
                if (DatoPaciente == null)
                {
                    return null;
                }

                var getEAVGraph = getEAV(I, DatoPaciente.PacId);
                if (getEAVGraph == null)
                {
                    return null;
                }

                int E = 0; int R1 = 0; int R2 = 0;

                string Cervical_HC_CC1_His1 = "";
                string Cervical_HC_CC2_His1 = "";
                string Cervical_HC_CC3_His1 = "";
                string Cervical_HC_CC4_His1 = "";
                string Cervical_HC_CC5_His1 = "";
                string Cervical_HC_CC6_His1 = "";

                string Cervical_HC_CC1_His2 = "";
                string Cervical_HC_CC2_His2 = "";
                string Cervical_HC_CC3_His2 = "";
                string Cervical_HC_CC4_His2 = "";
                string Cervical_HC_CC5_His2 = "";
                string Cervical_HC_CC6_His2 = "";

                string Cervical_HC_CC1_His3 = "";
                string Cervical_HC_CC2_His3 = "";
                string Cervical_HC_CC3_His3 = "";
                string Cervical_HC_CC4_His3 = "";
                string Cervical_HC_CC5_His3 = "";
                string Cervical_HC_CC6_His3 = "";

                string Cervical_HC_CD1_His1 = "";
                string Cervical_HC_CD2_His1 = "";
                string Cervical_HC_CD3_His1 = "";
                string Cervical_HC_CD4_His1 = "";

                string Cervical_HC_CD1_His2 = "";
                string Cervical_HC_CD2_His2 = "";
                string Cervical_HC_CD3_His2 = "";
                string Cervical_HC_CD4_His2 = "";

                string Cervical_HC_CD1_His3 = "";
                string Cervical_HC_CD2_His3 = "";
                string Cervical_HC_CD3_His3 = "";
                string Cervical_HC_CD4_His3 = "";

                string Hombro_HC_H1_His1 = "";
                string Hombro_HC_H2_His1 = "";
                string Hombro_HC_H3_His1 = "";
                string Hombro_HC_H4_His1 = "";
                string Hombro_HC_H5_His1 = "";
                string Hombro_HC_H6_His1 = "";

                string Hombro_HC_H1_His2 = "";
                string Hombro_HC_H2_His2 = "";
                string Hombro_HC_H3_His2 = "";
                string Hombro_HC_H4_His2 = "";
                string Hombro_HC_H5_His2 = "";
                string Hombro_HC_H6_His2 = "";

                string Hombro_HC_H1_His3 = "";
                string Hombro_HC_H2_His3 = "";
                string Hombro_HC_H3_His3 = "";
                string Hombro_HC_H4_His3 = "";
                string Hombro_HC_H5_His3 = "";
                string Hombro_HC_H6_His3 = "";

                string Cadera_HC_C1_His1 = "";
                string Cadera_HC_C2_His1 = "";
                string Cadera_HC_C3_His1 = "";
                string Cadera_HC_C4_His1 = "";
                string Cadera_HC_C5_His1 = "";
                string Cadera_HC_C6_His1 = "";

                string Cadera_HC_C1_His2 = "";
                string Cadera_HC_C2_His2 = "";
                string Cadera_HC_C3_His2 = "";
                string Cadera_HC_C4_His2 = "";
                string Cadera_HC_C5_His2 = "";
                string Cadera_HC_C6_His2 = "";

                string Cadera_HC_C1_His3 = "";
                string Cadera_HC_C2_His3 = "";
                string Cadera_HC_C3_His3 = "";
                string Cadera_HC_C4_His3 = "";
                string Cadera_HC_C5_His3 = "";
                string Cadera_HC_C6_His3 = "";

                string Codo_HC_T1_His1 = "";
                string Codo_HC_T2_His1 = "";

                string Codo_HC_T1_His2 = "";
                string Codo_HC_T2_His2 = "";

                string Codo_HC_T1_His3 = "";
                string Codo_HC_T2_His3 = "";

                string Rodilla_HC_T3_His1 = "";
                string Rodilla_HC_T4_His1 = "";

                string Rodilla_HC_T3_His2 = "";
                string Rodilla_HC_T4_His2 = "";

                string Rodilla_HC_T3_His3 = "";
                string Rodilla_HC_T4_His3 = "";

                string Antebrazo_HC_T5_His1 = "";
                string Antebrazo_HC_T6_His1 = "";

                string Antebrazo_HC_T5_His2 = "";
                string Antebrazo_HC_T6_His2 = "";

                string Antebrazo_HC_T5_His3 = "";
                string Antebrazo_HC_T6_His3 = "";

                string Tobillo_HC_T7_His1 = "";
                string Tobillo_HC_T8_His1 = "";
                string Tobillo_HC_T9_His1 = "";
                string Tobillo_HC_T10_His1 = "";

                string Tobillo_HC_T7_His2 = "";
                string Tobillo_HC_T8_His2 = "";
                string Tobillo_HC_T9_His2 = "";
                string Tobillo_HC_T10_His2 = "";

                string Tobillo_HC_T7_His3 = "";
                string Tobillo_HC_T8_His3 = "";
                string Tobillo_HC_T9_His3 = "";
                string Tobillo_HC_T10_His3 = "";

                string RadioCarpiana_HC_T11_His1 = "";
                string RadioCarpiana_HC_T12_His1 = "";
                string RadioCarpiana_HC_T13_His1 = "";
                string RadioCarpiana_HC_T14_His1 = "";

                string RadioCarpiana_HC_T11_His2 = "";
                string RadioCarpiana_HC_T12_His2 = "";
                string RadioCarpiana_HC_T13_His2 = "";
                string RadioCarpiana_HC_T14_His2 = "";

                string RadioCarpiana_HC_T11_His3 = "";
                string RadioCarpiana_HC_T12_His3 = "";
                string RadioCarpiana_HC_T13_His3 = "";
                string RadioCarpiana_HC_T14_His3 = "";

                //Tabla 3
                string HisExtCui = "";
                string HisExtCuD = "";
                string HisEscalenosI = "";
                string HisEscalenosD = "";
                string HisTrapSupI = "";
                string HisTrapSupD = "";
                string HisTrapMedI = "";
                string HisTrapMedD = "";
                string HisTrapInfI = "";
                string HisTrapInfD = "";
                string HisSerAntI = "";
                string HisSerAntD = "";
                string HisPecMayI = "";
                string HisPecMayD = "";
                string HisRomI = "";
                string HisRomD = "";
                string HisAbSupI = "";
                string HisAbSupD = "";
                string HisAbdInfI = "";
                string HisAbdInfD = "";
                string HisOblicuoI = "";
                string HisOblicuoD = "";
                string HisExtDorsalI = "";
                string HisExtDorsalD = "";
                string HisExtLumbarI = "";
                string HisExtLumbarD = "";
                string HisGluMayI = "";
                string HisGluMayD = "";
                string HisGluMedI = "";
                string HisGluMedD = "";
                string HisCuadriI = "";
                string HisCuadriD = "";
                string HisIsquiI = "";
                string HisIsquiD = "";
                string HisGemeloI = "";
                string HisGemeloD = "";
                string HisTibAntI = "";
                string HisTibAntD = "";

                string HisExtCui2 = "";
                string HisExtCuD2 = "";
                string HisEscalenosI2 = "";
                string HisEscalenosD2 = "";
                string HisTrapSupI2 = "";
                string HisTrapSupD2 = "";
                string HisTrapMedI2 = "";
                string HisTrapMedD2 = "";
                string HisTrapInfI2 = "";
                string HisTrapInfD2 = "";
                string HisSerAntI2 = "";
                string HisSerAntD2 = "";
                string HisPecMayI2 = "";
                string HisPecMayD2 = "";
                string HisRomI2 = "";
                string HisRomD2 = "";
                string HisAbSupI2 = "";
                string HisAbSupD2 = "";
                string HisAbdInfI2 = "";
                string HisAbdInfD2 = "";
                string HisOblicuoI2 = "";
                string HisOblicuoD2 = "";
                string HisExtDorsalI2 = "";
                string HisExtDorsalD2 = "";
                string HisExtLumbarI2 = "";
                string HisExtLumbarD2 = "";
                string HisGluMayI2 = "";
                string HisGluMayD2 = "";
                string HisGluMedI2 = "";
                string HisGluMedD2 = "";
                string HisCuadriI2 = "";
                string HisCuadriD2 = "";
                string HisIsquiI2 = "";
                string HisIsquiD2 = "";
                string HisGemeloI2 = "";
                string HisGemeloD2 = "";
                string HisTibAntI2 = "";
                string HisTibAntD2 = "";

                string HisExtCui3 = "";
                string HisExtCuD3 = "";
                string HisEscalenosI3 = "";
                string HisEscalenosD3 = "";
                string HisTrapSupI3 = "";
                string HisTrapSupD3 = "";
                string HisTrapMedI3 = "";
                string HisTrapMedD3 = "";
                string HisTrapInfI3 = "";
                string HisTrapInfD3 = "";
                string HisSerAntI3 = "";
                string HisSerAntD3 = "";
                string HisPecMayI3 = "";
                string HisPecMayD3 = "";
                string HisRomI3 = "";
                string HisRomD3 = "";
                string HisAbSupI3 = "";
                string HisAbSupD3 = "";
                string HisAbdInfI3 = "";
                string HisAbdInfD3 = "";
                string HisOblicuoI3 = "";
                string HisOblicuoD3 = "";
                string HisExtDorsalI3 = "";
                string HisExtDorsalD3 = "";
                string HisExtLumbarI3 = "";
                string HisExtLumbarD3 = "";
                string HisGluMayI3 = "";
                string HisGluMayD3 = "";
                string HisGluMedI3 = "";
                string HisGluMedD3 = "";
                string HisCuadriI3 = "";
                string HisCuadriD3 = "";
                string HisIsquiI3 = "";
                string HisIsquiD3 = "";
                string HisGemeloI3 = "";
                string HisGemeloD3 = "";
                string HisTibAntI3 = "";
                string HisTibAntD3 = "";

                //TABLA 4
                string HisEscaAntIT4 = "";
                string HisEscaAntDT4 = "";
                string HisEscaMedIT4 = "";
                string HisEscaMedDT4 = "";
                string HisEscaPosIT4 = "";
                string HisEscaPosDT4 = "";
                string HisECMIT4 = "";
                string HisECMDT4 = "";
                string HisCualLumIT4 = "";
                string HisCualLumDT4 = "";
                string HisCuadIT4 = "";
                string HisCuadDT4 = "";
                string HisIsquiI2T4 = "";
                string HisIsqui2DT4 = "";
                string HisTensorIT4 = "";
                string HisTensorDT4 = "";
                string HisGastroIT4 = "";
                string HisGastroDT4 = "";
                string HisTAquilesIT4 = "";
                string HisTAquilesDT4 = "";

                string HisEscaAntIT42 = "";
                string HisEscaAntDT42 = "";
                string HisEscaMedIT42 = "";
                string HisEscaMedDT42 = "";
                string HisEscaPosIT42 = "";
                string HisEscaPosDT42 = "";
                string HisECMIT42 = "";
                string HisECMDT42 = "";
                string HisCualLumIT42 = "";
                string HisCualLumDT42 = "";
                string HisCuadIT42 = "";
                string HisCuadDT42 = "";
                string HisIsquiI2T42 = "";
                string HisIsqui2DT42 = "";
                string HisTensorIT42 = "";
                string HisTensorDT42 = "";
                string HisGastroIT42 = "";
                string HisGastroDT42 = "";
                string HisTAquilesIT42 = "";
                string HisTAquilesDT42 = "";

                string HisEscaAntIT43 = "";
                string HisEscaAntDT43 = "";
                string HisEscaMedIT43 = "";
                string HisEscaMedDT43 = "";
                string HisEscaPosIT43 = "";
                string HisEscaPosDT43 = "";
                string HisECMIT43 = "";
                string HisECMDT43 = "";
                string HisCualLumIT43 = "";
                string HisCualLumDT43 = "";
                string HisCuadIT43 = "";
                string HisCuadDT43 = "";
                string HisIsquiI2T43 = "";
                string HisIsqui2DT43 = "";
                string HisTensorIT43 = "";
                string HisTensorDT43 = "";
                string HisGastroIT43 = "";
                string HisGastroDT43 = "";
                string HisTAquilesIT43 = "";
                string HisTAquilesDT43 = "";

                if (getEAVGraph.Count == 285)
                {
                    E = Convert.ToInt32(getEAVGraph["EAV1"]);
                    R1 = Convert.ToInt32(getEAVGraph["EAV2"]);
                    R2 = Convert.ToInt32(getEAVGraph["EAV3"]);

                    Cervical_HC_CC1_His1 = getEAVGraph["Cervical_HC_CC11"];
                    Cervical_HC_CC2_His1 = getEAVGraph["Cervical_HC_CC21"];
                    Cervical_HC_CC3_His1 = getEAVGraph["Cervical_HC_CC31"];
                    Cervical_HC_CC4_His1 = getEAVGraph["Cervical_HC_CC41"];
                    Cervical_HC_CC5_His1 = getEAVGraph["Cervical_HC_CC51"];
                    Cervical_HC_CC6_His1 = getEAVGraph["Cervical_HC_CC61"];

                    Cervical_HC_CC1_His2 = getEAVGraph["Cervical_HC_CC12"];
                    Cervical_HC_CC2_His2 = getEAVGraph["Cervical_HC_CC22"];
                    Cervical_HC_CC3_His2 = getEAVGraph["Cervical_HC_CC32"];
                    Cervical_HC_CC4_His2 = getEAVGraph["Cervical_HC_CC42"];
                    Cervical_HC_CC5_His2 = getEAVGraph["Cervical_HC_CC52"];
                    Cervical_HC_CC6_His2 = getEAVGraph["Cervical_HC_CC62"];

                    Cervical_HC_CC1_His3 = getEAVGraph["Cervical_HC_CC13"];
                    Cervical_HC_CC2_His3 = getEAVGraph["Cervical_HC_CC23"];
                    Cervical_HC_CC3_His3 = getEAVGraph["Cervical_HC_CC33"];
                    Cervical_HC_CC4_His3 = getEAVGraph["Cervical_HC_CC43"];
                    Cervical_HC_CC5_His3 = getEAVGraph["Cervical_HC_CC53"];
                    Cervical_HC_CC6_His3 = getEAVGraph["Cervical_HC_CC63"];

                    Cervical_HC_CD1_His1 = getEAVGraph["Cervical_HC_CD11"];
                    Cervical_HC_CD2_His1 = getEAVGraph["Cervical_HC_CD21"];
                    Cervical_HC_CD3_His1 = getEAVGraph["Cervical_HC_CD31"];
                    Cervical_HC_CD4_His1 = getEAVGraph["Cervical_HC_CD41"];

                    Cervical_HC_CD1_His2 = getEAVGraph["Cervical_HC_CD12"];
                    Cervical_HC_CD2_His2 = getEAVGraph["Cervical_HC_CD22"];
                    Cervical_HC_CD3_His2 = getEAVGraph["Cervical_HC_CD32"];
                    Cervical_HC_CD4_His2 = getEAVGraph["Cervical_HC_CD42"];

                    Cervical_HC_CD1_His3 = getEAVGraph["Cervical_HC_CD13"];
                    Cervical_HC_CD2_His3 = getEAVGraph["Cervical_HC_CD23"];
                    Cervical_HC_CD3_His3 = getEAVGraph["Cervical_HC_CD33"];
                    Cervical_HC_CD4_His3 = getEAVGraph["Cervical_HC_CD43"];

                    Hombro_HC_H1_His1 = getEAVGraph["Hombro_HC_H11"];
                    Hombro_HC_H2_His1 = getEAVGraph["Hombro_HC_H21"];
                    Hombro_HC_H3_His1 = getEAVGraph["Hombro_HC_H31"];
                    Hombro_HC_H4_His1 = getEAVGraph["Hombro_HC_H41"];
                    Hombro_HC_H5_His1 = getEAVGraph["Hombro_HC_H51"];
                    Hombro_HC_H6_His1 = getEAVGraph["Hombro_HC_H61"];

                    Hombro_HC_H1_His2 = getEAVGraph["Hombro_HC_H12"];
                    Hombro_HC_H2_His2 = getEAVGraph["Hombro_HC_H22"];
                    Hombro_HC_H3_His2 = getEAVGraph["Hombro_HC_H32"];
                    Hombro_HC_H4_His2 = getEAVGraph["Hombro_HC_H42"];
                    Hombro_HC_H5_His2 = getEAVGraph["Hombro_HC_H52"];
                    Hombro_HC_H6_His2 = getEAVGraph["Hombro_HC_H62"];

                    Hombro_HC_H1_His3 = getEAVGraph["Hombro_HC_H13"];
                    Hombro_HC_H2_His3 = getEAVGraph["Hombro_HC_H23"];
                    Hombro_HC_H3_His3 = getEAVGraph["Hombro_HC_H33"];
                    Hombro_HC_H4_His3 = getEAVGraph["Hombro_HC_H43"];
                    Hombro_HC_H5_His3 = getEAVGraph["Hombro_HC_H53"];
                    Hombro_HC_H6_His3 = getEAVGraph["Hombro_HC_H63"];

                    Cadera_HC_C1_His1 = getEAVGraph["Cadera_HC_C11"];
                    Cadera_HC_C2_His1 = getEAVGraph["Cadera_HC_C21"];
                    Cadera_HC_C3_His1 = getEAVGraph["Cadera_HC_C31"];
                    Cadera_HC_C4_His1 = getEAVGraph["Cadera_HC_C41"];
                    Cadera_HC_C5_His1 = getEAVGraph["Cadera_HC_C51"];
                    Cadera_HC_C6_His1 = getEAVGraph["Cadera_HC_C61"];

                    Cadera_HC_C1_His2 = getEAVGraph["Cadera_HC_C12"];
                    Cadera_HC_C2_His2 = getEAVGraph["Cadera_HC_C22"];
                    Cadera_HC_C3_His2 = getEAVGraph["Cadera_HC_C32"];
                    Cadera_HC_C4_His2 = getEAVGraph["Cadera_HC_C42"];
                    Cadera_HC_C5_His2 = getEAVGraph["Cadera_HC_C52"];
                    Cadera_HC_C6_His2 = getEAVGraph["Cadera_HC_C62"];

                    Cadera_HC_C1_His3 = getEAVGraph["Cadera_HC_C13"];
                    Cadera_HC_C2_His3 = getEAVGraph["Cadera_HC_C23"];
                    Cadera_HC_C3_His3 = getEAVGraph["Cadera_HC_C33"];
                    Cadera_HC_C4_His3 = getEAVGraph["Cadera_HC_C43"];
                    Cadera_HC_C5_His3 = getEAVGraph["Cadera_HC_C53"];
                    Cadera_HC_C6_His3 = getEAVGraph["Cadera_HC_C63"];

                    Codo_HC_T1_His1 = getEAVGraph["Codo_HC_T1A1"];
                    Codo_HC_T2_His1 = getEAVGraph["Codo_HC_T2A1"];

                    Codo_HC_T1_His2 = getEAVGraph["Codo_HC_T1A2"];
                    Codo_HC_T2_His2 = getEAVGraph["Codo_HC_T2A2"];

                    Codo_HC_T1_His3 = getEAVGraph["Codo_HC_T1A3"];
                    Codo_HC_T2_His3 = getEAVGraph["Codo_HC_T2A3"];

                    Rodilla_HC_T3_His1 = getEAVGraph["Rodilla_HC_T31"];
                    Rodilla_HC_T4_His1 = getEAVGraph["Rodilla_HC_T41"];

                    Rodilla_HC_T3_His2 = getEAVGraph["Rodilla_HC_T32"];
                    Rodilla_HC_T4_His2 = getEAVGraph["Rodilla_HC_T42"];

                    Rodilla_HC_T3_His3 = getEAVGraph["Rodilla_HC_T33"];
                    Rodilla_HC_T4_His3 = getEAVGraph["Rodilla_HC_T43"];

                    Antebrazo_HC_T5_His1 = getEAVGraph["Antebrazo_HC_T51"];
                    Antebrazo_HC_T6_His1 = getEAVGraph["Antebrazo_HC_T61"];

                    Antebrazo_HC_T5_His2 = getEAVGraph["Antebrazo_HC_T52"];
                    Antebrazo_HC_T6_His2 = getEAVGraph["Antebrazo_HC_T62"];

                    Antebrazo_HC_T5_His3 = getEAVGraph["Antebrazo_HC_T53"];
                    Antebrazo_HC_T6_His3 = getEAVGraph["Antebrazo_HC_T63"];

                    Tobillo_HC_T7_His1 = getEAVGraph["Tobillo_HC_T71"];
                    Tobillo_HC_T8_His1 = getEAVGraph["Tobillo_HC_T81"];
                    Tobillo_HC_T9_His1 = getEAVGraph["Tobillo_HC_T91"];
                    Tobillo_HC_T10_His1 = getEAVGraph["Tobillo_HC_T101"];

                    Tobillo_HC_T7_His2 = getEAVGraph["Tobillo_HC_T72"];
                    Tobillo_HC_T8_His2 = getEAVGraph["Tobillo_HC_T82"];
                    Tobillo_HC_T9_His2 = getEAVGraph["Tobillo_HC_T92"];
                    Tobillo_HC_T10_His2 = getEAVGraph["Tobillo_HC_T102"];

                    Tobillo_HC_T7_His3 = getEAVGraph["Tobillo_HC_T73"];
                    Tobillo_HC_T8_His3 = getEAVGraph["Tobillo_HC_T83"];
                    Tobillo_HC_T9_His3 = getEAVGraph["Tobillo_HC_T93"];
                    Tobillo_HC_T10_His3 = getEAVGraph["Tobillo_HC_T103"];

                    RadioCarpiana_HC_T11_His1 = getEAVGraph["RadioCarpiana_HC_T111"];
                    RadioCarpiana_HC_T12_His1 = getEAVGraph["RadioCarpiana_HC_T121"];
                    RadioCarpiana_HC_T13_His1 = getEAVGraph["RadioCarpiana_HC_T131"];
                    RadioCarpiana_HC_T14_His1 = getEAVGraph["RadioCarpiana_HC_T141"];

                    RadioCarpiana_HC_T11_His2 = getEAVGraph["RadioCarpiana_HC_T112"];
                    RadioCarpiana_HC_T12_His2 = getEAVGraph["RadioCarpiana_HC_T122"];
                    RadioCarpiana_HC_T13_His2 = getEAVGraph["RadioCarpiana_HC_T132"];
                    RadioCarpiana_HC_T14_His2 = getEAVGraph["RadioCarpiana_HC_T142"];

                    RadioCarpiana_HC_T11_His3 = getEAVGraph["RadioCarpiana_HC_T113"];
                    RadioCarpiana_HC_T12_His3 = getEAVGraph["RadioCarpiana_HC_T123"];
                    RadioCarpiana_HC_T13_His3 = getEAVGraph["RadioCarpiana_HC_T133"];
                    RadioCarpiana_HC_T14_His3 = getEAVGraph["RadioCarpiana_HC_T143"];

                    //Tabla 3
                    HisExtCui = getEAVGraph["HisExtCui1"];
                    HisExtCuD = getEAVGraph["HisExtCuD1"];
                    HisEscalenosI = getEAVGraph["HisEscalenosI1"];
                    HisEscalenosD = getEAVGraph["HisEscalenosD1"];
                    HisTrapSupI = getEAVGraph["HisTrapSupI1"];
                    HisTrapSupD = getEAVGraph["HisTrapSupD1"];
                    HisTrapMedI = getEAVGraph["HisTrapMedI1"];
                    HisTrapMedD = getEAVGraph["HisTrapMedD1"];
                    HisTrapInfI = getEAVGraph["HisTrapInfI1"];
                    HisTrapInfD = getEAVGraph["HisTrapInfD1"];
                    HisSerAntI = getEAVGraph["HisSerAntI1"];
                    HisSerAntD = getEAVGraph["HisSerAntD1"];
                    HisPecMayI = getEAVGraph["HisPecMayI1"];
                    HisPecMayD = getEAVGraph["HisPecMayD1"];
                    HisRomI = getEAVGraph["HisRomI1"];
                    HisRomD = getEAVGraph["HisRomD1"];
                    HisAbSupI = getEAVGraph["HisAbSupI1"];
                    HisAbSupD = getEAVGraph["HisAbSupD1"];
                    HisAbdInfI = getEAVGraph["HisAbdInfI1"];
                    HisAbdInfD = getEAVGraph["HisAbdInfD1"];
                    HisOblicuoI = getEAVGraph["HisOblicuoI1"];
                    HisOblicuoD = getEAVGraph["HisOblicuoD1"];
                    HisExtDorsalI = getEAVGraph["HisExtDorsalI1"];
                    HisExtDorsalD = getEAVGraph["HisExtDorsalD1"];
                    HisExtLumbarI = getEAVGraph["HisExtLumbarI1"];
                    HisExtLumbarD = getEAVGraph["HisExtLumbarD1"];
                    HisGluMayI = getEAVGraph["HisGluMayI1"];
                    HisGluMayD = getEAVGraph["HisGluMayD1"];
                    HisGluMedI = getEAVGraph["HisGluMedI1"];
                    HisGluMedD = getEAVGraph["HisGluMedD1"];
                    HisCuadriI = getEAVGraph["HisCuadriI1"];
                    HisCuadriD = getEAVGraph["HisCuadriD1"];
                    HisIsquiI = getEAVGraph["HisIsquiI1"];
                    HisIsquiD = getEAVGraph["HisIsquiD1"];
                    HisGemeloI = getEAVGraph["HisGemeloI1"];
                    HisGemeloD = getEAVGraph["HisGemeloD1"];
                    HisTibAntI = getEAVGraph["HisTibAntI1"];
                    HisTibAntD = getEAVGraph["HisTibAntD1"];

                    HisExtCui2 = getEAVGraph["HisExtCui2"];
                    HisExtCuD2 = getEAVGraph["HisExtCuD2"];
                    HisEscalenosI2 = getEAVGraph["HisEscalenosI2"];
                    HisEscalenosD2 = getEAVGraph["HisEscalenosD2"];
                    HisTrapSupI2 = getEAVGraph["HisTrapSupI2"];
                    HisTrapSupD2 = getEAVGraph["HisTrapSupD2"];
                    HisTrapMedI2 = getEAVGraph["HisTrapMedI2"];
                    HisTrapMedD2 = getEAVGraph["HisTrapMedD2"];
                    HisTrapInfI2 = getEAVGraph["HisTrapInfI2"];
                    HisTrapInfD2 = getEAVGraph["HisTrapInfD2"];
                    HisSerAntI2 = getEAVGraph["HisSerAntI2"];
                    HisSerAntD2 = getEAVGraph["HisSerAntD2"];
                    HisPecMayI2 = getEAVGraph["HisPecMayI2"];
                    HisPecMayD2 = getEAVGraph["HisPecMayD2"];
                    HisRomI2 = getEAVGraph["HisRomI2"];
                    HisRomD2 = getEAVGraph["HisRomD2"];
                    HisAbSupI2 = getEAVGraph["HisAbSupI2"];
                    HisAbSupD2 = getEAVGraph["HisAbSupD2"];
                    HisAbdInfI2 = getEAVGraph["HisAbdInfI2"];
                    HisAbdInfD2 = getEAVGraph["HisAbdInfD2"];
                    HisOblicuoI2 = getEAVGraph["HisOblicuoI2"];
                    HisOblicuoD2 = getEAVGraph["HisOblicuoD2"];
                    HisExtDorsalI2 = getEAVGraph["HisExtDorsalI2"];
                    HisExtDorsalD2 = getEAVGraph["HisExtDorsalD2"];
                    HisExtLumbarI2 = getEAVGraph["HisExtLumbarI2"];
                    HisExtLumbarD2 = getEAVGraph["HisExtLumbarD2"];
                    HisGluMayI2 = getEAVGraph["HisGluMayI2"];
                    HisGluMayD2 = getEAVGraph["HisGluMayD2"];
                    HisGluMedI2 = getEAVGraph["HisGluMedI2"];
                    HisGluMedD2 = getEAVGraph["HisGluMedD2"];
                    HisCuadriI2 = getEAVGraph["HisCuadriI2"];
                    HisCuadriD2 = getEAVGraph["HisCuadriD2"];
                    HisIsquiI2 = getEAVGraph["HisIsquiI2"];
                    HisIsquiD2 = getEAVGraph["HisIsquiD2"];
                    HisGemeloI2 = getEAVGraph["HisGemeloI2"];
                    HisGemeloD2 = getEAVGraph["HisGemeloD2"];
                    HisTibAntI2 = getEAVGraph["HisTibAntI2"];
                    HisTibAntD2 = getEAVGraph["HisTibAntD2"];

                    HisExtCui3 = getEAVGraph["HisExtCui3"];
                    HisExtCuD3 = getEAVGraph["HisExtCuD3"];
                    HisEscalenosI3 = getEAVGraph["HisEscalenosI3"];
                    HisEscalenosD3 = getEAVGraph["HisEscalenosD3"];
                    HisTrapSupI3 = getEAVGraph["HisTrapSupI3"];
                    HisTrapSupD3 = getEAVGraph["HisTrapSupD3"];
                    HisTrapMedI3 = getEAVGraph["HisTrapMedI3"];
                    HisTrapMedD3 = getEAVGraph["HisTrapMedD3"];
                    HisTrapInfI3 = getEAVGraph["HisTrapInfI3"];
                    HisTrapInfD3 = getEAVGraph["HisTrapInfD3"];
                    HisSerAntI3 = getEAVGraph["HisSerAntI3"];
                    HisSerAntD3 = getEAVGraph["HisSerAntD3"];
                    HisPecMayI3 = getEAVGraph["HisPecMayI3"];
                    HisPecMayD3 = getEAVGraph["HisPecMayD3"];
                    HisRomI3 = getEAVGraph["HisRomI3"];
                    HisRomD3 = getEAVGraph["HisRomD3"];
                    HisAbSupI3 = getEAVGraph["HisAbSupI3"];
                    HisAbSupD3 = getEAVGraph["HisAbSupD3"];
                    HisAbdInfI3 = getEAVGraph["HisAbdInfI3"];
                    HisAbdInfD3 = getEAVGraph["HisAbdInfD3"];
                    HisOblicuoI3 = getEAVGraph["HisOblicuoI3"];
                    HisOblicuoD3 = getEAVGraph["HisOblicuoD3"];
                    HisExtDorsalI3 = getEAVGraph["HisExtDorsalI3"];
                    HisExtDorsalD3 = getEAVGraph["HisExtDorsalD3"];
                    HisExtLumbarI3 = getEAVGraph["HisExtLumbarI3"];
                    HisExtLumbarD3 = getEAVGraph["HisExtLumbarD3"];
                    HisGluMayI3 = getEAVGraph["HisGluMayI3"];
                    HisGluMayD3 = getEAVGraph["HisGluMayD3"];
                    HisGluMedI3 = getEAVGraph["HisGluMedI3"];
                    HisGluMedD3 = getEAVGraph["HisGluMedD3"];
                    HisCuadriI3 = getEAVGraph["HisCuadriI3"];
                    HisCuadriD3 = getEAVGraph["HisCuadriD3"];
                    HisIsquiI3 = getEAVGraph["HisIsquiI3"];
                    HisIsquiD3 = getEAVGraph["HisIsquiD3"];
                    HisGemeloI3 = getEAVGraph["HisGemeloI3"];
                    HisGemeloD3 = getEAVGraph["HisGemeloD3"];
                    HisTibAntI3 = getEAVGraph["HisTibAntI3"];
                    HisTibAntD3 = getEAVGraph["HisTibAntD3"];

                    //Tabla 4
                    HisEscaAntIT4 = getEAVGraph["HisEscaAntI1"];
                    HisEscaAntDT4 = getEAVGraph["HisEscaAntD1"];
                    HisEscaMedIT4 = getEAVGraph["HisEscaMedI1"];
                    HisEscaMedDT4 = getEAVGraph["HisEscaMedD1"];
                    HisEscaPosIT4 = getEAVGraph["HisEscaPosI1"];
                    HisEscaPosDT4 = getEAVGraph["HisEscaPosD1"];
                    HisECMIT4 = getEAVGraph["HisECMI1"];
                    HisECMDT4 = getEAVGraph["HisECMD1"];
                    HisCualLumIT4 = getEAVGraph["HisCualLumI1"];
                    HisCualLumDT4 = getEAVGraph["HisCualLumD1"];
                    HisCuadIT4 = getEAVGraph["HisCuadI1"];
                    HisCuadDT4 = getEAVGraph["HisCuadD1"];
                    HisIsquiI2T4 = getEAVGraph["HisIsquiI21"];
                    HisIsqui2DT4 = getEAVGraph["HisIsqui2D1"];
                    HisTensorIT4 = getEAVGraph["HisTensorI1"];
                    HisTensorDT4 = getEAVGraph["HisTensorD1"];
                    HisGastroIT4 = getEAVGraph["HisGastroI1"];
                    HisGastroDT4 = getEAVGraph["HisGastroD1"];
                    HisTAquilesIT4 = getEAVGraph["HisTAquilesI1"];
                    HisTAquilesDT4 = getEAVGraph["HisTAquilesD1"];

                    HisEscaAntIT42 = getEAVGraph["HisEscaAntI2"];
                    HisEscaAntDT42 = getEAVGraph["HisEscaAntD2"];
                    HisEscaMedIT42 = getEAVGraph["HisEscaMedI2"];
                    HisEscaMedDT42 = getEAVGraph["HisEscaMedD2"];
                    HisEscaPosIT42 = getEAVGraph["HisEscaPosI2"];
                    HisEscaPosDT42 = getEAVGraph["HisEscaPosD2"];
                    HisECMIT42 = getEAVGraph["HisECMI2"];
                    HisECMDT42 = getEAVGraph["HisECMD2"];
                    HisCualLumIT42 = getEAVGraph["HisCualLumI2"];
                    HisCualLumDT42 = getEAVGraph["HisCualLumD2"];
                    HisCuadIT42 = getEAVGraph["HisCuadI2"];
                    HisCuadDT42 = getEAVGraph["HisCuadD2"];
                    HisIsquiI2T42 = getEAVGraph["HisIsquiI22"];
                    HisIsqui2DT42 = getEAVGraph["HisIsqui2D2"];
                    HisTensorIT42 = getEAVGraph["HisTensorI2"];
                    HisTensorDT42 = getEAVGraph["HisTensorD2"];
                    HisGastroIT42 = getEAVGraph["HisGastroI2"];
                    HisGastroDT42 = getEAVGraph["HisGastroD2"];
                    HisTAquilesIT42 = getEAVGraph["HisTAquilesI2"];
                    HisTAquilesDT42 = getEAVGraph["HisTAquilesD2"];

                    HisEscaAntIT43 = getEAVGraph["HisEscaAntI3"];
                    HisEscaAntDT43 = getEAVGraph["HisEscaAntD3"];
                    HisEscaMedIT43 = getEAVGraph["HisEscaMedI3"];
                    HisEscaMedDT43 = getEAVGraph["HisEscaMedD3"];
                    HisEscaPosIT43 = getEAVGraph["HisEscaPosI3"];
                    HisEscaPosDT43 = getEAVGraph["HisEscaPosD3"];
                    HisECMIT43 = getEAVGraph["HisECMI3"];
                    HisECMDT43 = getEAVGraph["HisECMD3"];
                    HisCualLumIT43 = getEAVGraph["HisCualLumI3"];
                    HisCualLumDT43 = getEAVGraph["HisCualLumD3"];
                    HisCuadIT43 = getEAVGraph["HisCuadI3"];
                    HisCuadDT43 = getEAVGraph["HisCuadD3"];
                    HisIsquiI2T43 = getEAVGraph["HisIsquiI23"];
                    HisIsqui2DT43 = getEAVGraph["HisIsqui2D3"];
                    HisTensorIT43 = getEAVGraph["HisTensorI3"];
                    HisTensorDT43 = getEAVGraph["HisTensorD3"];
                    HisGastroIT43 = getEAVGraph["HisGastroI3"];
                    HisGastroDT43 = getEAVGraph["HisGastroD3"];
                    HisTAquilesIT43 = getEAVGraph["HisTAquilesI3"];
                    HisTAquilesDT43 = getEAVGraph["HisTAquilesD3"];
                }
                else if (getEAVGraph.Count == 190)
                {
                    E = Convert.ToInt32(getEAVGraph["EAV1"]);
                    R1 = Convert.ToInt32(getEAVGraph["EAV2"]);

                    Cervical_HC_CC1_His1 = getEAVGraph["Cervical_HC_CC11"];
                    Cervical_HC_CC2_His1 = getEAVGraph["Cervical_HC_CC21"];
                    Cervical_HC_CC3_His1 = getEAVGraph["Cervical_HC_CC31"];
                    Cervical_HC_CC4_His1 = getEAVGraph["Cervical_HC_CC41"];
                    Cervical_HC_CC5_His1 = getEAVGraph["Cervical_HC_CC51"];
                    Cervical_HC_CC6_His1 = getEAVGraph["Cervical_HC_CC61"];

                    Cervical_HC_CC1_His2 = getEAVGraph["Cervical_HC_CC12"];
                    Cervical_HC_CC2_His2 = getEAVGraph["Cervical_HC_CC22"];
                    Cervical_HC_CC3_His2 = getEAVGraph["Cervical_HC_CC32"];
                    Cervical_HC_CC4_His2 = getEAVGraph["Cervical_HC_CC42"];
                    Cervical_HC_CC5_His2 = getEAVGraph["Cervical_HC_CC52"];
                    Cervical_HC_CC6_His2 = getEAVGraph["Cervical_HC_CC62"];

                    Cervical_HC_CD1_His1 = getEAVGraph["Cervical_HC_CD11"];
                    Cervical_HC_CD2_His1 = getEAVGraph["Cervical_HC_CD21"];
                    Cervical_HC_CD3_His1 = getEAVGraph["Cervical_HC_CD31"];
                    Cervical_HC_CD4_His1 = getEAVGraph["Cervical_HC_CD41"];

                    Cervical_HC_CD1_His2 = getEAVGraph["Cervical_HC_CD12"];
                    Cervical_HC_CD2_His2 = getEAVGraph["Cervical_HC_CD22"];
                    Cervical_HC_CD3_His2 = getEAVGraph["Cervical_HC_CD32"];
                    Cervical_HC_CD4_His2 = getEAVGraph["Cervical_HC_CD42"];

                    Hombro_HC_H1_His1 = getEAVGraph["Hombro_HC_H11"];
                    Hombro_HC_H2_His1 = getEAVGraph["Hombro_HC_H21"];
                    Hombro_HC_H3_His1 = getEAVGraph["Hombro_HC_H31"];
                    Hombro_HC_H4_His1 = getEAVGraph["Hombro_HC_H41"];
                    Hombro_HC_H5_His1 = getEAVGraph["Hombro_HC_H51"];
                    Hombro_HC_H6_His1 = getEAVGraph["Hombro_HC_H61"];

                    Hombro_HC_H1_His2 = getEAVGraph["Hombro_HC_H12"];
                    Hombro_HC_H2_His2 = getEAVGraph["Hombro_HC_H22"];
                    Hombro_HC_H3_His2 = getEAVGraph["Hombro_HC_H32"];
                    Hombro_HC_H4_His2 = getEAVGraph["Hombro_HC_H42"];
                    Hombro_HC_H5_His2 = getEAVGraph["Hombro_HC_H52"];
                    Hombro_HC_H6_His2 = getEAVGraph["Hombro_HC_H62"];

                    Cadera_HC_C1_His1 = getEAVGraph["Cadera_HC_C11"];
                    Cadera_HC_C2_His1 = getEAVGraph["Cadera_HC_C21"];
                    Cadera_HC_C3_His1 = getEAVGraph["Cadera_HC_C31"];
                    Cadera_HC_C4_His1 = getEAVGraph["Cadera_HC_C41"];
                    Cadera_HC_C5_His1 = getEAVGraph["Cadera_HC_C51"];
                    Cadera_HC_C6_His1 = getEAVGraph["Cadera_HC_C61"];

                    Cadera_HC_C1_His2 = getEAVGraph["Cadera_HC_C12"];
                    Cadera_HC_C2_His2 = getEAVGraph["Cadera_HC_C22"];
                    Cadera_HC_C3_His2 = getEAVGraph["Cadera_HC_C32"];
                    Cadera_HC_C4_His2 = getEAVGraph["Cadera_HC_C42"];
                    Cadera_HC_C5_His2 = getEAVGraph["Cadera_HC_C52"];
                    Cadera_HC_C6_His2 = getEAVGraph["Cadera_HC_C62"];

                    Codo_HC_T1_His1 = getEAVGraph["Codo_HC_T1A1"];
                    Codo_HC_T2_His1 = getEAVGraph["Codo_HC_T2A1"];

                    Codo_HC_T1_His2 = getEAVGraph["Codo_HC_T1A2"];
                    Codo_HC_T2_His2 = getEAVGraph["Codo_HC_T2A2"];

                    Rodilla_HC_T3_His1 = getEAVGraph["Rodilla_HC_T31"];
                    Rodilla_HC_T4_His1 = getEAVGraph["Rodilla_HC_T41"];

                    Rodilla_HC_T3_His2 = getEAVGraph["Rodilla_HC_T32"];
                    Rodilla_HC_T4_His2 = getEAVGraph["Rodilla_HC_T42"];

                    Antebrazo_HC_T5_His1 = getEAVGraph["Antebrazo_HC_T51"];
                    Antebrazo_HC_T6_His1 = getEAVGraph["Antebrazo_HC_T61"];

                    Antebrazo_HC_T5_His2 = getEAVGraph["Antebrazo_HC_T52"];
                    Antebrazo_HC_T6_His2 = getEAVGraph["Antebrazo_HC_T62"];

                    Tobillo_HC_T7_His1 = getEAVGraph["Tobillo_HC_T71"];
                    Tobillo_HC_T8_His1 = getEAVGraph["Tobillo_HC_T81"];
                    Tobillo_HC_T9_His1 = getEAVGraph["Tobillo_HC_T91"];
                    Tobillo_HC_T10_His1 = getEAVGraph["Tobillo_HC_T101"];

                    Tobillo_HC_T7_His2 = getEAVGraph["Tobillo_HC_T72"];
                    Tobillo_HC_T8_His2 = getEAVGraph["Tobillo_HC_T82"];
                    Tobillo_HC_T9_His2 = getEAVGraph["Tobillo_HC_T92"];
                    Tobillo_HC_T10_His2 = getEAVGraph["Tobillo_HC_T102"];

                    RadioCarpiana_HC_T11_His1 = getEAVGraph["RadioCarpiana_HC_T111"];
                    RadioCarpiana_HC_T12_His1 = getEAVGraph["RadioCarpiana_HC_T121"];
                    RadioCarpiana_HC_T13_His1 = getEAVGraph["RadioCarpiana_HC_T131"];
                    RadioCarpiana_HC_T14_His1 = getEAVGraph["RadioCarpiana_HC_T141"];

                    RadioCarpiana_HC_T11_His2 = getEAVGraph["RadioCarpiana_HC_T112"];
                    RadioCarpiana_HC_T12_His2 = getEAVGraph["RadioCarpiana_HC_T122"];
                    RadioCarpiana_HC_T13_His2 = getEAVGraph["RadioCarpiana_HC_T132"];
                    RadioCarpiana_HC_T14_His2 = getEAVGraph["RadioCarpiana_HC_T142"];

                    //Tabla 3
                    HisExtCui = getEAVGraph["HisExtCui1"];
                    HisExtCuD = getEAVGraph["HisExtCuD1"];
                    HisEscalenosI = getEAVGraph["HisEscalenosI1"];
                    HisEscalenosD = getEAVGraph["HisEscalenosD1"];
                    HisTrapSupI = getEAVGraph["HisTrapSupI1"];
                    HisTrapSupD = getEAVGraph["HisTrapSupD1"];
                    HisTrapMedI = getEAVGraph["HisTrapMedI1"];
                    HisTrapMedD = getEAVGraph["HisTrapMedD1"];
                    HisTrapInfI = getEAVGraph["HisTrapInfI1"];
                    HisTrapInfD = getEAVGraph["HisTrapInfD1"];
                    HisSerAntI = getEAVGraph["HisSerAntI1"];
                    HisSerAntD = getEAVGraph["HisSerAntD1"];
                    HisPecMayI = getEAVGraph["HisPecMayI1"];
                    HisPecMayD = getEAVGraph["HisPecMayD1"];
                    HisRomI = getEAVGraph["HisRomI1"];
                    HisRomD = getEAVGraph["HisRomD1"];
                    HisAbSupI = getEAVGraph["HisAbSupI1"];
                    HisAbSupD = getEAVGraph["HisAbSupD1"];
                    HisAbdInfI = getEAVGraph["HisAbdInfI1"];
                    HisAbdInfD = getEAVGraph["HisAbdInfD1"];
                    HisOblicuoI = getEAVGraph["HisOblicuoI1"];
                    HisOblicuoD = getEAVGraph["HisOblicuoD1"];
                    HisExtDorsalI = getEAVGraph["HisExtDorsalI1"];
                    HisExtDorsalD = getEAVGraph["HisExtDorsalD1"];
                    HisExtLumbarI = getEAVGraph["HisExtLumbarI1"];
                    HisExtLumbarD = getEAVGraph["HisExtLumbarD1"];
                    HisGluMayI = getEAVGraph["HisGluMayI1"];
                    HisGluMayD = getEAVGraph["HisGluMayD1"];
                    HisGluMedI = getEAVGraph["HisGluMedI1"];
                    HisGluMedD = getEAVGraph["HisGluMedD1"];
                    HisCuadriI = getEAVGraph["HisCuadriI1"];
                    HisCuadriD = getEAVGraph["HisCuadriD1"];
                    HisIsquiI = getEAVGraph["HisIsquiI1"];
                    HisIsquiD = getEAVGraph["HisIsquiD1"];
                    HisGemeloI = getEAVGraph["HisGemeloI1"];
                    HisGemeloD = getEAVGraph["HisGemeloD1"];
                    HisTibAntI = getEAVGraph["HisTibAntI1"];
                    HisTibAntD = getEAVGraph["HisTibAntD1"];

                    HisExtCui2 = getEAVGraph["HisExtCui2"];
                    HisExtCuD2 = getEAVGraph["HisExtCuD2"];
                    HisEscalenosI2 = getEAVGraph["HisEscalenosI2"];
                    HisEscalenosD2 = getEAVGraph["HisEscalenosD2"];
                    HisTrapSupI2 = getEAVGraph["HisTrapSupI2"];
                    HisTrapSupD2 = getEAVGraph["HisTrapSupD2"];
                    HisTrapMedI2 = getEAVGraph["HisTrapMedI2"];
                    HisTrapMedD2 = getEAVGraph["HisTrapMedD2"];
                    HisTrapInfI2 = getEAVGraph["HisTrapInfI2"];
                    HisTrapInfD2 = getEAVGraph["HisTrapInfD2"];
                    HisSerAntI2 = getEAVGraph["HisSerAntI2"];
                    HisSerAntD2 = getEAVGraph["HisSerAntD2"];
                    HisPecMayI2 = getEAVGraph["HisPecMayI2"];
                    HisPecMayD2 = getEAVGraph["HisPecMayD2"];
                    HisRomI2 = getEAVGraph["HisRomI2"];
                    HisRomD2 = getEAVGraph["HisRomD2"];
                    HisAbSupI2 = getEAVGraph["HisAbSupI2"];
                    HisAbSupD2 = getEAVGraph["HisAbSupD2"];
                    HisAbdInfI2 = getEAVGraph["HisAbdInfI2"];
                    HisAbdInfD2 = getEAVGraph["HisAbdInfD2"];
                    HisOblicuoI2 = getEAVGraph["HisOblicuoI2"];
                    HisOblicuoD2 = getEAVGraph["HisOblicuoD2"];
                    HisExtDorsalI2 = getEAVGraph["HisExtDorsalI2"];
                    HisExtDorsalD2 = getEAVGraph["HisExtDorsalD2"];
                    HisExtLumbarI2 = getEAVGraph["HisExtLumbarI2"];
                    HisExtLumbarD2 = getEAVGraph["HisExtLumbarD2"];
                    HisGluMayI2 = getEAVGraph["HisGluMayI2"];
                    HisGluMayD2 = getEAVGraph["HisGluMayD2"];
                    HisGluMedI2 = getEAVGraph["HisGluMedI2"];
                    HisGluMedD2 = getEAVGraph["HisGluMedD2"];
                    HisCuadriI2 = getEAVGraph["HisCuadriI2"];
                    HisCuadriD2 = getEAVGraph["HisCuadriD2"];
                    HisIsquiI2 = getEAVGraph["HisIsquiI2"];
                    HisIsquiD2 = getEAVGraph["HisIsquiD2"];
                    HisGemeloI2 = getEAVGraph["HisGemeloI2"];
                    HisGemeloD2 = getEAVGraph["HisGemeloD2"];
                    HisTibAntI2 = getEAVGraph["HisTibAntI2"];
                    HisTibAntD2 = getEAVGraph["HisTibAntD2"];

                    //Tabla 4
                    HisEscaAntIT4 = getEAVGraph["HisEscaAntI1"];
                    HisEscaAntDT4 = getEAVGraph["HisEscaAntD1"];
                    HisEscaMedIT4 = getEAVGraph["HisEscaMedI1"];
                    HisEscaMedDT4 = getEAVGraph["HisEscaMedD1"];
                    HisEscaPosIT4 = getEAVGraph["HisEscaPosI1"];
                    HisEscaPosDT4 = getEAVGraph["HisEscaPosD1"];
                    HisECMIT4 = getEAVGraph["HisECMI1"];
                    HisECMDT4 = getEAVGraph["HisECMD1"];
                    HisCualLumIT4 = getEAVGraph["HisCualLumI1"];
                    HisCualLumDT4 = getEAVGraph["HisCualLumD1"];
                    HisCuadIT4 = getEAVGraph["HisCuadI1"];
                    HisCuadDT4 = getEAVGraph["HisCuadD1"];
                    HisIsquiI2T4 = getEAVGraph["HisIsquiI21"];
                    HisIsqui2DT4 = getEAVGraph["HisIsqui2D1"];
                    HisTensorIT4 = getEAVGraph["HisTensorI1"];
                    HisTensorDT4 = getEAVGraph["HisTensorD1"];
                    HisGastroIT4 = getEAVGraph["HisGastroI1"];
                    HisGastroDT4 = getEAVGraph["HisGastroD1"];
                    HisTAquilesIT4 = getEAVGraph["HisTAquilesI1"];
                    HisTAquilesDT4 = getEAVGraph["HisTAquilesD1"];

                    HisEscaAntIT42 = getEAVGraph["HisEscaAntI2"];
                    HisEscaAntDT42 = getEAVGraph["HisEscaAntD2"];
                    HisEscaMedIT42 = getEAVGraph["HisEscaMedI2"];
                    HisEscaMedDT42 = getEAVGraph["HisEscaMedD2"];
                    HisEscaPosIT42 = getEAVGraph["HisEscaPosI2"];
                    HisEscaPosDT42 = getEAVGraph["HisEscaPosD2"];
                    HisECMIT42 = getEAVGraph["HisECMI2"];
                    HisECMDT42 = getEAVGraph["HisECMD2"];
                    HisCualLumIT42 = getEAVGraph["HisCualLumI2"];
                    HisCualLumDT42 = getEAVGraph["HisCualLumD2"];
                    HisCuadIT42 = getEAVGraph["HisCuadI2"];
                    HisCuadDT42 = getEAVGraph["HisCuadD2"];
                    HisIsquiI2T42 = getEAVGraph["HisIsquiI22"];
                    HisIsqui2DT42 = getEAVGraph["HisIsqui2D2"];
                    HisTensorIT42 = getEAVGraph["HisTensorI2"];
                    HisTensorDT42 = getEAVGraph["HisTensorD2"];
                    HisGastroIT42 = getEAVGraph["HisGastroI2"];
                    HisGastroDT42 = getEAVGraph["HisGastroD2"];
                    HisTAquilesIT42 = getEAVGraph["HisTAquilesI2"];
                    HisTAquilesDT42 = getEAVGraph["HisTAquilesD2"];
                }
                else if (getEAVGraph.Count == 95)
                {
                    E = Convert.ToInt32(getEAVGraph["EAV1"]);

                    Cervical_HC_CC1_His1 = getEAVGraph["Cervical_HC_CC11"];
                    Cervical_HC_CC2_His1 = getEAVGraph["Cervical_HC_CC21"];
                    Cervical_HC_CC3_His1 = getEAVGraph["Cervical_HC_CC31"];
                    Cervical_HC_CC4_His1 = getEAVGraph["Cervical_HC_CC41"];
                    Cervical_HC_CC5_His1 = getEAVGraph["Cervical_HC_CC51"];
                    Cervical_HC_CC6_His1 = getEAVGraph["Cervical_HC_CC61"];

                    Cervical_HC_CD1_His1 = getEAVGraph["Cervical_HC_CD11"];
                    Cervical_HC_CD2_His1 = getEAVGraph["Cervical_HC_CD21"];
                    Cervical_HC_CD3_His1 = getEAVGraph["Cervical_HC_CD31"];
                    Cervical_HC_CD4_His1 = getEAVGraph["Cervical_HC_CD41"];

                    Hombro_HC_H1_His1 = getEAVGraph["Hombro_HC_H11"];
                    Hombro_HC_H2_His1 = getEAVGraph["Hombro_HC_H21"];
                    Hombro_HC_H3_His1 = getEAVGraph["Hombro_HC_H31"];
                    Hombro_HC_H4_His1 = getEAVGraph["Hombro_HC_H41"];
                    Hombro_HC_H5_His1 = getEAVGraph["Hombro_HC_H51"];
                    Hombro_HC_H6_His1 = getEAVGraph["Hombro_HC_H61"];

                    Cadera_HC_C1_His1 = getEAVGraph["Cadera_HC_C11"];
                    Cadera_HC_C2_His1 = getEAVGraph["Cadera_HC_C21"];
                    Cadera_HC_C3_His1 = getEAVGraph["Cadera_HC_C31"];
                    Cadera_HC_C4_His1 = getEAVGraph["Cadera_HC_C41"];
                    Cadera_HC_C5_His1 = getEAVGraph["Cadera_HC_C51"];
                    Cadera_HC_C6_His1 = getEAVGraph["Cadera_HC_C61"];

                    Codo_HC_T1_His1 = getEAVGraph["Codo_HC_T1A1"];
                    Codo_HC_T2_His1 = getEAVGraph["Codo_HC_T2A1"];

                    Rodilla_HC_T3_His1 = getEAVGraph["Rodilla_HC_T31"];
                    Rodilla_HC_T4_His1 = getEAVGraph["Rodilla_HC_T41"];

                    Antebrazo_HC_T5_His1 = getEAVGraph["Antebrazo_HC_T51"];
                    Antebrazo_HC_T6_His1 = getEAVGraph["Antebrazo_HC_T61"];

                    Tobillo_HC_T7_His1 = getEAVGraph["Tobillo_HC_T71"];
                    Tobillo_HC_T8_His1 = getEAVGraph["Tobillo_HC_T81"];
                    Tobillo_HC_T9_His1 = getEAVGraph["Tobillo_HC_T91"];
                    Tobillo_HC_T10_His1 = getEAVGraph["Tobillo_HC_T101"];

                    RadioCarpiana_HC_T11_His1 = getEAVGraph["RadioCarpiana_HC_T111"];
                    RadioCarpiana_HC_T12_His1 = getEAVGraph["RadioCarpiana_HC_T121"];
                    RadioCarpiana_HC_T13_His1 = getEAVGraph["RadioCarpiana_HC_T131"];
                    RadioCarpiana_HC_T14_His1 = getEAVGraph["RadioCarpiana_HC_T141"];

                    //Tabla 3
                    HisExtCui = getEAVGraph["HisExtCui1"];
                    HisExtCuD = getEAVGraph["HisExtCuD1"];
                    HisEscalenosI = getEAVGraph["HisEscalenosI1"];
                    HisEscalenosD = getEAVGraph["HisEscalenosD1"];
                    HisTrapSupI = getEAVGraph["HisTrapSupI1"];
                    HisTrapSupD = getEAVGraph["HisTrapSupD1"];
                    HisTrapMedI = getEAVGraph["HisTrapMedI1"];
                    HisTrapMedD = getEAVGraph["HisTrapMedD1"];
                    HisTrapInfI = getEAVGraph["HisTrapInfI1"];
                    HisTrapInfD = getEAVGraph["HisTrapInfD1"];
                    HisSerAntI = getEAVGraph["HisSerAntI1"];
                    HisSerAntD = getEAVGraph["HisSerAntD1"];
                    HisPecMayI = getEAVGraph["HisPecMayI1"];
                    HisPecMayD = getEAVGraph["HisPecMayD1"];
                    HisRomI = getEAVGraph["HisRomI1"];
                    HisRomD = getEAVGraph["HisRomD1"];
                    HisAbSupI = getEAVGraph["HisAbSupI1"];
                    HisAbSupD = getEAVGraph["HisAbSupD1"];
                    HisAbdInfI = getEAVGraph["HisAbdInfI1"];
                    HisAbdInfD = getEAVGraph["HisAbdInfD1"];
                    HisOblicuoI = getEAVGraph["HisOblicuoI1"];
                    HisOblicuoD = getEAVGraph["HisOblicuoD1"];
                    HisExtDorsalI = getEAVGraph["HisExtDorsalI1"];
                    HisExtDorsalD = getEAVGraph["HisExtDorsalD1"];
                    HisExtLumbarI = getEAVGraph["HisExtLumbarI1"];
                    HisExtLumbarD = getEAVGraph["HisExtLumbarD1"];
                    HisGluMayI = getEAVGraph["HisGluMayI1"];
                    HisGluMayD = getEAVGraph["HisGluMayD1"];
                    HisGluMedI = getEAVGraph["HisGluMedI1"];
                    HisGluMedD = getEAVGraph["HisGluMedD1"];
                    HisCuadriI = getEAVGraph["HisCuadriI1"];
                    HisCuadriD = getEAVGraph["HisCuadriD1"];
                    HisIsquiI = getEAVGraph["HisIsquiI1"];
                    HisIsquiD = getEAVGraph["HisIsquiD1"];
                    HisGemeloI = getEAVGraph["HisGemeloI1"];
                    HisGemeloD = getEAVGraph["HisGemeloD1"];
                    HisTibAntI = getEAVGraph["HisTibAntI1"];
                    HisTibAntD = getEAVGraph["HisTibAntD1"];

                    //Tabla 4
                    HisEscaAntIT4 = getEAVGraph["HisEscaAntI1"];
                    HisEscaAntDT4 = getEAVGraph["HisEscaAntD1"];
                    HisEscaMedIT4 = getEAVGraph["HisEscaMedI1"];
                    HisEscaMedDT4 = getEAVGraph["HisEscaMedD1"];
                    HisEscaPosIT4 = getEAVGraph["HisEscaPosI1"];
                    HisEscaPosDT4 = getEAVGraph["HisEscaPosD1"];
                    HisECMIT4 = getEAVGraph["HisECMI1"];
                    HisECMDT4 = getEAVGraph["HisECMD1"];
                    HisCualLumIT4 = getEAVGraph["HisCualLumI1"];
                    HisCualLumDT4 = getEAVGraph["HisCualLumD1"];
                    HisCuadIT4 = getEAVGraph["HisCuadI1"];
                    HisCuadDT4 = getEAVGraph["HisCuadD1"];
                    HisIsquiI2T4 = getEAVGraph["HisIsquiI21"];
                    HisIsqui2DT4 = getEAVGraph["HisIsqui2D1"];
                    HisTensorIT4 = getEAVGraph["HisTensorI1"];
                    HisTensorDT4 = getEAVGraph["HisTensorD1"];
                    HisGastroIT4 = getEAVGraph["HisGastroI1"];
                    HisGastroDT4 = getEAVGraph["HisGastroD1"];
                    HisTAquilesIT4 = getEAVGraph["HisTAquilesI1"];
                    HisTAquilesDT4 = getEAVGraph["HisTAquilesD1"];
                }

                var chartGrapchEAV = ChartsGenerateEAV(E.ToString(),
                                                       R1.ToString(),
                                                       R2.ToString());

                Dictionary<int, Dictionary<string, double>> D = new Dictionary<int, Dictionary<string, double>>();


                List<ClaseReportsFibro> dTF = new List<ClaseReportsFibro>();

                dTF.Add(new ClaseReportsFibro
                {
                    Fecha1 = Convert.ToDateTime(I.Desde),
                    Fecha2 = Convert.ToDateTime(I.Hasta),
                    Name = I.Name,
                    Graph = chartGrapchEAV,
                    Dato1 = Cervical_HC_CC1_His1,
                    Dato2 = Cervical_HC_CC1_His2,
                    Dato3 = Cervical_HC_CC1_His3,
                    Dato4 = Cervical_HC_CC2_His1,
                    Dato5 = Cervical_HC_CC2_His2,
                    Dato6 = Cervical_HC_CC2_His3,
                    Dato7 = Cervical_HC_CC4_His1,
                    Dato8 = Cervical_HC_CC4_His2,
                    Dato9 = Cervical_HC_CC4_His3,
                    Dato10 = Cervical_HC_CC3_His1,
                    Dato11 = Cervical_HC_CC3_His2,
                    Dato12 = Cervical_HC_CC3_His3,
                    Dato13 = Cervical_HC_CC5_His1,
                    Dato14 = Cervical_HC_CC5_His2,
                    Dato15 = Cervical_HC_CC5_His3,
                    Dato16 = Cervical_HC_CC6_His1,
                    Dato17 = Cervical_HC_CC6_His2,
                    Dato18 = Cervical_HC_CC6_His3,

                    Dato19 = Cervical_HC_CD1_His1,
                    Dato20 = Cervical_HC_CD1_His2,
                    Dato21 = Cervical_HC_CD1_His3,
                    Dato22 = Cervical_HC_CD2_His1,
                    Dato23 = Cervical_HC_CD2_His2,
                    Dato24 = Cervical_HC_CD2_His3,
                    Dato25 = Cervical_HC_CD3_His1,
                    Dato26 = Cervical_HC_CD3_His2,
                    Dato27 = Cervical_HC_CD3_His3,
                    Dato28 = Cervical_HC_CD4_His1,
                    Dato29 = Cervical_HC_CD4_His2,
                    Dato30 = Cervical_HC_CD4_His3,

                    Dato31 = Hombro_HC_H1_His1,
                    Dato32 = Hombro_HC_H1_His2,
                    Dato33 = Hombro_HC_H1_His3,
                    Dato34 = Hombro_HC_H2_His1,
                    Dato35 = Hombro_HC_H2_His2,
                    Dato36 = Hombro_HC_H2_His3,
                    Dato37 = Hombro_HC_H3_His1,
                    Dato38 = Hombro_HC_H3_His2,
                    Dato39 = Hombro_HC_H3_His3,
                    Dato40 = Hombro_HC_H4_His1,
                    Dato41 = Hombro_HC_H4_His2,
                    Dato42 = Hombro_HC_H4_His3,
                    Dato43 = Hombro_HC_H5_His1,
                    Dato44 = Hombro_HC_H5_His2,
                    Dato45 = Hombro_HC_H5_His3,
                    Dato46 = Hombro_HC_H6_His1,
                    Dato47 = Hombro_HC_H6_His2,
                    Dato48 = Hombro_HC_H6_His3,

                    Dato49 = Cadera_HC_C1_His1,
                    Dato50 = Cadera_HC_C1_His2,
                    Dato51 = Cadera_HC_C1_His3,
                    Dato52 = Cadera_HC_C2_His1,
                    Dato53 = Cadera_HC_C2_His2,
                    Dato54 = Cadera_HC_C2_His3,
                    Dato55 = Cadera_HC_C3_His1,
                    Dato56 = Cadera_HC_C3_His2,
                    Dato57 = Cadera_HC_C3_His3,
                    Dato58 = Cadera_HC_C4_His1,
                    Dato59 = Cadera_HC_C4_His2,
                    Dato60 = Cadera_HC_C4_His3,
                    Dato61 = Cadera_HC_C5_His1,
                    Dato62 = Cadera_HC_C5_His2,
                    Dato63 = Cadera_HC_C5_His3,
                    Dato64 = Cadera_HC_C6_His1,
                    Dato65 = Cadera_HC_C6_His2,
                    Dato66 = Cadera_HC_C6_His3,

                    Dato67 = Codo_HC_T1_His1,
                    Dato68 = Codo_HC_T1_His2,
                    Dato69 = Codo_HC_T1_His3,
                    Dato70 = Codo_HC_T2_His1,
                    Dato71 = Codo_HC_T2_His2,
                    Dato72 = Codo_HC_T2_His3,

                    Dato73 = Rodilla_HC_T3_His1,
                    Dato74 = Rodilla_HC_T3_His2,
                    Dato75 = Rodilla_HC_T3_His3,
                    Dato76 = Rodilla_HC_T4_His1,
                    Dato77 = Rodilla_HC_T4_His2,
                    Dato78 = Rodilla_HC_T4_His3,

                    Dato79 = Antebrazo_HC_T5_His1,
                    Dato80 = Antebrazo_HC_T5_His2,
                    Dato81 = Antebrazo_HC_T5_His3,
                    Dato82 = Antebrazo_HC_T6_His1,
                    Dato83 = Antebrazo_HC_T6_His2,
                    Dato84 = Antebrazo_HC_T6_His3,

                    Dato85 = Tobillo_HC_T7_His1,
                    Dato86 = Tobillo_HC_T7_His2,
                    Dato87 = Tobillo_HC_T7_His3,
                    Dato88 = Tobillo_HC_T8_His1,
                    Dato89 = Tobillo_HC_T8_His2,
                    Dato90 = Tobillo_HC_T8_His3,
                    Dato91 = Tobillo_HC_T9_His1,
                    Dato92 = Tobillo_HC_T9_His2,
                    Dato93 = Tobillo_HC_T9_His3,
                    Dato94 = Tobillo_HC_T10_His1,
                    Dato95 = Tobillo_HC_T10_His2,
                    Dato96 = Tobillo_HC_T10_His3,

                    Dato97 = RadioCarpiana_HC_T11_His1,
                    Dato98 = RadioCarpiana_HC_T11_His2,
                    Dato99 = RadioCarpiana_HC_T11_His3,
                    Dato100 = RadioCarpiana_HC_T12_His1,
                    Dato101 = RadioCarpiana_HC_T12_His2,
                    Dato102 = RadioCarpiana_HC_T12_His3,
                    Dato103 = RadioCarpiana_HC_T13_His1,
                    Dato104 = RadioCarpiana_HC_T13_His2,
                    Dato105 = RadioCarpiana_HC_T13_His3,
                    Dato106 = RadioCarpiana_HC_T14_His1,
                    Dato107 = RadioCarpiana_HC_T14_His2,
                    Dato108 = RadioCarpiana_HC_T14_His3,

                    Dato109 = "Izquierdo: " + HisExtCui,
                    Dato110 = "Izquierdo: " + HisExtCui2,
                    Dato111 = "Izquierdo: " + HisExtCui3,
                    Dato112 = "Derecho: " + HisExtCuD,
                    Dato113 = "Derecho: " + HisExtCuD2,
                    Dato114 = "Derecho: " + HisExtCuD3,

                    Dato115 = "Izquierdo: " + HisEscalenosI,
                    Dato116 = "Izquierdo: " + HisEscalenosI2,
                    Dato117 = "Izquierdo: " + HisEscalenosI3,
                    Dato118 = "Derecho: " + HisEscalenosD,
                    Dato119 = "Derecho: " + HisEscalenosD2,
                    Dato120 = "Derecho: " + HisEscalenosD3,

                    Dato121 = "Izquierdo: " + HisTrapSupI,
                    Dato122 = "Izquierdo: " + HisTrapSupI2,
                    Dato123 = "Izquierdo: " + HisTrapSupI3,
                    Dato124 = "Derecho: " + HisTrapSupD,
                    Dato125 = "Derecho: " + HisTrapSupD2,
                    Dato126 = "Derecho: " + HisTrapSupD3,

                    Dato127 = "Izquierdo: " + HisTrapMedI,
                    Dato128 = "Izquierdo: " + HisTrapMedI2,
                    Dato129 = "Izquierdo: " + HisTrapMedI3,
                    Dato130 = "Derecho: " + HisTrapMedD,
                    Dato131 = "Derecho: " + HisTrapMedD2,
                    Dato132 = "Derecho: " + HisTrapMedD3,

                    Dato133 = "Izquierdo: " + HisTrapInfI,
                    Dato134 = "Izquierdo: " + HisTrapInfI2,
                    Dato135 = "Izquierdo: " + HisTrapInfI3,
                    Dato136 = "Derecho: " + HisTrapInfD,
                    Dato137 = "Derecho: " + HisTrapInfD2,
                    Dato138 = "Derecho: " + HisTrapInfD3,

                    Dato139 = "Izquierdo: " + HisSerAntI,
                    Dato140 = "Izquierdo: " + HisSerAntI2,
                    Dato141 = "Izquierdo: " + HisSerAntI3,
                    Dato142 = "Derecho: " + HisSerAntD,
                    Dato143 = "Derecho: " + HisSerAntD2,
                    Dato144 = "Derecho: " + HisSerAntD3,

                    Dato145 = "Izquierdo: " + HisPecMayI,
                    Dato146 = "Izquierdo: " + HisPecMayI2,
                    Dato147 = "Izquierdo: " + HisPecMayI3,
                    Dato148 = "Derecho: " + HisPecMayD,
                    Dato149 = "Derecho: " + HisPecMayD2,
                    Dato150 = "Derecho: " + HisPecMayD3,

                    Dato151 = "Izquierdo: " + HisRomI,
                    Dato152 = "Izquierdo: " + HisRomI2,
                    Dato153 = "Izquierdo: " + HisRomI3,
                    Dato154 = "Derecho: " + HisRomD,
                    Dato155 = "Derecho: " + HisRomD2,
                    Dato156 = "Derecho: " + HisRomD3,

                    Dato157 = "Izquierdo: " + HisAbSupI,
                    Dato158 = "Izquierdo: " + HisAbSupI2,
                    Dato159 = "Izquierdo: " + HisAbSupI3,
                    Dato160 = "Derecho: " + HisAbSupD,
                    Dato161 = "Derecho: " + HisAbSupD2,
                    Dato162 = "Derecho: " + HisAbSupD3,

                    Dato163 = "Izquierdo: " + HisAbdInfI,
                    Dato164 = "Izquierdo: " + HisAbdInfI2,
                    Dato165 = "Izquierdo: " + HisAbdInfI3,
                    Dato166 = "Derecho: " + HisAbdInfD,
                    Dato167 = "Derecho: " + HisAbdInfD2,
                    Dato168 = "Derecho: " + HisAbdInfD3,

                    Dato169 = "Izquierdo: " + HisOblicuoI,
                    Dato170 = "Izquierdo: " + HisOblicuoI2,
                    Dato171 = "Izquierdo: " + HisOblicuoI3,
                    Dato172 = "Derecho: " + HisOblicuoD,
                    Dato173 = "Derecho: " + HisOblicuoD2,
                    Dato174 = "Derecho: " + HisOblicuoD3,

                    Dato175 = "Izquierdo: " + HisExtDorsalI,
                    Dato176 = "Izquierdo: " + HisExtDorsalI2,
                    Dato177 = "Izquierdo: " + HisExtDorsalI3,
                    Dato178 = "Derecho: " + HisExtDorsalD,
                    Dato179 = "Derecho: " + HisExtDorsalD2,
                    Dato180 = "Derecho: " + HisExtDorsalD3,

                    Dato181 = "Izquierdo: " + HisExtLumbarI,
                    Dato182 = "Izquierdo: " + HisExtLumbarI2,
                    Dato183 = "Izquierdo: " + HisExtLumbarI3,
                    Dato184 = "Derecho: " + HisExtLumbarD,
                    Dato185 = "Derecho: " + HisExtLumbarD2,
                    Dato186 = "Derecho: " + HisExtLumbarD3,

                    Dato187 = "Izquierdo: " + HisGluMayI,
                    Dato188 = "Izquierdo: " + HisGluMayI2,
                    Dato189 = "Izquierdo: " + HisGluMayI3,
                    Dato190 = "Derecho: " + HisGluMayD,
                    Dato191 = "Derecho: " + HisGluMayD2,
                    Dato192 = "Derecho: " + HisGluMayD3,

                    Dato193 = "Izquierdo: " + HisGluMedI,
                    Dato194 = "Izquierdo: " + HisGluMedI2,
                    Dato195 = "Izquierdo: " + HisGluMedI3,
                    Dato196 = "Derecho: " + HisGluMedD,
                    Dato197 = "Derecho: " + HisGluMedD2,
                    Dato198 = "Derecho: " + HisGluMedD3,

                    Dato199 = "Izquierdo: " + HisCuadriI,
                    Dato200 = "Izquierdo: " + HisCuadriI2,
                    Dato201 = "Izquierdo: " + HisCuadriI3,
                    Dato202 = "Derecho: " + HisCuadriD,
                    Dato203 = "Derecho: " + HisCuadriD2,
                    Dato204 = "Derecho: " + HisCuadriD3,

                    Dato205 = "Izquierdo: " + HisIsquiI,
                    Dato206 = "Izquierdo: " + HisIsquiI2,
                    Dato207 = "Izquierdo: " + HisIsquiI3,
                    Dato208 = "Derecho: " + HisIsquiD,
                    Dato209 = "Derecho: " + HisIsquiD2,
                    Dato210 = "Derecho: " + HisIsquiD3,

                    Dato211 = "Izquierdo: " + HisGemeloI,
                    Dato212 = "Izquierdo: " + HisGemeloI2,
                    Dato213 = "Izquierdo: " + HisGemeloI3,
                    Dato214 = "Derecho: " + HisGemeloD,
                    Dato215 = "Derecho: " + HisGemeloD2,
                    Dato216 = "Derecho: " + HisGemeloD3,

                    Dato217 = "Izquierdo: " + HisTibAntI,
                    Dato218 = "Izquierdo: " + HisTibAntI2,
                    Dato219 = "Izquierdo: " + HisTibAntI3,
                    Dato220 = "Derecho: " + HisTibAntD,
                    Dato221 = "Derecho: " + HisTibAntD2,
                    Dato222 = "Derecho: " + HisTibAntD3,

                    Dato223 = "Izquierdo: " + HisEscaAntIT4,
                    Dato224 = "Izquierdo: " + HisEscaAntIT42,
                    Dato225 = "Izquierdo: " + HisEscaAntIT43,
                    Dato226 = "Derecho: " + HisEscaAntDT4,
                    Dato227 = "Derecho: " + HisEscaAntDT42,
                    Dato228 = "Derecho: " + HisEscaAntDT43,

                    Dato229 = "Izquierdo: " + HisEscaMedIT4,
                    Dato230 = "Izquierdo: " + HisEscaMedIT42,
                    Dato231 = "Izquierdo: " + HisEscaMedIT43,
                    Dato232 = "Derecho: " + HisEscaMedDT4,
                    Dato233 = "Derecho: " + HisEscaMedDT42,
                    Dato234 = "Derecho: " + HisEscaMedDT43,

                    Dato235 = "Izquierdo: " + HisEscaPosIT4,
                    Dato236 = "Izquierdo: " + HisEscaPosIT42,
                    Dato237 = "Izquierdo: " + HisEscaPosIT43,
                    Dato238 = "Derecho: " + HisEscaPosDT4,
                    Dato239 = "Derecho: " + HisEscaPosDT42,
                    Dato240 = "Derecho: " + HisEscaPosDT43,

                    Dato241 = "Izquierdo: " + HisECMIT4,
                    Dato242 = "Izquierdo: " + HisECMIT42,
                    Dato243 = "Izquierdo: " + HisECMIT43,
                    Dato244 = "Derecho: " + HisECMDT4,
                    Dato245 = "Derecho: " + HisECMDT42,
                    Dato246 = "Derecho: " + HisECMDT43,

                    Dato247 = "Izquierdo: " + HisCualLumIT4,
                    Dato248 = "Izquierdo: " + HisCualLumIT42,
                    Dato249 = "Izquierdo: " + HisCualLumIT43,
                    Dato250 = "Derecho: " + HisCualLumDT4,
                    Dato251 = "Derecho: " + HisCualLumDT42,
                    Dato252 = "Derecho: " + HisCualLumDT43,

                    Dato253 = "Izquierdo: " + HisCuadIT4,
                    Dato254 = "Izquierdo: " + HisCuadIT42,
                    Dato255 = "Izquierdo: " + HisCuadIT43,
                    Dato256 = "Derecho: " + HisCuadDT4,
                    Dato257 = "Derecho: " + HisCuadDT42,
                    Dato258 = "Derecho: " + HisCuadDT43,

                    Dato259 = "Izquierdo: " + HisIsquiI2T4,
                    Dato260 = "Izquierdo: " + HisIsquiI2T42,
                    Dato261 = "Izquierdo: " + HisIsquiI2T43,
                    Dato262 = "Derecho: " + HisIsqui2DT4,
                    Dato263 = "Derecho: " + HisIsqui2DT42,
                    Dato264 = "Derecho: " + HisIsqui2DT43,

                    Dato265 = "Izquierdo: " + HisTensorIT4,
                    Dato266 = "Izquierdo: " + HisTensorIT42,
                    Dato267 = "Izquierdo: " + HisTensorIT43,
                    Dato268 = "Derecho: " + HisTensorDT4,
                    Dato269 = "Derecho: " + HisTensorDT42,
                    Dato270 = "Derecho: " + HisTensorDT43,

                    Dato271 = "Izquierdo: " + HisGastroIT4,
                    Dato272 = "Izquierdo: " + HisGastroIT42,
                    Dato273 = "Izquierdo: " + HisGastroIT43,
                    Dato274 = "Derecho: " + HisGastroDT4,
                    Dato275 = "Derecho: " + HisGastroDT42,
                    Dato276 = "Derecho: " + HisGastroDT43,

                    Dato277 = "Izquierdo: " + HisTAquilesIT4,
                    Dato278 = "Izquierdo: " + HisTAquilesIT42,
                    Dato279 = "Izquierdo: " + HisTAquilesIT43,
                    Dato280 = "Derecho: " + HisTAquilesDT4,
                    Dato281 = "Derecho: " + HisTAquilesDT42,
                    Dato282 = "Derecho: " + HisTAquilesDT43
                });

                return dTF;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        List<CXN_HCTF> ITFGeneral.getAllTF(int Paciente, DateTime Desde, DateTime Hasta)
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

                    String Query = "SELECT TF.HC_Adm, H.Hor_Pac_Fecha_Cita, H.Hor_Imp_Age, TF.TipoHistoria " +
                                   "FROM CXN_HCTF TF " +
                                   "INNER JOIN CXN_HORARIO H ON TF.HC_Adm = H.Hor_Id " +
                                   "WHERE TF.HC_PacId = '" + Paciente + "' " +
                                   "AND H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString("yyyy-MM-dd") + "' AND '" + Convert.ToDateTime(Hasta).ToString("yyyy-MM-dd") + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_HCTF> T = new List<CXN_HCTF>();

                        while (Reader.Read() == true)
                        {
                            T.Add(new CXN_HCTF
                            {
                                HC_Adm = Convert.ToInt32(Reader["HC_Adm"]),
                                HC_Fecha = Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]),
                                HC_Pac = Reader["Hor_Imp_Age"].ToString(),
                                TipoHistoria = Reader["TipoHistoria"].ToString()
                            });
                        }

                        return T;
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

        bool ITFGeneral.updateTipoHistoria(int Admision, string TipoHistoria)
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

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_HCTF " +
                                                          "SET  " +
                                                          "TipoHistoria = @param1 " +
                                                          "WHERE HC_Adm = '" + Admision + "'", con);

                    Busqueda.Parameters.AddWithValue("@param1", TipoHistoria);
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
    }
}
