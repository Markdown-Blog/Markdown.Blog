//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using Markdown.Blog.Domain.Models;
//using Markdown.Blog.Domain.Services;
//using Markdown.Blog.Infrastructure.Serialization;
//using Markdown.Blog.Infrastructure.Compression;
//using Markdown.Blog.Infrastructure.Storage;
//using Markdown.Blog.Application.Publishing;

//namespace Markdown.Blog.Server
//{
//    /// <summary>
//    /// Server-side functionality for building and updating blog indexes
//    /// </summary>
//    public static class BlogIndexBuilder
//    {
//        /// <summary>
//        /// Builds a blog index with JSON and binary outputs.
//        /// </summary>
//        public static void BuildBlogIndex(int id, List<BlogMetadata> blogMetadataList, out string json, out byte[] binary)
//        {
//            // Domain: construct aggregate
//            IBlogIndexFactory factory = new BlogIndexFactory();
//            var index = factory.Create(id, blogMetadataList, DateTime.UtcNow);

//            // Infrastructure: serialize & compress
//            IBlogIndexSerializer serializer = new BlogIndexJsonSerializer();
//            ICompressor compressor = new GzipCompressor();

//            json = serializer.Serialize(index);
//            binary = compressor.Compress(json);
//        }

//        /// <summary>
//        /// Updates the blog index files in the division directory.
//        /// </summary>
//        public static async Task<BlogIndexUpdateResult> UpdateIndexAsync(
//            string divisionDirectory,
//            List<BlogMetadata> blogMetadataList,
//            int? currentVersion = null)
//        {
//            // Application: orchestrate end-to-end update using adapters
//            var publisher = new BlogIndexPublisher(
//                new BlogIndexFactory(),
//                new BlogIndexJsonSerializer(),
//                new GzipCompressor(),
//                new FileSystemBlogIndexStorage());

//            return await publisher.UpdateIndexAsync(divisionDirectory, blogMetadataList, currentVersion);
//        }

//        // File naming and version handling are moved to infrastructure storage.
//    }
//}