using Domain;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class NotasReportPrevio : ConfigForm.BaseForm
    {
        private static readonly INotasCuracion repoNotas = new MNotasCuracion();
        private static readonly IReportes repoReportes = new MReportes();

        public int Adm_Nota_Export;

        DataTable dt;
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Cantidad;
        DataColumn Graba;

        public NotasReportPrevio()
        {
            InitializeComponent();
            
            ConfigForm.MoverForma(panel2, this);
        }

        private void NotasReportPrevio_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;
               
                textBox2.ContextMenu = new ContextMenu();
                textBox3.ContextMenu = new ContextMenu();
                textBox9.ContextMenu = new ContextMenu();
                textBox10.ContextMenu = new ContextMenu();

                var getNota = repoNotas.seeNotaPrevReport(Adm_Nota_Export);
                if (getNota != null)
                {
                    textBox4.Text = getNota.Not_Epidemia.ToString();
                    textBox5.Text = getNota.Not_CaracTej.ToString();
                    textBox1.Text = getNota.Not_Nota.ToString();
                    textBox2.Text = getNota.Not_Observa.ToString();
                    textBox10.Text = getNota.Not_Recomienda.ToString();
                    textBox6.Text = Convert.ToDateTime(getNota.Not_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    textBox7.Text = getNota.Not_Cup.ToString();
                    textBox8.Text = getNota.Not_Adm.ToString();
                    textBox9.Text = getNota.Not_Adherencia.ToString();
                    textBox3.Text = getNota.Not_NotaAcla.ToString();
                    textBox11.Text = getNota.Not_Edad.ToString();
                    Cargo();
                }
                else
                {
                    CatchException();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CatchException()
        {
            try
            {
                bool consNotaJefe = repoNotas.consNotaJefe(Adm_Nota_Export);
                if (consNotaJefe != false)
                {
                    var Reporte = repoNotas.NotasJefe(Adm_Nota_Export);
                    if (Reporte == null)
                    {
                        MessageBox.Show("No se logro exportar, error desconocido",
                            "No se logro exportar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        this.Dispose();
                        this.Close();
                        return;
                    }

                    ConfigForm.GenerarReportViewer("Dataset_Notas",
               "ZamenisHealth.Reportes.RDLC_NotaEnfermeraJefe.rdlc",
               Reporte);

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Admision con inconvenientes", "Inconveniente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error - ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Dispose();
                this.Close();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Visible == false)
                {
                    MessageBox.Show("No es posible exportar la nota de curacion si no tiene cargos agregados");
                    return;
                }

                var Reporte = repoReportes.NotasMetodo(Adm_Nota_Export);
                if (Reporte == null)
                {
                    MessageBox.Show("No se logro exportar, error desconocido",
                        "No se logro exportar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                if (Preferencias.CuracionesCORE == "A" && Reporte[0].listaMedidas != null)
                {
                    Reportes.Maestro maestro = new Reportes.Maestro();
                    maestro.Universal.LocalReport.DataSources.Clear();
                    maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Notas", Reporte));
                    maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_NotasMed", Reporte[0].listaMedidas));
                    maestro.Universal.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_NotasCore.rdlc";
                    maestro.Universal.SetDisplayMode(DisplayMode.PrintLayout);
                    maestro.Universal.ZoomMode = ZoomMode.Percent;
                    maestro.Universal.ZoomPercent = 100;
                    maestro.Universal.LocalReport.EnableExternalImages = true;
                    maestro.Universal.Font = new Font("Arial", 8);
                    maestro.Universal.RefreshReport();
                    maestro.Universal.Visible = true;
                    maestro.Universal.Dock = System.Windows.Forms.DockStyle.Fill;
                    maestro.ShowDialog();
                }
                else
                {
                    ConfigForm.GenerarReportViewer("DataSet_Notas",
                                                   "ZamenisHealth.Reportes.RDLC_Notas.rdlc",
                                                   Reporte);
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                MessageBox.Show(ex.Message);
            }
        }

        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Item = dt.Columns.Add("Item", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
            Graba = dt.Columns.Add("Graba", typeof(string));
        }

        private void Cargo()
        {
            try
            {
                var getCargo = repoNotas.CargoNota(Adm_Nota_Export);
                if (getCargo != null)
                {
                    dataGridView1.Visible = true;
                    label10.Visible = false;

                    Encabezados();
                    int Contador = 1;

                    foreach (var i in getCargo)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Codigo"] = i.Car_Cod.ToString();
                        row["Item"] = i.Car_Item.ToString();
                        row["Cantidad"] = Convert.ToInt32(i.Car_Cant);
                        row["Graba"] = i.Car_Usr_Graba;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    Encabezados();
                    dataGridView1.Visible = false;
                    label10.Visible = true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Codigo"].Width = 100;
            D.Columns["Item"].Width = 600;
            D.Columns["Cantidad"].Width = 100;
            D.Columns["Graba"].Width = 150;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Item"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Graba"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Codigo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Item"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Cantidad"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Graba"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;

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
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void textBox10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
        }

        private void textBox10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
        }

        private void textBox9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
   
}