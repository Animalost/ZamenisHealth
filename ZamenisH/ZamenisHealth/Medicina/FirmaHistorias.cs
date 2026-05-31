using System;
using System.Windows.Forms;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Medicina
{
    public partial class FirmaHistorias : ConfigForm.BaseForm
    {
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly IJuntas repositorioJuntas = new MJuntas();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        public FirmaHistorias()
        {
            InitializeComponent();
            
            ConfigForm.MoverForma(TittleLbl, this);
        }

        public void CargarDocumentos()
        {
            var ListaDocs = repositorioPacientes.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox2.Items.Add(i);
                }
            }
        }

        private void FirmaHistorias_Load(object sender, EventArgs e)
        {
            
            CargarDocumentos();

            var esmedico = repositorioBodegas.EsProfesional("Medico", Comunes.Contenedor.UsuarioLogueado);
            if (esmedico != true)
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "Su usuario no es tipo medico, no puede firmar historias clinicas";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                this.Dispose();
                this.Close();
                return;
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes("FirmaDocs");
            buscarPacientes.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Admision", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Tipo", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Firmado", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Profesional", 250, HorizontalAlignment.Left);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || textBox1.Text == "" || comboBox1.Text == "")
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "Debe seleccionar los campos de busqueda completos";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }

            Consultar();
        }
        private void Consultar()
        {
            try
            {
                var lista = repositorioJuntas.listarJuntasFirma(comboBox2.Text, textBox1.Text);
                if (lista != null)
                {
                    Encabezados();

                    foreach (var i in lista)
                    {
                        label4.Text = i.Jun_TF; //paciente

                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                             i.Jun_Adm.ToString(),
                             Convert.ToDateTime(i.Jun_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                             i.Jun_Tipo,
                             i.Jun_Observa,
                             i.Jun_MedFirma
                        }));
                    }
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " - Error Lista de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0].SubItems[3].Text == "Firmado")
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "Este documento ya esta firmado";
                MG.TipoImagen = 3;
                MG.ShowDialog();
                return;
            }

            DialogResult result = MessageBox.Show("¿Desea firmar digitalmente esta historia?",
                                                 "Zamenis Health - Firma de Historias",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                repositorioJuntas.Firmar(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text), Comunes.Contenedor.UsuarioLogueado);

                Consultar();

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "Documento Firmado Correctamente";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
        }
    }
}
