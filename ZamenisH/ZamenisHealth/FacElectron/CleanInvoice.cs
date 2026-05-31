using DocumentosElectronicos.Servicio;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron
{
    public partial class CleanInvoice : Forma
    {
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();

        private MensajesGeneral MG;
        private int Cia;

        public CleanInvoice()
        {
            InitializeComponent();
        }

        private void CleanInvoice_Load(object sender, EventArgs e)
        {
            
            List<CXN_CIA> compañias = repoCia.getAllCompañias();
            if (compañias != null)
            {
                foreach (CXN_CIA c in compañias)
                {
                    comboBox2.Items.Add(c.Com_Nombre);
                }

                comboBox2.SelectedIndex = 0; 
            }

            Titulo.Text = "Limpiar Factura";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnReFacturar = new ToolStripButton();
            btnReFacturar = createToolButton("ReFacturar");
            MenuLateral.Items.Add(btnReFacturar);
            btnReFacturar.Click += button2_Click;

                ToolStripButton btnLimpiar = new ToolStripButton();
            btnLimpiar = createToolButton("Limpiar");
            MenuLateral.Items.Add(btnLimpiar);
            btnLimpiar.Click += button1_Click;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Antes de limpiar considere refacturar nuevamente ¿Desea continuar con la limpieza?",
                                                 "Zamenis Health - Eliminar Radicacion DIAN",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (comboBox1.Text == "Factura Aseguradora y Otras")
                    {
                        int FacZamenis = ActualizarDocumentos.GetFacZamenis(textBox1.Text, Cia);
                        if (FacZamenis > 0)
                        {
                            bool cleanFac = ActualizarDocumentos.CleanInvoice(textBox1.Text, Cia, "Aseguradoras", FacZamenis);
                            if (cleanFac == false)
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se logro limpiar esta factura";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "Factura Liberada";
                                MG.TipoImagen = 3;
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se encuentra la Factura Electronica";
                            MG.TipoImagen = 0;
                            MG.ShowDialog();
                        }
                    }
                    else if (comboBox1.Text == "Factura de Caja")
                    {
                        bool cleanFac = ActualizarDocumentos.CleanInvoice(textBox1.Text, Cia, "Caja", null);
                        if (cleanFac == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro limpiar esta factura";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Factura Liberada";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                        }
                    }
                    if (comboBox1.Text == "Factura de Ventas")
                    {
                        bool cleanFac = ActualizarDocumentos.CleanInvoice(textBox1.Text, Cia, "Ventas", null);
                        if (cleanFac == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro limpiar esta factura";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Factura Liberada";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Seleccione una opcion valida";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cia = repoCia.getPrestadorbyName(comboBox2.Text).Com_Identificador;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea refacturar nuevamente?  Esta opcion solo esta disponible para la facturacion de Aseguradoras",
                                                 "Zamenis Health - Refacturacion Orden de Pedido",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_FACTURA getFactura = repoFacturacion.getFacturasTable(textBox1.Text.Trim(), Cia);
                    if (getFactura != null)
                    {
                        int OrdenAnterior = getFactura.Fac_Num_Fac;

                        CXN_CIA getDataCia = repoCia.getPrestadorbyCode(Cia);
                        if (getDataCia != null)
                        {
                            int OrdenPedido = getDataCia.Com_OP;
                            int OrdenPedidoNueva = OrdenPedido + 1;
                            getFactura.Fac_Num_Fac = OrdenPedido;
                            getFactura.Fac_Fecha = DateTime.Now.Date;

                            bool insertaFac = repoFacturacion.InsertCopyTableFacturas(getFactura);
                            if (insertaFac == true)
                            {
                                repoCia.ConsecutivoActualiza(Cia, "OP", OrdenPedidoNueva);

                                List<CXN_CARGOS> getCargos = repoFacturacion.getCargosTable(OrdenAnterior, Cia);
                                if (getCargos != null)
                                {
                                    foreach (var i in getCargos)
                                    {
                                        i.Car_Factura = OrdenPedido.ToString().Trim();
                                        repoFacturacion.InsertCopyTableCargos(i);
                                    }

                                    MG = new MensajesGeneral();
                                    MG.Mensaje = "Factura clonada con exito.  Numero de Orden de Pedido: " + OrdenPedido.ToString();
                                    MG.TipoImagen = 3;
                                    MG.ShowDialog();

                                    this.Dispose();
                                    this.Close();
                                }
                                else
                                {
                                    MG = new MensajesGeneral();
                                    MG.Mensaje = "No se logro clonar los cargos de la factura";
                                    MG.TipoImagen = 1000;
                                    MG.ShowDialog();
                                }
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se logro clonar la factura";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se encontro el catalogo de datos del prestador seleccionado";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "La factura no existe como factura de Aseguradora";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
