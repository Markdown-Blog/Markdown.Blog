using Markdown.Blog.Domain.Models;

namespace Markdown.Blog.Infrastructure.Serialization
{
    /// <summary>
    /// Serializer interface for BlogIndex.
    /// </summary>
    public interface IBlogIndexSerializer
    {
        /// <summary>
        /// Serialize BlogIndex to JSON string.
        /// </summary>
        string Serialize(BlogIndex index);

        /// <summary>
        /// Deserialize JSON string to BlogIndex.
        /// </summary>
        BlogIndex Deserialize(string json);
    }
}