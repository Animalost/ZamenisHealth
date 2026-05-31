using Domain;
using Domain.INV;
using Persistence.INV.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.INV.Metodos
{
    public class MSubBodegas : ISubBodegas
    {
        (int PosisionSub, int CantidadSub) ISubBodegas.GetInventarySubByBodPosProd(int Bodega, int PosProdTable)
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

                    String Cargar_Hora = "SELECT I.Cantidad, I.Id " +
                                         "FROM INV_INVENTARIOBODEGAS I " +
                                         "INNER JOIN INV_PRODUCTOS P ON I.CodInvPpal = P.Id " +
                                         "WHERE I.Bodega = @param1 " +
                                         "AND I.CodInvPpal = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Bodega);
                        Carga_Command.Parameters.AddWithValue("@param2", PosProdTable);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return (Convert.ToInt32(Lectura_Hora["Id"]), Convert.ToInt32(Lectura_Hora["Cantidad"]));
                            }
                            else
                            {
                                return (0, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return (0, 0);
            }
        }
        (int Cantidad, int PosIdTablePpal) ISubBodegas.GetInventarySubByBodPos(int PosisionTablaInv)
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

                    String Cargar_Hora = "SELECT Cantidad, CodInvPpal " +
                                         "FROM INV_INVENTARIOBODEGAS " +
                                         "WHERE Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", PosisionTablaInv);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return (Convert.ToInt32(Lectura_Hora["Cantidad"]), Convert.ToInt32(Lectura_Hora["CodInvPpal"]));
                            }
                            else
                            {
                                return (0, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return (0, 0);
            }
        }
        bool ISubBodegas.IngresarNuevo(INV_INVENTARIOBODEGAS I)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into INV_INVENTARIOBODEGAS (CodInvPpal, " +
                                                          "Cantidad, " +
                                                          "Bodega, " +
                                                          "Lote, " +
                                                          "Factura, " +
                                                          "Costo, " +
                                                          "IVA) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5, " +
                                                          "@param6, " +
                                                          "@param7)", con);

                    cmd.Parameters.AddWithValue("@param1", I.CodInvPpal);
                    cmd.Parameters.AddWithValue("@param2", I.Cantidad);
                    cmd.Parameters.AddWithValue("@param3", I.Bodega);
                    cmd.Parameters.AddWithValue("@param4", I.Lote);
                    cmd.Parameters.AddWithValue("@param5", I.Factura);
                    cmd.Parameters.AddWithValue("@param6", I.Costo);
                    cmd.Parameters.AddWithValue("@param7", I.IVA);

                    int c = cmd.ExecuteNonQuery();
                    if (c > 0)
                    {
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
        bool ISubBodegas.ActualizarCantidad(INV_INVENTARIOBODEGAS I)
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

                    string Busqueda = "UPDATE INV_INVENTARIOBODEGAS " +
                                      "SET " +
                                      "Cantidad = @param1 " +
                                      "WHERE Id = @param2";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", I.Cantidad);
                    Accion.Parameters.AddWithValue("@param2", I.Id);

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


        List<INV_HISTORICOPPAL> ISubBodegas.LoadInventary(int SubBodega, string Filtro)
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
                    
                    if (string.IsNullOrEmpty(Filtro))
                    {
                        Cargar_Hora = "SELECT I.Id, P.Nombre, I.Cantidad, B.Bod_Responsable, P.CodigoInterno  " +
                                          "FROM INV_INVENTARIOBODEGAS I " +
                                          "INNER JOIN INV_PRODUCTOS P ON I.CodInvPpal = P.Id " +
                                          "INNER JOIN CXN_BODEGAS B ON I.Bodega = B.Bod_Numero " +
                                          "WHERE I.Bodega = @param1 " +
                                          "AND I.Cantidad > 0";
                    }
                    else
                    {
                        Cargar_Hora = "SELECT I.Id, P.Nombre, I.Cantidad, B.Bod_Responsable, P.CodigoInterno  " +
                                          "FROM INV_INVENTARIOBODEGAS I " +
                                          "INNER JOIN INV_PRODUCTOS P ON I.CodInvPpal = P.Id " +
                                          "INNER JOIN CXN_BODEGAS B ON I.Bodega = B.Bod_Numero " +
                                          "WHERE I.Bodega = @param1 " +
                                          "AND I.Cantidad > 0 " +
                                          "AND P.Nombre LIKE '%" + Filtro + "%'";
                    }                                                         

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", SubBodega);

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
                                        Lote = "",
                                        Factura = "",
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        Costo = 0,
                                        CodigoProveedor = Lectura_Hora["CodigoInterno"].ToString(),
                                        Usuario = Lectura_Hora["Bod_Responsable"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"])
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
        bool ISubBodegas.Reset(int SubBodega)
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

                    string Busqueda = "UPDATE INV_INVENTARIOBODEGAS " +
                                      "SET " +
                                      "Cantidad = @param1 " +
                                      "WHERE Bodega = @param2";

                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    Accion.Parameters.AddWithValue("@param1", "0");
                    Accion.Parameters.AddWithValue("@param2", SubBodega);

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
        void ISubBodegas.IngresarSalidaSub(INV_SALIDASSUB S)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into INV_SALIDASSUB (CodPpal, " +
                                                          "Cantidad, " +
                                                          "Bodega, " +
                                                          "Fecha, " +
                                                          "Usuario) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3, " +
                                                          "@param4, " +
                                                          "@param5)", con);

                    cmd.Parameters.AddWithValue("@param1", S.CodPpal);
                    cmd.Parameters.AddWithValue("@param2", S.Cantidad);
                    cmd.Parameters.AddWithValue("@param3", S.Bodega);
                    cmd.Parameters.AddWithValue("@param4", Convert.ToDateTime(S.Fecha));
                    cmd.Parameters.AddWithValue("@param5", S.Usuario);

                    int c = cmd.ExecuteNonQuery();                   
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<INV_HISTORICOPPAL> ISubBodegas.getHistorial(DateTime Desde, DateTime Hasta)
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

                    String Cargar_Hora = "SELECT P.Nombre, P.CodigoInterno, S.Cantidad, S.Fecha, B.Bod_Responsable, S.Usuario, S.Id " +
                                         "FROM INV_SALIDASSUB S " +
                                         "INNER JOIN INV_INVENTARIOPPAL Pp ON S.CodPpal = Pp.Id " +
                                         "INNER JOIN INV_PRODUCTOS P ON Pp.CodProducto = P.Id " +
                                         "INNER JOIN CXN_BODEGAS B ON S.Bodega = B.Bod_Numero " +
                                         "WHERE S.Fecha BETWEEN @param1 AND @param2 " +
                                         "ORDER BY B.Bod_Responsable, S.Fecha DESC";                   

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Convert.ToDateTime(Convert.ToDateTime(Desde.Date)));
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Convert.ToDateTime(Hasta.Date)));

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
                                        CodigoInterno = Lectura_Hora["CodigoInterno"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        Cantidad = Convert.ToInt32(Lectura_Hora["Cantidad"]),
                                        CodigoProveedor = Lectura_Hora["Bod_Responsable"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Desde = Desde,
                                        Hasta = Hasta
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
    }
}
