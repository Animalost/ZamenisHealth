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

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class OrdenesPendientes : Forma
    {
        private static readonly IAgenda repoAg = new MAgenda();
        private static readonly IPlanos repoPlanos = new MPlanos();

        private DataTable dt;
        private DataColumn POS;
        private DataColumn Admision;
        private DataColumn Paciente;
        private DataColumn Fecha;
        private DataColumn Estado;
        private DataColumn Registra;

        public OrdenesPendientes()
        {
            InitializeComponent();     
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;

            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Estado = dt.Columns.Add("Estado", typeof(string));
            Registra = dt.Columns.Add("Registra", typeof(string));
        }
        private void OrdenesPendientes_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Consumos Pendientes";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnDescargar = new ToolStripButton();
            btnDescargar = createToolButton("Descargar");
            MenuLateral.Items.Add(btnDescargar);
            btnDescargar.Click += button5_Click;

            Cargar();
        }
        void Cargar()
        {
            try
            {              
                Comunes.MensajesGeneral MG = new MensajesGeneral();

                List<CXN_HORARIO> getListado =  repoAg.getListPendientes();                

                if (getListado != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_HORARIO i in getListado)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Admision] = i.Hor_Id;
                        row[Paciente] = i.Hor_Imp_Age.ToString();
                        row[Fecha] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[Estado] = i.Hor_Estado.ToString().Trim() == "P" ? "Pendiente" :
                                      i.Hor_Estado.ToString().Trim() == "H" ? "Hecho" : 
                                      "Eliminado";
                        row[Registra] = i.Hor_Observacion.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay consumos pendientes";
                    MG.ShowDialog();

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

            D.Columns["Admision"].Width = 100;
            D.Columns["Paciente"].Width = 300;
            D.Columns["Fecha"].Width = 100;
            D.Columns["Estado"].Width = 100;
            D.Columns["Registra"].Width = 100;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Registra"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
        }   
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                repoPlanos.ExpPlanoOrdenesPendientes(Convert.ToDateTime(dateTimePicker1.Value), Convert.ToDateTime(dateTimePicker2.Value));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            DatosCita D = new DatosCita(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()), "OPendConsumer");
            D.ShowDialog();
            Cargar();
        }
    }
}
