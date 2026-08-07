using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class CancelacionesWEB : Forma2
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IAgendaC repoAG = new MAgendaC();

        private List<CXN_HORARIO> CitasFromAgenda;
        private string TDOC, DOC;
        DataTable dt;
        DataColumn POS;
        DataColumn Admision;
        DataColumn FechaCita;
        DataColumn Paciente;
        DataColumn Razon;

        public CancelacionesWEB()
        {
            InitializeComponent();
        }

        public CancelacionesWEB(List<CXN_HORARIO> citasFromAgenda)
        {
            InitializeComponent();
            this.CitasFromAgenda = citasFromAgenda;
            comboBox1.Enabled = false;
            textBox1.Enabled = false;

            if (this.CitasFromAgenda != null)
            {
                Encabezados();
                int Contador = 1;

                foreach (var i in this.CitasFromAgenda)
                {
                    DataRow row = dt.NewRow();

                    row["POS"] = Contador;
                    row["Admision"] = i.Hor_Id.ToString();
                    row["FechaCita"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    row["Paciente"] = i.Hor_Imp_Age.ToString();
                    row["Razon"] = i.Hor_Pac_RCancela.ToString();

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

        private void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            FechaCita = dt.Columns.Add("FechaCita", typeof(DateTime));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Razon = dt.Columns.Add("Razon", typeof(string));    
        }
        private void CancelacionesWEB_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cancelaciones WEB";

            gridZH1.dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;


        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes Busca_Pac = new Comunes.BuscarPacientes();
            Busca_Pac.Tipo_Busca_Pac = "CANCELAWEB";
            Busca_Pac.ShowDialog();
        }
        public void setDatos(string tdoc, string doc)
        {
            this.TDOC = tdoc;
            this.DOC = doc;

            textBox1.Text = this.DOC;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == 1) 
                {
                    Encabezados();                 
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    List<CXN_HORARIO> getDatos = repoAG.consultaCancelaWEB("XXX");
                    if (getDatos != null)
                    {
                        Encabezados();
                        int Contador = 1;

                        foreach (var i in getDatos)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Admision"] = i.Hor_Id.ToString();
                            row["FechaCita"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row["Paciente"] = i.Hor_Imp_Age.ToString();
                            row["Razon"] = i.Hor_Pac_RCancela.ToString();

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
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }           
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                CXN_PACIENTES getPaciente = new CXN_PACIENTES();

                
                    getPaciente = repoPac.LlamarPacienteNumDoc(textBox1.Text);
                

                if (getPaciente != null)
                {
                    List<CXN_HORARIO> getDatos = new List<CXN_HORARIO>();

                    
                        getDatos = repoAG.consultaCancelaWEB(getPaciente.Pac_Id.ToString());
                    

                    if (getDatos != null)
                    {
                        Encabezados();
                        int Contador = 1;

                        foreach (var i in getDatos)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Admision"] = i.Hor_Id.ToString();
                            row["FechaCita"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row["Paciente"] = i.Hor_Imp_Age.ToString();
                            row["Razon"] = i.Hor_Pac_RCancela.ToString();

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
                else
                {
                    Encabezados();
                }
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;          

            D.Columns["POS"].Visible = false;
            D.Columns["Razon"].Visible = false;
        }
    }
}
