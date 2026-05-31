using APIFhir.VisorConsultas;
using FormAndControls;
using Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FrontFHIR.VisorZamenis
{
    public partial class VerRDAEncuentrosClinicos : Forma2
    {
        private readonly IConsultaPaciente rConsultaPaciente;
        private readonly IGetPDFB64 rGetB64FIHR;
        private string IdComposition;
        private DateTime Fecha;
        private string URLPdf;

        private MensajesGeneral MG;
        
        DataTable dt;
        DataColumn POS;
        DataColumn Categoriadt;
        DataColumn Medicamentodt;
        DataColumn Frecuenciadt;
        DataColumn Viadt;
        DataColumn Dosisdt;
        DataColumn Intervalodt;

        public VerRDAEncuentrosClinicos(string idComposition, DateTime fecha)
        {
            InitializeComponent();
            IdComposition = idComposition;
            Fecha = fecha;
            rConsultaPaciente = new ConsultaPaciente();
            rGetB64FIHR = new GetPDFB64();
        }

        private void VerRDAEncuentrosClinicos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Encuentro Clinico " + Convert.ToDateTime(Fecha).ToString("yyyy-MM-dd");
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarEncuentro();
        }
        async void CargarEncuentro()
        {
            try
            {
                var resJson = await rConsultaPaciente.GetData("", 10, "GetIdComposition", IdComposition);
                if (resJson.Est == "OK")
                {
                    var doc = JsonDocument.Parse(resJson.Resp);
                    var root = doc.RootElement;
                    var entries = root.GetProperty("entry");

                    // listas
                    List<string> condicionesRefs = new List<string>();
                    List<string> alergiasRefs = new List<string>();
                    List<string> ordenesRefs = new List<string>();
                    List<string> medicamentosRefs = new List<string>();
                    List<string> familiaresRefs = new List<string>();

                    // recursos
                    List<string> condiciones = new List<string>();
                    List<string> alergias = new List<string>();
                    List<string> medicamentos = new List<string>();
                    List<string> antecedentesFam = new List<string>();

                    // nuevas secciones
                    List<string> eps = new List<string>();
                    List<string> datosDemograficos = new List<string>();
                    List<string> incapacidad = new List<string>();
                    List<string> factoresRiesgo = new List<string>();
                    List<string> documentos = new List<string>();

                    // referencias
                    List<string> epsRefs = new List<string>();
                    List<string> obsRefs = new List<string>();
                    List<string> incapacidadRefs = new List<string>();
                    List<string> riesgoRefs = new List<string>();
                    List<string> docRefs = new List<string>();
                    string practitionerRef = "";
                    string organizationRef = "";
                    string titles = "";

                    foreach (var entry in entries.EnumerateArray())
                    {
                        if (!entry.TryGetProperty("resource", out JsonElement resource))
                            continue;

                        string tipo = resource.GetProperty("resourceType").GetString();

                        //Llenar las secciones
                        if (tipo == "Composition")
                        {
                            #region OBTENER DATOS DE PRESTAODS, PRACTITIONER Y TIPO CONSULTA
                            titles = resource.TryGetProperty("title", out var tTitle) ? tTitle.GetString() ?? "" : "";

                            string start = "";
                            string end = "";

                            // PRACTITIONER URL
                            if (resource.TryGetProperty("attester", out var attesters))
                            {
                                foreach (var att in attesters.EnumerateArray())
                                {
                                    if (att.TryGetProperty("party", out var party) &&
                                        party.TryGetProperty("reference", out var pref))
                                    {
                                        practitionerRef = pref.GetString() ?? "";
                                        break;
                                    }
                                }
                            }

                            // CUSTODIAN (IPS)
                            if (resource.TryGetProperty("custodian", out var cust) && cust.TryGetProperty("reference", out var cref))
                            {
                                organizationRef = cref.GetString() ?? "";
                            }

                            // PERIODO DEL ENCUENTRO
                            if (resource.TryGetProperty("event", out var events))
                            {
                                foreach (var ev in events.EnumerateArray())
                                {
                                    if (ev.TryGetProperty("period", out var period))
                                    {
                                        if (period.TryGetProperty("start", out var s))
                                            start = s.GetString() ?? "";

                                        if (period.TryGetProperty("end", out var e))
                                            end = e.GetString() ?? "";

                                        break;
                                    }
                                }

                                label17.Text = Convert.ToDateTime(start).ToString("yyyy-MM-dd HH:mm:ss tt");
                                label13.Text = Convert.ToDateTime(end).ToString("yyyy-MM-dd HH:mm:ss tt");
                                label46.Text = titles;
                            }
                            #endregion

                            if (resource.TryGetProperty("section", out var sections))
                            {
                                foreach (var sec in sections.EnumerateArray())
                                {
                                    string title = sec.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";

                                    bool estaVacio = sec.TryGetProperty("emptyReason", out _);

                                    if (!sec.TryGetProperty("entry", out var refs) || estaVacio)
                                        continue;

                                    foreach (var r in refs.EnumerateArray())
                                    {
                                        string reference = r.GetProperty("reference").GetString();

                                        //trae los references URL para consultar

                                        if (title.Contains("plan de beneficios")) //HECHO - la eps
                                            epsRefs.Add(reference);

                                        else if (title.Contains("demográficos")) //HECHO - ocupacion
                                            obsRefs.Add(reference);

                                        else if (title.Contains("diagnósticos")) //HECHO - diagnosticos de la atencion
                                            condicionesRefs.Add(reference);

                                        else if (title.Contains("alergias")) //HECHO - Alergias en la atencion
                                            alergiasRefs.Add(reference);

                                        else if (title.Contains("prescripciones")) //HECHO - Ordenes dadas en la atencion
                                            ordenesRefs.Add(reference);

                                        else if (title.Contains("medicamentos")) //HECHO - Medicamentos recetados en la atencion
                                            medicamentosRefs.Add(reference);

                                        else if (title.Contains("Documentos")) //HECHO - PDF
                                            docRefs.Add(reference);

                                        else if (title.Contains("incapacidad")) //HECHO - INCAPACIDADES
                                            incapacidadRefs.Add(reference);

                                        else if (title.Contains("riesgo")) //HECHO - FACTORES DE RIESGO
                                            riesgoRefs.Add(reference);
                                    }
                                }
                            }
                        }
                    }

                    //Empezar a extraer recursos a partir de las referencias
                    #region ORGANIZATION EPS
                    string nombreOrg = "";
                    string codigo = "";
                    string tipoorg2 = "";
                    bool activo = false;

                    var dataOrg = await ExtraerRecursos(epsRefs[0], "Organization");
                    if (dataOrg != "")
                    {
                        if (!string.IsNullOrEmpty(dataOrg))
                        {
                            var docOrg = JsonDocument.Parse(dataOrg);
                            var resource = docOrg.RootElement;

                            // NAME
                            if (resource.TryGetProperty("name", out var n))
                                nombreOrg = n.GetString() ?? "";

                            // ACTIVE
                            if (resource.TryGetProperty("active", out var a))
                                activo = a.GetBoolean();

                            // IDENTIFIER
                            if (resource.TryGetProperty("identifier", out var identifiers))
                            {
                                foreach (var id in identifiers.EnumerateArray())
                                {
                                    if (id.TryGetProperty("value", out var v))
                                        codigo = v.GetString() ?? "";

                                    if (id.TryGetProperty("type", out var type) &&
                                        type.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            if (DecompiladorRespuesta.GetStringSafe(c, "code") == "EAPB")
                                            {
                                                tipoorg2 = "EAPB";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            label1.Text = nombreOrg.ToUpper().Trim();
                            label2.Text = tipoorg2.ToUpper().Trim();
                            label5.Text = codigo.ToUpper().Trim();
                            label8.Text = activo ? "ACTIVO" : "FALSO";
                        }
                    }
                    #endregion

                    #region ORGANIZATION IPS
                    var dataIPS = await ExtraerRecursos(organizationRef, "Organization");
                    if (dataIPS != "")
                    {
                        string nombrePrestador = "";
                        string nit = "";
                        string codigoPrestador = "";
                        string tipoCodigo = "";
                        string tipoDisplay = "";
                        string naturalezaDisplay = "";
                        string ciudad = "";

                        if (!string.IsNullOrEmpty(dataIPS))
                        {
                            var docIPS = JsonDocument.Parse(dataIPS);
                            var resource = docIPS.RootElement;

                            // NOMBRE
                            if (resource.TryGetProperty("name", out var n))
                                nombrePrestador = n.GetString() ?? "";

                            // IDENTIFICACION
                            if (resource.TryGetProperty("identifier", out var identifiers))
                            {
                                foreach (var id in identifiers.EnumerateArray())
                                {
                                    string value = id.TryGetProperty("value", out var v) ? v.GetString() ?? "" : "";

                                    if (id.TryGetProperty("type", out var type) &&
                                        type.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            string code = DecompiladorRespuesta.GetStringSafe(c, "code");

                                            // NIT
                                            if (code == "NIT")
                                            {
                                                nit = value;
                                            }

                                            // Código Prestador
                                            if (code == "CodigoPrestador")
                                            {
                                                codigoPrestador = value;
                                            }
                                        }
                                    }
                                }
                            }

                            // TIPO
                            if (resource.TryGetProperty("type", out var types))
                            {
                                int index = 0;

                                foreach (var t in types.EnumerateArray())
                                {
                                    if (t.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            string code = DecompiladorRespuesta.GetStringSafe(c, "code");
                                            string display = DecompiladorRespuesta.GetStringSafe(c, "display");

                                            // Tipo organización (IPS)
                                            if (index == 0)
                                            {
                                                tipoCodigo = code;
                                                tipoDisplay = display;
                                            }

                                            // Naturaleza (Privada)
                                            if (index == 1)
                                            {
                                                naturalezaDisplay = display;
                                            }
                                        }
                                    }

                                    index++;
                                }
                            }

                            // DIRECCION
                            if (resource.TryGetProperty("address", out var addresses))
                            {
                                foreach (var addr in addresses.EnumerateArray())
                                {
                                    if (addr.TryGetProperty("city", out var c))
                                    {
                                        ciudad = c.GetString() ?? "";
                                        break;
                                    }
                                }
                            }

                            label37.Text = nombrePrestador.ToUpper().Trim();
                            label35.Text = tipoCodigo + " " + nit.ToUpper().Trim();
                            label22.Text = codigoPrestador.ToUpper().Trim();
                            label20.Text = tipoDisplay.ToUpper().Trim();
                            label42.Text = naturalezaDisplay.ToUpper().Trim();
                            label40.Text = ciudad.ToUpper().Trim();
                        }
                    }
                    #endregion

                    #region PRACTITIONER
                    string numeroDocProfesional = "";
                    string tipoDocProfesional = "";
                    string tipoDocDisplayProfesional = "";
                    string apellidosProfesional = "";
                    string nombresProfesional = "";
                    string codProfesionProfesional = "";
                    string displayProfesionProfesional = "";

                    var dataProfesional = await ExtraerRecursos(practitionerRef, "Practitioner");
                    if (dataProfesional != "")
                    {
                        if (!string.IsNullOrEmpty(dataProfesional))
                        {
                            var docProfesional = JsonDocument.Parse(dataProfesional);
                            var resource = docProfesional.RootElement;

                            if (resource.TryGetProperty("identifier", out var identifiers))
                            {
                                foreach (var id in identifiers.EnumerateArray())
                                {
                                    if (id.TryGetProperty("value", out var v))
                                        numeroDocProfesional = v.GetString() ?? "";

                                    if (id.TryGetProperty("type", out var type) &&
                                        type.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            tipoDocProfesional = DecompiladorRespuesta.GetStringSafe(c, "code");
                                            tipoDocDisplayProfesional = DecompiladorRespuesta.GetStringSafe(c, "display");
                                            break;
                                        }
                                    }

                                    break; // solo el primero
                                }
                            }
                            if (resource.TryGetProperty("name", out var names))
                            {
                                foreach (var n in names.EnumerateArray())
                                {
                                    // Apellidos
                                    if (n.TryGetProperty("family", out var f))
                                        apellidosProfesional = f.GetString() ?? "";

                                    // Nombres
                                    if (n.TryGetProperty("given", out var g))
                                    {
                                        List<string> listaNombres = new List<string>();

                                        foreach (var item in g.EnumerateArray())
                                        {
                                            listaNombres.Add(item.GetString() ?? "");
                                        }

                                        nombresProfesional = string.Join(" ", listaNombres);
                                    }

                                    break; // solo el primero
                                }
                            }
                            if (resource.TryGetProperty("qualification", out var qualifications))
                            {
                                foreach (var q in qualifications.EnumerateArray())
                                {
                                    if (q.TryGetProperty("code", out var code) &&
                                        code.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            codProfesionProfesional = DecompiladorRespuesta.GetStringSafe(c, "code");
                                            displayProfesionProfesional = DecompiladorRespuesta.GetStringSafe(c, "display");
                                            break;
                                        }
                                    }

                                    break; // solo el primero
                                }
                            }

                            label54.Text = nombresProfesional + " " + apellidosProfesional;
                            label52.Text = tipoDocProfesional + " " + numeroDocProfesional;
                            label44.Text = displayProfesionProfesional.ToUpper().Trim();
                            label50.Text = codProfesionProfesional.ToUpper().Trim();
                        }
                    }
                    #endregion

                    #region OCUPACION
                    if (obsRefs.Count > 0)
                    {
                        string codigoOcupacion = "";
                        string displayOcupacion = "";

                        var dataOcupacion = await ExtraerRecursos(obsRefs[0], "Ocupacion");
                        if (dataOcupacion != "")
                        {
                            if (!string.IsNullOrEmpty(dataOcupacion))
                            {
                                var docOcupacion = JsonDocument.Parse(dataOcupacion);
                                var resource = docOcupacion.RootElement;

                                if (resource.TryGetProperty("valueCodeableConcept", out var vcc) && vcc.TryGetProperty("coding", out var codings))
                                {
                                    foreach (var c in codings.EnumerateArray())
                                    {
                                        codigoOcupacion = DecompiladorRespuesta.GetStringSafe(c, "code");
                                        displayOcupacion = DecompiladorRespuesta.GetStringSafe(c, "display");
                                        break; // solo el primero
                                    }
                                }

                                label11.Text = displayOcupacion.ToUpper().Trim();
                                label15.Text = codigoOcupacion.ToUpper().Trim();
                            }
                        }
                    }
                    else
                    {
                        panel2.Visible = false;
                    }
                    #endregion

                    #region SERVICE REQUEST
                    // LISTAS
                    List<string> catCodes = new List<string>();
                    List<string> catDisplays = new List<string>();

                    List<string> procCodes = new List<string>();
                    List<string> procDisplays = new List<string>();

                    List<string> reasonCodes = new List<string>();
                    List<string> reasonDisplays = new List<string>();

                    if (ordenesRefs != null && ordenesRefs.Count > 0)
                    {
                        foreach (var i in ordenesRefs)
                        {
                            var dataSR = await ExtraerRecursos(i, "ServiceRequest");
                            if (dataSR != "")
                            {
                                var docSR = JsonDocument.Parse(dataSR);
                                var resource = docSR.RootElement;

                                // CATEGORY (pueden venir varias)
                                if (resource.TryGetProperty("category", out var categories))
                                {
                                    foreach (var cat in categories.EnumerateArray())
                                    {
                                        if (cat.TryGetProperty("coding", out var codings))
                                        {
                                            foreach (var c in codings.EnumerateArray())
                                            {
                                                catCodes.Add(DecompiladorRespuesta.GetStringSafe(c, "code"));
                                                catDisplays.Add(DecompiladorRespuesta.GetStringSafe(c, "display"));
                                            }
                                        }
                                    }
                                }

                                // CODE (aunque normalmente es uno, igual lo manejamos como lista)
                                if (resource.TryGetProperty("code", out var codeObj))
                                {
                                    if (codeObj.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            procCodes.Add(DecompiladorRespuesta.GetStringSafe(c, "code"));
                                            procDisplays.Add(DecompiladorRespuesta.GetStringSafe(c, "display"));
                                        }
                                    }
                                }

                                // REASON CODE (pueden venir varios)
                                if (resource.TryGetProperty("reasonCode", out var reasons))
                                {
                                    foreach (var r in reasons.EnumerateArray())
                                    {
                                        if (r.TryGetProperty("coding", out var codings))
                                        {
                                            foreach (var c in codings.EnumerateArray())
                                            {
                                                reasonCodes.Add(DecompiladorRespuesta.GetStringSafe(c, "code"));
                                                reasonDisplays.Add(DecompiladorRespuesta.GetStringSafe(c, "display"));
                                            }
                                        }
                                    }
                                }                              
                            }
                        }

                        List<string> lineas = new List<string>();

                        int max = new[] { catCodes.Count, procCodes.Count, reasonCodes.Count }.Max();

                        if (max > 0)
                        {
                            for (int i = 0; i < max; i++)
                            {
                              /*  string linea =
                                    $"{catCodes.ElementAtOrDefault(i)} - {catDisplays.ElementAtOrDefault(i)} " +
                                    $"{procCodes.ElementAtOrDefault(i)} - {procDisplays.ElementAtOrDefault(i)} " +
                                    $"{reasonCodes.ElementAtOrDefault(i)} | {reasonDisplays.ElementAtOrDefault(i)}";*/

                                string linea =
                                  $"{procCodes.ElementAtOrDefault(i)} - {procDisplays.ElementAtOrDefault(i)}";

                                lineas.Add(linea.Trim());
                            }

                            if (lineas.Count > 0)
                            {
                                listBox15.DataSource = lineas;
                                listBox15.ClearSelected();
                            }
                            else
                            {
                                panel23.Visible = false;
                            }                            
                        }
                        else
                        {
                            panel23.Visible = false;
                        }                       
                    }
                    else
                    {
                        panel23.Visible = false;
                    }
                    #endregion

                    #region DIAGNOSTICOS
                    List<string> diagnosticos = new List<string>();
                    bool isPpal = false;
                    string CodePpal = "";
                    string NamePpal = "";

                    if (condicionesRefs != null)
                    {
                        foreach (var i in condicionesRefs)
                        {
                            var dataDX = await ExtraerRecursos(i, "DX");
                            if (dataDX != "")
                            {
                                var docDX = JsonDocument.Parse(dataDX);
                                var resource = docDX.RootElement;

                                if (resource.TryGetProperty("code", out var code) && code.TryGetProperty("coding", out var codings))
                                {
                                    foreach (var c in codings.EnumerateArray())
                                    {
                                        string codigotemp = DecompiladorRespuesta.GetStringSafe(c, "code");
                                        string displaytemp = DecompiladorRespuesta.GetStringSafe(c, "display");

                                        if (isPpal == false)
                                        {
                                            CodePpal = codigotemp;
                                            NamePpal = displaytemp;
                                            isPpal = true;
                                        }
                                        else
                                        {
                                            diagnosticos.Add($"{codigotemp} - {displaytemp}");
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        label57.Text = CodePpal;
                        label49.Text = NamePpal;

                        if (diagnosticos.Count > 0)
                        {
                            listBox1.DataSource = diagnosticos;
                            listBox1.ClearSelected();
                        }
                        else
                        {
                            label58.Visible = false;
                            listBox1.Visible = false;
                        }
                    }
                    #endregion

                    #region ALERGIAS
                    List<allergyList> allergy = new List<allergyList>();
                    string codigoAllergy = "";
                    string displayAllergy = "";
                    string textoAllergy = "";

                    if (alergiasRefs != null && alergiasRefs.Count > 0)
                    {
                        foreach (var i in alergiasRefs)
                        {
                            var dataAllergy = await ExtraerRecursos(i, "Allergy");
                            if (dataAllergy != "")
                            {
                                var docAllergy = JsonDocument.Parse(dataAllergy);
                                var resource = docAllergy.RootElement;

                                if (resource.TryGetProperty("code", out var code))
                                {
                                    if (code.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            codigoAllergy = DecompiladorRespuesta.GetStringSafe(c, "code");
                                            displayAllergy = DecompiladorRespuesta.GetStringSafe(c, "display");
                                            break;
                                        }
                                    }

                                    if (code.TryGetProperty("text", out var t))
                                    {
                                        textoAllergy = t.GetString() ?? "";
                                    }

                                    allergy.Add(new allergyList
                                    {
                                        Code = codigoAllergy,
                                        Tipo = displayAllergy,
                                        Alegia = textoAllergy,
                                    });
                                }
                            }
                        }

                        if (allergy.Count > 0)
                        {
                            List<allergyList> MedicamentAllergy = allergy.Where(x => x.Tipo == "Medicamento").ToList();
                            if (MedicamentAllergy.Count > 0)
                            {
                                foreach (var i in MedicamentAllergy)
                                {
                                    listBox2.Items.Add(i.Alegia);
                                    listBox2.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel11.Visible = false;
                            }

                            List<allergyList> FoodAllergy = allergy.Where(x => x.Tipo == "Alimento").ToList();
                            if (FoodAllergy.Count > 0)
                            {
                                foreach (var i in FoodAllergy)
                                {
                                    listBox5.Items.Add(i.Alegia);
                                    listBox5.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel12.Visible = false;
                            }

                            List<allergyList> SustAmbientAllergy = allergy.Where(x => x.Tipo == "Sustancia del ambiente").ToList();
                            if (SustAmbientAllergy.Count > 0)
                            {
                                foreach (var i in SustAmbientAllergy)
                                {
                                    listBox6.Items.Add(i.Alegia);
                                    listBox6.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel13.Visible = false;
                            }

                            List<allergyList> SustPielAllergy = allergy.Where(x => x.Tipo == "Sustancia que entran en contacto con la piel").ToList();
                            if (SustPielAllergy.Count > 0)
                            {
                                foreach (var i in SustPielAllergy)
                                {
                                    listBox7.Items.Add(i.Alegia);
                                    listBox7.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel14.Visible = false;
                            }

                            List<allergyList> PicaduraAllergy = allergy.Where(x => x.Tipo == "Picadura de insectos").ToList();
                            if (PicaduraAllergy.Count > 0)
                            {
                                foreach (var i in PicaduraAllergy)
                                {
                                    listBox8.Items.Add(i.Alegia);
                                    listBox8.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel15.Visible = false;
                            }

                            List<allergyList> OtrasAllergy = allergy.Where(x => x.Tipo == "Otra").ToList();
                            if (OtrasAllergy.Count > 0)
                            {
                                foreach (var i in OtrasAllergy)
                                {
                                    listBox9.Items.Add(i.Alegia);
                                    listBox9.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel16.Visible = false;
                            }
                        }
                        else
                        {
                            panel5.Visible = false;
                        }
                    }
                    else
                    {
                        panel5.Visible = false;
                    }
                    #endregion

                    #region PDF

                    var dataPDF = await ExtraerRecursos(docRefs[0], "Ocupacion");
                    if (dataPDF != "")
                    {
                        if (!string.IsNullOrEmpty(dataPDF))
                        {
                            var docPDF = JsonDocument.Parse(dataPDF);
                            var resource = docPDF.RootElement;

                            if (resource.TryGetProperty("content", out var contents))
                            {
                                foreach (var c in contents.EnumerateArray())
                                {
                                    if (c.TryGetProperty("attachment", out var attachment) &&
                                        attachment.TryGetProperty("url", out var u))
                                    {
                                        URLPdf = u.GetString() ?? "";
                                        break;
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(URLPdf))
                        {
                            pictureBox4.Visible = true;
                        }
                        else
                        {
                            pictureBox4.Visible = false;
                        }
                    }
                    #endregion

                    #region INCAPACIDAD
                    string codigoIncapacidad = "";
                    string displayIncapacidad = "";

                    string textoTiempoIncapacidad = "";
                    decimal valorIncapacidad = 0;
                    string unidadIncapacidad = "";

                    if (incapacidadRefs != null && incapacidadRefs.Count > 0)
                    {
                        var dataIncapacidad = await ExtraerRecursos(incapacidadRefs[0], "Incapacidad");
                        if (dataIncapacidad != null)
                        {
                            var docIncapacidad = JsonDocument.Parse(dataIncapacidad);
                            var resource = docIncapacidad.RootElement;

                            if (resource.TryGetProperty("component", out var components))
                            {
                                foreach (var comp in components.EnumerateArray())
                                {
                                    string id = comp.TryGetProperty("id", out var i) ? i.GetString() ?? "" : "";

                                    if (id == "LicenseScope")
                                    {
                                        if (comp.TryGetProperty("valueCodeableConcept", out var vcc) &&
                                            vcc.TryGetProperty("coding", out var codings))
                                        {
                                            foreach (var c in codings.EnumerateArray())
                                            {
                                                codigoIncapacidad = DecompiladorRespuesta.GetStringSafe(c, "code");
                                                displayIncapacidad = DecompiladorRespuesta.GetStringSafe(c, "display");
                                                break;
                                            }
                                        }
                                    }

                                    if (id == "LicenseTime")
                                    {
                                        // texto
                                        if (comp.TryGetProperty("code", out var code) &&
                                            code.TryGetProperty("text", out var t))
                                        {
                                            textoTiempoIncapacidad = t.GetString() ?? "";
                                        }

                                        // valueQuantity
                                        if (comp.TryGetProperty("valueQuantity", out var vq))
                                        {
                                            if (vq.TryGetProperty("value", out var v))
                                                valorIncapacidad = v.GetDecimal();

                                            if (vq.TryGetProperty("unit", out var u))
                                                unidadIncapacidad = u.GetString() ?? "";
                                        }
                                    }
                                }
                            }

                            label24.Text = codigoIncapacidad.ToUpper().Trim();
                            label26.Text = displayIncapacidad.ToUpper().Trim();
                            //label28.Text = $"{ textoTiempoIncapacidad.ToUpper().Trim() } Tiempo: { valorIncapacidad } Unidad: { unidadIncapacidad }";
                            label28.Text = $"{valorIncapacidad} {unidadIncapacidad}";
                        }
                    }
                    else
                    {
                        panel3.Visible = false;
                    }
                    #endregion

                    #region MEDICAMENTOS
                    List<MedicamentList> medicaments = new List<MedicamentList>();

                    string categoriaCode = "";
                    string categoriaDisplay = "";

                    string timingCode = "";
                    string timingDisplay = "";

                    string routeCode = "";
                    string routeDisplay = "";

                    string doseValue = "";
                    string doseUnit = "";

                    string rateValue = "";
                    string rateUnit = "";

                    string codigoMedicamento = "";
                    string displayMedicamento = "";
                    string textoMedicamento = "";

                    if (medicamentosRefs != null && medicamentosRefs.Count > 0)
                    {
                        foreach (var i in medicamentosRefs)
                        {
                            var dataMedicamentos = await ExtraerRecursos(i, "Medicamentos");
                            if (dataMedicamentos != "")
                            {
                                var docMedicamentos = JsonDocument.Parse(dataMedicamentos);
                                var resource = docMedicamentos.RootElement;

                                // =========================
                                // CATEGORY
                                // =========================
                                if (resource.TryGetProperty("category", out var categories))
                                {
                                    foreach (var cat in categories.EnumerateArray())
                                    {
                                        if (cat.TryGetProperty("coding", out var codings))
                                        {
                                            foreach (var c in codings.EnumerateArray())
                                            {
                                                categoriaCode = DecompiladorRespuesta.GetStringSafe(c, "code");
                                                categoriaDisplay = DecompiladorRespuesta.GetStringSafe(c, "display");
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }

                                // =========================
                                // MEDICAMENTO
                                // =========================
                                if (resource.TryGetProperty("medicationCodeableConcept", out var med))
                                {
                                    if (med.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            codigoMedicamento = DecompiladorRespuesta.GetStringSafe(c, "code");
                                            displayMedicamento = DecompiladorRespuesta.GetStringSafe(c, "display");
                                            break;
                                        }
                                    }

                                    if (med.TryGetProperty("text", out var t))
                                    {
                                        textoMedicamento = t.GetString() ?? "";
                                    }

                                    //string medTexto = $"{codigoMedicamento} - {displayMedicamento} --> {textoMedicamento}";
                                    //string medTexto = $"{codigoMedicamento} - {displayMedicamento}";
                                    //medicaments.Add(medTexto);
                                }

                                // =========================
                                // DOSAGE INSTRUCTION
                                // =========================
                                if (resource.TryGetProperty("dosageInstruction", out var dosageArray))
                                {
                                    foreach (var dosage in dosageArray.EnumerateArray())
                                    {
                                        // -------- TIMING --------
                                        if (dosage.TryGetProperty("timing", out var timing))
                                        {
                                            if (timing.TryGetProperty("code", out var code))
                                            {
                                                if (code.TryGetProperty("coding", out var codings))
                                                {
                                                    foreach (var c in codings.EnumerateArray())
                                                    {
                                                        timingCode = DecompiladorRespuesta.GetStringSafe(c, "code");
                                                        timingDisplay = DecompiladorRespuesta.GetStringSafe(c, "display");
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        // -------- ROUTE --------
                                        if (dosage.TryGetProperty("route", out var route))
                                        {
                                            if (route.TryGetProperty("coding", out var codings))
                                            {
                                                foreach (var c in codings.EnumerateArray())
                                                {
                                                    routeCode = DecompiladorRespuesta.GetStringSafe(c, "code");
                                                    routeDisplay = DecompiladorRespuesta.GetStringSafe(c, "display");
                                                    break;
                                                }
                                            }
                                        }

                                        // -------- DOSE & RATE --------
                                        if (dosage.TryGetProperty("doseAndRate", out var doseArray))
                                        {
                                            foreach (var dr in doseArray.EnumerateArray())
                                            {
                                                // DOSE
                                                if (dr.TryGetProperty("doseQuantity", out var dose))
                                                {
                                                    doseValue = DecompiladorRespuesta.GetStringSafe(dose, "value");
                                                    doseUnit = DecompiladorRespuesta.GetStringSafe(dose, "unit");
                                                }

                                                // RATE
                                                if (dr.TryGetProperty("rateQuantity", out var rate))
                                                {
                                                    rateValue = DecompiladorRespuesta.GetStringSafe(rate, "value");
                                                    rateUnit = DecompiladorRespuesta.GetStringSafe(rate, "unit");
                                                }

                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }
                            }

                            medicaments.Add(new MedicamentList
                            {
                                Categoria = categoriaCode + " - " + categoriaDisplay,
                                Medicamento = codigoMedicamento + " - " + displayMedicamento,
                                //Frecuencia = timingCode + " - " + timingDisplay,
                                //Via = routeCode + " - " + routeDisplay,
                                Frecuencia = $"Cada {timingCode} {timingDisplay}",
                                Via = routeDisplay,
                                Dosis = doseValue + " - " + doseUnit,
                                Intervalo = $"Por {rateValue} {rateUnit}"
                            });
                        }

                        Encabezados();
                        int Contador = 1;

                        foreach (var i in medicaments)
                        {
                            DataRow row = dt.NewRow();

                            row[POS] = Contador;
                            row[Categoriadt] = i.Categoria.ToString();
                            row[Medicamentodt] = i.Medicamento.ToString();
                            row[Frecuenciadt] = i.Frecuencia.ToString();
                            row[Viadt] = i.Via.ToString();
                            row[Dosisdt] = i.Dosis.ToString();
                            row[Intervalodt] = i.Intervalo.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos(dataGridView1, dt);
                    }
                    else
                    {
                        panel6.Visible = false;
                    }
                    #endregion

                    #region RIESGO
                    List<RiskList> RiesgoList = new List<RiskList>();
                    string codigoRiesgo = "";
                    string displayRiesgo = "";
                    string textoRiesgo = "";

                    if (riesgoRefs != null && riesgoRefs.Count > 0)
                    {
                        foreach (var i in riesgoRefs)
                        {
                            var dataRiesgo = await ExtraerRecursos(i, "Medicamentos");
                            if (dataRiesgo != "")
                            {
                                var docRiesgo = JsonDocument.Parse(dataRiesgo);
                                var resource = docRiesgo.RootElement;

                                if (resource.TryGetProperty("code", out var code))
                                {
                                    if (code.TryGetProperty("coding", out var codings))
                                    {
                                        foreach (var c in codings.EnumerateArray())
                                        {
                                            codigoRiesgo = DecompiladorRespuesta.GetStringSafe(c, "code");
                                            displayRiesgo = DecompiladorRespuesta.GetStringSafe(c, "display");
                                            break;
                                        }
                                    }

                                    if (code.TryGetProperty("text", out var t))
                                    {
                                        textoRiesgo = t.GetString() ?? "";
                                    }

                                    RiesgoList.Add(new RiskList
                                    {
                                        Code = codigoRiesgo,
                                        Tipo = displayRiesgo,
                                        Riesgo = textoRiesgo
                                    });
                                }
                            }
                        }

                        if (RiesgoList.Count > 0)
                        {
                            List<RiskList> QuimicoRisk = RiesgoList.Where(x => x.Tipo == "Químicos").ToList();
                            if (QuimicoRisk.Count > 0)
                            {
                                foreach (var i in QuimicoRisk)
                                {
                                    listBox4.Items.Add(i.Riesgo);
                                    listBox4.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel17.Visible = false;
                            }

                            List<RiskList> FisicoRisk = RiesgoList.Where(x => x.Tipo == "Físicos").ToList();
                            if (FisicoRisk.Count > 0)
                            {
                                foreach (var i in FisicoRisk)
                                {
                                    listBox10.Items.Add(i.Riesgo);
                                    listBox10.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel18.Visible = false;
                            }

                            List<RiskList> BiomecanicoRisk = RiesgoList.Where(x => x.Tipo == "Biomecánicos").ToList();
                            if (BiomecanicoRisk.Count > 0)
                            {
                                foreach (var i in BiomecanicoRisk)
                                {
                                    listBox11.Items.Add(i.Riesgo);
                                    listBox11.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel19.Visible = false;
                            }

                            List<RiskList> PsicosocialRisk = RiesgoList.Where(x => x.Tipo == "Psicosociales").ToList();
                            if (PsicosocialRisk.Count > 0)
                            {
                                foreach (var i in PsicosocialRisk)
                                {
                                    listBox12.Items.Add(i.Riesgo);
                                    listBox12.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel20.Visible = false;
                            }

                            List<RiskList> BiologicoRisk = RiesgoList.Where(x => x.Tipo == "Biológicos").ToList();
                            if (BiologicoRisk.Count > 0)
                            {
                                foreach (var i in BiologicoRisk)
                                {
                                    listBox13.Items.Add(i.Riesgo);
                                    listBox13.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel21.Visible = false;
                            }

                            List<RiskList> OtroRisk = RiesgoList.Where(x => x.Tipo == "Otro").ToList();
                            if (OtroRisk.Count > 0)
                            {
                                foreach (var i in OtroRisk)
                                {
                                    listBox14.Items.Add(i.Riesgo);
                                    listBox14.ClearSelected();
                                }                                
                            }
                            else
                            {
                                panel22.Visible = false;
                            }
                        }
                        else
                        {
                            panel7.Visible = false;
                        }                    
                    }
                    else
                    {
                        panel7.Visible = false;
                    }
                    #endregion
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro cargar el encuentro clinico, intente nuevamente",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Categoria"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Medicamento"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Frecuencia"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Via"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Dosis"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Intervalo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;

            foreach (DataGridViewRow row in D.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                }
            }
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Categoriadt = dt.Columns.Add("Categoria", typeof(string));
            Medicamentodt = dt.Columns.Add("Medicamento", typeof(string));
            Frecuenciadt = dt.Columns.Add("Frecuencia", typeof(string));
            Viadt = dt.Columns.Add("Via", typeof(string));
            Dosisdt = dt.Columns.Add("Dosis", typeof(string));
            Intervalodt = dt.Columns.Add("Intervalo", typeof(string));
        }
        public class MedicamentList
        {
            public string Categoria { get; set;  }
            public string Medicamento { get; set; }
            public string Frecuencia { get; set; }
            public string Via { get; set; }
            public string Dosis { get; set; }
            public string Intervalo { get; set; }
        }
        public class allergyList
        {
            public string Code { get; set; }
            public string Tipo { get; set; }
            public string Alegia { get; set; }
        }
        public class RiskList
        {
            public string Code { get; set; }
            public string Tipo { get; set; }
            public string Riesgo { get; set; }
        }
        async Task<string> ExtraerRecursos(string URL, string Tipo)
        {
            try
            {
                var resJson = await rConsultaPaciente.GetData("", 10, Tipo, URL);
                if (resJson.Est == "OK")
                {
                    return resJson.Resp;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }
        private async void pictureBox4_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var resJson = await rGetB64FIHR.GetDataB64(10, URLPdf);
                if (resJson.Est == "OK")
                {
                    var doc = JsonDocument.Parse(resJson.Resp);
                    var root = doc.RootElement;

                    string base64Pdf = "";

                    if (root.TryGetProperty("content", out var contents) && contents.ValueKind == JsonValueKind.Array && contents.GetArrayLength() > 0)
                    {
                        var content = contents[0];

                        if (content.TryGetProperty("attachment", out var attachment))
                        {
                            if (attachment.TryGetProperty("data", out var dataProp))
                            {
                                base64Pdf = dataProp.GetString();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(base64Pdf))
                    {
                        byte[] pdfBytes = Convert.FromBase64String(base64Pdf);

                        using (var stream = new MemoryStream(pdfBytes))
                        {
                            string base64 = Convert.ToBase64String(stream.ToArray());
                            string html = $@"
                                            <html>
                                              <body style='margin:0'>
                                                <iframe 
                                                    width='100%' 
                                                    height='100%' 
                                                    src='data:application/pdf;base64,{base64}#toolbar=0'>
                                                </iframe>
                                              </body>
                                            </html>";

                            VisorPDF visorPDF = new VisorPDF(html, base64);
                            visorPDF.ShowDialog();
                        }
                    }
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay PDF cargado para esta atencion",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ResizePanel(Panel panel)
        {
            switch (panel.Name)
            {
                case "panel1":
                    if (panel.Height == 125)
                        panel.Height = 25;
                    else
                        panel.Height = 125;
                    break;

                case "panel9":
                    if (panel.Height == 180)
                        panel.Height = 25;
                    else
                        panel.Height = 180;
                    break;

                case "panel8":
                    if (panel.Height == 121)
                        panel.Height = 25;
                    else
                        panel.Height = 121;
                    break;

                case "panel10":
                    if (panel.Height == 123)
                        panel.Height = 25;
                    else
                        panel.Height = 123;
                    break;

                case "panel2":
                    if (panel.Height == 93)
                        panel.Height = 25;
                    else
                        panel.Height = 93;
                    break;

                case "panel4":
                    if (panel.Height == 25)
                    {
                        panel.AutoSize = true;
                        panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    }
                    else
                    {
                        panel.AutoSize = false;
                        panel.AutoSizeMode = AutoSizeMode.GrowOnly;
                        panel.Height = 25;
                    }                        
                    break;

                case "panel5":
                    if (panel.Height == 25)
                    {
                        panel.AutoSize = true;
                        panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    }
                    else
                    {
                        panel.AutoSize = false;
                        panel.AutoSizeMode = AutoSizeMode.GrowOnly;
                        panel.Height = 25;
                    }
                    break;

                case "panel3":
                    if (panel.Height == 124)
                        panel.Height = 25;
                    else
                        panel.Height = 124;
                    break;

                case "panel6":
                    if (panel.Height == 167)
                        panel.Height = 25;
                    else
                        panel.Height = 167;
                    break;

                case "panel7":
                    if (panel.Height == 25)
                    {
                        panel.AutoSize = true;
                        panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    }
                    else
                    {
                        panel.AutoSize = false;
                        panel.AutoSizeMode = AutoSizeMode.GrowOnly;
                        panel.Height = 25;
                    }
                    break;

                case "panel23":
                    if (panel.Height == 167)
                        panel.Height = 25;
                    else
                        panel.Height = 167;
                    break;

                default:
                    break;
            }
        }
        private void label4_Click(object sender, EventArgs e)
        {
            ResizePanel(panel1);
        }
        private void label39_Click(object sender, EventArgs e)
        {
            ResizePanel(panel9);
        }
        private void label19_Click(object sender, EventArgs e)
        {
            ResizePanel(panel8);
        }
        private void label56_Click(object sender, EventArgs e)
        {
            ResizePanel(panel10);
        }
        private void label10_Click(object sender, EventArgs e)
        {
            ResizePanel(panel2);
        }
        private void label30_Click(object sender, EventArgs e)
        {
            ResizePanel(panel4);
        }
        private void label31_Click(object sender, EventArgs e)
        {
            ResizePanel(panel5);
        }
        private void label23_Click(object sender, EventArgs e)
        {
            ResizePanel(panel3);
        }
        private void label32_Click(object sender, EventArgs e)
        {
            ResizePanel(panel6);
        }
        private void label33_Click(object sender, EventArgs e)
        {
            ResizePanel(panel7);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox1);
        }
        void ClearSelList(ListBox L)
        {
            L.ClearSelected();
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox2);
        }
        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox5);
        }
        private void listBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox6);
        }
        private void listBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox7);
        }
        private void listBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox8);
        }
        private void listBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox9);
        }
        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox4);
        }
        private void listBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox10);
        }
        private void listBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox11);
        }
        private void listBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox12);
        }
        private void listBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox13);
        }
        private void listBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelList(listBox14);
        }
        private void label71_Click(object sender, EventArgs e)
        {
            ResizePanel(panel23);
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}
