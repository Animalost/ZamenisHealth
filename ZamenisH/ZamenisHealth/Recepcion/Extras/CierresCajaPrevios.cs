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
using System.Linq;
using System.Windows.Forms;
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

                gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
                gridZH1.CeldaHeight = true;

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
            gridZH1.dataGridView1.DataSource = null;
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
                gridZH1.dataGridView1.DataSource = dt;
                gridZH1.dataGridView1.Columns["POS"].Visible = false;
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
                string cons = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

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
                Console.WriteLine(ex.ToString());
            }            
        }
    }
}
