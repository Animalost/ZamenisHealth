using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MMedicinaGeneral : IMedicinaGeneral
    {
        CXN_HCMG IMedicinaGeneral.getIngresadoTemporal(int Admision)
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
                                             "FROM CXN_HCMG " +
                                             "WHERE HC_Adm = '" + Admision + "'";
                    SqlCommand Carga_Cod_Prof = new SqlCommand(Cargar_Cod_Prof, con);
                    SqlDataReader Lectura_Cod_Prof = (Carga_Cod_Prof.ExecuteReader());
                    if (Lectura_Cod_Prof.Read() == true)
                    {
                        CXN_HCMG H = new CXN_HCMG
                        {
                            HC_MotivoC = Lectura_Cod_Prof["HC_MotivoC"].ToString(),
                            HC_EnfA = Lectura_Cod_Prof["HC_EnfA"].ToString(),
                            HC_PruebasDiag = Lectura_Cod_Prof["HC_PruebasDiag"].ToString(),

                            HC_Neurologico = Lectura_Cod_Prof["HC_Neurologico"].ToString(),
                            HC_Respiratorio = Lectura_Cod_Prof["HC_Respiratorio"].ToString(),
                            HC_Cardiovascular = Lectura_Cod_Prof["HC_Cardiovascular"].ToString(),
                            HC_GastroIntestinal = Lectura_Cod_Prof["HC_GastroIntestinal"].ToString(),
                            HC_GastroUrinario = Lectura_Cod_Prof["HC_GastroUrinario"].ToString(),
                            HC_Ocupacion = Lectura_Cod_Prof["HC_Ocupacion"].ToString(),
                            HC_Piel = Lectura_Cod_Prof["HC_Piel"].ToString(),
                            HC_OsteoMuscular = Lectura_Cod_Prof["HC_OsteoMuscular"].ToString(),

                            HC_AntFam = Lectura_Cod_Prof["HC_AntFam"].ToString(),
                            HC_AntPat = Lectura_Cod_Prof["HC_AntPat"].ToString(),
                            HC_AntFarma = Lectura_Cod_Prof["HC_AntFarma"].ToString(),
                            HC_AntQui = Lectura_Cod_Prof["HC_AntQui"].ToString(),
                            HC_AntAle = Lectura_Cod_Prof["HC_AntAle"].ToString(),
                            HC_Hematolin = Lectura_Cod_Prof["HC_Hematolin"].ToString(),

                            HC_EstadoNut = Lectura_Cod_Prof["HC_EstadoNut"].ToString(),
                            HC_Presart = Lectura_Cod_Prof["HC_Presart"].ToString(),
                            HC_Frecar = Lectura_Cod_Prof["HC_Frecar"].ToString(),
                            HC_Temp = Lectura_Cod_Prof["HC_Temp"].ToString(),
                            HC_FreRes = Lectura_Cod_Prof["HC_FreRes"].ToString(),
                            HC_Peso = Lectura_Cod_Prof["HC_Peso"].ToString(),
                            HC_Altura = Lectura_Cod_Prof["HC_Altura"].ToString(),
                            HC_IMC = Lectura_Cod_Prof["HC_IMC"].ToString(),
                            HC_ITB = Lectura_Cod_Prof["HC_ITB"].ToString(),

                            HC_TejCom = Lectura_Cod_Prof["HC_TejCom"].ToString(),
                            HC_CaracTej = Lectura_Cod_Prof["HC_CaracTej"].ToString(),
                            HC_SignosInf = Lectura_Cod_Prof["HC_SignosInf"].ToString(),
                            HC_PielCirc = Lectura_Cod_Prof["HC_PielCirc"].ToString(),
                            HC_Exudado = Lectura_Cod_Prof["HC_Exudado"].ToString(),
                            HC_ConsCant = Lectura_Cod_Prof["HC_ConsCant"].ToString(),
                            HC_Estado = Lectura_Cod_Prof["HC_Estado"].ToString(),
                            HC_Dolor = Lectura_Cod_Prof["HC_Dolor"].ToString(),
                            HC_DescHer = Lectura_Cod_Prof["HC_DescHer"].ToString(),

                            HC_Imp_Dx = Lectura_Cod_Prof["HC_Imp_Dx"].ToString(),
                            HC_Patologia = Lectura_Cod_Prof["HC_Patologia"].ToString(),
                            HC_SubPat = Lectura_Cod_Prof["HC_SubPat"].ToString(),
                            HC_RH = Lectura_Cod_Prof["HC_RH"].ToString(),
                            HC_DX1T = Lectura_Cod_Prof["HC_DX1T"].ToString(),
                            HC_Analisis = Lectura_Cod_Prof["HC_Analisis"].ToString(),
                            HC_PManejo = Lectura_Cod_Prof["HC_PManejo"].ToString(),
                            HC_Complicacion = Lectura_Cod_Prof["HC_Complicacion"].ToString(),
                            HC_DX1 = Lectura_Cod_Prof["HC_DX1"].ToString(),
                            HC_DX2 = Lectura_Cod_Prof["HC_DX2"].ToString(),
                            HC_DX3 = Lectura_Cod_Prof["HC_DX3"].ToString(),
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
        CXN_HCMG IMedicinaGeneral.getLastHistory(int Paciente, DateTime Fecha)
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
                                             "FROM CXN_HCMG " +
                                             "WHERE HC_PacId = '" + Paciente + "' " +
                                             "AND HC_Fecha <> '" + Convert.ToDateTime(Fecha).ToString(getData["Format_Fecha"]) + "' " +
                                             "AND HC_Cant = '1' " +
                                             "ORDER BY HC_Fecha DESC";
                    SqlCommand Carga_Cod_Prof = new SqlCommand(Cargar_Cod_Prof, con);
                    SqlDataReader Lectura_Cod_Prof = (Carga_Cod_Prof.ExecuteReader());
                    if (Lectura_Cod_Prof.Read() == true)
                    {
                        CXN_HCMG H = new CXN_HCMG
                        {
                            HC_MotivoC = Lectura_Cod_Prof["HC_MotivoC"].ToString(),
                            HC_EnfA = Lectura_Cod_Prof["HC_EnfA"].ToString(),
                            HC_PruebasDiag = Lectura_Cod_Prof["HC_PruebasDiag"].ToString(),

                            HC_Neurologico = Lectura_Cod_Prof["HC_Neurologico"].ToString(),
                            HC_Respiratorio = Lectura_Cod_Prof["HC_Respiratorio"].ToString(),
                            HC_Cardiovascular = Lectura_Cod_Prof["HC_Cardiovascular"].ToString(),
                            HC_GastroIntestinal = Lectura_Cod_Prof["HC_GastroIntestinal"].ToString(),
                            HC_GastroUrinario = Lectura_Cod_Prof["HC_GastroUrinario"].ToString(),
                            HC_Ocupacion = Lectura_Cod_Prof["HC_Ocupacion"].ToString(),
                            HC_Piel = Lectura_Cod_Prof["HC_Piel"].ToString(),
                            HC_OsteoMuscular = Lectura_Cod_Prof["HC_OsteoMuscular"].ToString(),

                            HC_AntFam = Lectura_Cod_Prof["HC_AntFam"].ToString(),
                            HC_AntPat = Lectura_Cod_Prof["HC_AntPat"].ToString(),
                            HC_AntFarma = Lectura_Cod_Prof["HC_AntFarma"].ToString(),
                            HC_AntQui = Lectura_Cod_Prof["HC_AntQui"].ToString(),
                            HC_AntAle = Lectura_Cod_Prof["HC_AntAle"].ToString(),
                            HC_Hematolin = Lectura_Cod_Prof["HC_Hematolin"].ToString(),

                            HC_EstadoNut = Lectura_Cod_Prof["HC_EstadoNut"].ToString(),
                            HC_Presart = Lectura_Cod_Prof["HC_Presart"].ToString(),
                            HC_Frecar = Lectura_Cod_Prof["HC_Frecar"].ToString(),
                            HC_Temp = Lectura_Cod_Prof["HC_Temp"].ToString(),
                            HC_FreRes = Lectura_Cod_Prof["HC_FreRes"].ToString(),
                            HC_Peso = Lectura_Cod_Prof["HC_Peso"].ToString(),
                            HC_Altura = Lectura_Cod_Prof["HC_Altura"].ToString(),
                            HC_IMC = Lectura_Cod_Prof["HC_IMC"].ToString(),
                            HC_ITB = Lectura_Cod_Prof["HC_ITB"].ToString(),

                            HC_TejCom = Lectura_Cod_Prof["HC_TejCom"].ToString(),
                            HC_CaracTej = Lectura_Cod_Prof["HC_CaracTej"].ToString(),
                            HC_SignosInf = Lectura_Cod_Prof["HC_SignosInf"].ToString(),
                            HC_PielCirc = Lectura_Cod_Prof["HC_PielCirc"].ToString(),
                            HC_Exudado = Lectura_Cod_Prof["HC_Exudado"].ToString(),
                            HC_ConsCant = Lectura_Cod_Prof["HC_ConsCant"].ToString(),
                            HC_Estado = Lectura_Cod_Prof["HC_Estado"].ToString(),
                            HC_Dolor = Lectura_Cod_Prof["HC_Dolor"].ToString(),
                            HC_DescHer = Lectura_Cod_Prof["HC_DescHer"].ToString(),

                            HC_Imp_Dx = Lectura_Cod_Prof["HC_Imp_Dx"].ToString(),
                            HC_Patologia = Lectura_Cod_Prof["HC_Patologia"].ToString(),
                            HC_SubPat = Lectura_Cod_Prof["HC_SubPat"].ToString(),
                            HC_RH = Lectura_Cod_Prof["HC_RH"].ToString(),
                            HC_DX1T = Lectura_Cod_Prof["HC_DX1T"].ToString(),
                            HC_Analisis = Lectura_Cod_Prof["HC_Analisis"].ToString(),
                            HC_PManejo = Lectura_Cod_Prof["HC_PManejo"].ToString(),
                            HC_Complicacion = Lectura_Cod_Prof["HC_Complicacion"].ToString(),
                            HC_DX1 = Lectura_Cod_Prof["HC_DX1"].ToString(),
                            HC_DX2 = Lectura_Cod_Prof["HC_DX2"].ToString(),
                            HC_DX3 = Lectura_Cod_Prof["HC_DX3"].ToString(),
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
        bool IMedicinaGeneral.ActualizaHCMG(CXN_HCMG H)
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

                    string Busqueda = (@"UPDATE CXN_HCMG " +
                                      "SET HC_Pac = @param1, " +
                                                         "HC_Prof = @param2, " +
                                                         "HC_Ase = @param3, " +
                                                         "HC_Com = @param4, " +
                                                         "HC_PacId = @param5, " +
                                                         "HC_Edad = @param6, " +
                                                         "HC_FechaNto = @param7, " +
                                                         "HC_MotivoC = @param8, " +
                                                         "HC_EnfA = @param9, " +
                                                         "HC_GradoC = @param10, " +
                                                         "HC_TipoLes = @param11, " +
                                                         "HC_ActEje = @param12, " +
                                                         "HC_Vez = @param13, " +
                                                         "HC_Neurologico = @param14, " +
                                                         "HC_Cardiovascular = @param15, " +
                                                         "HC_Gastrointestinal = @param16, " +
                                                         "HC_Gastrourinario = @param17, " +
                                                         "HC_OsteoMuscular = @param18, " +
                                                         "HC_Piel = @param19, " +
                                                         "HC_Ocupacion = @param20, " +
                                                         "HC_AparienciaG = @param21, " +
                                                         "HC_EstadoEmo = @param22, " +
                                                         "HC_EstadoNut = @param23, " +
                                                         "HC_Exudado = @param24, " +
                                                         "HC_PresArt = @param25, " +
                                                         "HC_Frecar = @param26, " +
                                                         "HC_FreRes = @param27, " +
                                                         "HC_Temp = @param28, " +
                                                         "HC_Peso = @param29, " +
                                                         "HC_Altura = @param30, " +
                                                         "HC_IMC = @param31, " +
                                                         "HC_ITB = @param32, " +
                                                         "HC_DescHer = @param33, " +
                                                         "HC_TejCom = @param34, " +
                                                         "HC_CaracTej = @param35, " +
                                                         "HC_SignosInf = @param36, " +
                                                         "HC_PielCirc = @param37, " +
                                                         "HC_ConsCant = @param38, " +
                                                         "HC_Estado = @param39, " +
                                                         "HC_Dolor = @param40, " +
                                                         "HC_Analisis = @param41, " +
                                                         "HC_Complicacion = @param42, " +
                                                         "HC_PruebasDiag = @param43, " +
                                                         "HC_ProtoInst = @param44, " +
                                                         "HC_PManejo = @param45, " +
                                                         "HC_DX1 = @param46, " +
                                                         "HC_DX2 = @param47, " +
                                                         "HC_DX3 = @param48, " +
                                                         "HC_DX1T = @param49, " +
                                                         "HC_AntFam = @param50, " +
                                                         "HC_AntPat = @param51, " +
                                                         "HC_AntQui = @param52, " +
                                                         "HC_AntAle = @param53, " +
                                                         "HC_AntFarma = @param54, " +
                                                         "HC_Hematolin = @param55, " +
                                                         "HC_Patologia = @param56, " +
                                                         "HC_Fecha = @param57, " +
                                                         "HC_Cant = @param58, " +
                                                         "HC_RH = @param59, " +
                                                         "HC_SubPat = @param60, " +
                                                         "HC_Imp_Dx = @param61, " +
                                                         "HC_Respiratorio = @param62, " +
                                                         "HC_Epidemia = @param63, " +
                                                         "HC_ServCatalogo = @param64, " +
                                                         "HC_CupCatalogo = @param65, " +
                                                         "HC_TipoINGSAL = @param66 " +
                                      "WHERE HC_Adm = '" + H.HC_Adm + "' " +
                                      "AND HC_Cant = '0'");
                    SqlCommand Accion = new SqlCommand(Busqueda, con);

                    Accion.Parameters.AddWithValue("@param1", H.HC_Pac);
                    Accion.Parameters.AddWithValue("@param2", H.HC_Prof);
                    Accion.Parameters.AddWithValue("@param3", H.HC_Ase);
                    Accion.Parameters.Add(new SqlParameter("@param7", SqlDbType.DateTime)).Value = H.HC_FechaNto;
                    Accion.Parameters.AddWithValue("@param4", H.HC_Com);
                    Accion.Parameters.AddWithValue("@param5", H.HC_Pacid);
                    Accion.Parameters.AddWithValue("@param6", H.HC_Edad);
                    Accion.Parameters.AddWithValue("@param8", H.HC_MotivoC);
                    Accion.Parameters.AddWithValue("@param9", H.HC_EnfA);
                    Accion.Parameters.AddWithValue("@param10", H.HC_GradoC);
                    Accion.Parameters.AddWithValue("@param11", H.HC_TipoLes);
                    Accion.Parameters.AddWithValue("@param12", H.HC_ActEje);
                    Accion.Parameters.AddWithValue("@param13", H.HC_Vez);
                    Accion.Parameters.AddWithValue("@param14", H.HC_Neurologico);
                    Accion.Parameters.AddWithValue("@param15", H.HC_Cardiovascular);
                    Accion.Parameters.AddWithValue("@param16", H.HC_GastroIntestinal);
                    Accion.Parameters.AddWithValue("@param17", H.HC_GastroUrinario);
                    Accion.Parameters.AddWithValue("@param18", H.HC_OsteoMuscular);
                    Accion.Parameters.AddWithValue("@param19", H.HC_Piel);
                    Accion.Parameters.AddWithValue("@param20", H.HC_Ocupacion);
                    Accion.Parameters.AddWithValue("@param21", H.HC_AparienciaG);
                    Accion.Parameters.AddWithValue("@param22", H.HC_EstadoEmo);
                    Accion.Parameters.AddWithValue("@param23", H.HC_EstadoNut);
                    Accion.Parameters.AddWithValue("@param24", H.HC_Exudado);
                    Accion.Parameters.AddWithValue("@param25", H.HC_Presart);
                    Accion.Parameters.AddWithValue("@param26", H.HC_Frecar);
                    Accion.Parameters.AddWithValue("@param27", H.HC_FreRes);
                    Accion.Parameters.AddWithValue("@param28", H.HC_Temp);
                    Accion.Parameters.AddWithValue("@param29", H.HC_Peso);
                    Accion.Parameters.AddWithValue("@param30", H.HC_Altura);
                    Accion.Parameters.AddWithValue("@param31", H.HC_IMC);
                    Accion.Parameters.AddWithValue("@param32", H.HC_ITB);
                    Accion.Parameters.AddWithValue("@param33", H.HC_DescHer);
                    Accion.Parameters.AddWithValue("@param34", H.HC_TejCom);
                    Accion.Parameters.AddWithValue("@param35", H.HC_CaracTej);
                    Accion.Parameters.AddWithValue("@param36", H.HC_SignosInf);
                    Accion.Parameters.AddWithValue("@param37", H.HC_PielCirc);
                    Accion.Parameters.AddWithValue("@param38", H.HC_ConsCant);
                    Accion.Parameters.AddWithValue("@param39", H.HC_Estado);
                    Accion.Parameters.AddWithValue("@param40", H.HC_Dolor);
                    Accion.Parameters.AddWithValue("@param41", H.HC_Analisis);
                    Accion.Parameters.AddWithValue("@param42", H.HC_Complicacion);
                    Accion.Parameters.AddWithValue("@param43", H.HC_PruebasDiag);
                    Accion.Parameters.AddWithValue("@param44", H.HC_ProtoInst);
                    Accion.Parameters.AddWithValue("@param45", H.HC_PManejo);
                    Accion.Parameters.AddWithValue("@param46", H.HC_DX1);
                    Accion.Parameters.AddWithValue("@param47", H.HC_DX2);
                    Accion.Parameters.AddWithValue("@param48", H.HC_DX3);
                    Accion.Parameters.AddWithValue("@param49", H.HC_DX1T);
                    Accion.Parameters.AddWithValue("@param50", H.HC_AntFam); //FAMILIAR
                    Accion.Parameters.AddWithValue("@param51", H.HC_AntPat);
                    Accion.Parameters.AddWithValue("@param52", H.HC_AntQui);
                    Accion.Parameters.AddWithValue("@param53", H.HC_AntAle);
                    Accion.Parameters.AddWithValue("@param54", H.HC_AntFarma); //FARMACOLOGICO
                    Accion.Parameters.AddWithValue("@param55", H.HC_Hematolin);
                    Accion.Parameters.AddWithValue("@param56", H.HC_Patologia);
                    Accion.Parameters.Add(new SqlParameter("@param57", SqlDbType.DateTime)).Value = H.HC_Fecha;
                    Accion.Parameters.AddWithValue("@param58", H.HC_Cant);
                    Accion.Parameters.AddWithValue("@param59", H.HC_RH);
                    Accion.Parameters.AddWithValue("@param60", H.HC_SubPat);
                    Accion.Parameters.AddWithValue("@param61", H.HC_Imp_Dx);
                    Accion.Parameters.AddWithValue("@param62", H.HC_Respiratorio);
                    Accion.Parameters.AddWithValue("@param63", H.HC_Epidemia);
                    Accion.Parameters.AddWithValue("@param64", H.HC_ServCatalogo);
                    Accion.Parameters.AddWithValue("@param65", H.HC_CupCatalogo);
                    Accion.Parameters.AddWithValue("@param66", H.HC_TipoINGSAL);
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
        bool IMedicinaGeneral.GrabaHCMG(CXN_HCMG H)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_HCMG " +
                                                         "(HC_Pac, " +
                                                         "HC_Prof, " +
                                                         "HC_Ase, " +
                                                         "HC_Com, " +
                                                         "HC_PacId, " +
                                                         "HC_Edad, " +
                                                         "HC_FechaNto, " +
                                                         "HC_MotivoC, " +
                                                         "HC_EnfA, " +
                                                         "HC_GradoC, " +
                                                         "HC_TipoLes, " +
                                                         "HC_ActEje, " +
                                                         "HC_Vez, " +
                                                         "HC_Neurologico, " +
                                                         "HC_Cardiovascular, " +
                                                         "HC_Gastrointestinal, " +
                                                         "HC_Gastrourinario, " +
                                                         "HC_OsteoMuscular, " +
                                                         "HC_Piel, " +
                                                         "HC_Ocupacion, " +
                                                         "HC_AparienciaG, " +
                                                         "HC_EstadoEmo, " +
                                                         "HC_EstadoNut, " +
                                                         "HC_Exudado, " +
                                                         "HC_PresArt, " +
                                                         "HC_Frecar, " +
                                                         "HC_FreRes, " +
                                                         "HC_Temp, " +
                                                         "HC_Peso, " +
                                                         "HC_Altura, " +
                                                         "HC_IMC, " +
                                                         "HC_ITB, " +
                                                         "HC_DescHer, " +
                                                         "HC_TejCom, " +
                                                         "HC_CaracTej, " +
                                                         "HC_SignosInf, " +
                                                         "HC_PielCirc, " +
                                                         "HC_ConsCant, " +
                                                         "HC_Estado, " +
                                                         "HC_Dolor, " +
                                                         "HC_Analisis, " +
                                                         "HC_Complicacion, " +
                                                         "HC_PruebasDiag, " +
                                                         "HC_ProtoInst, " +
                                                         "HC_PManejo, " +
                                                         "HC_DX1, " +
                                                         "HC_DX2, " +
                                                         "HC_DX3, " +
                                                         "HC_DX1T, " +
                                                         "HC_AntFam, " +
                                                         "HC_AntPat, " +
                                                         "HC_AntQui, " +
                                                         "HC_AntAle, " +
                                                         "HC_AntFarma, " +
                                                         "HC_Hematolin, " +
                                                         "HC_Patologia, " +
                                                         "HC_Fecha, " +
                                                         "HC_Adm, " +
                                                         "HC_Cant, " +
                                                         "HC_RH, " +
                                                         "HC_SubPat, " +
                                                         "HC_Imp_Dx, " +
                                                         "HC_Respiratorio, " +
                                                         "HC_Epidemia, " +
                                                         "HC_ServCatalogo, " +
                                                         "HC_CupCatalogo, " +
                                                         "HC_TipoINGSAL) " +
                                     "VALUES                  (@param1, " +
                                                              "@param2, @param3, @param4, @param5, " +
                                                              "@param6, @param7, @param8, @param9, " +
                                                              "@param10, @param11, @param12, @param13, " +
                                                              "@param14, @param15, @param16, @param17, " +
                                                              "@param18, @param19, @param20, @param21, " +
                                                              "@param22, @param23, @param24, @param25, " +
                                                              "@param26, @param27, @param28, @param29, " +
                                                              "@param30, @param31, @param32, @param33, " +
                                                              "@param34, @param35, @param36, @param37, " +
                                                              "@param38, @param39, @param40, @param41, " +
                                                              "@param42, @param43, @param44, @param45, " +
                                                              "@param46, @param47, @param48, @param49, " +
                                                              "@param50, @param51, @param52, @param53, " +
                                                              "@param54, @param55, @param56, @param57, " +
                                                              "@param58, @param59, @param60, @param61, @param62, @param63, @param64, @param65, @param66, @param67)", con);

                    cmd.Parameters.AddWithValue("@param1", H.HC_Pac);
                    cmd.Parameters.AddWithValue("@param2", H.HC_Prof);
                    cmd.Parameters.AddWithValue("@param3", H.HC_Ase);
                    cmd.Parameters.Add(new SqlParameter("@param7", SqlDbType.DateTime)).Value = H.HC_FechaNto;
                    cmd.Parameters.AddWithValue("@param4", H.HC_Com);
                    cmd.Parameters.AddWithValue("@param5", H.HC_Pacid);
                    cmd.Parameters.AddWithValue("@param6", H.HC_Edad);
                    cmd.Parameters.AddWithValue("@param8", H.HC_MotivoC);
                    cmd.Parameters.AddWithValue("@param9", H.HC_EnfA);
                    cmd.Parameters.AddWithValue("@param10", H.HC_GradoC);
                    cmd.Parameters.AddWithValue("@param11", H.HC_TipoLes);
                    cmd.Parameters.AddWithValue("@param12", H.HC_ActEje);
                    cmd.Parameters.AddWithValue("@param13", H.HC_Vez);
                    cmd.Parameters.AddWithValue("@param14", H.HC_Neurologico);
                    cmd.Parameters.AddWithValue("@param15", H.HC_Cardiovascular);
                    cmd.Parameters.AddWithValue("@param16", H.HC_GastroIntestinal);
                    cmd.Parameters.AddWithValue("@param17", H.HC_GastroUrinario);
                    cmd.Parameters.AddWithValue("@param18", H.HC_OsteoMuscular);
                    cmd.Parameters.AddWithValue("@param19", H.HC_Piel);
                    cmd.Parameters.AddWithValue("@param20", H.HC_Ocupacion);
                    cmd.Parameters.AddWithValue("@param21", H.HC_AparienciaG);
                    cmd.Parameters.AddWithValue("@param22", H.HC_EstadoEmo);
                    cmd.Parameters.AddWithValue("@param23", H.HC_EstadoNut);
                    cmd.Parameters.AddWithValue("@param24", H.HC_Exudado);
                    cmd.Parameters.AddWithValue("@param25", H.HC_Presart);
                    cmd.Parameters.AddWithValue("@param26", H.HC_Frecar);
                    cmd.Parameters.AddWithValue("@param27", H.HC_FreRes);
                    cmd.Parameters.AddWithValue("@param28", H.HC_Temp);
                    cmd.Parameters.AddWithValue("@param29", H.HC_Peso);
                    cmd.Parameters.AddWithValue("@param30", H.HC_Altura);
                    cmd.Parameters.AddWithValue("@param31", H.HC_IMC);
                    cmd.Parameters.AddWithValue("@param32", H.HC_ITB);
                    cmd.Parameters.AddWithValue("@param33", H.HC_DescHer);
                    cmd.Parameters.AddWithValue("@param34", H.HC_TejCom);
                    cmd.Parameters.AddWithValue("@param35", H.HC_CaracTej);
                    cmd.Parameters.AddWithValue("@param36", H.HC_SignosInf);
                    cmd.Parameters.AddWithValue("@param37", H.HC_PielCirc);
                    cmd.Parameters.AddWithValue("@param38", H.HC_ConsCant);
                    cmd.Parameters.AddWithValue("@param39", H.HC_Estado);
                    cmd.Parameters.AddWithValue("@param40", H.HC_Dolor);
                    cmd.Parameters.AddWithValue("@param41", H.HC_Analisis);
                    cmd.Parameters.AddWithValue("@param42", H.HC_Complicacion);
                    cmd.Parameters.AddWithValue("@param43", H.HC_PruebasDiag);
                    cmd.Parameters.AddWithValue("@param44", H.HC_ProtoInst);
                    cmd.Parameters.AddWithValue("@param45", H.HC_PManejo);
                    cmd.Parameters.AddWithValue("@param46", H.HC_DX1);
                    cmd.Parameters.AddWithValue("@param47", H.HC_DX2);
                    cmd.Parameters.AddWithValue("@param48", H.HC_DX3);
                    cmd.Parameters.AddWithValue("@param49", H.HC_DX1T);
                    cmd.Parameters.AddWithValue("@param50", H.HC_AntFam); //FAMILIAR
                    cmd.Parameters.AddWithValue("@param51", H.HC_AntPat);
                    cmd.Parameters.AddWithValue("@param52", H.HC_AntQui);
                    cmd.Parameters.AddWithValue("@param53", H.HC_AntAle);
                    cmd.Parameters.AddWithValue("@param54", H.HC_AntFarma); //FARMACOLOGICO
                    cmd.Parameters.AddWithValue("@param55", H.HC_Hematolin);
                    cmd.Parameters.AddWithValue("@param56", H.HC_Patologia);
                    cmd.Parameters.Add(new SqlParameter("@param57", SqlDbType.DateTime)).Value = H.HC_Fecha;
                    cmd.Parameters.AddWithValue("@param58", H.HC_Adm);
                    cmd.Parameters.AddWithValue("@param59", H.HC_Cant);
                    cmd.Parameters.AddWithValue("@param60", H.HC_RH);
                    cmd.Parameters.AddWithValue("@param61", H.HC_SubPat);
                    cmd.Parameters.AddWithValue("@param62", H.HC_Imp_Dx);
                    cmd.Parameters.AddWithValue("@param63", H.HC_Respiratorio);
                    cmd.Parameters.AddWithValue("@param64", H.HC_Epidemia);
                    cmd.Parameters.AddWithValue("@param65", H.HC_ServCatalogo);
                    cmd.Parameters.AddWithValue("@param66", H.HC_CupCatalogo);
                    cmd.Parameters.AddWithValue("@param67", H.HC_TipoINGSAL);
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
        List<CXN_HCMG> IMedicinaGeneral.ResumenHCMGNotas(int Paciente)
        {
            Dictionary<string,string> getData = Conexion.Conection();

            try
            {
                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora2 = "SELECT C.Car_Adm_Id, H.Hor_Pac_Fecha_Cita, B.Bod_Responsable " +
                                          "FROM CXN_HCMG N " +
                                          "INNER JOIN CXN_CARGOS C ON N.HC_Adm = C.Car_Adm_Id " +
                                          "INNER JOIN CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero " +
                                          "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                          "WHERE C.Car_Pac = '" + Paciente + "' " +
                                          "AND C.Car_Tipo = 'Historia' " +
                                          "AND N.HC_Cant = '1' " +
                                          "ORDER BY C.Car_Fecha DESC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<CXN_HCMG> L = new List<CXN_HCMG>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            L.Add(new CXN_HCMG
                            {
                                HC_Adm = Convert.ToInt32(Lectura_Hora2["Car_Adm_Id"]),
                                HC_Fecha = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]),
                                HC_PManejo = Lectura_Hora2["Bod_Responsable"].ToString()
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
        CXN_HCMG IMedicinaGeneral.getResumen(int Admision)
        {
            Dictionary<string,string> getData = Conexion.Conection();

            try
            {
                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora2 = "SELECT TOP 50 HC_PManejo, HC_DescHer, HC_Patologia, HC_Piel, HC_EnfA, HC_CaracTej, " +
                                          "HC_DX1, HC_DX2, HC_DX3, HC_DX1T, HC_CupCatalogo, HC_ServCatalogo, HC_Nota_Acl " +
                                          "FROM CXN_HCMG " +
                                          "WHERE HC_Adm = '" + Admision + "' " +
                                          "AND HC_Cant = '1' " +
                                          "ORDER BY HC_Fecha DESC";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                CXN_HCMG H = new CXN_HCMG
                                {
                                    HC_PManejo = Lectura_Hora2["HC_PManejo"].ToString(),
                                    HC_DescHer = Lectura_Hora2["HC_DescHer"].ToString(),
                                    HC_Patologia = Lectura_Hora2["HC_Patologia"].ToString(),
                                    HC_Piel = Lectura_Hora2["HC_Piel"].ToString(),
                                    HC_EnfA = Lectura_Hora2["HC_EnfA"].ToString(),
                                    HC_CaracTej = Lectura_Hora2["HC_CaracTej"].ToString(),
                                    HC_DX1 = Lectura_Hora2["HC_DX1"].ToString(),
                                    HC_DX1T = Lectura_Hora2["HC_DX1T"].ToString(),
                                    HC_DX2 = Lectura_Hora2["HC_DX2"].ToString(),
                                    HC_DX3 = Lectura_Hora2["HC_DX3"].ToString(),
                                    HC_CupCatalogo = Lectura_Hora2["HC_CupCatalogo"].ToString(),
                                    HC_ServCatalogo = Lectura_Hora2["HC_ServCatalogo"].ToString(),
                                    HC_Nota_Acl = Lectura_Hora2["HC_Nota_Acl"].ToString(),
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
        CXN_HCMG IMedicinaGeneral.getLastHistoryToCopy(int Paciente)
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

                    DateTime Hoy = DateTime.Now.Date;

                    String Cargar_Cod_Prof = "SELECT TOP 1 * " +
                                             "FROM CXN_HCMG " +
                                             "WHERE HC_PacId = '" + Paciente + "' " +
                                             "AND HC_Fecha <> '" + Convert.ToDateTime(Hoy).ToString(getData["Format_Fecha"]) + "' " +
                                             "AND HC_Cant = '1' " +
                                             "ORDER BY HC_Fecha DESC";
                    SqlCommand Carga_Cod_Prof = new SqlCommand(Cargar_Cod_Prof, con);
                    SqlDataReader getH = (Carga_Cod_Prof.ExecuteReader());
                    if (getH.Read() == true)
                    {
                        CXN_HCMG Hcmg = new CXN_HCMG
                        {
                            HC_Adm = Convert.ToInt32(getH["HC_Adm"]),
                            HC_Prof = Convert.ToInt32(getH["HC_Prof"]),
                            HC_Ase = Convert.ToInt32(getH["HC_Ase"]),
                            HC_Com = Convert.ToInt32(getH["HC_Com"]),
                            HC_Pacid = Convert.ToInt32(getH["HC_Pacid"]),
                            HC_MotivoC = "Ingresa paciente a consulta para dar inicio al paquete. Se continúa con igual  manejo instaurado por el Medico General Institucional",
                            HC_EnfA = getH["HC_EnfA"].ToString(),
                            HC_GradoC = getH["HC_GradoC"].ToString(),
                            HC_TipoLes = getH["HC_TipoLes"].ToString(),
                            HC_ActEje = getH["HC_ActEje"].ToString(),
                            HC_Vez = getH["HC_Vez"].ToString(),
                            HC_Neurologico = getH["HC_Neurologico"].ToString(),
                            HC_Cardiovascular = getH["HC_Cardiovascular"].ToString(),
                            HC_GastroIntestinal = getH["HC_GastroIntestinal"].ToString(),
                            HC_GastroUrinario = getH["HC_GastroUrinario"].ToString(),
                            HC_OsteoMuscular = getH["HC_OsteoMuscular"].ToString(),
                            HC_Piel = getH["HC_Piel"].ToString(),
                            HC_Ocupacion = getH["HC_Ocupacion"].ToString(),
                            HC_AparienciaG = getH["HC_AparienciaG"].ToString(),
                            HC_EstadoEmo = getH["HC_EstadoEmo"].ToString(),
                            HC_EstadoNut = getH["HC_EstadoNut"].ToString(),
                            HC_Exudado = getH["HC_Exudado"].ToString(),
                            HC_Presart = getH["HC_Presart"].ToString(),
                            HC_Frecar = getH["HC_Frecar"].ToString(),
                            HC_FreRes = getH["HC_FreRes"].ToString(),
                            HC_Temp = getH["HC_Temp"].ToString(),
                            HC_Peso = getH["HC_Peso"].ToString(),
                            HC_Altura = getH["HC_Altura"].ToString(),
                            HC_IMC = getH["HC_IMC"].ToString(),
                            HC_ITB = getH["HC_ITB"].ToString(),
                            HC_DescHer = getH["HC_DescHer"].ToString(),
                            HC_TejCom = getH["HC_TejCom"].ToString(),
                            HC_CaracTej = getH["HC_CaracTej"].ToString(),
                            HC_SignosInf = getH["HC_SignosInf"].ToString(),
                            HC_PielCirc = getH["HC_PielCirc"].ToString(),
                            HC_ConsCant = getH["HC_ConsCant"].ToString(),
                            HC_Estado = getH["HC_Estado"].ToString(),
                            HC_Dolor = getH["HC_Dolor"].ToString(),
                            HC_Analisis = getH["HC_Analisis"].ToString(),
                            HC_Complicacion = getH["HC_Complicacion"].ToString(),
                            HC_PruebasDiag = getH["HC_PruebasDiag"].ToString(),
                            HC_ProtoInst = getH["HC_ProtoInst"].ToString(),
                            HC_PManejo = getH["HC_PManejo"].ToString(),
                            HC_DX1 = getH["HC_DX1"].ToString(),
                            HC_DX2 = getH["HC_DX2"].ToString(),
                            HC_DX3 = getH["HC_DX3"].ToString(),
                            HC_DX1T = getH["HC_DX1T"].ToString(),
                            HC_AntFam = getH["HC_AntFam"].ToString(),
                            HC_AntPat = getH["HC_AntPat"].ToString(),
                            HC_AntQui = getH["HC_AntQui"].ToString(),
                            HC_AntAle = getH["HC_AntAle"].ToString(),
                            HC_AntFarma = getH["HC_AntFarma"].ToString(),
                            HC_Hematolin = getH["HC_Hematolin"].ToString(),
                            HC_Patologia = getH["HC_Patologia"].ToString(),
                            HC_Cant = 1,
                            HC_RH = getH["HC_RH"].ToString(),
                            HC_SubPat = getH["HC_SubPat"].ToString(),
                            HC_Imp_Dx = getH["HC_Imp_Dx"].ToString(),
                            HC_Respiratorio = getH["HC_Respiratorio"].ToString(),
                            HC_Epidemia = getH["HC_Epidemia"].ToString(),
                            HC_ServCatalogo = getH["HC_ServCatalogo"].ToString(),
                            HC_CupCatalogo = getH["HC_CupCatalogo"].ToString(),
                            HC_TipoINGSAL = getH["HC_TipoINGSAL"].ToString()
                        };

                        return Hcmg;
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
        List<CXN_HCMG> IMedicinaGeneral.ListaUltimasCitas(int Paciente, DateTime Fecha, int Medico)
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
                                          "FROM CXN_HCMG H " +
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
