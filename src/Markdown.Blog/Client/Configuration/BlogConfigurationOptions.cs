namespace Markdown.Blog.Client.Configuration
{
    public class BlogConfigurationOptions
    {
        public List<DivisionOptions> Divisions { get; set; }
    }

    public class DivisionOptions
    {
        public string GithubUsername { get; set; }
        public string GithubRepository { get; set; }
        public string DivisionName { get; set; }
        public ContentDeliveryOptions ContentDelivery { get; set; }
    }

    public class ContentDeliveryOptions
    {
        public string Provider { get; set; }
        public string Domain { get; set; }
        public bool SupportsETag { get; set; }
    }
} 