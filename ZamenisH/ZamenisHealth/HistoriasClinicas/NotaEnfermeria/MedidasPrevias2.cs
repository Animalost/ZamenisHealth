using Domain.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using ZamenisHealth.Comunes;
using static ZamenisHealth.Clases.ConfigForm;

namespace ZamenisHealth.HistoriasClinicas.NotaEnfermeria
{
    public partial class MedidasPrevias2 : BaseForm
    {
        private readonly INotasCuracion notasCuracion;
        private int Posision;
        MensajesGeneral MG;

        public MedidasPrevias2(int posision)
        {
            InitializeComponent();
            Posision = posision;
            notasCuracion = new MNotasCuracion();
        }

        private void MedidasPrevias2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Medida seleccionada";
            CargarSeleccion();
        }

        void CargarSeleccion()
        {
            try
            {
                CXN_NOTASMED verHeridaCompleta = notasCuracion.VerPosisionHerida(Posision);
                if (verHeridaCompleta != null)
                {
                    textBox1.Text = verHeridaCompleta.Largo.ToString();
                    textBox2.Text = verHeridaCompleta.Ancho.ToString();
                    textBox3.Text = verHeridaCompleta.Profundidad.ToString();
                    textBox4.Text = verHeridaCompleta.Total.ToString();
                    textBox5.Text = verHeridaCompleta.txtLocalizacion.ToString();
                    textBox8.Text = verHeridaCompleta.txtGranulacion.ToString();
                    textBox7.Text = verHeridaCompleta.txtFibrina.ToString();
                    textBox6.Text = verHeridaCompleta.txtNecroticoHumedo.ToString();
                    textBox11.Text = verHeridaCompleta.txtNecroticoSeco.ToString();
                    textBox10.Text = verHeridaCompleta.txtEpitelizacion.ToString();
                    textBox9.Text = verHeridaCompleta.txtOtros.ToString();
                    textBox12.Text = verHeridaCompleta.txtOtros.ToString();
                    textBox13.Text = verHeridaCompleta.Evolucion.ToString();
                    textBox14.Text = verHeridaCompleta.NovedadHerida.ToString();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se logor ver esta herida";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.ToString()); 
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
