using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Recepcion.Admision;

namespace ZamenisHealth.Medicina.Extras
{
    public partial class OcupacionPac : Forma2
    {
        private static readonly IFHIR rFHIR = new MFHIR();
        private ActualizarPaciente actualizarPaciente;
        private OtrosDatosPac actualizarPaciente2;
        private string FormaFrom;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Ocupacion;

        public OcupacionPac(ActualizarPaciente _actualizarPaciente)
        {
            InitializeComponent();
            actualizarPaciente = _actualizarPaciente;
            FormaFrom = "ActualizarPaciente";
        }

        public OcupacionPac(OtrosDatosPac _actualizarPaciente)
        {
            InitializeComponent();
            actualizarPaciente2 = _actualizarPaciente;
            FormaFrom = "OtrosDatosPac";
        }
        private void OcupacionPac_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Ocupaciones";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            gridZH1.CeldaHeight = true;

            cargarOcupacion("");
        }
        void cargarOcupacion(string Ocupaciona)
        {
            List<string> lista = rFHIR.GetOcupaciones(Ocupaciona);
            if (lista != null)
            {
                Encabezados();

                int Contador = 1;

                foreach (string i in lista)
                {
                    DataRow row = dt.NewRow();

                    row[POS] = Contador;
                    row[Ocupacion] = i;

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
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Ocupacion = dt.Columns.Add("Ocupacion", typeof(string));
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Cod = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                if (FormaFrom == "ActualizarPaciente")
                {
                    actualizarPaciente.textBox15.Text = Cod;
                }
                else if (FormaFrom == "OtrosDatosPac")
                {
                    actualizarPaciente2.textBox15.Text = Cod;
                } 

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            cargarOcupacion(textBox1.Text);
        }
    }
}
