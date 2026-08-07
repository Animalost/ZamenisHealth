using APIFhir.Controlador;
using APIFhir.Servicio;
using Domain.CXN;
using Domain.FHIR;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Windows.Forms;
using ZamenisHealth.Comunes;


namespace ZamenisHealth.FrontFHIR.RDAs
{
    public class RDAPaciente
    {
        private IFHIR rFHIR;
        private IPacientes rPaciente;
        private IAgendaC rAgendaC;
        private IBodegas rBodegas;
        private ILogin rLogin;
        private IAntecedentesGlobales repoAntGenGlobal;
        private ICompañia rCia;
        private IZonas rZonas;
        private EnviarRDAPaciente rEnvioRDA;

        private MensajesGeneral MG;
        private Condition condition;
        private List<Condition> conditionList;
        private MedicationStatement medicamentResource;
        private List<MedicationStatement> medicamentResourceLista;

        public RDAPaciente() 
        {
            rFHIR = new MFHIR();
            rPaciente = new MPacientes();
            rAgendaC = new MAgendaC();
            rBodegas = new MBodegas();
            rLogin = new MLogin();
            repoAntGenGlobal = new MAntecedentesGlobales();
            rCia = new MCompañia();
            rZonas = new MZonas();
            rEnvioRDA = new EndPoint_RDAPaciente();
        }

        public class AntPatologico
        {
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
        }
        public class AntecedentePaciente
        {
            public List<AntPatologico> Patologicos { get; set; }
            public List<AntPatologico> Farmacologicos { get; set; }
        }

        CXN_CIA DatoPrestador(int Cia)
        {
            return rCia.getPrestadorbyCode(Cia);
        }
        CXN_BODEGAS DatoProfesional(int Bodega)
        {
            return rBodegas.getDatosCode(Bodega);
        }
        CXN_LOGIN DatoLogin(string Usuario)
        {
            return rLogin.getUser(Usuario);
        }
        CXN_PACIENTES DatoPaciente(int Id)
        {
            return rPaciente.LlamarPacientebyId(Id);
        }
        List<CXN_CONDICIONES> AlergiasPaciente(int Paciente)
        {
            return rFHIR.ConsultarAlergias(Paciente);
        }     
        AntecedentePaciente AntecedentesPaciente(int Paciente)
        {
            try
            {
                List<CXN_ANTECEDENTESFAMILIARES> listaAntPatologicos = new List<CXN_ANTECEDENTESFAMILIARES>();
                List<CXN_ANTECEDENTESFAMILIARES> listaAntFarmacologicos = new List<CXN_ANTECEDENTESFAMILIARES>();
                List<CXN_ANTECEDENTESFAMILIARES> DatosAntecedentes = new List<CXN_ANTECEDENTESFAMILIARES>();
                DatosAntecedentes = repoAntGenGlobal.ObtenerAntecedentes(Paciente);
                if (DatosAntecedentes != null)
                {
                    listaAntPatologicos = DatosAntecedentes.Where(a => a.Parentesco == "PROPIO" && a.Tipo == "PATOLOGICO").ToList();
                    listaAntFarmacologicos = DatosAntecedentes.Where(a => a.Parentesco == "PROPIO" && a.Tipo == "FARMACOLOGICO").ToList();
                }

                List<AntPatologico> listaAntecedentesPatologicos = new List<AntPatologico>();
                List<AntPatologico> listaAntecedentesFarmacologicos = new List<AntPatologico>();

                if (listaAntPatologicos != null)
                {
                    foreach (var ant in listaAntPatologicos)
                    {
                        listaAntecedentesPatologicos.Add(new AntPatologico
                        {
                            Codigo = ant.CIECod,
                            Descripcion = ant.CieDesc
                        });
                    }
                }
                if (listaAntFarmacologicos != null)
                {
                    foreach (var ant in listaAntFarmacologicos)
                    {
                        listaAntecedentesFarmacologicos.Add(new AntPatologico
                        {
                            Codigo = ant.CIECod,
                            Descripcion = ant.CieDesc
                        });
                    }
                }

                AntecedentePaciente A = new AntecedentePaciente()
                {
                    Farmacologicos = listaAntecedentesFarmacologicos,
                    Patologicos = listaAntecedentesPatologicos
                };

                return A;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        List<CXN_ANTECEDENTESFAMILIARES> AntecedentesFamiliares(int Paciente)
        {
            return repoAntGenGlobal.ObtenerAntecedentes(Paciente);
        }
        List<Reference> CargarEntryDXAntecedentesPaciente(AntecedentePaciente A, string PacDoc)
        {
            try
            {
                List<Reference> entryTempDx = new List<Reference>();
                conditionList = new List<Condition>();

                if (A.Patologicos == null || A.Patologicos.Count == 0)
                {
                    /*condition = new Condition
                    {
                        resourceType = "Condition",
                        id = $"Condition-1",
                        meta = new Meta
                        {
                            profile = new List<string>
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/ConditionStatementRDA"
                            }
                        },                        
                    };

                    entryTempDx.Add(new Reference { reference = $"#Condition-1" });   */                 
                }
                else
                {
                    int contadorpatologico = 1;
                    condition = new Condition();

                    foreach (var i in A.Patologicos)
                    {
                        condition = new Condition
                        {
                            resourceType = "Condition",
                            id = $"Condition-{contadorpatologico}",
                            meta = new Meta
                            {
                                profile = new List<string>
                                    {
                                        "https://fhir.minsalud.gov.co/rda/StructureDefinition/ConditionStatementRDA"
                                    }
                            },
                            clinicalStatus = new CodeableConcept
                            {
                                coding = new List<Coding>
                                    {
                                        new Coding { system = "http://terminology.hl7.org/CodeSystem/condition-clinical", code = "active", display = "Active" }
                                    }
                            },
                            verificationStatus = new CodeableConcept
                            {
                                coding = new List<Coding>
                                    {
                                        new Coding { code = "unconfirmed", display = "Unconfirmed" }
                                    }
                            },
                            category = new List<CodeableConcept>
                                {
                                    new CodeableConcept
                                    {
                                        coding = new List<Coding>
                                        {
                                            new Coding { system = "http://terminology.hl7.org/CodeSystem/condition-category", code = "encounter-diagnosis", display = "Encounter Diagnosis" }
                                        }
                                    }
                                },
                            code = new CodeableConcept
                            {
                                text = i.Descripcion.ToUpper()
                            },
                            subject = new Reference
                            {
                                reference = $"#{PacDoc}"
                            }
                        };
                        conditionList.Add(condition);
                        entryTempDx.Add(new Reference { reference = $"#Condition-{contadorpatologico}" });
                        contadorpatologico++;
                    }
                }
                return entryTempDx;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        List<Reference> CargarEntryAlergiasPaciente(List<CXN_CONDICIONES> A, string PacDoc)
        {
            try
            {
                List<Reference> allergyReferences = new List<Reference>();
                if (A != null)
                {
                    int allergyCounter = 0;

                    foreach (var alergia in A)
                    {
                        var allergyResource = new AllergyIntolerance
                        {
                            resourceType = "AllergyIntolerance",
                            id = $"AllergyIntolerance-{allergyCounter}",
                            meta = new Meta
                            {
                                profile = new List<string>
                                      {
                                          "https://fhir.minsalud.gov.co/rda/StructureDefinition/AllergyIntoleranceStatementRDA"
                                      }
                            },
                            clinicalStatus = new CodeableConcept
                            {
                                coding = new List<Coding>
                                  {
                                      new Coding { code = "active", display = "Active" }
                                  }
                            },
                            verificationStatus = new CodeableConcept
                            {
                                coding = new List<Coding>
                                      {
                                          new Coding { code = "unconfirmed", display = "Unconfirmed" }
                                      }
                            },
                            code = new CodeableConcept
                            {
                                text = alergia.Detalle
                            },
                            patient = new Reference { reference = $"#{PacDoc}"}
                        };

                        //bundle.entry.Add(new Entry { resource = allergyResource });

                        // Guardar la referencia para la sección
                        allergyReferences.Add(new Reference { reference = $"#AllergyIntolerance-{allergyCounter}" });
                        allergyCounter++;
                    }

                    var allergySection = new Section
                    {
                        title = "Historial de alergias, intolerancias y reacciones adversas",
                        code = new CodeableConcept
                        {
                            coding = new List<Coding>
                                  {
                                      new Coding { system = "http://loinc.org", code = "48765-2", display = "Allergies and adverse reactions Document" }
                                  }
                        },
                        entry = allergyReferences
                    };
                }
                else
                {
                    allergyReferences.Add(new Reference
                    {
                        reference = "#AllergyIntolerance-0"
                    });
                }

                return allergyReferences;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        List<Reference> CargarEntryMedicamentos(AntecedentePaciente A, string PacDoc)
        {
            try
            {
                medicamentResourceLista = new List<MedicationStatement>();

                List<Reference> medicamentosReferences = new List<Reference>();
                if (A.Farmacologicos != null)
                {
                    int medicamentoCounter = 1;                    

                    foreach (var medicamento in A.Farmacologicos)
                    {
                        medicamentResource = new MedicationStatement
                        {
                            resourceType = "MedicationStatement",
                            id = $"MedicationStatement-{medicamentoCounter}",
                            meta = new Meta
                            {
                                profile = new List<string>
                                {
                                    "https://fhir.minsalud.gov.co/rda/StructureDefinition/MedicationStatementRDA"
                                }
                            },
                            status = "completed",
                            medicationCodeableConcept = new CodeableConcept
                            {
                                coding = new List<Coding>
                                {
                                    new Coding
                                    {
                                        system = "https://fhir.minsalud.gov.co/rda/CodeSystem/MipresINN", //CIE 10 
                                        code = medicamento.Codigo,
                                        display = medicamento.Descripcion.ToUpper()
                                    }
                                },
                                text = medicamento.Descripcion.ToUpper()
                            },
                            subject = new Reference
                            {
                                reference = $"#{PacDoc}"
                            }
                        };
                        medicamentResourceLista.Add(medicamentResource);
                        // Guardar la referencia para la sección
                        medicamentosReferences.Add(new Reference { reference = $"#MedicationStatement-{medicamentoCounter}" });
                        medicamentoCounter++;
                    }

                    var medicamentSection = new Section
                    {
                        title = "Historial de medicamentos",
                        code = new CodeableConcept
                        {
                            coding = new List<Coding>
                                  {
                                      new Coding { system = "http://loinc.org", code = "10160-0", display = "History of Medication use Narrative" }
                                  }
                        },
                        entry = medicamentosReferences
                    };
                }
                else
                {
                    medicamentResource = new MedicationStatement
                    {
                        resourceType = "MedicationStatement",
                        id = $"MedicationStatement-1",
                        meta = new Meta
                        {
                            profile = new List<string>
                                {
                                    "https://fhir.minsalud.gov.co/rda/StructureDefinition/MedicationStatementRDA"
                                }
                        },
                        status = "not-taken",
                        medicationCodeableConcept = new CodeableConcept
                        {                            
                            text = "Sin medicamentos recetados"
                        },
                        subject = new Reference
                        {
                            reference = $"#{PacDoc}"
                        }
                    };
                    medicamentResourceLista.Add(medicamentResource);

                    medicamentosReferences.Add(new Reference
                    {
                        reference = "#MedicationStatement-1"
                    });

                    var medicamentSection = new Section
                    {
                        title = "Historial de medicamentos",
                        code = new CodeableConcept
                        {
                            coding = new List<Coding>
                                  {
                                      new Coding { system = "http://loinc.org", code = "10160-0", display = "History of Medication use Narrative" }
                                  }
                        },
                        entry = medicamentosReferences
                    };
                }

                return medicamentosReferences;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        List<Reference> CargarEntryAntecedentesFamiliares(List<CXN_ANTECEDENTESFAMILIARES> A)
        {
            try
            {
                List<Reference> entrytemp = new List<Reference>();
                if (A != null && A.Count > 0)
                {
                    int antecedenteCounter = 1;

                    var filtro = A.Where(x => x.Parentesco != "PROPIO").ToList();

                    if (filtro == null || filtro.Count == 0)
                    {
                        return null;
                    }

                    var grupos = filtro.GroupBy(a => a.Parentesco).ToList();

                    foreach (var grupo in grupos)
                    {
                        // Cada grupo genera un solo FamilyMemberHistory
                        entrytemp.Add(new Reference
                        {
                            reference = $"#FamilyMemberHistory-{antecedenteCounter}"
                        });

                        antecedenteCounter++;
                    }

                    return entrytemp;
                }
                else
                {
                    return null;
                   /* entrytemp.Add(new Reference
                    {
                        reference = "#FamilyMemberHistory-1"
                    });*/
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
                    List<CXN_CONDICIONES> alergias = new List<CXN_CONDICIONES>();
                    List<CXN_ANTECEDENTESFAMILIARES> antecedentesFamiliares = new List<CXN_ANTECEDENTESFAMILIARES>();
                    AntecedentePaciente antecedentePaciente = new AntecedentePaciente();                    

                    //REGIONALIZAR
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo; //ISO3166-1

                    //Llamar datos para armar el Bundle
                    CXN_CIA DatosPrestador = DatoPrestador(datosCita.Hor_Pac_Cia);
                    CXN_BODEGAS Profesional = DatoProfesional(datosCita.Hor_Pac_Bod);
                    string TipoDocPac = rPaciente.getTipoDoc(datosCita.Pac_TipoId);
                    CXN_LOGIN Login = DatoLogin(Profesional.Bod_Usuario);
                    CXN_PACIENTES DatosPaciente = DatoPaciente(datosCita.Hor_Pac_Id);
                    antecedentePaciente = AntecedentesPaciente(datosCita.Hor_Pac_Id);
                    alergias = AlergiasPaciente(datosCita.Hor_Pac_Id);
                    antecedentesFamiliares = AntecedentesFamiliares(datosCita.Hor_Pac_Id);

                    //Verificar si ya esta radicada el RDA Paciente
                    if (rFHIR.VerificarEnvio(Admision, "Paciente") == true)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "RDA Paciente ya enviado",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                    else
                    {
                        #region CREAR BUNDLE
                        var bundle = new Bundle
                        {
                            resourceType = "Bundle",
                            language = "es-CO",
                            type = "document",
                            entry = new List<Entry>()
                        };
                        #endregion FIN CREAR BUNDLE

                        #region Horas de Ingreso y Salida del paciente a consulta
                        DateTime fecha = DateTime.Now;
                        DateTime llegada = new DateTime(datosCita.Hor_Pac_Fecha_Cita.Year,
                                                                        datosCita.Hor_Pac_Fecha_Cita.Month,
                                                                        datosCita.Hor_Pac_Fecha_Cita.Day,
                                                                        datosCita.Hor_Pac_Hora_Cita.Hour,
                                                                        datosCita.Hor_Pac_Hora_Cita.Minute,
                                                                        00);
                        DateTime salida = new DateTime(datosCita.Hor_Pac_Fecha_Cita.Year,
                                                       datosCita.Hor_Pac_Fecha_Cita.Month,
                                                       datosCita.Hor_Pac_Fecha_Cita.Day,
                                                       DateTime.Now.Hour,
                                                       DateTime.Now.Minute,
                                                       00).AddMinutes(-3);
                        #endregion                       

                        #region AGREGAR SECCIONES AL COMPOSITION
                        Composition comp2 = new Composition();
                        comp2.section = new List<Section>();
             
                        #region ANTECEDENTES PATOLOGICOS DEL PACIENTE - SECCION
                        if (antecedentePaciente.Patologicos.Count == 0)
                        {
                            comp2.section.Add(new Section
                            {
                                title = "Historial de diagnósticos de problemas de salud",
                                code = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding 
                                        { 
                                            system = "http://loinc.org", 
                                            code = "11450-4", 
                                            display = "Problem list - Reported" 
                                        }
                                    }
                                },
                                emptyReason = new CodeableConcept()
                                {
                                    coding = new List<Coding>()
                                    {
                                        new Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                            code = "nilknown",
                                            display = "Nil Known"
                                        }
                                    }
                                },
                                text = new Text()
                                {
                                    status = "generated",
                                    div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                                }
                            });
                        }
                        else
                        {
                            comp2.section.Add(new Section
                            {
                                title = "Historial de diagnósticos de problemas de salud",
                                code = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "11450-4", display = "Problem list - Reported" }
                                    }
                                },
                                entry = CargarEntryDXAntecedentesPaciente(antecedentePaciente, TipoDocPac + "-" + datosCita.Pac_IdNum)
                            });
                        }
                        #endregion
                        
                        #region ALERGIAS REPORTADAS POR EL PACIENTE - SECCION
                        if (alergias != null) 
                        {
                            comp2.section.Add(new Section
                            {
                                title = "Historial de alergias, intolerancias y reacciones adversas",
                                code = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "48765-2", display = "Allergies and adverse reactions Document" }
                                    }
                                },

                                entry = CargarEntryAlergiasPaciente(alergias, TipoDocPac + "-" + datosCita.Pac_IdNum)
                            });
                        }
                        else
                        {
                            comp2.section.Add(new Section
                            {
                                title = "Historial de alergias, intolerancias y reacciones adversas",
                                code = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "48765-2", display = "Allergies and adverse reactions Document" }
                                    }
                                },
                                emptyReason = new CodeableConcept()
                                {
                                    coding = new List<Coding>()
                                    {
                                        new Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                            code = "nilknown",
                                            display = "Nil Known"
                                        }
                                    }
                                },
                                text = new Text()
                                {
                                    status = "generated",
                                    div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                                }
                            });
                        }
                        #endregion

                        #region ANTECEDENTES MEDICAMENTOS DEL PACIENTE - SECCION
                        if (antecedentePaciente.Farmacologicos.Count > 0)
                        {
                            comp2.section.Add(new Section
                            {
                                title = "Historial de medicamentos",
                                code = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "10160-0", display = "History of Medication use Narrative" }
                                    }
                                },
                                entry = CargarEntryMedicamentos(antecedentePaciente, TipoDocPac + "-" + datosCita.Pac_IdNum)
                            });
                        }
                        else
                        {
                            comp2.section.Add(new Section
                            {
                                title = "Historial de medicamentos",
                                code = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "10160-0", display = "History of Medication use Narrative" }
                                    }
                                },
                                emptyReason = new CodeableConcept()
                                {
                                    coding = new List<Coding>()
                                    {
                                        new Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                            code = "nilknown",
                                            display = "Nil Known"
                                        }
                                    }
                                },
                                text = new Text()
                                {
                                    status = "generated",
                                    div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                                }
                            });
                        }
                        #endregion

                        #region ANTECEDENTES FAMILIARES DEL PACIENTE - SECCION
                        if (antecedentesFamiliares != null && antecedentesFamiliares.Count > 0)
                        {
                            var filtro = antecedentesFamiliares.Where(x => x.Parentesco != "PROPIO").ToList();

                            if (filtro == null || filtro.Count == 0)
                            {
                                comp2.section.Add(new Section
                                {
                                    title = "Historial de antecedentes familiares",
                                    code = new CodeableConcept
                                    {
                                        coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "10157-6", display = "History of family member diseases Narrative" }
                                    }
                                    },
                                    emptyReason = new CodeableConcept()
                                    {
                                        coding = new List<Coding>()
                                    {
                                        new Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                            code = "nilknown",
                                            display = "Nil Known"
                                        }
                                    }
                                    },
                                    text = new Text()
                                    {
                                        status = "generated",
                                        div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                                    }
                                });
                            }
                            else
                            {
                                comp2.section.Add(new Section
                                {
                                    title = "Historial de antecedentes familiares",
                                    code = new CodeableConcept
                                    {
                                        coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "10157-6", display = "History of family member diseases Narrative" }
                                    }
                                    },
                                    entry = CargarEntryAntecedentesFamiliares(antecedentesFamiliares)
                                });
                            }                           
                        }
                        else
                        {
                            comp2.section.Add(new Section
                            {
                                title = "Historial de antecedentes familiares",
                                code = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://loinc.org", code = "10157-6", display = "History of family member diseases Narrative" }
                                    }
                                },
                                emptyReason = new CodeableConcept()
                                {
                                    coding = new List<Coding>()
                                    {
                                        new Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/list-empty-reason",
                                            code = "nilknown",
                                            display = "Nil Known"
                                        }
                                    }
                                },
                                text = new Text()
                                {
                                    status = "generated",
                                    div = "<div xmlns = 'http://www.w3.org/1999/xhtml' > No existen elementos conocidos para esta lista y / o el paciente no declara información </div>"
                                }
                            });
                        }
                        #endregion

                        #endregion

                        #region CREAR EL COMPOSITION
                        var comp = new Composition
                        {
                            resourceType = "Composition",
                            meta = new Meta
                            {
                                profile = new List<string>
                                {
                                    "https://fhir.minsalud.gov.co/rda/StructureDefinition/CompositionPatientStatementRDA"
                                }
                            },
                            status = "final",
                            type = new CodeableConcept
                            {
                                coding = new List<Coding>
                                {
                                    new Coding { system = "http://loinc.org", code = "102089-0", display = "FHIR resource patient medical record" }
                                }
                            },
                            subject = new Reference { reference = "#" + TipoDocPac + "-" + datosCita.Pac_IdNum }, // tipo e Identifiacion paciente 
                            date = new DateTimeOffset(fecha, TimeSpan.FromHours(-5)),//DateTime.Parse("2025-12-10T09:59:00-05:00"),
                            //date = new DateTimeOffset(llegada),//DateTime.Parse("2025-12-10T09:59:00-05:00"),                           
                            author = new List<Reference>
                            {
                                new Reference { reference = "#" + DatosPrestador.Com_Cod_Prestador_2 }// codigo habilitacion prestador sin sucursal
                            },
                            title = "Resumen Digital de Atención en Salud - RDA de antecedentes manifestados por el paciente",
                            confidentiality = "N",
                            attester = new List<Attester>
                            {
                                new Attester
                                {
                                    mode = "legal",
                                    party = new Reference { reference = "#CC-" + datosCita.IdentificacionProfesional } // tipo e identificacion profesional que registra el RDA
                                }
                            },
                            custodian = new Reference { reference = "#" + DatosPrestador.Com_Cod_Prestador_2 }, // codigo habilitacion prestador sin sucursal
                            @event = new List<Event>
                            {
                                new Event
                                {
                                    code = new List<CodeableConcept>
                                    {
                                        new CodeableConcept
                                        {
                                            coding = new List<Coding>
                                            {
                                                new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianTechModality", code = datosCita.Hor_Pac_Modalidad, display = rFHIR.Modalidad(datosCita.Hor_Pac_Modalidad) } //Modalidad de atencion
                                            }
                                        },
                                        new CodeableConcept
                                        {
                                            coding = new List<Coding>
                                            {
                                                new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/GrupoServicios", code = datosCita.Hor_GrupoServicios, display = rFHIR.GrupoServicios(datosCita.Hor_GrupoServicios) } //Grupo de servicios
                                            }
                                        }
                                    },
                                    period = new Period
                                    {
                                        start = new DateTimeOffset(llegada),  //hora de inicio de la consulta
                                        end = new DateTimeOffset(DateTime.Now.AddMinutes(-3)) // hora de salida de la consulta
                                        //end = new DateTimeOffset(salida) // hora de salida de la consulta
                                    }
                                 }
                            },
                            section = comp2.section //Agregamos los section Temp del composition
                        };
                        bundle.entry.Add(new Entry { resource = comp }); //Agregamos los demas section 
                        #endregion

                        #region AGREGAMOS LOS RECURSOS DE CADA SECCION AL BUNDLE
                        
                        #region RECURSO PATIENT 
                        var patient = new Patient
                        {
                            resourceType = "Patient",
                            id = TipoDocPac + "-" + datosCita.Pac_IdNum, //Tipo y Numero de Identificacion Paciente
                            meta = new Meta
                            {
                                profile = new List<string>
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/PatientRDA"
                            }
                            },
                            active = true,
                            gender = rFHIR.Gender(datosCita.Pac_Sexo), //Genero del paciente
                            _gender = new ExtensionContainer
                            {
                                extension = new List<Extension>
                            {
                                new Extension
                                {
                                    url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionBiologicalGender",
                                    valueCoding = new Coding
                                    {
                                        system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianGenderGroup",
                                        code = rFHIR.Gender(datosCita.Pac_Sexo) == "M" ? "01" : "02",
                                        display = rFHIR.Gender(datosCita.Pac_Sexo) == "M" ? "Hombre" : "Mujer",
                                    }
                                }
                            }
                            },
                            birthDate = new DateTime(datosCita.Pac_FechaNto.Year, datosCita.Pac_FechaNto.Month, datosCita.Pac_FechaNto.Day), // Fecha de Nacimiento
                            _birthDate = new ExtensionContainer
                            {
                                extension = new List<Extension>
                            {
                                new Extension
                                {
                                    url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionBirthTime",
                                    valueTime = "14:30:00"
                                }
                            }
                            },
                            deceasedBoolean = false,
                            extension = new List<Extension>
                        {
                            new Extension
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientNationality",
                                valueCoding = new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ISO31661", code = datosCita.Pac_PaisOrigen, display = textInfo.ToTitleCase(rPaciente.getNamePais(datosCita.Pac_PaisOrigen).ToLower()) } // pais origen paciente
                            },
                            new Extension
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientEthnicity",
                                valueCoding = new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianEthnicGroup", code = string.IsNullOrEmpty(DatosPaciente.Etnia) ? "99" : rFHIR.Etnia(DatosPaciente.Etnia), display = string.IsNullOrEmpty(DatosPaciente.Etnia) ? "Ninguna de las anteriores" :  DatosPaciente.Etnia } //Etnia paciente
                            },
                            new Extension
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientDisability",
                                valueCoding = new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianDisabilityClassification", code = string.IsNullOrEmpty(DatosPaciente.Discapacidad) ? "08" : rFHIR.Discapacidad(DatosPaciente.Discapacidad), display = string.IsNullOrEmpty(DatosPaciente.Discapacidad) ? "Sin discapacidad" : DatosPaciente.Discapacidad} //discapacidad paciente 
                            },
                            new Extension
                            {
                                url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionPatientGenderIdentity",
                                valueCoding = new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianGenderIdentity", code = rFHIR.Gender(datosCita.Pac_Sexo) == "M" ? "01" : "02",  display = rFHIR.Gender(datosCita.Pac_Sexo) == "M" ? "Masculino" : "Femenino" }
                            }
                        },
                            name = new List<HumanName>
                        {
                            new HumanName
                            {
                                use = "official",
                                family = datosCita.PrimerApellido + " " + datosCita.SegundoApellido, //Apellidos del paciente separados por espacio
                                given = new List<string> { datosCita.PrimerNombre, datosCita.PrimerApellido }, //Nombre y apellido del paciente separados por espacio
                                _family = new ExtensionContainer
                                {
                                    extension = new List<Extension>
                                    {
                                        new Extension { url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionFathersFamilyName", valueString = datosCita.PrimerApellido },
                                        new Extension { url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionMothersFamilyName", valueString = datosCita.SegundoApellido }
                                    }
                                }
                            }
                        },
                            address = new List<Address>
                        {
                            new Address
                            {
                                id = "HomeAddress-0",
                                use = "home",
                                type = "physical",
                                city = string.IsNullOrEmpty(DatosPaciente.Pac_Dep_Cod) || string.IsNullOrEmpty(DatosPaciente.Pac_Mun_Cod) ? "Bogotá D.C." : rZonas.MunicipioNombre(DatosPaciente.Pac_Mun_Cod, DatosPaciente.Pac_Dep_Cod), //Nombre ciudad o municipio del paciente residencia
                                _city = new ExtensionContainer
                                {
                                    extension = new List<Extension>
                                    {
                                        new Extension
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionDivipolaMunicipality",
                                            valueCoding = new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/DIVIPOLA", code = string.IsNullOrEmpty(DatosPaciente.Pac_Dep_Cod) || string.IsNullOrEmpty(DatosPaciente.Pac_Mun_Cod) ? "11001" : DatosPaciente.Pac_Dep_Cod + DatosPaciente.Pac_Mun_Cod } //Codigo ciudad o municipio
                                        }
                                    }
                                },
                                country = "Colombia",
                                _country = new ExtensionContainer
                                {
                                    extension = new List<Extension>
                                    {
                                        new Extension
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionCountryCode",
                                            valueCoding = new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ISO31661", code = "170" }
                                        }
                                    }
                                },
                                extension = new List<Extension>
                                {
                                    new Extension
                                    {
                                        url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionResidenceZone",
                                        valueCoding = new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianResidenceZone", code = string.IsNullOrEmpty(DatosPaciente.Pac_Zona) ? "01" : DatosPaciente.Pac_Zona == "U" ? "01" : "02", display = string.IsNullOrEmpty(DatosPaciente.Pac_Zona) ? "Urbana" : DatosPaciente.Pac_Zona == "U" ? "Urbana" : "Rural" } //Zona Urbana
                                    }
                                }
                            }
                        },
                            identifier = new List<Identifier>
                        {
                            new Identifier
                            {
                                id = "NationalPersonIdentifier-0",
                                use = "official",
                                system = "https://fhir.minsalud.gov.co/rda/NamingSystem/RNEC",
                                value = datosCita.Pac_IdNum, //Identificacion del paciente
                                type = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://terminology.hl7.org/CodeSystem/v2-0203", code = "PN", display = "Person number" },
                                        new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianPersonIdentifier", code = TipoDocPac, display = datosCita.Com_Tipo_Doc } //Datos de identificacion del paciente
                                    }
                                }
                            }
                        }
                        };
                        bundle.entry.Add(new Entry { resource = patient });
                        #endregion

                        #region RECURSO ORGANIZATION
                        var org = new Organization
                        {
                            resourceType = "Organization",
                            id = DatosPrestador.Com_Cod_Prestador_2, //Codigo habilitacion prestador sin sucursal
                            meta = new Meta
                            {
                                profile = new List<string>
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/CareDeliveryOrganizationRDA"
                            }
                            },
                            identifier = new List<Identifier>
                        {
                            new Identifier
                            {
                                id = "TaxIdentifier-0",
                                use = "official",
                                value = DatosPrestador.Com_Identificacion + "-" + DatosPrestador.Com_DVerifica, // nit prestador con dv
                                type = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://terminology.hl7.org/CodeSystem/v2-0203", code = "TAX", display = "Tax ID number" },
                                        new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianOrganizationIdentifiers", code = "NIT", display = "Número de Identificación Tributaria" } //Nit para juridicos
                                    }
                                }
                            },
                            new Identifier
                            {
                                id = "HealthcareProviderIdentifier-0",
                                use = "official",
                                system = "https://fhir.minsalud.gov.co/rda/NamingSystem/REPS",
                                value = DatosPrestador.Com_Cod_Prestador_2, // Codigo habilitacion
                                type = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://terminology.hl7.org/CodeSystem/v2-0203", code = "PRN", display = "Provider number" },
                                        new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianOrganizationIdentifiers", code = "CodigoPrestador", display = "Código de habilitación de prestador de servicios de salud" }
                                    }
                                }
                            }
                        }
                        };
                        bundle.entry.Add(new Entry { resource = org });
                        #endregion

                        #region RECURSO ALLERGY INTOLERANCE
                        if (alergias != null)
                        {
                            int counter = 0;
                            foreach (var alergia in alergias)
                            {
                                var allergyResource = new AllergyIntolerance
                                {
                                    resourceType = "AllergyIntolerance",
                                    id = $"AllergyIntolerance-{counter++}",
                                    meta = new Meta
                                    {
                                        profile = new List<string>
                                    {
                                        "https://fhir.minsalud.gov.co/rda/StructureDefinition/AllergyIntoleranceStatementRDA"
                                    }
                                    },
                                    clinicalStatus = new CodeableConcept
                                    {
                                        coding = new List<Coding>
                                        {
                                            new Coding { code = "active", display = "Active" }
                                        }
                                    },
                                    verificationStatus = new CodeableConcept
                                    {
                                        coding = new List<Coding>
                                        {
                                            new Coding { code = "unconfirmed", display = "Unconfirmed" }
                                        }
                                    },
                                    code = new CodeableConcept
                                    {
                                        coding = new List<Coding>
                                        {
                                            new Coding
                                            {
                                                system = "https://fhir.minsalud.gov.co/rda/CodeSystem/TipoAlergia",
                                                code = alergia.CodigoFHIR,
                                                display =  rFHIR.GrupoAlergias(alergia.CodigoFHIR)
                                            }
                                        },
                                        text = alergia.Detalle.ToUpper()
                                    },
                                    patient = new Reference { reference = "#" + TipoDocPac + "-" + datosCita.Pac_IdNum }
                                };

                                bundle.entry.Add(new Entry { resource = allergyResource });
                            }
                        }
                        
                        
                        #endregion

                        #region CONDITION        
                        if (antecedentePaciente.Patologicos.Count > 0)
                        {
                            foreach (var i in conditionList)
                            {
                                bundle.entry.Add(new Entry { resource = i });
                            }                            
                        }                                                                            
                        #endregion

                        #region MEDICATION
                        if (medicamentResourceLista != null)
                        {
                            foreach (var i in medicamentResourceLista)
                            {
                                bundle.entry.Add(new Entry { resource = i });
                            }
                        }                        
                        #endregion

                        #region RECURSO PRACTITIONER
                        var practitioner = new Practitioner
                        {
                            resourceType = "Practitioner",
                            id = "CC-" + datosCita.IdentificacionProfesional, //Tipo e identificacion profesional que registra el RDA
                            meta = new Meta
                            {
                                profile = new List<string>
                            {
                                "https://fhir.minsalud.gov.co/rda/StructureDefinition/PractitionerRDA"
                            }
                            },
                            identifier = new List<Identifier>
                        {
                            new Identifier
                            {
                                id = "NationalPersonIdentifier-0",
                                use = "official",
                                value = datosCita.IdentificacionProfesional, //Identificacion profesional que registra el RDA
                                type = new CodeableConcept
                                {
                                    coding = new List<Coding>
                                    {
                                        new Coding { system = "http://terminology.hl7.org/CodeSystem/v2-0203", code = "PN", display = "Person number" },
                                        new Coding { system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianPersonIdentifier", code = "CC", display = "Cédula ciudadanía" } //Tipo identificaacion profesional que registra el RDA
                                    }
                                }
                            }
                        },
                            name = new List<HumanName>
                        {
                            new HumanName
                            {
                                use = "official",
                                family = "Apellidos",
                                given = new List<string> {  "Primernombre", "Segundonombre" } //Pendiente de validar
                            }
                        }
                        };
                        bundle.entry.Add(new Entry { resource = practitioner });
                        #endregion

                        #region RECURSO FAMILY MEMBER HISTORY
                        if (antecedentesFamiliares != null && antecedentesFamiliares.Count > 0)
                        {
                            int counter = 1;
                            var filtro = antecedentesFamiliares.Where(a => a.Parentesco != "PROPIO").ToList();
                            foreach (var grupo in filtro.GroupBy(a => a.Parentesco))
                            {
                                var fmh = new FamilyMemberHistory
                                {
                                    resourceType = "FamilyMemberHistory",
                                    id = $"FamilyMemberHistory-{counter++}",
                                    meta = new Meta
                                    {
                                        profile = new List<string>
                                    {
                                        "https://fhir.minsalud.gov.co/rda/StructureDefinition/FamilyMemberHistoryRDA"
                                    }
                                    },
                                    status = "completed",
                                    patient = new Reference { reference = "#" + TipoDocPac + "-" + datosCita.Pac_IdNum },
                                    relationship = new CodeableConcept
                                    {
                                        coding = new List<Coding>
                                    {
                                        new Coding
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ParentescoAntecedente",
                                            code = grupo.Key == "Padres" ? "01" : grupo.Key == "Hermanos" ? "02" : grupo.Key == "Tíos" ? "03" : "04", // 01=Padres, 02=Hermanos, 03=Tíos, 04=Abuelos
                                            display = grupo.Key
                                        }
                                    }
                                    },
                                    condition = grupo.Select(a => new ConditionFMH
                                    {
                                        code = new CodeableConcept
                                        {
                                            coding = new List<Coding>
                                        {
                                            new Coding
                                            {
                                                system = "http://hl7.org/fhir/sid/icd-10",
                                                code = a.CIECod,
                                                display = a.CieDesc
                                            }
                                        }
                                        }
                                    }).ToList()
                                };

                                bundle.entry.Add(new Entry { resource = fmh });
                            }
                        }
                        
                        #endregion

                        #endregion

                        #region SERIALIZAR JSON Y ACTIVAR POLIMORFISMO
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                            Converters = { new JsonStringEnumConverter() }
                        };

                        // Activar polimorfismo
                        options.TypeInfoResolver = new DefaultJsonTypeInfoResolver
                        {
                            Modifiers =
                        {
                            ti =>
                            {
                                if (ti.Type == typeof(Resource))
                                {
                                    ti.PolymorphismOptions = new JsonPolymorphismOptions
                                    {
                                        TypeDiscriminatorPropertyName = "resourceType",
                                        IgnoreUnrecognizedTypeDiscriminators = true,
                                        DerivedTypes =
                                        {
                                            new JsonDerivedType(typeof(Composition), "Composition"),
                                            new JsonDerivedType(typeof(Patient), "Patient"),
                                            new JsonDerivedType(typeof(Organization), "Organization"),
                                            new JsonDerivedType(typeof(Practitioner), "Practitioner"),
                                            new JsonDerivedType(typeof(Condition), "Condition"),
                                            new JsonDerivedType(typeof(AllergyIntolerance), "AllergyIntolerance"),
                                            new JsonDerivedType(typeof(FamilyMemberHistory), "FamilyMemberHistory"),
                                            new JsonDerivedType(typeof(MedicationStatement), "MedicationStatement")
                                        }
                                    };
                                }
                            }
                        }
                        };

                        string json = JsonSerializer.Serialize(bundle, options);
                        #endregion

                        #region RADICAR RDA AL MINISTERIO DE SALUD

                        string rutaArchivo = @"C:\CXN\Reportes\RDA\RDA_Paciente_" + datosCita.Pac_IdNum + ".json";
                        File.WriteAllText(rutaArchivo, json);

                        var res = await rEnvioRDA.SendBundleAsync(json, datosCita.Com_Identificador, "Paciente");

                        if (res.Est == "OK")
                        {
                            CXN_RDA saveRDAPaciente = new CXN_RDA
                            {
                                Admision = Admision,
                                RDAPaciente = res.Resp,
                                Especialidad = "Medicina General",
                                PersonaReportaRDAPaciente = Contenedor.UsuarioLogueado,
                                FechaReporteRDAPaciente = DateTime.Now
                            };

                            if (rFHIR.VerificarEnvio(Admision, "Paciente") == true)
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
                                Detalle = res.Resp,
                                Usuario = Contenedor.UsuarioLogueado,
                                Fecha = DateTime.Now
                            };

                            rFHIR.InsertarLOG_RDA(log);

                            string Text = res.Resp + "\r\r\r\r\r" + res.Est;

                            FileStream Querys = new FileStream("C:/Cxn/Reportes/Resultado_RDAPaciente_" + Admision.ToString() + ".json", FileMode.Append, FileAccess.Write);
                            StreamWriter Escriba = new StreamWriter(Querys);

                            Escriba.Write(Text);
                            Escriba.WriteLine();
                            Escriba.Flush();
                            Escriba.Close();
                        }

                        #endregion
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
