using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Domain.CXN;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MInventario : IInventario
    {
        List<Domain.CXN.CXN_INVENTARIO> IInventario.getProductbyName(string Name) 
        {
            try
            {
                var dataConection = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_INVENTARIO " +
                                   "WHERE InvItem LIKE '%" + Name + "%' " +
                                   "AND InvConvenio = '88' " +
                                   "ORDER BY InvCod ASC";

                    if (string.IsNullOrEmpty(Name))
                    {
                        Query = "SELECT * " +
                                "FROM CXN_INVENTARIO " +
                                "WHERE InvConvenio = '88' " +
                                "ORDER BY InvCod ASC";
                    }                

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<Domain.CXN.CXN_INVENTARIO> V = new List<Domain.CXN.CXN_INVENTARIO>();

                                while (Reader.Read() == true)
                                {
                                    V.Add(new Domain.CXN.CXN_INVENTARIO
                                    {
                                        InvCod = Reader["InvCod"].ToString(),
                                        InvItem = Reader["InvItem"].ToString(),
                                        InvPrecio = Convert.ToInt32(Reader["InvPrecio"])
                                    });

                                }

                                return V;
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

        Domain.CXN.CXN_INVENTARIO IInventario.getProductbyCode(string Code)
        {
            try
            {
                var dataConection = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_INVENTARIO " +
                                   "WHERE InvCod = '" + Code + "' " +
                                   "AND InvConvenio = '88'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        Domain.CXN.CXN_INVENTARIO V = new Domain.CXN.CXN_INVENTARIO
                        {
                            InvCod = Reader["InvCod"].ToString(),
                            InvItem = Reader["InvItem"].ToString(),
                            InvPrecio = Convert.ToInt32(Reader["InvPrecio"])
                        };

                        return V;
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

        (int valor, string item, string detalle) IInventario.ConsultarValor(int Ase, string Cod)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT InvPrecio, InvItem, InvDetalle " +
                                   "FROM CXN_INVENTARIO " +
                                   "WHERE InvCod = '" + Cod + "' " +
                                   "AND InvConvenio = '" + Ase + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return (Convert.ToInt32(Reader["InvPrecio"]),
                                Reader["InvItem"].ToString(),
                                Reader["InvDetalle"].ToString());
                    }
                    else
                    {
                        return (0, "", "");
                    }
                }
            }
            catch
            {
                return (0, "", "");
            }
        }

        CXN_INVENTARIO IInventario.ConsultarValor2(string Item, int Ase)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT InvPrecio, InvItem, InvDetalle, InvId, InvCod " +
                                   "FROM CXN_INVENTARIO " +
                                   "WHERE InvItem = '" + Item + "' " +
                                   "AND InvConvenio = '" + Ase + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        CXN_INVENTARIO I = new CXN_INVENTARIO
                        {
                            InvPrecio = Convert.ToInt32(Reader["InvPrecio"]),
                            InvItem = Reader["InvItem"].ToString(),
                            InvDetalle = Reader["InvDetalle"].ToString(),
                            InvCod = Reader["InvCod"].ToString(),
                            InvId = Convert.ToInt32(Reader["InvId"])
                        };

                        return I;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        List<Domain.CXN.CXN_INVENTARIO> IInventario.getAllElements(int Convenio, string Dato)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT InvCod, InvItem, InvPrecio " +
                                         "FROM CXN_INVENTARIO " +
                                         "WHERE InvItem LIKE '%" + Dato + "%' " +
                                         "AND InvConvenio = '" + Convenio + "' " +
                                         "ORDER BY InvCod ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<Domain.CXN.CXN_INVENTARIO> I = new List<Domain.CXN.CXN_INVENTARIO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            I.Add(new Domain.CXN.CXN_INVENTARIO
                            {
                                InvCod = Lectura_Hora["InvCod"].ToString(),
                                InvItem = Lectura_Hora["InvItem"].ToString(),
                                InvPrecio = Convert.ToInt32(Lectura_Hora["InvPrecio"])
                            });
                        }

                        return I;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch
            {
                return null;
            }
        }

        string IInventario.listaPrecios(int Convenio)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Lista_Precios.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);

                    SqlCommand comando = new SqlCommand("SELECT InvCod, InvItem, InvPrecio " +
                                                        "FROM  CXN_INVENTARIO " +
                                                        "WHERE InvConvenio = '" + Convenio + "'", con);

                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("CODIGO" + "," + "ITEM" + "," + "PRECIO");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read())
                    {
                        Escriba.Write(leer["InvCod"].ToString() + ",");
                        Escriba.Write(leer["InvItem"].ToString() + ",");
                        Escriba.Write(Convert.ToInt32(leer["InvPrecio"]).ToString("N0"));
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }
                    Escriba.Close();
                    return "1";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        bool IInventario.CrearProducto(Domain.CXN.CXN_INVENTARIO I)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_INVENTARIO (InvItem, " + //param1
                                                              "InvCod, " + //param2
                                                              "InvTipo, " + //param3
                                                              "InvCobro, " + //param4
                                                              "InvInvima, " + //param5
                                                              "InvUsrGraba, " + //param6
                                                              "InvPrecio, " + //param7
                                                              "InvDetalle, " + //param8
                                                              "InvConvenio, " + //param9
                                                              "InvFechaCre, " +
                                                              "InvCodBar) " + //param16
                                     "values                  (@param1, " + // Hor_Estado
                                                              "@param2, " + // Hor_Pac_Id
                                                              "@param3, " + // Hor_Pac_Bod
                                                              "@param4, " + // Hor_Pac_Tipo_Serv
                                                              "@param5, " + // Hor_Pac_Cia
                                                              "@param6, " + // Hor_Pac_Ase
                                                              "@param7, " + // Hor_Pac_Cup
                                                              "@param8, " + // Hor_Pac_UsrGraba
                                                              "@param9, " + // Hor_Imp_Age
                                                              "@param11," +
                                                              "@param12)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", I.InvItem);
                    cmd.Parameters.AddWithValue("@param2", I.InvCod);
                    cmd.Parameters.AddWithValue("@param3", I.InvTipo);
                    cmd.Parameters.AddWithValue("@param4", I.InvCobro);
                    cmd.Parameters.AddWithValue("@param5", I.InvInvima);
                    cmd.Parameters.AddWithValue("@param6", I.InvUsrGraba);
                    cmd.Parameters.AddWithValue("@param7", I.InvPrecio);
                    cmd.Parameters.AddWithValue("@param8", I.InvDetalle);
                    cmd.Parameters.AddWithValue("@param9", I.InvConvenio);
                    cmd.Parameters.Add(new SqlParameter("@param11", SqlDbType.DateTime)).Value = I.InvFechaCre;
                    cmd.Parameters.AddWithValue("@param12", I.InvCodBar);
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

        List<Domain.CXN.CXN_INVENTARIO> IInventario.getAllProducts()
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT I.InvId, I.InvItem, I.InvCod, I.InvPrecio, A.Ase_Descripcion " +
                                       "FROM CXN_INVENTARIO I " +
                                       "INNER JOIN CXN_ASEGURADORA A ON I.InvConvenio = A.Ase_Identificador " +
                                       "ORDER BY I.InvConvenio, I.InvCod ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<Domain.CXN.CXN_INVENTARIO> I = new List<Domain.CXN.CXN_INVENTARIO>();

                        while (Lectura_Hora.Read() == true)
                        {
                            I.Add(new Domain.CXN.CXN_INVENTARIO
                            {
                                InvId = Convert.ToInt32(Lectura_Hora["InvId"]),
                                InvCod = Lectura_Hora["InvCod"].ToString(),
                                InvItem = Lectura_Hora["InvItem"].ToString(),
                                InvDetalle = Lectura_Hora["Ase_Descripcion"].ToString(),
                                InvPrecio = Convert.ToInt32(Lectura_Hora["InvPrecio"])
                            });
                        }

                        return I;
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

        List<Domain.CXN.CXN_INVENTARIO> IInventario.getAllProducts(int Aseguradora)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT InvId, InvItem, InvCod, InvPrecio, InvTipo, InvDetalle " +
                                         "FROM CXN_INVENTARIO " +
                                         "WHERE InvConvenio = @Aseguradora " +
                                         "ORDER BY InvItem ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@Aseguradora", Aseguradora);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<Domain.CXN.CXN_INVENTARIO> I = new List<Domain.CXN.CXN_INVENTARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    I.Add(new Domain.CXN.CXN_INVENTARIO
                                    {
                                        InvId = Convert.ToInt32(Lectura_Hora["InvId"]),
                                        InvCod = Lectura_Hora["InvCod"].ToString(),
                                        InvItem = Lectura_Hora["InvItem"].ToString(),
                                        InvPrecio = Convert.ToInt32(Lectura_Hora["InvPrecio"]),
                                        InvTipo = Lectura_Hora["InvTipo"].ToString(),
                                        InvDetalle = Lectura_Hora["InvDetalle"].ToString()
                                    });
                                }

                                return I;
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

        bool IInventario.updateProducto(Domain.CXN.CXN_INVENTARIO I)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_INVENTARIO " +
                                      "SET InvItem = '" + I.InvItem + "', " +
                                      "InvDetalle = '" + I.InvDetalle + "', " +
                                      "InvInvima = '" + I.InvInvima + "', " +
                                      "InvPrecio = '" + I.InvPrecio + "', " +
                                      "InvTipo = '" + I.InvTipo + "', " +
                                      "InvUsrGraba = '" + I.InvUsrGraba + "', " +
                                      "InvCobro = '" + I.InvCobro + "', " +
                                      "InvCodBar = '" + I.InvCodBar + "' " +
                                      "WHERE InvId = '" + I.InvId + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        Domain.CXN.CXN_INVENTARIO IInventario.getProductForEdit(int Pos)
        {
            try
            {
                var dataConection = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT I.InvItem, I.InvDetalle, I.InvInvima, I.InvPrecio, I.InvCodBar, I.InvCobro, I.InvTipo, A.Ase_Descripcion " +
                                   "FROM CXN_INVENTARIO I " +
                                   "INNER JOIN CXN_ASEGURADORA A ON I.InvConvenio = A.Ase_Identificador " +
                                   "WHERE I.InvId = '" + Pos + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        Domain.CXN.CXN_INVENTARIO V = new Domain.CXN.CXN_INVENTARIO
                        {
                            InvItem = Reader["InvItem"].ToString(),
                            InvDetalle = Reader["InvDetalle"].ToString(),
                            InvInvima = Reader["InvInvima"].ToString(),
                            InvPrecio = Convert.ToInt32(Reader["InvPrecio"]),
                            InvCobro = Reader["InvCobro"].ToString(),
                            InvTipo = Reader["InvTipo"].ToString(),
                            InvCod = Reader["Ase_Descripcion"].ToString(),
                            InvCodBar = Reader["InvCodBar"].ToString()
                        };

                        return V;
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

        List<Domain.CXN.CXN_INVENTARIO> IInventario.getAllProductsByType(int Aseguradora, string Tipo)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT InvId, InvItem, InvCod, InvPrecio, InvTipo, InvDetalle " +
                                         "FROM CXN_INVENTARIO " +
                                         "WHERE InvConvenio = @Aseguradora " +
                                         "AND InvTipo = @Tipo " +
                                         "ORDER BY InvItem ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@Aseguradora", Aseguradora);
                        Carga_Command.Parameters.AddWithValue("@Tipo", Tipo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<Domain.CXN.CXN_INVENTARIO> I = new List<Domain.CXN.CXN_INVENTARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    I.Add(new Domain.CXN.CXN_INVENTARIO
                                    {
                                        InvId = Convert.ToInt32(Lectura_Hora["InvId"]),
                                        InvCod = Lectura_Hora["InvCod"].ToString(),
                                        InvItem = Lectura_Hora["InvItem"].ToString(),
                                        InvPrecio = Convert.ToInt32(Lectura_Hora["InvPrecio"]),
                                        InvTipo = Lectura_Hora["InvTipo"].ToString(),
                                        InvDetalle = Lectura_Hora["InvDetalle"].ToString()
                                    });
                                }

                                return I;
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
