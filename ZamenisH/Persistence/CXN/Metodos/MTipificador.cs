using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MTipificador : ITipificador
    {
        List<CXN_TIPIFICADORRAZON> ITipificador.ListaRazones()
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_TIPIFICADORRAZON " +
                                         "ORDER BY RazonLlamada ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_TIPIFICADORRAZON> L = new List<CXN_TIPIFICADORRAZON>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_TIPIFICADORRAZON
                                    {
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        RazonLlamada = Lectura_Hora["RazonLlamada"].ToString()
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
        bool ITipificador.Crea(CXN_TIPIFICADOR A)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_TIPIFICADOR (NombreLlama, " +
                                                          "TipoIdPaciente, " +
                                                          "NumIdPaciente, " +
                                                          "Celular, " +
                                                          "Email, " +
                                                          "Aseguradora, " +
                                                          "RazonLlamada, " +
                                                          "Gestion, " +
                                                          "Usuario, " +
                                                          "Hora, " +
                                                          "Fecha, " +
                                                          "Ingreso, " +
                                                          "NombrePaciente) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6, " +
                                                          "@param7, " +
                                                          "@param8, " +
                                                          "@param9, " +
                                                          "@param10, " +
                                                          "@param11, " +
                                                          "@param12, " +
                                                          "@param13)", con);

                    cmd.Parameters.AddWithValue("@param1", A.NombreLlama);
                    cmd.Parameters.AddWithValue("@param2", A.TipoIdPaciente);
                    cmd.Parameters.AddWithValue("@param3", A.NumIdPaciente);
                    cmd.Parameters.AddWithValue("@param4", A.Celular);
                    cmd.Parameters.AddWithValue("@param5", A.Email);
                    cmd.Parameters.AddWithValue("@param6", A.Aseguradora);
                    cmd.Parameters.AddWithValue("@param7", A.RazonLlamada);
                    cmd.Parameters.AddWithValue("@param8", A.Gestion);
                    cmd.Parameters.AddWithValue("@param9", A.Usuario);
                    cmd.Parameters.AddWithValue("@param10", Convert.ToDateTime(A.Hora));
                    cmd.Parameters.AddWithValue("@param11", Convert.ToDateTime(A.Fecha));
                    cmd.Parameters.AddWithValue("@param12", A.Ingreso);
                    cmd.Parameters.AddWithValue("@param13", A.NombrePaciente);

                    int c = cmd.ExecuteNonQuery();
                    if (c > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        List<CXN_TIPIFICADOR> ITipificador.Reporte(DateTime Desde, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_TIPIFICADOR " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_TIPIFICADOR.Aseguradora = CXN_ASEGURADORA.Ase_Identificador " +
                                         "WHERE Fecha BETWEEN @param1 AND @param2 " +
                                         "ORDER BY Fecha, Hora DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(new DateTime(Desde.Year, Desde.Month, Desde.Day, 00, 00, 00)));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(new DateTime(Hasta.Year, Hasta.Month, Hasta.Day, 00, 00, 00)));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_TIPIFICADOR> L = new List<CXN_TIPIFICADOR>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_TIPIFICADOR
                                    {
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        RazonLlamada = Lectura_Hora["RazonLlamada"].ToString(),
                                        Aseguradora = Convert.ToInt32(Lectura_Hora["Aseguradora"]),
                                        Celular = Lectura_Hora["Celular"].ToString(),
                                        Email = Lectura_Hora["Email"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        Gestion = Lectura_Hora["Gestion"].ToString(),
                                        Hora = Convert.ToDateTime(Lectura_Hora["Hora"]),
                                        Ingreso = Lectura_Hora["Ingreso"].ToString(),
                                        NombreLlama = Lectura_Hora["NombreLlama"].ToString(),
                                        NumIdPaciente = Lectura_Hora["NumIdPaciente"].ToString(),
                                        TipoIdPaciente = Lectura_Hora["TipoIdPaciente"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        AseguradoraName = Lectura_Hora["Ase_Descripcion"].ToString(),
                                        NombrePaciente = Lectura_Hora["NombrePaciente"].ToString(),
                                        Desde = Convert.ToDateTime(Desde),
                                        Hasta = Convert.ToDateTime(Hasta)
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
