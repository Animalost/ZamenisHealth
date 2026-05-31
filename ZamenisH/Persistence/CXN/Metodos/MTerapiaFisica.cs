using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;

namespace Persistence.CXN.Metodos
{
    public class MTerapiaFisica : ITerapiaFisica
    {
        bool ITerapiaFisica.insertHistoria(CXN_HCTF H)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HCTF (HC_Fecha , " + //param1
                                                          "HC_Pac , " + //param2
                                                          "HC_Cant , " + //param3
                                                          "HC_Prof , " + //param4
                                                          "HC_Ase , " + //param5
                                                          "HC_Cia , " + //param6
                                                          "HC_PacId , " + //param7
                                                          "HC_Adm , " + //param8
                                                          "HC_FechaNto , " + //param9
                                                          "HC_Edad , " + //param10
                                                          "HC_EstadoC , " + //param11
                                                          "HC_Hijos , " + //param12
                                                          "HC_Estudios , " + //param13
                                                          "HC_CIE10 , " + //param5
                                                          "HC_Dx , " + //param6
                                                          "HC_TiempoE , " + //param7
                                                          "HC_TratamientosP , " + //param8
                                                          "HC_ExaDiag , " + //param9
                                                          "HC_Ocupacion , " + //param10
                                                          "HC_MotivoCons , " + //param11
                                                          "HC_AntFam , " + //param12
                                                          "HC_AntPat , " + //param13
                                                          "HC_AntNeu , " + //param5
                                                          "HC_SalMen , " + //param6
                                                          "HC_OsteoMusc , " + //param7
                                                          "HC_Endocrino , " + //param8
                                                          "HC_Quir , " + //param9
                                                          "HC_Resp , " + //param10
                                                          "HC_Derma , " + //param11
                                                          "HC_Farma , " + //param12
                                                          "HC_DolorEIAN , " + //param13
                                                          "HC_EVA , " + //param5
                                                          "HC_FrecDol , " + //param6
                                                          "HC_TipoDol , " + //param7
                                                          "HC_CaracDol , " + //param8
                                                          "Hc_Text60 , " + //param9
                                                          "HC_SensiDol , " + //param10
                                                          "HC_SintAso , " + //param11
                                                          "HC_HabTox , " + //param12
                                                          "HC_ActFis , " + //param13
                                                          "HC_MedExt , " + //param5
                                                          "HC_Cual1 , " + //param6
                                                          "HC_Cual2 , " + //param7
                                                          "HC_CC1 , " + //param8
                                                          "HC_CC2 , " + //param9
                                                          "HC_CC3 , " + //param10
                                                          "HC_CC4 , " + //param11
                                                          "HC_CC5 , " + //param12
                                                          "HC_CC6 , " + //param13
                                                          "HC_CD1 , " + //param5
                                                          "HC_CD2 , " + //param6
                                                          "HC_CD3 , " + //param7
                                                          "HC_CD4 , " + //param8
                                                          "HC_H1 , " + //param9
                                                          "HC_H2 , " + //param10
                                                          "HC_H3 , " + //param11
                                                          "HC_H4 , " + //param12
                                                          "HC_H5 , " + //param13
                                                          "HC_H6 , " + //param5
                                                          "HC_C1 , " + //param6
                                                          "HC_C2 , " + //param7
                                                          "HC_C3 , " + //param8
                                                          "HC_C4 , " + //param9
                                                          "HC_C5 , " + //param10
                                                          "HC_C6 , " + //param11
                                                          "Hc_T1 , " + //param12
                                                          "Hc_T2 , " + //param13
                                                          "Hc_T3 , " + //param5
                                                          "Hc_T4 , " + //param6
                                                          "Hc_T5 , " + //param7
                                                          "Hc_T6 , " + //param8
                                                          "Hc_T7 , " + //param9
                                                          "Hc_T8 , " + //param10
                                                          "Hc_T9 , " + //param11
                                                          "Hc_T10 , " + //param12
                                                          "Hc_T11 , " + //param13
                                                          "Hc_T12 , " + //param5
                                                          "Hc_T13 , " + //param6
                                                          "Hc_T14 , " + //param7
                                                          "HisExtCui , " + //param8
                                                          "HisExtCuD , " + //param9
                                                          "HisEscalenosI , " + //param10
                                                          "HisEscalenosD , " + //param11
                                                          "HisTrapSupI , " + //param12
                                                          "HisTrapSupD , " + //param13
                                                          "HisTrapMedI , " + //param14
                                                          "HisTrapMedD , " + //param8
                                                          "HisTrapInfI , " + //param9
                                                          "HisTrapInfD , " + //param10
                                                          "HisSerAntI , " + //param11
                                                          "HisSerAntD , " + //param12
                                                          "HisPecMayI , " + //param13
                                                          "HisPecMayD , " + //param14
                                                          "HisRomI , " + //param8
                                                          "HisRomD , " + //param9
                                                          "HisAbSupI , " + //param10
                                                          "HisAbSupD , " + //param11
                                                          "HisAbdInfI , " + //param12
                                                          "HisAbdInfD , " + //param13
                                                          "HisOblicuoI , " + //param14
                                                          "HisOblicuoD , " + //param8
                                                          "HisExtDorsalI , " + //param9
                                                          "HisExtDorsalD , " + //param10
                                                          "HisExtLumbarI , " + //param11
                                                          "HisExtLumbarD , " + //param12
                                                          "HisGluMayI , " + //param13
                                                          "HisGluMayD , " + //param14
                                                          "HisGluMedI , " + //param8
                                                          "HisGluMedD , " + //param9
                                                          "HisCuadriI , " + //param10
                                                          "HisCuadriD , " + //param11
                                                          "HisIsquiI , " + //param12
                                                          "HisIsquiD , " + //param13
                                                          "HisGemeloI , " + //param14
                                                          "HisGemeloD , " + //param10
                                                          "HisTibAntI , " + //param11
                                                          "HisTibAntD , " + //param12
                                                          "HisEscaAntI , " + //param13
                                                          "HisEscaAntD , " + //param14
                                                          "HisEscaMedI , " + //param10
                                                          "HisEscaMedD , " + //param11
                                                          "HisEscaPosI , " + //param12
                                                          "HisEscaPosD , " + //param13
                                                          "HisECMI , " + //param14
                                                          "HisECMD , " + //param10
                                                          "HisCualLumI , " + //param11
                                                          "HisCualLumD , " + //param12
                                                          "HisCuadI , " + //param13
                                                          "HisCuadD , " + //param14
                                                          "HisIsquiI2 , " + //param10
                                                          "HisIsqui2D , " + //param11
                                                          "HisTensorI , " + //param12
                                                          "HisTensorD , " + //param13
                                                          "HisGastroI , " + //param14
                                                          "HisGastroD , " + //param10
                                                          "HisTAquilesI , " + //param11
                                                          "HisTAquilesD , " + //param12
                                                          "HisCabeza , " + //param13
                                                          "HisHombros , " + //param14
                                                          "HisBrazos , " + //param15
                                                          "HisCadera , " + //param13
                                                          "HisEscapula , " + //param14
                                                          "HisColumna , " + //param15
                                                          "HisCrestasIliacas , " + //param13
                                                          "HisGluteos , " + //param14
                                                          "HisFosa , " + //param15
                                                          "HisDiagnostico , " + //param13
                                                          "HisPronostico , " + //param14
                                                          "HisAcciones , " + //param15
                                                          "HisConducta , " + //param13
                                                          "Marcha_Mec , " + //param14
                                                          "HisRodilla , " + //param15
                                                          "HisRotula , " + //param13
                                                          "HisTibia , " + //param14
                                                          "HisTobillo , " + //param15
                                                          "HisPie , " + //param13
                                                          "HisCuello , " + //param14
                                                          "HisEspaldaA , " + //param15
                                                          "HisTorax , " + //param13
                                                          "HisAbdomen , " + //param14
                                                          "HisEspaldaB , " + //param15
                                                          "HisPelvis , " + //param13
                                                          "HisRodillas ) " + //param16
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
                                                          "@param58, " + // Hor_Pac_Id
                                                          "@param59, " + // Hor_Pac_Bod
                                                          "@param60, " + // Hor_Pac_Tipo_Serv
                                                          "@param61, " + // Hor_Pac_Cia
                                                          "@param62, " + // Hor_Pac_Ase
                                                          "@param63, " + // Hor_Pac_Cup
                                                          "@param64, " + // Hor_Pac_UsrGraba
                                                          "@param65, " + // Hor_Imp_Age
                                                          "@param66, " + // Hor_Pac_Fecha
                                                          "@param67, " + // Hor_Pac_Fecha_Cita
                                                          "@param68, " + // Hor_Pac_Hora
                                                          "@param69, " + // Hor_Pac_Id_Hora
                                                          "@param70, " + // Hor_Pac_Hora_Cita
                                                          "@param71, " + // Hor_Observacion
                                                          "@param72, " + // Hor_Pac_Id
                                                          "@param73, " + // Hor_Pac_Bod
                                                          "@param74, " + // Hor_Pac_Tipo_Serv
                                                          "@param75, " + // Hor_Pac_Cia
                                                          "@param76, " + // Hor_Pac_Ase
                                                          "@param77, " + // Hor_Pac_Cup
                                                          "@param78, " + // Hor_Pac_UsrGraba
                                                          "@param79, " + // Hor_Imp_Age
                                                          "@param80, " + // Hor_Pac_Fecha
                                                          "@param81, " + // Hor_Pac_Fecha_Cita
                                                          "@param82, " + // Hor_Pac_Hora
                                                          "@param83, " + // Hor_Pac_Id_Hora
                                                          "@param84, " + // Hor_Pac_Hora_Cita
                                                          "@param85, " + // Hor_Observacion
                                                          "@param86, " + // Hor_Pac_Id
                                                          "@param87, " + // Hor_Pac_Bod
                                                          "@param88, " + // Hor_Pac_Tipo_Serv
                                                          "@param89, " + // Hor_Pac_Cia
                                                          "@param90, " + // Hor_Pac_Ase
                                                          "@param91, " + // Hor_Pac_Cup
                                                          "@param92, " + // Hor_Pac_UsrGraba
                                                          "@param93, " + // Hor_Imp_Age
                                                          "@param94, " + // Hor_Pac_Fecha
                                                          "@param95, " + // Hor_Pac_Fecha_Cita
                                                          "@param96, " + // Hor_Pac_Hora
                                                          "@param97, " + // Hor_Pac_Id_Hora
                                                          "@param98, " + // Hor_Pac_Hora_Cita
                                                          "@param99, " + // Hor_Observacion
                                                          "@param100, " + // Hor_Pac_Id
                                                          "@param101, " + // Hor_Pac_Bod
                                                          "@param102, " + // Hor_Pac_Tipo_Serv
                                                          "@param103, " + // Hor_Pac_Cia
                                                          "@param104, " + // Hor_Pac_Ase
                                                          "@param105, " + // Hor_Pac_Cup
                                                          "@param106, " + // Hor_Pac_UsrGraba
                                                          "@param107, " + // Hor_Imp_Age
                                                          "@param108, " + // Hor_Pac_Fecha
                                                          "@param109, " + // Hor_Pac_Fecha_Cita
                                                          "@param110, " + // Hor_Pac_Hora
                                                          "@param111, " + // Hor_Pac_Id_Hora
                                                          "@param112, " + // Hor_Pac_Hora_Cita
                                                          "@param113, " + // Hor_Observacion
                                                          "@param114, " + // Hor_Pac_Id
                                                          "@param115, " + // Hor_Pac_Bod
                                                          "@param116, " + // Hor_Pac_Tipo_Serv
                                                          "@param117, " + // Hor_Pac_Cia
                                                          "@param118, " + // Hor_Pac_Ase
                                                          "@param119, " + // Hor_Pac_Cup
                                                          "@param120, " + // Hor_Pac_UsrGraba
                                                          "@param121, " + // Hor_Imp_Age
                                                          "@param122, " + // Hor_Pac_Fecha
                                                          "@param123, " + // Hor_Pac_Fecha_Cita
                                                          "@param124, " + // Hor_Pac_Hora
                                                          "@param125, " + // Hor_Pac_Id_Hora
                                                          "@param126, " + // Hor_Pac_Hora_Cita
                                                          "@param127, " + // Hor_Pac_Id
                                                          "@param128, " + // Hor_Pac_Bod
                                                          "@param129, " + // Hor_Pac_Tipo_Serv
                                                          "@param130, " + // Hor_Pac_Cia
                                                          "@param131, " + // Hor_Pac_Ase
                                                          "@param132, " + // Hor_Pac_Cup
                                                          "@param133, " + // Hor_Pac_UsrGraba
                                                          "@param134, " + // Hor_Imp_Age
                                                          "@param135, " + // Hor_Pac_Fecha
                                                          "@param136, " + // Hor_Pac_Fecha_Cita
                                                          "@param137, " + // Hor_Pac_Hora
                                                          "@param138, " + // Hor_Pac_Id_Hora
                                                          "@param139, " + // Hor_Pac_Hora_Cita
                                                          "@param140, " + // Hor_Pac_Id
                                                          "@param141, " + // Hor_Pac_Bod
                                                          "@param142, " + // Hor_Pac_Tipo_Serv
                                                          "@param143, " + // Hor_Pac_Cia
                                                          "@param144, " + // Hor_Pac_Ase
                                                          "@param145, " + // Hor_Pac_Cup
                                                          "@param146, " + // Hor_Pac_UsrGraba
                                                          "@param147, " + // Hor_Imp_Age
                                                          "@param148, " + // Hor_Pac_Fecha
                                                          "@param149, " + // Hor_Pac_Fecha_Cita
                                                          "@param150, " + // Hor_Pac_Hora
                                                          "@param151, " + // Hor_Pac_Id_Hora
                                                          "@param152, " + // Hor_Pac_Hora_Cita
                                                          "@param153, " + // Hor_Observacion
                                                          "@param154, " + // Hor_Pac_Id
                                                          "@param155, " + // Hor_Pac_Bod
                                                          "@param156, " + // Hor_Pac_Tipo_Serv
                                                          "@param157, " + // Hor_Pac_Cia
                                                          "@param158, " + // Hor_Pac_Ase
                                                          "@param159, " + // Hor_Pac_Cup
                                                          "@param160, " + // Hor_Pac_UsrGraba
                                                          "@param161, " + // Hor_Imp_Age
                                                          "@param162, " +
                                                          "@param163)", con); // 163

                    cmd.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = H.HC_Fecha;  // Hora que graba cita
                    cmd.Parameters.AddWithValue("@param2", H.HC_Pac);
                    cmd.Parameters.AddWithValue("@param3", H.HC_Cant);
                    cmd.Parameters.AddWithValue("@param4", H.HC_Prof);
                    cmd.Parameters.AddWithValue("@param5", H.HC_Ase);
                    cmd.Parameters.AddWithValue("@param6", H.HC_Cia);
                    cmd.Parameters.AddWithValue("@param7", H.HC_PacId);
                    cmd.Parameters.AddWithValue("@param8", H.HC_Adm);
                    cmd.Parameters.Add(new SqlParameter("@param9", SqlDbType.DateTime)).Value = H.HC_FechaNto;
                    cmd.Parameters.AddWithValue("@param10", H.HC_Edad);
                    cmd.Parameters.AddWithValue("@param11", H.HC_EstadoC);
                    cmd.Parameters.AddWithValue("@param12", H.HC_Hijos);
                    cmd.Parameters.AddWithValue("@param13", H.HC_Estudios);
                    cmd.Parameters.AddWithValue("@param14", H.HC_CIE10);
                    cmd.Parameters.AddWithValue("@param15", H.HC_Dx);
                    cmd.Parameters.AddWithValue("@param16", H.HC_TiempoE);
                    cmd.Parameters.AddWithValue("@param17", H.HC_TratamientosP);
                    cmd.Parameters.AddWithValue("@param18", H.HC_ExaDiag);
                    cmd.Parameters.AddWithValue("@param19", H.HC_Ocupacion);
                    cmd.Parameters.AddWithValue("@param20", H.HC_MotivoCons);
                    cmd.Parameters.AddWithValue("@param21", H.HC_AntFam);
                    cmd.Parameters.AddWithValue("@param22", H.HC_AntPat);
                    cmd.Parameters.AddWithValue("@param23", H.HC_AntNeu);
                    cmd.Parameters.AddWithValue("@param24", H.HC_SalMen);
                    cmd.Parameters.AddWithValue("@param25", H.HC_OsteoMusc);
                    cmd.Parameters.AddWithValue("@param26", H.HC_Endocrino);
                    cmd.Parameters.AddWithValue("@param27", H.HC_Quir);
                    cmd.Parameters.AddWithValue("@param28", H.HC_Resp);
                    cmd.Parameters.AddWithValue("@param29", H.HC_Derma);
                    cmd.Parameters.AddWithValue("@param30", H.HC_Farma);
                    cmd.Parameters.AddWithValue("@param31", H.HC_DolorEIAN);
                    cmd.Parameters.AddWithValue("@param32", H.HC_Eva);
                    cmd.Parameters.AddWithValue("@param33", H.HC_FrecDol);
                    cmd.Parameters.AddWithValue("@param34", H.HC_TipoDol);
                    cmd.Parameters.AddWithValue("@param35", H.HC_CaracDol);
                    cmd.Parameters.AddWithValue("@param36", H.HC_Text60);
                    cmd.Parameters.AddWithValue("@param37", H.HC_SensiDol);
                    cmd.Parameters.AddWithValue("@param38", H.HC_SintAso);
                    cmd.Parameters.AddWithValue("@param39", H.HC_HabTox);
                    cmd.Parameters.AddWithValue("@param40", H.HC_ActFis);
                    cmd.Parameters.AddWithValue("@param41", H.HC_MedExt);
                    cmd.Parameters.AddWithValue("@param42", H.HC_Cual1);
                    cmd.Parameters.AddWithValue("@param43", H.HC_Cual2);
                    cmd.Parameters.AddWithValue("@param44", H.HC_CC1);
                    cmd.Parameters.AddWithValue("@param45", H.HC_CC2);
                    cmd.Parameters.AddWithValue("@param46", H.HC_CC3);
                    cmd.Parameters.AddWithValue("@param47", H.HC_CC4);
                    cmd.Parameters.AddWithValue("@param48", H.HC_CC5);
                    cmd.Parameters.AddWithValue("@param49", H.HC_CC6);
                    cmd.Parameters.AddWithValue("@param50", H.HC_CD1);
                    cmd.Parameters.AddWithValue("@param51", H.HC_CD2);
                    cmd.Parameters.AddWithValue("@param52", H.HC_CD3);
                    cmd.Parameters.AddWithValue("@param53", H.HC_CD4);
                    cmd.Parameters.AddWithValue("@param54", H.HC_H1);
                    cmd.Parameters.AddWithValue("@param55", H.HC_H2);
                    cmd.Parameters.AddWithValue("@param56", H.HC_H3);
                    cmd.Parameters.AddWithValue("@param57", H.HC_H4);
                    cmd.Parameters.AddWithValue("@param58", H.HC_H5);
                    cmd.Parameters.AddWithValue("@param59", H.HC_H6);
                    cmd.Parameters.AddWithValue("@param60", H.HC_C1);
                    cmd.Parameters.AddWithValue("@param61", H.HC_C2);
                    cmd.Parameters.AddWithValue("@param62", H.HC_C3);
                    cmd.Parameters.AddWithValue("@param63", H.HC_C4);
                    cmd.Parameters.AddWithValue("@param64", H.HC_C5);
                    cmd.Parameters.AddWithValue("@param65", H.HC_C6);
                    cmd.Parameters.AddWithValue("@param66", H.HC_T1);
                    cmd.Parameters.AddWithValue("@param67", H.HC_T2);
                    cmd.Parameters.AddWithValue("@param68", H.HC_T3);
                    cmd.Parameters.AddWithValue("@param69", H.HC_T4);
                    cmd.Parameters.AddWithValue("@param70", H.HC_T5);
                    cmd.Parameters.AddWithValue("@param71", H.HC_T6);
                    cmd.Parameters.AddWithValue("@param72", H.HC_T7);
                    cmd.Parameters.AddWithValue("@param73", H.HC_T8);
                    cmd.Parameters.AddWithValue("@param74", H.HC_T9);
                    cmd.Parameters.AddWithValue("@param75", H.HC_T10);
                    cmd.Parameters.AddWithValue("@param76", H.HC_T11);
                    cmd.Parameters.AddWithValue("@param77", H.HC_T12);
                    cmd.Parameters.AddWithValue("@param78", H.HC_T13);
                    cmd.Parameters.AddWithValue("@param79", H.HC_T14);
                    cmd.Parameters.AddWithValue("@param80", H.HisExtCuI);
                    cmd.Parameters.AddWithValue("@param81", H.HisExtCuD);
                    cmd.Parameters.AddWithValue("@param82", H.HisEscalenosI);
                    cmd.Parameters.AddWithValue("@param83", H.HisEscalenosD);
                    cmd.Parameters.AddWithValue("@param84", H.HisTrapSupI);
                    cmd.Parameters.AddWithValue("@param85", H.HisTrapSupD);
                    cmd.Parameters.AddWithValue("@param86", H.HisTrapMedI);
                    cmd.Parameters.AddWithValue("@param87", H.HisTrapMedD);
                    cmd.Parameters.AddWithValue("@param88", H.HisTrapInfI);
                    cmd.Parameters.AddWithValue("@param89", H.HisTrapInfD);
                    cmd.Parameters.AddWithValue("@param90", H.HisSerAntI);
                    cmd.Parameters.AddWithValue("@param91", H.HisSerAntD);
                    cmd.Parameters.AddWithValue("@param92", H.HisPecMayI);
                    cmd.Parameters.AddWithValue("@param93", H.HisPecMayD);
                    cmd.Parameters.AddWithValue("@param94", H.HisRomI);
                    cmd.Parameters.AddWithValue("@param95", H.HisRomD);
                    cmd.Parameters.AddWithValue("@param96", H.HisAbSupI);
                    cmd.Parameters.AddWithValue("@param97", H.HisAbSupD);
                    cmd.Parameters.AddWithValue("@param98", H.HisAbdInfI);
                    cmd.Parameters.AddWithValue("@param99", H.HisAbdInfD);
                    cmd.Parameters.AddWithValue("@param100", H.HisOblicuoI);
                    cmd.Parameters.AddWithValue("@param101", H.HisOblicuoD);
                    cmd.Parameters.AddWithValue("@param102", H.HisExtDorsalI);
                    cmd.Parameters.AddWithValue("@param103", H.HisExtDorsalD);
                    cmd.Parameters.AddWithValue("@param104", H.HisExtLumbarI);
                    cmd.Parameters.AddWithValue("@param105", H.HisExtLumbarD);
                    cmd.Parameters.AddWithValue("@param106", H.HisGluMayI);
                    cmd.Parameters.AddWithValue("@param107", H.HisGluMayD);
                    cmd.Parameters.AddWithValue("@param108", H.HisGluMedI);
                    cmd.Parameters.AddWithValue("@param109", H.HisGluMedD);
                    cmd.Parameters.AddWithValue("@param110", H.HisCuadriI);
                    cmd.Parameters.AddWithValue("@param111", H.HisCuadriD);
                    cmd.Parameters.AddWithValue("@param112", H.HisIsquiI);
                    cmd.Parameters.AddWithValue("@param113", H.HisIsquiD);
                    cmd.Parameters.AddWithValue("@param114", H.HisGemeloI);
                    cmd.Parameters.AddWithValue("@param115", H.HisGemeloD);
                    cmd.Parameters.AddWithValue("@param116", H.HisTibAntI);
                    cmd.Parameters.AddWithValue("@param117", H.HisTibAntD);
                    cmd.Parameters.AddWithValue("@param118", H.HisEscaAntI);
                    cmd.Parameters.AddWithValue("@param119", H.HisEscaAntD);
                    cmd.Parameters.AddWithValue("@param120", H.HisEscaMedI);
                    cmd.Parameters.AddWithValue("@param121", H.HisEscaMedD);
                    cmd.Parameters.AddWithValue("@param122", H.HisEscaPosI);
                    cmd.Parameters.AddWithValue("@param123", H.HisEscaPosD);
                    cmd.Parameters.AddWithValue("@param124", H.HisECMI);
                    cmd.Parameters.AddWithValue("@param125", H.HisECMD);
                    cmd.Parameters.AddWithValue("@param126", H.HisCualLumI);
                    cmd.Parameters.AddWithValue("@param127", H.HisCualLumD);
                    cmd.Parameters.AddWithValue("@param128", H.HisCuadI);
                    cmd.Parameters.AddWithValue("@param129", H.HisCuadD);
                    cmd.Parameters.AddWithValue("@param130", H.HisIsquiI2);
                    cmd.Parameters.AddWithValue("@param131", H.HisIsqui2D);
                    cmd.Parameters.AddWithValue("@param132", H.HisTensorI);
                    cmd.Parameters.AddWithValue("@param133", H.HisTensorD);
                    cmd.Parameters.AddWithValue("@param134", H.HisGastroI);
                    cmd.Parameters.AddWithValue("@param135", H.HisGastroD);
                    cmd.Parameters.AddWithValue("@param136", H.HisTAquilesI);
                    cmd.Parameters.AddWithValue("@param137", H.HisTAquilesD);
                    cmd.Parameters.AddWithValue("@param138", H.HisCabeza);
                    cmd.Parameters.AddWithValue("@param139", H.HisHombros);
                    cmd.Parameters.AddWithValue("@param140", H.HisBrazos);
                    cmd.Parameters.AddWithValue("@param141", H.HisCadera);
                    cmd.Parameters.AddWithValue("@param142", H.HisEscapula);
                    cmd.Parameters.AddWithValue("@param143", H.HisColumna);
                    cmd.Parameters.AddWithValue("@param144", H.HisCrestasIliacas);
                    cmd.Parameters.AddWithValue("@param145", H.HisGluteos);
                    cmd.Parameters.AddWithValue("@param146", H.HisFosa);
                    cmd.Parameters.AddWithValue("@param147", H.HisDiagnostico);
                    cmd.Parameters.AddWithValue("@param148", H.HisPronostico);
                    cmd.Parameters.AddWithValue("@param149", H.HisAcciones);
                    cmd.Parameters.AddWithValue("@param150", H.HisConducta);
                    cmd.Parameters.AddWithValue("@param151", H.Marcha_Mec);
                    cmd.Parameters.AddWithValue("@param152", H.HisRodilla);
                    cmd.Parameters.AddWithValue("@param153", H.HisRotula);
                    cmd.Parameters.AddWithValue("@param154", H.HisTibia);
                    cmd.Parameters.AddWithValue("@param155", H.HisTobillo);
                    cmd.Parameters.AddWithValue("@param156", H.HisPie);
                    cmd.Parameters.AddWithValue("@param157", H.HisCuello);
                    cmd.Parameters.AddWithValue("@param158", H.HisEspaldaA);
                    cmd.Parameters.AddWithValue("@param159", H.HisTorax);
                    cmd.Parameters.AddWithValue("@param160", H.HisAbdomen);
                    cmd.Parameters.AddWithValue("@param161", H.HisEspaldaB);
                    cmd.Parameters.AddWithValue("@param162", H.HisPelvis);
                    cmd.Parameters.AddWithValue("@param163", H.HisRodillas);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        CXN_HCTF ITerapiaFisica.getLastHistory(int Paciente)
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

                    String CONV = "SELECT * FROM CXN_HCTF WHERE HC_PacId = '" + Paciente + "' ORDER BY HC_Fecha DESC";
                    SqlCommand Com_CONV = new SqlCommand(CONV, con);
                    SqlDataReader Lec_CONV = (Com_CONV.ExecuteReader());
                    if (Lec_CONV.Read() == true)
                    {
                        CXN_HCTF H = new CXN_HCTF
                        {
                            HC_Fecha = Convert.ToDateTime(Lec_CONV["HC_Fecha"]),
                            HC_EstadoC = Lec_CONV["HC_EstadoC"].ToString(),
                            HC_Hijos = Lec_CONV["HC_Hijos"].ToString(),
                            HC_Ocupacion = Lec_CONV["HC_Ocupacion"].ToString(),
                            HC_Estudios = Lec_CONV["HC_Estudios"].ToString(),
                            HC_CIE10 = Lec_CONV["HC_CIE10"].ToString(),
                            HC_Dx = Lec_CONV["HC_Dx"].ToString(),
                            HC_TiempoE = Lec_CONV["HC_TiempoE"].ToString(),
                            HC_TratamientosP = Lec_CONV["HC_TratamientosP"].ToString(),
                            HC_ExaDiag = Lec_CONV["HC_ExaDiag"].ToString(),
                            HC_AntFam = Lec_CONV["HC_AntFam"].ToString(),
                            HC_AntPat = Lec_CONV["HC_AntPat"].ToString(),
                            HC_AntNeu = Lec_CONV["HC_AntNeu"].ToString(),
                            HC_SalMen = Lec_CONV["HC_SalMen"].ToString(),
                            HC_OsteoMusc = Lec_CONV["HC_OsteoMusc"].ToString(),
                            HC_Endocrino = Lec_CONV["HC_Endocrino"].ToString(),
                            HC_Quir = Lec_CONV["HC_Quir"].ToString(),
                            HC_Resp = Lec_CONV["HC_Resp"].ToString(),
                            HC_Derma = Lec_CONV["HC_Derma"].ToString(),
                            HC_Farma = Lec_CONV["HC_Farma"].ToString(),
                            HC_DolorEIAN = Lec_CONV["HC_DolorEIAN"].ToString(),
                            HC_Eva = Lec_CONV["HC_EVA"].ToString(),
                            HC_FrecDol = Lec_CONV["HC_FrecDol"].ToString(),
                            HC_TipoDol = Lec_CONV["HC_TipoDol"].ToString(),
                            HC_Cual1 = Lec_CONV["HC_Cual1"].ToString(),
                            HC_Cual2 = Lec_CONV["HC_Cual2"].ToString(),
                            HC_CC1 = Lec_CONV["HC_CC1"].ToString(),
                            HC_CC2 = Lec_CONV["HC_CC2"].ToString(),
                            HC_CC3 = Lec_CONV["HC_CC3"].ToString(),
                            HC_CC4 = Lec_CONV["HC_CC4"].ToString(),
                            HC_CC5 = Lec_CONV["HC_CC5"].ToString(),
                            HC_CC6 = Lec_CONV["HC_CC6"].ToString(),
                            HC_CD1 = Lec_CONV["HC_CD1"].ToString(),
                            HC_CD2 = Lec_CONV["HC_CD2"].ToString(),
                            HC_CD3 = Lec_CONV["HC_CD3"].ToString(),
                            HC_CD4 = Lec_CONV["HC_CD4"].ToString(),
                            HC_H1 = Lec_CONV["HC_H1"].ToString(),
                            HC_H2 = Lec_CONV["HC_H2"].ToString(),
                            HC_H3 = Lec_CONV["HC_H3"].ToString(),
                            HC_H4 = Lec_CONV["HC_H4"].ToString(),
                            HC_H5 = Lec_CONV["HC_H5"].ToString(),
                            HC_H6 = Lec_CONV["HC_H6"].ToString(),
                            HC_C1 = Lec_CONV["HC_C1"].ToString(),
                            HC_C2 = Lec_CONV["HC_C2"].ToString(),
                            HC_C3 = Lec_CONV["HC_C3"].ToString(),
                            HC_C4 = Lec_CONV["HC_C4"].ToString(),
                            HC_C5 = Lec_CONV["HC_C5"].ToString(),
                            HC_C6 = Lec_CONV["HC_C6"].ToString(),
                            HC_T1 = Lec_CONV["Hc_T1"].ToString(),
                            HC_T2 = Lec_CONV["Hc_T2"].ToString(),
                            HC_T3 = Lec_CONV["Hc_T3"].ToString(),
                            HC_T4 = Lec_CONV["Hc_T4"].ToString(),
                            HC_T5 = Lec_CONV["Hc_T5"].ToString(),
                            HC_T6 = Lec_CONV["Hc_T6"].ToString(),
                            HC_T7 = Lec_CONV["Hc_T7"].ToString(),
                            HC_T8 = Lec_CONV["Hc_T8"].ToString(),
                            HC_T9 = Lec_CONV["Hc_T9"].ToString(),
                            HC_T10 = Lec_CONV["Hc_T10"].ToString(),
                            HC_T11 = Lec_CONV["Hc_T11"].ToString(),
                            HC_T12 = Lec_CONV["Hc_T12"].ToString(),
                            HC_T13 = Lec_CONV["Hc_T13"].ToString(),
                            HC_T14 = Lec_CONV["Hc_T14"].ToString(),
                            HisExtCuI = Lec_CONV["HisExtCui"].ToString(),
                            HisExtCuD = Lec_CONV["HisExtCuD"].ToString(),
                            HisEscalenosI = Lec_CONV["HisEscalenosI"].ToString(),
                            HisEscalenosD = Lec_CONV["HisEscalenosD"].ToString(),
                            HisTrapSupI = Lec_CONV["HisTrapSupI"].ToString(),
                            HisTrapSupD = Lec_CONV["HisTrapSupD"].ToString(),
                            HisTrapMedI = Lec_CONV["HisTrapMedI"].ToString(),
                            HisTrapMedD = Lec_CONV["HisTrapMedD"].ToString(),
                            HisTrapInfI = Lec_CONV["HisTrapInfI"].ToString(),
                            HisTrapInfD = Lec_CONV["HisTrapInfD"].ToString(),
                            HisSerAntI = Lec_CONV["HisSerAntI"].ToString(),
                            HisSerAntD = Lec_CONV["HisSerAntD"].ToString(),
                            HisPecMayI = Lec_CONV["HisPecMayI"].ToString(),
                            HisPecMayD = Lec_CONV["HisPecMayD"].ToString(),
                            HisRomI = Lec_CONV["HisRomI"].ToString(),
                            HisRomD = Lec_CONV["HisRomD"].ToString(),
                            HisAbSupI = Lec_CONV["HisAbSupI"].ToString(),
                            HisAbSupD = Lec_CONV["HisAbSupD"].ToString(),
                            HisAbdInfI = Lec_CONV["HisAbdInfI"].ToString(),
                            HisAbdInfD = Lec_CONV["HisAbdInfD"].ToString(),
                            HisOblicuoI = Lec_CONV["HisOblicuoI"].ToString(),
                            HisOblicuoD = Lec_CONV["HisOblicuoD"].ToString(),
                            HisExtDorsalI = Lec_CONV["HisExtDorsalI"].ToString(),
                            HisExtDorsalD = Lec_CONV["HisExtDorsalD"].ToString(),
                            HisExtLumbarI = Lec_CONV["HisExtLumbarI"].ToString(),
                            HisExtLumbarD = Lec_CONV["HisExtLumbarD"].ToString(),
                            HisGluMayI = Lec_CONV["HisGluMayI"].ToString(),
                            HisGluMayD = Lec_CONV["HisGluMayD"].ToString(),
                            HisGluMedI = Lec_CONV["HisGluMedI"].ToString(),
                            HisGluMedD = Lec_CONV["HisGluMedD"].ToString(),
                            HisCuadriI = Lec_CONV["HisCuadriI"].ToString(),
                            HisCuadriD = Lec_CONV["HisCuadriD"].ToString(),
                            HisIsquiI = Lec_CONV["HisIsquiI"].ToString(),
                            HisIsquiD = Lec_CONV["HisIsquiD"].ToString(),
                            HisGemeloI = Lec_CONV["HisGemeloI"].ToString(),
                            HisGemeloD = Lec_CONV["HisGemeloD"].ToString(),
                            HisTibAntI = Lec_CONV["HisTibAntI"].ToString(),
                            HisTibAntD = Lec_CONV["HisTibAntD"].ToString(),
                            HisEscaAntI = Lec_CONV["HisEscaAntI"].ToString(),
                            HisEscaAntD = Lec_CONV["HisEscaAntD"].ToString(),
                            HisEscaMedI = Lec_CONV["HisEscaMedI"].ToString(),
                            HisEscaMedD = Lec_CONV["HisEscaMedD"].ToString(),
                            HisEscaPosI = Lec_CONV["HisEscaPosI"].ToString(),
                            HisEscaPosD = Lec_CONV["HisEscaPosD"].ToString(),
                            HisECMI = Lec_CONV["HisECMI"].ToString(),
                            HisECMD = Lec_CONV["HisECMD"].ToString(),
                            HisCualLumI = Lec_CONV["HisCualLumI"].ToString(),
                            HisCualLumD = Lec_CONV["HisCualLumD"].ToString(),
                            HisCuadI = Lec_CONV["HisCuadI"].ToString(),
                            HisCuadD = Lec_CONV["HisCuadD"].ToString(),
                            HisIsquiI2 = Lec_CONV["HisIsquiI2"].ToString(),
                            HisIsqui2D = Lec_CONV["HisIsqui2D"].ToString(),
                            HisTensorI = Lec_CONV["HisTensorI"].ToString(),
                            HisTensorD = Lec_CONV["HisTensorD"].ToString(),
                            HisGastroI = Lec_CONV["HisGastroI"].ToString(),
                            HisGastroD = Lec_CONV["HisGastroD"].ToString(),
                            HisTAquilesI = Lec_CONV["HisTAquilesI"].ToString(),
                            HisTAquilesD = Lec_CONV["HisTAquilesD"].ToString(),
                            HisDiagnostico = Lec_CONV["HisDiagnostico"].ToString(),
                            HisPronostico = Lec_CONV["HisPronostico"].ToString(),
                            HisAcciones = Lec_CONV["HisAcciones"].ToString(),
                            HisConducta = Lec_CONV["HisConducta"].ToString(),
                            Marcha_Mec = Lec_CONV["Marcha_Mec"].ToString()
                        };

                        if (Lec_CONV["HC_ActFis"].ToString() != "X") { H.HC_ActFis = "NO"; } else { H.HC_ActFis = "SI"; }
                        if (Lec_CONV["HC_MedExt"].ToString() != "X") { H.HC_MedExt = "NO"; } else { H.HC_MedExt = "SI"; }

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

    }
}
