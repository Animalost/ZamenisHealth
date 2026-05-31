using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;

namespace ZamenisHealth.Medicina.OrdenesHistory
{
    public partial class TipoOrdenMedica : Forma2
    {
        private IAgendaC oController;

        private string Forma, TServ;
        private int Admision;

        public TipoOrdenMedica(string forma, int admision)
        {
            InitializeComponent();
            Forma = forma;
            Admision = admision;

            oController = new MAgendaC();
        }

        private void TipoOrdenMedica_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Seleccione Tipo de Orden";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            otrosDatosPacienteHorario data = oController.cargarAdmision(Admision, "'A','H','P'");
            TServ = data.Hor_Pac_Tipo_Serv;
        }

        private void boton1_Click(object sender, EventArgs e)
        {
            OrdenServicios listaServicios = new OrdenServicios(Forma, Admision);
            listaServicios.ShowDialog();
        }

        private void boton3_Click(object sender, EventArgs e)
        {
            OrdenIncapacidad listaServicios = new OrdenIncapacidad(Forma, Admision);
            listaServicios.ShowDialog();
        }

        private void boton2_Click(object sender, EventArgs e)
        {
            OrdenDCI listaServicios = new OrdenDCI(Forma, Admision);
            listaServicios.ShowDialog();
        }

        private void boton4_Click(object sender, EventArgs e)
        {
            OrdenesMedicas O = new OrdenesMedicas(TServ);
            O.ShowDialog();
        }
    }
}
