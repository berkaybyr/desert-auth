namespace desert_auth.Models
{
    public class User
    {
        public long ID { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool isGM { get; set; }
        public string FamilyName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool isEmailConfirmed { get; set; }
        public long Balance { get; set; }
        public string LastIP { get; set; }
        public long  TotalPlaytime { get; set; }
        public DateTime PremiumEnd { get; set; }    
    }
}
