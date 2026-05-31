using APIController.Services.FHIR_IHCE;
using Domain;
using Domain.CXN;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FrontFHIR.RadicarRDA
{
    public class RDAAmbulatorio_IHCE
    {
        private IFHIR rFHIR;
        private IPacientes rPaciente;
        private IAgendaC rAgendaC;
        private IBodegas rBodegas;
        private IAntecedentesGlobales repoAntGenGlobal;
        private ICompañia rCia;
        private IReportes rReportes;
        private ICIE10 rCIE10;
        private ICondiciones rCondiciones;
        private IOrdenes rOrdenes;
        private IRIPS_Res2275_2023 rRIPS;
        private ILogin rLogin;
        private ICargos rCargos;

        private MensajesGeneral MG;
        private RDAAmbulatorio_IHCE_Class rda;

        public RDAAmbulatorio_IHCE()
        {
            rFHIR = new MFHIR();
            rPaciente = new MPacientes();
            rAgendaC = new MAgendaC();
            rBodegas = new MBodegas();
            repoAntGenGlobal = new MAntecedentesGlobales();
            rCia = new MCompañia();
            rReportes = new MReportes();
            rCIE10 = new MCIE10();
            rCondiciones = new MCondiciones();
            rOrdenes = new MOrdenes();
            rRIPS = new MRIPS_Res2275_2023();
            rLogin = new MLogin();
            rCargos = new MCargos();
        }

        public async void EnviarRDAAmbulatorio(int Admision, string Especialidad)
        {
            try
            {
                otrosDatosPacienteHorario datosCita = rAgendaC.cargarAdmision(Admision, "'H'");
                if (datosCita == null)
                {
                    MG = new MensajesGeneral
                    {
                        Mensaje = "Error al cargar los datos de la admision " + Admision + " probablemente no tiene historia asociada aun",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return; //Admision no H
                }
                else
                {
                    CXN_PACIENTES DatoPac = rPaciente.LlamarPacientebyId(datosCita.Hor_Pac_Id);
                    CXN_CIA DatoCia = rCia.getPrestadorbyCode(datosCita.Hor_Pac_Cia);
                    CXN_BODEGAS DatoProfesional = rBodegas.getDatosCode(datosCita.Hor_Pac_Bod);
                    CXN_TOKENS_FHIR getToken = rFHIR.RecuperarClaseToken(datosCita.Hor_Pac_Cia);
                    CXN_LOGIN getNameDoc = rLogin.getUser(DatoProfesional.Bod_Usuario);
                    CXN_CARGOS DatosCargo = rCargos.GetCargosFHIR(Admision);

                    if (getToken == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Error al cargar el token de acceso al Ministerio de Salud",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                    else
                    {
                        #region CARGAR HISTORIA CLINICA
                        List<HCMG> getHistory = new List<HCMG>();
                        List<ReportHCFI> getHistoryFI = new List<ReportHCFI>();

                        if (Especialidad == "Medicina General")
                        {
                            getHistory = rReportes.MedicinaGeneral(Admision);
                            if (getHistory == null)
                            {
                                MG = new MensajesGeneral
                                {
                                    Mensaje = $"Historia {Admision.ToString()} no econtrada",
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();
                                return;
                            }
                        }
                        else if (Especialidad == "Fisiatria")
                        {
                            getHistoryFI = rReportes.ReporteFisiatria(Admision);
                            if (getHistoryFI == null)
                            {
                                MG = new MensajesGeneral
                                {
                                    Mensaje = $"Historia {Admision.ToString()} no econtrada",
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral
                            {
                                Mensaje = "Especialidad no encontrada para interoperar IHCE",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                            return;
                        }

                        //RECURSO PDF
                        ReportViewer R = new ReportViewer();
                        R.LocalReport.DataSources.Clear();

                        if (Especialidad == "Medicina General")
                        {
                            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_HCMG", getHistory));
                            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_HCMG.rdlc";
                        }
                        else if (Especialidad == "Fisiatria")
                        {
                            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_HCFI", getHistoryFI));
                            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_HCFI.rdlc";
                        }

                        R.SetDisplayMode(DisplayMode.PrintLayout);
                        R.ZoomMode = ZoomMode.Percent;
                        R.ZoomPercent = 100;
                        R.Font = new System.Drawing.Font("Arial", 7);
                        R.LocalReport.EnableExternalImages = true;
                        R.RefreshReport();
                        R.Dock = System.Windows.Forms.DockStyle.Fill;
                        byte[] bytes = R.LocalReport.Render("PDF");
                        string pdfBase64 = Convert.ToBase64String(bytes);

                        #endregion

                        //CARGAR DIAGNOSTICOS
                        CXN_OM listaDX = rCIE10.CargaDXByAdmitionHCMG(Admision); //Todas las especialidades

                        //CARGAR ALERGIAS
                        List<CXN_ALERGIASINSITE> DatosAlergias = new List<CXN_ALERGIASINSITE>();
                        DatosAlergias = rCondiciones.getCondicionesINSITE(Admision);

                        #region Horas Ingreso y Egreso
                        DateTime llegada = new DateTime(datosCita.Hor_Pac_Fecha_Cita.Year,
                                                                  datosCita.Hor_Pac_Fecha_Cita.Month,
                                                                  datosCita.Hor_Pac_Fecha_Cita.Day,
                                                                  datosCita.Hor_Pac_Hora_Cita.Hour,
                                                                  datosCita.Hor_Pac_Hora_Cita.Minute,
                                                                  00);
                        DateTime saltemp = llegada.AddMinutes(20);

                        DateTime salida = new DateTime(datosCita.Hor_Pac_Fecha_Cita.Year,
                                                       datosCita.Hor_Pac_Fecha_Cita.Month,
                                                       datosCita.Hor_Pac_Fecha_Cita.Day,
                                                       saltemp.Hour,
                                                       saltemp.Minute,
                                                       00);
                        #endregion

                        #region CARGAR ORDENES MEDICAS ASOCIADAS
                        List<CXN_ORDENESFHIR> Incapacidades = new List<CXN_ORDENESFHIR>();
                        List<CXN_ORDENESFHIR> ServiciosOrdenados = new List<CXN_ORDENESFHIR>();
                        List<CXN_ORDENESFHIR> MedicamentosOrdenados = new List<CXN_ORDENESFHIR>();

                        List<CXN_ORDENESFHIR> Ordenes = rOrdenes.getOrdenesAdmition(Admision);
                        if (Ordenes != null)
                        {
                            Incapacidades = Ordenes.Where(x => x.Tipo == "INCAPACIDAD MEDICA").ToList();
                            ServiciosOrdenados = Ordenes.Where(x => x.Tipo == "ORDEN DE SERVICIOS").ToList();
                            MedicamentosOrdenados = Ordenes.Where(x => x.Tipo == "ORDEN DE MEDICAMENTOS").ToList();
                        }
                        #endregion                       

                        rda = new RDAAmbulatorio_IHCE_Class()
                        {
                            // Datos Tecnicos
                            Token = getToken.Token,
                            // Datos Paciente
                            TDocumento = DatoPac.Pac_TipoId,
                            NDocumento = DatoPac.Pac_IdNum,
                            OcupacionText = string.IsNullOrEmpty(DatoPac.Pac_Ocupacion) ||
                                            DatoPac.Pac_Ocupacion == "N/A" ? 
                                            "" :
                                            DatoPac.Pac_Ocupacion,
                            CodePaisOrigen = DatoPac.Pac_PaisOrigen,
                            NameEtnia = DatoPac.Etnia,
                            NameDiscapacidad = DatoPac.Discapacidad,
                            CodeIdentidadGenero = DatoPac.Pac_Sexo == null ? "01" :
                                                DatoPac.Pac_Sexo == "M" ? "01" :
                                                DatoPac.Pac_Sexo == "F" ? "02" :
                                                "03",
                            NameIdentidadGenero = DatoPac.Pac_Sexo == null ? "Masculino" :
                                                   DatoPac.Pac_Sexo == "M" ? "Masculino" :
                                                   DatoPac.Pac_Sexo == "F" ? "Femenino" :
                                                   "Indeterminado o Intersexual",
                            PrimerNombre = DatoPac.Pac_PrimerN,
                            SegundoNombre = DatoPac.Pac_SegundoN,
                            PrimerApellido = DatoPac.Pac_PrimerA,
                            SegundoApellido = DatoPac.Pac_SegundoA,
                            CodeDepartamento = DatoPac.Pac_Dep_Cod,
                            CodeMunicipio = DatoPac.Pac_Mun_Cod,
                            CodePaisResidencia = DatoPac.Pac_Residencia,
                            CodeZonaUrbana = DatoPac.Pac_Zona == null ? "01" : DatoPac.Pac_Zona.Trim() == "U" ? "01" : "02",
                            NameZonaUrbana = DatoPac.Pac_Zona == null ? "Urbana" : DatoPac.Pac_Zona == "U" ? "Urbana" : "Rural",
                            DisplaySexoBiologico = DatoPac.Pac_Sexo == null ? "male" : DatoPac.Pac_Sexo == "M" ? "male" : "female",
                            CodeSexoBiologico = DatoPac.Pac_Sexo == null ? "01" : DatoPac.Pac_Sexo.Trim() == "M" ? "01" : "02",
                            DisplayCodeSexoBiologico = DatoPac.Pac_Sexo == null ? "Hombre" : DatoPac.Pac_Sexo.Trim() == "M" ? "Hombre" : "Mujer",
                            FechaNto = Convert.ToDateTime(DatoPac.Pac_FechaNto),
                            HoraNto = Convert.ToDateTime(DatoPac.HoraNto),
                            // Diagnosticos
                            DX1 = listaDX.OM_DX1,
                            DX2 = string.IsNullOrEmpty(listaDX.OM_DX2) ? "" : listaDX.OM_DX2,
                            DX3 = string.IsNullOrEmpty(listaDX.OM_DX3) ? "" : listaDX.OM_DX3,
                            // Datos Prestador y Medico
                            NombrePrestador = DatoCia.Com_Nombre,
                            NDocumentoMedico = DatoProfesional.Bod_Reg_Med,
                            CodigoHabilitacion = DatoCia.Com_Cod_Prestador_2,
                            CodigoHabilitacionSucursal = DatoCia.Com_Cod_Prestador,
                            CodAseguradora = datosCita.PacienteAseguradora.Trim(),
                            NitPrestador = datosCita.Com_Identificacion,
                            TDocumentoMedico = "CC",
                            PrimerApellidoMedico = getNameDoc.Log_PrimerA,
                            SegundoApellidoMedico = getNameDoc.Log_SegundoA,
                            PrimerNombreMedico = getNameDoc.Log_PrimerN,
                            SegundoNombreMedico = getNameDoc.Log_SegundoN,
                            // Datos Historia
                            FechaCita = Convert.ToDateTime(datosCita.Hor_Pac_Fecha_Cita),
                            HoraIngreso = Convert.ToDateTime(llegada),
                            HoraEgreso = Convert.ToDateTime(salida),
                            HistoriaBase64 = pdfBase64,
                            //Encouter
                            CodeEncounter = datosCita.Hor_Pac_Modalidad == null ? "01" : datosCita.Hor_Pac_Modalidad.Trim(),
                            DisplayEncounter = datosCita.Hor_Pac_Modalidad == null ? "Intramural" :
                                               datosCita.Hor_Pac_Modalidad == "01" ? "Intramural" :
                                               datosCita.Hor_Pac_Modalidad == "02" ? "Extramural unidad móvil" :
                                               datosCita.Hor_Pac_Modalidad == "03" ? "Extramural domiciliaria" :
                                               datosCita.Hor_Pac_Modalidad == "04" ? "Extramural jornada desalud" :
                                               datosCita.Hor_Pac_Modalidad == "05" ? "Extramural (atención prehospitalaria o transporte asistencial)" :
                                               datosCita.Hor_Pac_Modalidad == "06" ? "Telemedicina interactiva" :
                                               datosCita.Hor_Pac_Modalidad == "07" ? "Telemedicina no interactiva" :
                                               datosCita.Hor_Pac_Modalidad == "08" ? "Telemedicina - Telexpertlcia" :
                                               datosCita.Hor_Pac_Modalidad == "09" ? "Telemedicina - Telemonitoreo" : "Intramural",
                            CodeGrupoServicios = datosCita.Hor_GrupoServicios == null ? "01" : datosCita.Hor_GrupoServicios.Trim(),
                            NameGrupoServicios = datosCita.Hor_GrupoServicios == null ? "Consulta externa" :
                                              datosCita.Hor_GrupoServicios == "01" ? "Consulta externa" :
                                              datosCita.Hor_GrupoServicios == "02" ? "Apoyo diagnóstico y complementación terapéutica" :
                                              datosCita.Hor_GrupoServicios == "03" ? "Internación" :
                                              datosCita.Hor_GrupoServicios == "04" ? "Quirúrgico" :
                                              datosCita.Hor_GrupoServicios == "05" ? "Atención inmediata" :
                                              "Consulta externa",
                            CodeREPS = datosCita.Com_Doc_Soporte.ToString(),
                            NameREPS = rAgendaC.getNameServicioFHIR(datosCita.Com_Doc_Soporte),
                            CodeServicio = datosCita.Hor_Pac_Cup.Trim(),
                            NameServicio = rCIE10.GetDXName(datosCita.Hor_Pac_Cup.Trim()),
                            CodeConsultaExterna = DatosCargo.Car_CExterna == 0 ? "38" : DatosCargo.Car_CExterna.ToString(),
                            NameConsultaExterna = DatosCargo.Car_CExterna == 0 ? "ENFERMEDAD GENERAL" : rRIPS.getNameTecnoSaludCExterna(DatosCargo.Car_CExterna).ToUpper().Trim(),
                            CodeImpresionDiagnostica = DatosCargo.Car_Imp_Dx == 0 ? "02" :
                                                       DatosCargo.Car_Imp_Dx == 1 ? "01" :
                                                       DatosCargo.Car_Imp_Dx == 2 ? "02" :
                                                       DatosCargo.Car_Imp_Dx == 3 ? "03" : "02",
                            NameImpresionDiagnostica = DatosCargo.Car_Imp_Dx == 0 ? "Confirmado Nuevo" :
                                                       DatosCargo.Car_Imp_Dx == 1 ? "Impresión Diagnóstica" :
                                                       DatosCargo.Car_Imp_Dx == 2 ? "Confirmado Nuevo" :
                                                       DatosCargo.Car_Imp_Dx == 3 ? "Confirmado Repetido" : "Confirmado Nuevo",
                            CodeEgreso = DatosCargo.Car_CodeEgreso == 0 ? "01" :
                                                   DatosCargo.Car_CodeEgreso == 1 ? "01" :
                                                   DatosCargo.Car_CodeEgreso == 2 ? "02" :
                                                   DatosCargo.Car_CodeEgreso == 3 ? "03" :
                                                   DatosCargo.Car_CodeEgreso == 4 ? "04" :
                                                   DatosCargo.Car_CodeEgreso == 5 ? "05" :
                                                   DatosCargo.Car_CodeEgreso == 6 ? "06" :
                                                   DatosCargo.Car_CodeEgreso == 7 ? "07" :
                                                   DatosCargo.Car_CodeEgreso == 8 ? "08" : "01",
                            NameEgreso = DatosCargo.Car_CodeEgreso == 0 ? "PACIENTE CON DESTINO A SU DOMICILIO" :
                                                   DatosCargo.Car_CodeEgreso == 1 ? "PACIENTE CON DESTINO A SU DOMICILIO" :
                                                   DatosCargo.Car_CodeEgreso == 2 ? "PACIENTE MUERTO" :
                                                   DatosCargo.Car_CodeEgreso == 3 ? "PACIENTE DERIVADO A OTRO SERVICIO" :
                                                   DatosCargo.Car_CodeEgreso == 4 ? "REFERIDO A OTRA INSTITUCION" :
                                                   DatosCargo.Car_CodeEgreso == 5 ? "CONTRAREFERIDO A OTRA INSTITUCION" :
                                                   DatosCargo.Car_CodeEgreso == 6 ? "DERIVADO O REFERIDO A HOSPITALIZACION DOMICILIRIA" :
                                                   DatosCargo.Car_CodeEgreso == 7 ? "DERIVADO A SERVICIO SOCIAL" :
                                                   DatosCargo.Car_CodeEgreso == 8 ? "PACIENTE CONTINUA EN EL SERVICIO (CORTE FACTURACION)" : "PACIENTE CON DESTINO A SU DOMICILIO",
                            //Organization
                            idOrganization = datosCita.PacienteAseguradora.Trim(),
                            nameOrganization = datosCita.Com_Telefono_SMS.ToUpper().Trim(),
                            Incapacidades = new Incapacidad(),
                            AlergiasInSite = new List<AlergiasInSite>(),
                            FactorRiesgo = new List<FactoresRiesgo>(),
                            ServiciosOrdenados = new List<ServiciosOrdenadosInSite>(),
                            MedicamentosOrdenados = new List<MedicamentosOrdenadosInSite>()
                        };

                        //AGREGAR ALERGIAS A LA CLASE MADRE
                        if (DatosAlergias != null)
                        {
                            foreach (var i in DatosAlergias)
                            {
                                rda.AlergiasInSite.Add(new AlergiasInSite
                                {
                                    CodeFHIR = i.CodeFHIR,
                                    Observacion = i.Observacion,
                                });
                            }                            
                        }

                        //AGREGAR INCAPACIDAD A LA CLASE MADRE
                        if (Incapacidades.Count > 0)
                        {
                            rda.Incapacidades = new Incapacidad()
                            {
                                TipoOrdenCode = Incapacidades[0].Servicio,
                                TipoOrdenText = Incapacidades[0].Servicio == "Nueva" ? "01" : "02",
                                Dias = Convert.ToInt32(Incapacidades[0].CUP) //dias en este caso
                            };
                        }

                        //AGREGAR FACTORES DE RIESGO
                        List<CXN_CONDICIONES> _listaCondiciones = rCondiciones.getCondicionesFHIR(datosCita.Hor_Pac_Id);
                        if (_listaCondiciones != null)
                        {
                            foreach (var i in _listaCondiciones)
                            {
                                rda.FactorRiesgo.Add(new FactoresRiesgo 
                                { 
                                    Codigo = rFHIR.GetCodeRisk(i.Condicion.Trim()),
                                    Descripcion = i.Condicion.Trim(),
                                    Detalle = i.Detalle.Trim()
                                });                                
                            }                            
                        }

                        //AGREGAR SERVICIOS ORDENADOS
                        if (ServiciosOrdenados != null)
                        {
                            foreach (var i in ServiciosOrdenados)
                            {
                                rda.ServiciosOrdenados.Add(new ServiciosOrdenadosInSite 
                                { 
                                    CUP = i.CUP,
                                    Descripcion = i.Servicio,
                                    FechaOrden = Convert.ToDateTime(datosCita.Hor_Pac_Fecha_Cita),
                                    ImpDiagnosticaCode = string.IsNullOrEmpty(datosCita.HorTecnoSalud) ? "15" : datosCita.HorTecnoSalud,
                                    ImpDiagnosticaText = string.IsNullOrEmpty(datosCita.HorTecnoSalud) ? "DIAGNOSTICO" : rRIPS.getNameTecnoSalud(datosCita.HorTecnoSalud)
                                });
                            }
                        }

                        //AGREGAR MEDICAMENTOS ORDENADOS
                        if (MedicamentosOrdenados != null)
                        {
                            foreach (var i in MedicamentosOrdenados)
                            {
                                rda.MedicamentosOrdenados.Add(new MedicamentosOrdenadosInSite
                                {
                                    TipoTecnologiaText = i.TipoTecnologia,
                                    TipoTecnologiaCodigo = i.TipoTecnologia == "Medicamento con registro sanitario" ? "02" :
                                                           i.TipoTecnologia == "Medicamento vital no disponible" ? "03" :
                                                           i.TipoTecnologia == "Preparación magistral" ? "04" :
                                                           i.TipoTecnologia == "Medicamento UNIRS" ? "05" : "02",
                                    MedicamentoCodigo = i.CodMedicamento,
                                    MedicamentoDescripcion = i.Medicamento,
                                    TecnologiaSaludCodigo = datosCita.HorTecnoSalud.Trim(),
                                    TecnologiaSaludTexto = rFHIR.GetDesctecnoSalud(datosCita.HorTecnoSalud.Trim()).Trim().ToUpper(),
                                    DuracionTiempoDias = string.IsNullOrEmpty(i.Duracion) ? 3 : Convert.ToInt32(i.Duracion),
                                    DuracionTiempo = i.Tiempo == "Minutos" ? "min" :
                                                     i.Tiempo == "Horas" ? "h" :
                                                     i.Tiempo == "Día" ? "d" :
                                                     i.Tiempo == "Semanas" ? "wk" :
                                                     i.Tiempo == "Mes" ? "mo" :
                                                     i.Tiempo == "Año" ? "a" :
                                                     i.Tiempo == "Según respuesta al tratamiento" ? "" : "a",
                                    DuracionTiempoCodigo = i.Tiempo == "Minutos" ? "1" :
                                                           i.Tiempo == "Horas" ? "2" :
                                                           i.Tiempo == "Día" ? "3" :
                                                           i.Tiempo == "Semanas" ? "4" :
                                                           i.Tiempo == "Mes" ? "5" :
                                                           i.Tiempo == "Año" ? "6" :
                                                           i.Tiempo == "Según respuesta al tratamiento" ? "7" : "3",
                                    DuracionTiempoTexto = i.Tiempo,
                                    ViaCodigo = rFHIR.GetCodeConsumo(i.Via.ToUpper().Trim()),
                                    ViaTexto = i.Via.ToUpper().Trim(),
                                    Concentracion = Convert.ToInt32(i.Cantidad),
                                    ConcentracionUnidadMedidaTexto = i.UMM,
                                    ConcentracionUnidadMedidaCodigo = rOrdenes.GetUMMCode(i.UMM),
                                    DosificacionNumero = Convert.ToInt32(i.Cada),
                                    DosificacionFrecuenciaTexto = i.FrecAdmi,
                                    DosificacionFrecuenciaCodigo = i.FrecAdmi == "Minutos" ? "1" :
                                                                   i.FrecAdmi == "Horas" ? "2" :
                                                                   i.FrecAdmi == "Día" ? "3" :
                                                                   i.FrecAdmi == "Semanas" ? "4" :
                                                                   i.FrecAdmi == "Mes" ? "5" :
                                                                   i.FrecAdmi == "Año" ? "6" :
                                                                   i.FrecAdmi == "Según respuesta al tratamiento" ? "7" : "3"
                                });
                            }
                        }

                        //Serializar JSON y Transformar en Base64
                        var json = JsonConvert.SerializeObject(rda);
                        var jsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                        string rutaArchivo = @"C:\CXN\Reportes\RDA\RDA_CAmbulatoria_" + datosCita.Pac_IdNum + ".json";
                        File.WriteAllText(rutaArchivo, json);

                        //return;

                        //Radicar y Grabar RDA
                        API_FHIR Radica = new API_FHIR();
                        (bool status, string res) respuesta = await Radica.SendRDAAmbulatorio(jsonBase64, Program.URLApiConexion);
                        if (respuesta.status == true)
                        {
                            CXN_RDA saveRDAPaciente = new CXN_RDA
                            {
                                Admision = Admision,
                                RDAAmbulatorio = respuesta.res,
                                Especialidad = "Medicina General",
                                PersonaReportaRDAAmbulatorio = Contenedor.UsuarioLogueado,
                                FechaReporteRDAAmbulatorio = DateTime.Now,
                                URLPdf = ""
                            };

                            if (VerificarRDA(Admision) != null)
                            {
                                //update
                                rFHIR.ActualizarEnvio_RDACExterna(saveRDAPaciente);
                            }
                            else
                            {
                                //insert
                                rFHIR.InsertarEnvio_RDAAmbulatorio(saveRDAPaciente);
                            }

                            CXN_RDA_LOG log = new CXN_RDA_LOG
                            {
                                Admision = Admision,
                                Clase = "RDA AMBULATORIO",
                                Detalle = "EXITOSO",
                                Usuario = Contenedor.UsuarioLogueado,
                                Fecha = DateTime.Now
                            };

                            rFHIR.InsertarLOG_RDA(log);
                        }
                        else
                        {
                            CXN_RDA_LOG log = new CXN_RDA_LOG
                            {
                                Admision = Admision,
                                Clase = "RDA AMBULATORIO",
                                Detalle = respuesta.res,
                                Usuario = Contenedor.UsuarioLogueado,
                                Fecha = DateTime.Now
                            };

                            rFHIR.InsertarLOG_RDA(log);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        CXN_RDA VerificarRDA(int Admision)
        {
            return rFHIR.ConsultarAdmision(Admision);
        }
    }
}
