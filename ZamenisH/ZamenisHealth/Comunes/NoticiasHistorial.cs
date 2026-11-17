using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Clases.Controles;

namespace ZamenisHealth.Comunes
{
    public partial class NoticiasHistorial : Forma2
    {
        private INoticias oNoticias = new MNoticias();
        private IGenerales repositorioGenerales = new MGenerales();
        private int Contador = 1;


        private MensajesGeneral MG;
        public NoticiasHistorial()
        {
            InitializeComponent();
        }

        private void NoticiasHistorial_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Noticias";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarNoticias();
        }
        Panel CrearPanel(CXN_NOTICIAS noticia)
        {
            try
            {
                string NameTitle = "Titulo" + Contador;
                string NameImagen = "Imagen" + Contador;

                Image recoveryImage = repositorioGenerales.ByteToImage(Convert.FromBase64String(noticia.URL));

                PanelCSS panel = new PanelCSS();
                panel.BackColor = Color.White;
                panel.Size = new Size(712, 161);
                panel.BorderStyle = BorderStyle.None;
                panel.Cursor = Cursors.Hand;

                flowLayoutPanel1.Controls.Add(panel);

                Label lblTitulo = new Label();
                lblTitulo.Name = NameTitle;
                lblTitulo.Text = noticia.Titulo;
                lblTitulo.BackColor = Color.Transparent;
                lblTitulo.AutoSize = false;
                lblTitulo.Cursor = Cursors.Hand;
                lblTitulo.Font = new Font("Microsoft Sanz Serif", 12, FontStyle.Bold);
                lblTitulo.ForeColor = Color.Blue;
                lblTitulo.Location = new Point(160, 15);
                lblTitulo.Size = new Size(520, 50);
                lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
                panel.Controls.Add(lblTitulo);

                Label lblResumen = new Label();
                lblResumen.Text = noticia.Resumen;
                lblResumen.BackColor = Color.Transparent;
                lblResumen.ForeColor = Color.Black;
                lblResumen.AutoSize = false;
                lblResumen.Font = new Font("Microsoft Sanz Serif", 11, FontStyle.Regular);
                lblResumen.Location = new Point(150, 67);
                lblResumen.Size = new Size(540, 75);
                lblResumen.TextAlign = ContentAlignment.TopLeft;
                lblResumen.Cursor = Cursors.Hand;
                panel.Controls.Add(lblResumen);

                Label lblFecha = new Label();
                lblFecha.Text = noticia.Fecha.ToString("dd/MM/yyyy");
                lblFecha.BackColor = Color.Transparent;
                lblFecha.ForeColor = Color.Fuchsia;
                lblFecha.AutoSize = false;
                lblFecha.Font = new Font("Microsoft Sanz Serif", 11, FontStyle.Bold);
                lblFecha.Location = new Point(20, 10);
                lblFecha.Size = new Size(115, 22);
                lblFecha.TextAlign = ContentAlignment.MiddleCenter;
                lblFecha.Cursor = Cursors.Hand;
                panel.Controls.Add(lblFecha);

                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = recoveryImage;
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Location = new Point(30, 30);
                pictureBox.Size = new Size(95, 113);
                pictureBox.Name = NameImagen;
                pictureBox.Cursor = Cursors.Hand;
                panel.Controls.Add(pictureBox);

                lblTitulo.Click += (s, e) =>
                {
                    try
                    {
                        AbrirImagenBase64(noticia.URL);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                panel.Click += (s, e) =>
                {
                    try
                    {
                        AbrirImagenBase64(noticia.URL);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                lblResumen.Click += (s, e) =>
                {
                    try
                    {
                        AbrirImagenBase64(noticia.URL);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                pictureBox.Click += (s, e) =>
                {
                    try
                    {
                        AbrirImagenBase64(noticia.URL);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                lblFecha.Click += (s, e) =>
                {
                    try
                    {
                        AbrirImagenBase64(noticia.URL);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                #region Hovers
                void ActivarHover(object s, EventArgs e)
                {
                    panel.BackColor = Color.Thistle;
                }

                void DesactivarHover(object s, EventArgs e)
                {
                    // Solo restaurar cuando el cursor salga de todo el panel
                    if (!panel.ClientRectangle.Contains(
                        panel.PointToClient(Cursor.Position)))
                    {
                        panel.BackColor = Color.White;
                    }
                }

                Control[] controles =
                {
                    panel,
                    lblTitulo,
                    lblResumen,
                    lblFecha,
                    pictureBox
                };

                foreach (Control control in controles)
                {
                    control.MouseEnter += ActivarHover;
                    control.MouseLeave += DesactivarHover;
                }
                #endregion

                Contador++;

                return panel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }        

        private void AbrirImagenBase64(string base64)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64))
                    return;

                // Si viene con encabezado data:image/png;base64, lo eliminamos
                if (base64.Contains(","))
                    base64 = base64.Substring(base64.IndexOf(",") + 1);

                // Convertir Base64 a bytes
                byte[] imagenBytes = Convert.FromBase64String(base64);

                // Crear archivo temporal
                string ruta = Path.Combine(
                    Path.GetTempPath(),
                    $"Imagen_{Guid.NewGuid()}.png"
                );

                // Guardar imagen
                File.WriteAllBytes(ruta, imagenBytes);

                // Abrir con el visor predeterminado de Windows
                Process.Start(new ProcessStartInfo
                {
                    FileName = ruta,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible abrir la imagen: " + ex.Message);
            }
        }
        void CargarNoticias()
        {
            try
            {
                List<CXN_NOTICIAS> lista = oNoticias.GetAllNoticias();
                if (lista != null)
                {
                    lista = lista.Where(x => x.Estado == true).ToList();

                    foreach (CXN_NOTICIAS noticia in lista)
                    {
                        flowLayoutPanel1.Controls.Add(CrearPanel(noticia));
                    }
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se encontraron noticias",
                        TipoImagen = 0
                    };

                    MG.ShowDialog();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }
    }
}
