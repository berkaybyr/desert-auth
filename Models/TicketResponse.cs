namespace desert_auth.Models
{
    public class TicketResponse
    {
        public long ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public long TicketID { get; set; }
        public string? Header { get; set; }
        public string Content { get; set; }
        public string SentBy { get; set; }
    }
}
