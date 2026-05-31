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
using ZamenisHealth.Recepcion;

namespace ZamenisHealth.FrontFHIR
{
    public partial class Logs : Forma2
    {
        private IFHIR fHIR;
        private IAgendaC agendaC;
        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Id;
        DataColumn Admision;
        DataColumn Clase;
        DataColumn Estado;
        DataColumn Fecha;
        DataColumn Usuario;

        private int Adm_Selected;
        private string Adm_Estado;

        public Logs()
        {
            InitializeComponent();
            fHIR = new MFHIR();
            agendaC = new MAgendaC();
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Logs FHIR - MinSalud - Zamenis Health";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarFiltros();
        }
        void CargarFiltros()
        {
            foreach (var item in Años())
            {
                comboBox1.Items.Add(item.ToString());
            }

            foreach (var item2 in Meses())
            {
                comboBox2.Items.Add(item2);
            }

            comboBox1.Text = DateTime.Now.Year.ToString();
            comboBox2.Text = DateTime.Now.Month.ToString();
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Clase = dt.Columns.Add("Clase", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Usuario = dt.Columns.Add("Usuario", typeof(string));
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime Desde = new DateTime(Convert.ToInt32(comboBox1.Text), getMonthNumber(comboBox2.Text), 1);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox1.Text), getMonthNumber(comboBox2.Text), getMonthLastDay(comboBox2.Text));

                List<CXN_RDA_LOG> logs = fHIR.GetLogs(Desde, Hasta);
                if (logs != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_RDA_LOG i in logs)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id;
                        row["Admision"] = i.Admision;
                        row["Clase"] = i.Clase;
                        row["Estado"] = i.Detalle != "EXITOSO" ? "ERROR" : "EXITOSO";
                        row["Fecha"] = Convert.ToDateTime(i.Fecha).ToString("yyyy-MM-dd");
                        row["Usuario"] = i.Usuario;

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
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Admision"].Width = 120;
            D.Columns["Clase"].Width = 250;
            D.Columns["Estado"].Width = 120;
            D.Columns["Fecha"].Width = 120;
            D.Columns["Usuario"].Width = 120;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Clase"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Usuario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;

            foreach (DataGridViewRow row in D.Rows)
            {
                string esta = row.Cells["Estado"].Value.ToString();

                if (esta == "ERROR")
                {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Green;
                }
            }
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Pos = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                if (Pos > 0)
                {
                    string estado = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (estado == "ERROR")
                    {
                        Logs2 logs2 = new Logs2(Pos);   
                        logs2.ShowDialog();
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Este resultado se obtuvo de una consulta exitosa, por lo tanto no hay detalles para mostrar",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    Adm_Estado = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (Adm_Estado == "ERROR")
                    {
                        contextMenuStrip1.Visible = true;
                        contextMenuStrip1.Location = new Point(dataGridView1.Location.X + 600, dataGridView1.Location.Y + 100);
                        Adm_Selected = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                    }                    
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void datosUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                otrosDatosPacienteHorario getPacId = agendaC.cargarAdmision(Adm_Selected, "'H'");
                if (getPacId != null) 
                {
                    string TDoc = getPacId.Pac_TipoId;
                    string Doc = getPacId.Pac_IdNum;

                    CrearEditarPaciente crearEditarPaciente = new CrearEditarPaciente(TDoc, Doc);
                    crearEditarPaciente.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
