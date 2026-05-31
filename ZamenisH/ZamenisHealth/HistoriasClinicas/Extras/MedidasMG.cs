using System;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class MedidasMG : ConfigForm.BaseForm
    {
        private static readonly IMedidasHerida repositorioMedidasHerida = new MMedidasHerida();
        private static readonly IMenu repositorioNMenu = new MMenu();

        private int Admision, Paciente;

        private void label78_Click(object sender, EventArgs e)
        {
            try
            {
                decimal Valor1, Valor2, Valor3, Res;
                Valor1 = Convert.ToDecimal(textBox46.Text);
                Valor2 = Convert.ToDecimal(textBox45.Text);
                Valor3 = Convert.ToDecimal(textBox44.Text);

                Res = (Valor1 * Valor2) * Valor3;
                //textBox5.Text = Convert.ToDecimal(Res).ToString();
                textBox43.Text = Convert.ToDecimal(decimal.Round(Res, 2)).ToString();

                if (Convert.ToDecimal(textBox43.Text) < 10)
                {
                    comboBox25.Text = "Baja Complejidad";
                    return;
                }
                else if (Convert.ToDecimal(textBox43.Text) < 29)
                {
                    comboBox25.Text = "Media  Complejidad";
                    return;
                }
                else
                {
                    comboBox25.Text = "Alta  Complejidad";
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void label85_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Si el resultado es menor a 10 es Baja Complejidad, \n\r" +
                          "Si es menor a 29 es Media Complejidad, \n\r" +
                          "si es Mayor a 30 es Alta Complejidad \n\r",
                          "Valores de Referencia",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox46.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox45.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox44.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox43.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (comboBox25.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox46.Text != "" && textBox47.Text == "") { MessageBox.Show("Debe diligenciar la zona anatomica de la herida"); return; }

                DateTime Hoy = DateTime.Now;

                CXN_HCMED H = new CXN_HCMED
                {
                    Med_Adm = Admision,
                    Med_Fecha = Hoy,
                    Med_Largo = textBox46.Text,
                    Med_Ancho = textBox45.Text,
                    Med_Determinacion = comboBox25.Text,
                    Med_Observacion = textBox47.Text,
                    Med_Pac = Paciente,
                    Med_Profundidad = textBox44.Text
                };

                bool ingMed = repositorioMedidasHerida.insertarHerida(H);
                if (ingMed != true)
                {
                    MessageBox.Show("Error insertando medida, revise datos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("Ingresado!!", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox46.Text = "";
                    textBox45.Text = "";
                    textBox44.Text = "";
                    textBox43.Text = "";
                    textBox47.Text = "";
                    comboBox25.Text = "";
                    Carga_Med();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public MedidasMG(int _admision, int _paciente)
        {
            InitializeComponent();
            this.Admision = _admision;
            this.Paciente = _paciente;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                repositorioMedidasHerida.deleteMedida(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                Carga_Med();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void MedidasMG_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                Carga_Med();

                var getMenus = repositorioNMenu.getMenus();
                if (getMenus != null)
                {
                    foreach (var i16 in getMenus)
                    {
                        comboBox25.Items.Add(i16.Tamaño.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox47_TextChanged(object sender, EventArgs e)
        {
            textBox47.CharacterCasing = CharacterCasing.Upper;
        }

        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Med_Id", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Admision", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Largo", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Ancho", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Profundidad", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Determinacion", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Observacion", 200, HorizontalAlignment.Left);
        }
        private void Carga_Med()
        {
            try
            {
                var getMedidas = repositorioMedidasHerida.Carga_Med(Admision);
                if (getMedidas != null)
                {
                    Encabezados();

                    foreach (var i in getMedidas)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                           {
                                i.Med_Id.ToString(),
                                i.Med_Adm.ToString(),
                                i.Med_Largo.ToString(),
                                i.Med_Ancho.ToString(),
                                i.Med_Profundidad.ToString(),
                                i.Med_Determinacion.ToString(),
                                i.Med_Observacion.ToString()
                           }));
                    }
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return;
            }
        }
    }
}
