using DocumentosElectronicos;
using DocumentosElectronicos.Controlador;
using DocumentosElectronicos.Request;
using DocumentosElectronicos.Servicio;

using Domain;
using Domain.CXN;

using FormAndControls;

using Microsoft.Reporting.WinForms;
using Microsoft.Web.WebView2.Core;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron.EmbededFac
{
    public partial class GenerarFacturaXML : Forma2
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        public extern static void ReleaseCapturing();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        public extern static void SendMessageMove(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private readonly ICompañia repoCia = new MCompañia();
        private readonly IGenerales repoGenerales = new MGenerales();
        private readonly IConfSystem repoConfSystem = new MConfSystem();
        private readonly static IFacElectron repoFelectron = new MFacElectron();
        private readonly static IHelisa repoWSClients = new MHelisa();
        private readonly static IPacientes repoPacs = new MPacientes();
        private readonly static IRcCaja repoRcCaja = new MRcCaja();

        //private WebView2 webView;
        private int Cia;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                //MessageBox.Show("Esta ventana no puede cerrarse desde aquí.");
            }

            base.OnFormClosing(e);
        }

        public GenerarFacturaXML(int cia)
        {
            InitializeComponent();
            InicializarWebView();
            Cia = cia;
        }
        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            (int W, int H) sizePredterminada = (1129, 665);

            int sizeActualH = this.Size.Height;
            int sizeActualW = this.Size.Width;

            if (sizePredterminada.W != sizeActualW && sizePredterminada.H != sizeActualH)
            {
                this.Size = new Size(sizePredterminada.W, sizePredterminada.H);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.WhiteSmoke;
                PanelTitulo.BorderStyle = BorderStyle.None;
                return;
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
                this.Bounds = Screen.FromControl(this).WorkingArea;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.WhiteSmoke;
                PanelTitulo.BorderStyle = BorderStyle.None;
                return;
            }
        }
        async void GenerarFacturaXML_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Facturacion Electronica";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = Properties.Resources.maximizar;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Size = new Size(24, 24);
            PanelTitulo.Controls.Add(pictureBox);
            pictureBox.Location = new Point(PanelTitulo.Width - 110, 11);
            pictureBox.Anchor = AnchorStyles.Top;
            pictureBox.Anchor = AnchorStyles.Right;
            pictureBox.Cursor = Cursors.Hand;
            pictureBox.BringToFront();

            pictureBox.Click += pictureBox1_Click;

            await webView.EnsureCoreWebView2Async(null);
        }

        private async void InicializarWebView()
        {
            /*webView = new WebView2
            {
                //Dock = DockStyle.Fill
            };*/

            Controls.Add(webView);

            await webView.EnsureCoreWebView2Async();

            // Configuración moderna (sin bloqueos ni advertencias)
            var settings = webView.CoreWebView2.Settings;
            settings.AreDefaultContextMenusEnabled = true;
            settings.AreDevToolsEnabled = true;
            settings.IsScriptEnabled = true;
            settings.AreHostObjectsAllowed = true;

            CXN_CIA dataPrest = repoCia.getPrestadorbyCode(Cia);

            string nitEnc = repoGenerales.Base64Encode(dataPrest.Com_Identificacion);

            if (!string.IsNullOrEmpty(dataPrest.Diferenciador))
            {
                nitEnc = repoGenerales.Base64Encode(dataPrest.Com_Identificacion + dataPrest.Diferenciador);
            }            

            string getURL = repoConfSystem.getListado()["EmbededSystemFE"];
            //webView.Source = new Uri("https://localhost:7064/Facturacion/GenerarFactura?nit=" + nitEnc);
            webView.Source = new Uri(getURL + "?nit=" + nitEnc);
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            webView.MouseDown += WebView_MouseDown;
        }
        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                MensajesGeneral MG2;

                string json = e.WebMessageAsJson;
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                string tipo = data.tipo;

                if (tipo == "cerrarFormulario")
                {
                    Task.Run(() =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            this.Dispose();
                            this.Close();
                        }));
                    });
                }
                else if (tipo == "generarFacturas")
                {
                    string clase = data.clase;
                    string selPrestador = data.selPrestador;
                    string facturas = data.facturas;

                    string[] facturasTotal = facturas.ToString().Split('\n');

                    Task.Run(() =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            List<CXN_FACTURA> listResultados = new List<CXN_FACTURA>();

                            foreach (var f in facturasTotal)
                            {
                                if (clase == "RCaja")
                                {
                                    int facturaZTemp = Convert.ToInt32(f);
                                    int facturaZ = repoRcCaja.getLastAdmition(facturaZTemp);

                                    List<RCCAJA> getFacturaZamenis = ExportarPDF.ExportarReciboCaja(facturaZ);
                                    CXN_CIA getDataFacElectron = repoCia.getPrestadorbyCode(Cia);

                                    var factura = new Factura
                                    {
                                        Encabezado = new Encabezado
                                        {
                                            llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                            nitemisor = getFacturaZamenis[0].EmpresaIdentificacion,
                                            codSucursal = getFacturaZamenis[0].EmpresaIdentificacion,
                                            tiporeceptor = getFacturaZamenis[0].TDocReceptor == "NIT" ? "1" : "2",
                                            tipoDocRec = Definiciones.tipoDocRec(99, Definiciones.tipoDocRec(88, repoPacs.getTipoDoc(getFacturaZamenis[0].TDocReceptor))),
                                            nitreceptor = getFacturaZamenis[0].PacienteAseguradora,
                                            nombrereceptor = getFacturaZamenis[0].PacienteNombre,
                                            mailreceptor = string.IsNullOrEmpty(getFacturaZamenis[0].Correo) ? "administrador@slsoft.net" : getFacturaZamenis[0].Correo,
                                            tipocomprobante = Definiciones.tipocomprobante("Factura de Venta Nacional"),
                                            extra1 = "",

                                            noresolucion = getDataFacElectron.Com_Resolucion_Electron,
                                            prefijo = getDataFacElectron.Com_Prefijo_Electron,
                                            folio = getDataFacElectron.Com_Doc_Electron.ToString(),

                                            fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                                            hora = DateTime.Now.ToString("HH:mm:ss"),
                                            moneda = "COP",
                                            subtotal = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].Valor),
                                            metodopago = Definiciones.metodopago("Contado"),
                                            mediopago = "10",
                                            fechavencimiento = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd"),
                                            terminospago = "1",
                                            baseimpuesto = ConvertirDecimal.ConvertirValor(0),
                                            totalsindescuento = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].Valor),
                                            totaldescuentos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestosretenidos = ConvertirDecimal.ConvertirValor(0),
                                            total = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].Valor),
                                            montoletra = getFacturaZamenis[0].Letras,
                                            tipoOpera = "10",
                                            ordenCompra = facturaZ.ToString(),
                                        },
                                        Detalle = new List<Detalle>(),
                                        Impuestos = new List<Impuesto>()
                                    };

                                    int comprobante = 1;

                                    foreach (RCCAJA i in getFacturaZamenis)
                                    {
                                        factura.Detalle.Add(new Detalle
                                        {
                                            llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                            idConcepto = comprobante.ToString(),
                                            cantidad = Convert.ToInt32(i.Cantidad).ToString("N2").Replace(",", "."),
                                            unidadmedida = "EA",
                                            descripcion = i.Observacion,
                                            precioUnitario = ConvertirDecimal.ConvertirValor(i.Valor),
                                            importe = ConvertirDecimal.ConvertirValor(Convert.ToInt32(i.Valor) * Convert.ToInt32(i.Cantidad)),
                                            impuestolinea = ConvertirDecimal.ConvertirValor(0),
                                            tasa = ConvertirDecimal.ConvertirValor(0).ToString("N2").Replace(",", "."),
                                            tipo = "01",
                                            baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            identificacionproductos = "100000"
                                        });

                                        comprobante++;
                                    }

                                    comprobante = 1;

                                    factura.Impuestos.Add(new Impuesto
                                    {
                                        llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                        idImpuesto = comprobante.ToString(),
                                        baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                        tasa = ConvertirDecimal.ConvertirValor(0),
                                        tipoImpuesto = "01",
                                        importe = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    PrintXML.Print(factura, getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron);

                                    Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1XML", Cia);
                                    if (dataWS == null)
                                    {
                                        listResultados.Add(new CXN_FACTURA
                                        {
                                            Fac_Tipo_Doc = "Caja",
                                            Fac_Estado = "Error",
                                            Fac_Num_Fac = facturaZ,
                                            Fac_Cia = Cia,
                                            Homologo = "",
                                            Fac_Observa = "No hay resultados para login de Facturacion Electronica"
                                        });
                                    }
                                    else
                                    {
                                        string TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                        if (string.IsNullOrEmpty(TokenGenerado))
                                        {
                                            bool generar = GenerateTokens.GenerarTokenNuevo(Cia);
                                            if (generar == false)
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "Caja",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = facturaZ,
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                                                });

                                                ResultadoRadicacion resultadoRadicacion2 = new ResultadoRadicacion(listResultados);
                                                resultadoRadicacion2.ShowDialog();

                                                return;
                                            }
                                            else
                                            {
                                                TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                            }
                                        }

                                        RequestRecibidoFElectronDecodificado sendXML = new RequestRecibidoFElectronDecodificado
                                        {
                                            TipoFactura = "Caja",
                                            Contraseña = dataWS["Pass"],
                                            Usuario = dataWS["User"],
                                            Factura = factura,
                                            ClaveTecnica = TokenGenerado
                                        };

                                        GenerarXML G = new GenerarXML();
                                        var res = G.SendXML(sendXML, Program.URLApiConexion).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.StatusCode == "00" && res.error == "")
                                            {
                                                string resolucion = "Res. DIAN No. " + getDataFacElectron.Com_Resolucion_Electron + " Habilitada para Facturación Electrónica de " + Convert.ToDateTime(getDataFacElectron.Com_Fecha_Electron).ToString("yyyy-MM-dd") + ", " + getDataFacElectron.Com_Numeracion_Electron;

                                                UpdateFacturaElectronica datosNuevosFacElectron = new UpdateFacturaElectronica
                                                {
                                                    FacturaElectronica = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                                    FacturaZamenis = getFacturaZamenis[0].RC_ID,
                                                    Hora = DateTime.Now,
                                                    Fecha = DateTime.Now.Date,
                                                    Cufe = res.cufe,
                                                    Resolucion = resolucion,
                                                    ResolucionNumeracion = getDataFacElectron.Com_Numeracion_Electron,
                                                    Prestador = Cia
                                                };

                                                bool uRc = ActualizarDocumentos.AddHomologoCaja(datosNuevosFacElectron);
                                                if (uRc != true)
                                                {
                                                    listResultados.Add(new CXN_FACTURA
                                                    {
                                                        Fac_Tipo_Doc = "Caja",
                                                        Fac_Estado = "Novedad",
                                                        Fac_Num_Fac = facturaZ,
                                                        Fac_Cia = Cia,
                                                        Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                        Fac_Observa = "No se logro homologar el PDF"
                                                    });
                                                }
                                                else
                                                {
                                                    int NueCons = getDataFacElectron.Com_Doc_Electron + 1;
                                                    repoCia.ConsecutivoActualiza(Cia, "CONSELECTRON", NueCons);

                                                    List<RCCAJA> Exportar = ExportarPDF.ExportarReciboCaja(getFacturaZamenis[0].RC_ID);
                                                    if (Exportar == null)
                                                    {
                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "Caja",
                                                            Fac_Estado = "Novedad",
                                                            Fac_Num_Fac = facturaZ,
                                                            Fac_Cia = Cia,
                                                            Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                            Fac_Observa = "Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "Caja",
                                                            Fac_Estado = "OK",
                                                            Fac_Num_Fac = facturaZ,
                                                            Fac_Cia = Cia,
                                                            Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                            Fac_Observa = "Factura Generada Exitosamente consulte el PDF en la carpeta de ReportesDIAN"
                                                        });

                                                        ReportViewer RCaja = new ReportViewer();

                                                        var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                                                        RCaja.LocalReport.DataSources.Clear();
                                                        RCaja.LocalReport.DataSources.Add(new ReportDataSource("ReciboCajaDataset", Exportar));
                                                        RCaja.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_RcCajaImpTermica.rdlc";
                                                        RCaja.SetDisplayMode(DisplayMode.PrintLayout);
                                                        RCaja.ZoomMode = ZoomMode.Percent;
                                                        RCaja.ZoomPercent = 100;
                                                        RCaja.Font = new System.Drawing.Font("Arial", 7);
                                                        RCaja.LocalReport.EnableExternalImages = true;
                                                        RCaja.RefreshReport();
                                                        RCaja.Dock = System.Windows.Forms.DockStyle.Fill;

                                                        byte[] bytes = RCaja.LocalReport.Render("PDF");

                                                        string GetCUFE = ActualizarDocumentos.GetCUFE(datosNuevosFacElectron.FacturaElectronica, Cia, "Caja");
                                                        //Radicar PDF
                                                        GenerateXMLPDF.GenerarBase64PDF(bytes, Cia, GetCUFE, datosNuevosFacElectron.FacturaElectronica, sendXML.ClaveTecnica);

                                                        FileStream fss = new FileStream("C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\FEV_" + getDataFacElectron.Com_Identificacion + "_" + datosNuevosFacElectron.FacturaElectronica + ".pdf", FileMode.Create);
                                                        fss.Write(bytes, 0, bytes.Length);
                                                        fss.Close();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "Caja",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = facturaZ,
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = res.error
                                                });

                                                string jsone2 = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                                                webView.CoreWebView2.PostWebMessageAsJson(jsone2);

                                                ResultadoRadicacion R2 = new ResultadoRadicacion(listResultados);
                                                R2.ShowDialog();

                                                return;
                                            }
                                        }
                                        else
                                        {
                                            listResultados.Add(new CXN_FACTURA
                                            {
                                                Fac_Tipo_Doc = "Caja",
                                                Fac_Estado = "Error",
                                                Fac_Num_Fac = facturaZ,
                                                Fac_Cia = Cia,
                                                Homologo = "",
                                                Fac_Observa = "Sin Respuesta"
                                            });
                                        }
                                    }
                                }
                                else if (clase == "Ventas")
                                {
                                    int facturaZ = Convert.ToInt32(f);
                                    List<FacturacionRpt> getFacturaZamenis = ExportarPDF.ExportarFacturaVentas(facturaZ, Cia, "OP");
                                    CXN_CIA getDataFacElectron = repoCia.getPrestadorbyCode(Cia);

                                    int vrimpuestosretenidos = 0;
                                    int vrNeto = getFacturaZamenis[0].VrNetoaPagar; //aqui taigo este total sin descuentos, no los estoy reportando cuotas, copagos, etc para reporarlos se cambia a VrTotalFac pero ahi que arreglarlo cuando lo trae
                                    string letra = getFacturaZamenis[0].ValorLetras;

                                    if (repoConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                                    {
                                        vrimpuestosretenidos = getFacturaZamenis[0].VrFUENTE + getFacturaZamenis[0].VrICA;
                                        vrNeto = getFacturaZamenis[0].VrNetoaPagar - getFacturaZamenis[0].VrFUENTE - getFacturaZamenis[0].VrICA;
                                        letra = getFacturaZamenis[0].LetraImpuestos;
                                    }

                                    var factura = new Factura
                                    {
                                        Encabezado = new Encabezado
                                        {
                                            llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                            nitemisor = getFacturaZamenis[0].EmpresaIdentificacion,
                                            codSucursal = "",
                                            tiporeceptor = getFacturaZamenis[0].Com_Direccion == "NIT" ? "1" : "2",
                                            tipoDocRec = Definiciones.tipoDocRec(88, Definiciones.tipoDocRec(88, repoPacs.getTipoDoc(getFacturaZamenis[0].Com_Direccion))),
                                            nitreceptor = getFacturaZamenis[0].PacienteAseguradora,
                                            nombrereceptor = getFacturaZamenis[0].PacienteNombre,
                                            mailreceptor = string.IsNullOrEmpty(getFacturaZamenis[0].ProfesionalNombre) ? "administrador@slsoft.net" : repoPacs.ValidaEmail(getFacturaZamenis[0].ProfesionalNombre) == false ? "administrador@slsoft.net" : getFacturaZamenis[0].ProfesionalNombre,
                                            tipocomprobante = Definiciones.tipocomprobante("Factura de Venta Nacional"),

                                            noresolucion = getDataFacElectron.Com_Resolucion_Electron,
                                            prefijo = getDataFacElectron.Com_Prefijo_Electron,
                                            folio = getDataFacElectron.Com_Doc_Electron.ToString(),

                                            fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                                            hora = DateTime.Now.ToString("HH:mm:ss"),
                                            moneda = "COP",
                                            subtotal = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].VrNetoaPagar),
                                            metodopago = Definiciones.metodopago(getFacturaZamenis[0].MetodoP),
                                            mediopago = Definiciones.medioPago(getFacturaZamenis[0].MedioP),
                                            fechavencimiento = DateTime.Now.AddDays(getFacturaZamenis[0].Dias).ToString("yyyy-MM-dd"),
                                            terminospago = "30",
                                            baseimpuesto = ConvertirDecimal.ConvertirValor(0),
                                            totalsindescuento = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].VrNetoaPagar),
                                            totaldescuentos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestosretenidos = ConvertirDecimal.ConvertirValor(vrimpuestosretenidos),
                                            total = ConvertirDecimal.ConvertirValor(vrNeto),
                                            montoletra = letra,
                                            tipoOpera = "10", //Estandar para ventas
                                            ordenCompra = facturaZ.ToString(),
                                        },
                                        Detalle = new List<Detalle>(),
                                        Impuestos = new List<Impuesto>()
                                    };

                                    int comprobante = 1;

                                    foreach (FacturacionRpt i in getFacturaZamenis)
                                    {
                                        factura.Detalle.Add(new Detalle
                                        {
                                            llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                            idConcepto = comprobante.ToString(),
                                            cantidad = Convert.ToInt32(i.CantidadProd).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            unidadmedida = "EA",
                                            descripcion = i.ItemProd,
                                            precioUnitario = ConvertirDecimal.ConvertirValor(i.VrUnitarioProd),
                                            importe = ConvertirDecimal.ConvertirValor(Convert.ToInt32(i.CantidadProd) * Convert.ToInt32(i.VrUnitarioProd)),
                                            impuestolinea = ConvertirDecimal.ConvertirValor(0),
                                            tasa = ConvertirDecimal.ConvertirValor(0).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            tipo = "01", //Impuesto sobre las ventas
                                            baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            identificacionproductos = i.CodigoProd.ToString(),
                                        });

                                        comprobante++;
                                    }

                                    comprobante = 1;

                                    factura.Impuestos.Add(new Impuesto
                                    {
                                        llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                        idImpuesto = comprobante.ToString(),
                                        baseimpuestos = ConvertirDecimal.ConvertirValor(0), //ConvertirValor(detalleFactura[0].VrNetoaPagar),
                                        tasa = ConvertirDecimal.ConvertirValor(0),
                                        tipoImpuesto = "01",
                                        importe = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    PrintXML.Print(factura, getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron);

                                    Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1XML", Cia);
                                    if (dataWS == null)
                                    {
                                        listResultados.Add(new CXN_FACTURA
                                        {
                                            Fac_Tipo_Doc = "Ventas",
                                            Fac_Estado = "Error",
                                            Fac_Num_Fac = Convert.ToInt32(facturaZ),
                                            Fac_Cia = Cia,
                                            Homologo = "",
                                            Fac_Observa = "No hay resultados para login de Facturacion Electronica"
                                        });
                                    }
                                    else
                                    {
                                        string TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                        if (string.IsNullOrEmpty(TokenGenerado))
                                        {
                                            bool generar = GenerateTokens.GenerarTokenNuevo(Cia);
                                            if (generar == false)
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "Ventas",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = facturaZ,
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                                                });

                                                ResultadoRadicacion resultadoRadicacion2 = new ResultadoRadicacion(listResultados);
                                                resultadoRadicacion2.ShowDialog();

                                                return;
                                            }
                                            else
                                            {
                                                TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                            }
                                        }

                                        RequestRecibidoFElectronDecodificado sendXML = new RequestRecibidoFElectronDecodificado
                                        {
                                            TipoFactura = "Ventas",
                                            Contraseña = dataWS["Pass"],
                                            Usuario = dataWS["User"],
                                            Factura = factura,
                                            ClaveTecnica = TokenGenerado
                                        };

                                        GenerarXML G = new GenerarXML();
                                        var res = G.SendXML(sendXML, Program.URLApiConexion).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.StatusCode == "00" && res.error == "")
                                            {
                                                string resolucion = "Res. DIAN No. " + getDataFacElectron.Com_Resolucion_Electron + " Habilitada para Facturación Electrónica de " + Convert.ToDateTime(getDataFacElectron.Com_Fecha_Electron).ToString("yyyy-MM-dd") + ", " + getDataFacElectron.Com_Numeracion_Electron;

                                                UpdateFacturaElectronica datosNuevosFacElectron = new UpdateFacturaElectronica
                                                {
                                                    FacturaElectronica = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                                    FacturaZamenis = facturaZ,
                                                    Hora = DateTime.Now,
                                                    Fecha = DateTime.Now.Date,
                                                    Cufe = res.cufe,
                                                    Resolucion = resolucion,
                                                    ResolucionNumeracion = getDataFacElectron.Com_Numeracion_Electron,
                                                    Prestador = Cia
                                                };

                                                bool uRc = ActualizarDocumentos.AddHomologoVentas(datosNuevosFacElectron);
                                                if (uRc != true)
                                                {
                                                    listResultados.Add(new CXN_FACTURA
                                                    {
                                                        Fac_Tipo_Doc = "Ventas",
                                                        Fac_Estado = "Novedad",
                                                        Fac_Num_Fac = Convert.ToInt32(facturaZ),
                                                        Fac_Cia = Cia,
                                                        Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                        Fac_Observa = "No se logro homologar el PDF"
                                                    });
                                                }
                                                else
                                                {
                                                    int NueCons = getDataFacElectron.Com_Doc_Electron + 1;
                                                    repoCia.ConsecutivoActualiza(Cia, "CONSELECTRON", NueCons);

                                                    List<FacturacionRpt> Exportar = ExportarPDF.ExportarFacturaVentas(facturaZ, Cia, "OP");
                                                    if (Exportar == null)
                                                    {
                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "Ventas",
                                                            Fac_Estado = "Novedad",
                                                            Fac_Num_Fac = Convert.ToInt32(facturaZ),
                                                            Fac_Cia = Cia,
                                                            Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                            Fac_Observa = "Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        List<FacturasR> rTemp = new List<FacturasR>();

                                                        string QrElectron = "NumFac:" + datosNuevosFacElectron.FacturaElectronica + "\r\n" +
                                                                  "FecFac:" + Convert.ToDateTime(Exportar[0].FechaBase).ToString("yyyy-MM-dd") + "\r\n" +
                                                                  "HorFac:" + Convert.ToDateTime(Exportar[0].Hora).ToString("hh:mm:ss tt") + "\r\n" +
                                                                  "NitFac:" + Exportar[0].EmpresaIdentificacion.ToString() + "\r\n" +
                                                                  "DocAdq:" + Exportar[0].PacienteAseguradora + "\r\n" +
                                                                  "ValFac:" + Convert.ToInt32(Exportar[0].VrNetoaPagar) + "\r\n" + //total antes de iva
                                                                  "ValIva" + "0" + "\r\n" + //Total IVA 
                                                                  "ValOtroIm:" + "0" + "\r\n" +
                                                                  "ValTolFac" + Convert.ToInt32(Exportar[0].VrNetoaPagar) + "\r\n" +
                                                                  "CUFE:" + Exportar[0].CUFE + "\r\n" +
                                                                  "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=" + Exportar[0].CUFE;


                                                        Image Code_QR_Fac_CUFE = repoGenerales.CodifyQR(QrElectron);

                                                        foreach (var item in Exportar)
                                                        {
                                                            rTemp.Add(new FacturasR
                                                            {
                                                                VrFUENTE = item.VrFUENTE,
                                                                VrICA = item.VrICA,
                                                                PercentFUENTE = item.PercentFUENTE,
                                                                PercentICA = item.PercentICA,
                                                                LetrasImpuestos = item.LetraImpuestos,
                                                                VrNeto = item.TotalImpuestos,

                                                                Letras = item.ValorLetras,
                                                                Car_Cod = item.CodigoProd,
                                                                Car_Item = item.ItemProd,
                                                                Cantidad = Convert.ToInt32(item.CantidadProd),
                                                                Car_Val_Un = Convert.ToInt32(item.VrUnitarioProd),
                                                                Total = Convert.ToInt32(item.VrTotalProd),
                                                                EmpresaDireccion = item.EmpresaDireccion,
                                                                EmpresaTelefono = item.EmpresaTelefono,
                                                                EmpresaIdentificacion = item.EmpresaIdentificacion,
                                                                Fac_Num_Fac = 0,
                                                                PacienteNombre = item.PacienteNombre,
                                                                PacienteDireccion = item.PacienteDireccion,
                                                                PacienteIdentificacion = item.PacienteIdentificacion,
                                                                PacienteTelefono = item.PacienteTelefono,
                                                                PacienteAseguradora = item.PacienteNombre,
                                                                Ase_NitCia = item.PacienteIdentificacion,
                                                                Ase_DVNitCia = "",
                                                                FechaBase = item.FechaBase,
                                                                Fac_Fecha_Des = Convert.ToDateTime(item.FechaBase),
                                                                Fac_Fecha_Has = Convert.ToDateTime(item.FechaBase),
                                                                Fac_Num_Aut = "",
                                                                Ase_Telefono = item.PacienteTelefono,
                                                                Ase_Direccion = item.PacienteDireccion,
                                                                Fac_Res = item.Resolucion,
                                                                Fac_Observa = "",
                                                                Fac_Descuento = 0,
                                                                Fac_Total = Convert.ToInt32(item.VrNetoaPagar),
                                                                Fac_Neto = Convert.ToInt32(item.VrNetoaPagar),
                                                                Usuario = item.UsuarioFactura,
                                                                Com_Logo = item.Logo,
                                                                Cufe = item.CUFE,
                                                                QRCufe = repoGenerales.GetBytes(Code_QR_Fac_CUFE),
                                                                EmpresaNombre = item.NumFac.ToString(),

                                                                Com_Direccion = item.Com_Direccion, //TID
                                                                DocE_1 = item.PacienteAseguradora, //Idnum
                                                                DocE_2 = item.ProfesionalNombre, //pacemail
                                                                DocE_3 = item.ProfesionalNombre, //pacemail
                                                                DocE_4 = item.Com_Resolucion_Electron, //res
                                                                ProfesionalNombre = item.PrefijoElectron, //prefijoi
                                                                DocE_5 = Convert.ToInt32(item.NumElectron), //num

                                                                NombrePrestador = item.EmpresaNombre
                                                            });
                                                        }

                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "Ventas",
                                                            Fac_Estado = "OK",
                                                            Fac_Num_Fac = Convert.ToInt32(facturaZ),
                                                            Fac_Cia = Cia,
                                                            Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                            Fac_Observa = "Factura Generada Exitosamente consulte el PDF en la carpeta de reportesDIAN"
                                                        });


                                                        ReportViewer RVenta = new ReportViewer();
                                                        var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                                                        RVenta.LocalReport.DataSources.Clear();
                                                        RVenta.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", rTemp));

                                                        if (repoConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                                                        {
                                                            RVenta.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludIMPUESTOS.rdlc";
                                                        }
                                                        else
                                                        {
                                                            RVenta.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc";
                                                        }

                                                        RVenta.SetDisplayMode(DisplayMode.PrintLayout);
                                                        RVenta.ZoomMode = ZoomMode.Percent;
                                                        RVenta.ZoomPercent = 100;
                                                        RVenta.Font = new System.Drawing.Font("Arial", 7);
                                                        RVenta.LocalReport.EnableExternalImages = true;
                                                        RVenta.RefreshReport();
                                                        RVenta.Dock = System.Windows.Forms.DockStyle.Fill;

                                                        byte[] bytes = RVenta.LocalReport.Render("PDF");

                                                        string GetCUFE = ActualizarDocumentos.GetCUFE(datosNuevosFacElectron.FacturaElectronica, Cia, "Ventas");
                                                        //Radicar PDF
                                                        GenerateXMLPDF.GenerarBase64PDF(bytes, Cia, GetCUFE, datosNuevosFacElectron.FacturaElectronica, sendXML.ClaveTecnica);

                                                        FileStream fss = new FileStream("C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\FEV_" + getDataFacElectron.Com_Identificacion.ToString() + "_" + datosNuevosFacElectron.FacturaElectronica + ".pdf", FileMode.Create);
                                                        fss.Write(bytes, 0, bytes.Length);
                                                        fss.Close();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "Ventas",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = Convert.ToInt32(facturaZ),
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = res.error + " -- Error estado DIAN"
                                                });

                                                string jsone3 = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                                                webView.CoreWebView2.PostWebMessageAsJson(jsone3);

                                                ResultadoRadicacion R2 = new ResultadoRadicacion(listResultados);
                                                R2.ShowDialog();

                                                return;
                                            }
                                        }
                                        else
                                        {
                                            listResultados.Add(new CXN_FACTURA
                                            {
                                                Fac_Tipo_Doc = "Ventas",
                                                Fac_Estado = "Error",
                                                Fac_Num_Fac = Convert.ToInt32(facturaZ),
                                                Fac_Cia = Cia,
                                                Homologo = "",
                                                Fac_Observa = "Error en respuesta de la API: Sin Respuesta"
                                            });
                                        }
                                    }
                                }
                                else if (clase == "Aseguradora")
                                {
                                    int facturaZ = Convert.ToInt32(f);
                                    List<FacturasR> getFacturaZamenis = ExportarPDF.ExportarFacturaAseguradoras(facturaZ, Cia, "OP");
                                    CXN_CIA getDataFacElectron = repoCia.getPrestadorbyCode(Cia);

                                    int vrimpuestosretenidos = 0;
                                    int vrNeto = getFacturaZamenis[0].Fac_Total; //aqui taigo este total sin descuentos, no los estoy reportando cuotas, copagos, etc para reporarlos se cambia a Fac_Neto
                                    string letra = getFacturaZamenis[0].Letras;

                                    if (repoConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                                    {
                                        vrimpuestosretenidos = getFacturaZamenis[0].VrFUENTE + getFacturaZamenis[0].VrICA;
                                        vrNeto = getFacturaZamenis[0].Fac_Total - getFacturaZamenis[0].VrFUENTE - getFacturaZamenis[0].VrICA;
                                        letra = getFacturaZamenis[0].LetrasImpuestos;
                                    }

                                    var factura = new Factura
                                    {
                                        Encabezado = new Encabezado
                                        {
                                            llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                            nitemisor = getFacturaZamenis[0].EmpresaIdentificacion,
                                            codSucursal = getFacturaZamenis[0].EmpresaIdentificacion,
                                            tiporeceptor = "02",
                                            tipoDocRec = Definiciones.tipoDocRec(getFacturaZamenis[0].Admision, getFacturaZamenis[0].Admision == 99 || getFacturaZamenis[0].Admision == 88 ? getFacturaZamenis[0].Com_Direccion : "NI"),
                                            nitreceptor = getFacturaZamenis[0].Admision == 99 || getFacturaZamenis[0].Admision == 88 ? getFacturaZamenis[0].DocE_1 : getFacturaZamenis[0].Ase_NitCia,
                                            digitoverificacion = getFacturaZamenis[0].Admision == 99 || getFacturaZamenis[0].Admision == 88 ? "" : getFacturaZamenis[0].Ase_DVNitCia,
                                            nombrereceptor = getFacturaZamenis[0].Admision == 99 || getFacturaZamenis[0].Admision == 88 ? getFacturaZamenis[0].PacienteNombre : getFacturaZamenis[0].PacienteAseguradora,
                                            mailreceptor = getFacturaZamenis[0].Admision == 99 || getFacturaZamenis[0].Admision == 88 ? string.IsNullOrEmpty(getFacturaZamenis[0].DocE_2) ? "administrador@slsoft.net" : getFacturaZamenis[0].DocE_2 : string.IsNullOrEmpty(getFacturaZamenis[0].DocE_3) ? "administrador@slsoft.net" : getFacturaZamenis[0].DocE_3,
                                            tipocomprobante = Definiciones.tipocomprobante("Factura de Venta Nacional"),

                                            noresolucion = getDataFacElectron.Com_Resolucion_Electron,
                                            prefijo = getDataFacElectron.Com_Prefijo_Electron,
                                            folio = getDataFacElectron.Com_Doc_Electron.ToString(),

                                            mailreceptorcontacto = getFacturaZamenis[0].Admision == 99 || getFacturaZamenis[0].Admision == 88 ? getFacturaZamenis[0].DocE_2 : getFacturaZamenis[0].DocE_3,
                                            paisreceptor = "CO",
                                            fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                                            hora = DateTime.Now.ToString("HH:mm:ss"),
                                            moneda = "COP",
                                            subtotal = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].Fac_Total),
                                            metodopago = Definiciones.metodopago(getFacturaZamenis[0].MetodoP),
                                            mediopago = Definiciones.medioPago(getFacturaZamenis[0].MedioP),
                                            fechavencimiento = getFacturaZamenis[0].Admision == 99 || getFacturaZamenis[0].Admision == 88 ? DateTime.Now.ToString("yyyy-MM-dd") : DateTime.Now.AddDays(getFacturaZamenis[0].Dias).ToString("yyyy-MM-dd"),
                                            terminospago = "30",
                                            baseimpuesto = ConvertirDecimal.ConvertirValor(0),
                                            totalsindescuento = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].Fac_Total),
                                            totaldescuentos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestosretenidos = ConvertirDecimal.ConvertirValor(vrimpuestosretenidos),
                                            total = ConvertirDecimal.ConvertirValor(vrNeto),
                                            montoletra = letra,
                                            tipoOpera = "SS-CUFE",
                                            extra1 = getFacturaZamenis[0].Fac_Observa,
                                            ordenCompra = facturaZ.ToString().Trim(),
                                            periodoFacturacion = new periodoFacturacion
                                            {
                                                FechaInicial = Convert.ToDateTime(getFacturaZamenis[0].Fac_Fecha_Des).ToString("yyyy-MM-dd"),
                                                FechaFin = Convert.ToDateTime(getFacturaZamenis[0].Fac_Fecha_Has).ToString("yyyy-MM-dd")
                                            }
                                        },
                                        Detalle = new List<Detalle>(),
                                        Impuestos = new List<Impuesto>(),
                                        Salud = new List<Salud>(),
                                    };

                                    int comprobante = 1;

                                    foreach (var i in getFacturaZamenis)
                                    {
                                        factura.Detalle.Add(new Detalle
                                        {
                                            llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                            idConcepto = comprobante.ToString(),
                                            cantidad = Convert.ToInt32(i.Cantidad).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            unidadmedida = "EA",
                                            descripcion = i.Car_Item,
                                            precioUnitario = ConvertirDecimal.ConvertirValor(i.Car_Val_Un),
                                            importe = Convert.ToInt32(i.Cantidad) * ConvertirDecimal.ConvertirValor(i.Car_Val_Un),
                                            impuestolinea = ConvertirDecimal.ConvertirValor(0),
                                            tasa = ConvertirDecimal.ConvertirValor(0).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            tipo = "01",
                                            baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            identificacionproductos = i.Car_Cod.ToString(),
                                        });

                                        comprobante++;
                                    }

                                    comprobante = 1;

                                    factura.Impuestos.Add(new Impuesto
                                    {
                                        llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                        idImpuesto = comprobante.ToString(),
                                        baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                        tasa = ConvertirDecimal.ConvertirValor(0),
                                        tipoImpuesto = "01",
                                        importe = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    comprobante = 1;

                                    factura.Salud.Add(new Salud
                                    {
                                        llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                        codPresSS = getFacturaZamenis[0].CodPrestador,
                                        modConPag = Definiciones.modConPag(getFacturaZamenis[0].ModPago),
                                        cobPan = Definiciones.cobPan(getFacturaZamenis[0].Cobertura),
                                        numCont = "",
                                        numPol = "",
                                        copago = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].Copago),
                                        cuotaM = ConvertirDecimal.ConvertirValor(getFacturaZamenis[0].Fac_Descuento),
                                        cuotaR = ConvertirDecimal.ConvertirValor(0),
                                        pagosComp = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1XML", Cia);
                                    if (dataWS == null)
                                    {
                                        listResultados.Add(new CXN_FACTURA
                                        {
                                            Fac_Tipo_Doc = "Aseguradora",
                                            Fac_Estado = "Error",
                                            Fac_Num_Fac = facturaZ,
                                            Fac_Cia = Cia,
                                            Homologo = "",
                                            Fac_Observa = "No hay resultados para login de Facturacion Electronica"
                                        });
                                    }
                                    else
                                    {
                                        string TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                        if (string.IsNullOrEmpty(TokenGenerado))
                                        {
                                            bool generar = GenerateTokens.GenerarTokenNuevo(Cia);
                                            if (generar == false)
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "Aseguradora",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = facturaZ,
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                                                });

                                                ResultadoRadicacion resultadoRadicacion2 = new ResultadoRadicacion(listResultados);
                                                resultadoRadicacion2.ShowDialog();

                                                return;
                                            }
                                            else
                                            {
                                                TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                            }
                                        }

                                        RequestRecibidoFElectronDecodificado sendXML = new RequestRecibidoFElectronDecodificado
                                        {
                                            TipoFactura = "Aseguradora",
                                            Contraseña = dataWS["Pass"],
                                            Usuario = dataWS["User"],
                                            Factura = factura,
                                            ClaveTecnica = TokenGenerado
                                        };

                                        GenerarXML G = new GenerarXML();
                                        var res = G.SendXML(sendXML, Program.URLApiConexion).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.StatusCode == "00" && res.error == "")
                                            {
                                                string resolucion = "Res. DIAN No. " + getDataFacElectron.Com_Resolucion_Electron + " Habilitada para Facturación Electrónica de " + Convert.ToDateTime(getDataFacElectron.Com_Fecha_Electron).ToString("yyyy-MM-dd") + ", " + getDataFacElectron.Com_Numeracion_Electron;

                                                UpdateFacturaElectronica datosNuevosFacElectron = new UpdateFacturaElectronica
                                                {
                                                    FacturaElectronica = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                                    FacturaZamenis = facturaZ,
                                                    Hora = DateTime.Now,
                                                    Fecha = DateTime.Now.Date,
                                                    Cufe = res.cufe,
                                                    Resolucion = resolucion,
                                                    ResolucionNumeracion = getDataFacElectron.Com_Numeracion_Electron,
                                                    Prestador = Cia
                                                };

                                                bool uRc = ActualizarDocumentos.AddHomologoAseguradoras(datosNuevosFacElectron);
                                                if (uRc != true)
                                                {
                                                    listResultados.Add(new CXN_FACTURA
                                                    {
                                                        Fac_Tipo_Doc = "Aseguradora",
                                                        Fac_Estado = "Novedad",
                                                        Fac_Num_Fac = facturaZ,
                                                        Fac_Cia = Cia,
                                                        Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                        Fac_Observa = "No se logro homologar el PDF"
                                                    });
                                                }
                                                else
                                                {
                                                    int NueCons = getDataFacElectron.Com_Doc_Electron + 1;
                                                    repoCia.ConsecutivoActualiza(Cia, "CONSELECTRON", NueCons);

                                                    List<FacturasR> Exportar = ExportarPDF.ExportarFacturaAseguradoras(facturaZ, Cia, "OP");
                                                    if (Exportar == null)
                                                    {
                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "Aseguradora",
                                                            Fac_Estado = "Novedad",
                                                            Fac_Num_Fac = facturaZ,
                                                            Fac_Cia = Cia,
                                                            Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                            Fac_Observa = "Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        ReportViewer RepoTemp = new ReportViewer();

                                                        RepoTemp.LocalReport.DataSources.Clear();
                                                        RepoTemp.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", Exportar));

                                                        if (repoConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                                                        {
                                                            RepoTemp.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludIMPUESTOS.rdlc";
                                                        }
                                                        else
                                                        {
                                                            RepoTemp.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc";
                                                        }

                                                        RepoTemp.SetDisplayMode(DisplayMode.PrintLayout);
                                                        RepoTemp.ZoomMode = ZoomMode.Percent;
                                                        RepoTemp.ZoomPercent = 100;
                                                        RepoTemp.Font = new System.Drawing.Font("Arial", 7);
                                                        RepoTemp.LocalReport.EnableExternalImages = true;
                                                        RepoTemp.RefreshReport();
                                                        RepoTemp.Dock = System.Windows.Forms.DockStyle.Fill;

                                                        byte[] bytes = RepoTemp.LocalReport.Render("PDF");

                                                        string GetCUFE = ActualizarDocumentos.GetCUFE(datosNuevosFacElectron.FacturaElectronica, Cia, "AseguraOtras");
                                                        GenerateXMLPDF.GenerarBase64PDF(bytes, Cia, GetCUFE, datosNuevosFacElectron.FacturaElectronica, sendXML.ClaveTecnica);

                                                        FileStream fss = new FileStream("C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\FEV_" + getDataFacElectron.Com_Identificacion + "_" + datosNuevosFacElectron.FacturaElectronica + ".pdf", FileMode.Create);
                                                        fss.Write(bytes, 0, bytes.Length);
                                                        fss.Close();

                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "Aseguradora",
                                                            Fac_Estado = "OK",
                                                            Fac_Num_Fac = facturaZ,
                                                            Fac_Cia = Cia,
                                                            Homologo = datosNuevosFacElectron.FacturaElectronica,
                                                            Fac_Observa = "Factura Generada Exitosamente, el PDF lo puede consultar el C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\" + datosNuevosFacElectron.FacturaElectronica + ".pdf"
                                                        });
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "Aseguradora",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = facturaZ,
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = res.error + " -- : Respuesta inconveniente en servicio DIAN, intente nuevamente o mas tarde"
                                                });

                                                string jsone4 = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                                                webView.CoreWebView2.PostWebMessageAsJson(jsone4);

                                                ResultadoRadicacion R2 = new ResultadoRadicacion(listResultados);
                                                R2.ShowDialog();

                                                return;
                                            }
                                        }
                                        else
                                        {
                                            listResultados.Add(new CXN_FACTURA
                                            {
                                                Fac_Tipo_Doc = "Aseguradora",
                                                Fac_Estado = "Error",
                                                Fac_Num_Fac = facturaZ,
                                                Fac_Cia = Cia,
                                                Homologo = "",
                                                Fac_Observa = res.error + " -- : Sin Resupuesta API"
                                            });
                                        }
                                    }
                                }
                                else if (clase == "RCajaNC")
                                {
                                    int Cia = Convert.ToInt32(selPrestador);
                                    int NFacZ = Convert.ToInt32(f);
                                    string facturaE = repoFelectron.GetHomologoRcCaja(Cia, NFacZ);
                                    List<RCCAJA> temp = ExportarPDF.ExportarReciboCaja(Convert.ToInt32(f));
                                    CXN_CIA getDataFacElectron = repoCia.getPrestadorbyCode(Cia);
                                    string NumeroNCActual = getDataFacElectron.Com_Prefijo_Electron_NC.ToString() + getDataFacElectron.Com_Doc_Electron_NC.ToString();

                                    var factura = new DocumentosElectronicos.Request.Factura
                                    {
                                        Encabezado = new Encabezado
                                        {
                                            llaveComprobante = NumeroNCActual,
                                            nitemisor = temp[0].EmpresaIdentificacion,
                                            codSucursal = "",
                                            tiporeceptor = "2", // Natural
                                            tipoDocRec = Definiciones.tipoDocRec(88, "CC"),
                                            nitreceptor = temp[0].DocReceptor,
                                            digitoverificacion = "",
                                            nombrereceptor = temp[0].PacienteNombre,
                                            mailreceptor = string.IsNullOrEmpty(temp[0].Correo) ? "administrador@slsoft.net" : temp[0].Correo,
                                            tipocomprobante = "91", // nota credito

                                            noresolucion = getDataFacElectron.Com_Resolucion_Electron,
                                            prefijo = getDataFacElectron.Com_Prefijo_Electron_NC,
                                            folio = getDataFacElectron.Com_Doc_Electron_NC.ToString(),

                                            fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                                            hora = DateTime.Now.ToString("HH:mm:ss"),
                                            moneda = "COP",
                                            subtotal = ConvertirDecimal.ConvertirValor(temp[0].Valor),
                                            metodopago = "1", //Contado
                                            mediopago = "10", // Efectivo
                                            fechavencimiento = Convert.ToDateTime(temp[0].FechaRealElectron.AddDays(0)).ToString("yyyy-MM-dd"), //si es efectivo no aplica fecha de vencimiento
                                            terminospago = "30",
                                            baseimpuesto = ConvertirDecimal.ConvertirValor(0),
                                            totalsindescuento = ConvertirDecimal.ConvertirValor(temp[0].Valor),
                                            totaldescuentos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestosretenidos = ConvertirDecimal.ConvertirValor(0),
                                            total = ConvertirDecimal.ConvertirValor(temp[0].Valor),
                                            montoletra = temp[0].Letras,
                                            tipoOpera = "20", //Nota Credito
                                            extra1 = "Nota Credito Numero " + getDataFacElectron.Com_Prefijo_Electron_NC.ToString() + getDataFacElectron.Com_Doc_Electron_NC.ToString() + " asociada a la Factura Electronica Numero " + temp[0].PrefijoElectron + temp[0].NumeroElectron.ToString(),
                                            ordenCompra = temp[0].Recibo.ToString(),
                                            ncidfact = NumeroNCActual,
                                            nccod = "2", // Anulacion Total de Factura Electronica
                                            nciddoc = temp[0].PrefijoElectron.ToString() + temp[0].NumeroElectron.ToString(),
                                            ncuuid = temp[0].Com_Direccion,
                                            ncfecha = Convert.ToDateTime(temp[0].FechaRealElectron).ToString("yyyy-MM-dd"),
                                            ndidfact = "",
                                            ndcod = "",
                                            ndiddoc = "",
                                            nduuid = "",
                                            ndfecha = ""
                                        },
                                        Detalle = new List<Detalle>(),
                                        Impuestos = new List<Impuesto>(),
                                        Salud = new List<Salud>()
                                    };

                                    int comprobante = 1;


                                    foreach (RCCAJA i in temp)
                                    {
                                        factura.Detalle.Add(new Detalle
                                        {
                                            llaveComprobante = NumeroNCActual,
                                            idConcepto = comprobante.ToString(),
                                            cantidad = "1.00", //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            unidadmedida = "EA",
                                            descripcion = i.Observacion,
                                            precioUnitario = ConvertirDecimal.ConvertirValor(i.Valor),
                                            importe = ConvertirDecimal.ConvertirValor(i.Valor),
                                            impuestolinea = ConvertirDecimal.ConvertirValor(0),
                                            tasa = ConvertirDecimal.ConvertirValor(0).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            tipo = "01",
                                            baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            identificacionproductos = "100000",
                                        });

                                        comprobante++;
                                    }

                                    comprobante = 1;

                                    factura.Impuestos.Add(new Impuesto
                                    {
                                        llaveComprobante = NumeroNCActual,
                                        idImpuesto = comprobante.ToString(),
                                        baseimpuestos = ConvertirDecimal.ConvertirValor(0), //ConvertirValor(temp[0].Valor),
                                        tasa = ConvertirDecimal.ConvertirValor(0),
                                        tipoImpuesto = "01",
                                        importe = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1XML", Cia);
                                    if (dataWS == null)
                                    {
                                        listResultados.Add(new CXN_FACTURA
                                        {
                                            Fac_Tipo_Doc = "CajaNC",
                                            Fac_Estado = "Error",
                                            Fac_Num_Fac = Convert.ToInt32(f),
                                            Fac_Cia = Cia,
                                            Homologo = "",
                                            Fac_Observa = "No hay resultados para login de Facturacion Electronica"
                                        });
                                    }
                                    else
                                    {
                                        string TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                        if (string.IsNullOrEmpty(TokenGenerado))
                                        {
                                            bool generar = GenerateTokens.GenerarTokenNuevo(Cia);
                                            if (generar == false)
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "CajaNC",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = Convert.ToInt32(f),
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                                                });

                                                ResultadoRadicacion resultadoRadicacion2 = new ResultadoRadicacion(listResultados);
                                                resultadoRadicacion2.ShowDialog();

                                                return;
                                            }
                                            else
                                            {
                                                TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                            }
                                        }

                                        RequestRecibidoFElectronDecodificado sendXML = new RequestRecibidoFElectronDecodificado
                                        {
                                            TipoFactura = "CajaNC",
                                            Contraseña = dataWS["Pass"],
                                            Usuario = dataWS["User"],
                                            Factura = factura,
                                            ClaveTecnica = TokenGenerado
                                        };

                                        GenerarXML G = new GenerarXML();
                                        var res = G.SendXML(sendXML, Program.URLApiConexion).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.StatusCode == "00" && res.error == "")
                                            {
                                                string resolucion = "Res. DIAN No. " + getDataFacElectron.Com_Resolucion_Electron + " Habilitada para Facturación Electrónica de " + Convert.ToDateTime(getDataFacElectron.Com_Fecha_Electron).ToString("yyyy-MM-dd") + ", " + getDataFacElectron.Com_Numeracion_Electron;

                                                List<CXN_CARGOSNC> listTempCar = new List<CXN_CARGOSNC>();

                                                foreach (var i in factura.Detalle)
                                                {
                                                    string CantidadesTemp = i.cantidad.Value.ToString().Trim().Substring(0, i.cantidad.Value.ToString().Trim().Length - 3);
                                                    int Cantidades2 = Convert.ToInt32(CantidadesTemp);

                                                    listTempCar.Add(new CXN_CARGOSNC
                                                    {
                                                        Codigo = i.identificacionproductos,
                                                        Item = i.descripcion,
                                                        Cantidad = Cantidades2,
                                                        VrUnitario = Convert.ToInt32(i.precioUnitario),
                                                        VrTotal = Convert.ToInt32(i.importe),
                                                        Tipo = "CajaNC"
                                                    });
                                                }

                                                CXN_FACTURANC datosNuevosFacElectron = new CXN_FACTURANC
                                                {
                                                    FacturaElectronica = factura.Encabezado.nciddoc,
                                                    NumeroNC = NumeroNCActual,
                                                    OrdenPedido = Convert.ToInt32(factura.Encabezado.ordenCompra),
                                                    Usuario = Contenedor.UsuarioLogueado,
                                                    Cufe = res.cufe,
                                                    Resolucion = factura.Encabezado.noresolucion,
                                                    Prestador = Cia,
                                                    listaCargos = listTempCar
                                                };

                                                string insertarNC = repoFelectron.insertNC(datosNuevosFacElectron);
                                                if (insertarNC != "OK")
                                                {
                                                    listResultados.Add(new CXN_FACTURA
                                                    {
                                                        Fac_Tipo_Doc = "CajaNC",
                                                        Fac_Estado = "Error",
                                                        Fac_Num_Fac = Convert.ToInt32(f),
                                                        Fac_Cia = Cia,
                                                        Homologo = "",
                                                        Fac_Observa = factura.Encabezado.ordenCompra + ": ESTADO 1000 ZAMENIS: Se genero la radicacion correcta pero no se grabo el cufe en Bd, error en tabla"
                                                    });
                                                }
                                                else
                                                {
                                                    int NueCons = getDataFacElectron.Com_Doc_Electron_NC + 1;
                                                    repoCia.ConsecutivoActualiza(Cia, "CONSELECTRONNC", NueCons);

                                                    List<RCCAJA> Exportar = ExportarPDF.ExportarReciboCajaNC(Convert.ToInt32(factura.Encabezado.ordenCompra));
                                                    if (Exportar == null)
                                                    {
                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "CajaNC",
                                                            Fac_Estado = "Error",
                                                            Fac_Num_Fac = Convert.ToInt32(f),
                                                            Fac_Cia = Cia,
                                                            Homologo = "",
                                                            Fac_Observa = factura.Encabezado.ordenCompra + ": ESTADO 18 ZAMENIS: Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        GeneradorXML.DocPrestadorTemp = "";
                                                        GeneradorXML.PrefElectronTemp = "";
                                                        GeneradorXML.NumElectronTemp = "";

                                                        GeneradorXML.DocPrestadorTemp = getDataFacElectron.Com_Identificacion;
                                                        GeneradorXML.PrefElectronTemp = getDataFacElectron.Com_Prefijo_Electron_NC;
                                                        GeneradorXML.NumElectronTemp = getDataFacElectron.Com_Doc_Electron_NC.ToString();

                                                        GeneradorXML.rTempCaja = new List<RCCAJA>();
                                                        GeneradorXML.rTempCaja = Exportar;

                                                        GenerateXMLPDF.GeneratePDFCajaNC(Cia, sendXML.ClaveTecnica);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "CajaNC",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = Convert.ToInt32(f),
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "Error"
                                                });

                                                string jsone3 = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                                                webView.CoreWebView2.PostWebMessageAsJson(jsone3);

                                                ResultadoRadicacion Rw = new ResultadoRadicacion(listResultados);
                                                Rw.ShowDialog();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            listResultados.Add(new CXN_FACTURA
                                            {
                                                Fac_Tipo_Doc = "CajaNC",
                                                Fac_Estado = "Error",
                                                Fac_Num_Fac = Convert.ToInt32(f),
                                                Fac_Cia = Cia,
                                                Homologo = "",
                                                Fac_Observa = "Sin Respuesta"
                                            });
                                        }
                                    }
                                }
                                else if (clase == "VentasNC")
                                {
                                    int Cia = Convert.ToInt32(selPrestador);
                                    int NFacZ = Convert.ToInt32(f);
                                    string facturaE = repoFelectron.GetHomologoVentas(Cia, NFacZ);

                                    List<FacturacionRpt> temp = ExportarPDF.ExportarFacturaVentas(NFacZ, Cia, "OP");
                                    CXN_CIA getDataFacElectron = repoCia.getPrestadorbyCode(Cia);
                                    string NumeroNCActual = getDataFacElectron.Com_Prefijo_Electron_NC + getDataFacElectron.Com_Doc_Electron_NC;

                                    var factura = new DocumentosElectronicos.Request.Factura
                                    {
                                        Encabezado = new Encabezado
                                        {
                                            llaveComprobante = NumeroNCActual,
                                            nitemisor = temp[0].EmpresaIdentificacion,
                                            codSucursal = "",
                                            tiporeceptor = "2", // Natural
                                            tipoDocRec = Definiciones.tipoDocRec(88, "CC"),
                                            nitreceptor = temp[0].PacienteAseguradora,
                                            digitoverificacion = "",
                                            nombrereceptor = temp[0].PacienteNombre,
                                            mailreceptor = string.IsNullOrEmpty(temp[0].ProfesionalNombre) ? "administrador@slsoft.net" : temp[0].ProfesionalNombre,
                                            tipocomprobante = "91", // nota credito

                                            noresolucion = getDataFacElectron.Com_Resolucion_Electron,
                                            prefijo = getDataFacElectron.Com_Prefijo_Electron_NC,
                                            folio = getDataFacElectron.Com_Doc_Electron_NC.ToString(),

                                            fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                                            hora = DateTime.Now.ToString("HH:mm:ss"),
                                            moneda = "COP",
                                            subtotal = ConvertirDecimal.ConvertirValor(temp[0].VrNetoaPagar),
                                            metodopago = temp[0].MetodoP,
                                            mediopago = temp[0].MedioP,
                                            fechavencimiento = temp[0].FechaBase.AddDays(temp[0].Dias).ToString("yyyy-MM-dd"), //si es efectivo no aplica fecha de vencimiento
                                            terminospago = temp[0].Dias.ToString(),
                                            baseimpuesto = ConvertirDecimal.ConvertirValor(0),
                                            totalsindescuento = ConvertirDecimal.ConvertirValor(temp[0].VrNetoaPagar),
                                            totaldescuentos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestosretenidos = ConvertirDecimal.ConvertirValor(0),
                                            total = ConvertirDecimal.ConvertirValor(temp[0].VrNetoaPagar),
                                            montoletra = temp[0].ValorLetras,
                                            tipoOpera = "20", //Nota Credito
                                            extra1 = "Nota Credito Numero " + getDataFacElectron.Com_Prefijo_Electron_NC + getDataFacElectron.Com_Doc_Electron_NC.ToString() + " asociada a la Factura Electronica Numero " + temp[0].NumFac,
                                            ordenCompra = temp[0].Admision.ToString(),
                                            ncidfact = NumeroNCActual,
                                            nccod = "2", // Anulacion Total de Factura Electronica
                                            nciddoc = temp[0].NumFac.ToString(),
                                            ncuuid = temp[0].CUFE,
                                            ncfecha = Convert.ToDateTime(temp[0].FechaBase).ToString("yyyy-MM-dd"),
                                            ndidfact = "",
                                            ndcod = "",
                                            ndiddoc = "",
                                            nduuid = "",
                                            ndfecha = ""
                                        },
                                        Detalle = new List<Detalle>(),
                                        Impuestos = new List<Impuesto>(),
                                        Salud = new List<Salud>()
                                    };

                                    int comprobante = 1;


                                    foreach (FacturacionRpt i in temp)
                                    {
                                        int Cantidad = Convert.ToInt32(i.CantidadProd.ToString().Trim(), CultureInfo.InvariantCulture);

                                        factura.Detalle.Add(new Detalle
                                        {
                                            llaveComprobante = NumeroNCActual,
                                            idConcepto = comprobante.ToString(),
                                            cantidad = Cantidad.ToString() + ".00", //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            unidadmedida = "EA",
                                            descripcion = i.ItemProd,
                                            precioUnitario = ConvertirDecimal.ConvertirValor(i.VrUnitarioProd),
                                            importe = ConvertirDecimal.ConvertirValor(Cantidad) * ConvertirDecimal.ConvertirValor(i.VrUnitarioProd),
                                            impuestolinea = ConvertirDecimal.ConvertirValor(0),
                                            tasa = ConvertirDecimal.ConvertirValor(0).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            tipo = "01",
                                            baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            identificacionproductos = i.CodigoProd.ToString(),
                                        });

                                        comprobante++;
                                    }

                                    comprobante = 1;

                                    factura.Impuestos.Add(new Impuesto
                                    {
                                        llaveComprobante = NumeroNCActual,
                                        idImpuesto = comprobante.ToString(),
                                        baseimpuestos = ConvertirDecimal.ConvertirValor(0), //ConvertirValor(temp[0].VrNetoaPagar),
                                        tasa = ConvertirDecimal.ConvertirValor(0),
                                        tipoImpuesto = "01",
                                        importe = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1XML", Cia);
                                    if (dataWS == null)
                                    {
                                        listResultados.Add(new CXN_FACTURA
                                        {
                                            Fac_Tipo_Doc = "VentasNC",
                                            Fac_Estado = "Error",
                                            Fac_Num_Fac = Convert.ToInt32(f),
                                            Fac_Cia = Cia,
                                            Homologo = "",
                                            Fac_Observa = "No hay resultados para login de Facturacion Electronica"
                                        });
                                    }
                                    else
                                    {
                                        string TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                        if (string.IsNullOrEmpty(TokenGenerado))
                                        {
                                            bool generar = GenerateTokens.GenerarTokenNuevo(Cia);
                                            if (generar == false)
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "VentasNC",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = Convert.ToInt32(f),
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                                                });

                                                ResultadoRadicacion resultadoRadicacion2 = new ResultadoRadicacion(listResultados);
                                                resultadoRadicacion2.ShowDialog();

                                                return;
                                            }
                                            else
                                            {
                                                TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                            }
                                        }

                                        RequestRecibidoFElectronDecodificado sendXML = new RequestRecibidoFElectronDecodificado
                                        {
                                            TipoFactura = "Ventas",
                                            Contraseña = dataWS["Pass"],
                                            Usuario = dataWS["User"],
                                            Factura = factura,
                                            ClaveTecnica = TokenGenerado
                                        };

                                        GenerarXML G = new GenerarXML();
                                        var res = G.SendXML(sendXML, Program.URLApiConexion).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.StatusCode == "00" && res.error == "")
                                            {
                                                string resolucion = "Res. DIAN No. " + getDataFacElectron.Com_Resolucion_Electron + " Habilitada para Facturación Electrónica de " + Convert.ToDateTime(getDataFacElectron.Com_Fecha_Electron).ToString("yyyy-MM-dd") + ", " + getDataFacElectron.Com_Numeracion_Electron;

                                                List<CXN_CARGOSNC> listTempCar = new List<CXN_CARGOSNC>();

                                                foreach (var i in factura.Detalle)
                                                {
                                                    string CantidadesTemp = i.cantidad.Value.ToString().Trim().Substring(0, i.cantidad.Value.ToString().Trim().Length - 3);
                                                    int Cantidades2 = Convert.ToInt32(CantidadesTemp);

                                                    listTempCar.Add(new CXN_CARGOSNC
                                                    {
                                                        Codigo = i.identificacionproductos,
                                                        Item = i.descripcion,
                                                        Cantidad = Cantidades2,
                                                        VrUnitario = Convert.ToInt32(i.precioUnitario),
                                                        VrTotal = Convert.ToInt32(i.importe),
                                                        Tipo = "VENTAS"
                                                    });
                                                }

                                                CXN_FACTURANC datosNuevosFacElectron = new CXN_FACTURANC
                                                {
                                                    FacturaElectronica = factura.Encabezado.nciddoc,
                                                    NumeroNC = NumeroNCActual,
                                                    OrdenPedido = Convert.ToInt32(factura.Encabezado.ordenCompra),
                                                    Usuario = Contenedor.UsuarioLogueado,
                                                    Cufe = res.cufe,
                                                    Resolucion = factura.Encabezado.noresolucion,
                                                    Prestador = Cia,
                                                    listaCargos = listTempCar
                                                };

                                                string insertarNC = repoFelectron.insertNC(datosNuevosFacElectron);
                                                if (insertarNC != "OK")
                                                {
                                                    listResultados.Add(new CXN_FACTURA
                                                    {
                                                        Fac_Tipo_Doc = "VentasNC",
                                                        Fac_Estado = "Error",
                                                        Fac_Num_Fac = Convert.ToInt32(f),
                                                        Fac_Cia = Cia,
                                                        Homologo = "",
                                                        Fac_Observa = factura.Encabezado.ordenCompra + ": ESTADO 1000 ZAMENIS: Se genero la radicacion correcta pero no se grabo el cufe en Bd, error en tabla"
                                                    });
                                                }
                                                else
                                                {
                                                    int NueCons = getDataFacElectron.Com_Doc_Electron_NC + 1;
                                                    repoCia.ConsecutivoActualiza(Cia, "CONSELECTRONNC", NueCons);

                                                    List<FacturacionRpt> Exportar = ExportarPDF.ExportarFacturaVentasNC(Convert.ToInt32(factura.Encabezado.ordenCompra), Cia, "OP", factura.Encabezado.llaveComprobante);
                                                    if (Exportar == null)
                                                    {
                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "VentasNC",
                                                            Fac_Estado = "Error",
                                                            Fac_Num_Fac = Convert.ToInt32(f),
                                                            Fac_Cia = Cia,
                                                            Homologo = "",
                                                            Fac_Observa = factura.Encabezado.ordenCompra + ": ESTADO 18 ZAMENIS: Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        GeneradorXML.rTemp = new List<FacturasR>();

                                                        foreach (var item in Exportar)
                                                        {
                                                            GeneradorXML.rTemp.Add(new FacturasR
                                                            {
                                                                Letras = item.ValorLetras,
                                                                Car_Cod = item.CodigoProd,
                                                                Car_Item = item.ItemProd,
                                                                Cantidad = Convert.ToInt32(item.CantidadProd),
                                                                Car_Val_Un = Convert.ToInt32(item.VrUnitarioProd),
                                                                Total = Convert.ToInt32(item.VrTotalProd),
                                                                EmpresaDireccion = item.EmpresaDireccion,
                                                                EmpresaTelefono = item.EmpresaTelefono,
                                                                EmpresaIdentificacion = item.EmpresaIdentificacion,
                                                                Fac_Num_Fac = 0,
                                                                PacienteNombre = item.PacienteNombre,
                                                                PacienteDireccion = item.PacienteDireccion,
                                                                PacienteIdentificacion = item.PacienteIdentificacion,
                                                                PacienteTelefono = item.PacienteTelefono,
                                                                PacienteAseguradora = item.PacienteNombre,
                                                                Ase_NitCia = item.PacienteIdentificacion,
                                                                Ase_DVNitCia = "",
                                                                FechaBase = item.FechaBase,
                                                                Fac_Fecha_Des = Convert.ToDateTime(item.FechaBase),
                                                                Fac_Fecha_Has = Convert.ToDateTime(item.FechaBase),
                                                                Fac_Num_Aut = "",
                                                                Ase_Telefono = item.PacienteTelefono,
                                                                Ase_Direccion = item.PacienteDireccion,
                                                                Fac_Res = item.Resolucion,
                                                                Fac_Observa = "",
                                                                Fac_Descuento = 0,
                                                                Fac_Total = Convert.ToInt32(item.VrNetoaPagar),
                                                                Fac_Neto = Convert.ToInt32(item.VrNetoaPagar),
                                                                Usuario = item.UsuarioFactura,
                                                                Com_Logo = item.Logo,
                                                                Cufe = item.CUFE,
                                                                QRCufe = item.QRLogo,
                                                                EmpresaNombre = item.NumFac.ToString(),

                                                                Com_Direccion = item.Com_Direccion, //TID
                                                                DocE_1 = item.PacienteAseguradora, //Idnum
                                                                DocE_2 = item.ProfesionalNombre, //pacemail
                                                                DocE_3 = item.ProfesionalNombre, //pacemail
                                                                DocE_4 = item.Com_Resolucion_Electron, //res
                                                                ProfesionalNombre = factura.Encabezado.prefijo, //prefijoi
                                                                DocE_5 = Convert.ToInt32(factura.Encabezado.folio), //num

                                                                NombrePrestador = item.EmpresaNombre
                                                            });
                                                        }

                                                        GeneradorXML.DocPrestadorTemp = "";
                                                        GeneradorXML.PrefElectronTemp = "";
                                                        GeneradorXML.NumElectronTemp = "";

                                                        GeneradorXML.DocPrestadorTemp = getDataFacElectron.Com_Identificacion;
                                                        GeneradorXML.PrefElectronTemp = getDataFacElectron.Com_Prefijo_Electron_NC;
                                                        GeneradorXML.NumElectronTemp = getDataFacElectron.Com_Doc_Electron_NC.ToString();

                                                        GenerateXMLPDF.GeneratePDFVentasNC(Cia, sendXML.ClaveTecnica);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "VentasNC",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = Convert.ToInt32(f),
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "Error"
                                                });

                                                string jsone3 = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                                                webView.CoreWebView2.PostWebMessageAsJson(jsone3);

                                                ResultadoRadicacion Rw = new ResultadoRadicacion(listResultados);
                                                Rw.ShowDialog();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            listResultados.Add(new CXN_FACTURA
                                            {
                                                Fac_Tipo_Doc = "VentasNC",
                                                Fac_Estado = "Error",
                                                Fac_Num_Fac = Convert.ToInt32(f),
                                                Fac_Cia = Cia,
                                                Homologo = "",
                                                Fac_Observa = "Sin Respuesta"
                                            });
                                        }
                                    }
                                }
                                else if (clase == "AseguradoraNC")
                                {
                                    int Cia = Convert.ToInt32(selPrestador);
                                    int NFacZ = Convert.ToInt32(f);
                                    string facturaE = repoFelectron.GetHomologo(Cia, NFacZ);

                                    List<FacturasR> temp = ExportarPDF.ExportarFacturaAseguradoras(NFacZ, Cia, "OP");
                                    CXN_CIA getDataFacElectron = repoCia.getPrestadorbyCode(Cia);
                                    string llComprobante = getDataFacElectron.Com_Prefijo_Electron_NC + getDataFacElectron.Com_Doc_Electron_NC;

                                    var factura = new Factura
                                    {
                                        Encabezado = new Encabezado
                                        {
                                            llaveComprobante = llComprobante,
                                            nitemisor = temp[0].EmpresaIdentificacion,
                                            codSucursal = "",
                                            tiporeceptor = temp[0].Admision == 99 || temp[0].Admision == 88 ? "2" : "1", // Juridico
                                            tipoDocRec = temp[0].Admision == 99 || temp[0].Admision == 88 ? Definiciones.tipoDocRec(99, temp[0].ReteFuente) : "31", // Nit
                                            nitreceptor = temp[0].Admision == 99 || temp[0].Admision == 88 ? temp[0].DocE_1 : temp[0].Ase_NitCia,
                                            digitoverificacion = temp[0].Admision == 99 || temp[0].Admision == 88 ? "" : temp[0].Ase_DVNitCia,
                                            nombrereceptor = temp[0].Admision == 99 || temp[0].Admision == 88 ? temp[0].PacienteAseguradora : temp[0].PacienteNombre,
                                            mailreceptor = temp[0].Admision == 99 || temp[0].Admision == 88 ? string.IsNullOrEmpty(temp[0].DocE_2) ? "administrador@slsoft.net" : temp[0].DocE_2 : string.IsNullOrEmpty(temp[0].DocE_3) ? "administrador@slsoft.net" : temp[0].DocE_3,
                                            tipocomprobante = "91", // nota credito

                                            noresolucion = getDataFacElectron.Com_Resolucion_Electron,
                                            prefijo = getDataFacElectron.Com_Prefijo_Electron_NC,
                                            folio = getDataFacElectron.Com_Doc_Electron_NC.ToString(),

                                            mailreceptorcontacto = temp[0].Admision == 99 || temp[0].Admision == 88 ? temp[0].DocE_2 : temp[0].DocE_3,
                                            paisreceptor = "CO", //Colombia
                                            fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                                            hora = DateTime.Now.ToString("HH:mm:ss"),
                                            moneda = "COP",
                                            subtotal = ConvertirDecimal.ConvertirValor(temp[0].Fac_Total),
                                            metodopago = temp[0].MetodoP,
                                            mediopago = temp[0].MedioP,
                                            fechavencimiento = temp[0].FechaBase.AddDays(temp[0].Dias).ToString("yyyy-MM-dd"), //si es efectivo no aplica fecha de vencimiento
                                            terminospago = temp[0].Dias.ToString(),
                                            baseimpuesto = ConvertirDecimal.ConvertirValor(0),
                                            totalsindescuento = ConvertirDecimal.ConvertirValor(temp[0].Fac_Total),
                                            //totaldescuentos = ConvertirValor(temp[0].Fac_Descuento),
                                            totaldescuentos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            totalimpuestosretenidos = ConvertirDecimal.ConvertirValor(0),
                                            //total = ConvertirValor(temp[0].Fac_Total - ConvertirValor(temp[0].Fac_Descuento)),
                                            total = ConvertirDecimal.ConvertirValor(temp[0].Fac_Total),
                                            montoletra = temp[0].Letras,
                                            tipoOpera = "20", //Nota Credito
                                            extra1 = temp[0].Fac_Observa,
                                            extra2 = "Nota Credito Numero " + getDataFacElectron.Com_Prefijo_Electron_NC + getDataFacElectron.Com_Doc_Electron_NC.ToString() + " asociada a la Factura Electronica Numero " + temp[0].EmpresaNombre,
                                            ordenCompra = temp[0].Fac_Num_Fac.ToString(),
                                            periodoFacturacion = new periodoFacturacion
                                            {
                                                FechaInicial = temp[0].Fac_Fecha_Des.ToString("yyyy-MM-dd"),
                                                FechaFin = temp[0].Fac_Fecha_Has.ToString("yyyy-MM-dd")
                                            },
                                            ncidfact = getDataFacElectron.Com_Prefijo_Electron_NC + getDataFacElectron.Com_Doc_Electron_NC,
                                            nccod = "2", //Anulacion total factura electronicas
                                            nciddoc = temp[0].EmpresaNombre.ToString(),
                                            ncuuid = temp[0].Cufe,
                                            ncfecha = Convert.ToDateTime(temp[0].FechaBase).ToString("yyyy-MM-dd"),
                                            ndidfact = "",
                                            ndcod = "",
                                            ndiddoc = "",
                                            nduuid = "",
                                            ndfecha = ""
                                        },
                                        Detalle = new List<Detalle>(),
                                        Impuestos = new List<Impuesto>(),
                                        Salud = new List<Salud>()
                                    };

                                    int comprobante = 1;

                                    foreach (FacturasR i in temp)
                                    {
                                        factura.Detalle.Add(new Detalle
                                        {
                                            llaveComprobante = llComprobante,
                                            idConcepto = comprobante.ToString(),
                                            cantidad = ConvertirDecimal.ConvertirValor(i.Cantidad).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            unidadmedida = "EA",
                                            descripcion = i.Car_Item,
                                            precioUnitario = ConvertirDecimal.ConvertirValor(i.Car_Val_Un),
                                            importe = ConvertirDecimal.ConvertirValor(i.Cantidad) * ConvertirDecimal.ConvertirValor(i.Car_Val_Un),
                                            impuestolinea = ConvertirDecimal.ConvertirValor(0),
                                            tasa = ConvertirDecimal.ConvertirValor(0).ToString("N2").Replace(",", "."), //esta en el excel string pero segun ejemplo de xmls debe ser decimal
                                            tipo = "01",
                                            baseimpuestos = ConvertirDecimal.ConvertirValor(0),
                                            identificacionproductos = i.Car_Cod.ToString(),
                                        });

                                        comprobante++;
                                    }

                                    comprobante = 1;

                                    factura.Impuestos.Add(new Impuesto
                                    {
                                        llaveComprobante = llComprobante,
                                        idImpuesto = comprobante.ToString(),
                                        baseimpuestos = ConvertirDecimal.ConvertirValor(0), //ConvertirValor(temp[0].Fac_Total),
                                        tasa = ConvertirDecimal.ConvertirValor(0),
                                        tipoImpuesto = "01",
                                        importe = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    comprobante = 1;

                                    factura.Salud.Add(new Salud
                                    {
                                        llaveComprobante = llComprobante,
                                        codPresSS = temp[0].CodPrestador,
                                        modConPag = Definiciones.modConPag(temp[0].ModPago),
                                        cobPan = Definiciones.cobPan(temp[0].Cobertura),
                                        numCont = "",
                                        numPol = "",
                                        copago = ConvertirDecimal.ConvertirValor(temp[0].Copago),
                                        cuotaM = ConvertirDecimal.ConvertirValor(temp[0].Fac_Descuento),
                                        cuotaR = ConvertirDecimal.ConvertirValor(0),
                                        pagosComp = ConvertirDecimal.ConvertirValor(0)
                                    });

                                    Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1XML", Cia);
                                    if (dataWS == null)
                                    {
                                        listResultados.Add(new CXN_FACTURA
                                        {
                                            Fac_Tipo_Doc = "AseguradoraNC",
                                            Fac_Estado = "Error",
                                            Fac_Num_Fac = Convert.ToInt32(f),
                                            Fac_Cia = Cia,
                                            Homologo = "",
                                            Fac_Observa = "No hay resultados para login de Facturacion Electronica"
                                        });
                                    }
                                    else
                                    {
                                        string TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                        if (string.IsNullOrEmpty(TokenGenerado))
                                        {
                                            bool generar = GenerateTokens.GenerarTokenNuevo(Cia);
                                            if (generar == false)
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "AseguradoraNC",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = Convert.ToInt32(f),
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                                                });

                                                ResultadoRadicacion resultadoRadicacion2 = new ResultadoRadicacion(listResultados);
                                                resultadoRadicacion2.ShowDialog();

                                                return;
                                            }
                                            else
                                            {
                                                TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                                            }
                                        }

                                        RequestRecibidoFElectronDecodificado sendXML = new RequestRecibidoFElectronDecodificado
                                        {
                                            TipoFactura = "AseguradoraNC",
                                            Contraseña = dataWS["Pass"],
                                            Usuario = dataWS["User"],
                                            Factura = factura,
                                            ClaveTecnica = TokenGenerado
                                        };

                                        GenerarXML G = new GenerarXML();
                                        var res = G.SendXML(sendXML, Program.URLApiConexion).GetAwaiter().GetResult();
                                        if (res != null)
                                        {
                                            if (res.StatusCode == "00" && res.error == "")
                                            {
                                                string resolucion = "Res. DIAN No. " + getDataFacElectron.Com_Resolucion_Electron + " Habilitada para Facturación Electrónica de " + Convert.ToDateTime(getDataFacElectron.Com_Fecha_Electron).ToString("yyyy-MM-dd") + ", " + getDataFacElectron.Com_Numeracion_Electron;

                                                List<CXN_CARGOSNC> listTempCar = new List<CXN_CARGOSNC>();

                                                foreach (var i in factura.Detalle)
                                                {
                                                    string CantidadesTemp = i.cantidad.Value.ToString().Trim().Substring(0, i.cantidad.Value.ToString().Trim().Length - 3);
                                                    int Cantidades2 = Convert.ToInt32(CantidadesTemp);

                                                    listTempCar.Add(new CXN_CARGOSNC
                                                    {
                                                        Codigo = i.identificacionproductos,
                                                        Item = i.descripcion,
                                                        Cantidad = Cantidades2,
                                                        VrUnitario = Convert.ToInt32(i.precioUnitario),
                                                        VrTotal = Convert.ToInt32(i.importe),
                                                        Tipo = "Salud"
                                                    });
                                                }

                                                CXN_FACTURANC datosNuevosFacElectron = new CXN_FACTURANC
                                                {
                                                    FacturaElectronica = factura.Encabezado.nciddoc,
                                                    NumeroNC = factura.Encabezado.llaveComprobante,
                                                    OrdenPedido = Convert.ToInt32(factura.Encabezado.ordenCompra),
                                                    Usuario = Contenedor.UsuarioLogueado,
                                                    Cufe = res.cufe,
                                                    Resolucion = factura.Encabezado.noresolucion,
                                                    Prestador = Cia,
                                                    listaCargos = listTempCar
                                                };

                                                string insertarNC = repoFelectron.insertNC(datosNuevosFacElectron);
                                                if (insertarNC != "OK")
                                                {
                                                    listResultados.Add(new CXN_FACTURA
                                                    {
                                                        Fac_Tipo_Doc = "AseguradoraNC",
                                                        Fac_Estado = "Error",
                                                        Fac_Num_Fac = Convert.ToInt32(f),
                                                        Fac_Cia = Cia,
                                                        Homologo = "",
                                                        Fac_Observa = factura.Encabezado.ordenCompra + ": ESTADO 1000 ZAMENIS: Se genero la radicacion correcta pero no se grabo el cufe en Bd, error en tabla"
                                                    });
                                                }
                                                else
                                                {
                                                    int NueCons = getDataFacElectron.Com_Doc_Electron_NC + 1;
                                                    repoCia.ConsecutivoActualiza(Cia, "CONSELECTRONNC", NueCons);

                                                    List<FacturasR> Exportar = ExportarPDF.ExportarFacturaAseguradorasNC(factura.Encabezado.llaveComprobante, Cia);
                                                    if (Exportar == null)
                                                    {
                                                        listResultados.Add(new CXN_FACTURA
                                                        {
                                                            Fac_Tipo_Doc = "AseguradoraNC",
                                                            Fac_Estado = "Error",
                                                            Fac_Num_Fac = Convert.ToInt32(f),
                                                            Fac_Cia = Cia,
                                                            Homologo = "",
                                                            Fac_Observa = factura.Encabezado.ordenCompra + ": ESTADO 18 ZAMENIS: Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        GeneradorXML.DocPrestadorTemp = "";
                                                        GeneradorXML.PrefElectronTemp = "";
                                                        GeneradorXML.NumElectronTemp = "";

                                                        GeneradorXML.DocPrestadorTemp = getDataFacElectron.Com_Identificacion;
                                                        GeneradorXML.PrefElectronTemp = getDataFacElectron.Com_Prefijo_Electron_NC;
                                                        GeneradorXML.NumElectronTemp = getDataFacElectron.Com_Doc_Electron_NC.ToString();

                                                        GeneradorXML.rTemp = new List<FacturasR>();
                                                        GeneradorXML.rTemp = Exportar;

                                                        GenerateXMLPDF.GeneratePDFSaludNC(Cia, sendXML.ClaveTecnica);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                listResultados.Add(new CXN_FACTURA
                                                {
                                                    Fac_Tipo_Doc = "AseguradoraNC",
                                                    Fac_Estado = "Error",
                                                    Fac_Num_Fac = Convert.ToInt32(f),
                                                    Fac_Cia = Cia,
                                                    Homologo = "",
                                                    Fac_Observa = "Error"
                                                });

                                                string jsone3 = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                                                webView.CoreWebView2.PostWebMessageAsJson(jsone3);

                                                ResultadoRadicacion Rw = new ResultadoRadicacion(listResultados);
                                                Rw.ShowDialog();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            listResultados.Add(new CXN_FACTURA
                                            {
                                                Fac_Tipo_Doc = "AseguradoraNC",
                                                Fac_Estado = "Error",
                                                Fac_Num_Fac = Convert.ToInt32(f),
                                                Fac_Cia = Cia,
                                                Homologo = "",
                                                Fac_Observa = "Sin Respuesta"
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    MG2 = new MensajesGeneral
                                    {
                                        Mensaje = "Seleccion de tipo de factura invalida",
                                        TipoImagen = 1000
                                    };

                                    string jsone2 = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                                    webView.CoreWebView2.PostWebMessageAsJson(jsone2);

                                    return;
                                }
                            }

                            string jsone = JsonSerializer.Serialize(new { tipo = "activarBotones" });
                            webView.CoreWebView2.PostWebMessageAsJson(jsone);

                            ResultadoRadicacion R = new ResultadoRadicacion(listResultados);
                            R.ShowDialog();

                        }));
                    });
                }
                else if (tipo == "cerrarFormulario")
                {
                    Task.Run(() =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            this.Dispose();
                            this.Close();
                        }));
                    });
                }                
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    this.Invoke((Action)(() =>
                    {
                        MessageBox.Show("Error al procesar mensaje del WebView2: " + ex.Message);
                    }));
                });
            }
        }
        private void WebView_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapturing();
            SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }

        private void GenerarFacturaXML_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapturing();
            SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
