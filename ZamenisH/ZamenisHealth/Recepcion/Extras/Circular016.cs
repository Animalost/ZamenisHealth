using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class Circular016 : Forma
    {
        private static readonly IAgendaC repositorioHorario = new MAgendaC();

        private int Pac_Id;
        private DateTime FechaCitaAsignada;

        public Circular016(int _pacid, DateTime _fecha)
        {
            InitializeComponent();
            this.Pac_Id = _pacid;
            this.FechaCitaAsignada = _fecha;
        }

        private void Circular016_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Circular 016";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button3_Click;

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                bool insertar = repositorioHorario.insert016(this.Pac_Id, this.FechaCitaAsignada);             

                if (insertar != true)
                {
                    MessageBox.Show("No se logro inserta el dato en el registro", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

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
