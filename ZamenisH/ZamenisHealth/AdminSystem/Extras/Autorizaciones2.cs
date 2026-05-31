using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Medicina;

namespace ZamenisHealth.AdminSystem.Extras
{
    public partial class Autorizaciones2 : Forma
    {
        private static readonly IOrdenes repoO = new MOrdenes();

        private int Orden, Cia;
        ToolStripButton btnBuscar;

        public Autorizaciones2(int orden, int cia)
        {
            InitializeComponent();
            this.Orden = orden;
            this.Cia = cia;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "")
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Digite un numero de autorizacion";
                    MG.ShowDialog();
                    return;
                }

                bool searchA = repoO.SearchAutorization(textBox1.Text);
                if (searchA == true)
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este numero de autorizacion ya esta en uso con otra orden medica de este u otro paciente";
                    MG.ShowDialog();
                    return;
                }

                bool insertAut = repoO.updateAutorizacion(this.Orden, textBox1.Text, this.Cia);
                if (insertAut =! true) 
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro agregar la autorizacion";
                    MG.ShowDialog();
                    return;
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Agregado";
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bool update = repoO.updateRadicar(this.Orden, this.Cia);
                if (update =! true)
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro confirmar la radicacion";
                    MG.ShowDialog();
                    return;
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Radicado";
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

        private void Autorizaciones2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Autorizaciones";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += toolStripButton2_Click;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                string Tip = repoO.SearchTypeOrden(this.Orden, this.Cia);

                switch (Tip)
                {
                    case "S":
                        var CopyServices = repoO.Generar_OrdenMedica(this.Orden, this.Cia, Comunes.Contenedor.UsuarioLogueado);
                        if (CopyServices != null)
                        {
                            OrdenesMedicasT OT = new OrdenesMedicasT(this.Cia,
                                                             Convert.ToInt32(this.Orden),
                                                             Comunes.Contenedor.UsuarioLogueado);

                            OT.ShowDialog();
                        }
                        break;


                    case "M":
                        var CopyMedicamento = repoO.Genera_Orden_Medicamento(this.Orden, this.Cia, Contenedor.UsuarioLogueado);
                        if (CopyMedicamento == null)
                        {
                            MessageBox.Show("No se logro exportar la orden",
                                "Error generico",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        ConfigForm.GenerarReportViewer("DataSet_OM",
                                              "ZamenisHealth.Reportes.RDLC_OrdenesMedicamentos.rdlc",
                                              CopyMedicamento);
                        break;


                    default:
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro exportar la orden";
                        MG.ShowDialog();
                        break;
                }             
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
