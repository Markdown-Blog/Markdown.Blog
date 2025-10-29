using System;
using System.Collections.Generic;

namespace Markdown.Blog.Application.DTOs
{
    /// <summary>
    /// 博客内容数据传输对象
    /// </summary>
    public class BlogContentDto
    {
        /// <summary>
        /// 博客元数据
        /// </summary>
        public BlogMetadataDto Metadata { get; set; } = new BlogMetadataDto();

        /// <summary>
        /// 原始Markdown内容
        /// </summary>
        public string MarkdownContent { get; set; } = string.Empty;

        /// <summary>
        /// 渲染后的HTML内容
        /// </summary>
        public string HtmlContent { get; set; } = string.Empty;

        /// <summary>
        /// 目录结构
        /// </summary>
        public List<TableOfContentItem> TableOfContents { get; set; } = new List<TableOfContentItem>();

        /// <summary>
        /// 博客中的图片列表
        /// </summary>
        public List<string> Images { get; set; } = new List<string>();

        /// <summary>
        /// 博客中的链接列表
        /// </summary>
        public List<string> Links { get; set; } = new List<string>();

        /// <summary>
        /// 相关博客推荐
        /// </summary>
        public List<BlogMetadataDto> RelatedBlogs { get; set; } = new List<BlogMetadataDto>();
    }

    /// <summary>
    /// 目录项
    /// </summary>
    public class TableOfContentItem
    {
        /// <summary>
        /// 标题文本
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 锚点链接
        /// </summary>
        public string Anchor { get; set; } = string.Empty;

        /// <summary>
        /// 标题级别 (1-6)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 子目录项
        /// </summary>
        public List<TableOfContentItem> Children { get; set; } = new List<TableOfContentItem>();
    }
}