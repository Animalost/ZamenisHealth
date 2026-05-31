using System;
using System.Drawing;
using System.Windows.Forms;
using Domain;
using Domain.Fibromialgia;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Persistence.Fibromialgia.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Fibromialgia.Encuestas.Informe
{
    public partial class InformesEncuestas : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IEncuestasXPaciente repoEncuestasXPaciente = new MEncuestasXPaciente();
        private static readonly IConsolidadoSF36 repoConsolidadoSF36 = new MConsolidadoSF36();
        private static readonly IConsolidadoSF36Porcentual repoConsolidadoSF36Porcentual = new MConsolidadoSF36Porcentual();
        private static readonly IConsolidadoGeneral repoConsolidadoGeneral = new MConsolidadoGeneral();
        private static readonly IEncuestaSatisfaccion repoEncuestaSatisfaccion = new MEncuestaSatisfaccion();
        private static readonly IImpactoFibromialgia repoImpactoFibromialgia = new MImpactoFibromialgia();
        private static readonly ITFGeneral repoTFGeneral = new MTFGeneral();

        private int AdmitionChange = 0;
        public InformesEncuestas()
        {
            InitializeComponent();
        }

        private void InformesEncuestas_Load(object sender, EventArgs e)
        {
            try
            {
                this.Titulo.Text = "Generacion de Informes Encuestas de Pacientes";

                var getDocs = repoPac.ListaDocs();
                if (getDocs != null)
                {
                    foreach (var i in getDocs)
                    {
                        comboBox2.Items.Add(i);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == 1)
                {
                    groupBox1.Visible = true;
                    this.Size = new Size(672, 269);
                }
                if (comboBox1.SelectedIndex == 2)
                {
                    groupBox1.Visible = false;
                    this.Size = new Size(672, 269);
                }
                if (comboBox1.SelectedIndex == 3)
                {
                    groupBox1.Visible = false;
                    this.Size = new Size(672, 269);
                }
                if (comboBox1.SelectedIndex == 4)
                {
                    groupBox1.Visible = false;
                    this.Size = new Size(672, 269);
                }
                if (comboBox1.SelectedIndex == 5)
                {
                    groupBox1.Visible = false;
                    this.Size = new Size(672, 269);
                }
                if (comboBox1.SelectedIndex == 6)
                {
                    groupBox1.Visible = false;
                    this.Size = new Size(672, 269);
                }
                if (comboBox1.SelectedIndex == 7)
                {
                    groupBox1.Visible = true;
                    this.Size = new Size(672, 442);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new MensajesGeneral();

                if (comboBox1.SelectedIndex == 1)
                {
                    if (comboBox2.Text == "" || textBox1.Text == "")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione tipo de documento del paciente y digite el numero de documento del paciente";
                        MG.ShowDialog();
                        return;
                    }

                    var getPac = repoPac.LlamarPacienteDOC(comboBox2.Text, textBox1.Text);
                    if (getPac != null)
                    {

                        var getInforme = repoEncuestasXPaciente.ExportarResGlobalPaciente(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                                   Convert.ToDateTime(dateTimePicker2.Value.Date),
                                                                                                   comboBox2.Text,
                                                                                                   textBox1.Text,
                                                                                                   getPac.Pac_PrimerA + " " +
                                                                                                   getPac.Pac_SegundoA + " " +
                                                                                                   getPac.Pac_PrimerN + " " +
                                                                                                   getPac.Pac_SegundoN);
                        if (getInforme != null)
                        {
                            ConfigForm.GenerarReportViewer("DatasetInformesFibro",
              "ZamenisHealth.Reportes.Fibro.InformesEncuestas.EncuestasXPaciente.rdlc",
              getInforme);

                        }
                        else
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No hay datos para exportar";
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "El documento digitado no existe";
                        MG.ShowDialog();
                        return;
                    }
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    var getInforme = repoConsolidadoSF36.getInformeRDL(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                      Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSetConsolidadoSF36",
              "ZamenisHealth.Reportes.Fibro.InformesEncuestas.ConsolidadoSF36.rdlc",
              getInforme);

                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    var getInforme = repoConsolidadoSF36Porcentual.getInformeRDLC(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                      Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DatasetSF36Porcentual",
              "ZamenisHealth.Reportes.Fibro.InformesEncuestas.ConsolidadoSF36Porcentual.rdlc",
              getInforme);

                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (comboBox1.SelectedIndex == 4)
                {
                    var getInforme = repoConsolidadoGeneral.generateReportRDLC(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                                     Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DatasetConsolidadoGeneral",
              "ZamenisHealth.Reportes.Fibro.InformesEncuestas.InformeFibro.rdlc",
              getInforme);

                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (comboBox1.SelectedIndex == 5)
                {
                    var getInforme = repoEncuestaSatisfaccion.ExportarResGlobalGeneral(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                                     Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSetSatisfaccion",
              "ZamenisHealth.Reportes.Fibro.InformesEncuestas.EncuestaSatisfaccion.rdlc",
              getInforme);

                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (comboBox1.SelectedIndex == 6)
                {
                    var getInforme = repoImpactoFibromialgia.ExportarImpacto(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                          Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSetImpacto",
             "ZamenisHealth.Reportes.Fibro.InformesEncuestas.ImpactoFibromialgia.rdlc",
             getInforme);

                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (comboBox1.SelectedIndex == 7)
                {
                    if (comboBox2.Text == "" || textBox1.Text == "")
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione tipo de documento del paciente y digite el numero de documento del paciente";
                        MG.ShowDialog();
                        return;
                    }

                    var getPac = repoPac.LlamarPacienteDOC(comboBox2.Text, textBox1.Text);
                    if (getPac != null)
                    {
                        DatosForInforme D = new DatosForInforme
                        {
                            Desde = Convert.ToDateTime(dateTimePicker1.Value.Date),
                            Hasta = Convert.ToDateTime(dateTimePicker2.Value.Date),
                            TID = comboBox2.Text,
                            IDD = textBox1.Text,
                            Name = getPac.Pac_PrimerA + " " +
                                   getPac.Pac_SegundoA + " " +
                                   getPac.Pac_PrimerN + " " +
                                   getPac.Pac_SegundoN
                        };

                        var getInforme = repoTFGeneral.ExportarInforme1TF(D);
                        if (getInforme != null)
                        {
                            var getHistorys = repoTFGeneral.getAllTF(getPac.Pac_Id, Convert.ToDateTime(dateTimePicker1.Value.Date), Convert.ToDateTime(dateTimePicker2.Value.Date));
                            if (getHistorys != null)
                            {
                                Encabezados();

                                foreach (var i in getHistorys)
                                {
                                    listView1.Items.Add(new ListViewItem(new string[]
                                    {
                                         i.HC_Adm.ToString(),
                                         Convert.ToDateTime(i.HC_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                         i.HC_Pac.ToString(),
                                         i.TipoHistoria.ToString()
                                    }));
                                }
                            }
                            else
                            {
                                Encabezados();
                            }

                            ConfigForm.GenerarReportViewer("DatasetInformeTF",
             "ZamenisHealth.Reportes.Fibro.InformesEncuestas.InformeTF.rdlc",
             getInforme);

                        }
                        else
                        {
                            Encabezados();

                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No hay datos para exportar";
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "El documento digitado no existe";
                        MG.ShowDialog();
                        return;
                    }
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay datos para exportar";
                    MG.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Admision", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Paciente", 300, HorizontalAlignment.Left);
            listView1.Columns.Add("Tipo", 80, HorizontalAlignment.Left);
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                AdmitionChange = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral Mg = new MensajesGeneral();

                if (AdmitionChange <= 0 || comboBox3.Text == "")
                {
                    Mg.TipoImagen = 1000;
                    Mg.Mensaje = "Debe seleccionar una historia para cambiar su estado y un estado de la lista desplegable";
                    Mg.ShowDialog();
                    return;
                }

                bool update = repoTFGeneral.updateTipoHistoria(AdmitionChange, comboBox3.Text);
                if (update != true)
                {
                    Mg.TipoImagen = 1000;
                    Mg.Mensaje = "No se logro actualizar la historia";
                    Mg.ShowDialog();
                }
                else
                {
                    Mg.TipoImagen = 3;
                    Mg.Mensaje = "Actualizado, haga click en buscar nuevamente para visualizar el cambio";
                    Mg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
