using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Persistence.CXN.Metodos
{
    public class MRestorePass : IRestorePass
    {
        int IRestorePass.InsertRestorePass(CXN_RESTOREPASS R)
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

                    SqlCommand cmd = new SqlCommand(@"INSERT INTO CXN_RESTOREPASS (Usuario, " +
                                                          "Fecha, " +
                                                          "Habilitado) " +
                                 "values                  (@param1, " +
                                                          "@param2, " +
                                                          "@param3); SELECT SCOPE_IDENTITY();", con);
                    
                    cmd.Parameters.AddWithValue("@param1", R.Usuario);
                    cmd.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = R.Fecha;
                    cmd.Parameters.AddWithValue("@param3", R.Habilitado);
                    
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
    }
}
