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
    public partial class Encuesta1 : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IEncuesta1 repoEncuesta1 = new MEncuesta1();

        private int CodPac;


        public Encuesta1(int CodPaciente)
        {
            InitializeComponent();
            this.CodPac = CodPaciente;
        }

        private void Encuesta1_Load(object sender, EventArgs e)
        {
            Comunes.MensajesGeneral MG = new MensajesGeneral();

            this.Titulo.Text = "Cuestionario Español Sobre el Impacto de la Fibromialgia (FIQ) y SF36";
            this.ImageClose.Visible = false;

            var getPac = repoPac.LlamarPacientebyId(this.CodPac);
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                DialogResult result = MessageBox.Show("¿Desea grabar esta encuesta?",
                                                  "Zamenis Health - Encuestas Fibromialgia",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "" || comboBox5.Text == "" ||
                        comboBox6.Text == "" || comboBox7.Text == "" || comboBox8.Text == "" || comboBox9.Text == "" || comboBox10.Text == "" ||
                        comboBox11.Text == "" || comboBox12.Text == "" || comboBox13.Text == "" || comboBox14.Text == "" || comboBox15.Text == "" ||
                        comboBox16.Text == "" || comboBox17.Text == "" || comboBox18.Text == "" || comboBox19.Text == "" || comboBox20.Text == "" ||
                        comboBox21.Text == "" || comboBox22.Text == "" || comboBox23.Text == "" || comboBox24.Text == "" || comboBox25.Text == "" ||
                        comboBox26.Text == "" || comboBox27.Text == "" || comboBox28.Text == "" || comboBox29.Text == "" || comboBox30.Text == "" ||
                        comboBox31.Text == "" || comboBox32.Text == "" || comboBox33.Text == "" || comboBox34.Text == "" || comboBox35.Text == "" ||
                        comboBox36.Text == "" || comboBox37.Text == "" || comboBox38.Text == "" || comboBox39.Text == "" || comboBox40.Text == "" ||
                        comboBox41.Text == "" || comboBox42.Text == "" || comboBox43.Text == "" || comboBox44.Text == "" || comboBox45.Text == "" ||
                        comboBox46.Text == "" || comboBox47.Text == "" || comboBox48.Text == "" || comboBox49.Text == "" || comboBox50.Text == "" ||
                        comboBox51.Text == "" || comboBox52.Text == "" || comboBox53.Text == "" || comboBox54.Text == "" || comboBox55.Text == "" ||
                        comboBox56.Text == "" || this.CodPac <= 0)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Debe seleccionar una opcion en cada una de las listas desplegables";
                        MG.ShowDialog();
                        return;
                    }

                    FIB_ENCUESTA1 F = new FIB_ENCUESTA1
                    {
                        Pregunta1 = Convert.ToInt32(comboBox1.SelectedIndex - 1),
                        Pregunta2 = Convert.ToInt32(comboBox2.SelectedIndex - 1),
                        Pregunta3 = Convert.ToInt32(comboBox3.SelectedIndex - 1),
                        Pregunta4 = Convert.ToInt32(comboBox4.SelectedIndex - 1),
                        Pregunta5 = Convert.ToInt32(comboBox5.SelectedIndex - 1),
                        Pregunta6 = Convert.ToInt32(comboBox6.SelectedIndex - 1),
                        Pregunta7 = Convert.ToInt32(comboBox7.SelectedIndex - 1),
                        Pregunta8 = Convert.ToInt32(comboBox8.SelectedIndex - 1),
                        Pregunta9 = Convert.ToInt32(comboBox9.SelectedIndex - 1),
                        Pregunta10 = Convert.ToInt32(comboBox10.SelectedIndex - 1),
                        Pregunta11 = Convert.ToInt32(comboBox11.SelectedIndex - 1),
                        Pregunta12 = Convert.ToInt32(comboBox12.SelectedIndex - 1),
                        Pregunta13 = Convert.ToInt32(comboBox13.SelectedIndex - 1),
                        Pregunta14 = Convert.ToInt32(comboBox14.SelectedIndex - 1),
                        Pregunta15 = Convert.ToInt32(comboBox15.SelectedIndex - 1),
                        Pregunta16 = Convert.ToInt32(comboBox16.SelectedIndex - 1),
                        Pregunta17 = Convert.ToInt32(comboBox17.SelectedIndex - 1),
                        Pregunta18 = Convert.ToInt32(comboBox18.SelectedIndex - 1),
                        Pregunta19 = Convert.ToInt32(comboBox19.SelectedIndex - 1),
                        Pregunta20 = Convert.ToInt32(comboBox20.SelectedIndex - 1),
                        Pregunta21 = Convert.ToInt32(comboBox21.SelectedIndex - 1),
                        Pregunta22 = Convert.ToInt32(comboBox22.SelectedIndex - 1),
                        Pregunta23 = Convert.ToInt32(comboBox23.SelectedIndex - 1),
                        Pregunta24 = Convert.ToInt32(comboBox24.SelectedIndex - 1),
                        Pregunta25 = Convert.ToInt32(comboBox25.SelectedIndex - 1),
                        Pregunta26 = Convert.ToInt32(comboBox26.SelectedIndex - 1),
                        Pregunta27 = Convert.ToInt32(comboBox27.SelectedIndex - 1),
                        Pregunta28 = Convert.ToInt32(comboBox28.SelectedIndex - 1),
                        Pregunta29 = Convert.ToInt32(comboBox29.SelectedIndex - 1),
                        Pregunta30 = Convert.ToInt32(comboBox30.SelectedIndex - 1),
                        Pregunta31 = Convert.ToInt32(comboBox31.SelectedIndex - 1),
                        Pregunta32 = Convert.ToInt32(comboBox32.SelectedIndex - 1),
                        Pregunta33 = Convert.ToInt32(comboBox33.SelectedIndex - 1),
                        Pregunta34 = Convert.ToInt32(comboBox34.SelectedIndex - 1),
                        Pregunta35 = Convert.ToInt32(comboBox35.SelectedIndex - 1),
                        Pregunta36 = Convert.ToInt32(comboBox36.SelectedIndex - 1),
                        Pregunta37 = Convert.ToInt32(comboBox37.SelectedIndex - 1),
                        Pregunta38 = Convert.ToInt32(comboBox38.SelectedIndex - 1),
                        Pregunta39 = Convert.ToInt32(comboBox39.SelectedIndex - 1),
                        Pregunta40 = Convert.ToInt32(comboBox40.SelectedIndex - 1),
                        Pregunta41 = Convert.ToInt32(comboBox41.SelectedIndex - 1),
                        Pregunta42 = Convert.ToInt32(comboBox42.SelectedIndex - 1),
                        Pregunta43 = Convert.ToInt32(comboBox43.SelectedIndex - 1),
                        Pregunta44 = Convert.ToInt32(comboBox44.SelectedIndex - 1),
                        Pregunta45 = Convert.ToInt32(comboBox45.SelectedIndex - 1),
                        Pregunta46 = Convert.ToInt32(comboBox46.SelectedIndex - 1),
                        Pregunta47 = Convert.ToInt32(comboBox47.SelectedIndex - 1),
                        Pregunta48 = Convert.ToInt32(comboBox48.SelectedIndex - 1),
                        Pregunta49 = Convert.ToInt32(comboBox49.SelectedIndex - 1),
                        Pregunta50 = Convert.ToInt32(comboBox50.SelectedIndex - 1),
                        Pregunta51 = Convert.ToInt32(comboBox51.SelectedIndex - 1),
                        Pregunta52 = Convert.ToInt32(comboBox52.SelectedIndex - 1),
                        Pregunta53 = Convert.ToInt32(comboBox53.SelectedIndex - 1),
                        Pregunta54 = Convert.ToInt32(comboBox54.SelectedIndex - 1),
                        Pregunta55 = Convert.ToInt32(comboBox55.SelectedIndex - 1),
                        Pregunta56 = Convert.ToInt32(comboBox56.SelectedIndex - 1),
                        fechaEncuesta = Convert.ToDateTime(dateTimePicker1.Value.Date),
                        FechaEncuesta = Convert.ToDateTime(dateTimePicker1.Value),
                        IdPaciente = Convert.ToInt32(this.CodPac),
                        UsuarioRegistra = Comunes.Contenedor.UsuarioLogueado,
                    };

                    bool _grabar = repoEncuesta1.registrarEncuesta1(F);
                    if (_grabar != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se ha logrado grabar la encuesta";
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Grabado Exitosamente";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
