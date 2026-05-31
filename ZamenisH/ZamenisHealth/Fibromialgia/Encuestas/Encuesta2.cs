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
    public partial class Encuesta2 : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IEncuesta2 repoEncuesta2 = new MEncuesta2();

        private int CodPac;
        private int Sumatoria = 0;

        public Encuesta2(int cod)
        {
            InitializeComponent();
            this.CodPac = cod;
        }

        private void Encuesta2_Load(object sender, EventArgs e)
        {
            Comunes.MensajesGeneral MG = new MensajesGeneral();

            this.Titulo.Text = "Indice de Dolor Generalizado WCI e Indice de Gravedad de Sintomas IGS Parte 1";
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
        void SumatoriaChexks(CheckBox C)
        {
            try
            {
                if (C.Checked == true) { Sumatoria = Sumatoria + 1; } else { Sumatoria = Sumatoria - 1; }
                if (Sumatoria <= 0) { Sumatoria = 0; }
                label10.Text = Sumatoria.ToString();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox1);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox2);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox3);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox4);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox5);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox6);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox7);
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox14);
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox13);
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox12);
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox11);
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox10);
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox9);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox8);
        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox19);
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox18);
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox17);
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox16);
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            SumatoriaChexks(checkBox15);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sumatoria2(comboBox1);
        }
        void Sumatoria2(ComboBox C)
        {
            int V = Convert.ToInt32(comboBox1.SelectedIndex - 1) + Convert.ToInt32(comboBox2.SelectedIndex - 1) + Convert.ToInt32(comboBox3.SelectedIndex - 1);
            label16.Text = V.ToString();
            if (Convert.ToInt32(label16.Text) <= 0) { label16.Text = "0"; }
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
                    if (this.CodPac <= 0 || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Debe seleccionar 1 opcion de cada lista desplegable";
                        MG.ShowDialog();
                        return;
                    }

                    FIB_ENCUESTA2 E2 = new FIB_ENCUESTA2
                    {
                        IdPaciente = this.CodPac,
                        FechaEncuesta = Convert.ToDateTime(dateTimePicker1.Value),
                        fechaEncuesta = Convert.ToDateTime(dateTimePicker1.Value),
                        Estado = "V",
                        UsuarioRegistra = Contenedor.UsuarioLogueado,
                        Pregunta1 = (checkBox1.Checked == true) ? checkBox1.Text : "",
                        Pregunta2 = (checkBox2.Checked == true) ? checkBox2.Text : "",
                        Pregunta3 = (checkBox3.Checked == true) ? checkBox3.Text : "",
                        Pregunta4 = (checkBox4.Checked == true) ? checkBox4.Text : "",
                        Pregunta5 = (checkBox5.Checked == true) ? checkBox5.Text : "",
                        Pregunta6 = (checkBox6.Checked == true) ? checkBox6.Text : "",
                        Pregunta7 = (checkBox7.Checked == true) ? checkBox7.Text : "",
                        Pregunta8 = (checkBox8.Checked == true) ? checkBox8.Text : "",
                        Pregunta9 = (checkBox9.Checked == true) ? checkBox9.Text : "",
                        Pregunta10 = (checkBox10.Checked == true) ? checkBox10.Text : "",
                        Pregunta11 = (checkBox11.Checked == true) ? checkBox11.Text : "",
                        Pregunta12 = (checkBox12.Checked == true) ? checkBox12.Text : "",
                        Pregunta13 = (checkBox13.Checked == true) ? checkBox13.Text : "",
                        Pregunta14 = (checkBox14.Checked == true) ? checkBox14.Text : "",
                        Pregunta15 = (checkBox15.Checked == true) ? checkBox15.Text : "",
                        Pregunta16 = (checkBox16.Checked == true) ? checkBox16.Text : "",
                        Pregunta17 = (checkBox17.Checked == true) ? checkBox17.Text : "",
                        Pregunta18 = (checkBox18.Checked == true) ? checkBox18.Text : "",
                        Pregunta19 = (checkBox19.Checked == true) ? checkBox19.Text : "",
                        ResultadoP19 = Convert.ToInt32(label10.Text),
                        Fatiga = comboBox1.SelectedIndex - 1,
                        Sueño = comboBox2.SelectedIndex - 1,
                        Trastorno = comboBox3.SelectedIndex - 1,
                        ResultadoP3 = Convert.ToInt32(label16.Text)
                    };

                    bool save = repoEncuesta2.registrarEncuesta(E2);
                    if (save != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro grabar la encuesta";
                        MG.ShowDialog();
                        return;
                    }

                    MG.TipoImagen = 3;
                    MG.Mensaje = "Grabado exitosamente";
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sumatoria2(comboBox2);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sumatoria2(comboBox3);
        }
    }
}
