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
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class RcCaja : Forma2
    {
        private static readonly IRcCaja repoRcCaja = new MRcCaja();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly IPacientes repoAgendaMedicaPacientes = new MPacientes();
        private static readonly ICategoria repoCategoria = new MCategoria();
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IHelisa repoWSClients = new MHelisa(); 
        private static readonly IFacElectron repoFelectron = new MFacElectron();

        private int Adm_RC;
        private string Rc_Caja_Observacion;
        private MensajesGeneral MG;
        private string conRecaudo;

        List<RCCAJA> ExportaReport;

        public RcCaja(int Adm)
        {
            InitializeComponent();
            this.Adm_RC = Adm;
        }

        private void RcCaja_Load(object sender, EventArgs e)
        {
            ConfigForm.SoloNumeros(textBox1);

            Titulo.Text = "Recaudos";

            
            comboBox1.SelectedIndex = 0;

            int idPac = repoAgendaMedicaConsultas.getIdPacByAdmition(this.Adm_RC, "P");            

            if (idPac != 0)
            {
                CXN_PACIENTES Cat = repoAgendaMedicaPacientes.LlamarPacientebyId(idPac);                

                if (Cat != null)
                {
                    switch (Cat.Pac_Categoria)
                    {
                        case "A":
                            comboBox2.SelectedIndex = 0;
                            textBox1.Text = repoCategoria.getValor("A").ToString();
                            break;
                        case "B":
                            comboBox2.SelectedIndex = 1;
                            textBox1.Text = repoCategoria.getValor("B").ToString();
                            break;

                        case "C":
                            comboBox2.SelectedIndex = 2;
                            textBox1.Text = repoCategoria.getValor("C").ToString();
                            break;

                        case "Z":
                            comboBox2.SelectedIndex = 3;
                            textBox1.Text = repoCategoria.getValor("Z").ToString();
                            break;

                        default:
                            comboBox2.SelectedIndex = 4;
                            textBox1.Text = repoCategoria.getValor("N").ToString();
                            break;
                    }

                    textBox4.Text = Cat.Pac_Email.ToString();
                }              
            }
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                if (textBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe ingresar un valor valido";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "DEBE SELECCIONAR UN CONCEPTO DE RECAUDO";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox3.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "DEBE SELECCIONAR UNA FORMA DE PAGO";
                    MG.ShowDialog();
                    return;
                }

                if (Convert.ToInt32(textBox1.Text) >= 1 && comboBox1.Text == "NO APLICA")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Si ha puesto un valor superior a 0, el concepto de recaudo no puede ser no aplica";
                    MG.ShowDialog();
                    return;
                }

                if (Convert.ToInt32(textBox1.Text) == 0 && comboBox1.Text != "NO APLICA")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Si ha puesto un valor 0, el concepto de recaudo debe ser no aplica";
                    MG.ShowDialog();
                    return;
                }

                if (textBox4.Text != "")
                {
                    if (repoAgendaMedicaPacientes.ValidaEmail(textBox4.Text) != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Debe diligenciar un corro electronico valido, si no tiene deje este campo vacio";
                        MG.ShowDialog();
                        return;
                    }
                }

                conRecaudo = "05";

                /*
                    NO APLICA
                    CUOTA MODERADORA
                    COPAGO
                    PAGOS COMPARTIDOS EN PLANES VOLUNTARIOS DE SALUD
                    ANTICIPO
                */

                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        conRecaudo = "05"; //NO APLICA
                        break;

                    case 1:
                        conRecaudo = "02"; //CUOTA MODERADORA
                        break;

                    case 2:
                        conRecaudo = "01"; //COPAGO
                        break;

                    case 3:
                        conRecaudo = "03"; //PAGOS COMPARTIDOS
                        break;

                    case 4:
                        conRecaudo = "04"; //ANTICIPO
                        break;

                    default:
                        conRecaudo = "05"; //NO APLICA
                        break;
                }                

             //   bool updateHor = repoRcCaja.updateReciboHorario(Convert.ToInt32(textBox1.Text), Adm_RC, conRecaudo, "", comboBox3.Text);                

               /* if (updateHor != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se actualizo el recibo de caja, Ingrese por la opcion recibos de la agenda y agreguelo de nuevo";
                    MG.ShowDialog();
                }*/
                //else
                //{
                    otrosDatosPacienteHorario DatosAdmision = repoAgendaMedicaConsultas.cargarAdmision(Adm_RC, "'P','H','A'");                    

                    if (DatosAdmision != null)
                    {
                        DateTime Hoy = DateTime.Now;

                        if (textBox2.Text == "")
                        {
                            Rc_Caja_Observacion = "Venta o Recaudo Pines / Bonos";
                        }
                        else
                        {
                            Rc_Caja_Observacion = textBox2.Text;
                        }

                        DateTime f = new DateTime(DatosAdmision.Hor_Pac_Fecha_Cita.Year,
                                                  DatosAdmision.Hor_Pac_Fecha_Cita.Month,
                                                  DatosAdmision.Hor_Pac_Fecha_Cita.Day,
                                                  DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                        CXN_RC_CAJA R = new CXN_RC_CAJA
                        {
                            Rc_Caja_Pac = DatosAdmision.Hor_Pac_Id,
                            Rc_Caja_Ase = DatosAdmision.Hor_Pac_Ase,
                            Rc_Caja_Cia = DatosAdmision.Hor_Pac_Cia,
                            Rc_Caja_Fecha = f,
                            Rc_Caja_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                            Rc_Caja_Valor = Convert.ToInt32(textBox1.Text),
                            Rc_Caja_Adm = Adm_RC,
                            Rc_Caja_Observacion = Rc_Caja_Observacion,
                            FormaPago = comboBox3.Text,
                            Hor_ConceptoRecaudo = conRecaudo,
                            Num_Cruce = 0
                        };

                        int insertRc = repoRcCaja.AgregarRecibo(R);                        

                        if (insertRc <= 0)
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No logro insertar el recibo de caja, Error grave";
                            MG.ShowDialog();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(textBox4.Text))
                            {                     
                                repoAgendaMedicaPacientes.updateEmail(R.Rc_Caja_Pac, textBox4.Text);                                  
                            }                            

                            DialogResult result = MessageBox.Show("Seleccione SI para imprrimir recibo de caja termico, seleccione NO para imprimir recibo de caja tamaño carta",
                                                 "Zamenis Health - Impresion de Recibos",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                Exporta(insertRc, true);
                            }

                            if (result == DialogResult.No)
                            {
                                Exporta(insertRc, false);
                            }
                        }
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No logro cargar la admision, Error grave";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                //}
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return;
            }
        } 

        async Task GenerarFacturaElectronica(int PosId)
        {
            try
            {
                List<RCCAJA> getFacturaZamenis = ExportarPDF.ExportarReciboCaja(PosId);
                int Cia = getFacturaZamenis[0].Admision; //identifiador cia
                CXN_CIA getDataFacElectron = repoCia.getPrestadorbyCode(Cia);

                var factura = new Factura
                {
                    Encabezado = new Encabezado
                    {
                        llaveComprobante = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                        nitemisor = getFacturaZamenis[0].EmpresaIdentificacion,
                        codSucursal = getFacturaZamenis[0].EmpresaIdentificacion,
                        tiporeceptor = getFacturaZamenis[0].TDocReceptor == "NIT" ? "1" : "2",
                        tipoDocRec = Definiciones.tipoDocRec(99, Definiciones.tipoDocRec(88, repoAgendaMedicaPacientes.getTipoDoc(getFacturaZamenis[0].TDocReceptor))),
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
                        ordenCompra = Adm_RC.ToString(),
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
                   /* listResultados.Add(new CXN_FACTURA
                    {
                        Fac_Tipo_Doc = "Caja",
                        Fac_Estado = "Error",
                        Fac_Num_Fac = facturaZ,
                        Fac_Cia = Cia,
                        Homologo = "",
                        Fac_Observa = "No hay resultados para login de Facturacion Electronica"
                    });*/
                }
                else
                {
                    string TokenGenerado = repoFelectron.GetTokenSaved(Cia).Trim();
                    if (string.IsNullOrEmpty(TokenGenerado))
                    {
                        bool generar = GenerateTokens.GenerarTokenNuevo(Cia);
                        if (generar == false)
                        {
                           /* listResultados.Add(new CXN_FACTURA
                            {
                                Fac_Tipo_Doc = "Caja",
                                Fac_Estado = "Error",
                                Fac_Num_Fac = facturaZ,
                                Fac_Cia = Cia,
                                Homologo = "",
                                Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                            });

                            ResultadoRadicacion resultadoRadicacion2 = new ResultadoRadicacion(listResultados);
                            resultadoRadicacion2.ShowDialog();*/

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

                    /*XMLResponseF1 res = new XMLResponseF1()
                    {
                        StatusCode = "00",
                        error = "",
                    };*/
                    
                    var res = G.SendXML(sendXML, Program.URLApiConexion).GetAwaiter().GetResult();
                    if (res != null)
                    {
                        if (res.StatusCode == "00" && res.error == "")
                        {
                            string resolucion = "Res. DIAN No. " + getDataFacElectron.Com_Resolucion_Electron + " Habilitada para Facturación Electrónica de " + Convert.ToDateTime(getDataFacElectron.Com_Fecha_Electron).ToString("yyyy-MM-dd") + ", " + getDataFacElectron.Com_Numeracion_Electron;

                            UpdateFacturaElectronica datosNuevosFacElectron = new UpdateFacturaElectronica
                            {
                                FacturaElectronica = getDataFacElectron.Com_Prefijo_Electron + getDataFacElectron.Com_Doc_Electron,
                                FacturaZamenis = PosId,
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

                            }
                            else
                            {
                                int NueCons = getDataFacElectron.Com_Doc_Electron + 1;
                                repoCia.ConsecutivoActualiza(Cia, "CONSELECTRON", NueCons);

                                List<RCCAJA> Exportar = ExportarPDF.ExportarReciboCaja(PosId);
                                if (Exportar == null)
                                {

                                }
                                else
                                {
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
                            return;
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private async void Exporta(int PosId, bool TRecibo)
        {
            try
            {
                ExportaReport = repoRcCaja.ReciboRpt(PosId);

                await GenerarFacturaElectronica(PosId);

                if (TRecibo == true)
                {
                    if (ExportaReport != null)
                    {
                        Thread thread = new Thread(M2);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Hubo un inconveniente con este recibo, ingrese por copias recepcion o consulte el administrador del sistema";
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                    }
                }
                else
                {
                    if (ExportaReport != null)
                    {
                        Thread thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Hubo un inconveniente con este recibo, ingrese por copias recepcion o consulte el administrador del sistema";
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void M()
        {
            try
            {
                ConfigForm.GenerarReportViewer("ReciboCajaDataset",
              "ZamenisHealth.Reportes.RDLC_RcCaja.rdlc",
              ExportaReport);

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void M2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("ReciboCajaDataset",
              "ZamenisHealth.Reportes.RDLC_RcCajaImpTermica.rdlc",
              ExportaReport);

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        textBox1.Text = repoCategoria.getValor("A").ToString();
                        break;
                    case 1:
                        textBox1.Text = repoCategoria.getValor("B").ToString();
                        break;

                    case 2:
                        textBox1.Text = repoCategoria.getValor("C").ToString();
                        break;

                    case 3:
                        textBox1.Text = repoCategoria.getValor("Z").ToString();
                        break;

                    default:
                        textBox1.Text = repoCategoria.getValor("N").ToString();
                        break;
                }            
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
