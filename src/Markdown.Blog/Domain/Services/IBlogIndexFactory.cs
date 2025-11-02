using System;
using System.Collections.Generic;
using Markdown.Blog.Domain.Models;

namespace Markdown.Blog.Domain.Services
{
    /// <summary>
    /// Factory interface for constructing BlogIndex aggregates.
    /// </summary>
    public interface IBlogIndexFactory
    {
        /// <summary>
        /// Creates a BlogIndex aggregate with the provided version, metadata list and timestamp.
        /// </summary>
        /// <param name="id">Index version number.</param>
        /// <param name="blogMetadataList">List of blog metadata entries.</param>
        /// <param name="utcNow">Timestamp in UTC when the index is generated.</param>
        /// <returns>A constructed BlogIndex aggregate.</returns>
        BlogIndex Create(int id, List<BlogMetadata> blogMetadataList, DateTime utcNow);
    }
}