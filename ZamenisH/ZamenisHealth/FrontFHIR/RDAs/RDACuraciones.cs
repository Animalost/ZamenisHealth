using APIFhir.Controlador;
using APIFhir.Servicio;
using Domain;
using Domain.CXN;
using Microsoft.Reporting.WinForms;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FrontFHIR.RDAs
{
    public class RDACuraciones
    {
        private IFHIR rFHIR;
        private IPacientes rPaciente;
        private IAgendaC rAgendaC;
        private IBodegas rBodegas;
        private ILogin rLogin;
        private ICargos rCargos;
        private ICIE10 rCIE10;
        private IZonas rZonas;
        private IReportes rReportes;
        private IOrdenes rOrdenes;
        private ICondiciones rCondiciones;
        private IGenerales rGenerales;
        private IRIPS_Res2275_2023 rRIPS;
        private EnviarRDAPaciente enviarRDA;
        private ICompañia rCompañia;

        private MensajesGeneral MG;
        private CXN_BODEGAS DatosProfesional;
        private CXN_LOGIN DatosLogin;
        private CXN_CARGOS DatosCargo;
        private CXN_PACIENTES DatosPaciente;
        private List<CXN_CONDICIONES> DatosAlergias;

        private CXN_CIA DatosPrestador;

        public RDACuraciones()
        {
            rFHIR = new MFHIR();
            rPaciente = new MPacientes();
            rAgendaC = new MAgendaC();
            rBodegas = new MBodegas();
            rLogin = new MLogin();
            rCargos = new MCargos();
            rCIE10 = new MCIE10();
            rZonas = new MZonas();
            rReportes = new MReportes();
            rOrdenes = new MOrdenes();
            rCondiciones = new MCondiciones();
            rGenerales = new MGenerales();
            rRIPS = new MRIPS_Res2275_2023();
            enviarRDA = new EndPoint_RDAPaciente();
            rCompañia = new MCompañia();
        }

        public async Task RadicarRDA(int Admision, string Especialidad)
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
                    //Verificar si ya se envio el RDA Consulta Externa
                    CXN_RDA verificar = rFHIR.ConsultarAdmision(Admision);

                    DatosPrestador = new CXN_CIA();
                    DatosPrestador = rCompañia.getPrestadorbyCode(datosCita.Com_Identificador);

                    if (verificar != null && !string.IsNullOrEmpty(verificar.RDAAmbulatorio))
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "RDA Consulta Externa ya enviado",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return; //RDA Consulta Externa ya enviado
                    }

                    DatosProfesional = rBodegas.getDatosCode(datosCita.Hor_Pac_Bod);
                    if (DatosProfesional == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Error al cargar los datos del profesional",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return; //Error al cargar datos profesional
                    }

                    DatosLogin = rLogin.getUser(DatosProfesional.Bod_Usuario);
                    if (DatosLogin == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Error al cargar los datos basicos del profesional",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return; //Error al cargar datos login profesional
                    }

                    DatosCargo = rCargos.GetCargosFHIR(Admision);
                    if (DatosCargo == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Error al cargar los datos del cargo",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return; //Error al cargar datos cargo 
                    }

                    string DxTexto = rCIE10.BuscaDX(DatosCargo.Car_Dx1);
                    if (string.IsNullOrEmpty(DxTexto))
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Error al cargar los datos de los diagnosticos",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return; //Error al cargar descripcion DX
                    }

                    DatosPaciente = new CXN_PACIENTES();
                    DatosPaciente = rPaciente.LlamarPacientebyId(datosCita.Hor_Pac_Id);
                    if (DatosPaciente == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Error al cargar los datos del paciente",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return; //Error al cargar datos paciente
                    }

                    string TipoDocPac = rPaciente.getTipoDoc(datosCita.Pac_TipoId);
                    if (TipoDocPac == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Tipo documento paciente no valido",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return; //Tipo Documento paciente no valido
                    }

                    List<ReportNotas> getHistory = rReportes.NotasMetodo(Admision);
                    if (getHistory == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = $"Nota {Admision.ToString()} no econtrada",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return;
                    }

                    DatosAlergias = new List<CXN_CONDICIONES>();
                    DatosAlergias = rFHIR.ConsultarAlergias(datosCita.Hor_Pac_Id);

                    DateTime fecha = DateTime.Now;

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


                    //CREAR RECURSOS
                    var builder = new Domain.FHIR.CE.RDAConsultaExterna.RdaBundleBuilder();

                    string PATIENT_ID = "#" + TipoDocPac + "-" + datosCita.Pac_IdNum.Trim();
                    string PRACTITIONER_ID = "#CC-" + datosCita.IdentificacionProfesional.Trim();

                    const string ENCOUNTER_ID = "Encounter-1";
                    const string CONDITION_MAIN_ID = "Condition-1";

                    List<Domain.FHIR.CE.RDAConsultaExterna.Entry> entryTemp = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>();
                    #region Verificar Cantidad de Diagnosticos y agaregarlos al Composition a traves de entry DIAGNOSTICOS
                    if (!string.IsNullOrEmpty(getHistory[0].HC_EstadoEmo.Trim()))
                    {
                        entryTemp.Add(new Domain.FHIR.CE.RDAConsultaExterna.Entry
                        {
                            reference = "#" + CONDITION_MAIN_ID
                        });
                    }
                    else
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = $"Nota {Admision.ToString()} no registra diagnosticos",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return;
                    }
                    #endregion FIN Verificar Cantidad de Diagnosticos / agaregarlos al Composition a traves de entry DIAGNOSTICOS

                    #region ALERGIAS
                    //Lista recurso AllergyIntolerance
                    List<Domain.FHIR.CE.RDAConsultaExterna.AllergyIntolerance> allergyTemp = new List<Domain.FHIR.CE.RDAConsultaExterna.AllergyIntolerance>();
                    //valido si existen alergias
                    if (DatosAlergias != null)
                    {
                        int allergyCounter = 0;

                        //Si existen alergias recorrol la lista de alergias  e instancio un rcurso AllergyIntolerance
                        foreach (var alergia in DatosAlergias)
                        {
                            var allergyResource = new Domain.FHIR.CE.RDAConsultaExterna.AllergyIntolerance
                            {
                                resourceType = "AllergyIntolerance",
                                id = $"AllergyIntolerance-{allergyCounter}",
                                meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                                {
                                    profile = new List<string>()
                                    {
                                        "https://fhir.minsalud.gov.co/rda/StructureDefinition/AllergyIntoleranceRDA"
                                    }
                                },
                                clinicalStatus = new Domain.FHIR.CE.RDAConsultaExterna.ClinicalStatus()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            code = "active",
                                            display="Active"
                                        }
                                    }
                                },
                                code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/TipoAlergia",
                                            code = alergia.CodigoFHIR,
                                            display = rFHIR.GrupoAlergias(alergia.CodigoFHIR)
                                        }

                                    },
                                    text = alergia.Detalle
                                },
                                patient = new Domain.FHIR.CE.RDAConsultaExterna.Patient()
                                {
                                    reference = PATIENT_ID
                                },
                                encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter()
                                {
                                    reference = "#" + ENCOUNTER_ID
                                }
                            };

                            // Guardar la referencia para la sección
                            allergyTemp.Add(allergyResource);
                            allergyCounter++;
                        }
                        //Si no hay alergias el recurso no existe
                    }

                    Domain.FHIR.CE.RDAConsultaExterna.Section sectionHistorialAllergies = new Domain.FHIR.CE.RDAConsultaExterna.Section();

                    List<Domain.FHIR.CE.RDAConsultaExterna.Entry> entryTempAllergy = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>();

                    if (DatosAlergias != null)
                    {
                        foreach (var i in allergyTemp)
                        {
                            entryTempAllergy.Add(new Domain.FHIR.CE.RDAConsultaExterna.Entry
                            {
                                reference = "#" + i.id
                            });
                        }
                    }

                    if (DatosAlergias != null)
                    {
                        sectionHistorialAllergies = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                        {
                            title = "Historial de alergias, intolerancias y reacciones adversas",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "48765-2",
                                             display = "Allergies and adverse reactions Document"
                                        }
                                    }
                            },
                            entry = entryTempAllergy //Aqui agrego el EntryTempAllergy
                        };
                    }
                    else
                    {
                        sectionHistorialAllergies = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                        {
                            title = "Historial de alergias, intolerancias y reacciones adversas",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "48765-2",
                                             display = "Allergies and adverse reactions Document"
                                        }
                                    }
                            },
                            emptyReason = new Domain.FHIR.CE.RDAConsultaExterna.EmptyReason()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                        code = "nilknown",
                                        display = "Nil Known"
                                    }
                                }
                            },
                            text = new Domain.FHIR.CE.RDAConsultaExterna.Text()
                            {
                                status = "generated",
                                div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                            }
                        };
                    }

                    #endregion FIN ALERGIAS

                    #region INCAPACIDADES
                    string TipoOrden = "";
                    int diasOM = 0;
                    Domain.FHIR.CE.RDAConsultaExterna.Section sectionIncTemp = new Domain.FHIR.CE.RDAConsultaExterna.Section();
                    int NumOrden = rOrdenes.getIdOMFHIR(datosCita.Hor_Pac_Id, Convert.ToDateTime(datosCita.Hor_Pac_Fecha_Cita));
                    if (NumOrden > 0)
                    {
                        var DataOrden = rOrdenes.Generar_OrdenMedica(NumOrden, datosCita.Com_Identificador, Contenedor.UsuarioLogueado);
                        if (DataOrden != null)
                        {
                            sectionIncTemp = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                            {
                                title = "Datos incapacidad (SIPE – Sistema de Incapacidades y Prestaciones Economicas)",
                                code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "105583-9",
                                             display = "Worker Sick leave form"
                                        }
                                    }
                                },
                                entry = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>()
                                {
                                   new Domain.FHIR.CE.RDAConsultaExterna.Entry()
                                   {
                                       reference = "#Observation-0"
                                   }
                                }
                            };

                            TipoOrden = DataOrden[0].OM_FHIR;
                            diasOM = DataOrden[0].OM_Dias;
                        }
                    }
                    else
                    {
                        sectionIncTemp = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                        {
                            title = "Datos incapacidad (SIPE – Sistema de Incapacidades y Prestaciones Economicas)",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                    {
                                        system = "http://loinc.org",
                                        code = "105583-9",
                                        display = "Worker Sick leave form"
                                    }
                                }
                            },
                            emptyReason = new Domain.FHIR.CE.RDAConsultaExterna.EmptyReason()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                        code = "nilknown",
                                        display = "Nil Known"
                                    }
                                }
                            },
                            text = new Domain.FHIR.CE.RDAConsultaExterna.Text()
                            {
                                status = "generated",
                                div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                            }
                        };
                    }
                    #endregion FIN INCAPACIDADES

                    #region FACTORES DE RIESGO - RISK
                    Domain.FHIR.CE.RDAConsultaExterna.Section sectionRiskTemp = new Domain.FHIR.CE.RDAConsultaExterna.Section();
                    List<CXN_CONDICIONES> _listaCondiciones = rCondiciones.getCondicionesFHIR(datosCita.Hor_Pac_Id);
                    List<Domain.FHIR.CE.RDAConsultaExterna.RiskAssessment> lista_Risk = new List<Domain.FHIR.CE.RDAConsultaExterna.RiskAssessment>();
                    if (_listaCondiciones != null)
                    {
                        int RiskCounter = 1;
                        List<Domain.FHIR.CE.RDAConsultaExterna.Entry> entryRiskTemp = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>();

                        sectionRiskTemp = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                        {
                            title = "Factores de riesgo",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                    {
                                        system = "http://loinc.org",
                                        code = "75492-9",
                                        display = "Risk assessment and screening note"
                                    }
                                }
                            },
                            entry = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Entry()
                                {
                                    reference = ""
                                }
                            }
                        };

                        foreach (CXN_CONDICIONES i in _listaCondiciones)
                        {
                            entryRiskTemp.Add(new Domain.FHIR.CE.RDAConsultaExterna.Entry
                            {
                                reference = $"#RiskAssessment-{RiskCounter}"
                            });

                            RiskCounter++;
                        }

                        sectionRiskTemp.entry = entryRiskTemp;
                        RiskCounter = 1;

                        foreach (var i in _listaCondiciones)
                        {
                            lista_Risk.Add(new Domain.FHIR.CE.RDAConsultaExterna.RiskAssessment
                            {
                                resourceType = "RiskAssessment",
                                id = $"RiskAssessment-{RiskCounter}",
                                meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                                {
                                    profile = new List<string>()
                                    {
                                        "https://fhir.minsalud.gov.co/rda/StructureDefinition/RiskFactorRDA"
                                    }
                                },
                                status = "registered",
                                code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/FactorRiesgo",
                                            code = rFHIR.GetCodeRisk(i.Condicion.Trim()),
                                            display = i.Condicion.Trim()
                                        }
                                    },
                                    text = i.Detalle.Trim()
                                },
                                subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                                {
                                    reference = PATIENT_ID
                                },
                                encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter()
                                {
                                    reference = "#" + ENCOUNTER_ID
                                },
                            });
                        }
                    }
                    else
                    {
                        sectionRiskTemp = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                        {
                            title = "Factores de riesgo",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                    {
                                        system = "http://loinc.org",
                                        code = "75492-9",
                                        display = "Risk assessment and screening note"
                                    }
                                }
                            },
                            entry = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Entry()
                                {
                                    reference = "#RiskAssessment-1"
                                }
                            }
                        };

                        lista_Risk.Add(new Domain.FHIR.CE.RDAConsultaExterna.RiskAssessment
                        {
                            resourceType = "RiskAssessment",
                            id = "RiskAssessment-1",
                            meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                            {
                                profile = new List<string>()
                                    {
                                        "https://fhir.minsalud.gov.co/rda/StructureDefinition/RiskFactorRDA"
                                    }
                            },
                            status = "registered",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "https://fhir.minsalud.gov.co/rda/CodeSystem/FactorRiesgo",
                                        code = "06",
                                        display = "Otro"
                                    }
                                },
                                text = "NO REPORTA"
                            },
                            subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                            {
                                reference = PATIENT_ID
                            },
                            encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter()
                            {
                                reference = "#" + ENCOUNTER_ID
                            },
                        });
                    }
                    #endregion FIN FACTORES DE RIESGO - RISK

                    #region MEDICAMENTOS
                    int NumOrdenMed = rOrdenes.getIdOMMEDFHIR(datosCita.Hor_Pac_Id, Convert.ToDateTime(datosCita.Hor_Pac_Fecha_Cita));
                    List<Ordenes> Exportar = new List<Ordenes>();
                    Domain.FHIR.CE.RDAConsultaExterna.Section sectionMed = new Domain.FHIR.CE.RDAConsultaExterna.Section();
                    if (NumOrdenMed > 0)
                    {
                        Exportar = rOrdenes.Genera_Orden_Medicamento(NumOrdenMed, datosCita.Com_Identificador, Contenedor.UsuarioLogueado);

                        List<Domain.FHIR.CE.RDAConsultaExterna.Entry> entryTempMed = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>();

                        int CountMeds = 0;

                        foreach (var i in Exportar)
                        {
                            entryTempMed.Add(new Domain.FHIR.CE.RDAConsultaExterna.Entry
                            {
                                reference = $"#MedicationRequest-{CountMeds}"
                            });

                            CountMeds++;
                        }

                        sectionMed = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                        {
                            title = "Historial de medicamentos",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "10160-0",
                                             display = "History of Medication use Narrative"
                                        }
                                    }
                            },
                            entry = entryTempMed
                        };
                    }
                    else
                    {
                        sectionMed = new Domain.FHIR.CE.RDAConsultaExterna.Section()
                        {
                            title = "Historial de medicamentos",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "10160-0",
                                             display = "History of Medication use Narrative"
                                        }
                                    }
                            },
                            emptyReason = new Domain.FHIR.CE.RDAConsultaExterna.EmptyReason()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                        code = "nilknown",
                                        display = "Nil Known"
                                    }
                                }
                            },
                            text = new Domain.FHIR.CE.RDAConsultaExterna.Text()
                            {
                                status = "generated",
                                div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                            }
                        };
                    }

                    #endregion MEDICAMENTOS

                    #region RECURSO COMPOSITION
                    var composition = new Domain.FHIR.CE.RDAConsultaExterna.Composition
                    {
                        resourceType = "Composition",
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                        {
                            profile = new List<string>()
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/CompositionAmbulatoryRDA"
                            }
                        },
                        status = "final",
                        type = new Domain.FHIR.CE.RDAConsultaExterna.Type()
                        {
                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                {
                                    system = "http://loinc.org",
                                    code = "51845-6",
                                    display = "Outpatient Consult note"
                                }
                            }
                        },
                        subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                        {
                            reference = PATIENT_ID
                        },
                        encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter2()
                        {
                            reference = "#" + ENCOUNTER_ID
                        },
                        date = new DateTimeOffset(fecha, TimeSpan.FromHours(-5)),
                        author = new List<Domain.FHIR.CE.RDAConsultaExterna.Author>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Author()
                            {
                                reference = "#" + DatosPrestador.Com_Cod_Prestador_2.Trim()
                            }
                        },
                        title = "RDA Consulta",
                        confidentiality = "N",
                        attester = new List<Domain.FHIR.CE.RDAConsultaExterna.Attester>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Attester()
                            {
                                mode = "legal",
                                party = new Domain.FHIR.CE.RDAConsultaExterna.Party()
                                {
                                    reference = PRACTITIONER_ID
                                }
                            }
                        },
                        custodian = new Domain.FHIR.CE.RDAConsultaExterna.Custodian()
                        {
                            reference = "#" + DatosPrestador.Com_Cod_Prestador_2
                        },
                        @event = new Domain.FHIR.CE.RDAConsultaExterna.Event()
                        {
                            period = new Domain.FHIR.CE.RDAConsultaExterna.Period()
                            {
                                start = new DateTimeOffset(llegada, TimeSpan.FromHours(-5)),  //hora de inicio de la consulta
                                end = new DateTimeOffset(salida, TimeSpan.FromHours(-5)) // hora de salida de la consulta
                            }
                        },
                        section = new List<Domain.FHIR.CE.RDAConsultaExterna.Section>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Section() //ASEGURADORAS
                            {
                                title = "Entidad(es) responsable(s) por el plan de beneficios en salud (consulta)",
                                code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "48768-6",
                                             display = "Payment sources Document"
                                        }
                                    }
                                },
                                    entry = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Entry()
                                        {
                                            reference = "#" + datosCita.PacienteAseguradora.Trim()
                                        }
                                    }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Section() //OCUPACION
                            {
                                 title = "Otros datos demográficos",
                                 code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                                 {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "74208-0",
                                             display = "Demographic information + History of occupation Document"
                                        }
                                    }
                                 },
                                    entry = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Entry()
                                        {
                                            reference = "#Observation-1"
                                        }
                                    }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Section() //DIAGNOSTICOS
                            {
                                 title = "Historial de diagnósticos de problemas de salud",
                                 code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                                 {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "11450-4",
                                             display = "Problem list - Reported"
                                        }
                                    }
                                 },
                                 entry = entryTemp //Aqui agrego el EntryTemp
                            },
                            sectionHistorialAllergies, //ALERGIAS
                            sectionMed, //MEDICAMENTOS
                            new Domain.FHIR.CE.RDAConsultaExterna.Section() //DOCUMENTOS SOPORTE
                            {
                                 title = "Documentos de soporte",
                                 code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                                 {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "55107-7",
                                             display = "Addendum Document"
                                        }
                                    }
                                 },
                                    entry = new List<Domain.FHIR.CE.RDAConsultaExterna.Entry>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Entry()
                                        {
                                            reference = "#DocumentReference-0"
                                        }
                                    }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Section() //SERVICE REQUEST DERIVA ATENCION INMEDIATA Y URGENCIA N/A
                            {
                                 title = "Órdenes, prescripciones o solicitudes de servicio",
                                 code = new Domain.FHIR.CE.RDAConsultaExterna.Code2()
                                 {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                             system = "http://loinc.org",
                                             code = "61146-1",
                                             display = "Orders for services Document"
                                        }
                                    }
                                 },
                                 emptyReason = new Domain.FHIR.CE.RDAConsultaExterna.EmptyReason()
                                 {
                                     coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                     {
                                         new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                         {
                                             system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                             code = "nilknown",
                                             display = "Nil Known"
                                         }
                                     }
                                 },
                                 text = new Domain.FHIR.CE.RDAConsultaExterna.Text()
                                 {
                                     status = "generated",
                                     div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                                 }
                            }
                        }
                    };

                    composition.section.Add(sectionIncTemp); //INCAPACIDADES              
                    composition.section.Add(sectionRiskTemp); //FACTORES DE RIESGO

                    builder.AddResource(composition);
                    #endregion FIN RECURSO COMPOSITION

                    #region RECURSO PATIENT
                    string paisName = DatosPaciente.Pac_PaisOrigen == null ? "Colombia" : rPaciente.getNamePais(DatosPaciente.Pac_PaisOrigen);
                    TextInfo ti = new CultureInfo("es-CO", false).TextInfo;
                    string paisNamePrint = ti.ToTitleCase(paisName.ToLower());
                    string ciudadName = DatosPaciente.Pac_Mun_Cod == null || DatosPaciente.Pac_Dep_Cod == null ? "Bogotá D.C." :
                                        rZonas.MunicipioNombre(DatosPaciente.Pac_Mun_Cod, DatosPaciente.Pac_Dep_Cod) == "BOGOTÁ, D.C." ? "Bogotá D.C." :
                                        rZonas.MunicipioNombre(DatosPaciente.Pac_Mun_Cod, DatosPaciente.Pac_Dep_Cod);

                    var resourcePatient = new Domain.FHIR.CE.RDAConsultaExterna.Patient
                    {
                        resourceType = "Patient",
                        id = PATIENT_ID.Replace("#", ""),
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                        {
                            profile = new List<string>()
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/PatientRDA"
                            }
                        },
                        extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension2>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Extension2()
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientNationality",
                                valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                {
                                    system= "https://fhir.minsalud.gov.co/rda/CodeSystem/ISO31661",
                                    code = DatosPaciente.Pac_PaisOrigen.Trim() == null ? "170" : DatosPaciente.Pac_PaisOrigen.Trim(),
                                    display = paisNamePrint
                                }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Extension2()
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientEthnicity",
                                valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                {
                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianEthnicGroup",
                                    code = DatosPaciente.Etnia == null ? "6" : rGenerales.GetCodeEtnia(DatosPaciente.Etnia),
                                    display = DatosPaciente.Etnia == null ? "Otras etnias" : DatosPaciente.Etnia
                                }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Extension2()
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientDisability",
                                valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                {
                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianDisabilityClassification",
                                    code = string.IsNullOrEmpty(DatosPaciente.Discapacidad.Trim()) ? "08" : rGenerales.GetCodeEtnia(DatosPaciente.Discapacidad.Trim()),
                                    display = string.IsNullOrEmpty(DatosPaciente.Discapacidad.Trim()) ? "Sin discapacidad" : DatosPaciente.Discapacidad.Trim()
                                }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Extension2()
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientGenderIdentity",
                                valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                {
                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianGenderIdentity",
                                    code = DatosPaciente.Pac_Sexo == null ? "01" :
                                           DatosPaciente.Pac_Sexo == "M" ? "01" :
                                           DatosPaciente.Pac_Sexo == "F" ? "02" :
                                           "03",
                                    display = DatosPaciente.Pac_Sexo == null ? "Masculino" :
                                              DatosPaciente.Pac_Sexo == "M" ? "Masculino" :
                                              DatosPaciente.Pac_Sexo == "F" ? "Femenino" :
                                              "Indeterminado o Intersexual"
                                }
                            }
                        },
                        identifier = new List<Domain.FHIR.CE.RDAConsultaExterna.Identifier>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Identifier()
                            {
                                type = new Domain.FHIR.CE.RDAConsultaExterna.Type()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/v2-0203",
                                            code = "PN",
                                            display = "Person number"
                                        },
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianPersonIdentifier",
                                            code = rPaciente.getTipoDoc(DatosPaciente.Pac_TipoId.Trim()),
                                            display = DatosPaciente.Pac_TipoId.Trim()
                                        }
                                    }
                                },
                                id = "NationalPersonIdentifier-0",
                                use = "official",
                                system = "https://fhir.minsalud.gov.co/rda/NamingSystem/RNEC",
                                value = datosCita.Pac_IdNum.Trim()
                            }
                        },
                        name = new List<Domain.FHIR.CE.RDAConsultaExterna.Name>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Name()
                            {
                                given = new List<string>()
                                {
                                    datosCita.Pac_PrimerN.ToUpper().Trim(),
                                    datosCita.Pac_SegundoN.ToUpper().Trim()
                                },
                                use = "official",
                                family = (datosCita.Pac_PrimerA + " " + datosCita.SegundoApellido).ToUpper().Trim(),
                                _family = new Domain.FHIR.CE.RDAConsultaExterna.Family()
                                {
                                    extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension3>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Extension3()
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionFathersFamilyName",
                                            valueString = DatosPaciente.Pac_PrimerA.ToUpper().Trim()
                                        },
                                        new Domain.FHIR.CE.RDAConsultaExterna.Extension3()
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionMothersFamilyName",
                                            valueString = DatosPaciente.Pac_SegundoA.ToUpper().Trim()
                                        }
                                    }
                                }
                            }
                        },
                        address = new List<Domain.FHIR.CE.RDAConsultaExterna.Address>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Address()
                            {
                                id = "HomeAddress-0",
                                use = "home",
                                type = "physical",
                                city = ciudadName,
                                _city = new Domain.FHIR.CE.RDAConsultaExterna.City()
                                {
                                    extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionDivipolaMunicipality",
                                            valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                            {
                                                code = DatosPaciente.Pac_Dep_Cod == null || DatosPaciente.Pac_Mun_Cod == null ? "11001" :
                                                       DatosPaciente.Pac_Dep_Cod + DatosPaciente.Pac_Mun_Cod,
                                                system = "https://fhir.minsalud.gov.co/rda/CodeSystem/DIVIPOLA"
                                            }
                                        }
                                    }
                                },
                                country = "Colombia",
                                _country = new Domain.FHIR.CE.RDAConsultaExterna.Country()
                                {
                                    extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionCountryCode",
                                            valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                            {
                                                system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ISO31661",
                                                code = "170"
                                            }
                                        }
                                    }
                                },
                                extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension2>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Extension2()
                                    {
                                        url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionResidenceZone",
                                        valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianResidenceZone",
                                            code = DatosPaciente.Pac_Zona == null ? "01" : DatosPaciente.Pac_Zona.Trim() == "U" ? "01" : "02",
                                            display = DatosPaciente.Pac_Zona == null ? "Urbana" : DatosPaciente.Pac_Zona == "U" ? "Urbana" : "Rural"
                                        }
                                    }
                                }
                            }
                        },
                        active = true,
                        gender = DatosPaciente.Pac_Sexo == null ? "male" : DatosPaciente.Pac_Sexo == "M" ? "male" : "female",
                        _gender = new Domain.FHIR.CE.RDAConsultaExterna.Gender()
                        {
                            extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                                {
                                    url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionBiologicalGender",
                                    valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                    {
                                        system =  "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianGenderGroup",
                                        code = DatosPaciente.Pac_Sexo == null ? "01" : DatosPaciente.Pac_Sexo.Trim() == "M" ? "01" : "02",
                                        display = DatosPaciente.Pac_Sexo == null ? "Hombre" : DatosPaciente.Pac_Sexo.Trim() == "M" ? "Hombre" : "Mujer"
                                    }
                                }
                            }
                        },
                        birthDate = Convert.ToDateTime(new DateTime(datosCita.Pac_FechaNto.Year, datosCita.Pac_FechaNto.Month, datosCita.Pac_FechaNto.Day)),
                        _birthDate = new Domain.FHIR.CE.RDAConsultaExterna.BirthDate()
                        {
                            extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                                {
                                    url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionBirthTime",
                                    valueTime = "14:30:00"
                                }
                            }
                        },
                        deceasedBoolean = false,
                    };
                    builder.AddResource(resourcePatient);
                    #endregion FIN RECURSO PATIENT

                    #region RECURSO ORGANIZATION
                    var resourceOrganization = new Domain.FHIR.CE.RDAConsultaExterna.Resource()
                    {
                        resourceType = "Organization",
                        id = DatosPrestador.Com_Cod_Prestador_2,
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                        {
                            profile = new List<string>()
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/CareDeliveryOrganizationRDA"
                            }
                        },
                        identifier = new List<Domain.FHIR.CE.RDAConsultaExterna.Identifier>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Identifier()
                            {
                                id = "TaxIdentifier-0",
                                use = "official",
                                type = new Domain.FHIR.CE.RDAConsultaExterna.Type()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/v2-0203",
                                            code = "TAX",
                                            display = "Tax ID number"
                                        },
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianOrganizationIdentifiers",
                                            code = "NIT",
                                            display = "Número de Identificación Tributaria"
                                        }
                                    }
                                },
                                value = "Desconocido"
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Identifier()
                            {
                                id = "HealthcareProviderIdentifier-0",
                                use = "official",
                                type = new Domain.FHIR.CE.RDAConsultaExterna.Type()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/v2-0203",
                                            code = "PRN",
                                            display = "Provider number"
                                        },
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianOrganizationIdentifiers",
                                            code = "CodigoPrestador",
                                            display = "Código de habilitación de prestador de servicios de salud"
                                        }
                                    }
                                },
                                system = "http://co.fhir.guide/NamingSystem/REPS",
                                value = DatosPrestador.Com_Cod_Prestador_2
                            }
                        }
                    };
                    builder.AddResource(resourceOrganization);
                    #endregion FIN RECURSO ORGANIZATION

                    #region RECURSO PRACTITIONER
                    var practitioner = new Domain.FHIR.CE.RDAConsultaExterna.Practitioner
                    {
                        resourceType = "Practitioner",
                        id = PRACTITIONER_ID.Replace("#", ""),
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta
                        {
                            profile = new List<string>()
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/PractitionerRDA"
                            }
                        },
                        identifier = new List<Domain.FHIR.CE.RDAConsultaExterna.Identifier>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Identifier
                            {
                                id = "NationalPersonIdentifier-0",
                                use = "official",
                                type = new Domain.FHIR.CE.RDAConsultaExterna.Type
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/v2-0203",
                                            code = "PN",
                                            display = "Person number"
                                        },
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianPersonIdentifier",
                                            code = "CC",
                                            display = "Cédula ciudadanía"
                                        }
                                    }
                                },
                                value = datosCita.IdentificacionProfesional.Trim()
                            }
                        },
                        name = new List<Domain.FHIR.CE.RDAConsultaExterna.Name>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Name
                            {
                                use = "official",
                                family = DatosLogin.Log_PrimerA.ToUpper().Trim() + " " + DatosLogin.Log_SegundoA.ToUpper().Trim(),
                                _family = new Domain.FHIR.CE.RDAConsultaExterna.Family()
                                {
                                    extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension3>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Extension3()
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionFathersFamilyName",
                                            valueString = "PrimerApellido"
                                        },
                                        new Domain.FHIR.CE.RDAConsultaExterna.Extension3()
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionMothersFamilyName",
                                            valueString =  "SegundoApellido"
                                        }
                                    }
                                },
                                given = new List<string>()
                                {
                                    DatosLogin.Log_PrimerN.ToUpper().Trim(),
                                    DatosLogin.Log_SegundoN.ToUpper().Trim()
                                },
                            }
                        }
                    };
                    builder.AddResource(practitioner);
                    #endregion FIN RECURSO PRACTITIONER

                    #region RECURSO CONDITION - DX
                    var conditionPrincipal = new Domain.FHIR.CE.RDAConsultaExterna.Condition
                    {
                        resourceType = "Condition",
                        id = CONDITION_MAIN_ID,
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta
                        {
                            profile = new List<string>
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/ConditionRDA"
                            }
                        },
                        clinicalStatus = new Domain.FHIR.CE.RDAConsultaExterna.ClinicalStatus
                        {
                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                {
                                    code = "active",
                                    system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                    display = "Active"
                                }
                            }
                        },
                        verificationStatus = new Domain.FHIR.CE.RDAConsultaExterna.VerificationStatus
                        {
                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                {
                                    code = "confirmed",
                                    display = "Confirmed"
                                }
                            }
                        },
                        category = new List<Domain.FHIR.CE.RDAConsultaExterna.Category>
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Category
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                    {
                                        system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                        code = "encounter-diagnosis",
                                        display = "Encounter Diagnosis"
                                    }
                                }
                            }
                        },
                        code = new Domain.FHIR.CE.RDAConsultaExterna.Code
                        {
                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                {
                                    system = "http://hl7.org/fhir/sid/icd-10",
                                    code = DatosCargo.Car_Dx1,
                                    display = DxTexto
                                }
                            }
                        },
                        subject = new Domain.FHIR.CE.RDAConsultaExterna.Reference
                        {
                            reference = PATIENT_ID
                        }
                    };
                    builder.AddResource(conditionPrincipal);
                    #endregion FIN RECURSO CONDITION - DX

                    //Allergy Intolerance
                    if (allergyTemp != null)
                    {
                        foreach (var i in allergyTemp)
                        {
                            builder.AddResource(i);
                        }
                    }

                    //Medication
                    if (NumOrdenMed > 0)
                    {
                        int CountMed = 0;

                        foreach (var i in Exportar)
                        {
                            var medication = new Domain.FHIR.CE.RDAConsultaExterna.MedicationRequest()
                            {
                                resourceType = "MedicationRequest",
                                id = $"MedicationRequest-{CountMed}",
                                meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                                {
                                    profile = new List<string>()
                                    {
                                        "https://fhir.minsalud.gov.co/rda/StructureDefinition/MedicationRequestRDA"
                                    }
                                },
                                status = "active",
                                intent = "order",
                                category = new List<Domain.FHIR.CE.RDAConsultaExterna.Category>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Category()
                                    {
                                        coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                        {
                                            new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                            {
                                                system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianHealthTechnologyCategory",
                                                code = i.OM_Tecnologia,
                                                display = i.OM_Tecnologia == "02" ? "Medicamento con registro sanitario" :
                                                          i.OM_Tecnologia == "03" ? "Medicamento vital no disponible" :
                                                          i.OM_Tecnologia == "04" ? "Preparación magistral" :
                                                          i.OM_Tecnologia == "05" ? "Medicamento UNIRS" : "Medicamento con registro sanitario"
                                            }
                                        }
                                    }
                                },
                                reportedBoolean = true,
                                medicationCodeableConcept = new Domain.FHIR.CE.RDAConsultaExterna.MedicationCodeableConcept()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/MipresINN",
                                            code = rFHIR.GetCodeMedicamento(i.OM_Medicamento),
                                            display = i.OM_Medicamento
                                        }
                                    }
                                },
                                subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                                {
                                    reference = PATIENT_ID
                                },
                                encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter()
                                {
                                    reference = "#" + ENCOUNTER_ID
                                },
                                authoredOn = new DateTime(datosCita.Hor_Pac_Fecha_Cita.Year, datosCita.Hor_Pac_Fecha_Cita.Month, datosCita.Hor_Pac_Fecha_Cita.Day),
                                requester = new Domain.FHIR.CE.RDAConsultaExterna.Requester()
                                {
                                    reference = "#CC-" + datosCita.IdentificacionProfesional
                                },
                                reasonCode = new List<Domain.FHIR.CE.RDAConsultaExterna.ReasonCode>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.ReasonCode()
                                    {
                                        coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                        {
                                            new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                            {
                                                system = "https://fhir.minsalud.gov.co/rda/CodeSystem/RIPSFinalidadConsultaVersion2",
                                                code = datosCita.HorTecnoSalud.Trim(),
                                                display = rFHIR.GetDesctecnoSalud(datosCita.HorTecnoSalud.Trim()).Trim().ToUpper()
                                            }
                                        }
                                    }
                                },
                                dosageInstruction = new List<Domain.FHIR.CE.RDAConsultaExterna.Dosage>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Dosage()
                                    {
                                        timing = new Domain.FHIR.CE.RDAConsultaExterna.Timing()
                                        {
                                            repeat = new Domain.FHIR.CE.RDAConsultaExterna.Repeat()
                                            {
                                                duration = Convert.ToInt32(i.OM_Duracion),
                                                durationUnit = "d"
                                            }
                                        },
                                        code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                                        {
                                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                            {
                                                new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                                {
                                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/MedicationTime",
                                                    code = i.OM_Cantidad,
                                                    display = "Día"
                                                }
                                            }
                                        },
                                        route = new Domain.FHIR.CE.RDAConsultaExterna.Route()
                                        {
                                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                            {
                                                new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                                {
                                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/VAD",
                                                    code = rFHIR.GetCodeConsumo(i.OM_Via.ToUpper().Trim()),
                                                    display = i.OM_Via.ToUpper().Trim()
                                                }
                                            }
                                        },
                                        doseAndRate = new List<Domain.FHIR.CE.RDAConsultaExterna.DoseAndRate>()
                                        {
                                            new Domain.FHIR.CE.RDAConsultaExterna.DoseAndRate()
                                            {
                                                doseQuantity = new Domain.FHIR.CE.RDAConsultaExterna.DoseQuantity()
                                                {
                                                    value = 10,
                                                    unit = "mg",
                                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/UMM",
                                                    code = "168"
                                                },
                                                rateQuantity = new Domain.FHIR.CE.RDAConsultaExterna.RateQuantity()
                                                {
                                                    value = 0,
                                                    unit = "Día",
                                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/MedicationTime",
                                                    code = "3"
                                                }
                                            }
                                        }
                                    }
                                }
                            };
                            builder.AddResource(medication);
                            CountMed++;
                        }
                    }

                    #region RECURSO ENCOUNTER
                    var encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter
                    {
                        resourceType = "Encounter",
                        id = ENCOUNTER_ID,
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta
                        {
                            profile = new List<string>()
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/EncounterAmbulatoryRDA"
                            }
                        },
                        identifier = new List<Domain.FHIR.CE.RDAConsultaExterna.Identifier>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Identifier()
                            {
                                id = "EncounterIdentifier",
                                use = "usual",
                                system = "https://fhir.minsalud.gov.co/rda/NamingSystem/Encounters",
                                value = "ADT-HS-9864463-12"
                            }
                        },
                        status = "finished",
                        @class = new Domain.FHIR.CE.RDAConsultaExterna.Class
                        {
                            system = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                            code = "AMB",
                            display = "ambulatory"
                        },
                        type = new List<Domain.FHIR.CE.RDAConsultaExterna.Type3>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Type3()
                            {
                                coding = new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                {
                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianTechModality",
                                    code = datosCita.Hor_Pac_Modalidad == null ? "01" : datosCita.Hor_Pac_Modalidad.Trim(),
                                    display = datosCita.Hor_Pac_Modalidad == null ? "Intramural" :
                                              datosCita.Hor_Pac_Modalidad == "01" ? "Intramural" :
                                              datosCita.Hor_Pac_Modalidad == "02" ? "Extramural unidad móvil" :
                                              datosCita.Hor_Pac_Modalidad == "03" ? "Extramural domiciliaria" :
                                              datosCita.Hor_Pac_Modalidad == "04" ? "Extramural jornada desalud" :
                                              datosCita.Hor_Pac_Modalidad == "05" ? "Extramural (atención prehospitalaria o transporte asistencial)" :
                                              datosCita.Hor_Pac_Modalidad == "06" ? "Telemedicina interactiva" :
                                              datosCita.Hor_Pac_Modalidad == "07" ? "Telemedicina no interactiva" :
                                              datosCita.Hor_Pac_Modalidad == "08" ? "Telemedicina - Telexpertlcia" :
                                              datosCita.Hor_Pac_Modalidad == "09" ? "Telemedicina - Telemonitoreo" : "Intramural"
                                }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Type3()
                            {
                                coding = new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                {
                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/GrupoServicios",
                                    code = datosCita.Hor_GrupoServicios == null ? "01" : datosCita.Hor_GrupoServicios.Trim(),
                                    display = datosCita.Hor_GrupoServicios == null ? "Consulta externa" :
                                              datosCita.Hor_GrupoServicios == "01" ? "Consulta externa" :
                                              datosCita.Hor_GrupoServicios == "02" ? "Apoyo diagnóstico y complementación terapéutica" :
                                              datosCita.Hor_GrupoServicios == "03" ? "Internación" :
                                              datosCita.Hor_GrupoServicios == "04" ? "Quirúrgico" :
                                              datosCita.Hor_GrupoServicios == "05" ? "Atención inmediata" :
                                              "Consulta externa"
                                }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Type3()
                            {
                                 coding = new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                 {
                                     system = "https://fhir.minsalud.gov.co/rda/CodeSystem/REPShealthcareServices",
                                     code = datosCita.Com_Doc_Soporte.ToString(),
                                     display = rAgendaC.getNameServicioFHIR(datosCita.Com_Doc_Soporte)
                                 }
                            },
                            new Domain.FHIR.CE.RDAConsultaExterna.Type3()
                            {
                                 coding = new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                 {
                                     system = "https://fhir.minsalud.gov.co/rda/CodeSystem/EntornoAtencion",
                                     code = "05",
                                     display = "Institucional"
                                 }
                            }
                        },
                        serviceType = new Domain.FHIR.CE.RDAConsultaExterna.ServiceType()
                        {
                            coding = new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                            {
                                system = "https://fhir.minsalud.gov.co/rda/CodeSystem/CUPS",
                                code = datosCita.Hor_Pac_Cup.Trim() == "1005436" ||
                                       datosCita.Hor_Pac_Cup.Trim() == "1005436." ||
                                       datosCita.Hor_Pac_Cup.Trim() == "1005435" ||
                                       datosCita.Hor_Pac_Cup.Trim() == "1005435." ||
                                       datosCita.Hor_Pac_Cup.Trim() == "1005434." ||
                                       datosCita.Hor_Pac_Cup.Trim() == "1005434" ? 
                                       "890201" : datosCita.Hor_Pac_Cup.Trim(),
                                display = datosCita.Hor_Pac_Cup.Trim() == "1005436" ||
                                          datosCita.Hor_Pac_Cup.Trim() == "1005436." ||
                                          datosCita.Hor_Pac_Cup.Trim() == "1005435" ||
                                          datosCita.Hor_Pac_Cup.Trim() == "1005435." ||
                                          datosCita.Hor_Pac_Cup.Trim() == "1005434." ||
                                          datosCita.Hor_Pac_Cup.Trim() == "1005434" ?
                                          "CONSULTA DE PRIMERA VEZ POR MEDICINA GENERAL" : datosCita.Com_Nombre_SMS.ToUpper().Trim()
                            }
                        },
                        subject = new Domain.FHIR.CE.RDAConsultaExterna.Reference
                        {
                            reference = PATIENT_ID
                        },
                        participant = new List<Domain.FHIR.CE.RDAConsultaExterna.EncounterParticipant>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.EncounterParticipant
                            {
                                id = "AttenderPhysician",
                                type = new List<Domain.FHIR.CE.RDAConsultaExterna.Type>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Type
                                    {
                                        coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                        {
                                            new Domain.FHIR.CE.RDAConsultaExterna.Coding
                                            {
                                                system = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                                code = "ATND",
                                                display = "attender"
                                            }
                                        }
                                    }
                                },
                                individual = new Domain.FHIR.CE.RDAConsultaExterna.Reference
                                {
                                    reference = "#CC-" + datosCita.IdentificacionProfesional.Trim()
                                }
                            }
                        },
                        period = new Domain.FHIR.CE.RDAConsultaExterna.Period
                        {
                            start = DateTime.Now.AddMinutes(-30),
                            end = DateTime.Now
                        },
                        reasonCode = new List<Domain.FHIR.CE.RDAConsultaExterna.ReasonCode>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.ReasonCode()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "https://fhir.minsalud.gov.co/rda/CodeSystem/RIPSCausaExternaVersion2",
                                        code = DatosCargo.Car_CExterna == 0 ? "38" : DatosCargo.Car_CExterna.ToString(),
                                        display = DatosCargo.Car_CExterna == 0 ? "ENFERMEDAD GENERAL" : rRIPS.getNameTecnoSaludCExterna(DatosCargo.Car_CExterna).ToUpper().Trim()
                                    }
                                }
                            }
                        },
                        diagnosis = new List<Domain.FHIR.CE.RDAConsultaExterna.Diagnosis>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Diagnosis()
                            {
                                id = "MainDiagnosis",
                                extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                                    {
                                        url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionDiagnosisType",
                                        valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/RIPSTipoDiagnosticoPrincipalVersion2",
                                            code = DatosCargo.Car_Imp_Dx == 0 ? "02" :
                                                   DatosCargo.Car_Imp_Dx == 1 ? "01" :
                                                   DatosCargo.Car_Imp_Dx == 2 ? "02" :
                                                   DatosCargo.Car_Imp_Dx == 3 ? "03" : "02",
                                            display = DatosCargo.Car_Imp_Dx == 0 ? "Confirmado Nuevo" :
                                                      DatosCargo.Car_Imp_Dx == 1 ? "Impresión Diagnóstica" :
                                                      DatosCargo.Car_Imp_Dx == 2 ? "Confirmado Nuevo" :
                                                      DatosCargo.Car_Imp_Dx == 3 ? "Confirmado Repetido" : "Confirmado Nuevo"
                                        }
                                    }
                                },
                                condition = new Domain.FHIR.CE.RDAConsultaExterna.Condition()
                                {
                                    reference = "#" + CONDITION_MAIN_ID
                                },
                                use = new Domain.FHIR.CE.RDAConsultaExterna.Use()
                                {
                                    coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                    {
                                        new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianDiagnosisRole",
                                            code = "8319008",
                                            display = "diagnóstico primario"
                                        }
                                    }
                                },
                                rank = 1
                            }
                        },
                        extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionDischargeDisposition",
                                extension = new List<Domain.FHIR.CE.RDAConsultaExterna.Extension>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                                    {
                                        url = "DispositionCode",
                                        valueCoding = new Domain.FHIR.CE.RDAConsultaExterna.ValueCoding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/CondicionyDestinoUsuarioEgreso",
                                            code = DatosCargo.Car_CodeEgreso == 0 ? "01" :
                                                   DatosCargo.Car_CodeEgreso == 1 ? "01" :
                                                   DatosCargo.Car_CodeEgreso == 2 ? "02" :
                                                   DatosCargo.Car_CodeEgreso == 3 ? "03" :
                                                   DatosCargo.Car_CodeEgreso == 4 ? "04" :
                                                   DatosCargo.Car_CodeEgreso == 5 ? "05" :
                                                   DatosCargo.Car_CodeEgreso == 6 ? "06" :
                                                   DatosCargo.Car_CodeEgreso == 7 ? "07" :
                                                   DatosCargo.Car_CodeEgreso == 8 ? "08" : "01",
                                            display = DatosCargo.Car_CodeEgreso == 0 ? "PACIENTE CON DESTINO A SU DOMICILIO" :
                                                   DatosCargo.Car_CodeEgreso == 1 ? "PACIENTE CON DESTINO A SU DOMICILIO" :
                                                   DatosCargo.Car_CodeEgreso == 2 ? "PACIENTE MUERTO" :
                                                   DatosCargo.Car_CodeEgreso == 3 ? "PACIENTE DERIVADO A OTRO SERVICIO" :
                                                   DatosCargo.Car_CodeEgreso == 4 ? "REFERIDO A OTRA INSTITUCION" :
                                                   DatosCargo.Car_CodeEgreso == 5 ? "CONTRAREFERIDO A OTRA INSTITUCION" :
                                                   DatosCargo.Car_CodeEgreso == 6 ? "DERIVADO O REFERIDO A HOSPITALIZACION DOMICILIRIA" :
                                                   DatosCargo.Car_CodeEgreso == 7 ? "DERIVADO A SERVICIO SOCIAL" :
                                                   DatosCargo.Car_CodeEgreso == 8 ? "PACIENTE CONTINUA EN EL SERVICIO (CORTE FACTURACION)" : "PACIENTE CON DESTINO A SU DOMICILIO",
                                        }
                                    },
                                    new Domain.FHIR.CE.RDAConsultaExterna.Extension()
                                    {
                                        url = "ReferenceOrganization",
                                        valueReference = new Domain.FHIR.CE.RDAConsultaExterna.ValueReference()
                                        {
                                            reference = "#" + DatosPrestador.Com_Cod_Prestador_2.Trim()
                                        }
                                    }
                                }

                            }
                        },
                        location = new List<Domain.FHIR.CE.RDAConsultaExterna.Location>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Location()
                            {
                                location = new Domain.FHIR.CE.RDAConsultaExterna.Location()
                                {
                                    reference = "#" + DatosPrestador.Com_Cod_Prestador_2 + "-01"
                                }
                            }
                        },
                        serviceProvider = new Domain.FHIR.CE.RDAConsultaExterna.ServiceProvider()
                        {
                            reference = "#" + DatosPrestador.Com_Cod_Prestador_2
                        }
                    };
                    builder.AddResource(encounter);
                    #endregion FIN RECURSO ENCOUNTER

                    #region RECURSO LOCATION
                    var location = new Domain.FHIR.CE.RDAConsultaExterna.Location()
                    {
                        resourceType = "Location",
                        id = DatosPrestador.Com_Cod_Prestador_2 + "-01",
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                        {
                            profile = new List<string>()
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/CareDeliveryLocationRDA"
                            }
                        },
                        identifier = new List<Domain.FHIR.CE.RDAConsultaExterna.Identifier>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Identifier()
                            {
                                use = "official",
                                system = "http://co.fhir.guide/NamingSystem/REPS",
                                value = DatosPrestador.Com_Cod_Prestador_2 + "-01"
                            }
                        },
                        name = DatosPrestador.Com_Nombre.Trim().ToUpper(),
                        managingOrganization = new Domain.FHIR.CE.RDAConsultaExterna.ManagingOrganization()
                        {
                            reference = "#" + DatosPrestador.Com_Cod_Prestador_2
                        }
                    };
                    builder.AddResource(location);
                    #endregion FIN RECURSO LOCATION

                    //recurso Observation - Incapacidades
                    if (NumOrden > 0)
                    {
                        var observation = new Domain.FHIR.CE.RDAConsultaExterna.Observation()
                        {
                            resourceType = "Observation",
                            id = "Observation-0",
                            meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                            {
                                profile = new List<string>()
                                {
                                     "https://fhir.minsalud.gov.co/rda/StructureDefinition/AttendanceAllowanceRDA"
                                }
                            },
                            status = "final",
                            code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "http://snomed.info/sct",
                                        code = "160983005",
                                        display = "permiso de concurrencia"
                                    }
                                },
                                text = "Datos incapacidad (SIPE – Sistema de Incapacidades y Prestaciones Economicas)"
                            },
                            subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                            {
                                reference = PATIENT_ID
                            },
                            encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter()
                            {
                                reference = "#" + ENCOUNTER_ID
                            },
                            component = new List<Domain.FHIR.CE.RDAConsultaExterna.Component>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Component()
                                {
                                    id = "LicenseScope",
                                    code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                                    {
                                        coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                        {
                                            new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                            {
                                                system = "http://snomed.info/sct",
                                                code = "255590007",
                                                display = "alcance"
                                            }
                                        },
                                        text =  "Incapacidad - Alcance de la incapacidad"
                                    },
                                    valueCodeableConcept = new Domain.FHIR.CE.RDAConsultaExterna.ValueCodeableConcept()
                                    {
                                         coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                            {
                                                 new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                                {
                                                     system =  "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianLicenseScope",
                                                     code = TipoOrden,
                                                     display = TipoOrden == "01" ? "Nueva" : "Prórroga"
                                                }
                                            }
                                    }
                                },
                                new Domain.FHIR.CE.RDAConsultaExterna.Component()
                                {
                                    id = "LicenceTime",
                                    code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                                    {
                                        coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                        {
                                            new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                            {
                                                system = "http://snomed.info/sct",
                                                code = "410670007",
                                                display =  "tiempo"
                                            }
                                        },
                                        text =  "Días de incapacidad"
                                    },
                                    valueQuantity = new Domain.FHIR.CE.RDAConsultaExterna.ValueQuantity()
                                    {
                                        value = diasOM,
                                        unit = "días",
                                        system =  "http://unitsofmeasure.org",
                                        code = "d"
                                    }
                                }
                            },
                        };
                        builder.AddResource(observation);
                    }

                    #region RECURSO OBSERVATION - OCUPATION
                    var observation2 = new Domain.FHIR.CE.RDAConsultaExterna.Observation()
                    {
                        resourceType = "Observation",
                        id = "Observation-1",
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                        {
                            profile = new List<string>()
                            {
                                 "https://fhir.minsalud.gov.co/rda/StructureDefinition/PatientOccupationAtEncounterRDA"
                            }
                        },
                        status = "final",
                        code = new Domain.FHIR.CE.RDAConsultaExterna.Code()
                        {
                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                {
                                    system = "http://snomed.info/sct",
                                    code = "184104002",
                                    display = "ocupación del paciente"
                                }
                            },
                            text = "Ocupación del paciente en el momento de la atención"
                        },
                        subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                        {
                            reference = PATIENT_ID
                        },
                        valueCodeableConcept = new Domain.FHIR.CE.RDAConsultaExterna.ValueCodeableConcept()
                        {
                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                {
                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/CIUO88AC",
                                    code = DatosPaciente.Pac_Ocupacion == "" ? "9210" :
                                           rFHIR.GetCodeOcupacion(DatosPaciente.Pac_Ocupacion),
                                    display =  DatosPaciente.Pac_Ocupacion == "" ? "Personal doméstico" :
                                               DatosPaciente.Pac_Ocupacion
                                }
                            }
                        }
                    };
                    builder.AddResource(observation2);
                    #endregion FIN RECURSO OBSERVATION - OCUPATION

                    //recurso RiskAssessment                    
                    foreach (var i in lista_Risk)
                    {
                        builder.AddResource(i);
                    }

                    #region RECURSO ORGANIZATION
                    var organization = new Domain.FHIR.CE.RDAConsultaExterna.Organization()
                    {
                        resourceType = "Organization",
                        id = datosCita.PacienteAseguradora.Trim(),
                        name = datosCita.Com_Telefono_SMS.ToUpper().Trim()
                    };
                    builder.AddResource(organization);
                    #endregion FIN RECURSO ORGANIZATION

                    //Recurso ServiceRequest No agregado debido a que la 1888 de 2025 dice que es para tencio inmediata y urgencias al momento de egreso
                    /* var serviceRequest = new Domain.FHIR.CE.RDAConsultaExterna.ServiceRequest()
                     {
                         resourceType = "ServiceRequest",
                         id = "ServiceRequest-0",
                         meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                         {
                             profile = new List<string>()
                             {
                                  "https://fhir.minsalud.gov.co/rda/StructureDefinition/OtherTechnologyServiceRequestRDA"
                             }
                         },
                         subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                         {
                             reference = PATIENT_ID
                         },
                         encounter = new Domain.FHIR.CE.RDAConsultaExterna.Encounter()
                         {
                             reference = "#" + ENCOUNTER_ID
                         }
                     };
                     builder.AddResource(serviceRequest);*/


                    #region RECURSO PDF
                    ReportViewer R = new ReportViewer();

                    byte[] bytes = null;
                    string pdfBase64 = "";

                    R.LocalReport.DataSources.Clear();

                    if (Preferencias.CuracionesCORE == "A" && getHistory[0].listaMedidas != null)
                    {
                        R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Notas", getHistory));
                        R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_NotasMed", getHistory[0].listaMedidas));
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_NotasCore.rdlc";                      
                    }
                    else
                    {
                        R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Notas", getHistory));
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_Notas.rdlc";
                    }

                    R.SetDisplayMode(DisplayMode.PrintLayout);
                    R.ZoomMode = ZoomMode.Percent;
                    R.ZoomPercent = 100;
                    R.Font = new System.Drawing.Font("Arial", 7);
                    R.LocalReport.EnableExternalImages = true;
                    R.RefreshReport();
                    R.Dock = System.Windows.Forms.DockStyle.Fill;
                    bytes = R.LocalReport.Render("PDF");
                    pdfBase64 = Convert.ToBase64String(bytes);

                    var documentReference = new Domain.FHIR.CE.RDAConsultaExterna.DocumentReference()
                    {
                        resourceType = "DocumentReference",
                        id = "DocumentReference-0",
                        meta = new Domain.FHIR.CE.RDAConsultaExterna.Meta()
                        {
                            profile = new List<string>()
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/DocumentReferenceEPIRDA"
                            }
                        },
                        text = new Domain.FHIR.CE.RDAConsultaExterna.Text()
                        {
                            status = "generated",
                            div = "\u003Cdiv xmlns=\u0022http://www.w3.org/1999/xhtml\u0022\u003EDocument Reference\u003C/div\u003E"
                        },
                        status = "current",
                        type = new Domain.FHIR.CE.RDAConsultaExterna.Type()
                        {
                            coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                {
                                    system = "http://loinc.org",
                                    code = "18842-5",
                                    display  ="Discharge summary"
                                },
                                new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                {
                                    system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianDocumentTypes",
                                    code = "EPI",
                                    display = "Epicrisis"
                                }
                            }
                        },
                        category = new List<Domain.FHIR.CE.RDAConsultaExterna.Category>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Category()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "http://loinc.org",
                                        code  ="55108-5",
                                        display = "Clinical presentation Document"
                                    }
                                }
                            }
                        },
                        subject = new Domain.FHIR.CE.RDAConsultaExterna.Subject()
                        {
                            reference = PATIENT_ID
                        },
                        date = "2025-02-18T14:00:00-05:00",
                        author = new List<Domain.FHIR.CE.RDAConsultaExterna.Author>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Author()
                            {
                                reference = "#" + DatosPrestador.Com_Cod_Prestador_2.Trim()
                            }
                        },
                        custodian = new Domain.FHIR.CE.RDAConsultaExterna.Custodian()
                        {
                            reference = "Organization/MinSalud"
                        },
                        description = "Epicrisis del encuentro de atención en salud - RDA",
                        securityLabel = new List<Domain.FHIR.CE.RDAConsultaExterna.SecurityLabel>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.SecurityLabel()
                            {
                                coding = new List<Domain.FHIR.CE.RDAConsultaExterna.Coding>()
                                {
                                    new Domain.FHIR.CE.RDAConsultaExterna.Coding()
                                    {
                                        system = "http://terminology.hl7.org/CodeSystem/v3-Confidentiality",
                                        code  ="R",
                                        display = "restricted"
                                    }
                                }
                            }
                        },
                        content = new List<Domain.FHIR.CE.RDAConsultaExterna.Content>()
                        {
                            new Domain.FHIR.CE.RDAConsultaExterna.Content()
                            {
                                attachment = new Domain.FHIR.CE.RDAConsultaExterna.Attachment()
                                {
                                    data = pdfBase64
                                },
                                format = new Domain.FHIR.CE.RDAConsultaExterna.Format()
                                {
                                    system = "urn:ietf:bcp:13",
                                    code = "application/pdf",
                                    display = "PDF"
                                }
                            }
                        },
                        context = new Domain.FHIR.CE.RDAConsultaExterna.Context()
                        {
                            encounter = new List<Domain.FHIR.CE.RDAConsultaExterna.Encounter>()
                            {
                                new Domain.FHIR.CE.RDAConsultaExterna.Encounter()
                                {
                                    reference = "#" + ENCOUNTER_ID
                                }
                            }
                        }
                    };
                    builder.AddResource(documentReference);
                    #endregion FIN RECURSO PDF

                    //Consttruir Bundle
                    var bundle = builder.Build();

                    //FINAL SERIALIZAR EL JSON COMPLETO
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };

                    string json = JsonSerializer.Serialize(bundle, options);

                    string ruta = @"C:\CXN\Reportes\RDA\RDA_NotasEnfermeria_" + Admision.ToString() + ".json";
                    File.WriteAllText(
                        ruta,
                        json,
                        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false) // UTF-8 sin BOM
                    );

                    //REPORTAR AL MINISTERIO
                    var res = await enviarRDA.SendBundleAsync(json, datosCita.Com_Identificador, "CExterna");

                    if (res.Est == "OK")
                    {
                        CXN_RDA saveRDAPaciente = new CXN_RDA
                        {
                            Admision = Admision,
                            RDAAmbulatorio = res.Resp,
                            Especialidad = Especialidad,
                            PersonaReportaRDAAmbulatorio = Contenedor.UsuarioLogueado,
                            FechaReporteRDAAmbulatorio = DateTime.Now
                        };

                        if (verificar != null)
                        {
                            //update
                            rFHIR.ActualizarEnvio_RDACExterna(saveRDAPaciente);
                        }
                        else
                        {
                            //insert
                            rFHIR.InsertarEnvio_RDAAmbulatorio(saveRDAPaciente);
                        }
                    }
                    else
                    {
                        MessageBox.Show(res.Resp, res.Est, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        string Text = res.Resp + "\r\r\r\r\r" + res.Est;

                        FileStream Querys = new FileStream("C:/Cxn/Reportes/Resultado_RDANotas_" + Admision.ToString() + ".json", FileMode.Append, FileAccess.Write);
                        StreamWriter Escriba = new StreamWriter(Querys);

                        Escriba.Write(Text);
                        Escriba.WriteLine();
                        Escriba.Flush();
                        Escriba.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
