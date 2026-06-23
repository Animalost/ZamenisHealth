using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class CyT2 : Forma
    {
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private int CodAse;

        public CyT2(int codAse)
        {
            InitializeComponent();
            CodAse = codAse;
        }

        private void CyT2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Convenios y Tarifas";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += toolStripButton1_Click;

            ConfigForm.SoloNumeros(textBox1);

            if (CodAse > 0)
            {
                ConsultarAseguradora(CodAse);
            }
        }
        void ConsultarAseguradora(int CodAse)
        {
            try
            {
                var getdatosAse = repoAse.getInfoFromAsebyCode(Convert.ToInt32(CodAse));
                if (getdatosAse != null)
                {
                    textBox1.Enabled = false;
                    textBox1.Text = getdatosAse.Ase_Identificador.ToString();
                    textBox2.Text = getdatosAse.Ase_Descripcion;
                    textBox3.Text = getdatosAse.Ase_NitCia;
                    textBox4.Text = getdatosAse.Ase_Cod_Prest;
                    textBox5.Text = getdatosAse.Ase_Cod_Emp;
                    textBox6.Text = getdatosAse.Ase_Direccion;
                    textBox7.Text = getdatosAse.Ase_Telefono;
                    textBox8.Text = getdatosAse.Ase_Responable;
                    textBox9.Text = getdatosAse.Ase_DVNitCia;
                    textBox15.Text = getdatosAse.Ase_Email;

                    CodAse = getdatosAse.Ase_Identificador;

                    MessageBox.Show("Este identificador de aseguradora ya existe, se han cargado los datos para edicion",
                        "Existente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    textBox1.Enabled = false;
                    CodAse = 0;
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
        private void textBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                ConsultarAseguradora(Convert.ToInt32(textBox1.Text));
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
            if (CodAse > 0)
            {
                //Edita
                Ac_Ase();
            }
            else
            {
                //crea
                Crea_Ase();
            }
        }
        private void Ac_Ase()
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox6.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox7.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox15.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                CXN_ASEGURADORA A = new CXN_ASEGURADORA
                {
                    Ase_Descripcion = textBox2.Text,
                    Ase_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Ase_NitCia = textBox3.Text,
                    Ase_Cod_Prest = textBox4.Text,
                    Ase_Cod_Emp = textBox5.Text,
                    Ase_Direccion = textBox6.Text,
                    Ase_Responable = textBox8.Text,
                    Ase_Telefono = textBox7.Text,
                    Ase_DVNitCia = textBox9.Text,
                    Ase_Identificador = Convert.ToInt32(textBox1.Text),
                    Ase_Email = textBox15.Text
                };

                bool _updateAse = repoAse.updateAse(A);
                if (_updateAse != true)
                {
                    Comunes.MensajesGeneral MG1 = new Comunes.MensajesGeneral();
                    MG1.TipoImagen = 1000;
                    MG1.Mensaje = "No se ha logrado actualizar la aseguradora";
                    MG1.ShowDialog();
                    return;
                }

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Aseguradora Actualizada";
                MG.ShowDialog();

                CyT f1 = Application.OpenForms.OfType<CyT>().SingleOrDefault();
                f1.LoadAse();

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
        private void Crea_Ase()
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox6.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox7.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox15.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                CXN_ASEGURADORA A = new CXN_ASEGURADORA
                {
                    Ase_Identificador = Convert.ToInt32(textBox1.Text),
                    Ase_Descripcion = textBox2.Text,
                    Ase_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Ase_NitCia = textBox3.Text,
                    Ase_Cod_Prest = textBox4.Text,
                    Ase_Cod_Emp = textBox5.Text,
                    Ase_Direccion = textBox6.Text,
                    Ase_Responable = textBox8.Text,
                    Ase_Telefono = textBox7.Text,
                    Ase_DVNitCia = textBox9.Text,
                    Ase_Email = textBox15.Text
                };

                bool _createAse = repoAse.createAse(A);
                if (_createAse != true)
                {
                    Comunes.MensajesGeneral MG2 = new Comunes.MensajesGeneral();
                    MG2.TipoImagen = 1000;
                    MG2.Mensaje = "No se ha logrado crear la aseguradora";
                    MG2.ShowDialog();
                    return;
                }

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Aseguradora creada";
                MG.ShowDialog();

                CyT f1 = Application.OpenForms.OfType<CyT>().SingleOrDefault();
                f1.LoadAse();

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
    }
}
