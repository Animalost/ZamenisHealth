using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class EditarCupMasivo : Forma
    {
        private IConvenios oConvenios;
        private IFacturacion oFacturacion;
        private ICargos oCargos;
        private IAgendaC oAgenda;

        private MensajesGeneral MG;
        private DateTime Desde;
        private DateTime Hasta;
        private int Ase, IdPac;

        public EditarCupMasivo(DateTime desde, DateTime hasta, int ase, int idpac)
        {
            InitializeComponent();
            Desde = desde;
            Hasta = hasta;
            Ase = ase;
            IdPac = idpac;

            oConvenios = new MConvenios();
            oFacturacion = new MFacturacion();
            oCargos = new MCargos();
            oAgenda = new MAgendaC();
        }

        private void EditarCupMasivo_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cambiar CUP Masivo";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button1_Click;

            CargaServicios();
        }

        void CargaServicios()
        {
            var L = oConvenios.getConvenios(Ase);
            if (L != null)
            {
                foreach (var i in  L)
                {
                    comboBox1.Items.Add(i.Con_Id_Serv + " - " + i.Con_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }

        void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Seleccione un servico para modificar los existentes",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    string tipCargo = "";

                    switch (comboBox4.Text)
                    {
                        case "Curaciones":
                            tipCargo = "Nota";
                            break;
                        case "Medicina General":
                            tipCargo = "Historia";
                            break;
                        case "":
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Seleccion invalida",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                            return;
                    }

                    //Obtener Valores Nuevos
                    string resultadoCUP = comboBox1.Text.Split('-')[0].Trim();
                    var getNuevosDatos = oConvenios.ServicioNombre(resultadoCUP, Ase, tipCargo == "Nota" ? "CU" : "MG");
                    if (getNuevosDatos != null)
                    {
                        List<int> ListaAdmisiones = oFacturacion.GetAdmitionByType(tipCargo, IdPac, Desde, Hasta, Ase);
                        if (ListaAdmisiones != null)
                        {
                            foreach (int i in ListaAdmisiones)
                            {
                                CXN_CARGOS C = new CXN_CARGOS
                                {
                                    Car_Cod = getNuevosDatos.Con_Id_Serv,
                                    Car_Val_Un = getNuevosDatos.Con_Valor,
                                    Car_Val_Tot = getNuevosDatos.Con_Valor,
                                    Car_Item = getNuevosDatos.Con_Nombre,
                                    Car_Tipo_Serv = getNuevosDatos.Con_Tipo_Serv,
                                    Car_Adm_Id = i,
                                    Car_Tipo = tipCargo,
                                    Car_Ase = Ase
                                };

                                oCargos.updateCargosMasivo(C);
                                oAgenda.updateServicoFromFactura(i, C.Car_Cod);
                            }

                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Hecho",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();

                            this.Dispose();
                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No hay cargos para facturar en este rango",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                    }   
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Error inesperado obteniendo datos del servicio nuevo seleccionado",
                            TipoImagen = 1000
                        };
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
