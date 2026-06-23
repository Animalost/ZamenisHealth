using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using ZamenisHealth.Clases.Controles;

namespace ZamenisHealth.AdminSystem.Prestadores
{
    public partial class Prestador : Forma
    {
        private ICompañia oCompañia;
        private DataTable dt;

        public Prestador()
        {
            InitializeComponent();
            oCompañia = new MCompañia();
        }

        private void Prestador_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Prestadores";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;
            gridZH1.CeldaHeight = true;

            CargarPrestadores();

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Crear");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button1_Click;
        }
        void button1_Click(object sender, EventArgs e)
        {
            Compañias C = new Compañias(0);
            C.ShowDialog();
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Cod = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                
                Compañias C = new Compañias(Cod);
                C.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            DataColumn Identificador = dt.Columns.Add("Identificador", typeof(int));
            DataColumn Compañia = dt.Columns.Add("Compañia", typeof(string));
            DataColumn Nit = dt.Columns.Add("Nit", typeof(string));
        }
        void CargarPrestadores()
        {
            try
            {
                List<CXN_CIA> getPrestadores = oCompañia.getAllCompañias();
                if (getPrestadores != null)
                {
                    Encabezados();

                    foreach (var i in getPrestadores)
                    {
                        DataRow row = dt.NewRow();

                        row["Identificador"] = i.Com_Identificador;
                        row["Compañia"] = i.Com_Nombre.ToString();
                        row["Nit"] = i.Com_Identificacion.ToString() + " - " + i.Com_DVerifica.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
    }
}
