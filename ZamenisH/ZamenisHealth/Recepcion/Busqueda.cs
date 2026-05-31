using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Agenda2
{
    public partial class Busqueda : Form
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IAgendaC repositorioFechasAgendaa = new MAgendaC();
        private static readonly IAgenda repositorioHorario = new MAgenda();
        private static readonly ICompañia repositorioCompañia = new MCompañia();
        private static readonly IBodegas repositorioBodegas = new MBodegas();

        private DateTime Fecha;
        private bool EsAgenda2, Vienedela1;

        public Busqueda(DateTime fecha, bool _EsAgenda2, bool _vienedela1)
        {
            InitializeComponent();

            this.Fecha = fecha;
            this.EsAgenda2 = _EsAgenda2;
            this.Vienedela1 = _vienedela1;
        }

        private void Busqueda_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = this.Fecha;
            Encabezados();
        }

        void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Admision", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Hora", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Paciente", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Medico", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Estado", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Prestador", 250, HorizontalAlignment.Left);
        }

        void Buscar_X_Docs()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (textBox1.Text == "")
                {
                    MG.Mensaje = "Debe escribir el numero de documento";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                var _paciente = repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text);
                if (_paciente != null)
                {
                    textBox2.Text = _paciente.Pac_PrimerA + " " + _paciente.Pac_SegundoA + " " + _paciente.Pac_PrimerN + " " + _paciente.Pac_SegundoN;
                    Busca_Paciente(_paciente.Pac_Id);
                }
                else
                {
                    Encabezados();

                    textBox2.Text = "(No hay Resultados)";

                    MG.Mensaje = "Este tipo y numero de identificacion no existen en sistema";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Busca_Paciente(int Pac_Id)
        {
            try
            {
                var _citasXpac = repositorioFechasAgendaa.ListarCitasXPaciente(Pac_Id, dateTimePicker1.Value);
                if (_citasXpac != null)
                {
                    Encabezados();

                    foreach (var i in _citasXpac)
                    {
                        string Est;
                        switch (i.Hor_Estado)
                        {
                            case "A":
                                Est = "Agendado";
                                break;
                            case "P":
                                Est = "Admisionado";
                                break;
                            case "H":
                                Est = "Asistio";
                                break;
                            default:
                                Est = "Inconsistencia";
                                break;
                        }

                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                                i.Hor_Id.ToString(),
                                Convert.ToDateTime(i.Hor_Pac_Hora_Cita).ToString("HH:mm tt"),
                                textBox2.Text,
                                i.Hor_Observacion,
                                Est,
                                i.Hor_Pac_Razon
                        }));

                        foreach (ListViewItem lvw in listView1.Items)
                        {
                            if (lvw.SubItems[4].Text == "Agendado")
                            {
                                lvw.ForeColor = Color.Black;
                                lvw.BackColor = Color.White;
                            }
                            if (lvw.SubItems[4].Text == "Admisionado")
                            {
                                lvw.ForeColor = Color.Green;
                                lvw.BackColor = Color.LightGreen;
                            }
                            if (lvw.SubItems[4].Text == "Asistio")
                            {
                                lvw.ForeColor = Color.Blue;
                                lvw.BackColor = Color.LightBlue;
                            }
                            if (lvw.SubItems[4].Text == "Inconsistencia")
                            {
                                lvw.ForeColor = Color.Red;
                                lvw.BackColor = Color.Orange;
                            }
                        }

                    }
                }
                else
                {
                    Encabezados();

                    textBox2.Text = "(No hay Citas)";
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "Este paciente no tiene citas agendadas el dia seleccionado";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void setDoc(string nid)
        {
            textBox1.Text = nid.ToString();
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "Agenda2";
            buscarPacientes.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Buscar_X_Docs();
        }

        private void Busqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                Buscar_X_Docs();
            }
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void label6_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();               

                string Admision = listView1.SelectedItems[0].SubItems[0].Text;
                string _consEstadoAdmision = repositorioHorario.consularAdmisionEstado(Convert.ToInt32(Admision));

                if (_consEstadoAdmision != "0")
                {
                    if (_consEstadoAdmision == "A")
                    {
                        var getCompany = repositorioCompañia.getPrestadorbyName(listView1.SelectedItems[0].SubItems[5].Text);
                        if (getCompany != null)
                        {
                            var getBodega = repositorioBodegas.getDatosName(listView1.SelectedItems[0].SubItems[3].Text);
                            if (getBodega != null)
                            {
                               /* Grilla2 g = new Grilla2(getBodega.Bod_Numero, 
                                                        getCompany.Com_Identificador, 
                                                        this.Fecha, 
                                                        Convert.ToDateTime(this.Fecha).ToString("dddd"), 
                                                        listView1.SelectedItems[0].SubItems[5].Text, 
                                                        listView1.SelectedItems[0].SubItems[3].Text);

                                g.ShowDialog();*/
                                
                                ConsultaAdmision RCA = new ConsultaAdmision(true, this.Vienedela1);
                                RCA.label3.Text = Admision;
                                RCA.ShowDialog();
                            }
                            else
                            {
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "Error inesperado en esta admision, pruebe en la agenda clasica";
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Error inesperado en esta admision, pruebe en la agenda clasica";
                            MG.ShowDialog();
                        }                        
                    }

                    if (_consEstadoAdmision == "P")
                    {
                        Extras.DatosCita Datos_Pac_Age = new Extras.DatosCita(Convert.ToInt32(Admision));
                        Datos_Pac_Age.ShowDialog();
                    }

                    if (_consEstadoAdmision == "H")
                    {
                        Extras.DatosCita Datos_Pac_Age = new Extras.DatosCita(Convert.ToInt32(Admision));
                        Datos_Pac_Age.ShowDialog();
                    }

                    if (EsAgenda2 == false)
                    {
                        Agenda f1 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                        if (f1.Checked == true)
                        {
                            f1.RechargeTrueCheck();
                        }
                        else
                        {
                            f1.MetodoAgenda();
                        }
                    }

                    Buscar_X_Docs();
                }
                else
                {
                    Buscar_X_Docs();

                    MG.Mensaje = "Hay un inconveniente con esta admision, pruebe con la seleccion normal de la agenda";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
