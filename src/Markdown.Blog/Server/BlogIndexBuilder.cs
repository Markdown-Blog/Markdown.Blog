using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Markdown.Blog.Shared.Models;
using Markdown.Blog.Infrastructure.Utilities;
using Markdown.Blog.Shared.Constants;

namespace Markdown.Blog.Server
{
    /// <summary>
    /// Server-side functionality for building and updating blog indexes
    /// </summary>
    public static class BlogIndexBuilder
    {
        /// <summary>
        /// Builds a blog index with JSON and binary outputs.
        /// </summary>
        public static void BuildBlogIndex(int id, List<BlogMetadata> blogMetadataList, out string json, out byte[] binary)
        {
            var blogIndex = new BlogIndex
            {
                Id = id,
                DateTime = DateTime.UtcNow,
                BlogMetadataList = blogMetadataList
            };

            // Serialize the entire BlogIndex object to JSON
            json = JsonConvert.SerializeObject(blogIndex, Newtonsoft.Json.Formatting.None);

            // Compress JSON string to binary data
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                using (var writer = new StreamWriter(gzipStream))
                {
                    writer.Write(json);
                }
                binary = memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Updates the blog index files in the division directory.
        /// </summary>
        public static async Task<BlogIndexUpdateResult> UpdateIndexAsync(
            string divisionDirectory,
            List<BlogMetadata> blogMetadataList,
            int? currentVersion = null)
        {
            var result = new BlogIndexUpdateResult();

            // Get current version if not provided
            int version = currentVersion ?? await GetCurrentVersionAsync(divisionDirectory);
            int newVersion = version + 1;

            // Build new index
            BuildBlogIndex(newVersion, blogMetadataList, out string json, out byte[] binary);

            // Save index files
            await File.WriteAllTextAsync(
                Path.Combine(divisionDirectory, GetBlogIndexFileNameUncompressed()),
                json);
            await File.WriteAllBytesAsync(
                Path.Combine(divisionDirectory, GetBlogIndexFileNameCompressed()),
                binary);

            // Write version file
            await File.WriteAllTextAsync(
                Path.Combine(divisionDirectory, GetBlogIndexFileNameVersion()),
                newVersion.ToString());

            // Calculate compression stats
            result.UncompressedSize = json.Length;
            result.CompressedSize = binary.Length;
            result.Version = newVersion;

            return result;
        }

        private static async Task<int> GetCurrentVersionAsync(string divisionDirectory)
        {
            var versionFile = Path.Combine(divisionDirectory, GetBlogIndexFileNameVersion());
            if (!File.Exists(versionFile))
                return 0;

            string versionStr = await File.ReadAllTextAsync(versionFile);
            return int.TryParse(versionStr, out int version) ? version : 0;
        }

        public static string GetBlogIndexFileNameUncompressed() => BlogIndexFileNames.Json;
        public static string GetBlogIndexFileNameCompressed() => BlogIndexFileNames.CompressedJson;
        public static string GetBlogIndexFileNameVersion() => BlogIndexFileNames.Version;
    }

    public class BlogIndexUpdateResult
    {
        public int UncompressedSize { get; set; }
        public int CompressedSize { get; set; }
        public int Version { get; set; }
    }
}