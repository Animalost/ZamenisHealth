using FormAndControls;

using Persistence;

using System;

namespace ZamenisHealth.Facturacion
{
    public partial class FacturaAbierta0 : Forma2
    {
        public FacturaAbierta0()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {            
            Facturacion.FacturaAbierta facturaAbierta = new FacturaAbierta();
            facturaAbierta.ShowDialog();
        }

        private void FacturaAbierta0_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Clase de Factura";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }
    }
}
