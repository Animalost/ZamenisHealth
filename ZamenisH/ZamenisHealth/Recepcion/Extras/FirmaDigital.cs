using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class FirmaDigital : Forma2
    {
        private IFirmasDigitales oController;
        private int Admision, IdPaciente;
        private string Tipo;

        Bitmap lienzo;
        bool dibujando = false;
        Point ultimoPunto;
        bool estabaDentro  = false;
        private MensajesGeneral MG;

        public FirmaDigital(int admision, int idPaciente, string tipo)
        {
            InitializeComponent();
            Admision = admision;
            IdPaciente = idPaciente;
            Tipo = tipo;

            oController = new MFirmasDigitales();
        }
        private void FirmaDigital_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Firma Digital";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            lienzo = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = lienzo;

            Cursor.Clip = pictureBox1.RectangleToScreen(pictureBox1.ClientRectangle);
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (pictureBox1.ClientRectangle.Contains(e.Location))
            {
                dibujando = true;
                ultimoPunto = e.Location;
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dibujando)
                return;

            bool dentro = pictureBox1.ClientRectangle.Contains(e.Location);

            if (dentro)
            {
                using (Graphics g = Graphics.FromImage(lienzo))
                using (Pen lapiz = new Pen(Color.Black, 6))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawLine(lapiz, ultimoPunto, e.Location);
                }

                pictureBox1.Invalidate();
            }

            ultimoPunto = e.Location;
            estabaDentro = dentro;
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            dibujando = false;
        }
        private void FirmaDigital_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Clip = Rectangle.Empty;
        }
        private void FirmaDigital_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Save();
            }
            if (e.KeyData == Keys.Escape)
            {
                this.Dispose();
                this.Close();
            }
            if (e.KeyData == Keys.Delete)
            {
                LimpiarFirma();
            }
        }
        Rectangle ObtenerAreaFirma(Bitmap imagen)
        {
            int minX = imagen.Width;
            int minY = imagen.Height;
            int maxX = 0;
            int maxY = 0;

            for (int y = 0; y < imagen.Height; y++)
            {
                for (int x = 0; x < imagen.Width; x++)
                {
                    Color pixel = imagen.GetPixel(x, y);

                    if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            if (maxX <= minX || maxY <= minY)
                return new Rectangle(0, 0, imagen.Width, imagen.Height);

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }
        Bitmap AjustarFirma(Bitmap original)
        {
            Rectangle area = ObtenerAreaFirma(original);

            Bitmap nueva = new Bitmap(area.Width, area.Height);

            using (Graphics g = Graphics.FromImage(nueva))
            {
                g.DrawImage(original,
                    new Rectangle(0, 0, nueva.Width, nueva.Height),
                    area,
                    GraphicsUnit.Pixel);
            }

            return nueva;
        }
        void Save()
        {
            try
            {
                byte[] imageSign = null;
                Bitmap firmaOriginal = null;                

                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    firmaOriginal = new Bitmap(ms);
                }

                Bitmap firmaRecortada = AjustarFirma(firmaOriginal);

                using (MemoryStream ms2 = new MemoryStream())
                {
                    firmaRecortada.Save(ms2, ImageFormat.Png);
                    ms2.Position = 0;
                    imageSign = ms2.ToArray();
                }

                if (imageSign == null)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay firma para grabar",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                    return;
                }

                if (Tipo == "Admisiones" || Tipo == "DatosCita")
                {
                    CXN_FIRMASDIGITALES cXN_FIRMASDIGITALES = new CXN_FIRMASDIGITALES()
                    {
                        Admision = Admision,
                        Firma = imageSign,
                        Paciente = IdPaciente,
                        Usuario = Contenedor.UsuarioLogueado
                    };

                    bool insert = oController.InsertSign(cXN_FIRMASDIGITALES);
                    if (insert == false)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No fue posible guardar la firma de la atencion",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                        return;
                    }

                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Grabado",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else if (Tipo == "Historia_MedicinaGeneral_2")
                {
                    Historia_MedicinaGeneral_2 f2 = Application.OpenForms.OfType<Historia_MedicinaGeneral_2>().SingleOrDefault();
                    f2.setFirma(imageSign);

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Acceso a forma indebido",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void LimpiarFirma()
        {
            if (MessageBox.Show("¿Desea borrar la firma?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lienzo = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = lienzo;

                dibujando = false;
                ultimoPunto = Point.Empty;
            }           
        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            estabaDentro = true;
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            estabaDentro = false;
            dibujando = false;
        }
    }
}
