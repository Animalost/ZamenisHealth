using Domain;
using Domain.CXN;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Historial_Medico_3 : Forma
    {
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IOrdenes repoOrdenes = new MOrdenes();
        private static readonly ICompañia repoCompañia = new MCompañia();

        private int cia, Orden;
        private string Tipo;
        
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (label6.Text == "") { MessageBox.Show("No hay orden seleccionada", "Verifique", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                switch (comboBox3.Text)
                {
                    case "Deseo reimprimir una copia de la orden medica":
                        Exportar(Convert.ToInt32(label6.Text));
                        break;

                    case "Deseo renovar la orden medica identica":
                        Renovar();
                        break;

                    case "Deseo actualizar algunos datos y renovar la orden medica":
                        Renovar();
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        public Historial_Medico_3(int _orden, string _tipo, string _espe, int _cia)
        {
            InitializeComponent(); 
            this.Orden = _orden;
            this.Tipo = _tipo;
            this.cia = _cia;
        }
        private void Historial_Medico_3_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Ordenes Medicas";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnProcesar = new ToolStripButton();
                btnProcesar = createToolButton("Procesar");
                MenuLateral.Items.Add(btnProcesar);
                btnProcesar.Click += button3_Click;

                this.VistaPrevia.RefreshReport();
                label6.Text = this.Orden.ToString();

                if (Tipo == "Servicios")
                {
                    var CopyServices = repoOrdenes.Generar_OrdenMedica(Convert.ToInt32(label6.Text), cia, Comunes.Contenedor.UsuarioLogueado);
                    if (CopyServices == null)
                    {
                        MessageBox.Show("No se logro exportar la orden",
                            "Error generico",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    VistaPrevia.LocalReport.DataSources.Clear();
                    VistaPrevia.LocalReport.DataSources.Add(new ReportDataSource("Dataset_OM", CopyServices));
                    VistaPrevia.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc";
                    VistaPrevia.SetDisplayMode(DisplayMode.PrintLayout);
                    VistaPrevia.ZoomMode = ZoomMode.Percent;
                    VistaPrevia.ZoomPercent = 75;
                    VistaPrevia.LocalReport.EnableExternalImages = true;
                    VistaPrevia.RefreshReport();
                    VistaPrevia.Visible = true;
                    VistaPrevia.Dock = System.Windows.Forms.DockStyle.Fill;
                }

                if (Tipo == "Medicamentos")
                {
                    var CopyMedicamento = repoOrdenes.Genera_Orden_Medicamento(Convert.ToInt32(label6.Text), cia, Contenedor.UsuarioLogueado);
                    if (CopyMedicamento == null)
                    {
                        MessageBox.Show("No se logro exportar la orden",
                            "Error generico",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    VistaPrevia.LocalReport.DataSources.Clear();
                    VistaPrevia.LocalReport.DataSources.Add(new ReportDataSource("DataSet_OM", CopyMedicamento));
                    VistaPrevia.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_OrdenesMedicamentos.rdlc";
                    VistaPrevia.SetDisplayMode(DisplayMode.PrintLayout);
                    VistaPrevia.ZoomMode = ZoomMode.Percent;
                    VistaPrevia.ZoomPercent = 75;
                    VistaPrevia.LocalReport.EnableExternalImages = true;
                    VistaPrevia.RefreshReport();
                    VistaPrevia.Visible = true;
                    VistaPrevia.Dock = System.Windows.Forms.DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Exportar(int Numero)
        {
            try
            {
                if (Tipo == "Servicios")
                {
                    var CopyServices = repoOrdenes.Generar_OrdenMedica(Numero, cia, Comunes.Contenedor.UsuarioLogueado);
                    if (CopyServices != null)
                    {
                        OrdenesMedicasT OT = new OrdenesMedicasT(cia,
                                                         Convert.ToInt32(Numero),
                                                         Comunes.Contenedor.UsuarioLogueado);

                        OT.ShowDialog();                     
                    }
                }

                if (Tipo == "Medicamentos")
                {
                    var CopyMedicamento = repoOrdenes.Genera_Orden_Medicamento(Numero, cia, Contenedor.UsuarioLogueado);
                    if (CopyMedicamento == null)
                    {
                        MessageBox.Show("No se logro exportar la orden",
                            "Error generico",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    ConfigForm.GenerarReportViewer("DataSet_OM",
               "ZamenisHealth.Reportes.RDLC_OrdenesMedicamentos.rdlc",
               CopyMedicamento);

                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Renovar()
        {
            try
            {
                string T = "";
                if (Tipo == "Servicios")
                {
                    T = "S";
                }

                if (Tipo == "Medicamentos")
                {
                    T = "M";
                }

                var Valida = repoBodegas.EsProfesional("Medico", Comunes.Contenedor.UsuarioLogueado);
                if (Valida == false)
                {
                    MessageBox.Show("Su usuario no es tipo medico NO puede generar nuevas ordenes medicas",
                        "Acceso Denegado!!!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult result = MessageBox.Show("¿Desea renovar la fecha de esta orden?",
                   "Zamenis_Health",
                   MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        //var MedRenueva = Clases.General.ProfesionalCodeByUser(Comunes.Contenedor.UsuarioLogueado);

                        var getOM = repoOrdenes.getOrden(Convert.ToInt32(label6.Text), T, cia);
                        if (getOM != null)
                        {
                            var NumOrden = repoCompañia.getPrestadorbyCode(cia);
                            if (NumOrden == null)
                            {
                                MessageBox.Show("Error en numero de orden a asignar", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            DateTime OM_Fecha = DateTime.Now;
                            string OM_Desc = "";

                            if (comboBox3.Text == "Deseo actualizar algunos datos y renovar la orden medica")
                            {
                                if (Tipo == "Medicamentos")
                                {
                                    MessageBox.Show("Solamente puede renovar y modificar ordenes medicas que sean de tipo servicios, las ordenes " +
                                        "de medicamentos como esta, solo pueden ser renovadas de fecha mas no de items, por lo tanto si desea " +
                                        "cambiar un elemento de la orden de medicamentos debera crear una nueva, de lo contrario renueve esta orden " +
                                        "solo de fecha cambiando la opcion arriba.",
                                       "A tener en cuenta",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Exclamation);
                                    return;
                                }

                                if (textBox2.Text == "")
                                {
                                    MessageBox.Show("Esta renovando datos de la orden medica, por lo tanto el campo de texto no puede " +
                                        "estar vacio",
                                        "Corregir",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                                    return;
                                }
                                OM_Desc = textBox2.Text;
                            }
                            else
                            {
                                OM_Desc = getOM.OM_Desc.ToString();
                            }

                            CXN_OM OI = new CXN_OM
                            {
                                OM_Pac = getOM.OM_Pac,
                                OM_Ase = getOM.OM_Ase,
                                OM_Cia = getOM.OM_Cia,
                                OM_Prof = Comunes.Contenedor.UsuarioLogueado,
                                OM_Fecha = OM_Fecha,
                                OM_DX1 = getOM.OM_DX1,
                                OM_DX1T = getOM.OM_DX1T,
                                OM_DX2 = getOM.OM_DX2,
                                OM_DX2T = getOM.OM_DX2T,
                                OM_DX3 = getOM.OM_DX3,
                                OM_DX3T = getOM.OM_DX3T,
                                OM_Edad = getOM.OM_Edad,
                                OM_Genero = getOM.OM_Genero,
                                OM_Direccion = getOM.OM_Direccion,
                                OM_Tipo = T,
                                OM_Duracion = getOM.OM_Duracion,
                                OM_Cantidad = getOM.OM_Cantidad,
                                OM_Via = getOM.OM_Via,
                                OM_Medicamento = getOM.OM_Medicamento,
                                OM_Presentacion = getOM.OM_Presentacion,
                                OM_Detalle = getOM.OM_Detalle,
                                OM_Telefono = getOM.OM_Telefono,
                                OM_Num = Convert.ToInt32(NumOrden.Com_OM),
                                OM_Firma = getOM.OM_Firma,
                                OM_Desc = OM_Desc,
                                OM_TEspecialidad = getOM.OM_TEspecialidad
                            };

                            bool _createOrden = repoOrdenes.insertOM(OI);
                            if (_createOrden != true)
                            {
                                MessageBox.Show("Hubo un conflicto con esta orden y no se logro renovar, " +
                               "vuelva a intentar o genere una nueva",
                               "Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                                this.Dispose();
                                this.Close();
                            }
                            else
                            {
                                int Nuevo = Convert.ToInt32(NumOrden.Com_OM) + 1;

                                var NumORdenNuevo = repoCompañia.ConsecutivoActualiza(cia, "OM", Nuevo);
                                if (NumORdenNuevo == false)
                                {
                                    MessageBox.Show("No se logro actualizar el nuevo consecutivo de ordenes medicas " +
                                        "antes de continuar con otra orden nueva, por favor reporte este incidente al " +
                                        "administrador de sistema",
                                        "Error grave en consecutivos",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                }

                                Exportar(Convert.ToInt32(NumOrden.Com_OM));
                                this.Dispose();
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Hubo un conflicto con esta orden y no se logro renovar, " +
                             "vuelva a intentar o genere una nueva",
                             "Error",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
                            this.Dispose();
                            this.Close();
                        }
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
