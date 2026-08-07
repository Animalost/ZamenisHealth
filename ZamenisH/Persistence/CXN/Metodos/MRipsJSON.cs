using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using Domain.CXN;

namespace Persistence.CXN.Metodos
{
    public class MRipsJSON : IRIPSJSON
    {
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();

        //INDIVIDUAL
        Transaccion IRIPSJSON.GenrateIndividual(RIPS_Class R)
        {
            try
            {
                Transaccion transaccion = new Transaccion();

                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }                    

                    CXN_FACTURA getFactura = getFacturas(R.aseRIPS, R.ciaRIPS, R.tDocumentRIPS, R.Numfac);
                    //USUARIO
                    string Query = "SELECT Pac_Mun_Cod,  Pac_PaisOrigen, Pac_Sexo, Pac_Zona, Pac_FechaNto, Pac_IdNum, Pac_TipoId, Pac_Regimen, Pac_Dep_Cod " +
                                "FROM CXN_PACIENTES WHERE Pac_Id = @id_pac";

                    string codMunicipioResidenciaU = "";
                    string codPaisOrigenU = "";
                    string codPaisResidenciaU = "";
                    string codSexoU = "";
                    string codZonaTerritorialResidenciaU = "";
                    int consecutivoU = 1;
                    string fechaNacimientoU = "";
                    string incapacidadU = "";
                    string numDocumentoIdentificacionU = "";
                    string tipoDocumentoIdentificacionU = "";
                    string tipoUsuarioU = R.TipoInd == "EPS" ? "01" : getCodeCobertura(getFactura.Cobertura);

                    if (getFactura != null)
                    {
                        //USUARIOS
                        using (SqlCommand Commando = new SqlCommand(Query, con))
                        {
                            Commando.Parameters.AddWithValue("@id_pac", getFactura.Fac_Pac);

                            using (SqlDataReader Reader = (Commando.ExecuteReader()))
                            {
                                if (Reader.Read() == true)
                                {
                                    codMunicipioResidenciaU = Reader["Pac_Dep_Cod"].ToString() + Reader["Pac_Mun_Cod"].ToString();
                                    codPaisOrigenU = Reader["Pac_PaisOrigen"].ToString();
                                    codPaisResidenciaU = "170";
                                    codSexoU = Reader["Pac_Sexo"].ToString();
                                    codZonaTerritorialResidenciaU = (Reader["Pac_Zona"].ToString() == "R" ? "02" : "01");
                                    consecutivoU = 1;
                                    fechaNacimientoU = Convert.ToDateTime(Reader["Pac_FechaNto"]).ToString("yyyy-MM-dd");
                                    incapacidadU = "NO";
                                    numDocumentoIdentificacionU = Reader["Pac_IdNum"].ToString();
                                    tipoDocumentoIdentificacionU = repoPacientes.getTipoDoc(Reader["Pac_TipoId"].ToString());
                                    //tipoUsuarioU = Reader["Pac_Regimen"].ToString();
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }


                        transaccion = new Transaccion
                        {
                            numDocumentoIdObligado = repoCia.getPrestadorbyCode(R.ciaRIPS).Com_Identificacion,
                            numFactura = R.Numfac.Replace("'",""),
                            tipoNota = null,
                            numNota = null,

                            usuarios = new List<Usuario>  // Inicializamos la lista de usuarios
                        {
                            new Usuario
                            {
                                 codMunicipioResidencia = codMunicipioResidenciaU,
                                 codPaisOrigen = codPaisOrigenU,
                                 codPaisResidencia = codPaisResidenciaU,
                                 codSexo = codSexoU,
                                 codZonaTerritorialResidencia = codZonaTerritorialResidenciaU,
                                 consecutivo = consecutivoU,
                                 fechaNacimiento = Convert.ToDateTime(fechaNacimientoU).ToString("yyyy-MM-dd"),
                                 incapacidad = incapacidadU,
                                 numDocumentoIdentificacion = numDocumentoIdentificacionU,
                                 tipoDocumentoIdentificacion = tipoDocumentoIdentificacionU,
                                 tipoUsuario = tipoUsuarioU,

                                servicios = new Servicios
                                {
                                    consultas = new List<Consultas>(),        // Inicializa la lista de Consultas
                                    procedimientos = new List<Procedimientos>(), // Inicializa la lista de Procedimientos
                                    otrosServicios = new List<OtrosServicios>()   // Inicializa la lista de OtrosServicios
                                }
                            }
                        }
                        };

                        if (getFactura != null)
                        {
                            //CONSULTA Y PROCEDIMIENTO
                            string Query2 = "WITH CTE AS ( " +
                                            "SELECT H.Hor_id, CIA.Com_Cod_Prestador, C.Car_Fecha, H.Hor_Pac_Hora_Cita, F.Fac_Num_Aut, H.Hor_Pac_Modalidad, " +
                                            "H.Hor_GrupoServicios, CO.Con_CodServicio, H.HorTecnoSalud, C.Car_CExterna, C.Car_Dx1, C.Car_Dx2, C.Car_Dx3, " +
                                            "C.Car_Imp_Dx, B.Bod_Reg_Med, C.Car_val_Tot, H.Hor_RcCaja, H.Hor_ConceptoRecaudo, H.Hor_DocFEModerador, " +
                                            "C.Car_Cod, CO.Con_CUP, " +
                                            "ROW_NUMBER() OVER (PARTITION BY H.Hor_id ORDER BY H.Hor_id) AS rn  " +
                                            "FROM " +
                                            "CXN_CARGOS C " +
                                            "INNER JOIN " +
                                            "CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                            "INNER JOIN " +
                                            "CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                            "INNER JOIN " +
                                            "CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero " +
                                            "INNER JOIN " +
                                            "CXN_CONVENIOS CO ON C.Car_Cod = CO.Con_Id_Serv " +
                                            "INNER JOIN " +
                                            "CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador " +
                                            "AND C.Car_Ase = CO.Con_Aseguradora " +
                                            "WHERE " +
                                            "F.Homologo IN (" + R.Numfac + ") " +
                                            "AND F.Fac_Estado = @estadoF " +
                                            "AND C.Car_Estado = @estadoC " +
                                            "AND C.Car_Tipo = @tipo) " +
                                            "SELECT * " +
                                            "FROM CTE " +
                                            "WHERE rn = 1";

                            //OTROS SERVICIOS
                            string Query3 = "SELECT C.Car_Cant AS Cantidad, C.Car_Val_Tot AS Total, C.Car_Cod, C.Car_Val_Un, C.Car_Fecha, H.Hor_Pac_Hora_Cita, C.Car_Item, B.Bod_Reg_Med, I.InvCodBar, CIA.Com_Cod_Prestador " +
                                       "FROM CXN_CARGOS C   " +
                                       "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac  " +
                                       "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id   " +
                                       "INNER JOIN CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero  " +
                                       "INNER JOIN CXN_INVENTARIO I ON C.Car_Cod = I.InvCod  " +
                                        "INNER JOIN CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador  " +
                                       "AND C.Car_Ase = I.InvConvenio   " +
                                       "WHERE F.Homologo IN (" + R.Numfac + ")   " +
                                       "AND F.Fac_Estado = @estadoF   " +
                                       "AND C.Car_Estado = @estadoC   " +
                                       "AND C.Car_Tipo = @tipo";


                            //CONSULTAS
                            using (SqlCommand CommandoConsultas = new SqlCommand(Query2, con))
                            {
                                CommandoConsultas.Parameters.AddWithValue("@homologo", R.Numfac);
                                CommandoConsultas.Parameters.AddWithValue("@estadoF", "F");
                                CommandoConsultas.Parameters.AddWithValue("@estadoC", "F");
                                CommandoConsultas.Parameters.AddWithValue("@tipo", "Historia");

                                using (SqlDataReader ReaderConsultas = (CommandoConsultas.ExecuteReader()))
                                {
                                    if (ReaderConsultas.HasRows)
                                    {
                                        int consCon = 1;

                                        while (ReaderConsultas.Read() == true)
                                        {
                                            transaccion.usuarios[0].servicios.consultas.Add(new Consultas
                                            {
                                                codPrestador = ReaderConsultas["Com_Cod_Prestador"].ToString(),
                                                fechaInicioAtencion = Convert.ToDateTime(ReaderConsultas["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderConsultas["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                                                numAutorizacion = (ReaderConsultas["Fac_Num_Aut"] == DBNull.Value || ReaderConsultas["Fac_Num_Aut"].ToString() == "" ? "" : ReaderConsultas["Fac_Num_Aut"].ToString()),
                                                codConsulta = ReaderConsultas["Con_CUP"].ToString(),
                                                modalidadGrupoServicioTecSal = (ReaderConsultas["Hor_Pac_Modalidad"] == DBNull.Value || ReaderConsultas["Hor_Pac_Modalidad"].ToString() == "" ? "" : ReaderConsultas["Hor_Pac_Modalidad"].ToString()),
                                                grupoServicios = (ReaderConsultas["Hor_GrupoServicios"] == DBNull.Value || ReaderConsultas["Hor_GrupoServicios"].ToString() == "" ? "01" : ReaderConsultas["Hor_GrupoServicios"].ToString()),
                                                codServicio = Convert.ToInt32(ReaderConsultas["Con_CodServicio"]),
                                                finalidadTecnologiaSalud = "15", //(ReaderConsultas["HorTecnoSalud"] == DBNull.Value || ReaderConsultas["HorTecnoSalud"].ToString() == "" ? "01" : ReaderConsultas["HorTecnoSalud"].ToString()),
                                                causaMotivoAtencion = Convert.ToInt32(ReaderConsultas["Car_CExterna"]).ToString(),
                                                codDiagnosticoPrincipal = (ReaderConsultas["Car_Dx1"] == DBNull.Value || ReaderConsultas["Car_Dx1"].ToString() == "" ? null : ReaderConsultas["Car_Dx1"].ToString()),
                                                codDiagnosticoRelacionado1 = (ReaderConsultas["Car_Dx2"] == DBNull.Value || ReaderConsultas["Car_Dx2"].ToString() == "" ? null : ReaderConsultas["Car_Dx2"].ToString()),
                                                codDiagnosticoRelacionado2 = (ReaderConsultas["Car_Dx3"] == DBNull.Value || ReaderConsultas["Car_Dx3"].ToString() == "" ? null : ReaderConsultas["Car_Dx3"].ToString()),
                                                codDiagnosticoRelacionado3 = null,
                                                tipoDiagnosticoPrincipal = (ReaderConsultas["Car_Imp_Dx"].ToString() == "1" ? "01" : ReaderConsultas["Car_Imp_Dx"].ToString() == "2" ? "02" : "03"),
                                                tipoDocumentoIdentificacion = "CC",
                                                numDocumentoIdentificacion = ReaderConsultas["Bod_Reg_Med"].ToString(),
                                                vrServicio = Convert.ToInt32(ReaderConsultas["Car_val_Tot"]),

                                                conceptoRecaudo = "05",
                                                valorPagoModerador = 0,
                                                numFEVPagoModerador = "",

                                                //conceptoRecaudo = (ReaderConsultas["Hor_RcCaja"] == DBNull.Value || ReaderConsultas["Hor_RcCaja"].ToString() == "" ? "05" : ReaderConsultas["Hor_ConceptoRecaudo"].ToString()),
                                                //valorPagoModerador = (ReaderConsultas["Hor_RcCaja"] == DBNull.Value || ReaderConsultas["Hor_RcCaja"].ToString() == "" ? 0 : Convert.ToInt32(ReaderConsultas["Hor_RcCaja"])),
                                                //numFEVPagoModerador = (ReaderConsultas["Hor_DocFEModerador"] == DBNull.Value || ReaderConsultas["Hor_DocFEModerador"].ToString() == "" ? "" : ReaderConsultas["Hor_DocFEModerador"].ToString()),
                                                consecutivo = consCon
                                            });

                                            consCon++;
                                        }

                                        consCon = 1;
                                    }
                                }
                            }

                            //PROCEDIMIENTOS
                            using (SqlCommand CommandoNotas = new SqlCommand(Query2, con))
                            {
                                CommandoNotas.Parameters.AddWithValue("@homologo", R.Numfac);
                                CommandoNotas.Parameters.AddWithValue("@estadoF", "F");
                                CommandoNotas.Parameters.AddWithValue("@estadoC", "F");
                                CommandoNotas.Parameters.AddWithValue("@tipo", "Nota");

                                using (SqlDataReader ReaderNotas = (CommandoNotas.ExecuteReader()))
                                {
                                    if (ReaderNotas.HasRows)
                                    {
                                        int ConsNota = 1;

                                        while (ReaderNotas.Read() == true)
                                        {
                                            transaccion.usuarios[0].servicios.procedimientos.Add(new Procedimientos
                                            {
                                                codPrestador = ReaderNotas["Com_Cod_Prestador"].ToString(),
                                                fechaInicioAtencion = Convert.ToDateTime(ReaderNotas["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderNotas["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                                                idMIPRES = null,
                                                numAutorizacion = (ReaderNotas["Fac_Num_Aut"] == DBNull.Value || ReaderNotas["Fac_Num_Aut"].ToString() == "" ? "" : ReaderNotas["Fac_Num_Aut"].ToString()),
                                                codProcedimiento = ReaderNotas["Con_CUP"].ToString(),
                                                viaIngresoServicioSalud = "02",
                                                modalidadGrupoServicioTecSal = (ReaderNotas["Hor_Pac_Modalidad"] == DBNull.Value || ReaderNotas["Hor_Pac_Modalidad"].ToString() == "" ? "" : ReaderNotas["Hor_Pac_Modalidad"].ToString()),
                                                grupoServicios = (ReaderNotas["Hor_GrupoServicios"] == DBNull.Value || ReaderNotas["Hor_GrupoServicios"].ToString() == "" ? "01" : ReaderNotas["Hor_GrupoServicios"].ToString()),
                                                codServicio = Convert.ToInt32(ReaderNotas["Con_CodServicio"]),
                                                finalidadTecnologiaSalud = (ReaderNotas["HorTecnoSalud"] == DBNull.Value || ReaderNotas["HorTecnoSalud"].ToString() == "" ? "16" : ReaderNotas["HorTecnoSalud"].ToString()),
                                                tipoDocumentoIdentificacion = "CC",
                                                numDocumentoIdentificacion = ReaderNotas["Bod_Reg_Med"].ToString(),
                                                codDiagnosticoPrincipal = (ReaderNotas["Car_Dx1"] == DBNull.Value || ReaderNotas["Car_Dx1"].ToString() == "" ? null : ReaderNotas["Car_Dx1"].ToString()),
                                                codDiagnosticoRelacionado = (ReaderNotas["Car_Dx2"] == DBNull.Value || ReaderNotas["Car_Dx2"].ToString() == "" ? null : ReaderNotas["Car_Dx2"].ToString()),
                                                codComplicacion = (ReaderNotas["Car_Dx3"] == DBNull.Value || ReaderNotas["Car_Dx3"].ToString() == "" ? null : ReaderNotas["Car_Dx3"].ToString()),
                                                vrServicio = Convert.ToInt32(ReaderNotas["Car_val_Tot"]),

                                                conceptoRecaudo = "05",
                                                valorPagoModerador = 0,
                                                numFEVPagoModerador = "",

                                                //conceptoRecaudo = (ReaderNotas["Hor_RcCaja"] == DBNull.Value || ReaderNotas["Hor_RcCaja"].ToString() == "" ? "05" : ReaderNotas["Hor_ConceptoRecaudo"].ToString()),
                                                //valorPagoModerador = (ReaderNotas["Hor_RcCaja"] == DBNull.Value || ReaderNotas["Hor_RcCaja"].ToString() == "" ? 0 : Convert.ToInt32(ReaderNotas["Hor_RcCaja"])),
                                                //numFEVPagoModerador = ReaderNotas["Hor_DocFEModerador"].ToString(),
                                                consecutivo = ConsNota
                                            });

                                            ConsNota++;
                                        }

                                        ConsNota = 1;
                                    }
                                }
                            }

                            //OTROS SERVICIOS
                            using (SqlCommand CommandoOtros = new SqlCommand(Query3, con))
                            {
                                CommandoOtros.Parameters.AddWithValue("@homologo", R.Numfac);
                                CommandoOtros.Parameters.AddWithValue("@estadoF", "F");
                                CommandoOtros.Parameters.AddWithValue("@estadoC", "F");
                                CommandoOtros.Parameters.AddWithValue("@tipo", "Cargo");

                                using (SqlDataReader ReaderOtros = (CommandoOtros.ExecuteReader()))
                                {
                                    if (ReaderOtros.HasRows)
                                    {
                                        int consServ = 1;

                                        while (ReaderOtros.Read() == true)
                                        {
                                            transaccion.usuarios[0].servicios.otrosServicios.Add(new OtrosServicios
                                            {
                                                codPrestador = ReaderOtros["Com_Cod_Prestador"].ToString(),
                                                numAutorizacion = "",
                                                idMIPRES = null,
                                                fechaSuministroTecnologia = Convert.ToDateTime(ReaderOtros["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderOtros["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                                                tipoOS = "01",
                                                codTecnologiaSalud = ReaderOtros["Car_Cod"].ToString(),
                                                nomTecnologiaSalud = ReaderOtros["Car_Item"].ToString(),
                                                cantidadOS = Convert.ToInt32(ReaderOtros["Cantidad"]),
                                                tipoDocumentoIdentificacion = "CC",
                                                numDocumentoIdentificacion = ReaderOtros["Bod_Reg_Med"].ToString(),
                                                vrUnitOS = Convert.ToInt32(ReaderOtros["Car_val_Un"]),
                                                vrServicio = Convert.ToInt32(ReaderOtros["Total"]),
                                                conceptoRecaudo = "05",
                                                valorPagoModerador = 0,
                                                numFEVPagoModerador = "",
                                                consecutivo = consServ
                                            });

                                            consServ++;
                                        }

                                        consServ = 1;
                                    }
                                }
                            }
                        }
                    }                    

                    return transaccion;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error #", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        #region MINSALUD DOCKER
        //GRUPAL MINSALUD
        TransaccionDocker IRIPSJSON.GenrateTotalMinSalud(int ciaRIPS, string fac, string xmlB64)
        {
            try
            {
                TransaccionDocker transaccion = new TransaccionDocker();
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    con.Open();

                    CXN_FACTURA getFactura = repoFacturacion.getFacElectronica(fac);
                    if (getFactura == null)
                        return null;

                    // Obtener datos del paciente
                    Usuario usuario = ObtenerDatosPaciente(con, getFactura.Fac_Pac);

                    if (usuario == null)
                        return null;

                    transaccion = new TransaccionDocker
                    {
                        rips = new RipsDocker
                        {
                            numDocumentoIdObligado = repoCia.getPrestadorbyCode(getFactura.Fac_Cia).Com_Identificacion,
                            numFactura = fac,
                            tipoNota = null,
                            numNota = null,
                            usuarios = new List<Usuario> { usuario }
                        },
                        xmlFevFile = xmlB64
                    };

                    usuario.servicios.consultas = ObtenerConsultas(con, fac);
                    usuario.servicios.procedimientos = ObtenerProcedimientos(con, fac);
                    usuario.servicios.otrosServicios = ObtenerOtrosServicios(con, fac);
                }

                return transaccion;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error #", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private Usuario ObtenerDatosPaciente(SqlConnection con, int idPaciente)
        {
            string query = "SELECT Pac_Mun_Cod, Pac_PaisOrigen, Pac_Sexo, Pac_Zona, Pac_FechaNto, Pac_IdNum, Pac_TipoId, Pac_Regimen, Pac_Dep_Cod FROM CXN_PACIENTES WHERE Pac_Id = @id_pac";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@id_pac", idPaciente);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario
                        {
                            codMunicipioResidencia = reader["Pac_Dep_Cod"].ToString() + reader["Pac_Mun_Cod"].ToString(),
                            codPaisOrigen = reader["Pac_PaisOrigen"].ToString(),
                            codPaisResidencia = "170",
                            codSexo = reader["Pac_Sexo"].ToString(),
                            codZonaTerritorialResidencia = (reader["Pac_Zona"].ToString() == "R" ? "02" : "01"),
                            consecutivo = 1,
                            fechaNacimiento = Convert.ToDateTime(reader["Pac_FechaNto"]).ToString("yyyy-MM-dd"),
                            incapacidad = "NO",
                            numDocumentoIdentificacion = reader["Pac_IdNum"].ToString(),
                            tipoDocumentoIdentificacion = repoPacientes.getTipoDoc(reader["Pac_TipoId"].ToString()),
                            tipoUsuario = reader["Pac_Regimen"].ToString(),
                            servicios = new Servicios()
                        };
                    }
                }
            }

            return null;
        }
        private List<Consultas> ObtenerConsultas(SqlConnection con, string fac)
        {
            List<Consultas> consultas = new List<Consultas>();

            string query = "WITH CTE AS ( " +
                                            "SELECT H.Hor_id, CIA.Com_Cod_Prestador, C.Car_Fecha, H.Hor_Pac_Hora_Cita, F.Fac_Num_Aut, H.Hor_Pac_Modalidad, " +
                                            "H.Hor_GrupoServicios, CO.Con_CodServicio, H.HorTecnoSalud, C.Car_CExterna, C.Car_Dx1, C.Car_Dx2, C.Car_Dx3, " +
                                            "C.Car_Imp_Dx, B.Bod_Reg_Med, C.Car_val_Tot, H.Hor_RcCaja, H.Hor_ConceptoRecaudo, H.Hor_DocFEModerador, " +
                                            "C.Car_Cod, CO.Con_CUP, " +
                                            "ROW_NUMBER() OVER (PARTITION BY H.Hor_id ORDER BY H.Hor_id) AS rn  " +
                                            "FROM " +
                                            "CXN_CARGOS C " +
                                            "INNER JOIN " +
                                            "CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                            "INNER JOIN " +
                                            "CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                            "INNER JOIN " +
                                            "CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero " +
                                            "INNER JOIN " +
                                            "CXN_CONVENIOS CO ON C.Car_Cod = CO.Con_Id_Serv " +
                                            "INNER JOIN " +
                                            "CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador " +
                                            "AND C.Car_Ase = CO.Con_Aseguradora " +
                                            "WHERE " +
                                            "F.Homologo = @homologo " +
                                            "AND F.Fac_Estado = @estadoF " +
                                            "AND C.Car_Estado = @estadoC " +
                                            "AND C.Car_Tipo = @tipo) " +
                                            "SELECT * " +
                                            "FROM CTE " +
                                            "WHERE rn = 1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@homologo", fac);
                cmd.Parameters.AddWithValue("@estadoF", "F");
                cmd.Parameters.AddWithValue("@estadoC", "F");
                cmd.Parameters.AddWithValue("@tipo", "Historia");

                using (SqlDataReader ReaderConsultas = cmd.ExecuteReader())
                {
                    int consecutivo = 1;
                    while (ReaderConsultas.Read())
                    {
                        consultas.Add(new Consultas
                        {
                            codPrestador = ReaderConsultas["Com_Cod_Prestador"].ToString(),
                            fechaInicioAtencion = Convert.ToDateTime(ReaderConsultas["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderConsultas["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                            numAutorizacion = (ReaderConsultas["Fac_Num_Aut"] == DBNull.Value || ReaderConsultas["Fac_Num_Aut"].ToString() == "" ? "" : ReaderConsultas["Fac_Num_Aut"].ToString()),
                            codConsulta = ReaderConsultas["Con_CUP"].ToString(),
                            modalidadGrupoServicioTecSal = (ReaderConsultas["Hor_Pac_Modalidad"] == DBNull.Value || ReaderConsultas["Hor_Pac_Modalidad"].ToString() == "" ? "" : ReaderConsultas["Hor_Pac_Modalidad"].ToString()),
                            grupoServicios = (ReaderConsultas["Hor_GrupoServicios"] == DBNull.Value || ReaderConsultas["Hor_GrupoServicios"].ToString() == "" ? "01" : ReaderConsultas["Hor_GrupoServicios"].ToString()),
                            codServicio = Convert.ToInt32(ReaderConsultas["Con_CodServicio"]),
                            finalidadTecnologiaSalud = "15", //(ReaderConsultas["HorTecnoSalud"] == DBNull.Value || ReaderConsultas["HorTecnoSalud"].ToString() == "" ? "01" : ReaderConsultas["HorTecnoSalud"].ToString()),
                            causaMotivoAtencion =  string.IsNullOrEmpty(ReaderConsultas["Car_CExterna"].ToString()) || Convert.ToInt32(ReaderConsultas["Car_CExterna"]) == 0 ? "38" : Convert.ToInt32(ReaderConsultas["Car_CExterna"]).ToString(),
                            codDiagnosticoPrincipal = (ReaderConsultas["Car_Dx1"] == DBNull.Value || ReaderConsultas["Car_Dx1"].ToString() == "" ? null : ReaderConsultas["Car_Dx1"].ToString()),
                            codDiagnosticoRelacionado1 = (ReaderConsultas["Car_Dx2"] == DBNull.Value || ReaderConsultas["Car_Dx2"].ToString() == "" ? null : ReaderConsultas["Car_Dx2"].ToString()),
                            codDiagnosticoRelacionado2 = (ReaderConsultas["Car_Dx3"] == DBNull.Value || ReaderConsultas["Car_Dx3"].ToString() == "" ? null : ReaderConsultas["Car_Dx3"].ToString()),
                            codDiagnosticoRelacionado3 = null,
                            tipoDiagnosticoPrincipal = (ReaderConsultas["Car_Imp_Dx"].ToString() == "1" ? "01" : ReaderConsultas["Car_Imp_Dx"].ToString() == "2" ? "02" : "03"),
                            tipoDocumentoIdentificacion = "CC",
                            numDocumentoIdentificacion = ReaderConsultas["Bod_Reg_Med"].ToString(),                           
                            vrServicio = Convert.ToInt32(ReaderConsultas["Car_val_Tot"]),
                          
                            //conceptoRecaudo = (ReaderConsultas["Hor_RcCaja"] == DBNull.Value || ReaderConsultas["Hor_RcCaja"].ToString() == "" ? "05" : ReaderConsultas["Hor_ConceptoRecaudo"].ToString()),
                            conceptoRecaudo = "05",
                            //valorPagoModerador = (ReaderConsultas["Hor_RcCaja"] == DBNull.Value || ReaderConsultas["Hor_RcCaja"].ToString() == "" ? 0 : Convert.ToInt32(ReaderConsultas["Hor_RcCaja"])),
                            valorPagoModerador = 0,
                            //numFEVPagoModerador = (ReaderConsultas["Hor_DocFEModerador"] == DBNull.Value || ReaderConsultas["Hor_DocFEModerador"].ToString() == "" ? "" : ReaderConsultas["Hor_DocFEModerador"].ToString()),
                            numFEVPagoModerador = "",
                           
                            consecutivo = consecutivo
                        });

                        consecutivo++;
                    }
                }
            }

            return consultas;
        }
        private List<Procedimientos> ObtenerProcedimientos(SqlConnection con, string fac)
        {
            List<Procedimientos> procedimientos = new List<Procedimientos>();

            string query = "WITH CTE AS ( " +
                                            "SELECT H.Hor_id, CIA.Com_Cod_Prestador, C.Car_Fecha, H.Hor_Pac_Hora_Cita, F.Fac_Num_Aut, H.Hor_Pac_Modalidad, " +
                                            "H.Hor_GrupoServicios, CO.Con_CodServicio, H.HorTecnoSalud, C.Car_CExterna, C.Car_Dx1, C.Car_Dx2, C.Car_Dx3, " +
                                            "C.Car_Imp_Dx, B.Bod_Reg_Med, C.Car_val_Tot, H.Hor_RcCaja, H.Hor_ConceptoRecaudo, H.Hor_DocFEModerador, " +
                                            "C.Car_Cod, CO.Con_CUP, " +
                                            "ROW_NUMBER() OVER (PARTITION BY H.Hor_id ORDER BY H.Hor_id) AS rn  " +
                                            "FROM " +
                                            "CXN_CARGOS C " +
                                            "INNER JOIN " +
                                            "CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                            "INNER JOIN " +
                                            "CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                            "INNER JOIN " +
                                            "CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero " +
                                            "INNER JOIN " +
                                            "CXN_CONVENIOS CO ON C.Car_Cod = CO.Con_Id_Serv " +
                                            "INNER JOIN " +
                                            "CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador " +
                                            "AND C.Car_Ase = CO.Con_Aseguradora " +
                                            "WHERE " +
                                            "F.Homologo = @homologo " +
                                            "AND F.Fac_Estado = @estadoF " +
                                            "AND C.Car_Estado = @estadoC " +
                                            "AND C.Car_Tipo = @tipo) " +
                                            "SELECT * " +
                                            "FROM CTE " +
                                            "WHERE rn = 1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@homologo", fac);
                cmd.Parameters.AddWithValue("@estadoF", "F");
                cmd.Parameters.AddWithValue("@estadoC", "F");
                cmd.Parameters.AddWithValue("@tipo", "Nota");

                using (SqlDataReader ReaderNotas = cmd.ExecuteReader())
                {
                    int consecutivo = 1;
                    while (ReaderNotas.Read())
                    {
                        procedimientos.Add(new Procedimientos
                        {
                            codPrestador = ReaderNotas["Com_Cod_Prestador"].ToString(),
                            fechaInicioAtencion = Convert.ToDateTime(ReaderNotas["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderNotas["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                            idMIPRES = null,
                            numAutorizacion = (ReaderNotas["Fac_Num_Aut"] == DBNull.Value || ReaderNotas["Fac_Num_Aut"].ToString() == "" ? "" : ReaderNotas["Fac_Num_Aut"].ToString()),
                            codProcedimiento = ReaderNotas["Con_CUP"].ToString(),
                            viaIngresoServicioSalud = "02",
                            modalidadGrupoServicioTecSal = (ReaderNotas["Hor_Pac_Modalidad"] == DBNull.Value || ReaderNotas["Hor_Pac_Modalidad"].ToString() == "" ? "" : ReaderNotas["Hor_Pac_Modalidad"].ToString()),
                            grupoServicios = (ReaderNotas["Hor_GrupoServicios"] == DBNull.Value || ReaderNotas["Hor_GrupoServicios"].ToString() == "" ? "01" : ReaderNotas["Hor_GrupoServicios"].ToString()),
                            codServicio = Convert.ToInt32(ReaderNotas["Con_CodServicio"]),
                            finalidadTecnologiaSalud = (ReaderNotas["HorTecnoSalud"] == DBNull.Value || ReaderNotas["HorTecnoSalud"].ToString() == "" ? "16" : ReaderNotas["HorTecnoSalud"].ToString()),
                            tipoDocumentoIdentificacion = "CC",
                            numDocumentoIdentificacion = ReaderNotas["Bod_Reg_Med"].ToString(),
                            codDiagnosticoPrincipal = (ReaderNotas["Car_Dx1"] == DBNull.Value || ReaderNotas["Car_Dx1"].ToString() == "" ? null : ReaderNotas["Car_Dx1"].ToString()),
                            codDiagnosticoRelacionado = (ReaderNotas["Car_Dx2"] == DBNull.Value || ReaderNotas["Car_Dx2"].ToString() == "" ? null : ReaderNotas["Car_Dx2"].ToString()),
                            codComplicacion = (ReaderNotas["Car_Dx3"] == DBNull.Value || ReaderNotas["Car_Dx3"].ToString() == "" ? null : ReaderNotas["Car_Dx3"].ToString()),
                            vrServicio = Convert.ToInt32(ReaderNotas["Car_val_Tot"]),
                          
                            //conceptoRecaudo = (ReaderNotas["Hor_RcCaja"] == DBNull.Value || ReaderNotas["Hor_RcCaja"].ToString() == "" ? "05" : ReaderNotas["Hor_ConceptoRecaudo"].ToString()),
                            //valorPagoModerador = (ReaderNotas["Hor_RcCaja"] == DBNull.Value || ReaderNotas["Hor_RcCaja"].ToString() == "" ? 0 : Convert.ToInt32(ReaderNotas["Hor_RcCaja"])),
                            //numFEVPagoModerador = ReaderNotas["Hor_DocFEModerador"].ToString(),
                            conceptoRecaudo = "05",
                            valorPagoModerador = 0,
                            numFEVPagoModerador = "",

                            consecutivo = consecutivo
                        });

                        consecutivo++;
                    }
                }
            }

            return procedimientos;
        }
        private List<OtrosServicios> ObtenerOtrosServicios(SqlConnection con, string fac)
        {
            List<OtrosServicios> otrosServicios = new List<OtrosServicios>();

            string query = "SELECT C.Car_Cant AS Cantidad, C.Car_Val_Tot AS Total, C.Car_Cod, C.Car_Val_Un, C.Car_Fecha, H.Hor_Pac_Hora_Cita, C.Car_Item, B.Bod_Reg_Med, I.InvCodBar, CIA.Com_Cod_Prestador " +
                                       "FROM CXN_CARGOS C   " +
                                       "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac  " +
                                       "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id   " +
                                       "INNER JOIN CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero  " +
                                       "INNER JOIN CXN_INVENTARIO I ON C.Car_Cod = I.InvCod  " +
                                        "INNER JOIN CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador  " +
                                       "AND C.Car_Ase = I.InvConvenio   " +
                                       "WHERE F.Homologo = @homologo   " +
                                       "AND F.Fac_Estado = @estadoF   " +
                                       "AND C.Car_Estado = @estadoC   " +
                                       "AND C.Car_Tipo = @tipo";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@homologo", fac);
                cmd.Parameters.AddWithValue("@estadoF", "F");
                cmd.Parameters.AddWithValue("@estadoC", "F");
                cmd.Parameters.AddWithValue("@tipo", "Cargo");

                using (SqlDataReader ReaderOtros = cmd.ExecuteReader())
                {
                    int consecutivo = 1;
                    while (ReaderOtros.Read())
                    {
                        otrosServicios.Add(new OtrosServicios
                        {
                            codPrestador = ReaderOtros["Com_Cod_Prestador"].ToString(),
                            numAutorizacion = "",
                            idMIPRES = null,
                            fechaSuministroTecnologia = Convert.ToDateTime(ReaderOtros["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderOtros["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                            tipoOS = "01",
                            codTecnologiaSalud = ReaderOtros["Car_Cod"].ToString(),
                            nomTecnologiaSalud = ReaderOtros["Car_Item"].ToString(),
                            cantidadOS = Convert.ToInt32(ReaderOtros["Cantidad"]),
                            tipoDocumentoIdentificacion = "CC",
                            numDocumentoIdentificacion = ReaderOtros["Bod_Reg_Med"].ToString(),
                            vrUnitOS = Convert.ToInt32(ReaderOtros["Car_val_Un"]),
                            vrServicio = Convert.ToInt32(ReaderOtros["Total"]),
                            conceptoRecaudo = "05",
                            valorPagoModerador = 0,
                            numFEVPagoModerador = "",
                            consecutivo = consecutivo
                        });

                        consecutivo++;
                    }
                }
            }

            return otrosServicios;
        }
        #endregion

        //COBERTURAS
        string getCodeCobertura(string Texto)
        {
            switch (Texto)
            {
                case "02 - Presupuesto máximo":
                    return "02";
                case "03 - Prima EPS / EOC, no asegurados SOAT":
                    return "03";
                case "04 - Cobertura Póliza SOAT":
                    return "04";
                case "05 - Cobertura ARL":
                    return "05";
                case "06 - Cobertura ADRES":
                    return "06";
                case "07 - Cobertura Salud Pública":
                    return "07";
                case "08 - Cobertura entidad territorial, recursos de oferta":
                    return "08";
                case "09 - Urgencias población migrante":
                    return "09";
                case "10 - Plan complementario en salud":
                    return "10";
                case "11 - Plan medicina prepagada":
                    return "11";
                case "12 - Pólizas en salud":
                    return "12";
                case "13 - Cobertura Régimen Especial o Excepción":
                    return "13";
                case "14 - Cobertura Fondo Nacional de Salud de las Personas Privadas de la Libertad":
                    return "14";
                case "15 - Particular":
                    return "15";
                case "16 - Plan de beneficios en Salud dinanciado con UPC contributivo":
                    return "16";
                case "17 - Plan de beneficios en Salud dinanciado con UPC subsidiado":
                    return "17";
                default:
                    return "16";
            }
        }


        //FIN COBERTURAS

        //GRUPAL
        Dictionary<string, Transaccion> IRIPSJSON.GenrateTotal(RIPS_Class R)
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

                    Dictionary<string, Transaccion> D = new Dictionary<string, Transaccion>();

                    List<CXN_FACTURA> getFactura = getFacturas(R.aseRIPS, R.ciaRIPS, R.tDocumentRIPS, R.desdeRIPS, R.hastaRIPS); 
                    if (getFactura != null)
                    {
                        foreach (CXN_FACTURA f in getFactura)
                        {
                            //USUARIO
                            string Query = "SELECT Pac_Mun_Cod,  Pac_PaisOrigen, Pac_Residencia, Pac_Sexo, Pac_Zona, Pac_FechaNto, Pac_IdNum, Pac_TipoId, Pac_Regimen, Pac_Dep_Cod " +
                                        "FROM CXN_PACIENTES WHERE Pac_Id = @id_pac";

                            string codMunicipioResidenciaU = "";
                            string codPaisOrigenU = "";
                            string codPaisResidenciaU = "";
                            string codSexoU = "";
                            string codZonaTerritorialResidenciaU = "";
                            int consecutivoU = 1;
                            string fechaNacimientoU = "";
                            string incapacidadU = "";
                            string numDocumentoIdentificacionU = "";
                            string tipoDocumentoIdentificacionU = "";
                            string tipoUsuarioU = R.claseRIPS == "EPS" ? "01" : getCodeCobertura(f.Cobertura); //tabla cobertura

                            //USUARIOS
                            using (SqlCommand Commando = new SqlCommand(Query, con))
                            {
                                Commando.Parameters.AddWithValue("@id_pac", f.Fac_Pac);

                                using (SqlDataReader Reader = (Commando.ExecuteReader()))
                                {
                                    if (Reader.Read() == true) 
                                    {
                                        codMunicipioResidenciaU = Reader["Pac_Dep_Cod"].ToString() + Reader["Pac_Mun_Cod"].ToString();
                                        codPaisOrigenU = Reader["Pac_PaisOrigen"].ToString();
                                        codPaisResidenciaU = Reader["Pac_Residencia"].ToString();
                                        codSexoU = Reader["Pac_Sexo"].ToString();
                                        codZonaTerritorialResidenciaU = (Reader["Pac_Zona"].ToString() == "R" ? "02" : "01");
                                        consecutivoU = 1;
                                        fechaNacimientoU = Convert.ToDateTime(Reader["Pac_FechaNto"]).ToString("yyyy-MM-dd");
                                        incapacidadU = "NO";
                                        numDocumentoIdentificacionU = Reader["Pac_IdNum"].ToString();
                                        tipoDocumentoIdentificacionU = repoPacientes.getTipoDoc(Reader["Pac_TipoId"].ToString());
                                        //tipoUsuarioU = Reader["Pac_Regimen"].ToString(); //COBERTURA
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }

                            Transaccion transaccion = new Transaccion
                            {
                                numDocumentoIdObligado = repoCia.getPrestadorbyCode(R.ciaRIPS).Com_Identificacion,
                                numFactura = f.Homologo,
                                tipoNota = null,
                                numNota = null,
                                usuarios = new List<Usuario>  // Inicializamos la lista de usuarios
                                    {
                                        new Usuario
                                        {
                                             codMunicipioResidencia = codMunicipioResidenciaU,
                                             codPaisOrigen = codPaisOrigenU,
                                             codPaisResidencia = codPaisResidenciaU,
                                             codSexo = codSexoU,
                                             codZonaTerritorialResidencia = codZonaTerritorialResidenciaU,
                                             consecutivo = consecutivoU,
                                             fechaNacimiento = Convert.ToDateTime(fechaNacimientoU).ToString("yyyy-MM-dd"),
                                             incapacidad = incapacidadU,
                                             numDocumentoIdentificacion = numDocumentoIdentificacionU,
                                             tipoDocumentoIdentificacion = tipoDocumentoIdentificacionU,
                                             tipoUsuario = tipoUsuarioU,

                                            servicios = new Servicios
                                            {
                                                consultas = new List<Consultas>(),        // Inicializa la lista de Consultas
                                                procedimientos = new List<Procedimientos>(), // Inicializa la lista de Procedimientos
                                                otrosServicios = new List<OtrosServicios>()   // Inicializa la lista de OtrosServicios
                                            }
                                        }
                                    }
                            };


                            if (getFactura != null)
                            {
                                //CONSULTA Y PROCEDIMIENTO
                                /*string Query2 = "SELECT CIA.Com_Cod_Prestador, C.Car_Fecha, H.Hor_Pac_Hora_Cita, F.Fac_Num_Aut, CO.Con_CUP, H.Hor_Pac_Modalidad,   " +
                                            "H.Hor_GrupoServicios, CO.Con_CodServicio, H.HorTecnoSalud, C.Car_CExterna, C.Car_Dx1, C.Car_Dx2, C.Car_Dx3, C.Car_Imp_Dx, B.Bod_Reg_Med, C.Car_val_Tot, " +
                                            "H.Hor_RcCaja, H.Hor_ConceptoRecaudo, H.Hor_DocFEModerador, C.Car_Cod, CO.Con_CUP  " +
                                            "FROM CXN_CARGOS C   " +
                                            "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac   " +
                                            "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id   " +
                                            "INNER JOIN CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero   " +
                                            "INNER JOIN CXN_CONVENIOS CO ON C.Car_Cod = CO.Con_Id_Serv   " +
                                            "INNER JOIN CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador  " +
                                            "AND C.Car_Ase = CO.Con_Aseguradora   " +
                                            "WHERE F.Homologo = @homologo   " +
                                            "AND F.Fac_Estado = @estadoF   " +
                                            "AND C.Car_Estado = @estadoC   " +
                                            "AND C.Car_Tipo = @tipo";*/

                                string Query2 = "WITH CTE AS ( " +
                                                "SELECT H.Hor_id, CIA.Com_Cod_Prestador, C.Car_Fecha, H.Hor_Pac_Hora_Cita, F.Fac_Num_Aut, H.Hor_Pac_Modalidad, " +
                                                "H.Hor_GrupoServicios, CO.Con_CodServicio, H.HorTecnoSalud, C.Car_CExterna, C.Car_Dx1, C.Car_Dx2, C.Car_Dx3, " +
                                                "C.Car_Imp_Dx, B.Bod_Reg_Med, C.Car_val_Tot, H.Hor_RcCaja, H.Hor_ConceptoRecaudo, H.Hor_DocFEModerador, " +
                                                "C.Car_Cod, CO.Con_CUP, " +
                                                "ROW_NUMBER() OVER (PARTITION BY H.Hor_id ORDER BY H.Hor_id) AS rn  " +
                                                "FROM " +
                                                "CXN_CARGOS C " +
                                                "INNER JOIN " +
                                                "CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac " +
                                                "INNER JOIN " +
                                                "CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                                "INNER JOIN " +
                                                "CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero " +
                                                "INNER JOIN " +
                                                "CXN_CONVENIOS CO ON C.Car_Cod = CO.Con_Id_Serv " +
                                                "INNER JOIN " +
                                                "CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador " +
                                                "AND C.Car_Ase = CO.Con_Aseguradora " +
                                                "WHERE " +
                                                "F.Homologo = @homologo " +
                                                "AND F.Fac_Estado = @estadoF " +
                                                "AND C.Car_Estado = @estadoC " +
                                                "AND C.Car_Tipo = @tipo) " +
                                                "SELECT * " +
                                                "FROM CTE " +
                                                "WHERE rn = 1";

                                //OTROS SERVICIOS
                                string Query3 = "SELECT C.Car_Cant AS Cantidad, C.Car_Val_Tot AS Total, C.Car_Cod, C.Car_Val_Un, C.Car_Fecha, H.Hor_Pac_Hora_Cita, C.Car_Item, B.Bod_Reg_Med, I.InvCodBar, CIA.Com_Cod_Prestador " +
                                           "FROM CXN_CARGOS C   " +
                                           "INNER JOIN CXN_FACTURA F ON C.Car_Factura = F.Fac_Num_Fac  " +
                                           "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id   " +
                                           "INNER JOIN CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero  " +
                                           "INNER JOIN CXN_INVENTARIO I ON C.Car_Cod = I.InvCod  " +
                                            "INNER JOIN CXN_CIA CIA ON F.Fac_Cia = CIA.Com_Identificador  " +
                                           "AND C.Car_Ase = I.InvConvenio   " +
                                           "WHERE F.Homologo = @homologo   " +
                                           "AND F.Fac_Estado = @estadoF   " +
                                           "AND C.Car_Estado = @estadoC   " +
                                           "AND C.Car_Tipo = @tipo";


                                //CONSULTAS
                                using (SqlCommand CommandoConsultas = new SqlCommand(Query2, con))
                                {
                                    CommandoConsultas.Parameters.AddWithValue("@homologo", f.Homologo);
                                    CommandoConsultas.Parameters.AddWithValue("@estadoF", "F");
                                    CommandoConsultas.Parameters.AddWithValue("@estadoC", "F");
                                    CommandoConsultas.Parameters.AddWithValue("@tipo", "Historia");

                                    using (SqlDataReader ReaderConsultas = (CommandoConsultas.ExecuteReader()))
                                    {
                                        if (ReaderConsultas.HasRows)
                                        {
                                            int consCon = 1;

                                            while (ReaderConsultas.Read() == true)
                                            {
                                                transaccion.usuarios[0].servicios.consultas.Add(new Consultas
                                                {
                                                    codPrestador = ReaderConsultas["Com_Cod_Prestador"].ToString(),
                                                    fechaInicioAtencion = Convert.ToDateTime(ReaderConsultas["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderConsultas["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                                                    numAutorizacion = (ReaderConsultas["Fac_Num_Aut"] == DBNull.Value || ReaderConsultas["Fac_Num_Aut"].ToString() == "" ? "" : ReaderConsultas["Fac_Num_Aut"].ToString()),
                                                    codConsulta = ReaderConsultas["Con_CUP"].ToString(),
                                                    modalidadGrupoServicioTecSal = (ReaderConsultas["Hor_Pac_Modalidad"] == DBNull.Value || ReaderConsultas["Hor_Pac_Modalidad"].ToString() == "" ? "" : ReaderConsultas["Hor_Pac_Modalidad"].ToString()),
                                                    grupoServicios = (ReaderConsultas["Hor_GrupoServicios"] == DBNull.Value || ReaderConsultas["Hor_GrupoServicios"].ToString() == "" ? "01" : ReaderConsultas["Hor_GrupoServicios"].ToString()),
                                                    codServicio = Convert.ToInt32(ReaderConsultas["Con_CodServicio"]),
                                                    finalidadTecnologiaSalud = "15", //(ReaderConsultas["HorTecnoSalud"] == DBNull.Value || ReaderConsultas["HorTecnoSalud"].ToString() == "" ? "01" : ReaderConsultas["HorTecnoSalud"].ToString()),
                                                    causaMotivoAtencion = Convert.ToInt32(ReaderConsultas["Car_CExterna"]).ToString(),
                                                    codDiagnosticoPrincipal = (ReaderConsultas["Car_Dx1"] == DBNull.Value || ReaderConsultas["Car_Dx1"].ToString() == "" ? null : ReaderConsultas["Car_Dx1"].ToString()),
                                                    codDiagnosticoRelacionado1 = (ReaderConsultas["Car_Dx2"] == DBNull.Value || ReaderConsultas["Car_Dx2"].ToString() == "" ? null : ReaderConsultas["Car_Dx2"].ToString()),
                                                    codDiagnosticoRelacionado2 = (ReaderConsultas["Car_Dx3"] == DBNull.Value || ReaderConsultas["Car_Dx3"].ToString() == "" ? null : ReaderConsultas["Car_Dx3"].ToString()),
                                                    codDiagnosticoRelacionado3 = null,
                                                    tipoDiagnosticoPrincipal = (ReaderConsultas["Car_Imp_Dx"].ToString() == "1" ? "01" : ReaderConsultas["Car_Imp_Dx"].ToString() == "2" ? "02" : "03"),
                                                    tipoDocumentoIdentificacion = "CC",
                                                    numDocumentoIdentificacion = ReaderConsultas["Bod_Reg_Med"].ToString(),
                                                    vrServicio = Convert.ToInt32(ReaderConsultas["Car_val_Tot"]),
                                                    conceptoRecaudo = (ReaderConsultas["Hor_RcCaja"] == DBNull.Value || ReaderConsultas["Hor_RcCaja"].ToString() == "" ? "05" : ReaderConsultas["Hor_ConceptoRecaudo"].ToString()),
                                                    valorPagoModerador = (ReaderConsultas["Hor_RcCaja"] == DBNull.Value || ReaderConsultas["Hor_RcCaja"].ToString() == "" ? 0 : Convert.ToInt32(ReaderConsultas["Hor_RcCaja"])),
                                                    numFEVPagoModerador = (ReaderConsultas["Hor_DocFEModerador"] == DBNull.Value || ReaderConsultas["Hor_DocFEModerador"].ToString() == "" ? "" : ReaderConsultas["Hor_DocFEModerador"].ToString()),
                                                    consecutivo = consCon
                                                });

                                                consCon++;
                                            }

                                            consCon = 1;
                                        }
                                    }
                                }

                                //PROCEDIMIENTOS
                                using (SqlCommand CommandoNotas = new SqlCommand(Query2, con))
                                {
                                    CommandoNotas.Parameters.AddWithValue("@homologo", f.Homologo);
                                    CommandoNotas.Parameters.AddWithValue("@estadoF", "F");
                                    CommandoNotas.Parameters.AddWithValue("@estadoC", "F");
                                    CommandoNotas.Parameters.AddWithValue("@tipo", "Nota");

                                    using (SqlDataReader ReaderNotas = (CommandoNotas.ExecuteReader()))
                                    {
                                        if (ReaderNotas.HasRows)
                                        {
                                            int ConsNota = 1;

                                            while (ReaderNotas.Read() == true)
                                            {
                                                transaccion.usuarios[0].servicios.procedimientos.Add(new Procedimientos
                                                {
                                                    codPrestador = ReaderNotas["Com_Cod_Prestador"].ToString(),
                                                    fechaInicioAtencion = Convert.ToDateTime(ReaderNotas["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderNotas["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                                                    idMIPRES = null,
                                                    numAutorizacion = (ReaderNotas["Fac_Num_Aut"] == DBNull.Value || ReaderNotas["Fac_Num_Aut"].ToString() == "" ? "" : ReaderNotas["Fac_Num_Aut"].ToString()),
                                                    codProcedimiento = ReaderNotas["Con_CUP"].ToString(),
                                                    viaIngresoServicioSalud = "02",
                                                    modalidadGrupoServicioTecSal = (ReaderNotas["Hor_Pac_Modalidad"] == DBNull.Value || ReaderNotas["Hor_Pac_Modalidad"].ToString() == "" ? "" : ReaderNotas["Hor_Pac_Modalidad"].ToString()),
                                                    grupoServicios = (ReaderNotas["Hor_GrupoServicios"] == DBNull.Value || ReaderNotas["Hor_GrupoServicios"].ToString() == "" ? "01" : ReaderNotas["Hor_GrupoServicios"].ToString()),
                                                    codServicio = Convert.ToInt32(ReaderNotas["Con_CodServicio"]),
                                                    finalidadTecnologiaSalud = (ReaderNotas["HorTecnoSalud"] == DBNull.Value || ReaderNotas["HorTecnoSalud"].ToString() == "" ? "16" : ReaderNotas["HorTecnoSalud"].ToString()),
                                                    tipoDocumentoIdentificacion = "CC",
                                                    numDocumentoIdentificacion = ReaderNotas["Bod_Reg_Med"].ToString(),
                                                    codDiagnosticoPrincipal = (ReaderNotas["Car_Dx1"] == DBNull.Value || ReaderNotas["Car_Dx1"].ToString() == "" ? null : ReaderNotas["Car_Dx1"].ToString()),
                                                    codDiagnosticoRelacionado = (ReaderNotas["Car_Dx2"] == DBNull.Value || ReaderNotas["Car_Dx2"].ToString() == "" ? null : ReaderNotas["Car_Dx2"].ToString()),
                                                    codComplicacion = (ReaderNotas["Car_Dx3"] == DBNull.Value || ReaderNotas["Car_Dx3"].ToString() == "" ? null : ReaderNotas["Car_Dx3"].ToString()),
                                                    vrServicio = Convert.ToInt32(ReaderNotas["Car_val_Tot"]),
                                                    conceptoRecaudo = (ReaderNotas["Hor_RcCaja"] == DBNull.Value || ReaderNotas["Hor_RcCaja"].ToString() == "" ? "05" : ReaderNotas["Hor_ConceptoRecaudo"].ToString()),
                                                    valorPagoModerador = (ReaderNotas["Hor_RcCaja"] == DBNull.Value || ReaderNotas["Hor_RcCaja"].ToString() == "" ? 0 : Convert.ToInt32(ReaderNotas["Hor_RcCaja"])),
                                                    numFEVPagoModerador = ReaderNotas["Hor_DocFEModerador"].ToString(),
                                                    consecutivo = ConsNota
                                                });

                                                ConsNota++;
                                            }

                                            ConsNota = 1;
                                        }
                                    }
                                }

                                //OTROS SERVICIOS
                                using (SqlCommand CommandoOtros = new SqlCommand(Query3, con))
                                {
                                    CommandoOtros.Parameters.AddWithValue("@homologo", f.Homologo);
                                    CommandoOtros.Parameters.AddWithValue("@estadoF", "F");
                                    CommandoOtros.Parameters.AddWithValue("@estadoC", "F");
                                    CommandoOtros.Parameters.AddWithValue("@tipo", "Cargo");

                                    using (SqlDataReader ReaderOtros = (CommandoOtros.ExecuteReader()))
                                    {
                                        if (ReaderOtros.HasRows)
                                        {
                                            int consServ = 1;

                                            while (ReaderOtros.Read() == true)
                                            {
                                                transaccion.usuarios[0].servicios.otrosServicios.Add(new OtrosServicios
                                                {
                                                    codPrestador = ReaderOtros["Com_Cod_Prestador"].ToString(),
                                                    numAutorizacion = "",
                                                    idMIPRES = null,
                                                    fechaSuministroTecnologia = Convert.ToDateTime(ReaderOtros["Car_Fecha"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ReaderOtros["Hor_Pac_Hora_Cita"]).ToString("hh:mm"),
                                                    tipoOS = "01",
                                                    codTecnologiaSalud = ReaderOtros["Car_Cod"].ToString(),
                                                    nomTecnologiaSalud = ReaderOtros["Car_Item"].ToString(),
                                                    cantidadOS = Convert.ToInt32(ReaderOtros["Cantidad"]),
                                                    tipoDocumentoIdentificacion = "CC",
                                                    numDocumentoIdentificacion = ReaderOtros["Bod_Reg_Med"].ToString(),
                                                    vrUnitOS = Convert.ToInt32(ReaderOtros["Car_val_Un"]),
                                                    vrServicio = Convert.ToInt32(ReaderOtros["Total"]),
                                                    conceptoRecaudo = "05",
                                                    valorPagoModerador = 0,
                                                    numFEVPagoModerador = "",
                                                    consecutivo = consServ
                                                });

                                                consServ++;
                                            }

                                            consServ = 1;
                                        }
                                    }
                                }

                                D.Add(f.Homologo.ToString(), transaccion);
                            }
                        }

                        return D;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error #", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        //Obtener facturas grupales
        List<CXN_FACTURA> getFacturas(int Ase, int Cia, string Tipo, DateTime Desde, DateTime Hasta)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Homologo, Fac_Pac, Fac_ConSub, Fac_Num_Fac, Cobertura " +
                            "FROM CXN_FACTURA " +
                            "WHERE Fac_Ase = @param1 " +
                            "AND Fac_Cia = @param2 " +
                            "AND Fac_Estado = @param3 " +
                            "AND Fac_Tipo_Doc = @param4 " +
                            "AND Fac_Fecha BETWEEN @param5 AND @param6";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Ase);
                        Commando.Parameters.AddWithValue("@param2", Cia);
                        Commando.Parameters.AddWithValue("@param3", "F");
                        Commando.Parameters.AddWithValue("@param4", Tipo);
                        Commando.Parameters.Add(new SqlParameter("@param5", SqlDbType.DateTime)).Value = Convert.ToDateTime(Desde);
                        Commando.Parameters.Add(new SqlParameter("@param6", SqlDbType.DateTime)).Value = Convert.ToDateTime(Hasta);

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            List<CXN_FACTURA> F = new List<CXN_FACTURA>();

                            if (leer.HasRows)
                            {
                                while (leer.Read() == true)
                                {
                                    F.Add(new CXN_FACTURA 
                                    {
                                        Homologo = leer["Homologo"].ToString(),
                                        Fac_Pac = Convert.ToInt32(leer["Fac_Pac"]),
                                        Fac_ConSub = leer["Fac_ConSub"].ToString(),
                                        Fac_Num_Fac = Convert.ToInt32(leer["Fac_Num_Fac"]),
                                        Cobertura = leer["Cobertura"].ToString()
                                    });
                                }

                                return F;
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

        //Obtener Factura                
        CXN_FACTURA getFacturas(int Ase, int Cia, string Tipo, string Fac)
        {
            try
            {
                Dictionary<string, string> getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Homologo, Fac_Pac, Fac_ConSub, Cobertura " +
                            "FROM CXN_FACTURA " +
                            "WHERE Fac_Ase = @param1 " +
                            "AND Fac_Cia = @param2 " +
                            "AND Fac_Estado = @param3 " +
                            "AND Fac_Tipo_Doc = @param4 " +
                            "AND Homologo IN (" + Fac + ")";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Ase);
                        Commando.Parameters.AddWithValue("@param2", Cia);
                        Commando.Parameters.AddWithValue("@param3", "F");
                        Commando.Parameters.AddWithValue("@param4", Tipo);
                        Commando.Parameters.AddWithValue("@param5", Fac);

                        using (SqlDataReader leer = (Commando.ExecuteReader()))
                        {
                            if (leer.Read() == true)
                            {
                                CXN_FACTURA L = new CXN_FACTURA
                                {
                                    Homologo = leer["Homologo"].ToString(),
                                    Fac_Pac = Convert.ToInt32(leer["Fac_Pac"]),
                                    Fac_ConSub = leer["Fac_ConSub"].ToString(),
                                    Cobertura = leer["Cobertura"].ToString()
                                };

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
    }
}
