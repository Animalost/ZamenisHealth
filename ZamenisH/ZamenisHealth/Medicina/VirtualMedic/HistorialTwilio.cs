using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.VirtualMedic
{
    public partial class HistorialTwilio : Forma
    {
        private ITwilio oController;

        private MensajesGeneral MG;
        ToolStripButton btnBuscar;
        
        DataTable dt;
        DataColumn POS;
        DataColumn Admision;
        DataColumn Paciente;
        DataColumn UrlCliente;
        DataColumn Fecha;
        DataColumn UsuarioGenera;
        DataColumn Estado;
        DataColumn Conectar;
        DataColumn URLADMIN;

        public HistorialTwilio()
        {
            InitializeComponent();
            oController = new MTwilio();
        }

        private void HistorialTwilio_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Historial de Links";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += btnBuscar_Click;
        }

        void btnBuscar_Click(object sender, EventArgs e) 
        {
            try
            {
                List<CXN_LINKSCORTOS> getList = oController.getUrlsGenerated(textBox1.Text.Trim());
                if (getList != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_LINKSCORTOS i in getList)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = i.Admision.ToString();
                        row["Paciente"] = i.complemento.Paciente.ToString().Trim();
                        row["UrlCliente"] = i.Cliente.Trim();
                        row["Fecha"] = Convert.ToDateTime(i.Fecha);
                        row["UsuarioGenera"] = i.Usuario;
                        row["Estado"] = i.complemento.Estado;
                        row["Conectar"] = i.complemento.Estado.Trim() != "VENCIDO" ? "INGRESAR A SALA" : "";
                        row["URLADMIN"] = i.Servidor.Trim();
                        
                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    Encabezados();

                    MG = new MensajesGeneral
                    {
                        Mensaje = "No hay links generados para este paciente",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Admision"].Width = 80;
            D.Columns["Paciente"].Width = 300;
            D.Columns["UrlCliente"].Width = 250;
            D.Columns["Fecha"].Width = 150;
            D.Columns["UsuarioGenera"].Width = 120;
            D.Columns["Estado"].Width = 80;
            D.Columns["Conectar"].Width = 120;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["UrlCliente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["UsuarioGenera"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false; 
            D.Columns["URLADMIN"].Visible = false;

            foreach (DataGridViewRow row in D.Rows)
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
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            UrlCliente = dt.Columns.Add("UrlCliente", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            UsuarioGenera = dt.Columns.Add("UsuarioGenera", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
            Conectar = dt.Columns.Add("Conectar", typeof(string));
            URLADMIN = dt.Columns.Add("URLADMIN", typeof(string));
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3) //copiar
                {
                    Clipboard.SetText(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString().Trim());

                    MG = new MensajesGeneral
                    {
                        Mensaje = "Copiado al portapapeles, notifique esta URL a la administracion o al paciente segun corresponda",
                        TipoImagen = 3
                    };
                    MG.ShowDialog();
                }
                else if (e.ColumnIndex == 7)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString().Trim() == "VIGENTE")
                    {
                        string urlConecta = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString().Trim();
                        System.Diagnostics.Process.Start(urlConecta);
                    }
                    else
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "No se puede conectar a esta sala debido a que la vigencia del link de conexion ya vencio.  Los links tienen 1 hora de vigencia",
                            TipoImagen = 0
                        };
                        MG.ShowDialog();
                    }                    
                }
                else
                {
                    return;
                }                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
