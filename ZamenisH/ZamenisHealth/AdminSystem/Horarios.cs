using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Horarios : ConfigForm.BaseForm
    {
        private static readonly IDisponibilidad repoDisponibilidad = new MDisponibilidad();
        private static readonly IBodegas repoBodegas = new MBodegas();

        int Bod;
        public Horarios()
        {
            InitializeComponent();
            ConfigForm.GraficarControl(button1, 1, Color.Red);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();

        }

        private void Horarios_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Horarios";
                

                var getProfesionales = repoBodegas.getBodegas();
                if (getProfesionales != null)
                {
                    foreach (var i in getProfesionales)
                    {
                        comboBox1.Items.Add(i);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void EncabezadosLv1()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Posision", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Profesional", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Dia", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Hora", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Habilita", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Identificador", 100, HorizontalAlignment.Left);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var Dato = repoBodegas.ProfesionalId(comboBox1.Text);
                if (Dato.CodProf == 0)
                {
                    MessageBox.Show("Profesional sin bodega", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Bod = 0;
                    listView1.Clear();
                    return;
                }

                Bod = Dato.CodProf;

                var getHorarios = repoDisponibilidad.getHorariosByCodeMed(Bod);
                if (getHorarios != null)
                {
                    EncabezadosLv1();

                    foreach (var i in getHorarios)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                             i.Id.ToString(),
                             i.Med.ToString(),
                             i.Dia.ToString(),
                             Convert.ToDateTime(i.Hora).ToString("HH:mm tt"),
                             i.Habilita.ToString(),
                             i.Ide.ToString()
                        }));
                    }
                }
                else
                {
                    EncabezadosLv1();

                    DialogResult result = MessageBox.Show("Este profesional aun no tiene horarios asignados.  ¿Desea asignar uno nuevo?",
                                               "Zamenis Health - Horarios",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Horarios_3 H = new Horarios_3(Bod);
                        H.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            Horarios_2 Horarios_2 = new Horarios_2();
            Horarios_2.Tipo_Horario = listView1.SelectedItems[0].SubItems[0].Text;
            Horarios_2.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigForm.PosGrillaMDI = Bod;
            this.Dispose();
            this.Close();

            Horarios_3 H3 = new Horarios_3(ConfigForm.PosGrillaMDI);
            H3.ShowDialog();
        }
    }
}
