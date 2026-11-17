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
    public partial class ReportesCopias4 : Forma
    {
        private IReportes repoReportes;
        private ICompañia repoCompañia;
        private IFacturacion repoFacturacion;
        private ICManejo repoCManejo;
        private IPacientes repoPacientes;
        private IAseguradoras repoAseguradoras;
        private IAgendaC repoAgenda;
        private IFirmasDigitales repoFirmas;
        private IRcCaja repoCaja;
        private IConvenios repoConvenios;
        private IVentas repoVentas;
        private IFacElectron repoFacElectron;
        private IHelisa repoWSClients;


        private CXN_FACTURA Datos;
        private bool IsAdministrator;
        private MensajesGeneral MG;
        private DataView vistaHistorial;
        private ToolStripButton btnGenerar;
        private ToolStripButton btnMarcar;
        private ToolStripButton btnDesMarcar;
        private ToolStripButton btnEnviarPDF;

        private Dictionary<int, string> DNotas;
        private Dictionary<int, string> DHistorias;

        DataTable dt = new DataTable();

        DataColumn CheckForSend;
        DataColumn Documento;
        DataColumn Fecha;
        DataColumn Homologo;
        DataColumn Valor;
        DataColumn Paciente;

        public ReportesCopias4(CXN_FACTURA datos, bool isAdministrator)
        {
            InitializeComponent();
            repoReportes = new MReportes();
            repoCompañia = new MCompañia();
            repoFacturacion = new MFacturacion();
            repoCManejo = new MCManenejo();
            repoPacientes = new MPacientes();
            repoAseguradoras = new MAseguradoras();
            repoAgenda = new MAgendaC();
            repoFirmas = new MFirmasDigitales();
            repoCaja = new MRcCaja();
            repoConvenios = new MConvenios();
            repoVentas = new MVentas();
            repoFacElectron = new MFacElectron();
            repoWSClients = new MHelisa();

            Datos = datos;
            IsAdministrator = isAdministrator;
        }

        private void ReportesCopias4_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Descargar Doc. Clinico Masivo";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.DisableRowsAdded();
            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;

            //CargarFacturas();

            btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Generar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += btnGenerar_Click;
            btnGenerar.Visible = IsAdministrator == true ? true : false;

            btnEnviarPDF = new ToolStripButton();
            btnEnviarPDF = createToolButton("Radicar PDF");
            MenuLateral.Items.Add(btnEnviarPDF);
            btnEnviarPDF.Click += btnEnviarPDF_Click;
            btnEnviarPDF.Visible = IsAdministrator == true ? true : false;

            btnMarcar = new ToolStripButton();
            btnMarcar = createToolButton("Marcar Todos");
            MenuLateral.Items.Add(btnMarcar);
            btnMarcar.Click += btnMarcar_Click;

            btnDesMarcar = new ToolStripButton();
            btnDesMarcar = createToolButton("Desmarcar Todos");
            MenuLateral.Items.Add(btnDesMarcar);
            btnDesMarcar.Click += btnDesMarcar_Click;
        }
        async void btnEnviarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Visible = true;
                btnGenerar.Enabled = false;
                btnEnviarPDF.Enabled = false;
                ImageClose.Enabled = false;
                btnMarcar.Enabled = false;
                btnDesMarcar.Enabled = false;

                try
                {
                    string TokenGenerado = repoFacElectron.GetTokenSaved(Datos.Fac_Cia).Trim();
                    if (TokenGenerado == null)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                        return;
                    }

                    Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1XML", Datos.Fac_Cia);
                    if (dataWS == null)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No hay resultados para login de Facturacion Electronica",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                        return;
                    }

                    try
                    {
                        gridZH1.dataGridView1.EndEdit();

                        foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                        {
                            object valor = row.Cells["✔"].Value;
                            bool isChecked = valor != null && Convert.ToBoolean(valor);

                            int FacSelected = Convert.ToInt32(row.Cells["Documento"].Value);
                            string HomologoFac = row.Cells["Homologo"].Value.ToString();

                            if (isChecked == true)
                            {
                                await EnviarPDF(HomologoFac, TokenGenerado);
                            }
                        }

                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Proceso finalizado",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Proceso finalizado con errores: " + ex.Message,
                            TipoImagen = 0
                        };

                        MG.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                pictureBox1.Visible = false;
                btnGenerar.Enabled = true;
                btnEnviarPDF.Enabled = true;
                ImageClose.Enabled = true;
                btnMarcar.Enabled = true;
                btnDesMarcar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void btnGenerar_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "Facturas Aseguradoras y Particulares")
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "La generacin de doucmentos clinicos solo esta disponible para Curaciones y consultas en Aseguradoras",
                    TipoImagen = 1000
                };

                MG.ShowDialog();
                return;
            }

            pictureBox1.Visible = true;
            btnGenerar.Enabled = false;
            btnEnviarPDF.Enabled = false;
            ImageClose.Enabled = false;
            btnMarcar.Enabled = false;
            btnDesMarcar.Enabled = false;

            try
            {                
                try
                {
                    gridZH1.dataGridView1.EndEdit();

                    foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                    {
                        object valor = row.Cells["✔"].Value;
                        bool isChecked = valor != null && Convert.ToBoolean(valor);

                        int FacSelected = Convert.ToInt32(row.Cells["Documento"].Value);
                        string HomologoFac = row.Cells["Homologo"].Value.ToString();

                        if (isChecked == true)
                        {
                            await Ejecutar(FacSelected, HomologoFac);
                        }
                    }

                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Proceso finalizado",
                        TipoImagen = 3
                    };
                    
                    MG.ShowDialog();
                }
                catch (Exception ex)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Proceso finalizado con errores: " + ex.Message,
                        TipoImagen = 0
                    };

                    MG.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            pictureBox1.Visible = false;
            btnGenerar.Enabled = true;
            btnEnviarPDF.Enabled = true;
            ImageClose.Enabled = true;
            btnMarcar.Enabled = true;
            btnDesMarcar.Enabled = true;
        }
        private void btnMarcar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    object valor = row.Cells["✔"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    if (!isChecked)
                    {
                        row.Cells["✔"].Value = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDesMarcar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    object valor = row.Cells["✔"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    if (isChecked)
                    {
                        row.Cells["✔"].Value = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int PosRow = e.RowIndex;
                int PosCol = e.ColumnIndex;

                DataGridViewRow row = gridZH1.dataGridView1.Rows[PosRow];

                object valor = row.Cells["✔"].Value;
                bool isChecked = valor != null && Convert.ToBoolean(valor);

                if (isChecked == true)
                {
                    isChecked = false;
                }
                else
                {
                    isChecked = true;
                }

                row.Cells["✔"].Value = isChecked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();

            CheckForSend = dt.Columns.Add("✔", typeof(bool));
            Documento = dt.Columns.Add("Documento", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Homologo = dt.Columns.Add("Homologo", typeof(string));
            Valor = dt.Columns.Add("Valor", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));

            gridZH1.dataGridView1.DataSource = dt;
        }
        void CargarFacturas()
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe seleccionar una opcion a filtrar",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    switch (comboBox1.Text)
                    {
                        case "Facturas Aseguradoras y Particulares":
                            FacturasAseguradoras();
                            break;

                        case "Facturas de Recaudo de Efectivo Recepcion":
                            FacturasCaja();
                            break;

                        case "Facturas de Ventas Recepcion":
                            FacturasVentas();
                            break;

                        case "Notas Credito":
                            NCredito();
                            break;

                        default:
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Debe seleccionar una opcion a filtrar",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                            break;
                    }                    
                }                             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void NCredito()
        {
            try
            {
                List<FacturacionReports> ExportaRpt = repoVentas.Rpt_FacturasNC(Datos.Fac_Fecha_Des,
                                                                    Datos.Fac_Fecha_Has,
                                                                    Datos.Fac_Cia,
                                                                    "NOTAS CREDITO",
                                                                    "OP");
                if (ExportaRpt == null)
                {
                    Encabezados();

                    MG = new MensajesGeneral()
                    {
                        TipoImagen = 0,
                        Mensaje = "No hay resultados en este rango de fechas y tipo de documento"
                    };
                    
                    MG.ShowDialog();
                }
                else
                {
                    Encabezados();
                    dt.BeginLoadData();

                    try
                    {
                        foreach (ReportesRecepcion report in ExportaRpt)
                        {
                            DataRow row = dt.NewRow();

                            row[Documento] = report.Admision.ToString();
                            row[Fecha] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row[Homologo] = report.Homologo.ToString();
                            row[Valor] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                            row[Paciente] = report.PacienteNombre.ToString();

                            dt.Rows.Add(row);
                        }
                    }
                    finally
                    {
                        dt.EndLoadData();
                    }

                    vistaHistorial = new DataView(dt);
                    var grid = gridZH1.dataGridView1;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    grid.DataSource = vistaHistorial;

                    Estilos(grid);

                    if (grid.Rows.Count > 0 && !gridZH1.CeldaHeight)
                    {
                        grid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    }

                    grid.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void FacturasVentas()
        {
            try
            {
                List<ReportesRecepcion> ExportaRpt = repoVentas.Rpt_FacturasVenta(Datos.Fac_Fecha_Des,
                                                                    Datos.Fac_Fecha_Has,
                                                                    Datos.Fac_Cia,
                                                                    "VENTAS",
                                                                    "OP");
                if (ExportaRpt == null)
                {
                    Encabezados();

                    MG = new MensajesGeneral()
                    {
                        TipoImagen = 0,
                        Mensaje = "No hay resultados en este rango de fechas y tipo de documento"
                    };
                    
                    MG.ShowDialog();
                }
                else
                {
                    Encabezados();
                    dt.BeginLoadData();

                    try
                    {
                        foreach (ReportesRecepcion report in ExportaRpt)
                        {
                            DataRow row = dt.NewRow();

                            row[Documento] = report.Admision.ToString();
                            row[Fecha] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row[Homologo] = report.Homologo.ToString();
                            row[Valor] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                            row[Paciente] = report.PacienteNombre.ToString();

                            dt.Rows.Add(row);
                        }
                    }
                    finally
                    {
                        dt.EndLoadData();
                    }

                    vistaHistorial = new DataView(dt);
                    var grid = gridZH1.dataGridView1;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    grid.DataSource = vistaHistorial;

                    Estilos(grid);

                    if (grid.Rows.Count > 0 && !gridZH1.CeldaHeight)
                    {
                        grid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    }

                    grid.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void FacturasCaja()
        {
            try
            {
                List<ReportesRecepcion> ExportaRpt = repoVentas.Rpt_RecibosdeCaja(Datos.Fac_Fecha_Des,
                                                                    Datos.Fac_Fecha_Has,
                                                                    Datos.Fac_Cia,
                                                                    "PRESTADOR PRINCIPAL",
                                                                    false);

                if (ExportaRpt == null)
                {
                    Encabezados();

                    MG = new MensajesGeneral()
                    {
                        TipoImagen = 0,
                        Mensaje = "No hay resultados en este rango de fechas y tipo de documento"
                    };
                    
                    MG.ShowDialog();
                }
                else
                {
                    Encabezados();
                    dt.BeginLoadData();

                    try
                    {
                        foreach (ReportesRecepcion report in ExportaRpt)
                        {
                            DataRow row = dt.NewRow();

                            row["✔"] = false;
                            row[Documento] = report.Admision.ToString();
                            row[Fecha] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row[Homologo] = report.Homologo.ToString();
                            row[Valor] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                            row[Paciente] = report.PacienteNombre.ToString();

                            dt.Rows.Add(row);
                        }
                    }
                    finally
                    {
                        dt.EndLoadData();
                    }

                    vistaHistorial = new DataView(dt);
                    var grid = gridZH1.dataGridView1;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    grid.DataSource = vistaHistorial;

                    Estilos(grid);

                    if (grid.Rows.Count > 0 && !gridZH1.CeldaHeight)
                    {
                        grid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    }

                    grid.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void FacturasAseguradoras()
        {
            try
            {
                List<FacturacionReports> lista = repoReportes.Exportar(Datos.Fac_Fecha_Des.Date,
                                                  Datos.Fac_Fecha_Has.Date,
                                                  Datos.Fac_Cia,
                                                  Datos.Fac_Ase,
                                                  "OP");
                if (lista == null)
                {
                    Encabezados();

                    MG = new MensajesGeneral()
                    {
                        TipoImagen = 0,
                        Mensaje = "No hay resultados en este rango de fechas y tipo de documento"
                    };

                    MG.ShowDialog();
                }
                else
                {
                    Encabezados();
                    dt.BeginLoadData();

                    try
                    {
                        foreach (FacturacionReports report in lista)
                        {
                            DataRow row = dt.NewRow();

                            row["✔"] = false;
                            row[Documento] = report.Admision;
                            row[Fecha] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row[Homologo] = report.Homologo.ToString();
                            row[Valor] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                            row[Paciente] = report.PacienteNombre.ToString();

                            dt.Rows.Add(row);
                        }
                    }
                    finally
                    {
                        dt.EndLoadData();
                    }

                    vistaHistorial = new DataView(dt);
                    var grid = gridZH1.dataGridView1;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    grid.DataSource = vistaHistorial;

                    Estilos(grid);

                    if (grid.Rows.Count > 0 && !gridZH1.CeldaHeight)
                    {
                        grid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                    }

                    grid.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D)
        {
            D.Columns["✔"].Width = 20;
            D.Columns["Documento"].Width = 100;
            D.Columns["Homologo"].Width = 100;
            D.Columns["Valor"].Width = 100;
            D.Columns["Paciente"].Width = 300;

            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.ReadOnly = false;
            D.Columns["✔"].ReadOnly = false;

            D.Columns["Documento"].ReadOnly = true;
            D.Columns["Homologo"].ReadOnly = true;
            D.Columns["Valor"].ReadOnly = true;
            D.Columns["Paciente"].ReadOnly = true;

            gridZH1.dataGridView1.ClearSelection();
        }

        #region RADICAR PDF
        async Task EnviarPDF(string HomologoFac, string TokenGenerado)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (comboBox1.Text == "Facturas Aseguradoras y Particulares")
                    {
                        GenerateXMLPDF.ReenviarPDFSalud(HomologoFac, Datos.Fac_Cia, "Salud", TokenGenerado);
                    }
                    else if (comboBox1.Text == "Facturas de Recaudo de Efectivo Recepcion")
                    {
                        GenerateXMLPDF.ReenviarPDFSalud(HomologoFac, Datos.Fac_Cia, "Caja", TokenGenerado);
                    }
                    else if (comboBox1.Text == "Facturas de Ventas Recepcion")
                    {
                        GenerateXMLPDF.ReenviarPDFSalud(HomologoFac, Datos.Fac_Cia, "Ventas", TokenGenerado);
                    }
                    else if (comboBox1.Text == "Notas Credito")
                    {
                        GenerateXMLPDF.ReenviarPDFSalud(HomologoFac, Datos.Fac_Cia, "SaludNC", TokenGenerado);
                        GenerateXMLPDF.ReenviarPDFSalud(HomologoFac, Datos.Fac_Cia, "VentasNC", TokenGenerado);
                        GenerateXMLPDF.ReenviarPDFSalud(HomologoFac, Datos.Fac_Cia, "CajaNC", TokenGenerado);
                    }
                    else
                    {
                        return;
                    }

                    gridZH1.dataGridView1.ClearSelection();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region SOPORTES CLINICOS POR DOCUMENTO
        async Task Ejecutar(int FacSelected, string HomologoFac)
        {
            try
            {
                await Task.Run(() =>
                {
                    DNotas = new Dictionary<int, string>();
                    DHistorias = new Dictionary<int, string>();

                    var GetDics = repoReportes.getAdmitionByInvoiceZamenis(FacSelected, Datos.Fac_Cia);

                    DNotas = GetDics.DicNotas;
                    DHistorias = GetDics.DicHistorias;

                    ProcesarDocumentos(HomologoFac, FacSelected);

                    gridZH1.dataGridView1.ClearSelection();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ProcesarDocumentos(string HomologoFac, int FacZamenis)
        {
            try
            {
                CXN_CIA CIA = repoCompañia.getPrestadorbyCode(Datos.Fac_Cia);

                string Carpeta = $"C:\\CXN\\Reportes\\{CIA.Com_Identificacion}_{HomologoFac}";

                List<int> getIdCamManejo = new List<int>();

                CXN_FACTURA dataFac = repoFacturacion.getFacturasTable(HomologoFac, Datos.Fac_Cia);
                if (dataFac != null)
                {
                    getIdCamManejo = repoCManejo.getManejosxPaciente(dataFac.Fac_Pac, dataFac.Fac_Fecha_Des, dataFac.Fac_Fecha_Has);
                }

                Dictionary<int, object> getRDLCMasivoNotas = repoReportes.NotasMetodoRDLC(DNotas);
                Dictionary<int, object> getRDLCMasivoHistorias = repoReportes.NotasMetodoRDLC(DHistorias);

                //Si no existe la carpeta, se crea, si existe pasa 
                if (!Directory.Exists(Carpeta))
                {
                    Directory.CreateDirectory(Carpeta);
                }

                int IdPaciente = 0;
                string NumAutorizacion = "";

                //Factura
                List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_Export(FacZamenis, Datos.Fac_Cia, "OP");
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
                    FileStream fss = new FileStream(Carpeta + "\\FEV_" + CIA.Com_Identificacion.ToString() + "_" + HomologoFac + ".pdf", FileMode.Create);
                    fss.Write(bytes, 0, bytes.Length);
                    fss.Close();

                    IdPaciente = GenerarDocumentoGrafico[0].DocE_6;
                    NumAutorizacion = GenerarDocumentoGrafico[0].Fac_Num_Aut;
                }

                //Notas de Curacion
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
                            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_Notas.rdlc";
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
                    U.Unificar_Estructura_2(@"C:\CXN\Reportes\Notas\",
                                          1.ToString(),
                                          totalArchivos.ToString(),
                                          Carpeta,
                                          "PDX_" + CIA.Com_Identificacion + "_" + HomologoFac + ".pdf");

                    R.Dispose();

                    foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Notas\", "*.pdf"))
                    {
                        BorrarArchivoConReintentos(archivo);
                    }
                }

                //Historias Clinicas
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
                    U.Unificar_Estructura_2(@"C:\CXN\Reportes\Historias\",
                                          1.ToString(),
                                          totalArchivos2.ToString(),
                                          Carpeta,
                                          "HEV_" + CIA.Com_Identificacion + "_" + HomologoFac + ".pdf");

                    R2.Dispose();

                    foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Historias\", "*.pdf"))
                    {
                        BorrarArchivoConReintentos(archivo);
                    }
                }

                //Cambios de Manejo
                if (getIdCamManejo != null && getIdCamManejo.Count > 0)
                {
                    ReportViewer R3 = new ReportViewer();
                    int Contador3 = 1;
                    int totalArchivos = getIdCamManejo.Count;

                    foreach (int i in getIdCamManejo)
                    {
                        List<ReportCMAN> H_HCCMAN = repoReportes.CambiosManejo(i);
                        if (H_HCCMAN != null)
                        {
                            R3.LocalReport.DataSources.Clear();
                            R3.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CMAN", H_HCCMAN));
                            R3.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_CMAN.rdlc";
                            R3.SetDisplayMode(DisplayMode.PrintLayout);
                            R3.ZoomMode = ZoomMode.Percent;
                            R3.ZoomPercent = 100;
                            R3.Font = new System.Drawing.Font("Arial", 7);
                            R3.LocalReport.EnableExternalImages = true;
                            R3.RefreshReport();
                            //maestro.Visible = true;
                            R3.Dock = System.Windows.Forms.DockStyle.Fill;

                            byte[] bytes = R3.LocalReport.Render("PDF");
                            FileStream fss = new FileStream("C:\\CXN\\Reportes\\Notas\\" + Contador3.ToString() + ".pdf", FileMode.Create);
                            fss.Write(bytes, 0, bytes.Length);
                            fss.Close();
                            Contador3 = Contador3 + 1;
                        }
                    }

                    Contador3 = Contador3 - 1;
                    Comunes.UnificadorPDFMasivo U = new Comunes.UnificadorPDFMasivo();
                    U.Unificar_Estructura_2(@"C:\CXN\Reportes\Notas\",
                                          1.ToString(),
                                          totalArchivos.ToString(),
                                          Carpeta,
                                          "OPF_" + CIA.Com_Identificacion + "_" + HomologoFac + ".pdf");

                    R3.Dispose();

                    foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Notas\", "*.pdf"))
                    {
                        BorrarArchivoConReintentos(archivo);
                    }
                }

                if (repoFacturacion.getValCuotasReceived(FacZamenis, Datos.Fac_Cia) > 0)
                {
                    //hoja de cuotas
                    Cuotas(Carpeta, FacZamenis, IdPaciente, NumAutorizacion, HomologoFac);
                }
                else
                {
                    //hoja de firmas
                    Firmas(Carpeta, FacZamenis, IdPaciente, NumAutorizacion, HomologoFac);
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Firmas(string Carpet, int FacZamenis, int IdPaciente, string NumAutorizacion, string HomologoFac)
        {
            try
            {
                CXN_CIA DatNombre = repoCompañia.getPrestadorbyCode(Datos.Fac_Cia);
                CXN_PACIENTES pacData = repoPacientes.LlamarPacientebyId(IdPaciente);
                CXN_ASEGURADORA aseData = repoAseguradoras.getInfoFromAsebyCode(pacData.Pac_Aseguradora);
                List<int> Admisiones = repoFacturacion.getAdmitionsByFac(FacZamenis, Datos.Fac_Cia);

                if (Admisiones != null)
                {
                    List<FirmasR> lista = new List<FirmasR>();
                    int Contador = 1;

                    foreach (var i in Admisiones)
                    {
                        otrosDatosPacienteHorario dataAdm = repoAgenda.cargarAdmision(i, "'H'");

                        CXN_FIRMASDIGITALES fTemp = repoFirmas.getFirmas(i);
                        byte[] firmaByte = null;

                        if (fTemp == null)
                        {
                            firmaByte = Convert.FromBase64String(repoFirmas.ImageNull());
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
                            Com_Direccion = NumAutorizacion, //autorizacion
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
                    FileStream fss = new FileStream(Carpet + "\\CRC_" + DatNombre.Com_Identificacion.ToString() + "_" + HomologoFac + ".pdf", FileMode.Create);
                    fss.Write(bytes, 0, bytes.Length);
                    fss.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Cuotas(string Carpet, int FacZamenis, int IdPaciente, string NumAutorizacion, string HomologoFac)
        {
            try
            {
                List<otrosDatosPacienteHorario> H = new List<otrosDatosPacienteHorario>();
                CXN_CIA DataCompany = repoCompañia.getPrestadorbyCode(Datos.Fac_Cia);
                CXN_PACIENTES DataPaciente = repoPacientes.LlamarPacientebyId(IdPaciente);
                List<int> Admisiones = repoFacturacion.getAdmitionsByFac(FacZamenis, Datos.Fac_Cia);

                foreach (int numRC in Admisiones)
                {
                    otrosDatosPacienteHorario otherData = repoAgenda.cargarAdmision(numRC, "'H','P'");

                    (int Valor, string Concepto) dato = repoCaja.getValRcCaja(numRC);
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
                FileStream fss = new FileStream(Carpet + "\\CRC_" + DataCompany.Com_Identificacion.ToString() + "_" + HomologoFac + ".pdf", FileMode.Create);
                fss.Write(bytes, 0, bytes.Length);
                fss.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        #endregion

        private void boton1_Click(object sender, EventArgs e)
        {
            CargarFacturas();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Encabezados();

            switch (comboBox1.Text)
            {
                case "Facturas Aseguradoras y Particulares":
                    btnGenerar.Visible = true;
                    btnEnviarPDF.Visible = true;
                    break;

                case "Facturas de Recaudo de Efectivo Recepcion":
                    btnGenerar.Visible = true;
                    btnEnviarPDF.Visible = true;
                    break;

                case "Facturas de Ventas Recepcion":
                    btnGenerar.Visible = true;
                    btnEnviarPDF.Visible = true;
                    break;

                case "Notas Credito":
                    btnGenerar.Visible = false;
                    btnEnviarPDF.Visible = true;
                    break;

                default:
                    btnGenerar.Visible = false;
                    btnEnviarPDF.Visible = false;
                    break;
            }
        }
    }
}
