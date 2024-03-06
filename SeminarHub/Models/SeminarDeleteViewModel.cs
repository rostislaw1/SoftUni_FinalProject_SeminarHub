namespace SeminarHub.Models
{
    public class SeminarDeleteViewModel
    {
        public int Id { get; set; } 
        public DateTime DateAndTime { get; set; }
        public string Topic { get; set; } = string.Empty;
    }
}
