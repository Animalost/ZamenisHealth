using APIRips;
using Domain;
using Domain.CXN;
using FormAndControls;
using Newtonsoft.Json.Linq;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using static APIRips.ClassRequest.SendRIPMinSaludResponse;

namespace ZamenisHealth.Facturacion
{
    public partial class RIPS_Docker : Forma2
    {
        private DateTime FechaGrid;
        private int Ase, Cia;
        private string Tipo, FacGrid, CUVGrid;
        private Comunes.MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Documento;
        DataColumn Fecha;
        DataColumn Homologo;
        DataColumn Valor;
        DataColumn Paciente;
        DataColumn CUV;

        private static readonly IReportes repoReportes = new MReportes();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private static readonly IHelisa repoHelisa = new MHelisa();
        private static readonly IGenerales repoGenerales = new MGenerales();
        private static readonly IRIPSJSON repoRIPSJSON = new MRipsJSON();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();
        private static readonly IAseguradoras repoAse = new MAseguradoras();
        private static readonly ICompañia repoCom = new MCompañia();
        private static readonly IRIPS_Res2275_2023 repoRIPDocker = new MRIPS_Res2275_2023();

        private Dictionary<string, DateTime> Dic = new Dictionary<string, DateTime>();
        private MensajesGeneral M;
        private string TokenGeneradoMinSalud;

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string FacturaElectronica = Convert.ToString(row.Cells["Homologo"].Value); 
                    string FechaFacturaElectronica = Convert.ToString(row.Cells["Fecha"].Value); 
                    string CUVGenerado = Convert.ToString(row.Cells["CUV"].Value); 

                    if (FacturaElectronica == "") { MessageBox.Show("Hay espacios vacios en la columna Homologo"); return; }
                    if (FechaFacturaElectronica == "") { MessageBox.Show("Hay espacios vacios en la columna Fecha"); return; }

                    if (string.IsNullOrEmpty(CUVGenerado) || CUVGenerado == "")
                    {
                        Dic.Add(FacturaElectronica, Convert.ToDateTime(FechaFacturaElectronica));
                    }                    
                }

                if (Dic.Count <= 0)
                {
                    MessageBox.Show("No hay Facturas sin CUVs generados para radicar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    DialogResult result = MessageBox.Show("Los datos parecen correctos, ¿Desea radicar en el Ministerio de Salud?",
                                                     "Zamenis Health - Radicar MinSalud",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Dictionary<string, string> datos = repoHelisa.Claves("MinSalud", Cia);

                        string Usuario = datos["User"];
                        string Contraseña = datos["Pass"];
                        string Nit = datos["Nit"];
                        string Tipo = datos["Tipo"];

                        var jsonResponse = await Methods.Loguear(textBox1.Text, Usuario, Contraseña, Nit, Tipo);
                        if (jsonResponse.statusCode == 200) 
                        { 
                            if (jsonResponse.data != null)
                            {
                                TokenGeneradoMinSalud = jsonResponse.data.token;
                                button1.Enabled = false;
                                button2.Enabled = true;

                                bool crearToken = repoRIPDocker.insertToken(TokenGeneradoMinSalud, Cia);
                                if (crearToken == true)
                                {
                                    MG = new MensajesGeneral();
                                    MG.TipoImagen = 3;
                                    MG.Mensaje = "Token Creado";
                                    MG.ShowDialog();    
                                }
                                else
                                {
                                    string err = !string.IsNullOrEmpty(jsonResponse.errorMessage) ? jsonResponse.errorMessage : "Error MinSalud";

                                    MG = new MensajesGeneral();
                                    MG.TipoImagen = 1000;
                                    MG.Mensaje = "No se logro crear el Token: " + err;
                                    MG.ShowDialog();
                                }
                            }
                            else
                            {
                                button2.Enabled = false;
                                button1.Enabled = true;
                                MessageBox.Show("Error " + jsonResponse.statusCode + ": " + jsonResponse.errorMessage + ".  Se ha retornado NULL como respuesta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            button2.Enabled = false;
                            button1.Enabled = true;
                            MessageBox.Show("Error " + jsonResponse.statusCode + ": " + jsonResponse.errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                button2.Enabled = false;
                button1.Enabled = true;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> datosFacturas = new Dictionary<string, string>();
                Dictionary<string, string> datos = repoHelisa.Claves("Helisa", Cia);

                string Empresa = datos["Company"];
                string Usuario = datos["User"];
                string Cliente = datos["Client"];
                string Pwd = datos["Pass"];                              

                //recorro el grid
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string cuv = Convert.ToString(row.Cells["CUV"].Value);
                    string NoDocumento = Convert.ToString(row.Cells["Homologo"].Value);
                    int Periodo = Convert.ToInt32(Convert.ToDateTime(row.Cells["Fecha"].Value).ToString("yyyy"));

                    string carpeta = @"C:\CXN\Reportes\documentos"; // Ruta de la carpeta donde están los archivos                    

                    try
                    {
                        if (string.IsNullOrEmpty(cuv))
                        {
                            string nombreArchivoBuscado = "ACUSE" + Empresa + "_01_" + NoDocumento + "_" + Periodo + ".xml"; // Nombre del archivo a buscar
                            string[] archivos = Directory.GetFiles(carpeta, nombreArchivoBuscado, SearchOption.TopDirectoryOnly);

                            if (archivos.Length > 0)
                            {
                                string rutaArchivo = archivos[0]; // Tomamos el primer archivo encontrado
                                string contenido = File.ReadAllText(rutaArchivo); // Leer contenido del archivo

                                string xmlBase64 = repoGenerales.Base64Encode(contenido);
                                datosFacturas.Add(NoDocumento, xmlBase64);
                            }
                            else
                            {
                                Console.WriteLine("Archivo no encontrado.");
                            }
                        }                       
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

                int da = datosFacturas.Count;
                progressBar1.Value = 0;
                progressBar1.Maximum = 100;
                double au = 100 / da;

                //recorro el diccionario para obtener las facturas unicas
                foreach (var i in datosFacturas)
                {
                    progressBar1.Increment(Convert.ToInt32(au));
                    //Obtengo el RIP Json y le asigno el xml en base64
                    TransaccionDocker d = repoRIPSJSON.GenrateTotalMinSalud(Cia, i.Key, i.Value);
                    if (d != null)
                    {
                        string fac = i.Key.ToString();
                        string respuestaMala = "";

                        string getToken = repoRIPDocker.getLastToken();
                        if (getToken == null) 
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "El token no es valido, debe generar uno nuevo";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            break;
                        }

                        var (statusCode, responseData, errorMessage) = await Methods.SendFacturaMinSalud(textBox2.Text, getToken, d);

                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Permite caracteres especiales sin escapar
                        };

                        if (statusCode == 200)
                        {
                            string formattedJson = System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(responseData).RootElement, options);
                            string CUV = "";

                            ApiResponse response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(formattedJson);
                            if (response.ResultState == true)
                            {
                                CUV = response.CodigoUnicoValidacion.ToString().Trim();
                                repoFacturacion.UpdateCUV(fac, CUV);                                
                            }

                            JObject obj = JObject.Parse(formattedJson);
                            int idcargue = 0;

                            if (obj.TryGetValue("ProcesoId", out JToken procesoIdToken))
                            {
                                int procesoId = procesoIdToken.Value<int>();
                                idcargue = procesoId;
                            }

                            string filePath = $@"C:\CXN\Reportes\ResultadosMSPS_" + fac + "_ID" + idcargue + "_A_CUV.txt";
                            //string filePath = $@"C:\CXN\Reportes\" + fac + ".json";
                            File.WriteAllText(filePath, formattedJson, new UTF8Encoding(false));
                        }
                        else
                        {
                            string formattedJson = System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(responseData).RootElement, options);

                            respuestaMala = respuestaMala + "FACTURA: " + fac + " Error " + statusCode.ToString() + ": " + formattedJson + "\n\r";
                            string filePath = $@"C:\CXN\Reportes\ERROR_" + fac + ".json";
                            File.WriteAllText(filePath, respuestaMala, new UTF8Encoding(false));
                        }
                    }
                }

                progressBar1.Value = 0;

                MG = new MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Hecho";
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Button_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var A = repoAse.getInfoFromAsebyName(comboBox1.Text);
            Ase = Convert.ToInt32(A.Ase_Identificador);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var C = repoCom.getPrestadorbyName(comboBox2.Text);
            Cia = Convert.ToInt32(C.Com_Identificador);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox4.Text)
                {
                    case "Facturas":
                        Tipo = "FA";
                        break;

                    case "Ordenes de Pedido":
                        Tipo = "OP";
                        break;

                    case "Documentos Equivalentes":
                        Tipo = "DE";
                        break;

                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione una opcion valida del tipo de documentos";
                        MG.ShowDialog();
                        return;
                }

                List<FacturacionReports> lista = repoReportes.Exportar(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                       Convert.ToDateTime(dateTimePicker2.Value.Date),
                                                                       Cia,
                                                                       Ase,
                                                                       Tipo);
                if (lista == null)
                {
                    Encabezados();

                    M = new MensajesGeneral();
                    M.TipoImagen = 0;
                    M.Mensaje = "No hay resultados en este rango de fechas y tipo de documento";
                    M.ShowDialog();
                    return;
                }

                Encabezados();

                int Contador = 1;

                foreach (FacturacionReports report in lista)
                {
                    DataRow row = dt.NewRow();

                    row["POS"] = Contador;
                    row["Documento"] = report.Admision.ToString();
                    row["Fecha"] = Convert.ToDateTime(report.FechaBase).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    row["Homologo"] = report.Homologo.ToString();
                    row["Valor"] = "$ " + Convert.ToInt32(report.ValorReciboFactura).ToString("N0");
                    row["Paciente"] = report.PacienteNombre.ToString();
                    row["CUV"] = report.Com_Direccion;

                    dt.Rows.Add(row);
                    dt.AcceptChanges();

                    Contador = Contador + 1;
                }

                Contador = 1;
                Estilos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Font = new Font("Arial", 12);
                menu.Padding = new Padding(5, 5, 5, 30);

                menu.Items.Add("Ingresar CUV Manual", Properties.Resources2.MayorQue_Black).Name = "CUV_Manual";
                menu.Items["CUV_Manual"].Click += CUV_Manual;
                menu.Items.Add("Radicar RIPS en MinSalud", Properties.Resources2.MayorQue_Black).Name = "CUV_Automatico";
                menu.Items["CUV_Automatico"].Click += CUV_Automatico;

                FechaGrid = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                FacGrid = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                CUVGrid = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();

                Rectangle coordenada = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                int anchoCelda = coordenada.Location.X - 100;
                int altoCelda = coordenada.Location.Y;
                int X = anchoCelda + dataGridView1.Location.X;
                int Y = altoCelda + dataGridView1.Location.Y;
                menu.Show(dataGridView1, new System.Drawing.Point(X, Y));
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void CUV_Manual(object sender, EventArgs e)
        {
            try
            {
                if (CUVGrid == "" || CUVGrid == null)
                {
                    RIPS_Docker_2 R = new RIPS_Docker_2(FacGrid);
                    R.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "CUV ya fue generado para esta factura";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async void CUV_Automatico(object sender, EventArgs e)
        {
            try
            {
                if (CUVGrid == "" || CUVGrid == null)
                {
                    string NoDocumento = FacGrid;
                    string xmlBase64 = "";

                    Dictionary<string, string> datos = repoHelisa.Claves("Helisa", Cia);

                    string Empresa = datos["Company"];
                    string Usuario = datos["User"];
                    string Cliente = datos["Client"];
                    string Pwd = datos["Pass"];

                    int Periodo = Convert.ToInt32(Convert.ToDateTime(FechaGrid).ToString("yyyy"));

                    string carpeta = @"C:\CXN\Reportes\documentos"; // Ruta de la carpeta donde están los archivos                    

                    string nombreArchivoBuscado = "ACUSE" + Empresa + "_01_" + NoDocumento + "_" + Periodo + ".xml"; // Nombre del archivo a buscar
                    string[] archivos = Directory.GetFiles(carpeta, nombreArchivoBuscado, SearchOption.TopDirectoryOnly);

                    if (archivos.Length > 0)
                    {
                        string rutaArchivo = archivos[0]; // Tomamos el primer archivo encontrado
                        string contenido = File.ReadAllText(rutaArchivo); // Leer contenido del archivo

                        xmlBase64 = repoGenerales.Base64Encode(contenido);
                    }
                    else
                    {
                        TXTException T = new TXTException { FechaHora = DateTime.Now, Error = "Archivo no encontrado: " + NoDocumento, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                    }

                    //Obtengo el RIP Json y le asigno el xml en base64
                    TransaccionDocker d = repoRIPSJSON.GenrateTotalMinSalud(Cia, NoDocumento, xmlBase64);
                    if (d != null)
                    {
                        string fac = NoDocumento;
                        string respuestaMala = "";

                        var (statusCode, responseData, errorMessage) = await Methods.SendFacturaMinSalud(textBox2.Text, TokenGeneradoMinSalud, d);

                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Permite caracteres especiales sin escapar
                        };

                        if (statusCode == 200)
                        {
                            string formattedJson = System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(responseData).RootElement, options);
                            string CUV = "";

                            ApiResponse response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(formattedJson);
                            if (response.ResultState == true)
                            {
                                CUV = response.CodigoUnicoValidacion.ToString().Trim();
                                repoFacturacion.UpdateCUV(fac, CUV);
                            }

                            JObject obj = JObject.Parse(formattedJson);
                            int idcargue = 0;

                            if (obj.TryGetValue("ProcesoId", out JToken procesoIdToken))
                            {
                                int procesoId = procesoIdToken.Value<int>();
                                idcargue = procesoId;
                            }

                            string filePath = $@"C:\CXN\Reportes\ResultadosMSPS_" + fac + "_ID" + idcargue + "_A_CUV.txt";
                            //string filePath = $@"C:\CXN\Reportes\" + fac + ".json";
                            File.WriteAllText(filePath, formattedJson, new UTF8Encoding(false));
                        }
                        else if (statusCode == 500)
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Error 500 del servidor del Ministerio de Salud: " + errorMessage.ToString();
                            MG.ShowDialog();
                        }
                        else
                        {
                            string formattedJson = System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(responseData).RootElement, options);

                            respuestaMala = respuestaMala + "FACTURA: " + fac + " Error " + statusCode.ToString() + ": " + formattedJson + "\n\r";
                            string filePath = $@"C:\CXN\Reportes\ERROR_" + fac + ".json";
                            File.WriteAllText(filePath, respuestaMala, new UTF8Encoding(false));
                        }
                    }

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Hecho";
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "CUV ya fue generado para esta factura";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public RIPS_Docker()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RIPS_Docker_3 rIPS_Docker_3 = new RIPS_Docker_3();
            rIPS_Docker_3.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea actualizar las url de las API?",
                                                  "Zamenis Health - Ministerio de Salud",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
                    {
                        CXN_IMAGEN_SYSTEM I = new CXN_IMAGEN_SYSTEM
                        {
                            Tab_Clave = textBox1.Text.Trim(),
                            Tab_Nombre = "UrlMinSaludLoginRIPS"
                        };

                        bool update = repoConfSystem.updateImagenSystem(I);
                        if (update == true)
                        {
                            CXN_IMAGEN_SYSTEM I2 = new CXN_IMAGEN_SYSTEM
                            {
                                Tab_Clave = textBox2.Text.Trim(),
                                Tab_Nombre = "UrlMinSaludRIPS"
                            };

                            bool update2 = repoConfSystem.updateImagenSystem(I2);
                            if (update2 == true)
                            {
                                MG = new MensajesGeneral();
                                MG.TipoImagen = 3;
                                MG.Mensaje = "URLs actualizadas correctamente";
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "No se pudo actualizar la URL de MinSalud RIPS";
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No se pudo actualizar las URLs";
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Por favor, complete ambos campos de URL";
                        MG.ShowDialog(); ;
                    }
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Documento = dt.Columns.Add("Documento", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Homologo = dt.Columns.Add("Homologo", typeof(string));
            Valor = dt.Columns.Add("Valor", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            CUV = dt.Columns.Add("CUV", typeof(string));
        }

        private void RIPS_Docker_Load(object sender, EventArgs e)
        {
            try
            {
                

                this.Titulo.Text = "RIPS Docker - Radicacion MinSalud";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                var Ases = repoAse.getAseguradoras();
                if (Ases != null)
                {
                    foreach (var i in Ases)
                    {
                        comboBox1.Items.Add(i.Ase_Descripcion);
                    }
                }

                var Cias = repoCom.getAllCompañias();
                if (Cias != null)
                {
                    foreach (var i in Cias)
                    {
                        comboBox2.Items.Add(i.Com_Nombre);
                    }
                }        
                
                Dictionary<string, string> urlAPIS = repoConfSystem.getListado();
                textBox1.Text = urlAPIS["UrlMinSaludLoginRIPS"];
                textBox2.Text = urlAPIS["UrlMinSaludRIPS"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Estilos()
        {
            try
            {
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ScrollBars = ScrollBars.Both;

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["Documento"].Width = 110;
                dataGridView1.Columns["Fecha"].Width = 110;
                dataGridView1.Columns["Homologo"].Width = 110;
                dataGridView1.Columns["Valor"].Width = 200;
                dataGridView1.Columns["Paciente"].Width = 400;
                dataGridView1.Columns["CUV"].Width = 500;
                dataGridView1.Font = new Font("Arial", 11);

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                dataGridView1.Columns["Documento"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Homologo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Valor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["CUV"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["Documento"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Homologo"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Valor"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["CUV"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridView1.Columns["POS"].Visible = false;

                foreach (DataGridViewRow row in dataGridView1.Rows)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }          
        }        
    }
}
