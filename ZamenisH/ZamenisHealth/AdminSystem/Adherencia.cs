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
    public partial class Adherencia : Forma
    {      
        private static readonly IAdherencia repoAdh = new MAdherencia();

        public Adherencia()
        {
            InitializeComponent();
        }
        
        private void Adherencia_Load(object sender, EventArgs e)
        {            
            Titulo.Text = "Adherencia";
            SubTitulo.Text = $"Zamenis Health { Conexion.VersionApp }";
            LogoMain.Image = Properties.Resources.Logo_Zamenis_APPS;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += toolStripButton3_Click;

            Cargar();
        }

        void EncabezadoLv1()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Aposito", 200, HorizontalAlignment.Left);
        }

        private void Cargar()
        {
            try
            {
                var listaAp = repoAdh.listaApositos();
                if (listaAp != null)
                {
                    EncabezadoLv1();

                    foreach (var i in listaAp)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                             i.Adh_Aposito.ToString()
                        }));
                    }
                }
                else
                {
                    EncabezadoLv1();
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
                textBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;

                string _desc = repoAdh.DescripcionAposito(listView1.SelectedItems[0].SubItems[0].Text);
                richTextBox1.Text = _desc;
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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Cargar();
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

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Diligencie nombre y descripcion", "No es posible continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (richTextBox1.Text == "") { MessageBox.Show("Diligencie nombre y descripcion", "No es posible continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                CXN_ADHERENCIA ADH = new CXN_ADHERENCIA
                {
                    Adh_Aposito = textBox1.Text,
                    Adh_Descripcion = richTextBox1.Text
                };

                string _existe = repoAdh.DescripcionAposito(ADH.Adh_Aposito);
                if (_existe == "")
                {
                    //crear
                    bool _crear = repoAdh.Crea(ADH);
                    if (_crear != true)
                    {
                        MessageBox.Show("La adherencia no logro ser guardada, vuelva a intentar",
                               "Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    //editar
                    bool _editar = repoAdh.Actualiza(ADH);
                    if (_editar != true)
                    {
                        MessageBox.Show("La adherencia no logro ser guardada, vuelva a intentar",
                               "Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                        return;
                    }
                }

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Hecho";
                MG.ShowDialog();

                Cargar();
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
