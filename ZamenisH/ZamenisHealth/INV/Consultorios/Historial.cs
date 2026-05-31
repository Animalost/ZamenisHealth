using Domain.CXN;
using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV.Consultorios
{
    public partial class Historial : Forma
    {
        private ICompañia compañia;
        private ISubBodegas subbodegas;
        private int CodeCia;
        private MensajesGeneral MG;

        public Historial()
        {
            InitializeComponent();
            compañia = new MCompañia();
            subbodegas = new MSubBodegas();
        }

        private void Historial_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Historial";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnConsultar = new ToolStripButton();
            btnConsultar = createToolButton("Consultar");
            MenuLateral.Items.Add(btnConsultar);
            btnConsultar.Click += toolStripButton2_Click;

            List <CXN_CIA> c = compañia.getAllCompañias();
            if (c != null)
            {
                foreach (CXN_CIA c2 in c)
                {
                    comboBox1.Items.Add(c2.Com_Nombre);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CodeCia = compañia.getPrestadorbyName(comboBox1.Text).Com_Identificador;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar una opcion de los 3 campos";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    DateTime Desde = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox3.Text), 1);
                    DateTime Hasta = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                    List<INV_HISTORICOPPAL> getList = subbodegas.getHistorial(Desde, Hasta);
                    if (getList != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSet_HistoricoSub", "ZamenisHealth.Reportes.RDLC_HistorialSubBodegas.rdlc", getList);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No hay resultados para estos criterios seleccionados";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
