using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.AgendaDiaria
{
    public partial class RecibosAsociados : Forma2
    {
        private IRcCaja oRcCaja;

        private int Admision;
        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn Adm;
        DataColumn Recibo;
        DataColumn Fecha;
        DataColumn Usuario;
        DataColumn Valor;
        DataColumn Observacion;

        public RecibosAsociados(int admision)
        {
            InitializeComponent();
            Admision = admision;
            oRcCaja = new MRcCaja();
        }

        private void RecibosAsociados_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Recibos de Caja: " + Admision.ToString();
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            CargarRecibos();

            gridZH1.dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            gridZH1.CeldaHeight = true;
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string NRc = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                List<RCCAJA> Exporta = oRcCaja.ReciboRpt(Convert.ToInt32(NRc));
                if (Exporta != null)
                {
                    ConfigForm.GenerarReportViewer("ReciboCajaDataset", "ZamenisHealth.Reportes.RDLC_RcCajaImpTermica.rdlc", Exporta);
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Hubo un inconveniente con este recibo, ingrese por copias recepcion o consulte el administrador del sistema",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Adm = dt.Columns.Add("Adm", typeof(string));
            Recibo = dt.Columns.Add("Recibo", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Usuario = dt.Columns.Add("Usuario", typeof(string));
            Valor = dt.Columns.Add("Valor", typeof(string));
            Observacion = dt.Columns.Add("Observacion", typeof(string));
            gridZH1.CeldaHeight = true;
        }

        void CargarRecibos()
        {
            try
            {
                List<CXN_RC_CAJA> _lista = oRcCaja.ListaRecibosPorAdmision(Admision);
                if (_lista != null)
                {
                    Encabezados();

                    foreach (CXN_RC_CAJA i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["Adm"] = i.Rc_Caja_Adm.ToString();
                        row["Recibo"] = i.Rc_Id.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Rc_Caja_Fecha).ToString("yyyy-MM-dd");
                        row["Usuario"] = i.Rc_Caja_UsrGraba;
                        row["Valor"] = "$ " + Convert.ToInt32(i.Rc_Caja_Valor).ToString("N0");
                        row["Observacion"] = i.Rc_Caja_Observacion;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                    }

                    gridZH1.dataGridView1.DataSource = dt;
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
