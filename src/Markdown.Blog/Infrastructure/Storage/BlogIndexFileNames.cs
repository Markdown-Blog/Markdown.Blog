namespace Markdown.Blog.Infrastructure.Storage
{
    /// <summary>
    /// File names used by BlogIndex storage.
    /// </summary>
    public static class BlogIndexFileNames
    {
        public const string Json = "index.json";
        public const string CompressedJson = "index.json.gz";
        public const string Version = "index.version";
        public const string Diff = "index.{0}.diff.json";
    }
}