namespace desert_auth.Models
{
    public class Register
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string IP { get; set; }
    }
}
