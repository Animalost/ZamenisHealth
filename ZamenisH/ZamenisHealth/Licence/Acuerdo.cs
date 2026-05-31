using Domain;
using Microsoft.Reporting.WinForms;
using Persistence;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Licence
{
    public partial class Acuerdo : ConfigForm.BaseForm
    {
        public Acuerdo()
        {
            InitializeComponent();
        }
        private void Acuerdo_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Licencias";
            

            textBox2.Text = Conexion.ConectionDictionary["Tercero"];
            textBox3.Text = Convert.ToDateTime(Conexion.ConectionDictionary["Vencimiento"]).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
            textBox4.Text = "***** LICENCIA PRIVADA UNICA EN ESTE EQUIPO *****";
            textBox5.Text = Conexion.ConectionDictionary["Telefono"];
            textBox6.Text = Conexion.ConectionDictionary["Direccion"];
            textBox7.Text = Conexion.ConectionDictionary["Nit"];
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://github.com/Tulpep/Notification-Popup-Window/blob/master/LICENSE", "PopUps");
            L.ShowDialog();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://github.com/falahati/WinFormAnimation/blob/master/LICENSE", "WinAnimation");
            L.ShowDialog();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://download.microsoft.com/download/4/3/3/43327557-9868-4AC5-B387-21B534474F02/License_SysClrTypes.rtf", "SQLServerClearTypes");
            L.ShowDialog();
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://download.microsoft.com/download/D/E/7/DE7EBACA-6109-4756-A9E3-11417DAD778E/License_ReportViewer.rtf", "ReportViewer");
            L.ShowDialog();
        }

        private void toolStripButton40_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://visualstudio.microsoft.com/es/license-terms/vs2022-ga-community/", "VisualStudio");
            L.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel1.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel2.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel3.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel4.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel5.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel6.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel7.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel8.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel9.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel10.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel11.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel12.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel13.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel14.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel15.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel16.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel18_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel18.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel17.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel20_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel20.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel19_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel19.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel22_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel22.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel21_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel21.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel24_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel24.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel23_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel23.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel26_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel26.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel25_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel25.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel28_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel28.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel27_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel27.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel30_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel30.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void linkLabel29_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LicenceAdds L = new LicenceAdds(linkLabel29.Text, "FlaTicon");
            L.ShowDialog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Reportes.Maestro maestro = new Reportes.Maestro();
                maestro.Universal.LocalReport.DataSources.Clear();
                //maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", M));
                maestro.Universal.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.LicenceZH.rdlc";
                maestro.Universal.SetDisplayMode(DisplayMode.PrintLayout);
                maestro.Universal.ZoomMode = ZoomMode.Percent;
                maestro.Universal.ZoomPercent = 100;
                maestro.Universal.LocalReport.EnableExternalImages = true;
                maestro.Universal.RefreshReport();
                maestro.Universal.Visible = true;
                maestro.Universal.Dock = System.Windows.Forms.DockStyle.Fill;
                maestro.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }           
        }
 
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://static.altiria.com/comercial/condiciones-generales-co.pdf", "Altiria360");
            L.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://learn.microsoft.com/es-es/Legal/sql/sql-server-management-studio-license-terms?redirectedfrom=MSDN", "SQLManagement");
            L.ShowDialog();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://www.nuget.org/packages/Pdfium.Net.SDK/4.86.2704/license", "PDFium");
            L.ShowDialog();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            LicenceAdds L = new LicenceAdds("https://github.com/falahati/CircularProgressBar/blob/master/LICENSE", "ProgBar");
            L.ShowDialog();
        }
    }
}
