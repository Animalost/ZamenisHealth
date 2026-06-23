using Domain;
using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.AgendaDiaria;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class Busqueda : Forma
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IAgendaC repositorioFechasAgendaa = new MAgendaC();
        private static readonly IAgenda repositorioHorario = new MAgenda();
        private static readonly ICompañia repositorioCompañia = new MCompañia();
        private static readonly IBodegas repositorioBodegas = new MBodegas();

        private DateTime Fecha;
        private string Document = "";

        public Busqueda(DateTime fecha)
        {
            InitializeComponent();
            this.Fecha = fecha;
        }
        public Busqueda(DateTime fecha, string Documento)
        {
            InitializeComponent();
            this.Fecha = fecha;
            this.Document = Documento;
            setDoc(this.Document);
            Buscar_X_Docs();
        }
        private void Busqueda2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Citas por Paciente";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;
            
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

                CXN_PACIENTES _paciente = new CXN_PACIENTES();

                
                    _paciente = repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text);
                

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Busca_Paciente(int Pac_Id)
        {
            try
            {
                List<CXN_HORARIO> _citasXpac = new List<CXN_HORARIO>();

                
                    _citasXpac = repositorioFechasAgendaa.ListarCitasXPaciente(Pac_Id, dateTimePicker1.Value);
                

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
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
        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                string Admision = listView1.SelectedItems[0].SubItems[0].Text;
                otrosDatosPacienteHorario dataCita = repositorioFechasAgendaa.cargarAdmision(Convert.ToInt32(Admision), "'A','P','H'");
                if (dataCita == null)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay datos pasra esta admision";
                    MG.ShowDialog();
                    return;
                }

                string _consEstadoAdmision = dataCita.Hor_Estado;
                
                if (_consEstadoAdmision != "0")
                {
                    if (_consEstadoAdmision == "A")
                    {
                        CXN_CIA getCompany =  repositorioCompañia.getPrestadorbyName(listView1.SelectedItems[0].SubItems[5].Text);

                        if (getCompany != null)
                        {
                            CXN_BODEGAS getBodega = repositorioBodegas.getDatosName(listView1.SelectedItems[0].SubItems[3].Text);
                            
                            if (getBodega != null)
                            {
                                MenuOpcionesAgenda menuOpcionesAgenda = new MenuOpcionesAgenda(Admision.ToString(), dataCita.Hor_Pac_Id_Hora, dataCita.Hor_Pac_Cia, dataCita.Hor_Pac_Hora_Cita.ToString(), dataCita.Hor_Pac_Bod);
                                menuOpcionesAgenda.ShowDialog();
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
                        DatosCita f = new Extras.DatosCita(Convert.ToInt32(Admision), "Busqueda");
                        f.ShowDialog();
                    }

                    if (_consEstadoAdmision == "H")
                    {
                        DatosCita f = new Extras.DatosCita(Convert.ToInt32(Admision), "Busqueda");
                        f.ShowDialog();
                    }

                    Agendamiento f1 = Application.OpenForms.OfType<Agendamiento>().FirstOrDefault();
                    f1.EventoInicial();

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
