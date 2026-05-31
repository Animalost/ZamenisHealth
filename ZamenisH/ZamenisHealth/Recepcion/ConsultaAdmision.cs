using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Admision;

namespace ZamenisHealth.Recepcion
{
    public partial class ConsultaAdmision : Forma2
    {
        private static readonly IAgendaC repositorioHorario = new MAgendaC();
        private string TipoServ;

        public ConsultaAdmision(string admition)
        {
            InitializeComponent();
            label3.Text = admition.ToString();
        }

        private void ConsultaAdmision_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Gestion de Admisiones";
                
                MensajesGeneral MG = new MensajesGeneral();

                otrosDatosPacienteHorario _dado = repositorioHorario.cargarAdmision(Convert.ToInt32(label3.Text), "'A'");
                
                if (_dado != null)
                {
                    label2.Text = _dado.Hor_Imp_Age;
                    textBox1.Text = _dado.Pac_TipoId + " " + _dado.Pac_IdNum;
                    textBox2.Text = _dado.Hor_Pac_UsrGraba;
                    textBox3.Text = Convert.ToDateTime(_dado.Hor_Pac_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + " - " + Convert.ToDateTime(_dado.Hor_Pac_Hora).ToString("HH:mm tt");
                    textBox4.Text = _dado.Hor_Observacion;
                    textBox5.Text = _dado.Bod_Responsable;

                    TipoServ = _dado.Hor_Pac_Tipo_Serv;

                    DateTime H = DateTime.Now.Date;
                    DateTime fC = Convert.ToDateTime(_dado.Hor_Pac_Fecha_Cita);

                    if (Convert.ToDateTime(H).ToString("dd-MM-yyyy") != Convert.ToDateTime(fC).ToString("dd-MM-yyyy"))
                    {
                        MG.TipoImagen = 0;
                        MG.Mensaje = "Atencion!!! \n\r\r Esta admisionando una cita que no corresponde al dia de HOY calendario.  \n\r\r Esta cita es del dia " + Convert.ToDateTime(fC).ToString("dd-MM-yyyy");
                        MG.ShowDialog();
                    }
                }
                else
                {                    
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Esta admision acaba de cambiar de estado, ya fue admisionada";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (TipoServ == "CU" || TipoServ == "MG")
                {
                    AdmisionesCuraciones a = new AdmisionesCuraciones(Convert.ToInt32(label3.Text));
                    a.ShowDialog();
                }
                else
                {
                    ADMISIONAR();
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CANCELAR();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RETARDO();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                INASISTENCIA();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void INASISTENCIA()
        {
            try
            {
                NovedadesAdmision f = new NovedadesAdmision(Convert.ToInt32(label3.Text), "Inasistencia");

                this.Dispose();
                this.Close();
                
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void RETARDO()
        {
            try
            {
                NovedadesAdmision f = new NovedadesAdmision(Convert.ToInt32(label3.Text), "Retardo");

                this.Dispose();
                this.Close();
                
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void CANCELAR()
        {
            try
            {
                NovedadesAdmision f = new NovedadesAdmision(Convert.ToInt32(label3.Text), "Cancela");

                this.Dispose();
                this.Close();
               
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void ADMISIONAR()
        {
            try
            {
                Admisiones A = new Admisiones(Convert.ToInt32(label3.Text));

                this.Dispose();
                this.Close();
               
                A.ShowDialog();                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void ConsultaAdmision_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Dispose();
                this.Close();
            }
            else if (e.KeyData == Keys.A)
            {
                ADMISIONAR();
            }
            else if (e.KeyData == Keys.C)
            {
                CANCELAR();
            }
            else if (e.KeyData == Keys.R)
            {
                RETARDO();
            }
            else if (e.KeyData == Keys.M)
            {
                INASISTENCIA();
            }
            else
            {
                return;
            }
        }
    }
}
