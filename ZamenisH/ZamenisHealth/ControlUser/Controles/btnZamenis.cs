using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZamenisHealth.ControlUser.Controles
{
    public partial class btnZamenis : UserControl
    {
        public string captionBtn;
        public string tooltipBtn;
        public event EventHandler ButtonClick;

        public btnZamenis()
        {
            InitializeComponent();
            pictureBox1.Click += pictureBox1_Click;
            pictureBox2.Click += pictureBox1_Click;
            label1.Click += pictureBox1_Click;
            this.Click += pictureBox1_Click;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }

        public string LabelText
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            label1.BackColor = Color.SteelBlue;
            label1.ForeColor = Color.Yellow;

            pictureBox1.BackColor = Color.SteelBlue;
            pictureBox1.ForeColor = Color.Yellow;

            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = Properties.Resources.MayorQue_Black;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.BackColor = Color.DarkBlue;
            label1.ForeColor = Color.White;

            pictureBox1.BackColor = Color.DarkBlue;
            pictureBox1.ForeColor = Color.White;

            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = Properties.Resources.MayorQue;
        }

        private void btnZamenis_Load(object sender, EventArgs e)
        {
            ToolTip T = new ToolTip();
            T.ShowAlways = true;
            T.SetToolTip(label1, this.tooltipBtn);
            T.SetToolTip(pictureBox1, this.tooltipBtn);
            T.SetToolTip(pictureBox2, this.tooltipBtn);

            label1.Text = captionBtn;
        }
    }
}
