using System;
using System.Collections.Generic;
using Markdown.Blog.Domain.Models;

namespace Markdown.Blog.Domain.Services
{
    /// <summary>
    /// Default implementation of IBlogIndexFactory to create BlogIndex aggregates.
    /// </summary>
    public class BlogIndexFactory : IBlogIndexFactory
    {
        public BlogIndex Create(int id, List<BlogMetadata> blogMetadataList, DateTime utcNow)
        {
            return new BlogIndex
            {
                Id = id,
                DateTime = utcNow,
                BlogMetadataList = blogMetadataList
            };
        }
    }
}