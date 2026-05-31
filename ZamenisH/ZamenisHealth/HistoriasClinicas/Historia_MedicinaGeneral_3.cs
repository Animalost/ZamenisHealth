using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_MedicinaGeneral_3 : ConfigForm.BaseForm
    {
        private static readonly ICargos repositorioCargos = new MCargos();
        private static readonly IInventario repositorioInventario = new MInventario();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IFacturacion repositorioFactura = new MFacturacion();

        private int PacienteId, valser, Cia;

        public Historia_MedicinaGeneral_3(int _pacienteId, int _cia)
        {
            InitializeComponent();
            this.PacienteId = _pacienteId;
            this.Cia = _cia;

            
            ConfigForm.MoverForma(label1, this);
        }
    
        private void Historia_MedicinaGeneral_3_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                listView1.Clear();
                listView1.View = View.Details;
                listView1.GridLines = true;
                listView1.FullRowSelect = true;
                listView1.Columns.Add("Codigo", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("Item", 250, HorizontalAlignment.Left);
                listView1.Columns.Add("Cantidad", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("Vr.Unitario", 100, HorizontalAlignment.Left);
                listView1.Columns.Add("Vr.Total", 100, HorizontalAlignment.Left);
                listView1.Columns.Add("Total", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("Unitario", 0, HorizontalAlignment.Left);

                ListaCarga CargaPlant = repositorioCargos.Carga_Plantilla();
                if (CargaPlant != null)
                {
                    Dictionary<string, string> D = new Dictionary<string, string>();
                    
                    if (CargaPlant.Cod1 != "" && CargaPlant.Cod1 != null) 
                    { 
                        if (CargaPlant.Can1 != "" && CargaPlant.Can1 != null)
                        {
                            D.Add(CargaPlant.Cod1, CargaPlant.Can1);
                        }
                    }
                    if (CargaPlant.Cod2 != "" && CargaPlant.Cod2 != null)
                    {
                        if (CargaPlant.Can2 != "" && CargaPlant.Can2 != null)
                        {
                            D.Add(CargaPlant.Cod2, CargaPlant.Can2);
                        }
                    }
                    if (CargaPlant.Cod3 != "" && CargaPlant.Cod3 != null)
                    {
                        if (CargaPlant.Can3 != "" && CargaPlant.Can3 != null)
                        {
                            D.Add(CargaPlant.Cod3, CargaPlant.Can3);
                        }
                    }
                    if (CargaPlant.Cod4 != "" && CargaPlant.Cod4 != null)
                    {
                        if (CargaPlant.Can4 != "" && CargaPlant.Can4 != null)
                        {
                            D.Add(CargaPlant.Cod4, CargaPlant.Can4);
                        }
                    }
                    if (CargaPlant.Cod5 != "" && CargaPlant.Cod5 != null)
                    {
                        if (CargaPlant.Can5 != "" && CargaPlant.Can5 != null)
                        {
                            D.Add(CargaPlant.Cod5, CargaPlant.Can5);
                        }
                    }
                    if (CargaPlant.Cod6 != "" && CargaPlant.Cod6 != null)
                    {
                        if (CargaPlant.Can6 != "" && CargaPlant.Can6 != null)
                        {
                            D.Add(CargaPlant.Cod6, CargaPlant.Can6);
                        }
                    }
                    if (CargaPlant.Cod7 != "" && CargaPlant.Cod7 != null)
                    {
                        if (CargaPlant.Can7 != "" && CargaPlant.Can7 != null)
                        {
                            D.Add(CargaPlant.Cod7, CargaPlant.Can7);
                        }
                    }
                    if (CargaPlant.Cod8 != "" && CargaPlant.Cod8 != null)
                    {
                        if (CargaPlant.Can8 != "" && CargaPlant.Can8 != null)
                        {
                            D.Add(CargaPlant.Cod8, CargaPlant.Can8);
                        }
                    }
                    if (CargaPlant.Cod9 != "" && CargaPlant.Cod9 != null)
                    {
                        if (CargaPlant.Can9 != "" && CargaPlant.Can9 != null)
                        {
                            D.Add(CargaPlant.Cod9, CargaPlant.Can9);
                        }
                    }
                    if (CargaPlant.Cod10 != "" && CargaPlant.Cod10 != null)
                    {
                        if (CargaPlant.Can10 != "" && CargaPlant.Can10 != null)
                        {
                            D.Add(CargaPlant.Cod10, CargaPlant.Can10);
                        }
                    }
                    if (CargaPlant.Cod11 != "" && CargaPlant.Cod11 != null)
                    {
                        if (CargaPlant.Can11 != "" && CargaPlant.Can11 != null)
                        {
                            D.Add(CargaPlant.Cod11, CargaPlant.Can11);
                        }
                    }
                    if (CargaPlant.Cod12 != "" && CargaPlant.Cod12 != null)
                    {
                        if (CargaPlant.Can12 != "" && CargaPlant.Can12 != null)
                        {
                            D.Add(CargaPlant.Cod12, CargaPlant.Can12);
                        }
                    }
                    if (CargaPlant.Cod13 != "" && CargaPlant.Cod13 != null)
                    {
                        if (CargaPlant.Can13 != "" && CargaPlant.Can13 != null)
                        {
                            D.Add(CargaPlant.Cod13, CargaPlant.Can13);
                        }
                    }
                    if (CargaPlant.Cod14 != "" && CargaPlant.Cod14 != null)
                    {
                        if (CargaPlant.Can14 != "" && CargaPlant.Can14 != null)
                        {
                            D.Add(CargaPlant.Cod14, CargaPlant.Can14);
                        }
                    }
                    if (CargaPlant.Cod15 != "" && CargaPlant.Cod15 != null)
                    {
                        if (CargaPlant.Can15 != "" && CargaPlant.Can15 != null)
                        {
                            D.Add(CargaPlant.Cod15, CargaPlant.Can15);
                        }
                    }
                    if (CargaPlant.Cod16 != "" && CargaPlant.Cod16 != null)
                    {
                        if (CargaPlant.Can16 != "" && CargaPlant.Can16 != null)
                        {
                            D.Add(CargaPlant.Cod16, CargaPlant.Can16);
                        }
                    }
                    if (CargaPlant.Cod17 != "" && CargaPlant.Cod17 != null)
                    {
                        if (CargaPlant.Can17 != "" && CargaPlant.Can17 != null)
                        {
                            D.Add(CargaPlant.Cod17, CargaPlant.Can17);
                        }
                    }
                    if (CargaPlant.Cod18 != "" && CargaPlant.Cod18 != null)
                    {
                        if (CargaPlant.Can18 != "" && CargaPlant.Can18 != null)
                        {
                            D.Add(CargaPlant.Cod18, CargaPlant.Can18);
                        }
                    }

                    foreach (KeyValuePair<string, string> i in D)
                    {
                        if (i.Key != "" || i.Key != null)
                        {
                            (int valor, string item, string detalle) Dato = repositorioInventario.ConsultarValor(99, i.Key);

                            listView1.Items.Add(new ListViewItem(new string[] {
                                i.Key,
                                Dato.item,
                                i.Value.ToString(),
                                "$ " + Dato.valor.ToString("N0"),
                                "$ " + (Convert.ToInt32(i.Value) * Convert.ToInt32(Dato.valor)).ToString("N0"),
                                (Convert.ToInt32(i.Value) * Convert.ToInt32(Dato.valor)).ToString(),
                                Convert.ToInt32(i.Value).ToString()
                            }));
                        }
                    }
                }

                List<CXN_INVENTARIO> getLista = repositorioInventario.getAllProducts(99);
                if (getLista != null)
                {
                    foreach (CXN_INVENTARIO i in getLista)
                    {
                        listBox1.Items.Add(i.InvItem);
                    }                    
                }

                CXN_PACIENTES namePac = repositorioPacientes.LlamarPacientebyId(this.PacienteId);
                label2.Text = namePac.Pac_PrimerA + " " + namePac.Pac_SegundoA + " " + namePac.Pac_PrimerN + " " + namePac.Pac_SegundoN;

                CXN_CONVENIOS getCur = repositorioConvenios.ServicioNombre("869500", 99, "CU");
                if (getCur != null) 
                {
                    label7.Text = getCur.Con_Valor.ToString("N0");
                    valser = getCur.Con_Valor;
                }
                else
                {
                    label7.Text = "0";
                    valser = getCur.Con_Valor = 0;
                }

                Sumar();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                CXN_INVENTARIO getDato = new CXN_INVENTARIO();
                getDato = repositorioInventario.ConsultarValor2(listBox1.SelectedItem.ToString().Trim(), 99);
                if (getDato != null)
                {
                    Historia_MedicinaGeneral_4 H = new Historia_MedicinaGeneral_4(getDato);
                    H.ShowDialog();
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Hubo un error inesperado";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public void AddItem(CXN_INVENTARIO I, int cant)
        {
            try
            {
                listView1.Items.Add(new ListViewItem(new string[]
                {
                    I.InvCod,
                    I.InvItem,
                    cant.ToString(),
                    "$ " + I.InvPrecio.ToString("N0"),
                    "$ " + (Convert.ToInt32(I.InvPrecio) * Convert.ToInt32(cant)).ToString("N0"),
                    (Convert.ToInt32(I.InvPrecio) * Convert.ToInt32(cant)).ToString()
                }));

                Sumar();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                listView1.SelectedItems[0].Remove();
                Sumar();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG;

                DialogResult result = MessageBox.Show("Desea imprimir esta cotizacion?",
                                                  "Zamenis Health - Cotizaciones",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var Doc = repositorioCompañias.getPrestadorbyCode(this.Cia);
                    if (Doc == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Error en consecutivos de facturacion";
                        MG.ShowDialog();                    
                        return;
                    }

                    if (Doc.Com_Cotiza == 0)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Error en consecutivos de facturacion";
                        MG.ShowDialog();                       
                        return;
                    }

                    CXN_CONVENIOS getCur = repositorioConvenios.ServicioNombre("869500", 99, "CU");
                    if (getCur != null)
                    {
                        CXN_CARGOS S1 = new CXN_CARGOS();
                        S1.Car_Cant = Convert.ToInt32(1);
                        S1.Car_Val_Tot = Convert.ToInt32(getCur.Con_Valor);
                        S1.Car_Pac = Convert.ToInt32(this.PacienteId);
                        S1.Car_Cia = Convert.ToInt32(this.Cia);
                        S1.Car_Ase = Convert.ToInt32(99);
                        S1.Car_Cod = getCur.Con_Id_Serv;
                        S1.Car_Item = getCur.Con_Nombre;
                        S1.Car_Factura = Doc.Com_Cotiza.ToString();
                        S1.Car_Estado = "C";
                        S1.Car_Tipo = "Cotizacion";
                        S1.Car_Tipo_Doc = "CO";
                        S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                        string resS1 = repositorioCargos.insertService(S1);
                        if (resS1 != "OK")
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Inconveniente con este servicio " + S1.Car_Item + " seleccionado";
                            MG.ShowDialog();
                            return;
                        }
                    }

                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        CXN_CARGOS S1 = new CXN_CARGOS();
                        S1.Car_Cant = Convert.ToInt32(listView1.Items[i].SubItems[2].Text);
                        S1.Car_Val_Tot = Convert.ToInt32(listView1.Items[i].SubItems[5].Text);
                        S1.Car_Pac = Convert.ToInt32(this.PacienteId);
                        S1.Car_Cia = Convert.ToInt32(this.Cia);
                        S1.Car_Ase = Convert.ToInt32(99);
                        S1.Car_Cod = listView1.Items[i].SubItems[0].Text;
                        S1.Car_Item = listView1.Items[i].SubItems[1].Text;
                        S1.Car_Factura = Doc.Com_Cotiza.ToString();
                        S1.Car_Estado = "C";
                        S1.Car_Tipo = "Cotizacion";
                        S1.Car_Tipo_Doc = "CO";
                        S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                        string resS1 = repositorioCargos.insertService(S1);
                        if (resS1 != "OK")
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Inconveniente con este servicio " + S1.Car_Item + " seleccionado";
                            MG.ShowDialog();
                            return;
                        }
                    }

                    CXN_FACTURA F = new CXN_FACTURA();
                    F.Fac_Tipo_Doc = "CO";
                    F.Fac_Num_Fac = Convert.ToInt32(Doc.Com_Cotiza);
                    F.Fac_Cia = this.Cia;
                    F.Fac_Pac = this.PacienteId;
                    F.Fac_Res = "N/A";
                    F.Homologo = Doc.Com_Cotiza.ToString();
                    F.Fac_Ase = 99;
                    F.Fac_Observa = "COTIZACION DE SERVICIOS";
                    F.Fac_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;
                    F.Fac_Estado = "C";

                    string _insertDoc = repositorioFactura.insertarDocumento(F);
                    if (_insertDoc != "OK")
                    {
                        MessageBox.Show("Se agrego todo el repertorio de servicios pero no se logro grabar el documento como cotizacion.  " +
                            "Reporte inmediatamente al administrador de sistema antes de continuar con la siguiente cotizacion.",
                            "Error Grave",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    int Nuevo = Convert.ToInt32(Doc.Com_Cotiza) + 1;
                    var ACCON = repositorioCompañias.ConsecutivoActualiza(this.Cia, "COTIZA", Nuevo);
                    if (ACCON != true)
                    {
                        MessageBox.Show("Se genero su documento tipo cotizacion pero no se logro actualizar el consecutivo siguiente, " +
                            "este es un error grave, debe reportar inmediato al administrador del sistema antes de crear otra cotizacion nueva",
                            "ADVERTENCIA!!!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    MessageBox.Show("Hecho, numero de documento: " + Doc.Com_Cotiza,
                                        "Facturado Exitoso",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);

                    DialogResult result2 = MessageBox.Show("¿Desea Imprimir el Documento?", "Zamenis Health - Cotizaciones", MessageBoxButtons.YesNo);
                    if (result2 == DialogResult.Yes)
                    {
                        var Exporta = repositorioCargos.GenerarDocumento(Convert.ToInt32(Doc.Com_Cotiza), this.Cia);
                        if (Exporta == null)
                        {
                            MessageBox.Show("No se logro exportar el documento, por favor ingrese por la opcion de copias",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            this.Dispose();
                            this.Close();
                        }

                        ConfigForm.GenerarReportViewer("DataSet_Cotizacion",
               "ZamenisHealth.Reportes.RDLC_Cotizacion.rdlc",
               Exporta);

                        this.Dispose();
                        this.Close();
                    }
                }               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Sumar()
        {
            try
            {
                int suma = 0;
             
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].SubItems[5].Text != "")
                        suma += Convert.ToInt32(listView1.Items[i].SubItems[5].Text);
                }                

                label8.Text = "$ " + (valser + suma).ToString("N0");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
