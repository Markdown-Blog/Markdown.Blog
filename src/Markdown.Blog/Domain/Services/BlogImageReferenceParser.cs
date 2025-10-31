using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Markdown.Blog.Domain.Models;

namespace Markdown.Blog.Domain.Services
{
    /// <summary>
    /// Parses blog image references from Markdown content and produces domain BlogImage models.
    /// Host-agnostic pure logic, usable by client and server.
    /// </summary>
    public static class BlogImageReferenceParser
    {
        /// <summary>
        /// Parses all blog image references from Markdown content.
        /// Looks for image references in the format: ![alt](./assets/{assetId}/{imageFileName} "title")
        /// </summary>
        /// <param name="mdFilePath">Path to the Markdown file being processed</param>
        /// <param name="markdownContent">Raw Markdown content to parse</param>
        /// <returns>List of BlogImage objects representing all valid image references found in the content</returns>
        public static List<BlogImage> ParseBlogImagesFromMarkdown(string mdFilePath, string markdownContent)
        {
            var blogImages = new List<BlogImage>();

            // Regular expression pattern to match image references in format:
            // ![alt](./assets/{assetId}/{imageFileName} "title")
            var imagePattern = @"!\[(.*?)\]\(\./assets/([^/]+)/([^/\)\s]+)(?:\s+""([^""]*)"")?\)";
            var matches = Regex.Matches(markdownContent, imagePattern);

            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 4)
                {
                    string alt = match.Groups[1].Value;
                    string assetId = match.Groups[2].Value;
                    string imageFileName = match.Groups[3].Value;
                    string title = match.Groups.Count > 4 ? match.Groups[4].Value : "";

                    try
                    {
                        var blogImage = new BlogImage(
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
                        continue;
                    }
                }
            }

            return blogImages;
        }
    }
}