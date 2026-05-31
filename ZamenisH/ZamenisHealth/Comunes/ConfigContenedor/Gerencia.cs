using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Gerenciales;
using ZamenisHealth.HistoriasClinicas;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Gerencia : Form
    {
        private ICruces oController = new MCruces();
        private MensajesGeneral MG;

        public Gerencia()
        {
            InitializeComponent();
        }

        private void label55_Click(object sender, EventArgs e)
        {
            Cruces C = new Cruces();
            C.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Gerencia ver = new ZamenisHealth.Gerencia();
            ver.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            ReportesGerenciales reportesGerenciales = new ReportesGerenciales();
            reportesGerenciales.ShowDialog();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Historia_MedicinaGeneral_2_2 hMG = new Historia_MedicinaGeneral_2_2();
            hMG.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                string CargoDigitado = Microsoft.VisualBasic.Interaction.InputBox(
                                                "Digite el numero de cierre de caja.  Esta accion no se puede deshacer",
                                                "Eliminar Cierre de Caja",
                                                    "");
                if (CargoDigitado != "")
                {
                    oController.EliminaCierre(Convert.ToInt32(CargoDigitado));

                    MG = new MensajesGeneral
                    {
                        Mensaje = "Cierre de caja eliminado correctamente",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Grave", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
    }
}
 