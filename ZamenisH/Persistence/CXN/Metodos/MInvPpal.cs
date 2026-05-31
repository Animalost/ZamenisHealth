using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Persistence.CXN.Metodos
{
    public class MInvPpal : IInvPpal
    {
        List<CXN_INVPPALLISTA> IInvPpal.GetInventario(int Prestador)
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

                    String Cargar_Hora = "SELECT I.Id, L.CodigoProveedor, L.CodigoPrestador, L.Producto, P.Nombre, I.Cantidad, L.ValorUnitario " +
                                         "FROM CXN_INVPPAL I " +
                                         "INNER JOIN CXN_INVPPALLISTA L ON I.Prestador = L.Prestador " +
                                         "INNER JOIN CXN_PROVEEDORES P ON L.Proveedor = P.Codigo " +
                                         "AND I.IdListaProd = L.Id " +
                                         "WHERE I.Cantidad > 0 " +
                                         "AND I.Prestador = @param1 " +
                                         "AND L.Prestador = @param1 " +
                                         "ORDER BY L.Producto";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_INVPPALLISTA> L = new List<CXN_INVPPALLISTA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_INVPPALLISTA
                                    {
                                        CodigoProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                        CodigoPrestador = Lectura_Hora["CodigoPrestador"].ToString(),
                                        Producto = Lectura_Hora["Producto"].ToString(),
                                        Nombre = Lectura_Hora["Nombre"].ToString(),
                                        Codigo = Convert.ToInt32(Lectura_Hora["Cantidad"]), //Cantidad del Producto
                                        ValorUnitario = Convert.ToInt32(Lectura_Hora["ValorUnitario"]),
                                        Prestador = Convert.ToInt32(Lectura_Hora["ValorUnitario"]) * Convert.ToInt32(Lectura_Hora["Cantidad"]), //Valor Total
                                        Id = Convert.ToInt32(Lectura_Hora["Id"])
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

        List<CXN_INVPPALLISTA> IInvPpal.GetProducto(int Prestador, int Proveedor, string Codigo)
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

                    String Cargar_Hora = "SELECT CodigoProveedor, CodigoPrestador, Producto, ValorUnitario " +
                                         "FROM CXN_INVPPALLISTA " +
                                         "WHERE Proveedor = @param1 " +
                                         "AND CodigoProveedor = @param2 " +
                                         "AND Prestador = @param3 " +
                                         "ORDER BY ValorUnitario ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Proveedor);
                        Carga_Command.Parameters.AddWithValue("@param2", Codigo);
                        Carga_Command.Parameters.AddWithValue("@param3", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_INVPPALLISTA> L = new List<CXN_INVPPALLISTA>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_INVPPALLISTA
                                    {
                                        CodigoProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                        CodigoPrestador = Lectura_Hora["CodigoPrestador"].ToString(),
                                        Producto = Lectura_Hora["Producto"].ToString(),
                                        ValorUnitario = Convert.ToInt32(Lectura_Hora["ValorUnitario"])                                       
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

        CXN_INVPPALLISTA IInvPpal.GetProdByPos(int Posision)
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

                    String Cargar_Hora = "SELECT I.Id, L.CodigoProveedor, L.CodigoPrestador, L.Producto, P.Nombre, I.Cantidad, L.ValorUnitario " +
                                         "FROM CXN_INVPPAL I " +
                                         "INNER JOIN CXN_INVPPALLISTA L ON I.Prestador = L.Prestador " +
                                         "AND L.Id = I.IdListaProd " + 
                                         "INNER JOIN CXN_PROVEEDORES P ON L.Proveedor = P.Codigo " +
                                         "WHERE I.Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Posision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_INVPPALLISTA L = new CXN_INVPPALLISTA
                                {
                                    CodigoProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                    CodigoPrestador = Lectura_Hora["CodigoPrestador"].ToString(),
                                    Producto = Lectura_Hora["Producto"].ToString(),
                                    Nombre = Lectura_Hora["Nombre"].ToString(),
                                    Codigo = Convert.ToInt32(Lectura_Hora["Cantidad"]), //cantidad
                                    ValorUnitario = Convert.ToInt32(Lectura_Hora["ValorUnitario"])
                                };

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

        int IInvPpal.GetPosProducto(int Prestador, int Proveedor, string Codigo, int Valor)
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

                    String Cargar_Hora = "SELECT Id, CodigoProveedor, Proveedor, Prestador, ValorUnitario " +
                                         "FROM CXN_INVPPALLISTA " +
                                         "WHERE Proveedor = @param1 " +
                                         "AND CodigoProveedor = @param2 " +
                                         "AND Prestador = @param3 " +
                                         "ORDER BY ValorUnitario ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Proveedor);
                        Carga_Command.Parameters.AddWithValue("@param2", Codigo);
                        Carga_Command.Parameters.AddWithValue("@param3", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                int Res = 0;

                                while (Lectura_Hora.Read() == true)
                                {
                                    int ValorTemp = Convert.ToInt32(Lectura_Hora["ValorUnitario"]);

                                    if (Convert.ToInt32(Lectura_Hora["Proveedor"]) == Proveedor &&
                                        Convert.ToInt32(Lectura_Hora["Prestador"]) == Prestador &&
                                        Lectura_Hora["CodigoProveedor"].ToString() == Codigo &&
                                        ValorTemp == Valor)
                                    {
                                        Res = Convert.ToInt32(Lectura_Hora["Id"]);                                        
                                    }
                                }

                                return Res;
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

        int IInvPpal.InsertarProducto(CXN_INVPPALLISTA L)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_INVPPALLISTA (Producto, " + 
                                                      "Proveedor, " + 
                                                      "CodigoProveedor, " + 
                                                      "ValorUnitario, " + 
                                                      "Fecha, " + 
                                                      "CodigoPrestador, " + 
                                                      "Prestador) " + 
                         "values                  (@param1, " + 
                                                  "@param2, " + 
                                                  "@param3, " + 
                                                  "@param4, " + 
                                                  "@param5, " + 
                                                  "@param6, " + 
                                                  "@param7); SELECT SCOPE_IDENTITY();", con);

                    cmd.Parameters.AddWithValue("@param1", L.Producto);
                    cmd.Parameters.AddWithValue("@param2", L.Proveedor);
                    cmd.Parameters.AddWithValue("@param3", L.CodigoProveedor);
                    cmd.Parameters.AddWithValue("@param4", L.ValorUnitario);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = DateTime.Now;
                    cmd.Parameters.AddWithValue("@param6", L.CodigoPrestador);
                    cmd.Parameters.AddWithValue("@param7", L.Prestador);                   

                    object _primaryKey = cmd.ExecuteScalar();
                    if (Convert.ToInt32(_primaryKey) >= 1)
                    {
                        return Convert.ToInt32(_primaryKey);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return 0;
            }
        }

        int IInvPpal.InsertarProductoEnInventario(CXN_INVPPAL L)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_INVPPAL (IdListaProd, " +
                                                      "Cantidad, " +
                                                      "Prestador) " +
                         "values                  (@param1, " +
                                                  "@param2, " +
                                                  "@param3); SELECT SCOPE_IDENTITY();", con);

                    cmd.Parameters.AddWithValue("@param1", L.IdListaProd);
                    cmd.Parameters.AddWithValue("@param2", L.Cantidad);
                    cmd.Parameters.AddWithValue("@param3", L.Prestador);

                    object _primaryKey = cmd.ExecuteScalar();
                    if (Convert.ToInt32(_primaryKey) >= 1)
                    {
                        return Convert.ToInt32(_primaryKey);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return 0;
            }
        }
        int IInvPpal.GetPosInInventario(int CodePosision, int Prestador)
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

                    String Cargar_Hora = "SELECT Cantidad " +
                                         "FROM CXN_INVPPAL " +
                                         "WHERE IdListaProd = @param1 " +
                                         "AND Prestador = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", CodePosision);
                        Carga_Command.Parameters.AddWithValue("@param2", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {

                                return Convert.ToInt32(Lectura_Hora["Cantidad"]);
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

        int IInvPpal.UpdateCantidad(CXN_INVPPAL I)
        {
            Dictionary<string, string> getData = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getData["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now;

                SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_INVPPAL " +
                                  "SET " +
                                  "Cantidad = @param1 " +
                                  "WHERE IdListaProd = @param2 " +
                                  "AND Prestador = @param3", con);

                Busqueda.Parameters.AddWithValue("@param1", I.Cantidad);
                Busqueda.Parameters.AddWithValue("@param2", I.IdListaProd);
                Busqueda.Parameters.AddWithValue("@param3", I.Prestador);
                int h = Busqueda.ExecuteNonQuery();
                return h;
            }
        }
        void IInvPpal.InsertarCargo(CXN_INVPPALCARGOS C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_INVPPALCARGOS (Fecha, " +
                                                      "IdListProd, " +
                                                      "Usuario, " +
                                                      "Cantidad, " +
                                                      "Clase, " +
                                                      "Prestador, " +
                                                      "Observacion) " +
                         "values                  (@param1, " +
                                                  "@param2, " +
                                                  "@param3, " +
                                                  "@param4, " +
                                                  "@param5, " +
                                                  "@param6, " +
                                                  "@param7); SELECT SCOPE_IDENTITY();", con);

                    cmd.Parameters.Add(new SqlParameter("@param1", SqlDbType.DateTime)).Value = DateTime.Now;                    
                    cmd.Parameters.AddWithValue("@param2", C.IdListProd);
                    cmd.Parameters.AddWithValue("@param3", C.Usuario);
                    cmd.Parameters.AddWithValue("@param4", C.Cantidad);
                    cmd.Parameters.AddWithValue("@param5", C.Clase);
                    cmd.Parameters.AddWithValue("@param6", C.Prestador);
                    cmd.Parameters.AddWithValue("@param7", C.Observacion);
                    cmd.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        List<CXN_INVPPALCARGOS> IInvPpal.GetCargos(int Prestador, string PrestadorName)
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

                    DateTime Hoy = DateTime.Now.Date;
                    DateTime Final = Hoy.AddDays(-60);
                    Hoy = Hoy.AddDays(1);

                    String Cargar_Hora = "SELECT C.Fecha, C.Usuario, C.Cantidad, C.Clase, C.Observacion, " +
                                         "I.Producto, I.CodigoProveedor, I.CodigoPrestador, I.Proveedor, I.ValorUnitario, " +
                                         "P.Nombre " +
                                         "FROM CXN_INVPPALCARGOS C " +
                                         "INNER JOIN CXN_INVPPALLISTA I ON C.Prestador = I.Prestador " +
                                         "AND C.IdListProd = I.Id " +
                                         "INNER JOIN CXN_PROVEEDORES P ON I.Proveedor = P.Codigo " +
                                         "WHERE C.Prestador = @param1 " +
                                         "AND I.Prestador = @param1 " +
                                         "AND C.Fecha BETWEEN @param2 AND @param3 " + 
                                         "ORDER BY C.Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);
                        Carga_Command.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Final.Date;
                        Carga_Command.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = Hoy.Date;

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_INVPPALCARGOS> L = new List<CXN_INVPPALCARGOS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_INVPPALCARGOS
                                    {
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Clase = Lectura_Hora["Clase"].ToString(),
                                        Producto = Lectura_Hora["Producto"].ToString(), 
                                        CodigoProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                        CodigoPrestador = Lectura_Hora["CodigoPrestador"].ToString(), 
                                        Nombre = Lectura_Hora["Nombre"].ToString(), //Nombre Proveedor
                                        ValorUnitario = Convert.ToInt32(Lectura_Hora["ValorUnitario"]),
                                        Responsable = PrestadorName,
                                        Observacion = Lectura_Hora["Observacion"].ToString()
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
