using FormAndControls;
using Persistence;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Properties;

namespace ZamenisHealth.Medicina
{
    public partial class Cargos3 : Forma2
    {
        public Cargos3()
        {
            InitializeComponent();
            SoloNumeros(textBox1);
        }

        private async void Cargos3_Load(object sender, EventArgs e)
        {          
            // Esperamos a que terminen de llegar las teclas
            await Task.Delay(500);

            Titulo.Text = "Cantidad a Ingresar";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ImageClose.Visible = false;
            ImageMinimize.Visible = false;

            PictureBox btnClose = new PictureBox();
            btnClose.BackColor = Color.WhiteSmoke;
            btnClose.Image = Resources.cerca;
            btnClose.SizeMode = PictureBoxSizeMode.StretchImage;
            btnClose.Size = new Size(24, 24);
            PanelTitulo.Controls.Add(btnClose);
            btnClose.Location = new Point(PanelTitulo.Width - 50, 11);
            btnClose.Anchor = AnchorStyles.Top;
            btnClose.Anchor = AnchorStyles.Right;
            btnClose.Cursor = Cursors.Hand;
            btnClose.BringToFront();

            btnClose.Click += BtnClose_Click;

            textBox1.Enabled = true;

            textBox1.Clear();
            textBox1.Focus();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Cargos2 f1 = Application.OpenForms
                        .OfType<Cargos2>()
                        .LastOrDefault();

            if (f1 != null)
            {
                f1.CantidadCargo = 0;
            }

            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(textBox1.Text))
                    {
                        MessageBox.Show(
                            "Digite una cantidad válida",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }

                    Cargos2 f1 = Application.OpenForms
                        .OfType<Cargos2>()
                        .LastOrDefault();

                    if (f1 != null)
                    {
                        f1.CantidadCargo = Convert.ToInt32(textBox1.Text);
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
