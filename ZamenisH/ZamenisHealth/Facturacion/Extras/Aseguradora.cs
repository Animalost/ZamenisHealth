using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion.Extras
{
    public partial class Aseguradora : Forma
    {
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly IAgenda repoAgenda = new MAgenda();
        private int Admision, Posision;

        public Aseguradora(int admision, int posision)
        {
            InitializeComponent();
            this.Admision = admision;
            this.Posision = posision;

            this.label2.Text = this.Admision.ToString();
            this.label3.Text = this.Posision.ToString();
        }

        void Cerrar()
        {            
            this.Dispose();
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar una aseguradora valida";
                    MG.ShowDialog();
                    return;
                }

                CXN_ASEGURADORA getAse = repoAse.getInfoFromAsebyName(comboBox1.Text);
                if (getAse != null)
                {
                    bool _updateAse = repoCargos.updateAse(getAse.Ase_Identificador, this.Posision);
                    if (_updateAse == true)
                    {
                        repoAgenda.updateAseguradoraFromCargo(getAse.Ase_Identificador, Convert.ToInt32(label2.Text));

                        MensajesGeneral MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Hecho";
                        MG.ShowDialog();                       

                        Cerrar();
                    }
                    else
                    {
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Hubo un error inesperado";
                        MG.ShowDialog();
                    }
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Hubo un error inesperado";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Aseguradora_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Cambiar Aseguradora";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button1_Click;

                List <CXN_ASEGURADORA> getAse = repoAse.getAseguradoras();
                if (getAse  != null) 
                {
                    foreach (CXN_ASEGURADORA i in getAse)
                    {
                        comboBox1.Items.Add(i.Ase_Descripcion);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
