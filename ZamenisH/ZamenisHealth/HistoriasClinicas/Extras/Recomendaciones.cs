using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class Recomendaciones : ConfigForm.BaseForm
    {
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly IFHIR rFHIR = new MFHIR();

        private int Carta, Paciente;
        private CondicionesP c;
        private MensajesGeneral MG;

        DataTable dt;
        DataColumn POS;
        DataColumn Id;
        DataColumn Condicion;
        DataColumn Detalle;

        public Recomendaciones(int paciente)
        {
            InitializeComponent();
            this.Paciente = paciente;
        }

        private void Cargar()
        {
            try
            {
                List<string> getConditions = repoCond.getCondiciones(this.Paciente);
                if (getConditions != null)
                {
                    string resultado = getConditions.Find(x => x == label8.Text);
                    if (resultado != null)
                    {
                        label8.BackColor = Color.LightGreen;
                        label8.ForeColor = Color.Green;
                    }
                    else
                    {
                        label8.BackColor = Color.White;
                        label8.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label9.Text);
                    if (resultado != null)
                    {
                        label9.BackColor = Color.LightGreen;
                        label9.ForeColor = Color.Green;
                    }
                    else
                    {
                        label9.BackColor = Color.White;
                        label9.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label10.Text);
                    if (resultado != null)
                    {
                        label10.BackColor = Color.LightGreen;
                        label10.ForeColor = Color.Green;
                    }
                    else
                    {
                        label10.BackColor = Color.White;
                        label10.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label11.Text);
                    if (resultado != null)
                    {
                        label11.BackColor = Color.LightGreen;
                        label11.ForeColor = Color.Green;
                    }
                    else
                    {
                        label11.BackColor = Color.White;
                        label11.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label12.Text);
                    if (resultado != null)
                    {
                        label12.BackColor = Color.LightGreen;
                        label12.ForeColor = Color.Green;
                    }
                    else
                    {
                        label12.BackColor = Color.White;
                        label12.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label13.Text);
                    if (resultado != null)
                    {
                        label13.BackColor = Color.LightGreen;
                        label13.ForeColor = Color.Green;
                    }
                    else
                    {
                        label13.BackColor = Color.White;
                        label13.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label14.Text);
                    if (resultado != null)
                    {
                        label14.BackColor = Color.LightGreen;
                        label14.ForeColor = Color.Green;
                    }
                    else
                    {
                        label14.BackColor = Color.White;
                        label14.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label15.Text);
                    if (resultado != null)
                    {
                        label15.BackColor = Color.LightGreen;
                        label15.ForeColor = Color.Green;
                    }
                    else
                    {
                        label15.BackColor = Color.White;
                        label15.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == label16.Text);
                    if (resultado != null)
                    {
                        label16.BackColor = Color.LightGreen;
                        label16.ForeColor = Color.Green;
                    }
                    else
                    {
                        label16.BackColor = Color.White;
                        label16.ForeColor = Color.Black;
                    }
                }
                else
                {
                    label8.BackColor = Color.White;
                    label8.ForeColor = Color.Black;
                    label9.BackColor = Color.White;
                    label9.ForeColor = Color.Black;
                    label10.BackColor = Color.White;
                    label10.ForeColor = Color.Black;
                    label11.BackColor = Color.White;
                    label11.ForeColor = Color.Black;
                    label12.BackColor = Color.White;
                    label12.ForeColor = Color.Black;
                    label13.BackColor = Color.White;
                    label13.ForeColor = Color.Black;
                    label14.BackColor = Color.White;
                    label14.ForeColor = Color.Black;
                    label15.BackColor = Color.White;
                    label15.ForeColor = Color.Black;
                    label16.BackColor = Color.White;
                    label16.ForeColor = Color.Black;
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

        private void Recomendaciones_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Recomendaciones";
                Cargar();
                CargarRisk();
                CargarPrevRisk();
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

        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Condicion = dt.Columns.Add("Condicion", typeof(string));
            Detalle = dt.Columns.Add("Detalle", typeof(string));
        }

        void CargarPrevRisk()
        {
            try
            {
                List<CXN_CONDICIONES> _lista = repoCond.getCondicionesFHIR(Paciente);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_CONDICIONES i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id.ToString();
                        row["Condicion"] = i.Condicion.ToString();
                        row["Detalle"] = i.Detalle;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {                    
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Condicion"].Width = 120;
            D.Columns["Detalle"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Condicion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Detalle"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;

            foreach (DataGridViewRow row in D.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                }
            }
        }

        void CargarRisk()
        {
            comboBox1.Items.Add("Ninguno");

            List<string> car = rFHIR.CargarRISK();
            if (car != null)
            {
                foreach (string s in car)
                {
                    comboBox1.Items.Add(s);
                }
            }
        }

        private void Recomendaciones_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Escape)
                {
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

        private void pictureBox7_MouseMove(object sender, MouseEventArgs e)
        {
            //CAIDA
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente tiene algun tipo de riesgo de caida durante la atencion o marcha con dificultad, usa bastón, usa de silla de ruedas, o tiene discapacidad";
            label5.Size = new System.Drawing.Size(236, 160);
            label5.Location = new System.Drawing.Point(11, 81);
            this.Carta = 1;
        }

        private void pictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            //INFECCION
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente tiene alguna infeccion viral o similar que puede ser contagiosa";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(256, 81);
            this.Carta = 2;
        }

        private void pictureBox8_MouseMove(object sender, MouseEventArgs e)
        {
            //DETERIORO DE LA PIEL
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente tiene algun tipo de deterioro de la piel que indica que debe seguir un tratamiento especial";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(408, 81);
            this.Carta = 3;
        }

        private void pictureBox9_MouseMove(object sender, MouseEventArgs e)
        {
            //ALERGIA
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente tiene algun tipo de alergia, tambien recuerde agregarla en la opcion de alergias";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(559, 81);
            this.Carta = 4;
        }

        private void pictureBox10_MouseMove(object sender, MouseEventArgs e)
        {
            //COMUNICACION
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente tiene limitacion acustica y/o lenguaje oral";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(711, 81);
            this.Carta = 5;
        }

        private void pictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            //ACOMPAÑANTE
            label5.Visible = true;
            label5.Text = "Marque esta opcion si por alguna razon el medico autoriza la presencia de un acompañante durante la atencion del paciente";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(635, 289);
            this.Carta = 6;
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            //PACIENTE DIFICIL
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente presenta riesgo de fugarse, tiene comportamiento agresivo o inapropiado";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(452, 289);
            this.Carta = 7;
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            //MAYOR 70 AÑOS
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente es mayor de 70 años";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(274, 289);
            this.Carta = 8;
        }

        private void pictureBox5_MouseMove(object sender, MouseEventArgs e)
        {
            //PSIQUIATRICO
            label5.Visible = true;
            label5.Text = "Marque esta opcion si el paciente es psiquiatrico";
            label5.Size = new System.Drawing.Size(138, 160);
            label5.Location = new System.Drawing.Point(88, 289);
            this.Carta = 9;
        }

        private void Recomendaciones_MouseMove(object sender, MouseEventArgs e)
        {
            label5.Visible = false;
            label5.Text = "";
            this.Carta = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea agregar esta condicion?  Una vez agregada no se puede eliminar",
                                                "Zamenis Health - FHIR Ministerio de Salud",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (comboBox1.Text == "" || string.IsNullOrEmpty(textBox1.Text))
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Debe seleccionar un riesgo y agregar su descripcion",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                    }
                    else
                    {
                        CXN_CONDICIONES C = new CXN_CONDICIONES
                        {
                            Condicion = comboBox1.Text,
                            Paciente = Paciente,
                            Usuario = Contenedor.UsuarioLogueado,
                            Detalle = textBox1.Text.Trim(),
                            Fecha = DateTime.Now.Date,
                            Habilita = "A",
                            CodigoFHIR = "XX"
                        };

                        repoCond.createCondiciones(C);

                        CargarPrevRisk();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            try
            {
                switch (this.Carta)
                {
                    case 1:
                        c = new CondicionesP(label8.Text, this.Paciente);
                        c.ShowDialog();
                        break;
                    case 2:
                        c = new CondicionesP(label9.Text, this.Paciente);
                        c.ShowDialog();
                        break;
                    case 3:
                        c = new CondicionesP(label10.Text, this.Paciente);
                        c.ShowDialog();
                        break;
                    case 4:
                        Alergias ale = new Alergias(label11.Text, this.Paciente);
                        ale.ShowDialog();
                        break;
                    case 5:
                        c = new CondicionesP(label12.Text, this.Paciente);
                        c.ShowDialog();
                        break;
                    case 6:
                        c = new CondicionesP(label16.Text, this.Paciente);
                        c.ShowDialog();
                        break;
                    case 7:
                        c = new CondicionesP(label15.Text, this.Paciente);
                        c.ShowDialog();
                        break;
                    case 8:
                        c = new CondicionesP(label14.Text, this.Paciente);
                        c.ShowDialog();
                        break;
                    case 9:
                        c = new CondicionesP(label13.Text, this.Paciente);
                        c.ShowDialog();
                        break;

                    default:
                        this.Carta = 0;
                        break;
                }

                Cargar();
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
