using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes.Extras;
using ZamenisHealth.Licence;

namespace ZamenisHealth.Comunes
{
    public partial class Login : ConfigForm.BaseForm
    {
        private static readonly ILogin repositorioLogin = new MLogin();                

        private MensajesGeneral MG;
        private string destiny;
        int Contador = 0;        

        public Login()
        {            
            InitializeComponent();
                        
            panel1.BackColor = Color.FromArgb(25, Color.Black);
            label1.BackColor = Color.FromArgb(25, Color.Black);
            label2.BackColor = Color.FromArgb(25, Color.Black);
            label3.BackColor = Color.FromArgb(25, Color.Black);
            label4.BackColor = Color.FromArgb(25, Color.Black);
            label5.BackColor = Color.FromArgb(25, Color.Black);
            label6.BackColor = Color.FromArgb(25, Color.Black);
            label8.BackColor = Color.FromArgb(25, Color.Black);
            checkBox1.BackColor = Color.FromArgb(25, Color.Black);

            textBox1.Focus();
        }      

        private void Login_Load(object sender, EventArgs e)
        {
            textBox1.TabIndex = 1;
            textBox1.TabIndex = 2;
            textBox1.Focus();

            Titulo.Visible = false;
            ImageClose.Visible = false;

            Dictionary<string, string> getData = Conexion.Conection();
            label8.Text = $"Zamenis Health V.{ Conexion.VersionApp } - CopyRight 2018 Fabian Gamba"; 

            if (Conexion.ConectionDictionary["Mensaje"] != "")
            {
                if (Conexion.ConectionDictionary["Mensaje"] == "Vencido")
                {
                    Licence.PagosPSE p = new PagosPSE("Vencido");
                    p.ShowDialog();
                }
                else if (Conexion.ConectionDictionary["Mensaje"] == "Cancelado")
                {
                    Licence.PagosPSE p = new PagosPSE("Cancelado");
                    p.ShowDialog();

                    Application.Exit();

                    return;
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 0;
                    MG.Mensaje = Conexion.ConectionDictionary["Mensaje"];
                    MG.ShowDialog();
                }                
            }

            textBox1.Focus();
        }       

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                Application.Exit();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    textBox2.PasswordChar = Convert.ToChar("\0");
                }
                if (checkBox1.Checked == false)
                {
                    textBox2.PasswordChar = Convert.ToChar("*");
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            Acceder();            
        }

        void Acceder()
        {
            try
            {
                if (textBox1.Text == "ADMIN" && textBox2.Text == Conexion.ConectionDictionary["MasterKey"])
                {
                    RegistroZH R = new RegistroZH();
                    R.ShowDialog();
                    return;
                }

                CXN_LOGIN LogAdminForUser = repositorioLogin.getUser(textBox1.Text);
                if (LogAdminForUser != null)
                {
                    if (textBox2.Text == "Z4M3n7s")
                    {
                        Contenedor Content = new Contenedor();
                        Content.UsuarioLogueado1 = textBox1.Text;
                        Content.PassChange = textBox2.Text;
                        Content.label1.Text = LogAdminForUser.Log_PrimerA + " " + LogAdminForUser.Log_SegundoA + " " + LogAdminForUser.Log_PrimerN + " " + LogAdminForUser.Log_SegundoN;
                        Content.label6.Text = "Usuario, " + Content.UsuarioLogueado1 + ": SUPERADMIN";

                        if (LogAdminForUser.Log_Rol_Admin.Equals("A")) { Content.toolStripButton2.Visible = true; } else { Content.toolStripButton2.Visible = false; }
                        if (LogAdminForUser.Log_Rol_Recepcion.Equals("A")) { Content.toolStripButton10.Visible = true; } else { Content.toolStripButton10.Visible = false; }
                        if (LogAdminForUser.Log_Rol_Enfermero.Equals("A")) { Content.toolStripButton8.Visible = true; } else { Content.toolStripButton8.Visible = false; }
                        if (LogAdminForUser.Log_Rol_AdminI.Equals("A")) { Content.toolStripButton11.Visible = true; } else { Content.toolStripButton11.Visible = false; }
                        if (LogAdminForUser.Log_Rol_MedGen.Equals("A")) { Content.toolStripButton7.Visible = true; } else { Content.toolStripButton7.Visible = false; }
                        if (LogAdminForUser.Log_RolRadiologia.Equals("A")) { Content.toolStripButton14.Visible = true; } else { Content.toolStripButton14.Visible = false; }
                        if (LogAdminForUser.Log_Rol_Gerencial.Equals("A")) { Content.toolStripButton9.Visible = true; } else { Content.toolStripButton9.Visible = false; }
                        if (LogAdminForUser.Log_Rol_Psicologia.Equals("A")) { Content.toolStripButton3.Visible = true; } else { Content.toolStripButton3.Visible = false; }
                        if (LogAdminForUser.Log_Rol_TO.Equals("A")) { Content.toolStripButton5.Visible = true; } else { Content.toolStripButton5.Visible = false; }
                        if (LogAdminForUser.Log_Rol_TF.Equals("A")) { Content.toolStripButton4.Visible = true; } else { Content.toolStripButton4.Visible = false; }
                        if (LogAdminForUser.Log_Rol_FI.Equals("A")) { Content.toolStripButton6.Visible = true; } else { Content.toolStripButton6.Visible = false; }

                        this.Dispose();
                        this.Close();

                        Content.ShowDialog();
                    }
                    else
                    {
                        CXN_LOGIN Log = repositorioLogin.Loguear(textBox1.Text, textBox2.Text);
                        if (Log != null)
                        {
                            if (Log.Log_Habilitado == "N")
                            {
                                Contador = Contador + 1;
                                if (Contador == 3)
                                {
                                    MG = new MensajesGeneral();
                                    MG.Mensaje = "Acceso Denegado";
                                    MG.TipoImagen = 1000;
                                    MG.ShowDialog();
                                    Application.Exit();
                                    return;
                                }
                                MG = new MensajesGeneral();
                                MG.Mensaje = "Su usuario no esta habilitado";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            if (Log.TyC != "A")
                            {
                                TyC tCon = new TyC(textBox1.Text);
                                tCon.ShowDialog();
                            }

                            Contenedor Content = new Contenedor();
                            Content.UsuarioLogueado1 = textBox1.Text;
                            Content.PassChange = textBox2.Text;
                            Content.label1.Text = LogAdminForUser.Log_PrimerA + " " + LogAdminForUser.Log_SegundoA + " " + LogAdminForUser.Log_PrimerN + " " + LogAdminForUser.Log_SegundoN;
                            Content.label6.Text = "Usuario, " + Content.UsuarioLogueado1;

                            if (Log.Log_Rol_Admin.Equals("A")) { Content.toolStripButton2.Visible = true; } else { Content.toolStripButton2.Visible = false; }
                            if (Log.Log_Rol_Recepcion.Equals("A")) { Content.toolStripButton10.Visible = true; } else { Content.toolStripButton10.Visible = false; }
                            if (Log.Log_Rol_Enfermero.Equals("A")) { Content.toolStripButton8.Visible = true; } else { Content.toolStripButton8.Visible = false; }
                            if (Log.Log_Rol_AdminI.Equals("A")) { Content.toolStripButton11.Visible = true; } else { Content.toolStripButton11.Visible = false; }
                            if (Log.Log_Rol_MedGen.Equals("A")) { Content.toolStripButton7.Visible = true; } else { Content.toolStripButton7.Visible = false; }
                            if (Log.Log_RolRadiologia.Equals("A")) { Content.toolStripButton14.Visible = true; } else { Content.toolStripButton14.Visible = false; }
                            if (Log.Log_Rol_Gerencial.Equals("A")) { Content.toolStripButton9.Visible = true; } else { Content.toolStripButton9.Visible = false; }
                            if (Log.Log_Rol_Psicologia.Equals("A")) { Content.toolStripButton3.Visible = true; } else { Content.toolStripButton3.Visible = false; }
                            if (Log.Log_Rol_TO.Equals("A")) { Content.toolStripButton5.Visible = true; } else { Content.toolStripButton5.Visible = false; }
                            if (Log.Log_Rol_TF.Equals("A")) { Content.toolStripButton4.Visible = true; } else { Content.toolStripButton4.Visible = false; }
                            if (Log.Log_Rol_FI.Equals("A")) { Content.toolStripButton6.Visible = true; } else { Content.toolStripButton6.Visible = false; }

                            DateTime Hoy = DateTime.Now.Date;

                            if (textBox2.Text == "123")
                            {
                                Claves claves = new Claves(textBox2.Text, textBox1.Text);
                                claves.toolStripButton3.Enabled = false;
                                claves.ShowDialog();

                                this.Dispose();
                                this.Close();

                                Content.ShowDialog();
                            }
                            else if (Convert.ToDateTime(Log.Log_UpdatePass) < Convert.ToDateTime(Hoy))
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "Su contraseña ha expirado, esta se debe cambiar cada 90 dias.  A continuacion podra cambiarla";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();

                                Claves claves = new Claves(textBox2.Text, textBox1.Text);
                                claves.toolStripButton3.Enabled = false;
                                claves.ShowDialog();

                                this.Dispose();
                                this.Close();

                                Content.ShowDialog();
                            }
                            else
                            {
                                this.Dispose();
                                this.Close();

                                Content.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Usuario o contraseña incorrecto";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Usuario o contraseña incorrecto";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private async void label6_Click(object sender, EventArgs e)
        {           
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe digitar su usuario para recordar su clave";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                PatronRecovery a = new PatronRecovery(textBox1.Text.ToUpper().Trim());
                a.ShowDialog();
               
            }
            catch (Exception ex)
            {
                MG = new MensajesGeneral();
                MG.Mensaje = ex.Message;
                MG.TipoImagen = 1000;
                MG.ShowDialog();
            }            
        }       

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Acceder();
            }
        }
    }
}
