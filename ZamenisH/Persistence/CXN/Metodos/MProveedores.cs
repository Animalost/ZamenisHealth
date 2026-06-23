using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Persistence.CXN.Metodos
{
    public class MProveedores : IProveedores
    {
        List<CXN_PROVEEDORES> IProveedores.getListadoProvs() 
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

                    String Query = "SELECT *  " +
                                   "FROM CXN_PROVEEDORES " +
                                   "ORDER BY Nombre ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_PROVEEDORES> P = new List<CXN_PROVEEDORES>();

                                while (Reader.Read() == true)
                                {
                                    P.Add(new CXN_PROVEEDORES
                                    {
                                        Nombre = Reader["Nombre"].ToString(),
                                        Codigo = Convert.ToInt32(Reader["Codigo"])
                                    });
                                }

                                return P;
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
        void IProveedores.deleteProds(int Num_Ord)
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

                    string Busqueda2 = "DELETE FROM CXN_PEDIDOS WHERE Num_Pedido = '" + Num_Ord + "' AND Estado = 'P'";
                    SqlCommand Accion2 = new SqlCommand(Busqueda2, con);
                    int Guarda2;
                    Guarda2 = Accion2.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        bool IProveedores.AddProducto(CXN_PEDIDOS P)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PEDIDOS (Cod_Pro, " + //param1
                                                         "Cod_Ins, " + //param2
                                                         "Item, " + //param3
                                                         "FECHA, " + //param4
                                                         "Num_Pedido, " + //param5
                                                         "Usuario, " + //param6
                                                         "Estado, " + //param6
                                                         "Cod_ItemI, " + //param6
                                                         "Cantidad, " + //param6
                                                         "Cod_Item_Pro, " + //param6
                                                         "Tipo, " +
                                                         "Observacion) " + //param16
                                "values                  (@param1, " + // Hor_Estado
                                                         "@param2, " + // Hor_Pac_Id
                                                         "@param3, " + // Hor_Pac_Bod
                                                         "@param4, " + // Hor_Pac_Tipo_Serv
                                                         "@param5, " + // Hor_Pac_Cia
                                                         "@param6, " + // Hor_Pac_Ase
                                                         "@param7, " + // Hor_Pac_Ase
                                                         "@param8, " + // Hor_Pac_Ase
                                                         "@param9, " + // Hor_Pac_Ase
                                                         "@param10, " + // Hor_Pac_Ase
                                                         "@param11, " +
                                                         "@param12)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", P.Cod_Pro);
                    cmd.Parameters.AddWithValue("@param2", P.Cod_Ins);
                    cmd.Parameters.AddWithValue("@param3", P.Item);
                    cmd.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = P.Fecha;
                    cmd.Parameters.AddWithValue("@param5", P.Num_Pedido);
                    cmd.Parameters.AddWithValue("@param6", P.Usuario);
                    cmd.Parameters.AddWithValue("@param7", P.Estado);
                    cmd.Parameters.AddWithValue("@param8", P.Cod_ItemI);
                    cmd.Parameters.AddWithValue("@param9", P.Cantidad);
                    cmd.Parameters.AddWithValue("@param10", P.Cod_Item_Pro);
                    cmd.Parameters.AddWithValue("@param11", P.Tipo);
                    cmd.Parameters.AddWithValue("@param12", P.Observacion);
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
        List<CXN_PEDIDOS> IProveedores.getProdAdded(int Orden)
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

                    String Cargar_Hora2 = "SELECT * " +
                                          "FROM CXN_PEDIDOS " +
                                          "WHERE Num_Pedido = '" + Orden + "' " +
                                          "AND Estado = 'P'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<CXN_PEDIDOS> P = new List<CXN_PEDIDOS>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            P.Add(new CXN_PEDIDOS
                            {
                                Id = Convert.ToInt32(Lectura_Hora2["ID"]),
                                Cod_ItemI = Lectura_Hora2["Cod_ItemI"].ToString(),
                                Item = Lectura_Hora2["Item"].ToString(),
                                Cantidad = Convert.ToInt32(Lectura_Hora2["Cantidad"])
                            });
                        }

                        return P;
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
        bool IProveedores.InsertarPedido(CXN_PEDIDOSF F)
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

                    changeEstadoProds(Convert.ToInt32(F.Num_Orden));

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PEDIDOSF (Codigo_Prest, " + //param1
                                                           "Codigo_Provee, " + //param2
                                                           "Num_Orden, " + //param3
                                                           "FECHA, " + //param4
                                                           "Estado, " + //param5
                                                           "ObservacionG) " + //param16
                                  "values                  (@param1, " + // Hor_Estado
                                                           "@param2, " + // Hor_Pac_Id
                                                           "@param3, " + // Hor_Pac_Bod
                                                           "@param4, " + // Hor_Pac_Tipo_Serv
                                                           "@param5, " + // Hor_Pac_Cia
                                                           "@param6)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", F.Codigo_Prest);
                    cmd.Parameters.AddWithValue("@param2", F.Codigo_Provee);
                    cmd.Parameters.AddWithValue("@param3", F.Num_Orden);
                    cmd.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = F.Fecha;
                    cmd.Parameters.AddWithValue("@param5", F.Estado);
                    cmd.Parameters.AddWithValue("@param6", F.ObservacionG);
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
        void changeEstadoProds(int Num_Ord)
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

                    string Busqueda = "UPDATE CXN_PEDIDOS SET " +
                                     "Estado = 'G' " +
                                     "WHERE Num_Pedido = '" + Num_Ord + "' " +
                                     "AND Estado = 'P'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        Inventario_Proveedores IProveedores.getProducto(string Cod_Pro, int Id_Pro)
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

                    String Cargar_Hora2 = "SELECT * " +
                                          "FROM Inventario_Proveedores " +
                                          "WHERE Codigo_Pro = '" + Cod_Pro + "' " +
                                          "AND Identificador_Pro = '" + Id_Pro + "'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
                    {
                        Inventario_Proveedores P = new Inventario_Proveedores
                        {
                            Item = Lectura_Hora2["Item"].ToString(),
                            Cod_institucion = Lectura_Hora2["Cod_Institucion"].ToString(),
                            Tipo = Lectura_Hora2["Tipo"].ToString()
                        };

                        return P;
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
        bool IProveedores.deleteProdFromOrder(int Num_Orden)
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

                    string Busqueda2 = "DELETE FROM CXN_PEDIDOS WHERE Id = '" + Num_Orden + "'";
                    SqlCommand Accion2 = new SqlCommand(Busqueda2, con);
                    int Guarda2;
                    Guarda2 = Accion2.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        int IProveedores.getCodProvbyName(string name)
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

                    String Cargar_Hora = "SELECT Codigo " +
                                         "FROM CXN_PROVEEDORES " +
                                         "WHERE Nombre = '" + name + "'";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return Convert.ToInt32(Lectura_Hora["Codigo"]);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        string IProveedores.getNameProvbCode(int Code)
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

                    String Cargar_Hora = "SELECT Nombre " +
                                         "FROM CXN_PROVEEDORES " +
                                         "WHERE Codigo = '" + Code + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Lectura_Hora["Nombre"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }
        CXN_PROVEEDORES IProveedores.getProvByCode(int Code)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_PROVEEDORES " +
                                         "WHERE Codigo = '" + Code + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        CXN_PROVEEDORES P = new CXN_PROVEEDORES
                        {
                            Nombre = Lectura_Hora["Nombre"].ToString(),
                            Identificacion = Lectura_Hora["Identificacion"].ToString(),
                            Telefono = Lectura_Hora["Telefono"].ToString(),
                            Responsable = Lectura_Hora["Responsable"].ToString(),
                            Email = Lectura_Hora["Email"].ToString(),
                            Direccion = Lectura_Hora["Direccion"].ToString()
                        };

                        return P;
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
        bool IProveedores.updateProveedor(CXN_PROVEEDORES P)
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

                    string Busqueda = "UPDATE CXN_PROVEEDORES " +
                                          "SET Nombre = '" + P.Nombre + "', " +
                                          "Identificacion = '" + P.Identificacion + "', " +
                                          "Telefono = '" + P.Telefono + "', " +
                                          "Direccion = '" + P.Direccion + "', " +
                                          "Email = '" + P.Email + "', " +
                                          "Responsable = '" + P.Responsable + "' " +
                                          "WHERE Codigo = '" + P.Codigo + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    if (Guarda > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IProveedores.createProveedor(CXN_PROVEEDORES P)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PROVEEDORES (Nombre, " + //param1
                                                                               "Identificacion, " + //param2
                                                                               "Telefono, " + //param3
                                                                               "Direccion, " + //param4
                                                                               "Codigo, " + //param5
                                                                               "Email, " + //param6
                                                                               "Responsable) " + //param16
                                "values                  (@param1, " + // Hor_Estado
                                                         "@param2, " + // Hor_Pac_Id
                                                         "@param3, " + // Hor_Pac_Bod
                                                         "@param4, " + // Hor_Pac_Tipo_Serv
                                                         "@param5, " + // Hor_Pac_Cia
                                                         "@param6, " + // Hor_Pac_Ase
                                                         "@param7)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", P.Nombre);
                    cmd.Parameters.AddWithValue("@param2", P.Identificacion);
                    cmd.Parameters.AddWithValue("@param3", P.Telefono);
                    cmd.Parameters.AddWithValue("@param4", P.Direccion);
                    cmd.Parameters.AddWithValue("@param5", P.Codigo);
                    cmd.Parameters.AddWithValue("@param6", P.Email);
                    cmd.Parameters.AddWithValue("@param7", P.Responsable);
                    int c = cmd.ExecuteNonQuery();
                    if (c > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CXN_PEDIDOSF> IProveedores.getPedidos(DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora2 = "SELECT Fecha, Nombre, Com_Nombre, Num_Orden, Estado " +
                                           "FROM CXN_PEDIDOSF " +
                                           "INNER JOIN CXN_CIA ON CXN_PEDIDOSF.Codigo_Prest = CXN_CIA.Com_Identificador " +
                                           "INNER JOIN CXN_PROVEEDORES ON CXN_PEDIDOSF.Codigo_Provee = CXN_PROVEEDORES.Codigo " +
                                           "WHERE Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "' " +
                                           "ORDER BY CXN_PEDIDOSF.Num_Orden ASC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<CXN_PEDIDOSF> P = new List<CXN_PEDIDOSF>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            P.Add(new CXN_PEDIDOSF
                            {
                                Fecha = Convert.ToDateTime(Lectura_Hora2["Fecha"]),
                                ObservacionG = Lectura_Hora2["Nombre"].ToString(),
                                Codigo_Prest = Lectura_Hora2["Com_Nombre"].ToString(),
                                Num_Orden = Lectura_Hora2["Num_Orden"].ToString(),
                                Estado = Lectura_Hora2["Estado"].ToString()
                            });
                        }

                        return P;
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
        List<Inventario_Proveedores> IProveedores.getInventario()
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

                    String Query = "SELECT * " +
                                   "FROM Inventario_Proveedores " +
                                   "ORDER BY Identificador_Pro ASC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<Inventario_Proveedores> P = new List<Inventario_Proveedores>();

                        while (Reader.Read() == true)
                        {
                            P.Add(new Inventario_Proveedores
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                Codigo_Pro = Reader["Codigo_Pro"].ToString(),
                                Item = Reader["Item"].ToString(),
                                Cod_institucion = Reader["Cod_Institucion"].ToString(),
                                Valor_SinIva = Reader["Valor_SinIva"].ToString(),
                                Identificador_Pro = Convert.ToInt32(Reader["Identificador_Pro"]),
                                Tipo = Reader["Tipo"].ToString()
                            });
                        }

                        return P;
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
        Inventario_Proveedores IProveedores.getProdById(int Id)
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

                    String Query = "SELECT * " +
                                   "FROM Inventario_Proveedores " +
                                   "WHERE Id = '" + Id + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        Inventario_Proveedores P = new Inventario_Proveedores
                        {
                            Id = Convert.ToInt32(Reader["Id"]),
                            Codigo_Pro = Reader["Codigo_Pro"].ToString(),
                            Item = Reader["Item"].ToString(),
                            Cod_institucion = Reader["Cod_Institucion"].ToString(),
                            Valor_SinIva = Convert.ToInt32(Reader["Valor_SinIva"]).ToString(),
                            Identificador_Pro = Convert.ToInt32(Reader["Identificador_Pro"]),
                            Tipo = Reader["Tipo"].ToString()
                        };

                        return P;
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
        bool IProveedores.updateInventario(Inventario_Proveedores P)
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

                    string Busqueda = "UPDATE Inventario_Proveedores " +
                                         "SET Codigo_Pro = '" + P.Codigo_Pro + "', " +
                                         "Item = '" + P.Item + "', " +
                                         "Cod_Institucion = '" + P.Cod_institucion + "', " +
                                         "Valor_SinIva = '" + P.Valor_SinIva + "', " +
                                         "Tipo = '" + P.Tipo + "' " +
                                         "WHERE Id = '" + P.Id + "'";
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
        bool IProveedores.createInventario(Inventario_Proveedores P)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into Inventario_Proveedores (Codigo_Pro, " + //param1
                                                                                "Item, " + //param2
                                                                                "Cod_Institucion, " + //param3
                                                                                "Valor_SinIva, " + //param4
                                                                                "Identificador_Pro, " + //param5
                                                                                "Tipo) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", P.Codigo_Pro);
                    cmd.Parameters.AddWithValue("@param2", P.Item);
                    cmd.Parameters.AddWithValue("@param3", P.Cod_institucion);
                    cmd.Parameters.AddWithValue("@param4", P.Valor_SinIva);
                    cmd.Parameters.AddWithValue("@param5", P.Identificador_Pro);
                    cmd.Parameters.AddWithValue("@param6", P.Tipo);
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
        bool IProveedores.updateInventario2(Inventario_Proveedores P)
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

                    string Busqueda = "UPDATE Inventario_Proveedores SET " +
                                      "Item = '" + P.Item + "', " +
                                      "Cod_Institucion = '" + P.Cod_institucion + "', " +
                                      "Valor_SinIva = '" + P.Valor_SinIva + "', " +
                                      "Tipo = '" + P.Tipo + "' " +
                                      "WHERE Id = '" + P.Id + "'";
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
    }
}
