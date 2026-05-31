using APIController.Services.FHIR_IHCE;
using Domain.CXN;
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
    public class RDAPaciente_IHCE
    {
        private IFHIR rFHIR;
        private IPacientes rPaciente;
        private IAgendaC rAgendaC;
        private IBodegas rBodegas;
        private IAntecedentesGlobales repoAntGenGlobal;
        private ICompañia rCia;

        private MensajesGeneral MG;
        private ClassRDAPaciente claseRDAPaciente;

        public RDAPaciente_IHCE()
        {
            rFHIR = new MFHIR();
            rPaciente = new MPacientes();
            rAgendaC = new MAgendaC();
            rBodegas = new MBodegas();
            repoAntGenGlobal = new MAntecedentesGlobales();
            rCia = new MCompañia();
        }
        public async void EnviarRDAPaciente(int Admision)
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
                        claseRDAPaciente = new ClassRDAPaciente()
                        {
                            // Token
                            Token = getToken.Token,
                            // DatosPaciente
                            TipoDocumentoPaciente = rPaciente.getTipoDoc(datosCita.Pac_TipoId),
                            NumeroDocumentoPaciente = datosCita.Pac_IdNum,
                            SexoBiologico = datosCita.Pac_Sexo == "M" ? "male" : "female",
                            CodeIdentidadGenero = DatoPac.IdentidadGenero,
                            CodPaisNacimiento = DatoPac.Pac_PaisOrigen,
                            CodPaisResidencia = DatoPac.Pac_Residencia,
                            ZonaUrbana = DatoPac.Pac_Zona,
                            PrimerNombre = DatoPac.Pac_PrimerN,
                            SegundoNombre = DatoPac.Pac_SegundoN,
                            PrimerApellido = DatoPac.Pac_PrimerA,
                            SegundoApellido = DatoPac.Pac_SegundoA,
                            CodeDepartamento = DatoPac.Pac_Dep_Cod,
                            CodeMunicipio = DatoPac.Pac_Mun_Cod,
                            CodEtnia = Convert.ToInt32(rFHIR.Etnia(DatoPac.Etnia)),
                            CodDiscapacidad = rFHIR.Discapacidad(DatoPac.Discapacidad),
                            FechaNacimiento = Convert.ToDateTime(DatoPac.Pac_FechaNto),
                            HoraNacimiento = Convert.ToDateTime(DatoPac.HoraNto),
                            // Datos Cita
                            FechaAtencion = Convert.ToDateTime(datosCita.Hor_Pac_Fecha_Cita),
                            HoraIngreso = Convert.ToDateTime(datosCita.Hor_Pac_Hora_Cita),
                            HoraSalida = Convert.ToDateTime(datosCita.Hor_Pac_Hora_Cita).AddMinutes(20),
                            ModalidadAtencion = datosCita.Hor_Pac_Modalidad,
                            GrupoServicios = datosCita.Hor_GrupoServicios,
                            // Datos Prestador
                            CodHabilitacionPrestador  = DatoCia.Com_Cod_Prestador_2,
                            NitPrestador = DatoCia.Com_Identificacion,
                            DVPrestador = DatoCia.Com_DVerifica,
                            // Datos Profesional
                            IdentificacionProfesional = DatoProfesional.Bod_Reg_Med,
                            TipoIdentificacionProfesional = "CC",
                            // Antecedentes
                            antecedentesPaciente = new AntecedentePaciente(),
                            antecedentesFamiliares = new List<AntecedentesFamiliares>()                          
                        };

                        // Antecedentes Paciente Patologicos y Farmacologicos
                        List<CXN_ANTECEDENTESFAMILIARES> DatosAntecedentes = repoAntGenGlobal.ObtenerAntecedentes(DatoPac.Pac_Id);
                        if (DatosAntecedentes != null)
                        {
                            List<CXN_ANTECEDENTESFAMILIARES> antPatologicos = DatosAntecedentes.Where(a => a.Parentesco == "PROPIO" && a.Tipo == "PATOLOGICO").ToList();
                            List<CXN_ANTECEDENTESFAMILIARES> antFarmacologicos = DatosAntecedentes.Where(a => a.Parentesco == "PROPIO" && a.Tipo == "FARMACOLOGICO").ToList();

                            List<AntPatologico> listaAntecedentesPatologicos = new List<AntPatologico>();
                            List<AntPatologico> listaAntecedentesFarmacologicos = new List<AntPatologico>();

                            if (antPatologicos != null)
                            {
                                foreach (var ant in antPatologicos)
                                {
                                    claseRDAPaciente.antecedentesPaciente.Patologicos.Add(new AntPatologico 
                                    {
                                        Codigo = ant.CIECod,
                                        Descripcion = ant.CieDesc
                                    });
                                }
                            }
                            if (antFarmacologicos != null)
                            {
                                foreach (var ant in antFarmacologicos)
                                {
                                    claseRDAPaciente.antecedentesPaciente.Farmacologicos.Add(new AntPatologico
                                    {
                                        Codigo = ant.CIECod,
                                        Descripcion = ant.CieDesc
                                    });
                                }
                            }
                        }

                        //Antecedentes Alergicos Paciente 
                        List<CXN_CONDICIONES> DatosAlergicos = rFHIR.ConsultarAlergias(DatoPac.Pac_Id);
                        if (DatosAlergicos != null)
                        {
                            foreach (CXN_CONDICIONES i in DatosAlergicos)
                            {
                                claseRDAPaciente.antecedentesPaciente.Alergias.Add(new AntPatologico
                                {
                                    Codigo = i.CodigoFHIR,
                                    Detalle = i.Detalle
                                });
                            }
                        }

                        //Antecedentes Familiares
                        List<CXN_ANTECEDENTESFAMILIARES> AntFamiliares = repoAntGenGlobal.ObtenerAntecedentes(DatoPac.Pac_Id);
                        if (AntFamiliares != null)
                        {
                            var filtro = AntFamiliares.Where(a => a.Parentesco != "PROPIO").ToList();
                            foreach (var grupo in filtro)
                            {
                                claseRDAPaciente.antecedentesFamiliares.Add(new AntecedentesFamiliares
                                {
                                    CIECod = grupo.CIECod,
                                    CieDesc = grupo.CieDesc,
                                    Parentesco = grupo.Parentesco,
                                });
                            }
                        }

                        //Serializar JSON y Transformar en Base64
                        var json = JsonConvert.SerializeObject(claseRDAPaciente);
                        var jsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                        string rutaArchivo = @"C:\CXN\Reportes\RDA\RDA_Paciente_" + datosCita.Pac_IdNum + ".json";
                        File.WriteAllText(rutaArchivo, json);

                        //return;

                        //Radicar y Grabar RDA
                        API_FHIR Radica = new API_FHIR();
                        (bool status, string res) respuesta = await Radica.SendRDAPaciente(jsonBase64, Program.URLApiConexion);
                        if (respuesta.status == true)
                        {
                            CXN_RDA saveRDAPaciente = new CXN_RDA
                            {
                                Admision = Admision,
                                RDAPaciente = respuesta.res,
                                Especialidad = "Medicina General",
                                PersonaReportaRDAPaciente = Contenedor.UsuarioLogueado,
                                FechaReporteRDAPaciente = DateTime.Now
                            };

                            if (VerificarRDA(Admision) != null)
                            {
                                //update
                                rFHIR.ActualizarEnvio_RDAPaciente(saveRDAPaciente);
                            }
                            else
                            {
                                //insert
                                rFHIR.InsertarEnvio_RDAPaciente(saveRDAPaciente);
                            }

                            CXN_RDA_LOG log = new CXN_RDA_LOG
                            {
                                Admision = Admision,
                                Clase = "RDA PACIENTE",
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
                                Clase = "RDA PACIENTE",
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
