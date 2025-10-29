using Markdown.Blog.Shared.Models;

namespace Markdown.Blog.Domain.Services
{
    /// <summary>
    /// Domain service interface for constructing blog image paths.
    /// This interface defines the contract for path construction logic
    /// while keeping the domain layer free from infrastructure dependencies.
    /// </summary>
    public interface IBlogImagePathService
    {
        /// <summary>
        /// Constructs the absolute path for a blog image
        /// </summary>
        /// <param name="blogImage">The blog image domain entity</param>
        /// <returns>The absolute path to the image file</returns>
        string GetAbsolutePath(BlogImage blogImage);

        /// <summary>
        /// Constructs the relative path for a blog image (relative to the markdown file)
        /// </summary>
        /// <param name="blogImage">The blog image domain entity</param>
        /// <returns>The relative path to the image file</returns>
        string GetRelativePath(BlogImage blogImage);

        /// <summary>
        /// Constructs an image path based on the specified path type
        /// </summary>
        /// <param name="markdownFilePath">The markdown file path</param>
        /// <param name="assetId">The asset ID for the image folder</param>
        /// <param name="imageFileName">The image file name</param>
        /// <param name="pathType">The type of path to construct</param>
        /// <returns>The constructed image path</returns>
        string ConstructImagePath(string markdownFilePath, string assetId, string imageFileName, ImagePathType pathType);
    }

    /// <summary>
    /// Enumeration defining the types of image paths that can be constructed
    /// </summary>
    public enum ImagePathType
    {
        /// <summary>
        /// Absolute path to the image file
        /// </summary>
        Absolute,

        /// <summary>
        /// Path relative to the markdown file
        /// </summary>
        RelativeToMarkdown
    }
}