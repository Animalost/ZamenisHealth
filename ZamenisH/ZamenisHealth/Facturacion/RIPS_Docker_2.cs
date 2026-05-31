using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;

namespace ZamenisHealth.Facturacion
{
    public partial class RIPS_Docker_2 : Forma2
    {
        private readonly IFacturacion facturacion = new MFacturacion();
        private string FElectron;
        private Comunes.MensajesGeneral MG;

        public RIPS_Docker_2(string fElectron)
        {
            InitializeComponent();
            this.FElectron = fElectron;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(FElectron))
                {
                    MessageBox.Show("Error: Fecha Electronica no puede ser nula o vacia.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Error: Fecha Electronica no puede ser nula o vacia.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool result = facturacion.UpdateCUVManual(FElectron, textBox1.Text);
                if (result == true)
                {
                    MG = new Comunes.MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "CUV Actualizado Correctamente";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MG = new Comunes.MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No fue posible actualizar el CUV";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RIPS_Docker_2_Load(object sender, EventArgs e)
        {
            this.Titulo.Text = "Registrar CUV SISPRO";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }
    }
}
