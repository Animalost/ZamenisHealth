using System;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Estadistica : Forma2
    {
        private static readonly INotasCuracion repoNotasCuracion = new MNotasCuracion();
        private static readonly IAgendaC repoAgendaC = new MAgendaC();
        private static readonly IMenu repoMenu = new MMenu();
        public int Admition;
        
        public Estadistica()
        {
            InitializeComponent();    
        }

        private void Estadistica_Load(object sender, EventArgs e)
        {
            try
            {
                int dataPac = repoAgendaC.getIdPacByAdmition(Admition, "H");
                CXN_NOTAS N = repoNotasCuracion.getLastNota(dataPac);
                label11.Text = N.Not_Edad;

                Titulo.Text = "Estadistica de Enfermeria";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                ImageMinimize.Visible = false;
                ImageClose.Visible = false;

                textBox2.MaxLength = 99;

                var _repoMenu = repoMenu.getMenus();
                if (_repoMenu != null)
                {
                    foreach (var i1 in _repoMenu)
                    {
                        if (!string.IsNullOrEmpty(i1.Rango_Edad))
                        {
                            comboBox1.Items.Add(i1.Rango_Edad);
                        }                        
                    }

                    foreach (var i2 in _repoMenu)
                    {
                        if (!string.IsNullOrEmpty(i2.Patologia))
                        {
                            comboBox2.Items.Add(i2.Patologia);
                        }         
                    }

                    foreach (var i3 in _repoMenu)
                    {
                        if (!string.IsNullOrEmpty(i3.Evolucion))
                        {
                            comboBox3.Items.Add(i3.Evolucion);
                        }        
                    }

                    foreach (var i4 in _repoMenu)
                    {
                        if (!string.IsNullOrEmpty(i4.Ingreso))
                        {
                            comboBox6.Items.Add(i4.Ingreso);
                        }  
                    }
                }
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
                if (comboBox1.Text == "") { MessageBox.Show("Seleccione todas las opciones", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (comboBox2.Text == "") { MessageBox.Show("Seleccione todas las opciones", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (comboBox3.Text == "") { MessageBox.Show("Seleccione todas las opciones", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (comboBox5.Text == "") { MessageBox.Show("Seleccione todas las opciones", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (comboBox6.Text == "") { MessageBox.Show("Seleccione todas las opciones", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (comboBox4.Text == "") { MessageBox.Show("Seleccione todas las opciones", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox1.Text == "") { MessageBox.Show("Seleccione todas las opciones", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (comboBox2.Text == "Otros" && textBox2.Text == "")
                {
                    MessageBox.Show("Al seleccionar como patologia otros, debe escribir que tipo de patologia es de manera manual", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show("Una vez guardada esta estadistica, no se podran deshacer cambios. ¿Realmente desea guardar?",
                                                 "Zamenis Health - Enfermeria Estadisticas",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string Infectad;
                    switch (comboBox5.Text)
                    {
                        case "SI":
                            Infectad = "S";
                            break;

                        case "NO":
                            Infectad = "N";
                            break;

                        default:
                            Infectad = "N";
                            break;
                    }

                    string Alt;
                    switch (comboBox4.Text)
                    {
                        case "SI":
                            Alt = "S";
                            break;

                        case "NO":
                            Alt = "N";
                            break;

                        default:
                            Alt = "N";
                            break;
                    }

                    string Pat;
                    switch (comboBox2.Text)
                    {
                        case "Otros":
                            Pat = comboBox2.Text + " - " + textBox2.Text;
                            break;

                        default:
                            Pat = comboBox2.Text;
                            break;
                    }

                    DateTime Hoy = DateTime.Now.Date;

                    CXN_ESTADISTICAS E = new CXN_ESTADISTICAS
                    {
                        Est_Edad = comboBox1.Text,
                        Est_Localizacion = textBox1.Text,
                        Est_Evolucion = comboBox3.Text,
                        Est_Patologia = Pat,
                        Est_Infectado = Infectad,
                        Est_Ingreso = comboBox6.Text,
                        Est_Alta = Alt,
                        Est_Admision = Admition,
                        Est_Usuario = Comunes.Contenedor.UsuarioLogueado,
                        Est_Fecha = Convert.ToDateTime(Hoy)
                    };

                    repoNotasCuracion.Estadisticas_Curaciones(E);

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Otros")
            {
                label6.Visible = true;
                textBox2.Visible = true;
            }
            else
            {
                label6.Visible = false;
                textBox2.Visible = false;
            }
        }

        private void Estadistica_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                MessageBox.Show("Esta combinacion de teclas esta deshabilitada", "Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Handled = true;
            }
        }
    }
}
