using FormAndControls;
using System;

namespace ZamenisHealth.PagosApp
{
    public partial class Payments2 : Forma2
    {
        private string TempFile;

        public Payments2(string tempFile)
        {
            InitializeComponent();
            TempFile = tempFile;
        }

        private void Payments2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Factura de Servicios";
            webBrowser1.Navigate(TempFile);

            toolStrip1.SendToBack();
        }
    }
}
