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
    public partial class ActivarProfesionales : Forma2
    {
        private static readonly IBodegas repositorioBodegas = new MBodegas();

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Profesional;
        DataColumn Estado;

        public ActivarProfesionales()
        {
            InitializeComponent();
        }
        private void label2_Click(object sender, EventArgs e)
        {
            try
            {
                Filtrar();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
        }
        private void Filtrar()
        {
            try
            {
                List<CXN_BODEGAS> _listaProfesionales = repositorioBodegas.Filtrar();                

                if (_listaProfesionales != null)
                {
                    Encabezados();
                    int Contador = 1;

                    foreach (var i in _listaProfesionales)
                    {
                        string Est;
                        switch (i.Bod_Estado)
                        {
                            case "A":
                                Est = "Activo";
                                break;

                            default:
                                Est = "Inactivo";
                                break;
                        }

                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Codigo"] = i.Bod_Numero.ToString();
                        row["Profesional"] = i.Bod_Responsable.ToString();
                        row["Estado"] = Est;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);

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

            D.Columns["Codigo"].Width = 80;
            D.Columns["Profesional"].Width = 600;
            D.Columns["Estado"].Width = 80;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Codigo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Profesional"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Estado"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;

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
        private void ActivarProfesionales_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Activar Profesionales";
                
                
                Filtrar();              
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Estado = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                int Bodega = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());

                DialogResult result = MessageBox.Show("Este profesional esta actualmente " + Estado +
                    " en las agendas web y suya.  ¿Desea cambiar el estado de este profesional?",
                    "Zamenis Health - Autorizacion de Profesionales en la Agenda",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    switch (Estado)
                    {
                        case "Activo":                           
                            repositorioBodegas.ActivarDesactivarBodega(Bodega, "N");                           
                            break;

                        case "Inactivo":                           
                            repositorioBodegas.ActivarDesactivarBodega(Bodega, "A");                           
                            break;

                        default:
                            MessageBox.Show("Hubo un error, cierre esta pantalla y vuelva a intentarlo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                if (result == DialogResult.No)
                {
                    Filtrar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void ActivarProfesionales_FormClosing(object sender, FormClosingEventArgs e)
        {
            Agenda f2 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();
            f2.Dispose();
            f2.Close();

            Agenda F = new Agenda();
            F.ShowDialog();
        }
    }
}
