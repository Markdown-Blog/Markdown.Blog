namespace Markdown.Blog.Client.Configuration
{
    public class ContentDeliveryConfiguration
    {
        public string Provider { get; set; } // "Cloudflare", "Github", etc.
        public string Domain { get; set; }
        public bool SupportsETag { get; set; }
    }
}
