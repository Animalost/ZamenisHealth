using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;

namespace Persistence.CXN.Metodos
{
    public class MCondiciones : ICondiciones
    {
        List<string> ICondiciones.getCondiciones(int Pac)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT DISTINCT Condicion " +
                                   "FROM CXN_CONDICIONES " +
                                   "WHERE Paciente = @param1 " +
                                   "AND Habilita = @param2";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Pac);
                        Commando.Parameters.AddWithValue("@param2", "A");

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<string> list = new List<string>();

                                while (Reader.Read() == true)
                                {
                                    list.Add(Reader["Condicion"].ToString());
                                }

                                return list;
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
        List<CXN_CONDICIONES> ICondiciones.getCondiciones(int Pac, string Condicion)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_CONDICIONES " +
                                   "WHERE Paciente = @param1 " +
                                   "AND Condicion = @param3";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Pac);
                        Commando.Parameters.AddWithValue("@param3", Condicion);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_CONDICIONES> list = new List<CXN_CONDICIONES>();

                                while (Reader.Read() == true)
                                {
                                    list.Add(new CXN_CONDICIONES 
                                    { 
                                        Condicion = Reader["Condicion"].ToString(),
                                        Detalle = Reader["Detalle"].ToString(),
                                        Fecha =  Convert.ToDateTime(Reader["Fecha"]),
                                        Habilita = Reader["Habilita"].ToString(),
                                        Paciente = Convert.ToInt32(Reader["Paciente"]),
                                        Usuario = Reader["Usuario"].ToString(),
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        CodigoFHIR = Reader["CodigoFHIR"].ToString()
                                    });
                                }

                                return list;
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
        string ICondiciones.getCondicionesForPrint(int Pac, DateTime Fecha)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_CONDICIONES " +
                                   "WHERE Paciente = @param1 " +
                                   "AND Habilita = @param2 " +
                                   "AND Fecha <= @param3 " +
                                   "ORDER BY Fecha DESC";
                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Pac);
                        Commando.Parameters.AddWithValue("@param2", "A");
                        Commando.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = Fecha;

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                string res = "";

                                while (Reader.Read() == true)
                                {
                                    res = res + "Categoria: " + Reader["Condicion"].ToString() + "\n\r" +
                                        Reader["Detalle"].ToString() + "\n\r" +
                                        "Fecha: " + Convert.ToDateTime(Reader["Fecha"]).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "\n\r" +
                                        "Usuario: " + Reader["Usuario"].ToString() + "\n\r\n\r";                                    
                                }

                                return res;
                            }
                            else
                            {
                                return "Ninguna";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "Ninguna";
            }
        }
        void ICondiciones.updateHabilita(CXN_CONDICIONES c)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE CXN_CONDICIONES SET Habilita = @Habilita, Excluye = @Excluye, FechaExcluye = @FechaExcluye WHERE Id = @Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Habilita", c.Habilita);
                        cmd.Parameters.AddWithValue("@Excluye", (object)c.Excluye ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaExcluye", (object)c.FechaExcluye ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id", c.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void ICondiciones.createCondiciones(CXN_CONDICIONES c)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Codefhir = "";

                    if (!string.IsNullOrEmpty(c.CodigoFHIR))
                    {
                        Codefhir = c.CodigoFHIR;
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_CONDICIONES (Condicion, " + //param1
                                                                              "Paciente, " + //param2
                                                                              "Usuario, " + //param3
                                                                              "Detalle, " + //param4
                                                                              "Fecha, " + //param5
                                                                              "Habilita, " +
                                                                              "CodigoFHIR) " + //param16
                                                     "values                  (@param1, " + // Hor_Estado
                                                                              "@param2, " + // Hor_Pac_Id
                                                                              "@param3, " + // Hor_Pac_Bod
                                                                              "@param4, " + // Hor_Pac_Tipo_Serv
                                                                              "@param5, " + // Hor_Pac_Cia
                                                                              "@param6, " +
                                                                              "@param7)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", c.Condicion);
                    cmd.Parameters.AddWithValue("@param2", c.Paciente);
                    cmd.Parameters.AddWithValue("@param3", c.Usuario);
                    cmd.Parameters.AddWithValue("@param4", c.Detalle);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = c.Fecha;
                    cmd.Parameters.AddWithValue("@param6", c.Habilita);
                    cmd.Parameters.AddWithValue("@param7", Codefhir);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<CXN_CONDICIONES> ICondiciones.getCondicionesFHIR(int Pac)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_CONDICIONES " +
                                   "WHERE Paciente = @param1 " +
                                   "AND CodigoFHIR = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Pac);
                        Commando.Parameters.AddWithValue("@param2", "XX");

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_CONDICIONES> list = new List<CXN_CONDICIONES>();

                                while (Reader.Read() == true)
                                {
                                    list.Add(new CXN_CONDICIONES
                                    {
                                        Condicion = Reader["Condicion"].ToString(),
                                        Detalle = Reader["Detalle"].ToString(),
                                        Fecha = Convert.ToDateTime(Reader["Fecha"]),
                                        Habilita = Reader["Habilita"].ToString(),
                                        Paciente = Convert.ToInt32(Reader["Paciente"]),
                                        Usuario = Reader["Usuario"].ToString(),
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        CodigoFHIR = Reader["CodigoFHIR"].ToString()
                                    });
                                }

                                return list;
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
        void ICondiciones.createCondicionesInSite(CXN_ALERGIASINSITE c)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ALERGIASINSITE (Paciente, " + //param1
                                                                              "Admision, " + //param2
                                                                              "Alergia, " + //param3
                                                                              "Observacion, " +
                                                                              "CodeFHIR) " + //param16
                                                     "values                  (@param1, " + // Hor_Estado
                                                                              "@param2, " + // Hor_Pac_Id
                                                                              "@param3, " + // Hor_Pac_Bod
                                                                              "@param4, " +
                                                                              "@param5)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", c.Paciente);
                    cmd.Parameters.AddWithValue("@param2", c.Admision);
                    cmd.Parameters.AddWithValue("@param3", c.Alergia);
                    cmd.Parameters.AddWithValue("@param4", c.Observacion);
                    cmd.Parameters.AddWithValue("@param5", c.CodeFHIR);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<CXN_ALERGIASINSITE> ICondiciones.getCondicionesINSITE(int Admision)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_ALERGIASINSITE " +
                                   "WHERE Admision = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_ALERGIASINSITE> list = new List<CXN_ALERGIASINSITE>();

                                while (Reader.Read() == true)
                                {
                                    list.Add(new CXN_ALERGIASINSITE
                                    {
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Admision = Convert.ToInt32(Reader["Admision"]),
                                        Alergia = Reader["Alergia"].ToString(),
                                        CodeFHIR = Reader["CodeFHIR"].ToString(),
                                        Observacion = Reader["Observacion"].ToString(),
                                        Paciente = Convert.ToInt32(Reader["Paciente"])
                                    });
                                }

                                return list;
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
        void ICondiciones.deleteINSITE(int Id)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM CXN_ALERGIASINSITE WHERE Id = @Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Id", Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
