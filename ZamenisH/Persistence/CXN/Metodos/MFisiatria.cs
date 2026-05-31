using System;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System.Collections.Generic;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MFisiatria :IFisiatria
    {
        bool IFisiatria.ActualizaHCFI(CXN_HCFI HC)
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

                    string Busqueda = (@"UPDATE CXN_HCFI " +
                                      "SET HC_Pac = @param1, " +
                                                         "HC_PacId = @param2, " +
                                                         "HC_Fecha = @param4, " +
                                                         "HC_Prof = @param5, " +
                                                         "HC_Cia = @param6, " +
                                                         "HC_Ase = @param7, " +
                                                         "HC_FechaNto = @param8, " +
                                                         "HC_Edad = @param9, " +
                                                         "HC_Cant = @param10, " +
                                                         "HC_MotCons = @param11, " +
                                                         "HC_EnfAct = @param12, " +
                                                         "HC_Neurologico = @param13, " +
                                                         "HC_Mental = @param14, " +
                                                         "HC_OrgSent = @param15, " +
                                                         "HC_Respiratorio = @param16, " +
                                                         "HC_Cardiovascular = @param17, " +
                                                         "HC_GastroI = @param18, " +
                                                         "HC_GenitoU = @param19, " +
                                                         "HC_OsteoM = @param20, " +
                                                         "HC_PielFan = @param21, " +
                                                         "HC_Hematolin = @param22, " +
                                                         "HC_Ant = @param23, " +
                                                         "HC_Presart = @param24, " +
                                                         "HC_Peso = @param25, " +
                                                         "HC_Frecar = @param26, " +
                                                         "HC_Talla = @param27, " +
                                                         "HC_FrecResp = @param28, " +
                                                         "HC_IMC = @param29, " +
                                                         //"HC_Temp = @param30, " +
                                                         //"HC_PerimetroC = @param31, " +
                                                         "HC_EstCons = @param32, " +
                                                         //"HC_PerimetroA = @param33, " +
                                                         "HC_Glasshow = @param34, " +
                                                         //"HC_Embriaguez = @param35, " +
                                                         "HC_ObservaFis = @param36, " +
                                                         "HC_ObservaNeu = @param37, " +
                                                         "HC_Cabeza = @param38, " +
                                                         "HC_Orl = @param39, " +
                                                         //"HC_Genitales = @param40, " +
                                                         "HC_Abdomen = @param41, " +
                                                         //"HC_Ombligo = @param42, " +
                                                         //"HC_Ano = @param43, " +
                                                         "HC_Torax = @param44, " +
                                                         "HC_Extremidades = @param45, " +
                                                         "HC_Cuello = @param46, " +
                                                         "HC_Pulmonar = @param47, " +
                                                         "HC_DX1 = @param48, " +
                                                         "HC_DX2 = @param49, " +
                                                         "HC_DX3 = @param50, " +
                                                         "HC_Analisis = @param51, " +
                                                         "HC_Egreso = @param52, " +
                                                         "HC_PManejo = @param53, " +
                                                         "HC_RH = @param54, " +
                                                         "HC_EAV = @param55, " +
                                                         "HC_Acudiente = @param56, " +
                                                         "HC_NotaDX1 = @param57, " +
                                                         "HC_NotaDX2 = @param58, " +
                                                         "HC_NotaDX3 = @param59, " +
                                                         "HC_ImpDX1 = @param60, " +
                                                         "HC_ImpDX2 = @param61, " +
                                                         "HC_ImpDX3 = @param62, " +
                                                         "HC_OsteoMus = @param63, " +
                                                         "HC_Causa_Externa = @param64, " +
                                                         "HC_Piel2 = @param65, " +
                                                         "HC_Epidemia = @param66 " +
                                      "WHERE HC_Adm = '" + HC.HC_Adm + "' " +
                                      "AND HC_Cant = '0'");
                    SqlCommand Accion = new SqlCommand(Busqueda, con);

                    Accion.Parameters.AddWithValue("@param1", HC.HC_Pac);
                    Accion.Parameters.AddWithValue("@param2", HC.HC_Pacid);
                    Accion.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = HC.HC_Fecha;
                    Accion.Parameters.AddWithValue("@param5", HC.HC_Prof);
                    Accion.Parameters.AddWithValue("@param6", HC.HC_Cia);
                    Accion.Parameters.AddWithValue("@param7", HC.HC_Ase);
                    Accion.Parameters.Add(new SqlParameter("@param8", SqlDbType.DateTime)).Value = HC.HC_FechaNto;
                    Accion.Parameters.AddWithValue("@param9", HC.HC_Edad);
                    Accion.Parameters.AddWithValue("@param10", HC.HC_Cant);
                    Accion.Parameters.AddWithValue("@param11", HC.HC_MotCons);
                    Accion.Parameters.AddWithValue("@param12", HC.HC_EnfAct);
                    Accion.Parameters.AddWithValue("@param13", HC.HC_Neurologico);
                    Accion.Parameters.AddWithValue("@param14", HC.HC_Mental);
                    Accion.Parameters.AddWithValue("@param15", HC.HC_OrgSent);
                    Accion.Parameters.AddWithValue("@param16", HC.HC_Respiratorio);
                    Accion.Parameters.AddWithValue("@param17", HC.HC_Cardiovascular);
                    Accion.Parameters.AddWithValue("@param18", HC.HC_GastroI);
                    Accion.Parameters.AddWithValue("@param19", HC.HC_GenitoU);
                    Accion.Parameters.AddWithValue("@param20", HC.HC_OsteoM);
                    Accion.Parameters.AddWithValue("@param21", HC.HC_PielFan);
                    Accion.Parameters.AddWithValue("@param22", HC.HC_Hematolin);
                    Accion.Parameters.AddWithValue("@param23", HC.HC_Ant);
                    Accion.Parameters.AddWithValue("@param24", HC.HC_Presart);
                    Accion.Parameters.AddWithValue("@param25", HC.HC_Peso);
                    Accion.Parameters.AddWithValue("@param26", HC.HC_Frecar);
                    Accion.Parameters.AddWithValue("@param27", HC.HC_Talla);
                    Accion.Parameters.AddWithValue("@param28", HC.HC_FrecResp);
                    Accion.Parameters.AddWithValue("@param29", HC.HC_IMC);
                    //Accion.Parameters.AddWithValue("@param30", HC.HC_Temp);
                    //Accion.Parameters.AddWithValue("@param31", HC.HC_PerimetroC);
                    Accion.Parameters.AddWithValue("@param32", HC.HC_EstCons);
                    //Accion.Parameters.AddWithValue("@param33", HC.HC_Abdomen);
                    Accion.Parameters.AddWithValue("@param34", HC.HC_Glasshow);
                    //Accion.Parameters.AddWithValue("@param35", HC.HC_Embriaguez);
                    Accion.Parameters.AddWithValue("@param36", HC.HC_ObservaFis);
                    Accion.Parameters.AddWithValue("@param37", HC.HC_ObservaNeu);
                    Accion.Parameters.AddWithValue("@param38", HC.HC_Cabeza);
                    Accion.Parameters.AddWithValue("@param39", HC.HC_Orl);
                    //Accion.Parameters.AddWithValue("@param40", HC.HC_Genitales);
                    Accion.Parameters.AddWithValue("@param41", HC.HC_Abdomen);
                    //Accion.Parameters.AddWithValue("@param42", HC.HC_Ombligo);
                    //Accion.Parameters.AddWithValue("@param43", HC.HC_Ano);
                    Accion.Parameters.AddWithValue("@param44", HC.HC_Torax);
                    Accion.Parameters.AddWithValue("@param45", HC.HC_Extremidades);
                    Accion.Parameters.AddWithValue("@param46", HC.HC_Cuello);
                    Accion.Parameters.AddWithValue("@param47", HC.HC_Pulmonar);
                    Accion.Parameters.AddWithValue("@param48", HC.HC_DX1);
                    Accion.Parameters.AddWithValue("@param49", HC.HC_DX2);
                    Accion.Parameters.AddWithValue("@param50", HC.HC_DX3);
                    Accion.Parameters.AddWithValue("@param51", HC.HC_Analisis); //FAMILIAR
                    Accion.Parameters.AddWithValue("@param52", HC.HC_Egreso);
                    Accion.Parameters.AddWithValue("@param53", HC.HC_PManejo);
                    Accion.Parameters.AddWithValue("@param54", HC.HC_RH);
                    Accion.Parameters.AddWithValue("@param55", HC.HC_EAV); //FARMACOLOGICO
                    Accion.Parameters.AddWithValue("@param56", HC.HC_Acudiente);
                    Accion.Parameters.AddWithValue("@param57", HC.HC_NotaDX1);
                    Accion.Parameters.AddWithValue("@param58", HC.HC_NotaDX2);
                    Accion.Parameters.AddWithValue("@param59", HC.HC_NotaDX3);
                    Accion.Parameters.AddWithValue("@param60", HC.HC_ImpDX1);
                    Accion.Parameters.AddWithValue("@param61", HC.HC_ImpDX2);
                    Accion.Parameters.AddWithValue("@param62", HC.HC_ImpDX3);
                    Accion.Parameters.AddWithValue("@param63", HC.HC_OsteoMUS);
                    Accion.Parameters.AddWithValue("@param64", HC.HC_Causa_Externa);
                    Accion.Parameters.AddWithValue("@param65", HC.HC_Piel2);
                    Accion.Parameters.AddWithValue("@param66", HC.HC_Epidemia);
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

        bool IFisiatria.GrabaHCFI(CXN_HCFI HC)
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

                    SqlCommand Accion = new SqlCommand(@"INSERT INTO CXN_HCFI " +
                                                         "(HC_Pac, " +
                                                         "HC_PacId, " +
                                                         "HC_Adm, " +
                                                         "HC_Fecha, " +
                                                         "HC_Prof, " +
                                                         "HC_Cia, " +
                                                         "HC_Ase, " +
                                                         "HC_FechaNto, " +
                                                         "HC_Edad, " +
                                                         "HC_Cant, " +
                                                         "HC_MotCons, " +
                                                         "HC_EnfAct, " +
                                                         "HC_Neurologico, " +
                                                         "HC_Mental, " +
                                                         "HC_OrgSent, " +
                                                         "HC_Respiratorio, " +
                                                         "HC_Cardiovascular, " +
                                                         "HC_GastroI, " +
                                                         "HC_GenitoU, " +
                                                         "HC_OsteoM, " +
                                                         "HC_PielFan, " +
                                                         "HC_Hematolin, " +
                                                         "HC_Ant, " +
                                                         "HC_Presart, " +
                                                         "HC_Peso, " +
                                                         "HC_Frecar, " +
                                                         "HC_Talla, " +
                                                         "HC_FrecResp, " +
                                                         "HC_IMC, " +
                                                         //"HC_Temp, " +
                                                         //"HC_PerimetroC, " +
                                                         "HC_EstCons, " +
                                                         //"HC_PerimetroA, " +
                                                         "HC_Glasshow, " +
                                                         //"HC_Embriaguez, " +
                                                         "HC_ObservaFis, " +
                                                         "HC_ObservaNeu, " +
                                                         "HC_Cabeza, " +
                                                         "HC_Orl, " +
                                                         //"HC_Genitales, " +
                                                         "HC_Abdomen, " +
                                                         //"HC_Ombligo, " +
                                                         //"HC_Ano, " +
                                                         "HC_Torax, " +
                                                         "HC_Extremidades, " +
                                                         "HC_Cuello, " +
                                                         "HC_Pulmonar, " +
                                                         "HC_DX1, " +
                                                         "HC_DX2, " +
                                                         "HC_DX3, " +
                                                         "HC_Analisis, " +
                                                         "HC_Egreso, " +
                                                         "HC_PManejo, " +
                                                         "HC_RH, " +
                                                         "HC_EAV, " +
                                                         "HC_Acudiente, " +
                                                         "HC_NotaDX1, " +
                                                         "HC_NotaDX2, " +
                                                         "HC_NotaDX3, " +
                                                         "HC_ImpDX1, " +
                                                         "HC_ImpDX2, " +
                                                         "HC_ImpDX3, " +
                                                         "HC_OsteoMus, " +
                                                         "HC_Causa_Externa, " +
                                                         "HC_Piel2, " +
                                                         "HC_Epidemia) " +
                                     "VALUES                  (@param1, " +
                                                              "@param2, @param3, @param4, @param5, " +
                                                              "@param6, @param7, @param8, @param9, " +
                                                              "@param10, @param11, @param12, @param13, " +
                                                              "@param14, @param15, @param16, @param17, " +
                                                              "@param18, @param19, @param20, @param21, " +
                                                              "@param22, @param23, @param24, @param25, " +
                                                              "@param26, @param27, @param28, @param29, " +
                                                              "@param32, " +
                                                              "@param34, @param36, @param37, " +
                                                              "@param38, @param39, @param41, " +
                                                              "@param44, @param45, " +
                                                              "@param46, @param47, @param48, @param49, " +
                                                              "@param50, @param51, @param52, @param53, " +
                                                              "@param54, @param55, @param56, @param57, " +
                                                              "@param58, @param59, @param60, @param61, " +
                                                              "@param62, @param63, @param64, @param65, @param66)", con);

                    Accion.Parameters.AddWithValue("@param1", HC.HC_Pac);
                    Accion.Parameters.AddWithValue("@param2", HC.HC_Pacid);
                    Accion.Parameters.AddWithValue("@param3", HC.HC_Adm);
                    Accion.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = HC.HC_Fecha;
                    Accion.Parameters.AddWithValue("@param5", HC.HC_Prof);
                    Accion.Parameters.AddWithValue("@param6", HC.HC_Cia);
                    Accion.Parameters.AddWithValue("@param7", HC.HC_Ase);
                    Accion.Parameters.Add(new SqlParameter("@param8", SqlDbType.DateTime)).Value = HC.HC_FechaNto;
                    Accion.Parameters.AddWithValue("@param9", HC.HC_Edad);
                    Accion.Parameters.AddWithValue("@param10", HC.HC_Cant);
                    Accion.Parameters.AddWithValue("@param11", HC.HC_MotCons);
                    Accion.Parameters.AddWithValue("@param12", HC.HC_EnfAct);
                    Accion.Parameters.AddWithValue("@param13", HC.HC_Neurologico);
                    Accion.Parameters.AddWithValue("@param14", HC.HC_Mental);
                    Accion.Parameters.AddWithValue("@param15", HC.HC_OrgSent);
                    Accion.Parameters.AddWithValue("@param16", HC.HC_Respiratorio);
                    Accion.Parameters.AddWithValue("@param17", HC.HC_Cardiovascular);
                    Accion.Parameters.AddWithValue("@param18", HC.HC_GastroI);
                    Accion.Parameters.AddWithValue("@param19", HC.HC_GenitoU);
                    Accion.Parameters.AddWithValue("@param20", HC.HC_OsteoM);
                    Accion.Parameters.AddWithValue("@param21", HC.HC_PielFan);
                    Accion.Parameters.AddWithValue("@param22", HC.HC_Hematolin);
                    Accion.Parameters.AddWithValue("@param23", HC.HC_Ant);
                    Accion.Parameters.AddWithValue("@param24", HC.HC_Presart);
                    Accion.Parameters.AddWithValue("@param25", HC.HC_Peso);
                    Accion.Parameters.AddWithValue("@param26", HC.HC_Frecar);
                    Accion.Parameters.AddWithValue("@param27", HC.HC_Talla);
                    Accion.Parameters.AddWithValue("@param28", HC.HC_FrecResp);
                    Accion.Parameters.AddWithValue("@param29", HC.HC_IMC);
                    //Accion.Parameters.AddWithValue("@param30", HC.HC_Temp);
                    //Accion.Parameters.AddWithValue("@param31", HC.HC_PerimetroC);
                    Accion.Parameters.AddWithValue("@param32", HC.HC_EstCons);
                    //Accion.Parameters.AddWithValue("@param33", HC.HC_Abdomen);
                    Accion.Parameters.AddWithValue("@param34", HC.HC_Glasshow);
                    //Accion.Parameters.AddWithValue("@param35", HC.HC_Embriaguez);
                    Accion.Parameters.AddWithValue("@param36", HC.HC_ObservaFis);
                    Accion.Parameters.AddWithValue("@param37", HC.HC_ObservaNeu);
                    Accion.Parameters.AddWithValue("@param38", HC.HC_Cabeza);
                    Accion.Parameters.AddWithValue("@param39", HC.HC_Orl);
                    //Accion.Parameters.AddWithValue("@param40", HC.HC_Genitales);
                    Accion.Parameters.AddWithValue("@param41", HC.HC_Abdomen);
                    //Accion.Parameters.AddWithValue("@param42", HC.HC_Ombligo);
                    //Accion.Parameters.AddWithValue("@param43", HC.HC_Ano);
                    Accion.Parameters.AddWithValue("@param44", HC.HC_Torax);
                    Accion.Parameters.AddWithValue("@param45", HC.HC_Extremidades);
                    Accion.Parameters.AddWithValue("@param46", HC.HC_Cuello);
                    Accion.Parameters.AddWithValue("@param47", HC.HC_Pulmonar);
                    Accion.Parameters.AddWithValue("@param48", HC.HC_DX1);
                    Accion.Parameters.AddWithValue("@param49", HC.HC_DX2);
                    Accion.Parameters.AddWithValue("@param50", HC.HC_DX3);
                    Accion.Parameters.AddWithValue("@param51", HC.HC_Analisis); //FAMILIAR
                    Accion.Parameters.AddWithValue("@param52", HC.HC_Egreso);
                    Accion.Parameters.AddWithValue("@param53", HC.HC_PManejo);
                    Accion.Parameters.AddWithValue("@param54", HC.HC_RH);
                    Accion.Parameters.AddWithValue("@param55", HC.HC_EAV); //FARMACOLOGICO
                    Accion.Parameters.AddWithValue("@param56", HC.HC_Acudiente);
                    Accion.Parameters.AddWithValue("@param57", HC.HC_NotaDX1);
                    Accion.Parameters.AddWithValue("@param58", HC.HC_NotaDX2);
                    Accion.Parameters.AddWithValue("@param59", HC.HC_NotaDX3);
                    Accion.Parameters.AddWithValue("@param60", HC.HC_ImpDX1);
                    Accion.Parameters.AddWithValue("@param61", HC.HC_ImpDX2);
                    Accion.Parameters.AddWithValue("@param62", HC.HC_ImpDX3);
                    Accion.Parameters.AddWithValue("@param63", HC.HC_OsteoMUS);
                    Accion.Parameters.AddWithValue("@param64", HC.HC_Causa_Externa);
                    Accion.Parameters.AddWithValue("@param65", HC.HC_Piel2);
                    Accion.Parameters.AddWithValue("@param66", HC.HC_Epidemia);
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

        CXN_HCFI IFisiatria.getLastHistory(int Paciente, DateTime Fecha)
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

                    String Cargar_Cod_Prof = "SELECT TOP 1 * " +
                                                "FROM CXN_HCFI " +
                                                "WHERE HC_PacId = '" + Paciente + "' " +
                                                "AND HC_Fecha <> '" + Convert.ToDateTime(Fecha.Date).ToString(getData["Format_Fecha"]) + "' " +
                                                "AND HC_Cant = '1' " +
                                                "ORDER BY HC_Fecha DESC";
                    SqlCommand Carga_Cod_Prof = new SqlCommand(Cargar_Cod_Prof, con);
                    SqlDataReader Lectura_Cod_Prof = (Carga_Cod_Prof.ExecuteReader());
                    if (Lectura_Cod_Prof.Read() == true)
                    {
                        CXN_HCFI H = new CXN_HCFI
                        {
                            HC_Fecha = Convert.ToDateTime(Lectura_Cod_Prof["HC_Fecha"]),
                            HC_MotCons = Lectura_Cod_Prof["HC_MotCons"].ToString(),
                            HC_EnfAct = Lectura_Cod_Prof["HC_EnfAct"].ToString(),

                            HC_Neurologico = Lectura_Cod_Prof["HC_Neurologico"].ToString(),
                            HC_Mental = Lectura_Cod_Prof["HC_Mental"].ToString(),
                            HC_OrgSent = Lectura_Cod_Prof["HC_OrgSent"].ToString(),
                            HC_Respiratorio = Lectura_Cod_Prof["HC_Respiratorio"].ToString(),
                            HC_Cardiovascular = Lectura_Cod_Prof["HC_Cardiovascular"].ToString(),
                            HC_GastroI = Lectura_Cod_Prof["HC_GastroI"].ToString(),
                            HC_GenitoU = Lectura_Cod_Prof["HC_GenitoU"].ToString(),
                            HC_OsteoM = Lectura_Cod_Prof["HC_OsteoM"].ToString(),
                            HC_Hematolin = Lectura_Cod_Prof["HC_Hematolin"].ToString(),
                            HC_PielFan = Lectura_Cod_Prof["HC_PielFan"].ToString(),

                            HC_Ant = Lectura_Cod_Prof["HC_Ant"].ToString(),
                            HC_Epidemia = Lectura_Cod_Prof["HC_Epidemia"].ToString(),

                            HC_Acudiente = Lectura_Cod_Prof["HC_Acudiente"].ToString(),
                            HC_Presart = Lectura_Cod_Prof["HC_Presart"].ToString(),
                            HC_Peso = Lectura_Cod_Prof["HC_Peso"].ToString(),
                            HC_Frecar = Lectura_Cod_Prof["HC_FreCar"].ToString(),
                            HC_Talla = Lectura_Cod_Prof["HC_Talla"].ToString(),
                            HC_FrecResp = Lectura_Cod_Prof["HC_FrecResp"].ToString(),

                            HC_PerimetroC = Lectura_Cod_Prof["HC_PerimetroC"].ToString(),
                            HC_EstCons = Lectura_Cod_Prof["HC_EstCons"].ToString(),
                            HC_PerimetroA = Lectura_Cod_Prof["HC_PerimetroA"].ToString(),
                            HC_Glasshow = Lectura_Cod_Prof["HC_Glasshow"].ToString(),
                            HC_Embriaguez = Lectura_Cod_Prof["HC_Embriaguez"].ToString(),
                            HC_IMC = Lectura_Cod_Prof["HC_IMC"].ToString(),
                            HC_Temp = Lectura_Cod_Prof["HC_Temp"].ToString(),
                            HC_RH = Lectura_Cod_Prof["HC_RH"].ToString(),
                            HC_EAV = Lectura_Cod_Prof["HC_EAV"].ToString(),

                            HC_ObservaFis = Lectura_Cod_Prof["HC_ObservaFis"].ToString(),
                            HC_ObservaNeu = Lectura_Cod_Prof["HC_ObservaNeu"].ToString(),
                            HC_Cabeza = Lectura_Cod_Prof["HC_Cabeza"].ToString(),
                            HC_Orl = Lectura_Cod_Prof["HC_Orl"].ToString(),
                            HC_Cuello = Lectura_Cod_Prof["HC_Cuello"].ToString(),
                            HC_Torax = Lectura_Cod_Prof["HC_Torax"].ToString(),
                            HC_Pulmonar = Lectura_Cod_Prof["HC_Pulmonar"].ToString(),
                            HC_Abdomen = Lectura_Cod_Prof["HC_Abdomen"].ToString(),
                            HC_Ombligo = Lectura_Cod_Prof["HC_Ombligo"].ToString(),
                            HC_Ano = Lectura_Cod_Prof["HC_Ano"].ToString(),
                            HC_Genitales = Lectura_Cod_Prof["HC_GEnitales"].ToString(),
                            HC_Extremidades = Lectura_Cod_Prof["HC_Extremidades"].ToString(),
                            HC_OsteoMUS = Lectura_Cod_Prof["HC_OsteoMus"].ToString(),
                            HC_Piel2 = Lectura_Cod_Prof["HC_Piel2"].ToString(),

                            HC_DX1 = Lectura_Cod_Prof["HC_DX1"].ToString(),
                            HC_DX2 = Lectura_Cod_Prof["HC_DX2"].ToString(),
                            HC_DX3 = Lectura_Cod_Prof["HC_DX3"].ToString(),
                            HC_ImpDX1 = Lectura_Cod_Prof["HC_ImpDX1"].ToString(),
                            HC_ImpDX2 = Lectura_Cod_Prof["HC_ImpDX2"].ToString(),
                            HC_ImpDX3 = Lectura_Cod_Prof["HC_ImpDX3"].ToString(),
                            HC_NotaDX1 = Lectura_Cod_Prof["HC_NotaDX1"].ToString(),
                            HC_NotaDX2 = Lectura_Cod_Prof["HC_NotaDX2"].ToString(),
                            HC_NotaDX3 = Lectura_Cod_Prof["HC_NotaDX3"].ToString(),

                            HC_Causa_Externa = Lectura_Cod_Prof["HC_Causa_Externa"].ToString(),
                            HC_Egreso = Lectura_Cod_Prof["HC_Egreso"].ToString(),
                            HC_Analisis = Lectura_Cod_Prof["HC_Analisis"].ToString(),
                            HC_PManejo = Lectura_Cod_Prof["HC_PManejo"].ToString()
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

        CXN_HCFI IFisiatria.restoreHistory(int Admision)
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

                    String Cargar_Cod_Prof = "SELECT  * " +
                                             "FROM CXN_HCFI " +
                                             "WHERE HC_Adm = '" + Admision + "'";
                    SqlCommand Carga_Cod_Prof = new SqlCommand(Cargar_Cod_Prof, con);
                    SqlDataReader Lectura_Cod_Prof = (Carga_Cod_Prof.ExecuteReader());
                    if (Lectura_Cod_Prof.Read() == true)
                    {
                        CXN_HCFI H = new CXN_HCFI
                        {
                            HC_Fecha = Convert.ToDateTime(Lectura_Cod_Prof["HC_Fecha"]),
                            HC_MotCons = Lectura_Cod_Prof["HC_MotCons"].ToString(),
                            HC_EnfAct = Lectura_Cod_Prof["HC_EnfAct"].ToString(),

                            HC_Neurologico = Lectura_Cod_Prof["HC_Neurologico"].ToString(),
                            HC_Mental = Lectura_Cod_Prof["HC_Mental"].ToString(),
                            HC_OrgSent = Lectura_Cod_Prof["HC_OrgSent"].ToString(),
                            HC_Respiratorio = Lectura_Cod_Prof["HC_Respiratorio"].ToString(),
                            HC_Cardiovascular = Lectura_Cod_Prof["HC_Cardiovascular"].ToString(),
                            HC_GastroI = Lectura_Cod_Prof["HC_GastroI"].ToString(),
                            HC_GenitoU = Lectura_Cod_Prof["HC_GenitoU"].ToString(),
                            HC_OsteoM = Lectura_Cod_Prof["HC_OsteoM"].ToString(),
                            HC_Hematolin = Lectura_Cod_Prof["HC_Hematolin"].ToString(),
                            HC_PielFan = Lectura_Cod_Prof["HC_PielFan"].ToString(),

                            HC_Ant = Lectura_Cod_Prof["HC_Ant"].ToString(),
                            HC_Epidemia = Lectura_Cod_Prof["HC_Epidemia"].ToString(),

                            HC_Acudiente = Lectura_Cod_Prof["HC_Acudiente"].ToString(),
                            HC_Presart = Lectura_Cod_Prof["HC_Presart"].ToString(),
                            HC_Peso = Lectura_Cod_Prof["HC_Peso"].ToString(),
                            HC_Frecar = Lectura_Cod_Prof["HC_FreCar"].ToString(),
                            HC_Talla = Lectura_Cod_Prof["HC_Talla"].ToString(),
                            HC_FrecResp = Lectura_Cod_Prof["HC_FrecResp"].ToString(),

                            HC_PerimetroC = Lectura_Cod_Prof["HC_PerimetroC"].ToString(),
                            HC_EstCons = Lectura_Cod_Prof["HC_EstCons"].ToString(),
                            HC_PerimetroA = Lectura_Cod_Prof["HC_PerimetroA"].ToString(),
                            HC_Glasshow = Lectura_Cod_Prof["HC_Glasshow"].ToString(),
                            HC_Embriaguez = Lectura_Cod_Prof["HC_Embriaguez"].ToString(),
                            HC_IMC = Lectura_Cod_Prof["HC_IMC"].ToString(),
                            HC_Temp = Lectura_Cod_Prof["HC_Temp"].ToString(),
                            HC_RH = Lectura_Cod_Prof["HC_RH"].ToString(),
                            HC_EAV = Lectura_Cod_Prof["HC_EAV"].ToString(),

                            HC_ObservaFis = Lectura_Cod_Prof["HC_ObservaFis"].ToString(),
                            HC_ObservaNeu = Lectura_Cod_Prof["HC_ObservaNeu"].ToString(),
                            HC_Cabeza = Lectura_Cod_Prof["HC_Cabeza"].ToString(),
                            HC_Orl = Lectura_Cod_Prof["HC_Orl"].ToString(),
                            HC_Cuello = Lectura_Cod_Prof["HC_Cuello"].ToString(),
                            HC_Torax = Lectura_Cod_Prof["HC_Torax"].ToString(),
                            HC_Pulmonar = Lectura_Cod_Prof["HC_Pulmonar"].ToString(),
                            HC_Abdomen = Lectura_Cod_Prof["HC_Abdomen"].ToString(),
                            HC_Ombligo = Lectura_Cod_Prof["HC_Ombligo"].ToString(),
                            HC_Ano = Lectura_Cod_Prof["HC_Ano"].ToString(),
                            HC_Genitales = Lectura_Cod_Prof["HC_GEnitales"].ToString(),
                            HC_Extremidades = Lectura_Cod_Prof["HC_Extremidades"].ToString(),
                            HC_OsteoMUS = Lectura_Cod_Prof["HC_OsteoMus"].ToString(),
                            HC_Piel2 = Lectura_Cod_Prof["HC_Piel2"].ToString(),

                            HC_DX1 = Lectura_Cod_Prof["HC_DX1"].ToString(),
                            HC_DX2 = Lectura_Cod_Prof["HC_DX2"].ToString(),
                            HC_DX3 = Lectura_Cod_Prof["HC_DX3"].ToString(),
                            HC_ImpDX1 = Lectura_Cod_Prof["HC_ImpDX1"].ToString(),
                            HC_ImpDX2 = Lectura_Cod_Prof["HC_ImpDX2"].ToString(),
                            HC_ImpDX3 = Lectura_Cod_Prof["HC_ImpDX3"].ToString(),
                            HC_NotaDX1 = Lectura_Cod_Prof["HC_NotaDX1"].ToString(),
                            HC_NotaDX2 = Lectura_Cod_Prof["HC_NotaDX2"].ToString(),
                            HC_NotaDX3 = Lectura_Cod_Prof["HC_NotaDX3"].ToString(),

                            HC_Causa_Externa = Lectura_Cod_Prof["HC_Causa_Externa"].ToString(),
                            HC_Egreso = Lectura_Cod_Prof["HC_Egreso"].ToString(),
                            HC_Analisis = Lectura_Cod_Prof["HC_Analisis"].ToString(),
                            HC_PManejo = Lectura_Cod_Prof["HC_PManejo"].ToString()
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

        List<CXN_HCMG> IFisiatria.ListaUltimasCitas(int Paciente, DateTime Fecha, int Medico)
        {
            Dictionary<string, string> getData = Conexion.Conection();

            try
            {
                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora2 = "SELECT TOP 5 HC_Adm, H.HC_Fecha, B.Bod_Responsable " +
                                          "FROM CXN_HCFI H " +
                                          "INNER JOIN CXN_BODEGAS B ON H.HC_Prof = B.Bod_Numero " +
                                          "WHERE H.HC_PacId = @param1 " +
                                          "AND H.HC_Fecha BETWEEN @param2 AND @param3 " +
                                          "AND H.HC_Cant = '1' " +
                                          "AND H.HC_Prof = @param4 " +
                                          "ORDER BY H.HC_Fecha DESC";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        DateTime Desde = new DateTime(Fecha.Year, 1, 1);

                        Carga_Command2.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command2.Parameters.AddWithValue("@param2", Convert.ToDateTime(Desde.Date));
                        Carga_Command2.Parameters.AddWithValue("@param3", Convert.ToDateTime(Fecha));
                        Carga_Command2.Parameters.AddWithValue("@param4", Medico);

                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.HasRows)
                            {
                                List<CXN_HCMG> L = new List<CXN_HCMG>();

                                while (Lectura_Hora2.Read() == true)
                                {
                                    L.Add(new CXN_HCMG
                                    {
                                        HC_Adm = Convert.ToInt32(Lectura_Hora2["HC_Adm"]),
                                        HC_Fecha = Convert.ToDateTime(Lectura_Hora2["HC_Fecha"]),
                                        HC_IMC = Lectura_Hora2["Bod_Responsable"].ToString()
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
    }
}
