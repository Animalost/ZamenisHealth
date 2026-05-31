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
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron
{
    public partial class MainElectron : Forma
    {
        private readonly static IFacturacion repoFacs = new MFacturacion();
        private readonly static IVentas repoVentas = new MVentas();
        private readonly static ICompañia repoCia = new MCompañia();
        private readonly static IPacientes repoPacs = new MPacientes();
        private readonly static IHelisa repoWSClients = new MHelisa();
        private readonly static IFacElectron repoFelectron = new MFacElectron();
        private readonly static IGenerales repoGen = new MGenerales();
        private readonly static IRoles repoRoles = new MRoles();
        private readonly static IConfSystem repoConfSystem = new MConfSystem();

        private MensajesGeneral MG;
        private int Cia;
        private ToolStripButton btnBuscar;


        DataTable dt;
        DataColumn POS;
        DataColumn CheckForSend;
        DataColumn DocumentoZamenis;
        DataColumn FechaGeneracion;
        DataColumn FechaInicio;
        DataColumn FechaFinal;
        DataColumn Valor;
        DataColumn Paciente;
        DataColumn Aseguradora;
        DataColumn Usuario;

        public MainElectron()
        {
            InitializeComponent();
        }

        void Permisos()
        {
            CXN_ROLES R = repoRoles.getRoles(Contenedor.UsuarioLogueado);
            if (R == null)
            {
                btnBuscar.Enabled = false;              
            }
            else
            {
                btnBuscar.Enabled = (R.AdminGenerarToken == "A" ? true : false);              
            }
        }

        private void MainElectron_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Notas Credito";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                ToolStripButton btnToken = new ToolStripButton();
                btnToken = createToolButton("Token");
                MenuLateral.Items.Add(btnToken);
                btnToken.Click += button3_Click;

                btnBuscar = new ToolStripButton();
                btnBuscar = createToolButton("Buscar");
                MenuLateral.Items.Add(btnBuscar);
                btnBuscar.Click += button1_Click;

                ToolStripButton btnMarcar = new ToolStripButton();
                btnMarcar = createToolButton("Marcar Todo");
                MenuLateral.Items.Add(btnMarcar);
                btnMarcar.Click += button4_Click;

                ToolStripButton btnDesMarcar = new ToolStripButton();
                btnDesMarcar = createToolButton("Desmarcar Todo");
                MenuLateral.Items.Add(btnDesMarcar);
                btnDesMarcar.Click += button5_Click;

                ToolStripButton btnRadicarFE = new ToolStripButton();
                btnRadicarFE = createToolButton("Radicar Facturacion");
                MenuLateral.Items.Add(btnRadicarFE);
                btnRadicarFE.Click += button2_Click;

                ToolStripButton btnNC = new ToolStripButton();
                btnNC = createToolButton("Notas Credito");
                MenuLateral.Items.Add(btnNC);
                btnNC.Click += button6_Click;


                foreach (string mes in Meses())
                {
                    comboBox3.Items.Add(mes);
                }

                foreach (string año in Años())
                {
                    comboBox4.Items.Add(año);
                }

                Permisos();

                List<CXN_CIA> companias = repoCia.getAllCompañias();
                if (companias != null && companias.Count > 0)
                {
                    foreach (CXN_CIA cia in companias)
                    {
                        comboBox1.Items.Add(cia.Com_Nombre);
                    }

                    comboBox1.SelectedIndex = 0; // Selecciona la primera compañia por defecto
                }

                comboBox2.SelectedIndex = 0;
                dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;

                Encabezados();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cia = repoCia.getPrestadorbyName(comboBox1.SelectedItem.ToString()).Com_Identificador;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            CheckForSend = dt.Columns.Add("CheckForSend", typeof(bool));
            DocumentoZamenis = dt.Columns.Add("DocumentoZamenis", typeof(int));
            FechaGeneracion = dt.Columns.Add("FechaGeneracion", typeof(DateTime));
            FechaInicio = dt.Columns.Add("FechaInicio", typeof(DateTime));
            FechaFinal = dt.Columns.Add("FechaFinal", typeof(DateTime));
            Valor = dt.Columns.Add("Valor", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Aseguradora = dt.Columns.Add("Aseguradora", typeof(string));
            Usuario = dt.Columns.Add("Usuario", typeof(string));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox2.Text) ||
                string.IsNullOrEmpty(comboBox3.Text) ||
                string.IsNullOrEmpty(comboBox4.Text))
            {
                MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "Debe seleccionar Mes, Año y Tipo de Factura";
                MG.ShowDialog();
                return;
            }

            BuscarDocumentos();            
        }

        private async void BuscarDocumentos()
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    await FiltrarVentasNegocio(0); //CXN_FACTURAS
                    break;

                case 1:
                    await FiltrarVentasNegocio(1); //CXN_VENTAS
                    break;

                case 2:
                    await FiltrarRcCaja(); //CXN_HORARIO
                    break;

                default:
                    MG = new MensajesGeneral
                    {
                        Mensaje = "Por favor, seleccione un tipo de documento para filtrar.",
                        TipoImagen = 1000
                    };
                    break;
            }
        }

        async Task FiltrarRcCaja()
        {
            try
            {
                DateTime Desde = new DateTime(Convert.ToInt32(comboBox4.Text), getMonthNumber(comboBox3.Text), 01);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox4.Text), getMonthNumber(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                List<ReportesRecepcion> getFacturas = new List<ReportesRecepcion>();

                getFacturas = await Task.Run(() =>
                {
                    return repoVentas.Rpt_RecibosdeCaja(Desde.Date, Hasta.Date, Cia, "", false);
                });

                Encabezados(); 

                if (getFacturas != null && getFacturas.Count > 0)
                {
                    int Contador = 1;

                    foreach (ReportesRecepcion i in getFacturas)
                    {
                        if (string.IsNullOrEmpty(i.Homologo) == true)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["CheckForSend"] = false;
                            row["DocumentoZamenis"] = Convert.ToInt32(i.Admision);
                            row["FechaGeneracion"] = Convert.ToDateTime(i.FechaBase).ToString("yyyy/MM/dd");
                            row["FechaInicio"] = Convert.ToDateTime(i.FechaBase).ToString("yyyy/MM/dd");
                            row["FechaFinal"] = Convert.ToDateTime(i.FechaBase).ToString("yyyy/MM/dd");
                            row["Valor"] = "$ " + Convert.ToInt32(i.ValorReciboFactura).ToString("N0");
                            row["Paciente"] = i.PacienteNombre;
                            row["Aseguradora"] = i.PacienteAseguradora;
                            row["Usuario"] = i.ProfesionalNombre;

                            dt.Rows.Add(row);
                            Contador++;
                        }                        
                    }

                    dt.AcceptChanges(); // Solo una vez al final
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    Encabezados();
                    //MessageBox.Show("No se encontraron facturas para convertir en el rango de fechas seleccionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async Task FiltrarVentasNegocio(int indiceCombo)
        {
            try
            {
                DateTime Desde = new DateTime(Convert.ToInt32(comboBox4.Text), getMonthNumber(comboBox3.Text), 01);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox4.Text), getMonthNumber(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                // Parte que puede ir en segundo plano
                List<CXN_FACTURA> getFacturas = new List<CXN_FACTURA>();
                if (indiceCombo == 0)
                {
                    getFacturas = await Task.Run(() =>
                    {
                        return repoFacs.GetFacturasForConvertElectron(Cia, Desde.Date, Hasta.Date);
                    });
                }
                else if (indiceCombo == 1)
                {                    
                    getFacturas=  repoVentas.ListaFacturasFacElectron(Desde.Date, Hasta.Date, Cia);                    
                }
               
                Encabezados(); // Esto sí puede ir antes

                if (getFacturas != null && getFacturas.Count > 0)
                {
                    int Contador = 1;

                    foreach (CXN_FACTURA i in getFacturas)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["CheckForSend"] = false;
                        row["DocumentoZamenis"] = Convert.ToInt32(i.Fac_Num_Fac);
                        row["FechaGeneracion"] = Convert.ToDateTime(i.Fac_Fecha).ToString("yyyy/MM/dd");
                        row["FechaInicio"] = Convert.ToDateTime(i.Fac_Fecha_Des).ToString("yyyy/MM/dd");
                        row["FechaFinal"] = Convert.ToDateTime(i.Fac_Fecha_Has).ToString("yyyy/MM/dd");
                        row["Valor"] = "$ " + Convert.ToInt32(i.VrCompartido).ToString("N0");
                        row["Paciente"] = i.Fac_Observa;
                        row["Aseguradora"] = i.Cobertura;
                        row["Usuario"] = i.Fac_Usr_Graba;

                        dt.Rows.Add(row);
                        Contador++;
                    }

                    dt.AcceptChanges(); // Solo una vez al final
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    Encabezados();
                    //MessageBox.Show("No se encontraron facturas para convertir en el rango de fechas seleccionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }           
        }

        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;
            D.RowHeadersVisible = false;

            D.DataSource = t;

            D.ReadOnly = false;
            D.Columns["CheckForSend"].ReadOnly = false;
            D.Columns["DocumentoZamenis"].ReadOnly = true;
            D.Columns["FechaGeneracion"].ReadOnly = true;
            D.Columns["FechaInicio"].ReadOnly = true;
            D.Columns["FechaFinal"].ReadOnly = true;
            D.Columns["Valor"].ReadOnly = true;
            D.Columns["Paciente"].ReadOnly = true;
            D.Columns["Aseguradora"].ReadOnly = true;
            D.Columns["Usuario"].ReadOnly = true;

            D.Columns["DocumentoZamenis"].Width = 120;
            D.Columns["CheckForSend"].Width = 40;
            D.Columns["FechaGeneracion"].Width = 120;
            D.Columns["FechaInicio"].Width = 120;
            D.Columns["FechaFinal"].Width = 120;
            D.Columns["Valor"].Width = 120;
            D.Columns["Paciente"].Width = 400;
            D.Columns["Aseguradora"].Width = 400;
            D.Columns["Usuario"].Width = 120;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["DocumentoZamenis"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["CheckForSend"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["FechaGeneracion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["FechaInicio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["FechaFinal"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Valor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Aseguradora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Usuario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["DocumentoZamenis"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["CheckForSend"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["FechaGeneracion"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["FechaInicio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["FechaFinal"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Valor"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Aseguradora"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Usuario"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;

            D.Columns["CheckForSend"].HeaderText = "Sel";
            D.Columns["DocumentoZamenis"].HeaderText = "Documento Zamenis";
            D.Columns["FechaGeneracion"].HeaderText = "Fecha Generacion";
            D.Columns["FechaInicio"].HeaderText = "Fecha Inicio";
            D.Columns["FechaFinal"].HeaderText = "Fecha Final";            

            ConfigForm.colorGrid(D);

            D.ClearSelection();           
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                List<CXN_FACTURA> listResultados = new List<CXN_FACTURA>();

                //FACTURAS DE ASEGURADORAS Y PARTICULARES
                if (comboBox2.SelectedIndex == 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);

                        if (isChecked == true)
                        {
                            int facturaZ = Convert.ToInt32(row.Cells["DocumentoZamenis"].Value.ToString());
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
                                if (TokenGenerado == null)
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
                                }
                                else
                                {
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

                                            BuscarDocumentos();

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
                        }
                    }

                    BuscarDocumentos();

                    ResultadoRadicacion R = new ResultadoRadicacion(listResultados);
                    R.ShowDialog();
                }
                //FACTURAS DE VENTANILLA
                else if (comboBox2.SelectedIndex == 1)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);

                        if (isChecked == true)
                        {
                            int facturaZ = Convert.ToInt32(row.Cells["DocumentoZamenis"].Value.ToString());
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
                                if (TokenGenerado == null)
                                {
                                    listResultados.Add(new CXN_FACTURA
                                    {
                                        Fac_Tipo_Doc = "Ventas",
                                        Fac_Estado = "Error",
                                        Fac_Num_Fac = Convert.ToInt32(facturaZ),
                                        Fac_Cia = Cia,
                                        Homologo = "",
                                        Fac_Observa = "No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion"
                                    });
                                }
                                else
                                {
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


                                                    Image Code_QR_Fac_CUFE = repoGen.CodifyQR(QrElectron);

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
                                                            QRCufe = repoGen.GetBytes(Code_QR_Fac_CUFE),
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

                                            BuscarDocumentos();

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
                        }                        
                    }

                    BuscarDocumentos();

                    ResultadoRadicacion resultadoRadicacion = new ResultadoRadicacion(listResultados);
                    resultadoRadicacion.ShowDialog();
                }
                //FACTURAS DE RECAUDO DE BONOS - CAJA
                else if (comboBox2.SelectedIndex == 2)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);

                        if (isChecked == true)
                        {
                            int facturaZ = Convert.ToInt32(row.Cells["DocumentoZamenis"].Value.ToString());
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
                                if (TokenGenerado == null)
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
                                }
                                else
                                {
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
                                                FacturaZamenis = facturaZ,
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

                                                List<RCCAJA> Exportar = ExportarPDF.ExportarReciboCaja(facturaZ);
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

                                            BuscarDocumentos();

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
                        }
                    }

                    BuscarDocumentos();

                    ResultadoRadicacion resultadoRadicacion = new ResultadoRadicacion(listResultados);
                    resultadoRadicacion.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral
                    {
                        Mensaje = "Seleccion Invalida",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
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
                Dictionary<string, string> dataWS = repoWSClients.Claves("Factura1Token", Cia);
                if (dataWS == null)
                {
                    throw new Exception("No se encontraron las claves de acceso para el prestador especificado.");
                }

                GenerarToken G = new GenerarToken();
                var resAPIToken = G.GetToken(dataWS["User"], dataWS["Pass"], Cia, Program.URLApiConexion).GetAwaiter().GetResult();
                
                if (resAPIToken.StatusCode == "OK")
                {
                    if (repoFelectron.insertToken(resAPIToken.xml, Cia) == "OK")
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "Token Generado Exitosamente.",
                            TipoImagen = 3
                        };
                    }
                    else
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "No se pudo grabar el token en la base de datos.",
                            TipoImagen = 1000
                        };
                    }
                }
                else
                {
                    throw new Exception("No se pudo generar el token de acceso. Verifique las credenciales.");
                }
               
                MG.ShowDialog();
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

        private void button5_Click(object sender, EventArgs e)
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

        private void button6_Click(object sender, EventArgs e)
        {
            NotaCredito notaCredito = new NotaCredito();
            notaCredito.ShowDialog();
        }
    }
}
