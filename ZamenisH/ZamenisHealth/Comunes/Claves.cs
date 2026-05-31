using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes
{
    public partial class Claves : Forma2
    {

        private static readonly ILogin repositorioLogin = new MLogin();
        private static readonly IGenerales repositorioGenerales = new MGenerales();

        private string ClaveActual, UsuarioActual;
        private string _questionToAI;
        private MensajesGeneral MG;

        public Claves(string claveActual, string usuarioActual)
        {
            InitializeComponent();

            this.ClaveActual = claveActual;
            this.UsuarioActual = usuarioActual;            
        }

        public Claves(string usuarioActual)
        {
            InitializeComponent();
            this.UsuarioActual = usuarioActual;
            textBox1.Text = "NNNNN";
            textBox1.Enabled = false;
            textBox3.Enabled = true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        void CambiarPorPatron()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligencias una clave nueva y su confirmacion";
                    MG.ShowDialog();
                }
                else if (textBox2.Text != textBox3.Text)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La clave nueva es diferente a la confirmacion";
                    MG.ShowDialog();
                }
                else
                {
                    var plainTextBytes = repositorioGenerales.GetMD5(textBox2.Text);

                    bool changePass = repositorioLogin.changeClave(UsuarioActual, plainTextBytes);

                    if (changePass == true)
                    {
                        Comunes.MensajesGeneral mensajesGeneral = new MensajesGeneral
                        {
                            Mensaje = "Clave actualizada correctamente, vuelva a ingresar al sistema con su nueva clave",
                            TipoImagen = 1
                        };

                        mensajesGeneral.ShowDialog();

                        Application.Exit();
                    }
                    else
                    {
                        Comunes.MensajesGeneral mensajesGeneral = new MensajesGeneral
                        {
                            Mensaje = "Hubo un error interno y no se logro cambiar la clave, intente mas tarde",
                            TipoImagen = 1000
                        };
                        mensajesGeneral.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Enabled == false)
                {
                    CambiarPorPatron();
                }
                else
                {
                    if (textBox1.Text != ClaveActual)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "La clave actual no coincide";
                        MG.ShowDialog();
                        return;
                    }
                    if (textBox2.Text != textBox3.Text)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "La clave nueva es diferente a la confirmacion";
                        MG.ShowDialog();
                        return;
                    }
                    if (textBox1.Text == "")
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Los campos no pueden estar vacios";
                        MG.ShowDialog();
                        return;
                    }
                    if (textBox2.Text == "")
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Los campos no pueden estar vacios";
                        MG.ShowDialog();
                        return;
                    }
                    if (textBox3.Text == "")
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Los campos no pueden estar vacios";
                        MG.ShowDialog();
                        return;
                    }
                    if (textBox3.Text == "123")
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "La contraseña debe cumplir los requisitos descritos";
                        MG.ShowDialog();
                        return;
                    }

                    var plainTextBytes = repositorioGenerales.GetMD5(textBox2.Text);

                    bool changePass =  repositorioLogin.changeClave(UsuarioActual, plainTextBytes);

                    if (changePass == true)
                    {
                        Comunes.MensajesGeneral mensajesGeneral = new MensajesGeneral
                        {
                            Mensaje = "Clave actualizada correctamente, si olvida su clave contacte al administrador del sistema",
                            TipoImagen = 1
                        };

                        mensajesGeneral.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        Comunes.MensajesGeneral mensajesGeneral = new MensajesGeneral
                        {
                            Mensaje = "Hubo un error interno y no se logro cambiar la clave, intente mas tarde",
                            TipoImagen = 1000
                        };
                        mensajesGeneral.ShowDialog();
                    }
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Claves_Load(object sender, EventArgs e)
        {
            ImageClose.Visible = false;            
            ImageMinimize.Visible = false;

            
            Titulo.Text = "Cambiar Clave";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            VerificarClave(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != textBox3.Text)
            {
                label5.ForeColor = Color.Red;
                textBox3.Enabled = true;
                toolStripButton2.Enabled = false;
            }
            else
            {
                label5.ForeColor = Color.Green;
                textBox3.Enabled = true;
                toolStripButton2.Enabled = true;
            }
        }

        void VerificarClave(string Clave)
        {
            try
            {             
                if (Clave.Length < 8)
                {
                    label16.ForeColor = Color.Red;
                }
                else
                {
                    label16.ForeColor = Color.Green;
                }

                if (Clave.Contains("*") || Clave.Contains("/") || Clave.Contains(".") || Clave.Contains("-") || Clave.Contains("+"))
                {
                    label13.ForeColor = Color.Green;
                }
                else
                {
                    label13.ForeColor = Color.Red;
                }

                if (Clave.Contains("0") || Clave.Contains("1") || Clave.Contains("2") || Clave.Contains("3") || Clave.Contains("4") ||
                       Clave.Contains("5") || Clave.Contains("6") || Clave.Contains("7") || Clave.Contains("8") || Clave.Contains("9"))
                {
                    label14.ForeColor = Color.Green;
                }
                else
                {
                    label14.ForeColor = Color.Red;
                }

                bool tieneMinusculas = Clave.Any(c => char.IsLower(c));
                if (tieneMinusculas != true)
                {
                    label15.ForeColor = Color.Red;
                }
                else
                {
                    label15.ForeColor = Color.Green;
                }

                bool tieneMayusculas = Clave.Any(c => char.IsUpper(c));
                if (tieneMayusculas != true)
                {
                    label18.ForeColor = Color.Red;
                }
                else
                {
                    label18.ForeColor = Color.Green;
                }

                if (label16.ForeColor == Color.Green && label13.ForeColor == Color.Green &&
                    label14.ForeColor == Color.Green && label15.ForeColor == Color.Green && label18.ForeColor == Color.Green)
                {
                    textBox3.Enabled = true;
                }
                else
                {
                    textBox3.Enabled = false;
                }
            }
            catch
            {
                MessageBox.Show("El validador de contraseñas presento un error, se cerrara este formulario automaticamente, " +
                    "si lo prefiere vuelva a ingresar",
                    "Error inesperado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Dispose();
                this.Close();
            }
        }     
    }
}
