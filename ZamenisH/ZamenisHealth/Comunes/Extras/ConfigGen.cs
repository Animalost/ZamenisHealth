using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class ConfigGen : Forma
    {
        private static readonly IConfSystem repoConf = new MConfSystem();

        public ConfigGen()
        {
            InitializeComponent();            
        }
        private void ConfigGen_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Adherencia";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button1_Click;

                ToolStripButton btnFolder = new ToolStripButton();
                btnFolder = createToolButton("Open INI");
                MenuLateral.Items.Add(btnFolder);
                btnFolder.Click += button3_Click;

                MensajesGeneral MG;

                Dictionary<string, string> getConfig = repoConf.getListado();      

                if (getConfig != null) 
                {
                    comboBox0.Text = (getConfig["Noticia"] == "A" ? comboBox0.Text = "Habilitar" : "Deshabilitar");
                    comboBox4.Text = (getConfig["EncuestaSatisCU"] == "A" ? comboBox4.Text = "SI" : "NO");
                    comboBox1.Text = (getConfig["AutocompletarNotas"] == "A" ? comboBox1.Text = "SI" : "NO");
                    comboBox2.Text = (getConfig["UpperCaseNotas"] == "A" ? comboBox2.Text = "SI" : "NO");
                    comboBox3.Text = (getConfig["EstadisticaEnfermeria"] == "A" ? comboBox3.Text = "SI" : "NO");                                        
                    comboBox6.Text = (getConfig["Recomendaciones"] == "A" ? comboBox6.Text = "SI" : "NO");
                    comboBox7.Text = (getConfig["AutocompletarFirmas"] == "A" ? comboBox7.Text = "SI" : "NO");
                    comboBox10.Text = (getConfig["MedicinaGeneralEstadistica"] == "A" ? comboBox10.Text = "SI" : "NO");
                    comboBox13.Text = (getConfig["EncuestaQRWeb"] == "A" ? comboBox13.Text = "SI" : "NO");
                    comboBox14.Text = (getConfig["ReportarImpuestosDIAN"] == "A" ? comboBox14.Text = "SI" : "NO");
                    comboBox17.Text = (getConfig["IHCE"] == "A" ? comboBox17.Text = "SI" : "NO");

                    comboBox5.Text = (Preferencias.Colorimetria == "A" ? comboBox5.Text = "SI" : "NO");
                    comboBox8.Text = (Preferencias.TicketCitas == "A" ? comboBox8.Text = "SI" : "NO");
                    comboBox12.Text = (Preferencias.AppSound == "A" ? comboBox12.Text = "SI" : "NO");
                    comboBox9.Text = (Preferencias.TCPIP == "A" ? comboBox9.Text = "SI" : "NO");
                    textBox1.Text = Preferencias.Bloqueo.ToString();
                    comboBox15.Text = (Preferencias.CuracionesCORE == "A" ? comboBox15.Text = "SI" : "NO");
                    comboBox16.Text = (Preferencias.TabletaFirmas == "A" ? comboBox16.Text = "SI" : "NO");

                    if (Conexion.ConectionDictionary["Videoconferencia"] == "A")
                    {
                        label13.Visible = true;
                        comboBox11.Visible = true;
                        comboBox11.Text = (getConfig["Teleconsulta"] == "A" ? comboBox11.Text = "SI" : "NO");
                    }
                    else
                    {
                        label13.Visible = false;
                        comboBox11.Text = "NO";
                        comboBox11.Visible = false;
                    }                    
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error en configuracion de base de datos, ERROR FATAL";
                    MG.ShowDialog();

                    this.Dispose();this.Close();
                }

                ToolTip Titulo1 = new ToolTip(); Titulo1.ShowAlways = true;                
                ToolTip Titulo2 = new ToolTip(); Titulo2.ShowAlways = true; 
                ToolTip Titulo3 = new ToolTip(); Titulo3.ShowAlways = true; 
                ToolTip Titulo4 = new ToolTip(); Titulo4.ShowAlways = true; 
                ToolTip Titulo5 = new ToolTip(); Titulo5.ShowAlways = true; 
                ToolTip Titulo6 = new ToolTip(); Titulo6.ShowAlways = true; 
                ToolTip Titulo7 = new ToolTip(); Titulo7.ShowAlways = true; 
                ToolTip Titulo8 = new ToolTip(); Titulo8.ShowAlways = true; 
                ToolTip Titulo9 = new ToolTip(); Titulo9.ShowAlways = true;
                ToolTip Titulo10 = new ToolTip(); Titulo10.ShowAlways = true;
                ToolTip Titulo11 = new ToolTip(); Titulo11.ShowAlways = true;
                ToolTip Titulo12 = new ToolTip(); Titulo12.ShowAlways = true;
                ToolTip Titulo13 = new ToolTip(); Titulo13.ShowAlways = true;

                Titulo1.SetToolTip(comboBox0, "Muestra la pantalla inicial de noticia del dia al abrir la aplicacion");
                Titulo2.SetToolTip(comboBox1, "Al agendar cita de curaciones de inicio de paquete, crea una nueva HC MG a partir de la ultima creada");
                Titulo3.SetToolTip(comboBox2, "Habilita el texto en mayusculas en la nota de enfermeria");
                Titulo4.SetToolTip(comboBox3, "Muestra una encuesta al final de la nota de enfermeria para llevar estadisticas de control");
                Titulo5.SetToolTip(comboBox8, "Permite imprimir un tikete al agendar una nueva cita con dos codigos de barras");
                Titulo6.SetToolTip(comboBox7, "Genera una hoja de firmas de curaciones acorde a la cantidad de sesiones de la autorizacion de curaciones");
                Titulo7.SetToolTip(comboBox4, "Muestra al final de la nota de enfermeria una encuesta de satisfaccion de atencion al usuario de todo el personal asistencial y administrativo");
                Titulo8.SetToolTip(comboBox9, "Permite enviar una señal desde el consultorio de medicina general hacia la recepcion informando que el paciente ya termino atencion medica");
                Titulo9.SetToolTip(textBox1, "Al colocar 0 se inhabilita esta opcion.  Permite bloquear dias de la agenda para no poder agendar citas.");
                Titulo10.SetToolTip(comboBox5, "Muestra un color en la agenda al momento de admisionar un paciente de acuerdo al tipo de atencion medica que requiere");
                Titulo11.SetToolTip(comboBox6, "Muestra al momento de agendar un paciente un recuadro que permite seleccionar las condiciones de entrada del paciente");
                Titulo12.SetToolTip(comboBox10, "Permite seleccionar desde la historia clinica de medicina general el tipo de atencion recomendado de curacion hacia la recepcion y llevar el control de medicina general para el informe medico");
  
                if (comboBox11.Visible == true)
                {
                    Titulo13.SetToolTip(comboBox11, "Si la licencia tiene permitida la habilitacion del modulo de TeleConsulta, esta opcion funcionara de lo contrario debera hacerse a la licencia de Teleconsultas");
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
                MensajesGeneral MG;
                
                CXN_IMAGEN_SYSTEM I = new CXN_IMAGEN_SYSTEM
                {
                    Tab_Nombre = "Noticia",
                    Tab_Clave = (comboBox0.Text == "Habilitar" ? "A" : "N")
                };

                bool saveConf = repoConf.updateImagenSystem(I);                
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar noticias";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "EncuestaSatisCU";
                I.Tab_Clave = (comboBox4.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar Encuestas de Satisfaccion";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "AutocompletarNotas";
                I.Tab_Clave = (comboBox1.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar el autocompletado de notas";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "UpperCaseNotas";
                I.Tab_Clave = (comboBox2.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar el Upper Case";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "EstadisticaEnfermeria";
                I.Tab_Clave = (comboBox3.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar las estadisticas en notas de curacion";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "Recomendaciones";
                I.Tab_Clave = (comboBox6.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar la recomendacion de citas";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "AutocompletarFirmas";
                I.Tab_Clave = (comboBox7.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar el autocompletado de firmas para curaciones";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "MedicinaGeneralEstadistica";
                I.Tab_Clave = (comboBox10.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar las recomendaciones de Medicina General";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "ReportarImpuestosDIAN";
                I.Tab_Clave = (comboBox14.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar la facturacion con impuestos DIAN";
                    MG.ShowDialog();
                }

                if (comboBox11.Visible == true)
                {
                    I.Tab_Nombre = "Teleconsulta";
                    I.Tab_Clave = (comboBox11.Text == "SI" ? "A" : "N");

                    saveConf = repoConf.updateImagenSystem(I);
                    if (saveConf == false)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro actualizar el modulo de Teleconsulta";
                        MG.ShowDialog();
                    }
                }

                I.Tab_Nombre = "EncuestaQRWeb";
                I.Tab_Clave = (comboBox13.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar la Encuesta QR Web";
                    MG.ShowDialog();
                }

                I.Tab_Nombre = "IHCE";
                I.Tab_Clave = (comboBox17.Text == "SI" ? "A" : "N");

                saveConf = repoConf.updateImagenSystem(I);
                if (saveConf == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar el reporte IHCE";
                    MG.ShowDialog();
                }

                Preferencias.SetPreferences("TCPIP", (comboBox9.Text == "SI" ? "A" : "N"));
                Preferencias.SetPreferences("Colorimetria", (comboBox5.Text == "SI" ? "A" : "N"));
                Preferencias.SetPreferences("Sonido", (comboBox12.Text == "SI" ? "A" : "N"));
                Preferencias.SetPreferences("TicketCitas", (comboBox8.Text == "SI" ? "A" : "N"));
                Preferencias.SetPreferences("Bloqueo", (textBox1.Text == "" ? "0" : textBox1.Text));
                Preferencias.SetPreferences("CuracionesCORE", (comboBox15.Text == "SI" ? "A" : "N"));
                Preferencias.SetPreferences("TabletaFirmas", (comboBox16.Text == "SI" ? "A" : "N"));

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ConfigForm.OpenIniFile();
        }
    }
}
