using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MImpuestos : IImpuestos
    {
        List<FUENTE2> IImpuestos.getFuente()
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM FUENTE2 " +
                                         "ORDER BY Concepto ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<FUENTE2> B = new List<FUENTE2>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    B.Add(new FUENTE2
                                    {
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Concepto = Lectura_Hora["Concepto"].ToString(),
                                        Retencion = Lectura_Hora["Retencion"].ToString()

                                    });
                                }
                                return B;
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        List<ICA2> IImpuestos.getICA()
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM ICA2 " +
                                         "ORDER BY Concepto ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ICA2> B = new List<ICA2>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    B.Add(new ICA2
                                    {
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Concepto = Lectura_Hora["Concepto"].ToString(),
                                        ICA = Lectura_Hora["ICA"].ToString()

                                    });
                                }
                                return B;
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        int IImpuestos.Fuente(int SubTotal, string FuenteTarifa)
        {
            int RtFte = 0;

            if (FuenteTarifa.Contains(",") == true)
            {
                var d = ((SubTotal * decimal.Parse(FuenteTarifa)) / 100);
                RtFte = Convert.ToInt32(d);
                return RtFte;
            }

            RtFte = Convert.ToInt32(FuenteTarifa);
            return ((SubTotal * RtFte)) / 100;
        }
        decimal IImpuestos.ICA(string IcaTarifa)
        {
            try
            {
                if (IcaTarifa.Contains("/"))
                {
                    string[] partes = IcaTarifa.Split('/');

                    if (partes.Length == 2 &&
                        decimal.TryParse(partes[0], out decimal numerador) &&
                        decimal.TryParse(partes[1], out decimal denominador) &&
                        denominador != 0)
                    {
                        decimal resultado = numerador / denominador;
                        return resultado;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
