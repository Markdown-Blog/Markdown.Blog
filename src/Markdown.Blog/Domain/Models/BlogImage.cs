using System;
using System.IO;

namespace Markdown.Blog.Domain.Models
{
    /// <summary>
    /// Represents a blog image in the domain model.
    /// This is a pure domain entity without infrastructure dependencies.
    /// </summary>
    public class BlogImage
    {
        /// <summary>
        /// Original markdown file path
        /// </summary>
        public string MarkdownFilePath { get; }

        /// <summary>
        /// Asset ID for the image folder
        /// </summary>
        public string AssetId { get; }

        /// <summary>
        /// Original image file name
        /// </summary>
        public string ImageFileName { get; }

        /// <summary>
        /// Image title for display purposes
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Alternative text for accessibility
        /// </summary>
        public string AltText { get; set; }

        /// <summary>
        /// Creates a new instance of BlogImage
        /// </summary>
        /// <param name="mdFilePath">Markdown file path, must end with .md</param>
        /// <param name="assetId">Asset ID for the image folder</param>
        /// <param name="imageFileName">Original image filename</param>
        /// <param name="title">Optional title for the image</param>
        /// <param name="altText">Optional alternative text for accessibility</param>
        /// <exception cref="ArgumentException">
        /// Thrown when mdFilePath doesn't end with .md, or
        /// assetId is empty, or
        /// imageFileName has no extension
        /// </exception>
        public BlogImage(string mdFilePath, string assetId, string imageFileName, string title = null!, string altText = null!)
        {
            // Validate parameters
            if (!mdFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("File path must end with .md", nameof(mdFilePath));

            if (string.IsNullOrEmpty(assetId))
                throw new ArgumentException("assetId cannot be empty", nameof(assetId));

            if (string.IsNullOrEmpty(Path.GetExtension(imageFileName)))
                throw new ArgumentException("imageFileName must include file extension", nameof(imageFileName));

            // Set properties
            MarkdownFilePath = mdFilePath;
            AssetId = assetId;
            ImageFileName = imageFileName;
            Title = title;
            AltText = altText;
        }
    }
}