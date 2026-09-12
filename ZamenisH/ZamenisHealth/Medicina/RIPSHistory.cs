using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class RIPSHistory : Forma2
    {
        private static readonly IRIPS repoRips = new MRIPS();
        private static readonly IRIPS_Res2275_2023 repoRIp2 = new MRIPS_Res2275_2023();
        public int Adm_Cargo;

        public RIPSHistory()
        {
            InitializeComponent();
        }

        private void RIPSHistory_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Resolucion 2275 de 2023";

                ToolTip toolTip1 = new ToolTip();
                ToolTip toolTip12 = new ToolTip();
                ToolTip toolTip13 = new ToolTip();

                toolTip1.ShowAlways = true;
                toolTip12.ShowAlways = true;
                toolTip13.ShowAlways = true;

                toolTip1.SetToolTip(comboBox1, "El ambito quiere decir el tipo de atencion que se brindo al paciente");
                toolTip12.SetToolTip(comboBox2, "Esta finalidad es la clase de servicio que se presto en la atencion");
                toolTip13.SetToolTip(comboBox3, "Seleccione su tipo de profesion, si no se encuentra en la lista, seleccione otro");

                List<string> getListaTecnoSalud = repoRIp2.getTecSalud();
                if (getListaTecnoSalud != null)
                {
                    foreach (string c in getListaTecnoSalud)
                    {
                        comboBox7.Items.Add(c);
                    }
                }

                List<string> getListaTecnoSaludCEXTERNA = repoRIp2.getTecSaludCEXTERNA();
                if (getListaTecnoSaludCEXTERNA != null)
                {
                    foreach (string c in getListaTecnoSaludCEXTERNA)
                    {
                        comboBox4.Items.Add(c);
                    }
                }

                ImageClose.Visible = false;
                ImageMinimize.Visible = false;
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
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "" || comboBox5.Text == "" || comboBox6.Text == "" || comboBox7.Text == "" || comboBox4.Text == "" || comboBox8.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar una opcion de cada lista desplegable";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                DialogResult result = MessageBox.Show("Una vez guardada esta informacion, no se podran deshacer cambios. ¿Realmente desea guardar?",
                                                "Zamenis Health - RIPS",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int Ambito = repoRips.TipoRipCargo(comboBox1.Text, "Ambito");
                    int Personal = repoRips.TipoRipCargo(comboBox3.Text, "Personal");
                    string causaexterna = repoRIp2.getCodeTecnoSaludCExterna(comboBox4.Text);
                    int Finalidad = repoRips.TipoRipCargo(comboBox2.Text, "Finalidad");
                    int Motivo = repoRips.TipoRipCargo(comboBox6.Text, "Motivo");
                    int IMPDX = repoRips.TipoRipCargo(comboBox5.Text, "IMPDX");
                    int CODEEGRESO = repoRips.TipoRipCargo(comboBox8.Text, "CODEEGRESO");

                    repoRips.updateCargo(Ambito, Personal, Convert.ToInt32(causaexterna), Finalidad, Motivo, IMPDX, Adm_Cargo, CODEEGRESO);
                    repoRIp2.updateTecnoSalud(Adm_Cargo, repoRIp2.getCodeTecnoSalud(comboBox7.Text));                    

                    MG.Mensaje = "Registro RIPS grabado con exito";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
