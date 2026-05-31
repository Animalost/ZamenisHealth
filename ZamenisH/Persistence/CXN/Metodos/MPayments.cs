using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MPayments : IPayments
    {
        LinkPagos IPayments.getStatusMonth()
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
                                   "FROM LinkPagos " +
                                   "WHERE Mes = @param1 " +
                                   "AND Año = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        int Año = DateTime.Now.Year;
                        string Mes = getMonth(DateTime.Now.Month).ToUpper();

                        Commando.Parameters.AddWithValue("@param1", Mes);
                        Commando.Parameters.AddWithValue("@param2", Año);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                LinkPagos L = new LinkPagos
                                {
                                    Año = Convert.ToInt32(Reader["Año"]),
                                    Estado = Reader["Estado"].ToString(),
                                    Link = Reader["Link"].ToString(),
                                    Id = Convert.ToInt32(Reader["Id"]),
                                    Mes = Reader["Mes"].ToString()
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        LinkPagos getStatusMonth(int Year, string Month)
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
                                   "FROM LinkPagos " +
                                   "WHERE Mes = @param1 " +
                                   "AND Año = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        int Año = DateTime.Now.Year;
                        string Mes = getMonth(DateTime.Now.Month).ToUpper();

                        Commando.Parameters.AddWithValue("@param1", Month);
                        Commando.Parameters.AddWithValue("@param2", Year);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                LinkPagos L = new LinkPagos
                                {
                                    Año = Convert.ToInt32(Reader["Año"]),
                                    Estado = Reader["Estado"].ToString(),
                                    Link = Reader["Link"].ToString(),
                                    Id = Convert.ToInt32(Reader["Id"]),
                                    Mes = Reader["Mes"].ToString()
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        List<Pagos> IPayments.getPagosHechos()
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
                                   "FROM PagosApp " +
                                   "ORDER BY Id ASC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<Pagos> L = new List<Pagos>();

                                while (Reader.Read() == true)
                                {
                                    if (Reader["Estado"].ToString() != "P")
                                    {
                                        LinkPagos P = getStatusMonth(Convert.ToInt32(Reader["Año"]), Reader["Mes"].ToString());

                                        L.Add(new Pagos
                                        {
                                            Año = Convert.ToInt32(Reader["Año"]),
                                            Id = Convert.ToInt32(Reader["Id"]),
                                            Estado = Reader["Estado"].ToString() != "P" ? "PENDIENTE" : "PAGADO",
                                            Factura = Reader["Factura"].ToString(),
                                            Mes = Reader["Mes"].ToString(),
                                            Link = P.Link
                                        });
                                    }
                                    else
                                    {
                                        L.Add(new Pagos
                                        {
                                            Año = Convert.ToInt32(Reader["Año"]),
                                            Id = Convert.ToInt32(Reader["Id"]),
                                            Estado = Reader["Estado"].ToString() != "P" ? "PENDIENTE" : "PAGADO",
                                            Factura = Reader["Factura"].ToString(),
                                            Mes = Reader["Mes"].ToString(),
                                            Link = ""
                                        });
                                    }
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
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        string getMonth(int Month)
        {
            switch (Month)
            {
                case 1:
                    return "ENERO";
                case 2:
                    return "FEBRERO";
                case 3:
                    return "MARZO";
                case 4:
                    return "ABRIL";
                case 5:
                    return "MAYO";
                case 6:
                    return "JUNIO";
                case 7:
                    return "JULIO";
                case 8:
                    return "AGOSTO";
                case 9:
                    return "SEPTIEMBRE";
                case 10:
                    return "OCTUBRE";
                case 11:
                    return "NOVIEMBRE";
                case 12:
                    return "DICIEMBRE";
                default:
                    return "";
            }
        }
    }
}
