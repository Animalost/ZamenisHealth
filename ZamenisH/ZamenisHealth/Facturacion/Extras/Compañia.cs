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
    public partial class Compañia : Forma
    {
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly ICargos repoCargos = new MCargos();

        private int Admision, Posision;

        public Compañia(int admision, int Posision)
        {
            InitializeComponent();
            this.Admision = admision;
            this.Posision = Posision;

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
                    MG.Mensaje = "Debe seleccionar un prestador valido";
                    MG.ShowDialog();
                    return;
                }

                CXN_CIA getCia = repoCia.getPrestadorbyName(comboBox1.Text);
                if (getCia != null)
                {
                    bool _updateCia = repoCargos.updateCia(getCia.Com_Identificador, this.Posision);
                    if (_updateCia == true)
                    {
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
        private void Compañia_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Cambio de Prestador";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button1_Click;

                List <CXN_CIA> getPrestadores = repoCia.getAllCompañias();
                if (getPrestadores != null) 
                {
                    foreach (CXN_CIA i in getPrestadores)
                    {
                        comboBox1.Items.Add(i.Com_Nombre);
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
