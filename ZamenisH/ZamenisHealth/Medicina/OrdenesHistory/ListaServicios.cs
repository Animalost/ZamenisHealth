using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Medicina.OrdenesExtra;

namespace ZamenisHealth.Medicina.OrdenesHistory
{
    public partial class ListaServicios : Forma2
    {
        private ICIE10 cIE10;
        private IConvenios convenios;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Cup;
        DataColumn Servicio;

        private OrdenServicios Forma;
        private CrearOrdenExtra2 Forma2;
        private string TipoForma;

        public ListaServicios(OrdenServicios forma)
        {
            InitializeComponent();
            cIE10 = new MCIE10();
            convenios = new MConvenios();
            Forma = forma;
            TipoForma = "OrdenServicios";
        }
        public ListaServicios(CrearOrdenExtra2 forma)
        {
            InitializeComponent();
            cIE10 = new MCIE10();
            convenios = new MConvenios();
            Forma2 = forma;
            TipoForma = "CrearOrdenExtra2";
        }

        private void ListaServicios_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Seleccione Servicio";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            gridZH1.CeldaHeight = true;

            CargarServicios("");
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Cup = dt.Columns.Add("Cup", typeof(string));
            Servicio = dt.Columns.Add("Servicio", typeof(string));
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
        }
        void CargarServicios(string Texto)
        {
            try
            {
                List<CXN_CUP> L = new List<CXN_CUP>();

                List<CXN_CONVENIOS> listaCon = convenios.getConvenios();
                if (listaCon != null)
                {
                    var sinDuplicados = listaCon.GroupBy(x => x.Con_Id_Serv).Select(g => g.First()).ToList();

                    foreach (CXN_CONVENIOS c in sinDuplicados)
                    {
                        L.Add(new CXN_CUP 
                        { 
                            CUP = c.Con_Id_Serv,
                            Servicio = c.Con_Nombre
                        });
                    }                    
                }

                List<CXN_CUP> listado = cIE10.GetLista(Texto);
                if (listado != null) 
                {
                    L.AddRange(listado);

                    Encabezados();

                    int Contador = 1;

                    foreach (var i in L)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Cup] = i.CUP.ToString();
                        row[Servicio] = i.Servicio.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(gridZH1.dataGridView1, dt);
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
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string CUP = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string SERVICIO = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                if (TipoForma == "OrdenServicios")
                {
                    Forma.textBox1.Text = CUP;
                    Forma.textBox2.Text = SERVICIO;
                }
                else if (TipoForma == "CrearOrdenExtra2")
                {
                    Forma2.textBox8.Text = CUP;
                    Forma2.textBox7.Text = SERVICIO;
                }               

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            CargarServicios(textBox2.Text);
        }
    }
}
