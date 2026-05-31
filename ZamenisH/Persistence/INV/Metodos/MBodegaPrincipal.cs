using Domain;
using Domain.CXN;
using Domain.INV;
using Persistence.INV.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.INV.Metodos
{
    public class MBodegaPrincipal : IBodegaPrincipal
    {
        List<CXN_BODEGAS> IBodegaPrincipal.LoadConsultorios()
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
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Estado = @param1 " +
                                         "ORDER BY Bod_Responsable ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", "A");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_BODEGAS> In = new List<CXN_BODEGAS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    In.Add(new CXN_BODEGAS 
                                    {
                                        Bod_Numero = Convert.ToInt32(Lectura_Hora["Bod_Numero"]),
                                        Bod_Responsable = Lectura_Hora["Bod_Responsable"].ToString()
                                    });                                                                                                                    
                                }

                                return In;
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
        INV_INVENTARIOPPAL IBodegaPrincipal.GetCantidad(INV_INVENTARIOPPAL I)
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
                                         "FROM INV_INVENTARIOPPAL " +
                                         "WHERE CodProducto = @param1 " +
                                         "AND Lote = @param2 " +
                                         "AND Factura = @param3 " +
                                         "AND Costo = @param4";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", I.CodProducto);
                        Carga_Command.Parameters.AddWithValue("@param2", I.Lote);
                        Carga_Command.Parameters.AddWithValue("@param3", I.Factura);
                        Carga_Command.Parameters.AddWithValue("@param4", I.Costo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                INV_INVENTARIOPPAL In = new INV_INVENTARIOPPAL
                                {
                                    CodProducto = Convert.ToInt32(Lectura_Hora["CodProducto"]),
                                    Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                    IVA = Convert.ToInt32(Lectura_Hora["IVA"]),
                                    Costo = Convert.ToInt32(Lectura_Hora["Costo"]),
                                    Factura = Lectura_Hora["Factura"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                    Lote = Lectura_Hora["Lote"].ToString()
                                };

                                return In;
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
        bool IBodegaPrincipal.ActualizarCantidad(INV_INVENTARIOPPAL P, string User, bool Historico)
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

                    string Busqueda = "UPDATE INV_INVENTARIOPPAL " +
                                      "SET " +
                                      "Cantidad = @param1 " +
                                      "WHERE Id = @param2";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", P.Cantidad);
                    Accion.Parameters.AddWithValue("@param2", P.Id);

                    int Guarda = Accion.ExecuteNonQuery();
                    if (Guarda > 0) 
                    {
                        if (Historico == true)
                        {
                            INV_HISTORICOPPAL hP = new INV_HISTORICOPPAL
                            {
                                Cantidad = P.Cantidad,
                                CodProducto = P.CodProducto,
                                Costo = P.Costo,
                                Factura = P.Factura,
                                IVA = P.IVA,
                                Lote = P.Lote,
                                Fecha = DateTime.Now,
                                Usuario = User
                            };

                            IngresarHistorico(hP);
                        }
                       
                        return true; 
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        bool IBodegaPrincipal.IngresarNuevo(INV_INVENTARIOPPAL H, string User)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into INV_INVENTARIOPPAL (Lote, " +
                                                          "Factura, " +
                                                          "Cantidad, " +
                                                          "Costo, " +
                                                          "CodProducto, " +
                                                          "IVA) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6)", con);

                    cmd.Parameters.AddWithValue("@param1", H.Lote);
                    cmd.Parameters.AddWithValue("@param2", H.Factura);
                    cmd.Parameters.AddWithValue("@param3", H.Cantidad);
                    cmd.Parameters.AddWithValue("@param4", H.Costo);
                    cmd.Parameters.AddWithValue("@param5", H.CodProducto);
                    cmd.Parameters.AddWithValue("@param6", H.IVA);

                    int c = cmd.ExecuteNonQuery();
                    if (c > 0)
                    {
                        INV_HISTORICOPPAL hP = new INV_HISTORICOPPAL
                        {
                            Cantidad = H.Cantidad,
                            CodProducto = H.CodProducto,
                            Costo = H.Costo,
                            Factura = H.Factura,
                            IVA = H.IVA,
                            Lote = H.Lote,
                            Fecha = DateTime.Now,
                            Usuario = User
                        };

                        IngresarHistorico(hP);

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        void IngresarHistorico(INV_HISTORICOPPAL H)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into INV_HISTORICOPPAL (Lote, " +
                                                          "Factura, " +
                                                          "Cantidad, " +
                                                          "Costo, " +
                                                          "CodProducto, " +
                                                          "IVA, " +
                                                          "Fecha, " +
                                                          "Usuario) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6, " +
                                                          "@param7, " +
                                                          "@param8)", con);

                    cmd.Parameters.AddWithValue("@param1", H.Lote);
                    cmd.Parameters.AddWithValue("@param2", H.Factura);
                    cmd.Parameters.AddWithValue("@param3", H.Cantidad);
                    cmd.Parameters.AddWithValue("@param4", H.Costo);
                    cmd.Parameters.AddWithValue("@param5", H.CodProducto);
                    cmd.Parameters.AddWithValue("@param6", H.IVA);
                    cmd.Parameters.AddWithValue("@param7", Convert.ToDateTime(H.Fecha));
                    cmd.Parameters.AddWithValue("@param8", H.Usuario);
                    int c = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<INV_HISTORICOPPAL> IBodegaPrincipal.GetInventary(INV_PRODUCTOS I)
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
                    
                    if (I.Nombre == "")
                    {
                        Cargar_Hora = "SELECT I.Nombre, IP.Lote, IP.Factura, IP.Cantidad, IP.Costo, IP.Id, I.CodigoProveedor, I.CodigoInterno " +
                                      "FROM INV_INVENTARIOPPAL IP " +
                                      "INNER JOIN INV_PRODUCTOS I ON IP.CodProducto = I.Id " +
                                      "WHERE I.Prestador = @param2 " +
                                      "AND IP.Cantidad > '0'";
                    }
                    else
                    {
                        Cargar_Hora = "SELECT I.Nombre, IP.Lote, IP.Factura, IP.Cantidad, IP.Costo, IP.Id, I.CodigoProveedor, I.CodigoInterno " +
                                      "FROM INV_INVENTARIOPPAL IP " +
                                      "INNER JOIN INV_PRODUCTOS I ON IP.CodProducto = I.Id " +
                                      "WHERE I.Nombre LIKE '%" + I.Nombre + "%' " +
                                      "AND I.Prestador = @param2 " +
                                      "AND IP.Cantidad > '0'";
                    }                   

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param2", I.Prestador);
                        Carga_Command.Parameters.AddWithValue("@param3", I.Proveedor);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<INV_HISTORICOPPAL> In = new List<INV_HISTORICOPPAL>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    In.Add(new INV_HISTORICOPPAL
                                    {
                                        CodigoProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                        CodigoInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Lote = Lectura_Hora["Lote"].ToString(),
                                        Factura = Lectura_Hora["Factura"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Costo = Convert.ToInt32(Lectura_Hora["Costo"]),
                                        Nombre = Lectura_Hora["Nombre"].ToString() 
                                    });
                                }

                                return In;
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
        INV_HISTORICOPPAL IBodegaPrincipal.GetInventaryPpalByPos(int PosisionTabla)
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

                    String Cargar_Hora = "SELECT I.Nombre, IP.Lote, IP.Factura, IP.Cantidad, IP.Costo, IP.Id, I.Proveedor, I.Prestador, IP.CodProducto, I.CodigoProveedor, I.CodigoInterno " +
                                         "FROM INV_INVENTARIOPPAL IP " +
                                         "INNER JOIN INV_PRODUCTOS I ON IP.CodProducto = I.Id " +
                                         "WHERE IP.Id = @param1";                    

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", PosisionTabla);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                INV_HISTORICOPPAL In = new INV_HISTORICOPPAL
                                {
                                    CodigoProveedor = Lectura_Hora["CodigoProveedor"].ToString(),
                                    CodigoInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                    Id = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                    Lote = Lectura_Hora["Lote"].ToString(),
                                    Factura = Lectura_Hora["Factura"].ToString(),
                                    Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                    Costo = Convert.ToInt32(Lectura_Hora["Costo"]),
                                    Nombre = Lectura_Hora["Nombre"].ToString(),
                                    Proveedor = Convert.ToInt32(Lectura_Hora["Proveedor"]),
                                    Prestador = Convert.ToInt32(Lectura_Hora["Prestador"]),
                                    CodProducto = Convert.ToInt32(Lectura_Hora["CodProducto"])
                                };                              

                                return In;
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
        List<INV_HISTORICOPPAL> IBodegaPrincipal.LoadInventary(int Prestador)
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

                    String Cargar_Hora = "SELECT P.Nombre, I.Lote, I.Factura, I.Cantidad, I.Costo, C.Com_Nombre  " +
                                          "FROM INV_INVENTARIOPPAL I " +
                                          "INNER JOIN INV_PRODUCTOS P ON I.CodProducto = P.Id " +
                                          "INNER JOIN CXN_CIA C ON P.Prestador = C.Com_Identificador " +
                                          "WHERE P.Prestador = @param1 " +
                                          "AND I.Cantidad > 0";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Prestador);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<INV_HISTORICOPPAL> In = new List<INV_HISTORICOPPAL>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    In.Add(new INV_HISTORICOPPAL
                                    {
                                        Nombre = Lectura_Hora["Nombre"].ToString(),
                                        Lote = Lectura_Hora["Lote"].ToString(),
                                        Factura = Lectura_Hora["Factura"].ToString(),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Costo = Convert.ToInt32(Lectura_Hora["Costo"]),
                                        CodigoProveedor = Lectura_Hora["Com_Nombre"].ToString(),
                                        Usuario = Lectura_Hora["Com_Nombre"].ToString()
                                    });
                                }

                                return In;
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
        bool IBodegaPrincipal.Reset()
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

                    string Busqueda = "UPDATE INV_INVENTARIOPPAL " +
                                      "SET " +
                                      "Cantidad = @param1";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", "0");

                    int Guarda = Accion.ExecuteNonQuery();
                    if (Guarda > 0)
                    {   
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
