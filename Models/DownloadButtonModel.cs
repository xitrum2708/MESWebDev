namespace MESWebDev.Models
{
    public class DownloadButtonModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public Dictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();
        public string ButtonText { get; set; }
    }
}