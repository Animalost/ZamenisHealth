using Persistence;

using System;
using System.Drawing;
using System.Windows.Forms;

using ZamenisHealth.Clases;

namespace ZamenisHealth.Licence
{
    public partial class PagosPSE: ConfigForm.BaseForm
    {
        private string TipoPSE;

        public PagosPSE(string tipoPSE)
        {
            InitializeComponent();
            TipoPSE = tipoPSE;
        }

        private void PagosPSE_Load(object sender, EventArgs e)
        {            
            this.Titulo.Text = "Estado de su licencia";

            if (TipoPSE == "Vencido")
            {
                label2.Text = "El estado actual de su licencia es VENCIDA, aun cuenta con un periodo de uso de la aplicacion en este " +
                    "estado.  Sin embargo, en cualquier momento puede terminar el uso definitivo de la aplicacion hasta haber realizado el pago.";

                label2.ForeColor = Color.Blue;
                this.ImageClose.Visible = true;
                button1.Visible = false;
            }
            if (TipoPSE == "Cancelado")
            {
                this.ImageClose.Visible = false;

                label2.Text = "El estado actual de su licencia es CANCELADO, para seguir usando los servicios de Zamenis Health debe realizar " +
                    "el pago correspondiente en el siguiente link de pago";

                label2.ForeColor = Color.Red;

                button1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Conexion.ConectionDictionary["URLPagos"]);
        }
    }
}
