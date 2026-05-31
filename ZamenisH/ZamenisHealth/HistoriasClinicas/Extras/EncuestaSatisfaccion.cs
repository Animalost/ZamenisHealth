using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class EncuestaSatisfaccion : Forma2
    {
        private static readonly IEncuestasSatis repoEncu = new MEncuestasSatis();
        private static readonly IAgendaC repoAgendaC = new MAgendaC();

        private int Admision;

        public EncuestaSatisfaccion(int _admision)
        {
            InitializeComponent();
            this.Admision = _admision;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea cancelar esta encuesta? Al cancelar la encuesta debe escribir una razon valida para no realizarla. \n\r " +
                                                  "Una vez marque SI no habra vuelta atras para realizar la encuesta",
                                                  "Cancelar Encuesta Obligatoria",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                EncuestaSatisfaccion2 e2 = new EncuestaSatisfaccion2(this.Admision);
                e2.ShowDialog();

                this.Dispose();
                this.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.SelectedIndex == 0 || comboBox2.SelectedIndex == 0 || comboBox3.SelectedIndex == 0 ||
                    comboBox4.SelectedIndex == 0 || comboBox5.SelectedIndex == 0 || comboBox6.SelectedIndex == 0) 
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar una respuesta obligatoria a cada pregunta";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" ||
                    comboBox4.Text == "" || comboBox5.Text == "" || comboBox6.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar una respuesta obligatoria a cada pregunta";
                    MG.ShowDialog();
                    return;
                }

                DateTime Hoy = repoAgendaC.cargarAdmision(this.Admision, "'P','H'").Hor_Pac_Fecha_Cita;

                bool ConsActActual = repoEncu.getCantEncuestaCU("CU", Capitalize(Hoy.Month), Hoy.Year);
                if (ConsActActual == false) 
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Ya se han completado las encuestas por este mes.  Muchas Gracias";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    CXN_ENCUESTASATIS en = new CXN_ENCUESTASATIS
                    {
                        Admision = this.Admision,
                        Estado = "H",
                        Observacion = textBox1.Text,
                        P1 = comboBox1.SelectedIndex,
                        P2 = comboBox2.SelectedIndex,
                        P3 = comboBox3.SelectedIndex,
                        P4 = comboBox4.SelectedIndex,
                        P5 = comboBox5.SelectedIndex,
                        P6 = comboBox6.SelectedIndex,
                        Lugar = "Consultorio"
                    };

                    if (repoEncu.GrabarEncuesta(en) == true)
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Encuesta grabada.  Puede continuar con la historia clinica";
                        MG.ShowDialog();

                        CXN_CONFENCUESTA cE = new CXN_CONFENCUESTA
                        {
                            Mes = Capitalize(Hoy.Month),
                            Año = Hoy.Year,
                            Servicio = "CU"
                        };

                        int NueValActual = repoEncu.getActual(cE) + 1;

                        cE.Actual = NueValActual;

                        repoEncu.updateActual(cE);

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro grabar la encuesta, por favor vuelva a intentar si presenta nuevamente los inconvenientes contacte a soporte";
                        MG.ShowDialog();
                    }
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        string Capitalize(int Mes)
        {
            switch (Mes)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";
                default:
                    return "";
            }
        }
        private void EncuestaSatisfaccion_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Encuesta de Satisfaccion";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            Titulo.Visible = false;
            ImageClose.Visible = false;
            ImageMinimize.Visible = false;
        }
    }
}
