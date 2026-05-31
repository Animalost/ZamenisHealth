using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MMedicamentos : IMedicamentos
    {
        List<CXN_MEDICAMENTOS> IMedicamentos.PorDesc(string Med)
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

                    String Cargar_Hora = "SELECT * FROM CXN_MEDICAMENTOS WHERE Medicamento Like '%" + Med + "%'";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_MEDICAMENTOS> C = new List<CXN_MEDICAMENTOS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_MEDICAMENTOS
                                    {
                                        Codigo = Lectura_Hora["Codigo"].ToString(),
                                        Medicamento = Lectura_Hora["Medicamento"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"])
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
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}
