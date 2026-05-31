using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Persistence
{
    public static class PTablesSQL
    {
        public static List<string> GetColumnNames(Type tableName)
        {
            try
            {                
                PropertyInfo[] propiedades = tableName.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                string[] nombresAtributos = propiedades
            .Where(p => p.DeclaringType == tableName)
                            .Select(p => p.Name)
                            .ToArray();

                List<string> L = new List<string>();
                foreach (string i in nombresAtributos)
                {
                    L.Add(i);
                }

                return L;                
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static int ConsultarBase(string connectionString, string tableName, string columnName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";
                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        return (int)Command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static bool ConsultarTablaExistente(string connectionString, string tableName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        int count = Convert.ToInt32(Command.ExecuteScalar());
                       
                        if (count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

    }
}
