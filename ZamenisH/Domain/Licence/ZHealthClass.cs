namespace Domain.Licence
{
    public class ZhealthClass
    {
        public Roles Roles { get; set; }
        public Conexiones Conexiones { get; set; }
        public Prestador Prestador { get; set; }
        public Generales Generales { get; set; }
        public OtrosPermisos OtrosPermisos { get; set; }
        public Recordatorios Recordatorios { get; set; }
        public Pagos Pagos { get; set; }
    }
    public class Roles
    {
        public string Recepcion { get; set; }
        public string Opciones { get; set; }
        public string Administracion { get; set; }
        public string Gerencial { get; set; }
        public string Enfermeria { get; set; }
        public string Medicina_General { get; set; }
        public string Radiologia { get; set; }
        public string Fisiatria { get; set; }
        public string TOcupacional { get; set; }
        public string TFisica { get; set; }
        public string Psicologia { get; set; }
    }
    public class Conexiones
    {
        public string ConexionPrincipal { get; set; }
        public string ConexionInventarios { get; set; }
        public string ConexionAdjuntos { get; set; }
    }
    public class Prestador
    {
        public string Tercero { get; set; }
        public string Nit { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
    public class Generales
    {
        public string FormatoFecha { get; set; }
        public string Equipo { get; set; }
        public string Habilitar { get; set; }
        public string Vencimiento { get; set; }
        public string Mensaje { get; set; }
        public string KeyAdmin { get; set; }
        public string UrlAPI { get; set; }
    }
    public class OtrosPermisos
    {
        public string VideoConferencia { get; set; }
        public string Recordatorios { get; set; }
        public string Perplexity { get; set; }
    }
    public class Recordatorios
    {
        public string API { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
    }
    public class Pagos
    {
        public string Estado { get; set; }
        public string URL { get; set; }
    }
}
