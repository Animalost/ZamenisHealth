using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Facturacion.OpenInvoice;

namespace ZamenisHealth.Facturacion
{
    public partial class FacturaAbierta2 : Forma2
    {
        private readonly IInventario repoInv = new MInventario();
        private readonly IConvenios repoCon = new MConvenios();

        private int Ase;

        private MensajesGeneral MG;
        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Descripcion;
        DataColumn Valor;
        DataColumn Tipo;
        DataColumn CTipo;
        DataColumn InvDetalle;

        public FacturaAbierta2(int _ase)
        {
            InitializeComponent();
            this.Ase = _ase;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) //Material
            {
                CargaMaterial();
            }
            if (comboBox1.SelectedIndex == 1) //Servicio
            {
                CargaServicios();
            }
        }
        private void FacturaAbierta2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Facturacion Abierta";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;

            Encabezados();
        }
        void CargaServicios()
        {
            try
            {
                List<CXN_CONVENIOS> getP = repoCon.getConvenios(Ase);
                if (getP != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_CONVENIOS h in getP)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Codigo"] = h.Con_Id_Serv.ToString();
                        row["Descripcion"] = h.Con_Nombre.ToString();
                        row["Valor"] = Convert.ToInt32(h.Con_Valor.ToString());
                        row["Tipo"] = h.Con_Tipo_Serv;
                        row["CTipo"] = (h.Con_Clase == "QX" ? "Nota" : "Historia");
                        row["InvDetalle"] = "N/A";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }        
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                CXN_CARGOS Car = new CXN_CARGOS()
                {
                    Car_Val_Un = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()),
                    Car_Cant = 1,
                    Car_Cod = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(),
                    Car_Item = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString(),
                    Car_Tipo_Serv = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString(),
                    Car_Tipo = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString(),
                    Car_Detalle = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString(),
                    Car_Ase = Ase
                };

                Cantidades C = new Cantidades(Car);
                C.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }           
        }
        void Estilos()
        {
            gridZH1.dataGridView1.DataSource = dt;

            gridZH1.dataGridView1.Columns["POS"].Visible = false;
            gridZH1.dataGridView1.Columns["Tipo"].Visible = false;
            gridZH1.dataGridView1.Columns["CTipo"].Visible = false;
            gridZH1.dataGridView1.Columns["InvDetalle"].Visible = false;
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Descripcion = dt.Columns.Add("Descripcion", typeof(string));
            Valor = dt.Columns.Add("Valor", typeof(int));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
            CTipo = dt.Columns.Add("CTipo", typeof(string));
            InvDetalle = dt.Columns.Add("InvDetalle", typeof(string));
        }
        void CargaMaterial()
        {
            try
            {
                List<CXN_INVENTARIO> getP = repoInv.getAllProducts(Ase);
                if (getP != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_INVENTARIO h in getP)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Codigo"] = h.InvCod.ToString();
                        row["Descripcion"] = h.InvItem.ToString();
                        row["Valor"] = Convert.ToInt32(h.InvPrecio.ToString());
                        row["Tipo"] = h.InvTipo;
                        row["CTipo"] = "Cargo";
                        row["InvDetalle"] = h.InvDetalle;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
