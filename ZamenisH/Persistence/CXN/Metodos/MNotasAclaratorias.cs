using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MNotasAclaratorias : INotasAclaratorias
    {
        List<CXN_HORARIO> INotasAclaratorias.NotasMedicas(string TID, string NID, string Tipo)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query;
                    SqlCommand Commando;
                    SqlDataReader Reader;

                    switch (Tipo)
                    {
                        case "Curaciones":
                            Query = "SELECT N.Not_Adm AS Admision, H.Hor_Imp_Age AS Paciente, H.Hor_Pac_Fecha_Cita AS Fecha, B.Bod_Responsable AS Prof " +
                                             "FROM CXN_NOTAS N " +
                                             "INNER JOIN CXN_HORARIO H ON N.Not_Adm = H.Hor_Id " +
                                             "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                             "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                             "WHERE P.Pac_TipoId = '" + TID + "' " +
                                             "AND P.Pac_IdNum = '" + NID + "' " +
                                             "ORDER BY H.Hor_Pac_Fecha_Cita DESC";
                            Commando = new SqlCommand(Query, con);
                            Reader = (Commando.ExecuteReader());
                            break;

                        case "MedGen":
                            Query = "SELECT HC_Adm AS Admision, HC_Fecha AS Fecha, HC_Pac AS Paciente, Bod_Responsable AS Prof " +
                                             " FROM CXN_PACIENTES " +
                                             " INNER JOIN CXN_HCMG ON CXN_PACIENTES.Pac_Id = CXN_HCMG.HC_PacId " +
                                             " INNER JOIN CXN_BODEGAS ON CXN_HCMG.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                             " WHERE Pac_TipoId = '" + TID + "' " +
                                             " AND Pac_IdNum = '" + NID + "' " +
                                             " AND HC_Cant = '1' " +
                                             " ORDER BY CXN_HCMG.HC_Fecha DESC";
                            Commando = new SqlCommand(Query, con);
                            Reader = (Commando.ExecuteReader());
                            break;

                        case "Fisiatria":
                            Query = "SELECT HC_Adm AS Admision, HC_Fecha AS Fecha, HC_Pac AS Paciente, Bod_Responsable AS Prof " +
                                         " FROM CXN_PACIENTES " +
                                         " INNER JOIN CXN_HCFI ON CXN_PACIENTES.Pac_Id = CXN_HCFI.HC_PacId " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCFI.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE Pac_TipoId = '" + TID + "' " +
                                         " AND Pac_IdNum = '" + NID + "' " +
                                         " AND HC_Cant = '1' " +
                                         " ORDER BY CXN_HCFI.HC_Fecha DESC";
                            Commando = new SqlCommand(Query, con);
                            Reader = (Commando.ExecuteReader());
                            break;

                        case "Radiologia":
                            Query = "SELECT HCAdm AS Admision, Fecha AS Fecha, PacienteNombre AS Paciente, Bod_Responsable AS Prof " +
                                         " FROM CXN_PACIENTES " +
                                         " INNER JOIN CXN_HCRADIOLOGIA ON CXN_PACIENTES.Pac_Id = CXN_HCRADIOLOGIA.PacId " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCRADIOLOGIA.Medico = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE Pac_TipoId = '" + TID + "' " +
                                         " AND Pac_IdNum = '" + NID + "' " +
                                         " AND HCCant = '1' " +
                                         " ORDER BY CXN_HCRADIOLOGIA.Fecha DESC";
                            Commando = new SqlCommand(Query, con);
                            Reader = (Commando.ExecuteReader());
                            break;

                        default:
                            return null;
                    }

                    if (Reader.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Reader.Read() == true)
                        {
                            H.Add(new CXN_HORARIO
                            {
                                Hor_Id = Convert.ToInt32(Reader["Admision"]),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Reader["Fecha"]),
                                Com_Nombre = Reader["Prof"].ToString(),
                                Hor_Imp_Age = Reader["Paciente"].ToString()
                            });
                        }

                        return H;
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
    }
}
