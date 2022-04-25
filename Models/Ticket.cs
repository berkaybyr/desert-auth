namespace desert_auth.Models
{
    public class Ticket
    {
        public long ID { get; set; }        
        public DateTime CreatedOn { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }        
        public string? Header { get; set; }        
        public string Content { get; set; }
        public int Status { get; set; } //0 = Waiting // 1 = In Progress // 2 = Done
        public string Priority { get; set; } //0 = Low // 1 = Medium // 2 = High        
        public int Category { get; set; } //0 = General // 1 = Suggestion // 2 = Account Problems // 3 =  // 3 = PlayerReport //  2 = BugExploit
        public string UpdatedBy { get; set; }      
        


    }
}

