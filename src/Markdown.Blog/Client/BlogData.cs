using System;
using System.Threading.Tasks;
using Markdown.Blog.Shared.Models;

namespace Markdown.Blog.Client
{
    /// <summary>
    /// Represents a blog post with its content and metadata.
    /// Extends BlogMetadata to include the actual content of the blog post.
    /// This class is primarily used on the client side.
    /// </summary>
    public class BlogData : BlogMetadata
    {
        /// <summary>
        /// Gets or sets the division or category of the blog post.
        /// </summary>
        public Division Division { get; set; }

        /// <summary>
        /// Gets or sets the raw Markdown content of the blog post.
        /// </summary>
        public string MdContent { get; set; }

        /// <summary>
        /// Gets a value indicating whether the markdown content has been loaded.
        /// </summary>
        public bool IsMdContentLoaded => MdContent != null;

        /// <summary>
        /// Loads the markdown content from the remote source and assigns it to MdContent.
        /// </summary>
        /// <param name="division">The division containing repository information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<bool> LoadMdContent(Division division)
        {
            MdContent = await BlogContentLoader.GetContentAsync(division, this);
            return IsMdContentLoaded;
        }

        public BlogData(Division division, BlogMetadata metadata) : base()
        {
            // Copy all properties from metadata to this instance
            Division = division;
            FilePath = metadata.FilePath;
            Title = metadata.Title;
            Description = metadata.Description;
            Date = metadata.Date;
            Tags = metadata.Tags;
            CoverImages = metadata.CoverImages;
            Hierarchy = metadata.Hierarchy;
            PathSegments = metadata.PathSegments;
            MdContent = null;
        }
    }
}
