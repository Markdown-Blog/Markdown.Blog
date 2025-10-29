using System;
using System.Collections.Generic;

namespace Markdown.Blog.Shared.Models
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
        /// Gets or sets the cover image paths for the blog post in the BlogMetadata object.
        /// </summary>
        public List<string> CoverImages { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the hierarchical structure of the blog post in the BlogMetadata object.
        /// </summary>
        public BlogHierarchy Hierarchy { get; set; }

        /// <summary>
        /// Gets or sets the path segments that represent the hierarchical location of the blog post.
        /// For example: ["Documentation", "OutSystems 11", "Building apps", "Data management", "Data operations", "SQL Queries"]
        /// If null, path-based access mode is disabled.
        /// </summary>
        public List<string>? PathSegments { get; set; }

        /// <summary>
        /// Gets or sets whether this blog post is a draft.
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// Determines if the blog post is displayed in path mode.
        /// </summary>
        /// <returns>true if the blog post is displayed in path mode; otherwise, false.</returns>
        public bool IsPathMode => PathSegments != null;
    }
}
