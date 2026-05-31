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

namespace ZamenisHealth.Comunes
{
    public partial class MensajeroSend : Forma
    {
        private static readonly IMensajeria repositorioMensajeria = new MMensajeria();
        private static readonly IGenerales repositorioGenerales = new MGenerales();

        MensajesGeneral MG;
        private string persona = "";
        private string personaUser;

        DataTable dt;
        DataColumn POS;
        DataColumn Usuario;
        DataColumn Persona;

        public MensajeroSend()
        {
            InitializeComponent();
        }
        public MensajeroSend(string Persona)
        {
            InitializeComponent();
            persona = Persona; //nombre persona
        }
        private void CargarMensajes()
        {
            try
            {
                richTextBox3.Clear();
                List<CXN_MESSENGER> _getMessages = repositorioMensajeria.getAllMessages(personaUser, Contenedor.UsuarioLogueado, dateTimePicker1.Value.Date);

                if (_getMessages != null)
                {
                    foreach (CXN_MESSENGER i in _getMessages)
                    {
                        int start = richTextBox3.Text.Length;

                        richTextBox3.AppendText(i.Fecha.ToString("dd-MM-yyyy") + " " + i.Hora.ToString("HH:mm:ss tt") + " - " + i.Men_Usuario_De.ToString() + "\n");
                        richTextBox3.AppendText(repositorioGenerales.Base64Decode(i.Men_Mensaje.ToString()) + " \n\r\r");

                        richTextBox3.Select(start, richTextBox3.Text.Length - start);

                        richTextBox3.SelectionFont = new Font(richTextBox3.Font, FontStyle.Bold);

                        int positionDate = richTextBox3.Text.IndexOf(i.Fecha.ToString("dd-MM-yyyy") + " " + i.Hora.ToString("HH:mm:ss tt"));
                        int lengthDate = (i.Fecha.ToString("dd-MM-yyyy") + " " + i.Hora.ToString("HH:mm:ss tt")).Length;

                        richTextBox3.Select(positionDate, lengthDate);
                        richTextBox3.SelectionColor = Color.Blue;

                        richTextBox3.DeselectAll();
                    }
                }
                else
                {
                    richTextBox3.Clear();
                }
            }            
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void MensajeroSend_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Enviar Mensaje";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                //this.ImageClose.Visible = false;
                this.ImageMinimize.Visible = false;

                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnEnviar = new ToolStripButton();
                btnEnviar = createToolButton("Enviar Mensaje");
                MenuLateral.Items.Add(btnEnviar);
                btnEnviar.Click += toolStripButton2_Click;

                Contenedor f2 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
                f2.timer3.Enabled = false;

                Dictionary<string, string> usersActive = repositorioMensajeria.getUsersforSendMessage();

                if (usersActive != null)
                {
                    Encabezados();
                    int Contador = 1;

                    foreach (var i in usersActive)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Usuario"] = i.Key.ToString().Trim();
                        row["Persona"] = i.Value.ToString().Trim();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador++;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }

                if (persona != "")
                {
                    foreach (DataGridViewRow fila in dataGridView1.Rows)
                    {
                        if (fila.Cells["Persona"].Value != null &&
                            fila.Cells["Persona"].Value.ToString().Contains(persona))
                        {
                            fila.Selected = true;
                            dataGridView1.FirstDisplayedScrollingRowIndex = fila.Index; // Para hacer scroll hasta ahí
                     
                            personaUser = fila.Cells["Usuario"].Value.ToString();

                            break; // Si solo deseas seleccionar la primera coincidencia
                        }
                    }

                    CargarMensajes();
                }

                boton1.BringToFront();
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

            D.Columns["Usuario"].Width = 200;
            D.Columns["Persona"].Width = 470;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Usuario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Persona"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Usuario"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Persona"].SortMode = DataGridViewColumnSortMode.NotSortable;

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
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Usuario = dt.Columns.Add("Usuario", typeof(string));
            Persona = dt.Columns.Add("Persona", typeof(string));
        }     
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                if (richTextBox2.Text == "" || personaUser == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe diligenciar un mensaje y seleccionar un destinatario";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                string _message = repositorioGenerales.Base64Encode(richTextBox2.Text + " " + " \n\r");

                CXN_MESSENGER X = new CXN_MESSENGER
                {
                    Estado = "E",
                    Men_Mensaje = _message,
                    Men_Usuario_Para = personaUser,
                    Men_Usuario_De = Contenedor.UsuarioLogueado
                };

                bool _saveMessage = false;

                _saveMessage = repositorioMensajeria.SendMessage(X);

                if (_saveMessage != true)
                {
                    MG.Mensaje = "Su mensaje no se ha logrado enviar, hubo un problema";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                else
                {
                    MG.Mensaje = "Enviado";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();

                    Contenedor f2 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
                    f2.timer3.Enabled = true;

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime hoy = DateTime.Now.Date;

                if (Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) != Convert.ToDateTime(hoy).ToString(Conexion.ConectionDictionary["Format_Fecha"]))
                {
                    richTextBox2.Enabled = false;
                    CargarMensajes();
                }
                else
                {
                    richTextBox2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime hoy = DateTime.Now.Date;
                dateTimePicker1.Value = hoy;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            persona = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            personaUser = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            CargarMensajes();
        }
        private void MensajeroSend_FormClosing(object sender, FormClosingEventArgs e)
        {
            Contenedor f2 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
            f2.timer3.Enabled = true;
        }
    }
}
