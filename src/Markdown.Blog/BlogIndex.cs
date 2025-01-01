using System;
using System.Collections.Generic;

namespace Markdown.Blog
{
	/// <summary>
	/// Represents an index containing metadata for multiple blog entries.
	/// This index maintains version control and tracks the latest changes across all blog posts.
	/// </summary>
	public class BlogIndex
	{
		/// <summary>
		/// Gets or sets the version number of this index.
		/// Increments when any blog post is modified, serving as a version identifier.
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Gets or sets the timestamp when this index was generated in UTC.
		/// Typically reflects the most recent modification time of any blog post in the system.
		/// </summary>
		public DateTime DateTime { get; set; }
		/// <summary>
		/// Gets or sets the collection of blog entry metadata.
		/// Contains information such as titles, descriptions, and other metadata for all blog entries in this index.
		/// </summary>
		public List<BlogMetadata> BlogMetadataList { get; set; }
	}
}
