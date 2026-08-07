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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class Facturar2 : Forma
    {
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly ICompañia repoCompañia = new MCompañia();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();
        private static readonly ICManejo repoCManejo = new MCManenejo();
        private static readonly IReportes repoReportes = new MReportes();
        private static readonly IRcCaja repoRCCaja = new MRcCaja();
        private static readonly IAgendaC repoAge = new MAgendaC();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly IFacElectron repoFacelectron = new MFacElectron();
        private static readonly IImpuestos impuestos = new MImpuestos();
        private static readonly IFirmasDigitales fDigitales = new MFirmasDigitales();
        private static readonly IAseguradoras fAseguradora = new MAseguradoras();

        public int Ase, Cia, Pac_Id;
        public bool FacIndividual;
        public DateTime Desde, Hasta;
        private Dictionary<int, string> D;
        private string NitCia;
        private int globalVal, AdmisionInicial;
        private bool Rips2000;
        private ToolStripButton btnGrabar;
        private otrosDatosPacienteHorario otherData;
        private List<otrosDatosPacienteHorario> H;
        MensajesGeneral MG;

        DataTable dt;
        DataColumn Tipo;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Cantidad;
        DataColumn Vr_Unitario;
        DataColumn Vr_Total;

        public string AFacturar;
        private List<int> listAdmitionToSumVal;
        public Facturar2(bool rips2000)
        {
            InitializeComponent();

            SoloNumeros(textBox7);
            SoloNumeros(textBox2);
            SoloNumeros(textBox8);
            SoloNumeros(textBox9);
            SoloNumeros(textBox11);
            SoloNumeros(textBox13);
            this.Rips2000 = rips2000;
        }

        void CargarMediosPago()
        {
            try
            {
                List<CXN_MEDIOSPAGO> getMedios = repoFacelectron.ListaMediosPago();
                if (getMedios != null)
                {
                    comboBox5.Items.Clear();

                    foreach (CXN_MEDIOSPAGO m in getMedios)
                    {
                        comboBox5.Items.Add(m.Medio);
                    }

                    comboBox5.Text = "Consignación bancaria";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Facturar2_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Generar Prefactura";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Generar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += toolStripButton6_Click;

                gridZH1.CeldaHeight = true;
                CargarMediosPago();                
            
                List<string> Regi = repoPacientes.ListaRegimen();
                if (Regi != null)
                {
                    foreach (string r in Regi)
                    {
                        comboBox1.Items.Add(r);
                    }
                }

                if (this.AFacturar != "Curaciones y Consultas")
                {
                    checkBox1.Visible = false;
                    checkBox2.Visible = false;
                }               

                //Obtener suma de cuotas moderadoras
                Facturar fTemp = Application.OpenForms.OfType<Facturar>().LastOrDefault();
                List<int> numAdmToFac = new List<int>();

                foreach (DataGridViewRow row in fTemp.dataGridView1.Rows)
                {
                    int Admition = Convert.ToInt32(row.Cells["Admision"].Value.ToString());
                    numAdmToFac.Add(Admition);
                }

                listAdmitionToSumVal = removeDuplicates(numAdmToFac);
                AdmisionInicial = listAdmitionToSumVal[0];

                otrosDatosPacienteHorario getAut = repoAge.cargarAdmision(AdmisionInicial, "'H'");
                if (getAut != null)
                {
                    textBox1.Text = getAut.Hor_Autoriza;
                    textBox13.Text = getAut.Hor_CantSesion.ToString();
                }

                int ValDescuentoCopago01 = 0;
                int ValDescuentoCMod02 = 0;
                int ValDescuentoVrCompartido03 = 0;
                int ValDescuentoAnticipo04 = 0;

                if (this.Rips2000 == true)
                {
                    H = new List<otrosDatosPacienteHorario>();
                    CXN_CIA DataCompany = repoCompañia.getPrestadorbyCode(Cia);
                    CXN_PACIENTES DataPaciente = repoPacientes.LlamarPacientebyId(Pac_Id);

                    textBox12.Text = DataPaciente.Pac_Email.ToString().Trim();

                    foreach (int numRC in listAdmitionToSumVal)
                    {
                        otherData = repoAge.cargarAdmision(numRC, "'H','P'");

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

                                ValDescuentoCopago01 = ValDescuentoCopago01 + dato.Valor;
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

                                if (repoRCCaja.EsConsulta(numRC, "DX") == true)
                                {
                                    ValDescuentoCMod02 = ValDescuentoCMod02 + dato.Valor; //sumar cuotas moderadoras consultas
                                }
                                if (repoRCCaja.EsConsulta(numRC, "QX") == true)
                                {
                                    ValDescuentoAnticipo04 = ValDescuentoAnticipo04 + dato.Valor; //sumar cuotas moderadoras notas
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

                                ValDescuentoVrCompartido03 = ValDescuentoVrCompartido03 + dato.Valor;
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

                                ValDescuentoAnticipo04 = ValDescuentoAnticipo04 + dato.Valor;
                                break;
                        }
                    }
                }
                if (this.Rips2000 == false)
                {
                    foreach (int numRC in listAdmitionToSumVal)
                    {
                        (int Valor, string Concepto) dato = repoRCCaja.getValRcCaja(numRC);
                        switch (dato.Concepto)
                        {
                            case "01":
                                ValDescuentoCopago01 = ValDescuentoCopago01 + dato.Valor;
                                break;
                            case "02":
                                ValDescuentoCMod02 = ValDescuentoCMod02 + dato.Valor; //sumar cuotas moderadoras consultas
                                break;
                            case "03":
                                ValDescuentoVrCompartido03 = ValDescuentoVrCompartido03 + dato.Valor;
                                break;
                            case "04":
                                ValDescuentoAnticipo04 = ValDescuentoAnticipo04 + dato.Valor;
                                break;
                        }
                    }
                }
               
                textBox7.Text = ValDescuentoCopago01.ToString();
                textBox2.Text = ValDescuentoCMod02.ToString();
                textBox8.Text = ValDescuentoVrCompartido03.ToString();
                textBox9.Text = ValDescuentoAnticipo04.ToString();

                if (Convert.ToInt32(textBox7.Text) > 0 || Convert.ToInt32(textBox2.Text) > 0 ||
                    Convert.ToInt32(textBox8.Text) > 0 || Convert.ToInt32(textBox9.Text) > 0)
                {
                    checkBox2.Checked = true;
                }

                //fin suma de cuotas moderadoras

                if (FacIndividual == true)
                {
                    CargaValores();
                }

                if (FacIndividual == false)
                {
                    btnGrabar.Enabled = false;
                    CargaValoresGlobal();
                }

                CXN_PACIENTES P = repoPacientes.LlamarPacientebyId(this.Pac_Id);
                if (P != null)
                {
                    textBox5.Text = P.Pac_Contrato;
                    comboBox1.Text = repoPacientes.Carga_Regimen(P.Pac_Regimen);
                }

                CXN_CIA C = repoCompañia.getPrestadorbyCode(this.Cia);
                if (C != null)
                {
                    textBox6.Text = C.Com_Cod_Prestador_2.ToString();
                    NitCia = C.Com_Identificacion;
                }

                ToolTip toolTip0 = new ToolTip();
                toolTip0.ShowAlways = true;
                toolTip0.SetToolTip(checkBox1, "Marque esta opcion para generar en la carpeta de reportes los documentos facturados");

                ToolTip toolTip1 = new ToolTip();
                ToolTip toolTip2 = new ToolTip();
                ToolTip toolTip3 = new ToolTip();
                ToolTip toolTip4 = new ToolTip();
                ToolTip toolTip5 = new ToolTip();
                ToolTip toolTip6 = new ToolTip();
                ToolTip toolTip7 = new ToolTip();
                ToolTip toolTip8 = new ToolTip();
                ToolTip toolTip9 = new ToolTip();
                ToolTip toolTip10 = new ToolTip();
                ToolTip toolTip11 = new ToolTip();

                toolTip1.ShowAlways = true;
                toolTip2.ShowAlways = true;
                toolTip3.ShowAlways = true;
                toolTip4.ShowAlways = true;
                toolTip5.ShowAlways = true;
                toolTip6.ShowAlways = true;
                toolTip7.ShowAlways = true;
                toolTip8.ShowAlways = true;
                toolTip9.ShowAlways = true;
                toolTip10.ShowAlways = true;
                toolTip11.ShowAlways = true;

                toolTip1.SetToolTip(textBox2, "Se debe registrar el valor total efectivamente pagado por el usuario y recaudado por el prestador de servicios de salud o el proveedor de tecnologias en salud, correspondiente al copago. En caso de facturas multiusuario, se registra la sumatoria del valor total del copago pagado por cada usuario. Este valor debe corresponder con el valor total de los copagos registrados en RIPS.\r\n\r\nObligatorio cuando se facture por usuario multiusuario. \r\n\r\nValores permitidos: Numérico. Se valida que no existan valores negativos. Valor sin símbolos ni separadores de miles y con el signo punto como separador de decimales.");
                toolTip2.SetToolTip(comboBox1, "Regimen del Paciente");
                toolTip3.SetToolTip(textBox7, "Descripción / Contenido: Se debe registrar el valor efectivamente pagado por el usuario y recaudado por el prestador de servicios de salud o el proveedor de tecnologías en salud, correspondiente a la cuota moderadora. Se debe registrar el valor pagado por el usuario. En caso de facturas multiusuario, se registra la sumatoria del valor total de cuotas moderadoras pagado por cada usuario. Este valor debe corresponder con el valor total de las cuotas \r\nmoderadoras registradas en RIPS. \r\n\r\nDiligenciamiento en la factura electrónica de venta: Obligatorio cuando se facture por usuario o multiusuario. \r\n\r\nValores permitidos: Numérico. Se valida que no existan valores negativos. Valor sin símbolos ni separadores de miles y con el signo punto como separador de decimales. ");
                toolTip4.SetToolTip(textBox6, "Descripción / Contenido: Debe registrarse el código asignado en el Sistema General de \r\nSeguridad Social en Salud (SGSSS) a los prestadores de servicios de salud que estén en el \r\nRegistro Especial de Prestadores de Servicios de Salud (REPS), o el código asignado por el \r\nMinisterio de Salud y Protección Social para los para los Proveedores de Tecnologías en Salud \r\ny demás casos de excepción. \r\nDiligenciamiento en la factura electrónica de venta: Obligatorio. \r\nValores permitidos: Texto. Diligenciar con los valores de la tabla de instituciones \r\n\"IPSCodHabilitación\" para prestadores de servicios de salud o con el código según aplique, de \r\nla tabla \"IPSnoREPS\" para los Proveedores de Tecnologías en Salud o demás casos de \r\nexcepción");
                toolTip5.SetToolTip(textBox8, "Descripción / Contenido: Se debe registrar el valor efectivamente pagado por el usuario y recaudado por el prestador de servicios de salud, correspondiente al pago compartido en los planes voluntarios de salud (medicina prepagada, pólizas de salud y planes complementarios en salud). Se registra únicamente el valor total del pago compartido recaudado directamente por el prestador de servicios de salud. En caso de facturas multiusuario, se registra la sumatoria del valor total de pagos compartidos pagado por cada usuario. Este valor debe corresponder con el valor total de los pagos compartidos registrados en RIPS. \r\n\r\nDiligenciamiento en la factura electrónica de venta: Obligatorio cuando se facture por usuario multiusuario.\r\n\r\nValores permitidos: Numérico. Se valida que no existan valores negativos. Valor sin símbolos ni separadores de miles y con el signo punto como separador de decimales.");
                toolTip6.SetToolTip(textBox1, "Número asignado por la \r\nentidad responsable de pago \r\ny demás pagadores a los que \r\naplique, para ordenar la \r\nprestación de servicios,");
                toolTip7.SetToolTip(textBox9, "Descripción 1 Contenido: Se debe registrar el valor del anticipo a legalizar, que se restará al valor de la presente factura en ejecución del contrato señalado en el numeral 3.4. \r\n\r\nDiligenciamiento en la factura électrónica de venta: Obligatorio cuando se hayan pactado anticipos en el acuerdo de voluntades. \r\n\r\nValores permitidos: Numérico. Se valida que no existan valores negativos. Valor sin símbolos ni separadores de miles y con el signo punto como separador de decimales.");
                toolTip8.SetToolTip(textBox5, "Descripción ¡Contenido: Se debe registrar el número del contrato objeto de facturación. \r\n\r\nDiligenciamiento en la factura electrónica de venta: Obligatorio cuando exista contrato o en caso contrario irá vacío. Se reporta solo si se ha suscrito contrato que cubra los ítems facturados. Las entidades obligadas a registrarse en el portal del Registro de Contratación de Servicios y Tecnologías de Salud (Artículo 4, Ley 1966 de 2019), deberán diligenciar el código del número de contrato que les expida esta plataforma una vez esté disponible. \r\n\r\nValores permitidos: Un valor único. Alfanumérico.");
                toolTip9.SetToolTip(comboBox2, "Diligenciamiento en la factura electrónica de venta: Obligatorio. Debe registrarse la \r\nmodalidad de pago acordada, de acuerdo con la definición contenida en el artículo 2.5.3.4.2.3 \r\ndel Decreto 780 de 2016. En caso de facturas multiusuario, todos deben pertenecer a la misma \r\nmodalidad de pago y a la misma cobertura o plan de beneficios. \r\nValores permitidos: Texto. Diligenciar con los valores de la tabla de referencia \r\n\"modalidadPago\", disponible en web.sispro.gov.co., los que son excluyentes entre sí. ");
                toolTip10.SetToolTip(comboBox3, "Diligenciamiento en la factura electrónica de venta: Obligatorio. Debe registrarse la cobertura o el plan de beneficios a la que pertenece el usuario. En caso de facturas mulfiusuario, todos deben pertenecer a la misma cobertura o plan de beneficios y a la misma modalidad de pago. \r\n\r\nValores permitidos: Texto. Diligenciar con los valores de la tabla de referencia \"coberturaPlan\", \r\ndisponible en web.sispro.gov.co., los que son excluyentes entre sí.");
                toolTip11.SetToolTip(textBox3, "Observaciones voluntarias a la factura");

                comboBox4.SelectedIndex = 0; // Selecciona el primer elemento del comboBox4

                if (Preferencias.TabletaFirmas != "A")
                {
                    checkBox3.Visible = false;
                    boton1.Visible = false;
                    boton2.Visible = false;
                }

                comboBox3.Items.Add("02 - Presupuesto máximo");
                comboBox3.Items.Add("03 - Prima EPS / EOC, no asegurados SOAT");
                comboBox3.Items.Add("04 - Cobertura Póliza SOAT");
                comboBox3.Items.Add("05 - Cobertura ARL");
                comboBox3.Items.Add("06 - Cobertura ADRES");
                comboBox3.Items.Add("07 - Cobertura Salud Pública");
                comboBox3.Items.Add("08 - Cobertura entidad territorial, recursos de oferta");
                comboBox3.Items.Add("09 - Urgencias población migrante");
                comboBox3.Items.Add("10 - Plan complementario en salud");
                comboBox3.Items.Add("11 - Plan medicina prepagada");
                comboBox3.Items.Add("12 - Pólizas en salud");
                comboBox3.Items.Add("13 - Cobertura Régimen Especial o Excepción");
                comboBox3.Items.Add("14 - Cobertura Fondo Nacional de Salud de las Personas Privadas de la Libertad");
                comboBox3.Items.Add("15 - Particular");
                comboBox3.Items.Add("16 - Plan de beneficios en Salud dinanciado con UPC contributivo");
                comboBox3.Items.Add("17 - Plan de beneficios en Salud dinanciado con UPC subsidiado");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }    
        static List<T> removeDuplicates<T>(List<T> list)
        {
            return new HashSet<T>(list).ToList();
        }
        void GenerarSoportedePagos()
        {
            try
            {
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
                FileStream fss = new FileStream("C:\\CXN\\Reportes\\CRC_" + NitCia.ToString() + "_FACTURA.pdf", FileMode.Create);
                fss.Write(bytes, 0, bytes.Length);
                fss.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void GenerarLote()
        {
            try
            {
                MenuLateral.Enabled = false;
                checkBox1.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                comboBox1.Enabled = false;

                D = new Dictionary<int, string>();

                Facturar f2 = Application.OpenForms.OfType<Facturar>().LastOrDefault();

                foreach (DataGridViewRow row in f2.dataGridView1.Rows)
                {
                    string Texto = row.Cells["TipoCargo"].Value.ToString();
                    int Admition = Convert.ToInt32(row.Cells["Admision"].Value.ToString());

                    if (Texto == "Nota")
                    {
                        D.Add(Convert.ToInt32(Admition), "Nota");
                    }
                    if (Texto == "Historia")
                    {
                        D.Add(Convert.ToInt32(Admition), "Historia");
                    }
                }

                if (this.AFacturar == "Curaciones y Consultas")
                {
                    if (checkBox1.Checked == true)
                    {
                        NotasTotal();
                    }
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 0;
                    MG.Mensaje = "Este proceso solo esta disponible para curaciones y consultas por el momento, se procedera a continuar sin generar el lote";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void NotasTotal()
        {
            try
            {
                Dictionary<int, object> getRDLCMasivo = repoReportes.NotasMetodoRDLC(D);

                if (getRDLCMasivo != null)
                {
                    //CIURACIONES
                    int Contador = 1;
                    ReportViewer R = new ReportViewer();

                    foreach (KeyValuePair<int, object> kvp in getRDLCMasivo)
                    {                        
                        if (kvp.Value is List<ReportNotas>)
                        {
                            List<ReportNotas> listaClase1 = (List<ReportNotas>)getRDLCMasivo[kvp.Key];

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

                    if (Contador > 1)
                    {
                        Contador = Contador - 1;
                        Comunes.UnificadorPDFMasivo U = new Comunes.UnificadorPDFMasivo();
                        U.Unificar_Estructura(@"C:\CXN\Reportes\Notas\",
                                              1.ToString(),
                                              Contador.ToString(),
                                              "PDX_" + NitCia + "_FACTURA.pdf");

                        foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Notas\", "*.pdf"))
                        {
                            BorrarArchivoConReintentos(archivo);
                        }
                    }                    

                    //MEDICINA GENERAL
                    Contador = 1;

                    foreach (KeyValuePair<int, object> kvp in getRDLCMasivo)
                    {
                        if (kvp.Value is List<HCMG>)
                        {
                            List<HCMG> listaClase2 = (List<HCMG>)getRDLCMasivo[kvp.Key];

                            R.LocalReport.DataSources.Clear();
                            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_HCMG", listaClase2));
                            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_HCMG.rdlc";
                            R.SetDisplayMode(DisplayMode.PrintLayout);
                            R.ZoomMode = ZoomMode.Percent;
                            R.ZoomPercent = 100;
                            R.Font = new System.Drawing.Font("Arial", 7);
                            R.LocalReport.EnableExternalImages = true;
                            R.RefreshReport();
                            //maestro.Visible = true;
                            R.Dock = System.Windows.Forms.DockStyle.Fill;

                            byte[] bytes = R.LocalReport.Render("PDF");
                            FileStream fss = new FileStream("C:\\CXN\\Reportes\\Historias\\" + Contador.ToString() + ".pdf", FileMode.Create);
                            fss.Write(bytes, 0, bytes.Length);
                            fss.Close();
                            Contador = Contador + 1;
                        }
                    }

                    if (Contador > 1)
                    {
                        Contador = Contador - 1;
                        Comunes.UnificadorPDFMasivo U1 = new Comunes.UnificadorPDFMasivo();
                        U1.Unificar_Estructura(@"C:\CXN\Reportes\Historias\",
                                              1.ToString(),
                                              Contador.ToString(),
                                              "HEV_" + NitCia + "_FACTURA.pdf");

                        foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Historias\", "*.pdf"))
                        {
                            BorrarArchivoConReintentos(archivo);
                        }
                    }                    

                    //CAMBIOS DE MANEJO
                    Contador = 1;

                    List<int> getIdCamManejo = repoCManejo.getManejosxPaciente(this.Pac_Id, this.Desde, this.Hasta);
                    if (getIdCamManejo != null)
                    {
                        foreach (int i in getIdCamManejo)
                        {
                            List<ReportCMAN> H_HCCMAN = repoReportes.CambiosManejo(i);
                            if (H_HCCMAN != null)
                            {
                                R.LocalReport.DataSources.Clear();
                                R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CMAN", H_HCCMAN));
                                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_CMAN.rdlc";
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
                    }

                    if (Contador > 1)
                    {
                        Contador = Contador - 1;
                        Comunes.UnificadorPDFMasivo U2 = new Comunes.UnificadorPDFMasivo();
                        U2.Unificar_Estructura(@"C:\CXN\Reportes\Notas\",
                                              1.ToString(),
                                              Contador.ToString(),
                                              "OPF_" + NitCia + "_FACTURA.pdf");


                        foreach (string archivo in Directory.GetFiles(@"C:\CXN\Reportes\Notas\", "*.pdf"))
                        {
                            BorrarArchivoConReintentos(archivo);
                        }
                    }                    

                    R.Dispose();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
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
        private void ExportarDocumento(int Documento, int Compañia, string Tipo, string TReport)
        {
            try
            {
                string R = repoPacientes.Regimen(comboBox1.Text);
                repoPacientes.UpdateFac2(this.Pac_Id, textBox5.Text, R);

                List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_Export(Documento, Compañia, Tipo);
                if (GenerarDocumentoGrafico == null)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 0;
                    MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                    MG.ShowDialog();
                    return;
                }

                Extras.TipoReportFactura R2 = new Extras.TipoReportFactura(null, GenerarDocumentoGrafico, false);
                R2.ShowDialog();             

                Facturar f2 = Application.OpenForms.OfType<Facturar>().LastOrDefault();
                f2.Encabezados();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private async void toolStripButton6_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text == "" || textBox7.Text == "" || textBox8.Text == "" || textBox9.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La seccion de descuentos monetarios no puede estar vacia o con valores negativos";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.Text == "" || comboBox1.Text == " ")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione regimen";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox6.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione una forma de pago";
                    MG.ShowDialog();
                    return;
                }

                if (textBox6.Text == "" || textBox5.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Los campos de letra azul deben ser diligenciados completos";
                    MG.ShowDialog();
                    return;
                }

                if (string.IsNullOrEmpty(textBox11.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe escribir el tiempo de vencimiento de la factura";
                    MG.ShowDialog();
                    return;
                }

                DialogResult result = MessageBox.Show("¿Desea generar esta orden de pedido?",
                    "Zamenis Health - Facturacion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (checkBox1.Checked == true)
                    {
                        Task oTask = null;
                        oTask = new Task(GenerarLote);

                        if (oTask != null)
                        {
                            oTask.Start();
                            await oTask;
                            //codigo cuando termine aqui
                            MenuLateral.Enabled = true;
                            checkBox1.Enabled = true;
                            textBox1.Enabled = true;
                            textBox2.Enabled = true;
                            textBox3.Enabled = true;
                            comboBox1.Enabled = true;
                        }
                    }

                    if (checkBox2.Checked == true) 
                    {
                        GenerarSoportedePagos();
                    }

                    DateTime Hoy = DateTime.Now;

                    //TRAE CONSECUTIVO DOCUMENTO
                    CXN_CIA Doc = repoCompañia.getPrestadorbyCode(Convert.ToInt32(Cia));

                    if (Doc == null)
                    {
                        MessageBox.Show("Error en consecutivos de facturacion, NO se puede continuar",
                            "Error Grave",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    //ACTUALIZAR DOCUMENTO PARTE 1/2
                    int DocumentoNuevo = Convert.ToInt32(Doc.Com_OP) + 1;

                    //REGIMEN
                    string regi = repoPacientes.Regimen(comboBox1.Text);

                    CXN_FACTURA F = new CXN_FACTURA
                    {
                        Fac_Num_Fac = Convert.ToInt32(Doc.Com_OP),
                        Fac_Res = "NO APLICA RESOLUCION ES ORDEN DE PEDIDO",
                        Homologo = Doc.Com_OP.ToString(),
                        Fac_Estado = "F",
                        Fac_Ase = Convert.ToInt32(Ase),
                        Fac_Cia = Convert.ToInt32(Cia),
                        Fac_Pac = Convert.ToInt32(Pac_Id),
                        Fac_Fecha = Hoy,
                        Fac_Fecha_Des = Convert.ToDateTime(Desde),
                        Fac_Fecha_Has = Convert.ToDateTime(Hasta),
                        Fac_Num_Aut = textBox1.Text,
                        Fac_Descuento = textBox2.Text,
                        Fac_Observa = textBox3.Text,
                        Fac_Usr_Graba = Comunes.Contenedor.UsuarioLogueado,
                        Fac_ConSub = regi.ToString(),
                        Fac_Tipo_Doc = "OP",
                        VrCompartido = Convert.ToInt32(textBox8.Text),
                        Copago = Convert.ToInt32(textBox7.Text),
                        Anticipo = Convert.ToInt32(textBox9.Text),
                        CodPrestador = textBox6.Text,
                        ContratoPoliza = textBox5.Text,
                        Cobertura = comboBox3.Text,
                        ModPago = comboBox2.Text,
                        DiasVencimiento = Convert.ToInt32(textBox11.Text),
                        MetodoPago = comboBox4.Text,
                        MedioPago = comboBox5.Text,
                        FormaPago = comboBox6.Text,
                        PercentICA = label24.Text,
                        PercentFUENTE = label23.Text,
                        Num_Cruce = 0
                    };

                    bool GrabarDocumento = repoFacturacion.Graba_Factura_Orden(F);
                    if (GrabarDocumento == false)
                    {
                        MessageBox.Show("No se logro grabar la factura, vuelva a intentar",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        bool GrabarCargo = repoFacturacion.Actualiza_Cargo_Facturado(F, AFacturar, false);
                        if (GrabarCargo == false)
                        {
                            MessageBox.Show("No se logro actualizar los cargos de la factura, esta es una incidencia grave, NO vuelva a intentar " +
                                "contacte al desarrollador inmediatamente sin cerrar esta pantalla",
                                "Error Gave",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            int subtotal = repoFacturacion.getTotalFac(F.Fac_Cia, F.Fac_Num_Fac);
                            decimal fuentetemp = impuestos.Fuente(subtotal, label23.Text);
                             decimal icatemp = impuestos.ICA(label24.Text);
                            decimal VrReteICA = subtotal * icatemp;

                            repoFacturacion.UpdateFuenteICA((int)fuentetemp, (int)VrReteICA, F.Fac_Num_Fac, F.Fac_Cia);

                            bool _updateCons = repoCompañia.ConsecutivoActualiza(Convert.ToInt32(Cia), "OP", DocumentoNuevo);
                            if (_updateCons == false)
                            {
                                MessageBox.Show("Su documento ha sido facturado con el consecutivo " + Doc.Com_OP.ToString() +
                                    " pero no se ha actualizado corretamente el siguiente numero, antes de seguir facturando reporte este " +
                                    "incidente de inmediato al desarrollador de la aplicacion",
                                    "Error grave",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                this.Dispose();
                                this.Close();
                                return;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(textBox12.Text))
                                {
                                    if (repoPacientes.ValidaEmail(textBox12.Text) == true)
                                    {
                                        repoPacientes.Actualiza_Email(this.Pac_Id, textBox12.Text);
                                    }
                                }

                                if (checkBox3.Checked == true)
                                {
                                    FirmasPrint(false);
                                }                               

                                MessageBox.Show("Hecho, numero de documento: " + Doc.Com_OP.ToString(),
                                                "Facturado Exitoso",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Exclamation);

                                string R = repoPacientes.Regimen(comboBox1.Text);
                                repoPacientes.UpdateFac2(this.Pac_Id, textBox5.Text, R);

                                DialogResult result2 = MessageBox.Show("¿Desea Imprimir el Documento?",
                                    "Zamenis Health - Facturacion",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

                                if (result2 == DialogResult.Yes)
                                {
                                    //CODIGO PARA EXPORTAR FACTURA
                                    ExportarDocumento(Convert.ToInt32(Doc.Com_OP), Cia, "OP", "");
                                }
                                if (result2 == DialogResult.No)
                                {
                                    Facturar f2 = Application.OpenForms.OfType<Facturar>().LastOrDefault();
                                    f2.Encabezados();
                                    this.Dispose();
                                    this.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void FirmasPrint(bool See)
        {
            try
            {
                //otrosDatosPacienteHorario dataAdm1 = repoAge.cargarAdmision(Adm_Selected, "'H','A','P'");
                CXN_CIA DatNombre = repoCompañia.getPrestadorbyCode(Cia);
                CXN_PACIENTES pacData = repoPacientes.LlamarPacientebyId(Pac_Id);
                CXN_ASEGURADORA aseData = fAseguradora.getInfoFromAsebyCode(pacData.Pac_Aseguradora);

                //List<(int Adm, string Aut, int Cant)> listAdmition = fDigitales.getAdmitionsByFacturacion(Pac_Id, Cia, Hasta.Date, Desde.Date);

                if (listAdmitionToSumVal != null)
                {
                    List<FirmasR> lista = new List<FirmasR>();
                    int Contador = 1;

                    foreach (var i in listAdmitionToSumVal)
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
                            Com_Direccion = textBox1.Text,// listAdmition[0].Aut, //autorizacion
                            Admision = i,
                            Cantidad = Convert.ToInt32(textBox13.Text),// i.Cant
                        });

                        if (!dataAdm.Com_Nombre_SMS.Contains("CONSULTA"))
                        {
                            Contador++;
                        }
                    }

                    if (See == false)
                    {
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
                        FileStream fss = new FileStream("C:\\CXN\\Reportes\\CRC_" + NitCia.ToString() + "_FACTURA.pdf", FileMode.Create);
                        fss.Write(bytes, 0, bytes.Length);
                        fss.Close();
                    }
                    else
                    {
                        ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.FirmasDigitales.rdlc", lista);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CargaValores()
        {
            try
            {
                String Query = "";
                switch (AFacturar)
                {
                    case "Todo":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND P.Pac_Id = '" + Pac_Id + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Curaciones y Consultas":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND P.Pac_Id = '" + Pac_Id + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('CU','MG') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Terapias":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND P.Pac_Id = '" + Pac_Id + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('TF','TO','PS') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Fisiatria":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                                "FROM CXN_CARGOS C " +
                                "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND P.Pac_Id = '" + Pac_Id + "' " +
                                "AND C.Car_Estado = 'G' " +
                                "AND C.Car_Cia = '" + Cia + "' " +
                                "AND C.Car_Ase = '" + Ase + "' " +
                                "AND C.Car_Tipo_Serv = 'FI' " +
                                "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Terapias y Fisiatria":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND P.Pac_Id = '" + Pac_Id + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('TF','TO','PS','FI') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Radiologia":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND P.Pac_Id = '" + Pac_Id + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('RA') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    default:
                        MessageBox.Show("Seleccione un tipo de datos a facturar",
                            "Falta informacion",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        this.Dispose();
                        this.Close();
                        return;
                }
                FiltroFacturas(Query);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }             
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Extras.ConsAutorizacion a = new Extras.ConsAutorizacion();
            a.ShowDialog();
        }
        void LeaveTextValores()
        {
            try
            {
                if (textBox7.Text.Length <= 0) { textBox7.Text = "0"; };
                if (textBox2.Text.Length <= 0) { textBox2.Text = "0"; };
                if (textBox8.Text.Length <= 0) { textBox8.Text = "0"; };
                if (textBox9.Text.Length <= 0) { textBox9.Text = "0"; };
                double vaT = globalVal - Convert.ToInt32(textBox7.Text) - Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox8.Text) - Convert.ToInt32(textBox9.Text);
                textBox10.Text = vaT.ToString("N0");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox7_Leave(object sender, EventArgs e)
        {
            LeaveTextValores();
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            LeaveTextValores();
        }
        private void textBox8_Leave(object sender, EventArgs e)
        {
            LeaveTextValores();
        }
        private void textBox9_Leave(object sender, EventArgs e)
        {
            LeaveTextValores();
        }
        private void CargaValoresGlobal()
        {
            try
            {
                String Query = "";
                switch (AFacturar)
                {
                    case "Todo":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Curaciones y Consultas":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('CU','MG') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Terapias":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('TF','TO','PS') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Fisiatria":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                                "FROM CXN_CARGOS C " +
                                "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                                "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                "AND C.Car_Estado = 'G' " +
                                "AND C.Car_Cia = '" + Cia + "' " +
                                "AND C.Car_Ase = '" + Ase + "' " +
                                "AND C.Car_Tipo_Serv = 'FI' " +
                                "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Terapias y Fisiatria":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('TF','TO','PS','FI') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    case "Radiologia":
                        Query = "SELECT C.Car_Cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo, " +
                                "SUM(CAST(C.Car_Cant AS INT)) AS Cantidad, " +
                                "SUM(CAST(C.Car_Val_Tot AS INT)) AS Total " +
                               "FROM CXN_CARGOS C " +
                               "INNER JOIN CXN_PACIENTES P ON C.Car_Pac = P.Pac_Id " +
                               "WHERE C.CAR_FECHA BETWEEN '" + Convert.ToDateTime(Desde.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                               "AND C.Car_Estado = 'G' " +
                               "AND C.Car_Cia = '" + Cia + "' " +
                               "AND C.Car_Ase = '" + Ase + "' " +
                               "AND C.Car_Tipo_Serv IN ('RA') " +
                               "GROUP BY C.Car_cod, C.Car_Item, C.Car_Val_Un, C.Car_Tipo";
                        break;

                    default:
                        MessageBox.Show("Seleccione un tipo de datos a facturar",
                            "Falta informacion",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        this.Dispose();
                        this.Close();
                        return;
                }
                FiltroFacturas(Query);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void label23_DoubleClick(object sender, EventArgs e)
        {
            Facturacion.Extras.Retenciones retenciones = new Facturacion.Extras.Retenciones("Fuente", "Salud");
            retenciones.ShowDialog();
        }
        private void label24_DoubleClick(object sender, EventArgs e)
        {
            Facturacion.Extras.Retenciones retenciones = new Facturacion.Extras.Retenciones("Ica", "Salud");
            retenciones.ShowDialog();
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true) 
            {
                checkBox3.Checked = false;
            }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                checkBox2.Checked = false;
            }
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                //AGREGAR AUTORIZACION A CXN_HORARIO
                repoAge.addAutroizacion(AdmisionInicial,
                                        textBox1.Text,
                                        string.IsNullOrEmpty(textBox13.Text) ? 0 : Convert.ToInt32(textBox13.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            //FirmasPrint(true);
            Facturar3 facturar3 = new Facturar3(listAdmitionToSumVal, Pac_Id);
            facturar3.ShowDialog();
        }
        private void Encabezados()
        {
            dt = new DataTable();
            Tipo = dt.Columns.Add("Tipo", typeof(string));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Item = dt.Columns.Add("Item", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
            Vr_Unitario = dt.Columns.Add("Vr_Unitario", typeof(string));
            Vr_Total = dt.Columns.Add("Vr_Total", typeof(string));
        }
        private void FiltroFacturas(string Query)
        {
            try
            {
                (List<CXN_CARGOS> lista, int ValorFac) getCargos = repoFacturacion.FiltroFacturas(Query);
                if (getCargos.lista != null)
                {
                    Encabezados();

                    foreach (CXN_CARGOS i in getCargos.lista)
                    {
                        DataRow row = dt.NewRow();

                        row["Tipo"] = i.Car_Tipo.ToString();
                        row["Codigo"] = i.Car_Cod.ToString();
                        row["Item"] = i.Car_Item.ToString();
                        row["Cantidad"] = Convert.ToInt32(i.Car_Cant);
                        row["Vr_Unitario"] = Convert.ToInt32(i.Car_Val_Un).ToString("N0");
                        row["Vr_Total"] = Convert.ToInt32(i.Car_Val_Tot).ToString("N0");

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        textBox4.Text = "$ " + getCargos.ValorFac.ToString("N0");
                        double ValNeto = Convert.ToInt32(getCargos.ValorFac) - Convert.ToInt32(textBox7.Text) - Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox8.Text) - Convert.ToInt32(textBox9.Text);
                        textBox10.Text = "$ " + ValNeto.ToString("N0");
                        globalVal = Convert.ToInt32(getCargos.ValorFac);
                    }

                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    Encabezados();

                    MessageBox.Show("No hay valores a facturar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["Tipo"].Visible = false;
            D.EnableHeadersVisualStyles = false;

            foreach (DataGridViewRow row in D.Rows)
            {
                string Texto = row.Cells["Tipo"].Value.ToString();

                if (Texto == "Historia")
                {
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    row.DefaultCellStyle.ForeColor = Color.Blue;
                    row.DefaultCellStyle.Font = new Font(gridZH1.dataGridView1.Font, FontStyle.Bold);
                }
                else if (Texto == "Nota")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Green;
                    row.DefaultCellStyle.Font = new Font(gridZH1.dataGridView1.Font, FontStyle.Bold);
                } 
            }

            gridZH1.dataGridView1.ClearSelection();
        }
    }
}
