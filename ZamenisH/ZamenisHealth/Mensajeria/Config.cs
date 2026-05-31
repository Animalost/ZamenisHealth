using Domain.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Mensajeria
{
    public partial class Config : ConfigForm.BaseForm
    {
        private static readonly ISender repoSender = new MSender();
        private static readonly IDatosEmail repoEmail = new MDatosEmail();
        public Config()
        {
            InitializeComponent();
        }

        private void Config_Load(object sender, EventArgs e)
        {
            this.Titulo.Visible = false;
            this.ImageClose.Visible = false;

            var Datos = repoEmail.FirstEmail();
            textBox2.Text = Datos.Ema_Email;
            textBox3.Text = Datos.Ema_Pass;
            textBox4.Text = Datos.Ema_Server;
            textBox6.Text = Datos.Ema_Puerto.ToString();
            textBox7.Text = Datos.Ema_Muestra;
            textBox8.Text = Datos.Ema_URL_CitasM;
            textBox9.Text = Datos.Ema_URL_CitasS;
            textBox10.Text = Datos.Ema_Baja_Email;
            textBox11.Text = Datos.Ema_Server;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

            DialogResult result = MessageBox.Show("¿Desea actualizar los datos?",
                                                  "Zamenis Health - Mensajeria",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                CXN_EMAIL E = new CXN_EMAIL
                {
                    Ema_Email = textBox2.Text,
                    Ema_Pass = textBox3.Text,
                    Ema_Server = textBox4.Text,
                    Ema_Puerto = Convert.ToInt32(textBox6.Text),
                    Ema_Muestra = textBox7.Text,
                    Ema_URL_CitasM = textBox8.Text,
                    Ema_URL_CitasS = textBox9.Text,
                    Ema_Baja_Email = textBox10.Text
                };

                bool update = repoSender.Actualizar(E);
                if (update != true)
                {
                    MG.Mensaje = "Hubo un error actualizando los datos del servidor";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    MG.Mensaje = "Datos del servidor actualizados correctamente";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
        }
    }
}
