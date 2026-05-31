using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class CierresCajaPrevios : Forma
    {
        private readonly ICompañia compañia = new MCompañia();
        private readonly ICIerresCaja cierres = new MCierresCaja();
        private int Cia;
        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Consecutivo;
        DataColumn Fecha;
        DataColumn Usuario;
        DataColumn Efectivo;

        public CierresCajaPrevios()
        {
            InitializeComponent();
        }

        private void CierresCajaPrevios_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Historico Cierres";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGenerar = new ToolStripButton();
                btnGenerar = createToolButton("Generar");
                MenuLateral.Items.Add(btnGenerar);
                btnGenerar.Click += button1_Click;
                
                

                List<CXN_CIA> prestadores = compañia.getAllCompañias();

                if (prestadores != null)
                {
                    foreach (var i in prestadores)
                    {
                        comboBox3.Items.Add(i.Com_Nombre);
                    }

                    comboBox3.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cia = compañia.getPrestadorbyName(comboBox3.Text).Com_Identificador;
        }

        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Consecutivo = dt.Columns.Add("Consecutivo", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Usuario = dt.Columns.Add("Usuario", typeof(string));
            Efectivo = dt.Columns.Add("Efectivo", typeof(string));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var lista = cierres.GetPrevios(Cia, Convert.ToDateTime(dateTimePicker1.Value.Date), Convert.ToDateTime(dateTimePicker2.Value.Date));
                if (lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var report in lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Consecutivo"] = report.Consecutivo.ToString();
                        row["Fecha"] = Convert.ToDateTime(report.FechaGeneracion).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Usuario"] = report.Usuario.ToString();
                        row["Efectivo"] = "$ " + Convert.ToInt32(report.AEntregar).ToString("N0");

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        void Estilos()
        {
            try
            {
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ScrollBars = ScrollBars.Both;

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["Consecutivo"].Width = 200;
                dataGridView1.Columns["Fecha"].Width = 110;
                dataGridView1.Columns["Usuario"].Width = 120;
                dataGridView1.Columns["Efectivo"].Width = 130;

                dataGridView1.Font = new Font("Arial", 11);

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                dataGridView1.Columns["Consecutivo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Usuario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Efectivo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["Consecutivo"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Usuario"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Efectivo"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridView1.Columns["POS"].Visible = false;

                foreach (DataGridViewRow row in dataGridView1.Rows)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string cons = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                List<CXN_REPORTECAJA> getReport = cierres.GetReport(cons);
                if (getReport != null)
                {
                    ConfigForm.GenerarReportViewer("DataSet_CierreCaja",
                        "ZamenisHealth.Reportes.CierreCaja.rdlc",
                        getReport);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se logro exportar el reporte";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
    }
}
