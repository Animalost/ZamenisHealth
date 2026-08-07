using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class RetomarHC : Forma2
    {
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();

        public string Tipo;
        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Paciente;

        public RetomarHC()
        {
            InitializeComponent();
        }

        private void RetomarHC_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Retomar Hitorias";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                var ValMed = repoBodegas.EsProfesional("Medico", Comunes.Contenedor.UsuarioLogueado);
                if (ValMed != true)
                {
                    MessageBox.Show("Su usuario no es tipo medico no puede continuar",
                        "ACESO DENEGADO!!!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.Dispose();
                    this.Close();
                    return;
                }

                gridZH1.dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
                gridZH1.CeldaHeight = true;
                CargarGrilla();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
        }
        private void CargarGrilla()
        {
            try
            {
                List<CXN_HORARIO> getGrilla = repoAgendaMedicaConsultas.CargarGrilla(Tipo);
                if (getGrilla != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_HORARIO i in getGrilla)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = i.Hor_Id.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Paciente"] = i.Hor_Imp_Age.ToString();

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {  
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (Tipo == "MG")
                {
                    HistoriasClinicas.Historia_MedicinaGeneral HCMG = new HistoriasClinicas.Historia_MedicinaGeneral();
                    HCMG.Admision = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    HCMG.Retoma = true;
                    HCMG.ShowDialog();
                }

                if (Tipo == "FI")
                {
                    HistoriasClinicas.Historia_Fisiatria HCFI = new HistoriasClinicas.Historia_Fisiatria();
                    HCFI.Admision = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    HCFI.Retoma = true;
                    HCFI.ShowDialog();
                }

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
