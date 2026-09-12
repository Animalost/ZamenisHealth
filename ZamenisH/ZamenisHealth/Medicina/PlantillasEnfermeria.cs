using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class PlantillasEnfermeria : Forma
    {
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IPlantilla repoPlantilla = new MPlantilla();
        public ToolStripButton btnGrabar = new ToolStripButton();

        public string Tipo_Plant;
        public PlantillasEnfermeria()
        {
            InitializeComponent();       
        }
        private void PlantillasEnfermeria_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Plantilla de Enfermeria";
                LogoMain.Image = Properties.Resources.Splash;

                btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += toolStripButton1_Click;

                
                var ValidaUsuario = repoBodegas.EsProfesional("Enfermero", Comunes.Contenedor.UsuarioLogueado);
                if (ValidaUsuario != true)
                {
                    MessageBox.Show("Su usuario no es tipo enfermero, no puede continuar",
                        "Acceso Denegado!!!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.Dispose();
                    this.Close();
                }



                var getPlantillas = repoPlantilla.getPlantillas(Comunes.Contenedor.UsuarioLogueado);
                if (getPlantillas != null)
                {
                    DataTable dt = new DataTable();
                    DataColumn Nota = dt.Columns.Add("Nota", typeof(string));
                    DataColumn Recomendacion = dt.Columns.Add("Recomendacion", typeof(string));
                    DataColumn Observacion = dt.Columns.Add("Observacion", typeof(string));

                    foreach (var i in getPlantillas)
                    {
                        DataRow row = dt.NewRow();

                        row["Nota"] = i.Pla_Nota.ToString();
                        row["Observacion"] = i.Pla_Observa.ToString();
                        row["Recomendacion"] = i.Pla_Recomienda.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns["Nota"].Width = 200;
                    dataGridView1.Columns["Observacion"].Width = 200;
                    dataGridView1.Columns["Recomendacion"].Width = 2000;
                    dataGridView1.Columns["Nota"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Observacion"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Recomendacion"].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                else
                {
                    DataTable dt = new DataTable();
                    DataColumn Nota = dt.Columns.Add("Nota", typeof(string));
                    DataColumn Recomendacion = dt.Columns.Add("Recomendacion", typeof(string));
                    DataColumn Observacion = dt.Columns.Add("Observacion", typeof(string));
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Tipo_Plant == "Notas")
                {
                    HistoriasClinicas.Historia_NotaEnfermeria f2 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                    f2.textBox14.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    f2.textBox15.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    f2.textBox16.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                    this.Dispose();
                    this.Close();
                    return;
                }
                if (Tipo_Plant == "NotasCore")
                {
                    HistoriasClinicas.NotaEnfermeria.NotaCuracion f2 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                    f2.textBox14.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    f2.textBox15.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    f2.textBox16.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                    this.Dispose();
                    this.Close();
                    return;
                }

                /* if (Tipo_Plant == "RegrabaNotas")
                 {
                     Historia_Notas_Regrabacion.Regraba_Nota_Pla = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                     Historia_Notas_Regrabacion.Regraba_Nota_Obs = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                     Historia_Notas_Regrabacion.Regraba_Nota_Rec = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                     Historia_Notas_Regrabacion.Regraba_Comprobador_Plantilla = "1";
                     this.Dispose();
                     this.Close();
                     return;
                 }*/
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar el campo de Nota de Curacion"); return; }

                CXN_PLANTILLA P = new CXN_PLANTILLA
                {
                    Pla_Usr = Comunes.Contenedor.UsuarioLogueado,
                    Pla_Nota = textBox1.Text,
                    Pla_Observa = textBox2.Text,
                    Pla_Recomienda = textBox3.Text
                };

                bool savePlan = repoPlantilla.savePlantillas(P);
                if (savePlan != true)
                {
                    MessageBox.Show("No se logro grabar plantilla", "Error desconocido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Grabado");

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
