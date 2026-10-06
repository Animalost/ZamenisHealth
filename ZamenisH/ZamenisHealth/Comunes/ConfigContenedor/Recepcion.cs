using Domain;
using Newtonsoft.Json.Linq;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Facturacion;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class Recepcion : Form
    {
        private static readonly IRoles repoRoles = new MRoles();
        private ZamenisHealth.Recepcion.AgendaDiaria.Agendamiento A;

        public bool aa;
        public Recepcion()
        {
            InitializeComponent();
        }

        private void pictureBox47_Click(object sender, EventArgs e)
        {
            if (A != null)
            {
                A.Dispose();
                A.Close();
            }               

            A = new ZamenisHealth.Recepcion.AgendaDiaria.Agendamiento();
            A.ShowDialog();
        }
        private void pictureBox48_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.CrearEditarPaciente A =new ZamenisHealth.Recepcion.CrearEditarPaciente();
            A.ShowDialog();
        }
        private void pictureBox49_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Asistencia A = new ZamenisHealth.Recepcion.Asistencia();
            A.ShowDialog();
        }
        private void pictureBox50_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Ventas.BuscarPaciente V = new ZamenisHealth.Recepcion.Ventas.BuscarPaciente();
            V.ShowDialog();
        }
        private void pictureBox51_Click(object sender, EventArgs e)
        {
            Medicina.Cargos C = new Medicina.Cargos(false);
            C.ShowDialog();
        }
        private void pictureBox52_Click(object sender, EventArgs e)
        {
            ReportesCopias F = new Facturacion.ReportesCopias(false);
            F.ShowDialog();
            /*ZamenisHealth.Recepcion.DocumentosCopias T = new ZamenisHealth.Recepcion.DocumentosCopias();
            T.ShowDialog();*/
        }
        private void pictureBox53_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Extras.Precios A = new ZamenisHealth.Recepcion.Extras.Precios();
            A.ShowDialog();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ZamenisHealth.Recepcion.Particulares P = new ZamenisHealth.Recepcion.Particulares();
            P.ShowDialog();
        }
        private void Recepcion_Load(object sender, EventArgs e)
        {
            try
            {

                CXN_DESKTOP_ROLES Rol = repoRoles.getDesktopRoles(Contenedor.UsuarioLogueado);
                if (Rol == null)
                {
                    flowLayoutPanel1.Visible = false;
                }
                else
                {
                    flowLayoutPanel1.Visible = true;

                    JObject obj = JObject.Parse(Rol.Recepcion);

                    if (obj["Recepción"] != null)
                    {
                        panel2.Visible = obj["Recepción"]["AgendaMedica"] != null ?
                                            obj["Recepción"]["AgendaMedica"].ToString() == "A" ? true : false : false;
                        panel3.Visible = obj["Recepción"]["Ventas"] != null ?
                                           obj["Recepción"]["Ventas"].ToString() == "A" ? true : false : false;
                        panel4.Visible = obj["Recepción"]["ListarPrecios"] != null ?
                                          obj["Recepción"]["ListarPrecios"].ToString() == "A" ? true : false : false;
                        panel5.Visible = obj["Recepción"]["CrearEditarPaciente"] != null ?
                                          obj["Recepción"]["CrearEditarPaciente"].ToString() == "A" ? true : false : false;
                        panel6.Visible = obj["Recepción"]["Cargos"] != null ?
                                          obj["Recepción"]["Cargos"].ToString() == "A" ? true : false : false;
                        panel7.Visible = obj["Recepción"]["Cotizaciones"] != null ?
                                         obj["Recepción"]["Cotizaciones"].ToString() == "A" ? true : false : false;
                        panel8.Visible = obj["Recepción"]["VerAsistencia"] != null ?
                                         obj["Recepción"]["VerAsistencia"].ToString() == "A" ? true : false : false;
                        panel9.Visible = obj["Recepción"]["CopiaDocumentos"] != null ?
                                        obj["Recepción"]["CopiaDocumentos"].ToString() == "A" ? true : false : false;
                    }
                    else
                    {
                        flowLayoutPanel1.Visible = false;
                    }                    
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }   
    }
}
