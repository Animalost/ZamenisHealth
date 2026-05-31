using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Homologos2 : ConfigForm.BaseForm
    {
        private static readonly IHomologos repoHomologos = new MHomologos();

        private string Tipo_Homologo;
        private int Pos, Documento, Cia;

        public Homologos2(string _tipoHomologo, int _documento, int _posision, int _cia)
        {
            InitializeComponent();
            this.Documento = _documento;
            this.Tipo_Homologo = _tipoHomologo;
            this.Pos = _posision;
            this.Cia = _cia;

            btnZamenis1.ButtonClick += btnZamenis1_ButtonClick;

            btnZamenis1.captionBtn = "Guardar";
            btnZamenis1.tooltipBtn = "Grabar estos datos";
        }

        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar el numero de la factura electronica"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar el CUFE de la factura electronica"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar la resolucion de la factura electronica"); return; }

                if (Tipo_Homologo == "General") { Comprobar(); return; }
                if (Tipo_Homologo == "Recepcion") { Comprobar_Ven(); return; }
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

        private void Homologos2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Homologos";
            
            label2.Text = this.Documento.ToString();
        }

        private void Homologa_General()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (label2.Text == "" || this.Pos == 0) { MessageBox.Show("No hay factura elegida a homologar"); return; }
                if (textBox1.Text == "") { MessageBox.Show("Digite numero de Homologo"); return; }

                bool addGeneral = repoHomologos.AddHomologoGeneral(textBox1.Text, this.Pos, dateTimePicker1.Value, dateTimePicker2.Value, textBox2.Text, textBox3.Text);
                if (addGeneral != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error interno, no se logro asociar el homologo.  Intente mas tarde";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Homologo agregado correctamente";
                    MG.ShowDialog();

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
        void Comprobar()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool consultaGeneral = repoHomologos.ConsultarExistenciaGeneral(textBox1.Text, this.Cia);
                if (consultaGeneral == true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este numero de factura electronica ya existe, el numero no puede existir o seria duplicada";
                    MG.ShowDialog();
                }
                else
                {
                    Homologa_General();
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

        private void Homologa_Recepcion()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (label2.Text == "" || this.Pos == 0) { MessageBox.Show("No hay factura elegida a homologar"); return; }
                if (textBox1.Text == "") { MessageBox.Show("Digite numero de Homologo"); return; }

                bool addGeneral = repoHomologos.AddHomologoRecepcion(textBox1.Text, Convert.ToInt32(label2.Text), dateTimePicker1.Value, dateTimePicker2.Value, textBox2.Text, textBox3.Text);
                if (addGeneral != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error interno, no se logro asociar el homologo.  Intente mas tarde";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Homologo agregado correctamente";
                    MG.ShowDialog();

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
        private void Comprobar_Ven()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool consultaVentas = repoHomologos.ConsultarExistenciaVentas(textBox1.Text, this.Cia);
                if (consultaVentas == true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este numero de factura electronica ya existe, el numero no puede existir o seria duplicada";
                    MG.ShowDialog();
                }
                else
                {
                    Homologa_Recepcion();
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
