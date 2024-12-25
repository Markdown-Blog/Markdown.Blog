using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace Markdown.Blog.Procedures
{
	public static class BlogMetadataYamlProcessor
	{
		/// <summary>
		/// Extracts YAML content from a markdown string.
		/// </summary>
		/// <param name="mdContent">The markdown content containing YAML front matter.</param>
		/// <returns>The extracted YAML content as a string, or an empty string if no YAML content is found.</returns>
		public static string ExtractYaml(string mdContent)
		{
			// Use regex to match YAML content between --- delimiters
			var match = Regex.Match(mdContent, @"^---\s*\n(.*?)\n---", RegexOptions.Singleline);
			return match.Success ? match.Groups[1].Value : string.Empty;
		}

		/// <summary>
		/// Extracts and deserializes blog metadata from markdown content.
		/// </summary>
		/// <param name="mdContent">The markdown content containing YAML front matter with blog metadata.</param>
		/// <param name="relativePath">The relative path to the markdown file.</param>
		/// <returns>A <see cref="BlogMetadata"/> object containing the deserialized metadata, or null if extraction fails.</returns>
		public static BlogMetadata ExtractBlogMetadata(string relativePath, string mdContent)
		{
			BlogMetadata result = null!;

			// First try to match the full format (YAML + cover image + content)
			var fullMatch = Regex.Match(mdContent,
				@"---\s*\n(.*?)\n---\s*\n(.*?)\n---\s*\n(.*)",
				RegexOptions.Singleline);

			// If full format not found, try simple format (YAML + content)
			var simpleMatch = Regex.Match(mdContent,
				@"---\s*\n(.*?)\n---\s*\n(.*)",
				RegexOptions.Singleline);

			// Determine which format was matched and extract content accordingly
			string yamlContent;
			string coverSection = string.Empty;

			if (fullMatch.Success)
			{
				// Full format with cover image section
				yamlContent = fullMatch.Groups[1].Value;
				coverSection = fullMatch.Groups[2].Value;
			}
			else if (simpleMatch.Success)
			{
				// Simple format without cover image
				yamlContent = simpleMatch.Groups[1].Value;
			}
			else
			{
				return result; // Return null if no valid format found
			}

			if (!string.IsNullOrEmpty(yamlContent))
			{
				var deserializer = new DeserializerBuilder()
					.WithNamingConvention(CamelCaseNamingConvention.Instance)
					.Build();

				result = deserializer.Deserialize<BlogMetadata>(yamlContent);

				// Set file path
				result.FilePath = relativePath;

				// Handle cover image with priority rules:
				// 1. If cover section exists and contains an image, use that image URL
				// 2. Otherwise, keep the coverImage value from YAML if it exists
				if (!string.IsNullOrEmpty(coverSection))
				{
					// Match all image references in the cover section
					// Only matches lines with valid markdown image syntax (![...](...)format)
					// Automatically ignores empty lines and non-image tag lines
					var imageMatches = Regex.Matches(coverSection.Trim(), @"!\[.*?\]\((.*?)\)");
					if (imageMatches.Count > 0)
					{
						// Initialize the CoverImages list if null
						result.CoverImages = result.CoverImages ?? new List<string>();

						// Add all found image paths to the list
						foreach (Match imageMatch in imageMatches)
						{
							result.CoverImages.Add(imageMatch.Groups[1].Value);
						}
					}
				}
				// Note: If no images in cover section, CoverImages will remain as initialized by YAML deserializer

				// Extract hierarchy information from file path
				var pathParts = relativePath.Split(Path.DirectorySeparatorChar);
				if (pathParts.Length >= 3)
				{
					result.Hierarchy = new BlogHierarchy
					{
						Division = pathParts[^4],
						Category = pathParts[^3],
						SubCategory = pathParts[^2]
					};
				}
				else
				{
					throw new InvalidOperationException($"Invalid file path structure: {relativePath}");
				}
			}

			return result;
		}
	}
}
