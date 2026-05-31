using System;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MEvolucionesFibromialgia : IEvolucionesFibromialgia
    {
        CXN_EVOFIB IEvolucionesFibromialgia.getContador(CXN_EVOFIB E)
        {
            try
            {
                var datosConexion = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datosConexion["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT TOP 1 * " +
                                         "FROM CXN_EVOFIB " +
                                         "WHERE Evo_Pac = '" + E.Evo_Pac + "' " +
                                         "AND Evo_Tipo = '" + E.Evo_Tipo + "' " +
                                         "AND Evo_Fecha <> '" + Convert.ToDateTime(E.Evo_Fecha).ToString("yyyy-MM-dd") + "' " +
                                         "ORDER BY Evo_Fecha DESC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        CXN_EVOFIB C = new CXN_EVOFIB();

                        while (Lectura_Hora.Read() == true)
                        {
                            C.Evo_Sesion = Lectura_Hora["Evo_Sesion"].ToString();
                            C.Evo_Fase = Lectura_Hora["Evo_Fase"].ToString();
                        }

                        return C;
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

        bool IEvolucionesFibromialgia.saveEvolution(CXN_EVOFIB E)
        {
            try
            {
                var datosConexion = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datosConexion["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_EVOFIB " +
                                                         "(Evo_Pac, " +
                                                         "Evo_Ase, " +
                                                         "Evo_Cia, " +
                                                         "Evo_Med, " +
                                                         "Evo_Fecha, " +
                                                         "Evo_Edad, " +
                                                         "Evo_Sesion, " +
                                                         "Evo_Tipo, " +
                                                         "Evo_Evo, " +
                                                         "Evo_Hora, " +
                                                         "Evo_Fase, " +
                                                         "Evo_Adm) " +
                                     "VALUES                  (@param1, " +
                                                              "@param2, @param3, @param4, @param5, " +
                                                              "@param6, @param7, @param8, @param9, " +
                                                              "@param10, @param11, @param12)", con);

                    cmd.Parameters.AddWithValue("@param1", E.Evo_Pac);//ok
                    cmd.Parameters.AddWithValue("@param2", E.Evo_Ase); //ok
                    cmd.Parameters.AddWithValue("@param3", E.Evo_Cia);//ok
                    cmd.Parameters.AddWithValue("@param4", E.Evo_Med);//ok
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = E.Evo_Fecha;//ok
                    cmd.Parameters.AddWithValue("@param6", E.Evo_Edad);//ok
                    cmd.Parameters.AddWithValue("@param7", E.Evo_Sesion);//ok
                    cmd.Parameters.AddWithValue("@param8", E.Evo_Tipo);//ok
                    cmd.Parameters.AddWithValue("@param9", E.Evo_Evo);//ok
                    cmd.Parameters.AddWithValue("@param10", E.Evo_Hora);//ok
                    cmd.Parameters.AddWithValue("@param11", E.Evo_Fase);//ok
                    cmd.Parameters.AddWithValue("@param12", E.Evo_Adm);//ok                        
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
