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
            SoloNumeros(textBox1);
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Logs FHIR - MinSalud - Zamenis Health";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            gridZH1.dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;

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
            comboBox2.Text = GetNameMonth(DateTime.Now.Month.ToString());           
        }
        string GetNameMonth(string MonthNumber)
        {
            switch (MonthNumber)
            {
                case "1":
                    return "ENERO";
                case "2":
                    return "FEBRERO";
                case "3":
                    return "MARZO";
                case "4":
                    return "ABRIL";
                case "5":
                    return "MAYO";
                case "6":
                    return "JUNIO";
                case "7":
                    return "JULIO";
                case "8":
                    return "AGOSTO";
                case "9":
                    return "SEPTIEMBRE";
                case "10":
                    return "OCTUBRE";
                case "11":
                    return "NOVIEMBRE";
                case "12":
                    return "DICIEMBRE";
                default:
                    return "";
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
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

                List<CXN_RDA_LOG> logs = fHIR.GetLogs(Desde, Hasta, 0);
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
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Pos = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                int Admi = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                string Clase = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

                if (Pos > 0)
                {
                    string estado = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (estado == "ERROR")
                    {
                        Logs2 logs2 = new Logs2(Pos, "ERROR", Clase, Admi);   
                        logs2.ShowDialog();
                    }
                    else
                    {
                        Logs2 logs2 = new Logs2(Pos, "EXITOSO", Clase, Admi);
                        logs2.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    Adm_Estado = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                    if (Adm_Estado == "ERROR")
                    {
                        Point posicionLocal = Cursor.Position;

                        contextMenuStrip1.Visible = true;
                        contextMenuStrip1.Location = new Point(posicionLocal.X, posicionLocal.Y);
                        Adm_Selected = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                    }                    
                }

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime Desde = new DateTime(Convert.ToInt32(comboBox1.Text), getMonthNumber(comboBox2.Text), 1);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox1.Text), getMonthNumber(comboBox2.Text), getMonthLastDay(comboBox2.Text));

                List<CXN_RDA_LOG> logs = fHIR.GetLogs(Desde, Hasta, Convert.ToInt32(textBox1.Text));
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
    }
}
