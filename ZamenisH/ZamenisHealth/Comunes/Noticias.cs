using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;

namespace ZamenisHealth.Comunes
{
    public partial class Noticias : Forma2
    {
        private readonly static IConfSystem repoSys = new MConfSystem();

        public Noticias()
        {
            InitializeComponent();
        
        }

        private void Noticias_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Noticias";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                pictureBox1.Image = Image.FromFile(repoSys.getDatoNoticias().Ruta);
                                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
