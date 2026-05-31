using System;
using ZamenisHealth.Clases; 

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class TipoCitaMG : ConfigForm.BaseForm
    {
        public TipoCitaMG()
        {
            InitializeComponent();
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            AgendarCita.Tipo_Serv = "N";
            this.Dispose();
            this.Close();
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            AgendarCita.Tipo_Serv = "A";
            this.Dispose();
            this.Close();
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            AgendarCita.Tipo_Serv = "R";
            this.Dispose();
            this.Close();
        }
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            AgendarCita.Tipo_Serv = "C";
            this.Dispose();
            this.Close();
        }
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            AgendarCita.Tipo_Serv = "E";
            this.Dispose();
            this.Close();
        }
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            AgendarCita.Tipo_Serv = "Z";
            this.Dispose();
            this.Close();
        }
    }
}
