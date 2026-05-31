using System;
using System.Data.SqlClient;
using System.Data;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;
using System.Collections.Generic;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MEncuesta1 : IEncuesta1
    {
        bool IEncuesta1.registrarEncuesta1(FIB_ENCUESTA1 E)
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

                    int d = E.fechaEncuesta.Day;
                    int m = E.fechaEncuesta.Month;
                    int y = E.fechaEncuesta.Year;

                    DateTime fecha = new DateTime(y, m, d);

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO FIB_ENCUESTA1 (IdPaciente, " + //param1
                                                                                "FechaEncuesta, " + //param2
                                                                                "UsuarioRegistra, " + //param3
                                                                                "Estado, " + //param4
                                                                                "Pregunta1, " + //param5
                                                                                "Pregunta2, " + //param6
                                                                                "Pregunta3, " + //param7
                                                                                "Pregunta4, " + //param8
                                                                                "Pregunta5, " + //param9
                                                                                "Pregunta6, " + //param10
                                                                                "Pregunta7, " + //param11
                                                                                "Pregunta8, " + //param12
                                                                                "Pregunta9, " + //param13
                                                                                "Pregunta10, " + //param14
                                                                                "Pregunta11, " + //param15
                                                                                "Pregunta12, " + //param15
                                                                                "Pregunta13, " + //param15
                                                                                "Pregunta14, " + //param15
                                                                                "Pregunta15, " + //param15
                                                                                "Pregunta16, " + //param15
                                                                                "Pregunta17, " + //param15
                                                                                "Pregunta18, " + //param15
                                                                                "Pregunta19, " +
                                                                                "Pregunta20, " +
                                                                                "Pregunta21, " +
                                                                                "Pregunta22, " +
                                                                                "Pregunta23, " +
                                                                                "Pregunta24, " +
                                                                                "Pregunta25, " +
                                                                                "Pregunta26, " +
                                                                                "Pregunta27, " +
                                                                                "Pregunta28, " +
                                                                                "Pregunta29, " +
                                                                                "Pregunta30, " +
                                                                                "Pregunta31, " +
                                                                                "Pregunta32, " +
                                                                                "Pregunta33, " +
                                                                                "Pregunta34, " +
                                                                                "Pregunta35, " +
                                                                                "Pregunta36, " +
                                                                                "Pregunta37, " +
                                                                                "Pregunta38, " +
                                                                                "Pregunta39, " +
                                                                                "Pregunta40, " +
                                                                                "Pregunta41, " +
                                                                                "Pregunta42, " +
                                                                                "Pregunta43, " +
                                                                                "Pregunta44, " +
                                                                                "Pregunta45, " +
                                                                                "Pregunta46, " +
                                                                                "Pregunta47, " +
                                                                                "Pregunta48, " +
                                                                                "Pregunta49, " +
                                                                                "Pregunta50, " +
                                                                                "Pregunta51, " +
                                                                                "Pregunta52, " +
                                                                                "Pregunta53, " +
                                                                                "Pregunta54, " +
                                                                                "Pregunta55, " +
                                                                                "Pregunta56) " +
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
                                                          "@param16, " + // Hor_Observacion
                                                          "@param17, " + // Hor_Observacion
                                                          "@param18, " + // Hor_Observacion
                                                          "@param19, " + // Hor_Observacion
                                                          "@param20, " + // Hor_Observacion
                                                          "@param21, " + // Hor_Observacion
                                                          "@param22, " + // Hor_Observacion
                                                          "@param23, " +
                                                          "@param24, " +
                                                          "@param25, " +
                                                          "@param26, " +
                                                          "@param27, " +
                                                          "@param28, " +
                                                          "@param29, " +
                                                          "@param30, " +
                                                          "@param31, " +
                                                          "@param32, " +
                                                          "@param33, " +
                                                          "@param34, " +
                                                          "@param35, " +
                                                          "@param36, " +
                                                          "@param37, " +
                                                          "@param38, " +
                                                          "@param39, " +
                                                          "@param40, " +
                                                          "@param41, " +
                                                          "@param42, " +
                                                          "@param43, " +
                                                          "@param44, " +
                                                          "@param45, " +
                                                          "@param46, " +
                                                          "@param47, " +
                                                          "@param48, " +
                                                          "@param49, " +
                                                          "@param50, " +
                                                          "@param51, " +
                                                          "@param52, " +
                                                          "@param53, " +
                                                          "@param54, " +
                                                          "@param55, " +
                                                          "@param56, " +
                                                          "@param57, " +
                                                          "@param58, " +
                                                          "@param59, " +
                                                          "@param60)", con);

                    cmd.Parameters.AddWithValue("@param1", E.IdPaciente);
                    cmd.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = fecha;
                    cmd.Parameters.AddWithValue("@param3", E.UsuarioRegistra);
                    cmd.Parameters.AddWithValue("@param4", "V");
                    cmd.Parameters.AddWithValue("@param5", E.Pregunta1);
                    cmd.Parameters.AddWithValue("@param6", E.Pregunta2);
                    cmd.Parameters.AddWithValue("@param7", E.Pregunta3);
                    cmd.Parameters.AddWithValue("@param8", E.Pregunta4);
                    cmd.Parameters.AddWithValue("@param9", E.Pregunta5);
                    cmd.Parameters.AddWithValue("@param10", E.Pregunta6);
                    cmd.Parameters.AddWithValue("@param11", E.Pregunta7);
                    cmd.Parameters.AddWithValue("@param12", E.Pregunta8);
                    cmd.Parameters.AddWithValue("@param13", E.Pregunta9);
                    cmd.Parameters.AddWithValue("@param14", E.Pregunta10);
                    cmd.Parameters.AddWithValue("@param15", E.Pregunta11);
                    cmd.Parameters.AddWithValue("@param16", E.Pregunta12);
                    cmd.Parameters.AddWithValue("@param17", E.Pregunta13);
                    cmd.Parameters.AddWithValue("@param18", E.Pregunta14);
                    cmd.Parameters.AddWithValue("@param19", E.Pregunta15);
                    cmd.Parameters.AddWithValue("@param20", E.Pregunta16);
                    cmd.Parameters.AddWithValue("@param21", E.Pregunta17);
                    cmd.Parameters.AddWithValue("@param22", E.Pregunta18);
                    cmd.Parameters.AddWithValue("@param23", E.Pregunta19);
                    cmd.Parameters.AddWithValue("@param24", E.Pregunta20);
                    cmd.Parameters.AddWithValue("@param25", E.Pregunta21);
                    cmd.Parameters.AddWithValue("@param26", E.Pregunta22);
                    cmd.Parameters.AddWithValue("@param27", E.Pregunta23);
                    cmd.Parameters.AddWithValue("@param28", E.Pregunta24);
                    cmd.Parameters.AddWithValue("@param29", E.Pregunta25);
                    cmd.Parameters.AddWithValue("@param30", E.Pregunta26);
                    cmd.Parameters.AddWithValue("@param31", E.Pregunta27);
                    cmd.Parameters.AddWithValue("@param32", E.Pregunta28);
                    cmd.Parameters.AddWithValue("@param33", E.Pregunta29);
                    cmd.Parameters.AddWithValue("@param34", E.Pregunta30);
                    cmd.Parameters.AddWithValue("@param35", E.Pregunta31);
                    cmd.Parameters.AddWithValue("@param36", E.Pregunta32);
                    cmd.Parameters.AddWithValue("@param37", E.Pregunta33);
                    cmd.Parameters.AddWithValue("@param38", E.Pregunta34);
                    cmd.Parameters.AddWithValue("@param39", E.Pregunta35);
                    cmd.Parameters.AddWithValue("@param40", E.Pregunta36);
                    cmd.Parameters.AddWithValue("@param41", E.Pregunta37);
                    cmd.Parameters.AddWithValue("@param42", E.Pregunta38);
                    cmd.Parameters.AddWithValue("@param43", E.Pregunta39);
                    cmd.Parameters.AddWithValue("@param44", E.Pregunta40);
                    cmd.Parameters.AddWithValue("@param45", E.Pregunta41);
                    cmd.Parameters.AddWithValue("@param46", E.Pregunta42);
                    cmd.Parameters.AddWithValue("@param47", E.Pregunta43);
                    cmd.Parameters.AddWithValue("@param48", E.Pregunta44);
                    cmd.Parameters.AddWithValue("@param49", E.Pregunta45);
                    cmd.Parameters.AddWithValue("@param50", E.Pregunta46);
                    cmd.Parameters.AddWithValue("@param51", E.Pregunta47);
                    cmd.Parameters.AddWithValue("@param52", E.Pregunta48);
                    cmd.Parameters.AddWithValue("@param53", E.Pregunta49);
                    cmd.Parameters.AddWithValue("@param54", E.Pregunta50);
                    cmd.Parameters.AddWithValue("@param55", E.Pregunta51);
                    cmd.Parameters.AddWithValue("@param56", E.Pregunta52);
                    cmd.Parameters.AddWithValue("@param57", E.Pregunta53);
                    cmd.Parameters.AddWithValue("@param58", E.Pregunta54);
                    cmd.Parameters.AddWithValue("@param59", E.Pregunta55);
                    cmd.Parameters.AddWithValue("@param60", E.Pregunta56);
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
