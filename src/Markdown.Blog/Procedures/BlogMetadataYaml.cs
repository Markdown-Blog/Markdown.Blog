using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Markdown.Blog.Procedures
{
	public class BlogMetadataYaml
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
		/// <returns>A <see cref="BlogMetadata"/> object containing the deserialized metadata, or null if extraction fails.</returns>
		public static BlogMetadata ExtractBlogMetadata(string mdContent)
		{
			BlogMetadata result = null!;

			// Extract YAML content from markdown
			string yamlContent = ExtractYaml(mdContent);

			if (!string.IsNullOrEmpty(yamlContent))
			{
				// Configure YAML deserializer with camel case naming convention
				var deserializer = new DeserializerBuilder()
					.WithNamingConvention(CamelCaseNamingConvention.Instance)
					.Build();

				// Deserialize YAML content to BlogMetadata object
				result = deserializer.Deserialize<BlogMetadata>(yamlContent);
			}

			return result;
		}
	}
}
