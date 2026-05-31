using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Autorizaciones : Forma
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly ICompañia repositorioCompañia = new MCompañia();
        private static readonly IOrdenes repositorioOrdenes = new MOrdenes();

        public Autorizaciones()
        {
            InitializeComponent();          
        }
        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Orden", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Medico", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Tipo", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Especialidad", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Radicada", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Autorizacion", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Consumida", 100, HorizontalAlignment.Left);
            listView1.Visible = true;
        }
        void Buscar()
        {
            try
            {
                CXN_PACIENTES getPac = repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text);
                if (getPac != null)
                {
                    var getCodePrest = repositorioCompañia.getPrestadorbyName(comboBox1.Text);
                    if (getCodePrest != null)
                    {
                        var getList = repositorioOrdenes.getOrdenes(getPac.Pac_TipoId, getPac.Pac_IdNum, getCodePrest.Com_Identificador);
                        if (getList != null)
                        {
                            Encabezados();

                            foreach (var i in getList)
                            {
                                listView1.Items.Add(new ListViewItem(new string[]
                                {
                                    i.OM_Num.ToString(),
                                    i.OM_Prof.ToString(),
                                    i.OM_Tipo.ToString(),
                                    Convert.ToDateTime(i.OM_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                    i.OM_TEspecialidad.ToString(),
                                    i.OM_Radicada.ToString(),
                                    i.OM_Autorizacion.ToString(),
                                    i.OM_Consumida.ToString()
                                }));
                            }
                        }
                        else
                        {
                            Encabezados();
                        }
                    }
                    else
                    {
                        Encabezados();
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
        void cargarPrest()
        {
            var getPrest = repositorioCompañia.getAllCompañias();
            if (getPrest != null)
            {
                foreach (var i in getPrest)
                {
                    comboBox1.Items.Add(i.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }
        private void Autorizaciones_Load(object sender, EventArgs e)
        {
            cargarPrest();
            Titulo.Text = "Adherencia";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += toolStripButton2_Click;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Buscar();
        }
        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                var getCodeCia = repositorioCompañia.getPrestadorbyName(comboBox1.Text);
                if (getCodeCia != null)
                {
                    Extras.Autorizaciones2 A = new Extras.Autorizaciones2(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text), getCodeCia.Com_Identificador);
                    A.ShowDialog();
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "El prestador seleccionado presenta inconvenientes";
                    MG.ShowDialog();
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
