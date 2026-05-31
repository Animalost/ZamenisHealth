using System.Linq;
using System.Text.Json;
using ZamenisHealth.FrontFHIR.VisorZamenis;

namespace ZamenisHealth.FrontFHIR.ClasesRDAs
{
    public class ExtractorRDAPaciente
    {
        public class Nacionalidad
        {
            public string nacionalidadCod { get; set; }
            public string nacionalidadPais { get; set; }
            public string etnia { get; set; }
            public string discapacidad { get; set; }
            public string identidadGenero { get; set; }
        }
        public class Residencia
        {
            public string zona { get; set; }
            public string municipio { get; set; }
            public string pais { get; set; }
        }

        public static string NombrePaciente(JsonElement root)
        {
            string nombreCompleto = "";

            if (root.TryGetProperty("name", out JsonElement names) && names.GetArrayLength() > 0)
            {
                var name = names[0];

                string given = "";
                if (name.TryGetProperty("given", out JsonElement givenArr))
                    given = string.Join(" ", givenArr.EnumerateArray().Select(x => x.GetString()));

                string family = DecompiladorRespuesta.GetStringSafe(name, "family");

                nombreCompleto = $"{given} {family}".Trim();
            }

            return nombreCompleto;
        }
        public static (string TDoc, string NDoc) IdentificacionPaciente(JsonElement root)
        {
            string tipoDoc = "";
            string numeroDoc = "";

            if (root.TryGetProperty("identifier", out JsonElement identifiers))
            {
                foreach (var id in identifiers.EnumerateArray())
                {
                    numeroDoc = DecompiladorRespuesta.GetStringSafe(id, "value");

                    if (id.TryGetProperty("type", out JsonElement type) &&
                        type.TryGetProperty("coding", out JsonElement codingArr))
                    {
                        foreach (var c in codingArr.EnumerateArray())
                        {
                            string system = DecompiladorRespuesta.GetStringSafe(c, "system");

                            if (system.Contains("ColombianPersonIdentifier"))
                            {
                                tipoDoc = DecompiladorRespuesta.GetStringSafe(c, "code");
                                break;
                            }
                        }
                    }
                }
            }

            return (tipoDoc, numeroDoc);
        }
        public static Nacionalidad NationalityData(JsonElement root)
        {
            string nacionalidadCod = "";
            string nacionalidadPais = "";
            string etnia = "";
            string discapacidad = "";
            string identidadGenero = "";

            if (root.TryGetProperty("extension", out JsonElement extensions))
            {
                foreach (var ext in extensions.EnumerateArray())
                {
                    string url = DecompiladorRespuesta.GetStringSafe(ext, "url");

                    if (!ext.TryGetProperty("valueCoding", out JsonElement val))
                        continue;

                    string code = DecompiladorRespuesta.GetStringSafe(val, "code");
                    string display = DecompiladorRespuesta.GetStringSafe(val, "display");

                    if (url.Contains("Nationality"))
                    {
                        nacionalidadCod = code;
                        nacionalidadPais = display;
                    }
                    else if (url.Contains("Ethnicity"))
                    {
                        etnia = display;
                    }
                    else if (url.Contains("Disability"))
                    {
                        discapacidad = display;
                    }
                    else if (url.Contains("GenderIdentity"))
                    {
                        identidadGenero = display;
                    }
                }
            }

            Nacionalidad N = new Nacionalidad
            {
                nacionalidadCod = nacionalidadCod,
                nacionalidadPais = nacionalidadPais,
                etnia = etnia,
                discapacidad = discapacidad,
                identidadGenero = identidadGenero
            };

            return N;
        }
        public static string SexoBiologico(JsonElement root)
        {
            string sexoBiologico = "";

            if (root.TryGetProperty("_gender", out JsonElement genderExt) &&
                genderExt.TryGetProperty("extension", out JsonElement extArr))
            {
                foreach (var ext in extArr.EnumerateArray())
                {
                    string url = DecompiladorRespuesta.GetStringSafe(ext, "url");

                    if (url.Contains("BiologicalGender"))
                    {
                        if (ext.TryGetProperty("valueCoding", out JsonElement val))
                            sexoBiologico = DecompiladorRespuesta.GetStringSafe(val, "display");
                    }
                }
            }

            return sexoBiologico;
        }
        public static (string FNto, string HNto) BirthData(JsonElement root)
        {
            string fechaNacimiento = DecompiladorRespuesta.GetStringSafe(root, "birthDate");
            string horaNacimiento = "";

            if (root.TryGetProperty("_birthDate", out JsonElement birthExt) &&
                birthExt.TryGetProperty("extension", out JsonElement extArr))
            {
                foreach (var ext in extArr.EnumerateArray())
                {
                    if (DecompiladorRespuesta.GetStringSafe(ext, "url").Contains("BirthTime"))
                    {
                        horaNacimiento = DecompiladorRespuesta.GetStringSafe(ext, "valueTime");
                    }
                }
            }

            return (fechaNacimiento, horaNacimiento);
        }
        public static Residencia ResidenceData(JsonElement root)
        {
            string zona = "";
            string municipio = "";
            string pais = "";

            if (root.TryGetProperty("address", out JsonElement addresses) &&
                addresses.GetArrayLength() > 0)
            {
                var addr = addresses[0];

                pais = DecompiladorRespuesta.GetStringSafe(addr, "country");

                // Zona
                if (addr.TryGetProperty("extension", out JsonElement extArr))
                {
                    foreach (var ext in extArr.EnumerateArray())
                    {
                        if (DecompiladorRespuesta.GetStringSafe(ext, "url").Contains("ResidenceZone"))
                        {
                            if (ext.TryGetProperty("valueCoding", out JsonElement val))
                                zona = DecompiladorRespuesta.GetStringSafe(val, "display");
                        }
                    }
                }

                // Municipio (DIVIPOLA)
                if (addr.TryGetProperty("_city", out JsonElement cityExt) &&
                    cityExt.TryGetProperty("extension", out JsonElement cityArr))
                {
                    foreach (var ext in cityArr.EnumerateArray())
                    {
                        if (DecompiladorRespuesta.GetStringSafe(ext, "url").Contains("Divipola"))
                        {
                            if (ext.TryGetProperty("valueCoding", out JsonElement val))
                                municipio = DecompiladorRespuesta.GetStringSafe(val, "code");
                        }
                    }
                }
            }

            Residencia R = new Residencia
            {
                zona = zona,
                municipio = municipio,
                pais = pais
            };

            return R;
        }
    }
}
