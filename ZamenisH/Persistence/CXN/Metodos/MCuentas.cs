using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MCuentas : ICuentas
    {
        List<CXN_CUENTAS> ICuentas.getCuentas()
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

                    String Query = "SELECT * " +
                                   "FROM CXN_CUENTAS";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_CUENTAS> c = new List<CXN_CUENTAS>();

                                while (Reader.Read() == true)
                                {
                                    c.Add(new CXN_CUENTAS { 
                                        CuentaCode = Reader["CuentaCode"].ToString(),
                                        CuentaLlave = Reader["CuentaLlave"].ToString(),
                                        CuentaName = Reader["CuentaName"].ToString(),
                                        Naturaleza = Reader["Naturaleza"].ToString(),
                                        Id = Convert.ToInt32(Reader["Id"]),
                                    });
                                }

                                return c;
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
    }
}
