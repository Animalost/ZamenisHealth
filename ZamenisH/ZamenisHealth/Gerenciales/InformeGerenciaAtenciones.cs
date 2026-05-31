using Domain.CXN;
using Domain.Informes;

using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Informes.Interfaces;
using Persistence.Informes.Methods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Gerenciales
{
    public partial class InformeGerenciaAtenciones : Forma
    {
        private static readonly IInformeGerencial repoGerencia = new MInformeGerencial();
        private static readonly IAseguradoras repoAseguradora = new MAseguradoras();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly ICompañia repoCia = new MCompañia();

        private MensajesGeneral MG;
        private int cia, ase;

        public InformeGerenciaAtenciones()
        {
            InitializeComponent();
        }
        private async void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            await GenerarInforme();
        }
        private void InformeGerenciaAtenciones_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Informe de Atenciones";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGenerar = new ToolStripButton();
                btnGenerar = createToolButton("Generar");
                MenuLateral.Items.Add(btnGenerar);
                btnGenerar.Click += btnZamenis1_ButtonClick;

                List <CXN_CIA> getCias = repoCia.getAllCompañias();
                if (getCias != null)
                {
                    foreach (CXN_CIA cia in getCias)
                    {
                        comboBox3.Items.Add(cia.Com_Nombre);
                    }

                    comboBox3.SelectedIndex = 0;
                }

                List<CXN_ASEGURADORA> getAses = repoAseguradora.getAseguradoras();
                if (getCias != null)
                {
                    foreach (CXN_ASEGURADORA asee in getAses)
                    {
                        comboBox5.Items.Add(asee.Ase_Descripcion);
                    }

                    comboBox5.SelectedIndex = 0;
                }

                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox4.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ase = repoAseguradora.getInfoFromAsebyName(comboBox5.Text).Ase_Identificador;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            cia = repoCia.getPrestadorbyName(comboBox3.Text).Com_Identificador;
        }
        async Task GenerarInforme()
        {
            try
            {
                if (comboBox1.SelectedIndex == 1) //Mes Facturado
                {
                    InformeGerencia I = new InformeGerencia
                    {
                        Aseguradora = ase,
                        Compañia = cia,
                        Año = Convert.ToInt32(comboBox4.Text),
                        Mes = comboBox2.Text
                    };

                    //OBTENER LISTA FE FACTURAS DEL MES
                    List<CXN_FACTURA> getLista = await repoGerencia.getDatosFactura(I);
                    if (getLista != null)
                    {
                        //OBTENER ENCABEZADOS
                        CXN_CIA getDataCia = repoCia.getPrestadorbyCode(cia);
                        string LogoCia = getDataCia.Com_Logo;
                        Byte[] bytesLogoCia = Convert.FromBase64String(LogoCia);

                        //QUIRAR DUPLICADOS SI LOS HAY DEPENDIEDO DEL HOMOLOGO
                        List<CXN_FACTURA> listaSinDuplicados = getLista
                                .GroupBy(x => x.Homologo) // Agrupar por Factura
                                .Select(g => g.First())  // Seleccionar el primer elemento de cada grupo
                                .ToList();

                        List<InformeGerencia_Imprime> prntInformTmp = new List<InformeGerencia_Imprime>();

                        //RECORRER LISTA DE FACTURAS
                        foreach (CXN_FACTURA i in listaSinDuplicados)
                        {
                            //OBTENER ID PACIENTE DE LA FACTURA
                            I.IdPaciente = i.Fac_Pac;
                            I.FacturaZamenis = i.Fac_Num_Fac;
                            I.TipoDoc = i.Fac_Tipo_Doc;

                            //BUSCAR LISTA DE FACTURAS DE ESE PACIENTE
                            List<CXN_FACTURA> getPacDataFac = await repoGerencia.getDatosFacturaXPaciente(I);
                            if (getPacDataFac != null)
                            {
                                //CONTAR CUANTAS FACTURAS TIENE EN TOTAL ESE PACIENTE
                                int TotalFacXPaciente = getPacDataFac.Count;

                                foreach (CXN_FACTURA i2 in getPacDataFac)
                                {
                                    //LLENAR LISTA PARA IMPRIMIR CON DATO DE CADA FACTURA DE CADA PACIENTE
                                    prntInformTmp.Add(new InformeGerencia_Imprime
                                    {
                                        IdPaciente = I.IdPaciente,
                                        Compañia = getDataCia.Com_Identificador,
                                        Aseguradora = I.Aseguradora,
                                        TipoDoc = I.TipoDoc,
                                        Paciente = i2.Fac_Observa,
                                        FechaFactura = Convert.ToDateTime(i2.Fac_Fecha),
                                        FechaFacturaDesde = Convert.ToDateTime(i2.Fac_Fecha_Des),
                                        FechaFacturaHasta = Convert.ToDateTime(i2.Fac_Fecha_Has),
                                        Identificacion = i2.Fac_Res,
                                        FacturaZamenis = i2.Fac_Num_Fac,
                                        FacturaElectronica = i2.Homologo,
                                        UsuarioGeneraFactura = i2.Fac_Usr_Graba,
                                        Patologia = i2.QRCufe,
                                        TotalFacturaPorPaciente = TotalFacXPaciente,
                                        Logo = bytesLogoCia,
                                        Empresa = getDataCia.Com_Nombre,
                                        MesGeneracion = comboBox2.Text + " - " + comboBox4.Text
                                    });
                                }
                            }
                        }

                        List<InformeGerencia_Imprime> prntInform = new List<InformeGerencia_Imprime>();
                        InformeGerencia_Imprime vT;
                        InformeGerencia_Imprime vT2;

                        // Inicializar ProgressBar
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = prntInformTmp.Count;
                        progressBar1.Value = 0;
                        progressBar1.Visible = true;

                        List<InformeGerencia_Imprime> listaFinalSinDuplicados = prntInformTmp
                                .GroupBy(x => x.FacturaElectronica) // Agrupar por Factura
                                .Select(g => g.First())  // Seleccionar el primer elemento de cada grupo
                                .ToList();

                        int cantFacGlobal = listaFinalSinDuplicados.Count;
                        int cantValGlobal = 0;

                        foreach (InformeGerencia_Imprime i3 in listaFinalSinDuplicados)
                        {
                            vT2 = new InformeGerencia_Imprime
                            {
                                IdPaciente = i3.IdPaciente,
                                Compañia = i3.Compañia,
                                Aseguradora = i3.Aseguradora,
                                FacturaZamenis = i3.FacturaZamenis,
                                TipoDoc = i3.TipoDoc,
                                Mes = comboBox2.Text,
                                Año = Convert.ToInt32(comboBox4.Text)
                            };

                            cantValGlobal = cantValGlobal + await repoGerencia.sumarValorXFactura(vT2);
                        }

                        foreach (InformeGerencia_Imprime fin in prntInformTmp)
                        {
                            progressBar1.Value++;

                            vT = new InformeGerencia_Imprime
                            {
                                IdPaciente = fin.IdPaciente,
                                Compañia = fin.Compañia,
                                Aseguradora = fin.Aseguradora,
                                FacturaZamenis = fin.FacturaZamenis,
                                TipoDoc = fin.TipoDoc,
                                Mes = comboBox2.Text,
                                Año = Convert.ToInt32(comboBox4.Text)
                            };

                            //OBTENER EL VALOR TOTAL DE CADA FACTURA DEL PACIENTE
                            int ValorXFactura = await repoGerencia.sumarValorXFactura(vT);
                            int ValorTotalFacturaxPac = await repoGerencia.sumarValorTotalFacturasXPac(vT);

                            prntInform.Add(new InformeGerencia_Imprime
                            {
                                Paciente = fin.Paciente,
                                FechaFactura = Convert.ToDateTime(fin.FechaFactura),
                                FechaFacturaDesde = Convert.ToDateTime(fin.FechaFacturaDesde),
                                FechaFacturaHasta = Convert.ToDateTime(fin.FechaFacturaHasta),
                                Identificacion = fin.Identificacion,
                                FacturaZamenis = fin.FacturaZamenis,
                                FacturaElectronica = fin.FacturaElectronica,
                                ValorFactura = ValorXFactura,
                                ValorTotalFacturacion = ValorTotalFacturaxPac,
                                UsuarioGeneraFactura = fin.UsuarioGeneraFactura,
                                Patologia = fin.Patologia,
                                TotalFacturaPorPaciente = fin.TotalFacturaPorPaciente,
                                Logo = bytesLogoCia,
                                Empresa = getDataCia.Com_Nombre,
                                MesGeneracion = comboBox2.Text + " - " + comboBox4.Text,
                                TotalFacturasGlobal = cantFacGlobal,
                                VrFacturasGlobal = cantValGlobal,
                                Mes = comboBox2.Text
                            });
                        }

                        //GENERAR INFORME
                        ConfigForm.GenerarReportViewer("DataSet_Gerencia",
              "ZamenisHealth.Reportes.RDLC_InformeGerencia.rdlc",
              prntInform);

                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "No hay datos con estos criterios de busqueda";
                        MG.ShowDialog();
                    }

                    pictureBox1.Visible = false;
                    progressBar1.Visible = false;
                }
                else if (comboBox1.SelectedIndex == 2) //Buscar por pacientes atendidos
                {
                    InformeGerencia I = new InformeGerencia
                    {
                        Aseguradora = ase,
                        Compañia = cia,
                        Año = Convert.ToInt32(comboBox4.Text),
                        Mes = comboBox2.Text
                    };

                    //OBTENER LISTA DE PACIENTES ATENDIDOS EN EL MES
                    List<int> getLista = await repoGerencia.getDatosAtendidosMES(I);
                    if (getLista != null)
                    {
                        //OBTENER ENCABEZADOS
                        CXN_CIA getDataCia = repoCia.getPrestadorbyCode(cia);
                        string LogoCia = getDataCia.Com_Logo;
                        Byte[] bytesLogoCia = Convert.FromBase64String(LogoCia);

                        List<InformeGerencia_Imprime> prntInformTmp = new List<InformeGerencia_Imprime>();

                        //RECORRER LISTA DE FACTURAS
                        foreach (int i in getLista)
                        {
                            //OBTENER ID PACIENTE DE LA FACTURA
                            I.IdPaciente = i;
                            
                            //BUSCAR LISTA DE FACTURAS DE ESE PACIENTE
                            List<CXN_FACTURA> getPacDataFac = await repoGerencia.getDatosFacturaXPaciente(I);
                            if (getPacDataFac != null)
                            {
                                //CONTAR CUANTAS FACTURAS TIENE EN TOTAL ESE PACIENTE
                                int TotalFacXPaciente = getPacDataFac.Count;

                                foreach (CXN_FACTURA i2 in getPacDataFac)
                                {
                                    //LLENAR LISTA PARA IMPRIMIR CON DATO DE CADA FACTURA DE CADA PACIENTE
                                    prntInformTmp.Add(new InformeGerencia_Imprime
                                    {
                                        IdPaciente = I.IdPaciente,
                                        Compañia = getDataCia.Com_Identificador,
                                        Aseguradora = I.Aseguradora,
                                        TipoDoc = i2.Fac_Tipo_Doc,
                                        Paciente = i2.Fac_Observa,
                                        FechaFactura = Convert.ToDateTime(i2.Fac_Fecha),
                                        FechaFacturaDesde = Convert.ToDateTime(i2.Fac_Fecha_Des),
                                        FechaFacturaHasta = Convert.ToDateTime(i2.Fac_Fecha_Has),
                                        Identificacion = i2.Fac_Res,
                                        FacturaZamenis = i2.Fac_Num_Fac,
                                        FacturaElectronica = i2.Homologo,
                                        UsuarioGeneraFactura = i2.Fac_Usr_Graba,
                                        Patologia = i2.QRCufe,
                                        TotalFacturaPorPaciente = TotalFacXPaciente,
                                        Logo = bytesLogoCia,
                                        Empresa = getDataCia.Com_Nombre,
                                        MesGeneracion = comboBox2.Text + " - " + comboBox4.Text
                                    });
                                }                                
                            }
                        }

                        // Inicializar ProgressBar
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = prntInformTmp.Count;
                        progressBar1.Value = 0;
                        progressBar1.Visible = true;

                        List<InformeGerencia_Imprime> prntInform = new List<InformeGerencia_Imprime>();                        

                        foreach (InformeGerencia_Imprime getVal in prntInformTmp)
                        {
                            progressBar1.Value++;

                            InformeGerencia_Imprime tmpgetVal = new InformeGerencia_Imprime
                            {
                                IdPaciente = getVal.IdPaciente,
                                Compañia = getVal.Compañia,
                                Aseguradora = getVal.Aseguradora,
                                Mes = comboBox2.Text,
                                Año = Convert.ToInt32(comboBox4.Text),
                                FacturaZamenis = getVal.FacturaZamenis,
                                TipoDoc = getVal.TipoDoc,
                            };

                            int ValorTotalFac = await repoGerencia.sumarValorTotalFacturasXPac(tmpgetVal);
                            int ValorIndFac = await repoGerencia.sumarValorXFactura(tmpgetVal);
                            BigInteger ValorGlobal = await repoGerencia.sumarValorGlobal(tmpgetVal);
                            
                            prntInform.Add(new InformeGerencia_Imprime {
                                IdPaciente = getVal.IdPaciente,
                                Compañia = getVal.Compañia,
                                Aseguradora = getVal.Aseguradora,
                                TipoDoc = getVal.TipoDoc,
                                Paciente = getVal.Paciente,
                                FechaFactura = Convert.ToDateTime(getVal.FechaFactura),
                                FechaFacturaDesde = Convert.ToDateTime(getVal.FechaFacturaDesde),
                                FechaFacturaHasta = Convert.ToDateTime(getVal.FechaFacturaHasta),
                                Identificacion = getVal.Identificacion,
                                FacturaZamenis = getVal.FacturaZamenis,
                                FacturaElectronica = getVal.FacturaElectronica,
                                UsuarioGeneraFactura = getVal.UsuarioGeneraFactura,
                                Patologia = getVal.Patologia,
                                TotalFacturaPorPaciente = getVal.TotalFacturaPorPaciente,
                                Logo = getVal.Logo,
                                Empresa = getVal.Empresa,
                                MesGeneracion = getVal.MesGeneracion,
                                ValorTotalFacturacion = ValorTotalFac,
                                TotalFacturasGlobal = prntInformTmp.Count,
                                ValorFactura = ValorIndFac,
                                VrFacturasGlobal = ValorGlobal
                            });                             
                        }

                        //GENERAR INFORME
                        ConfigForm.GenerarReportViewer("DataSet_Gerencia",
              "ZamenisHealth.Reportes.RDLC_InformeGerencia.rdlc",
              prntInform);

                    }
                    else
                    {
                        MG = new MensajesGeneral
                        {
                            TipoImagen = 3,
                            Mensaje = "No hay resultados con estos criterios de busqueda"
                        };

                        MG.ShowDialog();
                    }

                    pictureBox1.Visible = false;
                    progressBar1.Visible = false;
                }
                else if (comboBox1.SelectedIndex == 4) //Buscar por paciente idnum
                {
                    string CargoDigitado = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite documento del paciente",
                            "Informe por Paciente",
                                "");
                    if (CargoDigitado != "")
                    {
                        CXN_PACIENTES pac = repoPacientes.LlamarPacienteNumDoc(CargoDigitado);
                        if (pac == null)
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 3,
                                Mensaje = "No hay resultados para este documento digitado"
                            };

                            MG.ShowDialog();
                            pictureBox1.Visible = false;
                            progressBar1.Visible = false;
                            return;
                        }

                        InformeGerencia I = new InformeGerencia
                        {
                            Aseguradora = ase,
                            Compañia = cia,
                            Año = Convert.ToInt32(comboBox4.Text),
                            Mes = comboBox2.Text,
                            IdPaciente = pac.Pac_Id
                        };

                        if (I.IdPaciente == 0)
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 3,
                                Mensaje = "No hay resultados para este documento digitado"
                            };

                            MG.ShowDialog();
                            pictureBox1.Visible = false;
                            progressBar1.Visible = false;
                            return;
                        }

                        //OBTENER LISTA FE FACTURAS DEL PACIENTE
                        List<CXN_FACTURA> getLista = await repoGerencia.getDatosFacturaIndividual(I);
                        if (getLista != null)
                        {
                            //OBTENER ENCABEZADOS
                            CXN_CIA getDataCia = repoCia.getPrestadorbyCode(cia);
                            string LogoCia = getDataCia.Com_Logo;
                            Byte[] bytesLogoCia = Convert.FromBase64String(LogoCia);

                            //QUIRAR DUPLICADOS SI LOS HAY DEPENDIEDO DEL HOMOLOGO
                            List<CXN_FACTURA> listaSinDuplicados = getLista
                                    .GroupBy(x => x.Homologo) // Agrupar por Factura
                                    .Select(g => g.First())  // Seleccionar el primer elemento de cada grupo
                                    .ToList();

                            List<InformeGerencia_Imprime> prntInformTmp = new List<InformeGerencia_Imprime>();

                            //RECORRER LISTA DE FACTURAS
                            foreach (CXN_FACTURA i in listaSinDuplicados)
                            {
                                //OBTENER ID PACIENTE DE LA FACTURA
                                I.IdPaciente = i.Fac_Pac;
                                I.FacturaZamenis = i.Fac_Num_Fac;
                                I.TipoDoc = i.Fac_Tipo_Doc;

                                //CONTAR CUANTAS FACTURAS TIENE EN TOTAL ESE PACIENTE
                                int TotalFacXPaciente = listaSinDuplicados.Count;

                                //LLENAR LISTA PARA IMPRIMIR CON DATO DE CADA FACTURA DE CADA PACIENTE
                                prntInformTmp.Add(new InformeGerencia_Imprime
                                {
                                    IdPaciente = I.IdPaciente,
                                    Compañia = getDataCia.Com_Identificador,
                                    Aseguradora = I.Aseguradora,
                                    TipoDoc = I.TipoDoc,
                                    Paciente = pac.Pac_PrimerN + " " + pac.Pac_SegundoN + " " + pac.Pac_PrimerA + " " + pac.Pac_SegundoA,
                                    FechaFactura = Convert.ToDateTime(i.Fac_Fecha),
                                    FechaFacturaDesde = Convert.ToDateTime(i.Fac_Fecha_Des),
                                    FechaFacturaHasta = Convert.ToDateTime(i.Fac_Fecha_Has),
                                    Identificacion = pac.Pac_TipoId + " " + pac.Pac_IdNum,
                                    FacturaZamenis = i.Fac_Num_Fac,
                                    FacturaElectronica = i.Homologo,
                                    UsuarioGeneraFactura = i.Fac_Usr_Graba,
                                    Patologia = MInformeGerencial.getPatologia(I),
                                    TotalFacturaPorPaciente = TotalFacXPaciente,
                                    Logo = bytesLogoCia,
                                    Empresa = getDataCia.Com_Nombre,
                                    MesGeneracion = comboBox2.Text + " - " + comboBox4.Text
                                });
                            }

                            List<InformeGerencia_Imprime> prntInform = new List<InformeGerencia_Imprime>();
                            InformeGerencia_Imprime vT;
                            InformeGerencia_Imprime vT2;

                            // Inicializar ProgressBar
                            progressBar1.Minimum = 0;
                            progressBar1.Maximum = prntInformTmp.Count;
                            progressBar1.Value = 0;
                            progressBar1.Visible = true;

                            List<InformeGerencia_Imprime> listaFinalSinDuplicados = prntInformTmp
                                    .GroupBy(x => x.FacturaElectronica) // Agrupar por Factura
                                    .Select(g => g.First())  // Seleccionar el primer elemento de cada grupo
                                    .ToList();

                            int cantFacGlobal = listaFinalSinDuplicados.Count;
                            int cantValGlobal = 0;

                            foreach (InformeGerencia_Imprime i3 in listaFinalSinDuplicados)
                            {
                                vT2 = new InformeGerencia_Imprime
                                {
                                    IdPaciente = i3.IdPaciente,
                                    Compañia = i3.Compañia,
                                    Aseguradora = i3.Aseguradora,
                                    FacturaZamenis = i3.FacturaZamenis,
                                    TipoDoc = i3.TipoDoc,
                                    Mes = comboBox2.Text,
                                    Año = Convert.ToInt32(comboBox4.Text)
                                };

                                cantValGlobal = cantValGlobal + await repoGerencia.sumarValorXFactura(vT2);
                            }

                            foreach (InformeGerencia_Imprime fin in prntInformTmp)
                            {
                                progressBar1.Value++;

                                vT = new InformeGerencia_Imprime
                                {
                                    IdPaciente = fin.IdPaciente,
                                    Compañia = fin.Compañia,
                                    Aseguradora = fin.Aseguradora,
                                    FacturaZamenis = fin.FacturaZamenis,
                                    TipoDoc = fin.TipoDoc,
                                    Mes = comboBox2.Text,
                                    Año = Convert.ToInt32(comboBox4.Text)
                                };

                                //OBTENER EL VALOR TOTAL DE CADA FACTURA DEL PACIENTE
                                int ValorXFactura = await repoGerencia.sumarValorXFactura(vT);
                                int ValorTotalFacturaxPac = await repoGerencia.sumarValorTotalFacturasXPac(vT);

                                prntInform.Add(new InformeGerencia_Imprime
                                {
                                    Paciente = fin.Paciente,
                                    FechaFactura = Convert.ToDateTime(fin.FechaFactura),
                                    FechaFacturaDesde = Convert.ToDateTime(fin.FechaFacturaDesde),
                                    FechaFacturaHasta = Convert.ToDateTime(fin.FechaFacturaHasta),
                                    Identificacion = fin.Identificacion,
                                    FacturaZamenis = fin.FacturaZamenis,
                                    FacturaElectronica = fin.FacturaElectronica,
                                    ValorFactura = ValorXFactura,
                                    ValorTotalFacturacion = ValorTotalFacturaxPac,
                                    UsuarioGeneraFactura = fin.UsuarioGeneraFactura,
                                    Patologia = fin.Patologia,
                                    TotalFacturaPorPaciente = fin.TotalFacturaPorPaciente,
                                    Logo = bytesLogoCia,
                                    Empresa = getDataCia.Com_Nombre,
                                    MesGeneracion = comboBox2.Text + " - " + comboBox4.Text,
                                    TotalFacturasGlobal = cantFacGlobal,
                                    VrFacturasGlobal = cantValGlobal,
                                    Mes = comboBox2.Text
                                });
                            }

                            //GENERAR INFORME
                            ConfigForm.GenerarReportViewer("DataSet_Gerencia",
              "ZamenisHealth.Reportes.RDLC_InformeGerencia.rdlc",
              prntInform);

                        }

                        pictureBox1.Visible = false;
                        progressBar1.Visible = false;
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
