using Domain;
using Domain.CXN;
using FormAndControls;
using ICD11;
using ICD11.ClasesICD11;
using ICD11.ServiceICD11;
using Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas;

namespace ZamenisHealth.Medicina
{
    public partial class CIE11New : Forma2
    {
        public bool selectedCIE11 = false;
        public string FormaOrigen;

        public CIE11New(string formaOrigen)
        {
            InitializeComponent();
            FormaOrigen = formaOrigen;
        }

        private async void CIE11New_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Seleccion de CIE11";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                gridZH3.CeldaHeight = true;
                gridZH3.dataGridView1.CellClick += dataGridView3_CellClick;

                ICD11ApiService servicio = new ICD11ApiService();

                // 1. Consultar clasificación
                ICD11Classification clasificacion =
                    await servicio.ObtenerClasificacionAsync();

                if (clasificacion == null)
                {
                    MessageBox.Show(
                        "No se pudo obtener la clasificación CIE-11.",
                        "Error al consultar CIE-11",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                    $"Clasificación: {clasificacion.Title.Value}\n" +
                    $"Versión: {clasificacion.ReleaseId}\n" +
                    $"Idiomas: {string.Join(", ", clasificacion.AvailableLanguages)}",
                    "Clasificación CIE-11", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }                        
        }
        private async void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MensajesGeneral MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe escribir una parte del diagnostico que busca CIE11",
                        TipoImagen = 0
                    };
                    MG.ShowDialog();
                    return;
                }

                ICD11ApiService servicio = new ICD11ApiService();

                // 2. Buscar diagnósticos
                ICD11SearchResponse resultado =
                    await servicio.BuscarDiagnosticosAsync(textBox2.Text);

                if (resultado?.DestinationEntities != null)
                {
                    List<ICD11SearchResult> L = new List<ICD11SearchResult>();

                    foreach (ICD11SearchResult item in resultado.DestinationEntities)
                    {
                        string codigo = item.Code;
                        string descripcion = LimpiarTextoHTML(item.Title);
                        string identificador = item.Id;
                        string capitulo = item.Chapter;
                        bool esTerminal = item.IsLeaf;
                        double puntuacion = item.Score;

                        L.Add(new ICD11SearchResult
                        {
                            Code = codigo,
                            Title = descripcion,
                            StemId = identificador,
                            Chapter = capitulo,
                            IsLeaf = esTerminal,
                            Score = puntuacion
                        });
                    }

                    DataTable dt = new DataTable();
                    DataColumn Codigo = dt.Columns.Add("Codigo", typeof(string));
                    DataColumn Descripcion = dt.Columns.Add("Descripcion", typeof(string));
                    DataColumn Identificador = dt.Columns.Add("Identificador", typeof(string));
                    DataColumn Capitulo = dt.Columns.Add("Capitulo", typeof(string));
                    DataColumn Terminal = dt.Columns.Add("Terminal", typeof(string));
                    DataColumn Puntuacion = dt.Columns.Add("Puntuacion", typeof(string));

                    foreach (ICD11SearchResult item in L)
                    {
                        DataRow row = dt.NewRow();
                        row["Codigo"] = item.Code;
                        row["Descripcion"] = item.Title;
                        row["Identificador"] = item.StemId;
                        row["Capitulo"] = item.Chapter;
                        row["Terminal"] = item.IsLeaf ? "Sí" : "No";
                        row["Puntuacion"] = item.Score.ToString("F2");
                        dt.Rows.Add(row);
                    }

                    gridZH3.dataGridView1.DataSource = dt;
                    gridZH3.dataGridView1.Columns["Identificador"].Visible = false;
                    gridZH3.dataGridView1.Columns["Terminal"].Visible = false;
                    gridZH3.dataGridView1.Columns["Puntuacion"].Visible = false;
                }
                else
                {
                    DataTable dt = new DataTable();
                    DataColumn Codigo = dt.Columns.Add("Codigo", typeof(string));
                    DataColumn Descripcion = dt.Columns.Add("Descripcion", typeof(string));
                    DataColumn Identificador = dt.Columns.Add("Identificador", typeof(string));
                    DataColumn Capitulo = dt.Columns.Add("Capitulo", typeof(string));
                    DataColumn Terminal = dt.Columns.Add("Terminal", typeof(string));
                    DataColumn Puntuacion = dt.Columns.Add("Puntuacion", typeof(string));
                    gridZH3.dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error al consultar CIE-11",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        async void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ICD11ApiService servicio = new ICD11ApiService();

                string Cod = gridZH3.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string des = gridZH3.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string iden = gridZH3.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string cap = gridZH3.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                string ter = gridZH3.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                string pun = gridZH3.dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

                // 3. Consultar detalle de un diagnóstico conocido
                ICD11Diagnosis diagnostico =
                    await servicio.ObtenerDiagnosticoAsync(iden);

                if (diagnostico?.Title != null)
                {
                    CIE11 c = new CIE11(diagnostico.Code, diagnostico.Title.Value, diagnostico.Definition?.Value);
                    c.ShowDialog();

                    if (selectedCIE11 == true)
                    {
                        CXN_CIE11 cie11 = new CXN_CIE11
                        { 
                            Codigo = Cod,
                            Descripcion = des,
                            Identificador = iden,
                            Capitulo = cap,
                            Terminal = ter,
                            Puntacion = pun
                        };

                        switch (FormaOrigen)
                        {
                            case "MEDGEN":
                                Historia_MedicinaGeneral f1 = Application.OpenForms.OfType<Historia_MedicinaGeneral>().LastOrDefault();
                                f1.textBox53.Text = diagnostico.Code;
                                f1.textBox54.Text = diagnostico.Title.Value;
                                f1.cie11 = cie11;
                                break;

                            case "CURACIONES":
                                Historia_NotaEnfermeria f2 = Application.OpenForms.OfType<Historia_NotaEnfermeria>().LastOrDefault();
                                f2.textBox53.Text = diagnostico.Code;
                                f2.textBox54.Text = diagnostico.Title.Value;
                                f2.cie11 = cie11;
                                break;

                            case "FISIATRIA":
                                Historia_Fisiatria f3 = Application.OpenForms.OfType<Historia_Fisiatria>().LastOrDefault();
                                f3.textBox67.Text = diagnostico.Code;
                                f3.textBox68.Text = diagnostico.Title.Value;
                                f3.cie11 = cie11;
                                break;

                            case "PSICOLOGIA":
                                Historia_Psicologia f4 = Application.OpenForms.OfType<Historia_Psicologia>().LastOrDefault();
                                f4.textBox53.Text = diagnostico.Code;
                                f4.textBox54.Text = diagnostico.Title.Value;
                                f4.cie11 = cie11;
                                break;

                            case "TFISICA":
                                Historia_TerFisica f5 = Application.OpenForms.OfType<Historia_TerFisica>().LastOrDefault();
                                f5.textBox134.Text = diagnostico.Code;
                                f5.textBox135.Text = diagnostico.Title.Value;
                                f5.cie11 = cie11;
                                break;

                            case "TOCUPACIONAL":
                                Historia_TerOcupacional f6 = Application.OpenForms.OfType<Historia_TerOcupacional>().LastOrDefault();
                                f6.textBox53.Text = diagnostico.Code;
                                f6.textBox54.Text = diagnostico.Title.Value;
                                f6.cie11 = cie11;
                                break;

                            case "EVOFIROMIALGIA":
                                Historia_Evoluciones f7 = Application.OpenForms.OfType<Historia_Evoluciones>().LastOrDefault();
                                f7.textBox53.Text = diagnostico.Code;
                                f7.textBox54.Text = diagnostico.Title.Value;
                                f7.cie11 = cie11;
                                break;

                            case "ENFJEFE":
                                Historia_JefeEnfermeria f8 = Application.OpenForms.OfType<Historia_JefeEnfermeria>().LastOrDefault();
                                f8.textBox53.Text = diagnostico.Code;
                                f8.textBox54.Text = diagnostico.Title.Value;
                                f8.cie11 = cie11;
                                break;
                        }

                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private string LimpiarTextoHTML(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = System.Text.RegularExpressions.Regex.Replace(
                texto, "<.*?>", "");

            return System.Net.WebUtility.HtmlDecode(texto);
        }
    }
}
