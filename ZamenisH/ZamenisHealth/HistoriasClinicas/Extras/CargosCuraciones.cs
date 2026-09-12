using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class CargosCuraciones : ConfigForm.BaseForm
    {
        private static readonly IInventario repoInventario = new MInventario();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly ICargos repoCargos = new MCargos();

        private int Admision, Aseguradora, Compañia, Profesional, Paciente;
        private DateTime FechaAdmision;
        private string TipoServicio;

        private MensajesGeneral MG;

        DataTable dt;
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Cantidad;
        DataColumn Detalle;
        DataColumn VrUnitario;
        DataColumn CodigoEPS;

        public CargosCuraciones(int admision)
        {
            InitializeComponent();
            this.Admision = admision;
        }

        private void CargosCuraciones_Load(object sender, EventArgs e)
        {
            this.Titulo.Text = "Cargos de curaciones";

            otrosDatosPacienteHorario DatosAdmision = repoAgendaMedicaConsultas.cargarAdmision(Admision, "'P','H','A'");
            if (DatosAdmision == null)
            {
                MessageBox.Show("Error en esta admision",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Dispose();
                this.Close();
                return;
            }

            if (repoCargos.cargoExiste(Admision) == true)
            {
                MessageBox.Show("A Esta admision ya le grabo cargos, si desea corregirlos, comuniquese con la administracion",
                  "Error",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Error);
                this.Dispose();
                this.Close();
                return;
            }

            Aseguradora = DatosAdmision.Hor_Pac_Ase;
            Compañia = DatosAdmision.Hor_Pac_Cia;
            Profesional = DatosAdmision.Hor_Pac_Bod;
            Paciente = DatosAdmision.Hor_Pac_Id;
            FechaAdmision = Convert.ToDateTime(DatosAdmision.Hor_Pac_Fecha_Cita);
            TipoServicio = DatosAdmision.Hor_Pac_Tipo_Serv;

            comboBox1.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea grabar estos cargos para esta nota?",
                                                 "Zamenis Health - Grabacion de Cargos",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Grabar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        async void Grabar()
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow || row.Cells.Cast<DataGridViewCell>().All(c => c.Value == null || string.IsNullOrWhiteSpace(c.Value.ToString())))
                    {
                        continue; // Salta a la siguiente fila si la actual está vacía
                    }

                    string codigo = row.Cells["Codigo"]?.Value?.ToString();
                    string item = row.Cells["Item"]?.Value?.ToString();
                    string cantidad = row.Cells["Cantidad"]?.Value?.ToString();
                    string unitario = row.Cells["VrUnitario"]?.Value?.ToString();
                    string detalle = row.Cells["Detalle"]?.Value?.ToString();
                    string codigoeps = row.Cells["CodigoEPS"]?.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(codigo) ||
                        string.IsNullOrWhiteSpace(item) ||
                        string.IsNullOrWhiteSpace(cantidad) ||
                        string.IsNullOrWhiteSpace(unitario) || 
                        string.IsNullOrWhiteSpace(codigoeps))
                    {
                        // Si alguna está vacía, omitimos esta fila y pasamos a la siguiente
                        continue;
                    }

                    if (codigo == null || item == null || cantidad == null || unitario == null || codigoeps == null)
                    {
                        TXTException T = new TXTException
                        {
                            FechaHora = DateTime.Now,
                            Error = "No hay datos para grabar del cargo: " + Admision.ToString() + " --> NO GRABADO " + codigo,
                            Formulario = this.Name,
                            Metodo = OverridesExtern.GetCurrentMethodName(),
                            Usuario = Contenedor.UsuarioLogueado
                        };

                        OverridesExtern.GenerarTXTException(T);
                    }
                    else
                    {
                        CXN_CARGOS C = new CXN_CARGOS
                        {
                            Car_Adm_Id = Convert.ToInt32(Admision),
                            Car_Pac = Paciente,
                            Car_Cia = Compañia,
                            Car_Ase = Aseguradora,
                            Car_Prof = Profesional,
                            Car_Fecha = Convert.ToDateTime(FechaAdmision),
                            Car_Estado = "G",
                            Car_Tipo = "Cargo",
                            Car_Cod = codigo.ToString().Trim(),
                            Car_Cant = Convert.ToInt32(cantidad),
                            Car_Val_Un = Convert.ToInt32(unitario),
                            Car_Val_Tot = Convert.ToInt32(unitario) * Convert.ToInt32(cantidad),
                            Car_Item = item.Trim(),
                            Car_Detalle = detalle.Trim(),
                            Car_Usr_Graba = Contenedor.UsuarioLogueado,
                            Car_Tipo_Serv = TipoServicio.Trim(),
                            CarCodEPSConvenio = codigoeps.ToString().Trim()
                        };

                        await repoCargos.SaveCargo(C);
                    }
                }

                MG = new MensajesGeneral
                {
                    TipoImagen = 3,
                    Mensaje = "Cargos grabados correctamente"
                };
                MG.ShowDialog();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Encabezados()
        {
            try
            {
                dataGridView1.DataSource = null;
                dt = new DataTable();
                POS = dt.Columns.Add("POS", typeof(int));
                Codigo = dt.Columns.Add("Codigo", typeof(string));
                Item = dt.Columns.Add("Item", typeof(string));
                Cantidad = dt.Columns.Add("Cantidad", typeof(int));
                Detalle = dt.Columns.Add("Detalle", typeof(string));
                VrUnitario = dt.Columns.Add("VrUnitario", typeof(int));
                CodigoEPS = dt.Columns.Add("CodigoEPS", typeof(string));
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string TipoBusqueda = "";

                if (comboBox1.Text == "Apositos")
                {
                    TipoBusqueda = "Aposito";
                }
                if (comboBox1.Text == "Insumos")
                {
                    TipoBusqueda = "Insumo";
                }

                List<CXN_INVENTARIO> getProducts = repoInventario.getAllProductsByType(Aseguradora, TipoBusqueda);
                if (getProducts == null)
                {
                    Encabezados();
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 0;
                    MG.Mensaje = "No hay productos disponibles para el tipo seleccionado";
                    MG.ShowDialog();
                    return;
                }

                Encabezados();
                int Contador = 1;

                foreach (CXN_INVENTARIO item in getProducts)
                {
                    DataRow dr = dt.NewRow();
                    dr[POS] = Contador;
                    dr[Codigo] = item.InvCod;
                    dr[Item] = item.InvItem;

                    dr[Detalle] = item.InvDetalle;
                    dr[VrUnitario] = Convert.ToInt32(item.InvPrecio);
                    dr[CodigoEPS] = Convert.ToInt32(item.InvImagen);

                    dt.Rows.Add(dr);
                    dt.AcceptChanges();

                    Contador = Contador + 1;
                }

                Contador = 1;
                Estilos(dataGridView1, dt);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.ReadOnly = false;

            D.Columns["Codigo"].ReadOnly = true;
            D.Columns["Item"].ReadOnly = true;
            D.Columns["POS"].ReadOnly = true;
            D.Columns["Detalle"].ReadOnly = true;
            D.Columns["VrUnitario"].ReadOnly = true;

            D.Columns["Codigo"].Width = 80;
            D.Columns["Item"].Width = 460;
            D.Columns["Cantidad"].Width = 80;

            D.Font = new Font("Arial", 8, FontStyle.Bold);

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Item"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Codigo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Item"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Cantidad"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;
            D.Columns["Detalle"].Visible = false;
            D.Columns["VrUnitario"].Visible = false;
            D.Columns["CodigoEPS"].Visible = false;            

            foreach (DataGridViewRow row in D.Rows)
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
    }
}
