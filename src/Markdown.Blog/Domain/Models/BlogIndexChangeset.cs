using System.Collections.Generic;

namespace Markdown.Blog.Domain.Models
{
    public class BlogIndexChangeset
    {
        public int FromVersion { get; set; }
        public int ToVersion { get; set; }
        public List<BlogMetadata> Added { get; set; } = new List<BlogMetadata>();
        public List<BlogMetadata> Updated { get; set; } = new List<BlogMetadata>();
        public List<string> Deleted { get; set; } = new List<string>();
    }
}