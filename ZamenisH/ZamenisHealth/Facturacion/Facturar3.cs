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
using System.Reflection;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Facturacion
{
    public partial class Facturar3 : Forma2
    {
        private IFirmasDigitales fDigitales;

        private MensajesGeneral MG;
        private List<int> listaadmisiones;
        private int CodePaciente;

        private DataTable dt;
        private DataColumn Admision;
        private DataColumn Firma;

        public Facturar3(List<int> listaadmisiones, int codePaciente)
        {
            InitializeComponent();
            this.listaadmisiones = listaadmisiones;
            CodePaciente = codePaciente;
            fDigitales = new MFirmasDigitales();
        }
        void DoubleBuffered(DataGridView dgv, bool setting)
        {
            typeof(DataGridView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(dgv, setting, null);
        }
        private void Facturar3_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Firmas";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            //gridZH1.CeldaHeight = true;
            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;
            gridZH1.dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;

            gridZH1.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gridZH1.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            Recarga();
        }
        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            gridZH1.dataGridView1.RowTemplate.Height = 100;
            gridZH1.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gridZH1.dataGridView1.Columns["Admision"].Width = 200;

            DataGridViewImageColumn imgCol = (DataGridViewImageColumn)gridZH1.dataGridView1.Columns["Firma"];
            imgCol.ImageLayout = DataGridViewImageCellLayout.Stretch;
            imgCol.Width = 120;

            DoubleBuffered(gridZH1.dataGridView1, true);
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Adm = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                FirmaDigital firmaDigital = new FirmaDigital(Adm, CodePaciente, "Admisiones");
                firmaDigital.ShowDialog();

                Recarga();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DefaultCellStyle.NullValue = null;
            gridZH1.dataGridView1.DataSource = null;            

            dt = new DataTable();
            dt.Columns.Add("Admision", typeof(int));
            dt.Columns.Add("Firma", typeof(Image));
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            Recarga();
        }
        void Recarga()
        {
            try
            {                
                if (listaadmisiones != null)
                {
                    Encabezados();                                        

                    foreach (var i in listaadmisiones)
                    {
                        CXN_FIRMASDIGITALES fTemp = fDigitales.getFirmas(i);
                        Image firmaImg = null;

                        byte[] bytes = null;

                        if (fTemp == null)
                        {
                            bytes = Convert.FromBase64String(fDigitales.ImageNull());
                        }
                        else
                        {
                            bytes = fTemp.Firma;
                        }

                        using (MemoryStream ms = new MemoryStream(bytes))
                        using (var original = Image.FromStream(ms))
                        {
                            Bitmap bmp = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.Clear(Color.White); // 🔥 importante (evita negro)
                                g.DrawImage(original, 0, 0, original.Width, original.Height);
                            }

                            firmaImg = bmp;
                        }

                        DataRow row = dt.NewRow();
                        row["Admision"] = i;
                        row["Firma"] = firmaImg;

                        dt.Rows.Add(row);
                    }

                    gridZH1.dataGridView1.DataSource = dt;         
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay listas para facturar",
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
