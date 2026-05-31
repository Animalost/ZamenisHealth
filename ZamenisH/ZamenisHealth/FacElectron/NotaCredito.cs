using DocumentosElectronicos.Controlador;
using DocumentosElectronicos.Request;
using DocumentosElectronicos.Servicio;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron
{
    public partial class NotaCredito : Forma
    {
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IFacturacion repoFac = new MFacturacion();
        private static readonly IVentas repoVentas = new MVentas();
        private static readonly IRcCaja repoCaja = new MRcCaja();
        private static readonly IHelisa repoWSClients = new MHelisa();
        private static readonly IFacElectron repoFacElectron = new MFacElectron();

        private MensajesGeneral MG;
        private int Cia;
        private object Clase;

        DataTable dt;
        DataColumn CheckForSend;
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Cantidad;
        DataColumn VrUnitario;
        DataColumn VrTotal;
        DataColumn FZamenis;

        public NotaCredito()
        {
            InitializeComponent();
        }

        void CargarCia()
        {
            List<CXN_CIA> compañias = repoCia.getAllCompañias();
            if (compañias != null)
            {
                foreach (CXN_CIA c in compañias)
                {
                    comboBox1.Items.Add(c.Com_Nombre);
                }
                
                comboBox1.SelectedIndex = 0;
            }           
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            CheckForSend = dt.Columns.Add("CheckForSend", typeof(bool));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Item = dt.Columns.Add("Item", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
            VrUnitario = dt.Columns.Add("VrUnitario", typeof(string));
            VrTotal = dt.Columns.Add("VrTotal", typeof(string));
            FZamenis = dt.Columns.Add("FZamenis", typeof(int));
        }
        private void NotaCredito_Load(object sender, EventArgs e)
        {
            
            CargarCia();

            comboBox3.SelectedIndex = 1;
            comboBox3.Enabled = false;

            Titulo.Text = "Nota Credito";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;
            ImageClose.Visible = true;

            ToolStripButton btnConsultar = new ToolStripButton();
            btnConsultar = createToolButton("Consultar");
            MenuLateral.Items.Add(btnConsultar);
            btnConsultar.Click += button1_Click;

            ToolStripButton btnMarcar = new ToolStripButton();
            btnMarcar = createToolButton("Marcar Todo");
            MenuLateral.Items.Add(btnMarcar);
            btnMarcar.Click += button2_Click;

            ToolStripButton btnDesMarcar = new ToolStripButton();
            btnDesMarcar = createToolButton("Desmarcar Todo");
            MenuLateral.Items.Add(btnDesMarcar);
            btnDesMarcar.Click += button3_Click;

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Generar Nota");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button4_Click;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cia = repoCia.getPrestadorbyName(comboBox1.Text).Com_Identificador;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Clase = new object();

                switch (comboBox2.Text)
                {
                    case "Factura Aseguradoras":
                        List<FacturasR> getFactura = new List<FacturasR>();
                       
                        getFactura = repoFac.Fac_Export(textBox1.Text, Cia);                                                    

                        if (getFactura == null)
                        {
                            dataGridView1.DataSource = null;
                            Encabezados();
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Este documento no fue encontrado dentro del tipo de factura seleccionado";
                            MG.ShowDialog(this);
                            return;
                        }

                        Clase = new List<FacturasR>(getFactura);

                        break;
                    case "Factura Ventanilla":
                        List<FacturacionRpt> getFacturaVentas = repoVentas.Exp_Fac_Ven(textBox1.Text, Cia);
                        if (getFacturaVentas == null)
                        {
                            dataGridView1.DataSource = null;
                            Encabezados();
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Este documento no fue encontrado dentro del tipo de factura seleccionado";
                            MG.ShowDialog(this);
                            return;
                        }

                        Clase = new List<FacturacionRpt>(getFacturaVentas);
                        break;
                    case "Factura Recaudo de Pines/Bonos":
                        List<RCCAJA> getFacturaCaja = repoCaja.ReciboRpt(textBox1.Text);
                        if (getFacturaCaja == null)
                        {
                            dataGridView1.DataSource = null;
                            Encabezados();
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Este documento no fue encontrado dentro del tipo de factura seleccionado";
                            MG.ShowDialog(this);
                            return;
                        }

                        Clase = new List<RCCAJA>(getFacturaCaja);
                        break;

                    default:
                        dataGridView1.DataSource = null;
                        Encabezados();
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione un tipo de factura válido.";
                        MG.ShowDialog(this);
                        return;
                }

                LlenarGrid(Clase);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte del documento diligenciado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void LlenarGrid(object clase)
        {
            try
            {
                Encabezados();
                int Contador = 1;

                if (clase is List<FacturasR> listaFacturas && listaFacturas.Count > 0)
                {
                    foreach (var item in listaFacturas)
                    {
                        DataRow dr = dt.NewRow();
                        dr[POS] = Contador;
                        dr[CheckForSend] = false;
                        dr[Codigo] = item.Car_Cod;
                        dr[Item] = item.Car_Item;
                        dr[Cantidad] = item.Cantidad;
                        dr[VrUnitario] = Convert.ToInt32(item.Car_Val_Un).ToString("N0");
                        dr[VrTotal] = Convert.ToInt32(item.Total).ToString("N0");
                        dr[FZamenis] = Convert.ToInt32(item.Fac_Num_Fac);

                        dt.Rows.Add(dr);
                        dt.AcceptChanges();

                        Contador++;
                    }

                    Contador = 1;
                }
                else if (clase is List<FacturacionRpt> listaVentas && listaVentas.Count > 0)
                {
                    foreach (var item in listaVentas)
                    {
                        DataRow dr = dt.NewRow();
                        dr[POS] = Contador;
                        dr[CheckForSend] = false;
                        dr[Codigo] = item.CodigoProd;
                        dr[Item] = item.ItemProd;
                        dr[Cantidad] = item.CantidadProd;
                        dr[VrUnitario] = Convert.ToInt32(item.VrUnitarioProd).ToString("N0");
                        dr[VrTotal] = Convert.ToInt32(item.VrTotalProd).ToString("N0");
                        dr[FZamenis] = Convert.ToInt32(item.Admision);

                        dt.Rows.Add(dr);
                        dt.AcceptChanges();

                        Contador++;
                    }

                    Contador = 1;
                }
                else if (clase is List<RCCAJA> listaCaja && listaCaja.Count > 0)
                {
                    foreach (var item in listaCaja)
                    {
                        DataRow dr = dt.NewRow();
                        dr[POS] = Contador;
                        dr[CheckForSend] = false;
                        dr[Codigo] = "100000";
                        dr[Item] = item.Observacion;
                        dr[Cantidad] = 1;
                        dr[VrUnitario] = Convert.ToInt32(item.Valor).ToString("N0");
                        dr[VrTotal] = Convert.ToInt32(item.Valor).ToString("N0");
                        dr[FZamenis] = Convert.ToInt32(item.Recibo);

                        dt.Rows.Add(dr);
                        dt.AcceptChanges();

                        Contador++;
                    }

                    Contador = 1;
                }
                else
                {
                    dataGridView1.DataSource = null;
                    Encabezados();
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se encontraron registros para el documento ingresado.";
                    MG.ShowDialog(this);
                    return;
                }

                Estilos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar el grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        void Estilos()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;

            dataGridView1.ReadOnly = false;
            dataGridView1.Columns["CheckForSend"].ReadOnly = false;
            dataGridView1.Columns["Codigo"].ReadOnly = true;
            dataGridView1.Columns["Item"].ReadOnly = true;
            dataGridView1.Columns["Cantidad"].ReadOnly = true;
            dataGridView1.Columns["VrUnitario"].ReadOnly = true;
            dataGridView1.Columns["VrTotal"].ReadOnly = true;

            dataGridView1.Columns["Codigo"].Width = 110;
            dataGridView1.Columns["Item"].Width = 350;
            dataGridView1.Columns["Cantidad"].Width = 110;
            dataGridView1.Columns["VrUnitario"].Width = 200;
            dataGridView1.Columns["VrTotal"].Width = 200;
            dataGridView1.Font = new System.Drawing.Font("Arial", 11);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Item"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["VrUnitario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["VrTotal"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["Codigo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Item"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Cantidad"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["VrUnitario"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["VrTotal"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.Columns["POS"].Visible = false;
            dataGridView1.Columns["FZamenis"].Visible = false;

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
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    if (!isChecked)
                    {
                        row.Cells["CheckForSend"].Value = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    if (isChecked)
                    {
                        row.Cells["CheckForSend"].Value = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione el tipo de nota crédito a generar";
                    MG.ShowDialog(this);
                    return; 
                }

                if (Clase is List<FacturasR> listaFacturas && listaFacturas.Count > 0)
                {
                    GenerarClaseFacturasAseguradoras(listaFacturas);                   
                }
                else if (Clase is List<FacturacionRpt> listaVentas && listaVentas.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        object valor = row.Cells["CheckForSend"].Value;
                        bool isChecked = valor != null && Convert.ToBoolean(valor);

                        if (!isChecked)
                        {
                            row.Cells["CheckForSend"].Value = true;
                        }
                    }

                    GenerarClaseFacturasVentas(listaVentas);
                }
                else if (Clase is List<RCCAJA> listaCaja && listaCaja.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        object valor = row.Cells["CheckForSend"].Value;
                        bool isChecked = valor != null && Convert.ToBoolean(valor);

                        if (!isChecked)
                        {
                            row.Cells["CheckForSend"].Value = true;
                        }
                    }

                    GenerarClaseFacturasCaja(listaCaja);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay conceptos seleccionados para elaborar la nota credito";
                    MG.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la nota de crédito: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarClaseFacturasCaja(List<RCCAJA> clase)
        {
            try
            {
                List<FacturacionRpt> ClaseSendForXML = new List<FacturacionRpt>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);

                    string facturaE = textBox1.Text;

                    int Cantidades = Convert.ToInt32(row.Cells["Cantidad"].Value.ToString().Trim());
                    int VrUni = Convert.ToInt32(row.Cells["VrUnitario"].Value.ToString().Trim().Replace(".", ""));
                    int VrTot = Convert.ToInt32(row.Cells["VrTotal"].Value.ToString().Trim().Replace(".", ""));
                    int NFacZ = Convert.ToInt32(row.Cells["FZamenis"].Value.ToString().Trim());

                    List<RCCAJA> temp = ExportarPDF.ExportarReciboCaja(NFacZ);
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
                        MessageBox.Show("No hay resultados para login de Facturacion Electronica");
                        return;
                    }
                    string TokenGenerado = repoFacElectron.GetTokenSaved(Cia).Trim();
                    if (TokenGenerado == null)
                    {
                        MessageBox.Show("No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion");
                        return;
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
                                    Tipo = "Caja"
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

                            string insertarNC = repoFacElectron.insertNC(datosNuevosFacElectron);
                            if (insertarNC != "OK")
                            {
                                MessageBox.Show(factura.Encabezado.ordenCompra + ": ESTADO 1000 ZAMENIS: Se genero la radicacion correcta pero no se grabo el cufe en Bd, error en tabla");
                                return;
                            }

                            int NueCons = getDataFacElectron.Com_Doc_Electron_NC + 1;
                            repoCia.ConsecutivoActualiza(Cia, "CONSELECTRONNC", NueCons);

                            List<RCCAJA> Exportar = ExportarPDF.ExportarReciboCajaNC(Convert.ToInt32(NFacZ));
                            if (Exportar == null)
                            {
                                MessageBox.Show(factura.Encabezado.ordenCompra + ": ESTADO 18 ZAMENIS: Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja");
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
                        else
                        {
                            MessageBox.Show(res.error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sin Respuesta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la clase de facturas aseguradoras: " + ex.Message + "\n\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarClaseFacturasVentas(List<FacturacionRpt> clase)
        {
            try
            {
                List<FacturacionRpt> ClaseSendForXML = new List<FacturacionRpt>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);
                    string facturaE = textBox1.Text;

                    int Cantidades = Convert.ToInt32(row.Cells["Cantidad"].Value.ToString().Trim());
                    int VrUni = Convert.ToInt32(row.Cells["VrUnitario"].Value.ToString().Trim().Replace(".", ""));
                    int VrTot = Convert.ToInt32(row.Cells["VrTotal"].Value.ToString().Trim().Replace(".", ""));
                    int NFacZ = Convert.ToInt32(row.Cells["FZamenis"].Value.ToString().Trim());

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
                        MessageBox.Show("No hay resultados para login de Facturacion Electronica");
                        return;
                    }
                    string TokenGenerado = repoFacElectron.GetTokenSaved(Cia).Trim();
                    if (TokenGenerado == null)
                    {
                        MessageBox.Show("No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion");
                        return;
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

                            string insertarNC = repoFacElectron.insertNC(datosNuevosFacElectron);
                            if (insertarNC != "OK")
                            {
                                MessageBox.Show(factura.Encabezado.ordenCompra + ": ESTADO 1000 ZAMENIS: Se genero la radicacion correcta pero no se grabo el cufe en Bd, error en tabla");
                                return;
                            }

                            int NueCons = getDataFacElectron.Com_Doc_Electron_NC + 1;
                            repoCia.ConsecutivoActualiza(Cia, "CONSELECTRONNC", NueCons);

                            List<FacturacionRpt> Exportar = ExportarPDF.ExportarFacturaVentasNC(Convert.ToInt32(factura.Encabezado.ordenCompra), Cia, "OP", factura.Encabezado.llaveComprobante);
                            if (Exportar == null)
                            {
                                MessageBox.Show(factura.Encabezado.ordenCompra + ": ESTADO 18 ZAMENIS: Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja");
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
                        else
                        {
                            MessageBox.Show(res.error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sin Respuesta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la clase de facturas aseguradoras: " + ex.Message + "\n\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarClaseFacturasAseguradoras(List<FacturasR> clase)
        {
            try
            {
                List<FacturasR> ClaseSendForXML = new List<FacturasR>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string facturaE = textBox1.Text;

                    int Cantidades = Convert.ToInt32(row.Cells["Cantidad"].Value.ToString().Trim());
                    int VrUni = Convert.ToInt32(row.Cells["VrUnitario"].Value.ToString().Trim().Replace(".", ""));
                    int VrTot = Convert.ToInt32(row.Cells["VrTotal"].Value.ToString().Trim().Replace(".", ""));
                    int NFacZ = Convert.ToInt32(row.Cells["FZamenis"].Value.ToString().Trim());

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
                        MessageBox.Show("No hay resultados para login de Facturacion Electronica");
                        return;
                    }
                    string TokenGenerado = repoFacElectron.GetTokenSaved(Cia).Trim();
                    if (TokenGenerado == null)
                    {
                        MessageBox.Show("No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion");
                        return;
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

                            string insertarNC = repoFacElectron.insertNC(datosNuevosFacElectron);
                            if (insertarNC != "OK")
                            {
                                MessageBox.Show(factura.Encabezado.ordenCompra + ": ESTADO 1000 ZAMENIS: Se genero la radicacion correcta pero no se grabo el cufe en Bd, error en tabla");
                                return;
                            }

                            int NueCons = getDataFacElectron.Com_Doc_Electron_NC + 1;
                            repoCia.ConsecutivoActualiza(Cia, "CONSELECTRONNC", NueCons);

                            List<FacturasR> Exportar = ExportarPDF.ExportarFacturaAseguradorasNC(factura.Encabezado.llaveComprobante, Cia);
                            if (Exportar == null)
                            {
                                MessageBox.Show(factura.Encabezado.ordenCompra + ": ESTADO 18 ZAMENIS: Factura Generada Exitosamente pero no se logro generar el PDF, genere una copia del recibo de caja");
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
                        else
                        {
                            MessageBox.Show(res.error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sin Respuesta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la clase de facturas aseguradoras: " + ex.Message + "\n\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
