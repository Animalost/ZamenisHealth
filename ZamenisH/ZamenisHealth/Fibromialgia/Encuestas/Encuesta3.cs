using System;
using System.Windows.Forms;
using Domain;
using Domain.Fibromialgia;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Persistence.Fibromialgia.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Fibromialgia.Encuestas
{
    public partial class Encuesta3 : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IEncuesta3 repoEncuesta3 = new MEncuesta3();


        private int CodePac;

        public Encuesta3(int cod)
        {
            InitializeComponent();
            this.CodePac = cod;
        }     
        
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Al cerrar la encuesta no grabara nada.  ¿Desea cerrarla ralmente?",
                                               "Zamenis Health - Encuestas Fibromialgia",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Dispose();
                this.Close();
            }
        }

        private void Encuesta3_Load(object sender, EventArgs e)
        {
            Comunes.MensajesGeneral MG = new MensajesGeneral();

            this.Titulo.Text = "Encuesta de Satisfaccion";
            this.ImageClose.Visible = false;

            var getPac = repoPac.LlamarPacientebyId(this.CodePac);
            if (getPac != null)
            {
                label3.Text = getPac.Pac_PrimerA + " " + getPac.Pac_SegundoA + " " + getPac.Pac_PrimerN + " " + getPac.Pac_SegundoN;
                return;
            }

            MG.TipoImagen = 1000;
            MG.Mensaje = "El pacinete no existe";
            MG.ShowDialog();

            this.Dispose();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (this.CodePac <= 0 || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "" || comboBox5.Text == "" ||
                    comboBox6.Text == "" || comboBox7.Text == "" || comboBox8.Text == "" || comboBox9.Text == "" || comboBox10.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar las opciones de las listas desplegables";
                    MG.ShowDialog();
                    return;
                }

                FIB_ENCUESTA3 F = new FIB_ENCUESTA3
                {
                    IdPaciente = this.CodePac,
                    fechaEncuesta = Convert.ToDateTime(dateTimePicker1.Value),
                    FechaEncuesta = Convert.ToDateTime(dateTimePicker1.Value),
                    UsuarioRegistra = Contenedor.UsuarioLogueado,
                    Estado = "V",
                    Pregunta1 = comboBox1.Text,
                    Pregunta2 = comboBox2.Text,
                    Pregunta3 = comboBox3.Text,
                    Pregunta4 = comboBox4.Text,
                    Pregunta5 = comboBox5.Text,
                    Pregunta6 = comboBox6.Text,
                    Pregunta7 = comboBox7.Text,
                    Pregunta8 = comboBox8.Text,
                    Pregunta9 = comboBox9.Text,
                    Pregunta10 = comboBox10.Text
                };

                bool save = repoEncuesta3.registrarEncuesta(F);
                if (save != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro grabar la encuesta";
                    MG.ShowDialog();
                    return;
                }

                MG.TipoImagen = 3;
                MG.Mensaje = "Grabado Exitosamente";
                MG.ShowDialog();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
