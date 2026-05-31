using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Festivos : Forma
    {
        private static readonly IAgenda repoAgenda = new MAgenda();
        public Festivos()
        {
            InitializeComponent();          
        }
       
        private void Festivos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Adherencia";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar motivo del festivo"); return; }

                repoAgenda.addFestivo(dateTimePicker1.Value, textBox1.Text);
                MessageBox.Show("Agregado a la Agenda");
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
