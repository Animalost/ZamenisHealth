using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron
{
    public partial class ResultadoRadicacion : Forma2
    {
        private static readonly IFacturacion repoFacturacion = new MFacturacion();
        private static readonly IVentas repositorioVentas = new MVentas();
        private static readonly IRcCaja repositorioRcCaja = new MRcCaja();
        private static readonly IConfSystem repositorioConfSystem = new MConfSystem();
        private List<CXN_FACTURA> facturas = new List<CXN_FACTURA>();
        private MensajesGeneral MG;

        DataTable dt = new DataTable();

        DataColumn OrdenPedido;
        DataColumn FacturaElectronica;
        DataColumn Estado;
        DataColumn Observacion;
        DataColumn Cia;
        DataColumn Tipo;

        public ResultadoRadicacion(List<CXN_FACTURA> f)
        {
            InitializeComponent();
            facturas = f;
        }

        private void Encabezados()
        {
            try
            {
                dt = new DataTable();
                OrdenPedido = dt.Columns.Add("OrdenPedido", typeof(int));
                FacturaElectronica = dt.Columns.Add("FacturaElectronica", typeof(string));
                Estado = dt.Columns.Add("Estado", typeof(string));
                Observacion = dt.Columns.Add("Observacion", typeof(string));
                Cia = dt.Columns.Add("Cia", typeof(int));
                Tipo = dt.Columns.Add("Tipo", typeof(string));
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void ResultadoRadicacion_Load(object sender, EventArgs e)
        {
            try
            {
                this.Titulo.Text = "Resultados de Radicacion DIAN";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                Encabezados();

                foreach (CXN_FACTURA i in facturas)
                {
                    DataRow row = dt.NewRow();

                    row["OrdenPedido"] = i.Fac_Num_Fac;
                    row["FacturaElectronica"] = i.Homologo;
                    row["Estado"] = i.Fac_Estado;
                    row["Observacion"] = i.Fac_Observa.ToString();
                    row["Cia"] = i.Fac_Cia;
                    row["Tipo"] = i.Fac_Tipo_Doc;

                    dt.Rows.Add(row);
                    dt.AcceptChanges();
                }

                Estilos(dataGridView1, dt);               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;

            D.DataSource = t;

            D.Columns["OrdenPedido"].Width = 110;
            D.Columns["FacturaElectronica"].Width = 110;
            D.Columns["Estado"].Width = 110;
            D.Columns["Observacion"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["OrdenPedido"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["FacturaElectronica"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Observacion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["OrdenPedido"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["FacturaElectronica"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Estado"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Observacion"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["Tipo"].Visible = false;
            D.Columns["Cia"].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string Estado = row.Cells["Estado"].Value.ToString();

                if (Estado == "Error")
                {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else if (Estado == "Novedad")
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle.ForeColor = Color.Orange;
                }
                if (Estado == "OK")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Green;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int FacturaZamenisSel = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                string FacturaElectronicaSel = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string TipoSel = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                int CiaSel = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString());

                if (TipoSel == "Aseguradora")
                {
                    List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_Export(FacturaZamenisSel, CiaSel, "OP");
                    if (GenerarDocumentoGrafico == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Recuerde que puede consultar la factura en la carpeta C:\\CXN\\RespuetasDIAN\\RespuestaPDF\\" + FacturaElectronicaSel;
                        MG.ShowDialog();
                    }
                    else
                    {
                        if (repositorioConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                  "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSaludIMPUESTOS.rdlc",
                                                  GenerarDocumentoGrafico);
                        }
                        else
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                                  "ZamenisHealth.Reportes.FacElectron.FacturaElectronicaSalud.rdlc",
                                                  GenerarDocumentoGrafico);
                        }                       
                    }
                }
                else if (TipoSel == "Ventas")
                {
                    List<FacturacionRpt> Exportar = repositorioVentas.Exp_Fac_Ven(Convert.ToInt32(FacturaZamenisSel), Convert.ToInt32(CiaSel), "OP");
                    if (Exportar == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Recuerde que puede consultar la factura en la carpeta C:\\CXN\\RespuetasDIAN\\RespuestaPDF\\" + FacturaElectronicaSel;
                        MG.ShowDialog();
                    }
                    else
                    {
                        if (repositorioConfSystem.getListado()["ReportarImpuestosDIAN"] == "A")
                        {
                            ConfigForm.GenerarReportViewer("Dataset_Facturacion",
                                             "ZamenisHealth.Reportes.RDLC_FacRecepcionIMPUESTOS.rdlc",
                                             Exportar);
                        }
                        else
                        {
                            ConfigForm.GenerarReportViewer("Dataset_Facturacion",
                                             "ZamenisHealth.Reportes.RDLC_FacRecepcion.rdlc",
                                             Exportar);
                        }                        
                    }
                }
                if (TipoSel == "Caja")
                {
                    List<RCCAJA> Exporta = repositorioRcCaja.ReciboRpt(Convert.ToInt32(FacturaZamenisSel));
                    if (Exporta != null)
                    {
                        ConfigForm.GenerarReportViewer("ReciboCajaDataset",
                                      "ZamenisHealth.Reportes.RDLC_RcCajaImpTermica.rdlc",
                                      Exporta);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Recuerde que puede consultar la factura en la carpeta C:\\CXN\\RespuetasDIAN\\RespuestaPDF\\" + FacturaElectronicaSel;
                        MG.ShowDialog();
                    }
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Recuerde que puede consultar la factura en la carpeta C:\\CXN\\RespuetasDIAN\\RespuestaPDF\\" + FacturaElectronicaSel;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
