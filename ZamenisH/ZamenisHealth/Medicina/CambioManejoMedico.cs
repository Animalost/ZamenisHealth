using System;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class CambioManejoMedico : Forma
    {
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly ICManejo repositorioCMan = new MCManenejo();
        int Paciente;

        public CambioManejoMedico()
        {
            InitializeComponent();
        }

        private void CambioManejoMedico_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Cambios de Manejo";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                ToolStripButton btnAdherencia;
                ToolStripButton btnGrabar;

                btnAdherencia = new ToolStripButton();
                btnAdherencia = createToolButton("Adherencia");
                MenuLateral.Items.Add(btnAdherencia);
                btnAdherencia.Click += toolStripLabel1_Click;

                btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += toolStripButton3_Click;

                
                var Valida = repositorioBodegas.EsProfesional("Medico", Comunes.Contenedor.UsuarioLogueado);
                if (Valida == false)
                {
                    MessageBox.Show("Su usuario no es tipo medico, no puede continuar",
                        "Acceso Denegado!!!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.Dispose();
                    this.Close();
                    return;
                }
                Carga_Grid_Datos();
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
            listView1.Columns.Add("Registro", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Paciente", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Descripcion", 0, HorizontalAlignment.Left);
        }
        private void Carga_Grid_Datos()
        {
            try
            {
                var cambiosPendientes = repositorioCMan.cargarCambiosPendientes();
                if (cambiosPendientes != null)
                {
                    Encabezados();

                    foreach (var i in cambiosPendientes)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Cam_Id.ToString(),
                            i.Cam_Adherencia,
                            Convert.ToDateTime(i.Cam_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.Cam_Descripcion
                        }));
                    }
                }
                else
                {
                    Encabezados();
                    MessageBox.Show("No hay mas resultados, todo al dia", "Terminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar una observacion"); return; }
                if (label3.Text == "") { MessageBox.Show("Debe seleccionar un registro"); return; }
                if (richTextBox1.Text == "") { MessageBox.Show("Debe seleccionar pertinencia de apositos"); return; }

                DialogResult result = MessageBox.Show("Al autorizar este cambio de manejo se agregara automaticamente a la historia clinica " +
                    "como nota aclaratoria.  ¿Desea registrarlo realmente?",
                    "Zamenis Health - Alteracion de Historias Clinicas",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var MedicoGraba = repositorioBodegas.NombreProfesionalXUser(Comunes.Contenedor.UsuarioLogueado);

                    CXN_CMAN C = new CXN_CMAN
                    {
                        Cam_UsrActualiza = MedicoGraba,
                        Cam_Adherencia = richTextBox1.Text.ToUpper(),
                        Cam_ActualizaOb = textBox2.Text,
                        Cam_Id = Convert.ToInt32(label3.Text)
                    };

                    bool _updateCMAN = repositorioCMan.updateCambioManejo(C);
                    if (_updateCMAN != true)
                    {
                        MessageBox.Show("No se logro actualizar el registro");
                    }
                    else
                    {
                        Busca_Paciente();

                        MessageBox.Show("Registrado y Cerrado");
                        textBox1.Text = "";
                        textBox2.Text = "";
                        richTextBox1.Text = "";
                        label3.Text = "";
                        Carga_Grid_Datos();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Busca_Paciente()
        {
            try
            {
                int getIdPac = repositorioCMan.getIdPacByIdCMan(Convert.ToInt32(label3.Text));
                if (getIdPac >= 1)
                {
                    Paciente = getIdPac;
                    Alterar_Historia(Paciente);
                }
                else
                {
                    MessageBox.Show("No se logro ubicar el paciente, se guardara el cambio de manejo " +
                           "pero no se anexara a la historia clinica por el momento",
                           "Grabacion con incidencia pero puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Alterar_Historia(int Dato)
        {
            try
            {
                var getDatos = repositorioCMan.getLastIdByIdPac(Dato);
                if (getDatos.IdHc >= 1)
                {
                    DateTime Hoy = DateTime.Now;
                    var MedicoGraba = repositorioBodegas.NombreProfesionalXUser(Comunes.Contenedor.UsuarioLogueado);

                    string Datos_Grabar = getDatos.NotaAclaratoria + " | " + textBox1.Text + "\n\r" +
                        textBox2.Text + "\n\r" + richTextBox1.Text + "\n\r" + " | Nota Agregada por: " + MedicoGraba + " > " + Convert.ToDateTime(Hoy.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]);

                    bool insertNotaAclaratoria = repositorioCMan.insertNotaAclaratoria(getDatos.IdHc, Datos_Grabar);
                    if (insertNotaAclaratoria != true)
                    {
                        MessageBox.Show("No se logro grabar la nota en la historia clinica",
                         "Grabacion con incidencia pero puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    MessageBox.Show("Agregado correcto",
                         "Grabacion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("No se logro ubicar la historia, se guardara el cambio de manejo " +
                          "pero no se anexara a la historia clinica por el momento",
                          "Grabacion con incidencia pero puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "Sin apositos para esta historia clinica";
        }
        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                label3.Text = listView1.SelectedItems[0].SubItems[0].Text;
                textBox1.Text = listView1.SelectedItems[0].SubItems[3].Text;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            Medicina.Adherencia historia_Notas_Adherencia = new Medicina.Adherencia("CMAN");
            historia_Notas_Adherencia.ShowDialog();
        }
    }
}
