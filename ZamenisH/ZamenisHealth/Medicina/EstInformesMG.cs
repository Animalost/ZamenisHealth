using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Informes.Interfaces;
using Persistence.Informes.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class EstInformesMG : Forma
    {
        private static readonly IInformeEstadistico repoEst = new MInformeEstadistico();
        private static readonly IPacientes repoPac = new MPacientes();

        MensajesGeneral MG;

        public EstInformesMG()
        {
            InitializeComponent();           
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes P = new BuscarPacientes("INGSAL");
            P.ShowDialog();
        }

        public void setSelection(string Doc)
        {
            textBox1.Text = Doc;
        }

        void EncabezadosLV1()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Pos", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Adm", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha de Cita", 140, HorizontalAlignment.Left);
            listView1.Columns.Add("Clase", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Servicio", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("TServ", 0, HorizontalAlignment.Left);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CargarGrillas();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public void setCargarGrillas()
        {
            CargarGrillas();
        }

        void CargarGrillas()
        {
            try
            {
                CXN_PACIENTES getPac = repoPac.LlamarPacienteNumDoc(textBox1.Text);
                if (getPac != null)
                {
                    int Contador = 0;

                    List<CXN_HORARIO> getCitas = repoEst.getCitasAsistidas(getPac.Pac_Id, "'MG','CU'");
                    if (getCitas != null)
                    {
                        EncabezadosLV1();

                        foreach (CXN_HORARIO i in getCitas)
                        {
                            listView1.Items.Add(new ListViewItem(new string[]
                            {
                                Contador.ToString(),
                                i.Hor_Id.ToString(),
                                Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                repoEst.getClase(i.Hor_Id),
                                i.Hor_Observacion.ToString(),
                                i.Hor_Pac_Tipo_Serv.ToString()
                            }));

                            Contador++;
                        }

                        Sortear(listView1);
                    }                  
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "El documento digitado no existe en sistema";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Sortear(ListView LV)
        {
            try
            {
                for (int i = 0; i < LV.Items.Count; i++)
                {
                    if (Convert.ToInt32(LV.Items[i].SubItems[0].Text) % 2 == 0)
                    {
                        LV.Items[i].BackColor = Color.MediumAquamarine;
                    }
                    else
                    {
                        LV.Items[i].BackColor = Color.Aquamarine;
                    }
                }
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
                EstInformesMG2 M = new EstInformesMG2(Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text), listView1.SelectedItems[0].SubItems[5].Text);
                M.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void EstInformesMG_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Estadisticas";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnBuscar;

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;
        }
    }
}
