using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes
{
    public partial class TyC : Forma2
    {        

        private static readonly ILogin repoLogin = new MLogin();

        private string UserLogon;

        public TyC(string userLog)
        {
            InitializeComponent();
            this.UserLogon = userLog;
        }

        private void TyC_Load(object sender, EventArgs e)
        {
            try
            {
                ImageClose.Visible = false;
                ImageMinimize.Visible = false;

                boton2.Visible = false;

                Titulo.Text = "Adherencia";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                byte[] rtfData = Encoding.UTF8.GetBytes(Properties.Resources.Condiciones);

                using (MemoryStream stream = new MemoryStream(rtfData))
                {
                    richTextBox1.LoadFile(stream, RichTextBoxStreamType.RichText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Dispose();
                this.Close();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                    repoLogin.AcceptTyC(this.UserLogon);
                

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Dispose();
                this.Close();
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
