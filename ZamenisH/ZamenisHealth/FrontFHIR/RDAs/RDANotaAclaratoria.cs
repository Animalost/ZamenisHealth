using Domain.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZamenisHealth.FrontFHIR.RDAs
{
    public class RDANotaAclaratoria
    {
        private IFHIR rFHIR;
        private IPacientes rPaciente;
        private ILogin rLogin;

        public RDANotaAclaratoria()
        {
            rFHIR = new MFHIR();
            rPaciente = new MPacientes();
            rLogin = new MLogin();
        }

        public async Task EnviarRDA(int admision)
        {
            try
            {
                CXN_HCMG getData = rFHIR.GetNAMG(admision);
                if (getData != null)
                {
                    CXN_LOGIN DatLogin = rLogin.getDatosCode(getData.HC_Prof);
                    string TDOC = rPaciente.getTipoDoc(getData.HC_Ocupacion);

                    string DocPatient = $"#{TDOC}-{getData.HC_Pac.Trim()}";
                    string DocMed = $"#{TDOC}-{DatLogin.Log_Identificacion.Trim()}";

                    #region EXTRAER ID
                    string patientId = "";
                    string compositionId = "";
                    string encounterId = "";

                    using (JsonDocument doc = JsonDocument.Parse(getData.HC_DescHer))
                    {
                        var root = doc.RootElement;

                        if (root.TryGetProperty("entry", out JsonElement entries))
                        {
                            foreach (var entry in entries.EnumerateArray())
                            {
                                if (!entry.TryGetProperty("resource", out JsonElement resource))
                                    continue;

                                if (!resource.TryGetProperty("resourceType", out JsonElement typeProp))
                                    continue;

                                string resourceType = typeProp.GetString();

                                if (!resource.TryGetProperty("id", out JsonElement idProp))
                                    continue;

                                string id = idProp.GetString();

                                switch (resourceType)
                                {
                                    case "Patient":
                                        patientId = id;
                                        break;

                                    case "Composition":
                                        compositionId = id;
                                        break;

                                    case "Encounter":
                                        encounterId = id;
                                        break;
                                }
                            }
                        }
                    }
                    #endregion FIN EXTRAER ID

                    var builder = new Domain.FHIR.RDANotaAclaratoria.RdaBundleBuilder();

                    var observation = new Domain.FHIR.RDANotaAclaratoria.Observation
                    {
                        resourceType = "Observation",
                        id = "Observation-1",
                        meta = new Domain.FHIR.RDANotaAclaratoria.Meta()
                        {
                            profile = new List<string>()
                           {
                               "https://fhir.minsalud.gov.co/rda/StructureDefinition/ObservationClarificationNoteRDA"
                           }
                        },
                        status = "final",
                        code = new Domain.FHIR.RDANotaAclaratoria.Code()
                        {
                            coding = new List<Domain.FHIR.RDANotaAclaratoria.Coding>()
                           {
                               new Domain.FHIR.RDANotaAclaratoria.Coding()
                               {
                                   system = "http://snomed.info/sct",
                                   code =  "445664008",
                                   display =  "informe enmendado"
                               }
                           },
                            text = "Nota aclaratoria"
                        },
                        subject = new Domain.FHIR.RDANotaAclaratoria.Subject()
                        {
                            reference = $"Patient/{patientId}"
                        },
                        focus = new List<Domain.FHIR.RDANotaAclaratoria.Focus>()
                       {
                           new Domain.FHIR.RDANotaAclaratoria.Focus()
                           {
                               id = "CompositionRDA",
                               reference = $"Composition/{compositionId}"
                           }
                       },
                        encounter = new Domain.FHIR.RDANotaAclaratoria.Encounter()
                        {
                            reference = $"Encounter/{encounterId}"
                        },
                        effectiveDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        performer = new List<Domain.FHIR.RDANotaAclaratoria.Performer>()
                       {
                           new Domain.FHIR.RDANotaAclaratoria.Performer()
                           {
                               reference = DocMed
                           }
                       },
                        valueString = getData.HC_Nota_Acl.Trim()
                    };
                    var request = new Domain.FHIR.RDANotaAclaratoria.Request
                    {
                        method = "POST",
                        url = "Observation"
                    };
                    builder.AddResource(observation, request);

                    var practitioner = new Domain.FHIR.RDANotaAclaratoria.Practitioner
                    {
                        resourceType = "Practitioner",
                        id = $"CC-{DatLogin.Log_Identificacion.Trim()}",
                        meta = new Domain.FHIR.RDANotaAclaratoria.Meta()
                        {
                            profile = new List<string>()
                            {
                               "https://fhir.minsalud.gov.co/rda/StructureDefinition/PractitionerRDA"
                            }
                        },
                        identifier = new List<Domain.FHIR.RDANotaAclaratoria.Identifier>()
                        {
                            new Domain.FHIR.RDANotaAclaratoria.Identifier()
                            {
                                id = "NationalPersonIdentifier-0",
                                use = "official",
                                type = new Domain.FHIR.RDANotaAclaratoria.Type()
                                {
                                    coding = new List<Domain.FHIR.RDANotaAclaratoria.Coding>()
                                    {
                                        new Domain.FHIR.RDANotaAclaratoria.Coding()
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/v2-0203",
                                            code = "PN",
                                            display =  "Person number"
                                        },
                                        new Domain.FHIR.RDANotaAclaratoria.Coding()
                                        {
                                            system = "https://fhir.minsalud.gov.co/rda/CodeSystem/ColombianPersonIdentifier",
                                            code = "CC",
                                            display =  "Cédula ciudadanía"
                                        }
                                    }
                                },
                                value = DatLogin.Log_Identificacion.Trim()
                            }
                        },
                        active = "true",
                        name = new List<Domain.FHIR.RDANotaAclaratoria.Name>()
                        {
                            new Domain.FHIR.RDANotaAclaratoria.Name()
                            {
                                use = "official",
                                family = "APELLIDOS PROFESIONAL DE LA SALUD",
                                _family = new Domain.FHIR.RDANotaAclaratoria.Family()
                                {
                                    extension = new List<Domain.FHIR.RDANotaAclaratoria.Extension>()
                                    {
                                        new Domain.FHIR.RDANotaAclaratoria.Extension()
                                        {
                                            url = "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionFathersFamilyName",
                                            valueString = DatLogin.Log_PrimerA.Trim()
                                        },
                                        new Domain.FHIR.RDANotaAclaratoria.Extension()
                                        {
                                            url =  "https://fhir.minsalud.gov.co/rda/StructureDefinition/ExtensionMothersFamilyName",
                                            valueString = DatLogin.Log_SegundoA.Trim()
                                        }
                                    }
                                },
                                given = new List<string>()
                                {
                                     DatLogin.Log_PrimerN.Trim(),
                                     DatLogin.Log_SegundoN.Trim()
                                }
                            }
                        }
                    };
                    var request2 = new Domain.FHIR.RDANotaAclaratoria.Request
                    {
                        method = "GET",
                        url = DocMed
                    };
                    builder.AddResource(practitioner, request2);

                    var bundle = builder.Build();

                    //FINAL SERIALIZAR EL JSON COMPLETO
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };

                    string json = JsonSerializer.Serialize(bundle, options);

                    string ruta = $@"C:\CXN\Reportes\RDA\RDA_NotaAclaratoriaMG_{ admision.ToString() }.json";
                    File.WriteAllText(
                        ruta,
                        json,
                        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false) // UTF-8 sin BOM
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
