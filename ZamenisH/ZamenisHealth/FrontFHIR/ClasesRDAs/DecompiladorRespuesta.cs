using APIFhir.VisorConsultas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZamenisHealth.FrontFHIR.VisorZamenis
{
    public class DecompiladorRespuesta
    {
        private IConsultaPaciente rConsultaPaciente = new ConsultaPaciente();

        // Datos Basicos Paciente
        public string GetString(JsonElement element, string property)
        {
            return element.TryGetProperty(property, out var value)
                ? value.GetString()
                : null;
        }
        //Lista Rda composition
        DataTable CrearTablaBundle()
        {
            var dt = new DataTable();
            dt.Columns.Add("POS", typeof(int));
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Tipo de Recurso", typeof(string));
            dt.Columns.Add("Identificador", typeof(string));
            dt.Columns.Add("Fecha Encuentro", typeof(DateTime));
            return dt;
        }
        DataTable CrearTablaBundleEncuentrosClinicos()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("POS");
            dt.Columns.Add("Fecha Atencion");
            dt.Columns.Add("Atencion");
            dt.Columns.Add("ResourceId");
            dt.Columns.Add("ResourceFecha");
            dt.Columns.Add("Encounter");
            dt.Columns.Add("Medico");
            dt.Columns.Add("IPS");
            dt.Columns.Add("Aseguradora");
            dt.Columns.Add("Fecha");

            return dt;
        }
        public DataTable ExtraerDatosBundle(string json)
        {
            var dt = CrearTablaBundle();

            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // 🔹 lastUpdated del Bundle
            DateTime? bundleLastUpdated = null;
            if (root.TryGetProperty("meta", out var meta) &&
                meta.TryGetProperty("lastUpdated", out var blu))
            {
                if (DateTime.TryParse(blu.GetString(), out var dtBlu))
                    bundleLastUpdated = dtBlu;
            }

            // 🔹 entry[]
            if (!root.TryGetProperty("entry", out var entries))
                return dt;

            int Contador = 0;
            string titleComposition = "";

            foreach (var entry in entries.EnumerateArray())
            {
                if (!entry.TryGetProperty("resource", out var resource))
                    continue;

                string resourceType = resource.TryGetProperty("resourceType", out var rt)
                    ? rt.GetString()
                    : null;

                if (resourceType == "Composition")
                {
                    if (resource.TryGetProperty("title", out var t))
                    {
                        titleComposition = t.GetString() ?? "";
                    }
                }

                string resourceId = resource.TryGetProperty("id", out var id)
                    ? id.GetString()
                    : null;

                DateTime? resourceLastUpdated = null;
                if (resource.TryGetProperty("meta", out var rmeta) &&
                    rmeta.TryGetProperty("lastUpdated", out var rlu))
                    {
                        if (DateTime.TryParse(rlu.GetString(), out var dtRlu))
                            resourceLastUpdated = dtRlu;
                    }

                dt.Rows.Add(
                    Contador,
                    Convert.ToDateTime(bundleLastUpdated).ToString("yyyy-MM-dd"),
                    titleComposition,
                    resourceId,
                    Convert.ToDateTime(resourceLastUpdated).ToString("yyyy-MM-dd")
                );

                Contador++;
            }

            return dt;
        }
        public async Task<DataTable> ExtraerDatosBundleEncuentrosClinicos(string json)
        {
            var dt = CrearTablaBundleEncuentrosClinicos();

            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // 🔹 lastUpdated del Bundle
            DateTime? bundleLastUpdated = null;
            if (root.TryGetProperty("meta", out var meta) &&
                meta.TryGetProperty("lastUpdated", out var blu) &&
                DateTime.TryParse(blu.GetString(), out var dtBlu))
            {
                bundleLastUpdated = dtBlu;
            }

            // 🔹 entry[]
            if (!root.TryGetProperty("entry", out var entries))
                return dt;

            int contador = 1;

            foreach (var entry in entries.EnumerateArray())
            {
                if (!entry.TryGetProperty("resource", out var resource))
                    continue;

                // 🔥 SOLO COMPOSITION
                string resourceType = DecompiladorRespuesta.GetStringSafe(resource, "resourceType");
                if (resourceType != "Composition")
                    continue;

                // =========================
                // VARIABLES
                // =========================
                string resourceId = DecompiladorRespuesta.GetStringSafe(resource, "id");
                string title = DecompiladorRespuesta.GetStringSafe(resource, "title");
                string fecha = DecompiladorRespuesta.GetStringSafe(resource, "date");

                string encounterRef = "";
                string practitionerRef = "";
                string organizationRef = "";
                string organizationSectionRef = "";

                DateTime? resourceLastUpdated = null;

                // =========================
                // LAST UPDATED RESOURCE
                // =========================
                if (resource.TryGetProperty("meta", out var rmeta) &&
                    rmeta.TryGetProperty("lastUpdated", out var rlu) &&
                    DateTime.TryParse(rlu.GetString(), out var dtRlu))
                {
                    resourceLastUpdated = dtRlu;
                }

                // =========================
                // ENCOUNTER
                // =========================
                if (resource.TryGetProperty("encounter", out var encounter))
                {
                    encounterRef = DecompiladorRespuesta.GetStringSafe(encounter, "reference");
                }

                // =========================
                // CUSTODIAN (Organization principal)
                // =========================
                if (resource.TryGetProperty("custodian", out var custodian))
                {
                    organizationRef = DecompiladorRespuesta.GetStringSafe(custodian, "reference");
                }

                // =========================
                // PRACTITIONER (attester)
                // =========================
                if (resource.TryGetProperty("attester", out var attesters))
                {
                    foreach (var at in attesters.EnumerateArray())
                    {
                        if (at.TryGetProperty("party", out var party))
                        {
                            practitionerRef = DecompiladorRespuesta.GetStringSafe(party, "reference");
                            break;
                        }
                    }
                }

                // =========================
                // ORGANIZATION DESDE SECTION
                // =========================
                if (resource.TryGetProperty("section", out var sections))
                {
                    foreach (var sec in sections.EnumerateArray())
                    {
                        if (sec.TryGetProperty("entry", out var entriesSec))
                        {
                            foreach (var ent in entriesSec.EnumerateArray())
                            {
                                string refTmp = DecompiladorRespuesta.GetStringSafe(ent, "reference");

                                if (!string.IsNullOrEmpty(refTmp) && refTmp.StartsWith("Organization"))
                                {
                                    organizationSectionRef = refTmp;
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(organizationSectionRef))
                            break;
                    }
                }

                // =========================
                // INSERTAR FILA
                // =========================

                //Obtener desde recursos
                string Tipoconsulta = "";
                string Medico = "";
                string IPS = "";
                string EPS = "PARTICULAR";

                var res2 = await rConsultaPaciente.GetData("", 10, "Encounter", encounterRef);
                if (res2.Est == "OK")
                {
                    Tipoconsulta = ExtraerTipoConsulta(res2.Resp);
                }
                var res3 = await rConsultaPaciente.GetData("", 10, "Practitioner", practitionerRef);
                if (res3.Est == "OK")
                {
                    Medico = ExtraerPractitioner(res3.Resp);
                }
                var res4 = await rConsultaPaciente.GetData("", 10, "Organization", organizationRef);
                {
                    IPS = ExtraerNombreOrganizacion(res4.Resp);
                }
                var res5 = await rConsultaPaciente.GetData("", 10, "Organization", organizationSectionRef);
                {
                    EPS = ExtraerNombreOrganizacion(res5.Resp);
                }

                dt.Rows.Add(
                    contador,
                    bundleLastUpdated?.ToString("yyyy-MM-dd") ?? "",
                    Tipoconsulta, //title,
                    resourceId,
                    resourceLastUpdated?.ToString("yyyy-MM-dd") ?? "",
                    encounterRef,
                    Medico, //practitionerRef,
                    IPS,
                    EPS,
                    fecha
                );

                contador++;
            }

            return dt;
        }
        string ExtraerNombreOrganizacion(string resJson)
        {
            var doc = JsonDocument.Parse(resJson);
            var root = doc.RootElement;

            if (root.TryGetProperty("name", out var name))
                return name.GetString();

            return "";
        }
        string ExtraerTipoConsulta(string resJson)
        {
            var doc = JsonDocument.Parse(resJson);
            var root = doc.RootElement;

            string consultaExterna = "";
            string medicinaGeneral = "";

            if (root.TryGetProperty("type", out var types))
            {
                foreach (var t in types.EnumerateArray())
                {
                    if (t.TryGetProperty("coding", out var codings))
                    {
                        foreach (var c in codings.EnumerateArray())
                        {
                            string system = DecompiladorRespuesta.GetStringSafe(c, "system");
                            string display = DecompiladorRespuesta.GetStringSafe(c, "display");

                            if (!string.IsNullOrEmpty(system))
                            {
                                if (system.Contains("GrupoServicios"))
                                    consultaExterna = display;

                                if (system.Contains("REPShealthcareServices"))
                                    medicinaGeneral = display;
                            }
                        }
                    }
                }

                return $"{consultaExterna} {medicinaGeneral}";
            }

            return "";
        }
        string ExtraerPractitioner(string resJson)
        {
            var doc = JsonDocument.Parse(resJson);
            var root = doc.RootElement;

            string family = "";
            string given = "";

            if (root.TryGetProperty("name", out var names) && names.GetArrayLength() > 0)
            {
                var name = names[0];

                // Apellido
                family = name.TryGetProperty("family", out var f) ? f.GetString() : "";

                // Nombres
                if (name.TryGetProperty("given", out var g))
                {
                    List<string> nombres = new List<string>();

                    foreach (var item in g.EnumerateArray())
                    {
                        nombres.Add(item.GetString());
                    }

                    given = string.Join(" ", nombres);
                }
            }

            return $"{given} {family}".Trim();
        }
        //HELPERS
        public static string GetSafeText(JsonElement element, string prop)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(prop, out var v) &&
                v.ValueKind == JsonValueKind.String)
                return v.GetString();

            return string.Empty;
        }
        public static string GetStringSafe(JsonElement element, string prop)
        {
            if (element.TryGetProperty(prop, out JsonElement value) && value.ValueKind != JsonValueKind.Null)
            {
                switch (value.ValueKind)
                {
                    case JsonValueKind.String:
                        return value.GetString() ?? "";

                    case JsonValueKind.Number:
                        return value.ToString(); // 👈 clave

                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return value.GetBoolean().ToString();

                    default:
                        return value.ToString();
                }
            }

            return "";
        }
    }
}
