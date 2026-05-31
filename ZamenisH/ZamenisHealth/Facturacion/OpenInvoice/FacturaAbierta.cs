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
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class FacturaAbierta : Forma
    {
        private readonly ICompañia repoCia = new MCompañia();
        private readonly IAseguradoras repoAse = new MAseguradoras();
        private readonly IPacientes repoPac = new MPacientes();
        private readonly IRoles repoRoles = new MRoles();

        private MensajesGeneral MG;
        private int IdPac, IdAse, IdCia;
        private string Regimen;
        private List<CXN_CARGOS> listaCargos = new List<CXN_CARGOS>();

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Cantidad;
        DataColumn VrUnitario;
        DataColumn VrTotal;

        private ToolStripButton btnBuscarPac;
        private ToolStripButton btnCreatePac;
        private ToolStripButton btnAddCargo;
        private ToolStripButton btnFacturar;

        public FacturaAbierta()
        {
            InitializeComponent();       
        }

        public void setDataToFactura(CXN_CARGOS C)
        {
            listaCargos.Add(new CXN_CARGOS {
                Car_Adm_Id = 0, //Primer Form
                Car_Pac = IdPac,
                Car_Cia = IdCia,
                Car_Ase = C.Car_Ase,
                Car_Prof = C.Car_Prof,
                Car_Fecha = C.Car_Fecha,
                Car_Estado = C.Car_Estado,
                Car_Tipo = C.Car_Tipo,
                Car_Cod = C.Car_Cod,
                Car_Tipo_Serv = C.Car_Tipo_Serv,
                Car_Cant = C.Car_Cant,
                Car_Val_Un = C.Car_Val_Un,
                Car_Val_Tot = C.Car_Val_Tot,
                Car_Item = C.Car_Item,
                Car_Detalle = C.Car_Detalle,
                Car_Dx1 = C.Car_Dx1,
                Car_Dx2 = C.Car_Dx2,
                Car_Dx3 = C.Car_Dx3,
                Car_Regimen = Regimen,
                Car_Ambito = 0,
                Car_Finalidad = 0,
                Car_Personal = 0,
                Car_CExterna = 0,
                Car_Finalidad_CO = 0,
                Car_Imp_Dx = 0
            });

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();

            Encabezados();

            int contador = 1;
            int Total = 0;

            foreach (var i in listaCargos)
            {
                DataRow row = dt.NewRow();
                row["POS"] = contador;
                row["Codigo"] = i.Car_Cod;
                row["Item"] = i.Car_Item;
                row["Cantidad"] = i.Car_Cant;
                row["VrUnitario"] = Convert.ToInt32(i.Car_Val_Un);
                row["VrTotal"] = Convert.ToInt32(i.Car_Val_Tot);

                dt.Rows.Add(row);
                dt.AcceptChanges();

                dataGridView1.DataSource = dt;

                contador++;

                Total = Total + Convert.ToInt32(i.Car_Val_Tot);
            }

            label7.Text = "$ " + Convert.ToInt32(Total).ToString("N0");

            Estilos();
        }
        void Estilos()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;
            dataGridView1.Font = new Font("Arial", 11);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Item"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["VrUnitario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["VrTotal"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["Codigo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Item"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Cantidad"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["VrUnitario"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["VrTotal"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

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

            dataGridView1.ClearSelection();
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Item = dt.Columns.Add("Item", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
            VrUnitario = dt.Columns.Add("VrUnitario", typeof(int));
            VrTotal = dt.Columns.Add("VrTotal", typeof(int));
        }
        void CargarAse()
        {
            List<CXN_ASEGURADORA> getAse = repoAse.getAseguradoras();
            if (getAse != null)
            {
                foreach (CXN_ASEGURADORA a in getAse)
                {
                    comboBox2.Items.Add(a.Ase_Descripcion);
                }

                comboBox2.SelectedIndex = 0;
            }
        }
        void CargarCia()
        {
            List<CXN_CIA> getCia = repoCia.getAllCompañias();
            if (getCia != null)
            {
                foreach (CXN_CIA a in getCia)
                {
                    comboBox1.Items.Add(a.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdCia = repoCia.getPrestadorbyName(comboBox1.Text).Com_Identificador;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IdAse = repoAse.getInfoFromAsebyName(comboBox2.Text).Ase_Identificador;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            FacturaAbierta2 facturaAbierta2 = new FacturaAbierta2(this.IdAse);
            facturaAbierta2.ShowDialog();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (label7.Text == "$ 0")
            {
                MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "No hay datos para facturar";
                MG.ShowDialog();
            }
            else
            {
                if (listaCargos.Count > 0)
                {
                    FacturaAbierta3 facturaAbierta3 = new FacturaAbierta3(this.listaCargos);
                    facturaAbierta3.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay datos para facturar";
                    MG.ShowDialog();
                }
            }            
        }
        public void setSelection(string Doc)
        {
            textBox1.Text = Doc.ToString();
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "FacturacionAbierta";
            buscarPacientes.ShowDialog();
        }
        private void FacturaAbierta_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Facturacion Abierta";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            btnBuscarPac = new ToolStripButton();
            btnBuscarPac = createToolButton("Buscar Paciente");
            MenuLateral.Items.Add(btnBuscarPac);
            btnBuscarPac.Click += button1_Click;

            btnCreatePac = new ToolStripButton();
            btnCreatePac = createToolButton("Crear Paciente");
            MenuLateral.Items.Add(btnCreatePac);
            btnCreatePac.Click += button4_Click;

            btnAddCargo = new ToolStripButton();
            btnAddCargo = createToolButton("Agregar Cargos");
            MenuLateral.Items.Add(btnAddCargo);
            btnAddCargo.Click += button2_Click;

            btnFacturar = new ToolStripButton();
            btnFacturar = createToolButton("Terminar Factura");
            MenuLateral.Items.Add(btnFacturar);
            btnFacturar.Click += button3_Click;

            CargarAse();
            CargarCia();
            ValidarPermisoPacExiste();
        }       
        private void button4_Click(object sender, EventArgs e)
        {
            Recepcion.CrearEditarPaciente crearEditarPaciente = new Recepcion.CrearEditarPaciente();
            crearEditarPaciente.ShowDialog();
        }
        void ValidarPermisoPacExiste()
        {
            try
            {
                CXN_ROLES R = repoRoles.getRoles(Contenedor.UsuarioLogueado);
                if (R == null)
                {
                    btnCreatePac.Enabled = false;
                }
                else
                {
                    btnCreatePac.Enabled = (R.Rol_R_CrearPacientes == "A" ? true : false);
                   
                }               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_PACIENTES getPac = repoPac.LlamarPacienteNumDoc(textBox1.Text);
                if (getPac != null)
                {
                    label4.Text = getPac.Pac_PrimerA + " " + getPac.Pac_SegundoA + " " + getPac.Pac_PrimerN + " " + getPac.Pac_SegundoN;
                    this.IdPac = getPac.Pac_Id;
                    this.Regimen = getPac.Pac_Regimen;
                    btnBuscarPac.Enabled = false;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    textBox1.Enabled = false;
                }
                else
                {
                    this.IdPac = 0;
                    label4.Text = "";
                    this.Regimen = "";
                    btnBuscarPac.Enabled = true;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    textBox1.Enabled = true;

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "el numero de identificacion no existe";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }            
        }
    }
}
