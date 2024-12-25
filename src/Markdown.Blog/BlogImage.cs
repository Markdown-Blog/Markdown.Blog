using System;
using System.IO;

namespace Markdown.Blog
{
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

		public string Title { get; set; }

		public string AltText { get; set; }

		/// <summary>
		/// Creates a new instance of BlogImage
		/// </summary>
		/// <param name="mdFilePath">Markdown file path, must end with .md</param>
		/// <param name="assetId">Asset ID for the image folder</param>
		/// <param name="imageFileName">Original image filename</param>
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

		/// <summary>
		/// Path type for blog images
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

		/// <summary>
		/// 获取图片的绝对路径
		/// </summary>
		/// <returns>完整的图片路径</returns>
		public string GetAbsolutePath()
		{
			return Procedures.BlogImageProcessor.ConstructImagePath(MarkdownFilePath, AssetId, ImageFileName, ImagePathType.Absolute);
		}

		/// <summary>
		/// 获取相对于 Markdown 文件的图片路径
		/// </summary>
		/// <returns>相对路径</returns>
		public string GetRelativePath()
		{
			return Procedures.BlogImageProcessor.ConstructImagePath(MarkdownFilePath, AssetId, ImageFileName, ImagePathType.RelativeToMarkdown);
		}

	}
}