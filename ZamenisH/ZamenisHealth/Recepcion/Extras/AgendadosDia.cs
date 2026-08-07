using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.AgendaDiaria;

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
        }

        void loadGrid()
        {
            try
            {
                gridZH1.dataGridView1.DataSource = null;

                dt = new DataTable();
                DataColumn POS;
                DataColumn ADM;
                DataColumn CitaProgramada;

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

                gridZH1.dataGridView1.DataSource = dt;
                gridZH1.dataGridView1.Columns["POS"].Visible = false;
                gridZH1.dataGridView1.Columns["ADM"].Visible = false;
                
                
                foreach (DataGridViewRow i in gridZH1.dataGridView1.Rows)
                {
                    if (i.Cells["Adm"].Value != null)
                    {
                        i.Cells["CitaProgramada"].Value = i.Cells["CitaProgramada"].Value.ToString().Replace("\n", Environment.NewLine);
                    }
                }

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }           
        }       
        private void AgendadosDia_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Proximas Citas";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;
                gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
                gridZH1.CeldaHeight = true;

                loadGrid();                
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

                    Agendamiento f7 = Application.OpenForms.OfType<Agendamiento>().FirstOrDefault();
                    f7.EventoInicial();

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
                gridZH1.dataGridView1.ClearSelection();

                if (e.ColumnIndex == 2)
                {
                    if (gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() != "")
                    {
                        DialogResult result2 = MessageBox.Show("¿Desea cancelar esta cita?  Si no la desea cancelar, escriba NO",
                                "Zamenis Health",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                        if (result2 == DialogResult.Yes)
                        {
                            if (result2.ToString() != "NO")
                            {
                                Cancelar_Cita(Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
                            }
                        }
                    }
                }

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
