using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes
{
    public partial class Login2 : Forma2
    {
        private Login oLogin;
        private ILogin oLoginController;
        CXN_LOGIN Log;
        private MensajesGeneral MG;

        public Login2(Login _login)
        {
            InitializeComponent();
            oLogin = _login;
            oLoginController = new MLogin();
        }

        private void Login2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Iniciar Sesion en Zamenis Health";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            Log = new CXN_LOGIN();
            Log = oLoginController.getUser(oLogin.textBox1.Text.Trim());
            if (Log == null)
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "El usuario proporcionado no existe",
                    TipoImagen = 1000
                };
                MG.ShowDialog();

                this.Close();
            }
            else if (string.IsNullOrEmpty(Log.Patron))
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "El usuario proporcionado no tiene patron asignado, debe iniciar sesion con contraseña",
                    TipoImagen = 1000
                };
                MG.ShowDialog();

                this.Close();
            }
        }
        private void patron1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(patron1.PatronMarcado))
                {
                    if (Log.Patron == patron1.PatronMarcado.Trim())
                    {
                        if (Log.Log_Habilitado == "N")
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Su usuario no esta habilitado";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }

                        if (Log.TyC != "A")
                        {
                            TyC tCon = new TyC(oLogin.textBox1.Text);
                            tCon.ShowDialog();
                        }

                        Contenedor Content = new Contenedor();
                        Content.UsuarioLogueado1 = oLogin.textBox1.Text;
                        Content.PassChange = Log.Log_ClaveC;
                        Content.label1.Text = Log.Log_PrimerA + " " + Log.Log_SegundoA + " " + Log.Log_PrimerN + " " + Log.Log_SegundoN;
                        Content.label6.Text = "Usuario, " + Content.UsuarioLogueado1;

                        if (Log.Log_Rol_Admin.Equals("A")) { Content.toolStripButton2.Visible = true; } else { Content.toolStripButton2.Visible = false; }
                        if (Log.Log_Rol_Recepcion.Equals("A")) { Content.toolStripButton10.Visible = true; } else { Content.toolStripButton10.Visible = false; }
                        if (Log.Log_Rol_Enfermero.Equals("A")) { Content.toolStripButton8.Visible = true; } else { Content.toolStripButton8.Visible = false; }
                        if (Log.Log_Rol_AdminI.Equals("A")) { Content.toolStripButton11.Visible = true; } else { Content.toolStripButton11.Visible = false; }
                        if (Log.Log_Rol_MedGen.Equals("A")) { Content.toolStripButton7.Visible = true; } else { Content.toolStripButton7.Visible = false; }                        
                        if (Log.Log_Rol_Gerencial.Equals("A")) { Content.toolStripButton9.Visible = true; } else { Content.toolStripButton9.Visible = false; }
                        if (Log.Log_Rol_Psicologia.Equals("A")) { Content.toolStripButton3.Visible = true; } else { Content.toolStripButton3.Visible = false; }
                        if (Log.Log_Rol_TO.Equals("A")) { Content.toolStripButton5.Visible = true; } else { Content.toolStripButton5.Visible = false; }
                        if (Log.Log_Rol_TF.Equals("A")) { Content.toolStripButton4.Visible = true; } else { Content.toolStripButton4.Visible = false; }
                        if (Log.Log_Rol_FI.Equals("A")) { Content.toolStripButton6.Visible = true; } else { Content.toolStripButton6.Visible = false; }

                        DateTime Hoy = DateTime.Now.Date;

                        oLogin.Hide();

                        this.Close();

                        Content.ShowDialog();
                    }
                    else
                    {
                        patron1.PatronMarcado = "";

                        MG = new MensajesGeneral()
                        {
                            Mensaje = "El patron dibujado no es valido para el usuario",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
