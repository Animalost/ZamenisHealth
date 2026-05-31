using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MTwilio : ITwilio
    {
        public MTwilio() 
        {
        
        }

        List<CXN_LINKSCORTOS> ITwilio.getUrlsGenerated(string Docunmento)
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
                                         "FROM CXN_LINKSCORTOS L " +
                                         "INNER JOIN CXN_HORARIO H ON L.Admision = H.Hor_Id " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "WHERE P.Pac_IdNum = @param1 " +
                                         "ORDER BY H.Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Docunmento);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_LINKSCORTOS> L = new List<CXN_LINKSCORTOS>();
                                DateTime HoraActual = new DateTime();

                                while (Lectura_Hora.Read() == true)
                                {
                                    string est = "Vencido";

                                    if (Convert.ToDateTime(Lectura_Hora["Fecha"]).AddHours(1) > DateTime.Now.AddHours(1))
                                    {
                                        est = "Vigente";
                                    }

                                    L.Add(new CXN_LINKSCORTOS
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Cliente = Lectura_Hora["Cliente"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"]),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Servidor = Lectura_Hora["Servidor"].ToString(),
                                        Usuario = Lectura_Hora["Usuario"].ToString(),
                                        complemento = new ComplementoLinksCortos 
                                        { 
                                            Estado = est.ToUpper().Trim(),
                                            Paciente = Lectura_Hora["Hor_Imp_Age"].ToString()
                                        }
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
        void ITwilio.Insertar(CXN_LINKSCORTOS C)
        {
            try
            {
                var getDataCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_LINKSCORTOS (Admision, " + 
                                                          "Usuario, " +
                                                          "Cliente, " + 
                                                          "Servidor, " + 
                                                          "Fecha) " + 
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", C.Admision);
                    cmd.Parameters.AddWithValue("@param2", C.Usuario);
                    cmd.Parameters.AddWithValue("@param3", C.Cliente);
                    cmd.Parameters.AddWithValue("@param4", C.Servidor);
                    cmd.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = C.Fecha;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
