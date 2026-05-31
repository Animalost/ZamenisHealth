using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class CAsistencia : Forma
    {
        private static readonly IAgenda repoAge = new MAgenda();

        private int Admision;
        List<CXN_CIA> _datosCertificado;

        public ApartmentState ApartmentState { get; internal set; }

        public CAsistencia(int admision)
        {
            InitializeComponent();
            this.Admision = admision;
        }

        private void CAsistencia_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Certificado Asistencia";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Generar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button1_Click;

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                List<CXN_CIA> _datosCertificado = repoAge.certificadoAsistencia(Admision, "");                

                if (_datosCertificado != null)
                {
                    foreach (var i in _datosCertificado)
                    {
                        label4.Text = i.Com_Cod_Prestador.ToString();
                        break;
                    }                    
                }
                else
                {
                    MG.Mensaje = "No se logro cargar el dato de la admision, puede que no se halla cerrado la historia clinica o nota de enfermeria";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == 0) 
                {
                    textBox1.Text = "";
                    StreamReader leido = File.OpenText("C:\\Cxn\\Certificado.txt");
                    string contenido = null;
                    contenido = leido.ReadToEnd();
                    textBox1.Text = contenido.ToString();
                    leido.Close();

                    textBox1.ReadOnly = true;
                    label3.Text = "Este certificado defecto no lo puede editar aqui, para editarlo modifiquelo en la ruta C:\\CXN\\Certificado.txt.  Si desea otro tipo de certificado " +
                        "seleccione la opcion Certificado Personalizado";
                }

                if (comboBox1.SelectedIndex == 1)
                {
                    textBox1.Text = "";

                    textBox1.ReadOnly = false;
                    label3.Text = "Esta opcion le permite editar el certificado en este lugar, pero no puede grabar esta plantilla por lo tanto puede dejarlo guardado en sus " +
                        "documentos personales";
                }


                if (comboBox1.SelectedIndex == 2)
                {
                    textBox1.Text = "";

                    textBox1.ReadOnly = true;
                    label3.Text = "Esta opcion le permite generar un certificado de asistencia con todo el historico de asistencia del paciente desde la primera vez hasta este dia";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                string tex = textBox1.Text;

                if (comboBox1.SelectedIndex == 0) { tex = ""; }
                if (comboBox1.SelectedIndex == 2) { tex = "Historic"; }

                _datosCertificado = new List<CXN_CIA>();

                
                    _datosCertificado = repoAge.certificadoAsistencia(Admision, tex);
                
                
                if (_datosCertificado != null)
                {
                    if (tex == "Historic")
                    {
                        Thread thread2 = new Thread(M2);
                        thread2.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread2.Start();
                        return;
                    }

                    Thread thread = new Thread(M);
                    thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                    thread.Start();                   
                }
                else
                {
                    MG.Mensaje = "No se puede generar el certificado de asistencia porque el paciente no asistio este dia";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void M()
        {
            ConfigForm.GenerarReportViewer("DataSet_Compañia",
                                             "ZamenisHealth.Reportes.RDLC_CertAsistencia.rdlc",
                                             _datosCertificado);

        }

        void M2()
        {
            ConfigForm.GenerarReportViewer("DataSet_Compañia",
                                             "ZamenisHealth.Reportes.RDLC_CertAsistenciaHistorico.rdlc",
                                             _datosCertificado);

        }
    }
}
