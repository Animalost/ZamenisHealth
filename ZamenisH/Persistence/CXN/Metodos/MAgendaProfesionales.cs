using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MAgendaProfesionales : IAgendaProfesionales
    {
        string IAgendaProfesionales.getEstadoCita(int Admision)
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

                    String CONV = "SELECT Hor_Estado " +
                                  "FROM CXN_HORARIO " +
                                  "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Com_CONV = new SqlCommand(CONV, con);
                    SqlDataReader Lec_CONV = (Com_CONV.ExecuteReader());
                    if (Lec_CONV.Read() == true)
                    {
                        return Lec_CONV["Hor_Estado"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }

        string IAgendaProfesionales.getEnfCitaMG(int Pac, DateTime Fecha)
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

                    String CONV = "SELECT TOP 1 B.Bod_Responsable " +
                                  "FROM CXN_HORARIO H " +
                                  "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                  "WHERE H.Hor_Pac_Id = @param1 " +
                                  "AND H.Hor_Pac_Fecha_Cita = @param2 " +
                                  "AND H.Hor_Estado <> @param3 " +
                                  "AND H.Hor_Pac_Tipo_Serv = @param4 " +
                                  "ORDER BY Hor_Id DESC";

                    using (SqlCommand Com_CONV = new SqlCommand(CONV, con))
                    {
                        Com_CONV.Parameters.AddWithValue("@param1", Pac);
                        Com_CONV.Parameters.Add(new SqlParameter("@param2", SqlDbType.DateTime)).Value = Fecha;
                        Com_CONV.Parameters.AddWithValue("@param3", "C");
                        Com_CONV.Parameters.AddWithValue("@param4", "CU");

                        using (SqlDataReader Lec_CONV = (Com_CONV.ExecuteReader()))
                        {
                            if (Lec_CONV.Read() == true)
                            {
                                return Lec_CONV["Bod_Responsable"].ToString();
                            }
                            else
                            {
                                return "";
                            }
                        }                            
                    }                        
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "";
            }
        }

        List<CXN_HORARIO> IAgendaProfesionales.Carga_Agenda(CXN_HORARIO H1)
        {
            string ADM;
            string PAC;
            string ADM_EST;
            string MOD;
            string ASE;
            string PACID;
            string Arrastra;

            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    using (SqlCommand cmd = new SqlCommand("ConsultarAgendaMedico", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Med", H1.Hor_Pac_Bod);
                        cmd.Parameters.AddWithValue("@Dia", H1.Hor_Observacion);
                        cmd.Parameters.AddWithValue("@Cia", H1.Hor_Pac_Cia);
                        cmd.Parameters.AddWithValue("@Fecha", Convert.ToDateTime(H1.Hor_Pac_Fecha_Cita).ToString(getData["Format_Fecha"]));

                        using (SqlDataReader Lectura_Hora = cmd.ExecuteReader())
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    if (Lectura_Hora["Hor_Id"].ToString() != "")
                                    {
                                        ADM = Lectura_Hora["Hor_Id"].ToString();
                                        ADM_EST = Lectura_Hora["Hor_Estado"].ToString();
                                        PACID = Lectura_Hora["Hor_Pac_Id"].ToString();
                                        Arrastra = Lectura_Hora["Hor_ArrastraHistoria"].ToString();

                                        string MOD2 = Lectura_Hora["Hor_Pac_Modalidad"].ToString();

                                        switch (MOD2)
                                        {
                                            case "P":
                                                MOD = "Presencial";
                                                break;
                                            case "V":
                                                MOD = "Virtual";
                                                break;
                                            case "D":
                                                MOD = "Domicilio";
                                                break;
                                            default:
                                                MOD = "Presencial";
                                                break;
                                        }

                                        if (ADM_EST == "B" && Lectura_Hora["Hor_Imp_Age"].ToString() == "")
                                        {
                                            PAC = "ESPACIO BLOQUEADO DESDE RECEPCION";
                                            ASE = "ESPACIO BLOQUEADO DESDE RECEPCION";
                                        }
                                        else if (ADM_EST == "B" && Lectura_Hora["Hor_Imp_Age"].ToString() != "")
                                        {
                                            PAC = Lectura_Hora["Hor_Imp_Age"].ToString();
                                            ASE = "ESPACIO BLOQUEADO DESDE RECEPCION";
                                        }
                                        else
                                        {
                                            PAC = Lectura_Hora["Hor_Imp_Age"].ToString();
                                            ASE = Lectura_Hora["Ase_Descripcion"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        ADM = "";
                                        ADM_EST = "";
                                        PAC = "";
                                        MOD = "";
                                        ASE = "";
                                        PACID = "0";
                                        Arrastra = "";
                                    }

                                    H.Add(new CXN_HORARIO
                                    {
                                        Hor_Pac_Id_Hora = Lectura_Hora["Ide"].ToString(),
                                        Hor_Autoriza = Lectura_Hora["Habilita"].ToString(),
                                        Hor_Pac_Hora = Convert.ToDateTime(Lectura_Hora["Hora"]),
                                        Hor_RegAtn = ADM.ToString(),
                                        Hor_Imp_Age = PAC,
                                        Hor_Estado = ADM_EST,
                                        Hor_Pac_Modalidad = MOD,
                                        PacienteAseguradora = ASE,
                                        Hor_IniciaSesion = Lectura_Hora["Hor_IniciaSesion"].ToString(),
                                        Hor_Pac_Sal = Lectura_Hora["Hor_Pac_Sal"].ToString(),
                                        Hor_Pac_Id = Convert.ToInt32(PACID),
                                        Hor_ArrastraHistoria = Arrastra
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
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        /*List<CXN_HORARIO> IAgendaProfesionales.Carga_Agenda(CXN_HORARIO H1)
        {
            string ADM;
            string PAC;
            string ADM_EST;
            string MOD;
            string ASE;
            string PACID;

            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "WITH MiSubConsulta AS ( " +
                                         "SELECT " +
                                         "D.Hora, " +
                                         "D.Ide,  " +
                                         "D.Habilita, " +
                                         "A.Ase_Descripcion, " +
                                         "H.Hor_Estado, " +
                                         "H.Hor_Id, " +
                                         "H.Hor_Imp_Age, " +
                                         "H.Hor_Pac_Minutos, " +
                                         "H.Hor_Pac_Modalidad, " +
                                         "H.Hor_IniciaSesion, " +
                                         "H.Hor_Pac_Sal, " +
                                         "H.Hor_Pac_Id " +
                                         "FROM " +
                                         "CXN_HORARIO H " +
                                         "JOIN " +
                                         "CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                         "JOIN " +
                                         "CXN_DISPONIBILIDAD_2 D ON H.Hor_Pac_Id_Hora = D.Ide " +
                                         "WHERE " +
                                         "H.Hor_Pac_Bod = '" + H1.Hor_Pac_Bod + "' " +
                                         "AND H.Hor_Pac_Cia = '" + H1.Hor_Pac_Cia + "' " +
                                         "AND H.Hor_Pac_Fecha_Cita = '" + Convert.ToDateTime(H1.Hor_Pac_Fecha_Cita).ToString(getData["Format_Fecha"]) + "' " +
                                         "AND H.Hor_Pac_Id_Hora = D.Ide " +
                                         "AND H.Hor_Estado <> 'C'  " +
                                         "AND D.Med = '" + H1.Hor_Pac_Bod + "' " +
                                         "AND Dia = '" + H1.Hor_Observacion + "' " + //dia
                                         ") " +
                                         "SELECT D.Hora, D.Ide, D.Habilita, " +
                                         "T2.Ase_Descripcion,  " +
                                         "T2.Hor_Estado, T2.Hor_Id, T2.Hor_Imp_Age, T2.Hor_Pac_Minutos, T2.Hor_Pac_Modalidad, T2.Hor_IniciaSesion, T2.Hor_Pac_Sal, T2.Hor_Pac_Id " +
                                         "FROM " +
                                         "CXN_DISPONIBILIDAD_2 D " +
                                         "LEFT JOIN " +
                                         "MiSubConsulta T2 " +
                                         "ON D.Ide = T2.Ide " +
                                         "WHERE D.Med = '" + H1.Hor_Pac_Bod + "' " +
                                         "AND D.Dia = '" + H1.Hor_Observacion + "' " +
                                         "ORDER BY D.Hora ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());

                    List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                    while (Lectura_Hora.Read() == true)
                    {
                        if (Lectura_Hora["Hor_Id"].ToString() != "")
                        {
                            ADM = Lectura_Hora["Hor_Id"].ToString();
                            ADM_EST = Lectura_Hora["Hor_Estado"].ToString();
                            PACID = Lectura_Hora["Hor_Pac_Id"].ToString();

                            string MOD2 = Lectura_Hora["Hor_Pac_Modalidad"].ToString();

                            switch (MOD2)
                            {
                                case "P":
                                    MOD = "Presencial";
                                    break;
                                case "V":
                                    MOD = "Virtual";
                                    break;
                                case "D":
                                    MOD = "Domicilio";
                                    break;
                                default:
                                    MOD = "Presencial";
                                    break;
                            }

                            if (ADM_EST == "B" && Lectura_Hora["Hor_Imp_Age"].ToString() == "")
                            {
                                PAC = "ESPACIO BLOQUEADO DESDE RECEPCION";
                                ASE = "ESPACIO BLOQUEADO DESDE RECEPCION";
                            }
                            else if (ADM_EST == "B" && Lectura_Hora["Hor_Imp_Age"].ToString() != "")
                            {
                                PAC = Lectura_Hora["Hor_Imp_Age"].ToString();
                                ASE = "ESPACIO BLOQUEADO DESDE RECEPCION";
                            }
                            else
                            {
                                PAC = Lectura_Hora["Hor_Imp_Age"].ToString();
                                ASE = Lectura_Hora["Ase_Descripcion"].ToString();
                            }
                        }
                        else
                        {
                            ADM = "";
                            ADM_EST = "";
                            PAC = "";
                            MOD = "";
                            ASE = "";
                            PACID = "0";
                        }


                        H.Add(new CXN_HORARIO
                        {
                            Hor_Pac_Id_Hora = Lectura_Hora["Ide"].ToString(),
                            Hor_Autoriza = Lectura_Hora["Habilita"].ToString(),
                            Hor_Pac_Hora = Convert.ToDateTime(Lectura_Hora["Hora"]),
                            Hor_RegAtn = ADM.ToString(),
                            Hor_Imp_Age = PAC,
                            Hor_Estado = ADM_EST,
                            Hor_Pac_Modalidad = MOD,
                            PacienteAseguradora = ASE,
                            Hor_IniciaSesion = Lectura_Hora["Hor_IniciaSesion"].ToString(),
                            Hor_Pac_Sal = Lectura_Hora["Hor_Pac_Sal"].ToString(),
                            Hor_Pac_Id = Convert.ToInt32(PACID)
                        });
                    }

                    return H;

                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }*/
    }
}
