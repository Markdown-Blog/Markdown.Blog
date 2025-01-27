using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using Newtonsoft.Json;
using Markdown.Blog.Shared;

namespace Markdown.Blog.Client
{
    /// <summary>
    /// Handles deserialization of blog indexes received from the server
    /// </summary>
    public static class BlogIndexDeserializer
    {
        /// <summary>
        /// Restores a BlogIndex object from its JSON representation.
        /// </summary>
        public static BlogIndex RestoreBlogIndexFromJson(string json)
        {
            return JsonConvert.DeserializeObject<BlogIndex>(json)
                ?? new BlogIndex { BlogMetadataList = new List<BlogMetadata>() };
        }

        /// <summary>
        /// Restores a BlogIndex object from its compressed binary representation.
        /// </summary>
        public static BlogIndex RestoreBlogIndexFromBinary(byte[] binary)
        {
            using (var memoryStream = new MemoryStream(binary))
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<BlogIndex>(json)
                    ?? new BlogIndex { BlogMetadataList = new List<BlogMetadata>() };
            }
        }
    }
} 