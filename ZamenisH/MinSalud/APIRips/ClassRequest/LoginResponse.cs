namespace APIRips.ClassRequest
{
    public class LoginResponse
    {
        public string token { get; set; }
        public bool login { get; set; }
        public bool registrado { get; set; }
        public string[] errors { get; set; }
    }
}
