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

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class Alergias : Forma
    {
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly IFHIR repoFHIR = new MFHIR();
        private string Tipo;
        private int IdPaciente;

        DataTable dt;
        DataColumn POS;
        DataColumn Posision;
        DataColumn Detalle;
        DataColumn Rol;
        DataColumn Habilita;
        DataColumn Grupo;

        ToolStripButton btnBuscar, btnAdd;

        public Alergias(string tipo, int idPaciente)
        {
            InitializeComponent();
            Tipo = tipo;
            IdPaciente = idPaciente;
        }

        private void Alergias_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Alergia";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                btnBuscar = new ToolStripButton();
                btnBuscar = createToolButton("Consultar");
                MenuLateral.Items.Add(btnBuscar);
                btnBuscar.Click += toolStripButton1_Click;

                btnAdd = new ToolStripButton();
                btnAdd = createToolButton("Agregar");
                MenuLateral.Items.Add(btnAdd);
                btnAdd.Click += toolStripButton3_Click;

                label1.Text = Tipo;

                CargarGrilla();
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Posision = dt.Columns.Add("Posision", typeof(int));
            Detalle = dt.Columns.Add("Detalle", typeof(string));
            Rol = dt.Columns.Add("Rol", typeof(string));
            Habilita = dt.Columns.Add("Habilita", typeof(bool));
            Grupo = dt.Columns.Add("Grupo", typeof(string));
        }
        void CargarGrilla()
        {
            try
            {
                Encabezados();

                List<CXN_CONDICIONES> H = repoCond.getCondiciones(IdPaciente, Tipo);
                if (H != null)
                {
                    int Contador = 1;

                    foreach (var h in H)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Posision"] = Convert.ToInt32(h.Id);
                        row["Detalle"] = h.Detalle.ToString();
                        row["Habilita"] = (h.Habilita == "A" ? true : false);
                        row["Grupo"] = repoFHIR.GrupoAlergias(h.CodigoFHIR);

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }
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
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Detalle"].Width = 400;
            dataGridView1.Columns["Habilita"].Width = 80;
            dataGridView1.Columns["Grupo"].Width = 80;
            dataGridView1.Font = new Font("Arial", 10);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Detalle"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Habilita"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Grupo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["Detalle"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Habilita"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Grupo"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.Columns["POS"].Visible = false;
            dataGridView1.Columns["Posision"].Visible = false;

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

            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["Habilita"].ReadOnly = false;

            dataGridView1.Cursor = Cursors.Hand;

            dataGridView1.ReadOnly = false;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Habilita"].Index && e.RowIndex >= 0)
            {
                int posision = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Posision"].Value.ToString());
                bool habilita = (bool)dataGridView1.Rows[e.RowIndex].Cells["Habilita"].Value;

                CXN_CONDICIONES c = new CXN_CONDICIONES
                {
                    Id = posision,
                    Habilita = habilita ? "A" : "N",
                    Excluye = habilita ? "" : Contenedor.UsuarioLogueado,
                    FechaExcluye = habilita ? (DateTime?)null : DateTime.Now
                };

                repoCond.updateHabilita(c);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Habilita"].Index && e.RowIndex >= 0)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    textBox1.Focus();
                    MensajesGeneral Mg = new MensajesGeneral();
                    Mg.Mensaje = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                    Mg.TipoImagen = 0;
                    Mg.ShowDialog();
                }
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("Seleccione un tipo de alergia",
                                    "Zamenis Health - Condiciones Especiales de Pacientes",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Describa la alergia",
                                    "Zamenis Health - Condiciones Especiales de Pacientes",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("¿Desea agregar esta condicion?",
                                                 "Zamenis Health - Condiciones Especiales de Pacientes",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_CONDICIONES p = new CXN_CONDICIONES
                    {
                        Condicion = label1.Text,
                        Paciente = IdPaciente,
                        Usuario = Contenedor.UsuarioLogueado,
                        Detalle = textBox1.Text,
                        Fecha = Convert.ToDateTime(DateTime.Now.Date),
                        Habilita = "A",
                        CodigoFHIR = comboBox1.Text == "Medicamento" ? "01" :
                                     comboBox1.Text == "Alimento" ? "02" :
                                     comboBox1.Text == "Sustancia del ambiente" ? "03" :
                                     comboBox1.Text == "Sustancia que entran en contacto con la piel" ? "04" :
                                     comboBox1.Text == "Picadura de insectos" ? "05" : "06"
                    };

                    repoCond.createCondiciones(p);

                    CargarGrilla();
                    textBox1.Text = "";
                    MensajesGeneral Mg = new MensajesGeneral();
                    Mg.TipoImagen = 3;
                    Mg.Mensaje = "Agregado con exito";
                    Mg.ShowDialog();
                }
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
    }
}
