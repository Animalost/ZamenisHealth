using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;

namespace ZamenisHealth.FacElectron.EmbededFac
{
    public partial class FacPrincipal : Forma2
    {
        private readonly ICompañia repoCia = new MCompañia();
        private int Cia;

        public FacPrincipal()
        {
            InitializeComponent();
        }

        private void FacPrincipal_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Facturacion Electronica";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarCia();
        }

        void CargarCia()
        {
            List<CXN_CIA> c = repoCia.getAllCompañias();
            if (c != null)
            {
                foreach (CXN_CIA c2 in c)
                {
                    comboBox1.Items.Add(c2.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();

            GenerarFacturaXML generarFacturaXML = new GenerarFacturaXML(Cia);
            generarFacturaXML.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cia = repoCia.getPrestadorbyName(comboBox1.Text).Com_Identificador;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MenuFacElectron menuFacElectron = new MenuFacElectron();
            menuFacElectron.ShowDialog();
        }

        private void button3_cLick(object sender, EventArgs e)
        {
            NotaCredito notaCredito = new NotaCredito();
            notaCredito.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
