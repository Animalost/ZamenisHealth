using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class Facturar6Otros : Forma
    {
        private static readonly ICargos repoCargos = new MCargos();

        int Adm_Selected = 0;
        int Pos_Selected = 0;
        private DateTime Desde;
        private DateTime Hasta;
        private int Ase, IdPac;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Tipo;
        DataColumn Cargo;
        DataColumn Servicio;
        DataColumn Fecha;
        DataColumn Vr_Unitario;
        DataColumn Vr_Total;
        DataColumn Cantidad;
        DataColumn Estado;
        DataColumn Prestador;
        DataColumn Aseguradoras;
        DataColumn Id;

        public Facturar6Otros(DateTime desde, DateTime hasta, int ase, int idpac)
        {
            InitializeComponent();
            Desde = desde;
            Hasta = hasta;
            Ase = ase;
            IdPac = idpac;            
        }
        public Facturar6Otros(string _admition)
        {
            InitializeComponent();
            textBox1.Text = _admition.ToString();
        }

        private void Facturar6Otros_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Facturacion";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGestionar = new ToolStripButton();
            btnGestionar = createToolButton("Gestionar");
            MenuLateral.Items.Add(btnGestionar);
            btnGestionar.Click += button1_Click;

            if (Ase > 0 && IdPac > 0)
            {
                ToolStripButton btnCup = new ToolStripButton();
                btnCup = createToolButton("CUP Masivo");
                MenuLateral.Items.Add(btnCup);
                btnCup.Click += button2_Click;
            }

            dataGridView1.MouseWheel += new MouseEventHandler(dataGridView1_MouseWheel);          
        }
        private void button2_Click(object sender, EventArgs e)
        {
            EditarCupMasivo editarCupMasivo = new EditarCupMasivo(Desde, Hasta, Ase, IdPac);
            editarCupMasivo.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Buscar();
        }
        private void Encabezados()
        {
            try
            {
                dt = new DataTable();
                POS = dt.Columns.Add("POS", typeof(int));
                Id = dt.Columns.Add("Id", typeof(int));
                Cargo = dt.Columns.Add("Cargo", typeof(string));
                Servicio = dt.Columns.Add("Servicio", typeof(string));
                Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
                Vr_Unitario = dt.Columns.Add("Vr_Unitario", typeof(int));
                Cantidad = dt.Columns.Add("Cantidad", typeof(int));
                Vr_Total = dt.Columns.Add("Vr_Total", typeof(int));
                Aseguradoras = dt.Columns.Add("Aseguradora", typeof(string));
                Estado = dt.Columns.Add("Estado", typeof(string));
                Prestador = dt.Columns.Add("Prestador", typeof(string));
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Buscar()
        {
            try
            {
                var carTotal = repoCargos.BuscarCargoTotal(Convert.ToInt32(textBox1.Text));
                if (carTotal != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var i in carTotal)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Car_Id;
                        row["Cargo"] = i.Car_Adm_Id;
                        row["Servicio"] = i.Car_Item.ToString().Trim();
                        row["Fecha"] = Convert.ToDateTime(i.Car_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Vr_Unitario"] = i.Car_Val_Un;
                        row["Cantidad"] = i.Car_Cant;
                        row["Vr_Total"] = i.Car_Val_Tot;
                        row["Aseguradora"] = i.Car_Tipo_Doc.ToString();
                        row["Estado"] = i.Car_Estado.ToString().Trim();
                        row["Prestador"] = i.Car_Detalle.ToString();

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
                    MessageBox.Show("Este cargo no existe", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Cargo"].Width = 100;
            dataGridView1.Columns["Servicio"].Width = 350;
            dataGridView1.Columns["Fecha"].Width = 100;
            dataGridView1.Columns["Vr_Unitario"].Width = 100;
            dataGridView1.Columns["Cantidad"].Width = 100;
            dataGridView1.Columns["Vr_Total"].Width = 100;
            dataGridView1.Columns["Aseguradora"].Width = 350;
            dataGridView1.Columns["Estado"].Width = 90;
            dataGridView1.Columns["Prestador"].Width = 350;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Cargo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Servicio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Vr_Unitario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Vr_Total"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Aseguradora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Prestador"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["POS"].Visible = false;
            dataGridView1.Columns["Id"].Visible = false;

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
        void dataGridView1_MouseWheel(object sender, MouseEventArgs e)
        {
            int currentIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex;
            int scrollLines = SystemInformation.MouseWheelScrollLines;

            if (e.Delta > 0)
            {
                this.dataGridView1.FirstDisplayedScrollingRowIndex
                    = Math.Max(0, currentIndex - scrollLines);
            }
            else if (e.Delta < 0)
            {
                this.dataGridView1.FirstDisplayedScrollingRowIndex
                    = currentIndex + scrollLines;
            }
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                dataGridView1.ClearSelection();

                if (e.Button == MouseButtons.Right)
                {                    
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    ContextMenuStrip menu = new ContextMenuStrip();
                    menu.Font = new Font("Arial", 12);
                    menu.Padding = new Padding(5, 5, 5, 30);

                    menu.Items.Add("Compañia", Properties.Resources2.MayorQue_Black).Name = "Compañia";
                    menu.Items["Compañia"].Click += Compañia;

                    menu.Items.Add("Aseguradora", Properties.Resources2.MayorQue_Black).Name = "Aseguradora";
                    menu.Items["Aseguradora"].Click += Aseguradora;

                    menu.Items.Add("Fechas", Properties.Resources2.MayorQue_Black).Name = "Fechas";
                    menu.Items["Fechas"].Click += Fechas;

                    menu.Items.Add("Estados", Properties.Resources2.MayorQue_Black).Name = "Estados";
                    menu.Items["Estados"].Click += Estados;

                    menu.Items.Add("Cups", Properties.Resources2.MayorQue_Black).Name = "Cups";
                    menu.Items["Cups"].Click += Cups;

                    Adm_Selected = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                    Pos_Selected = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());

                    Rectangle coordenada = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                    int anchoCelda = coordenada.Location.X - 100;
                    int altoCelda = coordenada.Location.Y;

                    int X = anchoCelda + dataGridView1.Location.X;
                    int Y = altoCelda + dataGridView1.Location.Y;

                    menu.Show(dataGridView1, new System.Drawing.Point(X, Y));
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Cups(object sender, EventArgs e)
        {
            try
            {
                Extras.Cup C = new Extras.Cup(Adm_Selected, Pos_Selected);
                C.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estados(object sender, EventArgs e)
        {
            try
            {
                Extras.Estado E = new Extras.Estado(Adm_Selected, Pos_Selected);
                E.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Fechas(object sender, EventArgs e)
        {
            try
            {
                Extras.Fechas F = new Extras.Fechas(Adm_Selected, Pos_Selected);
                F.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Compañia(object sender, EventArgs e)
        {
            try
            {
                Extras.Compañia C = new Extras.Compañia(Adm_Selected, Pos_Selected);
                C.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Aseguradora(object sender, EventArgs e)
        {
            try
            {
                Extras.Aseguradora A = new Extras.Aseguradora(Adm_Selected, Pos_Selected);
                A.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
