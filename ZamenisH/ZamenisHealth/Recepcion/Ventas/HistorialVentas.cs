using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace ZamenisHealth.Recepcion.Ventas
{
    public partial class HistorialVentas : Forma2
    {
        private IVender oController;
        private int IdPac;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Fecha;
        DataColumn Nombre;
        DataColumn Estado;
        DataColumn Documento;
        DataColumn Item;
        DataColumn Cantidad;

        public HistorialVentas(int idPac)
        {
            InitializeComponent();
            IdPac = idPac;
            oController = new MVender();
        }

        private void HistorialVentas_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Historial de Venta";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;

            CargarGrilla();
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
            Documento = dt.Columns.Add("Documento", typeof(string));
            Item = dt.Columns.Add("Item", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
        }
        void CargarGrilla()
        {
            try
            {
                List<CXN_VENTAS> _lista = oController.getPrevios(IdPac);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_VENTAS i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Fecha] = i.Ven_Fecha.ToString("yyyy-MM-dd");
                        row[Nombre] = i.Grafica.ToString();
                        row[Estado] = i.Ven_Estado;
                        row[Documento] = i.Ven_Factura;
                        row[Item] = i.Ven_Cod;
                        row[Cantidad] = Convert.ToInt32(i.Ven_Cantidad);

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(gridZH1.dataGridView1, dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
        }
    }
}
