using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using Color = System.Drawing.Color;

namespace ZamenisHealth.AdminSystem
{
    public partial class CyT : Forma
    {
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly IConvenios repoConvenios = new MConvenios();

        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();

        DataColumn POS;
        DataColumn Identificador;
        DataColumn Descripcion;

        DataColumn Posision;
        DataColumn Aseguradora;
        DataColumn Valor;
        DataColumn AseCode;

        public CyT()
        {
            InitializeComponent();
        }

        void EncabezadoLv1()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Identificador = dt.Columns.Add("Identificador", typeof(string));
            Descripcion = dt.Columns.Add("Descripcion", typeof(string));
        }

        void EncabezadoLv2()
        {
            dt2 = new DataTable();
            POS = dt2.Columns.Add("POS", typeof(int));
            Posision = dt2.Columns.Add("Posision", typeof(int)); //
            Identificador = dt2.Columns.Add("Identificador", typeof(string));
            Descripcion = dt2.Columns.Add("Descripcion", typeof(string));
            Aseguradora = dt2.Columns.Add("Aseguradora", typeof(string));
            Valor = dt2.Columns.Add("Valor", typeof(string));
            AseCode = dt2.Columns.Add("AseCode", typeof(string)); //
        }

        private void CyT_Load(object sender, EventArgs e)
        {
            try
            {
                LogoMain.Image = Properties.Resources.Splash;

                Titulo.Text = "Convenios y Tarifas";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                ToolStripButton btnNewConvenio = new ToolStripButton();
                btnNewConvenio = createToolButton("Nuevo Convenio");
                MenuLateral.Items.Add(btnNewConvenio);
                btnNewConvenio.Click += toolStripButton1_Click;

                ToolStripButton btnNewAseguradora = new ToolStripButton();
                btnNewAseguradora = createToolButton("Nueva Aseguradora");
                MenuLateral.Items.Add(btnNewAseguradora);
                btnNewAseguradora.Click += toolStripButton3_Click;

                dataGridView1.Size = new Size(791, 529);
                dataGridView2.Size = new Size(791, 529);
                dataGridView1.Location = new Point(151, 126);
                dataGridView2.Location = new Point(151, 126);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        void Estilos()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Identificador"].Width = 150;
            dataGridView1.Columns["Descripcion"].Width = 500;
            dataGridView1.Font = new Font("Arial", 11);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Identificador"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Descripcion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["Identificador"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Descripcion"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.Columns["POS"].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                }
            }
        }

        void Estilos2()
        {
            dataGridView2.EnableHeadersVisualStyles = false;
            dataGridView2.ScrollBars = ScrollBars.Both;

            dataGridView2.DataSource = dt2;

            dataGridView2.Columns["Identificador"].Width = 100;
            dataGridView2.Columns["Descripcion"].Width = 350;
            dataGridView2.Columns["Aseguradora"].Width = 350;
            dataGridView2.Columns["Valor"].Width = 150;
            dataGridView2.Font = new Font("Arial", 11);

            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView2.Font, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView2.Columns["Posision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["Identificador"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["Descripcion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["Aseguradora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["Valor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns["Posision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns["Identificador"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns["Descripcion"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns["Aseguradora"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns["Valor"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView2.Columns["POS"].Visible = false;
            dataGridView2.Columns["Posision"].Visible = false;
            dataGridView2.Columns["AseCode"].Visible = false;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                }
            }
        }    

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CyT2 cyT2 = new CyT2(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
            cyT2.ShowDialog();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CyT3 cyT3 = new CyT3(Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString()));
            cyT3.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        public void LoadAse()
        {
            try
            {
                if (comboBox3.Text == "Aseguradoras")
                {
                    dataGridView1.Visible = true;
                    dataGridView2.Visible = false;

                    var getAseguradoras = repoAse.getAseguradoras();
                    if (getAseguradoras != null)
                    {
                        EncabezadoLv1();

                        int Contador = 1;

                        foreach (CXN_ASEGURADORA h in getAseguradoras)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Identificador"] = h.Ase_Identificador.ToString();
                            row["Descripcion"] = h.Ase_Descripcion.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();
                    }
                    else
                    {
                        EncabezadoLv1();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text == "Convenios")
                {
                    dataGridView1.Visible = false;
                    dataGridView2.Visible = true;

                    var getConvenios = repoConvenios.getConvenios();
                    if (getConvenios != null)
                    {
                        EncabezadoLv2();

                        int Contador = 1;

                        foreach (CXN_CONVENIOS h in getConvenios)
                        {
                            DataRow row = dt2.NewRow();

                            row["POS"] = Contador;
                            row["Posision"] = h.Con_Id.ToString();
                            row["Identificador"] = h.Con_Id_Serv.ToString();
                            row["Descripcion"] = h.Con_Nombre.ToString();
                            row["Aseguradora"] = h.Con_Clase.ToString();
                            row["Valor"] = h.Con_Valor.ToString("N0");
                            row["AseCode"] = h.Con_Aseguradora.ToString();

                            dt2.Rows.Add(row);
                            dt2.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos2();
                    }
                    else
                    {
                        EncabezadoLv2();
                    }
                }   
                
                if (comboBox3.Text == "Aseguradoras")
                {
                    LoadAse();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CyT2 cyT2 = new CyT2(0);
            cyT2.ShowDialog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CyT3 cyT3 = new CyT3(0);
            cyT3.ShowDialog();
        }
    }
}
