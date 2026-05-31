using Domain.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Persistence;
using FormAndControls;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class BloqueoDias : Forma
    {
        
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly IAgendaC repositorioFechasAgenda = new MAgendaC();

        private int Prof_Cod;
        DataTable dt = new DataTable();
        DataColumn Id;
        DataColumn POS;
        DataColumn Fecha;
        DataColumn Estado;
        DataColumn Bloquea;
        DataColumn DesBloquea;

        public BloqueoDias()
        {
            InitializeComponent();
        }

        private void BloqueoDias_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Bloqueo de Dias";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBloquear = new ToolStripButton();
            btnBloquear = createToolButton("Bloquear");
            MenuLateral.Items.Add(btnBloquear);
            btnBloquear.Click += button1_Click;

            List<string> prof = repositorioBodegas.Profesionales("Todos");            

            if (prof != null)
            {
                foreach (var i in prof)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            (int CodProf, string TipoBod) code = (0, "");

            
                code = repositorioBodegas.ProfesionalId(comboBox1.Text);
            

            Prof_Cod = Convert.ToInt32(code.CodProf);
            CargarList();
        }
        private void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Estado = dt.Columns.Add("Estado", typeof(string));
            Bloquea = dt.Columns.Add("Bloquea", typeof(string));
            DesBloquea = dt.Columns.Add("DesBloquea", typeof(string));
        }
        private void CargarList()
        {
            try
            {
                List<CXN_DIAS_WEB> _list = new List<CXN_DIAS_WEB>();

                
                    _list = repositorioFechasAgenda.CargarList(Prof_Cod);
                

                if (_list != null)
                {
                    Encabezados();
                    int Contador = 1;

                    foreach (CXN_DIAS_WEB i in _list)
                    {
                        string Est;

                        switch (i.F_Estado)
                        {
                            case "B":
                                Est = "Bloqueado";
                                break;

                            case "D":
                                Est = "Desbloqueado";
                                break;

                            default:
                                Est = "Error";
                                break;
                        }

                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.F_Id.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.F_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Estado"] = Est;
                        row["Bloquea"] = i.F_Bloquea;
                        row["DesBloquea"] = i.F_Desbloquea;

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " - Error en Grilla", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Fecha"].Width = 120;
            D.Columns["Estado"].Width = 120;
            D.Columns["Bloquea"].Width = 150;
            D.Columns["DesBloquea"].Width = 150;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Bloquea"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["DesBloquea"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Estado"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Bloquea"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["DesBloquea"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;

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
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 10)
            {
                Comunes.MensajesGeneral mensajesGeneral = new Comunes.MensajesGeneral();
                mensajesGeneral.Mensaje = "Escriba una razon valida para bloquear el dia";
                mensajesGeneral.TipoImagen = 1000;
                mensajesGeneral.ShowDialog();
                return;
            }

            if (comboBox1.Text == "")
            {
                Comunes.MensajesGeneral mensajesGeneral = new Comunes.MensajesGeneral();
                mensajesGeneral.Mensaje = "Debe seleccionar un profesional a bloquear";
                mensajesGeneral.TipoImagen = 1000;
                mensajesGeneral.ShowDialog();
                return;
            }

            int bloq = 0;

            
                bloq = repositorioFechasAgenda.ConsultarFecha(dateTimePicker1.Value.Date, Prof_Cod);
            

            if (bloq == 0)
            {
                Comunes.MensajesGeneral mensajesGeneral = new Comunes.MensajesGeneral();
                mensajesGeneral.Mensaje = "Hubo un error consultando estos datos, por favor vuelva a intentar";
                mensajesGeneral.TipoImagen = 1000;
                mensajesGeneral.ShowDialog();
                return;
            }

            if (bloq == 2)
            {
                Comunes.MensajesGeneral mensajesGeneral = new Comunes.MensajesGeneral();
                mensajesGeneral.Mensaje = "Esta fecha ya se encuentra bloqueada para este profesional";
                mensajesGeneral.TipoImagen = 0;
                mensajesGeneral.ShowDialog();
                return;
            }

            if (bloq == 1)
            {
                CXN_DIAS_WEB D = new CXN_DIAS_WEB();
                D.F_Fecha = dateTimePicker1.Value.Date;
                D.R_Razon = textBox1.Text;
                D.F_Prof = Prof_Cod;
                D.F_Bloquea = Comunes.Contenedor.UsuarioLogueado;
                D.F_Estado = "B";

                bool graba = false;

                
                    graba = repositorioFechasAgenda.Grabar(D);
                

                if (graba != true)
                {
                    Comunes.MensajesGeneral mensajesGeneral = new Comunes.MensajesGeneral();
                    mensajesGeneral.Mensaje = "No se logro grabar la informacion, error interno de la aplicacion";
                    mensajesGeneral.TipoImagen = 1000;
                    mensajesGeneral.ShowDialog();
                    return;
                }

                CargarList();
                Comunes.MensajesGeneral mensajesGeneral1 = new Comunes.MensajesGeneral();
                mensajesGeneral1.Mensaje = "El bloqueo de dias has sido grabado correctamente";
                mensajesGeneral1.TipoImagen = 3;
                mensajesGeneral1.ShowDialog();         
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CargarList();
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString() == "Desbloqueado")
            {
                Comunes.MensajesGeneral mensajesGeneral1 = new Comunes.MensajesGeneral();
                mensajesGeneral1.Mensaje = "Este dia ya esta desbloqueado";
                mensajesGeneral1.TipoImagen = 0;
                mensajesGeneral1.ShowDialog();
                return;
            }

            DialogResult result = MessageBox.Show("¿Desea desbloquear este dia?",
                                                  "Zamenis Health",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool des = false;

                
                    des = repositorioFechasAgenda.Desbloquear(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()), Comunes.Contenedor.UsuarioLogueado);
                

                if (des != true)
                {
                    Comunes.MensajesGeneral mensajesGeneral1 = new Comunes.MensajesGeneral();
                    mensajesGeneral1.Mensaje = "Hubo un fallo inesperado desbloqueando el dia seleccionado";
                    mensajesGeneral1.TipoImagen = 1000;
                    mensajesGeneral1.ShowDialog();
                    return;
                }

                CargarList();

                Comunes.MensajesGeneral mensajesGeneral = new Comunes.MensajesGeneral();
                mensajesGeneral.Mensaje = "El desbloqueo de dias has sido actualizado correctamente";
                mensajesGeneral.TipoImagen = 3;
                mensajesGeneral.ShowDialog();
            }
        }
    }
}
