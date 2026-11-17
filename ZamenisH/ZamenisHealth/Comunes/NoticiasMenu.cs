using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes
{
    public partial class NoticiasMenu : Forma2
    {
        private INoticias oNoticias = new MNoticias();
        private MensajesGeneral MG;
        private Byte[] bytes;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn TituloN;
        DataColumn Resumen;
        DataColumn Estado;

        public NoticiasMenu()
        {
            InitializeComponent();
        }

        private void NoticiasMenu_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Generador de Noticias";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            textBox1.MaxLength = 50;
            textBox2.MaxLength = 200;

            gridZH1.CeldaHeight = true;
            gridZH1.dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

            CargarNoticias();
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea cambiar el estado de esta noticia?",
                                                  "Cambio de estados",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int pos = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    string estado = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

                    bool update = oNoticias.UpdateEstado(pos, estado == "ACTIVO" ? false : true);
                    if (update == true)
                    {
                        CargarNoticias();
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se logro actualizar el estado de la noticia",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void CargarNoticias()
        {
            try
            {
                List<CXN_NOTICIAS> lista = oNoticias.GetAllNoticias();
                if (lista != null)
                {
                    Encabezados();

                    foreach (CXN_NOTICIAS noticia in lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = noticia.Id;
                        row["Titulo"] = noticia.Titulo.ToString();
                        row["Resumen"] = noticia.Resumen.ToString();
                        row["Estado"] = (bool)noticia.Estado == true ? "ACTIVO" : "INACTIVO";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                    gridZH1.dataGridView1.Columns["POS"].Visible = false;
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se encontraron noticias",
                        TipoImagen = 0
                    };

                    MG.ShowDialog();

                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            TituloN = dt.Columns.Add("Titulo", typeof(string));
            Resumen = dt.Columns.Add("Resumen", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
            gridZH1.dataGridView1.DataSource = dt;
        }

        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog o = new OpenFileDialog();
                o.InitialDirectory = "C:\\";
                o.Filter = "Image Files|*.PNG;*.JPG;*.JPEG";

                try
                {
                    if (o.ShowDialog() == DialogResult.OK)
                    {
                        string imagePath = o.FileName;
                        pictureBox1.Image = Image.FromFile(imagePath);
                        bytes = File.ReadAllBytes(imagePath);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox1.Text))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar un titulo y un resumen",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else if (bytes == null)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe seleccionar una imagen",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    CXN_NOTICIAS N = new CXN_NOTICIAS
                    {
                        Estado = true,
                        Fecha = DateTime.Now.Date,
                        Resumen = textBox2.Text,
                        Titulo = textBox1.Text,
                        URL = Convert.ToBase64String(bytes)
                    };

                    bool graba = oNoticias.CreaNoticia(N);
                    if (graba == true)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Noticia creada con exito",
                            TipoImagen = 3
                        };
                        MG.ShowDialog();

                        CargarNoticias();

                        textBox1.Text = "";
                        textBox2.Text = "";
                        bytes = null;
                        pictureBox1.Image.Dispose();
                        pictureBox1.Image = null;
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se logro crear la noticia",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
