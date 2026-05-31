using DocumentosElectronicos;
using DocumentosElectronicos.Request;
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Ventas
{
    public partial class Terminar : Forma
    {
        private IVender oController;

        private CXN_VENTAS_TEMP ventas;

        private int SubTotal, Descuentos, Fuente;
        private decimal ICA, IcaVal;
        private MensajesGeneral MG;

        public Terminar(CXN_VENTAS_TEMP ventasTemp)
        {
            InitializeComponent();
            ventas = ventasTemp;
            oController = new MVender();
        }

        private void Terminar_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Terminar Factura";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Generar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += btnGenerar_Click;

            Calcular();
        }
        void Calcular()
        {
            try
            {
                SubTotal = 0;
                Descuentos = 0;

                foreach (CXN_VENTAS i in ventas.Cargos)
                {
                    SubTotal = SubTotal + i.Ven_Total;
                }

                textBox1.Text = "$ " + SubTotal.ToString("N0");
                
                Descuentos = ((SubTotal * ventas.Descuento)) / 100;
                textBox2.Text = "$ " + Descuentos.ToString("N0");

                Fuente = oController.Fuente(SubTotal, ventas.RFuente);
                textBox4.Text = "$ " + Fuente.ToString("N0");

                ICA = oController.ICA(ventas.RICA);
                textBox5.Text = "$ " + (ICA * SubTotal).ToString("N2");
                IcaVal = (ICA * SubTotal);

                textBox6.Text = "$ " + (SubTotal - Descuentos - Fuente - (ICA * SubTotal)).ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea terminar esta prefactura?",
                                                      "Zamenis Health - Facturacion",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_CIA empresa = oController.getPrestadorbyCode(ventas.IdCia);

                    foreach (CXN_VENTAS v in ventas.Cargos)
                    {
                        int precio = v.Ven_Precio;

                        if (ventas.Descuento != 0)
                        {
                            int valTemp = ((v.Ven_Precio * ventas.Descuento)) / 100;
                            precio = v.Ven_Precio - valTemp;
                        }

                        CXN_VENTAS V = new CXN_VENTAS
                        {
                            Ven_Cod = v.Ven_Cod,
                            Ven_Cantidad = v.Ven_Cantidad,
                            Ven_Precio = precio,
                            Ven_Total = precio * v.Ven_Cantidad,
                            Ven_Cod_Pac = ventas.IdPac,
                            Ven_Cod_Cia = ventas.IdCia,
                            Ven_Estado = "F",
                            Ven_Factura = empresa.Com_OP.ToString(),
                            Ven_Usr_Graba = Contenedor.UsuarioLogueado,
                            Ven_Item = v.Ven_Item,
                            Ven_Res = "PRE-FACTURA",
                            Ven_Dcto = Descuentos,
                            Ven_Fecha = Convert.ToDateTime(DateTime.Now.Date),
                            Ven_Tipo_Doc = "OP",
                            DiasVencimiento = ventas.Vencimiento,
                            MetodoPago = ventas.MetodoPago,
                            MedioPago = ventas.MedioPago,
                            FormaPago = ventas.FormaPago,
                            PercentICA = ventas.RICA,
                            PercentFUENTE = ventas.RFuente,
                            Num_Cruce = 0
                        };

                        oController.insertarVenta(V);
                    }

                    oController.UpdateFuenteICA((int)Fuente, (int)IcaVal, empresa.Com_OP, ventas.IdCia);

                    //GENERAR ELECTRONICA
                    await FacturaElectronica(empresa.Com_OP);

                    Finalizar(empresa.Com_OP, "OP");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        async Task FacturaElectronica(int facturaZ)
        {
            try
            {
                List<FacturacionRpt> getFacturaZamenis = ExportarPDF.ExportarFacturaVentas(facturaZ, Convert.ToInt32(ventas.IdCia), "OP");
                CXN_CIA getDataFacElectron = oController.getPrestadorbyCode(ventas.IdCia);

                int vrimpuestosretenidos = 0;
                int vrNeto = getFacturaZamenis[0].VrNetoaPagar; //aqui taigo este total sin descuentos, no los estoy reportando cuotas, copagos, etc para reporarlos se cambia a VrTotalFac pero ahi que arreglarlo cuando lo trae
                string letra = getFacturaZamenis[0].ValorLetras;

                if (oController.getListado()["ReportarImpuestosDIAN"] == "A")
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
                        tipoDocRec = Definiciones.tipoDocRec(88, Definiciones.tipoDocRec(88, oController.getTipoDoc(getFacturaZamenis[0].Com_Direccion))),
                        nitreceptor = getFacturaZamenis[0].PacienteAseguradora,
                        nombrereceptor = getFacturaZamenis[0].PacienteNombre,
                        mailreceptor = string.IsNullOrEmpty(getFacturaZamenis[0].ProfesionalNombre) ? "administrador@slsoft.net" : oController.ValidaEmail(getFacturaZamenis[0].ProfesionalNombre) == false ? "administrador@slsoft.net" : getFacturaZamenis[0].ProfesionalNombre,
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

                Dictionary<string, string> dataWS = oController.Claves("Factura1XML", ventas.IdCia);
                if (dataWS == null)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Generado exitosamente con Factura Electronica pendiente",
                        TipoImagen = 3
                    };
                    MG.ShowDialog();
                }
                else
                {
                    string TokenGenerado = oController.GetTokenSaved(ventas.IdCia).Trim();
                    if (string.IsNullOrEmpty(TokenGenerado))
                    {
                        bool generar = GenerateTokens.GenerarTokenNuevo(ventas.IdCia);
                        if (generar == false)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Generado exitosamente con Factura Electronica pendiente",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();

                            return;
                        }
                        else
                        {
                            TokenGenerado = oController.GetTokenSaved(ventas.IdCia).Trim();
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
                                Prestador = ventas.IdCia
                            };

                            bool uRc = ActualizarDocumentos.AddHomologoVentas(datosNuevosFacElectron);
                            if (uRc != true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Generado exitosamente con Factura Electronica pendiente.  No se logro homologar el PDF",
                                    TipoImagen = 3
                                };
                                MG.ShowDialog();
                            }
                            else
                            {
                                int NueCons = getDataFacElectron.Com_Doc_Electron + 1;
                                oController.ConsecutivoActualiza(ventas.IdCia, "CONSELECTRON", NueCons);

                                List<FacturacionRpt> Exportar = ExportarPDF.ExportarFacturaVentas(facturaZ, ventas.IdCia, "OP");
                                if (Exportar == null)
                                {
                                    MG = new MensajesGeneral()
                                    {
                                        Mensaje = "Generado exitosamente con Factura Electronica pendiente.  No se logro homologar el PDF",
                                        TipoImagen = 3
                                    };
                                    MG.ShowDialog();
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


                                    Image Code_QR_Fac_CUFE = oController.CodifyQR(QrElectron);

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
                                            QRCufe = oController.GetBytes(Code_QR_Fac_CUFE),
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

                                    ReportViewer RVenta = new ReportViewer();
                                    var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                                    RVenta.LocalReport.DataSources.Clear();
                                    RVenta.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Facturacion", rTemp));

                                    if (oController.getListado()["ReportarImpuestosDIAN"] == "A")
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

                                    string GetCUFE = ActualizarDocumentos.GetCUFE(datosNuevosFacElectron.FacturaElectronica, ventas.IdCia, "Ventas");
                                    //Radicar PDF
                                    GenerateXMLPDF.GenerarBase64PDF(bytes, ventas.IdCia, GetCUFE, datosNuevosFacElectron.FacturaElectronica, sendXML.ClaveTecnica);

                                    FileStream fss = new FileStream("C:\\CXN\\RespuestasDIAN\\RespuestaPDF\\FEV_" + getDataFacElectron.Com_Identificacion.ToString() + "_" + datosNuevosFacElectron.FacturaElectronica + ".pdf", FileMode.Create);
                                    fss.Write(bytes, 0, bytes.Length);
                                    fss.Close();
                                }
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = $"{ res.error } -- Error estado DIAN",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Error en respuesta de la API: Sin Respuesta",
                            TipoImagen = 3
                        };
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = $"Generado exitosamente con Factura Electronica pendiente. { ex.Message }",
                    TipoImagen = 3
                };
                MG.ShowDialog();
            }
        }
        private void Finalizar(int Doc, string Tipo)
        {
            oController.ConsecutivoActualiza(ventas.IdCia, "OP", Doc + 1);

            Vender formulario = Application.OpenForms.OfType<Vender>().LastOrDefault();

            DialogResult result = MessageBox.Show("Vendido, puede imprimir el recibo o cerrar esta pantalla",
                                                  "Finalizar Documento",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Imprimir(Doc, Tipo);
            }
            if (result == DialogResult.No)
            {
                formulario.Close();

                this.Dispose();
                this.Close();
            }
        }
        private void Imprimir(int Doc, string Tipo)
        {
            try
            {
                List<FacturacionRpt> Exportar = oController.Exp_Fac_Ven(Doc, Convert.ToInt32(ventas.IdCia), Tipo);
                if (Exportar == null)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 0;
                    MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                    MG.ShowDialog();

                    Vender formulario1 = Application.OpenForms.OfType<Vender>().LastOrDefault();
                    formulario1.Close();

                    this.Close();
                    return;
                }

                Facturacion.Extras.TipoReportFactura R = new Facturacion.Extras.TipoReportFactura(Exportar, null, true);
                R.ShowDialog();

                Vender formulario = Application.OpenForms.OfType<Vender>().LastOrDefault();
                formulario.Close();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
