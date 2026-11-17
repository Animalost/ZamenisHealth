using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.Extras
{
    public partial class CargosAnterior : Forma2
    {
        private static readonly ICargos repoCargos = new MCargos();
        private int Admi, IdPaciente;

        DataTable dt;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Servicio;
        DataColumn Profesional;

        DataTable dt2;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Cantidad;

        public CargosAnterior(int _idPaciente)
        {
            InitializeComponent();
            this.IdPaciente = _idPaciente;
        }
        void EncabezadosLv2()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Servicio = dt.Columns.Add("Servicio", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
        }
        void Encabezados()
        {
            gridZH2.dataGridView1.DataSource = null;
            dt2 = new DataTable();
            Codigo = dt2.Columns.Add("Codigo", typeof(string));
            Item = dt2.Columns.Add("Item", typeof(string));
            Cantidad = dt2.Columns.Add("Cantidad", typeof(int));
        }
        private void CargosAnterior_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Historial Cargos Anteriores";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                gridZH1.CeldaHeight = true;
                gridZH2.CeldaHeight = true;
                gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;

                var getLista = repoCargos.getCargosPrevios(this.IdPaciente);
                if (getLista != null)
                {
                    EncabezadosLv2();

                    foreach (var i in getLista)
                    {
                        label14.Text = i.Car_Usr_Graba.ToString();

                        DataRow row = dt.NewRow();

                        row[Admision] = i.Car_Adm_Id;
                        row[Fecha] = Convert.ToDateTime(i.Car_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[Servicio] = i.Car_Item.ToString();
                        row[Profesional] = i.Car_Detalle.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                }
                else
                {
                    EncabezadosLv2();

                    Comunes.MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Este paciente no tiene historial de curaciones";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                Admi = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                var getLista = repoCargos.CargosAnt(Admi);
                if (getLista != null)
                {
                    Encabezados();

                    foreach (var i in getLista)
                    {
                        DataRow row = dt2.NewRow();

                        row[Codigo] = i.Car_Cod.ToString();
                        row[Item] = i.Car_Item.ToString();
                        row[Cantidad] = i.Car_Cant;

                        dt2.Rows.Add(row);
                        dt2.AcceptChanges();
                    }

                    gridZH2.dataGridView1.DataSource = dt2;
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
