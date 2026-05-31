using System;
using System.Windows.Forms;

namespace ZamenisHealth
{
    public partial class SplashScreen : Form
    {
        private Timer fadeInTimer;

        public SplashScreen()
        {
            InitializeComponent();            

            this.Opacity = 0;
            timer1.Enabled = true;
            timer1.Interval = 3000;

            fadeInTimer = new Timer();
            fadeInTimer.Interval = 50; // Velocidad del efecto en milisegundos
            fadeInTimer.Tick += FadeInTimer_Tick;
            fadeInTimer.Start();
        }
        private void FadeInTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1) // Incrementa la opacidad hasta 1 (100%)
            {
                this.Opacity += 0.05; // Incremento de opacidad
            }
            else
            {
                fadeInTimer.Stop(); // Detiene el timer al alcanzar opacidad máxima
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
