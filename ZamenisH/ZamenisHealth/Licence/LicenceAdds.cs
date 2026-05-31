using Domain;
using Persistence;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Licence
{
    public partial class LicenceAdds : ConfigForm.BaseForm
    {
        private string Tipo;

        public LicenceAdds(string URL, string tipo)
        {
            InitializeComponent();
            linkLabel1.Text = URL;
            this.Tipo = tipo;
        }

        private void LicenceAdds_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Licencias";
            if (this.Tipo == "PopUps")
            {
                richTextBox1.Visible = true;
                richTextBox2.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "CircProgressBar")
            {
                richTextBox2.Visible = true;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "WinAnimation")
            {
                richTextBox3.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "ValueTuple")
            {
                richTextBox4.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "SQLServerClearTypes")
            {
                richTextBox5.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "ReportViewer")
            {
                richTextBox6.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "VisualStudio")
            {
                richTextBox7.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "FlaTicon")
            {
                richTextBox8.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "Altiria360")
            {
                richTextBox9.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "SQLManagement")
            {
                richTextBox10.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox11.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "PDFium")
            {
                richTextBox11.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox12.Visible = false;
            }

            if (this.Tipo == "ProgBar")
            {
                richTextBox12.Visible = true;
                richTextBox2.Visible = false;
                richTextBox1.Visible = false;
                richTextBox3.Visible = false;
                richTextBox4.Visible = false;
                richTextBox5.Visible = false;
                richTextBox6.Visible = false;
                richTextBox7.Visible = false;
                richTextBox8.Visible = false;
                richTextBox9.Visible = false;
                richTextBox10.Visible = false;
                richTextBox11.Visible = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(linkLabel1.Text);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
