namespace Markdown.Blog.Application.Publishing
{
    /// <summary>
    /// Result of publishing/updating the BlogIndex artifacts.
    /// </summary>
    public class BlogIndexUpdateResult
    {
        public int UncompressedSize { get; set; }
        public int CompressedSize { get; set; }
        public int Version { get; set; }
    }
}