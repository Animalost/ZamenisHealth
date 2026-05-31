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

namespace ZamenisHealth
{
    public partial class Gerencia : Forma2
    {
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IFacturacion repositorioFactura = new MFacturacion();
        private static readonly IRcCaja repositorioRcCaja = new MRcCaja();
        private static readonly IPacientes repositorioPacientes = new MPacientes();

        private int Cia;

        public Gerencia()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox4.Text)
                {
                    case "Facturas":
                        Filter("FA");
                        break;

                    case "Ordenes de Pedido":
                        Filter("OP");
                        break;

                    case "Documento Equivalente":
                        Filter("DE");
                        break;

                    case "Recibos de Caja":
                        RCCAJAS();
                        break;

                    default:
                        MessageBox.Show("Seleccion invalida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }  
        private void CargarDocumentos()
        {
            List<string> ListaDocs =  repositorioPacientes.ListaDocs();       

            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }
        private void Gerencial_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Facturas Hechas por Paciente";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            CargarDocumentos();
            

            List<CXN_CIA> cias =  repositorioCompañias.getAllCompañias(); 
            if (cias != null)
            {
                foreach (var i in cias)
                {
                    comboBox3.Items.Add(i.Com_Nombre);
                }

                comboBox3.SelectedIndex = 0;
            }
        }
        private void Encabezados()
        {
            try
            {
                listView1.Clear();
                listView1.View = View.Details;
                listView1.GridLines = true;
                listView1.FullRowSelect = true;
                listView1.Columns.Add("Factura", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("Homologo", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("Emision", 150, HorizontalAlignment.Left);
                listView1.Columns.Add("Desde", 150, HorizontalAlignment.Left);
                listView1.Columns.Add("Hasta", 150, HorizontalAlignment.Left);
                listView1.Columns.Add("Estado", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("Usuario", 100, HorizontalAlignment.Left);
                listView1.Columns.Add("Tipo", 0, HorizontalAlignment.Left);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Encabezados2()
        {
            try
            {
                listView1.Clear();
                listView1.View = View.Details;
                listView1.GridLines = true;
                listView1.FullRowSelect = true;
                listView1.Columns.Add("Recibo", 200, HorizontalAlignment.Left);
                listView1.Columns.Add("Fecha", 200, HorizontalAlignment.Left);
                listView1.Columns.Add("Usuario", 200, HorizontalAlignment.Left);
                listView1.Columns.Add("A1", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("A2", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("A3", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("A4", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("A5", 0, HorizontalAlignment.Left);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Filter(string Tipo)
        {
            try
            {
                List<CXN_FACTURA> _previosPac =  repositorioFactura.Filter(Tipo,
                                                            comboBox1.Text,
                                                            textBox1.Text,
                                                            Cia);
                if (_previosPac != null)
                {
                    Encabezados();

                    foreach (var i in _previosPac)
                    {
                        label6.Text = i.Fac_Observa;

                        string Estados;
                        switch (i.Fac_Estado)
                        {
                            case "A":
                                Estados = "Anulado";
                                break;
                            case "F":
                                Estados = "Vigente";
                                break;
                            default:
                                Estados = "Error";
                                break;
                        }

                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                                i.Fac_Num_Fac.ToString(),
                                i.Homologo,
                                Convert.ToDateTime(i.Fac_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                Convert.ToDateTime(i.Fac_Fecha_Des).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                Convert.ToDateTime(i.Fac_Fecha_Has).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                Estados,
                                i.Fac_Usr_Graba,
                                i.Fac_Tipo_Doc
                        }));
                    }
                }
                else
                {
                    Encabezados();
                    label6.Text = "NO HAY RESULTADOS";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Expo_Factura(string Facs, int Cias, string Tipos)
        {
            if (Tipos == "OP" || Tipos == "FA")
            {
                List<FacturasR> GenerarDocumentoGrafico =  repositorioFactura.Fac_Export(Convert.ToInt32(Facs), Cias, Tipos);          
                if (GenerarDocumentoGrafico == null)
                {
                    MessageBox.Show("No se logro Exportar el documento, ingrese por la opcion de copias para generarlo",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                            "ZamenisHealth.Reportes.RDLC_Factura.rdlc",
                                            GenerarDocumentoGrafico);
            }

            if (Tipos == "DE")
            {
                List<FacturasR> GenerarDocumentoGraficoDEIND =  repositorioFactura.Exporta_DOCE_IND_Orden(Convert.ToInt32(Facs), Cias, Tipos);

                if (GenerarDocumentoGraficoDEIND == null)
                {
                    MessageBox.Show("No se logro Exportar el documento, ingrese por la opcion de copias para generarlo",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet_Facturacion",
                                            "ZamenisHealth.Reportes.RDLC_DEIND.rdlc",
                                            GenerarDocumentoGraficoDEIND);

            }

        }
        private void Expo_Recibo(string Datos)
        {
            List<RCCAJA> Exporta = repositorioRcCaja.ReciboRpt(Convert.ToInt32(Datos));

            if (Exporta != null)
            {
                ConfigForm.GenerarReportViewer("ReciboCajaDataset",
                                            "ZamenisHealth.Reportes.RDLC_RcCaja.rdlc",
                                            Exporta);

            }
        }
        private void RCCAJAS()
        {
            try
            {
                List<CXN_RC_CAJA> lista = repositorioRcCaja.RCCAJAS(comboBox1.Text, textBox1.Text);         

                if (lista != null)
                {
                    Encabezados2();

                    foreach (var i in lista)
                    {
                        label6.Text = i.Rc_Caja_Observacion;

                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                             i.Rc_Caja_Adm.ToString(),
                             Convert.ToDateTime(i.Rc_Caja_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                             i.Rc_Caja_UsrGraba,
                             "",
                             "",
                             "",
                             "",
                             ""
                        }));
                    }
                }
                else
                {
                    label6.Text = "NO HAY RESULTADOS";
                    Encabezados2();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Comunes.BuscarPacientes Busca_Pac = new Comunes.BuscarPacientes("Fac_Per");
                Busca_Pac.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }      
        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                string Dato = listView1.SelectedItems[0].SubItems[7].Text;

                switch (Dato)
                {
                    case "OP":
                        Expo_Factura(listView1.SelectedItems[0].SubItems[0].Text, Cia, "OP");
                        break;

                    case "FA":
                        Expo_Factura(listView1.SelectedItems[0].SubItems[0].Text, Cia, "FA");
                        break;

                    case "DE":
                        Expo_Factura(listView1.SelectedItems[0].SubItems[0].Text, Cia, "DE");
                        break;

                    default:
                        Dato = listView1.SelectedItems[0].SubItems[0].Text;
                        Expo_Recibo(Dato);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CXN_CIA cias =  repositorioCompañias.getPrestadorbyName(comboBox3.Text);
           
            Cia = cias.Com_Identificador;
        }
    }
}
