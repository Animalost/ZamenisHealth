using Domain.CONSUMOS;
using FormAndControls;
using Persistence;
using Persistence.CONSUMOS.Interfaces;
using Persistence.CONSUMOS.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Consumos
{
    public partial class MoverProducto : Forma2
    {
        private IProdsConsumo oProdConsumo;
        private IMovimientosConsumo oMovimientosConsumo;
        private IAsignacion oAsignacion;

        private int IdProducto;
        private string NameBodega;

        private MensajesGeneral MG;

        public MoverProducto(int idproducto, string nameBodega)
        {
            InitializeComponent();
            IdProducto = idproducto;
            NameBodega = nameBodega;

            oProdConsumo = new MProdsConsumo();
            oMovimientosConsumo = new MMovimientosConsumo();
            oAsignacion = new MAsignacion();
            SoloNumeros(textBox2);
        }

        private void MoverProducto_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Grabar Consumo";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarProducto();
        }
        void CargarProducto()
        {
            CON_PRODUCTOS getProd = oProdConsumo.GetProductoById(IdProducto);
            if (getProd == null)
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "No se logro cargar el prodcuto",
                    TipoImagen = 1000
                };
                MG.ShowDialog();

                this.Close();
            }
            else
            {
                textBox1.Text = getProd.Con_Prod_Name;
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Digite una cantidad a consumir",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else if (textBox2.Text == "0")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Digite una cantidad a consumir",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    CON_ASIGNACION dataUser = oAsignacion.GetBodega(NameBodega);

                    CON_CONSUMOS C = new CON_CONSUMOS
                    {
                        Con_Cons_Cantidad = Convert.ToInt32(textBox2.Text),
                        Con_Cons_Fecha = DateTime.Now.Date,
                        Con_Cons_Idconsultorio = dataUser.Asi_Number,
                        Con_Cons_IdProducto = IdProducto,
                        Con_Cons_Status = true,
                        Con_Cons_User = Contenedor.UsuarioLogueado
                    };

                    bool save = oMovimientosConsumo.SaveConsumo(C);
                    if (save == true)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Hecho",
                            TipoImagen = 3
                        };
                        MG.ShowDialog();

                        this.Close();
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se logro grabar el consumo",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
