using Domain;
using FormAndControls;
using Persistence;
using System;
using System.Collections.Generic;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Medicina.OrdenesExtra
{
    public partial class ExportarGenerados : Forma2
    {
        private List<Ordenes> OrdenMedicamentos;
        private List<Ordenes> OrdenesIncapacidad;
        private List<Ordenes> OrdenesServicios;
        private string TServ;
        private int Cia, NumOrdenIN, NumOrdenSE;

        public ExportarGenerados(List<Ordenes> ordenMedicamentos, 
                                 List<Ordenes> ordenesIncapacidad, 
                                 List<Ordenes> ordenesServicios, 
                                 string tServ, 
                                 int cia, 
                                 int numOrdenIN, 
                                 int numOrdenSE)
        {
            InitializeComponent();

            OrdenMedicamentos = ordenMedicamentos;
            OrdenesIncapacidad = ordenesIncapacidad;
            OrdenesServicios = ordenesServicios;
            TServ = tServ;
            Cia = cia;
            NumOrdenSE = numOrdenSE;
            NumOrdenIN = numOrdenIN;
        }

        private void ExportarGenerados_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Exportar";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            if (OrdenMedicamentos == null || OrdenMedicamentos.Count == 0)
            { 
                boton1.Visible = false;
            }
            if (OrdenesIncapacidad == null || OrdenesIncapacidad.Count == 0)
            {
                boton3.Visible = false;
            }
            if (OrdenesServicios == null || OrdenesServicios.Count == 0)
            {
                boton2.Visible = false;
            }
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            if (TServ == "FI")
            {
                ConfigForm.GenerarReportViewer("Dataset_OM", "ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc", OrdenesServicios);
            }
            else if (TServ == "MG" || TServ == "RA")
            {
                OrdenesMedicasT OT = new OrdenesMedicasT(Cia,
                                                         Convert.ToInt32(NumOrdenSE),
                                                         Comunes.Contenedor.UsuarioLogueado);

                OT.ShowDialog();
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            if (OrdenMedicamentos != null)
            {
                ConfigForm.GenerarReportViewer("DataSet_OM", "ZamenisHealth.Reportes.RDLC_OrdenesMedicamentos.rdlc", OrdenMedicamentos);
            }
        }
        private void boton3_Click(object sender, EventArgs e)
        {
            if (TServ == "FI")
            {
                ConfigForm.GenerarReportViewer("Dataset_OM", "ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc", OrdenesIncapacidad);
            }
            else if (TServ == "MG" || TServ == "RA")
            {
                OrdenesMedicasT OT = new OrdenesMedicasT(Cia,
                                                         Convert.ToInt32(NumOrdenIN),
                                                         Comunes.Contenedor.UsuarioLogueado);

                OT.ShowDialog();
            }
        }
    }
}
