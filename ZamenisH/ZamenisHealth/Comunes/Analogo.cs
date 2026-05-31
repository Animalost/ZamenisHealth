using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Comunes
{
    public partial class Analogo : Form
    {
        private Bitmap bmpS;
        private Bitmap bmpM;
        private Bitmap bmpH;

        private int horas;
        private int minutos;
        private int segundos;

        private float posH;

        public Analogo()
        {
            Process[] p = Process.GetProcessesByName("Clock");
            if (p.Length > 1)
                p[0].Kill();
            InitializeComponent();
            Iniciar();
        }

        private void Iniciar()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, 120, 120);
            Region regPath = new Region(path);
            this.Region = regPath;
            path.Dispose();

            bmpS = new Bitmap(Properties.Resources2.Secundario);
            bmpM = new Bitmap(Properties.Resources2.Minutero);
            bmpH = new Bitmap(Properties.Resources2.Horario);

            LeerOPacidad();
        }

        private void LeerOPacidad()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Clock");

            if (key == null)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Clock", "Opacidad", "100");
            }

            int opacidad = Convert.ToInt32(Registry.GetValue(@"HKEY_CURRENT_USER\Software\myClock", "Opacidad", "Default Value"));
            GC.Collect();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ConfigForm.ReleaseCapturing();
            ConfigForm.SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }

        private void EscribirOpacidad()
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\myClock", "Opacidad", trackBar1.Value.ToString());
        }

        private void Reloj()
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

        private void Dibujar(Bitmap horario, Bitmap minutero, Bitmap secundario)
        {
            Bitmap dibujo = new Bitmap(120, 120);
            Graphics gD = Graphics.FromImage(dibujo);

            gD.DrawImage(horario, 0, 0);
            gD.DrawImage(minutero, 0, 0);
            gD.DrawImage(secundario, 0, 0);

            pictureBox1.Image = dibujo;
            gD.Dispose();
        }

        private void PosicionHorario()
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

        private void Analogo_Load(object sender, EventArgs e)
        {
            int X = Screen.PrimaryScreen.Bounds.Width - 130;
            this.Location = new Point(X, 10);

            Contenedor f29 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
            f29.linkLabel5.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            horas = DateTime.Now.Hour;
            minutos = DateTime.Now.Minute;
            segundos = DateTime.Now.Second;

            PosicionHorario();
            Reloj();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();

            Contenedor f29 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
            f29.linkLabel5.Visible = true;
            f29.linkLabel6.Visible = true;

            PopupNotifier P = PopUps.setPopUp(Properties.Resources2.comprobado,
                                                                 Color.LightBlue,
                                                                 "RELOJ ANALOGO CERRADO",
                                                                 Color.Blue,
                                                                 "Recuerde que puede volver a habilitar el reloj haciendo click en la pantalla principal " +
                                                                 "en el icono del reloj");
            P.Popup();
        }

        private void trackBar1_MouseLeave(object sender, EventArgs e)
        {
            trackBar1.Visible = false;
            //label1.Visible = false;
            EscribirOpacidad();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //label1.Text = trackBar1.Value.ToString() + "%";
            this.Opacity = trackBar1.Value / 100.0;
        }
    }
}
