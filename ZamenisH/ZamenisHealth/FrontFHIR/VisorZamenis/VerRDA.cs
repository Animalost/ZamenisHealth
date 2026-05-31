using APIFhir.Controlador;
using APIFhir.Servicio;
using APIFhir.VisorConsultas;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.FrontFHIR.ClasesRDAs;
using static ZamenisHealth.FrontFHIR.ClasesRDAs.ExtractorRDAPaciente;

namespace ZamenisHealth.FrontFHIR.VisorZamenis
{
    public partial class VerRDA : Forma2
    {
        private readonly IPacientes rPacientes;
        private readonly IConsultaPaciente rConsultaPaciente;
        private readonly CreateToken repoCrearToken;
        private readonly IZonas zonas;

        private MensajesGeneral MG;
        private string TID, NID, AbrevDoc;
        private bool ConsultaAbierta;

        public VerRDA(bool consultaAbierta, string tid, string nid)
        {
            InitializeComponent();
            rPacientes = new MPacientes();
            rConsultaPaciente = new ConsultaPaciente();
            repoCrearToken = new EndPoint_Token();            
            zonas = new MZonas();

            ConsultaAbierta = consultaAbierta;
            TID = tid;
            NID = nid;
        }
        public VerRDA()
        {
            InitializeComponent();
            rPacientes = new MPacientes();
            rConsultaPaciente = new ConsultaPaciente();
            repoCrearToken = new EndPoint_Token();            
            zonas = new MZonas();

            ConsultaAbierta = true;
        }
        private async void VerRDA_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Visor IHCE - Ministerio de Salud y Proteccion Social";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            Titulo.AutoSize = true;

            CargarDocs();

            if (this.ConsultaAbierta == false)
            {
                comboBox1.Text = this.TID;
                textBox1.Text = this.NID;

                comboBox1.Enabled = false;
                textBox1.Enabled = false;
            }

            dataGridView1.Click += DataGridView1_Click;
            dataGridView2.Click += DataGridView2_Click;

            if (ConsultaAbierta == false)
            {
                comboBox1.Visible = false;
                textBox1.Visible = false;
                btnBuscar.Visible = false;

                AbrevDoc = rPacientes.getTipoDoc(comboBox1.Text);
                if (AbrevDoc == null)
                {
                    MG = new MensajesGeneral
                    {
                        Mensaje = "Tipo documento paciente no valido",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    dataGridView1.DataSource = null;
                    dataGridView2.DataSource = null;

                    var f = await repoCrearToken.ObtenerTokenIHCE(10);
                    await DatosPaciente(AbrevDoc);
                }                
            }
        }
        private void DataGridView1_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
        private void DataGridView2_Click(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
        }
        void CargarDocs()
        {
            List<string> docs = rPacientes.ListaDocs();
            foreach (string doc in docs)
            {
                comboBox1.Items.Add(doc);
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || string.IsNullOrEmpty(textBox1.Text))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar los dos campos",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    AbrevDoc = rPacientes.getTipoDoc(comboBox1.Text);
                    if (AbrevDoc == null)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Tipo documento paciente no valido",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                    }
                    else
                    {
                        dataGridView1.DataSource = null;
                        dataGridView2.DataSource = null;

                        var f = await repoCrearToken.ObtenerTokenIHCE(10);
                        await DatosPaciente(AbrevDoc);
                    }
                }
            }
            catch (Exception ex)
            {
                ClearData();
                dataGridView1.DataSource = null;
                dataGridView2.DataSource = null;
                Console.WriteLine(ex.Message);
                MessageBox.Show("Ho hay resultados para el documento solicitado", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        void ClearData()
        {
            label7.Text = "";
            label8.Text = "";
            label10.Text = "";
            label14.Text = "";
            label12.Text = "";
            label18.Text = "";
            label16.Text = "";
            label22.Text = "";
            label20.Text = "";
            label26.Text = "";
            label24.Text = "";
            label30.Text = "";
            label28.Text = "";
        }
        async Task DatosPaciente(string AbrevDoc)
        {
            try
            {
                var R = new DataConsultaRDA
                {
                    resourceType = "Parameters",
                    parameter = new List<Parameter>
                    {
                        new Parameter
                        {
                            name = "identifier",
                            part = new List<Part>
                            {
                                new Part
                                {
                                    name = "type",
                                    valueString = AbrevDoc
                                },
                                new Part
                                {
                                    name = "value",
                                    valueString = textBox1.Text.Trim()
                                }
                            }
                        },
                        new Parameter
                        {
                            name = "humanuser",
                            valueString = $"{AbrevDoc}-{textBox1.Text.Trim()}"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(R, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var res = await rConsultaPaciente.GetData(json, 10, "DataPac", "");
                if (res.Est == "OK")
                {
                    DecompiladorRespuesta dec = new DecompiladorRespuesta();
                    var doc = JsonDocument.Parse(res.Resp);
                    var root = doc.RootElement;

                    string NombrePaciente = ExtractorRDAPaciente.NombrePaciente(root);
                    (string Tipodocumento, string NumDocumento) DocPac = ExtractorRDAPaciente.IdentificacionPaciente(root);
                    Nacionalidad NationalityData = ExtractorRDAPaciente.NationalityData(root);
                    string SexBiologico = ExtractorRDAPaciente.SexoBiologico(root);
                    (string FNto, string HNto) birthData = ExtractorRDAPaciente.BirthData(root);
                    Residencia ResidenceData = ExtractorRDAPaciente.ResidenceData(root);

                    label7.Text = NombrePaciente.ToUpper().Trim();
                    label8.Text = DocPac.Tipodocumento.ToUpper().Trim() + " " + DocPac.NumDocumento.ToUpper().Trim();
                    label10.Text = NationalityData.discapacidad.ToUpper().Trim();
                    label14.Text = NationalityData.nacionalidadPais.ToUpper().Trim();
                    label12.Text = NationalityData.nacionalidadCod.ToUpper().Trim();
                    label18.Text = SexBiologico.ToUpper().Trim();
                    label16.Text = NationalityData.identidadGenero.ToUpper().Trim();
                    label22.Text = Convert.ToDateTime(birthData.FNto).ToString("yyyy-MM-dd");
                    label20.Text = Convert.ToDateTime(birthData.HNto).ToString("HH:mm:ss tt");
                    label26.Text = ResidenceData.pais.ToUpper().Trim();
                    label24.Text = "170";
                    label30.Text = ResidenceData.municipio.ToUpper().Trim();
                    label28.Text = ResidenceData.zona.ToUpper().Trim();

                    if (!string.IsNullOrEmpty(label30.Text) && label30.Text.Length == 5)
                    {
                        string departamento = label30.Text.Substring(0, 2); // "11"
                        string municipio = label30.Text.Substring(2, 3);    // "001"

                        string departamentoN = zonas.DepartamentoNombre(departamento);
                        string municipioN = zonas.MunicipioNombre(municipio, departamento);

                        label30.Text = $"{departamentoN.ToUpper().Trim()} - {municipioN.ToUpper().Trim()} - ({ResidenceData.municipio.ToUpper().Trim()})";
                    }

                    await ListaComposition(AbrevDoc);
                }
                else
                {
                    ClearData();
                    dataGridView1.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                ClearData();
                dataGridView1.DataSource = null;
                Console.WriteLine("DATOS PACIENTE: -->" + ex.Message);
                MessageBox.Show("Ho hay resultados para el documento solicitado", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        async Task ListaComposition(string AbrevDoc)
        {
            try
            {
                var R = new DataConsultaRDA
                {
                    resourceType = "Parameters",
                    parameter = new List<Parameter>
                    {
                        new Parameter
                        {
                            name = "identifier",
                            part = new List<Part>
                            {
                                new Part
                                {
                                    name = "type",
                                    valueString = AbrevDoc
                                },
                                new Part
                                {
                                    name = "value",
                                    valueString = textBox1.Text.Trim()
                                }
                            }
                        },
                        new Parameter
                        {
                            name = "humanuser",
                            valueString = $"{AbrevDoc}-{textBox1.Text.Trim()}"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(R, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                //Lista RDA Paciente
                var res = await rConsultaPaciente.GetData(json, 10, "ListComposition", "");
                if (res.Est == "OK")
                {
                    DecompiladorRespuesta dec = new DecompiladorRespuesta();
                    var tabla = dec.ExtraerDatosBundle(res.Resp);

                    dataGridView1.DataSource = tabla;
                    Estilos(dataGridView1, tabla);

                    dataGridView1.BorderStyle = BorderStyle.None;
                }
                else
                {
                    dataGridView1.BorderStyle = BorderStyle.FixedSingle;
                    dataGridView1.DataSource = null;
                }

                //Lista RDA Encuentros Clinicos
                var res2 = await rConsultaPaciente.GetData(json, 10, "ListEncounters", "");
                if (res2.Est == "OK")
                {
                    DecompiladorRespuesta dec = new DecompiladorRespuesta();
                    var tabla = await dec.ExtraerDatosBundleEncuentrosClinicos(res2.Resp);

                    dataGridView2.DataSource = tabla;
                    Estilos2(dataGridView2, tabla);

                    dataGridView2.BorderStyle = BorderStyle.None;
                }
                else
                {
                    dataGridView2.BorderStyle = BorderStyle.FixedSingle;
                    dataGridView2.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LISTA COMPOSITION: -->" + ex.Message);
            }
        }
        void Estilos2(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;
           
            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Fecha Atencion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Atencion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["ResourceId"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["ResourceFecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Encounter"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Medico"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["IPS"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Aseguradora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.Columns["Encounter"].Visible = false;
            D.Columns["Fecha"].Visible = false;
            D.Columns["ResourceId"].Visible = false;
            D.Columns["ResourceFecha"].Visible = false;
            // D.Columns["Fecha"].Visible = false;
            //D.Columns["Identificador"].Visible = false;
            D.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;
            D.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            D.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            foreach (DataGridViewRow row in D.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.AliceBlue;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }

            dataGridView2.ClearSelection();
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

            //D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Tipo de Recurso"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //D.Columns["Identificador"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha Encuentro"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.Columns["Fecha"].Visible = false;
            D.Columns["Identificador"].Visible = false;

            D.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;
            D.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            D.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            foreach (DataGridViewRow row in D.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.AliceBlue;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
            }

            dataGridView1.ClearSelection();
        }
        private async void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var R2 = new DataConsultaRDA
                {
                    resourceType = "Parameters",
                    parameter = new List<Parameter>
                    {
                        new Parameter
                        {
                            name = "identifier",
                            part = new List<Part>
                            {
                                new Part
                                {
                                    name = "type",
                                    valueString = AbrevDoc
                                },
                                new Part
                                {
                                    name = "value",
                                    valueString = textBox1.Text.Trim()
                                }
                            }
                        },
                        new Parameter
                        {
                            name = "humanuser",
                            valueString = $"{AbrevDoc}-{textBox1.Text.Trim()}"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(R2, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                string idComposition = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                DateTime FechaidComposition = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString());
                VerRDAPaciente verRDAPaciente = new VerRDAPaciente(idComposition, FechaidComposition, json);
                verRDAPaciente.ShowDialog();

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //OBTENGO EL IDCOMPISITION DEL ENCOUNTER
                string idComposition = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
                //OBTENGO LA FECHA DL IDCOMPISITION DEL ENCOUNTER
                DateTime idCompositionFecha = Convert.ToDateTime(dataGridView2.Rows[e.RowIndex].Cells[4].Value.ToString());

                VerRDAEncuentrosClinicos rdaEC = new VerRDAEncuentrosClinicos(idComposition, idCompositionFecha);
                rdaEC.ShowDialog();

                dataGridView2.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes buscarPacientes = new BuscarPacientes("FHIR");
            buscarPacientes.ShowDialog();
        }
        public void setDoc(string TID, string IDD)
        {
            comboBox1.Text = TID;
            textBox1.Text = IDD;
        }
    }
}
