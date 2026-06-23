using Domain;
using Domain.CXN_ADJUNTOS;
using Persistence.CXN_ADJUNTOS.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

namespace Persistence.CXN_ADJUNTOS.Metodos
{
    public class MAdjuntos : IAdjuntos
    {
        private static readonly IPacientes repoPac = new MPacientes();

        int IAdjuntos.uploadFile(Adj_Archivos A)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["ConexionAdjuntos"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into Adj_Archivos (Adj_Tipo, " + //param1
                                                         "Adj_Usr_Graba, " + //param2
                                                         "Adj_Fecha, " + //param3
                                                         "Adj_Archivo, " + //param4
                                                         "Adj_Paciente, " + //param5
                                                         "Adj_Observacion," +
                                                         "Adj_Clase) " + //param16
                                "values                  (@param1, " + // Hor_Estado
                                                         "@param2, " + // Hor_Pac_Id
                                                         "@param3, " + // Hor_Pac_Bod
                                                         "@param4, " + // Hor_Pac_Tipo_Serv
                                                         "@param5, " + // Hor_Pac_Cia
                                                         "@param6, " +
                                                         "@param7); SELECT SCOPE_IDENTITY();", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", A.Adj_Tipo);
                    cmd.Parameters.AddWithValue("@param2", A.Adj_Usr_Graba);
                    cmd.Parameters.Add(new SqlParameter("@param3", SqlDbType.DateTime)).Value = A.Adj_Fecha;
                    cmd.Parameters.AddWithValue("@param4", A.Adj_Archivo);
                    cmd.Parameters.AddWithValue("@param5", A.Adj_Paciente);
                    cmd.Parameters.AddWithValue("@param6", A.Adj_Observacion);
                    cmd.Parameters.AddWithValue("@param7", A.Adj_Clase);

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
        List<CXN_HORARIO> IAdjuntos.getAdjuntos(int Paciente)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["ConexionAdjuntos"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    CXN_PACIENTES getPac = repoPac.LlamarPacientebyId(Paciente);
                    if (getPac != null)
                    {
                        String Query = "SELECT * " +
                                       "FROM Adj_Archivos " +
                                       "WHERE Adj_Paciente = @param1 " +
                                       "ORDER BY Adj_Fecha DESC";

                        using (SqlCommand Commando = new SqlCommand(Query, con))
                        {
                            Commando.Parameters.AddWithValue("@param1", getPac.Pac_Id);

                            using (SqlDataReader Reader = (Commando.ExecuteReader()))
                            {
                                if (Reader.HasRows)
                                {
                                    List<CXN_HORARIO> D = new List<CXN_HORARIO>();

                                    while (Reader.Read() == true)
                                    {
                                        D.Add(new CXN_HORARIO
                                        {
                                            Hor_Id = Convert.ToInt32(Reader["Adj_Id"]),
                                            Hor_Imp_Age = getPac.Pac_PrimerA + " " + getPac.Pac_SegundoA + " " + getPac.Pac_PrimerN + " " + getPac.Pac_SegundoN,
                                            Hor_Observacion = Reader["Adj_Observacion"].ToString(),
                                            Hor_Pac_Fecha_Cita = Convert.ToDateTime(Reader["Adj_Fecha"]),
                                            Com_Tipo_Doc = Reader["Adj_Clase"].ToString()
                                        });
                                    }

                                    return D;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }                                                    
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        byte[] IAdjuntos.getPDF(int Posision)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["ConexionAdjuntos"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Adj_Archivo " +
                                   "FROM Adj_Archivos " +
                                   "WHERE Adj_Id = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Posision);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return (byte[])Reader["Adj_Archivo"];
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
