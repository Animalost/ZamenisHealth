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
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_MedicinaGeneral_2_2 : Forma2
    {
        private readonly IFirmasDigitales firmas = new MFirmasDigitales();
        private readonly IReportes reportes = new MReportes();
        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Paciente;
        DataColumn Usuario;
        DataColumn Tipo;
        DataColumn Id;

        public Historia_MedicinaGeneral_2_2()
        {
            InitializeComponent();
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
            Usuario = dt.Columns.Add("Usuario", typeof(string));
        }
        private void Historia_MedicinaGeneral_2_2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Historico Documentacion";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar un documento de paciente",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    List<CXN_FIRMASDIGITALES_MED> getList = firmas.getFirmas_MEDICAL(textBox1.Text.Trim());
                    if (getList != null)
                    {
                        Encabezados();

                        int Contador = 1;

                        foreach (var i in getList)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Id"] = i.Id;
                            row["Admision"] = i.Admision;
                            row["Fecha"] = Convert.ToDateTime(i.Fecha).ToString("yyyy-MM-dd");
                            row["Paciente"] = i.PacienteName;
                            row["Tipo"] = i.Tipo;
                            row["Usuario"] = i.Usuario;

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
                    }
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

            D.Columns["Admision"].Width = 100;
            D.Columns["Fecha"].Width = 100;
            D.Columns["Paciente"].Width = 400;
            D.Columns["Tipo"].Width = 200;
            D.Columns["Usuario"].Width = 100;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Tipo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Usuario"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;

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
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int idPos = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                int adm = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                string tipo = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

                List<CXN_CIA> f = reportes.ActasMedicos(adm, tipo);
                if (f != null)
                {
                    CXN_FIRMASDIGITALES_MED getDoc = firmas.getFirmas_MED(idPos);
                    if (getDoc != null)
                    {
                        foreach (CXN_CIA c in f)
                        {
                            c.FechaBase = getDoc.Fecha;
                            c.FirmaMed = getDoc.FirmaMedico;
                            c.FirmaPac = getDoc.FirmaPaciente;
                            c.IdProfesional = getDoc.IdProfesional;
                            c.IdPaciente = getDoc.IdPaciente;
                            c.DirPaciente = f[0].PacienteDireccion;
                            c.TelPaciente = f[0].PacienteTelefono;
                        }

                        switch (getDoc.Tipo)
                        {
                            case "Consentimiento Informado":
                                ConfigForm.GenerarReportViewer("Data_ActasMedicas", "ZamenisHealth.Reportes.RDLC_ActaConsentimiento.rdlc", f);
                                return;

                            case "Acta de Ingreso":
                                ConfigForm.GenerarReportViewer("Data_ActasMedicas", "ZamenisHealth.Reportes.RDLC_ActaIngreso.rdlc", f);
                                break;

                            case "Acta de Salida":
                                ConfigForm.GenerarReportViewer("Data_ActasMedicas", "ZamenisHealth.Reportes.RDLC_ActaEgreso.rdlc", f);
                                break;

                            default:
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Documento hecho, pero no se logro exportar, consulte en reportes de documentacion",
                                    TipoImagen = 1000
                                };

                                MG.ShowDialog();
                                break;
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Documento hecho, pero no se logro exportar, consulte en reportes de documentacion",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                    }
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Documento hecho, pero no se logro exportar, consulte en reportes de documentacion",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes buscarPacientes = new BuscarPacientes("ConInf");
            buscarPacientes.ShowDialog();
        }

        public void setDoc(string doc)
        {
            textBox1.Text = doc;
        }
    }
}
