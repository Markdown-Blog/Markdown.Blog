namespace Markdown.Blog.Infrastructure.Storage
{
    /// <summary>
    /// File names used by BlogIndex storage.
    /// </summary>
    public static class BlogIndexFileNames
    {
		public const string Version = "index.version";

		public const string Full = "index.json";
        public const string FullCompressed = "index.json.gz";
        
        public const string Diff = "index.{0}.diff.json";
        public const string DiffCompressed = "index.{0}.diff.json.gz";
	}
}