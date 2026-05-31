using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Formatos : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IFormatos repoFormatos = new MFormatos();
        private static readonly ICompañia repoCompañia = new MCompañia();

        int Cia;
        int Pac;
        public Formatos()
        {
            InitializeComponent();
        }
        
        private void CargarDocumentos()
        {
            var ListaDocs = repoPacientes.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox3.Items.Add(i);
                }

                comboBox3.SelectedIndex = 0;
            }

            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox4.Items.Add(i);
                }

                comboBox4.SelectedIndex = 0;
            }
        }

        private void Formatos_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Formatos";

                var _getFormatos = repoFormatos.getFormatos();
                if (_getFormatos != null)
                {
                    foreach (var i in _getFormatos)
                    {
                        comboBox1.Items.Add(i);
                    }

                    comboBox1.SelectedIndex = 0;
                }

                var _getCias = repoCompañia.getAllCompañias();
                if (_getCias != null)
                {
                    foreach (var i in _getCias)
                    {
                        comboBox2.Items.Add(i.Com_Nombre);
                    }

                    comboBox2.SelectedIndex = 0;
                }

                CargarDocumentos();

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
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ciasid = repoCompañia.getPrestadorbyName(comboBox2.Text);
            Cia = ciasid.Com_Identificador;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var getFormatS = repoFormatos.getFormatoSelected(comboBox1.Text);
                if (getFormatS != null)
                {
                    textBox1.Text = getFormatS.CiudadFecha;
                    textBox2.Text = getFormatS.Cuerpo;
                    textBox3.Text = getFormatS.Firma;
                    textBox4.Text = getFormatS.Firma2;
                    textBox5.Text = getFormatS.Pie;
                }
                else
                {
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
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

        private void textBox6_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var _getPac = repoPacientes.LlamarPacienteDOC(comboBox3.Text, textBox6.Text);
                if (_getPac != null)
                {
                    Pac = _getPac.Pac_Id;
                    textBox7.Text = _getPac.Pac_PrimerN + " " + _getPac.Pac_SegundoN + " " + _getPac.Pac_PrimerA + " " + _getPac.Pac_SegundoA;
                }
                else
                {
                    textBox7.Text = "";
                    Pac = 0;
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (Pac == 0) { MessageBox.Show("Debe escoger un paciente"); return; }
            if (Cia == 0) { MessageBox.Show("Debe seleccionar una compañia"); return; }
            if (textBox1.Text == "") { MessageBox.Show("Debe asignar una fecha"); return; }
            if (textBox2.Text == "") { MessageBox.Show("Debe escribir un cuerpo del mensaje"); return; }
            if (textBox3.Text == "") { MessageBox.Show("Debe asignar al menos la primera firma"); return; }
            if (textBox5.Text == "") { MessageBox.Show("Debe asignar el pie de pagina"); return; }

            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                DateTime Hoy = DateTime.Now;

                CXN_FORMATOS F = new CXN_FORMATOS
                {
                    Paciente = Pac,
                    CiudadFecha = textBox1.Text,
                    Compañia = Cia,
                    Cuerpo = textBox2.Text,
                    Pie = textBox5.Text,
                    Firma = textBox3.Text,
                    Firma2 = textBox4.Text,
                    Fecha = Convert.ToDateTime(Hoy),
                    Usuario = Comunes.Contenedor.UsuarioLogueado
                };

                bool _createDocument = repoFormatos.insertarFormato(F);
                if (_createDocument == true)
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Generado";
                    MG.ShowDialog();

                    var Report = repoFormatos.Export_Cert(Pac, 0, "Nuevo");
                    if (Report == null)
                    {
                        MessageBox.Show("Error de exportacion, pruebe a reimprimir una copia",
                            "Error Inesperado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        this.Dispose();
                        this.Close();
                        return;
                    }

                    ConfigForm.GenerarReportViewer("DataSet_Formatos",
                                            "ZamenisHealth.Reportes.RDLC_Formatos.rdlc",
                                            Report);

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro generar el documento";
                    MG.ShowDialog();
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                DateTime Hoy = DateTime.Now.Date;

                CXN_FORMATOS F = new CXN_FORMATOS
                {
                    Cuerpo = textBox2.Text,
                    Firma = textBox3.Text,
                    Firma2 = textBox4.Text,
                    Pie = textBox5.Text,
                    CiudadFecha = textBox1.Text,
                    Fecha = Convert.ToDateTime(Hoy),
                    Usuario = Comunes.Contenedor.UsuarioLogueado,
                    Nombre = comboBox1.Text
                };

                bool _updateFormtat = repoFormatos.updateFormato(F);
                if (_updateFormtat != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar el formato";
                    MG.ShowDialog();
                    return;
                }

                MG.TipoImagen = 3;
                MG.Mensaje = "Plantilla actualizada con exito";
                MG.ShowDialog();

                this.Dispose();
                this.Close();
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_FORMATOS F = new CXN_FORMATOS
                {
                    Nombre = comboBox1.Text,
                    Firma = textBox3.Text,
                    Firma2 = textBox4.Text,
                    Pie = textBox5.Text,
                    Cuerpo = textBox2.Text,
                };

                var Ejecutar = repoFormatos.Plantilla(F);
                if (Ejecutar == null)
                {
                    MessageBox.Show("No se logro exportar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet_Formatos",
                                            "ZamenisHealth.Reportes.RDLC_Formatos.rdlc",
                                            Ejecutar);
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

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            if (Cia == 0) { MessageBox.Show("Debe seleccionar una compañia"); return; }
            if (textBox1.Text == "") { MessageBox.Show("Debe asignar una fecha"); return; }
            if (textBox2.Text == "") { MessageBox.Show("Debe escribir un cuerpo del mensaje"); return; }
            if (textBox3.Text == "") { MessageBox.Show("Debe asignar primera firma"); return; }
            if (textBox4.Text == "") { MessageBox.Show("Debe asignar segunda firma"); return; }
            if (textBox5.Text == "") { MessageBox.Show("Debe asignar el pie de pagina"); return; }
            if (textBox8.Text == "") { MessageBox.Show("Debe asignar el nombre de la plantilla nueva"); return; }

            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                DateTime Hoy = DateTime.Now;

                CXN_FORMATOS F = new CXN_FORMATOS
                {
                    Nombre = textBox8.Text,
                    Cuerpo = textBox2.Text,
                    Firma = textBox3.Text,
                    Firma2 = textBox4.Text,
                    Pie = textBox5.Text,
                    CiudadFecha = textBox1.Text,
                    Fecha = Convert.ToDateTime(Hoy),
                    Usuario = Comunes.Contenedor.UsuarioLogueado
                };

                bool crearFormato = repoFormatos.crearFormato(F);
                if (crearFormato != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro crear el formato";
                    MG.ShowDialog();
                    return;
                }

                MG.TipoImagen = 3;
                MG.Mensaje = "Formato creado con exito";
                MG.ShowDialog();

                this.Dispose();
                this.Close();
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

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            groupBox2.Visible = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox2.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                DataTable dt = new DataTable();
                DataColumn ID = dt.Columns.Add("ID", typeof(string));
                DataColumn Paciente = dt.Columns.Add("Paciente", typeof(string));
                DataColumn Fecha = dt.Columns.Add("Fecha", typeof(string));
                DataColumn Usuario = dt.Columns.Add("Usuario", typeof(string));

                var getPrevDocsByPac = repoFormatos.getFormatosxPaciente(comboBox4.Text, textBox9.Text);
                if (getPrevDocsByPac != null)
                {
                    dataGridView1.Visible = true;

                    foreach (var i in getPrevDocsByPac)
                    {
                        DataRow row = dt.NewRow();

                        row["ID"] = i.Id.ToString();
                        row["Paciente"] = i.Cuerpo;
                        row["Fecha"] = Convert.ToDateTime(i.Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Usuario"] = i.Usuario;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns["ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["Usuario"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["ID"].Width = 80;
                    dataGridView1.Columns["Paciente"].Width = 300;
                    dataGridView1.Columns["Fecha"].Width = 120;
                    dataGridView1.Columns["Usuario"].Width = 120;
                }
                else
                {
                    dataGridView1.Visible = false;
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay registros para este documento";
                    MG.ShowDialog();
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

        private void textBox9_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "Formatos";
            buscarPacientes.ShowDialog();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var Report = repoFormatos.Export_Cert(0, Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()), "Previo");
                if (Report == null)
                {
                    MessageBox.Show("Error de exportacion",
                        "Error Inesperado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet_Formatos",
                                            "ZamenisHealth.Reportes.RDLC_Formatos.rdlc",
                                            Report);
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
