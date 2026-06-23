using System;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System.Collections.Generic;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MTerapiaOcupacional : ITerapiaOcupacional
    {
        bool ITerapiaOcupacional.insertHistoria(CXN_HCTO H)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_HCTO (HC_Pac , " + //param1
                                                         "HC_Cant, " + //param2
                                                         "HC_Prof, " + //param3
                                                         "HC_Ase, " + //param4
                                                         "HC_Cia, " + //param5
                                                         "HC_Fecha, " + //param6
                                                         "HC_Edad, " + //param7
                                                         "HC_PacId , " + //param8
                                                         "HC_Adm, " + //param9
                                                         "HC_FechaNto , " + //param10
                                                         "HC_CIE10, " + //param11
                                                         "HC_DiagMedico , " + //param12
                                                         "HC_OcuAct , " + //param13
                                                         "HC_HisFam , " + //param14
                                                         "HC_HisOcu , " + //param15
                                                         "HC_HabRut , " + //param15
                                                         "HC_DiagOcu , " + //param15
                                                         "HC_PronoOcu , " + //param15
                                                         "HC_Acciones , " + //param15
                                                         "HC_AntLaboral , " + //param15
                                                         "MSD_MCP , " + //param15
                                                         "MSD_MCA , " + //param15
                                                         "MSI_MCP  , " + //param15
                                                         "MSI_MCA , " + //param15
                                                         "MSD_MBP , " + //param15
                                                         "MSD_MBA , " + //param15
                                                         "MSI_MBP , " + //param15
                                                         "MSI_MBA , " + //param15
                                                         "MSD_MHP , " + //param15
                                                         "MSD_MHA , " + //param15
                                                         "MSI_MHP , " + //param15
                                                         "MSI_MHA , " + //param15
                                                         "MSD_MEP , " + //param15
                                                         "MSD_MEA , " + //param15
                                                         "MSI_MEP , " + //param15
                                                         "MSI_MEA , " + //param15
                                                         "MSD_MCIP , " + //param15
                                                         "MSD_MCIA , " + //param15
                                                         "MSI_MCIP , " + //param15
                                                         "MSI_MCIA , " + //param15
                                                         "MSD_MPP , " + //param15
                                                         "MSD_MPA , " + //param15
                                                         "MSI_MPP , " + //param15
                                                         "MSI_MPA , " + //param15
                                                         "MSD_MRP , " + //param15
                                                         "MSD_MRA , " + //param15
                                                         "MSI_MRP , " + //AQUI TERMINA PRIMERA 47
                                                         "MSI_MRA , " + //param15
                                                         "MSD_MPIEP , " + //param15
                                                         "MSD_MPIEA , " + //param15
                                                         "MSI_MPIEP , " + //param15
                                                         "MSI_MPIEA , " + //param15
                                                         "MSD_AAP , " + //param15
                                                         "MSD_AAA , " + //param15
                                                         "MSI_AAP , " + //param15
                                                         "MSI_AAA , " + //param15
                                                         "MSD_AABP , " + //param15
                                                         "MSD_AABA , " + //param15
                                                         "MSI_AABP , " + //param15
                                                         "MSI_AABA , " + //param15
                                                         "MSD_AADP , " + //param15
                                                         "MSD_AADA , " + //param15
                                                         "MSI_AADP , " + //param15
                                                         "MSI_AADA , " + //param15
                                                         "MSD_AATP , " + //param15
                                                         "MSD_AATA , " + //param15
                                                         "MSI_AATP , " + //param15
                                                         "MSI_AATA , " + //param15
                                                         "MSD_AALP , " + //param15
                                                         "MSD_AALA , " + //param15
                                                         "MSI_AALP , " + //param15
                                                         "MSI_AALA , " + //param15
                                                         "MSD_AGAP , " + //param15
                                                         "MSD_AGAA , " + //param15
                                                         "MSI_AGAP , " + //param15
                                                         "MSI_AGAA , " + //param15
                                                         "MSD_AGACILP , " + //param15
                                                         "MSD_AGACILA , " + //param15
                                                         "MSI_AGACILP , " + //param15
                                                         "MSI_AGACILA , " + //param15
                                                         "MSD_AGAESFP , " + //param15
                                                         "MSD_AGAESFA , " + //param15
                                                         "MSI_AGAESFP , " + //param15
                                                         "MSI_AGAESFA , " + //param15
                                                         "MSD_PINFP  , " + //param15
                                                         "MSD_PINFA , " + //param15
                                                         "MSI_PINFP , " + //param15
                                                         "MSI_PINFA , " + //param15
                                                         "MSD_PINTP , " + //param15
                                                         "MSD_PINTA , " + //param15
                                                         "MSI_PINTP , " + //param15
                                                         "MSI_PINTA , " + //param15
                                                         "MSD_PLATP , " + //param15
                                                         "MSD_PLATA , " + //param15
                                                         "MSI_PLATP , " + //param15
                                                         "MSI_PLATA , " + //param15
                                                         "HC_ObservaFMS , " + //param15
                                                         "HC_A_T1 , " + //param15
                                                         "HC_A_T2 , " + //param15
                                                         "HC_A_T3 , " + //param15
                                                         "HC_A_T4 , " + //param15
                                                         "HC_A_T5 , " + //param15
                                                         "HC_A_T6 , " + //param15
                                                         "HC_A_T7 , " + //param15
                                                         "HC_A_T8 , " + //param15
                                                         "HC_A_T9 , " + //param15
                                                         "HC_A_T10 , " + //param15
                                                         "HC_A_T11 , " + //param15
                                                         "HC_A_T12 , " + //param15
                                                         "HC_A_T13 , " + //param15
                                                         "HC_A_T14 , " + //param15
                                                         "HC_A_T15 , " + //param15
                                                         "HC_A_T16 , " + //param15
                                                         "HC_A_T17 , " + //param15
                                                         "HC_A_T18 , " + //param15
                                                         "HC_A_T19 , " + //param15
                                                         "HC_A_T20 , " + //param15
                                                         "HC_A_T21 , " + //param15
                                                         "HC_A_T22 , " + //param15
                                                         "HC_A_T23 , " + //param15
                                                         "HC_A_T24 , " + //param15
                                                         "HC_A_T25 , " + //param15
                                                         "HC_A_T26 , " + //param15
                                                         "HC_A_T27 , " + //param15
                                                         "HC_A_T28 , " + //param15
                                                         "HC_A_T29 , " + //param15
                                                         "HC_A_T30 , " + //param15
                                                         "HC_A_T31 , " + //param15
                                                         "HC_A_T32 , " + //param15
                                                         "HC_A_T33 , " + //param15
                                                         "HC_A_T34 , " + //param15
                                                         "HC_A_T35 , " + //param15
                                                         "HC_A_ObservaVD , " + //param15
                                                         "HC_A_T37 , " + //param15
                                                         "HC_A_T43 , " + //param15
                                                         "HC_A_T38 , " + //param15
                                                         "HC_A_T44 , " + //param15
                                                         "HC_A_T39 , " + //param15
                                                         "HC_A_T45 , " + //param15
                                                         "HC_A_T40 , " + //param15
                                                         "HC_A_T46 , " + //param15
                                                         "HC_A_T41 , " + //param15
                                                         "HC_A_T47 , " + //param15
                                                         "HC_A_T42 , " + //param15
                                                         "HC_A_T48 , " + //param15
                                                         "HC_A_T50 , " + //param15
                                                         "HC_A_T51 , " + //param15
                                                         "HC_A_T52 , " + //param15
                                                         "HC_A_T53 , " + //param15
                                                         "HC_A_T54 , " + //param15
                                                         "HC_A_T55 , " + //param15
                                                         "HC_A_T56 , " + //param15
                                                         "HC_A_T57 , " + //param15
                                                         "HC_A_T58 , " + //param15
                                                         "HC_A_T59 , " + //param15
                                                         "HC_A_T60 , " + //param15
                                                         "HC_A_ObservaAVD , " + //param15
                                                         "HC_A_ObservaH ) " + //Termina 2 de 25
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
                                                         "@param72, " + // Hor_Imp_Age
                                                         "@param73, " + // Hor_Pac_Fecha
                                                         "@param74, " + // Hor_Pac_Fecha_Cita
                                                         "@param75, " + // Hor_Pac_Hora
                                                         "@param76, " + // Hor_Pac_Id_Hora
                                                         "@param77, " + // Hor_Pac_Hora_Cita
                                                         "@param78, " + // Hor_Observacion
                                                         "@param79, " + // Hor_Observacion
                                                         "@param80, " + // Hor_Observacion
                                                         "@param81, " + // Hor_Observacion
                                                         "@param82, " + // Hor_Observacion
                                                         "@param83, " + // Hor_Observacion
                                                         "@param84, " + // Hor_Observacion
                                                         "@param85, " + // Hor_Observacion
                                                         "@param86, " + // Hor_Observacion
                                                         "@param87, " + // Hor_Observacion
                                                         "@param88, " + // Hor_Observacion
                                                         "@param89, " + // Hor_Observacion
                                                         "@param90, " + // Hor_Observacion
                                                         "@param91, " + // Hor_Observacion
                                                         "@param92, " + // Hor_Observacion
                                                         "@param93, " + // Hor_Imp_Age
                                                         "@param94, " + // Hor_Pac_Fecha
                                                         "@param95, " + // Hor_Pac_Fecha_Cita
                                                         "@param96, " + // Hor_Pac_Hora
                                                         "@param97, " + // Hor_Pac_Id_Hora
                                                         "@param98, " + // Hor_Pac_Hora_Cita
                                                         "@param99, " + // Hor_Observacion
                                                         "@param100, " + // Hor_Observacion
                                                         "@param101, " + // Hor_Observacion
                                                         "@param102, " + // Hor_Observacion
                                                         "@param103, " + // Hor_Observacion
                                                         "@param104, " + // Hor_Observacion
                                                         "@param105, " + // Hor_Observacion
                                                         "@param106, " + // Hor_Observacion
                                                         "@param107, " + // Hor_Observacion
                                                         "@param108, " + // Hor_Observacion
                                                         "@param109, " + // Hor_Observacion
                                                         "@param110, " + // Hor_Observacion
                                                         "@param111, " + // Hor_Observacion
                                                         "@param112, " + // Hor_Observacion
                                                         "@param113, " + // Hor_Observacion
                                                         "@param114, " + // Hor_Imp_Age
                                                         "@param115, " + // Hor_Pac_Fecha
                                                         "@param116, " + // Hor_Pac_Fecha_Cita
                                                         "@param117, " + // Hor_Pac_Hora
                                                         "@param118, " + // Hor_Pac_Id_Hora
                                                         "@param119, " + // Hor_Pac_Hora_Cita
                                                         "@param120, " + // Hor_Observacion
                                                         "@param121, " + // Hor_Observacion
                                                         "@param122, " + // Hor_Observacion
                                                         "@param123, " + // Hor_Observacion
                                                         "@param124, " + // Hor_Observacion
                                                         "@param125, " + // Hor_Observacion
                                                         "@param126, " + // Hor_Observacion
                                                         "@param127, " + // Hor_Observacion
                                                         "@param128, " + // Hor_Observacion
                                                         "@param129, " + // Hor_Observacion
                                                         "@param130, " + // Hor_Observacion
                                                         "@param131, " + // Hor_Observacion
                                                         "@param132, " + // Hor_Observacion
                                                         "@param133, " + // Hor_Observacion
                                                         "@param134, " + // Hor_Observacion
                                                         "@param135, " + // Hor_Observacion
                                                         "@param136, " + // Hor_Observacion
                                                         "@param137, " + // Hor_Observacion
                                                         "@param138, " + // Hor_Observacion
                                                         "@param139, " + // Hor_Observacion
                                                         "@param140, " + // Hor_Observacion
                                                         "@param141, " + // Hor_Observacion
                                                         "@param142, " + // Hor_Observacion
                                                         "@param143, " + // Hor_Observacion
                                                         "@param144, " + // Hor_Observacion
                                                         "@param145, " + // Hor_Observacion
                                                         "@param146, " + // Hor_Observacion
                                                         "@param147, " + // Hor_Observacion
                                                         "@param148, " + // Hor_Observacion
                                                         "@param149, " + // Hor_Observacion
                                                         "@param150, " + // Hor_Observacion
                                                         "@param151, " + // Hor_Observacion
                                                         "@param152, " + // Hor_Observacion
                                                         "@param153, " + // Hor_Observacion
                                                         "@param154, " + // Hor_Observacion
                                                         "@param155, " + // Hor_Observacion
                                                         "@param156, " + // Hor_Observacion
                                                         "@param157, " + // Hor_Observacion
                                                         "@param158)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", H.HC_Pac);
                    cmd.Parameters.AddWithValue("@param2", H.HC_Cant);
                    cmd.Parameters.AddWithValue("@param3", H.HC_Prof);
                    cmd.Parameters.AddWithValue("@param4", H.HC_Ase);
                    cmd.Parameters.AddWithValue("@param5", H.HC_Cia);
                    cmd.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = H.HC_Fecha; // Fecha que graba cita
                    cmd.Parameters.AddWithValue("@param7", H.HC_Edad);
                    cmd.Parameters.AddWithValue("@param8", H.HC_PacId);
                    cmd.Parameters.AddWithValue("@param9", H.HC_Adm);
                    cmd.Parameters.Add(new SqlParameter("@param10", SqlDbType.DateTime)).Value = H.HC_FechaNto;
                    cmd.Parameters.AddWithValue("@param11", H.HC_CIE10);
                    cmd.Parameters.AddWithValue("@param12", H.HC_DiagMedico);
                    cmd.Parameters.AddWithValue("@param13", H.HC_OcuAct);

                    cmd.Parameters.AddWithValue("@param14", H.HC_HisFam);
                    cmd.Parameters.AddWithValue("@param15", H.HC_HisOcu);
                    cmd.Parameters.AddWithValue("@param16", H.HC_HabRut);
                    cmd.Parameters.AddWithValue("@param17", H.HC_DiagOcu);
                    cmd.Parameters.AddWithValue("@param18", H.HC_PronoOcu);
                    cmd.Parameters.AddWithValue("@param19", H.HC_Acciones);
                    cmd.Parameters.AddWithValue("@param20", H.HC_AntLaboral);
                    cmd.Parameters.AddWithValue("@param21", H.MSD_MCP);
                    cmd.Parameters.AddWithValue("@param22", H.MSD_MCA);
                    cmd.Parameters.AddWithValue("@param23", H.MSI_MCP);
                    cmd.Parameters.AddWithValue("@param24", H.MSI_MCA);
                    cmd.Parameters.AddWithValue("@param25", H.MSD_MBP);
                    cmd.Parameters.AddWithValue("@param26", H.MSD_MBA);
                    cmd.Parameters.AddWithValue("@param27", H.MSI_MBP);
                    cmd.Parameters.AddWithValue("@param28", H.MSI_MBA);
                    cmd.Parameters.AddWithValue("@param29", H.MSD_MHP);
                    cmd.Parameters.AddWithValue("@param30", H.MSD_MHA);
                    cmd.Parameters.AddWithValue("@param31", H.MSI_MHP);
                    cmd.Parameters.AddWithValue("@param32", H.MSI_MHA);
                    cmd.Parameters.AddWithValue("@param33", H.MSD_MEP);
                    cmd.Parameters.AddWithValue("@param34", H.MSD_MEA);
                    cmd.Parameters.AddWithValue("@param35", H.MSI_MEP);
                    cmd.Parameters.AddWithValue("@param36", H.MSI_MEA);
                    cmd.Parameters.AddWithValue("@param37", H.MSD_MCIP);
                    cmd.Parameters.AddWithValue("@param38", H.MSD_MCIA);
                    cmd.Parameters.AddWithValue("@param39", H.MSI_MCIP);
                    cmd.Parameters.AddWithValue("@param40", H.MSI_MCIA);
                    cmd.Parameters.AddWithValue("@param41", H.MSD_MPP);
                    cmd.Parameters.AddWithValue("@param42", H.MSD_MPA);
                    cmd.Parameters.AddWithValue("@param43", H.MSI_MPP);
                    cmd.Parameters.AddWithValue("@param44", H.MSI_MPA);
                    cmd.Parameters.AddWithValue("@param45", H.MSD_MRP);
                    cmd.Parameters.AddWithValue("@param46", H.MSD_MRA);
                    cmd.Parameters.AddWithValue("@param47", H.MSI_MRP);

                    cmd.Parameters.AddWithValue("@param48", H.MSI_MRA);
                    cmd.Parameters.AddWithValue("@param49", H.MSD_MPIEP);
                    cmd.Parameters.AddWithValue("@param50", H.MSD_MPIEA);
                    cmd.Parameters.AddWithValue("@param51", H.MSI_MPIEP);
                    cmd.Parameters.AddWithValue("@param52", H.MSI_MPIEA);
                    cmd.Parameters.AddWithValue("@param53", H.MSD_AAP);
                    cmd.Parameters.AddWithValue("@param54", H.MSD_AAA);
                    cmd.Parameters.AddWithValue("@param55", H.MSI_AAP);
                    cmd.Parameters.AddWithValue("@param56", H.MSI_AAA);
                    cmd.Parameters.AddWithValue("@param57", H.MSD_AABP);
                    cmd.Parameters.AddWithValue("@param58", H.MSD_AABA);
                    cmd.Parameters.AddWithValue("@param59", H.MSI_AABP);
                    cmd.Parameters.AddWithValue("@param60", H.MSI_AABA);
                    cmd.Parameters.AddWithValue("@param61", H.MSD_AADP);
                    cmd.Parameters.AddWithValue("@param62", H.MSD_AADA);
                    cmd.Parameters.AddWithValue("@param63", H.MSI_AADP);
                    cmd.Parameters.AddWithValue("@param64", H.MSI_AADA);
                    cmd.Parameters.AddWithValue("@param65", H.MSD_AATP);
                    cmd.Parameters.AddWithValue("@param66", H.MSD_AATA);
                    cmd.Parameters.AddWithValue("@param67", H.MSI_AATP);
                    cmd.Parameters.AddWithValue("@param68", H.MSI_AATA);
                    cmd.Parameters.AddWithValue("@param69", H.MSD_AALP);
                    cmd.Parameters.AddWithValue("@param70", H.MSD_AALA);
                    cmd.Parameters.AddWithValue("@param71", H.MSI_AALP);
                    cmd.Parameters.AddWithValue("@param72", H.MSI_AALA);
                    cmd.Parameters.AddWithValue("@param73", H.MSD_AGAP);
                    cmd.Parameters.AddWithValue("@param74", H.MSD_AGAA);
                    cmd.Parameters.AddWithValue("@param75", H.MSI_AGAP);
                    cmd.Parameters.AddWithValue("@param76", H.MSI_AGAA);
                    cmd.Parameters.AddWithValue("@param77", H.MSD_AGACILP);
                    cmd.Parameters.AddWithValue("@param78", H.MSD_AGACILA);
                    cmd.Parameters.AddWithValue("@param79", H.MSI_AGACILP);
                    cmd.Parameters.AddWithValue("@param80", H.MSI_AGACILA);
                    cmd.Parameters.AddWithValue("@param81", H.MSD_AGAESFP);
                    cmd.Parameters.AddWithValue("@param82", H.MSD_AGAESFA);
                    cmd.Parameters.AddWithValue("@param83", H.MSI_AGAESFP);
                    cmd.Parameters.AddWithValue("@param84", H.MSI_AGAESFA);
                    cmd.Parameters.AddWithValue("@param85", H.MSD_PINFP);
                    cmd.Parameters.AddWithValue("@param86", H.MSD_PINFA);
                    cmd.Parameters.AddWithValue("@param87", H.MSI_PINFP);
                    cmd.Parameters.AddWithValue("@param88", H.MSI_PINFA);
                    cmd.Parameters.AddWithValue("@param89", H.MSD_PINTP);
                    cmd.Parameters.AddWithValue("@param90", H.MSD_PINTA);
                    cmd.Parameters.AddWithValue("@param91", H.MSI_PINTP);
                    cmd.Parameters.AddWithValue("@param92", H.MSI_PINTA);
                    cmd.Parameters.AddWithValue("@param93", H.MSD_PLATP);
                    cmd.Parameters.AddWithValue("@param94", H.MSD_PLATA);
                    cmd.Parameters.AddWithValue("@param95", H.MSI_PLATP);
                    cmd.Parameters.AddWithValue("@param96", H.MSI_PLATA);
                    cmd.Parameters.AddWithValue("@param97", H.HC_ObservaFMS);
                    cmd.Parameters.AddWithValue("@param98", H.HC_A_T1);
                    cmd.Parameters.AddWithValue("@param99", H.HC_A_T2);
                    cmd.Parameters.AddWithValue("@param100", H.HC_A_T3);
                    cmd.Parameters.AddWithValue("@param101", H.HC_A_T4);
                    cmd.Parameters.AddWithValue("@param102", H.HC_A_T5);
                    cmd.Parameters.AddWithValue("@param103", H.HC_A_T6);
                    cmd.Parameters.AddWithValue("@param104", H.HC_A_T7);
                    cmd.Parameters.AddWithValue("@param105", H.HC_A_T8);
                    cmd.Parameters.AddWithValue("@param106", H.HC_A_T9);
                    cmd.Parameters.AddWithValue("@param107", H.HC_A_T10);
                    cmd.Parameters.AddWithValue("@param108", H.HC_A_T11);
                    cmd.Parameters.AddWithValue("@param109", H.HC_A_T12);
                    cmd.Parameters.AddWithValue("@param110", H.HC_A_T13);
                    cmd.Parameters.AddWithValue("@param111", H.HC_A_T14);
                    cmd.Parameters.AddWithValue("@param112", H.HC_A_T15);
                    cmd.Parameters.AddWithValue("@param113", H.HC_A_T16);
                    cmd.Parameters.AddWithValue("@param114", H.HC_A_T17);
                    cmd.Parameters.AddWithValue("@param115", H.HC_A_T18);
                    cmd.Parameters.AddWithValue("@param116", H.HC_A_T19);
                    cmd.Parameters.AddWithValue("@param117", H.HC_A_T20);
                    cmd.Parameters.AddWithValue("@param118", H.HC_A_T21);
                    cmd.Parameters.AddWithValue("@param119", H.HC_A_T22);
                    cmd.Parameters.AddWithValue("@param120", H.HC_A_T23);
                    cmd.Parameters.AddWithValue("@param121", H.HC_A_T24);
                    cmd.Parameters.AddWithValue("@param122", H.HC_A_T25);
                    cmd.Parameters.AddWithValue("@param123", H.HC_A_T26);
                    cmd.Parameters.AddWithValue("@param124", H.HC_A_T27);
                    cmd.Parameters.AddWithValue("@param125", H.HC_A_T28);
                    cmd.Parameters.AddWithValue("@param126", H.HC_A_T29);
                    cmd.Parameters.AddWithValue("@param127", H.HC_A_T30);
                    cmd.Parameters.AddWithValue("@param128", H.HC_A_T31);
                    cmd.Parameters.AddWithValue("@param129", H.HC_A_T32);
                    cmd.Parameters.AddWithValue("@param130", H.HC_A_T33);
                    cmd.Parameters.AddWithValue("@param131", H.HC_A_T34);
                    cmd.Parameters.AddWithValue("@param132", H.HC_A_T35);
                    cmd.Parameters.AddWithValue("@param133", H.HC_A_ObservaVD);
                    cmd.Parameters.AddWithValue("@param134", H.HC_A_T37);
                    cmd.Parameters.AddWithValue("@param135", H.HC_A_T43);
                    cmd.Parameters.AddWithValue("@param136", H.HC_A_T38);
                    cmd.Parameters.AddWithValue("@param137", H.HC_A_T44);
                    cmd.Parameters.AddWithValue("@param138", H.HC_A_T39);
                    cmd.Parameters.AddWithValue("@param139", H.HC_A_T45);
                    cmd.Parameters.AddWithValue("@param140", H.HC_A_T40);
                    cmd.Parameters.AddWithValue("@param141", H.HC_A_T46);
                    cmd.Parameters.AddWithValue("@param142", H.HC_A_T41);
                    cmd.Parameters.AddWithValue("@param143", H.HC_A_T47);
                    cmd.Parameters.AddWithValue("@param144", H.HC_A_T42);
                    cmd.Parameters.AddWithValue("@param145", H.HC_A_T48);
                    cmd.Parameters.AddWithValue("@param146", H.HC_A_T50);
                    cmd.Parameters.AddWithValue("@param147", H.HC_A_T51);
                    cmd.Parameters.AddWithValue("@param148", H.HC_A_T52);
                    cmd.Parameters.AddWithValue("@param149", H.HC_A_T53);
                    cmd.Parameters.AddWithValue("@param150", H.HC_A_T54);
                    cmd.Parameters.AddWithValue("@param151", H.HC_A_T55);
                    cmd.Parameters.AddWithValue("@param152", H.HC_A_T56);
                    cmd.Parameters.AddWithValue("@param153", H.HC_A_T57);
                    cmd.Parameters.AddWithValue("@param154", H.HC_A_T58);
                    cmd.Parameters.AddWithValue("@param155", H.HC_A_T59);
                    cmd.Parameters.AddWithValue("@param156", H.HC_A_T60);
                    cmd.Parameters.AddWithValue("@param157", H.HC_A_ObservaAVD);
                    cmd.Parameters.AddWithValue("@param158", H.HC_A_ObservaH);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        CXN_HCTO ITerapiaOcupacional.getLastHistoy(int Paciente)
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

                    String CONV = "SELECT * from CXN_HCTO WHERE HC_PacId = '" + Paciente + "' order by HC_Fecha DESC";
                    SqlCommand Com_CONV = new SqlCommand(CONV, con);
                    SqlDataReader Lec_CONV = (Com_CONV.ExecuteReader());
                    if (Lec_CONV.Read() == true)
                    {
                        CXN_HCTO H = new CXN_HCTO
                        {
                            HC_Fecha = Convert.ToDateTime(Lec_CONV["HC_Fecha"]),
                            HC_CIE10 = Lec_CONV["HC_CIE10"].ToString(),
                            HC_DiagMedico = Lec_CONV["HC_DiagMedico"].ToString(),
                            HC_OcuAct = Lec_CONV["HC_OcuAct"].ToString(),
                            HC_HisFam = Lec_CONV["HC_HisFam"].ToString(),
                            HC_HisOcu = Lec_CONV["HC_HisOcu"].ToString(),
                            HC_HabRut = Lec_CONV["HC_HabRut"].ToString(),
                            HC_Acciones = Lec_CONV["HC_Acciones"].ToString(),
                            HC_AntLaboral = Lec_CONV["HC_AntLaboral"].ToString(),
                            HC_DiagOcu = Lec_CONV["HC_DiagOcu"].ToString(),
                            HC_PronoOcu = Lec_CONV["HC_PronoOcu"].ToString(),

                            HC_A_T1 = Lec_CONV["HC_A_T1"].ToString(),
                            HC_A_T2 = Lec_CONV["HC_A_T2"].ToString(),
                            HC_A_T3 = Lec_CONV["HC_A_T3"].ToString(),

                            HC_A_T4 = Lec_CONV["HC_A_T4"].ToString(),
                            HC_A_T5 = Lec_CONV["HC_A_T5"].ToString(),
                            HC_A_T6 = Lec_CONV["HC_A_T6"].ToString(),
                            HC_A_T7 = Lec_CONV["HC_A_T7"].ToString(),
                            HC_A_T8 = Lec_CONV["HC_A_T8"].ToString(),
                            HC_A_T9 = Lec_CONV["HC_A_T9"].ToString(),
                            HC_A_T10 = Lec_CONV["HC_A_T10"].ToString(),

                            HC_A_T11 = Lec_CONV["HC_A_T11"].ToString(),
                            HC_A_T12 = Lec_CONV["HC_A_T12"].ToString(),
                            HC_A_T13 = Lec_CONV["HC_A_T13"].ToString(),
                            HC_A_T14 = Lec_CONV["HC_A_T14"].ToString(),

                            HC_A_T15 = Lec_CONV["HC_A_T15"].ToString(),
                            HC_A_T16 = Lec_CONV["HC_A_T16"].ToString(),
                            HC_A_T17 = Lec_CONV["HC_A_T17"].ToString(),
                            HC_A_T18 = Lec_CONV["HC_A_T18"].ToString(),
                            HC_A_T19 = Lec_CONV["HC_A_T19"].ToString(),
                            HC_A_T20 = Lec_CONV["HC_A_T20"].ToString(),
                            HC_A_T21 = Lec_CONV["HC_A_T21"].ToString(),
                            HC_A_T22 = Lec_CONV["HC_A_T22"].ToString(),
                            HC_A_T23 = Lec_CONV["HC_A_T23"].ToString(),
                            HC_A_T24 = Lec_CONV["HC_A_T24"].ToString(),

                            HC_A_T25 = Lec_CONV["HC_A_T25"].ToString(),
                            HC_A_T26 = Lec_CONV["HC_A_T26"].ToString(),
                            HC_A_T27 = Lec_CONV["HC_A_T27"].ToString(),

                            HC_A_T28 = Lec_CONV["HC_A_T28"].ToString(),
                            HC_A_T29 = Lec_CONV["HC_A_T29"].ToString(),
                            HC_A_T30 = Lec_CONV["HC_A_T30"].ToString(),
                            HC_A_T31 = Lec_CONV["HC_A_T31"].ToString(),

                            HC_A_T32 = Lec_CONV["HC_A_T32"].ToString(),
                            HC_A_T33 = Lec_CONV["HC_A_T33"].ToString(),
                            HC_A_T34 = Lec_CONV["HC_A_T34"].ToString(),
                            HC_A_T35 = Lec_CONV["HC_A_T35"].ToString(),

                            HC_A_ObservaVD = Lec_CONV["HC_A_ObservaVD"].ToString(),

                            HC_A_T37 = Lec_CONV["HC_A_T37"].ToString(),
                            HC_A_T38 = Lec_CONV["HC_A_T38"].ToString(),
                            HC_A_T39 = Lec_CONV["HC_A_T39"].ToString(),
                            HC_A_T40 = Lec_CONV["HC_A_T40"].ToString(),
                            HC_A_T41 = Lec_CONV["HC_A_T41"].ToString(),
                            HC_A_T42 = Lec_CONV["HC_A_T42"].ToString(),
                            HC_A_T43 = Lec_CONV["HC_A_T43"].ToString(),
                            HC_A_T44 = Lec_CONV["HC_A_T44"].ToString(),
                            HC_A_T45 = Lec_CONV["HC_A_T45"].ToString(),
                            HC_A_T46 = Lec_CONV["HC_A_T46"].ToString(),
                            HC_A_T47 = Lec_CONV["HC_A_T47"].ToString(),
                            HC_A_T48 = Lec_CONV["HC_A_T48"].ToString(),

                            HC_A_ObservaH = Lec_CONV["HC_A_ObservaH"].ToString(),

                            HC_A_T50 = Lec_CONV["HC_A_T50"].ToString(),
                            HC_A_T51 = Lec_CONV["HC_A_T51"].ToString(),
                            HC_A_T52 = Lec_CONV["HC_A_T52"].ToString(),

                            HC_A_T53 = Lec_CONV["HC_A_T53"].ToString(),
                            HC_A_T54 = Lec_CONV["HC_A_T54"].ToString(),
                            HC_A_T55 = Lec_CONV["HC_A_T55"].ToString(),
                            HC_A_T56 = Lec_CONV["HC_A_T56"].ToString(),
                            HC_A_T57 = Lec_CONV["HC_A_T57"].ToString(),
                            HC_A_T58 = Lec_CONV["HC_A_T58"].ToString(),
                            HC_A_T59 = Lec_CONV["HC_A_T59"].ToString(),
                            HC_A_T60 = Lec_CONV["HC_A_T60"].ToString(),

                            HC_A_ObservaAVD = Lec_CONV["HC_A_ObservaAVD"].ToString()
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
    }
}
