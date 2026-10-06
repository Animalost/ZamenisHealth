using Domain.CONSUMOS;
using FormAndControls;
using Persistence;
using Persistence.CONSUMOS.Interfaces;
using Persistence.CONSUMOS.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Consumos
{
    public partial class VerMovimientosBasico : Forma2
    {
        private IMovimientosConsumo oMovimientosConsumo;
        private IAsignacion oAsignacion;
        private bool Administrator;

        DataTable dt;
        DataColumn CodigoInterno;
        DataColumn NombreProducto;
        DataColumn Cantidad;
        DataColumn Usuario;
        DataColumn IdPosConsumo;

        private MensajesGeneral MG;

        public VerMovimientosBasico(bool administrator)
        {
            InitializeComponent();
            Administrator = administrator;
            oMovimientosConsumo = new MMovimientosConsumo();
            oAsignacion = new MAsignacion();
        }

        private void VerMovimientosBasico_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Movimientos Historico";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;
            CargarBodega();
            ConsultarConsumos();

            gridZH1.dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
        }
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Administrator == true)
                {
                    int Posision = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString());

                    //Anular Moivimiento solo admninistador

                    CON_CONSUMOS C = new CON_CONSUMOS
                    {
                        Con_Cons_Usr_Anula = Contenedor.UsuarioLogueado,
                        Con_Cons_Fecha_Anula = DateTime.Now.Date,
                        Con_Cons_Id = Posision
                    };

                    bool anula = oMovimientosConsumo.AnularMovimiento(C);
                    if (anula == true)
                    {
                        ConsultarConsumos();
                    }
                    else
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "No se logro anularel movimiento",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CargarBodega()
        {
            List<CON_ASIGNACION> g = oAsignacion.GetBodegas();
            if (g != null)
            {
                foreach (CON_ASIGNACION con in g)
                {
                    comboBox1.Items.Add(con.Asi_Name);
                }

                comboBox1.SelectedItem = 0;
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            CodigoInterno = dt.Columns.Add("Codigo Interno", typeof(int));
            NombreProducto = dt.Columns.Add("Nombre del Producto", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad Consumida", typeof(string));
            Usuario = dt.Columns.Add("Usuario que Registra", typeof(string));
            IdPosConsumo = dt.Columns.Add("Posision", typeof(int));
        }
        void ConsultarConsumos()
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    Encabezados();
                    return;
                }
                else
                {
                    CON_ASIGNACION g = oAsignacion.GetBodega(comboBox1.Text);

                    List<CON_CONSUMOS> listado = oMovimientosConsumo.GetConsumos(g.Asi_Number, dateTimePicker1.Value.Date);
                    if (listado != null)
                    {
                        listado = listado.Where(x => x.Con_Cons_Status == true).ToList();

                        Encabezados();

                        foreach (var i in listado)
                        {
                            DataRow row = dt.NewRow();

                            row[CodigoInterno] = i.Con_Prod_Cod_Interno;
                            row[NombreProducto] = i.Con_Prod_Name.ToString();
                            row[Cantidad] = i.Con_Cons_Cantidad;
                            row[Usuario] = i.Con_Cons_User;
                            row[IdPosConsumo] = i.Con_Cons_Id;

                            dt.Rows.Add(row);
                            dt.AcceptChanges();
                        }

                        gridZH1.dataGridView1.DataSource = dt;
                        gridZH1.dataGridView1.Columns["Posision"].Visible = false;
                    }
                    else
                    {
                        Encabezados();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = "Codigo Interno|Nombre del Producto|Cantidad Consumida|Usuario que Registra \r";

                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    Texto = Texto + row.Cells["Codigo Interno"].Value.ToString() + "|" +
                                    row.Cells["Nombre del Producto"].Value.ToString() + "|" +
                                    row.Cells["Cantidad Consumida"].Value.ToString() + "|" +
                                    row.Cells["Usuario que Registra"].Value.ToString() + "\r";
                }

                string NameFile = "C:/Cxn/Reportes/Consumos_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".xls";
                FileStream QueryTxt = new FileStream(NameFile, FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(QueryTxt);
                Escriba.Write(Texto);
                Escriba.Close();

                if (File.Exists(NameFile))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = NameFile,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("El archivo no existe.");
                }
            }
            catch (Exception ex)            
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConsultarConsumos();
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ConsultarConsumos();
        }
    }
}
