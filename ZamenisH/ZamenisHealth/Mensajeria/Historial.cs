using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Mensajeria
{
    public partial class Historial : ConfigForm.BaseForm
    {
        private static readonly ILogSender repoLog = new MLogSender();

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Destino;
        DataColumn FechaEnviado;
        DataColumn Mensaje;
        DataColumn Estado;

        public Historial()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                var getHis = repoLog.Historial(dateTimePicker1.Value, dateTimePicker2.Value);
                if (getHis != null ) 
                {
                    int Contador = 1;
                    Encabezados();

                    foreach (CXN_LOG_SENDER h in getHis)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Destino"] = h.Log_Destinatario;
                        row["FechaEnviado"] = Convert.ToDateTime(h.Log_Fecha_Envio).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Mensaje"] = h.Log_Mensaje.ToString();
                        row["Estado"] = h.Log_Estado.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
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

        void Estilos()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Destino"].Width = 200;
            dataGridView1.Columns["FechaEnviado"].Width = 200;
            dataGridView1.Columns["Mensaje"].Width = 500;
            dataGridView1.Columns["Estado"].Width = 300;
            dataGridView1.Font = new Font("Arial", 11);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Destino"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["FechaEnviado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Mensaje"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["Destino"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["FechaEnviado"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Mensaje"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Estado"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.Columns["POS"].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                }
            }
        }

        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Destino = dt.Columns.Add("Destino", typeof(string));
            FechaEnviado = dt.Columns.Add("FechaEnviado", typeof(DateTime));
            Mensaje = dt.Columns.Add("Mensaje", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
        }

        private void Historial_Load(object sender, EventArgs e)
        {
            this.Titulo.Visible = false;
            this.ImageClose.Visible = false;
            Encabezados();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var getHis = repoLog.Historial(dateTimePicker1.Value, dateTimePicker2.Value, textBox1.Text);
                if (getHis != null)
                {
                    int Contador = 1;
                    Encabezados();

                    foreach (CXN_LOG_SENDER h in getHis)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Destino"] = h.Log_Destinatario;
                        row["FechaEnviado"] = Convert.ToDateTime(h.Log_Fecha_Envio).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Mensaje"] = h.Log_Mensaje.ToString();
                        row["Estado"] = h.Log_Estado.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Historial2 h = new Historial2(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString());
                h.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
