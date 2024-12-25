using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Markdown.Blog.Procedures
{
	public static class BlogImageProcessor
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
			// filename ends with !md is named mdFileNameShadow
			string mdFileNameShadow = mdFileName + "!md";

			// Construct directory path with !md
			string mdFilePathModified = Path.Combine(mdFileDirectory, mdFileNameShadow);

			// Construct assets path
			string[] pathComponents = new[] { "assets", assetId, imageFileName };

			// Return different path format based on pathType
			if (pathType == Markdown.Blog.BlogImage.ImagePathType.RelativeToMarkdown)
			{
				return Path.Combine(mdFileNameShadow, Path.Combine(pathComponents));
			}
			else // Absolute
			{
				return Path.Combine(mdFilePathModified, Path.Combine(pathComponents));
			}
		}

		/*暂时取消以下逻辑
		/// <summary>
		/// Constructs the path for the cover image, fixed as path/to/markdown/file!md/assets/cover.jpg
		/// </summary>
		/// <param name="mdFilePath">Path to the Markdown file</param>
		/// <returns>Path to the cover image</returns>
		public static string ConstructCoverPath(string mdFilePath)
		{
			if (!mdFilePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException("File path must end with .md", nameof(mdFilePath));

			// Get md file directory and name
			string mdFileDirectory = Path.GetDirectoryName(mdFilePath) ?? string.Empty;
			string mdFileName = Path.GetFileNameWithoutExtension(mdFilePath);

			// Construct directory path with !md
			string mdFilePathModified = Path.Combine(mdFileDirectory, mdFileName + "!md");

			// Construct cover image path
			return Path.Combine(mdFilePathModified, "assets", "cover.jpg");
		}
		*/

		/// <summary>
		/// Parses all blog image references from Markdown content.
		/// Looks for image references in the format: ![alt](./assets/{assetId}/{imageFileName} "title")
		/// </summary>
		/// <param name="mdFilePath">Path to the Markdown file being processed</param>
		/// <param name="markdownContent">Raw Markdown content to parse</param>
		/// <returns>List of BlogImage objects representing all valid image references found in the content</returns>
		/// <remarks>
		/// The method:
		/// 1. Uses regex to find all image references matching the pattern
		/// 2. Extracts alt text, asset ID, filename, and optional title
		/// 3. Creates BlogImage objects for valid references
		/// 4. Silently skips invalid references (e.g., missing required parameters)
		/// </remarks>
		/// <example>
		/// Valid image reference formats:
		/// ![Alt text](./assets/123456/image.png)
		/// ![Alt text](./assets/123456/image.jpg "Optional title")
		/// </example>
		public static List<Markdown.Blog.BlogImage> ParseBlogImagesFromMarkdown(string mdFilePath, string markdownContent)
		{
			var blogImages = new List<Markdown.Blog.BlogImage>();

			// Regular expression pattern to match image references in format:
			// ![alt](./assets/{assetId}/{imageFileName} "title")
			// Groups captured:
			// 1: Alt text
			// 2: Asset ID
			// 3: Image filename
			// 4: Optional title (if present)
			var imagePattern = @"!\[(.*?)\]\(\.\/assets\/([^\/]+)\/([^\/\)\s]+)(?:\s+""([^""]*)"")?\)";
			var matches = Regex.Matches(markdownContent, imagePattern);

			foreach (Match match in matches)
			{
				// Ensure we have at least the required groups (alt, assetId, imageFileName)
				if (match.Groups.Count >= 4)
				{
					// Extract components from the match
					string alt = match.Groups[1].Value;
					string assetId = match.Groups[2].Value;
					string imageFileName = match.Groups[3].Value;
					// Title is optional, use empty string if not present
					string title = match.Groups.Count > 4 ? match.Groups[4].Value : "";

					try
					{
						// Attempt to create and add a new BlogImage
						// Constructor will validate the parameters
						var blogImage = new Markdown.Blog.BlogImage(
							mdFilePath,
							assetId,
							imageFileName,
							title,
							alt
						);
						blogImages.Add(blogImage);
					}
					catch (ArgumentException)
					{
						// Skip invalid image references
						// This can happen if:
						// - Asset ID is empty
						// - Image filename has no extension
						// - Markdown file path is invalid
						continue;
					}
				}
			}

			return blogImages;
		}
	}

}

