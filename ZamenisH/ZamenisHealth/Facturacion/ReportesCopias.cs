using DocumentosElectronicos.Servicio;
using Domain;
using Domain.CXN;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class ReportesCopias : Forma
    {
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IReportes repoReportes = new MReportes();
        private static readonly IVentas repoVentas = new MVentas();
        private static readonly IPlanos repoPlanos = new MPlanos();
        private static readonly IFacElectron repoElectron = new MFacElectron();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IAgendaC repoAge = new MAgendaC();
        private static readonly IRcCaja repoRCCaja = new MRcCaja();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly IFirmasDigitales fDigitales = new MFirmasDigitales();

        private Dictionary<int, string> DNotas;
        private Dictionary<int, string> DHistorias;
        private Espera E;
        int Ase, Cia, FacSelected, PACID;
        string HomologoFac, AutorPrint;

        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Documento;
        DataColumn Fecha;
        DataColumn Homologo;
        DataColumn Valor;
        DataColumn Paciente;
        DataColumn AdminRec;
        DataColumn Admision;
        DataColumn PacId;
        DataColumn Autor;

        public ReportesCopias()
        {
            InitializeComponent();
        }

        void NotasCredito(string Tipo)
        {
            try
            {
                string tip = "";

                switch (Tipo)
                {
                    case "Nota Credito Salud":
                        tip = "Salud";
                        break;
                        case "Nota Credito Ventas":
                        tip = "Ventas";
                        break;
                        case "Nota Credito Caja":
                        tip = "Caja";
                        break;
                        default:
                        MensajesGeneral M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "Seleccion Invalida";
                        M.ShowDialog();
                        return;
                }

                Encabezados();

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), 01);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), getMonthLastDay(comboBox4.Text));

                List<CXN_FACTURANC> getDocc = repoElectron.GetNotasCredito(tip, Desde.Date, Hasta.Date, Cia);
                if (getDocc == null)
                {
                    MensajesGeneral M = new MensajesGeneral();
                    M.TipoImagen = 0;
                    M.Mensaje = "No hay resultados en este rango de fechas y tipo de documento";
                    M.ShowDialog();
                    return;
                }                

                int Contador = 1;

                foreach (CXN_FACTURANC report in getDocc)
                {
                    DataRow row = dt.NewRow();

                    row["POS"] = Contador;
                    row["Documento"] = report.NumeroNC.ToString();
                    row["Fecha"] = Convert.ToDateTime(report.FechaNC).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    row["Homologo"] = report.FacturaElectronica.ToString();
                    row["Valor"] = "$0.00";
                    row["Paciente"] = "N/A";
                    row["AdminRec"] = report.OrdenPedido.ToString();

                    dt.Rows.Add(row);
                    dt.AcceptChanges();

                    Contador = Contador + 1;
                }

                Contador = 1;
                Estilos();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex >= 6)
                {
                    NotasCredito(comboBox1.Text);
                    return;
                }

                MensajesGeneral M = new MensajesGeneral();
                List<FacturacionReports> lista = new List<FacturacionReports>();
                string Tips = "";
                string Tabla = "";

                switch (comboBox1.Text)
                {
                    case "Facturas Aseguradoras y Particulares": 
                        Tips = "OP";
                        Tabla = "CXN_FACTURAS";
                        break;

                    case "Facturas Ventas":
                        Tips = "OP";
                        Tabla = "CXN_VENTAS";
                        break;

                    case "Facturas Caja":
                        Tips = "OP";
                        Tabla = "CXN_HORARIO";
                        break;

                    case "Notas Credito":
                        Tips = "OP";
                        Tabla = "CXN_FACTURANC";
                        break;

                    default:
                        M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "Seleccion Invalida";
                        M.ShowDialog();
                        return;
                }

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), 01);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), getMonthLastDay(comboBox4.Text));

                if (comboBox1.Text == "Facturas Aseguradoras y Particulares")
                {
                    lista = repoReportes.Exportar(Desde.Date,
                                                  Hasta.Date,
                                                  Cia,
                                                  Ase,
                                                  Tips);
                    if (lista == null)
                    {
                        Encabezados();

                        M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "No hay resultados en este rango de fechas y tipo de documento";
                        M.ShowDialog();
                        return;
                    }

                    Encabezados();

                    int Contador = 1;

                    foreach (FacturacionReports report in lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Documento"] = report.Admision.ToString();
                        row["Fecha"] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Homologo"] = report.Homologo.ToString();
                        row["Valor"] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                        row["Paciente"] = report.PacienteNombre.ToString();
                        row["AdminRec"] = Tabla.ToString();
                        row["PacId"] = report.PacienteIdentificacion.ToString();
                        row["Autor"] = report.EmpresaDireccion.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }
                else if (comboBox1.Text == "Facturas Ventas")
                {
                    List<ReportesRecepcion> ExportaRpt = repoVentas.Rpt_FacturasVenta(Desde.Date,
                                                                     Hasta.Date,
                                                                     Cia,
                                                                     comboBox2.Text,
                                                                     Tips);
                    if (ExportaRpt == null)
                    {
                        Encabezados();

                        M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "No hay resultados en este rango de fechas y tipo de documento";
                        M.ShowDialog();
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
                            row["Valor"] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                            row["Paciente"] = report.PacienteNombre.ToString();
                            row["AdminRec"] = Tabla.ToString();
                            row["PacId"] = "";
                            row["Autor"] = "";

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();
                    }
                }
                else if (comboBox1.Text == "Facturas Caja")
                {
                    List<ReportesRecepcion> ExportaRpt = repoVentas.Rpt_RecibosdeCaja(Desde.Date,
                                                                     Hasta.Date,
                                                                     Cia,
                                                                     comboBox2.Text,
                                                                     false);
                    if (ExportaRpt == null)
                    {
                        Encabezados();

                        M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "No hay resultados en este rango de fechas y tipo de documento";
                        M.ShowDialog();
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
                            row["Valor"] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                            row["Paciente"] = report.PacienteNombre.ToString();
                            row["AdminRec"] = Tabla.ToString();
                            row["PacId"] = "";
                            row["Autor"] = "";

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();
                    }
                }
                else if (comboBox1.Text == "Notas Credito")
                {
                    List<FacturacionReports> ExportaRpt = repoVentas.Rpt_FacturasNC(Desde.Date,
                                                                     Hasta.Date,
                                                                     Cia,
                                                                     comboBox2.Text,
                                                                     Tips);
                    if (ExportaRpt == null)
                    {
                        Encabezados();

                        M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "No hay resultados en este rango de fechas y tipo de documento";
                        M.ShowDialog();
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
                            row["Valor"] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                            row["Paciente"] = report.PacienteNombre.ToString();
                            row["AdminRec"] = report.Tipo.ToString();
                            row["PacId"] = "";
                            row["Autor"] = "";

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
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Seleccion Invalida";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }      
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                string Tips;

                switch (comboBox1.SelectedIndex)
                {
                    case 1:
                        Tips = "OP";
                        break;

                    case 2:
                        Tips = "DE";

                        break;

                    default:
                        Tips = "";
                        MensajesGeneral M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "Seleccion no valida, solamente puede filtrar en tabla los datos pero para reportes ingrese por la opcion recepcion";
                        M.ShowDialog();
                        return;
                }

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), 01);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), getMonthLastDay(comboBox4.Text));

                List<FacturacionReports> Export = repoReportes.Exportar(Desde.Date,
                                                                      Hasta.Date,
                                                                      Cia,
                                                                      Ase,
                                                                      Tips);
                if (Export == null)
                {
                    MensajesGeneral M = new MensajesGeneral();
                    M.TipoImagen = 0;
                    M.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                    M.ShowDialog();
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet1",
             "ZamenisHealth.Reportes.RDLC_ServFacturados.rdlc",
             Export);

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                string Tips = "";

                switch (comboBox1.SelectedIndex)
                {
                    case 1: //facturacs
                        Tips = "FA";
                        break;

                    case 2: //op
                        Tips = "OP";
                        break;

                    case 3: //DE
                        Tips = "DE";
                        break;

                    default:
                        MensajesGeneral M = new MensajesGeneral();
                        M.TipoImagen = 0;
                        M.Mensaje = "Seleccion Invalida";
                        M.ShowDialog();
                        break;
                }

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), 01);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonthNumber(comboBox4.Text), getMonthLastDay(comboBox4.Text));

                CXN_FACTURA F = new CXN_FACTURA
                {
                    Fac_Fecha_Des = Convert.ToDateTime(Desde.Date),
                    Fac_Fecha_Has = Convert.ToDateTime(Hasta.Date),
                    Fac_Cia = Cia,
                    Fac_Ase = Ase,
                    Fac_Tipo_Doc = Tips
                };

                repoPlanos.ExpPlanoFacturacion(F);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "AP");
            }
        }
        private void ReportesCopias_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Reportes";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Listar Facturas");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += btnZamenis1_ButtonClick;

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Reporte Facturas");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += btnZamenis2_ButtonClick;

            ToolStripButton btnExportar = new ToolStripButton();
            btnExportar = createToolButton("Exportar Excel");
            MenuLateral.Items.Add(btnExportar);
            btnExportar.Click += btnZamenis3_ButtonClick;

            gridZH1.dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
            gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            gridZH1.CeldaHeight = true;

            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;

            Encabezados();

            var getCias = repoCia.getAllCompañias();
            if (getCias != null)
            {
                foreach (var i in getCias)
                {
                    comboBox2.Items.Add(i.Com_Nombre);
                }

                comboBox2.SelectedIndex = 0;
            }

            var getAse = repoAse.getAseguradoras();
            if (getAse != null)
            {
                foreach (var i in getAse)
                {
                    comboBox3.Items.Add(i.Ase_Descripcion);
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cias = repoCia.getPrestadorbyName(comboBox2.Text);
            Cia = cias.Com_Identificador;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var aseg = repoAse.getInfoFromAsebyName(comboBox3.Text);
            Ase = aseg.Ase_Identificador;
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Documento = dt.Columns.Add("Documento", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Homologo = dt.Columns.Add("Homologo", typeof(string));
            Valor = dt.Columns.Add("Valor", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            AdminRec = dt.Columns.Add("AdminRec", typeof(string));
            PacId = dt.Columns.Add("PacId", typeof(string));
            Autor = dt.Columns.Add("Autor", typeof(string));
        }
        void Estilos()
        {
            gridZH1.dataGridView1.DataSource = dt;

            gridZH1.dataGridView1.Columns["POS"].Visible = false;
            gridZH1.dataGridView1.Columns["PacId"].Visible = false;
            gridZH1.dataGridView1.Columns["Autor"].Visible = false;
            gridZH1.dataGridView1.Columns["AdminRec"].Visible = false;
        }

        #region SOPORTES CLINICOS POR DOCUMENTO
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (comboBox1.Text == "Facturas Aseguradoras y Particulares")
                    {
                        FacSelected = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                        PACID = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString());
                        HomologoFac = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                        AutorPrint = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();

                        Point posicionLocal = Cursor.Position;

                        //Generar Documentos Clinicos
                        contextMenuStrip1.Visible = true;
                        contextMenuStrip1.Location = new Point(posicionLocal.X, posicionLocal.Y);
                        contextMenuStrip1.Visible = true;

                        DNotas = new Dictionary<int, string>();
                        DHistorias = new Dictionary<int, string>();

                        var GetDics = repoReportes.getAdmitionByInvoiceZamenis(FacSelected, Cia);

                        DNotas = GetDics.DicNotas;
                        DHistorias = GetDics.DicHistorias;
                    }
                }

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void generarDocumentosClinicosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Visible = true;
                label1.Visible = true;

                await Task.Run(() =>
                {
                    ProcesarDocumentos(); 
                });

                pictureBox1.Visible = false;
                label1.Visible = false;

                MG = new MensajesGeneral()
                {
                    Mensaje = "Terminado",
                    TipoImagen = 3
                };
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ProcesarDocumentos()
        {
            try
            {
                CXN_CIA CIA = repoCia.getPrestadorbyCode(Cia);

                Dictionary<int, object> getRDLCMasivoNotas = repoReportes.NotasMetodoRDLC(DNotas);
                Dictionary<int, object> getRDLCMasivoHistorias = repoReportes.NotasMetodoRDLC(DHistorias);

                if (getRDLCMasivoNotas != null)
                {
                    int Contador = 1;
                    ReportViewer R = new ReportViewer();

                    int totalArchivos = getRDLCMasivoNotas.Keys.Count;

                    foreach (KeyValuePair<int, object> kvp in getRDLCMasivoNotas)
                    {
                        if (kvp.Value is List<ReportNotas>)
                        {
                            List<ReportNotas> listaClase1 = (List<ReportNotas>)getRDLCMasivoNotas[kvp.Key];

                            R.LocalReport.DataSources.Clear();
                            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Notas", listaClase1));

                            if (Preferencias.CuracionesCORE == "A" && listaClase1[0].listaMedidas != null)
                            {
                                R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_NotasMed", listaClase1[0].listaMedidas));
                                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_NotasCore.rdlc";
                            }
                            else
                            {
                                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_Notas.rdlc";
                            }

                            R.SetDisplayMode(DisplayMode.PrintLayout);
                            R.ZoomMode = ZoomMode.Percent;
                            R.ZoomPercent = 100;
                            R.Font = new System.Drawing.Font("Arial", 7);
                            R.LocalReport.EnableExternalImages = true;
                            R.RefreshReport();
                            //maestro.Visible = true;
                            R.Dock = System.Windows.Forms.DockStyle.Fill;

                            byte[] bytes = R.LocalReport.Render("PDF");
                            FileStream fss = new FileStream("C:\\CXN\\Reportes\\Notas\\" + Contador.ToString() + ".pdf", FileMode.Create);
                            fss.Write(bytes, 0, bytes.Length);
                            fss.Close();
                            Contador = Contador + 1;
                        }
                    }

                    Contador = Contador - 1;
                    Comunes.UnificadorPDFMasivo U = new Comunes.UnificadorPDFMasivo();
                    U.Unificar_Estructura(@"C:\CXN\Reportes\Notas\",
                                          1.ToString(),
                                          totalArchivos.ToString(),
                                          "PDX_" + CIA.Com_Identificacion + "_" + HomologoFac + ".pdf");

                    R.Dispose();

                    foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Notas\", "*.pdf"))
                    {
                        BorrarArchivoConReintentos(archivo);
                    }
                }

                if (getRDLCMasivoHistorias != null)
                {
                    int Contador2 = 1;
                    ReportViewer R2 = new ReportViewer();

                    int totalArchivos2 = getRDLCMasivoHistorias.Keys.Count;

                    foreach (KeyValuePair<int, object> kvp in getRDLCMasivoHistorias)
                    {
                        List<HCMG> listaClase2 = (List<HCMG>)getRDLCMasivoHistorias[kvp.Key];

                        R2.LocalReport.DataSources.Clear();
                        R2.LocalReport.DataSources.Add(new ReportDataSource("DataSet_HCMG", listaClase2));
                        R2.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_HCMG.rdlc";
                        R2.SetDisplayMode(DisplayMode.PrintLayout);
                        R2.ZoomMode = ZoomMode.Percent;
                        R2.ZoomPercent = 100;
                        R2.Font = new System.Drawing.Font("Arial", 7);
                        R2.LocalReport.EnableExternalImages = true;
                        R2.RefreshReport();
                        //maestro.Visible = true;
                        R2.Dock = System.Windows.Forms.DockStyle.Fill;

                        byte[] bytes = R2.LocalReport.Render("PDF");
                        FileStream fss = new FileStream("C:\\CXN\\Reportes\\Historias\\" + Contador2.ToString() + ".pdf", FileMode.Create);
                        fss.Write(bytes, 0, bytes.Length);
                        fss.Close();
                        Contador2 = Contador2 + 1;
                    }

                    Contador2 = Contador2 - 1;
                    Comunes.UnificadorPDFMasivo U = new Comunes.UnificadorPDFMasivo();
                    U.Unificar_Estructura(@"C:\CXN\Reportes\Historias\",
                                          1.ToString(),
                                          totalArchivos2.ToString(),
                                          "HEV_" + CIA.Com_Identificacion + "_" + HomologoFac + ".pdf");

                    R2.Dispose();

                    foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Historias\", "*.pdf"))
                    {
                        BorrarArchivoConReintentos(archivo);
                    }
                }

                if (repoFacturacion.getValCuotasReceived(FacSelected, Cia) > 0)
                {
                    //hoja de cuotas
                    Cuotas();
                }
                else
                {
                    //hoja de firmas
                    Firmas();
                }

                //Factura
                List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_Export(FacSelected, Cia, "OP");
                if (GenerarDocumentoGrafico != null)
                {
                    ReportViewer R = new ReportViewer();

                    R.LocalReport.DataSources.Clear();
                    R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", GenerarDocumentoGrafico));
                    R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc";
                    R.SetDisplayMode(DisplayMode.PrintLayout);
                    R.ZoomMode = ZoomMode.Percent;
                    R.ZoomPercent = 100;
                    R.Font = new System.Drawing.Font("Arial", 7);
                    R.LocalReport.EnableExternalImages = true;
                    R.RefreshReport();
                    //maestro.Visible = true;
                    R.Dock = System.Windows.Forms.DockStyle.Fill;

                    byte[] bytes = R.LocalReport.Render("PDF");
                    FileStream fss = new FileStream("C:\\CXN\\Reportes\\FEV_" + CIA.Com_Identificacion.ToString() + "_" + HomologoFac + ".pdf", FileMode.Create);
                    fss.Write(bytes, 0, bytes.Length);
                    fss.Close();
                }

                pictureBox1.Visible = false;
                label1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Firmas()
        {
            try
            {
                CXN_CIA DatNombre = repoCia.getPrestadorbyCode(Cia);
                CXN_PACIENTES pacData = repoPacientes.LlamarPacientebyId(PACID);
                CXN_ASEGURADORA aseData = repoAse.getInfoFromAsebyCode(pacData.Pac_Aseguradora);
                List<int> Admisiones = repoFacturacion.getAdmitionsByFac(FacSelected, Cia);

                if (Admisiones != null)
                {
                    List<FirmasR> lista = new List<FirmasR>();
                    int Contador = 1;

                    foreach (var i in Admisiones)
                    {
                        otrosDatosPacienteHorario dataAdm = repoAge.cargarAdmision(i, "'H'");

                        CXN_FIRMASDIGITALES fTemp = fDigitales.getFirmas(i);
                        byte[] firmaByte = null;

                        if (fTemp == null)
                        {
                            firmaByte = Convert.FromBase64String(fDigitales.ImageNull());
                        }
                        else
                        {
                            using (MemoryStream ms = new MemoryStream(fTemp.Firma))
                            using (Bitmap bmp = new Bitmap(ms))
                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                bmp.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
                                firmaByte = ms2.ToArray();
                            }
                        }

                        lista.Add(new FirmasR
                        {
                            PacienteNombre = pacData.Pac_PrimerA.ToString() + " " +
                                             pacData.Pac_SegundoA.ToString() + " " +
                                             pacData.Pac_PrimerN.ToString() + " " +
                                             pacData.Pac_SegundoN.ToString(),
                            PacienteAseguradora = aseData.Ase_Descripcion.ToString(),
                            PacienteIdentificacion = pacData.Pac_TipoId.ToString() + " " +
                                                     pacData.Pac_IdNum.ToString(),
                            PacienteTelefono = pacData.Pac_Telefono.ToString(),
                            PacienteDireccion = pacData.Pac_Direccion.ToString(),

                            EmpresaNombre = DatNombre.Com_Nombre,
                            EmpresaDireccion = DatNombre.Com_Direccion,
                            Com_UsuarioGraba = DatNombre.Com_Tipo_Doc + " " + DatNombre.Com_Identificacion, //idd prestaddor
                            EmpresaTelefono = dataAdm.Com_Nombre_SMS.Contains("CONSULTA") ? "C" : Contador.ToString(),
                            Logo = Convert.FromBase64String(DatNombre.Com_Logo),

                            FechaBase = Convert.ToDateTime(dataAdm.Hor_Pac_Fecha_Cita),
                            FirmaByte = firmaByte,
                            Con_Nombre = dataAdm.Com_Nombre_SMS,
                            Com_Direccion = AutorPrint, //autorizacion
                            Admision = i,
                            Cantidad = 0,// i.Cant
                        });

                        if (!dataAdm.Com_Nombre_SMS.Contains("CONSULTA"))
                        {
                            Contador++;
                        }
                    }

                    ReportViewer R = new ReportViewer();

                    R.LocalReport.DataSources.Clear();
                    R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Firmas", lista));
                    R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FirmasDigitales.rdlc";
                    R.SetDisplayMode(DisplayMode.PrintLayout);
                    R.ZoomMode = ZoomMode.Percent;
                    R.ZoomPercent = 100;
                    R.Font = new System.Drawing.Font("Arial", 7);
                    R.LocalReport.EnableExternalImages = true;
                    R.RefreshReport();
                    //maestro.Visible = true;
                    R.Dock = System.Windows.Forms.DockStyle.Fill;

                    byte[] bytes = R.LocalReport.Render("PDF");
                    FileStream fss = new FileStream("C:\\CXN\\Reportes\\CRC_" + DatNombre.Com_Identificacion.ToString() + "_" + HomologoFac + ".pdf", FileMode.Create);
                    fss.Write(bytes, 0, bytes.Length);
                    fss.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Cuotas()
        {
            try
            {
                List<otrosDatosPacienteHorario> H = new List<otrosDatosPacienteHorario>();
                CXN_CIA DataCompany = repoCia.getPrestadorbyCode(Cia);
                CXN_PACIENTES DataPaciente = repoPacientes.LlamarPacientebyId(PACID);
                List<int> Admisiones = repoFacturacion.getAdmitionsByFac(FacSelected, Cia);

                foreach (int numRC in Admisiones)
                {
                    otrosDatosPacienteHorario otherData = repoAge.cargarAdmision(numRC, "'H','P'");

                    (int Valor, string Concepto) dato = repoRCCaja.getValRcCaja(numRC);
                    switch (dato.Concepto)
                    {
                        case "01":

                            if (dato.Valor != 0)
                            {
                                H.Add(new otrosDatosPacienteHorario
                                {
                                    Hor_Id = numRC,
                                    Hor_Pac_Fecha_Cita = otherData.Hor_Pac_Fecha_Cita,
                                    Hor_Pac_Cup = otherData.Hor_Pac_Cup + " - " + repoConvenios.NameServiceCUP(otherData.Hor_Pac_Cup),
                                    Hor_Pac_Id = Convert.ToInt32(dato.Valor), //Valor
                                    Hor_ConceptoRecaudo = "COPAGO",
                                    Com_Nombre = DataCompany.Com_Nombre,
                                    Com_Identificacion = DataCompany.Com_Identificacion,
                                    Logo = Convert.FromBase64String(otherData.Com_Logo),
                                    PacienteNombre = DataPaciente.Pac_PrimerA + " " + DataPaciente.Pac_SegundoA + " " + DataPaciente.Pac_PrimerN + " " + DataPaciente.Pac_SegundoN,
                                    PacienteIdentificacion = DataPaciente.Pac_TipoId + " " + DataPaciente.Pac_IdNum
                                });
                            }
                            break;

                        case "02":
                            if (dato.Valor != 0)
                            {
                                H.Add(new otrosDatosPacienteHorario
                                {
                                    Hor_Id = numRC,
                                    Hor_Pac_Fecha_Cita = otherData.Hor_Pac_Fecha_Cita,
                                    Hor_Pac_Cup = otherData.Hor_Pac_Cup + " - " + repoConvenios.NameServiceCUP(otherData.Hor_Pac_Cup),
                                    Hor_Pac_Id = Convert.ToInt32(dato.Valor), //Valor
                                    Hor_ConceptoRecaudo = "CUOTA MODERADORA",
                                    Com_Nombre = DataCompany.Com_Nombre,
                                    Com_Identificacion = DataCompany.Com_Identificacion,
                                    Logo = Convert.FromBase64String(otherData.Com_Logo),
                                    PacienteNombre = DataPaciente.Pac_PrimerA + " " + DataPaciente.Pac_SegundoA + " " + DataPaciente.Pac_PrimerN + " " + DataPaciente.Pac_SegundoN,
                                    PacienteIdentificacion = DataPaciente.Pac_TipoId + " " + DataPaciente.Pac_IdNum
                                });
                            }
                            break;

                        case "03":
                            if (dato.Valor != 0)
                            {
                                H.Add(new otrosDatosPacienteHorario
                                {
                                    Hor_Id = numRC,
                                    Hor_Pac_Fecha_Cita = otherData.Hor_Pac_Fecha_Cita,
                                    Hor_Pac_Cup = otherData.Hor_Pac_Cup + " - " + repoConvenios.NameServiceCUP(otherData.Hor_Pac_Cup),
                                    Hor_Pac_Id = Convert.ToInt32(dato.Valor), //Valor
                                    Hor_ConceptoRecaudo = "PAGOS COMPARTIDOS",
                                    Com_Nombre = DataCompany.Com_Nombre,
                                    Com_Identificacion = DataCompany.Com_Identificacion,
                                    Logo = Convert.FromBase64String(otherData.Com_Logo),
                                    PacienteNombre = DataPaciente.Pac_PrimerA + " " + DataPaciente.Pac_SegundoA + " " + DataPaciente.Pac_PrimerN + " " + DataPaciente.Pac_SegundoN,
                                    PacienteIdentificacion = DataPaciente.Pac_TipoId + " " + DataPaciente.Pac_IdNum
                                });
                            }
                            break;

                        case "04":
                            if (dato.Valor != 0)
                            {
                                H.Add(new otrosDatosPacienteHorario
                                {
                                    Hor_Id = numRC,
                                    Hor_Pac_Fecha_Cita = otherData.Hor_Pac_Fecha_Cita,
                                    Hor_Pac_Cup = otherData.Hor_Pac_Cup + " - " + repoConvenios.NameServiceCUP(otherData.Hor_Pac_Cup),
                                    Hor_Pac_Id = Convert.ToInt32(dato.Valor), //Valor
                                    Hor_ConceptoRecaudo = "ANTICIPO",
                                    Com_Nombre = DataCompany.Com_Nombre,
                                    Com_Identificacion = DataCompany.Com_Identificacion,
                                    Logo = Convert.FromBase64String(otherData.Com_Logo),
                                    PacienteNombre = DataPaciente.Pac_PrimerA + " " + DataPaciente.Pac_SegundoA + " " + DataPaciente.Pac_PrimerN + " " + DataPaciente.Pac_SegundoN,
                                    PacienteIdentificacion = DataPaciente.Pac_TipoId + " " + DataPaciente.Pac_IdNum
                                });
                            }
                            break;
                    }
                }

                ReportViewer R = new ReportViewer();

                R.LocalReport.DataSources.Clear();
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_SoportesPagos", H));
                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_SoportePagos.rdlc";
                R.SetDisplayMode(DisplayMode.PrintLayout);
                R.ZoomMode = ZoomMode.Percent;
                R.ZoomPercent = 100;
                R.Font = new System.Drawing.Font("Arial", 7);
                R.LocalReport.EnableExternalImages = true;
                R.RefreshReport();
                //maestro.Visible = true;
                R.Dock = System.Windows.Forms.DockStyle.Fill;

                byte[] bytes = R.LocalReport.Render("PDF");
                FileStream fss = new FileStream("C:\\CXN\\Reportes\\CRC_" + DataCompany.Com_Identificacion.ToString() + "_" + HomologoFac +".pdf", FileMode.Create);
                fss.Write(bytes, 0, bytes.Length);
                fss.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void homologarDocumentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminSystem.Homologos homologos = new AdminSystem.Homologos();
            homologos.ShowDialog();
        }
        void BorrarArchivoConReintentos(string archivo, int maxIntentos = 5)
        {
            int intentos = 0;

            while (intentos < maxIntentos)
            {
                try
                {
                    File.Delete(archivo);
                    Console.WriteLine($"{archivo} eliminado.");
                    return; // sale si se logró eliminar
                }
                catch (IOException)
                {
                    intentos++;
                    System.Threading.Thread.Sleep(500); // espera 0.5 segundos
                }
                catch (UnauthorizedAccessException)
                {
                    intentos++;
                    System.Threading.Thread.Sleep(500);
                }
            }

            Console.WriteLine($"No se pudo eliminar {archivo} después de {maxIntentos} intentos.");
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Tips = "OP";

                ReportesCopias2 r2;

                CXN_FACTURA F = new CXN_FACTURA
                {
                    Fac_Num_Fac = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()),
                    Fac_Cia = Cia,                   
                    Homologo = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString(),
                    Fac_Observa = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString()
                };

                if (comboBox1.Text == "Facturas Ventas")
                {
                    F.Fac_Tipo_Doc = Tips;
                    r2 = new ReportesCopias2(F, false, true);
                    r2.ShowDialog();
                }
                else if (comboBox1.Text == "Facturas Aseguradoras y Particulares")
                {
                    F.Fac_Tipo_Doc = Tips;
                    r2 = new ReportesCopias2(F, false, false);
                    r2.ShowDialog();
                }
                else if (comboBox1.Text == "Notas Credito")
                {
                    string tip = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                    string ncredito = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                    int nzamenis = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());

                    if (tip == "Salud")
                    {
                        List<FacturasR> GenerarDocumentoGraficoNC = ExportarPDF.ExportarFacturaAseguradorasNC(ncredito, Cia);

                        ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                 "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludNC.rdlc",
                                 GenerarDocumentoGraficoNC);
                    }
                    
                    else if (tip == "Ventas")
                    {
                        List<FacturacionRpt> Exportar = ExportarPDF.ExportarFacturaVentasNC(nzamenis, Cia, "OP", ncredito);

                        ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludNC.rdlc",
                                Exportar);
                    }
                    else if (tip == "Caja")
                    {
                        List<RCCAJA> Exportar = ExportarPDF.ExportarReciboCajaNC(nzamenis);

                        ConfigForm.GenerarReportViewer("ReciboCajaDataset",
                                "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaCajaNC.rdlc",
                                Exportar);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No se logro exportar el reporte";
                        MG.TipoImagen = 0;
                        MG.ShowDialog();
                    }
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Reporte no valido";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }        
    }
}
