using Domain;
using Domain.CXN;
using FormAndControls;
using Newtonsoft.Json;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class RIPS_IND : Forma
    {
        private readonly static IAseguradoras repositorioAseguradoras = new MAseguradoras();
        private readonly static ICompañia repositorioCompañias = new MCompañia();
        private readonly static IRIPS repositorioRIPS = new MRIPS();
        private readonly static IPacientes repoPacientes = new MPacientes();
        private readonly static IRIPSJSON repoRipsJson = new MRipsJSON();

        private Comunes.MensajesGeneral MG;

        public RIPS_IND()
        {
            InitializeComponent();
        }

        //json
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (comboBox6.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Seleccione tipo de rips",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return;
                }

                CXN_CIA datCom = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                CXN_ASEGURADORA datAse = repositorioAseguradoras.getInfoFromAsebyName(comboBox1.Text);
                string TD = "";

                switch (comboBox4.Text)
                {
                    case "Ordenes de Pedido":
                        TD = "OP";
                        break;

                    case "Facturas":
                        TD = "FA";
                        break;

                    case "Documentos Equivalentes":
                        TD = "DE";
                        break;

                    default:
                        Comunes.MensajesGeneral MG2 = new Comunes.MensajesGeneral();
                        MG2.Mensaje = "Tipo de documento seleccionado invalido";
                        MG2.TipoImagen = 1000;
                        MG2.ShowDialog();
                        return;
                }

                RIPS_Class R = new RIPS_Class
                {
                    Numfac = textBox1.Text,
                    aseRIPS = datAse.Ase_Identificador,
                    ciaRIPS = datCom.Com_Identificador,
                    claseRIPS = comboBox3.Text,
                    regimenRIPS = comboBox5.Text,
                    tDocumentRIPS = TD,
                    TipoInd = comboBox6.Text
                };

                Transaccion T = repoRipsJson.GenrateIndividual(R);
                if (T == null)
                {
                    Comunes.MensajesGeneral MG3 = new Comunes.MensajesGeneral();
                    MG3.Mensaje = "No hay resultados";
                    MG3.TipoImagen = 3;
                    MG3.ShowDialog();
                }
                else
                {
                    string json = JsonConvert.SerializeObject(T, Formatting.Indented);

                    //Verifica vacios
                    string jsonLimpio = JsonVerifications.JsonLimpio(json);
                    //fin verifica vacios

                    string replaceFac = R.Numfac.Replace("'", "");
                    
                    System.IO.File.WriteAllText(@"C:\CXN\RIPS\" + replaceFac + ".json", jsonLimpio);

                    Comunes.MensajesGeneral MG4 = new Comunes.MensajesGeneral();
                    MG4.Mensaje = "Generado en C CXN RIPS";
                    MG4.TipoImagen = 3;
                    MG4.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        //txt
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA datCom = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                CXN_ASEGURADORA datAse = repositorioAseguradoras.getInfoFromAsebyName(comboBox1.Text);
                string TD = "";

                switch (comboBox4.Text)
                {
                    case "Ordenes de Pedido":
                        TD = "OP";
                        break;

                    case "Facturas":
                        TD = "FA";
                        break;

                    case "Documentos Equivalentes":
                        TD = "DE";
                        break;

                    default:
                        Comunes.MensajesGeneral MG2 = new Comunes.MensajesGeneral();
                        MG2.Mensaje = "Tipo de documento seleccionado invalido";
                        MG2.TipoImagen = 1000;
                        MG2.ShowDialog();
                        return;
                }

                RIPS_Class R = new RIPS_Class
                {
                    Numfac = textBox1.Text,
                    aseRIPS = datAse.Ase_Identificador,
                    ciaRIPS = datCom.Com_Identificador,
                    claseRIPS = comboBox3.Text,
                    regimenRIPS = comboBox5.Text,
                    tDocumentRIPS = TD
                };

                repositorioRIPS.setDatos(R.aseRIPS, R.ciaRIPS, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, R.tDocumentRIPS, R.regimenRIPS, R.Numfac);

                Comunes.MensajesGeneral MG4 = new Comunes.MensajesGeneral();
                MG4.Mensaje = "Generado en C CXN RIPS";
                MG4.TipoImagen = 3;
                MG4.ShowDialog();

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void RIPS_IND_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "RIPS Individuales";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btn3374 = new ToolStripButton();
                btn3374 = createToolButton("3374 de 2000");
                MenuLateral.Items.Add(btn3374);
                btn3374.Click += btnZamenis1_ButtonClick;

                ToolStripButton btn2275 = new ToolStripButton();
                btn2275 = createToolButton("Json");
                MenuLateral.Items.Add(btn2275);
                btn2275.Click += btnZamenis2_ButtonClick;

                var Asegura = repositorioAseguradoras.getAseguradoras();
                var Compa = repositorioCompañias.getAllCompañias();

                if (Asegura != null)
                {
                    foreach (var i in Asegura)
                    {
                        comboBox1.Items.Add(i.Ase_Descripcion);
                    }
                }

                if (Compa != null)
                {
                    foreach (var i in Compa)
                    {
                        comboBox2.Items.Add(i.Com_Nombre);
                    }
                }

                List<string> Regi = repoPacientes.ListaRegimen();
                if (Regi != null)
                {
                    foreach (var r in Regi)
                    {
                        comboBox5.Items.Add(r);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void panel3_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ConfigForm.ReleaseCapturing();
            ConfigForm.SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
