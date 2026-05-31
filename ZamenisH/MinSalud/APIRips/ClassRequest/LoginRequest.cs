namespace APIRips.ClassRequest
{
    public class identificacion
    {
        public string tipo { get; set; }
        public string numero { get; set; }
    }

    public class persona
    {
        public identificacion identificacion { get; set; }
    }

    public class loginRequest
    {
        public persona persona { get; set; }
        public string clave { get; set; }
        public string nit { get; set; }
    }
}
