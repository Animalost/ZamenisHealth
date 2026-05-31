using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.INV.Consultorios;
using ZamenisHealth.Medicina.DocumentosWEB;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Enfermeria : Form
    {
        private static readonly ILogin repositorioLogin = new MLogin();
        private static readonly IBodegas repositorioBodegas = new MBodegas();

        private MensajesGeneral MG;

        public Enfermeria()
        {
            InitializeComponent();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {           
            Medicina.AgendaM M = new Medicina.AgendaM(false);
            M.ShowDialog();
        }
        private void pictureBox12_Click(object sender, EventArgs e)
        {
            HistoriasClinicas.NotasAclaratorias NA = new HistoriasClinicas.NotasAclaratorias();
            NA.TipoNota = "Curaciones";
            NA.ShowDialog();
        }
        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Medicina.CambioManejoEnfermero CME = new Medicina.CambioManejoEnfermero();
            CME.ShowDialog();
        }
        private void pictureBox14_Click(object sender, EventArgs e)
        {
            Medicina.Cargos M = (new Medicina.Cargos(false));
            M.ShowDialog();
        }
        private void pictureBox15_Click(object sender, EventArgs e)
        {
            Medicina.PlantillasEnfermeria Historia_Notas_Plantilla = new Medicina.PlantillasEnfermeria();
            Historia_Notas_Plantilla.dataGridView1.Enabled = false;
            Historia_Notas_Plantilla.ShowDialog();
        }
        private void pictureBox16_Click(object sender, EventArgs e)
        {
            Medicina.VerImagenes VI = new Medicina.VerImagenes();
            VI.ShowDialog();
        }
        private void pictureBox17_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 m = new Medicina.Historial_Medico_1();
            m.ShowDialog();
        }
        private void pictureBox36_Click(object sender, EventArgs e)
        {
            Medicina.RegImagenes RI = new Medicina.RegImagenes();
            RI.ShowDialog();
        }
        private void pictureBox75_Click(object sender, EventArgs e)
        {
            CXN_BODEGAS get = repositorioBodegas.getDatosUser(Contenedor.UsuarioLogueado);
            if (get == null) 
            {
                MG = new MensajesGeneral();
                MG.Mensaje = "Su usuario no es tipo medico o enfermero";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
            }
            else
            {
                VerInventario I = new VerInventario(get.Bod_Numero, true);
                I.ShowDialog();
            }
        }
        private void Enfermeria_Load(object sender, EventArgs e)
        {
            try
            {
                var imageJefe = repositorioLogin.getUser(Contenedor.UsuarioLogueado);
                if (imageJefe == null)
                {
                    pictureBox36.Visible = false;
                    label2.Visible = false;
                    pictureBox1.Visible = false;
                    label1.Visible = false;
                }
                else
                {
                    if (imageJefe.Log_Fotos == "A")
                    {
                        pictureBox36.Visible = true;
                        label2.Visible = true;
                        pictureBox1.Visible = true;
                        label1.Visible = true;
                    }
                    else
                    {
                        pictureBox36.Visible = false;
                        label2.Visible = false;
                        pictureBox1.Visible = false;
                        label1.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Medicina.EstInformesMG E = new Medicina.EstInformesMG();
            E.ShowDialog();
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Medicina.UploadHistory U = new Medicina.UploadHistory();
            U.ShowDialog();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MenuDocWeb M = new MenuDocWeb();
            M.ShowDialog();
        }
    }
}
