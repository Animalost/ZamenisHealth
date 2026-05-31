using APIFhir.VisorConsultas;
using FormAndControls;
using Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FrontFHIR.VisorZamenis
{
    public partial class VerRDAPaciente : Forma2
    {
        private readonly IConsultaPaciente rConsultaPaciente;

        string IdCompositionPaciente;
        DateTime FechaIdCompositionPaciente;
        private MensajesGeneral MG;
        private string json;

        DataTable dt;
        DataColumn POS;
        DataColumn Vacunadt;
        DataColumn Fechadt;
        DataColumn Fabricantedt;
        DataColumn Lotedt;
        DataColumn Institucióndt;
        DataColumn Profesionaldt;
        DataColumn Dosisdt;

        public VerRDAPaciente(string idCompositionPaciente, DateTime fechaIdCompositionPaciente, string r)
        {
            InitializeComponent();
            IdCompositionPaciente = idCompositionPaciente;
            FechaIdCompositionPaciente = fechaIdCompositionPaciente;
            json = r;
            rConsultaPaciente = new ConsultaPaciente();
        }

        public class antecedentesFamiliaresGlobal
        {
            public string Parentesco { get; set; }
            public string Enfermedad { get; set; }
        }
        public class antecedentesAlergicos
        {
            public string Tipo { get; set; }
            public string Alergia { get; set; }
        }
        public class InmunizationClass
        {
            public string Vacuna { get; set;  }
            public string Fecha { get; set; }
            public string Fabricante { get; set; }
            public string Lote { get; set; }
            public string Institución { get; set; }
            public string Profesional { get; set; }
            public string Dosis { get; set; }
        }
        private void VerRDAPaciente_Load(object sender, EventArgs e)
        {
            Titulo.Text = $"Antecedentes Clinicos {Convert.ToDateTime(FechaIdCompositionPaciente).ToString("yyyy-MM-dd")}";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarAntecedentes();
            CargarInmunizacion();

            dataGridView1.Click += DataGridView1_Click;
        }
        private void DataGridView1_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
        async void CargarInmunizacion()
        {
            try
            {               
                var resJson = await rConsultaPaciente.GetData(json, 10, "Inmunization", IdCompositionPaciente);
                if (resJson.Est == "OK")
                {
                    List<InmunizationClass> listado = new List<InmunizationClass>();

                    var doc = JsonDocument.Parse(resJson.Resp);
                    var root = doc.RootElement;

                    // Validar si es bundle
                    if (root.TryGetProperty("entry", out var entries))
                    {
                        foreach (var entry in entries.EnumerateArray())
                        {
                            if (!entry.TryGetProperty("resource", out var resource))
                                continue;

                            // FILTRO CLAVE
                            string resourceType = DecompiladorRespuesta.GetStringSafe(resource, "resourceType");

                            if (resourceType != "Immunization")
                                continue;

                            InmunizationClass inmunizacion = new InmunizationClass();

                            // =========================
                            // DATOS SIMPLES
                            // =========================
                            if (resource.TryGetProperty("vaccineCode", out var vaccineCode))
                                inmunizacion.Vacuna = DecompiladorRespuesta.GetStringSafe(vaccineCode, "text");

                            inmunizacion.Fecha = DecompiladorRespuesta.GetStringSafe(resource, "occurrenceDateTime");
                            inmunizacion.Lote = DecompiladorRespuesta.GetStringSafe(resource, "lotNumber");

                            if (resource.TryGetProperty("manufacturer", out var manufacturer))
                            {
                                inmunizacion.Fabricante = DecompiladorRespuesta.GetStringSafe(manufacturer, "display");
                            }

                            // =========================
                            // PERFORMER
                            // =========================
                            if (resource.TryGetProperty("performer", out var performers))
                            {
                                foreach (var p in performers.EnumerateArray())
                                {
                                    if (p.TryGetProperty("actor", out var actor))
                                    {
                                        string tipo = DecompiladorRespuesta.GetStringSafe(actor, "type");
                                        string nombre = DecompiladorRespuesta.GetStringSafe(actor, "display");

                                        if (tipo == "Organization")
                                            inmunizacion.Institución = nombre;
                                        else if (tipo == "Practitioner")
                                            inmunizacion.Profesional = nombre;
                                    }
                                }
                            }

                            // =========================
                            // PROTOCOL
                            // =========================
                            if (resource.TryGetProperty("protocolApplied", out var protocols))
                            {
                                foreach (var pr in protocols.EnumerateArray())
                                {
                                    inmunizacion.Dosis = DecompiladorRespuesta.GetStringSafe(pr, "doseNumberString");
                                    break;
                                }
                            }

                            listado.Add(inmunizacion);
                        }
                    }
                    else if (root.TryGetProperty("resource", out var singleResource))
                    {
                        InmunizationClass inmunizacion = new InmunizationClass();

                        inmunizacion.Vacuna = DecompiladorRespuesta.GetStringSafe(singleResource.GetProperty("vaccineCode"), "text");
                        inmunizacion.Fecha = DecompiladorRespuesta.GetStringSafe(singleResource, "occurrenceDateTime");

                        listado.Add(inmunizacion);
                    }

                    Encabezados();
                    int Contador = 1;

                    foreach (var i in listado)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Vacunadt] = i.Vacuna.ToString();
                        row[Fechadt] = Convert.ToDateTime(i.Fecha).ToString("yyyy-MM-dd");
                        row[Fabricantedt] = i.Fabricante.ToString();
                        row[Lotedt] = i.Lote.ToString();
                        row[Institucióndt] = i.Institución.ToString();
                        row[Profesionaldt] = i.Profesional.ToString();
                        row[Dosisdt] = i.Dosis.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    panel18.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            D.Columns["Vacuna"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fabricante"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Lote"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Institución"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Dosis"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

            dataGridView1.ClearSelection();
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Vacunadt = dt.Columns.Add("Vacuna", typeof(string));
            Fechadt = dt.Columns.Add("Fecha", typeof(string));
            Fabricantedt = dt.Columns.Add("Fabricante", typeof(string));
            Lotedt = dt.Columns.Add("Lote", typeof(string));
            Institucióndt = dt.Columns.Add("Institución", typeof(string));
            Profesionaldt = dt.Columns.Add("Profesional", typeof(string));
            Dosisdt = dt.Columns.Add("Dosis", typeof(string));
        }
        async void CargarAntecedentes()
        {
            try
            {
                var resJson = await rConsultaPaciente.GetData("", 10, "GetIdComposition", IdCompositionPaciente);
                if (resJson.Est == "OK")
                {
                    var doc = JsonDocument.Parse(resJson.Resp);
                    var root = doc.RootElement;
                    var entries = root.GetProperty("entry");

                    string modalidad = "";
                    string modalidadDisplay = "";
                    string servicio = "";
                    string servicioDisplay = "";
                    string fechaInicio = "";
                    string fechaFin = "";

                    // listas
                    List<string> condicionesRefs = new List<string>();
                    List<string> alergiasRefs = new List<string>();
                    List<string> medicamentosRefs = new List<string>();
                    List<string> familiaresRefs = new List<string>();

                    // organization
                    string nit = "";
                    string codPrestador = "";
                    string orgNombre = "";
                    string orgCiudad = "";
                    string tipoOrg = "";
                    string naturalezaOrg = "";

                    // practitioner
                    string docTipo = "";
                    string docNumero = "";
                    string nombreProfesional = "";
                    string profesion = "";

                    // recursos
                    List<string> condiciones = new List<string>();
                    List<antecedentesAlergicos> alergias = new List<antecedentesAlergicos>();
                    List<string> medicamentos = new List<string>();
                    List<antecedentesFamiliaresGlobal> antecedentesFam = new List<antecedentesFamiliaresGlobal>();      

                    foreach (var entry in entries.EnumerateArray())
                    {
                        if (!entry.TryGetProperty("resource", out JsonElement resource))
                            continue;

                        string tipo = resource.GetProperty("resourceType").GetString();

                        //-----------------------------------
                        // 1. COMPOSITION
                        //-----------------------------------
                        if (tipo == "Composition")
                        {
                            if (resource.TryGetProperty("event", out var eventos))
                            {
                                foreach (var ev in eventos.EnumerateArray())
                                {
                                    // códigos
                                    if (ev.TryGetProperty("code", out var codes))
                                    {
                                        int i = 0;
                                        foreach (var c in codes.EnumerateArray())
                                        {
                                            var coding = c.GetProperty("coding")[0];

                                            string code = coding.GetProperty("code").GetString();
                                            string display = coding.GetProperty("display").GetString();

                                            if (i == 0)
                                            {
                                                modalidad = code ?? "";
                                                modalidadDisplay = display ?? "";
                                            }
                                            else if (i == 1)
                                            {
                                                servicio = code ?? "";
                                                servicioDisplay = display ?? "";
                                            }

                                            i++;
                                        }
                                    }

                                    // fechas
                                    if (ev.TryGetProperty("period", out var period))
                                    {
                                        fechaInicio = period.TryGetProperty("start", out var s) ? s.GetString() ?? "" : "";
                                        fechaFin = period.TryGetProperty("end", out var e) ? e.GetString() ?? "" : "";
                                    }
                                }
                            }

                            // secciones
                            if (resource.TryGetProperty("section", out var sections))
                            {
                                foreach (var sec in sections.EnumerateArray())
                                {
                                    string title = sec.GetProperty("title").GetString();

                                    if (!sec.TryGetProperty("entry", out var refs))
                                        continue;

                                    foreach (var r in refs.EnumerateArray())
                                    {
                                        string reference = r.GetProperty("reference").GetString();

                                        if (title.Contains("diagnósticos"))
                                            condicionesRefs.Add(reference);

                                        if (title.Contains("alergias"))
                                            alergiasRefs.Add(reference);

                                        if (title.Contains("medicamentos"))
                                            medicamentosRefs.Add(reference);

                                        if (title.Contains("familiares"))
                                            familiaresRefs.Add(reference);
                                    }
                                }
                            }
                        }

                        //-----------------------------------
                        // 2. ORGANIZATION
                        //-----------------------------------
                        else if (tipo == "Organization")
                        {
                            orgNombre = resource.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";

                            if (resource.TryGetProperty("identifier", out var ids))
                            {
                                foreach (var id in ids.EnumerateArray())
                                {
                                    var coding = id.GetProperty("type").GetProperty("coding");

                                    foreach (var c in coding.EnumerateArray())
                                    {
                                        string code = c.GetProperty("code").GetString();

                                        if (code == "NIT")
                                            nit = id.GetProperty("value").GetString() ?? "";

                                        if (code == "CodigoPrestador")
                                            codPrestador = id.GetProperty("value").GetString() ?? "";
                                    }
                                }
                            }

                            if (resource.TryGetProperty("type", out var types))
                            {
                                foreach (var t in types.EnumerateArray())
                                {
                                    var coding = t.GetProperty("coding")[0];

                                    string code = coding.GetProperty("code").GetString();
                                    string display = coding.GetProperty("display").GetString();

                                    if (code == "IPS") tipoOrg = display ?? "";
                                    if (code == "PRIV") naturalezaOrg = display ?? "";
                                }
                            }

                            if (resource.TryGetProperty("address", out var addr))
                            {
                                orgCiudad = addr[0].TryGetProperty("city", out var c) ? c.GetString() ?? "" : "";
                            }
                        }

                        //-----------------------------------
                        // 3. PRACTITIONER
                        //-----------------------------------
                        else if (tipo == "Practitioner")
                        {
                            if (resource.TryGetProperty("identifier", out var ids))
                            {
                                var id = ids[0];
                                docNumero = id.GetProperty("value").GetString() ?? "";

                                var coding = id.GetProperty("type").GetProperty("coding")[1];
                                docTipo = coding.GetProperty("display").GetString() ?? "";
                            }

                            if (resource.TryGetProperty("name", out var names))
                            {
                                var name = names[0];

                                string family = name.GetProperty("family").GetString() ?? "";
                                string given = string.Join(" ", name.GetProperty("given").EnumerateArray().Select(x => x.GetString()));

                                nombreProfesional = $"{given} {family}";
                            }

                            if (resource.TryGetProperty("qualification", out var q))
                            {
                                var coding = q[0].GetProperty("code").GetProperty("coding")[0];
                                profesion = coding.GetProperty("display").GetString() ?? "";
                            }
                        }

                        //-----------------------------------
                        // 4. CONDITION
                        //-----------------------------------
                        else if (tipo == "Condition")
                        {
                            string texto = "";

                            if (resource.TryGetProperty("code", out var code))
                            {
                                if (code.TryGetProperty("text", out var t))
                                    texto = t.GetString() ?? "";
                            }

                            if (!string.IsNullOrEmpty(texto))
                                condiciones.Add(texto);
                        }

                        //-----------------------------------
                        // 5. ALLERGY
                        //-----------------------------------
                        else if (tipo == "AllergyIntolerance")
                        {
                            string tipoAlergia = "";
                            string texto = "";

                            if (resource.TryGetProperty("code", out var code))
                            {
                                // coding[0].display
                                if (code.TryGetProperty("coding", out var codings) && codings.ValueKind == JsonValueKind.Array && codings.GetArrayLength() > 0)
                                {
                                    var coding = codings[0];

                                    if (coding.TryGetProperty("display", out var d))
                                        tipoAlergia = d.GetString() ?? "";
                                }

                                // code.text
                                if (code.TryGetProperty("text", out var t))
                                    texto = t.GetString() ?? "";
                            }

                            // Solo agregar si hay algo útil
                            if (!string.IsNullOrEmpty(tipoAlergia) || !string.IsNullOrEmpty(texto))
                            {
                                alergias.Add(new antecedentesAlergicos 
                                { 
                                    Tipo = tipoAlergia,
                                    Alergia = texto,
                                });
                            }
                        }

                        //-----------------------------------
                        // 6. MEDICATION
                        //-----------------------------------
                        else if (tipo == "MedicationStatement")
                        {
                            if (resource.TryGetProperty("medicationCodeableConcept", out var medConcept))
                            {
                                if (medConcept.TryGetProperty("coding", out var codings) && codings.GetArrayLength() > 0)
                                {
                                    var coding = codings[0];

                                    string display = coding.TryGetProperty("display", out var d)
                                        ? d.GetString() ?? ""
                                        : "";

                                    if (!string.IsNullOrEmpty(display))
                                        medicamentos.Add(display);
                                }
                            }
                        }

                        //-----------------------------------
                        // 7. FAMILY
                        //-----------------------------------
                        else if (tipo == "FamilyMemberHistory")
                        {
                            string parentesco = "";
                            string enfermedad = "";

                            // relationship.coding[0].display
                            if (resource.TryGetProperty("relationship", out var rel))
                            {
                                if (rel.TryGetProperty("coding", out var codings) &&
                                    codings.ValueKind == JsonValueKind.Array &&
                                    codings.GetArrayLength() > 0)
                                {
                                    var coding = codings[0];

                                    if (coding.TryGetProperty("display", out var d))
                                        parentesco = d.GetString() ?? "";
                                }
                            }

                            // condition[0].code.coding[0].display
                            if (resource.TryGetProperty("condition", out var conditions) &&
                                conditions.ValueKind == JsonValueKind.Array &&
                                conditions.GetArrayLength() > 0)
                            {
                                var cond = conditions[0];

                                if (cond.TryGetProperty("code", out var code))
                                {
                                    if (code.TryGetProperty("coding", out var codings) &&
                                        codings.ValueKind == JsonValueKind.Array &&
                                        codings.GetArrayLength() > 0)
                                    {
                                        var coding = codings[0];

                                        if (coding.TryGetProperty("display", out var d))
                                            enfermedad = d.GetString() ?? "";
                                    }
                                }
                            }

                            // agregar solo si hay algo
                            if (!string.IsNullOrEmpty(parentesco) || !string.IsNullOrEmpty(enfermedad))
                                antecedentesFam.Add(new antecedentesFamiliaresGlobal 
                                { 
                                    Parentesco = parentesco,
                                    Enfermedad = enfermedad
                                });
                        }
                    }

                    //Composition
                    label1.Text = modalidad + " - " + modalidadDisplay;
                    label2.Text = servicio + " - " + servicioDisplay;
                    label5.Text = !string.IsNullOrEmpty(fechaInicio) ? Convert.ToDateTime(fechaInicio).ToString("yyyy-MM-dd") : "";
                    label8.Text = !string.IsNullOrEmpty(fechaFin) ? Convert.ToDateTime(fechaFin).ToString("yyyy-MM-dd") : "";

                    //Organization
                    label11.Text = orgNombre.ToUpper().Trim();
                    label15.Text = nit.ToUpper().Trim();
                    label13.Text = codPrestador.ToUpper().Trim();
                    label19.Text = orgCiudad.ToUpper().Trim();
                    label17.Text = tipoOrg.ToUpper().Trim();
                    label21.Text = naturalezaOrg.ToUpper().Trim();

                    //Practitioner
                    label24.Text = nombreProfesional.ToUpper().Trim();
                    label26.Text = docTipo.ToUpper().Trim() + " " + docNumero.ToUpper().Trim();
                    label28.Text = profesion.ToUpper().Trim();

                    // Listas
                    //Antecedentes Patologicos
                    if (condiciones.Count > 0)
                    {
                        listBox1.DataSource = condiciones;
                    }
                    else
                    {
                        panel4.Visible = false;
                    }                        

                    //Alergias
                    if (alergias.Count > 0)
                    {
                        List<antecedentesAlergicos> Medicamento = alergias.Where(x => x.Tipo == "Medicamento").ToList();
                        if (Medicamento.Count > 0)
                        {
                            foreach (var i in Medicamento)
                            {
                                listBox2.Items.Add(i.Alergia);
                            }
                        }
                        else
                        {
                            panel8.Visible = false;
                        }

                        List<antecedentesAlergicos> Alimento = alergias.Where(x => x.Tipo == "Alimento").ToList();
                        if (Alimento.Count > 0)
                        {
                            foreach (var i in Alimento)
                            {
                                listBox8.Items.Add(i.Alergia);
                            }
                        }
                        else
                        {
                            panel9.Visible = false;
                        }
                        
                        List<antecedentesAlergicos> SustanciaAmbiental = alergias.Where(x => x.Tipo == "Sustancia del ambiente").ToList();
                        if (SustanciaAmbiental.Count > 0)
                        {
                            foreach (var i in SustanciaAmbiental)
                            {
                                listBox10.Items.Add(i.Alergia);
                            }
                        }
                        else
                        {
                            panel10.Visible = false;
                        }
                        
                        List<antecedentesAlergicos> SustanciaPiel = alergias.Where(x => x.Tipo == "Sustancia que entran en contacto con la piel").ToList();
                        if (SustanciaPiel.Count > 0) 
                        {
                            foreach (var i in SustanciaPiel)
                            {
                                listBox9.Items.Add(i.Alergia);
                            }
                        }
                        else
                        {
                            panel11.Visible = false;
                        }
                                                
                        List<antecedentesAlergicos> Picadura = alergias.Where(x => x.Tipo == "Picadura de insectos").ToList();
                        if (Picadura.Count > 0)
                        {
                            foreach (var i in Picadura)
                            {
                                listBox12.Items.Add(i.Alergia);
                            }
                        }
                        else
                        {
                            panel12.Visible = false;
                        }
                        
                        List<antecedentesAlergicos> Otra = alergias.Where(x => x.Tipo == "Otra").ToList();
                        if (Otra.Count > 0)
                        {
                            foreach (var i in Otra)
                            {
                                listBox11.Items.Add(i.Alergia);
                            }
                        }
                        else
                        {
                            panel13.Visible = false;
                        }
                    }
                    else
                    {
                        panel5.Visible = false;
                    }
                    
                    //Medicamentos
                    if (medicamentos.Count > 0)
                    {
                        listBox3.DataSource = medicamentos;
                    }
                    else
                    {
                        panel6.Visible = false;
                    }                        

                    //Antecedentes Familiares
                    if (antecedentesFam.Count > 0)
                    {
                        List<antecedentesFamiliaresGlobal> AntPadres = antecedentesFam.Where(x => x.Parentesco == "Padres").ToList();
                        if (AntPadres.Count > 0)
                        {
                            foreach (var i in AntPadres)
                            {
                                listBox4.Items.Add(i.Enfermedad);
                            }
                        }
                        else
                        {
                            panel14.Visible = false;
                        }                                                

                        List<antecedentesFamiliaresGlobal> AntHermanos = antecedentesFam.Where(x => x.Parentesco == "Hermanos").ToList();
                        if (AntHermanos.Count > 0)
                        {
                            foreach (var i in AntHermanos)
                            {
                                listBox5.Items.Add(i.Enfermedad);
                            }
                        }
                        else
                        {
                            panel15.Visible = false;
                        }                        

                        List<antecedentesFamiliaresGlobal> AntAbuelos = antecedentesFam.Where(x => x.Parentesco == "Abuelos").ToList();
                        if (AntAbuelos.Count > 0)
                        {
                            foreach (var i in AntAbuelos)
                            {
                                listBox7.Items.Add(i.Enfermedad);
                            }
                        }
                        else
                        {
                            panel16.Visible = false;
                        }                        

                        List<antecedentesFamiliaresGlobal> AntTios = antecedentesFam.Where(x => x.Parentesco == "Tíos").ToList();
                        if (AntTios.Count > 0)
                        {
                            foreach (var i in AntTios)
                            {
                                listBox6.Items.Add(i.Enfermedad);
                            }
                        }
                        else
                        {
                            panel17.Visible = false;
                        }
                    }                        
                    else
                    {
                        panel7.Visible = false;
                    }
             
                    listBox1.ClearSelected();
                    listBox2.ClearSelected();
                    listBox3.ClearSelected();
                    listBox4.ClearSelected();
                    listBox5.ClearSelected();
                    listBox6.ClearSelected();
                    listBox7.ClearSelected();
                    listBox8.ClearSelected();
                    listBox9.ClearSelected();
                    listBox10.ClearSelected();
                    listBox11.ClearSelected();
                    listBox12.ClearSelected();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se pudieron cargar los antecedentes del paciente",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.ClearSelected();
            listBox2.ClearSelected();
            listBox3.ClearSelected();
            listBox4.ClearSelected();
            listBox5.ClearSelected();
            listBox6.ClearSelected();
            listBox7.ClearSelected();
            listBox8.ClearSelected();
            listBox9.ClearSelected();
            listBox10.ClearSelected();
            listBox11.ClearSelected();
            listBox12.ClearSelected();
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

                case "panel2":
                    if (panel.Height == 152)
                        panel.Height = 25;
                    else
                        panel.Height = 152;
                    break;

                case "panel3":
                    if (panel.Height == 124)
                        panel.Height = 25;
                    else
                        panel.Height = 124;
                    break;

                case "panel4":
                    if (panel.Height == 167)
                        panel.Height = 25;
                    else
                        panel.Height = 167;
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

                case "panel6":
                    if (panel.Height == 167)
                        panel.Height = 25;
                    else
                        panel.Height = 167;
                    break;

                case "panel18":
                    if (panel.Height == 221)
                        panel.Height = 25;
                    else
                        panel.Height = 221;
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

                default:
                    break;
            }

            listBox1.ClearSelected();
            listBox2.ClearSelected();
            listBox3.ClearSelected();
            listBox4.ClearSelected();
            listBox5.ClearSelected();
            listBox6.ClearSelected();
            listBox7.ClearSelected();
        }
        private void label4_Click(object sender, EventArgs e)
        {
            ResizePanel(panel1);
        }
        private void label10_Click(object sender, EventArgs e)
        {
            ResizePanel(panel2);
        }
        private void label23_Click(object sender, EventArgs e)
        {
            ResizePanel(panel3);
        }
        private void label30_Click(object sender, EventArgs e)
        {
            ResizePanel(panel4);
        }
        private void label31_Click(object sender, EventArgs e)
        {
            ResizePanel(panel5);
        }
        private void label32_Click(object sender, EventArgs e)
        {
            ResizePanel(panel6);
        }
        private void label33_Click(object sender, EventArgs e)
        {
            ResizePanel(panel7);
        }
        private void label44_Click(object sender, EventArgs e)
        {
            ResizePanel(panel18);
        }
    }
}
