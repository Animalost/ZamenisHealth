using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MAntecedentesGlobales : IAntecedentesGlobales
    {
        void IAntecedentesGlobales.InsertarAntecedente(CXN_ANTECEDENTESFAMILIARES A)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ANTECEDENTESFAMILIARES (Parentesco, " +
                                                                  "CIECod, " +
                                                                  "CieDesc, " +
                                                                  "Paciente, " +
                                                                  "Fecha, " +
                                                                  "Usuario, " +
                                                                  "Tipo) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5, " +
                                                                  "@param6, " +
                                                                  "@param7)", con);

                    cmd.Parameters.AddWithValue("@param1", A.Parentesco);
                    cmd.Parameters.AddWithValue("@param2", A.CIECod);
                    cmd.Parameters.AddWithValue("@param3", A.CieDesc);
                    cmd.Parameters.AddWithValue("@param4", A.Paciente);
                    cmd.Parameters.AddWithValue("@param5", Convert.ToDateTime(DateTime.Now.Date));
                    cmd.Parameters.AddWithValue("@param6", A.Usuario);
                    cmd.Parameters.AddWithValue("@param7", A.Tipo);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        bool IAntecedentesGlobales.BuscaAntecedente(int Paciente, string CIE10, string Parentesco)
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
                                         "FROM CXN_ANTECEDENTESFAMILIARES " +
                                         "WHERE Paciente = @param1 " +
                                         "AND CIECod = @param2 " +
                                         "AND Parentesco = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param2", CIE10);
                        Carga_Command.Parameters.AddWithValue("@param3", Parentesco);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }                                           
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return true;
            }
        }
        List<CXN_ANTECEDENTESFAMILIARES> IAntecedentesGlobales.ObtenerAntecedentes(int Paciente)
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

                    String Cargar_Hora = "SELECT * FROM CXN_ANTECEDENTESFAMILIARES WHERE Paciente = @param1";
                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_ANTECEDENTESFAMILIARES> C = new List<CXN_ANTECEDENTESFAMILIARES>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_ANTECEDENTESFAMILIARES
                                    {
                                        CIECod = Lectura_Hora["CIECod"].ToString(),
                                        CieDesc = Lectura_Hora["CieDesc"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        Paciente = Convert.ToInt32(Lectura_Hora["Paciente"]),
                                        Parentesco = Lectura_Hora["Parentesco"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        Tipo = Lectura_Hora["Tipo"].ToString()
                                    });
                                }

                                return C;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }                                         
                }
            }
            catch
            {
                return null;
            }
        }
        void IAntecedentesGlobales.EliminarAntecedente(int Id)
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

                    string Busqueda = "DELETE FROM CXN_ANTECEDENTESFAMILIARES " +
                                      "WHERE Id = '" + Id + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);            
            }
        }

    }
}
