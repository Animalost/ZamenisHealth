using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZamenisHealth.Clases.Controles;
using ZamenisHealth.FrontFHIR.VisorZamenis;
using ZamenisHealth.Medicina.DocumentosWEB;
using ZamenisHealth.Medicina.OrdenesExtra;

namespace ZamenisHealth.Comunes
{
    public partial class BuscarPacientes : Forma2
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();

        private readonly Font fuenteNormal = new Font("Microsoft Sans Serif", 12, FontStyle.Regular);
        private readonly Font fuenteNegrita = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
        private FlowLayoutPanel flow;
        private ToolTip Mostrar = new ToolTip();
        public string Tipo_Busca_Pac;

        public BuscarPacientes()
        {
            InitializeComponent();            
        }

        public BuscarPacientes(string _tipo_Busca_Pac)
        {
            InitializeComponent();            
            this.Tipo_Busca_Pac = _tipo_Busca_Pac;
        }

        private void BuscarPacientes_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Buscar Paciente";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";         
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Buscar();
        }
        void CreaPanel(string Nombre, string TDocument, string NDocumento, Image image)
        {
            try
            {
                PanelCSS panel = new PanelCSS();
                panel.BackColor = Color.White;
                panel.BorderStyle = BorderStyle.None;
                panel.Size = new Size(582, 181);
                panel.Cursor = Cursors.Hand;

                Mostrar.SetToolTip(panel, "Click para seleccionar este paciente");
                Mostrar.ShowAlways = true;

                flow.Controls.Add(panel);

                //TITULOS
                Label tituloPaciente = new Label();
                tituloPaciente.Font = fuenteNegrita;
                tituloPaciente.ForeColor = Color.Blue;
                tituloPaciente.Text = "Paciente";
                tituloPaciente.Location = new Point(20, 15);
                tituloPaciente.AutoSize = true;
                tituloPaciente.BackColor = Color.Transparent;

                Label tituloTDocumento = new Label();
                tituloTDocumento.Font = fuenteNegrita;
                tituloTDocumento.ForeColor = Color.Blue;
                tituloTDocumento.Text = "Titulo Documento";
                tituloTDocumento.Location = new Point(20, 70);
                tituloTDocumento.AutoSize = true;
                tituloTDocumento.BackColor = Color.Transparent;

                Label tituloNDocumento = new Label();
                tituloNDocumento.Font = fuenteNegrita;
                tituloNDocumento.ForeColor = Color.Blue;
                tituloNDocumento.Text = "Numero Documento";
                tituloNDocumento.Location = new Point(20, 124);
                tituloNDocumento.AutoSize = true;
                tituloNDocumento.BackColor = Color.Transparent;

                panel.Controls.Add(tituloPaciente);
                panel.Controls.Add(tituloTDocumento);
                panel.Controls.Add(tituloNDocumento);

                PictureBox picture = new PictureBox();
                picture.Image = image;
                picture.SizeMode = PictureBoxSizeMode.StretchImage;
                picture.Size = new Size(75, 78);
                picture.Location = new Point(488, 85);
                picture.Cursor = Cursors.Hand;
                picture.BackColor = Color.Transparent;

                panel.Controls.Add(picture);

                //VALORES
                Label tituloPacienteValue = new Label();
                tituloPacienteValue.Font = fuenteNormal;
                tituloPacienteValue.ForeColor = Color.Black;
                tituloPacienteValue.Text = Nombre;
                tituloPacienteValue.Location = new Point(20, 38);
                tituloPacienteValue.AutoSize = true;
                tituloPacienteValue.BackColor = Color.Transparent;

                Label tituloTDocumentoValue = new Label();
                tituloTDocumentoValue.Font = fuenteNormal;
                tituloTDocumentoValue.ForeColor = Color.Black;
                tituloTDocumentoValue.Text = TDocument;
                tituloTDocumentoValue.Location = new Point(20, 93);
                tituloTDocumentoValue.AutoSize = true;
                tituloTDocumentoValue.BackColor = Color.Transparent;

                Label tituloNDocumentoValue = new Label();
                tituloNDocumentoValue.Font = fuenteNormal;
                tituloNDocumentoValue.ForeColor = Color.Black;
                tituloNDocumentoValue.Text = NDocumento;
                tituloNDocumentoValue.Location = new Point(20, 148);
                tituloNDocumentoValue.AutoSize = true;
                tituloNDocumentoValue.BackColor = Color.Transparent;

                panel.Controls.Add(tituloPacienteValue);
                panel.Controls.Add(tituloTDocumentoValue);
                panel.Controls.Add(tituloNDocumentoValue);

                panel.Click += (s, e) =>
                {
                    try
                    {
                        Seleccion(TDocument, NDocumento);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                #region Hovers
                void ActivarHover(object s, EventArgs e)
                {
                    panel.BackColor = Color.Thistle;
                }

                void DesactivarHover(object s, EventArgs e)
                {
                    // Solo restaurar cuando el cursor salga de todo el panel
                    if (!panel.ClientRectangle.Contains(
                        panel.PointToClient(Cursor.Position)))
                    {
                        panel.BackColor = Color.White;
                    }
                }

                Control[] controles =
                {
                    panel,
                    tituloPaciente,
                    tituloTDocumento,
                    tituloNDocumento,
                    picture,
                    tituloPacienteValue,
                    tituloTDocumentoValue,
                    tituloNDocumentoValue
                };

                foreach (Control control in controles)
                {
                    control.MouseEnter += ActivarHover;
                    control.MouseLeave += DesactivarHover;
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async Task Buscar()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    if (flow != null)
                        flow.Dispose();

                    MensajesGeneral MG = new MensajesGeneral
                    {
                        Mensaje = "Debe diligenciar al menos el campo de apellido",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    List<CXN_PACIENTES> _DatosPacientes = repositorioPacientes.LlamarPacienteDOCSimilares(textBox1.Text, textBox2.Text);
                    if (_DatosPacientes != null)
                    {
                        if (flow != null)
                            flow.Dispose();

                        flow = new FlowLayoutPanel();

                        flow.AutoScroll = true;
                        flow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        flow.BackColor = Color.White;
                        flow.BorderStyle = BorderStyle.None;
                        flow.FlowDirection = FlowDirection.TopDown;
                        flow.Size = new Size(614, 436);
                        flow.Location = new Point(12, 113);
                        flow.WrapContents = false;

                        this.Controls.Add(flow);

                        flow.SuspendLayout();

                        flow.Visible = false;
                        int contador = 0;

                        try
                        {
                            foreach (var i in _DatosPacientes)
                            {
                                CreaPanel(i.Pac_PrimerN.ToString(),
                                     i.Pac_TipoId.ToString(),
                                     i.Pac_IdNum.ToString().Trim(),
                                     i.Pac_Sexo == "M" ? Properties.Resources.man :
                                     i.Pac_Sexo == "F" ? Properties.Resources.woman :
                                     Properties.Resources.paciente);

                                if (contador % 10 == 0)
                                {
                                    flow.ResumeLayout(true);
                                    await Task.Delay(1);
                                    flow.SuspendLayout();
                                }
                            }
                        }
                        finally
                        {
                            flow.ResumeLayout(true);
                            flow.Visible = true;
                        }
                    }
                    else
                    {
                        if (flow != null)
                            flow.Dispose();

                        MensajesGeneral MG = new MensajesGeneral
                        {
                            Mensaje = "No se encontraron coincidencias con los criterios de busqueda ingresados",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                }                
            }
            catch (Exception ex)
            {
                if (flow != null)
                    flow.Dispose();
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private async void BuscarPacientes_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F5)
                {
                    await Buscar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.CharacterCasing = CharacterCasing.Upper;
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.CharacterCasing = CharacterCasing.Upper;
        }        
        void Seleccion(string TipoId, string Identidad)
        {
            try
            {
                switch (Tipo_Busca_Pac)
                {
                    case "Pacientes":
                        Recepcion.CrearEditarPaciente f1 = Application.OpenForms.OfType<Recepcion.CrearEditarPaciente>().LastOrDefault();
                        f1.comboBox1.Text = TipoId;
                        f1.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "BusquedaGeneral":
                        ConfigContenedor.DatosPac f2 = Application.OpenForms.OfType<ConfigContenedor.DatosPac>().LastOrDefault();
                        f2.comboBox1.Text = TipoId;
                        f2.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "AgendarCita":
                        Recepcion.AgendarCita f4 = Application.OpenForms.OfType<Recepcion.AgendarCita>().LastOrDefault();
                        f4.comboBox1.Text = TipoId;
                        f4.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Asistencia":
                        Recepcion.Asistencia f5 = Application.OpenForms.OfType<Recepcion.Asistencia>().LastOrDefault();
                        f5.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "ExpAgenda":
                        Recepcion.Extras.PrintAgendas f6 = Application.OpenForms.OfType<Recepcion.Extras.PrintAgendas>().LastOrDefault();
                        f6.comboBox3.Text = TipoId;
                        f6.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "BusVen":
                        Recepcion.Ventas.BuscarPaciente f7 = Application.OpenForms.OfType<Recepcion.Ventas.BuscarPaciente>().LastOrDefault();
                        f7.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Cotizaciones":
                        Recepcion.Particulares f14 = Application.OpenForms.OfType<Recepcion.Particulares>().LastOrDefault();
                        f14.comboBox1.Text = TipoId;
                        f14.textBox61.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Cotiza_Hist":
                        Recepcion.Extras.ParticularesPrevios f15 = Application.OpenForms.OfType<Recepcion.Extras.ParticularesPrevios>().LastOrDefault();
                        f15.comboBox1.Text = TipoId;
                        f15.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "CMANE":
                        Medicina.CambioManejoEnfermero f16 = Application.OpenForms.OfType<Medicina.CambioManejoEnfermero>().LastOrDefault();
                        f16.comboBox1.Text = TipoId;
                        f16.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "NotaAcla":
                        HistoriasClinicas.NotasAclaratorias f17 = Application.OpenForms.OfType<HistoriasClinicas.NotasAclaratorias>().LastOrDefault();
                        f17.comboBox1.Text = TipoId;
                        f17.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "VerImagen":
                        Medicina.VerImagenes f18 = Application.OpenForms.OfType<Medicina.VerImagenes>().LastOrDefault();
                        f18.comboBox1.Text = TipoId;
                        f18.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "OrdenesS":
                        Medicina.OrdenesMedicas f20 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().LastOrDefault();
                        f20.comboBox1.Text = TipoId;
                        f20.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Facturacion":
                        Facturacion.Facturar f22 = Application.OpenForms.OfType<Facturacion.Facturar>().LastOrDefault();
                        f22.comboBox1.Text = TipoId;
                        f22.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Formatos":
                        AdminSystem.Formatos f23 = Application.OpenForms.OfType<AdminSystem.Formatos>().LastOrDefault();
                        f23.comboBox4.Text = TipoId;
                        f23.textBox9.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Fac_Per":
                        Gerencia f24 = Application.OpenForms.OfType<Gerencia>().LastOrDefault();
                        f24.comboBox1.Text = TipoId;
                        f24.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "FirmaDocs":
                        Medicina.FirmaHistorias f26 = Application.OpenForms.OfType<Medicina.FirmaHistorias>().LastOrDefault();
                        f26.comboBox2.Text = TipoId;
                        f26.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "CompleteJM":
                        HistoriasClinicas.Historia_JM_Completar f27 = Application.OpenForms.OfType<HistoriasClinicas.Historia_JM_Completar>().LastOrDefault();
                        f27.setDatos(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "Encuesta":
                        Fibromialgia.Encuestas.Menu f28 = Application.OpenForms.OfType<Fibromialgia.Encuestas.Menu>().LastOrDefault();
                        f28.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "InfFibroGen":
                        Fibromialgia.Informes.InformesPrograma f29 = Application.OpenForms.OfType<Fibromialgia.Informes.InformesPrograma>().LastOrDefault();
                        f29.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "CANCELAWEB":
                        Recepcion.Extras.CancelacionesWEB f30 = Application.OpenForms.OfType<Recepcion.Extras.CancelacionesWEB>().LastOrDefault();
                        f30.setDatos(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "HistorialMedico":
                        Medicina.Historial_Medico_1 f32 = Application.OpenForms.OfType<Medicina.Historial_Medico_1>().LastOrDefault();
                        f32.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "Agenda2":
                        Recepcion.Extras.Busqueda f33 = Application.OpenForms.OfType<Recepcion.Extras.Busqueda>().LastOrDefault();
                        f33.setDoc(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "INGSAL":
                        Medicina.EstInformesMG f34 = Application.OpenForms.OfType<Medicina.EstInformesMG>().LastOrDefault();
                        f34.setSelection(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "UploadPDF":
                        Medicina.UploadHistory f35 = Application.OpenForms.OfType<Medicina.UploadHistory>().LastOrDefault();
                        f35.setSelection(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "FacturacionAbierta":
                        Facturacion.FacturaAbierta f36 = Application.OpenForms.OfType<Facturacion.FacturaAbierta>().LastOrDefault();
                        f36.setSelection(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "ConsentimientosWEB":
                        MenuDocWeb f37 = Application.OpenForms.OfType<MenuDocWeb>().LastOrDefault();
                        f37.setDoc(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "FHIR":
                        VerRDA f38 = Application.OpenForms.OfType<VerRDA>().LastOrDefault();
                        f38.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "OrdenesFHIRExtra":
                        CrearOrdenExtra f39 = Application.OpenForms.OfType<CrearOrdenExtra>().LastOrDefault();
                        f39.setDoc(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "ConInf":
                        HistoriasClinicas.Historia_MedicinaGeneral_2_2 f40 = Application.OpenForms.OfType<HistoriasClinicas.Historia_MedicinaGeneral_2_2>().LastOrDefault();
                        f40.setDoc(Identidad);
                        this.Dispose();
                        this.Close();
                        break;


                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
