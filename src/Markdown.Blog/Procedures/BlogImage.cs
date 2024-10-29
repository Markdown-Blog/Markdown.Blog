using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Markdown.Blog.Procedures
{
	public class BlogImage
	{

		/// <summary>
		/// Constructs the save path for blog images
		/// </summary>
		/// <param name="mdFilePath">Markdown file path, must end with .md, for example:
		/// - blogs/post1.md
		/// - docs/folder/article.md
		/// - path/to/markdown/file.md</param>
		/// <param name="assetId">Asset ID, used to generate unique image folder</param>
		/// <param name="imageFileName">Original image filename, typically obtained from GitHub user attachment URL, format:
		/// - 123456-uuid.png
		/// - 789012-uuid.jpg</param>
		/// <param name="pathType">Type of path to return:
		/// - Absolute: Complete path (path/to/markdown!md/assets/12345/image.png)
		/// - RelativeToMarkdown: Path relative to Markdown file (./assets/12345/image.png)</param>
		/// <returns>Constructed image save path</returns>
		/// <exception cref="ArgumentException">
		/// Thrown when mdFilePath doesn't end with .md, or
		/// assetId is empty, or
		/// imageFileName has no extension
		/// </exception>
		/// <example>
		/// Usage example:
		/// <code>
		/// string mdFilePath = "path/to/markdown/file.md";
		/// string assetId = "12345";
		/// string imageFileName = "67890-abcd-efgh-ijkl.png";
		///
		/// // Get relative path
		/// string relativePath = ImagePathHelper.ConstructImagePath(
		///     mdFilePath,
		///     assetId,
		///     imageFileName,
		///     ImagePathType.RelativeToMarkdown
		/// );
		/// // Output: ./assets/12345/67890-abcd-efgh-ijkl.png
		///
		/// // Get absolute path
		/// string absolutePath = ImagePathHelper.ConstructImagePath(
		///     mdFilePath,
		///     assetId,
		///     imageFileName,
		///     ImagePathType.Absolute
		/// );
		/// // Output: path/to/markdown/file!md/assets/12345/67890-abcd-efgh-ijkl.png
		/// </code>
		/// </example>
		public static string ConstructImagePath(string mdFilePath, string assetId, string imageFileName, Markdown.Blog.BlogImage.ImagePathType pathType)
		{
			if (!mdFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException("File path must end with .md", nameof(mdFilePath));

			if (string.IsNullOrEmpty(assetId))
				throw new ArgumentException("assetId cannot be empty", nameof(assetId));

			if (string.IsNullOrEmpty(Path.GetExtension(imageFileName)))
				throw new ArgumentException("imageFileName must include file extension", nameof(imageFileName));

			// Get md file directory and name
			string mdFileDirectory = Path.GetDirectoryName(mdFilePath) ?? string.Empty;
			string mdFileName = Path.GetFileNameWithoutExtension(mdFilePath);

			// Construct directory path with !md
			string mdFilePathModified = Path.Combine(mdFileDirectory, mdFileName + "!md");

			// Construct assets path
			string[] pathComponents = new[] { "assets", assetId, imageFileName };

			// Return different path format based on pathType
			if (pathType == Markdown.Blog.BlogImage.ImagePathType.RelativeToMarkdown)
			{
				return Path.Combine(".", Path.Combine(pathComponents));
			}
			else // Absolute
			{
				return Path.Combine(mdFilePathModified, Path.Combine(pathComponents));
			}
		}

	}
}
