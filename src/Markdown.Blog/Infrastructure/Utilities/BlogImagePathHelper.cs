using System;
using System.IO;

namespace Markdown.Blog.Infrastructure.Utilities
{
    /// <summary>
    /// Provides utility methods for handling blog image paths.
    /// This is a shared helper used by both server and client.
    /// </summary>
    public static class BlogImagePathHelper
    {
        /// <summary>
        /// Constructs the save path for blog images
        /// </summary>
        /// <param name="mdFilePath">Markdown file path, must end with .md</param>
        /// <param name="assetId">Asset ID, used to generate unique image folder</param>
        /// <param name="imageFileName">Original image filename</param>
        /// <param name="pathType">Type of path to return (Absolute or RelativeToMarkdown)</param>
        /// <returns>Constructed image save path</returns>
        public static string ConstructImagePath(string mdFilePath, string assetId, string imageFileName, ImagePathType pathType)
        {
            if (!mdFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("File path must end with .md", nameof(mdFilePath));

            if (string.IsNullOrEmpty(assetId))
                throw new ArgumentException("assetId cannot be empty", nameof(assetId));

            if (string.IsNullOrEmpty(Path.GetExtension(imageFileName)))
                throw new ArgumentException("imageFileName must include file extension", nameof(imageFileName));

            string mdFileDirectory = Path.GetDirectoryName(mdFilePath) ?? string.Empty;
            string mdFileName = Path.GetFileNameWithoutExtension(mdFilePath);
            string mdFileNameShadow = mdFileName + "!md";
            string mdFilePathModified = Path.Combine(mdFileDirectory, mdFileNameShadow);
            string[] pathComponents = new[] { "assets", assetId, imageFileName };

            return pathType == ImagePathType.RelativeToMarkdown
                ? Path.Combine(mdFileNameShadow, Path.Combine(pathComponents))
                : Path.Combine(mdFilePathModified, Path.Combine(pathComponents));
        }
    }

    /// <summary>
    /// Defines the type of path to be returned for image paths
    /// </summary>
    public enum ImagePathType
    {
        /// <summary>
        /// Complete path (path/to/markdown!md/assets/12345/image.png)
        /// </summary>
        Absolute,

        /// <summary>
        /// Path relative to Markdown file (./assets/12345/image.png)
        /// </summary>
        RelativeToMarkdown
    }
}