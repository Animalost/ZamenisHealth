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

namespace ZamenisHealth.Medicina
{
    public partial class Historial_Medico_1 : Forma
    {
        private static readonly IPacientes repoPac = new MPacientes();

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn IdPaciente;
        DataColumn Paciente;

        public Historial_Medico_1()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Buscar();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        private void Historial_Medico_1_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Registros Medicos";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnBuscar = new ToolStripButton();
                btnBuscar = createToolButton("Buscar IPS (F5)");
                MenuLateral.Items.Add(btnBuscar);
                btnBuscar.Click += btnZamenis1_ButtonClick;

                ToolStripButton btnIHCE = new ToolStripButton();
                btnIHCE = createToolButton("Buscar IHCE");
                MenuLateral.Items.Add(btnIHCE);
                btnIHCE.Click += button2_Click;
                btnIHCE.Visible = false;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "HistorialMedico";
            buscarPacientes.ShowDialog();
        }
        public void setDoc(string TID, string NID)
        {
            textBox1.Text = NID;
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            IdPaciente = dt.Columns.Add("IdPaciente", typeof(int));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
        }
        void Buscar()
        {
            try
            {
                CXN_PACIENTES getPac = repoPac.LlamarPacienteNumDoc(textBox1.Text);
                if (getPac != null)
                {
                    List<CXN_PACIENTES> lista = new List<CXN_PACIENTES>();

                    lista.Add(new CXN_PACIENTES
                    {
                        Pac_Id = getPac.Pac_Id,
                        Pac_PrimerN = getPac.Pac_PrimerA + " " +
                                      getPac.Pac_SegundoA + " " +
                                      getPac.Pac_PrimerN + " " +
                                      getPac.Pac_SegundoN
                    });

                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_PACIENTES i in lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["IdPaciente"] = i.Pac_Id.ToString();
                        row["Paciente"] = i.Pac_PrimerN.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
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
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Paciente"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;
            D.Columns["IdPaciente"].Visible = false;

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

            D.ClearSelection();
        }
        private void Historial_Medico_1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F5)
                {
                    Buscar();
                }
                if (e.KeyData == Keys.Escape)
                {
                    this.Dispose();
                    this.Close();
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
                Historial_Medico_2 m = new Historial_Medico_2(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
                m.ShowDialog();
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            FrontFHIR.VisorZamenis.VerRDA ass = new FrontFHIR.VisorZamenis.VerRDA();
            ass.ShowDialog();
        }
    }
}
