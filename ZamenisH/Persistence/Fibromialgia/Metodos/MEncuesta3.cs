using System;
using System.Data.SqlClient;
using System.Data;
using Domain.Fibromialgia;
using Persistence.Fibromialgia.Interfaces;
using System.Collections.Generic;
using Domain;

namespace Persistence.Fibromialgia.Metodos
{
    public class MEncuesta3 : IEncuesta3
    {
        bool IEncuesta3.registrarEncuesta(FIB_ENCUESTA3 E)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO FIB_ENCUESTA3 (IdPaciente, " + //param1
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
                                                                                "Pregunta10) " +
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
                                                          "@param14)", con);

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
