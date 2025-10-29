using System;
using System.Collections.Generic;

namespace Markdown.Blog.Application.DTOs
{
    /// <summary>
    /// 博客统计信息
    /// </summary>
    public class BlogStatisticsDto
    {
        /// <summary>
        /// 总博客数量
        /// </summary>
        public int TotalBlogs { get; set; }

        /// <summary>
        /// 已发布博客数量
        /// </summary>
        public int PublishedBlogs { get; set; }

        /// <summary>
        /// 草稿数量
        /// </summary>
        public int DraftBlogs { get; set; }

        /// <summary>
        /// 总字数
        /// </summary>
        public int TotalWordCount { get; set; }

        /// <summary>
        /// 平均字数
        /// </summary>
        public int AverageWordCount { get; set; }

        /// <summary>
        /// 总阅读时间（分钟）
        /// </summary>
        public int TotalReadingTimeMinutes { get; set; }

        /// <summary>
        /// 平均阅读时间（分钟）
        /// </summary>
        public int AverageReadingTimeMinutes { get; set; }

        /// <summary>
        /// 标签统计
        /// </summary>
        public List<TagStatistic> TagStatistics { get; set; } = new List<TagStatistic>();

        /// <summary>
        /// 分类统计
        /// </summary>
        public List<CategoryStatistic> CategoryStatistics { get; set; } = new List<CategoryStatistic>();

        /// <summary>
        /// 作者统计
        /// </summary>
        public List<AuthorStatistic> AuthorStatistics { get; set; } = new List<AuthorStatistic>();

        /// <summary>
        /// 月度发布统计
        /// </summary>
        public List<MonthlyStatistic> MonthlyStatistics { get; set; } = new List<MonthlyStatistic>();

        /// <summary>
        /// 最新博客
        /// </summary>
        public BlogMetadataDto? LatestBlog { get; set; }

        /// <summary>
        /// 最长博客
        /// </summary>
        public BlogMetadataDto? LongestBlog { get; set; }

        /// <summary>
        /// 最短博客
        /// </summary>
        public BlogMetadataDto? ShortestBlog { get; set; }

        /// <summary>
        /// 统计生成时间
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// 标签统计
    /// </summary>
    public class TagStatistic
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Tag { get; set; } = string.Empty;

        /// <summary>
        /// 使用次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 使用百分比
        /// </summary>
        public double Percentage { get; set; }
    }

    /// <summary>
    /// 分类统计
    /// </summary>
    public class CategoryStatistic
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 博客数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 占比百分比
        /// </summary>
        public double Percentage { get; set; }
    }

    /// <summary>
    /// 作者统计
    /// </summary>
    public class AuthorStatistic
    {
        /// <summary>
        /// 作者名称
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// 博客数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 总字数
        /// </summary>
        public int TotalWordCount { get; set; }

        /// <summary>
        /// 平均字数
        /// </summary>
        public int AverageWordCount { get; set; }
    }

    /// <summary>
    /// 月度统计
    /// </summary>
    public class MonthlyStatistic
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 博客数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 月份显示名称
        /// </summary>
        public string MonthName => new DateTime(Year, Month, 1).ToString("yyyy-MM");
    }
}