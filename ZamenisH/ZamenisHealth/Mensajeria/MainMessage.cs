using Persistence;

using System;
using System.Windows.Forms;

using ZamenisHealth.Clases;

namespace ZamenisHealth.Mensajeria
{
    public partial class MainMessage : ConfigForm.BaseForm
    {     
        public MainMessage()
        {
            InitializeComponent();

            btnZamenis1.ButtonClick += btnZamenis1_ButtonClick;
            btnZamenis2.ButtonClick += btnZamenis2_ButtonClick;            
            btnZamenis4.ButtonClick += btnZamenis4_ButtonClick;

            btnZamenis1.captionBtn = "Configuracion";
            btnZamenis1.tooltipBtn = "Cierra esta ventana";

            btnZamenis2.captionBtn = "Sender";
            btnZamenis2.tooltipBtn = "Enviar mensajes de Email o SMS";

            btnZamenis4.captionBtn = "Historial";
            btnZamenis4.tooltipBtn = "Ver historial de los mensajes enviados";
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new Config());
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            Sender s = new Sender();
            s.ShowDialog();
        }
        
        private void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new Historial());
        }

        private void AbrirFormEnPanel(object Formhijo)
        {
            if (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(fh);
            this.panel1.Tag = fh;
            fh.Show();
        }

        private void MainMessage_Load(object sender, EventArgs e)
        {
            this.Titulo.Text = "Gestor de Mensajeria";

            if (Conexion.ConectionDictionary["Recordatorios"] != "A")
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "Su licencia adquirida no cuenta con esta opcion disponible para este equipo, consulte al administrador";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                this.Dispose();
                this.Close();
                return;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
