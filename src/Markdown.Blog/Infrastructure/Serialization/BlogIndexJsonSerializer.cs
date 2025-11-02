using Markdown.Blog.Domain.Models;
using Newtonsoft.Json;

namespace Markdown.Blog.Infrastructure.Serialization
{
    /// <summary>
    /// JSON serializer for BlogIndex using Newtonsoft.Json.
    /// </summary>
    public class BlogIndexJsonSerializer : IBlogIndexSerializer
    {
        public string Serialize(BlogIndex index)
        {
            return JsonConvert.SerializeObject(index);
        }

        public BlogIndex Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<BlogIndex>(json);
        }
    }
}