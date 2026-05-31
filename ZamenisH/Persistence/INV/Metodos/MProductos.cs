using Domain;
using Domain.INV;
using Persistence.INV.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.INV.Metodos
{
    public class MProductos : IProductos
    {
        INV_PRODUCTOS IProductos.GetProducto(INV_PRODUCTOS P)
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
                                         "FROM INV_PRODUCTOS " +
                                         "WHERE CodigoProveedor = @param1 " +
                                         "AND Proveedor = @param2 " +
                                         "AND Prestador = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", P.CodigoProveedor);
                        Carga_Command.Parameters.AddWithValue("@param2", P.Proveedor);
                        Carga_Command.Parameters.AddWithValue("@param3", P.Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                INV_PRODUCTOS I = new INV_PRODUCTOS
                                {
                                    CodigoInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                    CodigoProveedor = Lectura_Hora["CodigoInterno"].ToString(),
                                    Nombre = Lectura_Hora["Nombre"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    Observacion = Lectura_Hora["Observacion"].ToString(),
                                    Prestador = Convert.ToInt32(Lectura_Hora["Prestador"]),
                                    Proveedor = Convert.ToInt32(Lectura_Hora["Proveedor"])
                                };

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
        INV_PRODUCTOS IProductos.GetProductobyId(int Posision)
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
                                         "FROM INV_PRODUCTOS " +
                                         "WHERE Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Posision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                INV_PRODUCTOS I = new INV_PRODUCTOS
                                {
                                    CodigoInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                    CodigoProveedor = Lectura_Hora["CodigoInterno"].ToString(),
                                    Nombre = Lectura_Hora["Nombre"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    Observacion = Lectura_Hora["Observacion"].ToString(),
                                    Prestador = Convert.ToInt32(Lectura_Hora["Prestador"]),
                                    Proveedor = Convert.ToInt32(Lectura_Hora["Proveedor"])
                                };

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
        int IProductos.CrearProducto(INV_PRODUCTOS P)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into INV_PRODUCTOS (CodigoInterno, " +
                                                          "CodigoProveedor, " +
                                                          "Nombre, " +
                                                          "Observacion, " +
                                                          "Proveedor, " +
                                                          "Prestador) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6); SELECT SCOPE_IDENTITY();", con);

                    cmd.Parameters.AddWithValue("@param1", P.CodigoInterno);
                    cmd.Parameters.AddWithValue("@param2", P.CodigoProveedor);
                    cmd.Parameters.AddWithValue("@param3", P.Nombre);
                    cmd.Parameters.AddWithValue("@param4", P.Observacion);
                    cmd.Parameters.AddWithValue("@param5", P.Proveedor);
                    cmd.Parameters.AddWithValue("@param6", P.Prestador);

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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        List<INV_PRODUCTOS> IProductos.CargarProductos(string Filtro, INV_PRODUCTOS P)
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

                    String Cargar_Hora = "";
                    
                    if (Filtro == "")
                    {
                        Cargar_Hora = "SELECT * " +
                                      "FROM INV_PRODUCTOS " +
                                      "WHERE Proveedor = @param2 " +
                                      "AND Prestador = @param3";
                    }
                    else
                    {
                        Cargar_Hora = "SELECT * " +
                                      "FROM INV_PRODUCTOS " +
                                      "WHERE Nombre LIKE '%" + Filtro + "%' " +
                                      "AND Proveedor = @param2 " +
                                      "AND Prestador = @param3";
                    }                

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param2", P.Proveedor);
                        Carga_Command.Parameters.AddWithValue("@param3", P.Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<INV_PRODUCTOS> L = new List<INV_PRODUCTOS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new INV_PRODUCTOS 
                                    {
                                        CodigoInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                        CodigoProveedor = Lectura_Hora["CodigoInterno"].ToString(),
                                        Nombre = Lectura_Hora["Nombre"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Observacion = Lectura_Hora["Observacion"].ToString(),
                                        Prestador = Convert.ToInt32(Lectura_Hora["Prestador"]),
                                        Proveedor = Convert.ToInt32(Lectura_Hora["Proveedor"])
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
        bool IProductos.ActualizarProducto(INV_PRODUCTOS P)
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

                    string Busqueda = "UPDATE INV_PRODUCTOS " +
                                      "SET " +
                                      "CodigoInterno = @param1, " +
                                      "CodigoProveedor = @param2, " +
                                      "Nombre = @param3, " +
                                      "Observacion = @param4 " +
                                      "WHERE Id = @param5";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", P.CodigoInterno);
                    Accion.Parameters.AddWithValue("@param2", P.CodigoProveedor);
                    Accion.Parameters.AddWithValue("@param3", P.Nombre);
                    Accion.Parameters.AddWithValue("@param4", P.Observacion);
                    Accion.Parameters.AddWithValue("@param5", P.Id);

                    int Guarda = Accion.ExecuteNonQuery();
                    if (Guarda > 0) { return true; }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //Reportes
        List<ReportsINV> IProductos.ReporteIngresos(DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora = "SELECT PR.Nombre, P.CodigoInterno, P.CodigoProveedor, P.Nombre AS PROD, HP.Lote, HP.Factura, HP.Cantidad, HP.Costo, HP.Usuario, C.Com_Nombre " +
                                         "FROM INV_HISTORICOPPAL HP " +
                                         "INNER JOIN INV_PRODUCTOS P ON HP.CodProducto = P.Id " +
                                         "INNER JOIN CXN_PROVEEDORES PR ON P.Proveedor = PR.Codigo " +
                                         "INNER JOIN CXN_CIA C ON P.Prestador = C.Com_Identificador " +
                                         "WHERE HP.Fecha BETWEEN @param1 AND @param2 " +
                                         "AND HP.Cantidad > '0' " +
                                         "ORDER BY PR.Nombre, P.Nombre ASC";                    

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ReportsINV> L = new List<ReportsINV>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new ReportsINV
                                    {
                                        CodeInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                        CodeProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                        Factura = Lectura_Hora["Factura"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Lote = Lectura_Hora["Lote"].ToString(),
                                        CostoUnitario = Convert.ToInt32(Lectura_Hora["Costo"]),
                                        CostoTotal = Convert.ToInt32(Lectura_Hora["Costo"]) * Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Desde = Desde,
                                        Hasta = Hasta,
                                        Prestador = Lectura_Hora["Com_Nombre"].ToString(),
                                        Proveedor = Lectura_Hora["Nombre"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        ProductoName = Lectura_Hora["PROD"].ToString(),
                                        TipoReporte = "REPORTE DE ENTRADAS"
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
        List<ReportsINV> IProductos.ReporteSalidas(DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora = "SELECT PR.Nombre, P.CodigoInterno, P.CodigoProveedor, P.Nombre AS PROD, HP.Lote, HP.Factura, S.Cantidad, HP.Costo, S.Usuario, C.Bod_Responsable " +
                                         "FROM INV_SALIDASSUB S " +
                                         "INNER JOIN INV_INVENTARIOPPAL HP ON S.CodPpal = HP.Id " +
                                         "INNER JOIN INV_PRODUCTOS P ON HP.CodProducto = P.Id " +
                                         "INNER JOIN CXN_PROVEEDORES PR ON P.Proveedor = PR.Codigo " +
                                         "INNER JOIN CXN_BODEGAS C ON S.Bodega = C.Bod_Numero " +
                                         "WHERE S.Fecha BETWEEN @param1 AND @param2 " +
                                         "AND S.Cantidad > '0' " +
                                         "ORDER BY PR.Nombre, P.Nombre ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(Desde.Date));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Hasta.Date));

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ReportsINV> L = new List<ReportsINV>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new ReportsINV
                                    {
                                        CodeInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                        CodeProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                        Factura = Lectura_Hora["Factura"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Lote = Lectura_Hora["Lote"].ToString(),
                                        CostoUnitario = Convert.ToInt32(Lectura_Hora["Costo"]),
                                        CostoTotal = Convert.ToInt32(Lectura_Hora["Costo"]) * Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Desde = Desde,
                                        Hasta = Hasta,
                                        Prestador = Lectura_Hora["Bod_Responsable"].ToString(),
                                        Proveedor = Lectura_Hora["Nombre"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        ProductoName = Lectura_Hora["PROD"].ToString(),
                                        TipoReporte = "REPORTE DE SALIDAS"
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
