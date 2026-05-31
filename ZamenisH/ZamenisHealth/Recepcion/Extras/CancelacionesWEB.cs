using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
                Estilos(dataGridView1, dt);
            }
            else
            {
                Encabezados();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Encabezados()
        {
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
                        Estilos(dataGridView1, dt);
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
            Comunes.MensajesGeneral MG = new MensajesGeneral();
            MG.Mensaje = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            MG.TipoImagen = 3;
            MG.ShowDialog();
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
                        Estilos(dataGridView1, dt);
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
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Admision"].Width = 120;
            D.Columns["FechaCita"].Width = 120;
            D.Columns["Paciente"].Width = 350;            

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["FechaCita"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["FechaCita"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;
            D.Columns["Razon"].Visible = false;

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
        }
    }
}
