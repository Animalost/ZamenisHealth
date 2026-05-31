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

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class ParticularesPrevios : Forma
    {

        private static readonly ICargos repositorioCargos = new MCargos();
        private static readonly IPacientes repositorioPacientes = new MPacientes();

        public ParticularesPrevios()
        {
            InitializeComponent();
        }

        private void ParticularesPrevios_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Historico Particulares";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnBuscar = new ToolStripButton();
                btnBuscar = createToolButton("Buscar");
                MenuLateral.Items.Add(btnBuscar);
                btnBuscar.Click += button1_Click;

                CargarDocumentos();
                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void CargarDocumentos()
        {
            List<string> ListaDocs = new List<string>();

            
                ListaDocs = repositorioPacientes.ListaDocs();
            

            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                List<CXN_CARGOS> _listaPrevia = new List<CXN_CARGOS>();

                
                    _listaPrevia = repositorioCargos.cotizacionesPrevias(comboBox1.Text, textBox1.Text);
                

                if (_listaPrevia != null)
                {
                    Encabezados();

                    foreach (var i in _listaPrevia)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                           {
                                i.Car_Factura,
                                Convert.ToDateTime(i.Car_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                i.Car_Detalle,
                                i.Car_Usr_Graba,
                                i.Car_Cia.ToString()
                           }));
                    }
                }
                else
                {
                    Encabezados();
                    MessageBox.Show("No hay resultados", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Documento", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Paciente", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Creador", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Cia", 0, HorizontalAlignment.Left);
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes Busca_Pac = new Comunes.BuscarPacientes("Cotiza_Hist");
            Busca_Pac.ShowDialog();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                List<CotizacionR> Exporta = new List<CotizacionR>();

                
                    Exporta = repositorioCargos.GenerarDocumento(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text),
                    Convert.ToInt32(listView1.SelectedItems[0].SubItems[4].Text));
                

                if (Exporta == null)
                {
                    MessageBox.Show("No se logro exportar el documento, por favor ingrese por la opcion de copias",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                }

                ConfigForm.GenerarReportViewer("DataSet_Cotizacion",
             "ZamenisHealth.Reportes.RDLC_Cotizacion.rdlc",
             Exporta);

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
