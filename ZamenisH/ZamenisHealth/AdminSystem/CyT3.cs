using Domain;
using Domain.CXN;
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
    public partial class CyT3 : Forma
    {
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly IBodegas repoBodegas = new MBodegas();
        private int Posision, Ase;

        public CyT3(int posision)
        {
            InitializeComponent();
            Posision = posision;
        }

        private void CyT3_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Adherencia";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += toolStripButton1_Click;

            CargarAse();
            CargarTipos();

            if (Posision > 0)
            {
                ConsultarConveio();
            }

            ConfigForm.SoloNumeros(textBox10);
        }
        void CargarTipos()
        {
            var getTiposBodega = repoBodegas.getTipos();
            if (getTiposBodega != null)
            {
                foreach (var i in getTiposBodega)
                {
                    comboBox1.Items.Add(i);
                }

                comboBox1.SelectedIndex = 0;
            }
        }
        void CargarAse()
        {
            var getAses = repoAse.getAseguradoras();
            if (getAses != null)
            {
                foreach (var i in getAses)
                {
                    comboBox2.Items.Add(i.Ase_Descripcion);
                }

                comboBox2.SelectedIndex = 0;
            }
        }
        void ConsultarConveio()
        {
            try
            {
                var getConvenio = repoConvenios.getConvenio(Posision);
                if (getConvenio != null)
                {
                    textBox12.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox1.Enabled = false;

                    textBox11.Text = getConvenio.Con_Nombre;
                    textBox12.Text = getConvenio.Con_Id_Serv;
                    textBox13.Text = getConvenio.Con_CUP;
                    textBox14.Text = getConvenio.Con_CodServicio;
                    textBox10.Text = Convert.ToInt32(getConvenio.Con_Valor).ToString(); ;
                    comboBox1.Text = getConvenio.Con_Tipo_Serv;
                    comboBox2.Text = getConvenio.Con_Clase;
                }
                else
                {
                    comboBox2.Enabled = false;
                    comboBox1.Enabled = true;

                    textBox11.Text = "";
                    textBox10.Text = "";
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
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Posision > 0)
                {
                    //edita
                    Ac_Con();
                }
                else
                {
                    //crear
                    Gra_Con();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Gra_Con()
        {
            try
            {
                if (textBox12.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox13.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox14.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (Ase == 0) { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                CXN_CONVENIOS C = new CXN_CONVENIOS
                {
                    Con_Id_Serv = textBox12.Text,
                    Con_Nombre = textBox11.Text,
                    Con_Tipo_Serv = comboBox1.Text,
                    Con_Valor = Convert.ToInt32(textBox10.Text),
                    Con_Aseguradora = Ase.ToString(),
                    Con_CUP = textBox13.Text,
                    Con_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Con_CodServicio = textBox14.Text
                };

                bool _createCon = repoConvenios.createConvenio(C);
                if (_createCon != true)
                {
                    Comunes.MensajesGeneral MG2 = new Comunes.MensajesGeneral();
                    MG2.TipoImagen = 1000;
                    MG2.Mensaje = "No se ha logrado crear el convenio";
                    MG2.ShowDialog();
                    return;
                }

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Convenio creado";
                MG.ShowDialog();

                this.Dispose();
                this.Close();
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
        private void Ac_Con()
        {
            try
            {
                if (textBox12.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox13.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox14.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (Ase == 0) { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                CXN_CONVENIOS C = new CXN_CONVENIOS
                {
                    Con_Nombre = textBox11.Text,
                    Con_Valor = Convert.ToInt32(textBox10.Text),
                    Con_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Con_Id_Serv = textBox12.Text,
                    Con_Aseguradora = Ase.ToString(),
                    Con_CUP = textBox13.Text,
                    Con_CodServicio = textBox14.Text
                };

                bool _createCon = repoConvenios.updateConvenio(C);
                if (_createCon != true)
                {
                    Comunes.MensajesGeneral MG2 = new Comunes.MensajesGeneral();
                    MG2.TipoImagen = 1000;
                    MG2.Mensaje = "No se ha logrado actualizar el convenio";
                    MG2.ShowDialog();
                    return;
                }

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Convenio actualizado";
                MG.ShowDialog();

                this.Dispose();
                this.Close();
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
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Cod = repoAse.getInfoFromAsebyName(comboBox2.Text);
            Ase = Cod.Ase_Identificador;
        }
    }
}
