using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Horarios_2 : ConfigForm.BaseForm
    {
        private static readonly IDisponibilidad repoDispo = new MDisponibilidad();

        public string Tipo_Horario;
        public Horarios_2()
        {
            InitializeComponent();
        }

        private void Horarios_2_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Horarios";
                var getSeleccion = repoDispo.ConsultarHora(Convert.ToInt32(Tipo_Horario));
                if (getSeleccion.Habilita != "")
                {
                    dateTimePicker1.Value = Convert.ToDateTime(getSeleccion.Hora);
                    comboBox1.Text = getSeleccion.Habilita;
                }
                else
                {
                    comboBox1.Text = "";
                    MessageBox.Show("No se cargo el catalogo de identificadores de hora");
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
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (comboBox1.Text == "") { MessageBox.Show("Debe seleccionar un estado del horario"); return; }

                CXN_DISPONIBILIDAD_2 D = new CXN_DISPONIBILIDAD_2
                {
                    Id = Convert.ToInt32(Tipo_Horario),
                    Hora = Convert.ToDateTime(dateTimePicker1.Value),
                    Habilita = comboBox1.Text
                };

                bool _update = repoDispo.UpdateHora(D);
                if (_update != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error interno.  No se logro actualizar la hora.  Intente mas tarde";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Actualizado";
                    MG.ShowDialog();

                    this.Close();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
