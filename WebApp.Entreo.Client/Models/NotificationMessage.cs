namespace WebApp.Entreo.Client.Models
{
    public class NotificationMessage
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}