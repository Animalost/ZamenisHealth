using System;
using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

using static ZamenisHealth.Clases.ConfigForm;

namespace ZamenisHealth.Fibromialgia.Informes
{
    public partial class EgresoJuntas : BaseForm
    {
        private static readonly IJuntas repoJuntas = new MJuntas();

        private int Admision;

        public EgresoJuntas(int _admision)
        {
            InitializeComponent();
            this.Admision = _admision;
        }
        public EgresoJuntas()
        {
            InitializeComponent();  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione una opcion";
                    MG.ShowDialog();
                    return;
                }

                bool updateEgreso = repoJuntas.UpdateEgreso(this.Admision, comboBox1.Text);
                if (updateEgreso != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro egresar esta junta";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Egresado con exito";
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
    }
}
