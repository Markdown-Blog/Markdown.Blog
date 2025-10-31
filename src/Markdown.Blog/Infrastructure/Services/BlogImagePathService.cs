using System;
using System.IO;
using Markdown.Blog.Domain.Models;
using Markdown.Blog.Domain.Services;

namespace Markdown.Blog.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of the blog image path service.
    /// This service provides concrete path construction logic for blog images.
    /// </summary>
    public class BlogImagePathService : IBlogImagePathService
    {
        /// <summary>
        /// Constructs the absolute path for a blog image
        /// </summary>
        /// <param name="blogImage">The blog image domain entity</param>
        /// <returns>The absolute path to the image file</returns>
        public string GetAbsolutePath(BlogImage blogImage)
        {
            return ConstructImagePath(
                blogImage.MarkdownFilePath,
                blogImage.AssetId,
                blogImage.ImageFileName,
                ImagePathType.Absolute);
        }

        /// <summary>
        /// Constructs the relative path for a blog image (relative to the markdown file)
        /// </summary>
        /// <param name="blogImage">The blog image domain entity</param>
        /// <returns>The relative path to the image file</returns>
        public string GetRelativePath(BlogImage blogImage)
        {
            return ConstructImagePath(
                blogImage.MarkdownFilePath,
                blogImage.AssetId,
                blogImage.ImageFileName,
                ImagePathType.RelativeToMarkdown);
        }

        /// <summary>
        /// Constructs an image path based on the specified path type
        /// </summary>
        /// <param name="markdownFilePath">The markdown file path</param>
        /// <param name="assetId">The asset ID for the image folder</param>
        /// <param name="imageFileName">The image file name</param>
        /// <param name="pathType">The type of path to construct</param>
        /// <returns>The constructed image path</returns>
        public string ConstructImagePath(string markdownFilePath, string assetId, string imageFileName, ImagePathType pathType)
        {
            // Validate input parameters
            if (!markdownFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("File path must end with .md", nameof(markdownFilePath));

            if (string.IsNullOrEmpty(assetId))
                throw new ArgumentException("assetId cannot be empty", nameof(assetId));

            if (string.IsNullOrEmpty(Path.GetExtension(imageFileName)))
                throw new ArgumentException("imageFileName must include file extension", nameof(imageFileName));

            // Construct path components
            string mdFileDirectory = Path.GetDirectoryName(markdownFilePath) ?? string.Empty;
            string mdFileName = Path.GetFileNameWithoutExtension(markdownFilePath);
            string mdFileNameShadow = mdFileName + "!md";
            string mdFilePathModified = Path.Combine(mdFileDirectory, mdFileNameShadow);
            string[] pathComponents = new[] { "assets", assetId, imageFileName };

            // Return appropriate path based on type
            return pathType == ImagePathType.RelativeToMarkdown
                ? Path.Combine(mdFileNameShadow, Path.Combine(pathComponents))
                : Path.Combine(mdFilePathModified, Path.Combine(pathComponents));
        }
    }
}
