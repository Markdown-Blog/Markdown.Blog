using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using Markdown.Blog.Shared;
using System.Text;

namespace Markdown.Blog.Client
{
    /// <summary>
    /// Handles the rendering and processing of blog content for client-side display
    /// </summary>
    public static class BlogContentRenderer
    {
        private static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        /// <summary>
        /// Converts markdown content to HTML
        /// </summary>
        public static string ConvertToHtml(string markdownContent)
        {
            if (string.IsNullOrEmpty(markdownContent))
                return string.Empty;

            return Markdig.Markdown.ToHtml(markdownContent, pipeline);
        }

        /// <summary>
        /// Converts markdown content to HTML with processed image paths
        /// </summary>
        public static string RenderContent(BlogData blogData)
        {
            if (!blogData.IsMdContentLoaded)
                return string.Empty;

            string content = blogData.MdContent;
            content = ProcessImagePaths(content, blogData);
            return ConvertToHtml(content);
        }

        /// <summary>
        /// Renders a preview of the blog content
        /// </summary>
        public static string RenderPreview(BlogData blogData, int maxLength = 200)
        {
            if (!blogData.IsMdContentLoaded)
                return string.Empty;

            string preview = ExtractPreview(blogData.MdContent, maxLength);
            return ConvertToHtml(preview);
        }

        /// <summary>
        /// Processes image paths in markdown content to make them absolute
        /// </summary>
        private static string ProcessImagePaths(string content, BlogData blogData)
        {
            // Replace relative image paths with absolute paths
            // Pattern: ![alt](./assets/assetId/filename)
            return Regex.Replace(
                content,
                @"!\[(.*?)\]\(\.\/assets\/([^\/]+)\/([^\/\)\s]+)\)",
                match =>
                {
                    string alt = match.Groups[1].Value;
                    string assetId = match.Groups[2].Value;
                    string filename = match.Groups[3].Value;

                    var absolutePath = BlogImagePathHelper.ConstructImagePath(
                        blogData.FilePath,
                        assetId,
                        filename,
                        ImagePathType.Absolute
                    );

                    return $"![{alt}]({absolutePath})";
                }
            );
        }

        /// <summary>
        /// Extracts the first paragraph of content as a preview
        /// </summary>
        public static string ExtractPreview(string content, int maxLength = 200)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            // Find first paragraph
            var match = Regex.Match(content, @"^(.*?)\n\n", RegexOptions.Singleline);
            string preview = match.Success ? match.Groups[1].Value : content;

            // Remove Markdown syntax
            preview = Regex.Replace(preview, @"\[([^\]]*)\]\([^\)]*\)", "$1"); // Links
            preview = Regex.Replace(preview, @"[*_~`]", ""); // Basic formatting
            preview = Regex.Replace(preview, @"^\s*[#>-]\s*", ""); // Headers, quotes, lists

            // Truncate if needed
            if (preview.Length > maxLength)
            {
                preview = preview.Substring(0, maxLength - 3) + "...";
            }

            return preview.Trim();
        }

        /// <summary>
        /// Extracts all headings from the markdown content
        /// </summary>
        public static List<(int Level, string Text)> ExtractHeadings(string content)
        {
            var headings = new List<(int Level, string Text)>();
            var matches = Regex.Matches(content, @"^(#{1,6})\s+(.+)$", RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                int level = match.Groups[1].Length;
                string text = match.Groups[2].Value.Trim();
                headings.Add((level, text));
            }

            return headings;
        }

        /// <summary>
        /// Generates a table of contents from markdown headings
        /// </summary>
        public static string GenerateTableOfContents(string content)
        {
            var headings = ExtractHeadings(content);
            if (!headings.Any())
                return string.Empty;

            var toc = new StringBuilder();
            foreach (var (level, text) in headings)
            {
                var indent = new string(' ', (level - 1) * 2);
                var anchor = GenerateHeadingAnchor(text);
                toc.AppendLine($"{indent}- [{text}](#{anchor})");
            }

            return toc.ToString();
        }

        private static string GenerateHeadingAnchor(string text)
        {
            return Regex.Replace(text.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        }
    }
} 