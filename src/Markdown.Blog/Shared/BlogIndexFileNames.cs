using System;

namespace Markdown.Blog.Shared
{
    /// <summary>
    /// Defines standard file names for blog index files
    /// </summary>
    public static class BlogIndexFileNames
    {
        /// <summary>
        /// Uncompressed JSON format of the full index
        /// </summary>
        public const string Json = "index.json";

        /// <summary>
        /// Compressed binary format of the full index
        /// </summary>
        public const string CompressedJson = "index.json.gz";

        /// <summary>
        /// Plain text file containing the current version number
        /// </summary>
        public const string Version = "index.version";
    }
} 