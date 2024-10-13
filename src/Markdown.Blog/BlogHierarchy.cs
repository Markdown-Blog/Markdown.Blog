using System;
using System.Collections.Generic;

namespace Markdown.Blog
{
	/// <summary>
	/// Represents the hierarchical structure of a blog post.
	/// </summary>
	public class BlogHierarchy
	{
		/// <summary>
		/// Gets or sets the division of the blog post in the BlogHierarchy object.
		/// </summary>
		public string Division { get; set; }

		/// <summary>
		/// Gets or sets the category of the blog post in the BlogHierarchy object.
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets or sets the subcategory of the blog post in the BlogHierarchy object.
		/// </summary>
		public string SubCategory { get; set; }
	}
}
