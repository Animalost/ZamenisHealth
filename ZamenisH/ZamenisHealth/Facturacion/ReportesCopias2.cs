using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class ReportesCopias2 : Forma2
    {
        private static readonly IFacturacion repoFacturacion = new MFacturacion();
        private static readonly IVentas repoVentas = new MVentas();

        private CXN_FACTURA F;
        private MensajesGeneral MG;
        private bool OtrasFacs, VentasRecepcion;

        public ReportesCopias2(CXN_FACTURA f, bool otrasFacs, bool ventasRecepcion)
        {
            InitializeComponent();
            this.F = f;
            OtrasFacs = otrasFacs;
            VentasRecepcion = ventasRecepcion;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //ASEGURADORA Y PARTICULARES
                if (VentasRecepcion == false && OtrasFacs == false)
                {
                    if (this.F.Fac_Tipo_Doc == "DE")
                    {
                        DialogResult result = MessageBox.Show("Presione SI para generar documento Equivalente Individual, presione NO para generar documento Equivalente Grupal",
                                                      "Facturacion - Documentos Equivalentes a la Factura",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            List<FacturasR> GenerarDocumentoGraficoDEIND = repoFacturacion.Exporta_DOCE_IND_Orden(this.F.Fac_Num_Fac, this.F.Fac_Cia, "DE");
                            if (GenerarDocumentoGraficoDEIND == null)
                            {
                                MensajesGeneral M = new MensajesGeneral();
                                M.TipoImagen = 0;
                                M.Mensaje = "No se logro Exportar el documento, ingrese por la opcion de copias para generarlo";
                                M.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_Facturacion", "ZamenisHealth.Reportes.RDLC_DEIND.rdlc", GenerarDocumentoGraficoDEIND);
                        }

                        if (result == DialogResult.No)
                        {
                            List<FacturasR> GenerarDocumentoGraficoDEGRO = repoFacturacion.Exporta_DOCE_Orden(this.F.Fac_Num_Fac, this.F.Fac_Cia, "DE");
                            if (GenerarDocumentoGraficoDEGRO == null)
                            {
                                MensajesGeneral M = new MensajesGeneral();
                                M.TipoImagen = 0;
                                M.Mensaje = "No se logro Exportar el documento, ingrese por la opcion de copias para generarlo";
                                M.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_Facturacion", "ZamenisHealth.Reportes.RDLC_DGROUP.rdlc", GenerarDocumentoGraficoDEGRO);
                        }
                    }

                    List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_Export(F.Fac_Num_Fac, F.Fac_Cia, F.Fac_Tipo_Doc);
                    if (GenerarDocumentoGrafico == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 0;
                        MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                        MG.ShowDialog();
                        return;
                    }

                    Extras.TipoReportFactura R = new Extras.TipoReportFactura(null, GenerarDocumentoGrafico, false);
                    R.ShowDialog();
                }
                //VENTAS
                else if (VentasRecepcion == true && OtrasFacs == false)
                {
                    List<FacturacionRpt> Exportar = repoVentas.Exp_Fac_Ven(F.Fac_Num_Fac, F.Fac_Cia, F.Fac_Tipo_Doc);
                    if (Exportar == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 0;
                        MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                        MG.ShowDialog();
                        return;
                    }

                    Extras.TipoReportFactura R = new Extras.TipoReportFactura(Exportar, null, true);
                    R.ShowDialog();
                }              
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No es posible generar este reporte, debe seleccionar una factura valida";
                    MG.ShowDialog();
                }                                           
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }     
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.F.Fac_Num_Fac == 0) { MessageBox.Show("No es posible anular"); return; }

                if (this.F.Fac_Observa == "CXN_FACTURAS")
                {
                    CXN_FACTURA getFac = repoFacturacion.getFacElectronica(F.Homologo);
                    if (getFac == null)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No se encuentra esta factura electronica";
                        MG.TipoImagen = 0;
                        MG.ShowDialog();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(getFac.Cufe) == true)
                        {
                            DialogResult result2 = MessageBox.Show("¿Desea Anular el Documento?", "Zamenis Health - Facturacion", MessageBoxButtons.YesNo);
                            if (result2 == DialogResult.Yes)
                            {
                                bool anula = repoFacturacion.Anula_Factura(this.F);
                                if (anula != true)
                                {
                                    MG = new MensajesGeneral();
                                    MG.Mensaje = "No se encontro este documento o ya esta anulado";
                                    MG.TipoImagen = 1000;
                                    MG.ShowDialog();
                                    return;
                                }
                                else
                                {
                                    MG = new MensajesGeneral();
                                    MG.Mensaje = "Factura Anulada Correctamente";
                                    MG.TipoImagen = 3;
                                    MG.ShowDialog();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No es posible anular una factura electronica, para este caso debe generar una nota credito";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }
                    }
                }                                   

                if (this.F.Fac_Observa == "CXN_VENTAS")
                {
                    CXN_FACTURA getFacRec = repoVentas.getDAtosFactura(this.F);
                    if (getFacRec == null)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No se encuentra esta factura de recepcion";
                        MG.TipoImagen = 0;
                        MG.ShowDialog();
                        return;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(getFacRec.Cufe) == true)
                        {
                            DialogResult result2 = MessageBox.Show("¿Desea Anular el Documento?", "Zamenis Health - Facturacion", MessageBoxButtons.YesNo);
                            if (result2 == DialogResult.Yes)
                            {
                                Anula_Recepcion();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No es posible anular una factura de recepcion, para este caso debe generar una nota credito";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }
                    }
                }       
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Anula_Recepcion()
        {
            try
            {
                DateTime Hoy = DateTime.Now.Date;

                string CargoDigitado = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite razon de anulacion de factura",
                            "Anular Factura");

                CXN_VENTAS V = new CXN_VENTAS
                {
                    Ven_Usr_Anula = Comunes.Contenedor.UsuarioLogueado,
                    Ven_Mot_Anula = CargoDigitado,
                    Ven_Fecha_Anula = Convert.ToDateTime(Hoy),
                    Ven_Factura = this.F.Fac_Num_Fac.ToString(),
                    Ven_Cod_Cia = this.F.Fac_Cia,
                    Ven_Tipo_Doc = this.F.Fac_Tipo_Doc
                };

                bool anula = repoVentas.Anula_Recepcion(V);
                if (anula != true)
                {
                    MessageBox.Show("No se logro anular");
                }
                else
                {
                    MessageBox.Show("Anulado");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void ReportesCopias2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Reportes";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }
    }
}
