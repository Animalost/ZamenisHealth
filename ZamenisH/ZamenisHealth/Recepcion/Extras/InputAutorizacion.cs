using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class InputAutorizacion : Forma
    {        
        private static readonly IAgendaC repositorioAgenda = new MAgendaC();
        private int admision;

        public InputAutorizacion(int Admision)
        {
            InitializeComponent();
            this.admision = Admision;    
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("Debe diligenciar una autorizacion y una cantidad de sesiones", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    bool inserta =  repositorioAgenda.addAutroizacion(this.admision, textBox1.Text, Convert.ToInt32(textBox2.Text));

                    if (inserta == true)
                    {
                        DatosCita f2 = Application.OpenForms.OfType<DatosCita>().SingleOrDefault();
                        f2.textBox9.Text = textBox1.Text;
                        f2.textBox15.Text = textBox2.Text;

                        Agenda f7 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                        f7.RechargeTrueCheck();

                        MessageBox.Show("Autorizacion ingresada correctamente", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No fue posible agregar la autorizacion, intente mas tarde", "Error Interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void InputAutorizacion_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Autorizaciones";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;
        }
    }
}
