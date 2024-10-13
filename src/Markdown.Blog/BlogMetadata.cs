using System;
using System.Collections.Generic;

namespace Markdown.Blog
{
	/// <summary>
	/// Represents metadata for a blog post.
	/// </summary>
	public class BlogMetadata
	{
		/// <summary>
		/// Gets or sets the file path of the blog post in the BlogMetadata object.
		/// </summary>
		public string FilePath { get; set; }

		/// <summary>
		/// Gets or sets the title of the blog post in the BlogMetadata object.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the description of the blog post in the BlogMetadata object.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the publication date of the blog post in the BlogMetadata object.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the list of tags associated with the blog post in the BlogMetadata object.
		/// </summary>
		public List<string> Tags { get; set; }

		/// <summary>
		/// Gets or sets the cover image path for the blog post in the BlogMetadata object.
		/// </summary>
		public string CoverImage { get; set; }

		/// <summary>
		/// Gets or sets the hierarchical structure of the blog post in the BlogMetadata object.
		/// </summary>
		public BlogHierarchy Hierarchy { get; set; }
	}
}
