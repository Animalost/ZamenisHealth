using Domain;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class AcercaDE : Forma2
    {
        public AcercaDE()
        {
            InitializeComponent();
        }

        private void AcercaDE_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Acerca de";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            Dictionary<string, string> getData = Conexion.Conection();
            label2.Text = getData["TextLicence"];
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Reportes.Maestro maestro = new Reportes.Maestro();
                maestro.Universal.LocalReport.DataSources.Clear();
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
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel2.Text);
        }
    }
}
