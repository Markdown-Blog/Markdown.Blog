using System;
using System.Text.RegularExpressions;
using Markdig;
using System.IO;

namespace Markdown.Blog.Procedures
{
	public class BlogContent
	{
		/// <summary>
		/// Convert Markdown content to HTML with GitHub image path resolution
		/// </summary>
		/// <param name="division">Division containing GitHub repository information</param>
		/// <param name="mdFilePath">Relative path of the markdown file in the repository</param>
		/// <param name="mdContent">Markdown formatted content</param>
		/// <returns>Converted HTML content with resolved image paths</returns>
		public static string ConvertToHtml(Division division, string mdFilePath, string mdContent)
		{
			// Remove YAML front matter and cover image section
			string contentOnly = ExtractContentOnly(mdContent);

			// Replace relative image paths with GitHub URLs
			string contentWithResolvedPaths = ResolveImagePaths(contentOnly, division, mdFilePath);

			// Configure Markdig pipeline with advanced extensions
			var pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.Build();

			// Convert Markdown to HTML
			return Markdig.Markdown.ToHtml(contentWithResolvedPaths, pipeline);
		}

		/// <summary>
		/// Replace relative image paths with GitHub raw content URLs
		/// </summary>
		private static string ResolveImagePaths(string content, Division division, string mdFilePath)
		{
			// Base URL for GitHub raw content
			string baseUrl = $"https://raw.githubusercontent.com/{division.GithubUsername}/{division.GithubRepository}/main";

			// Get the directory path of the markdown file
			string mdDirectory = Path.GetDirectoryName(mdFilePath)?.Replace('\\', '/') ?? "";

			// Replace image paths
			return Regex.Replace(
				content,
				@"!\[(.*?)\]\(((?!http[s]?://|/).*?)\)",
				match =>
				{
					string altText = match.Groups[1].Value;
					string imagePath = match.Groups[2].Value;

					// Combine the markdown directory path with the image path
					string fullPath = Path.Combine(mdDirectory, imagePath)
						.Replace('\\', '/');

					// Create the complete GitHub URL
					string githubUrl = $"{baseUrl}/{fullPath}";

					return $"![{altText}]({githubUrl})";
				}
			);
		}

		/// <summary>
		/// Extract pure content from Markdown, removing YAML front matter and cover image
		/// </summary>
		private static string ExtractContentOnly(string mdContent)
		{
			// Try to match full format (YAML + cover image + content)
			var fullMatch = Regex.Match(mdContent,
				@"---\s*\n.*?\n---\s*\n.*?\n---\s*\n(.*)",
				RegexOptions.Singleline);

			if (fullMatch.Success)
			{
				// Return content after the third ---
				return fullMatch.Groups[1].Value.Trim();
			}

			// If not full format, try simple format (YAML + content)
			var simpleMatch = Regex.Match(mdContent,
				@"---\s*\n.*?\n---\s*\n(.*)",
				RegexOptions.Singleline);

			if (simpleMatch.Success)
			{
				// Return content after the second ---
				return simpleMatch.Groups[1].Value.Trim();
			}

			// If no YAML front matter, return original content
			return mdContent.Trim();
		}
	}
}
