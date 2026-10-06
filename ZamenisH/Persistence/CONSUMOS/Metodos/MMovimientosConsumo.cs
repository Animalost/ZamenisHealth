using Domain;
using Domain.CONSUMOS;
using Persistence.CONSUMOS.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CONSUMOS.Metodos
{
    public class MMovimientosConsumo : IMovimientosConsumo
    {
        bool IMovimientosConsumo.SaveConsumo(CON_CONSUMOS C)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CON_CONSUMOS (Con_Cons_IdProducto, " +
                                                                  "Con_Cons_Cantidad, " +
                                                                  "Con_Cons_Fecha, " +
                                                                  "Con_Cons_User, " +
                                                                  "Con_Cons_Status, " +
                                                                  "Con_Cons_Idconsultorio) " +
                                         "values                  (@param1, " +
                                                                  "@param2, " +
                                                                  "@param3, " +
                                                                  "@param4, " +
                                                                  "@param5, " +
                                                                  "@param6)", con);

                    cmd.Parameters.AddWithValue("@param1", C.Con_Cons_IdProducto);
                    cmd.Parameters.AddWithValue("@param2", C.Con_Cons_Cantidad);
                    cmd.Parameters.AddWithValue("@param3", Convert.ToDateTime(C.Con_Cons_Fecha));
                    cmd.Parameters.AddWithValue("@param4", C.Con_Cons_User);
                    cmd.Parameters.AddWithValue("@param5", C.Con_Cons_Status);
                    cmd.Parameters.AddWithValue("@param6", C.Con_Cons_Idconsultorio);
                    return cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = System.DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<CON_CONSUMOS> IMovimientosConsumo.GetConsumos(int IdBodega, DateTime Fecha)
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

                    String Query = "SELECT P.Con_Prod_Cod_Interno, P.Con_Prod_Name, C.Con_Cons_Cantidad, C.Con_Cons_User, C.Con_Cons_Status, C.Con_Cons_Id " +
                                   "FROM CON_CONSUMOS C " +
                                   "INNER JOIN CON_PRODUCTOS P ON C.Con_Cons_IdProducto = P.Con_Prod_Id " +
                                   "WHERE C.Con_Cons_Idconsultorio = @param1 " +
                                   "AND C.Con_Cons_Fecha = @param2 " +
                                   "ORDER BY P.Con_Prod_Name ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", IdBodega);
                        Commando.Parameters.AddWithValue("@param2", Convert.ToDateTime(Fecha));

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CON_CONSUMOS> C = new List<CON_CONSUMOS>();

                                while (Reader.Read() == true)
                                {
                                    C.Add(new CON_CONSUMOS
                                    {
                                        Con_Cons_Id = Convert.ToInt32(Reader["Con_Cons_Id"]),
                                        Con_Prod_Cod_Interno = Reader["Con_Prod_Cod_Interno"].ToString(),
                                        Con_Cons_Cantidad = Convert.ToInt32(Reader["Con_Cons_Cantidad"]),
                                        Con_Prod_Name = Reader["Con_Prod_Name"].ToString(),
                                        Con_Cons_Status = (bool)Reader["Con_Cons_Status"],
                                        Con_Cons_User = Reader["Con_Cons_User"].ToString()
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        bool IMovimientosConsumo.AnularMovimiento(CON_CONSUMOS C)
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

                    string Busqueda = "UPDATE CON_CONSUMOS " +
                                      "SET Con_Cons_Status = @param1, " +
                                      "Con_Cons_Usr_Anula = @param2, " +
                                      "Con_Cons_Fecha_Anula = @param3 " +
                                      "WHERE Con_Cons_Id = @param4";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", false);
                        Accion.Parameters.AddWithValue("@param2", C.Con_Cons_Usr_Anula);
                        Accion.Parameters.AddWithValue("@param3", C.Con_Cons_Fecha_Anula);
                        Accion.Parameters.AddWithValue("@param4", C.Con_Cons_Id);

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
        List<CON_CONSUMOS> IMovimientosConsumo.GetConsumos(DateTime Fecha)
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

                    String Query = "SELECT P.Con_Prod_Cod_Interno, P.Con_Prod_Name, P.Con_Prod_Cod_Externo, " +
                                   "C.Con_Cons_Cantidad, C.Con_Cons_User, C.Con_Cons_Status, C.Con_Cons_Id, C.Con_Cons_Fecha, C.Con_Cons_Idconsultorio, " +
                                   "A.Asi_Name, A.Asi_Number " +
                                   "FROM CON_CONSUMOS C " +
                                   "INNER JOIN CON_PRODUCTOS P ON C.Con_Cons_IdProducto = P.Con_Prod_Id " +
                                   "INNER JOIN CON_ASIGNACION A ON C.Con_Cons_Idconsultorio = A.Asi_Number " +
                                   "WHERE C.Con_Cons_Fecha = @param2 " +
                                   "ORDER BY P.Con_Prod_Name ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param2", Convert.ToDateTime(Fecha));

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CON_CONSUMOS> C = new List<CON_CONSUMOS>();

                                while (Reader.Read() == true)
                                {
                                    C.Add(new CON_CONSUMOS
                                    {                                        
                                        Con_Prod_Cod_Interno = Reader["Con_Prod_Cod_Interno"].ToString(),
                                        Con_Prod_Name = Reader["Con_Prod_Name"].ToString(),
                                        Con_Prod_Cod_Externo = Reader["Con_Prod_Cod_Externo"].ToString(),
                                        Con_Cons_Cantidad = Convert.ToInt32(Reader["Con_Cons_Cantidad"]),
                                        Con_Cons_User = Reader["Con_Cons_User"].ToString(),
                                        Con_Cons_Status = (bool)Reader["Con_Cons_Status"],
                                        Con_Cons_Fecha = Convert.ToDateTime(Reader["Con_Cons_Fecha"]),
                                        Con_Cons_Id = Convert.ToInt32(Reader["Con_Cons_Id"]),
                                        Con_Cons_Idconsultorio = Convert.ToInt32(Reader["Con_Cons_Idconsultorio"]),
                                        Asi_Name = Reader["Asi_Name"].ToString(),
                                        Asi_Number = Convert.ToInt32(Reader["Asi_Number"])
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
    }
}
