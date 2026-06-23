using System;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System.Collections.Generic;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MPsicologia : IPsicologia
    {
        CXN_HCPSI IPsicologia.getLastHistory(int Paciente)
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

                    String Cargar_Agenda = "SELECT * " +
                                           "FROM CXN_HCPSI " +
                                           "WHERE HC_Pac_Id = '" + Paciente + "' " +
                                           "ORDER BY HC_Fecha DESC";
                    SqlCommand Carga_Agenda = new SqlCommand(Cargar_Agenda, con);
                    SqlDataReader Lectura_Agenda = (Carga_Agenda.ExecuteReader());
                    if (Lectura_Agenda.Read() == true)
                    {
                        CXN_HCPSI H = new CXN_HCPSI
                        {
                            HC_Patologico = Lectura_Agenda["HC_Patologico"].ToString(),
                            HC_Quirurgico = Lectura_Agenda["HC_Quirurgico"].ToString(),
                            HC_Farmacologico = Lectura_Agenda["HC_Farmacologico"].ToString(),
                            HC_Fracturas = Lectura_Agenda["HC_Fracturas"].ToString(),
                            HC_Tox_Ale = Lectura_Agenda["HC_Tox_Ale"].ToString(),
                            HC_Familiares = Lectura_Agenda["HC_Familiares"].ToString(),
                            HC_Psicologicos = Lectura_Agenda["HC_Psicologicos"].ToString(),
                            HC_Otros = Lectura_Agenda["HC_Otros"].ToString(),
                            HC_Esc_Dol = Lectura_Agenda["HC_Esc_Dol"].ToString(),
                            HC_Caracteristica = Lectura_Agenda["HC_Caracteristica"].ToString(),
                            HC_TEvolucion = Lectura_Agenda["HC_TEvolucion"].ToString(),
                            HC_Fisiatra = Lectura_Agenda["HC_Fisiatra"].ToString(),
                            HC_Psiquiatra = Lectura_Agenda["HC_Psiquiatra"].ToString(),
                            HC_Remite = Lectura_Agenda["HC_Remite"].ToString(),
                            HC_RedApSoc = Lectura_Agenda["HC_RedApSoc"].ToString(),
                            HC_FSoporte = Lectura_Agenda["HC_FSoporte"].ToString(),
                            HC_TFamilia = Lectura_Agenda["HC_TFamilia"].ToString(),
                            HC_TRelacion = Lectura_Agenda["HC_TRelacion"].ToString(),
                            HC_Estado = Lectura_Agenda["HC_Estado"].ToString(),
                            HC_TRelacion_2 = Lectura_Agenda["HC_TRelacion_2"].ToString(),
                            HC_OtroSoporte = Lectura_Agenda["HC_OtroSoporte"].ToString(),
                            HC_ObSoporte = Lectura_Agenda["HC_ObSoporte"].ToString(),
                            HC_Sust = Lectura_Agenda["HC_Sust"].ToString(),
                            HC_Compo = Lectura_Agenda["HC_Compo"].ToString(),
                            HC_EstadoOtro = Lectura_Agenda["HC_EstadoOtro"].ToString(),
                            HC_TiRelacion = Lectura_Agenda["HC_TiRelacion"].ToString(),
                            HC_Sust_2 = Lectura_Agenda["HC_Sust_2"].ToString(),
                            HC_EstSex = Lectura_Agenda["HC_EstSex"].ToString(),
                            HC_DolMol = Lectura_Agenda["HC_DolMol"].ToString(),
                            HC_AutoEsq = Lectura_Agenda["HC_AutoEsq"].ToString(),
                            HC_Satis = Lectura_Agenda["HC_Satis"].ToString(),
                            HC_ObservSex = Lectura_Agenda["HC_ObservSex"].ToString(),
                            HC_ObseAuto = Lectura_Agenda["HC_ObseAuto"].ToString(),
                            HC_Suicida = Lectura_Agenda["HC_Suicida"].ToString(),
                            HC_LabEst = Lectura_Agenda["HC_LabEst"].ToString(),
                            HC_ApGen_2 = Lectura_Agenda["HC_ApGen_2"].ToString(),
                            HC_ApGen_1 = Lectura_Agenda["HC_ApGen_1"].ToString(),
                            HC_ApGen_3 = Lectura_Agenda["HC_ApGen_3"].ToString(),
                            HC_Cons = Lectura_Agenda["HC_Cons"].ToString(),
                            HC_Aten_1 = Lectura_Agenda["HC_Aten_1"].ToString(),
                            HC_Aten_2 = Lectura_Agenda["HC_Aten_2"].ToString(),
                            HC_TiLab = Lectura_Agenda["HC_TiLab"].ToString(),
                            HC_ObLav = Lectura_Agenda["HC_ObLav"].ToString(),
                            HC_ObCons = Lectura_Agenda["HC_ObCons"].ToString(),
                            HC_ObAten = Lectura_Agenda["HC_ObAten"].ToString(),
                            HC_Sue_1 = Lectura_Agenda["HC_Sue_1"].ToString(),
                            HC_Sue_2 = Lectura_Agenda["HC_Sue_2"].ToString(),
                            HC_Sue_4 = Lectura_Agenda["HC_Sue_4"].ToString(),
                            HC_Sue_3 = Lectura_Agenda["HC_Sue_3"].ToString(),
                            HC_Orien_1 = Lectura_Agenda["HC_Orien_1"].ToString(),
                            HC_ObOrien = Lectura_Agenda["HC_ObOrien"].ToString(),
                            HC_ObSue = Lectura_Agenda["HC_ObSue"].ToString(),
                            HC_ObSue_2 = Lectura_Agenda["HC_ObSue_2"].ToString(),
                            HC_Riesgo = Lectura_Agenda["HC_Riesgo"].ToString(),
                            HC_FacPro = Lectura_Agenda["HC_FacPro"].ToString(),
                            HC_ImpDiag = Lectura_Agenda["HC_ImpDiag"].ToString(),
                            HC_Pronostico = Lectura_Agenda["HC_Pronostico"].ToString(),
                            HC_Paccion = Lectura_Agenda["HC_Paccion"].ToString(),
                            HC_Recomienda = Lectura_Agenda["HC_Recomienda"].ToString(),
                            HC_CIE10 = Lectura_Agenda["HC_CIE10"].ToString(),
                            HC_CIE10_2 = Lectura_Agenda["HC_CIE10_2"].ToString(),
                            HC_CIE10_3 = Lectura_Agenda["HC_CIE10_3"].ToString(),
                            HC_Ocupacion = Lectura_Agenda["HC_Ocupacion"].ToString(),
                            HC_Fecha = Convert.ToDateTime(Lectura_Agenda["HC_Fecha"])
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
        bool IPsicologia.saveHistory(CXN_HCPSI H)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HCPSI (HC_Pac, " + //param1
                                                             "HC_Cant, " + //param2
                                                             "HC_Prof, " + //param3
                                                             "HC_Ase, " + //param4
                                                             "HC_Cia, " + //param5
                                                             "HC_Fecha, " + //param6
                                                             "HC_Edad, " + //param7
                                                             "HC_Pac_Id, " + //param8
                                                             "HC_Adm, " + //param9
                                                             "HC_Ocupacion, " + //param10
                                                             "HC_CIE10, " + //param11
                                                             "HC_CIE10_2, " + //param12
                                                             "HC_CIE10_3, " + //param13
                                                             "HC_Patologico, " + //param14
                                                             "HC_Quirurgico, " + //param15
                                                             "HC_Farmacologico, " + //param15
                                                             "HC_Fracturas, " + //param15
                                                             "HC_Tox_Ale, " + //param15
                                                             "HC_Familiares, " + //param15
                                                             "HC_Psicologicos, " + //param15
                                                             "HC_Otros, " + //param15
                                                             "HC_Esc_Dol, " + //param15
                                                             "HC_Caracteristica, " + //param15
                                                             "HC_TEvolucion, " + //param15
                                                             "HC_Fisiatra, " + //param15
                                                             "HC_Psiquiatra, " + //param15
                                                             "HC_Remite, " + //param15
                                                             "HC_RedApSoc, " + //param15
                                                             "HC_FSoporte, " + //param15
                                                             "HC_ObSoporte, " + //param15
                                                             "HC_OtroSoporte, " + //param15
                                                             "HC_TFamilia, " + //param15
                                                             "HC_TRelacion, " + //param15
                                                             "HC_Sust, " + //param15
                                                             "HC_Compo, " + //param15
                                                             "HC_Estado, " + //param15
                                                             "HC_EstadoOtro, " + //param15
                                                             "HC_TRelacion_2, " + //param15
                                                             "HC_TiRelacion, " + //param15
                                                             "HC_Sust_2, " + //param15
                                                             "HC_EstSex, " + //param15
                                                             "HC_Satis, " + //param15
                                                             "HC_DolMol, " + //param15
                                                             "HC_ObservSex, " + //param15
                                                             "HC_AutoEsq, " + //param15
                                                             "HC_ObseAuto, " + //param15
                                                             "HC_Suicida, " + //AQUI TERMINA PRIMERA 47
                                                             "HC_LabEst, " + //param15
                                                             "HC_TiLab, " + //param15
                                                             "HC_ObLav, " + //param15
                                                             "HC_ApGen_1, " + //param15
                                                             "HC_ApGen_2, " + //param15
                                                             "HC_ApGen_3, " + //param15
                                                             "HC_Cons, " + //param15
                                                             "HC_ObCons, " + //param15
                                                             "HC_Aten_1, " + //param15
                                                             "HC_Aten_2, " + //param15
                                                             "HC_ObAten, " + //param15
                                                             "HC_Orien_1, " + //param15
                                                             "HC_ObOrien, " + //param15
                                                             "HC_Sue_1, " + //param15
                                                             "HC_Sue_2, " + //param15
                                                             "HC_Sue_3, " + //param15
                                                             "HC_Sue_4, " + //param15
                                                             "HC_ObSue, " + //param15
                                                             "HC_ObSue_2, " + //param15
                                                             "HC_Riesgo, " + //param15
                                                             "HC_FacPro, " + //param15
                                                             "HC_ImpDiag, " + //param15
                                                             "HC_Pronostico, " + //param15
                                                             "HC_Paccion, " + //param15
                                                             "HC_Recomienda) " + //Termina 2 de 25
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
                                                             "@param15, " + // Hor_Observacion
                                                             "@param16, " + // Hor_Pac_Id
                                                             "@param17, " + // Hor_Pac_Bod
                                                             "@param18, " + // Hor_Pac_Tipo_Serv
                                                             "@param19, " + // Hor_Pac_Cia
                                                             "@param20, " + // Hor_Pac_Ase
                                                             "@param21, " + // Hor_Pac_Cup
                                                             "@param22, " + // Hor_Pac_UsrGraba
                                                             "@param23, " + // Hor_Imp_Age
                                                             "@param24, " + // Hor_Pac_Fecha
                                                             "@param25, " + // Hor_Pac_Fecha_Cita
                                                             "@param26, " + // Hor_Pac_Hora
                                                             "@param27, " + // Hor_Pac_Id_Hora
                                                             "@param28, " + // Hor_Pac_Hora_Cita
                                                             "@param29, " + // Hor_Observacion
                                                             "@param30, " + // Hor_Pac_Id
                                                             "@param31, " + // Hor_Pac_Bod
                                                             "@param32, " + // Hor_Pac_Tipo_Serv
                                                             "@param33, " + // Hor_Pac_Cia
                                                             "@param34, " + // Hor_Pac_Ase
                                                             "@param35, " + // Hor_Pac_Cup
                                                             "@param36, " + // Hor_Pac_UsrGraba
                                                             "@param37, " + // Hor_Imp_Age
                                                             "@param38, " + // Hor_Pac_Fecha
                                                             "@param39, " + // Hor_Pac_Fecha_Cita
                                                             "@param40, " + // Hor_Pac_Hora
                                                             "@param41, " + // Hor_Pac_Id_Hora
                                                             "@param42, " + // Hor_Pac_Hora_Cita
                                                             "@param43, " + // Hor_Observacion
                                                             "@param44, " + // Hor_Pac_Id
                                                             "@param45, " + // Hor_Pac_Bod
                                                             "@param46, " + // Hor_Pac_Tipo_Serv
                                                             "@param47, " + // Hor_Pac_Cia
                                                             "@param48, " + // Hor_Pac_Ase
                                                             "@param49, " + // Hor_Pac_Cup
                                                             "@param50, " + // Hor_Pac_UsrGraba
                                                             "@param51, " + // Hor_Imp_Age
                                                             "@param52, " + // Hor_Pac_Fecha
                                                             "@param53, " + // Hor_Pac_Fecha_Cita
                                                             "@param54, " + // Hor_Pac_Hora
                                                             "@param55, " + // Hor_Pac_Id_Hora
                                                             "@param56, " + // Hor_Pac_Hora_Cita
                                                             "@param57, " + // Hor_Observacion
                                                             "@param58, " + // Hor_Observacion
                                                             "@param59, " + // Hor_Observacion
                                                             "@param60, " + // Hor_Observacion
                                                             "@param61, " + // Hor_Observacion
                                                             "@param62, " + // Hor_Observacion
                                                             "@param63, " + // Hor_Observacion
                                                             "@param64, " + // Hor_Observacion
                                                             "@param65, " + // Hor_Observacion
                                                             "@param66, " + // Hor_Observacion
                                                             "@param67, " + // Hor_Observacion
                                                             "@param68, " + // Hor_Observacion
                                                             "@param69, " + // Hor_Observacion
                                                             "@param70, " + // Hor_Observacion
                                                             "@param71, " + // Hor_Observacion
                                                             "@param72)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", H.HC_Pac);
                    cmd.Parameters.AddWithValue("@param2", H.HC_Cant);
                    cmd.Parameters.AddWithValue("@param3", H.HC_Prof);
                    cmd.Parameters.AddWithValue("@param4", H.HC_Ase);
                    cmd.Parameters.AddWithValue("@param5", H.HC_Cia);
                    cmd.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = H.HC_Fecha; // Fecha que graba cita
                    cmd.Parameters.AddWithValue("@param7", H.HC_Edad);
                    cmd.Parameters.AddWithValue("@param8", H.HC_Pac_id);
                    cmd.Parameters.AddWithValue("@param9", H.HC_Adm);
                    cmd.Parameters.AddWithValue("@param10", H.HC_Ocupacion);
                    cmd.Parameters.AddWithValue("@param11", H.HC_CIE10);
                    cmd.Parameters.AddWithValue("@param12", H.HC_CIE10_2);
                    cmd.Parameters.AddWithValue("@param13", H.HC_CIE10_3);

                    cmd.Parameters.AddWithValue("@param14", H.HC_Patologico);
                    cmd.Parameters.AddWithValue("@param15", H.HC_Quirurgico);
                    cmd.Parameters.AddWithValue("@param16", H.HC_Farmacologico);
                    cmd.Parameters.AddWithValue("@param17", H.HC_Fracturas);
                    cmd.Parameters.AddWithValue("@param18", H.HC_Tox_Ale);
                    cmd.Parameters.AddWithValue("@param19", H.HC_Familiares);
                    cmd.Parameters.AddWithValue("@param20", H.HC_Psicologicos);
                    cmd.Parameters.AddWithValue("@param21", H.HC_Otros);
                    cmd.Parameters.AddWithValue("@param22", H.HC_Esc_Dol);
                    cmd.Parameters.AddWithValue("@param23", H.HC_Caracteristica);
                    cmd.Parameters.AddWithValue("@param24", H.HC_TEvolucion);
                    cmd.Parameters.AddWithValue("@param25", H.HC_Fisiatra);
                    cmd.Parameters.AddWithValue("@param26", H.HC_Psiquiatra);
                    cmd.Parameters.AddWithValue("@param27", H.HC_Remite);
                    cmd.Parameters.AddWithValue("@param28", H.HC_RedApSoc);
                    cmd.Parameters.AddWithValue("@param29", H.HC_FSoporte);
                    cmd.Parameters.AddWithValue("@param30", H.HC_ObSoporte);
                    cmd.Parameters.AddWithValue("@param31", H.HC_OtroSoporte);
                    cmd.Parameters.AddWithValue("@param32", H.HC_TFamilia);
                    cmd.Parameters.AddWithValue("@param33", H.HC_TRelacion);
                    cmd.Parameters.AddWithValue("@param34", H.HC_Sust);
                    cmd.Parameters.AddWithValue("@param35", H.HC_Compo);
                    cmd.Parameters.AddWithValue("@param36", H.HC_Estado);
                    cmd.Parameters.AddWithValue("@param37", H.HC_EstadoOtro);
                    cmd.Parameters.AddWithValue("@param38", H.HC_TRelacion_2);
                    cmd.Parameters.AddWithValue("@param39", H.HC_TiRelacion);
                    cmd.Parameters.AddWithValue("@param40", H.HC_Sust_2);
                    cmd.Parameters.AddWithValue("@param41", H.HC_EstSex);
                    cmd.Parameters.AddWithValue("@param42", H.HC_Satis);
                    cmd.Parameters.AddWithValue("@param43", H.HC_DolMol);
                    cmd.Parameters.AddWithValue("@param44", H.HC_ObservSex);
                    cmd.Parameters.AddWithValue("@param45", H.HC_AutoEsq);
                    cmd.Parameters.AddWithValue("@param46", H.HC_ObseAuto);
                    cmd.Parameters.AddWithValue("@param47", H.HC_Suicida);

                    cmd.Parameters.AddWithValue("@param48", H.HC_LabEst);
                    cmd.Parameters.AddWithValue("@param49", H.HC_TiLab);
                    cmd.Parameters.AddWithValue("@param50", H.HC_ObLav);
                    cmd.Parameters.AddWithValue("@param51", H.HC_ApGen_1);
                    cmd.Parameters.AddWithValue("@param52", H.HC_ApGen_2);
                    cmd.Parameters.AddWithValue("@param53", H.HC_ApGen_3);
                    cmd.Parameters.AddWithValue("@param54", H.HC_Cons);
                    cmd.Parameters.AddWithValue("@param55", H.HC_ObCons);
                    cmd.Parameters.AddWithValue("@param56", H.HC_Aten_1);
                    cmd.Parameters.AddWithValue("@param57", H.HC_Aten_2);
                    cmd.Parameters.AddWithValue("@param58", H.HC_ObAten);
                    cmd.Parameters.AddWithValue("@param59", H.HC_Orien_1);
                    cmd.Parameters.AddWithValue("@param60", H.HC_ObOrien);
                    cmd.Parameters.AddWithValue("@param61", H.HC_Sue_1);
                    cmd.Parameters.AddWithValue("@param62", H.HC_Sue_2);
                    cmd.Parameters.AddWithValue("@param63", H.HC_Sue_3);
                    cmd.Parameters.AddWithValue("@param64", H.HC_Sue_4);
                    cmd.Parameters.AddWithValue("@param65", H.HC_ObSue);
                    cmd.Parameters.AddWithValue("@param66", H.HC_ObSue_2);
                    cmd.Parameters.AddWithValue("@param67", H.HC_Riesgo);
                    cmd.Parameters.AddWithValue("@param68", H.HC_FacPro);
                    cmd.Parameters.AddWithValue("@param69", H.HC_ImpDiag);
                    cmd.Parameters.AddWithValue("@param70", H.HC_Pronostico);
                    cmd.Parameters.AddWithValue("@param71", H.HC_Paccion);
                    cmd.Parameters.AddWithValue("@param72", H.HC_Recomienda);
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
    }
}
