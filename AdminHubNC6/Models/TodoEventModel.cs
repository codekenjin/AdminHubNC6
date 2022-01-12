namespace AdminHubNC6.Models
{
    public class TodoEventModel
    {
        public int id { get; set; }
        public string? title { get; set; }
        public System.DateTime? start { get; set; }
        public System.DateTime? end { get; set; }

        public string? description { get; set; }
        public string? color { get; set; }
        public bool allDay { get; set; }
    }
}
