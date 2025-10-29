using System;
using System.Collections.Generic;

namespace Markdown.Blog.Application.DTOs
{
    /// <summary>
    /// 博客元数据数据传输对象
    /// </summary>
    public class BlogMetadataDto
    {
        /// <summary>
        /// 博客文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 博客标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 博客描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PublishedDate { get; set; }

        /// <summary>
        /// 博客标签
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// 博客分类
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// 是否为草稿
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// 阅读时间（分钟）
        /// </summary>
        public int ReadingTimeMinutes { get; set; }

        /// <summary>
        /// 字数统计
        /// </summary>
        public int WordCount { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// 博客摘要
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 封面图片URL
        /// </summary>
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// SEO关键词
        /// </summary>
        public List<string> SeoKeywords { get; set; } = new List<string>();

        /// <summary>
        /// 博客优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsPinned { get; set; }
    }
}