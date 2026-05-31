using System;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Mensajeria
{
    public partial class Historial2 : ConfigForm.BaseForm
    {
        public Historial2(string Mens)
        {
            InitializeComponent();
            textBox1.Text = Mens;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Historial2_Load(object sender, EventArgs e)
        {
            this.Titulo.Visible = false;
            this.ImageClose.Visible = false;
        }
    }
}
