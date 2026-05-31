using Domain;
using FormAndControls;
using Persistence;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes
{
    public partial class Inactivate : Forma2
    {     
        public Inactivate()
        {
            InitializeComponent();
        }
       
        private void AbrirFormEnPanel2(object Formhijo)
        {
            if (this.panel1.Controls.Count > 0)
                this.panel1.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(fh);
            this.panel1.Tag = fh;
            panel1.Visible = true;
            fh.Show();
        }
        private void Inactivate_Load(object sender, EventArgs e)
        {
            try
            {
                ImageClose.Visible = false;
                ImageMinimize.Visible = false;
                Titulo.Text = "Inactividad Detectada...";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                
                AbrirFormEnPanel2(new Extras.Inactivate2());
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                this.Dispose();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Extras.Inactivate2 f29 = Application.OpenForms.OfType<Extras.Inactivate2>().SingleOrDefault();
                f29.T.Stop();
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea salir de la aplicacion? perdera los datos no guardados",
                                                "Zamenis Health - Cerrar Aplicacion",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
            if (result == DialogResult.No)
            {
                Extras.Inactivate2 f29 = Application.OpenForms.OfType<Extras.Inactivate2>().SingleOrDefault();
                f29.T.Stop();
                this.Dispose();
                this.Close();
            }
        }

       
    }
}
