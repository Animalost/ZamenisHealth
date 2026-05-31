using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class HistorialNotas : ConfigForm.BaseForm
    {
        private static readonly INotasCuracion repoNotasCuracion = new MNotasCuracion();

        private int Paciente;
        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Profesional;

        public HistorialNotas(int _paciente)
        {
            InitializeComponent();
            this.Paciente = _paciente;
        }

        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
        }

        void Estilos()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Admision"].Width = 120;
            dataGridView1.Columns["Fecha"].Width = 120;
            dataGridView1.Columns["Profesional"].Width = 700;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Profesional"].SortMode = DataGridViewColumnSortMode.NotSortable;

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

        private void HistorialNotas_Load(object sender, EventArgs e)
        {
            try
            {               
                List<CXN_HORARIO> getHist = repoNotasCuracion.getHistorial(Paciente);
                if (getHist != null)
                {
                    textBox19.Text = "Este es el historial del paciente, haga clic en las atenciones que desee observar.";

                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_HORARIO i in getHist)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = i.Hor_Id.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Profesional"] = i.Hor_Imp_Age.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Estilos();
                }
                else
                {
                    Encabezados();

                    Comunes.MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este paciente no tiene mas historial";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Encabezados();

                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);

                this.Dispose();
                this.Close();
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Medicina.NotasReportPrevio historia_Notas_Export = new Medicina.NotasReportPrevio();
                historia_Notas_Export.Adm_Nota_Export = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                historia_Notas_Export.ShowDialog();
                Estilos();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
