using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MBarCodes : IBarCodes
    {
        List<CXN_BARCODES> IBarCodes.listaBarras()
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
                                         "FROM CXN_BARCODES " +
                                         "ORDER BY Bar_Origin ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_BARCODES> L = new List<CXN_BARCODES>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    L.Add(new CXN_BARCODES
                                    {
                                        Bar_CodeBar = Lectura_Hora["Bar_CodeBar"].ToString(),
                                        Bar_Origin = Lectura_Hora["Bar_Origin"].ToString(),
                                        Bar_Id = Convert.ToInt32(Lectura_Hora["Bar_Id"])
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
        CXN_BARCODES IBarCodes.GetCode(string CodeInterno, string CodeBar)
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
                                         "FROM CXN_BARCODES " +
                                         "WHERE Bar_Origin = @param1 " +
                                         "AND Bar_CodeBar = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", CodeInterno);
                        Carga_Command.Parameters.AddWithValue("@param2", CodeBar);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_BARCODES L = new CXN_BARCODES
                                {
                                    Bar_CodeBar = Lectura_Hora["Bar_CodeBar"].ToString(),
                                    Bar_Origin = Lectura_Hora["Bar_Origin"].ToString(),
                                    Bar_Id = Convert.ToInt32(Lectura_Hora["Bar_Id"])
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
        bool IBarCodes.Create(CXN_BARCODES C)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_BARCODES (Bar_Origin, " +
                                                                  "Bar_CodeBar) " +
                                         "values                  (@param1, " +
                                                                  "@param2)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Bar_Origin);
                    cmd.Parameters.AddWithValue("@param2", C.Bar_CodeBar);
                    return cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IBarCodes.Update(CXN_BARCODES C)
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

                    string Busqueda = "UPDATE CXN_BARCODES " +
                                     "SET Bar_CodeBar = @param1 " +
                                     "WHERE Bar_Origin = @param2";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", C.Bar_CodeBar);
                        Accion.Parameters.AddWithValue("@param2", C.Bar_Origin);

                        return Accion.ExecuteNonQuery() > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
    }
}
