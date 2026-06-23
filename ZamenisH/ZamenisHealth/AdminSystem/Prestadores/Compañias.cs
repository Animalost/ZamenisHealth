using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Compañias : Forma
    {
        private static readonly ICompañia repoCIA = new MCompañia();

        private MensajesGeneral MG;
        private int CodeCia;
        private string LogoB64;
        private bool Nuevo;

        public Compañias(int codeCia)
        {
            InitializeComponent();
            CodeCia = codeCia;
        }

        private void Compañias_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Prestadores";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";            

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button1_Click;

            ConfigForm.SoloNumeros(textBox13);
            ConfigForm.SoloNumeros(textBox15);
            ConfigForm.SoloNumeros(textBox17);
            ConfigForm.SoloNumeros(textBox19);
            ConfigForm.SoloNumeros(textBox20);
            ConfigForm.SoloNumeros(textBox18);

            if (CodeCia > 0)
            {
                CXN_CIA dataCia = repoCIA.getPrestadorbyCode(CodeCia);
                if (dataCia != null)
                {
                    Nuevo = false;
                    textBox1.Text = dataCia.Com_Identificador.ToString(); textBox1.Enabled = false;
                    textBox3.Text = dataCia.Com_Identificacion.ToString();
                    textBox18.Text = dataCia.Com_DVerifica.ToString();
                    textBox2.Text = dataCia.Com_Nombre.ToString();
                    comboBox1.Text = dataCia.Com_Tipo_Doc.ToString();
                    textBox4.Text = dataCia.Com_Direccion.ToString();
                    textBox5.Text = dataCia.Com_Telefono.ToString();
                    textBox9.Text = dataCia.Com_Email.ToString();
                    textBox7.Text = dataCia.Com_Cod_Prestador.ToString();
                    textBox8.Text = dataCia.Com_Cod_Prestador_2.ToString();
                    textBox11.Text = dataCia.Com_Nombre_SMS.ToString();
                    textBox12.Text = dataCia.Com_Telefono_SMS.ToString();
                    textBox13.Text = dataCia.Com_OP.ToString();
                    textBox15.Text = dataCia.Com_Cotiza.ToString();
                    textBox17.Text = dataCia.Com_OM.ToString();
                    textBox19.Text = dataCia.Com_RIP.ToString();
                    textBox20.Text = dataCia.Com_Cierres.ToString();
                    LogoB64 = dataCia.Com_Logo.ToString();

                    pictureBox1.Image = Base64ToImage(LogoB64);
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro cargar la compañia, intente mas tarde",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();

                    this.Close();
                }
            }
            else
            {
                Nuevo = true;
            }
        }
        Image Base64ToImage(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                throw new ArgumentException("La cadena Base64 está vacía o es nula.");

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (FormatException)
            {
                throw new FormatException("La cadena no es un Base64 válido.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al convertir Base64 a imagen: " + ex.Message);
            }
        }             
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox18.Text)
                    || string.IsNullOrEmpty(textBox2.Text) || comboBox1.Text == "" || string.IsNullOrEmpty(textBox4.Text)
                    || string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox9.Text) || string.IsNullOrEmpty(textBox7.Text)
                    || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox11.Text) || string.IsNullOrEmpty(textBox12.Text)
                    || string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox15.Text) || string.IsNullOrEmpty(textBox17.Text)
                    || string.IsNullOrEmpty(textBox19.Text) || string.IsNullOrEmpty(textBox20.Text) || string.IsNullOrEmpty(LogoB64)) 
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar todos los campos y una imagen",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return;
                }

                CXN_CIA C = new CXN_CIA()
                {
                    Com_Identificador = Convert.ToInt32(textBox1.Text),
                    Com_Identificacion = textBox3.Text,
                    Com_DVerifica = textBox18.Text,
                    Com_Nombre = textBox2.Text,
                    Com_Tipo_Doc = comboBox1.Text,
                    Com_Direccion = textBox4.Text,
                    Com_Telefono = textBox5.Text,
                    Com_Email = textBox9.Text,
                    Com_Cod_Prestador = textBox7.Text,
                    Com_Cod_Prestador_2 = textBox8.Text,
                    Com_Nombre_SMS = textBox11.Text,
                    Com_Telefono_SMS = textBox12.Text,
                    Com_OP = Convert.ToInt32(textBox13.Text),
                    Com_Cotiza = Convert.ToInt32(textBox15.Text),
                    Com_OM = Convert.ToInt32(textBox17.Text),
                    Com_RIP = Convert.ToInt32(textBox19.Text),
                    Com_Cierres = Convert.ToInt32(textBox20.Text),
                    Com_Logo = LogoB64,

                    Com_ConsContable = 0,
                    Com_DE = 0,
                    Com_Doc_Electron = 0,
                    Com_Doc_Electron_NC = 0,
                    Com_Doc_Soporte = 0,
                    Com_Doc_Soporte_NC = 0,
                    Com_Fac = 0,
                    Com_Fecha_Electron = DateTime.Now,
                    Com_Fecha_Soporte = DateTime.Now,
                    Com_Numeracion_Electron = "XXX",
                    Com_Numeracion_Soporte = "XXX",
                    Com_PedPro = 1,
                    Com_Prefijo_Electron = "XXX",
                    Com_Prefijo_Electron_NC = "XXX",
                    Com_Prefijo_Soporte = "XXX",
                    Com_Prefijo_Soporte_NC = "XXX",
                    Com_Resolucion = "XXX",
                    Com_Resolucion_Electron = "XXX",
                    Com_Resolucion_Soporte = "XXX",
                    Com_SMS = 0,
                    Com_UsuarioGraba = Contenedor.UsuarioLogueado
                };

                if (Nuevo == true)
                {
                    var Datos = repoCIA.getPrestadorbyCode(Convert.ToInt32(textBox1.Text));
                    if (Datos != null)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Este codigo ya existe con otro prestador, elija uno diferente",
                            TipoImagen = 0
                        };
                        MG.ShowDialog();
                    }
                    else
                    {
                        bool save = repoCIA.createCompañia(C);
                        if (save == true)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Compañia creada exitosamente",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();

                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Error.  No se logro crear la compañia",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                    }                    
                }
                else
                {
                    bool update = repoCIA.updateCompañia(C);
                    if (update == true)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Compañia actualiazda exitosamente",
                            TipoImagen = 3
                        };
                        MG.ShowDialog();

                        this.Close();
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Error.  No se logro actualizar la compañia",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
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
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string dir = openFileDialog1.FileName;

                    Byte[] bytes = File.ReadAllBytes(dir);
                    LogoB64 = Convert.ToBase64String(bytes);

                    pictureBox1.Image = Base64ToImage(LogoB64);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
