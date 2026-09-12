using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class ReportesCopias3 : Forma
    {
        private static readonly IVentas repositorioVentas = new MVentas();
        private static readonly IReportes repoReportes = new MReportes();

        private int Cia, Ase;
        private DateTime Desde, Hasta;
        private string Tipos;

        private int FacZamenis;
        private string FormaPago;

        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Documento;
        DataColumn Fecha;
        DataColumn Homologo;
        DataColumn TipoPago;
        DataColumn Admision;

        public ReportesCopias3(int cia, DateTime desde, DateTime hasta, string tipos)
        {
            InitializeComponent();
            this.Cia = cia;
            this.Desde = desde;
            this.Hasta = hasta;
            Tipos = tipos;
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Documento = dt.Columns.Add("Documento", typeof(string));
            if (Tipos == "Recibos de Caja")
            {
                Admision = dt.Columns.Add("Admision", typeof(string));
            }
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Homologo = dt.Columns.Add("Homologo", typeof(string));
            TipoPago = dt.Columns.Add("TipoPago", typeof(string));
        }

        void Cargar()
        {
            try
            {
                if (Tipos == "Recibos de Caja")
                {
                    List<ReportesRecepcion> ExportaRpt = repositorioVentas.Rpt_RecibosdeCaja(Convert.ToDateTime(Desde).Date,
                                                                                        Convert.ToDateTime(Hasta).Date,
                                                                                        Cia,
                                                                                        "VENTAS",
                                                                                        false);

                    if (ExportaRpt == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 0;
                        MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        Encabezados();

                        int Contador = 1;

                        foreach (ReportesRecepcion report in ExportaRpt)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Documento"] = report.Recibo.ToString();
                            row["Admision"] = report.Admision.ToString();
                            row["Fecha"] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row["Homologo"] = report.Homologo.ToString();
                            row["TipoPago"] = report.PacienteTelefono.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();
                    }
                }
                else if (Tipos == "Ordenes de Pedido")
                {
                    List<ReportesRecepcion> ExportaRpt = repositorioVentas.Rpt_FacturasVenta(Convert.ToDateTime(Desde).Date,
                                                                     Convert.ToDateTime(Hasta).Date,
                                                                     Cia,
                                                                     "VENTAS EMPRESA",
                                                                     "OP");

                    if (ExportaRpt == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 0;
                        MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        Encabezados();

                        int Contador = 1;

                        foreach (ReportesRecepcion report in ExportaRpt)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Documento"] = report.Admision.ToString();
                            row["Fecha"] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row["Homologo"] = report.Homologo.ToString();
                            row["TipoPago"] = report.PacienteTelefono.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();
                    }
                }
                else if (Tipos == "Facturas Particulares")
                {
                    List<FacturacionReports> Export = repoReportes.Exportar(Convert.ToDateTime(Desde).Date,
                                                                    Convert.ToDateTime(Hasta).Date,
                                                                    Cia,
                                                                    99,
                                                                    "OP");
                    if (Export == null)
                    {

                        MG = new MensajesGeneral();
                        MG.TipoImagen = 0;
                        MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        Encabezados();

                        int Contador = 1;

                        foreach (FacturacionReports report in Export)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Documento"] = report.Admision.ToString();
                            row["Fecha"] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row["Homologo"] = report.Homologo.ToString();
                            row["TipoPago"] = report.PacienteTelefono.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();
                    }
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado una opcion valida");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                panel1.Visible = true;

                FacZamenis = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                FormaPago = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                label1.Text = FacZamenis.ToString();
                comboBox1.Text = FormaPago;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bool res = false;

                if (Tipos == "Recibos de Caja")
                {
                    res = repoReportes.UpdateFormaPago(FacZamenis, Cia, comboBox1.Text, "Caja");
                }
                else if (Tipos == "Ordenes de Pedido")
                {
                    res = repoReportes.UpdateFormaPago(FacZamenis, Cia, comboBox1.Text, "Ventas");
                }
                else if (Tipos == "Facturas Particulares")
                {
                    res = repoReportes.UpdateFormaPago(FacZamenis, Cia, comboBox1.Text, "Particular");
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccion no valida";
                    MG.ShowDialog();
                    return;
                }

                if (res == true)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Actualizado";
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar";
                    MG.ShowDialog();
                }                

                panel1.Visible = false;
                Cargar();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void ReportesCopias3_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Reportes";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Actualizar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;

            Cargar();
        }
        void Estilos()
        {
            try
            {
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ScrollBars = ScrollBars.Both;

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["Documento"].Width = 120;
                dataGridView1.Columns["Fecha"].Width = 120;
                dataGridView1.Columns["Homologo"].Width = 120;
                dataGridView1.Columns["TipoPago"].Width = 280;

                dataGridView1.Font = new Font("Arial", 11);

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                dataGridView1.Columns["Documento"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Homologo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["TipoPago"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["Documento"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Homologo"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["TipoPago"].SortMode = DataGridViewColumnSortMode.NotSortable;

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
    }
}
