using Domain;
using FormAndControls;
using Persistence;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class Inactivate2 : Forma2
    {
        int Iniciar = 60;
        public Timer T = new Timer();

        //RELOJ
        private Bitmap bmpS;
        private Bitmap bmpM;
        private Bitmap bmpH;
        private int horas;
        private int minutos;
        private int segundos;
        private float posH;
        //RELOJ
        public Inactivate2()
        {
            InitializeComponent();
            IniciarReloj();
        }
        private void IniciarReloj()
        {
            try
            {
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, 120, 120);
                Region regPath = new Region(path);
                pictureBox1.Region = regPath;
                path.Dispose();

                bmpS = new Bitmap(Properties.Resources2.Secundario);
                bmpM = new Bitmap(Properties.Resources2.Minutero);
                bmpH = new Bitmap(Properties.Resources2.Horario);
            }
            catch (Exception ex)
            {
                timer1.Enabled = false;
                pictureBox1.Visible = false;
                MessageBox.Show("Error en relog analogico", "Se cerrara", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Dibujar(Bitmap horario, Bitmap minutero, Bitmap secundario)
        {
            try
            {
                Bitmap dibujo = new Bitmap(120, 120);
                Graphics gD = Graphics.FromImage(dibujo);

                gD.DrawImage(horario, 0, 0);
                gD.DrawImage(minutero, 0, 0);
                gD.DrawImage(secundario, 0, 0);

                pictureBox1.Image = dibujo;
                gD.Dispose();
            }
            catch (Exception ex)
            {
                timer1.Enabled = false;
                pictureBox1.Visible = false;
                MessageBox.Show("Error en relog analogico", "Se cerrara", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Reloj()
        {
            try
            {
                Matrix matrixH = new Matrix();

                if (horas > 12)
                    matrixH.RotateAt(((horas - 12) * 30) + posH, new PointF(60, 60));
                else
                    matrixH.RotateAt((horas * 30) + posH, new PointF(60, 60));

                Bitmap horario = new Bitmap(120, 120);
                Graphics gH = Graphics.FromImage(horario);
                gH.Transform = matrixH;
                gH.DrawImage(bmpH, 0, 0);
                matrixH.Dispose();
                gH.Dispose();

                Matrix matrixM = new Matrix();
                matrixM.RotateAt(6 * minutos, new PointF(60, 60));
                Bitmap minutero = new Bitmap(120, 120);
                Graphics gM = Graphics.FromImage(minutero);
                gM.Transform = matrixM;
                gM.DrawImage(bmpM, 0, 0);
                matrixM.Dispose();
                gM.Dispose();

                Matrix matrixS = new Matrix();
                matrixS.RotateAt(6 * segundos, new PointF(60, 60));
                Bitmap secundario = new Bitmap(120, 120);
                Graphics gS = Graphics.FromImage(secundario);
                gS.Transform = matrixS;
                gS.DrawImage(bmpS, 0, 0);
                matrixS.Dispose();
                gS.Dispose();

                Dibujar(horario, minutero, secundario);
            }
            catch (Exception ex)
            {
                timer1.Enabled = false;
                pictureBox1.Visible = false;
                MessageBox.Show("Error en relog analogico", "Se cerrara", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void PosicionHorario()
        {
            try
            {
                if (minutos <= 15)
                    posH = 5f;
                if (minutos > 15 && minutos <= 30)
                    posH = 10f;
                if (minutos > 30 && minutos <= 45)
                    posH = 15f;
                if (minutos > 45 && minutos <= 59)
                    posH = 20f;
            }
            catch (Exception ex)
            {
                timer1.Enabled = false;
                pictureBox1.Visible = false;
                MessageBox.Show("Error en relog analogico", "Se cerrara", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void ActivaTimer()
        {
            T.Interval = 1000;
            T.Tick += new EventHandler(Conteo);
            T.Start();
        }

        private void Conteo(object sender, EventArgs e)
        {
            try
            {
                Iniciar = Iniciar - 1;
                Inactivate f29 = Application.OpenForms.OfType<Inactivate>().SingleOrDefault();
                f29.label2.Text = Iniciar.ToString();

                if (Iniciar == 0)
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Inactivate2_Load(object sender, EventArgs e)
        {
            ActivaTimer();
            PanelTitulo.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                horas = DateTime.Now.Hour;
                minutos = DateTime.Now.Minute;
                segundos = DateTime.Now.Second;

                PosicionHorario();
                Reloj();
            }
            catch (Exception ex)
            {
                timer1.Enabled = false;
                pictureBox1.Visible = false;
                MessageBox.Show("Error en relog analogico", "Se cerrara", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
