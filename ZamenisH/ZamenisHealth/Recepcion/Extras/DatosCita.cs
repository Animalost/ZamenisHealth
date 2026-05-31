using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class DatosCita : Forma2
    {
        private static readonly IAgendaC repositorioHorario = new MAgendaC();
        private static readonly IAgenda repositorioHorario2 = new MAgenda();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IFirmasDigitales firmasDigitales = new MFirmasDigitales();
        private static readonly IRcCaja RcCaja = new MRcCaja();

        private int admExport, pacid;
        bool ExisteFirma = false;
        private MensajesGeneral MG;

        public DatosCita(int admision, string FromForm)
        {
            InitializeComponent();
            this.admExport = admision;
            this.textBox8.Text = admision.ToString();        

            if (Preferencias.TabletaFirmas != "A")
            {
                label20.Visible = false;
                pictureBox1.Visible = false;
                button7.Visible = false;
            }

            if (FromForm == "AgendaM-A" || FromForm == "AgendaM-H")
            {
                button1.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
                button7.Visible = false;
            }

            if (FromForm == "OPendConsumer")
            {
                button6.Visible = true;
                button7.Visible = false;
            }
        }

        private void DatosCita_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Datos de Admision";
                this.ImageClose.Visible = false;
                this.ImageMinimize.Visible = false;

                otrosDatosPacienteHorario _list = repositorioHorario.cargarAdmision(Convert.ToInt32(textBox8.Text), "'P','H','A'");                
                if (_list != null)
                {
                    var f = RcCaja.getValRcCaja(Convert.ToInt32(textBox8.Text));
                    int Pagado = f.Valor;

                    textBox1.Text = _list.Pac_PrimerN + " " + _list.Pac_SegundoN + " " + _list.Pac_PrimerA + " " + _list.Pac_SegundoA;
                    textBox2.Text = _list.Pac_TipoId + " " + _list.Pac_IdNum;
                    textBox3.Text = _list.Pac_Telefono + " - " + _list.Pac_TelefonoAux;
                    textBox5.Text = _list.Pac_Email;
                    textBox6.Text = _list.Com_Nombre_SMS;
                    textBox7.Text = _list.Com_Telefono_SMS;
                    textBox9.Text = _list.Hor_Autoriza;
                    textBox10.Text = _list.Hor_ValDerechos;
                    textBox11.Text = _list.Hor_Observacion;
                    textBox12.Text = "$ " + Convert.ToInt32(Pagado).ToString("N2");
                    textBox13.Text = _list.Hor_RegAtn;
                    textBox16.Text = _list.Hor_Usr_Admisiona + " - " + Convert.ToDateTime(_list.Hor_Pac_Llegada).ToString("hh:mm:ss tt");
                    textBox17.Text = Convert.ToDateTime(_list.Hor_Pac_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + " - " + Convert.ToDateTime(_list.Hor_Pac_Hora).ToString("HH:mm tt");
                    textBox18.Text = Convert.ToDateTime(_list.Hor_Pac_Atendido).ToString("HH:mm tt");
                    pacid = Convert.ToInt32(_list.Hor_Pac_Id);

                    textBox15.Text = _list.Hor_CantSesion.ToString();

                    if (textBox9.Text == "") { button1.Enabled = true; }
                    if (textBox10.Text == "") { button3.Enabled = true; }
                    if (textBox13.Text == "") { button5.Enabled = true; }
                    if (textBox12.Text == "") { button4.Enabled = true; }

                    DateTime nacimiento = Convert.ToDateTime(_list.Pac_FechaNto);
                    int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                    textBox4.Text = Convert.ToDateTime(_list.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + " Edad: " + edad.ToString();

                    string canSes = SesionesCuracion(Convert.ToInt32(_list.Hor_Pac_Id),
                                                     Convert.ToDateTime(_list.Hor_Pac_Fecha_Cita),
                                                     _list.Hor_Pac_Tipo_Serv,
                                                     Convert.ToInt32(textBox8.Text));

                    CXN_PACIENTES P =  repositorioPacientes.LlamarPacientebyId(_list.Hor_Pac_Id);      

                    if (P != null)
                    {
                        if (P.Pac_Doble == "S")
                        {
                            label19.Text = label19.Text + "|| DOBLE ESPACIO";
                        }
                        if (P.Pac_2VXS == "S")
                        {
                            label19.Text = label19.Text + " || DOS VECES POR SEMANA";
                        }
                        if (P.Pac_Especial == "S")
                        {
                            label19.Text = label19.Text + " || PACIENTE DE TRATO ESPECIAL";
                        }
                    }
                    else
                    {
                        label19.Text = "";
                    }

                    textBox14.Text = canSes.ToString();

                    CXN_FIRMASDIGITALES getSign = firmasDigitales.getFirmas(admExport);
                    if (getSign != null) 
                    {
                        ExisteFirma = true;

                        using (MemoryStream ms = new MemoryStream(getSign.Firma))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        ExisteFirma = false;

                        string base64String = firmasDigitales.ImageNull();

                        if (base64String.Contains(","))
                        {
                            base64String = base64String.Split(',')[1];
                        }

                        byte[] imageBytes = Convert.FromBase64String(base64String);

                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se logro recuperar el catalogo de datos de la admision: " + textBox8.Text,
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        string SesionesCuracion(int pacid, DateTime fecha, string tserv, int Hoy)
        {
            try
            {
                return repositorioHorario.Calcular3(pacid, tserv);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "ERROR INESPERADO EN SESIONES";
            }
        }
        private void DatosCita_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Escape)
                {
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (button6.Visible == true)
            {
                if (!string.IsNullOrEmpty(textBox9.Text) && !string.IsNullOrEmpty(textBox15.Text))
                {
                    CXN_OPEND oP = new CXN_OPEND
                    {
                        OP_Estado = "H",
                        OP_Cambia = Contenedor.UsuarioLogueado,
                        OP_EstadoChange = DateTime.Now.Date,
                        OP_Adm = this.admExport
                    };
                    
                    repositorioHorario2.OPendUpdate(oP);                                        
                }
            }
            
            this.Dispose();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                InputAutorizacion d = new InputAutorizacion(Convert.ToInt32(textBox8.Text));
                d.ShowDialog();                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string inputPinValidacion = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de pin o la validacion a grabar para esta admision, si no desea grabar ningun valor " +
                            "entonces deje el campo en blanco y haga click en cancelar",
                            "Agregar Pines y/o validaciones",
                                "");
                if (inputPinValidacion != "")
                {
                    bool _add =  repositorioHorario.addValidacionPin(Convert.ToInt32(textBox8.Text), inputPinValidacion);        

                    if (_add != true)
                    {
                        MG.Mensaje = "No se logro agregar la validacion o pin";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else
                    {
                        textBox10.Text = inputPinValidacion.ToString();
                        MG.Mensaje = "Pin y/o validacion ingresado correctamente";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox12.Text) || textBox12.Text == "$ 0,00")
                {
                    RcCaja R = new RcCaja(this.admExport);
                    R.ShowDialog();
                }         
                else
                {
                    MessageBox.Show("Esta admision ya tiene un recibo de caja asociado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string inputPinValidacion = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de validacion a grabar para esta admision, si no desea grabar ningun valor " +
                            "entonces deje el campo en blanco y haga click en cancelar",
                            "Agregar Pines y/o validaciones",
                                "");
                if (inputPinValidacion != "")
                {
                    bool addReg =  repositorioHorario.addRegAtn(Convert.ToInt32(textBox8.Text), inputPinValidacion);
                    
                    if (addReg != true)
                    {
                        MG.Mensaje = "Error ineperado insertando registro";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else
                    {
                        textBox13.Text = inputPinValidacion.ToString();
                        MG.Mensaje = "Validacion ingresada correctamentee";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_OPEND oP = new CXN_OPEND
                {
                    OP_Estado = "E",
                    OP_Cambia = Contenedor.UsuarioLogueado,
                    OP_EstadoChange = DateTime.Now.Date,
                    OP_Adm = this.admExport
                };
                
                repositorioHorario2.OPendUpdate(oP);
                
                this.Dispose();
                this.Close();
            }
            catch (Exception ex) 
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (ExisteFirma == true)
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "Esta admision ya cuenta con una firma valida digital",
                    TipoImagen = 0
                };

                MG.ShowDialog();
            }
            else
            {
                FirmaDigital firmaDigital = new FirmaDigital(admExport, pacid, "DatosCita");
                firmaDigital.ShowDialog();

                this.Dispose();
                this.Close();
            }           
        }
    }
}
