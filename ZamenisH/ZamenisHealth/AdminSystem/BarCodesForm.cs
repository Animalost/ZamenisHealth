using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZXing;
using ZXing.Common;

namespace ZamenisHealth.AdminSystem
{
    public partial class BarCodesForm : Forma
    {
        private IBarCodes repoBar;
        private IGenerales repoGen;

        DataTable dt;
        DataColumn Id;
        DataColumn CodInterno;
        DataColumn CodBar;

        private MensajesGeneral MG;

        public BarCodesForm()
        {
            InitializeComponent();
            repoBar = new MBarCodes();
            repoGen = new MGenerales();
        }

        private void BarCodesForm_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Codigos de Barras - Cargos";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;
            gridZH1.dataGridView1.CellClick += gridZH1_CellClick;
            CargarGrilla();

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += Grabar_Click;
        }
        void Grabar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe digitar codigo interno y codigo de barras",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    CXN_BARCODES C = new CXN_BARCODES
                    {
                        Bar_CodeBar = textBox2.Text.Trim(),
                        Bar_Origin = textBox1.Text.Trim()
                    };

                    CXN_BARCODES getProd = repoBar.GetCode(textBox1.Text.Trim(), textBox2.Text.Trim());
                    if (getProd != null) 
                    {
                        /*bool save = repoBar.Update(C);
                        if (save == true)
                        {
                            CargarGrilla();
                            pictureBox1.Image = null;

                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Hecho",
                                TipoImagen = 3
                            };

                            MG.ShowDialog();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro actualizar",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                        }*/
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "El codigo ya existe",
                            TipoImagen = 1000
                        }; 
                        
                        MG.ShowDialog();
                    }
                    else
                    {
                        bool save = repoBar.Create(C);
                        if (save == true)
                        {
                            CargarGrilla();
                            pictureBox1.Image = null;

                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Hecho",
                                TipoImagen = 3
                            };

                            MG.ShowDialog();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro crear",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void gridZH1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Cod = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string bar = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                textBox1.Text = Cod.ToString();
                textBox2.Text = bar.ToString();

                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_39,
                    Options = new EncodingOptions
                    {
                        Height = 80,
                        Width = 300,
                        Margin = 2
                    }
                };

                Bitmap barcode = writer.Write(bar);
                byte[] data = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    barcode.Save(ms, ImageFormat.Png);
                    data = ms.ToArray();
                }

                pictureBox1.Image = repoGen.ByteToImage(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["Id"].Visible = false;
        }
        void CargarGrilla()
        {
            try
            {
                List<CXN_BARCODES> _lista = repoBar.listaBarras();
                if (_lista != null)
                {
                    Encabezados();

                    foreach (CXN_BARCODES i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row[Id] = i.Bar_Id;
                        row[CodInterno] = i.Bar_Origin.ToString();
                        row[CodBar] = i.Bar_CodeBar.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                    }

                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    gridZH1.dataGridView1.DataSource = null;
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Encabezados()
        {
            dt = new DataTable();
            Id = dt.Columns.Add("Id", typeof(int));
            CodInterno = dt.Columns.Add("Codiugo Interno", typeof(string));
            CodBar = dt.Columns.Add("Codigo de Barras", typeof(string));
        }
    }
}
