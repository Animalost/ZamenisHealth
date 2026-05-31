using FormAndControls;
using Persistence;
using System.Drawing;
using System.Windows.Forms;

namespace ZamenisHealth.Reportes
{
    public partial class Maestro : Forma2
    {
        public Maestro()
        {
            InitializeComponent();
            System.Threading.ApartmentState state = System.Threading.ApartmentState.STA;
            System.Threading.Thread.CurrentThread.SetApartmentState(state);
        }

        private void Maestro_Load(object sender, System.EventArgs e)
        {
            
            this.Universal.RefreshReport();

            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = Properties.Resources.maximizar;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Size = new Size(24, 24);
            PanelTitulo.Controls.Add(pictureBox);
            pictureBox.Location = new Point(PanelTitulo.Width - 110, 11);
            pictureBox.Anchor = AnchorStyles.Top;
            pictureBox.Anchor = AnchorStyles.Right;
            pictureBox.Cursor = Cursors.Hand;
            pictureBox.BringToFront();

            pictureBox.Click += pictureBox1_Click;

            Titulo.Text = "Reportes";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            /*PanelTitulo.Anchor = AnchorStyles.Right;
            PanelTitulo.Anchor = AnchorStyles.Top;
            PanelTitulo.Anchor = AnchorStyles.Left;*/

            /*Titulo.Anchor = AnchorStyles.Left;
            Titulo.Anchor = AnchorStyles.Top;
            Titulo.Anchor = AnchorStyles.Right;
            
            SubTitulo.Anchor = AnchorStyles.Left;
            SubTitulo.Anchor = AnchorStyles.Top;
            SubTitulo.Anchor = AnchorStyles.Right;*/

            Universal.BorderStyle = BorderStyle.None;
            panel1.BorderStyle = BorderStyle.None;
            PanelTitulo.BorderStyle = BorderStyle.None;
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            (int W, int H) sizePredterminada = (1000, 774);

            int sizeActualH = this.Size.Height;
            int sizeActualW = this.Size.Width;

            if (sizePredterminada.W != sizeActualW && sizePredterminada.H != sizeActualH)
            {
                this.Size = new Size(sizePredterminada.W, sizePredterminada.H);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.WhiteSmoke;
                PanelTitulo.BorderStyle = BorderStyle.None;
                return;
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
                this.Bounds = Screen.FromControl(this).WorkingArea;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.WhiteSmoke;
                PanelTitulo.BorderStyle = BorderStyle.None;
                return;
            }
        }       
    }
}
