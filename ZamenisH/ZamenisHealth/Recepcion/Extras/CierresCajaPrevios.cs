using Domain;
using Domain.CXN;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
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
        DataColumn Estado;

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
            Efectivo = dt.Columns.Add("Total", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var lista = cierres.GetPrevios(Cia, Convert.ToDateTime(dateTimePicker1.Value.Date), Convert.ToDateTime(dateTimePicker2.Value.Date));
                if (lista != null)
                {
                    lista = lista.Where(x => x.Tipo != "EGRESOS").ToList();

                    lista = lista
                        .GroupBy(x => x.Consecutivo)
                        .Select(g => new CXN_REPORTECAJA2
                        {
                            Consecutivo = g.Key,
                            Valor = g.Sum(x => x.Valor),
                            Usuario = g.First().Usuario,
                            Generacion = g.First().Generacion,
                            Compañia = g.First().Compañia,
                            Estado = g.First().Estado,
                        })
                        .ToList();

                    Encabezados();

                    int Contador = 1;

                    foreach (var report in lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Consecutivo"] = report.Consecutivo.ToString();
                        row["Fecha"] = Convert.ToDateTime(report.Generacion).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Usuario"] = report.Usuario.ToString();
                        row["Total"] = "$ " + Convert.ToInt32(report.Valor).ToString("N0");
                        row["Estado"] = report.Estado == "A" ? "ANULADO" : "VIGENTE";

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

                dataGridView1.Columns["Consecutivo"].Width = 100;
                dataGridView1.Columns["Fecha"].Width = 110;
                dataGridView1.Columns["Usuario"].Width = 120;
                dataGridView1.Columns["Total"].Width = 130;
                dataGridView1.Columns["Estado"].Width = 130;

                dataGridView1.Font = new Font("Arial", 11);

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                dataGridView1.Columns["Consecutivo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Usuario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Total"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["Consecutivo"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Usuario"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Total"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Estado"].SortMode = DataGridViewColumnSortMode.NotSortable;

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

                List<CXN_REPORTECAJA2> getReport = cierres.GetReport2(cons.ToString(), Cia);
                if (getReport != null)
                {
                    int TotalIngreso = getReport.Where(x => x.Tipo != "EGRESOS" && x.Clase == "Efectivo").Sum(x => x.Valor);
                    int TotalEgreso = getReport.Where(x => x.Tipo == "EGRESOS").Sum(x => x.Valor);
                    int Entregar = TotalIngreso - TotalEgreso;

                    foreach (var i in getReport)
                    {
                        i.Estado = i.Estado == "H" ? "VIGENTE" : "ANULADO";
                        i.IdRC = Entregar;
                        i.ObservacionGeneral = getReport[0].Observacion;
                    }

                    List<CXN_REPORTECAJA2> Caja = getReport.Where(x => x.Tipo == "CAJA").ToList();
                    List<CXN_REPORTECAJA2> Ventas = getReport.Where(x => x.Tipo == "VENTAS").ToList();
                    List<CXN_REPORTECAJA2> Particulares = getReport.Where(x => x.Tipo == "PARTICULARES").ToList();
                    List<CXN_REPORTECAJA2> Egresos = getReport.Where(x => x.Tipo == "EGRESOS").ToList();

                    Reportes.Maestro maestro = new Reportes.Maestro();
                    maestro.Universal.LocalReport.DataSources.Clear();

                    maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresEncabezado", getReport));
                    maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresCaja", Caja));
                    maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresVentas", Ventas));
                    maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresParticulares", Particulares));
                    maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresEgresos", Egresos));

                    maestro.Universal.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.CierreCaja2.rdlc";
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
