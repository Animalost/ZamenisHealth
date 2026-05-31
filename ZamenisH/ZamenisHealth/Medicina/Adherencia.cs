using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Adherencia : Forma2
    {               
        private static readonly IAdherencia rpositorioAdherencia = new MAdherencia();

        private string Tipo_ADH;
        List<string> Ap = new List<string>();

        public Adherencia(string tipoADH)
        {
            InitializeComponent();
            this.Tipo_ADH = tipoADH;
        }
        private void Adherencia_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Adherencia";
                var lista = rpositorioAdherencia.listaApositos();
                if (lista != null)
                {
                    foreach (var i in lista)
                    {
                        comboBox1.Items.Add(i.Adh_Aposito);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Ap.Clear();
            richTextBox1.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string desc = rpositorioAdherencia.DescripcionAposito(comboBox1.Text);
                if (desc != "")
                {
                    Ap.Add(comboBox1.Text + " -- Agregar descripcion aqui -- ");
                    richTextBox1.Text = richTextBox1.Text + comboBox1.Text + System.Environment.NewLine + desc + System.Environment.NewLine + System.Environment.NewLine;
                }
                else
                {
                    MessageBox.Show("Aposito con inconvenientes, contacte la administracion", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                switch (Tipo_ADH)
                {
                    case "Notas":
                        HistoriasClinicas.Historia_NotaEnfermeria f2 = Application.OpenForms.OfType<HistoriasClinicas.Historia_NotaEnfermeria>().SingleOrDefault();
                        f2.richTextBox1.Text = richTextBox1.Text;
                        f2.textBox15.Text = String.Join(Environment.NewLine, Ap);
                        Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                        MG.Mensaje = "Recuerde MODIFICAR la observacion de la nota de curacion con los apositos elegidos";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                        break;

                    case "CMAN":
                        Medicina.CambioManejoMedico f3 = Application.OpenForms.OfType<Medicina.CambioManejoMedico>().SingleOrDefault();
                        f3.richTextBox1.Text = richTextBox1.Text;
                        MessageBox.Show("Si considera que puede añadir una observacion adicional por el uso o cambio de apositos a nivel profesional, no dude en hacerlo", "Tip de Ayuda a Facturacion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.Dispose();
                        this.Close();
                        break;

                    case "NotasCore":
                        HistoriasClinicas.NotaEnfermeria.NotaCuracion f4 = Application.OpenForms.OfType<HistoriasClinicas.NotaEnfermeria.NotaCuracion>().SingleOrDefault();
                        f4.richTextBox1.Text = richTextBox1.Text;
                        f4.textBox15.Text = String.Join(Environment.NewLine, Ap);
                        Comunes.MensajesGeneral MG4 = new Comunes.MensajesGeneral();
                        MG4.Mensaje = "Recuerde MODIFICAR la observacion de la nota de curacion con los apositos elegidos";
                        MG4.TipoImagen = 3;
                        MG4.ShowDialog();
                        this.Dispose();
                        this.Close();
                        break;

                    default:
                        MessageBox.Show("No se logro cargar el tipo de formulario, cierre esta pantalla y vuelva a intentarlo", "Incidencia de sistema", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
