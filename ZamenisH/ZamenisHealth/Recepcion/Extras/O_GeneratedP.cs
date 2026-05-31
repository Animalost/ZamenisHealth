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
using ZamenisHealth.Medicina;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class O_GeneratedP : Forma
    {

        private static readonly IAgendaC repositorioFechasAgendaa = new MAgendaC();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly ICompañia repositorioCompañia = new MCompañia();
        private static readonly IOrdenes repositorioOrdenes = new MOrdenes();

        private int Admision;

        public O_GeneratedP(int admision)
        {
            InitializeComponent();
            this.Admision = admision;
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
            listView1.Columns.Add("Planillada", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("CIA", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Radicada", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Autorizacion", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Consumida", 100, HorizontalAlignment.Left);
            listView1.Visible = true;
        }
        void Buscar()
        {
            try
            {
                CXN_PACIENTES getPac = new CXN_PACIENTES();

                
                    getPac = repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text);
                

                if (getPac != null)
                {
                    CXN_CIA getCodePrest = new CXN_CIA();

                    
                        getCodePrest = repositorioCompañia.getPrestadorbyName(comboBox1.Text);
                    

                    if (getCodePrest != null)
                    {
                        List<CXN_OM> getList = new List<CXN_OM>();

                        
                            getList = repositorioOrdenes.getOrdenes(getPac.Pac_TipoId, getPac.Pac_IdNum, getCodePrest.Com_Identificador);
                        

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
                                    i.OM_Planillar.ToString(),
                                    i.OM_Cia.ToString(),
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void cargarPrest()
        {
            List<CXN_CIA> getPrest = new List<CXN_CIA>();

            
                getPrest = repositorioCompañia.getAllCompañias();
            

            if (getPrest != null)
            {
                foreach (var i in getPrest)
                {
                    comboBox1.Items.Add(i.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }
        private void O_GeneratedP_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Ordenes Medicas";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnBuscar = new ToolStripButton();
                btnBuscar = createToolButton("Buscar");
                MenuLateral.Items.Add(btnBuscar);
                btnBuscar.Click += button1_Click;

                cargarPrest();

                otrosDatosPacienteHorario getIdPac = repositorioFechasAgendaa.cargarAdmision(Admision, "'A','P','H'");                

                if (getIdPac != null)
                {
                    List<CXN_OM> getList = new List<CXN_OM>();

                    
                        getList = repositorioOrdenes.getOrdenes(getIdPac.Pac_TipoId, getIdPac.Pac_IdNum, getIdPac.Hor_Pac_Cia);
                    

                    if (getList  != null)
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
                                i.OM_Planillar.ToString(),
                                i.OM_Cia.ToString(),
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                OrdenesMedicasT T = new OrdenesMedicasT(Convert.ToInt32(listView1.SelectedItems[0].SubItems[6].Text),
                                                        Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text),
                                                        Contenedor.UsuarioLogueado.ToString());
                T.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Buscar();
        }
    }
}
