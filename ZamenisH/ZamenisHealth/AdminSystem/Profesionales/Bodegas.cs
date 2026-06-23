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
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Bodegas : Forma
    {
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly ILogin repoLogin = new MLogin();

        private MensajesGeneral MG;
        private ToolStripButton btnGrabar;
        private string UserSelected;
        private string FirmaB64;
        private bool Nuevo;

        public Bodegas(string userSelected)
        {
            InitializeComponent();
            UserSelected = userSelected;
        }

        private void Bodegas_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Bodegas";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += toolStripButton2_Click;

                var dataUserBod = repoBodegas.getDatosUser(UserSelected);
                if (dataUserBod != null) 
                { 
                    label8.Text = dataUserBod.Bod_Responsable.ToString();
                    textBox1.Text = dataUserBod.Bod_Numero.ToString(); textBox1.Enabled = false;
                    comboBox3.Text = dataUserBod.Bod_Estado == "A" ? "SI" : "NO";
                    textBox3.Text = dataUserBod.Bod_Reg_Med;
                    comboBox2.Text = dataUserBod.Bod_Tipo == "CU" ? "Enfermeria Curaciones" :
                                     dataUserBod.Bod_Tipo == "MG" ? "Medicina General" :
                                     dataUserBod.Bod_Tipo == "FI" ? "Fisiatria" :
                                     dataUserBod.Bod_Tipo == "TF" ? "Terapia Fisica" :
                                     dataUserBod.Bod_Tipo == "TO" ? "Terapia Ocupacional" :
                                     dataUserBod.Bod_Tipo == "PS" ? "Psicologia" :
                                     ""; comboBox2.Enabled = false;
                    FirmaB64 = dataUserBod.Bod_Firma;
                    pictureBox1.Image = Base64ToImage(FirmaB64);

                    Nuevo = false;
                }
                else
                {
                    var dataUserLog = repoLogin.getUser(UserSelected);
                    if (dataUserLog != null)
                    {
                        label8.Text = dataUserLog.Log_PrimerA + " " + dataUserLog.Log_SegundoA + " " +
                                      dataUserLog.Log_PrimerN + " " + dataUserLog.Log_SegundoN;
                        textBox1.Text = "";
                        comboBox3.Text = "SI";
                        textBox3.Text = dataUserLog.Log_Identificacion;
                        comboBox3.Text = "";
                        FirmaB64 = "";

                        Nuevo = true;
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Error general cargando usuario",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();

                        this.Close();
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
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox1.Text) || comboBox3.Text == "" ||
                    comboBox2.Text == "" || string.IsNullOrEmpty(FirmaB64))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar todos los campos",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    CXN_BODEGAS B = new CXN_BODEGAS
                    {
                        Bod_Numero = Convert.ToInt32(textBox1.Text),
                        Bod_Usuario = UserSelected,
                        Bod_Responsable = label8.Text,
                        Bod_Reg_Med = textBox3.Text,
                        Bod_Tipo = comboBox2.Text == "Enfermeria Curaciones" ? "CU" :
                                   comboBox2.Text == "Medicina General" ? "MG" :
                                   comboBox2.Text == "Fisiatria" ? "FI" :
                                   comboBox2.Text == "Psicologia" ? "PS" :
                                   comboBox2.Text == "Terapia Fisica" ? "TF" : "TO",
                        Bod_Firma = FirmaB64,
                        Bod_Estado = comboBox3.Text == "SI" ? "A" : "N"                       
                    };

                    if (Nuevo == true)
                    {
                        //insert
                        CXN_BODEGAS Bod = repoBodegas.getDatosCode(Convert.ToInt32(textBox1.Text));
                        if (Bod != null)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "El numero de bodega seleccionado ya esta en uso con otro profesional, elija otro",
                                TipoImagen = 0
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            bool save = repoBodegas.createUser(B);
                            if (save == true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Bodega creada",
                                    TipoImagen = 3
                                };
                                MG.ShowDialog();

                                this.Close();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "No se logro crear la bodega",
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();
                            }
                        }                    
                    }
                    else
                    {
                        //update
                        bool update = repoBodegas.updateUser(B);
                        if (update == true)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Bodega actualizada",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();

                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro actualizar la bodega",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
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
                    FirmaB64 = Convert.ToBase64String(bytes);
                    pictureBox1.Image = Base64ToImage(FirmaB64);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
