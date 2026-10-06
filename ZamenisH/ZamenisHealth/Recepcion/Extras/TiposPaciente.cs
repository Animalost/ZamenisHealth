using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class TiposPaciente : Forma
    {
        private IAgendaC oAgendaC;

        DataTable dt;
        DataColumn Paciente;
        DataColumn TipoDoc;
        DataColumn Doc;
        DataColumn DobleEspacio;
        DataColumn DVxS;
        DataColumn Bonos;
        DataColumn PACID;


        public TiposPaciente()
        {
            InitializeComponent();
            oAgendaC = new MAgendaC();
        }

        private void TiposPaciente_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cambiar Tipos de Paciente";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            
            gridZH1.dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            gridZH1.CeldaHeight = true;

            ToolStripButton btExportar = new ToolStripButton();
            btExportar = createToolButton("Exportar");
            MenuLateral.Items.Add(btExportar);
            btExportar.Click += btExportar_Click;

            ToolStripButton btnRecargar = new ToolStripButton();
            btnRecargar = createToolButton("Recargar");
            MenuLateral.Items.Add(btnRecargar);
            btnRecargar.Click += btnRecargar_Click;

            comboBox1.SelectedIndex = 0;
        }
        void btExportar_Click(object sender, EventArgs e)
        {
            try
            {
                string Texto = "PACIENTE|TIPO DE DOCUMENTO|DOCUMENTO|DOBLE ESPACIO|VARIAS VECES POR SEMANA|REQUIERE BONO \r";

                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    object valor = row.Cells["Doble Espacio"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);
                    string De = isChecked == true ? "SI" : "NO";

                    object valor1 = row.Cells["Varias Veces por Semana"].Value;
                    bool isChecked1 = valor1 != null && Convert.ToBoolean(valor1);
                    string DvXs = isChecked1 == true ? "SI" : "NO";

                    object valor2 = row.Cells["Requiere Bono"].Value;
                    bool isChecked2 = valor2 != null && Convert.ToBoolean(valor2);
                    string RB = isChecked2 == true ? "SI" : "NO";

                    Texto = Texto + row.Cells["Paciente"].Value.ToString() + "|" +
                                    row.Cells["Tipo Documento"].Value.ToString() + "|" +
                                    row.Cells["Documento"].Value.ToString() + "|" +
                                    De + "|" +
                                    DvXs + "|" +
                                    RB + "\r";
                }

                string NameFile = "C:/Cxn/Reportes/TiposPaciente_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".xls";
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
        void btnRecargar_Click(object sender, EventArgs e)
        {
            CargarGrilla();
        }
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;

                int _pacid = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString());

                if (e.ColumnIndex == 3) // Doble Espacio
                {
                    bool DE = Convert.ToBoolean(
                        gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].EditedFormattedValue
                    );

                    int Save = oAgendaC.UpdateTipoPaciente(_pacid, "DobleEspacio", DE);                   
                }
                else if (e.ColumnIndex == 4) // Varias veces por semana
                {
                    bool DVXS = Convert.ToBoolean(
                        gridZH1.dataGridView1.Rows[e.RowIndex].Cells[4].EditedFormattedValue
                    );

                    int Save = oAgendaC.UpdateTipoPaciente(_pacid, "2VxS", DVXS);
                }
                else if (e.ColumnIndex == 5) // Bonos
                {
                    bool BONO = Convert.ToBoolean(
                        gridZH1.dataGridView1.Rows[e.RowIndex].Cells[5].EditedFormattedValue
                    );

                    int Save = oAgendaC.UpdateTipoPaciente(_pacid, "Bonos", BONO);
                }
                else if (e.ColumnIndex == 2) // Copiar Documento
                {
                    Clipboard.SetText(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString().Trim());
                    MessageBox.Show("Documento Copiado", "Copiado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CargarGrilla()
        {
            try
            {
                List<CXN_PACIENTES> _lista = new List<CXN_PACIENTES>();

                if (comboBox1.Text == "Todo")
                {                    
                    _lista = oAgendaC.ListarTiposPaciente("", comboBox1.Text);
                }
                else
                {
                    _lista = oAgendaC.ListarTiposPaciente(textBox1.Text, comboBox1.Text);
                }
               
                if (_lista != null)
                {
                    var _listaFin = _lista.GroupBy(x => x.Pac_IdNum).Select(g => g.First()).ToList();

                    Encabezados();

                    foreach (CXN_PACIENTES i in _listaFin)
                    {
                        DataRow row = dt.NewRow();

                        row[Paciente] = i.Pac_PrimerN;
                        row[TipoDoc] = i.Pac_TipoId;
                        row[Doc] = i.Pac_IdNum;
                        row[DobleEspacio] = i.Pac_Doble == "N" ? false : true; //N S
                        row[DVxS] = i.Pac_2VXS == "N" ? false : true; //N S
                        row[Bonos] = i.Pac_Bonos == "N" ? false : true; //N A
                        row[PACID] = i.Pac_Id;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    gridZH1.dataGridView1.DataSource = null;
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CargarPorFiltro(string TipoFiltro)
        {
            try
            {
                List<CXN_PACIENTES> _lista = oAgendaC.ListarTiposPaciente("", comboBox1.Text);
                if (_lista != null)
                {
                    List<CXN_PACIENTES> _listaTemp = _lista.GroupBy(x => x.Pac_IdNum).Select(g => g.First()).ToList();
                    List<CXN_PACIENTES> _listaFin = new List<CXN_PACIENTES>();

                    if (TipoFiltro == "B")
                    {
                        _listaFin = _listaTemp.Where(x => x.Pac_Bonos == "A").OrderBy(x => x.Pac_PrimerN).ToList();
                    }
                    else if (TipoFiltro == "D")
                    {
                        _listaFin = _listaTemp.Where(x => x.Pac_Doble == "S").OrderBy(x => x.Pac_PrimerN).ToList();
                    }
                    else if (TipoFiltro == "V")
                    {
                        _listaFin = _listaTemp.Where(x => x.Pac_2VXS == "S").OrderBy(x => x.Pac_PrimerN).ToList();
                    }
                    else
                    {
                        Encabezados();
                        return;
                    }

                    Encabezados();

                    foreach (CXN_PACIENTES i in _listaFin)
                    {
                        DataRow row = dt.NewRow();

                        row[Paciente] = i.Pac_PrimerN;
                        row[TipoDoc] = i.Pac_TipoId;
                        row[Doc] = i.Pac_IdNum;
                        row[DobleEspacio] = i.Pac_Doble == "N" ? false : true; //N S
                        row[DVxS] = i.Pac_2VXS == "N" ? false : true; //N S
                        row[Bonos] = i.Pac_Bonos == "N" ? false : true; //N A
                        row[PACID] = i.Pac_Id;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    gridZH1.dataGridView1.DataSource = null;
                    Encabezados();
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
            D.ReadOnly = false;

            D.Columns["Doble Espacio"].ReadOnly = false;
            D.Columns["Varias Veces por Semana"].ReadOnly = false;
            D.Columns["Requiere Bono"].ReadOnly = false;
            D.Columns["PAciente"].ReadOnly = true;
            D.Columns["Tipo Documento"].ReadOnly = true;
            D.Columns["Documento"].ReadOnly = true;

            D.Columns["Doble Espacio"].Width = 50;
            D.Columns["Varias Veces por Semana"].Width = 50;
            D.Columns["Requiere Bono"].Width = 50;

            D.Columns["PACID"].Visible = false;
            gridZH1.dataGridView1.ClearSelection();
        }
        void Encabezados()
        {
            dt = new DataTable();
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            TipoDoc = dt.Columns.Add("Tipo Documento", typeof(string));
            Doc = dt.Columns.Add("Documento", typeof(string));
            DobleEspacio = dt.Columns.Add("Doble Espacio", typeof(bool));
            DVxS = dt.Columns.Add("Varias Veces por Semana", typeof(bool));
            Bonos = dt.Columns.Add("Requiere Bono", typeof(bool));
            PACID = dt.Columns.Add("PACID", typeof(int));
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Todo")
            {
                textBox1.Text = "";
                textBox1.Visible = false;
                boton1.Visible = false;
                CargarGrilla();
            }
            else if (comboBox1.Text == "--------------------------------------")
            {
                comboBox1.SelectedIndex = 1;
            }
            else if (comboBox1.Text == "Solo Bonos")
            {
                CargarPorFiltro("B");
            }
            else if (comboBox1.Text == "Solo Doble Espacio")
            {
                CargarPorFiltro("D");
            }
            else if (comboBox1.Text == "Solo Varias Veces por Semana")
            {
                CargarPorFiltro("V");
            }
            else
            {
                textBox1.Visible = true;
                boton1.Visible = true;
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            CargarGrilla();
        }
    }
}
