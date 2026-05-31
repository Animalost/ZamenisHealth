using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Horarios_3 : ConfigForm.BaseForm
    {
        private static readonly IDisponibilidad repoDisp = new MDisponibilidad();

        private int Bod;

        public Horarios_3(int _bod)
        {
            InitializeComponent();
            this.Bod = _bod;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (comboBox1.Text == "" || comboBox2.Text == "" || textBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar todos los campos";
                    MG.ShowDialog();
                }
                else
                {
                    CXN_DISPONIBILIDAD_2 D = new CXN_DISPONIBILIDAD_2
                    {
                        Med = this.Bod,
                        Dia = comboBox1.Text,
                        Ide = textBox1.Text,
                        Hora = Convert.ToDateTime(dateTimePicker1.Value)
                    };

                    bool _consultarExistente = repoDisp.ConsultarCodigo(D);
                    if (_consultarExistente == true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Este codigo ya existe para este dia, escoja uno diferente";
                        MG.ShowDialog();
                    }
                    else
                    {
                        string Estado = "";

                        switch (comboBox2.SelectedIndex)
                        {
                            case 1:
                                Estado = "N";
                                break;

                            case 2:
                                Estado = "A";
                                break;

                            default:
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "Seleccion de habilitacion invalida";
                                MG.ShowDialog();
                                return;
                        }

                        D.Habilita = Estado;

                        bool _createHour = repoDisp.CrearHora(D);
                        if (_createHour != true)
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Error inesperado.   No se logro grabar la hora nueva, intente mas tarde";
                            MG.ShowDialog();
                        }
                        else
                        {
                            MG.TipoImagen = 3;
                            MG.Mensaje = "Hora creada exitosamente";
                            MG.ShowDialog();

                            this.Dispose();
                            this.Close();

                            Horarios f = new Horarios();
                            f.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Horarios_3_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Horarios";
            textBox1.MaxLength = 4;
        }
    }
}
