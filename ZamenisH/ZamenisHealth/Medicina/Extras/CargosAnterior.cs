using System;
using System.Linq;
using System.Windows.Forms;
using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.Extras
{
    public partial class CargosAnterior : Forma2
    {
        private static readonly ICargos repoCargos = new MCargos();

        private int Admi, IdPaciente;

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                label14.Text = "";
                listView2.Clear();

                Cargos f29 = Application.OpenForms.OfType<Cargos>().SingleOrDefault();
                f29.HidePanel3();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        public CargosAnterior(int _idPaciente)
        {
            InitializeComponent();
            this.IdPaciente = _idPaciente;
        }
        private void listView2_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                Admi = Convert.ToInt32(listView2.SelectedItems[0].SubItems[0].Text);

                var getLista = repoCargos.CargosAnt(Admi);
                if (getLista != null)
                {
                    EncabezadosLv1();

                    foreach (var i in getLista)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Car_Cod.ToString(),
                            i.Car_Item.ToString(),
                            i.Car_Cant.ToString()
                        }));
                    }
                }
                else
                {
                    EncabezadosLv1();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void EncabezadosLv2()
        {
            listView2.Clear();
            listView2.View = View.Details;
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.Columns.Add("Admision", 100, HorizontalAlignment.Left);
            listView2.Columns.Add("Fecha", 100, HorizontalAlignment.Left);
            listView2.Columns.Add("Servicio", 250, HorizontalAlignment.Left);
            listView2.Columns.Add("Profesional", 250, HorizontalAlignment.Left);
        }
        private void CargosAnterior_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Historial Cargos Anteriores";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                PanelTitulo.Dock = DockStyle.Top;

                ImageClose.Visible = false;
                ImageMinimize.Visible = false;

                var getLista = repoCargos.getCargosPrevios(this.IdPaciente);
                if (getLista != null)
                {
                    EncabezadosLv2();

                    foreach (var i in getLista)
                    {
                        label14.Text = i.Car_Usr_Graba.ToString();

                        listView2.Items.Add(new ListViewItem(new string[]
                        {
                            i.Car_Adm_Id.ToString(),
                            Convert.ToDateTime(i.Car_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.Car_Item.ToString(),
                            i.Car_Detalle.ToString()
                        }));
                    }
                }
                else
                {
                    EncabezadosLv2();

                    Comunes.MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Este paciente no tiene historial de curaciones";
                    MG.ShowDialog();

                    Cargos f29 = Application.OpenForms.OfType<Cargos>().SingleOrDefault();
                    f29.HidePanel3();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void EncabezadosLv1()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Codigo", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Item", 500, HorizontalAlignment.Left);
            listView1.Columns.Add("Cantidad", 80, HorizontalAlignment.Left);
        }
    }
}
