using Domain.CXN;
using System;
using System.Windows.Forms;
using static ZamenisHealth.Clases.ConfigForm;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class MedidasEnfermeria2 : BaseForm
    {
        private CXN_NOTASMED NotasMed;

        public MedidasEnfermeria2(CXN_NOTASMED notasMed)
        {
            InitializeComponent();
            NotasMed = notasMed;
        }

        private void MedidasEnfermeria2_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Datos de la medida seleccionada";

                textBox1.Text = NotasMed.Ancho.ToString("N2");
                textBox2.Text = NotasMed.Largo.ToString("N2");
                textBox3.Text = NotasMed.Profundidad.ToString("N2");
                textBox4.Text = NotasMed.Total.ToString("N2");
                richTextBox1.Text = NotasMed.Evolucion.ToString();
                richTextBox2.Text = NotasMed.NovedadHerida.ToString();
                textBox8.Text = NotasMed.txtGranulacion.ToString();
                textBox6.Text = NotasMed.txtNecroticoHumedo.ToString();
                textBox7.Text = NotasMed.txtEpitelizacion.ToString();
                textBox5.Text = NotasMed.txtNecroticoSeco.ToString();
                richTextBox3.Text = NotasMed.txtOtros.ToString();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }            
        }
    }
}
