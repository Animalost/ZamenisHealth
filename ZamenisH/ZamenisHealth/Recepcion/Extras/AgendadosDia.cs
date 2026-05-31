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
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class AgendadosDia : Forma2
    {
        private static readonly IAgendaC repositorioHorario = new MAgendaC();
        List<CXN_HORARIO> L = new List<CXN_HORARIO>();
        private DataTable dt;

        public AgendadosDia(List<CXN_HORARIO> l)
        {
            InitializeComponent();        

            this.L = l;
            loadGrid();
        }

        void loadGrid()
        {
            try
            {
                dataGridView1.DataSource = null;

                dt = new DataTable();
                DataColumn POS;
                DataColumn ADM;
                DataColumn CitaProgramada;

                dt = new DataTable();
                POS = dt.Columns.Add("POS", typeof(int));
                ADM = dt.Columns.Add("ADM", typeof(int));
                CitaProgramada = dt.Columns.Add("CitaProgramada", typeof(string));
                int Contador = 1;

                foreach (CXN_HORARIO i in this.L)
                {
                    DataRow row = dt.NewRow();

                    row["POS"] = Contador;
                    row["Adm"] = i.Hor_Id;
                    row["CitaProgramada"] = "ADMISION: " + i.Hor_Id.ToString() + "\n" + 
                        "FECHA: " + i.Hor_Pac_Fecha_Cita.ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "\n" + 
                        "HORA: " + i.Hor_Pac_Hora_Cita.ToString("hh:mm tt") + "\n" + 
                        "PROFESIONAL: " + i.Hor_Observacion;

                    dt.Rows.Add(row);
                    dt.AcceptChanges();

                    Contador++;
                }

                Contador = 0;

                Estilos();

                dataGridView1.CellFormatting += DataGridView1_CellFormatting;
                dataGridView1.CellClick += DataGridView1_CellClick;

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ClearSelection();
        }
        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(string))
            {
                e.Value = ((string)e.Value).Replace("\n", Environment.NewLine);
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Style.WrapMode = DataGridViewTriState.True;
                dataGridView1.Rows[e.RowIndex].Height = 70;
            }           

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
        void Estilos()
        {
            try
            {
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ScrollBars = ScrollBars.Both;

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["POS"].Width = 0;
                dataGridView1.Columns["ADM"].Width = 0;
                dataGridView1.Columns["CitaProgramada"].Width = 600;

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                dataGridView1.Columns["POS"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["ADM"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["CitaProgramada"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["POS"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["ADM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["CitaProgramada"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridView1.Columns["POS"].Visible = false;
                dataGridView1.Columns["ADM"].Visible = false;                

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void AgendadosDia_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Cancelar_Cita(int Cancela)
        {
            try
            {
                string RCancela = Microsoft.VisualBasic.Interaction.InputBox(
                        "Escriba la razon por la cual cancela esta cita",
                        "Cancelar Cita",
                            "");
                while (RCancela == "")
                {
                    RCancela = Microsoft.VisualBasic.Interaction.InputBox(
                        "Digite una razon valida, en caso de no desear cancelar esta cita " +
                        "escriba la palabra NO en mayuscula",
                        "Cancelar Cita",
                            "");
                }

                if (RCancela == "NO")
                {
                    return;
                }
                else
                {
                    
                        repositorioHorario.CancelacionInterna(RCancela, Comunes.Contenedor.UsuarioLogueado, Cancela, "CANCELACION INTERNA");
                                        

                    Agenda f7 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                    f7.RechargeTrueCheck();

                    this.L.RemoveAll(item => item.Hor_Id == Cancela);
                    loadGrid();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }   
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView1.ClearSelection();

                if (e.ColumnIndex == 2)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() != "")
                    {
                        DialogResult result2 = MessageBox.Show("¿Desea cancelar esta cita?  Si no la desea cancelar, escriba NO",
                                "Zamenis Health",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                        if (result2 == DialogResult.Yes)
                        {
                            if (result2.ToString() != "NO")
                            {
                                Cancelar_Cita(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
                            }
                        }
                    }
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
