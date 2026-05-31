using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class EncuestaSatisfaccion2 : Forma2
    {
        private static readonly IEncuestasSatis repoEncu = new MEncuestasSatis();

        private int Admision;

        public EncuestaSatisfaccion2(int _admision)
        {
            InitializeComponent();
            this.Admision = _admision;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (richTextBox1.Text.Length <= 20)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La observacion que ha descrito no es clara.  Por favor sea mas especifico en la razon de la no realizacion de la encuesta";
                    MG.ShowDialog();
                    return;
                }

                CXN_ENCUESTASATIS en = new CXN_ENCUESTASATIS
                {
                    Admision = this.Admision,
                    Estado = "X",
                    Observacion = richTextBox1.Text
                };

                if (repoEncu.GrabarNoRealizacionEncuesta(en) == true)
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Grabado, puede continuar con la historia clinica";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No fue posible grabar la razon.  Vuelva a intentar o contacte a soporte";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void EncuestaSatisfaccion2_Load(object sender, EventArgs e)
        {
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            Titulo.Text = "Motivo";
            richTextBox1.MaxLength = 990;

            ImageClose.Visible = false;
            ImageMinimize.Visible = false;
        }
    }
}
