using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion
{
    public partial class Asistencia : Forma
    {
        private static readonly IAgenda repositorioAgendar = new MAgenda();
        private MensajesGeneral MG;
        private ToolStripButton btnBuscar;

        public Asistencia()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Asist();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public Asistencia(string TID)
        {
            InitializeComponent();
            textBox1.Text = TID;
            Asist();
        }

        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Numero", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("Admision", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Estado", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Hora", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Nombre", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Profesional", 300, HorizontalAlignment.Left);
            listView1.Columns.Add("Servicio", 300, HorizontalAlignment.Left);
            listView1.Columns.Add("Autorizacion", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Sesion", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Usuario que Asigna", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Usuario que Admisiona", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Usuario que Cancela", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Razon de Cancelacion", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("Modalidad", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Retardos", 200, HorizontalAlignment.Left);
            listView1.Visible = true;
        }
        public void Asist()
        {
            try
            {
                if (textBox1.Text != "" && comboBox1.Text != "")
                {
                    if (comboBox1.SelectedIndex == 0) // 100 Citas
                    {
                        List<CXN_HORARIO> _listaCistas = new List<CXN_HORARIO>();

                        
                            _listaCistas = repositorioAgendar.Asistencia(textBox1.Text);
                        

                        if (_listaCistas != null)
                        {
                            Encabezados();
                            int Contador = 1;

                            foreach (CXN_HORARIO i in _listaCistas)
                            {
                                string Modalidad = ModalidadName(i.Hor_Pac_Modalidad);                                

                                listView1.Items.Add(new ListViewItem(new string[]
                                {
                                    Contador.ToString(),
                                    i.Hor_Id.ToString(),
                                    i.Hor_Estado,
                                    Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                    Convert.ToDateTime(i.Hor_Pac_Hora_Cita).ToString("HH:mm tt"),
                                    i.Hor_Imp_Age,
                                    i.Hor_Regimen,
                                    i.Hor_RegAtn.ToString(),
                                    i.Hor_Autoriza,
                                    (i.Hor_IniciaSesion == "0" ? "" : i.Hor_IniciaSesion),
                                    i.Hor_Pac_UsrGraba,
                                    i.Hor_Usr_Admisiona,
                                    i.Hor_Usr_Cancela,
                                    i.Hor_Pac_RCancela,
                                    Modalidad,
                                    i.Hor_Pac_Minutos
                                }));

                                Contador++;
                            }

                            Estilo();
                        }
                        else
                        {
                            Encabezados();

                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Paciente no existe o sin historial de citas";
                            MG.ShowDialog();
                        }
                    }
                    else if (comboBox1.SelectedIndex == 1) // Ultimas Citas desde ultima autorizacion
                    {
                        List<CXN_HORARIO> _listaCistas = new List<CXN_HORARIO>();

                        
                            _listaCistas = repositorioAgendar.AsistenciaLastAut(textBox1.Text);
                        

                        if (_listaCistas != null)
                        {
                            Encabezados();
                            int Contador = 1;

                            foreach (CXN_HORARIO i in _listaCistas)
                            {
                                string Modalidad = ModalidadName(i.Hor_Pac_Modalidad);

                                listView1.Items.Add(new ListViewItem(new string[]
                                {
                                    Contador.ToString(),
                                    i.Hor_Id.ToString(),
                                    i.Hor_Estado,
                                    Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                    Convert.ToDateTime(i.Hor_Pac_Hora_Cita).ToString("HH:mm tt"),
                                    i.Hor_Imp_Age,
                                    i.Hor_Regimen,
                                    i.Hor_RegAtn.ToString(),
                                    i.Hor_Autoriza,
                                    (i.Hor_IniciaSesion == "0" ? "" : i.Hor_IniciaSesion),
                                    i.Hor_Pac_UsrGraba,
                                    i.Hor_Usr_Admisiona,
                                    i.Hor_Usr_Cancela,
                                    i.Hor_Pac_RCancela,
                                    Modalidad,
                                    i.Hor_Pac_Minutos
                                }));

                                Contador++;
                            }

                            Estilo();
                        }
                        else
                        {
                            Encabezados();

                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Paciente no existe o sin historial de citas";
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccine un filtro";
                        MG.ShowDialog();
                    }
                }
                else
                {
                   /* MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar un documento y un tipo de filtro";
                    MG.ShowDialog();*/
                }                
            }
            catch
            {
                listView1.Visible = false;
                listView1.Clear();
                MessageBox.Show("Paciente no existe o sin historial de citas", "No hay datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        string ModalidadName(string Codigo)
        {
            string Modalidad = "";

            switch (Codigo)
            {                
                case "01":
                    Modalidad = "Presencial";
                    break;

                case "06":
                    Modalidad = "Virtual";
                    break;

                case "03":
                    Modalidad = "Domicilio";
                    break;

                case "02":
                    Modalidad = "Extramural unidad móvil";
                    break;

                case "04":
                    Modalidad = "Extramural jornada de salud";
                    break;

                case "07":
                    Modalidad = "Telemedicina no interactiva";
                    break;

                case "08":
                    Modalidad = "Telemedicina telexperticia";
                    break;

                case "09":
                    Modalidad = "Telemedicina telemonitoreo";
                    break;

                default:
                    Modalidad = "Presencial";
                    break;                
            }

            return Modalidad;
        }
        void Estilo()
        {
            try
            {
                foreach (ListViewItem lvw in listView1.Items)
                {
                    if (lvw.SubItems[2].Text == "A")
                    {
                        lvw.SubItems[2].Text = "Sin Asistir";
                        lvw.ForeColor = Color.Black;
                        lvw.BackColor = Color.White;
                    }
                    if (lvw.SubItems[2].Text == "P")
                    {
                        lvw.SubItems[2].Text = "Pendiente";
                        lvw.ForeColor = Color.Green;
                        lvw.BackColor = Color.LightGreen;
                    }
                    if (lvw.SubItems[2].Text == "H")
                    {
                        lvw.SubItems[2].Text = "Asistio";
                        lvw.ForeColor = Color.Blue;
                        lvw.BackColor = Color.LightBlue;
                    }
                    if (lvw.SubItems[2].Text == "C")
                    {
                        lvw.SubItems[2].Text = "Cancelo";
                        lvw.ForeColor = Color.Red;
                        lvw.BackColor = Color.Orange;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Asistencia_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Asistencia";
            LogoMain.Image = Properties.Resources.Splash;

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += btnZamenis1_ButtonClick;

            ToolStripButton btnCWeb = new ToolStripButton();
            btnCWeb = createToolButton("Cancelaciones WEB");
            MenuLateral.Items.Add(btnCWeb);
            btnCWeb.Click += label5_Click;

            
            comboBox1.SelectedIndex = 0;
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes Busca_Pac = new Comunes.BuscarPacientes();
            Busca_Pac.Tipo_Busca_Pac = "Asistencia";
            Busca_Pac.ShowDialog();
        }      
        private void label5_Click(object sender, EventArgs e)
        {
            Extras.CancelacionesWEB W = new Extras.CancelacionesWEB();
            W.ShowDialog();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Asist();
        }
    }
}
