using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MDocumentosWEB : IDocumentosWEB
    {
        List<ConsentimientosTemp> IDocumentosWEB.GetHechos(int Cia, string Documento)
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

                    String Cargar_Hora = "";
                    
                    if (string.IsNullOrEmpty(Documento))
                    {
                        Cargar_Hora = "SELECT TOP 100 * FROM ConsentimientosTemp ORDER BY FechaCreacion DESC";
                    }
                    else
                    {
                        Cargar_Hora = "SELECT * FROM ConsentimientosTemp WHERE Documento = @param1 ORDER BY FechaCreacion DESC";
                    }                  

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        if (!string.IsNullOrEmpty(Documento))
                        {
                            Carga_Command.Parameters.AddWithValue("@param1", Documento);
                        }

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ConsentimientosTemp> C = new List<ConsentimientosTemp>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new ConsentimientosTemp
                                    {
                                        Documento = Lectura_Hora["Documento"].ToString(),
                                        Email = Lectura_Hora["Email"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        FechaCreacion = Convert.ToDateTime(Lectura_Hora["FechaCreacion"]),
                                        Compañia = Convert.ToInt32(Lectura_Hora["Compañia"]),
                                        FechaFirma = Lectura_Hora["FechaFirma"] == DBNull.Value ? "SIN FIRMAR" : Convert.ToDateTime(Lectura_Hora["FechaFirma"]).ToString("yyyy-MM-dd"),
                                        FirmaDoctorBase64 = Lectura_Hora["FirmaDoctorBase64"].ToString(),
                                        FirmaPacienteBase64 = Lectura_Hora["FirmaPacienteBase64"].ToString(),
                                        NombreDoctor = Lectura_Hora["NombreDoctor"].ToString(),
                                        NombrePaciente = Lectura_Hora["NombrePaciente"].ToString(),
                                        RegistroDoctor = Lectura_Hora["RegistroDoctor"].ToString(),
                                        Seleccion1 = Lectura_Hora["Seleccion1"].ToString(),
                                        Seleccion2 = Lectura_Hora["Seleccion2"].ToString(),
                                        Seleccion3 = Lectura_Hora["Seleccion3"].ToString(),
                                        Telefono = Lectura_Hora["Telefono"].ToString(),
                                        TipoConsentimiento = Lectura_Hora["TipoConsentimiento"].ToString(),
                                        Token = Lectura_Hora["Token"].ToString()
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        List<ConsentimientosTemp> IDocumentosWEB.Export(int Pos)
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
                                         "FROM ConsentimientosTemp CT " +
                                         "INNER JOIN CXN_CIA C ON CT.Compañia = C.Com_Identificador " + 
                                         "WHERE CT.Id = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Pos);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<ConsentimientosTemp> C = new List<ConsentimientosTemp>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    string firmaBase64 = Lectura_Hora["FirmaPacienteBase64"].ToString();

                                    if (firmaBase64.Contains(","))
                                        firmaBase64 = firmaBase64.Split(',')[1];

                                    C.Add(new ConsentimientosTemp
                                    {
                                        Documento = Lectura_Hora["Documento"].ToString(),
                                        Email = Lectura_Hora["Email"].ToString(),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        FechaCreacion = Convert.ToDateTime(Lectura_Hora["FechaCreacion"]),
                                        Compañia = Convert.ToInt32(Lectura_Hora["Compañia"]),
                                        FechaFirma = Lectura_Hora["FechaFirma"] == DBNull.Value ? "SIN FIRMAR" : Convert.ToDateTime(Lectura_Hora["FechaFirma"]).ToString("yyyy-MM-dd"),
                                        FirmaDoctorBase64 = Lectura_Hora["FirmaDoctorBase64"].ToString(),
                                        FirmaPacienteBase64 = Lectura_Hora["FirmaPacienteBase64"].ToString(),
                                        NombreDoctor = Lectura_Hora["NombreDoctor"].ToString(),
                                        NombrePaciente = Lectura_Hora["NombrePaciente"].ToString(),
                                        RegistroDoctor = Lectura_Hora["RegistroDoctor"].ToString(),
                                        Seleccion1 = Lectura_Hora["Seleccion1"].ToString() == "SI" ? "SI ACEPTO" : "RECHAZO EL PROCEDIMIENTO",
                                        Seleccion2 = Lectura_Hora["Seleccion2"].ToString() == "SI" ? "SI AUTORIZO" : "NO AUTORIZO",
                                        Seleccion3 = Lectura_Hora["Seleccion3"].ToString() == "SI" ? "SI AUTORIZO" : "NO AUTORIZO",
                                        Telefono = Lectura_Hora["Telefono"].ToString(),
                                        TipoConsentimiento = Lectura_Hora["TipoConsentimiento"].ToString(),
                                        Token = Lectura_Hora["Token"].ToString(),
                                        FirmaDoc = Convert.FromBase64String(Lectura_Hora["FirmaDoctorBase64"].ToString()),
                                        FirmaPac = Convert.FromBase64String(firmaBase64),

                                        DirEmpresa = Lectura_Hora["Com_Direccion"].ToString(),
                                        NitEmpresa = Lectura_Hora["Com_Identificacion"].ToString(),
                                        Logo = Convert.FromBase64String(Lectura_Hora["Com_Logo"].ToString()),
                                        Empresa = Lectura_Hora["Com_Nombre"].ToString(),
                                        TelEmpresa = Lectura_Hora["Com_Telefono"].ToString(),
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
