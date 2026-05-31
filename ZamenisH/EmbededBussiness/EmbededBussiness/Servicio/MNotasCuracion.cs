using Domain;
using Domain.CXN;

using EmbededBussiness.Interfaz;

using Microsoft.Data.SqlClient;

using System.Data;

namespace EmbededBussiness.Servicio
{
    public class MNotasCuraciones : INotasCuraciones
    {
        private string Recepcion;

        otrosDatosPacienteHorario INotasCuraciones.cargarAdmision(int Admision, string filter)
        {
            try
            {
                var getCon = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT TOP 1 H.Hor_Pac_Bod, H.Hor_Pac_Id, P.Pac_TipoId, P.Pac_IdNum, C.Con_Nombre, CI.Com_Telefono, CI.Com_Direccion, CI.Com_Identificacion, CI.Com_Logo, B.Bod_Responsable, H.Hor_Pac_Fecha_Cita, H.Hor_Pac_Hora_Cita, A.Ase_Descripcion, CI.Com_Nombre, H.Hor_Imp_Age, H.Hor_ArrastraHistoria, H.Hor_DocFEModeradorCUFE, " +
                                         "P.Pac_Email, P.Pac_Telefono, P.Pac_TelefonoAux, P.Pac_Id, H.Hor_Observacion, H.Hor_Autoriza, H.Hor_Imp_Age, P.Pac_Dep_Cod, P.Pac_Mun_Cod, P.Pac_Sexo, P.Pac_Regimen, P.Pac_FechaNto, H.Hor_Pac_Ase, " +
                                         "P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerA, P.Pac_SegundoA, H.Hor_Vales, H.Hor_Pac_Tipo_Serv, H.Hor_Pac_Cup, H.Hor_Pac_Fecha_Cita, H.Hor_ValDerechos, H.Hor_RcCaja, H.Hor_RegAtn, H.Hor_Usr_Admisiona, " +
                                         "H.Hor_Pac_Fecha, H.Hor_Pac_Hora, H.Hor_Pac_Cia, H.Hor_CantSesion, H.Hor_Pac_Atendido, H.Hor_Estado, H.Hor_Pac_Llegada, H.Hor_Pac_Sal, P.Pac_Zona, P.Pac_Contrato, P.Pac_PaisOrigen, P.Pac_Residencia, H.Hor_Pac_UsrGraba, H.HorObservaTemp, P.Pac_Categoria, P.Pac_ECivil, P.Pac_Acudiente, P.Pac_Parentesco, P.Pac_DireccionAcu, Pac_TelefonoAcu, Pac_CorreoAcu, VIH, Hepatitis " +
                                         "FROM CXN_HORARIO H  " +
                                         "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                         "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                         "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero  " +
                                         "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                         "INNER JOIN CXN_CIA CI ON H.Hor_Pac_Cia = CI.Com_Identificador  " +
                                         "WHERE H.Hor_Id = '" + Admision + "' " +
                                         "AND H.Hor_Estado IN (" + filter + ")";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                string arrastra = string.IsNullOrEmpty(Lectura_Hora["Hor_ArrastraHistoria"].ToString()) ? "N" : Lectura_Hora["Hor_ArrastraHistoria"].ToString();

                                otrosDatosPacienteHorario H = new otrosDatosPacienteHorario
                                {
                                    Hor_Pac_Tipo_Serv = Lectura_Hora["Hor_Pac_Tipo_Serv"].ToString(),
                                    Hor_Pac_Ase = Convert.ToInt32(Lectura_Hora["Hor_Pac_Ase"]),
                                    Hor_Pac_Cup = Lectura_Hora["Hor_Pac_Cup"].ToString(),
                                    Hor_Regimen = Lectura_Hora["Pac_Regimen"].ToString(),
                                    Hor_Pac_Id = Convert.ToInt32(Lectura_Hora["Hor_Pac_Id"]),
                                    Hor_Pac_Bod = Convert.ToInt32(Lectura_Hora["Hor_Pac_Bod"]),
                                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                    Hor_Pac_Hora_Cita = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora_Cita"]),
                                    Hor_Observacion = Lectura_Hora["Hor_Observacion"].ToString(),
                                    Hor_Vales = Lectura_Hora["Hor_Vales"].ToString(),
                                    Com_Nombre = Lectura_Hora["Com_Nombre"].ToString(),
                                    Com_Telefono = Lectura_Hora["Com_Telefono"].ToString(),
                                    Com_Direccion = Lectura_Hora["Com_Direccion"].ToString(),
                                    Com_Identificacion = Lectura_Hora["Com_Identificacion"].ToString(),

                                    Pac_TipoId = Lectura_Hora["Pac_TipoId"].ToString(),
                                    Pac_IdNum = Lectura_Hora["Pac_IdNum"].ToString(),
                                    Pac_PrimerN = Lectura_Hora["Pac_PrimerN"].ToString(),
                                    Pac_SegundoN = Lectura_Hora["Pac_SegundoN"].ToString(),
                                    Pac_PrimerA = Lectura_Hora["Pac_PrimerA"].ToString(),
                                    Pac_SegundoA = Lectura_Hora["Pac_SegundoA"].ToString(),
                                    Pac_Telefono = Lectura_Hora["Pac_Telefono"].ToString(),
                                    Pac_TelefonoAux = Lectura_Hora["Pac_TelefonoAux"].ToString(),
                                    Pac_Email = Lectura_Hora["Pac_Email"].ToString(),
                                    Pac_Dep_Cod = Lectura_Hora["Pac_Dep_Cod"].ToString(),
                                    Pac_Mun_Cod = Lectura_Hora["Pac_Mun_Cod"].ToString(),
                                    Bod_Responsable = Lectura_Hora["Bod_Responsable"].ToString(),
                                    Pac_FechaNto = Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"]),
                                    Pac_Sexo = Lectura_Hora["Pac_Sexo"].ToString(),
                                    Hor_ValDerechos = Lectura_Hora["Hor_ValDerechos"].ToString(),
                                    Hor_RcCaja = (string.IsNullOrEmpty(Lectura_Hora["Hor_RcCaja"].ToString()) ? "0" : Lectura_Hora["Hor_RcCaja"].ToString()),
                                    Hor_RegAtn = Lectura_Hora["Hor_RegAtn"].ToString(),
                                    Hor_Usr_Admisiona = Lectura_Hora["Hor_Usr_Admisiona"].ToString(),
                                    Hor_Pac_Fecha = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha"]),
                                    Hor_Pac_Hora = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Hora"]),
                                    Com_Nombre_SMS = Lectura_Hora["Con_Nombre"].ToString(),
                                    Com_Telefono_SMS = Lectura_Hora["Ase_Descripcion"].ToString(),
                                    Hor_Autoriza = Lectura_Hora["Hor_Autoriza"].ToString(),
                                    Hor_Pac_Cia = Convert.ToInt32(Lectura_Hora["Hor_Pac_Cia"]),
                                    Hor_Imp_Age = Lectura_Hora["Hor_Imp_Age"].ToString(),
                                    Hor_Pac_Sal = Lectura_Hora["Hor_Pac_Sal"].ToString(),
                                    Pac_Zona = (Lectura_Hora["Pac_Zona"] != DBNull.Value ? Lectura_Hora["Pac_Zona"].ToString() : "U"),
                                    Pac_Contrato = Lectura_Hora["Pac_Contrato"].ToString(),
                                    Pac_PaisOrigen = Lectura_Hora["Pac_PaisOrigen"].ToString(), //pais
                                    Pac_PaisResidencia = Lectura_Hora["Pac_Residencia"].ToString(), //pais
                                    Com_Logo = Lectura_Hora["Com_Logo"].ToString(),
                                    Hor_Pac_UsrGraba = Lectura_Hora["Hor_Pac_UsrGraba"].ToString(),
                                    HorObservaTemp = Lectura_Hora["HorObservaTemp"].ToString(),
                                    Pac_Categoria = Lectura_Hora["Pac_Categoria"].ToString(),
                                    Pac_ECivilLoadAdmition = Lectura_Hora["Pac_Ecivil"].ToString(),
                                    Pac_AcudienteLoadAdmition = Lectura_Hora["Pac_Acudiente"].ToString(),
                                    Pac_CorreoLoadAdmition = Lectura_Hora["Pac_CorreoAcu"].ToString(),
                                    Pac_DireccionLoadAdmition = Lectura_Hora["Pac_DireccionAcu"].ToString(),
                                    Pac_ParentescoLoadAdmition = Lectura_Hora["Pac_Parentesco"].ToString(),
                                    Pac_TelefonoLoadAdmition = Lectura_Hora["Pac_TelefonoAcu"].ToString(),
                                    Hor_ArrastraHistoria = arrastra,
                                    Hor_DocFEModeradorCUFE = Lectura_Hora["Hor_DocFEModeradorCUFE"].ToString(),
                                    VIH = Lectura_Hora["VIH"] == DBNull.Value ? "N" : Lectura_Hora["VIH"].ToString(),
                                    Hepatitis = Lectura_Hora["Hepatitis"] == DBNull.Value ? "N" : Lectura_Hora["Hepatitis"].ToString()
                                };

                                H.Hor_CantSesion = (Lectura_Hora["Hor_CantSesion"] != DBNull.Value ? Convert.ToInt32(Lectura_Hora["Hor_CantSesion"]) : 0);

                                if (Lectura_Hora["Hor_Estado"].ToString() == "P")
                                {
                                    H.Hor_Pac_Llegada = (Lectura_Hora["Hor_Pac_Llegada"] != null ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Llegada"]) : new DateTime(1900, 01, 01, 00, 00, 00));
                                }

                                if (Lectura_Hora["Hor_Estado"].ToString() == "H")
                                {
                                    if (Lectura_Hora["Hor_Pac_Atendido"] != DBNull.Value)
                                    {
                                        H.Hor_Pac_Atendido = (Lectura_Hora["Hor_Pac_Atendido"] != null ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Atendido"]) : new DateTime(1900, 01, 01, 00, 00, 00));
                                        H.Hor_Pac_Llegada = (!string.IsNullOrEmpty(Lectura_Hora["Hor_Pac_Llegada"].ToString()) ? Convert.ToDateTime(Lectura_Hora["Hor_Pac_Llegada"]) : new DateTime(1900, 01, 01, 00, 00, 00));
                                    }
                                    else
                                    {
                                        DateTime g = new DateTime(1900, 01, 01, 00, 00, 00);
                                        H.Hor_Pac_Atendido = Convert.ToDateTime(g);

                                        if (string.IsNullOrEmpty(Lectura_Hora["Hor_Pac_Llegada"].ToString()))
                                        {
                                            H.Hor_Pac_Llegada = Convert.ToDateTime(g);
                                        }
                                        else
                                        {
                                            H.Hor_Pac_Llegada = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Llegada"]);
                                        }
                                    }
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
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        CXN_CONVENIOS INotasCuraciones.ServicioNombre(string CUP, int Ase, string Tipo)
        {
            try
            {
                var dataConection = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT Con_Nombre, Con_Valor, Con_Id_Serv " +
                                   "FROM CXN_CONVENIOS " +
                                   "WHERE Con_Aseguradora = '" + Ase + "' " +
                                   "AND Con_Id_Serv = '" + CUP + "' " +
                                   "AND Con_Tipo_Serv = '" + Tipo + "'";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_CONVENIOS D = new CXN_CONVENIOS
                                {
                                    Con_Nombre = Reader["Con_Nombre"].ToString(),
                                    Con_Valor = Convert.ToInt32(Reader["Con_Valor"]),
                                    Con_Id_Serv = Reader["Con_Id_Serv"].ToString()
                                };
                                
                                return D;
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
        CXN_ASEGURADORA INotasCuraciones.getInfoFromAsebyCode(int Code)
        {
            try
            {
                var dataConection = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT * " +
                                   "FROM CXN_ASEGURADORA " +
                                   "WHERE Ase_Identificador = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_ASEGURADORA DH = new CXN_ASEGURADORA
                                {
                                    Ase_Id = Convert.ToInt32(Reader["Ase_Id"]),
                                    Ase_NitCia = Reader["Ase_NitCia"].ToString(),
                                    Ase_Responable = Reader["Ase_Responable"].ToString(),
                                    Ase_Identificador = Convert.ToInt32(Reader["Ase_Identificador"]),
                                    Ase_Telefono = Reader["Ase_Telefono"].ToString(),
                                    Ase_UsuarioGraba = Reader["Ase_UsuarioGraba"].ToString(),
                                    Ase_Cod_Emp = Reader["Ase_Cod_Emp"].ToString(),
                                    Ase_Cod_Prest = Reader["Ase_Cod_Prest"].ToString(),
                                    Ase_Descripcion = Reader["Ase_Descripcion"].ToString(),
                                    Ase_Direccion = Reader["Ase_Direccion"].ToString(),
                                    Ase_DVNitCia = Reader["Ase_DVNitCia"].ToString(),
                                    Ase_Email = Reader["Ase_Email"].ToString()
                                };

                                return DH;
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
        string INotasCuraciones.BuscaDX(string CodDX)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora2 = "SELECT Cie_Serv " +
                                          "FROM CXN_CIE10 " +
                                          "WHERE Cie_Cod = '" + CodDX + "'";

                    using (SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con))
                    {
                        using (SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader()))
                        {
                            if (Lectura_Hora2.Read() == true)
                            {
                                return Lectura_Hora2["Cie_Serv"].ToString();
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
                Console.WriteLine(ex.Message);
                return "";
            }            
        }
        CXN_PACIENTES INotasCuraciones.LlamarPacientebyId(int pacid)
        {
            try
            {
                var getDatCone = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getDatCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_PACIENTES " +
                                   "WHERE Pac_Id = '" + pacid + "'";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_PACIENTES DP = new CXN_PACIENTES
                                {
                                    Pac_TipoId = (Reader["Pac_TipoId"] == DBNull.Value ? "" : Reader["Pac_TipoId"].ToString()),
                                    Pac_IdNum = (Reader["Pac_IdNum"] == DBNull.Value ? "" : Reader["Pac_IdNum"].ToString()),
                                    Pac_FechaNto = (Reader["Pac_FechaNto"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(Reader["Pac_FechaNto"])),
                                    Pac_PrimerN = (Reader["Pac_PrimerN"] == DBNull.Value ? "" : Reader["Pac_PrimerN"].ToString()),
                                    Pac_SegundoN = (Reader["Pac_SegundoN"] == DBNull.Value ? "" : Reader["Pac_SegundoN"].ToString()),
                                    Pac_PrimerA = (Reader["Pac_PrimerA"] == DBNull.Value ? "" : Reader["Pac_PrimerA"].ToString()),
                                    Pac_SegundoA = (Reader["Pac_SegundoA"] == DBNull.Value ? "" : Reader["Pac_SegundoA"].ToString()),
                                    Pac_Sexo = (Reader["Pac_Sexo"] == DBNull.Value ? "" : Reader["Pac_Sexo"].ToString()),
                                    Pac_Telefono = (Reader["Pac_Telefono"] == DBNull.Value ? "" : Reader["Pac_Telefono"].ToString()),
                                    Pac_TelefonoAux = (Reader["Pac_TelefonoAux"] == DBNull.Value ? "" : Reader["Pac_TelefonoAux"].ToString()),
                                    Pac_Direccion = (Reader["Pac_Direccion"] == DBNull.Value ? "" : Reader["Pac_Direccion"].ToString()),
                                    Pac_Email = (Reader["Pac_Email"] == DBNull.Value ? "" : Reader["Pac_Email"].ToString()),
                                    Pac_Mun_Cod = (Reader["Pac_Mun_Cod"] == DBNull.Value ? "" : Reader["Pac_Mun_Cod"].ToString()),
                                    Pac_Zona = (Reader["Pac_Zona"] == DBNull.Value ? "" : Reader["Pac_Zona"].ToString()),
                                    Pac_Localidad = (Reader["Pac_Localidad"] == DBNull.Value ? "" : Reader["Pac_Localidad"].ToString()),
                                    Pac_Acudiente = (Reader["Pac_Acudiente"] == DBNull.Value ? "" : Reader["Pac_Acudiente"].ToString()),
                                    Pac_Parentesco = (Reader["Pac_Parentesco"] == DBNull.Value ? "" : Reader["Pac_Parentesco"].ToString()),
                                    Pac_DireccionAcu = (Reader["Pac_DireccionAcu"] == DBNull.Value ? "" : Reader["Pac_DireccionAcu"].ToString()),
                                    Pac_TelefonoAcu = (Reader["Pac_TelefonoAcu"] == DBNull.Value ? "" : Reader["Pac_TelefonoAcu"].ToString()),
                                    Pac_CorreoAcu = (Reader["Pac_CorreoAcu"] == DBNull.Value ? "" : Reader["Pac_CorreoAcu"].ToString()),
                                    Pac_Dep_Cod = (Reader["Pac_Dep_Cod"] == DBNull.Value ? "" : Reader["Pac_Dep_Cod"].ToString()),
                                    Pac_Id = Convert.ToInt32(Reader["Pac_Id"]),
                                    Pac_Doble = (Reader["Pac_Doble"] == DBNull.Value ? "" : Reader["Pac_Doble"].ToString()),
                                    Pac_2VXS = (Reader["Pac_2VXS"] == DBNull.Value ? "" : Reader["Pac_2VXS"].ToString()),
                                    Pac_Especial = (Reader["Pac_Especial"] == DBNull.Value ? "N" : Reader["Pac_Especial"].ToString()),
                                    Pac_Contrato = (Reader["Pac_Contrato"] == DBNull.Value ? "" : Reader["Pac_Contrato"].ToString()),
                                    Pac_Aseguradora = (Reader["Pac_Aseguradora"] == DBNull.Value ? 0 : Convert.ToInt32(Reader["Pac_Aseguradora"])),
                                    Pac_Regimen = (Reader["Pac_Regimen"] == DBNull.Value ? "01" : Reader["Pac_Regimen"].ToString()),
                                    Pac_FibInf = (Reader["Pac_FibInf"] == DBNull.Value ? "Incluido" : "Excluido"),
                                    Pac_PaisOrigen = (Reader["Pac_PaisOrigen"] == DBNull.Value ? "" : Reader["Pac_PaisOrigen"].ToString()),
                                    Pac_Categoria = (Reader["Pac_Categoria"] == DBNull.Value ? "" : Reader["Pac_Categoria"].ToString()),
                                    Pac_Residencia = (Reader["Pac_Residencia"] == DBNull.Value ? "" : Reader["Pac_Residencia"].ToString()),
                                    Pac_ECivil = (Reader["Pac_ECivil"] == DBNull.Value ? "" : Reader["Pac_ECivil"].ToString()),
                                    Pac_Ocupacion = (Reader["Pac_Ocupacion"] == DBNull.Value ? "" : Reader["Pac_Ocupacion"].ToString())
                                };


                                return DP;
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
        Dictionary<string, string> INotasCuraciones.getListado()
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "Select * from CXN_IMAGEN_SYSTEM";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                Dictionary<string, string> D = new Dictionary<string, string>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    if (Lectura_Hora["Tab_Nombre"].ToString() == "RellenarVaciosMG" && Lectura_Hora["Tab_Clave"].ToString() == "A")
                                    {
                                        D.Add(Lectura_Hora["Tab_Nombre"].ToString() + "1", Lectura_Hora["Tab_Config"].ToString());
                                    }

                                    D.Add(Lectura_Hora["Tab_Nombre"].ToString(), Lectura_Hora["Tab_Clave"].ToString());
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        bool INotasCuraciones.ConsultarNavyEnfermeria(int Paciente, int Bodega, DateTime Fecha)
        {
            try
            {
                var getCon = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {

                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Consulta_Previas = "SELECT * " +
                                              "FROM CXN_HORARIO " +
                                              "WHERE Hor_Pac_Id = @param1 " +
                                              "AND Hor_Estado <> @param2 " +
                                              "AND Hor_Pac_Tipo_Serv = @param3 " +
                                              "AND Hor_Pac_Fecha_Cita = @param4";

                    using (SqlCommand Carga_Commandos = new SqlCommand(Consulta_Previas, con))
                    {
                        Carga_Commandos.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Commandos.Parameters.AddWithValue("@param2", "C");
                        Carga_Commandos.Parameters.AddWithValue("@param3", "MG");
                        Carga_Commandos.Parameters.AddWithValue("@param4", Convert.ToDateTime(Fecha).ToString("yyyy/MM/dd"));

                        using (SqlDataReader Lectura_Horas_Previas = (Carga_Commandos.ExecuteReader()))
                        {
                            if (Lectura_Horas_Previas.HasRows)
                            {
                                while (Lectura_Horas_Previas.Read() == true)
                                {
                                    Console.WriteLine(Lectura_Horas_Previas["Hor_Id"].ToString());
                                    if (Lectura_Horas_Previas["Hor_Pac_Sal"] != DBNull.Value)
                                    {
                                        if (Lectura_Horas_Previas["Hor_Pac_Sal"].ToString() == "Z")
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return false;
        }
        string INotasCuraciones.Calcular3(int Paciente, string TipoServicio)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT TOP 1 Hor_Id, Hor_CantSesion " +
                                    "FROM CXN_HORARIO " +
                                    "WHERE Hor_Pac_Id = '" + Paciente + "' " +
                                    "AND Hor_Pac_Tipo_Serv = '" + TipoServicio + "' " +
                                    "AND Hor_IniciaSesion = 'S' " +
                                    "AND Hor_Estado IN ('P', 'H') " +
                                    "ORDER BY Hor_Id DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                var Can = Cantidad(Convert.ToInt32(Reader["Hor_Id"]), Paciente);
                                if (this.Recepcion != "YES")
                                {
                                    return "Sesion " + Can.ToString() + " de " + Reader["Hor_CantSesion"].ToString();
                                }
                                return "Sesion " + Can.ToString() + " de " + Reader["Hor_CantSesion"].ToString();
                            }
                            else
                            {
                                return "No es posible calcular sesion debido a que no hay registros de autorizaciones vigentes";
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        int Cantidad(int Hor_Id, int Hor_Pac_Id)
        {
            try
            {
                var getCon = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT COUNT(*) AS Total " +
                                    "FROM CXN_HORARIO " +
                                    "WHERE Hor_Pac_Id = '" + Hor_Pac_Id + "' " +
                                    "AND Hor_Pac_Tipo_Serv = 'CU' " +
                                    "AND Hor_Estado IN ('P', 'H') " +
                                    "AND Hor_Id >= '" + Hor_Id + "'";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Convert.ToInt32(Reader["Total"]);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }                                           
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        Dictionary<string, string> INotasCuraciones.SugerenciaServicio(int Paciente)
        {
            try
            {
                var getDataConection = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT TOP 1 HC_ServCatalogo, HC_CupCatalogo " +
                                    "FROM CXN_HCMG " +
                                    "WHERE HC_PacId = '" + Paciente + "' " +
                                    "ORDER BY HC_Fecha DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                Dictionary<string, string> DIC = new Dictionary<string, string>();
                                DIC.Add("Servicio", Reader["HC_ServCatalogo"].ToString());
                                DIC.Add("Cup", Reader["HC_CupCatalogo"].ToString());
                                return DIC;
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
        Dictionary<string, string> INotasCuraciones.Diagnosticos(int Paciente)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT TOP 1 HC_Patologia, HC_DX1, HC_DX2, HC_DX3, HC_Imp_Dx " +
                                   "FROM CXN_HCMG " +
                                   "WHERE HC_PacId = '" + Paciente + "' " +
                                   "ORDER BY HC_Fecha DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                Dictionary<string, string> D = new Dictionary<string, string>();

                                D.Add("DX1", Reader["HC_DX1"].ToString());
                                D.Add("DX2", Reader["HC_DX2"].ToString());
                                D.Add("DX3", Reader["HC_DX3"].ToString());
                                D.Add("Impresion", Reader["HC_Imp_Dx"].ToString());
                                D.Add("Patologia", Reader["HC_Patologia"].ToString());

                                return D;
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
        void INotasCuraciones.Graba_Hora_Atencion(int Atention)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hora_Atendido = DateTime.Now;
                    Hora_Atendido = Convert.ToDateTime(Hora_Atendido.ToString("HH:mm"));

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_Pac_Atendido = '" + Convert.ToDateTime(Hora_Atendido).ToString("HH:mm") + "' " +
                                      "WHERE Hor_Id = '" + Atention + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }
        void INotasCuraciones.OpenAdmition(int Admision, string Estado)
        {
            try
            {
                var getDataConection = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_HORARIO " +
                                      "SET Hor_AdmOpnened = '" + Estado + "' " +
                                      "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        List<string> INotasCuraciones.getCondiciones(int Pac)
        {
            try
            {
                var getDataConection = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT DISTINCT Condicion " +
                                   "FROM CXN_CONDICIONES " +
                                   "WHERE Paciente = @param1 " +
                                   "AND Habilita = @param2";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Pac);
                        Commando.Parameters.AddWithValue("@param2", "A");

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<string> list = new List<string>();

                                while (Reader.Read() == true)
                                {
                                    list.Add(Reader["Condicion"].ToString());
                                }

                                return list;
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
        bool INotasCuraciones.getCantEncuestaCU(string Service, string Mes, int Año)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_CONFENCUESTA " +
                                         "WHERE Servicio = @param1 " +
                                         "AND Mes = @param2 " +
                                         "AND Año = @param3";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Service);
                        Carga_Command.Parameters.AddWithValue("@param2", Mes);
                        Carga_Command.Parameters.AddWithValue("@param3", Año);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return (Convert.ToInt32(Lectura_Hora["Actual"]) < Convert.ToInt32(Lectura_Hora["Total"]) ? true : false);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        bool INotasCuraciones.getCantCitas(int Paciente, CXN_CONFENCUESTA C)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now.Date;

                    String Cargar_Hora = "SELECT COUNT(*) AS Cantidad " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Id = @param1 " +
                                         "AND Hor_Estado = @param2 " +
                                         "AND Hor_Pac_Tipo_Serv = @param3 " +
                                         "AND Hor_Pac_Fecha_Cita <> @param6 " +
                                         "AND Hor_Pac_Fecha_Cita BETWEEN @param4 AND @param5";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param2", "H");
                        Carga_Command.Parameters.AddWithValue("@param3", C.Servicio);

                        DateTime d = new DateTime(C.Año, Capitalize(C.Mes), 1, 00, 00, 000);
                        DateTime h = new DateTime(C.Año, Capitalize(C.Mes), 28, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hoy);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                int res = Convert.ToInt32(Lectura_Hora["Cantidad"]);
                                return (res >= 1 ? true : false);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        bool INotasCuraciones.getCantEncuestasPaciente(int Paciente, CXN_CONFENCUESTA C)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT COUNT(*) AS Cantidad " +
                                         "FROM CXN_ENCUESTASATIS E " +
                                         "INNER JOIN CXN_HORARIO H ON E.Admision = H.Hor_Id " +
                                         "WHERE H.Hor_Pac_Id = @param1 " +
                                         "AND H.Hor_Pac_Tipo_Serv = @param3 " +
                                         "AND H.Hor_Pac_Fecha_Cita BETWEEN @param4 AND @param5 " +
                                         "AND E.Estado = @param6";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param3", C.Servicio);

                        DateTime d = new DateTime(C.Año, Capitalize(C.Mes), 1, 00, 00, 000);
                        DateTime h = new DateTime(C.Año, Capitalize(C.Mes), 28, 00, 00, 000);

                        Carga_Command.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = Convert.ToDateTime(d);
                        Carga_Command.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Convert.ToDateTime(h);
                        Carga_Command.Parameters.AddWithValue("@param6", "H");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                return (Convert.ToInt32(Lectura_Hora["Cantidad"]) >= 1 ? false : true);
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        int Capitalize(string Mes)
        {
            switch (Mes)
            {
                case "Enero":
                    return 1;
                case "Febrero":
                    return 2;
                case "Marzo":
                    return 3;
                case "Abril":
                    return 4;
                case "Mayo":
                    return 5;
                case "Junio":
                    return 6;
                case "Julio":
                    return 7;
                case "Agosto":
                    return 8;
                case "Septiembre":
                    return 9;
                case "Octubre":
                    return 10;
                case "Noviembre":
                    return 11;
                case "Diciembre":
                    return 12;
                default:
                    return 0;
            }
        }
        List<CXN_NOTASMED> INotasCuraciones.LoadHeridas(int Admision)
        {
            try
            {
                Dictionary<string, string> getData = Conection.ConectCore();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_NOTASMED " +
                                         "WHERE Admision = @param1 ";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_NOTASMED> C = new List<CXN_NOTASMED>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_NOTASMED
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Largo = Convert.ToDecimal(Lectura_Hora["Largo"]),
                                        Ancho = Convert.ToDecimal(Lectura_Hora["Ancho"]),
                                        Profundidad = Convert.ToDecimal(Lectura_Hora["Profundidad"]),
                                        Total = Convert.ToDecimal(Lectura_Hora["Total"])
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
