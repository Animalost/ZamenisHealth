using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class ConsAdmision : Forma
    {
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IAgendaC repoAgenda = new MAgendaC();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IFirmasDigitales fDigitales = new MFirmasDigitales();

        private string Observa, TipServ;
        private DateTime Limite;

        private MensajesGeneral MG;

        private ToolStripButton btnBuscar;
        public ToolStripButton btnDelete = new ToolStripButton();
        private ToolStripButton btnDeleteSign;

        public ConsAdmision()
        {
            InitializeComponent();
        }

        private void ConsAdmision_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Autorizaciones";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Logo_Zamenis_APPS;

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            btnDelete = new ToolStripButton();
            btnDelete = createToolButton("Eliminar Historia");
            MenuLateral.Items.Add(btnDelete);
            btnDelete.Click += toolStripButton2_Click;
            btnDelete.Visible = false;

            btnDeleteSign = new ToolStripButton();
            btnDeleteSign = createToolButton("Eliminar Firma");
            MenuLateral.Items.Add(btnDeleteSign);
            btnDeleteSign.Click += toolStripButton3_Click;
            btnDeleteSign.Visible = false;                 
        }  
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var getAdmition = repoAgenda.searchAdmition(Convert.ToInt32(textBox1.Text));
                if (getAdmition.text == "")
                {
                    richTextBox1.Text = "Esta admision no existe";
                    btnDelete.Enabled = false;
                    btnBuscar.Enabled = true;
                    textBox1.Enabled = true;

                    btnBuscar.Enabled = true;
                    btnDelete.Visible = false;
                    btnDeleteSign.Visible = false;
                }
                else
                {
                    this.Limite = Convert.ToDateTime(getAdmition.limite);
                    this.Observa = getAdmition.observa;
                    this.TipServ = getAdmition.tiposerv;

                    richTextBox1.Text = getAdmition.text;
                    btnDelete.Visible = true;

                    if (Preferencias.TabletaFirmas != "A")
                    {
                        btnDeleteSign.Visible = false;
                    }
                    else
                    {
                        btnDeleteSign.Visible = true;
                    }

                    btnBuscar.Enabled = false;
                    textBox1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        { 
            try
            {
                DialogResult result = MessageBox.Show("¿Desea elimnar la firma digital de esta admision?",
                                                 "Zamenis Health - Eliminar Firma Digital",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    var getCargo = repoCargos.GetCargosFHIR(Convert.ToInt32(textBox1.Text));
                    if (getCargo != null) 
                    { 
                        if (getCargo.Car_Estado != "G")
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Esta admision ya esta facturada o no existe, no es posible eliminar su firma digital",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                        }
                        else
                        {
                            bool del = fDigitales.EliminarFirma(Convert.ToInt32(textBox1.Text));
                            if (del == true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Firma digital eliminada exitosamente",
                                    TipoImagen = 3
                                };

                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "No se logro eliminar la firma",
                                    TipoImagen = 1000
                                };

                                MG.ShowDialog();
                            }                          
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Esta admision no existe",
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
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea elimnar esta historia? Al realizar esta accion no se podra deshacer, este seguro realmente " +
                    "si es nesesario elimnar la historia clinica, este proceso solo se puede hacer en un alrededor de 8 dias calendario",
                                                 "Zamenis Health - Correccion de Historias",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    DateTime Hoy = DateTime.Now;
                    DateTime fecharegistro = DateTime.Parse(Limite.ToString());
                    var horas = (DateTime.Now - fecharegistro).TotalDays;

                    if (Convert.ToDouble(horas) >= 9)
                    {
                        MessageBox.Show("No es posible eliminar historias de mas de 8 dias de haberse generado, para este caso " +
                            "genere una nueva historia clinica",
                            "Acceso Denegado!!! -> " + horas.ToString() + " dias",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    repoCargos.deleteHistoria(Convert.ToInt32(textBox1.Text));

                    Observa = Observa + " || ESTA HISTORIA CLINICA HA SIDO ELIMINADA DEL SISTEMA POR EL USUARIO " + Comunes.Contenedor.UsuarioLogueado +
                            " el dia " + Convert.ToDateTime(Hoy).ToString(Conexion.ConectionDictionary["Format_Fecha"] + " hh:mm:ss");

                    repoAgendaMedica.addObservation(Convert.ToInt32(textBox1.Text), Observa);

                    repoAgendaMedica.deleteHistoria(Convert.ToInt32(textBox1.Text), this.TipServ);

                    MessageBox.Show("Eliminado del sistema, se ha grabado el registro de eliminacion con fecha y hora asociado " +
                       "a su usuario y documento de identificacion",
                       "Eliminado",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }        
    }
}
