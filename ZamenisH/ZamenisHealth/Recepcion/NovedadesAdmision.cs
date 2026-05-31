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
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion
{
    public partial class NovedadesAdmision : Forma
    {
        private static readonly IAgendaC repositorioFechasAgenda = new MAgendaC();
        private static readonly IAgenda repositorioHorario = new MAgenda();

        private int Admi;
        private string Tipo_Seleccion;

        public NovedadesAdmision(int _admi, string _tipoadmision)
        {
            InitializeComponent();
            this.Admi = _admi;
            this.Tipo_Seleccion = _tipoadmision;
        }
        private void CargarCombos()
        {
            try
            {
                List<string> _stRazones =  repositorioFechasAgenda.CargarRazones();

                if (_stRazones != null)
                {
                    foreach (var i in _stRazones)
                    {
                        comboBox6.Items.Add(i);
                        comboBox5.Items.Add(i);
                        comboBox4.Items.Add(i);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Carga_Datos_Admision()
        {
            try
            {
                CXN_HORARIO _datos = repositorioHorario.DatosforMailSMS(Admi);

                if (_datos != null)
                {
                    textBox1.Text = Convert.ToString(Admi);
                    textBox2.Text = _datos.Hor_Imp_Age;
                    textBox3.Text = Convert.ToDateTime(_datos.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]) +
                        " - " + Convert.ToDateTime(_datos.Hor_Pac_Hora_Cita).ToString("HH:mm tt");
                    textBox4.Text = _datos.Hor_Pac_Razon;
                    textBox5.Text = _datos.Com_Nombre;
                }
                else
                {
                    MessageBox.Show("Hay un error con esta admision, consulte con la administracion",
                          "Error General",
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
        private void NovedadesAdmision_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Gestionar Cita";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnConfirmar = new ToolStripButton();
                btnConfirmar = createToolButton("Confirmar");
                MenuLateral.Items.Add(btnConfirmar);
                btnConfirmar.Click += button1_Click;

                ConfigForm.SoloNumeros(textBox17);

                
                Cancelacion.Location = new Point(141, 305);
                Retardo.Location = new Point(141, 305);
                Inasistencia.Location = new Point(141, 305);

                Inasistencia.Size = new Size(873, 180);
                Retardo.Size = new Size(873, 180);
                Cancelacion.Size = new Size(873, 180);

                Carga_Datos_Admision();
                CargarCombos();

                switch (Tipo_Seleccion)
                {
                    case "Cancela":
                        Cancelacion.Visible = true;
                        Retardo.Visible = false;
                        Inasistencia.Visible = false;
                        break;

                    case "Retardo":
                        Retardo.Visible = true;
                        Cancelacion.Visible = false;
                        Inasistencia.Visible = false;
                        break;

                    case "Inasistencia":
                        Inasistencia.Visible = true;
                        Retardo.Visible = false;
                        Cancelacion.Visible = false;
                        break;

                    default:
                        MessageBox.Show("Inconveniente con esta admision, cierre esta pantalla y vuelva a ingresar",
                            "Error General",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Cancelar_Cita()
        {
            try
            {
                if (comboBox4.Text == "")
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = "Seleccione un motivo";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                repositorioHorario.CancelacionInterna(textBox16.Text, Comunes.Contenedor.UsuarioLogueado, Admi, comboBox4.Text);
                               
                Agenda f7 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();
                f7.RechargeTrueCheck();

                MessageBox.Show("Cita cancelada exitosamente!!", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                this.Dispose();
                this.Close();
                return;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                switch (Tipo_Seleccion)
                {
                    case "Cancela":
                        Cancelar_Cita();
                        break;

                    case "Retardo":
                        Retardo_Cita();
                        break;

                    case "Inasistencia":
                        Inasistencia_Cita();
                        break;

                    default:
                        MessageBox.Show("Inconveniente con esta admision, cierre esta pantalla y vuelva a ingresar",
                            "Error General",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Inasistencia_Cita()
        {
            try
            {
                if (comboBox6.Text == "")
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = "Seleccione un motivo";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                repositorioHorario.Inasistencia_Cita(Admi, comboBox6.Text);

                ConsultaAdmision f = new ConsultaAdmision(textBox1.Text);

                this.Dispose();
                this.Close();
                
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return;
            }
        }
        private void Retardo_Cita()
        {
            try
            {
                if (textBox17.Text == "")
                {
                    MessageBox.Show("Debe escribir el tiempo de retardo en minutos");
                    return;
                }

                
                    repositorioHorario.Retardo_Cita(textBox17.Text, comboBox5.Text, Admi);
                                

                Agenda f7 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                f7.RechargeTrueCheck();

                MessageBox.Show("Retardo ingresado", "Correcto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                ConsultaAdmision f = new ConsultaAdmision(textBox1.Text);

                this.Dispose();
                this.Close();
                
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return;
            }
        }   
        private void NovedadesAdmision_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConsultaAdmision f = new ConsultaAdmision(textBox1.Text);
            f.ShowDialog();
        }
    }
}
