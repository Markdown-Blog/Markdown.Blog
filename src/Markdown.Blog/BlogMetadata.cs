using System;
using System.Collections.Generic;

namespace Markdown.Blog
{
	public class BlogMetadata
	{
		public string FilePath { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set; }
		public List<string> Tags { get; set; }
		public string CoverImage { get; set; }
		public BlogHierarchy Hierarchy { get; set; }
	}
}
