using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Homologos : ConfigForm.BaseForm
    {
        private static readonly IHomologos repoHomologos = new MHomologos();
        private static readonly ICompañia repoCia = new MCompañia();

        int Cia;
        public Homologos()
        {
            InitializeComponent();

            btnZamenis1.ButtonClick += btnZamenis1_ButtonClick;
            btnZamenis3.ButtonClick += btnZamenis3_ButtonClick;

            btnZamenis1.captionBtn = "Masivo";
            btnZamenis1.tooltipBtn = "Subir archivos XLS de manera masiva al sistema";

            btnZamenis3.captionBtn = "Buscar";
            btnZamenis3.tooltipBtn = "Buscar numero de documento Zamenis para asignar Homologo"; 
        }

        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                this.Dispose();
                this.Close();

                Homologos3 H = new Homologos3();
                H.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "Facturacion General de Aseguradoras") { Filtra_Generales("UNOGeneral"); return; }
                if (comboBox1.Text == "Facturacion de Ventas") { Filtra_Generales("UNOVentas"); return; }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Homologos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Homologos";
            

            var cias = repoCia.getAllCompañias();
            if (cias != null)
            {
                foreach (var i in cias)
                {
                    comboBox2.Items.Add(i.Com_Nombre);
                }

                comboBox2.SelectedIndex = 0;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ciaid = repoCia.getPrestadorbyName(comboBox2.Text);
            Cia = ciaid.Com_Identificador;
        }
        private void Encabezados()
        {
            try
            {
                listView1.Clear();
                listView1.View = View.Details;
                listView1.GridLines = true;
                listView1.FullRowSelect = true;
                listView1.Columns.Add("Ide", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("Factura", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("Homologo", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("Paciente", 250, HorizontalAlignment.Left);
                listView1.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("Aseguradora", 230, HorizontalAlignment.Left);
                listView1.Columns.Add("Usuario", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("Tipo Documento", 0, HorizontalAlignment.Left);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Filtra_Generales(string Tipo)
        {
            try
            {
                var getDocuments = repoHomologos.Filtra_Generales(Tipo, Cia, Convert.ToInt32(textBox1.Text));
                if (getDocuments != null)
                {
                    Encabezados();
                    string Tips = "";

                    foreach (var i in getDocuments)
                    {
                        switch (i.Fac_Tipo_Doc)
                        {
                            case "OP":
                                Tips = "Orden de Pedido";
                                break;

                            case "FA":
                                Tips = "Factura de Venta";
                                break;

                            case "DE":
                                Tips = "Documento Equivalente";
                                break;

                            default:
                                Tips = "Error";
                                break;
                        }

                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                                i.Fac_Id.ToString(),
                                i.Fac_Num_Fac.ToString(),
                                i.Homologo.ToString(),
                                i.Fac_Observa.ToString(),
                                Convert.ToDateTime(i.Fac_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                i.Fac_Res.ToString(),
                                i.Fac_Usr_Graba.ToString(),
                                Tips.ToString()
                        }));
                    }
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "Facturacion General de Aseguradoras")
                {
                    ConfigForm.TextoMDI = "General";
                    ConfigForm.PosGrillaMDI = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text);
                    ConfigForm.FacZamenisMDI = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text);
                    ConfigForm.CompañiaMDI = Cia;

                    this.Dispose();
                    this.Close();

                    Homologos2 H = new Homologos2(ConfigForm.TextoMDI, ConfigForm.FacZamenisMDI, ConfigForm.PosGrillaMDI, ConfigForm.CompañiaMDI);
                    H.ShowDialog();
                    return;
                }
                if (comboBox1.Text == "Facturacion de Ventas")
                {
                    ConfigForm.TextoMDI = "Recepcion";
                    ConfigForm.PosGrillaMDI = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text);
                    ConfigForm.FacZamenisMDI = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text);
                    ConfigForm.CompañiaMDI = Cia;

                    this.Dispose();
                    this.Close();

                    Homologos2 H = new Homologos2(ConfigForm.TextoMDI, ConfigForm.FacZamenisMDI, ConfigForm.PosGrillaMDI, ConfigForm.CompañiaMDI);
                    H.ShowDialog();

                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
