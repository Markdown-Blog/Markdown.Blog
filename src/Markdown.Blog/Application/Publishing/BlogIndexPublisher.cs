//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Markdown.Blog.Domain.Models;
//using Markdown.Blog.Domain.Services;
//using Markdown.Blog.Infrastructure.Compression;
//using Markdown.Blog.Infrastructure.Serialization;
//using Markdown.Blog.Infrastructure.Storage;

//namespace Markdown.Blog.Application.Publishing
//{
//    /// <summary>
//    /// Use case service that orchestrates BlogIndex generation, serialization, compression, and storage.
//    /// </summary>
//    public class BlogIndexPublisher
//    {
//        private readonly IBlogIndexFactory _factory;
//        private readonly IBlogIndexSerializer _serializer;
//        private readonly ICompressor _compressor;
//        private readonly IBlogIndexStorage _storage;

//        public BlogIndexPublisher(
//            IBlogIndexFactory factory,
//            IBlogIndexSerializer serializer,
//            ICompressor compressor,
//            IBlogIndexStorage storage)
//        {
//            _factory = factory;
//            _serializer = serializer;
//            _compressor = compressor;
//            _storage = storage;
//        }

//        /// <summary>
//        /// Updates the BlogIndex artifacts at the given division directory.
//        /// </summary>
//        /// <param name="divisionDirectory">Target directory to persist index artifacts.</param>
//        /// <param name="blogMetadataList">Blog metadata entries.</param>
//        /// <param name="currentVersion">Optional current version (if caller already read it).</param>
//        public async Task<BlogIndexUpdateResult> UpdateIndexAsync(
//            string divisionDirectory,
//            List<BlogMetadata> blogMetadataList,
//            int? currentVersion = null)
//        {
//            var version = currentVersion ?? await _storage.GetCurrentVersionAsync(divisionDirectory);
//            var newVersion = version + 1;

//            var index = _factory.Create(newVersion, blogMetadataList, DateTime.UtcNow);

//            var json = _serializer.Serialize(index);
//            var binary = _compressor.Compress(json);

//            await _storage.SaveIndexAsync(divisionDirectory, json, binary, newVersion);

//            return new BlogIndexUpdateResult
//            {
//                UncompressedSize = json.Length,
//                CompressedSize = binary.Length,
//                Version = newVersion
//            };
//        }
//    }
//}