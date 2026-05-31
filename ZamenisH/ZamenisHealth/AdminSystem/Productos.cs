using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Productos : ConfigForm.BaseForm
    {
        private static readonly IInventario repoInventario = new MInventario();
        private static readonly IAseguradoras repoAseguradoras = new MAseguradoras();

        private MensajesGeneral MG;

        int Ase;
        int Pos;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn InvId;
        DataColumn InvCod;
        DataColumn InvItem;
        DataColumn InvDetalle;
        DataColumn InvPrecio;

        public Productos()
        {
            InitializeComponent();

            btnZamenis2.ButtonClick += btnZamenis2_ButtonClick;
            btnZamenis3.ButtonClick += btnZamenis3_ButtonClick;
            btnZamenis4.ButtonClick += btnZamenis4_ButtonClick;

            btnZamenis2.captionBtn = "Guardar";
            btnZamenis2.tooltipBtn = "Grabar estos datos";

            btnZamenis3.captionBtn = "Guardar";
            btnZamenis3.tooltipBtn = "Grabar estos datos";

            btnZamenis4.captionBtn = "Ocultar";
            btnZamenis4.tooltipBtn = "Oculta este cuadro de dialogo";
        }

        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MG = new Comunes.MensajesGeneral();

                if (textBox1.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (comboBox2.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Diligencie todos los campos"); return; }
                if (Ase == 0) { MessageBox.Show("Diligencie todos los campos"); return; }

                var consProd = repoInventario.ConsultarValor(Ase, textBox2.Text);
                if (consProd.item == "")
                {
                    DateTime Hoy = DateTime.Now;

                    CXN_INVENTARIO I = new CXN_INVENTARIO
                    {
                        InvItem = textBox1.Text,
                        InvCod = textBox2.Text,
                        InvTipo = comboBox1.Text,
                        InvCobro = comboBox2.Text,
                        InvInvima = textBox4.Text,
                        InvCodBar = textBox9.Text,
                        InvUsrGraba = Comunes.Contenedor.UsuarioLogueado,
                        InvPrecio = Convert.ToInt32(textBox3.Text),
                        InvDetalle = textBox5.Text,
                        InvConvenio = Ase,
                        InvFechaCre = Convert.ToDateTime(Hoy.ToString(Conexion.ConectionDictionary["Format_Fecha"]))
                    };

                    bool create = repoInventario.CrearProducto(I);
                    if (create != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Error interno, no se logro crear el producto";
                        MG.ShowDialog();
                    }
                    else
                    {
                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        comboBox1.Text = "";
                        comboBox2.Text = "";
                        Cargar_Grid();

                        MG.TipoImagen = 3;
                        MG.Mensaje = "Producto creado exitosamente";
                        MG.ShowDialog();
                    }
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este codigo para el convenio seleccionado ya existe";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (textBox7.Text == "") { MessageBox.Show("Diligencie los campos"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Diligencie los campos"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Diligencie los campos"); return; }
                if (textBox12.Text == "") { MessageBox.Show("Diligencie los campos"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Diligencie los campos"); return; }
                if (comboBox4.Text == "") { MessageBox.Show("Diligencie los campos"); return; }
                if (comboBox5.Text == "") { MessageBox.Show("Diligencie los campos"); return; }

                CXN_INVENTARIO I = new CXN_INVENTARIO
                {
                    InvItem = textBox7.Text,
                    InvDetalle = textBox8.Text,
                    InvInvima = textBox11.Text,
                    InvPrecio = Convert.ToInt32(textBox12.Text),
                    InvTipo = comboBox5.Text,
                    InvCodBar = textBox10.Text,
                    InvUsrGraba = Comunes.Contenedor.UsuarioLogueado,
                    InvCobro = comboBox4.Text,
                    InvId = Pos,
                };

                bool update = repoInventario.updateProducto(I);
                if (update != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error interno, no se logro actualizar el producto";
                    MG.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Actualizado");
                    groupBox1.Visible = false;
                    textBox7.Text = "";
                    textBox8.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                    comboBox4.Text = "";
                    comboBox5.Text = "";
                    Cargar_Grid();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            textBox7.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
        }

        private void Productos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Productos";
            ConfigForm.SoloNumeros(textBox12);
            ConfigForm.SoloNumeros(textBox3);

            
            var ases = repoAseguradoras.getAseguradoras();
            if (ases != null)
            {
                foreach (var i in ases)
                {
                    comboBox3.Items.Add(i.Ase_Descripcion);
                }

                comboBox3.SelectedIndex = 0;

                Cargar_Grid();
            }

            ToolTip T = new ToolTip();
            T.ShowAlways = true;
            T.SetToolTip(textBox9, "Este codigo es el habiitado a nivel internacional para los apositos e insumos y aceptados por la resolucion 2275 de 2023 del Ministerio de Salud.  Si no tiene un codigo escrib el mismo que tiene convenido con la aseguradora");
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var aseid = repoAseguradoras.getInfoFromAsebyName(comboBox3.Text);
            Ase = Convert.ToInt32(aseid.Ase_Identificador);
        }
        private void Encabezados()
        {
            try
            {
                dt = new DataTable();
                InvId = dt.Columns.Add("POS", typeof(int));
                InvId = dt.Columns.Add("InvId", typeof(int));
                InvCod = dt.Columns.Add("InvCod", typeof(string));
                InvItem = dt.Columns.Add("InvItem", typeof(string));
                InvDetalle = dt.Columns.Add("InvDetalle", typeof(string));
                InvPrecio = dt.Columns.Add("InvPrecio", typeof(int));
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Cargar_Grid()
        {
            try
            {
                var getProdAll = repoInventario.getAllProducts();
                if (getProdAll != null)
                {
                    Encabezados();
                    int Contador = 1;

                    foreach (var i in getProdAll)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["InvId"] = i.InvId.ToString();
                        row["InvCod"] = i.InvCod.ToString();
                        row["InvItem"] = i.InvItem.ToString();
                        row["InvDetalle"] = i.InvDetalle.ToString();
                        row["InvPrecio"] = Convert.ToInt32(i.InvPrecio).ToString();

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
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay productos creados aun en sistema";
                    MG.TipoImagen = 0;
                    MG.ShowDialog();
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

            dataGridView1.Columns["InvCod"].Width = 110;
            dataGridView1.Columns["InvItem"].Width = 350;
            dataGridView1.Columns["InvDetalle"].Width = 350;
            dataGridView1.Columns["InvPrecio"].Width = 115;
            dataGridView1.Font = new Font("Arial", 11);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["InvCod"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["InvItem"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["InvDetalle"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["InvPrecio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["InvCod"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["InvItem"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["InvDetalle"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["InvPrecio"].SortMode = DataGridViewColumnSortMode.NotSortable;

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                CXN_INVENTARIO getSelection = repoInventario.getProductForEdit(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
                if (getSelection != null)
                {
                    groupBox1.Visible = true;
                    Pos = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    textBox7.Text = getSelection.InvItem.ToString();
                    textBox8.Text = getSelection.InvDetalle.ToString();
                    textBox11.Text = getSelection.InvInvima.ToString();
                    textBox10.Text = getSelection.InvCodBar.ToString();
                    textBox12.Text = Convert.ToInt32(getSelection.InvPrecio).ToString();
                    comboBox4.Text = getSelection.InvCobro.ToString();
                    comboBox5.Text = getSelection.InvTipo.ToString();
                    textBox6.Text = getSelection.InvCod.ToString();
                }
                else
                {
                    groupBox1.Visible = false;
                    Pos = 0;
                    textBox7.Text = "";
                    textBox6.Text = "";
                    textBox8.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                    comboBox4.Text = "";
                    comboBox5.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
